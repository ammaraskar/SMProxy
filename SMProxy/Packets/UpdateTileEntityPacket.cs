using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LibNbt;

namespace SMProxy.Packets
{
    public class UpdateTileEntityPacket : Packet
    {
        public Vector3 Position;
        public byte Action;
        public NbtFile Data;

        public override byte PacketId
        {
            get { return 0x84; }
        }

        public override int TryReadPacket(byte[] buffer, int length)
        {
            int offset = 1;
            int x, z;
            short y;
            short dataLength;
            byte[] data;
            if (!DataUtility.TryReadInt32(buffer, ref offset, length, out x))
                return -1;
            if (!DataUtility.TryReadInt16(buffer, ref offset, length, out y))
                return -1;
            if (!DataUtility.TryReadInt32(buffer, ref offset, length, out z))
                return -1;
            if (!DataUtility.TryReadByte(buffer, ref offset, length, out Action))
                return -1;
            if (!DataUtility.TryReadInt16(buffer, ref offset, length, out dataLength))
                return -1;
            if (dataLength != 0)
            {
                if (!DataUtility.TryReadArray(buffer, ref offset, length, out data, dataLength))
                    return -1;
                Data = new NbtFile();
                Data.LoadFile(new MemoryStream(data), true);
            }
            Position = new Vector3(x, y, z);
            return offset;
        }
    }
}
