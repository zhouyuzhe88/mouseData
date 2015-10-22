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
            foreach (var s in h.Elementes[0])
            {
                MainCanvas.Children.Add(s);
            }
        }
    }
}
