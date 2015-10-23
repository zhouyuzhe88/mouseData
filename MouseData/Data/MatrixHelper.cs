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
    class MatrixHelper
    {
        public Experiment Exp { get; set; }

        public List<Shape>[] Elementes { get; set; }
        public StackPanel[] Titles { get; set; }
        public double[] MaxAvgWave { get; set; }
        public int ChannelCnt { get; set; }
        public int CavHeight { get; set; }
        public int CavWidth { get; set; }
        private int RowCnt { get; set; }
        private int ColCnt { get; set; }
        private bool MaxOnly { get; set; }

        private int XStart { get; set; }
        private int XEnd { get; set; }
        private int YStart { get; set; }
        private int YEnd { get; set; }

        /// <summary>
        /// chanel, row, col
        /// </summary>
        private int[][][] PointCnt { get; set; }

        private int[][][] WaveSum { get; set; }

        private int MLen { get; set; }

        public MatrixHelper(Experiment exp, bool maxOnly = true)
        {
            this.Exp = exp;
            this.MaxOnly = maxOnly;
            Init();
            Calculate();
            AddElement();
        }

        private void AddElement()
        {
            for (int ch = 0; ch < ChannelCnt; ++ch)
            {
                for (int row = 0; row < RowCnt; ++row)
                {
                    for (int col = 0; col < ColCnt; ++col)
                    {
                        Shape s = new Rectangle();
                        s.Fill = CalColor(ch, row, col);
                        /*VisualBrush b = new VisualBrush();
                        Label l = new Label();
                        l.Background = CalColor(ch, row, col);
                        l.Content = PointCnt[ch][row][col];
                        l.Foreground = Brushes.White;
                        b.Visual = l;
                        s.Fill = b;*/
                        s.Width = s.Height = MLen;
                        s.Margin = new Thickness(col * MLen + 5, row * MLen + 5 + 30 * Parameters.Rate, 0, 0);
                        Elementes[ch].Add(s);
                    }
                }
                Rectangle rec = new Rectangle();
                rec.Height = RowCnt * MLen + 10 + 30 * Parameters.Rate;
                rec.Width = ColCnt * MLen + 10;
                rec.Fill = Brushes.Transparent;
                rec.StrokeThickness = 2;
                rec.Stroke = Brushes.Black;
                Elementes[ch].Add(rec);
                Titles[ch] = DrawHelper.BuildTitle(Exp.ChannelTag[ch], MaxAvgWave[ch], 0.7);
            }
        }

        private Brush CalColor(int ch, int row, int col)
        {
            Brush brush;
            if (PointCnt[ch][row][col] == 0)
            {
                brush = Brushes.White;
            }
            else
            {
                double rate = (double)WaveSum[ch][row][col] / PointCnt[ch][row][col];
                int colorId;
                for (colorId = 0; colorId < Parameters.ColorRate.Count; ++colorId)
                {
                    if (rate >= Parameters.ColorRate[colorId])
                    {
                        break;
                    }
                }
                brush = Parameters.ColorList[colorId];
            }
            return brush;
        }

        private void Calculate()
        {
            foreach (Trail tr in Exp.Trils)
            {
                if (MaxOnly && tr.FoodCnt != 4)
                {
                    continue;
                }
                foreach (Segment seg in tr.Segments)
                {
                    Point p = seg.CenterPoint();
                    int row = ((int)(p.Y - YStart)) / Parameters.MLen;
                    int col = ((int)(p.X - XStart)) / Parameters.MLen;
                    if (0 <= row && row < RowCnt && 0 <= col && col < ColCnt)
                    {
                        for (int ch = 0; ch < ChannelCnt; ++ch)
                        {
                            ++PointCnt[ch][row][col];
                            WaveSum[ch][row][col] += seg.WaveList[ch].Count * Parameters.SegmentLength / (int)seg.Length;
                        }
                    }
                }
            }
            for (int ch = 0; ch < ChannelCnt; ++ch)
            {
                for (int row = 0; row < RowCnt; ++row)
                {
                    for (int col = 0; col < ColCnt; ++col)
                    {
                        if (PointCnt[ch][row][col] != 0)
                        {
                            MaxAvgWave[ch] = Math.Max(MaxAvgWave[ch], (double)WaveSum[ch][row][col] / PointCnt[ch][row][col]);
                        }
                    }
                }
            }
        }

        private void Init()
        {
            XStart = Parameters.XStart;
            XEnd = Parameters.XEnd;
            YStart = Parameters.YStart;
            YEnd = Parameters.YEnd;
            CavHeight = (int)((YEnd - YStart + 30) * Parameters.Rate);
            CavWidth = (int)((XEnd - XStart) * Parameters.Rate);
            MLen = (int)(Parameters.MLen * Parameters.Rate);
            ColCnt = (XEnd - XStart) / Parameters.MLen;
            RowCnt = (YEnd - YStart) / Parameters.MLen;
            ChannelCnt = Exp.ChannelCnt;

            MaxAvgWave = new double[ChannelCnt];
            PointCnt = new int[ChannelCnt][][];
            WaveSum = new int[ChannelCnt][][];
            for (int i = 0; i < ChannelCnt; ++i)
            {
                PointCnt[i] = new int[RowCnt][];
                WaveSum[i] = new int[RowCnt][];
                for (int j = 0; j < RowCnt; ++j)
                {
                    PointCnt[i][j] = new int[ColCnt];
                    WaveSum[i][j] = new int[ColCnt];
                }
            }

            Elementes = new List<Shape>[ChannelCnt];
            for (int i = 0; i < ChannelCnt; ++i)
            {
                Elementes[i] = new List<Shape>();
            }
            Titles = new StackPanel[ChannelCnt];
        }

    }
}
