using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Drawing.Drawing2D;

namespace BadmintonFootWork
{
    public partial class Form2 : Form
    {
        private Timer displayTimer;
        private Timer hideTimer;
        private Image imageToShow;
        private Point currentImagePosition;
        private const int numRows = 8; // 行数
        private const int numCols = 10; // 列数
        private Rectangle[,] blocks;
        private Timer countdownTimer;
        private double countdownSeconds; // 用于跟踪倒计时的秒数

        private Size originalFormSize;
        private Point originalLabel1Location;
        private Size originalLabel1Size;
        private Point originalLabel2Location;
        private Size originalLabel2Size;


        public Form2(int displayInterval, int hideInterval)
        {
            InitializeComponent();
            label2.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            label2.Dock = DockStyle.None;

            this.DoubleBuffered = true; // 设置双缓冲
            this.Size = new Size(900, 960);
            
            // 初始化定时器
            displayTimer = new Timer();
            displayTimer.Interval = displayInterval; // 设置时间间隔，例如2000毫秒
            displayTimer.Tick += DisplayTimer_Tick;

            hideTimer = new Timer();
            hideTimer.Interval = hideInterval; // 设置图片显示时间
            hideTimer.Tick += HideTimer_Tick;

            // 初始化倒计时定时器
            countdownTimer = new Timer();
            countdownTimer.Interval = 100; // 0.1秒更新一次
            countdownTimer.Tick += CountdownTimer_Tick;

            // 加载图片
            imageToShow = Image.FromFile("C:\\Users\\JackieNan\\Projects\\BadmintonFootwork\\羽毛球.png"); // 更改为您的图片路径

            blocks = new Rectangle[numRows, numCols];

            displayTimer.Start();

            originalFormSize = this.Size;
            originalLabel2Location = label2.Location;
            originalLabel2Size = label2.Size;

    }

        private void UpdateFontSizes()
        {
            float ratioHeight = (float)this.Height / originalFormSize.Height;
            float minFontSize = 15.0f;  // 设置最小字体大小

            if (ratioHeight > 0 && originalLabel2Size.Height > 0)
            {
                float calculatedSize2 = originalLabel2Size.Height * ratioHeight * 0.5f;
                float newSize2 = Math.Max(calculatedSize2, minFontSize);
                label2.Font = new Font(label2.Font.FontFamily, newSize2, label2.Font.Style);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            float ratioWidth = (float)this.Width / originalFormSize.Width;
            float ratioHeight = (float)this.Height / originalFormSize.Height;

            if (label2.Width > 0 && label2.Height > 0 && label2.Location.X >= 0 && label2.Location.Y >= 0)
            {
                label2.Location = new Point((int)(originalLabel2Location.X * ratioWidth), (int)(originalLabel2Location.Y * ratioHeight));
                label2.Size = new Size((int)(originalLabel2Size.Width * ratioWidth), (int)(originalLabel2Size.Height * ratioHeight));
            }

            UpdateFontSizes();  // 更新字体大小
            this.Invalidate();  // 标记整个控件为需要重绘
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            float ratio = 6.74f / 6.1f;
            Graphics graphics = e.Graphics;
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            
            // 绘制羽毛球场
            int courtWidth = this.Width - 50; // 为边界留出一些空间
            int courtHeight = (int)(courtWidth / ratio);

            // 确保球场的高度不会超过控件的当前高度
            if (courtHeight > this.Height - 20)
            {
                courtHeight = this.Height - 20;
                courtWidth = (int)(courtHeight * ratio);
            }

            // 计算球场的位置，使其居中
            int courtX = (this.Width - courtWidth) / 2 ;
            int courtY = (this.Height - courtHeight) / 2 + 50;
            // 定义羽毛球场的边界和区域
            Rectangle courtBounds = new Rectangle(courtX, courtY, courtWidth, courtHeight);
            
            //场地涂成绿的
            using (SolidBrush greenBrush = new SolidBrush(Color.Green))
            {
                graphics.FillRectangle(greenBrush, courtBounds);
            }
            // 绘制羽毛球场的外框
            Pen pen = new Pen(Color.White, 8);
            graphics.DrawRectangle(pen, courtBounds);
            graphics.DrawLine(pen, (float)(courtX + courtWidth * 0.46 / 6.1), courtBounds.Top, (float)(courtX + courtWidth * 0.46 / 6.1), courtBounds.Bottom);
            graphics.DrawLine(pen, (float)(courtX + courtWidth * 5.6 / 6.1), courtBounds.Top, (float)(courtX + courtWidth * 5.6 / 6.1), courtBounds.Bottom);
            graphics.DrawLine(pen, (float)(courtX + courtWidth * 3.03 / 6.1), courtBounds.Top, (float)(courtX + courtWidth * 3.03 / 6.1), courtBounds.Bottom - (float)(courtHeight * 2.06 / 6.74));
            graphics.DrawLine(pen, courtX, courtBounds.Top + (float)(courtHeight * 0.76 / 6.74), (courtX + courtWidth), courtBounds.Top + (float)(courtHeight * 0.76 / 6.74));
            graphics.DrawLine(pen, courtX, courtBounds.Bottom - (float)(courtHeight * 2.06 / 6.74), (courtX + courtWidth), courtBounds.Bottom - (float)(courtHeight * 2.06 / 6.74));

            int blockWidth = courtWidth / numCols;
            int blockHeight = courtHeight / numRows;

            // 初始化区块的位置
            for (int row = 0; row < numRows; row++)
            {
                for (int col = 0; col < numCols; col++)
                {
                    blocks[row, col] = new Rectangle(courtX + col * blockWidth, courtY + row * blockHeight, blockWidth, blockHeight);
                }
            }
            for (int row = 0; row < numRows; row++)
            {
                for (int col = 0; col < numCols; col++)
                {
                    if (currentImagePosition != Point.Empty && currentImagePosition.Y == row && currentImagePosition.X == col)
                    {
                        // 根据需要调整图片的大小
                        Rectangle destRect = new Rectangle(blocks[row, col].X, blocks[row, col].Y, blocks[row, col].Width, blocks[row, col].Height);
                        // 绘制图片
                        e.Graphics.DrawImage(imageToShow, destRect, new Rectangle(0, 0, imageToShow.Width, imageToShow.Height), GraphicsUnit.Pixel);
                    }
                }
            }

            // 释放资源
            pen.Dispose();
        }
        private void DisplayTimer_Tick(object sender, EventArgs e)
        {
            // 选择一个随机区块来显示图片
            Random random = new Random();
            int row = random.Next(numRows);
            int col = random.Next(numCols);
            currentImagePosition = new Point(col, row);

            // 开始图片显示的定时器
            hideTimer.Start();
            displayTimer.Stop(); // 停止显示图片的定时器直到图片隐藏后再次启动

            // 触发重绘事件以更新界面
            this.Invalidate(blocks[row, col]);
            countdownSeconds = hideTimer.Interval / 1000.0;
            UpdateCountdownLabel();
            countdownTimer.Start();

            // 更新倒计时显示
        }

        private void HideTimer_Tick(object sender, EventArgs e)
        {
            // 停止隐藏图片的定时器并清除当前图片位置
            hideTimer.Stop();
            var previousImagePosition = currentImagePosition;
            currentImagePosition = Point.Empty;

            // 触发重绘事件以清除图片
            this.Invalidate(blocks[previousImagePosition.Y, previousImagePosition.X]);

            // 重新开始显示图片的定时器
            displayTimer.Start();
            countdownSeconds = displayTimer.Interval  / 1000.0;
            UpdateCountdownLabel();
            countdownTimer.Start();
        }
        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            countdownSeconds -=0.1; // 减少一秒
            if (countdownSeconds <= 0)
            {
                countdownSeconds = 0; // 避免显示负数

                // 停止倒计时定时器
                countdownTimer.Stop();
            }
            // 更新标签显示剩余秒数
            UpdateCountdownLabel(); // 更新倒计时显示
        }
        private void UpdateCountdownLabel()
        {
            if (countdownSeconds > 0)
            {
                label2.Text = $"{countdownSeconds:F1} 秒";
            }
            else
            {
                label2.Text = "0 秒";
            }
        }

    }
} 


