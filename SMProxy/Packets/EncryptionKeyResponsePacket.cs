﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;

namespace SMProxy.Packets
{
    public class EncryptionKeyResponsePacket : Packet
    {
        [FriendlyName("Shared Secret (decrypted)")]
        public byte[] SharedSecret;
        [FriendlyName("Verify Token (decrypted)")]
        public byte[] VerifyToken;

        public override byte PacketId
        {
            get { return 0xFC; }
        }

        public override int TryReadPacket(byte[] buffer, int length)
        {
            short secretLength = 0, verifyLength = 0;
            int offset = 1;
            if (!DataUtility.TryReadInt16(buffer, ref offset, length, out secretLength))
                return -1;
            if (!DataUtility.TryReadArray(buffer, ref offset, length, out SharedSecret, secretLength))
                return -1;
            if (!DataUtility.TryReadInt16(buffer, ref offset, length, out verifyLength))
                return -1;
            if (!DataUtility.TryReadArray(buffer, ref offset, length, out VerifyToken, verifyLength))
                return -1;
            return offset;
        }

        public override void HandlePacket(Proxy proxy)
        {
            if (PacketContext == PacketContext.ClientToServer)
            {
                proxy.LocalSharedKey = Proxy.CryptoServiceProvider.Decrypt(SharedSecret, false);
                SharedSecret = proxy.LocalSharedKey;
                VerifyToken = Proxy.CryptoServiceProvider.Decrypt(VerifyToken, false);

                // Initialize local encryption
                proxy.LocalEncrypter = new BufferedBlockCipher(new CfbBlockCipher(new AesFastEngine(), 8));
                proxy.LocalEncrypter.Init(true,
                                      new ParametersWithIV(new KeyParameter(proxy.LocalSharedKey), proxy.LocalSharedKey, 0, 16));

                proxy.LocalReader.Decrypter = new BufferedBlockCipher(new CfbBlockCipher(new AesFastEngine(), 8));
                proxy.LocalReader.Decrypter.Init(false,
                                      new ParametersWithIV(new KeyParameter(proxy.LocalSharedKey), proxy.LocalSharedKey, 0, 16));

                // Send server mock response
                proxy.RemoteSocket.BeginSend(proxy.RemoteEncryptionResponse, 0, proxy.RemoteEncryptionResponse.Length,
                                             SocketFlags.None, null, null);
            }
            else
            {
                proxy.RemoteEncrypter = new BufferedBlockCipher(new CfbBlockCipher(new AesFastEngine(), 8));
                proxy.RemoteEncrypter.Init(true,
                                      new ParametersWithIV(new KeyParameter(proxy.RemoteSharedKey), proxy.RemoteSharedKey, 0, 16));

                proxy.RemoteReader.Decrypter = new BufferedBlockCipher(new CfbBlockCipher(new AesFastEngine(), 8));
                proxy.RemoteReader.Decrypter.Init(false,
                                      new ParametersWithIV(new KeyParameter(proxy.RemoteSharedKey), proxy.RemoteSharedKey, 0, 16));

                var response = new byte[] { 0xFC, 0x00, 0x00, 0x00, 0x00 };
                proxy.LocalSocket.BeginSend(response, 0, response.Length, SocketFlags.None, null, null);
                proxy.RemoteReader.EncryptionEnabled = proxy.LocalReader.EncryptionEnabled = true;
                proxy.LogProvider.Log("Encryption enabled.");
            }
        }

        public override bool OverrideSendPacket()
        {
            return true;
        }
    }
}
