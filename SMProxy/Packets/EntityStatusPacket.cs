﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMProxy.Packets
{
    public class EntityStatusPacket : Packet
    {
        public int EntityId;
        public byte Status;

        public override byte PacketId
        {
            get { return 0x26; }
        }

        public override int TryReadPacket(byte[] buffer, int length)
        {
            int offset = 1;
            if (!DataUtility.TryReadInt32(buffer, ref offset, length, out EntityId))
                return -1;
            if (!DataUtility.TryReadByte(buffer, ref offset, length, out Status))
                return -1;
            return offset;
        }
    }
}
