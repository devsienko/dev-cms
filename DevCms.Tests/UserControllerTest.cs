using System.Collections.Generic;
using System.Linq;
using DevCms.Areas.Admin.Controllers;
using DevCms.Db;
using DevCms.Models;
using DevCms.Util;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace DevCms.Tests
{
    public class UserControllerTest
    {
        [Fact]
        public void Index_Post_Test_InvalidModel()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.Users, GetUsers());

            var controller = new UsersController(mockRepo.Object);

            var model = new RegisterUserModel
            {
                //Email = "test email",
                Password = "123123",
                ConfirmPassword = "123123"
            };

            controller.ModelState.AddModelError("Email", "Required");
            var result = controller.Index(model);

            var viewResult = Assert.IsType<ViewResult>(result);
            var resultModel = Assert.IsAssignableFrom<RegisterUserModel>(
                viewResult.ViewData.Model);

            Assert.Null(resultModel.Email);
            Assert.Equal("123123", resultModel.Password);
            Assert.Equal("123123", resultModel.ConfirmPassword);

            mockRepo.Verify(db => db.SaveChanges(), Times.Never);
        }

        [Fact]
        public void Index_Post_Test()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.Users, GetUsers());

            var controller = new UsersController(mockRepo.Object);

            var model = new RegisterUserModel
            {
                Email = "test email",
                Password = "123123",
                ConfirmPassword = "123123"
            };

            Assert.Single(mockRepo.Object.Users);

            controller.Index(model);

            Assert.Equal(2, mockRepo.Object.Users.Count());
            Assert.Equal("test email", mockRepo.Object.Users.Last().Email);

            mockRepo.Verify(db => db.SaveChanges(), Times.Once);
        }

        [Fact]
        public void Index_Post_Test_UserExists()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.Users, GetUsers());

            var controller = new UsersController(mockRepo.Object);

            var model = new RegisterUserModel
            {
                Email = "test",
                Password = "123123",
                ConfirmPassword = "123123"
            };

            Assert.Single(mockRepo.Object.Users);

            var result = controller.Index(model);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<RegisterUserModel>(
                viewResult.ViewData.Model);

            Assert.Equal("Пользователь с таким Email уже существует.", viewResult.ViewData.ModelState["Email"].Errors[0].ErrorMessage);

            mockRepo.Verify(db => db.SaveChanges(), Times.Never);
        }

        [Fact]
        public void Edit_Test_InvalidId()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.Users, GetUsers());

            var controller = new UsersController(mockRepo.Object);

            var result = controller.Edit(-11);
            Assert.IsType<NotFoundResult>(result);

            result = controller.Edit(0);
            Assert.IsType<NotFoundResult>(result);

            result = controller.Edit(11);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Edit_Test()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.Users, GetUsers());

            var controller = new UsersController(mockRepo.Object);

            var result = controller.Edit(1);
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<EditUserModel>(
                viewResult.ViewData.Model);

            Assert.Equal(1, model.Id);
            Assert.Equal("test", model.Email);
        }

        [Fact]
        public void Edit_Test_Post()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.Users, GetUsers());

            var controller = new UsersController(mockRepo.Object);

            var model = new EditUserModel
            {
                Id = 1,
                Email = "email2",
                Password = "999999",
                ConfirmPassword = "999999"
            };

            Assert.Single(mockRepo.Object.Users);
            Assert.Equal(1, mockRepo.Object.Users.First().Id);
            Assert.Equal("test", mockRepo.Object.Users.First().Email);

            var result = controller.Edit(model);
            Assert.IsType<ViewResult>(result);

            Assert.Single(mockRepo.Object.Users);
            Assert.Equal(1, mockRepo.Object.Users.First().Id);
            Assert.Equal("email2", mockRepo.Object.Users.First().Email);
            mockRepo.Verify(db => db.SaveChanges(), Times.Once());
        }

        [Fact]
        public void Edit_Post_Test_UserExists()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.Users, GetUsers2());

            var controller = new UsersController(mockRepo.Object);

            var model = new EditUserModel
            {
                Id = 1,
                Email = "test2",
                Password = "123123",
                ConfirmPassword = "123123"
            };

            var result = controller.Edit(model);

            var viewResult = Assert.IsType<ViewResult>(result);
            var resultModel = Assert.IsAssignableFrom<EditUserModel>(
                viewResult.ViewData.Model);

            Assert.Equal("test2", resultModel.Email);
            Assert.Equal("Пользователь с таким Email уже существует.", viewResult.ViewData.ModelState["Email"].Errors[0].ErrorMessage);

            mockRepo.Verify(db => db.SaveChanges(), Times.Never);
        }

        [Fact]
        public void Edit_Post_Test_PasswordUpdated()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.Users, GetUsers());

            var controller = new UsersController(mockRepo.Object);

            var model = new EditUserModel
            {
                Id = 1,
                Email = "test2",
                Password = "123123",
                ConfirmPassword = "123123"
            };
            var oldPassword = mockRepo.Object.Users.First().Password;
            controller.Edit(model);
            var newPassword = mockRepo.Object.Users.First().Password;

            Assert.NotEqual(oldPassword, newPassword);
            mockRepo.Verify(db => db.SaveChanges(), Times.Once);
        }

        [Fact]
        public void Edit_Post_Test_PasswordNotUpdated()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.Users, GetUsers());

            var controller = new UsersController(mockRepo.Object);

            var model = new EditUserModel
            {
                Id = 1,
                Email = "test2",
                Password = PasswordHelper.SixAsterix,
                ConfirmPassword = PasswordHelper.SixAsterix
            };
            var oldPassword = mockRepo.Object.Users.First().Password;
            controller.Edit(model);
            var newPassword = mockRepo.Object.Users.First().Password;

            Assert.Equal(oldPassword, newPassword);
            mockRepo.Verify(db => db.SaveChanges(), Times.Once);
        }

        private List<User> GetUsers()
        {
            var result = new List<User>
            {
                new User
                {
                    Id = 1,
                    Email = "test",
                    Password = "******"
                }
            };
            return result;
        }
        private List<User> GetUsers2()
        {
            var result = new List<User>
            {
                new User
                {
                    Id = 1,
                    Email = "test",
                    Password = "123123"
                },
                new User
                {
                    Id = 2,
                    Email = "test2",
                    Password = "123123"
                }
            };
            return result;
        }
    }
}
