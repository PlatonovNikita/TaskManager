using System;
using System.Diagnostics;
using TaskManager.BL.Controller;
using TaskManager.BL.Model;

namespace TaskManager.CMD
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
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
    }

    public class Menu
    {
        /// <summary>
        /// Меню взаимодействия с пользователём.
        /// </summary>
        /// <param name="userController"></param>
        public static void MainMenu(UserController userController)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            BoardController boardController = null;
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Выберите действие!\n\n"
                                  + "[1] Выбрать доску с задачами.\n"
                                  + "[0] Выйти.");
                switch (Console.ReadLine())
                {
                    case "0":
                        return;
                    case "1":
                        boardController = GetBoardController(userController);
                        if (boardController != null)
                        {
                            BoardMenu(userController, boardController);
                            return;
                        }
                        break;
                }
            }
        }

        static void BoardMenu(UserController userController, BoardController boardController)
        {
            Console.Clear();
            while (true)
            {
                Console.WriteLine("Выберите действие!\n\n"
                                  + "[1] Выбрать доску с задачами.\n"
                                  + "[2] Вывеси содержимое доски.\n"
                                  + "[3] Добавить задачу.\n"
                                  + "[4] Удалить задачу.\n"
                                  + "[5] Сдать выполненную задачу.\n"
                                  + "[0] Выйти.");

                switch (Console.ReadLine())
                {
                    case "0":
                        return;
                    case "1":
                        boardController = GetBoardController(userController)??boardController;
                        Console.Clear();
                        break;
                    case "2":
                        TasksMenu(userController, boardController);
                        Console.Clear();
                        break;
                    case "3":
                        Console.Clear();
                        AddTaskToBoard(boardController);
                        break;
                    case "4":
                        Console.Clear();
                        DelTaskFromBoard(boardController);
                        break;
                    case "5":
                        Console.Clear();
                        if (userController.TaskOfUser == null)
                        {
                            Console.WriteLine("У вас нету активных задач!");
                        }
                        else 
                        {
                            PassTask(userController, boardController);
                        }
                        break;
                }
            }
        }

        static void TasksMenu(UserController userController, BoardController boardController)
        {
            while (true)
            {
                Console.Clear();
                int i = 0;
                foreach (var task in boardController.Tasks)
                {
                    Console.WriteLine($"[{++i}] {task}");
                }
                Console.WriteLine("[0] Назад.");
                if (Int32.TryParse(Console.ReadLine(), out int input)){
                    if (input == 0)
                    {
                        return;
                    }
                    if (input <= i)
                    {
                        TaskMenu(userController, boardController, --input);
                        continue;
                    }
                }
            }
        }

        static void TaskMenu(UserController userController, BoardController boardController, int input)
        {
            Console.Clear();
            while (true)
            {
                Task task = boardController.Tasks[input];
                Console.WriteLine(task);
                Console.WriteLine("[1] Взять/Сдать эту задачу"
                                  + "[2] Удалить эту задачу."
                                  + "[3] Назад.");
                switch (Console.ReadLine())
                {
                    case "1":
                        Console.Clear();
                        if(userController.NikOfUser != task.ExecutorsNik)
                        {
                            TakeTask(userController, boardController, task.Name);
                        }
                        else
                        {
                            PassTask(userController, boardController, task.Name);
                        }
                        break;
                    case "2":
                        Console.Clear();
                        Console.WriteLine($"Вы действительно хотите удалить задачу \"{boardController.Tasks[input].Name}\"");
                        if (YesOrNo())
                        {
                            boardController.DelTask(task.Name);
                            return;
                        }
                        break;
                    case "3":
                        return;
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
            Console.Clear();
            if (userController == null)
            {
                throw new ArgumentNullException("Контроллер пользоваьеля не может быть null.", nameof(userController));
            }
            if (userController.Boards.Count == 0)
            {
                Console.WriteLine("У вас нету ни единой доски.\n");
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

            Console.Write("Хотите создать доску задач?");
            if (YesOrNo())
            {
                Console.Write("Введите имя доски: ");
                var nameBoard = TryParseName();

                try
                {
                    userController.AddBoard(nameBoard);
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);//"Такая доска уже есть!"
                }
                return new BoardController(nameBoard);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Создание контроллера доски из списка пользователя.
        /// </summary>
        /// <param name="userController"> Контроллер доски. </param>
        /// <returns></returns>
        static BoardController GetFromUserBoard(UserController userController)
        {
            Console.WriteLine("Выберите доску.\n");
            int i = 0;
            foreach (var board in userController.Boards)
            {
                Console.WriteLine($"[{++i}] {board}");
            }
            Console.WriteLine("[new] Добавить доску.\n"
                              + "[0] Назад.");

            while (true)
            {
                var input = Console.ReadLine();

                if (input == "new")
                {
                    return NewBoardController(userController);
                }
                if (Int32.TryParse(input, out int number))
                {
                    if (number == 0)
                    {
                        return null;
                    }
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
                else
                {
                    Console.WriteLine("Такого номера нету!");
                }
            }
        }
        #endregion NewBoardController && GetFromUserBoard

        /// <summary>
        /// Консольный интерфейс добавления задач.
        /// </summary>
        /// <param name="boardController"></param>
        static void AddTaskToBoard(BoardController boardController)
        {
            Console.Clear();

            Console.Write("Введите нименование задачи: ");
            var nameTask = TryParseName();

            Console.WriteLine("Введите дату сдачи: ");
            var date = TryParseDate();

            Console.WriteLine("Введите приоритет задачи");
            var priority = TryParsePriority();

            try
            {
                boardController.AddTask(new Task(nameTask, date, priority));
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// Удалить задачу с доски.
        /// </summary>
        /// <param name="boardController"></param>
        static void DelTaskFromBoard(BoardController boardController)
        {
            Console.Clear();
            Console.WriteLine("Введите имя задачи.");

            try
            {
                boardController.DelTask(TryParseName());
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        #region Parse Function
        /// <summary>
        /// Проверяет не является ли строка пустой.
        /// </summary>
        /// <returns></returns>
        public static string TryParseName()
        {
            while (true)
            {
                var name = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(name))
                {
                    return name;
                }
                else
                {
                    Console.WriteLine("Имя не может быть пустым!");
                }
            }
        }
        /// <summary>
        /// Проверяет является ли строка преобразуемой к Priority.
        /// </summary>
        /// <returns> Приоритет. </returns>
        public static Priority TryParsePriority()
        {
            while (true)
            {
                if (BoardController.PriorityParse(Console.ReadLine(), out Priority result))
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
                if (DateTime.TryParse(Console.ReadLine(), out DateTime result) && result >= DateTime.Now)
                {
                    return result;
                }
                else
                {
                    Console.WriteLine("Дата должна быть актуальной и иметь формат \"дд.мм.гггг\"");
                }
            }
        }

        /// <summary>
        /// Просит согласия пользователя.
        /// </summary>
        /// <returns></returns>
        static bool YesOrNo()
        {
            while (true)
            {
                switch (Console.ReadLine())
                {
                    case "д":
                    case "да":
                    case "Д":
                    case "Да":
                    case "y":
                    case "yes":
                    case "Y":
                    case "Yes":
                        return true;
                    case "н":
                    case "нет":
                    case "Н":
                    case "Нет":
                    case "n":
                    case "no":
                    case "N":
                    case "No":
                        return false;
                    default:
                        Console.WriteLine("Нету такой команды!");
                        break;
                }
            }
        }
        #endregion Parse Function

        /// <summary>
        /// Взять задачу на выполнение.
        /// </summary>
        /// <param name="userController"> Контроллер пользователя, который будет выполнять задачу. </param>
        /// <param name="boardController"> Контроллер доски с текущей задачей. </param>
        /// <param name="nameTask"> Наименование текущей задачи. </param>
        static void TakeTask(UserController userController, BoardController boardController, string nameTask)
        {
            try
            {
                boardController.TakeTask(userController, nameTask);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        /// <summary>
        /// Сдать текущую задачу.
        /// </summary>
        /// <param name="userController"> Контроллер пользователя, который сдаёт задачу. </param>
        /// <param name="boardController"> Контроллер доски с текущей задачей. </param>
        /// <param name="nameTask"> Наименованиие текущей задачи. </param>
        static void PassTask(UserController userController, BoardController boardController, string nameTask)
        {
            try
            {
                boardController.PassTask(userController, nameTask);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Сдать текущую задачу.
        /// </summary>
        /// <param name="userController"> Контроллер пользователя, который сдаёт задачу. </param>
        /// <param name="boardController"> Контроллер доски с текущей задачей. </param>
        static void PassTask(UserController userController, BoardController boardController)
        {
            PassTask(userController, boardController, userController.TaskOfUser.Name);
        }
    }
}
