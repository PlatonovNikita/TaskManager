using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace TaskManager.BL.Controller
{
    public abstract class BaseController
    {
        protected IDataSaver Saver { get; private set; } = new SerializeDataSaver();
        protected void Save<T>(string fileName, List<T> item) where T : class
        {
            Saver.Save<T>(fileName, item);
        }

        protected List<T> Load<T>(string fileName) where T : class
        {
            return Saver.Load<T>(fileName);
        }
    }
}
