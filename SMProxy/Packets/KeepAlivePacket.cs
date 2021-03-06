﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMProxy.Packets
{
    public class KeepAlivePacket : Packet
    {
        public int KeepAlive;

        public override byte PacketId
        {
            get { return 0x00; }
        }

        public override int TryReadPacket(byte[] buffer, int length)
        {
            int offset = 1;
            if (!DataUtility.TryReadInt32(buffer, ref offset, length, out KeepAlive))
                return -1;
            return offset;
        }
    }
}
