using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMProxy.Packets
{
    public class ExplosionPacket : Packet
    {
        public Vector3 Position;
        public float Radius;
        public int Records;
        public byte[] RecordData; // TODO: Improve
        [FriendlyName("[unknown]")]
        public float Unknown1;
        [FriendlyName("[unknown]")]
        public float Unknown2;
        [FriendlyName("[unknown]")]
        public float Unknown3;

        public override byte PacketId
        {
            get { return 0x3C; }
        }

        public override int TryReadPacket(byte[] buffer, int length)
        {
            int offset = 1;
            double x, y, z;
            if (!DataUtility.TryReadDouble(buffer, ref offset, length, out x))
                return -1;
            if (!DataUtility.TryReadDouble(buffer, ref offset, length, out y))
                return -1;
            if (!DataUtility.TryReadDouble(buffer, ref offset, length, out z))
                return -1;
            if (!DataUtility.TryReadFloat(buffer, ref offset, length, out Radius))
                return -1;
            if (!DataUtility.TryReadInt32(buffer, ref offset, length, out Records))
                return -1;
            if (!DataUtility.TryReadArray(buffer, ref offset, length, out RecordData, (short)(Records * 3)))
                return -1;
            if (!DataUtility.TryReadFloat(buffer, ref offset, length, out Unknown1))
                return -1;
            if (!DataUtility.TryReadFloat(buffer, ref offset, length, out Unknown2))
                return -1;
            if (!DataUtility.TryReadFloat(buffer, ref offset, length, out Unknown3))
                return -1;
            return offset;
        }
    }
}
