using System;
using System.Windows.Forms;
using System.Drawing;

using Prism.Mvvm;
using Prism.Commands;

using JigsawWpfApp.Configs;
using JigsawWpfApp.Games;

using MahApps.Metro;
using MahApps.Metro.Controls.Dialogs;
using System.Windows.Media.Imaging;
using JigsawWpfApp.Views;

namespace JigsawWpfApp.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {

        public Config Config { get; set; }

        private GameController _gameController;

        private string _statusText = "";
        public string StatusText
        {
            get => "游戏状态：" + _statusText;
            set => SetProperty(ref _statusText, value);
        }

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

                        BitmapImage image = new BitmapImage(
                            new Uri(ofd.FileName, UriKind.RelativeOrAbsolute));
                        PuzzleForImage puzzle = new PuzzleForImage(image);//创建拼图
                        #region 设置宽高
                        /*------------------------------------------------------------------
                         *如果选择的图片宽高比例比屏幕的宽高比例大，则窗体和图片区域宽度采用600，高度等比缩放
                         * 反之，图片区域高度采用600，宽度等比算出，但是窗体宽度就不等比缩放了（照顾button）
                         ------------------------------------------------------------------*/
                        #endregion
                        var mainWindow = App.Current.MainWindow as MainWindow;
                        puzzle.SetGrid(mainWindow.GridImage);
                        //这里写创建拼图的代码 
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                
            });
        }

        public DelegateCommand OpenSerialPortCommand { get; set; }

        public DelegateCommand OpenPictureCommand { get; set; }

    }
}
