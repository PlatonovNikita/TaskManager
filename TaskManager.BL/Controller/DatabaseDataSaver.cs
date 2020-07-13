using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using TaskManager.BL.Model;

namespace TaskManager.BL.Controller
{
/*    class DatabaseDataSaver : IDataSaver , IDataAdd
    {
        public void Add<T>(T item) where T : class
        {
            using (var db = new ApplicationContext<T>())
            {
                db.items.Add(item);
                db.SaveChanges();
            }
        }

        public List<T> Load<T>(string fileName) where T : class
        {
            using (ApplicationContext<T> db = new ApplicationContext<T>())
            {
                return db.items.ToList<T>();
            }
        }

        public void Save<T>(string fileName, T item) where T : class
        {
            using (ApplicationContext<T> db = new ApplicationContext<T>())
            {
                db.items.Update(item);
                db.SaveChanges();
            }
        }
    }*/
}
