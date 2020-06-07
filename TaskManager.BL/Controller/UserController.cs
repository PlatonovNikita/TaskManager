using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using TaskManager.BL.Model;

namespace TaskManager.BL.Controller
{
    /// <summary>
    /// Контроллер пользователя.
    /// </summary>
    public class UserController
    {
        List<User> users;
        User User { get; }
        public ReadOnlyCollection<string> Boards { get { return User.Boards.AsReadOnly(); } }
        public bool IsNewUser { get; } = false;
        public string NikOfUser => User.NikName;
        public Task TaskOfUser => User.Task;
        public string NameOfUser 
        { 
            get 
            { 
                return User.Name; 
            } 
            set 
            {
                User.Name = value;
                Save();
            } 
        }


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
        /// 
        /// </summary>
        /// <param name="boardController"></param>
        /// <param name="task"></param>
        public void TakeTask(BoardController boardController, Task task)
        {
            //TODO: Реализовать через интерфейс, который гарантирует у объекта есть метод TakeTask() и поле bool IsTakes 
            if (boardController == null)
            {
                throw new ArgumentNullException("Контроллер доски не может быть null!", nameof(boardController));
            }

            if (!boardController.IsTakes)
            {
                throw new ArgumentException("PassTask должен быть вызван контроллером доски!");
            }

            User.Task = task;
            Save();
        }

        /// <summary>
        /// Изменяет текущую задачу пользователя на null.
        /// </summary>
        /// <param name="boardController"> Ссылка на контроллер доски. </param>
        public void PassTask(BoardController boardController)
        {
            if (boardController == null)
            {
                throw new ArgumentNullException("Контроллер доски не может быть null!", nameof(boardController));
            }

            if (!boardController.IsPasses)
            {
                throw new ArgumentException("PassTask должен быть вызван контроллером доски!");
            }

            User.Task = null;
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
                throw new ArgumentException("Такая доска уже есть в списке!");
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
