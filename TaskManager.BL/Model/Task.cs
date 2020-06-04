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

        string executorsNik;
        /// <summary>
        /// Ник исполниителя задачи.
        /// </summary>
        public string ExecutorsNik { 
            get 
            { 
                return executorsNik; 
            } 
            set 
            {
                executorsNik = (!string.IsNullOrWhiteSpace(value)) ? value : throw new ArgumentNullException("Ник не  может быть пустым!");
            } 
        }

        /// <summary>
        /// Приоритет выполнения.
        /// </summary>
        public Priority Priority { get; } = Priority.P4;

        DateTime deadLine;
        /// <summary>
        /// Дата дедлайна.
        /// </summary>
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

        public Task(string name, DateTime deadLine, Priority priority = Priority.P4)
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
        /// Парсит строку в соответствующе приоритеты.
        /// </summary>
        /// <param name="input"> Входная строка. </param>
        /// <param name="priority"> Приоритет. </param>
        /// <returns> Возвращает true, если удалось преобразовать строку. </returns>
        public static bool PriorityParse(string input, out Priority priority)
        {
            switch (input)
            {
                case "P4":
                    priority = Priority.P4;
                    return true;
                case "P3":
                    priority = Priority.P3;
                    return true;
                case "P2":
                    priority = Priority.P2;
                    return true;
                case "P1":
                    priority = Priority.P1;
                    return true;
                case "first":
                    priority = Priority.P1;
                    return true;
                case "second":
                    priority = Priority.P2;
                    return true;
                case "third":
                    priority = Priority.P3;
                    return true;
                case "":
                    priority = Priority.P4;
                    return true;
                default:
                    priority = Priority.P4;
                    return false;
            }
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