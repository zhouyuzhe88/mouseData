using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MouseData
{
    class Experiment
    {
        public List<Trail> Trils { get; set; }
        public double MaxX { get; set; }
        public double MaxY { get; set; }
        public int Id { get; set; }
        public string Tag { get; set; }
        public int ChannelCnt { get; set; }
        public int[] MaxWaveCnt { get; set; }
        public List<string> ChannelTag { get; set; }

        public Experiment()
        {
            this.Trils = new List<Trail>();
        }

        public int GetColorId(int channelId, double WaveCnt)
        {
            if (MaxWaveCnt[channelId] == 0)
            {
                return Parameters.ColorRate.Count - 1;
            }
            double rate = WaveCnt / (double)MaxWaveCnt[channelId];
            int i;
            for (i = 0; i < Parameters.ColorRate.Count; ++i)
            {
                if (rate >= Parameters.ColorRate[i])
                {
                    return i;
                }
            }
            return i;
        }
    }
}
