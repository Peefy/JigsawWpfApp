using System;
using System.Threading;
using System.Windows.Threading;
using System.IO.Ports;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using JigsawWpfApp.Communications;

namespace JigsawWpfApp.Games
{
    public class GameController : IDisposable
    {
        SerialPort _serialPort;
        Thread _sendGameStatusThread;
        Dispatcher _dip;
        SynchronizationContext _ds;

        public Queue<byte> Buffers { get; set; }

        public GameStatus GameStatus { get; set; } = GameStatus.Ready;

        public static int PacketLength { get; set; } = 3;

        public GameController()
        {
            _dip = Dispatcher.CurrentDispatcher;
            _ds = new DispatcherSynchronizationContext();
            _serialPort = new SerialPortBuilder().Default;
            Buffers = new Queue<byte>();
            string[] ports = SerialPort.GetPortNames();
            Array.Sort(ports);
            if (ports.Length >= 2)
            {
                _serialPort.PortName = ports[1];
                _serialPort.DataReceived += _serialPort_DataReceived;
            }
            else
            {
                _serialPort.PortName = ports.FirstOrDefault();
                _serialPort.DataReceived += _serialPort_DataReceived;
            }
            try
            {
                OpenPort();
            }
            catch
            {

            }

        }
        public delegate void GameComEventArgsEventHandler(object sender, GameComEventArgs e);
        public event GameComEventArgsEventHandler GameCommandRecieved;

        public void OpenPort()
        {
            _serialPort?.Open();
            Task.Run(() =>
            {
                try
                {
                    while (true)
                    {
                        if (_serialPort?.IsOpen == true)
                        {
                            byte[] data =
                            {
                                    ComPacket.Header,
                                    (byte)GameStatus,
                                };
                            _serialPort.Write(data, 0, data.Length);
                        }
                        if (Buffers.Count >= PacketLength)
                        {
                            var comPacket = new ComPacket();
                            if (Buffers.Dequeue() == ComPacket.Header)
                            {
                                comPacket.MsgType = (MsgType)Buffers.Dequeue();
                                comPacket.KeyValue = (KeyValue)Buffers.Dequeue();
                                _dip.Invoke(new Action(() =>
                                {
                                    GameCommandRecieved?.Invoke(this, new GameComEventArgs()
                                    {
                                        ComPacket = comPacket
                                    });
                                }));
                            }
                        }
                        Thread.Sleep(10);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
        }

        private void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var bytesNum = _serialPort.BytesToRead;
            byte[] buffers = new byte[bytesNum];
            _serialPort.Read(buffers, 0, bytesNum);
            for (var i = 0; i < bytesNum; ++i)
                Buffers.Enqueue(buffers[i]);
        }

        public KeyValue KeyValueToMoveCmd(KeyValue keyValue)
        {
            return keyValue;
        }

        public int KeyValueToGameNum(KeyValue keyValue)
        {
            var gameNum = ((int)keyValue - (int)KeyValue.A) +
                ((int)KeyValue.D - (int)KeyValue.A);
            return (gameNum <= 3) ? 3 : gameNum;
        }

        ~GameController()
        {
            this.Dispose();
        }

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                }
                if (_sendGameStatusThread != null)
                {
                    _sendGameStatusThread.Abort();
                    _sendGameStatusThread = null;
                }
                if (_serialPort != null)
                {
                    _serialPort.DataReceived -= _serialPort_DataReceived;
                    _serialPort.Dispose();
                }
                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~GameController() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}
