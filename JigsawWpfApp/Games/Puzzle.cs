using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JigsawWpfApp.Games
{
    public class Puzzle
    {
        public enum Diff         //游戏难度
        {
            Simple,//简单
            Ordinary,//普通
            Difficulty//困难
        }

        private struct Node      //单元格结构体
        {
            public Image Img;
            public int Num;
        }

        private Image _img;      //拼图图片
        public int Width;       //拼图边长
        private Diff _gameDif;   //游戏难度
        private Node[,] node;    //单元格数组
        public int N;           //单元格数组行列数

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="Img">拼图大图</param>
        /// <param name="GameDif">游戏难度，该类下结构体Diff</param>
        public Puzzle(Image Img, int Width, Diff GameDif)
        {
            this._gameDif = GameDif;
            this._img = Img;
            this.Width = Width;
            switch (this._gameDif)
            {
                case Diff.Simple:
                    this.N = 3;
                    node = new Node[3, 3];
                    break;
                case Diff.Ordinary:
                    this.N = 4;
                    node = new Node[4, 4];
                    break;
                case Diff.Difficulty:
                    this.N = 5;
                    node = new Node[5, 5];
                    break;
            }

            //分割图片形成各单元保存在数组中
            int Count = 0;
            for (int x = 0; x < this.N; x++)
            {
                for (int y = 0; y < this.N; y++)
                {

                    node[x, y].Img = CaptureImage(this._img, this.Width / this.N, this.Width / this.N, x * (this.Width / this.N), y * (this.Width / this.N));
                    node[x, y].Num = Count;
                    Count++;
                }
            }

            for (int x = 0; x < this.N; x++)
            {
                for (int y = 0; y < this.N; y++)
                {
                    Graphics newGra = Graphics.FromImage(node[x, y].Img);
                    newGra.DrawLine(new Pen(Color.White), new Point(0, 0), new Point(0, this.Width / this.N));
                    newGra.DrawLine(new Pen(Color.White), new Point(0, 0), new Point(this.Width / this.N, 0));
                    newGra.DrawLine(new Pen(Color.White), new Point(this.Width / this.N, this.Width / this.N), new Point(this.Width / this.N, 0));
                    newGra.DrawLine(new Pen(Color.White), new Point(this.Width / this.N, this.Width / this.N), new Point(0, this.Width / this.N));
                }
            }
            //(最后一项为空单独处理)
            node[N - 1, N - 1].Img = Image.FromFile("Image\\end.PNG");
            Graphics newGra2 = Graphics.FromImage(node[N - 1, N - 1].Img);
            newGra2.DrawLine(new Pen(Color.Red), new Point(1, 1), new Point(1, this.Width / this.N - 1));
            newGra2.DrawLine(new Pen(Color.Red), new Point(1, 1), new Point(this.Width / this.N - 1, 1));
            newGra2.DrawLine(new Pen(Color.Red), new Point(this.Width / this.N - 1, this.Width / this.N - 1), new Point(this.Width / this.N - 1, 1));
            newGra2.DrawLine(new Pen(Color.Red), new Point(this.Width / this.N - 1, this.Width / this.N - 1), new Point(1, this.Width / this.N - 1));
            //打乱拼图
            this.Upset();

        }


        /// <summary>
        /// 由图片fromImage中截图并返回
        /// </summary>
        /// <param name="fromImage">原图片</param>
        /// <param name="width">宽</param>
        /// <param name="height">高</param>
        /// <param name="spaceX">起始X坐标</param>
        /// <param name="spaceY">起始Y坐标</param>
        /// <returns></returns>
        public Image CaptureImage(Image fromImage, int width, int height, int spaceX, int spaceY)
        {
            int x = 0;
            int y = 0;
            int sX = fromImage.Width - width;
            int sY = fromImage.Height - height;
            if (sX > 0)
            {
                x = sX > spaceX ? spaceX : sX;
            }
            else
            {
                width = fromImage.Width;
            }
            if (sY > 0)
            {
                y = sY > spaceY ? spaceY : sY;
            }
            else
            {
                height = fromImage.Height;
            }

            //创建新图位图   
            Bitmap bitmap = new Bitmap(width, height);
            //创建作图区域   
            Graphics graphic = Graphics.FromImage(bitmap);
            //截取原图相应区域写入作图区   
            graphic.DrawImage(fromImage, 0, 0, new Rectangle(x, y, width, height), GraphicsUnit.Pixel);
            //从作图区生成新图   
            Image saveImage = Image.FromHbitmap(bitmap.GetHbitmap());
            return saveImage;
        }
        /// <summary>
        /// 移动坐标（x,y）拼图单元
        /// </summary>
        /// <param name="x">拼图单元x坐标</param>
        /// <param name="y">拼图单元y坐标</param>
        public bool Move(int x, int y)
        {
            if (x + 1 != N && node[x + 1, y].Num == N * N - 1)
            {
                Swap(new Point(x + 1, y), new Point(x, y));
                return true;
            }
            if (y + 1 != N && node[x, y + 1].Num == N * N - 1)
            {
                Swap(new Point(x, y + 1), new Point(x, y));
                return true;
            }
            if (x - 1 != -1 && node[x - 1, y].Num == N * N - 1)
            {
                Swap(new Point(x - 1, y), new Point(x, y));
                return true;
            }
            if (y - 1 != -1 && node[x, y - 1].Num == N * N - 1)
            {
                Swap(new Point(x, y - 1), new Point(x, y));
                return true;
            }
            return false;
        }
        //交换
        private void Swap(Point a, Point b)
        {
            Node temp = new Node();
            temp = this.node[a.X, a.Y];
            this.node[a.X, a.Y] = this.node[b.X, b.Y];
            this.node[b.X, b.Y] = temp;
        }

        public bool Judge()
        {
            int count = 0;
            for (int x = 0; x < this.N; x++)
            {
                for (int y = 0; y < this.N; y++)
                {
                    if (this.node[x, y].Num != count)
                        return false;
                    count++;
                }
            }
            return true;
        }

        public Image Display()
        {
            Bitmap bitmap = new Bitmap(this.Width, this.Width);
            //创建作图区域   
            Graphics newGra = Graphics.FromImage(bitmap);
            for (int x = 0; x < this.N; x++)
                for (int y = 0; y < this.N; y++)
                    newGra.DrawImage(node[x, y].Img, new Point(x * this.Width / this.N, y * this.Width / this.N));
            return bitmap;
        }

        /// <summary>
        /// 打乱拼图
        /// </summary>
        public void Upset()
        {
            int sum = 100000;
            if (this._gameDif == Diff.Simple) sum = 10000;
            //if (this._gameDif == Diff.ordinary) sum = 100000;
            Random ran = new Random();
            for (int i = 0, x = N - 1, y = N - 1; i < sum; i++)
            {
                long tick = DateTime.Now.Ticks;
                ran = new Random((int)(tick & 0xffffffffL) | (int)(tick >> 32) | ran.Next());
                switch (ran.Next(0, 4))
                {
                    case 0:
                        if (x + 1 != N)
                        {
                            Move(x + 1, y);
                            x = x + 1;
                        }
                        break;
                    case 1:
                        if (y + 1 != N)
                        {
                            Move(x, y + 1);
                            y = y + 1;
                        }
                        break;
                    case 2:
                        if (x - 1 != -1)
                        {
                            Move(x - 1, y);
                            x = x - 1;
                        }
                        break;
                    case 3:
                        if (y - 1 != -1)
                        {
                            Move(x, y - 1);
                            y = y - 1;
                        }
                        break;
                }
            }
        }
    }
}
