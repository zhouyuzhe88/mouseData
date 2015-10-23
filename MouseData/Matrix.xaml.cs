using System;
using System.Collections.Generic;
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
    /// Matrix.xaml 的交互逻辑
    /// </summary>
    public partial class Matrix : Window
    {
        MatrixHelper Helper { get; set; }
        internal Matrix(MatrixHelper h)
        {
            InitializeComponent();
            Helper = h;
            this.Title = "Experiment " + h.Exp.Id;
            int rowCnt = (Helper.ChannelCnt / 4 + (Helper.ChannelCnt % 4 == 0 ? 0 : 1));
            int colCnt = Helper.ChannelCnt <= 4 ? Helper.ChannelCnt : 4;
            this.MainCanvas.Width = (h.CavWidth + 20) * colCnt + 20;
            this.MainCanvas.Height = (h.CavHeight + 20) * rowCnt + 20;
            FixMargin();
            foreach (var eles in h.Elementes)
            {
                foreach (var s in eles)
                {
                    MainCanvas.Children.Add(s);
                }
            }
            foreach (var ele in h.Titles)
            {
                MainCanvas.Children.Add(ele);
            }
        }

        void FixMargin()
        {
            for (int channelId = 0; channelId < Helper.ChannelCnt; ++channelId)
            {
                int dTop = 20 + channelId / 4 * (Helper.CavHeight + 20), dLeft = 20 + channelId % 4 * (Helper.CavWidth + 20);
                Helper.Titles[channelId].Margin = new Thickness(dLeft, dTop, 0, 0);
                foreach (Shape s in Helper.Elementes[channelId])
                {
                    s.Margin = new Thickness(s.Margin.Left + dLeft, s.Margin.Top + dTop, 0, 0);
                }
            }
        }
    }
}
