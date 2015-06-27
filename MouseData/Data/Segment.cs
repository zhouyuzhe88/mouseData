using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MouseData
{
    class Segment
    {

        public List<TimePoint> Points { get; set; }
        public List<DateTime>[] WaveList { get; set; }
        public double Length { get; set; }
        public Trail Tra { get; set; }

        public Segment(int ChannelCnt)
        {
            Points = new List<TimePoint>();
            WaveList = new List<DateTime>[ChannelCnt];
            for (int i = 0; i < ChannelCnt; ++i)
            {
                WaveList[i] = new List<DateTime>();
            }
        }

        public Point CenterPoint()
        {
            if (Points.Count == 0)
            {
                return new Point(0, 0);
            }
            double x = 0, y = 0;
            Points.ForEach(p => { x += p.Position.X; y += p.Position.Y; });
            return new Point(x / Points.Count, y / Points.Count);
        }

        public double GetXPos()
        {
            double x = CenterPoint().X;
            return (x - Tra.Exp.MinX) / (Tra.Exp.MaxX - Tra.Exp.MinX);
        }
    }
}
