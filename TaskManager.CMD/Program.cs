using System;
using TaskManager.BL.Controller;
using TaskManager.BL.Model;

namespace TaskManager.CMD
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Вас приветствует приложение TaskMAnager!");

            Console.Write("Введите ваш ник: ");
            var nikName = Console.ReadLine();

            var userController = new UserController(nikName);
            if (userController.IsNewUser)
            {
                Console.Write("Введите ваше имя: ");
                var name = Console.ReadLine();
                userController.SetNewUser(name);
            }

            Console.WriteLine(userController.User);

            Console.WriteLine("Пожалуйста, ведите наименование доски задач.");
            var nameBoard = Console.ReadLine();

            var boardController = new BoardController(nameBoard);

            if (boardController.IsNewBoard)
            {
                Console.Write("Введите нименование задачи: ");
                var nameTask = Console.ReadLine();

                Console.WriteLine("Введите дату сдачи: ");
                var date = TryParseDate();

                Console.WriteLine("Введите приоритет задачи");
                var priority = TryParsePriority();

                boardController.AddTask(new Task(nameTask, date, priority));
            }

            Console.WriteLine(boardController.Board);

            Console.ReadKey();
        }
        
        public static Priority TryParsePriority()
        {
            while (true)
            {
                if(Task.PriorityParse(Console.ReadLine(), out Priority result))
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
    }
}
