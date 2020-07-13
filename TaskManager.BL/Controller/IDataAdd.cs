using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.BL.Controller
{
    public interface IDataAdd
    {
        void Add<T>(T item) where T : class;
    }
}
