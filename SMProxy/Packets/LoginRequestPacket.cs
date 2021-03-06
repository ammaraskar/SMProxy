﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMProxy.Packets
{
    public class LoginRequestPacket : Packet
    {
        public int EntityId;
        public string LevelType;
        public GameMode GameMode;
        public Dimension Dimension;
        public Difficulty Difficulty;
        public byte MaxPlayers;
        [FriendlyName("[discarded]")]
        public byte Discarded;

        public override byte PacketId
        {
            get { return 0x01; }
        }

        public override int TryReadPacket(byte[] buffer, int length)
        {
            int offset = 1;
            byte gameMode, dimension, difficulty, discarded;
            if (!DataUtility.TryReadInt32(buffer, ref offset, length, out EntityId))
                return -1;
            if (!DataUtility.TryReadString(buffer, ref offset, length, out LevelType))
                return -1;
            if (!DataUtility.TryReadByte(buffer, ref offset, length, out gameMode))
                return -1;
            if (!DataUtility.TryReadByte(buffer, ref offset, length, out dimension))
                return -1;
            if (!DataUtility.TryReadByte(buffer, ref offset, length, out difficulty))
                return -1;
            if (!DataUtility.TryReadByte(buffer, ref offset, length, out discarded))
                return -1;
            if (!DataUtility.TryReadByte(buffer, ref offset, length, out MaxPlayers))
                return -1;
            GameMode = (GameMode)gameMode;
            Dimension = (Dimension)dimension;
            Difficulty = (Difficulty)difficulty;
            return offset;
        }
    }
}
