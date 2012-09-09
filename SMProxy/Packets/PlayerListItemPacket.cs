using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMProxy.Packets
{
    public class PlayerListItemPacket : Packet
    {
        public string PlayerName;
        public bool Online;
        public short Ping;

        public override byte PacketId
        {
            get { return 0xC9; }
        }

        public override int TryReadPacket(byte[] buffer, int length)
        {
            int offset = 1;
            if (!DataUtility.TryReadString(buffer, ref offset, length, out PlayerName))
                return -1;
            if (!DataUtility.TryReadBoolean(buffer, ref offset, length, out Online))
                return -1;
            if (!DataUtility.TryReadInt16(buffer, ref offset, length, out Ping))
                return -1;
            return offset;
        }
    }
}
