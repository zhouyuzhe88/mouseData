using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MouseData
{
    /// <summary>
    /// Draw.xaml 的交互逻辑
    /// </summary>
    partial class Draw : Window
    {
        Dictionary<Button, List<Canvas>> ButCav { get; set; }
        Dictionary<Button, int> ButId { get; set; }
        bool[] TrailUse { get; set; }
        Canvas[][][] Canvases { get; set; }

        Experiment Exp { get; set; }
        int CavHeight { get; set; }
        int CavWidth { get; set; }
        Button BtAll { get; set; }
        Button BtAnalysis { get; set; }

        internal Draw(Experiment exp)
        {
            InitializeComponent();
            this.Exp = exp;
            this.ButCav = new Dictionary<Button, List<Canvas>>();
            this.ButId = new Dictionary<Button, int>();
            this.TrailUse = new bool[Exp.Trils.Count];
            for (int i = 0; i < Exp.Trils.Count; ++i)
            {
                this.TrailUse[i] = true;
            }
            DrawBackground();
            BtAnalysis = BuildBaseButton("Analysis");
            BtAnalysis.Click += BtAnalysis_Click;
            BtAll = BuildBaseButton("ALL");
            BtAll.Click += BtAll_Click;
            InitCanvases();
            for (int i = 0; i < exp.Trils.Count; ++i)
            {
                DrawTrail(i);
            }
        }

        void BtAnalysis_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Trail\tL/R\tArea\tFood\tChannel\tSPK");
            sb.AppendLine();
            for (int i = 0; i < Exp.Trils.Count; ++i)
            {
                if (this.TrailUse[i])
                {
                    sb.Append(Exp.Trils[i].Analsys());
                }
            }
            string res = sb.ToString();
            string fileName = string.Format("{0}-{1}-{2}-{3}-analsys.txt", Exp.Tag, Exp.Id, DateTime.Now.Hour, DateTime.Now.Minute);
            File.WriteAllText(fileName, res, Encoding.Default);
        }

        private void InitCanvases()
        {
            Canvases = new Canvas[Exp.Trils.Count][][];
            for (int i = 0; i < Exp.Trils.Count; ++i)
            {
                Trail t = Exp.Trils[i];
                Button bt = BuildBaseButton("Trail" + t.Id);
                bt.Click += bt_Click;
                List<Canvas> cavs = new List<Canvas>();
                this.ButCav[bt] = cavs;
                this.ButId[bt] = i;
                Canvases[i] = new Canvas[Exp.ChannelCnt][];
                for (int j = 0; j < Exp.ChannelCnt; ++j)
                {
                    Canvases[i][j] = new Canvas[Parameters.ColorRate.Count];
                    for (int k = 0; k < Parameters.ColorRate.Count; ++k)
                    {
                        Canvases[i][j][k] = BuildBaseCanvas(j / 4, j % 4);
                        cavs.Add(Canvases[i][j][k]);
                    }
                }
            }
            for (int k = Parameters.ColorRate.Count - 1; k >= 0; --k)
            {
                for (int j = 0; j < Exp.ChannelCnt; ++j)
                {
                    for (int i = 0; i < Exp.Trils.Count; ++i)
                    {
                        this.MainGrid.Children.Add(Canvases[i][j][k]);
                    }
                }
            }
        }

        private void DrawTrail(int ti)
        {
            Trail t = Exp.Trils[ti];
            foreach (Segment seg in t.Segments)
            {
                if (seg.Points.Count == 0 || seg.Length < Parameters.SegmentLength * Parameters.SegmentRate)
                {
                    continue;
                }
                for (int i = 0; i < Exp.ChannelCnt; ++i)
                {
                    int colorId = Exp.GetColorId(i, seg.WaveList[i].Count / seg.Length * Parameters.SegmentLength);
                    Point p = seg.Points.Last().Position;
                    AddPoint(Canvases[ti][i][colorId], Parameters.ColorRadio[colorId], (int)(p.X * Parameters.XRate),
                        (this.CavHeight + Exp.MaxY * Parameters.YRate) / 2 - (int)(p.Y * Parameters.YRate), Parameters.ColorList[colorId]);
                }
            }
        }

        private void DrawBackground()
        {
            this.Title = "Experiment " + Exp.Id;
            this.CavWidth = (int)(((int)Exp.MaxX / 50 + ((int)Exp.MaxX % 50 == 0 ? 0 : 1)) * 50 * Parameters.XRate + 50);
            this.CavHeight = (int)(((int)Exp.MaxY / 50 + ((int)Exp.MaxY % 50 == 0 ? 0 : 1)) * 50 * Parameters.YRate + 50);
            for (int i = 0; i < Exp.ChannelCnt; ++i)
            {
                Canvas cv = BuildBaseCanvas(i / 4, i % 4);
                AddAxis(cv);
                cv.Children.Add(DrawTitle(i));
                this.MainGrid.Children.Add(cv);
            }
            this.MainGrid.Width = (this.CavWidth + 20) * (Exp.ChannelCnt < 4 ? Exp.ChannelCnt : 4) + 20;
            this.MainGrid.Height = (this.CavHeight + 20) * (Exp.ChannelCnt / 4 + (Exp.ChannelCnt % 4 == 0 ? 0 : 1)) + 20;
        }

        private StackPanel DrawTitle(int channelId)
        {
            StackPanel res = new StackPanel();
            res.Orientation = Orientation.Vertical;
            TextBlock tb = new TextBlock();
            tb.Text = "  " + Exp.ChannelTag[channelId];
            res.Children.Add(tb);
            StackPanel colors = new StackPanel();
            colors.Orientation = Orientation.Horizontal;
            List<double> fLs = new List<double>() { Exp.MaxWaveCnt[channelId] * 1000.0 / Parameters.SegmentLength };
            for (int i = 0; i < Parameters.ColorRate.Count; ++i)
            {
                fLs.Add(Parameters.ColorRate[i] * fLs[0]);
            }
            ++fLs[0];
            TextBlock blk = new TextBlock();
            blk.Text = "  ";
            colors.Children.Add(blk);
            for (int i = 0; i < Parameters.ColorRate.Count; ++i)
            {
                Ellipse e = new Ellipse();
                e.Width = e.Height = 7 * Math.Min(1.2, Parameters.XRate * 1.4);
                e.Fill = Parameters.ColorList[i];
                colors.Children.Add(e);
                TextBlock t = new TextBlock();
                t.Text = " " + ((int)fLs[i] - 1) + " - " + (int)fLs[i + 1] + "HZ     ";
                t.FontSize *= Math.Min(1.2, Parameters.XRate * 1.4);
                colors.Children.Add(t);
            }
            res.Children.Add(colors);
            return res;
        }

        private void AddAxis(Canvas cv)
        {
            Line lineX = new Line();
            lineX.X1 = 0;
            lineX.Y1 = cv.Height;
            lineX.X2 = cv.Width;
            lineX.Y2 = cv.Height;
            lineX.Stroke = Brushes.DarkGray;
            lineX.StrokeThickness = 2;
            cv.Children.Add(lineX);

            Line lineY = new Line();
            lineY.X1 = 0;
            lineY.Y1 = cv.Height;
            lineY.X2 = 0;
            lineY.Y2 = 0;
            lineY.Stroke = Brushes.DarkGray;
            lineY.StrokeThickness = 2;
            cv.Children.Add(lineY);

            for (double i = 0; i < cv.Width; i += 50 * Parameters.XRate)
            {
                AddPoint(cv, 1.5, i, 0, Brushes.Black);
            }

            for (double i = 0; i < cv.Height; i += 50 * Parameters.YRate)
            {
                AddPoint(cv, 1.5, 0, i, Brushes.Black);
            }
        }

        private static void AddPoint(Canvas cv, double radio, double x, double y, Brush bruch)
        {
            Type t = Parameters.ShapeType;
            Shape shape = (Shape)Activator.CreateInstance(t);
            shape.Height = radio * 2 * Parameters.YRate;
            shape.Width = radio * 2 * Parameters.XRate;
            shape.Margin = new Thickness(x - radio, cv.Height - y - radio, 0, 0);
            shape.Fill = bruch;
            cv.Children.Add(shape);
        }

        Button BuildBaseButton(string content)
        {
            Button bt = new Button();
            bt.Focusable = false;
            bt.Background = Brushes.Gold;
            bt.Content = content;
            bt.Height = 40;
            bt.Width = 80;
            bt.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            this.filter.Children.Add(bt);
            return bt;
        }

        private Canvas BuildBaseCanvas(int r, int c)
        {
            Canvas cv = new Canvas();
            cv.Height = this.CavHeight;
            cv.Width = this.CavWidth;
            cv.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            cv.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            cv.Margin = new Thickness(20 + c * (this.CavWidth + 20), 20 + r * (this.CavHeight + 20), 0, 0);
            return cv;
        }

        void BtAll_Click(object sender, RoutedEventArgs e)
        {
            Button btAll = sender as Button;
            if (btAll.Background == Brushes.Gold)
            {
                btAll.Background = Brushes.Gray;
                for (int i = 0; i < Exp.Trils.Count; ++i)
                {
                    this.TrailUse[i] = false;
                }
                foreach (Button bt in ButCav.Keys)
                {
                    bt.Background = Brushes.Gray;
                    foreach (Canvas cv in ButCav[bt])
                    {
                        cv.Visibility = System.Windows.Visibility.Hidden;
                    }
                }
            }
            else
            {
                btAll.Background = Brushes.Gold;
                for (int i = 0; i < Exp.Trils.Count; ++i)
                {
                    this.TrailUse[i] = true;
                }
                foreach (Button bt in ButCav.Keys)
                {
                    bt.Background = Brushes.Gold;
                    foreach (Canvas cv in ButCav[bt])
                    {
                        cv.Visibility = System.Windows.Visibility.Visible;
                    }
                }
            }
        }

        void bt_Click(object sender, RoutedEventArgs e)
        {
            Button bt = sender as Button;
            if (bt.Background == Brushes.Gold)
            {
                bt.Background = Brushes.Gray;
                this.TrailUse[this.ButId[bt]] = false;
                foreach (Canvas cv in ButCav[bt])
                {
                    cv.Visibility = System.Windows.Visibility.Hidden;
                }
            }
            else
            {
                bt.Background = Brushes.Gold;
                this.TrailUse[this.ButId[bt]] = true;
                foreach (Canvas cv in ButCav[bt])
                {
                    cv.Visibility = System.Windows.Visibility.Visible;
                }
            }
        }
    }
}
