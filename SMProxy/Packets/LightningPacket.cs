﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMProxy.Packets
{
    public class LightningPacket : Packet
    {
        public int EntityId;
        [FriendlyName("[unknown]")]
        public bool Unknown;
        public Vector3 Position;

        public override byte PacketId
        {
            get { return 0x47; }
        }

        public override int TryReadPacket(byte[] buffer, int length)
        {
            int offset = 1;
            int x, y, z;
            if (!DataUtility.TryReadInt32(buffer, ref offset, length, out EntityId))
                return -1;
            if (!DataUtility.TryReadBoolean(buffer, ref offset, length, out Unknown))
                return -1;
            if (!DataUtility.TryReadInt32(buffer, ref offset, length, out x))
                return -1;
            if (!DataUtility.TryReadInt32(buffer, ref offset, length, out y))
                return -1;
            if (!DataUtility.TryReadInt32(buffer, ref offset, length, out z))
                return -1;
            Position = new Vector3(x, y, z);
            return offset;
        }
    }
}
