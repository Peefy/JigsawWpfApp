using System;
using System.Threading;
using System.IO.Ports;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JigsawWpfApp.Communications;

namespace JigsawWpfApp.Games
{
    public class GameController
    {
        private SerialPort _serialPort;
        private Thread _sendGameStatusThread;
        private int _packetLength = 3;
        
        public GameStatus GameStatus { get; set; } = GameStatus.Idle;

        public GameController()
        {
            _serialPort = new SerialPortBuilder().Default;
            string[] ports = SerialPort.GetPortNames();
            Array.Sort(ports);
            _serialPort.PortName = ports.FirstOrDefault();
            _serialPort.DataReceived += _serialPort_DataReceived;
            try
            {
                OpenPort();
            }
            catch (Exception ex)
            {
                throw (ex);
            }

        }
        public delegate void GameComEventArgsEventHandler(object sender, GameComEventArgs e);
        public event GameComEventArgsEventHandler GameCommandRecieved;

        public void OpenPort()
        {
            _serialPort?.Open();
            if(_sendGameStatusThread != null)
            {
                _sendGameStatusThread = new Thread(new ThreadStart(() =>
                {
                    try
                    {
                        if(_serialPort?.IsOpen == true)
                        {
                            byte[] data =
                            {
                                ComPacket.Header,
                                (byte)GameStatus,
                            };
                            _serialPort.Write(data, 0, data.Length);
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }));
                _sendGameStatusThread.Start();
            }
        }

        private void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] buffers = new byte[_packetLength];
            var bytes = _serialPort.Read(buffers, 0, _packetLength);
            var comPacket = new ComPacket();
            if(buffers[0] == ComPacket.Header)
            {
                comPacket.MsgType = (MsgType)buffers[1];
                comPacket.KeyValue = (KeyValue)buffers[2];
                GameCommandRecieved?.Invoke(_serialPort, new GameComEventArgs()
                {
                    ComPacket = comPacket
                });
            }
        }
    }
}
