﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMProxy.Packets
{
    public class EntityTeleportPacket : Packet
    {
        public int EntityId;
        public Vector3 Position;
        public float Yaw;
        public float Pitch;

        public override byte PacketId
        {
            get { return 0x22; }
        }

        public override int TryReadPacket(byte[] buffer, int length)
        {
            int offset = 1;
            double x, y, z;
            if (!DataUtility.TryReadInt32(buffer, ref offset, length, out EntityId))
                return -1;
            if (!DataUtility.TryReadAbsoluteInteger(buffer, ref offset, length, out x))
                return -1;
            if (!DataUtility.TryReadAbsoluteInteger(buffer, ref offset, length, out y))
                return -1;
            if (!DataUtility.TryReadAbsoluteInteger(buffer, ref offset, length, out z))
                return -1;
            if (!DataUtility.TryReadPackedByte(buffer, ref offset, length, out Yaw))
                return -1;
            if (!DataUtility.TryReadPackedByte(buffer, ref offset, length, out Pitch))
                return -1;
            Position = new Vector3(x, y, z);
            return offset;
        }
    }
}
