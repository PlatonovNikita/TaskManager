using System;
using System.Collections.Generic;

namespace TaskManager.BL.Model
{
    /// <summary>
    /// Приоритет задачи.
    /// </summary>
    [Serializable]
    public enum Priority
    {
        P1 = 3,
        P2 = 2,
        P3 = 1,
        P4 = 0,
        first = P1,
        second = P2,
        third = P3
    }
    /// <summary>
    /// Задача.
    /// </summary>
    [Serializable]
    public class Task
    {
        #region Свойства
        /// <summary>
        /// Имя задачи.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Приоритет выполнения.
        /// </summary>
        public Priority Priority { get; } = Priority.P4;

        /// <summary>
        /// Дата дедлайна.
        /// </summary>
        DateTime deadLine;
        public DateTime DeadLine { get { return deadLine; } set { deadLine = (value >= DateTime.Now) ? value : DateTime.Now; } }

        /// <summary>
        /// Список подзадач.
        /// </summary>
        public List<Task> SubTasks { get; } = null;

        /// <summary>
        /// Срок исполнения.
        /// </summary>
        public int Period { get { return DeadLine.DayOfYear - DateTime.Now.DayOfYear; } }
        #endregion Свойства 

        public Task(string name, Priority priority, DateTime deadLine)
        {
            #region Проверка
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("Имя задачи не может быть пустым!", nameof(name));
            }

            if (deadLine < DateTime.Now)
            {
                throw new ArgumentOutOfRangeException("Дедлайн не может быть в прошлом!", nameof(deadLine));
            }
            #endregion Проверка

            Name = name;
            Priority = priority;
            DeadLine = deadLine;
        }

        /// <summary>
        /// Возвращает строковое представление задачи.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Name}\nПриоритет: {Priority}\nСрок исполнения: {Period} дня(дней)";
        }


    }
}