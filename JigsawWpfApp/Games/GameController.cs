using System;
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

        public GameController()
        {
            _serialPort = new SerialPortBuilder().Default;
            string[] ports = SerialPort.GetPortNames();
            Array.Sort(ports);
            _serialPort.PortName = ports.FirstOrDefault();
            _serialPort.DataReceived += _serialPort_DataReceived;
            try
            {
                _serialPort.Open();
            }
            catch (Exception ex)
            {
                throw (ex);
            }

        }

        public event EventHandler<EventArgs> GameCommandRecieved;

        public void OpenPort()
        {
            _serialPort?.Open();
        }

        private void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            
        }
    }
}
