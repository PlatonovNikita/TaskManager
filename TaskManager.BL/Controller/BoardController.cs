using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using TaskManager.BL.Model;

namespace TaskManager.BL.Controller
{
    /// <summary>
    /// Контроллер доски задач.
    /// </summary>
    public class BoardController
    {
        List<Board> Boards { get; }
        Board Board { get; }
        public ReadOnlyCollection<Task> Tasks { get { return Board.Tasks.AsReadOnly(); } }
        public bool IsNewBoard { get; } = false;
        public bool IsPasses { get; private set; } = false;
        public bool IsTakes { get; private set; } = false;

        /// <summary>
        /// Создать контроллер.
        /// </summary>
        /// <param name="nameBoard"> Имя доски. </param>
        public BoardController(string nameBoard)
        {
            if (string.IsNullOrWhiteSpace(nameBoard))
            {
                throw new ArgumentNullException("Наименованиие доски не может быть пустым!", nameof(nameBoard));
            }

            Boards = GetBoardsData();
            Board = Boards.SingleOrDefault(b => b.Name == nameBoard);

            if (Board == null)
            {
                Board = new Board(nameBoard);
                IsNewBoard = true;
                Boards.Add(Board);
                Save();
            }
        }

        /// <summary>
        /// Добавить задачу.
        /// </summary>
        /// <param name="task"> Задача. </param>
        public void AddTask(Task task)
        {
            if (task == null)
            {
                throw new ArgumentNullException("Задача не может быть null!", nameof(task));
            }
            if (Board.Tasks.Count(t => t.Name == task.Name) == 0)
            {
                Board.Tasks.Add(task);
                Save();
            }
            else
            {
                throw new ArgumentException("Такая задача уже есть в списке задач!", nameof(task));
            }
        }

        /// <summary>
        /// Удалить задачу с данной доски.
        /// </summary>
        /// <param name="nameTask"> Наименование задача. </param>
        public void DelTask(string nameTask)
        {
            if (string.IsNullOrWhiteSpace(nameTask))
            {
                throw new ArgumentNullException("Имя задачи не может быть пустым!", nameof(nameTask));
            }
            if (Board.Tasks.Count(t => t.Name == nameTask) == 0)
            {
                throw new ArithmeticException("Такой задачи нет в списке задач!");
            }
            else
            {
                Board.Tasks.RemoveAll(b => b.Name == nameTask);
                Save();
            }

        }

        /// <summary>
        /// Взять задачу на выполнение.
        /// </summary>
        /// <param name="userController"> Контроллер пользователя. </param>
        /// <param name="nameTask"> Наименование задачи. </param>
        public void TakeTask(UserController userController, string nameTask)
        {
            #region Проверки
            if (userController == null)
            {
                throw new ArgumentNullException("Контроллер пользователя не может быть null!", nameof(userController));
            }
            if (string.IsNullOrWhiteSpace(nameTask))
            {
                throw new ArgumentNullException("Имя задачи не  может быть пустым!", nameof(nameTask));
            }
            if(userController.TaskOfUser != null)
            {
                throw new ArgumentException("У пользователя уже есть активная задача!");
            }
            #endregion Проверки

            Task task = Board.Tasks.Find(t => t.Name == nameTask);

            #region Проверки
            if (task == null)
            {
                throw new ArgumentException("У даски нет текущей задачи!");
            }
            if (task.ExecutorsNik != null)
            {
                throw new ArgumentException("У этой задачи уже есть исполнитель!");
            }
            #endregion Проверки

            IsTakes = true;
            try
            {
                userController.TakeTask(this, task);
            }
            finally
            {
                IsTakes = false;
            }
            task.ExecutorsNik = userController.NikOfUser;
            task.Status = Status.Performed;
            Save();
        }

        /// <summary>
        /// Сдать текущую задачу.
        /// </summary>
        /// <param name="userController"> Исполнитель. </param>
        /// <param name="nameTask"> Наименование задачи. </param>
        public void PassTask(UserController userController, string nameTask)
        {
            #region Проверки
            if (userController == null)
            {
                throw new ArgumentNullException("Контроллер пользователя не может быть null!", nameof(userController));
            }

            if (string.IsNullOrWhiteSpace(nameTask))
            {
                throw new ArgumentNullException("Имя задачи не  может быть пустым!", nameof(nameTask));
            }

            if (userController.TaskOfUser == null)
            {
                throw new ArgumentException("У пользователя нету активных задач!");
            }

            if (userController.TaskOfUser.Name != nameTask)
            {
                throw new ArgumentException($"У пользователя активна другая задача: \"{userController.TaskOfUser.Name}\"");
            }
            #endregion Проверки

            Task task = Board.Tasks.Find(t => t.Name == nameTask);

            #region Проверки
            if (task == null)
            {
                throw new ArgumentException("У даски нет текущей задачи!");
            }

            if (task.ExecutorsNik != userController.NikOfUser)
            {
                throw new ArgumentException($"У задачи другой исполнитель: {task.ExecutorsNik}");
            }
            #endregion Проверки

            IsPasses = true;
            try
            {
                userController.PassTask(this);
            }
            finally
            {
                IsPasses = false;
            }
            task.Status = Status.Complited;
            Save();
        }

        /// <summary>
        /// Получить набор досок из файла.
        /// </summary>
        /// <returns></returns>
        public List<Board> GetBoardsData()
        {
            var formatter = new BinaryFormatter();

            using (var fs = new FileStream("board.dat", FileMode.OpenOrCreate))
            {
                if (fs.Length > 0 && formatter.Deserialize(fs) is List<Board> boards)
                {
                    return boards;
                }
                else
                {
                    return new List<Board>();
                }
            }
        }

        /// <summary>
        /// Сохранить информацию в файл.
        /// </summary>
        public void Save()
        {
            var formatter = new BinaryFormatter();

            using (var fs = new FileStream("board.dat", FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, Boards);
            }
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
    }
}
