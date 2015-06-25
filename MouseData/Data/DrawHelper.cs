using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MouseData
{
    class DrawHelper
    {
        public Experiment Exp { get; set; }
        public int CavHeight { get; set; }
        public int CavWidth { get; set; }

        public int TrailCnt { get; set; }
        public int ChannelCnt { get; set; }
        public int ColorCnt { get; set; }
        public int RowCnt { get; set; }
        public int ColCnt { get; set; }

        /// <summary>
        /// color, channel, trail
        /// </summary>
        public List<Shape>[][][] Elementes { get; set; }
        public List<Shape>[] TrailElements { get; set; }
        public List<Shape>[] Axises { get; set; }
        public StackPanel[] Titles { get; set; }
        public bool[] TrailUse { get; set; }

        public DrawHelper(Experiment exp)
        {
            this.Exp = exp;
            Init();
            AddPoints();
            AddTitleAndAxis();
        }

        private void Init()
        {
            this.TrailCnt = Exp.Trils.Count;
            this.ChannelCnt = Exp.ChannelCnt;
            this.ColorCnt = Parameters.ColorRate.Count;
            this.RowCnt = (this.ChannelCnt / 4 + (this.ChannelCnt % 4 == 0 ? 0 : 1));
            this.ColCnt = this.ChannelCnt <= 4 ? this.ChannelCnt : 4;
            this.CavWidth = (int)(((int)Exp.MaxX / 50 + ((int)Exp.MaxX % 50 == 0 ? 0 : 1)) * 50 * Parameters.XRate + 50);
            this.CavHeight = (int)(((int)Exp.MaxY / 50 + ((int)Exp.MaxY % 50 == 0 ? 0 : 1)) * 50 * Parameters.YRate + 50);
            Elementes = new List<Shape>[ColorCnt][][];
            for (int i = 0; i < ColorCnt; ++i)
            {
                Elementes[i] = new List<Shape>[ChannelCnt][];
                for (int j = 0; j < ChannelCnt; ++j)
                {
                    Elementes[i][j] = new List<Shape>[TrailCnt];
                    for (int k = 0; k < TrailCnt; ++k)
                    {
                        Elementes[i][j][k] = new List<Shape>();
                    }
                }
            }
            TrailElements = new List<Shape>[TrailCnt];
            TrailUse = new bool[TrailCnt];
            for (int i = 0; i < TrailCnt; ++i)
            {
                TrailElements[i] = new List<Shape>();
                TrailUse[i] = true;
            }
            Axises = new List<Shape>[ChannelCnt];
            Titles = new StackPanel[ChannelCnt];
        }

        private void AddPoints()
        {
            for (int trailId = 0; trailId < TrailCnt; ++trailId)
            {
                Trail t = Exp.Trils[trailId];
                foreach (Segment seg in t.Segments)
                {
                    if (seg.Points.Count == 0 || seg.Length < Parameters.SegmentLength * Parameters.SegmentRate)
                    {
                        continue;
                    }
                    for (int channelId = 0; channelId < ChannelCnt; ++channelId)
                    {
                        int colorId = Exp.GetColorId(channelId, seg.WaveList[channelId].Count / seg.Length * Parameters.SegmentLength);
                        Shape s = BuildShape(seg, colorId);
                        Elementes[colorId][channelId][trailId].Add(s);
                        TrailElements[trailId].Add(s);
                    }
                }
            }
        }

        private void AddTitleAndAxis()
        {
            for (int i = 0; i < ChannelCnt; ++i)
            {
                Titles[i] = BuildTitle(i);
                Axises[i] = BuildAxis();
            }
        }

        private Shape BuildShape(Segment seg, int colorId)
        {
            Shape shape = (Shape)Activator.CreateInstance(Parameters.ShapeType);
            double radio = Parameters.ColorRadio[colorId];
            Point c = seg.CenterPoint();
            double x = c.X * Parameters.XRate;
            double y = (this.CavHeight + Exp.MaxY * Parameters.YRate) / 2 - c.Y * Parameters.YRate;
            shape.Margin = new Thickness(x - radio, CavHeight - y - radio, 0, 0);
            shape.Fill = Parameters.ColorList[colorId];
            shape.Width = shape.Height = radio * 2;
            return shape;
        }

        private List<Shape> BuildAxis()
        {
            List<Shape> axis = new List<Shape>();
            Line lineX = new Line();
            lineX.X1 = 0;
            lineX.Y1 = CavHeight;
            lineX.X2 = CavWidth;
            lineX.Y2 = CavHeight;
            lineX.Stroke = Brushes.DarkGray;
            lineX.StrokeThickness = 2;
            axis.Add(lineX);
            Line lineY = new Line();
            lineY.X1 = 0;
            lineY.Y1 = CavHeight;
            lineY.X2 = 0;
            lineY.Y2 = 0;
            lineY.Stroke = Brushes.DarkGray;
            lineY.StrokeThickness = 2;
            axis.Add(lineY);
            for (double i = 0; i < CavWidth; i += 50 * Parameters.XRate)
            {
                Ellipse e = new Ellipse();
                double r = 1.5;
                e.Width = e.Height = 2 * r;
                e.Margin = new Thickness(i - r, CavHeight - r, 0, 0);
                e.Fill = Brushes.Black;
                axis.Add(e);
            }
            for (double i = 0; i < CavHeight; i += 50 * Parameters.YRate)
            {
                Ellipse e = new Ellipse();
                double r = 1.5;
                e.Width = e.Height = 2 * r;
                e.Margin = new Thickness(-r, CavHeight - i - r, 0, 0);
                e.Fill = Brushes.Black;
                axis.Add(e);
            }
            return axis;
        }

        private StackPanel BuildTitle(int channelId)
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
                t.Text = " " + (int)Math.Round(fLs[i]) + " - " + (int)Math.Round(fLs[i + 1]) + "HZ     ";
                t.FontSize *= Math.Min(1.2, Parameters.XRate * 1.4);
                colors.Children.Add(t);
            }
            res.Children.Add(colors);
            return res;
        }
    }
}