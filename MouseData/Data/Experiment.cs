using System;
using System.Collections.Generic;
using System.IO;
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

        public void AnalysisSPK(bool[] trailUse)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Trail\tL/R\tArea\tFood\tChannel\tSPK");
            sb.AppendLine();
            for (int i = 0; i < this.Trils.Count; ++i)
            {
                if (trailUse[i])
                {
                    sb.Append(this.Trils[i].Analsys());
                }
            }
            string res = sb.ToString();
            string fileName = string.Format("{0}-{1}-{2}-{3}-analysis.txt", this.Tag, this.Id, DateTime.Now.Hour, DateTime.Now.Minute);
            File.WriteAllText(fileName, res, Encoding.Default);
        }
    }
}
