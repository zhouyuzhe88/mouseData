using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MouseData
{
    /// <summary>
    /// Draw.xaml 的交互逻辑
    /// </summary>
    partial class Draw : Window
    {
        const int Gap = 20;
        Dictionary<Button, int> ButId { get; set; }
        DrawHelper Helper { get; set; }
        Experiment Exp { get; set; }
        Button BtLeft { get; set; }
        Button BtRight { get; set; }

        internal Draw(DrawHelper h)
        {
            InitializeComponent();
            this.Helper = h;
            this.Exp = h.Exp;
            this.Title = "Experiment " + Exp.Id;
            this.MainCanvas.Width = (h.CavWidth + 20) * h.ColCnt + 20;
            this.MainCanvas.Height = (h.CavHeight + 20) * h.RowCnt + 20;
            this.ButId = new Dictionary<Button, int>();
            AddButtons();
            FixMargin();
            AddPoints(this.MainCanvas);
            AddAxisAndTitle(this.MainCanvas);
        }

        void AddButtons()
        {
            Button BtInfo = BuildBaseButton("Task Info", BtInfo_Click, Brushes.HotPink);
            Button BtAnalysis = BuildBaseButton("Export Data", BtAnalysis_Click, Brushes.HotPink);
            Button BtExport = BuildBaseButton("Capture", BtExport_Click, Brushes.HotPink);
            AddSepatator();
            Button BtAll = BuildBaseButton("ALL", BtAll_Click, Brushes.Orange);
            BtLeft = BuildBaseButton("Left", BtLR_Click, Brushes.Orange);
            BtRight = BuildBaseButton("Right", BtLR_Click, Brushes.Orange);
            AddSepatator(5);
            for (int i = 0; i < Exp.Trils.Count; ++i)
            {
                Button bt = BuildBaseButton("Trail" + Exp.Trils[i].Id, bt_Click);
                ButId[bt] = i;
            }
        }

        private void AddSepatator(int height = 10)
        {

            Rectangle separator = new Rectangle();
            separator.Width = 80;
            separator.Height = height;
            this.filter.Children.Add(separator);
        }

        void FixMargin()
        {
            for (int channelId = 0; channelId < Helper.ChannelCnt; ++channelId)
            {
                int dTop = 20 + channelId / 4 * (Helper.CavHeight + 20), dLeft = 20 + channelId % 4 * (Helper.CavWidth + 20);
                Helper.Titles[channelId].Margin = new Thickness(dLeft, dTop, 0, 0);
                foreach (Shape s in Helper.Axises[channelId])
                {
                    s.Margin = new Thickness(s.Margin.Left + dLeft, s.Margin.Top + dTop, 0, 0);
                }
                for (int colorId = 0; colorId < Helper.ColorCnt; ++colorId)
                {
                    for (int trailId = 0; trailId < Helper.TrailCnt; ++trailId)
                    {
                        foreach (Shape s in Helper.Elementes[colorId][channelId][trailId])
                        {
                            s.Margin = new Thickness(s.Margin.Left + dLeft, s.Margin.Top + dTop, 0, 0);
                        }
                    }
                }
            }
        }

        void AddPoints(Canvas cv)
        {
            for (int colorId = Helper.ColorCnt - 1; colorId >= 0; --colorId)
            {
                for (int channelId = 0; channelId < Helper.ChannelCnt; ++channelId)
                {
                    for (int trailId = 0; trailId < Helper.TrailCnt; ++trailId)
                    {
                        foreach (Shape s in Helper.Elementes[colorId][channelId][trailId])
                        {
                            cv.Children.Add(s);
                        }
                    }
                }
            }
        }

        void AddAxisAndTitle(Canvas cv)
        {
            for (int channelId = 0; channelId < Helper.ChannelCnt; ++channelId)
            {
                foreach (Shape s in Helper.Axises[channelId])
                {
                    cv.Children.Add(s);
                }
                cv.Children.Add(Helper.Titles[channelId]);
            }
        }

        Button BuildBaseButton(string content, RoutedEventHandler action, SolidColorBrush color = null)
        {
            Button bt = new Button();
            bt.Focusable = false;
            bt.Background = (color == null ? Brushes.Gold : color);
            bt.Content = content;
            bt.Height = 40;
            bt.Width = 80;
            bt.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            bt.Click += action;
            this.filter.Children.Add(bt);
            return bt;
        }

        void BtAnalysis_Click(object sender, RoutedEventArgs e)
        {
            this.Exp.AnalysisSPK(this.Helper.TrailUse);
            System.Windows.Forms.MessageBox.Show("Export Data Done");
        }

        void BtInfo_Click(object sender, RoutedEventArgs e)
        {
            string info = Exp.GetInfo();
            new SimpleOutput(this.Title + " Task Info", info).Show();
        }

        void BtExport_Click(object sender, RoutedEventArgs e)
        {
            RenderTargetBitmap bmp = new RenderTargetBitmap((int)MainCanvas.Width, (int)MainCanvas.Height, 96, 96, PixelFormats.Pbgra32);
            bmp.Render(MainCanvas);
            string file = string.Format("{0}-{1}-{2}-{3}.png", this.Exp.Tag, this.Exp.Id, DateTime.Now.Hour, DateTime.Now.Minute);
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmp));
            using (Stream stm = File.Create(file))
            {
                encoder.Save(stm);
            }
            System.Windows.Forms.MessageBox.Show("Capture Done");
        }

        void BtLR_Click(object sender, RoutedEventArgs e)
        {
            Button btLR = sender as Button;
            string content = btLR.Content.ToString();
            Brush target = btLR.Background == Brushes.Orange ? Brushes.Gold : Brushes.Gray;
            btLR.Background = btLR.Background == Brushes.Orange ? Brushes.LightGray : Brushes.Orange;
            ButId.Keys.ToList().Where(x => x.Background == target && Exp.Trils[ButId[x]].LR == content).ToList().ForEach(x => bt_Click(x, null));
        }

        void BtAll_Click(object sender, RoutedEventArgs e)
        {
            Button btAll = sender as Button;
            Brush target = btAll.Background == Brushes.Orange ? Brushes.Gold : Brushes.Gray;
            BtLeft.Background = BtRight.Background = btAll.Background = btAll.Background == Brushes.Orange ? Brushes.LightGray : Brushes.Orange;
            ButId.Keys.ToList().Where(x => x.Background == target).ToList().ForEach(x => bt_Click(x, null));
        }

        void bt_Click(object sender, RoutedEventArgs e)
        {
            Button bt = sender as Button;
            int trailId = ButId[bt];
            Brush color = bt.Background == Brushes.Gold ? Brushes.Gray : Brushes.Gold;
            bool use = bt.Background != Brushes.Gold;
            System.Windows.Visibility vis = bt.Background == Brushes.Gold ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible;
            bt.Background = color;
            Helper.TrailUse[trailId] = use;
            foreach (Shape s in Helper.TrailElements[trailId])
            {
                s.Visibility = vis;
            }
        }
    }
}