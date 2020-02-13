using System;
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
    public class AccountControllerTest
    {
        [Fact]
        public void Login_Test_InvalidModel()
        {
            var mockRepo = new Mock<DevCmsDb>();

            var mockAuth = new Mock<IAuthProvider>();
            var controller = new AccountController(mockRepo.Object, mockAuth.Object);

            controller.ModelState.AddModelError("Email", "Required");
            var model = new LoginModel
            {
                Email = "",
                Password = "123123"
            };
            var result = controller.Login(model);
            var viewResult = Assert.IsType<ViewResult>(result);
            var resultModel = Assert.IsAssignableFrom<LoginModel>(
                viewResult.ViewData.Model);

            Assert.Equal("", resultModel.Email);
            Assert.Equal("123123", resultModel.Password);
        }

        [Fact]
        public void Login_Test_UserNotFound_IncorrectLogin()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.Users, GetUsers());

            var mockAuth = new Mock<IAuthProvider>();
            var controller = new AccountController(mockRepo.Object, mockAuth.Object);

            var model = new LoginModel
            {
                Email = "Oleg",
                Password = "123123"
            };
            var result = controller.Login(model);
            var viewResult = Assert.IsType<ViewResult>(result);
            var resultModel = Assert.IsAssignableFrom<LoginModel>(
                viewResult.ViewData.Model);

            Assert.Equal("Oleg", resultModel.Email);
            Assert.Equal("123123", resultModel.Password);
            Assert.Equal("Некорректный логин и (или) пароль", 
                viewResult.ViewData.ModelState[""].Errors[0].ErrorMessage);
        }

        [Fact]
        public void Login_Test_LoginAttempt_FirstTime()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.Users, GetUsers());

            var mockAuth = new Mock<IAuthProvider>();
            var controller = new AccountController(mockRepo.Object, mockAuth.Object);

            var model = new LoginModel
            {
                Email = "test",
                Password = "111111"
            };

            Assert.Single(mockRepo.Object.Users);
            var currentUser = mockRepo.Object.Users.First();
            Assert.Null(currentUser.FailedLoginAttemptDateTime);
            Assert.Equal(0, currentUser.FailedLoginAttemptCounter);

            //first time
            var currDate = DateTime.UtcNow;


            var result = controller.Login(model);
            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.Equal(string.Format("Неверный пароль. Осталось попыток: {0}.", CommonSettings.FailedLoginAttemptLimit - 1),
                viewResult.ViewData.ModelState[""].Errors[0].ErrorMessage);

            Assert.True(currentUser.FailedLoginAttemptDateTime > currDate.AddSeconds(-3) 
                        && currentUser.FailedLoginAttemptDateTime < currDate.AddSeconds(6));
            Assert.NotNull(currentUser.FailedLoginAttemptDateTime);
            Assert.Equal(1, currentUser.FailedLoginAttemptCounter);

            Assert.Single(mockRepo.Object.Users);

            mockRepo.Verify(db => db.SaveChanges(), Times.Once);

            //after FailedLoginAttemptDateTime was expired
            currentUser.FailedLoginAttemptDateTime = DateTime.UtcNow.AddMilliseconds(-CommonSettings.AttemptMinute-1);
            currDate = DateTime.UtcNow;

            result = controller.Login(model);
            viewResult = Assert.IsType<ViewResult>(result);

            Assert.Equal(string.Format("Неверный пароль. Осталось попыток: {0}.", CommonSettings.FailedLoginAttemptLimit - 1),
                viewResult.ViewData.ModelState[""].Errors[0].ErrorMessage);

            Assert.True(currentUser.FailedLoginAttemptDateTime > currDate.AddSeconds(-3)
                        && currentUser.FailedLoginAttemptDateTime < currDate.AddSeconds(6));
            Assert.NotNull(currentUser.FailedLoginAttemptDateTime);
            Assert.Equal(1, currentUser.FailedLoginAttemptCounter);

            Assert.Single(mockRepo.Object.Users);

            mockRepo.Verify(db => db.SaveChanges(), Times.Once);
        }

        [Fact]
        public void Login_Test_LoginAttempt()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.Users, GetUsers());

            var mockAuth = new Mock<IAuthProvider>();
            var controller = new AccountController(mockRepo.Object, mockAuth.Object);

            var model = new LoginModel
            {
                Email = "test",
                Password = "111111"
            };
            var attemptsCount = 2;
            Assert.Single(mockRepo.Object.Users);
            var currentUser = mockRepo.Object.Users.First();
            var lastAttemptDate = DateTime.UtcNow.AddSeconds(-5);
            currentUser.FailedLoginAttemptDateTime = lastAttemptDate;
            currentUser.FailedLoginAttemptCounter = attemptsCount;

            var result = controller.Login(model);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(string.Format("Неверный пароль. Осталось попыток: {0}.", CommonSettings.FailedLoginAttemptLimit - attemptsCount - 1),
                viewResult.ViewData.ModelState[""].Errors[0].ErrorMessage);

            Assert.Equal(lastAttemptDate, currentUser.FailedLoginAttemptDateTime);
            Assert.Equal(attemptsCount + 1, currentUser.FailedLoginAttemptCounter);

            Assert.Single(mockRepo.Object.Users);

            mockRepo.Verify(db => db.SaveChanges(), Times.Once);
        }

        [Fact]
        public void Login_Test_LoginAttempt_LastAttempt()
        {
            var mockRepo = new Mock<DevCmsDb>();
            var mockAuth = new Mock<IAuthProvider>();
            mockRepo.SetupDbSetMock(db => db.Users, GetUsers());

            var controller = new AccountController(mockRepo.Object, mockAuth.Object);

            var model = new LoginModel
            {
                Email = "test",
                Password = "111111"
            };
            var attemptsCount = CommonSettings.FailedLoginAttemptLimit - 2;
            Assert.Single(mockRepo.Object.Users);
            var currentUser = mockRepo.Object.Users.First();
            var lastAttemptDate = DateTime.UtcNow.AddSeconds(-5);
            currentUser.FailedLoginAttemptDateTime = lastAttemptDate;
            currentUser.FailedLoginAttemptCounter = attemptsCount;

            var result = controller.Login(model);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Неверный пароль. У вас осталась 1 попытка.", viewResult.ViewData.ModelState[""].Errors[0].ErrorMessage);

            Assert.Equal(lastAttemptDate, currentUser.FailedLoginAttemptDateTime);
            Assert.Equal(attemptsCount + 1, currentUser.FailedLoginAttemptCounter);

            Assert.Single(mockRepo.Object.Users);

            mockRepo.Verify(db => db.SaveChanges(), Times.Once);
        }

        [Fact]
        public void Login_Test_LoginAttempt_NoChances()
        {
            var mockRepo = new Mock<DevCmsDb>();
            var mockAuth = new Mock<IAuthProvider>();
            mockRepo.SetupDbSetMock(db => db.Users, GetUsers());

            var controller = new AccountController(mockRepo.Object, mockAuth.Object);

            var model = new LoginModel
            {
                Email = "test",
                Password = "111111"
            };
            var attemptsCount = CommonSettings.FailedLoginAttemptLimit - 1;
            Assert.Single(mockRepo.Object.Users);
            var currentUser = mockRepo.Object.Users.First();
            var lastAttemptDate = DateTime.UtcNow.AddSeconds(-5);
            currentUser.FailedLoginAttemptDateTime = lastAttemptDate;
            currentUser.FailedLoginAttemptCounter = attemptsCount;

            var result = controller.Login(model);

            var viewResult = Assert.IsType<ViewResult>(result);
            var errorMessage = string.Format("Ваша учетная запись заблокирована после {0} неудачных попыток входа."
                                             + " Пожалуйста обратитесь к администрации сайта.", CommonSettings.FailedLoginAttemptLimit);
            Assert.Equal(errorMessage, viewResult.ViewData.ModelState[""].Errors[0].ErrorMessage);

            Assert.Equal(lastAttemptDate, currentUser.FailedLoginAttemptDateTime);
            Assert.Equal(attemptsCount + 1, currentUser.FailedLoginAttemptCounter);

            Assert.Single(mockRepo.Object.Users);

            mockRepo.Verify(db => db.SaveChanges(), Times.Once);
        }

        [Fact]
        public void Login_Test_GoodCredenitalButAccountWasLocked()
        {
            var mockRepo = new Mock<DevCmsDb>();
            var mockAuth = new Mock<IAuthProvider>();
            mockRepo.SetupDbSetMock(db => db.Users, GetLockedUsers());

            var controller = new AccountController(mockRepo.Object, mockAuth.Object);

            var model = new LoginModel
            {
                Email = "test",
                Password = "123123"
            };
            var result = controller.Login(model);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Ваша учетная запись заблокирована. Пожалуйста обратитесь к администрации сайта.", 
                viewResult.ViewData.ModelState[""].Errors[0].ErrorMessage);
            mockRepo.Verify(db => db.SaveChanges(), Times.Never);

            var resultModel = Assert.IsAssignableFrom<LoginModel>(
                viewResult.ViewData.Model);
            Assert.Equal(model.Email, resultModel.Email);
            Assert.Equal(model.Password, resultModel.Password);
        }

        [Fact]
        public void Login_Test_BadCredenitalButAccountWasLocked()
        {
            var mockRepo = new Mock<DevCmsDb>();
            var mockAuth = new Mock<IAuthProvider>();
            mockRepo.SetupDbSetMock(db => db.Users, GetLockedUsers());

            var controller = new AccountController(mockRepo.Object, mockAuth.Object);

            var model = new LoginModel
            {
                Email = "test",
                Password = "111111"//incorrect password
            };
            var result = controller.Login(model);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Ваша учетная запись заблокирована. Пожалуйста обратитесь к администрации сайта.",
                viewResult.ViewData.ModelState[""].Errors[0].ErrorMessage);
            mockRepo.Verify(db => db.SaveChanges(), Times.Never);

            var resultModel = Assert.IsAssignableFrom<LoginModel>(
                viewResult.ViewData.Model);
            Assert.Equal(model.Email, resultModel.Email);
            Assert.Equal(model.Password, resultModel.Password);
        }

        [Fact]
        public void Login_Test_Success()
        {
            var mockRepo = new Mock<DevCmsDb>();
            var mockAuth = new Mock<IAuthProvider>();
            mockRepo.SetupDbSetMock(db => db.Users, GetUsers());

            var controller = new AccountController(mockRepo.Object, mockAuth.Object);

            var model = new LoginModel
            {
                Email = "test",
                Password = "123123"
            };
            var result = controller.Login(model);
            var viewResult = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", viewResult.ActionName);
            Assert.Equal("Notifications", viewResult.ControllerName);
        }

        private List<User> GetUsers()
        {
            var result = new List<User>
            {
                new User
                {
                    Id = 1,
                    Email = "test",
                    Password = "s1PHWye7JWA57bknIpty1tYZaOM=",//123123
                    PasswordSalt = "8fsxKIn6pcT0bG5sjU400Q==",
                    RegistrationStatus = "Approved"
                }
            };
            return result;
        }

        private List<User> GetLockedUsers()
        {
            var result = new List<User>
            {
                new User
                {
                    Id = 1,
                    Email = "test",
                    Password = "s1PHWye7JWA57bknIpty1tYZaOM=",
                    PasswordSalt = "8fsxKIn6pcT0bG5sjU400Q==",
                    RegistrationStatus = "Locked"
                }
            };
            return result;
        }
    }
}
