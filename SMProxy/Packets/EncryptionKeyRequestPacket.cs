﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Net;

namespace SMProxy.Packets
{
    public class EncryptionKeyRequestPacket : Packet
    {
        public string ServerId;
        public byte[] PublicKey;
        public byte[] VerifyToken;

        public override byte PacketId
        {
            get { return 0xFD; }
        }

        public override int TryReadPacket(byte[] buffer, int length)
        {
            int offset = 1;
            short serverKeyLength = 0, verifyTokenLength = 0;
            if (!DataUtility.TryReadString(buffer, ref offset, length, out ServerId))
                return -1;
            if (!DataUtility.TryReadInt16(buffer, ref offset, length, out serverKeyLength))
                return -1;
            if (!DataUtility.TryReadArray(buffer, ref offset, length, out PublicKey, serverKeyLength))
                return -1;
            if (!DataUtility.TryReadInt16(buffer, ref offset, length, out verifyTokenLength))
                return -1;
            if (!DataUtility.TryReadArray(buffer, ref offset, length, out VerifyToken, verifyTokenLength))
                return -1;
            return offset;
        }

        public override void HandlePacket(Proxy proxy)
        {
            // Interact with the remote server
            proxy.RemoteSharedKey = new byte[16];
            RandomNumberGenerator random = RandomNumberGenerator.Create();
            random.GetBytes(proxy.RemoteSharedKey);

            AsnKeyParser keyParser = new AsnKeyParser(PublicKey);
            var key = keyParser.ParseRSAPublicKey();
            
            var cryptoService = new RSACryptoServiceProvider();
            cryptoService.ImportParameters(key);
            byte[] encryptedSharedSecret = cryptoService.Encrypt(proxy.RemoteSharedKey, false);
            byte[] encryptedVerify = cryptoService.Encrypt(VerifyToken, false);

            // Construct an 0xFC packet to send the server
            proxy.RemoteEncryptionResponse = new byte[] { 0xFC }
                .Concat(DataUtility.CreateInt16((short)encryptedSharedSecret.Length))
                .Concat(encryptedSharedSecret)
                .Concat(DataUtility.CreateInt16((short)encryptedVerify.Length))
                .Concat(encryptedVerify).ToArray();

            if (ServerId != "-")
            {
                // Generate session hash
                byte[] hashData = Encoding.ASCII.GetBytes(ServerId)
                    .Concat(proxy.RemoteSharedKey)
                    .Concat(PublicKey).ToArray();
                var hash = Cryptography.JavaHexDigest(hashData);
                var webClient = new WebClient();
                string result = webClient.DownloadString("http://session.minecraft.net/game/joinserver.jsp?user=" +
                    Uri.EscapeUriString(proxy.Settings.Username) +
                    "&sessionId=" + Uri.EscapeUriString(proxy.Settings.UserSession) +
                    "&serverId=" + Uri.EscapeUriString(hash));
                if (result != "OK")
                    Console.WriteLine("Warning: Unable to login as user " + proxy.Settings.Username + ", expect mixed results.");
            }

            // Interact with the local client
            var verifyToken = new byte[4];
            var csp = new RNGCryptoServiceProvider();
            csp.GetBytes(verifyToken);

            AsnKeyBuilder.AsnMessage encodedKey = AsnKeyBuilder.PublicKeyToX509(Proxy.ServerKey);

            // Construct an 0xFD to send the client
            byte[] localPacket = new[] { PacketId }
                .Concat(DataUtility.CreateString("-"))
                .Concat(DataUtility.CreateInt16((short)encodedKey.GetBytes().Length))
                .Concat(encodedKey.GetBytes())
                .Concat(DataUtility.CreateInt16((short)verifyToken.Length))
                .Concat(verifyToken).ToArray();
            proxy.LocalSocket.BeginSend(localPacket, 0, localPacket.Length, SocketFlags.None, null, null);

            base.HandlePacket(proxy);
        }

        public override bool OverrideSendPacket()
        {
            return true;
        }
    }
}
