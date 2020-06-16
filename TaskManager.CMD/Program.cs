using System;
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
}
