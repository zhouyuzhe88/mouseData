using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MouseData
{
    class Analyser
    {
        string[] MoveData { get; set; }
        string[] BehaveData { get; set; }
        string[] WaveData { get; set; }
        
        public List<Experiment> Experiments { get; set; }
        List<List<DateTime>> ChannelTime { get; set; }
        List<string> ChannelTag { get; set; }
        int ChannelCnt { get; set; }

        public Analyser(string movePath, string behavePath, string wavePath)
        {
            this.MoveData = File.ReadAllLines(movePath);
            this.BehaveData = File.ReadAllLines(behavePath);
            this.WaveData = File.ReadAllLines(wavePath);
            this.Experiments = new List<Experiment>();
            this.ChannelTime = new List<List<DateTime>>();
            this.ChannelTag = new List<string>();
        }

        private void GenerateSegment()
        {
            int pi = 0;
            int[] ti = new int[ChannelCnt];
            foreach(Experiment exp in Experiments)
            {
                exp.ChannelCnt = this.ChannelCnt;
                exp.MaxWaveCnt = new int[this.ChannelCnt];
                exp.ChannelTag = this.ChannelTag;
                foreach (Trail t in exp.Trils)
                {
                    DateTime start, end;
                    for (start = t.StartTime; start < t.EndTime; start = end)
                    {
                        end = start.AddMilliseconds(Parameters.SegmentLength) < t.EndTime ? start.AddMilliseconds(Parameters.SegmentLength) : t.EndTime;
                        Segment seg = new Segment(this.ChannelCnt);
                        pi = InsertPoint(pi, exp, start, end, seg);
                        InssertWave(ti, exp, start, end, seg);
                        t.Segments.Add(seg);
                        seg.Length = (end - start).TotalMilliseconds;
                    }
                }
            }
        }

        private void InssertWave(int[] ti, Experiment exp, DateTime start, DateTime end, Segment seg)
        {
            for (int i = 0; i < this.ChannelCnt; ++i)
            {
                for (; ti[i] < ChannelTime[i].Count; ++ti[i])
                {
                    DateTime tmp = ChannelTime[i][ti[i]];
                    if (tmp >= start && tmp <= end)
                    {
                        seg.WaveList[i].Add(tmp);
                    }
                    else if (tmp > end)
                    {
                        break;
                    }
                }
                exp.MaxWaveCnt[i] = Math.Max(exp.MaxWaveCnt[i], seg.WaveList[i].Count);
            }
        }

        private int InsertPoint(int pi, Experiment exp, DateTime start, DateTime end, Segment seg)
        {
            for (; pi < MoveData.Length; ++pi)
            {
                if (!TimePoint.IsPoint(MoveData[pi]))
                {
                    continue;
                }
                TimePoint tp = new TimePoint(MoveData[pi]);
                if (tp.Time >= start && tp.Time <= end)
                {
                    if (tp.Position.X != 0 && tp.Position.Y != 0)
                    {
                        seg.Points.Add(tp);
                        exp.MaxX = Math.Max(exp.MaxX, tp.Position.X);
                        exp.MaxY = Math.Max(exp.MaxY, tp.Position.Y);
                    }
                }
                else if (tp.Time > end)
                {
                    break;
                }
            }
            return pi;
        }

        private void ParseWave()
        {
            List<List<string>> waveMeta = new List<List<string>>();
            foreach (string s in this.WaveData)
            {
                waveMeta.Add(s.Split('\t').ToList());
            }
            int timeInd = waveMeta[0].FindIndex(x => x == Parameters.TimeFlag);
            if (timeInd == -1)
            {
                return;
            }
            double startTime = Convert.ToDouble(waveMeta[1][timeInd]);
            string pattern = "SPK(\\d*)a";
            Regex r = new Regex(pattern);
            MatchCollection matches = r.Matches(WaveData[0]);
            ChannelCnt = matches.Count;
            if (Experiments.Count == 0 || Experiments[0].Trils.Count == 0)
            {
                return;
            }
            DateTime baseTime = Experiments[0].Trils[0].StartTime.AddSeconds(-startTime);
            for (int i = 0; i < ChannelCnt; ++i)
            {
                ChannelTag.Add(waveMeta[0][i]);
                ChannelTime.Add(new List<DateTime>());
            }
            for (int i = 1; i < waveMeta.Count; ++i)
            {
                for (int j = 0; j < ChannelCnt; ++j)
                {
                    if (waveMeta[i][j] != "")
                    {
                        ChannelTime[j].Add(baseTime.AddSeconds(Convert.ToDouble(waveMeta[i][j])));
                    }
                }
            }
        }

        private void ParseTrail()
        {
            Experiment now = null;
            int id = 1;
            foreach (string data in BehaveData)
            {
                if (data.StartsWith("----- EXPERIMENT START -----"))
                {
                    if (now != null)
                    {
                        Experiments.Add(now);
                    }
                    now = new Experiment() { Id = id++ };
                }
                else if (Trail.IsATrail(data))
                {
                    Trail t = new Trail(data);
                    now.Trils.Add(t);
                }
            }
            if (now != null)
            {
                Experiments.Add(now);
            }
        }

        public void Work()
        {
            ParseTrail();
            ParseWave();
            GenerateSegment();
        }

    }
}
