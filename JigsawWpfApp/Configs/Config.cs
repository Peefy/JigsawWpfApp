using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Prism.Mvvm;

using JigsawWpfApp.Communications;

namespace JigsawWpfApp.Configs
{
    public class Config : BindableBase, IDisposable
    {
        [JsonIgnore]
        public static string ConfigFileName { get; set; } = "config.json";

        private string _mainWindowName = "拼图游戏";
        public string MainWindowName
        {
            get => _mainWindowName;
            set => SetProperty(ref _mainWindowName, value);
        }

        public int InitJigsawNumber { get; set; } = 4;

        public int InitStepNumber { get; set; } = 0;

        public int InitGameScore { get; set; } = 0;

        private string _openPortBtnText = "打开串口";
        public string OpenPortBtnText
        {
            get => _openPortBtnText;
            set => SetProperty(ref _openPortBtnText, value);
        }

        private string _openPicBtnText = "开始游戏";
        public string OpenPicBtnText
        {
            get => _openPicBtnText;
            set => SetProperty(ref _openPicBtnText, value);
        }

        [JsonIgnore]
        private static Lazy<Config> _instance;

        [JsonIgnore]
        public static Config Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Lazy<Config>(() => 
                    {
                        return ReadFromFile();
                    },LazyThreadSafetyMode.ExecutionAndPublication);
                return _instance.Value;
            }
        }

        public Config()
        {
            
        }

        ~Config()
        {
            this.Dispose(true);
        }

        public void SaveToJson()
        {
            var str = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(ConfigFileName, str);
        }

        public static Config ReadFromFile()
        {
            try
            {
                if (File.Exists(ConfigFileName) == false)
                    return new Config();
                var str = File.ReadAllText(ConfigFileName);
                var me = JsonConvert.DeserializeObject<Config>(str);
                return me;
            }
            catch (Exception)
            {
                return new Config();
            }
            
        }

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.SaveToJson();
                    // TODO: 释放托管状态(托管对象)。
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~Config() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        void IDisposable.Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}
