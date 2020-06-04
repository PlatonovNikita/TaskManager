using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using TaskManager.BL.Model;

namespace TaskManager.BL.Controller
{
    /// <summary>
    /// Контроллер пользователя.
    /// </summary>
    public class UserController
    {
        List<User> users;
        public User User { get; }
        public List<string> Boards { get { return User?.Boards; } }
        public bool IsNewUser { get; } = false;

        /// <summary>
        /// Создать контроллер пользователя.
        /// </summary>
        /// <param name="nikName"> Ник пользователя. </param>
        public UserController(string nikName)
        {
            if (string.IsNullOrWhiteSpace(nikName))
            {
                throw new ArgumentNullException("Имя не может быть пустым!", nameof(nikName));
            }

            users = GetUsersData();

            User = users.SingleOrDefault(user => user.NikName == nikName);

            if (User == null)
            {
                User = new User(nikName);
                IsNewUser = true;
                users.Add(User);
                Save();
            }

        }

        /// <summary>
        /// Настоить нового полььзователя.
        /// </summary>
        /// <param name="name"> Имя пользователя. </param>
        public void SetNewUser(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("Имя пользователя не может быть пустым!", nameof(name));
            }

            User.Name = name;
            Save();
        }
        
        /// <summary>
        /// Добавить новую доску в список доступных пользователю.
        /// </summary>
        /// <param name="nameBoard"> Имя доски. </param>
        public void AddBoard(string nameBoard)
        {
            if (string.IsNullOrWhiteSpace(nameBoard))
            {
                throw new ArgumentNullException("Имя доски не может быть пустым!", nameof(nameBoard));
            }
            if (User.Boards.Count(b => b == nameBoard) == 0)
            {
                User.Boards.Add(nameBoard);
                Save();
            }
            else
            {
                throw new AggregateException("Такая доска уже есть в списке!");
            }
        }

        /// <summary>
        /// Загружает пользователей из файла.
        /// </summary>
        /// <returns> Список пользователей. </returns>
        public List<User> GetUsersData()
        {
            var formatter = new BinaryFormatter();

            using (var fs = new FileStream("users.dat", FileMode.OpenOrCreate))
            {
                if (fs.Length != 0 && formatter.Deserialize(fs) is List<User> users)
                {
                    return users;
                }
                else
                {
                    return new List<User>();
                }
            }
        }

        /// <summary>
        /// Сохраняет пользователей в файл.
        /// </summary>
        public void Save()
        {
            var formatter = new BinaryFormatter();

            using (var fs = new FileStream("users.dat", FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, users);
            }

        }

    }
}
