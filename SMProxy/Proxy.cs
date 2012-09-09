using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Net.Sockets;
using Org.BouncyCastle.Crypto;
using SMProxy.Packets;
using System.Diagnostics;

namespace SMProxy
{
    public class Proxy
    {
        public const int ProtocolVersion = 39;

        internal static RSAParameters ServerKey;
        internal static RSACryptoServiceProvider CryptoServiceProvider;

        static Proxy()
        {
            // Generate a global server key
            CryptoServiceProvider = new RSACryptoServiceProvider(1024);
            ServerKey = CryptoServiceProvider.ExportParameters(true);
        }

        public Proxy(ILogProvider logProvider, ProxySettings settings)
        {
            LogProvider = logProvider;
            Settings = settings;
            LocalReader = new PacketReader(PacketContext.ClientToServer);
            RemoteReader = new PacketReader(PacketContext.ServerToClient);
        }

        public ProxySettings Settings;
        public Socket LocalSocket, RemoteSocket;
        public bool Connected { get; private set; }
        public ILogProvider LogProvider { get; set; }
        public PacketReader RemoteReader, LocalReader;
        public BufferedBlockCipher LocalEncrypter, RemoteEncrypter;
        internal byte[] LocalSharedKey, RemoteSharedKey;
        internal byte[] RemoteEncryptionResponse;

        /// <summary>
        /// Starts a proxy connection between the two sockets.
        /// </summary>
        /// <param name="localSocket">The client connection.</param>
        /// <param name="remoteSocket">The server connection.</param>
        public void Start(Socket localSocket, Socket remoteSocket)
        {
            LocalSocket = localSocket;
            RemoteSocket = remoteSocket;
            Connected = true;
            LocalSocket.BeginReceive(LocalReader.Buffer, 0, LocalReader.Buffer.Length, SocketFlags.None, HandleLocalPackets, null);
            RemoteSocket.BeginReceive(RemoteReader.Buffer, 0, RemoteReader.Buffer.Length, SocketFlags.None, HandleRemotePackets, null);
        }

        internal void HandleLocalPackets(IAsyncResult result)
        {
            SocketError error;
            int length = LocalSocket.EndReceive(result, out error);
            // Check for errors and disconnect if needed
            if (error != SocketError.Success || !LocalSocket.Connected || length == 0)
            {
                if (error != SocketError.Success)
                    LogProvider.Log("Local client " + LocalSocket.RemoteEndPoint + " disconnected: " + error);
                else
                    LogProvider.Log("Local client " + LocalSocket.RemoteEndPoint + " disconnected.");
                if (RemoteSocket.Connected)
                    RemoteSocket.BeginDisconnect(false, null, null);
                Connected = false;
                if (Settings.SingleSession)
                    Process.GetCurrentProcess().Kill();
                return;
            }

            // Attempt to parse recieved data
            try
            {
                var packets = LocalReader.TryReadPackets(length);
                foreach (var packet in packets)
                {
                    // If an invalid packet was sent...
                    if (packet is InvalidPacket)
                    {
                        // ...send it straight back without parsing it, then switch to raw mode.
                        // TODO: Refactor?
                        if (RemoteReader.EncryptionEnabled)
                            RemoteSocket.BeginSend(RemoteEncrypter.ProcessBytes(packet.Payload), 0, packet.Payload.Length, SocketFlags.None, null, null);
                        else
                            RemoteSocket.BeginSend(packet.Payload, 0, packet.Payload.Length, SocketFlags.None, null, null);
                        LogProvider.Log("Client sent unrecognized packet: 0x" + packet.PacketId.ToString("X2") + ". Switching client to generic TCP proxy");
                        LogProvider.Raw(packet.Payload, this, PacketContext.ClientToServer);
                        LocalSocket.BeginReceive(LocalReader.Buffer, 0, LocalReader.Buffer.Length, SocketFlags.None, HandleLocalRaw, null);
                        return;
                    }
                    packet.HandlePacket(this);
                    if (!packet.OverrideSendPacket())
                    {
                        if (RemoteReader.EncryptionEnabled)
                            RemoteSocket.BeginSend(RemoteEncrypter.ProcessBytes(packet.Payload), 0, packet.Payload.Length, SocketFlags.None, null, null);
                        else
                            RemoteSocket.BeginSend(packet.Payload, 0, packet.Payload.Length, SocketFlags.None, null, null);
                    }
                    LogProvider.Log(packet, this);
                }
                LocalSocket.BeginReceive(LocalReader.Buffer, LocalReader.Index, LocalReader.Buffer.Length - LocalReader.Index, SocketFlags.None, HandleLocalPackets, null);
            }
            catch (Exception e)
            {
                LogProvider.Log("Client exception: \"" + e.Message + "\" Switching client to generic TCP proxy");
                LocalSocket.BeginReceive(LocalReader.Buffer, 0, LocalReader.Buffer.Length, SocketFlags.None, HandleLocalRaw, null);
            }
        }

        private void HandleLocalRaw(IAsyncResult result)
        {
            SocketError error;
            int length = LocalSocket.EndReceive(result, out error);
            if (error != SocketError.Success || !LocalSocket.Connected || length == 0)
            {
                string log;
                if (error != SocketError.Success)
                    log = "Local client " + LocalSocket.RemoteEndPoint + " disconnected: " + error;
                else
                    log = "Local client " + LocalSocket.RemoteEndPoint + " disconnected.";
                LogProvider.Log(log);
                Console.WriteLine(log);
                if (RemoteSocket.Connected)
                    RemoteSocket.BeginDisconnect(false, null, null);
                Connected = false;
                if (Settings.SingleSession)
                    Process.GetCurrentProcess().Kill();
                return;
            }

            // TODO
        }

        internal void HandleRemotePackets(IAsyncResult result)
        {
            SocketError error;
            int length = RemoteSocket.EndReceive(result, out error);
            if (error != SocketError.Success || !RemoteSocket.Connected || length == 0)
            {
                string log;
                if (error != SocketError.Success)
                    log = "Remote server " + RemoteSocket.RemoteEndPoint + " disconnected: " + error;
                else
                    log = "Remote server " + RemoteSocket.RemoteEndPoint + " disconnected.";
                LogProvider.Log(log);
                Console.WriteLine(log);
                if (LocalSocket.Connected)
                    LocalSocket.BeginDisconnect(false, null, null);
                Connected = false;
                if (Settings.SingleSession)
                    Process.GetCurrentProcess().Kill();
                return;
            }

            // Attempt to parse recieved data
            try
            {
                var packets = RemoteReader.TryReadPackets(length);
                foreach (var packet in packets)
                {
                    // If an invalid packet was sent...
                    if (packet is InvalidPacket)
                    {
                        // ...send it straight back without parsing it, then switch to raw mode.
                        // TODO: Refactor?
                        if (LocalReader.EncryptionEnabled)
                            LocalSocket.BeginSend(LocalEncrypter.ProcessBytes(packet.Payload), 0, packet.Payload.Length, SocketFlags.None, null, null);
                        else
                            LocalSocket.BeginSend(packet.Payload, 0, packet.Payload.Length, SocketFlags.None, null, null);
                        LogProvider.Log("Server sent unrecognized packet: 0x" + packet.PacketId.ToString("X2") + ". Switching server to generic TCP proxy");
                        LogProvider.Raw(packet.Payload, this, PacketContext.ServerToClient);
                        RemoteSocket.BeginReceive(RemoteReader.Buffer, 0, RemoteReader.Buffer.Length, SocketFlags.None, HandleRemoteRaw, null);
                        return;
                    }
                    packet.HandlePacket(this);
                    if (!packet.OverrideSendPacket())
                    {
                        if (LocalReader.EncryptionEnabled)
                            LocalSocket.BeginSend(LocalEncrypter.ProcessBytes(packet.Payload), 0, packet.Payload.Length, SocketFlags.None, null, null);
                        else
                            LocalSocket.BeginSend(packet.Payload, 0, packet.Payload.Length, SocketFlags.None, null, null);
                    }
                    LogProvider.Log(packet, this);
                }
                RemoteSocket.BeginReceive(RemoteReader.Buffer, RemoteReader.Index, RemoteReader.Buffer.Length - RemoteReader.Index, SocketFlags.None, HandleRemotePackets, null);
            }
            catch (Exception e)
            {
                LogProvider.Log("Server exception: \"" + e.Message + "\" Switching server to generic TCP proxy");
                RemoteSocket.BeginReceive(RemoteReader.Buffer, 0, RemoteReader.Buffer.Length, SocketFlags.None, HandleRemoteRaw, null);
            }
        }

        private void HandleRemoteRaw(IAsyncResult result)
        {
            SocketError error;
            int length = RemoteSocket.EndReceive(result, out error);
            if (error != SocketError.Success || !RemoteSocket.Connected || length == 0)
            {
                if (error != SocketError.Success)
                    LogProvider.Log("Remote server " + RemoteSocket.RemoteEndPoint + " disconnected: " + error);
                else
                    LogProvider.Log("Remote server " + RemoteSocket.RemoteEndPoint + " disconnected.");
                if (LocalSocket.Connected)
                    LocalSocket.BeginDisconnect(false, null, null);
                Connected = false;
                if (Settings.SingleSession)
                    Process.GetCurrentProcess().Kill();
                return;
            }

            // TODO
        }
    }
}
