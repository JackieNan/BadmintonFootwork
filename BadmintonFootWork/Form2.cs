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
using static System.Net.Mime.MediaTypeNames;
using System.Net.NetworkInformation;

namespace BadmintonFootWork
{
    public partial class Form2 : Form
    {
        private Timer displayTimer;
        private Timer hideTimer;
        private System.Drawing.Image imageToShow;
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

            blocks = new Rectangle[numRows, numCols];

            displayTimer.Start();

            originalFormSize = this.Size;
            originalLabel2Location = label2.Location;
            originalLabel2Size = label2.Size;

            string base64Image =
        "iVBORw0KGgoAAAANSUhEUgAAAMgAAADICAYAAACtWK6eAAAAAXNSR0IArs4c6QAAE7tJREFUeF7tnQuW3bYNhq9WkmYlTlaSZiWpV1J7JW1W0mQlt4MxZXN0KeEH +AIo6JzWPR1JlwTxEQ+C1PaIKyQQEjiVwBayCQmEBM4lEICEdoQELiQQgCjU4/l8/uPxeOz/Kb3hr8fj8de2bfRvXI4lEIAwg5dg+Ofj8fjEQHEGCv3//308Hn/SvwGNL1oCkMJ4ZVD80WE4CZav27Z96fDueGVjCQQgSaAZFL8lS9FY1C+vI/drh4X+jcugBG4PSGdrgQ45wfL7tm0BCiqxQffdGpDn8/mvx+PRw43SDh+5XZ8jTtGKr/1ztwQkWY1/v7k4v7QXafUbyZpQjELwxjVZArcDxKDVOFMBAuXXsCZzCbkVIM/n8z9GrcYVJGFNJjJyC0AaulTv7k8arz2gLi0GkuuWr5vQomLNRXFJuFw1ElQ+uzwgCY7/KeVDj32m/6pR0MNiozbuCUgqBlH76NKAVMDRLVBObaLMGa3OS6+ARCqxyvuXBUQJRzcwjuNUsf4SkFQqveTxlQGRBuRUJ/WrRHgt7k2gUFslcUpA0kL4wDuWBESRrZqqcAprEivvgHK3uGU5QJ7PJ/n2tAiIXKYUTbhGE+skyAhX3rMUIMK4g/Zr/Fwpv+aPCwGf4hY277ThF64GCBp3mIRj1xMh6FTkGKXznSBbBhDBzGsajgwS1FV00Z9O+tv9tSsBQouBXCbIVMzBja4gJvmybdvv3Pvi73IJLAGIwHpQ8Z+bPReCEhlX4MvVdN4TqwCCWA+XAa1gnSSsSAeO3AMisB4/ey0dF/TRlYXsoM/NX7kCIEjmaupCYO2oCVwt1/2slVOP51cA5MkIZoksz/P5pCpgmgyuriX62kPRte90DQjoeiyzTgCW0ISbpaWh8Jx3QJDg3G3scRwv0IpEsB6AfJPA822hgJHFUsoCrrCHmxWAvMOBrDQv415lK+xIUiLcrEaQuHWxns8nVexe7srb3vbJNpKTmdeAbtYlIMkSUdBPlQc/nXTubzqAe/+P1xR57cC5VaDn88nFH0u5V5kFIaXm9th/SPcmqPaDJLR74ndY6NAKcuPcVCTUQOIZEC7+WM69ErhZ++krZB00e98RnbrF2cIuAQHjj2WyV4VslrUjU5etBfMKyC3jj8yCWANkb9qwQy8QE9fiHq+A3DL+yABBVtVb6If2HctsB3YHCOheLRt/pPUf64BQM5ewJqsCsmz8kQBBMlna2b/1c66tiUdA2IWyFdc/cq0Fa7IQRd9Tt/T9xON6CEGoTQkff9utNfEICJfeXXL9o0H8sad+aQ0DPuQhW1TcD+PWQuMSEleA3D3+AGuxjrM3Hb5NuymbLOxVni1MbXO1Z8UbIGx69+3zBMvGH0LXqqsiZpaFDuLmDst4gbbmtHzEd2x1jzdAuPQufabAVZ/QgQSt5/AZWnFs6t7lrgCjcuXuc6VMdytvF9Zf0e3TDqYQbAvOFxUJEjge4pS5x9/dAALOoEuufwhcq6nupcKamC9R8QTILeMPweFxZvaACNr8vqBo8Yzk3Rp5AoSLP6a5Fz1Mu3BB0Fzfk8VHA3iz8YgnQG63/gG6VmZnYEFa2qyr5QKQO8YfAjfFjGtVsqTg2Jl1tbwAwsYfK6V3wW21pFQuqgYEsJtLsngBhIs/XCgKGqt4d61OLAlbQ2cxYPcCCBd/mA3yUCiyNQ/WWqZ7TbtWx34L4hFT/TIPCOjDmhKqFApFIaLLCQF0tUxl5DwAws6oq8QfwEktU1fLteAfKgKQT16bmfA8AHKL+AOMO0jXzCiPBhjQIzATU5oGBBSmucyHVHEEWSuXrlUuDzAWMbO2swIgU+uPpDCcZHi4JIRr16oQsLNusxVLaR0QNjXoPf64i2ulsCImrKV1QLiZ1YyvqrEkoAtJr3bvRhasCDf5mchmmQUEVB63igP64ku5Vgcrwh1+ZyIOsQwI4qe6jT8ErpXbPl5ZVTAxMT1jZxkQLr3rdnstaB3dp3QZQJCzvQKQMyGuur327q7Vwc3iYszpgbpJCwLOsC7jD9S18p6dQ5IWgCwCkJN1gSXjD7AWaWnX6mBBzGeyrFqQ5eIPMCgl/XGdukYsR1abFYBIBJYJjvNN3SkR4E5Q902kNjVjpnkG+M7k9HE2Z0FWjD/CtSrjE4AophVAaPRWN2sD4VqdKwEw1mFBCiUIXPxhogQBZR/c43Er1ypzpbmxDkAKgCwTf4Bxx22yVorJMAA5pP3ok8WU4r26XKx/CFyr6bl+1Bq2vg9YDJ4+1qaCdMAndVNeAgw+6Zsrd7ElIOAEMj3WtAbIEu5VuFY8SmC2MgDJAjbEvTLvjoAzI3XbfF94NdffgUwiFsptzFgQcEaZXt15pRJRiIgDA7ig0wN06o0lQLiyA/PxBzIrJhWa7jrgqtz+TnAynB6gWwPEdfwBDvptU7qHbCU7GVpZDDZhQUDlMjGjlObTcK1wKwPGaCbcKzMWBEnvWplRTgBBZkR69NauFQkAdEPNTIZWLAhXcmA2/ohCxObWg15IH9Sh77r/mSqcm3zjHW/pjzutAOIy/gDdhVsvCCpij5Ie78B83bZtKCzTAfEcf4Duwi0LEQ9g/JJKiOightqLYPmaqhC6w2IBEJfba8O1wvRcICfshR/vom+s04IrQdPlsgCIu/hD4FqZycZ00Z6Ll6bMHk1+ZD16XrtFIVk3B8UCIO7ij3CtrvW9s9U4+/F3ULZtoxMbm11TAfEYf4ApaRog02UxzTQoe9FAq3HVfAKFZN/EmswGxFX8IXCtbleIOMlqXFkTGgOKUaqu2YC4ij9i++yrrlVaDcpCfU5v3TNcn96OPqL/XRu7NHG5pgEClmeYCXLBuONWrhXoIp/N4JdWNukHQULA0L/aFHGVDs0EBNn/YaLkIFyrjzpeaTVoZqdxhdcwMlh+U1oWNSQzAXERf4CWjjToFttnBZNFyXJUx2YVcKp+eyYgXHrXhMKFa/VNz5NiktX/QxH1iq0G9xvKpIAYkimAgL6r2ixywkX/DraTXmfCFUT7Jb1vttU4a68SWhEkswDhPr81XenCtfpuOZCxKulwc6txAYqkjaJ2zQKE3T8xe8O+wLVaco9HmiBonDTZo+HusdDlggtIZwHCxR9T3SuBsJdcLRf0/zipi2ZnqavH3S9wielVkI4NBwTshMhP5AQn+fudXauKDJGZLJ5g/KA1qxmAIOndaTMz6lrNdgEl0CP3VlgNev20Ca3UN0FfWFdwBiBmy0sEgp0GMKLsknsqrUbTwkBJu7l7WxWVzgDEZPwhSGVCvis3gBb+LpgQSs01ZTWODRQkGS4D9qGAgPHHlDUF0LWCsx8WAGDWD7SbmaYG4hK5Cia9U49gNCBI/DE8bSqYSd27VgKlcWc1TuIRdknhqkxoNCDm4g+Bwrh2rRrEGqICQ8lM3/NeQVarOPmNBsRc/HGHPR6CSaCkq64nBuoQGLAXY6phgFiMP8C4A8qX95wFa94tcB9NLfrV9PkkYCfv5eoqxpcjATEVfwhmVdPZmotAvOYsKnZ9oKUCj3gXOBm+uFkjATEVfwDfp6Bxc6kolVajyV7uEUov+Q2tBzMEEHC2HubrgrOJO9dKkPsv6ZbLyQCFBAzWX7yFUYCY2V4LziQkd1euVYXVcNdXFIpCLMKlfF8miVGAmIg/wFnElWvVIH3b7AwpreKOeg7IZr0E6qMA4dK7Qz5vIHCthi9WapQkrIZMaqC8Pox9d0BAl6Z7/AG2gyQ+pdRFMtQNrIbLRT+JjEr3grHwh0zWCECQ7ZBdlXIl10oAeklHXMVVtUAo10M+yGgEIFxgRP3o6tKs4FqF1WiDC5De/zBZjwCEiz+6phdBv9N0Shd0Dc40qLv72kZ1x7zFFCCgO9BtAAWK1RVS7dAnq0HnUFGaXHq5KUuXdqzmfmuAIOndbvEH6FqZ3OMhgPt2i35LAIKu6vba2+3ZtRK0PQJxIS0mLIhg9uviXs3+feGYfb8dnVRO3k+HQZM1bvLhGG0fLD8HZjP7pXkVPnOXtKPHPR5hNfqjBcbEfQARzNq5JJrHH2DcYSZr1SB9e5tSkVqEwEmo/Uq6Eg7qb9NjYwTt6GK5pAMIDtjZa030QdrnmfcDtVgvJU/V6yCg2bqSSzNIvLhWDazGLUtFauECAvSXmLgKkMoZMO9vNSReXKtKmYXVUFICehcvLr8akAaW49hVNSRg5+n3pilYWA2lZjd6DNTXl5InFSAChZR2TwwJmLqjdkxbLQcH50xWXVLh0oHxfj/ifpfW5MSACBRylynl5/8UfLpLBIll1yqshg2swAmqOBFpAEGqc7/DsW3br4pPZUGQgB2f4lpVWtlp1s6GSrdtBWI93ibx+oPjkDRZ1rUPRLaGRGDJhitbBOJtFbzmbeAkelqPB1sQ4YxYNFctIRG4Vl33muSDl2REBZqqz5ZFqUgNCuVnQT05XbCWAMKea5WaeDljt4BEMEMPO2xa0KbSSE7LrrVXSTtvRMfkqmAWAgT9ITRTVAOJwJINca0qCwxjz0YnngR6cjk5oYBwuwKpm6J9FVpIHo8H9F2LXmX0B5cK2W9/pgJhNTrBQa8FA3P2NB0WEEFgLnZnFJCgIhW3BX1xEj7FGBCohfeG1ZAIW3EvGHdA2U0EEMR6qGfDDpB0XVgTuJsRayiUu/YRARyQC34JCKgMIteqJICGkFS35WyAYtGvVnX7Py+AgxoDeRkcIEjmCvohTjyNIGnSlmNbwVz6WRehmYqTT/z9XAKKyQvWk1NAQKVoOviVkDR3rRSCz0cxYo0BVCuyiCKdvQIEOZEEJhGVlRKS5q6VIE1Y6ppoEFDZxH0/JDBKT64A4YLzbkqg6DxUu4UoWKXVgDIjSDvinkuXSpNeV1n0IiCge9V8P3kukhmQVFqNZpAGHK8SyPThN0Upj3pszgBB3KvuNU4jIQEzdme6q05zBwzXEkjj8pPydEl6eZX7fQYIl71qHhAz6VU6epOO4EQu0WyhCPIiEEdGoeKeSmuR/3J1GHAGCBd/dHWvjrLtZUnCalRoccNHs/H99FbPR1/nbXFVw0GNeAEE9MO7u1c9IakMxFXBXosRX+UdGRA1rtOZOGh8vm7bRoF89VUChP3g5ohCwFLPWliSSqsxzLWsHllDL8jGjVqFusqaHjSxGvkPlwDhUmhTlUQLSep0FBhq1E74TBojKugkd0mTdRL+4vsBhF3OCisBwmWwpmdslJBodvnRQDWflaSjb/3+AxAt4wiu603dqdKPlQDhDmUYGqCfSUgBCSfs49+7C1/aIEv3dwqs0S5+TunbL+gD2vs0gDQvL9E2viMkYTUOgzIZCGrNlAmrBAi3BmIGEJJaB0imu5DaCaPlcwMD67NmvwNBf2yVkdLIRwPI8BQv17FGkIgWGLk2efu7ESBoDOiQQUoEmfgQ0BKANLAkt7MaEzJNpTljtxLk0tIJnOauZQCpgMRE0mGEZqRFYEq9jsw05V0zD8RxHFwH6SWlUrhby7pWVgLrlCo3aSG4iWk5QJSWZAlIDAExNbDmlF7yd81CoQuX5A6WJAJriarr7tWUmrgJaFeDJPWHYgiqCuhZ03SlTe7iCB0a357SVPO6WkTzDElkmmpUu82zJUBodqLFwrOraodWm2bL3uIJEkNxBI1z91IO2UiOv1u7YcrUajoiNquQGAJimcAa0Qf0njNAuIJFN3FILggLkBgBwtyKNaqwo+87A4TbE+IqDpkJiYFME3X/VoF1S4jOAGHjkHS2qYl6GalAelqSyDRJR8P2/VcHxy3pZu3D0QqSiZuFcs0yUflqW9V1rbsChHOz3GWzjiLSQpLeQ3v3Z9Y0UenG3zNLwXUq5+upK0A4N4t66i6b1QCSGSNMFiIC6wmS5z5/wLlZ7q0IyVxhSUYMVQTWI6TM/AYHCHsEUDpNwv2CkgFIAggDQBybwAGCfItvCSsywZJEYG0QCBEgSWkQKzL1rKyWcu5oSQiICKxbDtaAd11akGxWRQ5ccx+wV6SAS0OVB9Zmt5QO0DHXP8ECkiChEmsK2K+uJTYdZZBwB+idQUEncQQQrrH40XgIkAQJl9Gi25aIRwTn90ZgvQgIZ92QAIKsi9DvuK3TEljLd3nOOsR7cZ001T0YEEHATre6DNpTgH61FyYfvGViLlMaaawxIkASJFwJyt5FVyXx4HdR9r65nACM6Z6L5mgAQdZGXCmSEI4l4iwX2mmgkWJAkhUhSChoRz4pYDq7JQjI9+EK18qA4o5qggqQDBLUX59yMveVEFO8gazvRNwxShsN/o4aEAUk72lgCxutFFaD2h6Ww6AC925SFSAVkNBi2vATvBMYmk+CuTgsr7ey3PH91YAoYpJdznttEn2RtNu5rVltlQYMamvAcUcyUp+bAFIBSQ5LsxKNwzbYmhMIw626MRzU9WaAZJBQ9W+NUu6WhT6kQilV1rp02BduIla6uW6a6H5TQPYeKYPgK4Hkp6fs/3tPMSOpZomwXS1wSjoW98ol0AWQhtZE3iP9EwRel29t65sUT86WQDdAMmtCpfK03tB6pm8lO3NrNK06Fu+pl0B3QDJrQqBQbGIFlACjXn+Wf8MQQDJrQnAQKJRypX9nXAHGDKk7/c2hgOQyarA+IRF5bGySSCvu/S6BaYAUYGltWQKKUPRqCZgA5NiLbF2DXDI63pOuUlp3T/nSv3+nWi9o7aRacvGCW0jAJCC3kHx00oUEAhAXwxSNnCWBAGSW5ON3XUggAHExTNHIWRL4P36yrG60qAk2AAAAAElFTkSuQmCC";
            base64Image = base64Image.Substring(base64Image.IndexOf(",") + 1); // 移除"data:image/png;base64,"部分
            byte[] imageBytes = Convert.FromBase64String(base64Image);
            using (var ms = new System.IO.MemoryStream(imageBytes))
            {
                imageToShow = System.Drawing.Image.FromStream(ms);
            }
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


