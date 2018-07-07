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
	public class uCsv
	{
		/// <summary>
		/// Append new record to existing records
		/// </summary>
		public static void AppendToExistingRecords<T>(T performance, string filePath)
		{
			if (null == performance) return;
			AppendToExistingRecords(new List<T> { performance }, filePath);
		}
		/// <summary>
		/// Append new records to existing records
		/// </summary>
		public static void AppendToExistingRecords<T>(List<T> performances, string filePath)
		{
			var records = uCsv.ReadFromCsv<T>(filePath);
			records.AddRange(performances);
			uCsv.WriteToCsv<T>(filePath, records);
		}

		public static bool WriteToCsv<T>(string path, List<T> records, string delimiter = ";")
		{
			using (var sw = new StreamWriter(path))
			{
				var writer = new CsvWriter(sw);
				writer.Configuration.Delimiter = delimiter;
				writer.Configuration.AutoMap<T>();

				writer.WriteHeader<T>();
                if (records != null && 0 < records.Count)
				{
					foreach (T node in records)
						writer.WriteRecord<T>(node);
				}
				writer.NextRecord();
			}
			return true;
		}
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
                reader.Configuration.PrepareHeaderForMatch = uStr.ToAlphaNumericOnly;
                reader.Configuration.Delimiter = delimiter;
				reader.Configuration.AutoMap<T>();

                var blas = reader.GetRecords<T>();
				//// read all at once
				//records = blas.ToList();

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


