﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace MouseData.Data
{
    class MatrixHelper
    {
        public Experiment Exp { get; set; }
        public double[] MaxAvgWave { get; set; }

        public List<Shape>[] Elementes { get; set; }

        private int RowCnt { get; set; }
        private int ColCnt { get; set; }
        private int ChanelCnt { get; set; }
        private int XStart { get; set; }
        private int XEnd { get; set; }
        private int YStart { get; set; }
        private int YEnd { get; set; }

        /// <summary>
        /// chanel, row, col
        /// </summary>
        private int[][][] PointCnt { get; set; }

        private int[][][] WaveSum { get; set; }

        public MatrixHelper(Experiment exp)
        {
            this.Exp = exp;
            Init();
            Calculate();
            for (int ch = 0; ch < ChanelCnt; ++ch)
            {
                for (int row = 0; row < RowCnt; ++row)
                {
                    for (int col = 0; col < ColCnt; ++col)
                    {
                        double rate = (double)WaveSum[ch][row][col] / PointCnt[ch][row][col];
                    }
                }
            }
        }

        private void Calculate()
        {
            foreach (Trail tr in Exp.Trils)
            {
                foreach (Segment seg in tr.Segments)
                {
                    Point p = seg.CenterPoint();
                    int row = ((int)(p.Y - YStart)) / Parameters.MLen;
                    int col = ((int)(p.X - XStart)) / Parameters.MLen;
                    if (0 <= row && row < RowCnt && 0 <= col && col < ColCnt)
                    {
                        for (int ch = 0; ch < ChanelCnt; ++ch)
                        {
                            ++PointCnt[ch][row][col];
                            WaveSum[ch][row][col] += seg.WaveList[ch].Count / (int)seg.Length * Parameters.SegmentLength;
                        }
                    }
                }
            }
            for (int ch = 0; ch < ChanelCnt; ++ch)
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
            ColCnt = (XEnd - XStart) / Parameters.MLen;
            YStart = Parameters.YStart;
            YEnd = Parameters.YEnd;
            RowCnt = (YEnd - YStart) / Parameters.MLen;
            ChanelCnt = Exp.ChannelCnt;

            MaxAvgWave = new double[ChanelCnt];
            PointCnt = new int[ChanelCnt][][];
            WaveSum = new int[ChanelCnt][][];
            for (int i = 0; i < ChanelCnt; ++i)
            {
                PointCnt[i] = new int[RowCnt][];
                WaveSum[i] = new int[RowCnt][];
                for (int j = 0; j < RowCnt; ++j)
                {
                    PointCnt[i][j] = new int[ColCnt];
                    WaveSum[i][j] = new int[ColCnt];
                }
            }

            Elementes = new List<Shape>[ChanelCnt];
            for (int i = 0; i < ChanelCnt; ++i)
            {
                Elementes[i] = new List<Shape>();
            }
        }
    }
}
