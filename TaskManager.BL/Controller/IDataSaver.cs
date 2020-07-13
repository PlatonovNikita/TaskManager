using System;
using System.Collections.Generic;

namespace TaskManager.BL.Controller
{
    public interface IDataSaver
    {
        void Save<T>(string fileName, List<T> item) where T : class;

        List<T> Load<T>(string fileName) where T : class;
    }
}
