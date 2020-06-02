using System;

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
        #endregion Свойства

        public User(string name, string nikName, Task task = null)
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
            #endregion Проверка

            Name = name;
            NikName = nikName;
            Task = task;
        }

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