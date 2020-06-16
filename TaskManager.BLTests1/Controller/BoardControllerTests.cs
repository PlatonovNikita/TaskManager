using TaskManager.BL.Controller;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using TaskManager.BL.Model;

namespace TaskManager.BL.Controller.Tests
{
    [TestClass()]
    public class BoardControllerTests
    {
        [TestMethod()]
        public void AddTaskTest()
        {
            // Arrange
            var nameBoard = Guid.NewGuid().ToString();
            var nameTask = "Name";
            var data = DateTime.Parse("01.01.2023");

            // Act
            var controller = new BoardController(nameBoard);
            controller.AddTask(new Model.Task(nameTask, data));
            var controller2 = new BoardController(nameBoard);
            var task = controller.Tasks.First();

            // Assert
            Assert.IsNull(task.ExecutorsNik);
            Assert.AreEqual(task.DeadLine, data);
            Assert.AreEqual(task.Name, nameTask);
            Assert.AreEqual(task.Priority, Model.Priority.P4);
            Assert.AreEqual(task.Status, Model.Status.NotPerformed);
            Assert.IsNull(task.SubTasks);
            Assert.ThrowsException<ArgumentException>(() => controller.AddTask(new Model.Task(nameTask, data)));
        }

        [TestMethod()]
        public void DelTaskTest()
        {
            // Arrange
            var nameBoard = Guid.NewGuid().ToString();
            var nameTask = "Name";
            var data = DateTime.Parse("01.01.2023");

            // Act
            var controller = new BoardController(nameBoard);
            controller.AddTask(new Model.Task(nameTask, data));
            controller.DelTask(nameTask);
            var controller2 = new BoardController(nameBoard);

            // Assert
            Assert.IsNotNull(controller2.Tasks);
            Assert.AreEqual(controller2.Tasks.Count, 0);
            Assert.ThrowsException<ArgumentException>(() => controller2.DelTask(nameTask));
        }

        [TestMethod()]
        public void TakeTaskTest()
        {
            // Arrange
            var userNik = Guid.NewGuid().ToString();
            var boardName = Guid.NewGuid().ToString();
            var taskName = Guid.NewGuid().ToString();

            // Act
            var userController = new UserController(userNik);
            var userController2 = new UserController(Guid.NewGuid().ToString());
            var boardController = new BoardController(boardName);
            boardController.AddTask(new Model.Task(taskName, DateTime.Now.AddDays(1)));
            boardController.TakeTask(userController, taskName);
            var boardController2 = new BoardController(boardName);

            // Assert
            Assert.AreEqual(userController.TaskOfUser.Name, taskName);
            Assert.AreEqual(boardController2.Tasks.First().Status, Model.Status.Performed);
            Assert.AreEqual(boardController.IsTakes, false);
            Assert.AreEqual(boardController2.IsTakes, false);
            Assert.ThrowsException<ArgumentException>(() => boardController2.TakeTask(userController2, Guid.NewGuid().ToString()));
            Assert.ThrowsException<ArgumentException>(() => boardController2.TakeTask(userController, taskName));
            Assert.ThrowsException<ArgumentException>(() => boardController2.TakeTask(userController2, taskName));
            Assert.ThrowsException<ArgumentException>(() => userController2.PassTask(boardController));
        }

        [TestMethod()]
        public void PassTaskTest()
        {
            // Arrange
            var userNik = Guid.NewGuid().ToString();
            var boardName = Guid.NewGuid().ToString();
            var taskName = Guid.NewGuid().ToString();

            // Act
            var userController = new UserController(userNik);
            var userController2 = new UserController(Guid.NewGuid().ToString());
            var boardController = new BoardController(boardName);
            boardController.AddTask(new Model.Task(taskName, DateTime.Now.AddDays(1)));
            boardController.TakeTask(userController, taskName);

            // Assert
            Assert.ThrowsException<ArgumentException>(() => boardController.PassTask(userController, Guid.NewGuid().ToString()));
            Assert.ThrowsException<ArgumentException>(() => boardController.PassTask(userController2, taskName));

            // Act
            boardController.PassTask(userController, taskName);
            var boardController2 = new BoardController(boardName);

            // Assert
            Assert.IsNull(userController.TaskOfUser);
            Assert.AreEqual(boardController2.Tasks.First().Status, Status.Complited);
            Assert.IsFalse(boardController2.IsPasses);
            Assert.IsFalse(boardController.IsPasses);
            Assert.ThrowsException<ArgumentException>(() => boardController2.TakeTask(userController2, taskName));

        }

        [TestMethod()]
        public void AddSubTaskTest()
        {
            // Arrange
            var nameBoard = Guid.NewGuid().ToString();
            var nameTask = "Name";
            var data = DateTime.Parse("01.01.2023");
            var nameSubTask = "NameSubTask";

            // Act
            var controller = new BoardController(nameBoard);
            controller.AddTask(new Model.Task(nameTask, data));
            var task = controller.Tasks.First(t => t.Name == nameTask);
            controller.AddSubTask(task, new Task(nameSubTask, data));
            var controller2 = new BoardController(nameBoard);

            // Assert
            Assert.IsNotNull(controller2.GetSubTasks(task)?.First(s => s.Name == nameSubTask));
            var subTask = controller2.GetSubTasks(task)?.First(s => s.Name == nameSubTask);
            Assert.ThrowsException<ArgumentException>(() => controller2.AddSubTask(task, new Task(nameSubTask, data)));
            Assert.AreEqual(subTask.DeadLine, data);
            Assert.AreEqual(subTask.Priority, Model.Priority.P4);
            Assert.AreEqual(subTask.Status, Status.Performed);
            Assert.IsNull(subTask.SubTasks);
        }

        [TestMethod()]
        public void DelSubTaskTest()
        {
            // Arrange
            var nameBoard = Guid.NewGuid().ToString();
            var nameTask = "Name";
            var data = DateTime.Parse("01.01.2023");
            var nameSubTask = "NameSubTask";

            // Act
            var controller = new BoardController(nameBoard);
            controller.AddTask(new Model.Task(nameTask, data));
            var task = controller.Tasks.First(t => t.Name == nameTask);
            controller.AddSubTask(task, new Task(nameSubTask, data));
            controller.DelSubTask(task, nameSubTask);
            var controller2 = new BoardController(nameBoard);

            // Assert
            Assert.IsNull(controller2.GetSubTasks(task));
            Assert.ThrowsException<ArgumentNullException>(() => controller2.DelSubTask(task, nameSubTask));
        }
    }
}