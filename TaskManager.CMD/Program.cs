using System;
using TaskManager.BL.Controller;
using TaskManager.BL.Model;

namespace TaskManager.CMD
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Вас приветствует приложение TaskManager!");

            Console.Write("Введите ваш ник: ");
            var nikName = Console.ReadLine();

            var userController = new UserController(nikName);
            if (userController.IsNewUser)
            {
                Console.Write("Введите ваше имя: ");
                var name = Console.ReadLine();
                userController.SetNewUser(name);
            }

            Menu.MainMenu(userController);
        }

        /// <summary>
        /// Консольный интерфейс добавления задач.
        /// </summary>
        /// <param name="boardController"></param>
        public static void AddTasks(BoardController boardController)
        {
            while (true)
            {
                Console.WriteLine("Вы хотите ввести задачу?");

                if (Console.ReadLine() == "да")
                {
                    Console.Write("Введите нименование задачи: ");
                    var nameTask = Console.ReadLine();

                    Console.WriteLine("Введите дату сдачи: ");
                    var date = TryParseDate();

                    Console.WriteLine("Введите приоритет задачи");
                    var priority = TryParsePriority();

                    boardController.AddTask(new Task(nameTask, date, priority));
                    Console.WriteLine(boardController.Board);
                }
                else
                {
                    break;
                }
            }

        }

        #region Parse Function
        /// <summary>
        /// Проверяет является ли строка преобразуемой к Priority.
        /// </summary>
        /// <returns> Приоритет. </returns>
        public static Priority TryParsePriority()
        {
            while (true)
            {
                if (Task.PriorityParse(Console.ReadLine(), out Priority result))
                {
                    return result;
                }
                else
                {
                    Console.WriteLine("Вы ввели не правильный приоритет, формат приоритета:\n",
                                      "P1 or first\n",
                                      "P2 or second\n",
                                      "P3 or third\n",
                                      "P4 or nothing");
                }
            }
        }

        /// <summary>
        /// Проверяет является ли строка преобразуемой к DataTime.
        /// </summary>
        /// <returns> Дату. </returns>
        public static DateTime TryParseDate()
        {
            while (true)
            {
                if (DateTime.TryParse(Console.ReadLine(), out DateTime result))
                {
                    return result;
                }
                else
                {
                    Console.WriteLine("Дата должна иметь формат \"дд.мм.гггг\"");
                }
            }
        }
        #endregion Parse Function
    }

    public class Menu
    {
        /// <summary>
        /// Меню взаимодействия с пользователём.
        /// </summary>
        /// <param name="userController"></param>
        public static void MainMenu(UserController userController)
        {
            BoardController boardController;
            while (true)
            {
                Console.WriteLine("\n\nВыберите действие!\n\n" +
                                  "[1] Выбрать доску с дадачами.\n" +
                                  "[0] Выйти.");
                switch (Console.ReadLine())
                {
                    case "0":
                        return;
                    case "1":
                        boardController = GetBoardController(userController);
                        Console.WriteLine(boardController.Board);
                        break;

                }

            }
        }

        /// <summary>
        /// Создать контроллер доски.
        /// </summary>
        /// <param name="userController"> Контроллер доски. </param>
        /// <returns></returns>
        static BoardController GetBoardController(UserController userController)
        {
            if (userController == null)
            {
                throw new ArgumentNullException("Контроллер пользоваьеля не может быть null.", nameof(userController));
            }
            if (userController.User?.Boards == null || userController.User?.Boards?.Capacity == 0)
            {
                return NewBoardController(userController);
            }
            else
            {
                return GetFromUserBoard(userController);
            }
        }

        #region NewBoardController && GetFromUserBoard
        /// <summary>
        /// Создание новой доски.
        /// </summary>
        /// <param name="userController"> Контроллер новой доскии. </param>
        /// <returns></returns>
        static BoardController NewBoardController(UserController userController)
        {
            while (true)
            {
                Console.Write("У вас нету ни единой доски.\n Хотите создать доску задач? ");
                switch (Console.ReadLine())
                {
                    case "д":
                    case "да":
                    case "y":
                    case "yes":
                        Console.Write("Введите имя доски: ");
                        var nameBoard = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(nameBoard))
                        {
                            Console.WriteLine("Имя доски не может быть пустым!");
                            break;
                        }
                        userController.AddBoard(nameBoard);
                        return new BoardController(nameBoard);
                    case "н":
                    case "нет":
                    case "n":
                    case "no":
                        return null;
                    default:
                        Console.WriteLine("Нету такой команды!");
                        break;
                }
            }
        }

        /// <summary>
        /// Создание контроллера доски из списка пользователя.
        /// </summary>
        /// <param name="userController"> Контроллер доски. </param>
        /// <returns></returns>
        static BoardController GetFromUserBoard(UserController userController)
        {
            Console.WriteLine("Выберите доску");
            int i = 0;
            foreach (var board in userController.Boards)
            {
                Console.WriteLine($"[{++i}] {board}");
            }
            while (true)
            {
                int number = Int32.Parse(Console.ReadLine());
                if (number < 1 || number > i)
                {
                    Console.WriteLine("Такого номера нету!");
                    continue;
                }
                else
                {
                    return new BoardController(userController.Boards[number - 1]);
                }
            }
        }
        #endregion NewBoardController && GetFromUserBoard

    }
}
