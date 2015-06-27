using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MouseData
{
    class Experiment
    {
        static Regex TaskReg { get; set; }
        public List<Trail> Trils { get; set; }
        public double MaxX { get; set; }
        public double MaxY { get; set; }
        public int Id { get; set; }
        public string Tag { get; set; }
        public string Task { get; set; }
        public int ChannelCnt { get; set; }
        public int[] MaxWaveCnt { get; set; }
        public List<string> ChannelTag { get; set; }

        static Experiment()
        {
            string pattern = @"Config=\s*(?<task>\S+)\[";
            TaskReg = new Regex(pattern);
        }

        public Experiment(string input)
        {
            this.Trils = new List<Trail>();
            this.Task = TaskReg.IsMatch(input) ? TaskReg.Matches(input)[0].Groups["task"].Value : "";
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

        public string GetInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("DATE\tRAT\tTASK\tEXP\tTRIAL\tORIENT\tREWA\tTIME");
            sb.AppendLine();
            string[] dar = this.Tag.Split('-');
            foreach (Trail t in Trils)
            {
                double time = (t.EndTime - t.StartTime).TotalSeconds;
                sb.Append(string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}", dar[0], dar[1], Task, Id, t.Id, t.LR, t.FoodCnt, time));
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}
