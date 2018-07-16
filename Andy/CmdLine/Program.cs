using LoadCsv;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmdLine
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Load training dataset");
            List<NnRow> dataset = Analysis.AnalyzeAndCreateColumnsForNNetwork(trainNotTest: true, useFull: true, loadBin: false);
            Analysis.WriteToCsvFile(@"NnInputs\hypotheses_train - Andy.csv", dataset); //Console.WriteLine("Write NN data to CSV for Keras");

            List<NnRow> datasetTest = Analysis.AnalyzeAndCreateColumnsForNNetwork(trainNotTest: false, useFull: true, loadBin: false);
            Analysis.WriteToCsvFile(@"NnInputs\hypotheses_test - Andy.csv", datasetTest); //Console.WriteLine("Write NN data to CSV for Keras");


            Console.WriteLine("Start training");
            string NnModelPath = Analysis.CreateNNetworkAndLearn(dataset);


            Console.WriteLine("Load model and predict on test dataset");
            List<DataSolution> predictions = Analysis.Predict(NnModelPath, datasetTest);

            Analysis.ExportToFile(@"NnInputs\mlDotNet_solution.csv", predictions);

            Console.WriteLine("All Done!");
        }
    }
}
