using System;
using System.Collections.Generic;

namespace TaskManager.BL.Model
{
    /// <summary>
    /// Пользователь.
    /// </summary>
    [Serializable]
    public class User
    {
        #region Свойства
        /// <summary>
        /// Имя пользвателя.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Уникальное ник пользователя.
        /// </summary>
        public string NikName { get; }

        /// <summary>
        /// Текущая задача.
        /// </summary>
        public Task Task { get; set; }

        /// <summary>
        /// Список досок доступных пользователю.
        /// </summary>
        public List<string> Boards { get; } = new List<string>();
        #endregion Свойства

        /// <summary>
        /// Создать пользователя.
        /// </summary>
        /// <param name="name"> Имя пользователя. </param>
        /// <param name="nikName"> Ник пользователя. </param>
        /// <param name="task"> Текущая задача пользователя. </param>
        /// <param name="boards"> Список доступных пользователю задач. </param>
        public User(string name, string nikName, Task task, List<string> boards)
        {
            #region Проверка
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("Имя пользователя не модет быть пустым!", nameof(name));
            }

            if (string.IsNullOrWhiteSpace(nikName))
            {
                throw new ArgumentNullException("Ник не может быть пустым!", nameof(nikName));
            }

            if (task == null)
            {
                throw new ArgumentNullException("Задача не может быть null!", nameof(task));
            }

            if (boards == null)
            {
                throw new ArgumentNullException("Список досок не может быть null!", nameof(boards));
            }
            #endregion Проверка

            Name = name;
            NikName = nikName;
            Task = task;
            Boards = boards;
        }

        /// <summary>
        /// Создать нового пользователя.
        /// </summary>
        /// <param name="nikName"> Ник пользователя. </param>
        public User(string nikName)
        {
            if (string.IsNullOrWhiteSpace(nikName))
            {
                throw new ArgumentNullException("Ник не может быть пустым!", nameof(nikName));
            }

            NikName = nikName;
        }

        /// <summary>
        /// Возвращает строковое представлениие пользователя.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{NikName}\nИмя: {Name}\n";
        }
    }
}