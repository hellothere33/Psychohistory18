using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Util.BinarySerializer
{
    public static class uBin
    {
        /// <summary>
        /// Writes out the object into a binary file.
        /// </summary>
        /// <param name="filePath">Make sure you put a file extension as well as the full path.</param>
        /// <param name="obj">Object to be written to file. Can accept any object.</param>
        public static void Serialize(string filePath, object obj)
        {
            BinaryFormatter   bf = new BinaryFormatter();
            using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                bf.Serialize(fs, obj);
            }
        }

        /// <summary>
        /// Read a binary file into a predefined class. 
        /// </summary>
        /// <typeparam name="T">e.g. Matrix of Double, Int, String, whatever object</typeparam>
        /// <param name="filePath">Full path to the file including the extension</param>
        /// <returns></returns>
        public static T Deserialize<T>(string filePath)
        {
            object obj;
            BinaryFormatter   bf = new BinaryFormatter();
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                obj = bf.Deserialize(fs);
            }

            var output = (T)obj;
            return output;
        }
    }
}