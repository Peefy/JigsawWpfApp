﻿namespace JigsawWpfApp.Games
{
    public class ComPacket
    {
        public static byte Header { get; set; } = 0xAA;
        public MsgType MsgType { get; set; } = MsgType.Move;
        public KeyValue KeyValue { get; set; } = KeyValue.Null;
        public GameStatus GameStatus { get; set; } = GameStatus.Idle;
    }
    
}
