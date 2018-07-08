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
            var fileFacturationTrain  = FileFacturation.LoadCsvFile(@"Data/facturation_train.csv");
            var fileFacturationTest   = FileFacturation.LoadCsvFile(@"Data/facturation_test.csv");
            var filePaiementsTrain    = FileFacturation.LoadCsvFile(@"Data/paiements_train.csv");
            var filePaiementsTest     = FileFacturation.LoadCsvFile(@"Data/paiements_test.csv");
            var filePerformanceTrain  = FileFacturation.LoadCsvFile(@"Data/performance_train.csv");
            var filePerformanceTest   = FileFacturation.LoadCsvFile(@"Data/performance_test.csv");
            var fileTransactionsTrain = FileFacturation.LoadCsvFile(@"Data/transactions_train.csv");
            var fileTransactionsTest  = FileFacturation.LoadCsvFile(@"Data/transactions_test.csv");



        }
    }
}
