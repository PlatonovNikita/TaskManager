using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace TaskManager.BL.Controller
{
    class SerializeDataSaver : IDataSaver
    {
        public List<T> Load<T>(string fileName) where T : class
        {
            var formatter = new BinaryFormatter();

            using (var fs = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                if (fs.Length != 0 && formatter.Deserialize(fs) is List<T> items)
                {
                    return items;
                }
                else
                {
                    return default;
                }
            }
        }

        public void Save<T>(string fileName, List<T> item) where T : class
        {
            var formatter = new BinaryFormatter();

            using (var fs = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, item);
            }
        }
    }
}
