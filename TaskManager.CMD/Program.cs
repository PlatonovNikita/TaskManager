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

            var controller = new UserController(nikName);
            if (controller.IsNewUser)
            {
                Console.Write("Введите ваше имя: ");
                var name = Console.ReadLine();
                controller.SetNewUser(name);
            }

            Console.WriteLine(controller.User);
            Console.ReadKey();
        }
    }
}
