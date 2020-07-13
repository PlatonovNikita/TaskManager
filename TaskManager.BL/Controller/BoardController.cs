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
    public class BoardController : BaseController
    {
        const string fileName = "board.dat";
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
            #region Проверки
            if (task == null)
            {
                throw new ArgumentNullException("Задача не может быть null!", nameof(task));
            }
            if (Board.Tasks.Count(t => t.Name == task.Name) != 0)
            {
                throw new ArgumentException("Такая задача уже есть в списке задач!", nameof(task));
            }
            #endregion Проверки

            Board.Tasks.Add(task);
            SortTasks();
            Save();
        }

        /// <summary>
        /// Удалить задачу с данной доски.
        /// </summary>
        /// <param name="nameTask"> Наименование задача. </param>
        public void DelTask(string nameTask)
        {
            #region Проверки
            if (string.IsNullOrWhiteSpace(nameTask))
            {
                throw new ArgumentNullException("Имя задачи не может быть пустым!", nameof(nameTask));
            }
            if (Board.Tasks.Count(t => t.Name == nameTask) == 0)
            {
                throw new ArgumentException("Такой задачи нет в списке задач!");
            }
            #endregion Проверки

            Board.Tasks.RemoveAll(b => b.Name == nameTask);
            Save();

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
            if (userController.TaskOfUser != null)
            {
                throw new ArgumentException("У пользователя уже есть активная задача!");
            }
            if (Board.Tasks.Count(t => t.Name == nameTask) == 0)
            {
                throw new ArgumentException("Такой задачи нет в списке задач!");
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
            SortTasks();
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
            if (Board.Tasks.Count(t => t.Name == nameTask) == 0)
            {
                throw new ArgumentException("Такой задачи нет в списке задач!");
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
            SortTasks();
            Save();
        }

        /// <summary>
        /// Отсортировать задачи на доске.
        /// </summary>
        public void SortTasks()
        {
            Board.Tasks.Sort(delegate (Task t1, Task t2)
            {
                if (t1.Status < t2.Status)
                {
                    return -1;
                }
                if (t1.Status == t2.Status)
                {
                    if (t1.Priority > t2.Priority)
                    {
                        return -1;
                    }
                    if (t1.Priority == t2.Priority)
                    {
                        if (string.Compare(t1.Name, t2.Name) < 0)
                        {
                            return -1;
                        }
                    }
                }
                return 1;
            });
        }


        /// <summary>
        /// Добавить подзадачу.
        /// </summary>
        /// <param name="nameTask"> Имя задачи, в которую требуется добавить подзадачу. </param>
        /// <param name="subtask"> Подзадача. </param>
        public void AddSubTask(Task task, Task subtask)
        {
            #region Проверки
            if (task == null)
            {
                throw new ArgumentNullException("Задача не может быть null!", nameof(task));
            }
            if (subtask == null)
            {
                throw new ArgumentNullException("Подзадача не может быть null!", nameof(subtask));
            }
            if (task.SubTasks == null)
            {
                task.SubTasks = new List<Task>();
            }
            else if (task.SubTasks.Count(s => s.Name == subtask.Name) != 0)
            {
                throw new ArgumentException("Такая подзадача уже есть в списке подзадачзадач!", nameof(subtask));
            }
            #endregion Проверки

            subtask.Status = Status.Performed;
            task.SubTasks.Add(subtask);
            Save();
        }

        /// <summary>
        /// Удалить подзадачу.
        /// </summary>
        /// <param name="nameTask"> Имя задачи, из которой требуется удалить подзадачу. </param>
        /// <param name="nameSubtask"> Имя подзадачи. </param>
        public void DelSubTask(Task task, string nameSubtask)
        {
            #region Проверки
            if (task == null)
            {
                throw new ArgumentNullException("Задача не может быть null!", nameof(task));
            }
            if (string.IsNullOrWhiteSpace(nameSubtask))
            {
                throw new ArgumentNullException("Имя подзадачи не может быть пустым!", nameof(nameSubtask));
            }
            if (task.SubTasks == null)
            {
                throw new ArgumentNullException("Список подзадач пуст!");
            }
            if (task.SubTasks.Count(s => s.Name == nameSubtask) == 0)
            {
                throw new ArgumentException("Такой подзадачи нет в списке подзадач");
            }
            #endregion Проверки

            task.SubTasks.RemoveAll(s => s.Name == nameSubtask);

            if (task.SubTasks.Count == 0)
            {
                task.SubTasks = null;
            }

            Save();

        }

        /// <summary>
        /// Получить список подзадач.
        /// </summary>
        /// <param name="nameTask"> Имя задачи у которй требуется взять список подзадач. </param>
        /// <returns></returns>
        public ReadOnlyCollection<Task> GetSubTasks(Task task)
        {
            if (task == null)
            {
                throw new ArgumentNullException("Задача не может быть null!", nameof(task));
            }

            return task.SubTasks?.AsReadOnly();
        }

        /// <summary>
        /// пометить подзадачу как выполненную.
        /// </summary>
        /// <param name="userController"> Контроллер пользователя. </param>
        /// <param name="task"> Задача, у которой мы хотим выполнить подзадачу. </param>
        /// <param name="subTask"> Подзадача, которую нужно пометить как выполненную. </param>
        public void PassSubTask(UserController userController, Task task, Task subTask)
        {
            #region Проверки
            if (userController == null)
            {
                throw new ArgumentNullException("Контроллер пользователя не может быть null!", nameof(userController));
            }
            if (task == null)
            {
                throw new ArgumentNullException("Задача не может быть null!", nameof(task));
            }
            if (userController.TaskOfUser == null)
            {
                throw new ArgumentException("У пользователя нету активных задач!");
            }
            if (userController.TaskOfUser.Name != task.Name)
            {
                throw new ArgumentException($"У пользователя активна другая задача: \"{userController.TaskOfUser.Name}\"");
            }
            if (task.ExecutorsNik != userController.NikOfUser)
            {
                throw new ArgumentException($"У задачи другой исполнитель: {task.ExecutorsNik}");
            }
            #endregion Проверки

            subTask.Status = Status.Complited;
            Save();
        }

        /// <summary>
        /// Получить набор досок из файла.
        /// </summary>
        /// <returns></returns>
        public List<Board> GetBoardsData()
        {
            return Load<Board>(fileName) ?? new List<Board>();
        }

        /// <summary>
        /// Сохранить информацию в файл.
        /// </summary>
        public void Save()
        {
            Save(fileName, Boards);
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
