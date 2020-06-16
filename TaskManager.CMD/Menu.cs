using System;
using System.Collections.ObjectModel;
using System.Linq;
using TaskManager.BL.Controller;
using TaskManager.BL.Model;

namespace TaskManager.CMD
{
    public class Menu : MenuFunction
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
                        boardController = GetBoardController(userController) ?? boardController;
                        Console.Clear();
                        break;
                    case "2":
                        TasksMenu(userController, boardController);
                        Console.Clear();
                        break;
                    case "3":
                        Console.Clear();
                        AddTaskToBoard(boardController, new Tasker());
                        break;
                    case "4":
                        Console.Clear();
                        DelTaskFromBoard(boardController, new Tasker());
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
                    default:
                        Console.Clear();
                        Console.WriteLine("Такой команды нету!");
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
                if (Int32.TryParse(Console.ReadLine(), out int input))
                {
                    if (input == 0)
                    {
                        return;
                    }
                    if (input <= i)
                    {
                        TaskMenu(userController, boardController, boardController.Tasks[--input]);
                    }
                }
            }
        }

        static void TaskMenu(UserController userController, BoardController boardController, Task task)
        {
            Console.Clear();
            while (true)
            {
                Console.WriteLine(task);
                Console.WriteLine("[1] Взять/Сдать эту задачу.\n"
                                  + "[2] Удалить эту задачу.\n"
                                  + "[3] Вывести все подзадачи.\n"
                                  + "[4] Добавить подзадачу.\n"
                                  + "[5] Удалиь подзадачу.\n"
                                  + "[0] Назад.");
                switch (Console.ReadLine())
                {
                    case "1":
                        Console.Clear();
                        if (userController.NikOfUser != task.ExecutorsNik)
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
                        Console.WriteLine($"Вы действительно хотите удалить задачу \"{task.Name}\"");
                        if (YesOrNo())
                        {
                            boardController.DelTask(task.Name);
                            return;
                        }
                        break;
                    case "3":
                        SubTasksMenu(userController, boardController, task);
                        break;
                    case "4":
                        AddTaskToBoard(boardController, new Subtasker(task));
                        break;
                    case "5":
                        DelTaskFromBoard(boardController, new Subtasker(task));
                        break;
                    case "0":
                        return;
                    default:
                        Console.Clear();
                        Console.WriteLine("Такой команды нету!");
                        break;
                }
            }
        }

        static void SubTasksMenu(UserController userController, BoardController boardController, Task task)
        {
            if (boardController.GetSubTasks(task) == null)
            {
                Console.WriteLine("Подзадач нет");
                return;
            }

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"{task.Name}:\n");
                int i = 0;
                foreach (var subTask in boardController.GetSubTasks(task))
                {
                    Console.WriteLine($"[{++i}] {subTask}");
                }
                Console.WriteLine("[0] Назад.");

                if (Int32.TryParse(Console.ReadLine(), out int input))
                {
                    if (input == 0)
                    {
                        return;
                    }
                    if (input <= i)
                    {
                        SubTaskMenu(userController, boardController, task, boardController.GetSubTasks(task).ElementAt(--input));
                    }
                }
            }
        }

        static void SubTaskMenu(UserController userController, BoardController boardController, Task task, Task subTask)
        {
            Console.Clear();
            while (true)
            {
                Console.WriteLine(subTask);
                Console.WriteLine("[1] Отметить как выполненную.\n"
                                  + "[2] Удалить эту подзадачу.\n"
                                  + "[0] Назад.");
                switch (Console.ReadLine())
                {
                    case "1":
                        Console.Clear();
                        PassSubTask(userController, boardController, task, subTask);
                        break;
                    case "2":
                        Console.Clear();
                        Console.WriteLine($"Вы действительно хотите удалить задачу \"{task.Name}\"");
                        if (YesOrNo())
                        {
                            boardController.DelSubTask(task, subTask.Name);
                            return;
                        }
                        break;
                    case "0":
                        return;
                }

            }
        }
    }
}
