using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MouseData
{
    class Segment
    {
        public List<TimePoint> Points { get; set; }
        public List<DateTime>[] WaveList { get; set; }
        public double Length { get; set; }

        public Segment(int ChannelCnt)
        {
            Points = new List<TimePoint>();
            WaveList = new List<DateTime>[ChannelCnt];
            for (int i = 0; i < ChannelCnt; ++i)
            {
                WaveList[i] = new List<DateTime>();
            }
        }
    }
}
