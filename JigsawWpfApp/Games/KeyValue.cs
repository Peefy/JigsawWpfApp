using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JigsawWpfApp.Games
{
    public enum KeyValue
    {
        Null = 0x00,
        Up = 0x41,
        Down = 0x42,
        Left = 0x43,
        Right = 0x44,
        A = 0x45,
        B = 0x46,
        C = 0x47,
        D = 0x48,
    }

    public enum GameStatus
    {
        Idle,
        Running,
        Over,
        Ready,
    }

    public enum MsgType
    {
        Move = 0x40,
    }

    public class ComPacket
    {
        public static byte Header { get; set; } = 0x40;
        public MsgType MsgType { get; set; } = MsgType.Move;
        public KeyValue KeyValue { get; set; } = KeyValue.Null;
        public GameStatus GameStatus { get; set; } = GameStatus.Idle;
    }

    public class GameComEventArgs : EventArgs
    {
        public ComPacket ComPacket { get; set; }
    }
    
}
