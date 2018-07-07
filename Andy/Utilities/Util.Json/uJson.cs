using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Serialization;
using Util.IO;

namespace Util.Json
{
    public class uJson
    {
        /// <summary>
        /// Write the object into a juman readable JavaScript Object Notation (JSON) file.
        /// </summary>
        public static string SerializeToString(object obj)
        {
            string text = JsonConvert.SerializeObject(obj, Formatting.Indented);
            return text;
        }

        public static T DeserializeFromString<T>(string text)
        {
            T obj = default(T);
            obj = JsonConvert.DeserializeObject<T>(text);
            return obj;
        }

        /// <summary>
        /// Write the object into a juman readable JavaScript Object Notation (JSON) file.
        /// </summary>
        public static void Serialize(string filePath, object obj)
        {
            uIO.DeleteIfFileExist(filePath);
            uIO.CreateFileParentDirectory(filePath);
            using (FileStream fs = new FileStream(filePath, FileMode.CreateNew))
            {
                string text = SerializeToString(obj);
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(text);
                sw.Flush();
                fs.Close();
            }
        }

        /// <summary>
        /// Read a JSON file into a predefined class.
        /// </summary>
        /// <typeparam name="T">e.g. Matrix of Double, Int, String, whatever</typeparam>
        /// <param name="filePath">Full path to the file including the extension</param>
        /// <returns></returns>
        public static T Deserialize<T>(string filePath)
        {
            if (!uIO.DoesFileExist(filePath)) return default(T);

            T obj = default(T);
            //BinaryFormatter bf = new BinaryFormatter();
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                StreamReader sr = new StreamReader(fs);
                string text = sr.ReadToEnd();
                obj = DeserializeFromString<T>(text);
            }
            return obj;
            //var output = (T)obj;
            //return output;
        }
    }
}
