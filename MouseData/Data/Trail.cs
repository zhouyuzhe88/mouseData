using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace MouseData
{
    class Trail
    {
        static Regex Line { get; set; }
        static DateTime LastTime { get; set; }

        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<Segment> Segments { get; set; }

        static Trail()
        {
            string pattern = @"t\s+(?<start>\d+)\s+Trail\s+(?<id>\d+)[\s\S]+?(?<end>\d+)[\s\S]+(?<dt>\d+)[\s\S]+p[\s\S]+\s+(?<h>\d+):(?<m>\d+):(?<s>\d+)\.(?<ms>\d+)\]";
            Line = new Regex(pattern);
        }

        public static bool IsATrail(string s)
        {
            return Line.IsMatch(s);
        }

        public Trail(string input)
        {
            var matche = Line.Matches(input)[0].Groups;
            this.Id = Convert.ToInt32(matche["id"].Value);
            int start = Convert.ToInt32(matche["start"].Value);
            int end = Convert.ToInt32(matche["end"].Value);
            int dt = Convert.ToInt32(matche["dt"].Value);
            int h = Convert.ToInt32(matche["h"].Value);
            int m = Convert.ToInt32(matche["m"].Value);
            int s = Convert.ToInt32(matche["s"].Value);
            int ms = Convert.ToInt32(matche["ms"].Value);
            DateTime baseTime = new DateTime(2000, 1, 1, h, m, s, ms);
            while (baseTime < LastTime)
            {
                baseTime = baseTime.AddHours(12);
            }
            LastTime = baseTime;
            this.EndTime = baseTime.AddMilliseconds(-dt * Parameters.FeedTime);
            this.StartTime = this.EndTime.AddMilliseconds(start - end);
            this.Segments = new List<Segment>();
        } 
    }
}
