using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MouseData
{
    class Parameters
    {
        public static readonly string TimeFlag;
        public static readonly int SegmentLength;
        public static readonly int FeedTime;
        public static readonly int Xseparate;
        public static readonly double SegmentRate;
        public static readonly double XRate;
        public static readonly double YRate;
        public static readonly List<double> ColorRate;
        public static readonly List<double> ColorRadio;
        public static readonly List<Brush> ColorList;
        public static readonly Type ShapeType;

        static Parameters()
        {
            var settings = System.Configuration.ConfigurationManager.AppSettings;
            TimeFlag = GetString("TimeFlag", "EVT03");
            SegmentRate = GetDouble("SegmentRate", 0.4);
            SegmentLength = GetInt("SegmentLength", 100);
            FeedTime = GetInt("FeedTime", 400);
            Xseparate = GetInt("Xseparate", 210);
            XRate = GetDouble("XRate", 1);
            YRate = GetDouble("YRate", 1);
            ColorRate = GetDoubleList("ColorRate", new List<double>() { 0.8, 0.6, 0.4, 0.2, 0 });
            ColorRadio = GetDoubleList("ColorRadio", new List<double>() { 3, 2, 2, 1, 1 });
            ColorList = GetColorList("ColorList");
            ShapeType = getShapeType("ShapeType", "Ellipse");
        }

        private static Type getShapeType(string tag, string def)
        {
            string t = GetString(tag, def);
            if (t.ToLower() == "rectangle")
            {
                return new System.Windows.Shapes.Rectangle().GetType();
            }
            return new System.Windows.Shapes.Ellipse().GetType();
        }

        private static List<Brush> GetColorList(string tag)
        {
            List<int> def = new List<int>() { 128, 255, 0, 0, 128, 255, 165, 0, 128, 255, 255, 0, 128, 0, 128, 0, 128, 0, 0, 255 };
            List<int> argb = GetIntList(tag, new List<int>());
            List<Brush> res = new List<Brush>();
            if (argb.Count == 0)
            {
                res.Add(Brushes.Crimson);
                res.Add(Brushes.Gold);
                res.Add(Brushes.YellowGreen);
                res.Add(Brushes.RoyalBlue);
                res.Add(Brushes.Navy);
                return res;
            }
            if (argb.Count % 4 != 0)
            {
                argb = def;
            }
            for (int i = 0; i < argb.Count; i += 4)
            {
                res.Add(new SolidColorBrush(Color.FromArgb((byte)(argb[i]), (byte)argb[i + 1], (byte)argb[i + 2], (byte)argb[i + 3])));
            }
            return res;
        }

        static string GetString(string tag, string def)
        {
            var settings = System.Configuration.ConfigurationManager.AppSettings;
            if (!string.IsNullOrWhiteSpace(settings[tag]))
            {
                return settings[tag];
            }
            else
            {
                return def;
            }
        }

        static double GetDouble(string tag, double def)
        {
            var settings = System.Configuration.ConfigurationManager.AppSettings;
            if (!string.IsNullOrWhiteSpace(settings[tag]))
            {
                try
                {
                    return Convert.ToDouble(settings[tag]);
                }
                catch
                {
                    return def;
                }
            }
            else
            {
                return def;
            }
        }

        static List<double> GetDoubleList(string tag, List<double> def)
        {
            var settings = System.Configuration.ConfigurationManager.AppSettings;
            if (!string.IsNullOrWhiteSpace(settings[tag]))
            {
                try
                {
                    List<double> res = new List<double>();
                    foreach (string s in settings[tag].Split(','))
                    {
                        res.Add(Convert.ToDouble(s));
                    }
                    return res;
                }
                catch
                {
                    return def;
                }
            }
            else
            {
                return def;
            }
        }

        static int GetInt(string tag, int def)
        {
            var settings = System.Configuration.ConfigurationManager.AppSettings;
            if (!string.IsNullOrWhiteSpace(settings[tag]))
            {
                try
                {
                    return Convert.ToInt32(settings[tag]);
                }
                catch
                {
                    return def;
                }
            }
            else
            {
                return def;
            }
        }

        static List<int> GetIntList(string tag, List<int> def)
        {
            var settings = System.Configuration.ConfigurationManager.AppSettings;
            if (!string.IsNullOrWhiteSpace(settings[tag]))
            {
                try
                {
                    List<int> res = new List<int>();
                    foreach (string s in settings[tag].Split(','))
                    {
                        res.Add(Convert.ToInt32(s));
                    }
                    return res;
                }
                catch
                {
                    return def;
                }
            }
            else
            {
                return def;
            }
        }

    }
}
