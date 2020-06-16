using System;
using TaskManager.BL.Controller;
using TaskManager.BL.Model;

namespace TaskManager.CMD
{
    internal class Tasker : IPusher
    {
        void IPusher.AddTask(BoardController boardController, Task task)
        {
            boardController.AddTask(task);
        }

        public void DelTask(BoardController boardController, string nameTask)
        {
            boardController.DelTask(nameTask);
        }
    }

    internal class Subtasker : IPusher
    {
        Task task { get; }

        public Subtasker(Task task)
        {
            this.task = task;
        }

        void IPusher.AddTask(BoardController boardController, Task subTask)
        {
            boardController.AddSubTask(task, subTask);
        }

        void IPusher.DelTask(BoardController boardController, string nameSubtask)
        {
            boardController.DelSubTask(task, nameSubtask);
        }
    }
}
