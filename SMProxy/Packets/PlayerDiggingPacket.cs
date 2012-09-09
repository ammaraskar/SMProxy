using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMProxy.Packets
{
    public class PlayerDiggingPacket : Packet
    {
        public byte Status;
        public Vector3 Position;
        public byte Face;

        public override byte PacketId
        {
            get { return 0x0E; }
        }

        public override int TryReadPacket(byte[] buffer, int length)
        {
            int offset = 1;
            int x, z;
            byte y;
            if (!DataUtility.TryReadByte(buffer, ref offset, length, out Status))
                return -1;
            if (!DataUtility.TryReadInt32(buffer, ref offset, length, out x))
                return -1;
            if (!DataUtility.TryReadByte(buffer, ref offset, length, out y))
                return -1;
            if (!DataUtility.TryReadInt32(buffer, ref offset, length, out z))
                return -1;
            if (!DataUtility.TryReadByte(buffer, ref offset, length, out Face))
                return -1;
            Position = new Vector3(x, y, z);
            return offset;
        }
    }
}
