using System;
using System.Windows.Forms;
using System.Drawing;

using Prism.Mvvm;
using Prism.Commands;

using JigsawWpfApp.Configs;
using JigsawWpfApp.Games;

using System.Windows.Media.Imaging;
using JigsawWpfApp.Views;

namespace JigsawWpfApp.ViewModels
{
    public class MainWindowViewModel : BindableBase , IDisposable
    {

        private int _gameNum = 3;

        public Config Config { get; set; }

        private string _statusText = "";
        public string StatusText
        {
            get => "游戏状态：" + _statusText;
            set => SetProperty(ref _statusText, value);
        }

        GameController _gameController;
        PuzzleForImage _puzzle;
        BitmapImage _image;

        public MainWindowViewModel()
        {
            Config = Config.Instance;
            try
            {
                _gameController = new GameController();
                _gameController.GameCommandRecieved += _gameController_GameCommandRecieved;
                StatusText = "游戏正常";
            }
            catch (Exception ex)
            {
                StatusText = $"error:{ex.Message}";
            }
            OpenSerialPortCommand = new DelegateCommand(() =>
            {
                try
                {
                    _gameController.GameStatus = GameStatus.Ready;
                    _gameController.OpenPort();
                    _gameController.GameCommandRecieved -= _gameController_GameCommandRecieved;
                    _gameController.GameCommandRecieved += _gameController_GameCommandRecieved;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    //StatusText = $"error:{ex.Message}";
                }             
            });
            OpenPictureCommand = new DelegateCommand(() => 
            {
                var ofd = new OpenFileDialog();
                ofd.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG";  //支持的图片格式
                ofd.Multiselect = false;
                //不允许多选
                if(ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        if (_puzzle != null)
                        {
                            _puzzle.Dispose();
                            _puzzle = null;
                        }
                            
                        _image = new BitmapImage(
                            new Uri(ofd.FileName, UriKind.RelativeOrAbsolute));
                        _puzzle = new PuzzleForImage(_image, _gameNum);//创建拼图
                        var mainWindow = App.Current.MainWindow as MainWindow;
                        _puzzle.SetGrid(mainWindow.GridImage);
                        _gameController.GameStatus = GameStatus.Running;
                        //这里写创建拼图的代码 
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                
            });
        }

        private void _gameController_GameCommandRecieved(object sender, GameComEventArgs e)
        {
            var comPacket = e.ComPacket;
            var controller = sender as GameController;
            var msgType = comPacket.MsgType;
            var keyValue = comPacket.KeyValue;
            if(msgType == MsgType.Move)
            {
                if(_puzzle.DoMove(keyValue) == true)
                {
                    controller.GameStatus = GameStatus.Ready;
                    MessageBox.Show("成功完成拼图，请在单片机上查看分数！");               
                }
            }
            else if(msgType == MsgType.SelectGameNum)
            {
                _gameNum = controller.KeyValueToGameNum(keyValue);
                OpenPictureCommand.Execute();
                
            }
            else if(msgType == MsgType.RestartGame)
            {
                if (comPacket.KeyValue == KeyValue.D)
                    OpenPictureCommand.Execute();
                
            }
        }

        public DelegateCommand OpenSerialPortCommand { get; set; }

        public DelegateCommand OpenPictureCommand { get; set; }

        ~MainWindowViewModel()
        {
            Dispose();
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

                if(_gameController != null)
                {
                    _gameController.Dispose();
                    _gameController = null;
                }
                if(_puzzle != null)
                {
                    _puzzle.Dispose();
                    _puzzle = null;
                }
                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~MainWindowViewModel() {
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
