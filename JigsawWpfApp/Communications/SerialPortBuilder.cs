using System;
using System.IO;
using System.IO.Ports;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace JigsawWpfApp.Communications
{
    public class SerialPortBuilder
    {
        [JsonIgnore]
        public static string JsonFileName { get; set; } = "com_config.json";

        [JsonIgnore]
        public SerialPort Default
        {
            get
            {
                return new SerialPort()
                {
                    BaudRate = 9600,
                    Parity = Parity.None,
                    DataBits = 8,
                    StopBits = StopBits.One,
                    PortName = ComText,
                };
            }
        }

        public string ComText { get; set; } = "COM4";

        public void SaveToJsonFile()
        {

        }

        public SerialPort FromJsonFile()
        {
            if (File.Exists(JsonFileName) == false)
                return Default;
            var str = File.ReadAllText(JsonFileName);
            return Default;
        }

    }
}
