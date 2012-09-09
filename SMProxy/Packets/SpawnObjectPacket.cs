using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMProxy.Packets
{
    public class SpawnObjectPacket : Packet
    {
        public int EntityId;
        public byte Type;
        public Vector3 Position;
        public int ObjectData;
        public Vector3 Speed;

        public override byte PacketId
        {
            get { return 0x17; }
        }

        public override int TryReadPacket(byte[] buffer, int length)
        {
            int offset = 1;
            int x, y, z;
            short speedX, speedY, speedZ;
            if (!DataUtility.TryReadInt32(buffer, ref offset, length, out EntityId))
                return -1;
            if (!DataUtility.TryReadByte(buffer, ref offset, length, out Type))
                return -1;
            if (!DataUtility.TryReadInt32(buffer, ref offset, length, out x))
                return -1;
            if (!DataUtility.TryReadInt32(buffer, ref offset, length, out y))
                return -1;
            if (!DataUtility.TryReadInt32(buffer, ref offset, length, out z))
                return -1;
            if (!DataUtility.TryReadInt32(buffer, ref offset, length, out ObjectData))
                return -1;
            Position = new Vector3(x, y, z);
            if (ObjectData <= 0)
                return offset;
            if (!DataUtility.TryReadInt16(buffer, ref offset, length, out speedX))
                return -1;
            if (!DataUtility.TryReadInt16(buffer, ref offset, length, out speedY))
                return -1;
            if (!DataUtility.TryReadInt16(buffer, ref offset, length, out speedZ))
                return -1;
            Speed = new Vector3(speedX, speedY, speedZ);
            return offset;
        }
    }
}
