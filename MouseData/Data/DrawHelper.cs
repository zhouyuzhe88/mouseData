using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace MouseData
{
    class DrawHelper
    {
        Experiment Exp { get; set; }
        int CavHeight { get; set; }
        int CavWidth { get; set; }

        int TrailCnt { get; set; }
        int ChannelCnt { get; set; }
        int ColorCnt { get; set; }

        /// <summary>
        /// color, channel, trail
        /// </summary>
        public List<Shape>[][][] Elementes { get; set; }
        public List<Shape>[] TrailElements { get; set; }
        public bool[] TrailUse { get; set; }

        public DrawHelper(Experiment exp)
        {
            this.Exp = exp;
            Init();
        }

        private void Init()
        {
            this.TrailCnt = Exp.Trils.Count;
            this.ChannelCnt = Exp.ChannelCnt;
            this.ColorCnt = Parameters.ColorRate.Count;
            // TODO: rm ((int)Exp.Max % 50 == 0 ? 0 : 1)) * 50
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

        private Shape BuildShape(Segment seg, int colorId)
        {
            Shape shape = (Shape)Activator.CreateInstance(Parameters.ShapeType);
            double radio = Parameters.ColorRadio[colorId];
            double x = 0, y = 0;
            seg.Points.ForEach(p => { x += p.Position.X; y += p.Position.Y; });
            x = x * Parameters.XRate / seg.Points.Count;
            y = (this.CavHeight + Exp.MaxY * Parameters.YRate) / 2 - y / seg.Points.Count * Parameters.YRate;
            shape.Margin = new Thickness(x - radio, CavHeight - y - radio, 0, 0);
            shape.Fill = Parameters.ColorList[colorId];
            shape.Width = shape.Height = radio * 2;
            return shape;
        }
    }
}
