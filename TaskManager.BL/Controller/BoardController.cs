using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using TaskManager.BL.Model;

namespace TaskManager.BL.Controller
{
    public class BoardController
    {
        public Board Board { get; }
        public bool IsNewBoard { get; } = false;

        public BoardController(string nameBoard)
        {
            if (string.IsNullOrWhiteSpace(nameBoard))
            {
                throw new ArgumentNullException("Наименованиие доски не может быть пустым!", nameof(nameBoard));
            }

            Board = GetBoardData();

            if(Board == null)
            {
                Board = new Board(nameBoard, new List<Task>());
                IsNewBoard = true;
                Save();
            }
            
        }

        public void AddTask(Task task)
        {
            if(task == null)
            {
                throw new ArgumentNullException("Задача не может быть null!", nameof(task));
            }

            if(Board.Tasks.SingleOrDefault(t => t == task))
            {
                throw new ArgumentException("Такая задача уже есть в списке задач!", nameof(task));
            }
            else
            {
                Board.Tasks.Add(task);
                Save();
            }
        }

        public Board GetBoardData()
        {
            var formatter = new BinaryFormatter();

            using (var fs = new FileStream("board.dat", FileMode.OpenOrCreate))
            {
                if(fs.Length > 0 && formatter.Deserialize(fs) is Board board)
                {
                    return board;
                }
                else
                {
                    return null;
                }
            }
        }
        public void Save()
        {
            var formatter = new BinaryFormatter();

            using (var fs = new FileStream("board.dat", FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, Board);
            }
        }
    }
}
