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
    public class MainWindowViewModel : BindableBase
    {

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
            }
            catch (Exception ex)
            {
                StatusText = $"error:{ex.Message}";
            }
            OpenSerialPortCommand = new DelegateCommand(() =>
            {
                try
                {
                    _gameController.OpenPort();
                    _gameController.GameCommandRecieved += _gameController_GameCommandRecieved;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    StatusText = $"error:{ex.Message}";
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

                        _image = new BitmapImage(
                            new Uri(ofd.FileName, UriKind.RelativeOrAbsolute));
                        _puzzle = new PuzzleForImage(_image,3);//创建拼图
                        var mainWindow = App.Current.MainWindow as MainWindow;
                        _puzzle.SetGrid(mainWindow.GridImage);
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
            if(comPacket.MsgType == MsgType.Move)
            {
                _puzzle.DoMove(comPacket.KeyValue);
            }
        }

        public DelegateCommand OpenSerialPortCommand { get; set; }

        public DelegateCommand OpenPictureCommand { get; set; }

    }
}
