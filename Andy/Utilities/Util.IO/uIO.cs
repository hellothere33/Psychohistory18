using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util.IO
{
    public class uIO
    {
        public const string ImageFileFilter = "Images|*.png;*.jpg;*.jpeg;*.bmp;*.tiff|All files|*.*";
        public const string ImageExt = "*.bmp; *.jpg; *.jpeg; *.png; *.tiff";
        public const string ExtXml = ".xml";

        public static bool DoesFileExist(string filePath)
        {
            if (File.Exists(filePath)) return true;
            return false;
        }
        public static void DeleteIfFileExist(string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        public static bool CreateDirAndFileIfDoesntExist(string filePath)
        {
            if (string.IsNullOrWhiteSpace (filePath)) return false;
            if (!CreateFileParentDirectory(filePath)) return false;
            PrintToText(filePath, null, "");
            return true;
        }

        public static void PrintToText(string filePath, string header, string text)
        {
            //string path = @"C:\temp2\";
            //string ext = @".txt";
            string name = filePath;//path + ext;

            string dir = Path.GetDirectoryName(name);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            bool exist = File.Exists(name);
            using (StreamWriter sw = exist ? File.AppendText(name) : File.CreateText(name))
            {
                if (!string.IsNullOrEmpty(header)) sw.WriteLine(header);
                if (!string.IsNullOrEmpty(text  )) sw.WriteLine(text);
                sw.Flush();
                sw.Close();
            }
        }

        public static void PrintToText(string filePath, string header, List<string> multiLineText)
        {
            if (null == multiLineText || multiLineText.Count <= 0) return;
            string name = filePath;//path + ext;

            string dir = Path.GetDirectoryName(name);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            bool exist = File.Exists(name);
            using (StreamWriter sw = exist ? File.AppendText(name) : File.CreateText(name))
            {
                if (!string.IsNullOrEmpty(header)) sw.WriteLine(header);

                foreach (string text in multiLineText)
                {
                    if (!string.IsNullOrEmpty(text)) sw.WriteLine(text);
                }
                sw.Flush();
                sw.Close();
            }
        }

        public static bool CreateFileParentDirectory(string filePath)
        {
            string dir = Path.GetDirectoryName(filePath);
            return CreateDirectory(dir);
        }
        public static bool CreateDirectory(string dir)
        {
            if (string.IsNullOrWhiteSpace(dir))
                return false;

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            return true;
        }

        public static List<string> GetImageFileNamesFromDirectory(string dir)
        {
            var images = GetImageFileEnumeratorsFromDirectory(dir);
            if (null == images) return null;

            List<string> imageFiles = new List<string>();
            foreach (var path in images)
                imageFiles.Add(path); // only save image files that has an accompanying .xml that returned non-null locations (zero location is ok)

            return imageFiles;
        }

        public static List<string> GetImageFileEnumeratorsFromDirectory(string dir)
        {
            if (string.IsNullOrWhiteSpace(dir)) return null;
            if (!Directory.Exists(dir))         return null;
            var images = Directory.EnumerateFiles(dir, "*.*", SearchOption.TopDirectoryOnly).Where(s => s.ToLower().EndsWith(".png")
                                                                                                     || s.ToLower().EndsWith(".bmp")
                                                                                                     || s.ToLower().EndsWith(".jpg")
                                                                                                     || s.ToLower().EndsWith(".jpeg"));
            return images.ToList();
        }

        /// <summary>
        /// Get the list of all files with a specific extension, within a specific directory. Use option parameter to determine recursive search.
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="ext"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public static List<string> GetFilesFromDirectory(string dir, string ext, SearchOption option = SearchOption.TopDirectoryOnly)
        {
            if (string.IsNullOrWhiteSpace(dir)) return null;
            if (!Directory.Exists(dir))         return null;
            var images = Directory.EnumerateFiles(dir, "*.*", option).Where(s => s.EndsWith(ext));
            return images.ToList();
        }
    }

    public class XYZWPR
    {
        private static char separator = '|';

        /// <summary>
        /// A standardized function to format xyzwpr information into various form that can be interpolated across application. 
        /// </summary>
        /// <param name="Nice"></param>
        /// <returns></returns>
        public static void ToStr(double x, double y, double z, double w, double p, double r, out string XYZWPR)
        {
            StringBuilder returnString = new StringBuilder();
            returnString.Append(x);     returnString.Append(separator);
            returnString.Append(y);     returnString.Append(separator);
            returnString.Append(z);     returnString.Append(separator);
            returnString.Append(w);     returnString.Append(separator);
            returnString.Append(p);     returnString.Append(separator);
            returnString.Append(z);     //returnString.Append(separator);

            XYZWPR = returnString.ToString();
        }

        public static void FromStr(string xyzwpr, out double x, out double y, out double z, out double w, out double p, out double r)
        {
            x = y = z = w = p = r = 0;
            string[] xyzwprStringArray = xyzwpr.Split(separator);
            
            if (xyzwprStringArray.Length == 6)
            {
                Double.TryParse(xyzwprStringArray[0], out x);
                Double.TryParse(xyzwprStringArray[1], out y);
                Double.TryParse(xyzwprStringArray[2], out z);
                Double.TryParse(xyzwprStringArray[3], out w);
                Double.TryParse(xyzwprStringArray[4], out p);
                Double.TryParse(xyzwprStringArray[5], out r);
            }
        }
    }

}
