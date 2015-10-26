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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace MouseData
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private Analyser Analyser { get; set; }
        private Dictionary<Button, Experiment> ButExp { get; set; }
        private Dictionary<Button, CheckBox> ButCb { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            FillFileName(new DirectoryInfo(System.Environment.CurrentDirectory));
        }

        private void FillFileName(DirectoryInfo dir, bool fullName = false)
        {
            foreach (FileInfo file in dir.GetFiles())
            {
                if (file.Name.EndsWith("Position.txt"))
                {
                    this.MoveFileTb.Text = fullName ? file.FullName : file.Name;
                }
                else if (file.Name.EndsWith("NR.txt"))
                {
                    this.WaveFileTb.Text = fullName ? file.FullName : file.Name;
                }
                else if (file.Name.EndsWith(".txt") && !file.Name.EndsWith("analsys.txt"))
                {
                    this.BehaveFileTb.Text = fullName ? file.FullName : file.Name;
                }
            }
        }

        private void ChooseFile(TextBox tb)
        {
            System.Windows.Forms.OpenFileDialog choose = new System.Windows.Forms.OpenFileDialog();
            if (choose.ShowDialog() == System.Windows.Forms.DialogResult.OK && !string.IsNullOrEmpty(choose.FileName))
            {
                tb.Text = choose.FileName;
            }
        }

        private void MoveBt_Click(object sender, RoutedEventArgs e)
        {
            ChooseFile(this.MoveFileTb);
        }

        private void BehaveBt_Click(object sender, RoutedEventArgs e)
        {
            ChooseFile(this.BehaveFileTb);
        }

        private void WaveBt_Click(object sender, RoutedEventArgs e)
        {
            ChooseFile(this.WaveFileTb);
        }

        private void Gaobt_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(this.MoveFileTb.Text) || !File.Exists(this.BehaveFileTb.Text) || !File.Exists(this.WaveFileTb.Text))
            {
                System.Windows.Forms.MessageBox.Show("Choose File!");
                return;
            }
            this.exps.Children.Clear();
            this.Analyser = new Analyser(MoveFileTb.Text, BehaveFileTb.Text, WaveFileTb.Text);
            this.Analyser.Work();
            Button bt_export = new Button();
            bt_export.Height = 30;
            bt_export.Focusable = false;
            bt_export.Click += bt_export_Click;
            bt_export.Content = "Export Data";
            this.exps.Children.Add(bt_export);
            Rectangle rec = new Rectangle();
            rec.Height = 10;
            this.exps.Children.Add(rec);
            ButExp = new Dictionary<Button, Experiment>();
            ButCb = new Dictionary<Button, CheckBox>();
            foreach (Experiment exp in this.Analyser.Experiments)
            {
                exp.Tag = this.BehaveFileTb.Text.Replace(".txt", "");
                StackPanel sp = new StackPanel()
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(0, 10, 0, 0),
                    VerticalAlignment = System.Windows.VerticalAlignment.Top
                };
                Label lb = new Label() { Content = "Experiment " + exp.Id };
                CheckBox cb = new CheckBox() { Content = "all", VerticalAlignment = System.Windows.VerticalAlignment.Center };

                sp.Children.Add(lb);

                Button[] bts = new Button[3];
                string[] titles = new string[3] { "Trails", "MaxWave", "AverageWave" };
                for (int i = 0; i < 3; ++i)
                {
                    bts[i] = new Button()
                    {
                        Height = 30,
                        Focusable = false,
                        Content = titles[i],
                        Margin = new Thickness(5, 0, 5, 0),
                        MinWidth = 100
                    };
                    bts[i].Click += bt_Click;
                    sp.Children.Add(bts[i]);
                    ButExp[bts[i]] = exp;
                    ButCb[bts[i]] = cb;
                }
                sp.Children.Add(cb);
                this.exps.Children.Add(sp);
            }
        }

        void bt_export_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("DATE\tCell\tRAT\tCHANNEL\tTASK\tEXP\tTRIAL\tORIENT\tREWARD\tAREA\tX_POS\tFR/BIN");
            sb.AppendLine();
            Analyser.Experiments.ForEach(exp => exp.AnalysisSPK(sb));
            string res = sb.ToString();
            string fileName = string.Format("{0}-{1}-{2}-analysis.txt", this.BehaveFileTb.Text.Replace(".txt", ""), DateTime.Now.Hour, DateTime.Now.Minute);
            File.WriteAllText(fileName, res, Encoding.Default);
            System.Windows.Forms.MessageBox.Show("Export Data Done");
        }

        void bt_Click(object sender, RoutedEventArgs e)
        {
            Button bt = sender as Button;
            switch (bt.Content.ToString())
            {
                case "Trails":
                    new Draw(new DrawHelper(this.ButExp[bt])).Show();
                    break;
                case "MaxWave":
                    new Matrix(new MatrixHelper(this.ButExp[bt], false, !(bool)ButCb[bt].IsChecked)).Show();
                    break;
                case "AverageWave":
                    new Matrix(new MatrixHelper(this.ButExp[bt], true, !(bool)ButCb[bt].IsChecked)).Show();
                    break;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            System.Windows.Forms.FolderBrowserDialog choose = new System.Windows.Forms.FolderBrowserDialog()
            {
                ShowNewFolderButton = false
            };
            if (choose.ShowDialog() == System.Windows.Forms.DialogResult.OK && !string.IsNullOrEmpty(choose.SelectedPath))
            {
                FillFileName(new DirectoryInfo(choose.SelectedPath), true);
            }
        }
    }
}
