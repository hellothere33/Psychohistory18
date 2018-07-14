using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Util.Net;

namespace Util.Csv
{
    /// <summary>
    /// This class takes care of CSV interactions via 
    /// </summary>
    public class uCsv
    {
        /// <summary>
        /// Append A new record to existing records
        /// </summary>
        public static void AppendToExistingRecords<T>(T performance, string filePath)
        {
            if (null == performance) return;
            AppendToExistingRecords(new List<T> { performance }, filePath);
        }
        
        /// <summary>
        /// Append LIST of new records to existing records by read first before writing. 
        /// </summary>
        public static void AppendToExistingRecords<T>(List<T> performances, string filePath)
        {
            //Read existing performance records
            var records = uCsv.ReadFromCsv<T>(filePath);

            //Add the performance from the input list. 
            records.AddRange(performances);
            uCsv.WriteToCsv<T>(filePath, records);
        }

        /// <summary>
        /// Write to CSV, without regard to existing records. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="records"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public static bool WriteToCsv<T>(string path, List<T> records, string delimiter = ";")
        {
            //Open new streamwriter, 
            using (var sw = new StreamWriter(path))
            {
                //open new CSV writer
                var writer = new CsvWriter(sw);
                
                //Add new line. 
                sw.NewLine = Environment.NewLine;

                //Specify delimiter
                writer.Configuration.Delimiter = delimiter;

                //Automap columns. 
                writer.Configuration.AutoMap<T>();

                //write header
                writer.WriteHeader<T>();

                //sw.WriteLine();

                //Flush write line. 
                writer.Flush();

                //If records (to be written) are not empty, 
                if (records != null)// && 0 < records.Count)
                {
                    //Write each node. 
                            //writer.WriteRecords<T>(records);
                    foreach (T node in records)
                    {
                        writer.WriteRecord<T>(node);
                        sw.WriteLine();
                        writer.Flush();
                    }
                }
                //writer.NextRecord();
            }
            return true;
        }


        /// <summary>
        /// Read from CSV, no writing. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public static List<T> ReadFromCsv<T>(string path, string delimiter = ";")
        {
            var records = new List<T>();
            if (!File.Exists(path))
            {   // create empty Csv with only headers
                WriteToCsv<T>(path, null);
                return records;
            }

            using (var sr = new StreamReader(path))
            {
                var reader = new CsvReader(sr);

                //Initiaate CSV Reader Configuration
                reader.Configuration.HasHeaderRecord = true;
                reader.Configuration.PrepareHeaderForMatch = uStr.ToAlphaNumericOnly;
                reader.Configuration.Delimiter = delimiter;
                reader.Configuration.AutoMap<T>();               

                
                /* Whole file read (
                //Get records. 
                var blas = reader.GetRecords<T>();
                // read all at once
                records = blas.ToList();
                */
                
                // Prefer reading one by one since we can know find out which line has a corrupted record
                const int nbExceptionsTolerated = 2;
                int exceptionsCount = 0;
                for (int i = 0; reader.Read(); i++)
                {
                    try
                    {
                        if (nbExceptionsTolerated < exceptionsCount) break; // too many exceptions detected, skip rest of file
                        var rec = reader.GetRecord<T>();
                        records.Add(rec);
                    }
                    catch (Exception ex)
                    {
                        System.Console.WriteLine(ex.Message);
                        exceptionsCount++;
                    }
                }
            }

            return records;
        }
        
    }
}


