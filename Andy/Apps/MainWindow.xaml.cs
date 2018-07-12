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
using LoadCsv;

namespace Apps
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btRun_Click(object sender, RoutedEventArgs e)
        {
            //txMsg.AppendText("LoadTrainSets\n");
            //FileAnalysis.LoadTrainSets(out _, out _, out _, out _);
            //txMsg.AppendText("LoadTestSets\n");
            //FileAnalysis.LoadTestSets();

            var NnRows = Analysis.AnalyzeAndCreateColumnsForNNetwork(true);
            //Analysis.WriteToCsvFile(@"NnInputs\hypotheses.csv", NnRows);
            Analysis.CreateNNetworkAndLearn(NnRows);

            txMsg.AppendText("Done!\n");
        }
    }
}
