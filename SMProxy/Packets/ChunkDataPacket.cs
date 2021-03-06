﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMProxy.Packets
{
    public class ChunkDataPacket : Packet
    {
        public int X;
        public int Z;
        public bool GroundUpContinuous;
        public ushort PrimaryBitMap;
        public ushort AddBitMap;
        public byte[] CompressedData;

        public override byte PacketId
        {
            get { return 0x33; }
        }

        public override int TryReadPacket(byte[] buffer, int length)
        {
            int offset = 1;
            int dataLength;
            if (!DataUtility.TryReadInt32(buffer, ref offset, length, out X))
                return -1;
            if (!DataUtility.TryReadInt32(buffer, ref offset, length, out Z))
                return -1;
            if (!DataUtility.TryReadBoolean(buffer, ref offset, length, out GroundUpContinuous))
                return -1;
            if (!DataUtility.TryReadUInt16(buffer, ref offset, length, out PrimaryBitMap))
                return -1;
            if (!DataUtility.TryReadUInt16(buffer, ref offset, length, out AddBitMap))
                return -1;
            if (!DataUtility.TryReadInt32(buffer, ref offset, length, out dataLength))
                return -1;
            if (!DataUtility.TryReadArray(buffer, ref offset, length, out CompressedData, (short)dataLength))
                return -1;
            return offset;
        }
    }
}
