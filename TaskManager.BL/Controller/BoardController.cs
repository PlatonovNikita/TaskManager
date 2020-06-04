using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using TaskManager.BL.Model;

namespace TaskManager.BL.Controller
{
    /// <summary>
    /// Контроллер доски задач.
    /// </summary>
    public class BoardController
    {
        public List<Board> Boards { get; }
        public Board Board { get; }
        public bool IsNewBoard { get; } = false;

        /// <summary>
        /// Создать контроллер.
        /// </summary>
        /// <param name="nameBoard"> Имя доски. </param>
        public BoardController(string nameBoard)
        {
            if (string.IsNullOrWhiteSpace(nameBoard))
            {
                throw new ArgumentNullException("Наименованиие доски не может быть пустым!", nameof(nameBoard));
            }

            Boards = GetBoardsData();
            Board = Boards.SingleOrDefault(b => b.Name == nameBoard);

            if(Board == null)
            {
                Board = new Board(nameBoard, new List<Task>());
                IsNewBoard = true;
                Save();
            }
            
        }

        /// <summary>
        /// Добавить задачу.
        /// </summary>
        /// <param name="task"> Задача. </param>
        public void AddTask(Task task)
        {
            if(task == null)
            {
                throw new ArgumentNullException("Задача не может быть null!", nameof(task));
            }
            if(Board.Tasks.Count(t => t.Name == task.Name) == 0)
            {
                Board.Tasks.Add(task);
                Save();
            }
            else
            {
                throw new ArgumentException("Такая задача уже есть в списке задач!", nameof(task));
            }
        }

        /// <summary>
        /// Получить набор досок из файла.
        /// </summary>
        /// <returns></returns>
        public List<Board> GetBoardsData()
        {
            var formatter = new BinaryFormatter();

            using (var fs = new FileStream("board.dat", FileMode.OpenOrCreate))
            {
                if(fs.Length > 0 && formatter.Deserialize(fs) is List<Board> boards)
                {
                    return boards;
                }
                else
                {
                    return new List<Board>();
                }
            }
        }

        /// <summary>
        /// Сохранить информацию в файл.
        /// </summary>
        public void Save()
        {
            var formatter = new BinaryFormatter();

            using (var fs = new FileStream("board.dat", FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, Boards);
            }
        }
    }
}
