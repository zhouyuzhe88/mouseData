using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace MouseData
{
    class TimePoint
    {
        static Regex Line { get; set; }
        static DateTime LastTime { get; set; }

        public DateTime Time { get; set; }
        public Point Position { get; set; }

        static TimePoint()
        {
            string pattern = @"^(?<h>\d+):(?<m>\d+):(?<s>\d+)\.(?<ms>\d+)\s+(?<x>\d+)\s+(?<y>\d+)$";
            Line = new Regex(pattern);
        }

        public static bool IsPoint(string s)
        {
            return Line.IsMatch(s);
        }

        public TimePoint(string input)
        {
            var matche = Line.Matches(input)[0].Groups;
            int x = Convert.ToInt32(matche["x"].Value);
            int y = Convert.ToInt32(matche["y"].Value);
            int h = Convert.ToInt32(matche["h"].Value);
            int m = Convert.ToInt32(matche["m"].Value);
            int s = Convert.ToInt32(matche["s"].Value);
            int ms = Convert.ToInt32(matche["ms"].Value);
            this.Time = new DateTime(2000, 1, 1, h, m, s, ms).AddHours(8);
            if (this.Time.Hour >= 12)
            {
                this.Time = this.Time.AddHours(-12);
            }
            if (this.Time < LastTime)
            {
                this.Time = this.Time.AddHours(12);
            }
            LastTime = this.Time;
            this.Position = new Point(x, y);
        } 
    }
}
