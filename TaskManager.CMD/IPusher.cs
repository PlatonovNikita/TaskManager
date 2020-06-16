using System;
using TaskManager.BL.Controller;
using TaskManager.BL.Model;

namespace TaskManager.CMD
{
    public interface IPusher
    {
        void AddTask(BoardController boardController, Task task);

        void DelTask(BoardController boardController, string nameTask);
    }
}
