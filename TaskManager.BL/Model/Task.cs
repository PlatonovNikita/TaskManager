using System;
using System.Collections.Generic;

namespace TaskManager.BL.Model
{
    /// <summary>
    /// Состояние задачи. 
    /// Не_выполняется/Выполняется/Выполнена
    /// </summary>
    [Serializable]
    public enum Status
    {
        NotPerformed = 0,
        Performed = 1,
        Complited = 2
    }

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
        public int Id { get; set; }
        #region Свойства
        /// <summary>
        /// Имя задачи.
        /// </summary>
        public string Name { get; }

        string executorsNik = null;
        /// <summary>
        /// Ник исполниителя задачи.
        /// </summary>
        public string ExecutorsNik
        {
            get
            {
                return executorsNik;
            }
            set
            {
                executorsNik = (!string.IsNullOrWhiteSpace(value)) ? value : throw new ArgumentNullException("Ник не  может быть пустым!");
            }
        }

        Status status = Status.NotPerformed;
        /// <summary>
        /// Состояние задачи. 
        /// Не_выполняется/Выполняется/Выполнена
        /// </summary>
        public Status Status
        {
            get
            {
                return status;
            }
            set
            {
                if (status == Status.Complited)
                {
                    throw new ArgumentException("Эта задача уже выполненна!");
                }
                if (value == Status.Complited && status == Status.NotPerformed)
                {
                    throw new ArgumentException("Эта задача никем не выполнялась!");
                }

                status = value;
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
        public List<Task> SubTasks { get; set; } = null;

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
        /// Возвращает строковое представление задачи.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Name}\nСостояние: {Status}\nПриоритет: {Priority}\nСрок исполнения: {Period} дня(дней)";
        }


    }
}