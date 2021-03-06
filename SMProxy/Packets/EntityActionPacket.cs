﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMProxy.Packets
{
    public class EntityActionPacket : Packet
    {
        public int EntityId;
        public byte Action;

        public override byte PacketId
        {
            get { return 0x13; }
        }

        public override int TryReadPacket(byte[] buffer, int length)
        {
            int offset = 1;
            if (!DataUtility.TryReadInt32(buffer, ref offset, length, out EntityId))
                return -1;
            if (!DataUtility.TryReadByte(buffer, ref offset, length, out Action))
                return -1;
            return offset;
        }
    }
}
