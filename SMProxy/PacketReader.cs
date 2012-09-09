using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMProxy.Packets;

namespace SMProxy
{
    // Based on the PacketReader from Craft.Net
    public class PacketReader
    {
        #region Packet type array

        public static readonly Type[] PacketTypes =
            {
                typeof(KeepAlivePacket), // 0x0
                typeof(LoginRequestPacket), // 0x1
                typeof(HandshakePacket), // 0x2
                typeof(ChatMessagePacket), // 0x3
                typeof(TimeUpdatePacket), // 0x4
                typeof(EntityEquipmentPacket), // 0x5
                typeof(SpawnPositionPacket), // 0x6
                typeof(UseEntityPacket), // 0x7
                typeof(UpdateHealthPacket), // 0x8
                typeof(RespawnPacket), // 0x9
                typeof(PlayerPacket), // 0xa
                typeof(PlayerPositionPacket), // 0xb
                typeof(PlayerLookPacket), // 0xc
                typeof(PlayerPositionAndLookPacket), // 0xd
                typeof(PlayerDiggingPacket), // 0xe
                typeof(PlayerBlockPlacementPacket), // 0xf
                typeof(HeldItemChangePacket), // 0x10
                typeof(UseBedPacket), // 0x11
                typeof(AnimationPacket), // 0x12
                typeof(EntityActionPacket), // 0x13
                typeof(SpawnNamedEntityPacket), // 0x14
                typeof(SpawnDroppedItemPacket), // 0x15
                typeof(CollectItemPacket), // 0x16
                typeof(SpawnObjectPacket), // 0x17
                typeof(SpawnMobPacket), // 0x18
                typeof(SpawnPaintingPacket), // 0x19
                typeof(SpawnExperienceOrbPacket), // 0x1a
                null, // 0x1b
                typeof(EntityVelocityPacket), // 0x1c
                typeof(DestroyEntityPacket), // 0x1d
                typeof(EntityPacket), // 0x1e
                typeof(EntityRelativeMovePacket), // 0x1f
                typeof(EntityLookPacket), // 0x20
                typeof(EntityLookAndRelativeMovePacket), // 0x21
                typeof(EntityTeleportPacket), // 0x22
                typeof(EntityHeadLookPacket), // 0x23
                null, // 0x24
                null, // 0x25
                typeof(EntityStatusPacket), // 0x26
                typeof(AttachEntityPacket), // 0x27
                typeof(EntityMetadataPacket), // 0x28
                typeof(EntityEffectPacket), // 0x29
                typeof(RemoveEntityEffectPacket), // 0x2a
                typeof(SetExperiencePacket), // 0x2b
                null, // 0x2c
                null, // 0x2d
                null, // 0x2e
                null, // 0x2f
                null, // 0x30
                null, // 0x31
                null, // 0x32
                typeof(ChunkDataPacket), // 0x33
                typeof(MultiBlockChangePacket), // 0x34
                typeof(BlockChangePacket), // 0x35
                typeof(BlockActionPacket), // 0x36
                typeof(BlockBreakAnimationPacket), // 0x37
                typeof(MapChunkBulkPacket), // 0x38
                null, // 0x39
                null, // 0x3a
                null, // 0x3b
                typeof(ExplosionPacket), // 0x3c
                typeof(SoundOrParticleEffectPacket), // 0x3d
                typeof(NamedSoundEffectPacket), // 0x3e
                null, // 0x3f
                null, // 0x40
                null, // 0x41
                null, // 0x42
                null, // 0x43
                null, // 0x44
                null, // 0x45
                typeof(ChangeGameStatePacket), // 0x46
                typeof(LightningPacket), // 0x47
                null, // 0x48
                null, // 0x49
                null, // 0x4a
                null, // 0x4b
                null, // 0x4c
                null, // 0x4d
                null, // 0x4e
                null, // 0x4f
                null, // 0x50
                null, // 0x51
                null, // 0x52
                null, // 0x53
                null, // 0x54
                null, // 0x55
                null, // 0x56
                null, // 0x57
                null, // 0x58
                null, // 0x59
                null, // 0x5a
                null, // 0x5b
                null, // 0x5c
                null, // 0x5d
                null, // 0x5e
                null, // 0x5f
                null, // 0x60
                null, // 0x61
                null, // 0x62
                null, // 0x63
                typeof(OpenWindowPacket), // 0x64
                typeof(CloseWindowPacket), // 0x65
                typeof(ClickWindowPacket), // 0x66
                typeof(SetSlotPacket), // 0x67
                typeof(SetWindowItemsPacket), // 0x68
                typeof(UpdateWindowPropertyPacket), // 0x69
                typeof(ConfirmTransactionPacket), // 0x6a
                typeof(CreativeInventoryActionPacket), // 0x6b
                typeof(EnchantItemPacket), // 0x6c
                null, // 0x6d
                null, // 0x6e
                null, // 0x6f
                null, // 0x70
                null, // 0x71
                null, // 0x72
                null, // 0x73
                null, // 0x74
                null, // 0x75
                null, // 0x76
                null, // 0x77
                null, // 0x78
                null, // 0x79
                null, // 0x7a
                null, // 0x7b
                null, // 0x7c
                null, // 0x7d
                null, // 0x7e
                null, // 0x7f
                null, // 0x80
                null, // 0x81
                typeof(UpdateSignPacket), // 0x82
                typeof(ItemDataPacket), // 0x83
                typeof(UpdateTileEntityPacket), // 0x84
                null, // 0x85
                null, // 0x86
                null, // 0x87
                null, // 0x88
                null, // 0x89
                null, // 0x8a
                null, // 0x8b
                null, // 0x8c
                null, // 0x8d
                null, // 0x8e
                null, // 0x8f
                null, // 0x90
                null, // 0x91
                null, // 0x92
                null, // 0x93
                null, // 0x94
                null, // 0x95
                null, // 0x96
                null, // 0x97
                null, // 0x98
                null, // 0x99
                null, // 0x9a
                null, // 0x9b
                null, // 0x9c
                null, // 0x9d
                null, // 0x9e
                null, // 0x9f
                null, // 0xa0
                null, // 0xa1
                null, // 0xa2
                null, // 0xa3
                null, // 0xa4
                null, // 0xa5
                null, // 0xa6
                null, // 0xa7
                null, // 0xa8
                null, // 0xa9
                null, // 0xaa
                null, // 0xab
                null, // 0xac
                null, // 0xad
                null, // 0xae
                null, // 0xaf
                null, // 0xb0
                null, // 0xb1
                null, // 0xb2
                null, // 0xb3
                null, // 0xb4
                null, // 0xb5
                null, // 0xb6
                null, // 0xb7
                null, // 0xb8
                null, // 0xb9
                null, // 0xba
                null, // 0xbb
                null, // 0xbc
                null, // 0xbd
                null, // 0xbe
                null, // 0xbf
                null, // 0xc0
                null, // 0xc1
                null, // 0xc2
                null, // 0xc3
                null, // 0xc4
                null, // 0xc5
                null, // 0xc6
                null, // 0xc7
                typeof(IncrementStatisticPacket), // 0xc8
                typeof(PlayerListItemPacket), // 0xc9
                typeof(PlayerAbilitiesPacket), // 0xca
                typeof(TabCompletePacket), // 0xcb
                typeof(LocaleAndViewDistancePacket), // 0xcc
                typeof(ClientStatusPacket), // 0xcd
                null, // 0xce
                null, // 0xcf
                null, // 0xd0
                null, // 0xd1
                null, // 0xd2
                null, // 0xd3
                null, // 0xd4
                null, // 0xd5
                null, // 0xd6
                null, // 0xd7
                null, // 0xd8
                null, // 0xd9
                null, // 0xda
                null, // 0xdb
                null, // 0xdc
                null, // 0xdd
                null, // 0xde
                null, // 0xdf
                null, // 0xe0
                null, // 0xe1
                null, // 0xe2
                null, // 0xe3
                null, // 0xe4
                null, // 0xe5
                null, // 0xe6
                null, // 0xe7
                null, // 0xe8
                null, // 0xe9
                null, // 0xea
                null, // 0xeb
                null, // 0xec
                null, // 0xed
                null, // 0xee
                null, // 0xef
                null, // 0xf0
                null, // 0xf1
                null, // 0xf2
                null, // 0xf3
                null, // 0xf4
                null, // 0xf5
                null, // 0xf6
                null, // 0xf7
                null, // 0xf8
                null, // 0xf9
                typeof(PluginMessagePacket), // 0xfa
                null, // 0xfb
                typeof(EncryptionKeyResponsePacket), // 0xfc
                typeof(EncryptionKeyRequestPacket), // 0xfd
                typeof(ServerListPingPacket), // 0xfe
                typeof(DisconnectPacket) // 0xff
            };

        #endregion

        /// <summary>
        /// Attempts to parse all packets in the given client and update
        /// its buffer.
        /// </summary>
        public static IEnumerable<Packet> TryReadPackets(Proxy proxy, int length, PacketContext packetContext)
        {
            var results = new List<Packet>();
            // buffer is a shrinking buffer of bytes to be interpreted as packets.
            byte[] buffer;
            // Get a buffer to parse that is the length of the recieved data
            // Fetch from the network buffer the last incomplete packet (proxy.*Index) plus the length of the data
            // read in the latest chunk of data.
            if (packetContext == PacketContext.ClientToServer)
                buffer = proxy.LocalBuffer.Take(proxy.LocalIndex + length).ToArray();
            else
                buffer = proxy.RemoteBuffer.Take(proxy.RemoteIndex + length).ToArray();
            // Decrypt the buffer if needed
            // If encryption is enabled, the previous packet will be decrypted, and the appended data
            // will not be. We must only decrypt the latest data, so we send just that data into the
            // decrypter and copy it back into the buffer.
            if (packetContext == PacketContext.ClientToServer && proxy.LocalEncryptionEnabled)
                Array.Copy(proxy.LocalDecrypter.ProcessBytes(buffer, proxy.LocalIndex, length), 0, buffer, proxy.LocalIndex, length);
            else if (packetContext == PacketContext.ServerToClient && proxy.RemoteEncryptionEnabled)
                Array.Copy(proxy.RemoteDecrypter.ProcessBytes(buffer, proxy.RemoteIndex, length), 0, buffer, proxy.RemoteIndex, length);

            length += packetContext == PacketContext.ClientToServer ? proxy.LocalIndex : proxy.RemoteIndex; // Update the length to include the incomplete packet
            while (buffer.Length > 0) // As long as the buffer is not empty
            {
                Type packetType = PacketTypes[buffer[0]]; // Get the correct type to parse this packet based on the first byte of the data
                if (packetType == null)
                {
                    // If there is no packet handler, it's an invalid packet.
                    results.Add(new InvalidPacket(buffer));
                    return results;
                }
                var workingPacket = (Packet)Activator.CreateInstance(packetType);
                workingPacket.PacketContext = packetContext;
                // Attempt to read the packet, returns the length of the read packet or -1
                int workingLength = workingPacket.TryReadPacket(buffer, buffer.Length);
                if (workingLength == -1) // Incomplete packet
                {
                    // Copy the incomplete packet into the recieve buffer and recieve more data
                    if (packetContext == PacketContext.ClientToServer)
                    {
                        // Copy the current packet buffer into the start of network buffer (decrypted)
                        Array.Copy(buffer, proxy.LocalBuffer, buffer.Length);
                        // Set the starting index to the end of that
                        proxy.LocalIndex = buffer.Length;
                    }
                    else
                    {
                        Array.Copy(buffer, proxy.RemoteBuffer, buffer.Length);
                        proxy.RemoteIndex = buffer.Length;
                    }
                    return results;
                }
                // Copy the payload from the buffer into the working packet
                // so it may be re-sent and logged.
                workingPacket.Payload = new byte[workingLength];
                Array.Copy(buffer, workingPacket.Payload, workingLength);
                // Add this packet to the results
                results.Add(workingPacket);
                // Shift the buffer over and remove the packet just parsed
                buffer = buffer.Skip(workingLength).ToArray();
            }

            return results;
        }
    }
}
