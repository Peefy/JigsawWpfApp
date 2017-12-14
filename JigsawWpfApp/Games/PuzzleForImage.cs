using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace JigsawWpfApp.Games
{
    /// <summary>
    /// 拼图生成类
    /// 传入一张图片 生成一个9*9的图片集合
    /// </summary>
    public class PuzzleForImage
    {

        public static int GameNum = 3;

        BitmapImage _image;                                         //原图片
        List<Rectangle> initialUnallocatedParts = new List<Rectangle>();//要返回拼图集合
        public List<Rectangle> allocatedParts = new List<Rectangle>();         //被打乱后的图片
        int[] map = new int[GameNum * GameNum];                                                //游戏地图 判断是否成功

        /// <summary>
        /// 新建对象时传入原图片
        /// </summary>
        /// <param name="image"></param>
        public PuzzleForImage(BitmapImage image)
        {
            _image = image;
            CreatePuzzleForImage();
        }

        /// <summary>
        /// 将拼图放到UI中
        /// </summary>
        /// <param name="GridImg"></param>
        public void SetGrid(Grid GridImg)
        {
            GridImg.Children.Clear();
            GridImg.Height = _image.Height / _image.Width * GridImg.Width;
            int index = 0;
            for (int i = 0; i < GameNum; i++)
            {
                for (int j = 0; j < GameNum; j++)
                {
                    allocatedParts[index].SetValue(Grid.RowProperty, i);
                    allocatedParts[index].SetValue(Grid.ColumnProperty, j);
                    GridImg.Children.Add(allocatedParts[index]);
                    index++;
                }
            }
            Application.Current.MainWindow.KeyDown += MainWindow_KeyDown;
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            Rectangle rectBlank = allocatedParts[allocatedParts.Count - 1];   //缺省方块
            int currentBlankRow = (int)rectBlank.GetValue(Grid.RowProperty);
            int currentBlankCol = (int)rectBlank.GetValue(Grid.ColumnProperty);
            int nowRow = currentBlankRow;
            int nowCol = currentBlankCol;
            if (e.Key == Key.S)
            {
                nowRow = nowRow - 1;
            }
            else if(e.Key == Key.W)
            {
                nowRow = nowRow + 1;
            }
            else if(e.Key == Key.D)
            {
                nowCol = nowCol - 1;
            }
            else if(e.Key == Key.A)
            {
                nowCol = nowCol + 1;
            }
            try
            {
                RectPart_MouseDown(allocatedParts[nowRow * GameNum + nowCol], null);

            }
            catch
            {

            }
        }

        /// <summary>
        /// 创建拼图
        /// </summary>
        private void CreatePuzzleForImage()
        {
            #region 拼图容器初始化
            initialUnallocatedParts.Clear();
            allocatedParts.Clear();
            #endregion

            #region 创建前8个拼图
            double width = 1.0 / GameNum;//每个拼图的宽度
            double height = 1.0 / GameNum;//每个拼图的高度

            int c = 0;
            for (var j = 0; j < GameNum; ++j)
            {
                for (var i = 0; i < GameNum; ++i)
                {
                    c++;
                    if (c <= GameNum * GameNum - 1)
                        CreateImagePart(i * width, j * height, width, height);
                }
            }
            #endregion

            //随机排列
            RandomizeTiles();

            //创建白色方块
            CreateBlankRect();
        }

        /// <summary>
        /// 绘制一个矩形
        /// 创建一个图像画刷使用X / Y /宽度/高度参数创建时，只显示图像的一部分。这是ImageBrush用来填充矩形，添加到未分配的矩形的内部表
        /// </summary>
        /// <param name="x">起点x</param>
        /// <param name="y">起点y</param>
        /// <param name="width">宽</param>
        /// <param name="height">高</param>
        private void CreateImagePart(double x, double y, double width, double height)
        {
            #region 定义笔刷
            ImageBrush ib = new ImageBrush();
            //ib.Stretch = Stretch.UniformToFill;                  //裁剪已适应屏幕 这个不能要，会造成图片不拉伸对不上
            ib.ImageSource = _image;
            ib.Viewbox = new Rect(x, y, width, height);
            ib.ViewboxUnits = BrushMappingMode.RelativeToBoundingBox; //按百分比设置宽高
            ib.TileMode = TileMode.None;                          //按百分比应该不会出现 image小于要切的值的情况
            #endregion

            #region 定义矩形
            Rectangle rectPart = new Rectangle();
            rectPart.Fill = ib;
            rectPart.Margin = new Thickness(0);
            rectPart.HorizontalAlignment = HorizontalAlignment.Stretch;
            rectPart.VerticalAlignment = VerticalAlignment.Stretch;
            rectPart.MouseDown += new MouseButtonEventHandler(RectPart_MouseDown);//添加矩形点击事件
            #endregion
            initialUnallocatedParts.Add(rectPart);                                         //将矩形添加到拼图集合
        }

        /// <summary>
        /// 将 图片打乱
        /// </summary>
        private void RandomizeTiles()
        {
            Random rand = new Random();
            for (int i = 0; i < 8; i++)
            {
                int index = 0;
                //if (initialUnallocatedParts.Count > 1)
                //{
                //    index = (int)(rand.NextDouble() * initialUnallocatedParts.Count);
                //}
                index = (int)(rand.NextDouble() * initialUnallocatedParts.Count);
                while (initialUnallocatedParts[index] == null)
                {
                    index = (int)(rand.NextDouble() * initialUnallocatedParts.Count);
                }
                allocatedParts.Add(initialUnallocatedParts[index]);
                //initialUnallocatedParts.RemoveAt(index); // 移除图片
                initialUnallocatedParts[index] = null;
                map[i] = index;                          // 添加地图
            }
        }

        /// <summary>
        /// 再创建一个空白矩形
        /// </summary>
        private void CreateBlankRect()
        {
            Rectangle rectPart = new Rectangle();
            rectPart.Fill = new SolidColorBrush(Colors.White);
            rectPart.Margin = new Thickness(0);
            rectPart.HorizontalAlignment = HorizontalAlignment.Stretch;
            rectPart.VerticalAlignment = VerticalAlignment.Stretch;
            allocatedParts.Add(rectPart);
            map[GameNum * GameNum - 1] = GameNum * GameNum - 1;
        }

        /// <summary>
        /// 方块点击事件
        /// </summary>
        public void RectPart_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //get the source Rectangle, and the blank Rectangle
            //NOTE : Blank Rectangle never moves, its always the last Rectangle
            //in the allocatedParts List, but it gets re-allocated to 
            //different Gri Row/Column
            Rectangle rectCurrent = sender as Rectangle;                        //当前方块
            Rectangle rectBlank = allocatedParts[allocatedParts.Count - 1];   //缺省方块

            //得到白块和缺省方块的位置
            int currentTileRow = (int)rectCurrent.GetValue(Grid.RowProperty);
            int currentTileCol = (int)rectCurrent.GetValue(Grid.ColumnProperty);
            int currentBlankRow = (int)rectBlank.GetValue(Grid.RowProperty);
            int currentBlankCol = (int)rectBlank.GetValue(Grid.ColumnProperty);

            //白块能移动的四个位置
            List<PossiblePositions> posibilities = new List<PossiblePositions>();
            posibilities.Add(new PossiblePositions { Row = currentBlankRow - 1, Col = currentBlankCol });
            posibilities.Add(new PossiblePositions { Row = currentBlankRow + 1, Col = currentBlankCol });
            posibilities.Add(new PossiblePositions { Row = currentBlankRow, Col = currentBlankCol - 1 });
            posibilities.Add(new PossiblePositions { Row = currentBlankRow, Col = currentBlankCol + 1 });

            //检查该方块是否能点击（白块能移动到当前方块的位置）
            bool validMove = false;
            foreach (PossiblePositions position in posibilities)
                if (currentTileRow == position.Row && currentTileCol == position.Col)
                    validMove = true;

            //only allow valid move
            if (validMove)
            {
                //交换位置
                rectCurrent.SetValue(Grid.RowProperty, currentBlankRow);
                rectCurrent.SetValue(Grid.ColumnProperty, currentBlankCol);

                rectBlank.SetValue(Grid.RowProperty, currentTileRow);
                rectBlank.SetValue(Grid.ColumnProperty, currentTileCol);

                //更新地图
                int indexCur = currentTileRow * 3 + currentTileCol;
                int indexBlank = currentBlankRow * 3 + currentBlankCol;
                int temp = map[indexCur];
                map[indexCur] = map[indexBlank];
                map[indexBlank] = temp;

                //判断是否成功
                if (isSuccess())
                {
                    MessageBox.Show("成功了，好棒啊！");
                }
            }
        }

        /// <summary>
        /// 验证是否游戏成功
        /// </summary>
        /// <returns></returns>
        private bool isSuccess()
        {
            for (int i = 0; i < 9; i++)
            {
                if (map[i] != i)
                {
                    return false;
                }
            }
            return true;
        }
    }

    /// <summary>
    /// Simply struct to store Row/Column data
    /// </summary>
    struct PossiblePositions
    {
        public int Row { get; set; }
        public int Col { get; set; }
    }

}
