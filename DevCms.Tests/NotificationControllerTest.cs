using System.Collections.Generic;
using System.Linq;
using DevCms.ContentTypes;
using DevCms.Controllers;
using DevCms.Models;
using DevCms.Util;
using Moq;
using Xunit;

namespace DevCms.Tests
{
    public class NotificationControllerTest
    {
        [Fact]
        public void Create_Test_InvalidFormData()
        {
            var mockRepo = new Mock<DevCmsDb>();
            var emailServiceMock = new Mock<EmailService>();

            var controller = new NotificationController(mockRepo.Object, emailServiceMock.Object);

            var result = controller.Create(null);
            Assert.Equal("Error. Incorrect form data.", result);

            result = controller.Create(new Notification { Email = "test", Name = ""});
            Assert.Equal("Error. Incorrect form data.", result);
        }

        [Fact]
        public void Create_Test()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.Notifications, new List<Notification>());
            var emailServiceMock = new Mock<EmailService>();

            var controller = new NotificationController(mockRepo.Object, emailServiceMock.Object);

            var model = new Notification
            {
                Email = "test",
                Name = "name",
                Phone = "",
                Message = "message"
            };

            Assert.Equal(0, mockRepo.Object.Notifications.Count());

            var result = controller.Create(model);
            Assert.Equal("success", result);

            mockRepo.Verify(db => db.SaveChanges(), Times.Once());
            emailServiceMock.Verify(s => s.SendEmail(It.IsAny<Notification>()), Times.Once());

            Assert.Equal(1, mockRepo.Object.Notifications.Count());
            Assert.Equal("test", mockRepo.Object.Notifications.First().Email);
            Assert.Equal("name", mockRepo.Object.Notifications.First().Name);
            Assert.Equal("message", mockRepo.Object.Notifications.First().Message);
        }
    }
}
