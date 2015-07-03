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
        public double MinX { get; set; }
        public double MaxY { get; set; }
        public int Id { get; set; }
        private string tag;
        public string Tag
        {
            get
            {
                return tag;
            }
            set
            {
                tag = value;
                string[] split = tag.Split('-');
                if (split.Length == 2)
                {
                    Date = split[0];
                    Rat = split[1];
                }
                else
                {
                    Date = Rat = "";
                }
            }
        }
        public string Date { get; set; }
        public string Rat { get; set; }
        public string Task { get; set; }
        public int ChannelCnt { get; set; }
        public int[] MaxWaveCnt { get; set; }
        public List<string> ChannelTag { get; set; }

        static Experiment()
        {
            string pattern = @"[C|c]onfig\s*=\s*(?<task>\S+)\s*\[";
            TaskReg = new Regex(pattern);
        }

        public Experiment(string input)
        {
            this.Trils = new List<Trail>();
            this.Task = TaskReg.IsMatch(input) ? TaskReg.Matches(input)[0].Groups["task"].Value : "";
            MinX = 0x7fffffff;
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

        public void AnalysisSPK(StringBuilder sb)
        {
            for (int i = 0; i < this.Trils.Count; ++i)
            {
                this.Trils[i].Analsys(sb);
            }
        }

        public string GetInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("DATE\tRAT\tTASK\tEXP\tTRIAL\tORIENT\tREWA\tTIME");
            sb.AppendLine();
            foreach (Trail t in Trils)
            {
                double time = (t.EndTime - t.StartTime).TotalSeconds;
                sb.AppendFormat("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}", Date, Rat, Task, Id, t.Id, t.LR, t.FoodCnt, time);
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}
