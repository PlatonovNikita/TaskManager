using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskManager.BL.Controller;
using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.BL.Controller.Tests
{
    [TestClass()]
    public class UserControllerTests
    {
        [TestMethod()]
        public void UserControllerTest()
        {
            // Arrange
            var userNik = Guid.NewGuid().ToString();

            // Act
            var controller = new UserController(userNik);

            // Assert
            Assert.AreEqual(controller.NikOfUser, userNik);
            Assert.AreEqual(controller.IsNewUser, true);
            Assert.AreEqual(controller.NameOfUser, "");
            Assert.IsNull(controller.TaskOfUser);
            Assert.IsNotNull(controller.Boards);
            
        }

        [TestMethod()]
        public void SetNewUserTest()
        {
            // Arrange
            var userNik = Guid.NewGuid().ToString();
            var name = Guid.NewGuid().ToString();

            // Act
            var controlller = new UserController(userNik);
            controlller.SetNewUser(name);

            // Assert
            Assert.AreEqual(controlller.NameOfUser, name);
        }

        [TestMethod()]
        public void AddBoardTest()
        {
            // Arrange
            var userNik = Guid.NewGuid().ToString();
            var board = Guid.NewGuid().ToString();

            // Act
            var controller = new UserController(userNik);
            controller.AddBoard(board);

            // Assert
            Assert.AreEqual(controller.Boards.Count, 1);
            Assert.AreEqual(controller.Boards[0], board);
            Assert.ThrowsException<ArgumentException>(() => controller.AddBoard(board));
            Assert.ThrowsException<ArgumentNullException>(() => controller.AddBoard(""));
        }

        [TestMethod()]
        public void SaveTest()
        {
            // Arrange
            var userNik = Guid.NewGuid().ToString();
            var userName = Guid.NewGuid().ToString();
            var boardName = Guid.NewGuid().ToString();

            // Act
            var controller = new UserController(userNik);
            controller.SetNewUser(userName);
            controller.AddBoard(boardName);
            var controller2 = new UserController(userNik);

            // Assert
            Assert.AreNotEqual(controller2.Boards.Count, 0);
            Assert.AreEqual(controller2.NameOfUser, userName);

        }
    }
}