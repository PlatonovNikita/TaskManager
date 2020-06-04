using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.BL.Model
{
    /// <summary>
    /// Доска с задачами.
    /// </summary>
    [Serializable]
    public class Board
    {
        #region Свойства
        /// <summary>
        /// Наименование доски с задачами.
        /// </summary>
        public string Name { get; }
        
        /// <summary>
        /// Набор задач.
        /// </summary>
        public List<Task> Tasks { get; }
        #endregion Свойства

        /// <summary>
        /// Создать доску с задачами.
        /// </summary>
        /// <param name="name"> Наименование доски. </param>
        /// <param name="tasks"> Список задач. </param>
        public Board(string name, List<Task> tasks)
        {
            #region Проверка
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("Наименование доски не может быть пустым!", nameof(name));
            }
            if (tasks == null)
            {
                throw new ArgumentNullException("Список задач не может быть пустым!", nameof(tasks));
            }
            #endregion Проверка

            Name = name;
            Tasks = tasks;
        }

        /// <summary>
        /// Возвращает строковое представлениие доски задач.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string tasks = "";

            foreach (var task in Tasks)
            {
                tasks += $"{task}\n\n";
            }

            return $"Доска: {Name}\nЗадачи:\n{tasks}";
        }
    }
}
