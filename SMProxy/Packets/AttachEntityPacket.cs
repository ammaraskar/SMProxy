﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMProxy.Packets
{
    public class AttachEntityPacket : Packet
    {
        public int EntityId;
        public int VehicleId;

        public override byte PacketId
        {
            get { return 0x27; }
        }

        public override int TryReadPacket(byte[] buffer, int length)
        {
            int offset = 1;
            if (!DataUtility.TryReadInt32(buffer, ref offset, length, out EntityId))
                return -1;
            if (!DataUtility.TryReadInt32(buffer, ref offset, length, out VehicleId))
                return -1;
            return offset;
        }
    }
}
