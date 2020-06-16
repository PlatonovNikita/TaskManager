using System;
using TaskManager.BL.Controller;
using TaskManager.BL.Model;

namespace TaskManager.CMD
{
    public abstract class MenuFunction
    {
        /// <summary>
        /// Создать контроллер доски.
        /// </summary>
        /// <param name="userController"> Контроллер доски. </param>
        /// <returns></returns>
        protected static BoardController GetBoardController(UserController userController)
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

        /// <summary>
        /// Создание новой доски.
        /// </summary>
        /// <param name="userController"> Контроллер новой доскии. </param>
        /// <returns></returns>
        protected static BoardController NewBoardController(UserController userController)
        {

            Console.Write("Хотите создать доску задач?");
            if (YesOrNo())
            {
                var nameBoard = TryParseName("доски");

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

            return null;
        }

        /// <summary>
        /// Создание контроллера доски из списка пользователя.
        /// </summary>
        /// <param name="userController"> Контроллер доски. </param>
        /// <returns></returns>
        protected static BoardController GetFromUserBoard(UserController userController)
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
                    if (number >= 1 && number <= i)
                    {
                        return new BoardController(userController.Boards[number - 1]);
                    }
                }

                Console.WriteLine("Такого номера нету!");
            }
        }

        /// <summary>
        /// Консольный интерфейс добавления задач.
        /// </summary>
        /// <param name="boardController"></param>
        protected static void AddTaskToBoard(BoardController boardController, IPusher pusher)
        {
            Console.Clear();

            var nameTask = TryParseName("задачи");
            var date = TryParseDate();
            var priority = TryParsePriority();

            if (pusher == null)
            {
                throw new ArgumentNullException("Следует передать интерфейс добавления задачи!");
            }
            try
            {
                pusher.AddTask(boardController ,new Task(nameTask, date, priority));
            }
            catch (ArgumentException ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Удалить задачу с доски.
        /// </summary>
        /// <param name="boardController"></param>
        protected static void DelTaskFromBoard(BoardController boardController, IPusher pusher)
        {
            Console.Clear();

            if(pusher == null)
            {
                throw new ArgumentNullException("Следует передать интерфейс удаления задачи!");
            }
            try
            {
                pusher.DelTask(boardController, TryParseName("задачи"));
            }
            catch (ArgumentException ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }

        #region Parse Function
        /// <summary>
        /// Проверяет не является ли строка пустой.
        /// </summary>
        /// <returns></returns>
        protected static string TryParseName(string massege)
        {
            while (true)
            {
                Console.Write($"Введите имя {massege}: ");
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
        protected static Priority TryParsePriority()
        {
            while (true)
            {
                Console.Write("Введите приоритет задачи: ");

                if (BoardController.PriorityParse(Console.ReadLine(), out Priority result))
                {
                    return result;
                }
                else
                {
                    Console.WriteLine("Вы ввели не правильный приоритет, формат приоритета:\n" + 
                                      "P1 or first\n" +
                                      "P2 or second\n" +
                                      "P3 or third\n" +
                                      "P4 or nothing");
                }
            }
        }

        /// <summary>
        /// Проверяет является ли строка преобразуемой к DataTime.
        /// </summary>
        /// <returns> Дату. </returns>
        protected static DateTime TryParseDate()
        {
            while (true)
            {
                Console.Write("Введите дату сдачи: ");

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
        protected static bool YesOrNo()
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
        protected static void TakeTask(UserController userController, BoardController boardController, string nameTask)
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
        protected static void PassTask(UserController userController, BoardController boardController, string nameTask)
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
        protected static void PassTask(UserController userController, BoardController boardController)
        {
            PassTask(userController, boardController, userController.TaskOfUser.Name);
        }

        protected static void PassSubTask(UserController userController, BoardController boardController, Task task, Task subTask)
        {
            try
            {
                boardController.PassSubTask(userController, task, subTask);
            }
            catch(ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
