using System;
using System.Collections.Generic;
using System.Linq;
using DevCms.Areas.Admin.Controllers;
using DevCms.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace DevCms.Tests
{
    public class AdminNotificationControllerTest
    {
        [Fact]
        public void Index_Test()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.Notifications, GetNotifications());

            var controller = new NotificationsController(mockRepo.Object);

            var result = controller.Index();
            var viewResult = Assert.IsType<ViewResult>(result);
            var resultModel = Assert.IsAssignableFrom<NotificationView>(
                viewResult.ViewData.Model);

            Assert.Single(resultModel.New);
            Assert.Single(resultModel.Viewed);

            Assert.Equal("test 2", resultModel.Viewed.First().Name);
            Assert.Equal("test 1", resultModel.New.First().Name);
        }

        [Fact]
        public void Index_Notification_NotFound()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.Notifications, GetNotifications());

            var controller = new NotificationsController(mockRepo.Object);

            var result = controller.Notification(null);
            Assert.IsType<NotFoundResult>(result);

            result = controller.Notification(0);
            Assert.IsType<NotFoundResult>(result);

            result = controller.Notification(-1);
            Assert.IsType<NotFoundResult>(result);

            result = controller.Notification(33);
            Assert.IsType<NotFoundResult>(result);

            result = controller.Notification(1);
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Index_Notification_ChangedStatus()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.Notifications, GetNotifications());

            var controller = new NotificationsController(mockRepo.Object);

            Assert.Equal(NotificationStatus.New, mockRepo.Object.Notifications.First(n => n.Id == 1).Status);

            var result = controller.Notification(1);
            Assert.IsType<ViewResult>(result);

            Assert.Equal(NotificationStatus.Viewed, mockRepo.Object.Notifications.First(n => n.Id == 1).Status);
            mockRepo.Verify(db => db.SaveChanges(), Times.Once());
        }

        [Fact]
        public void Index_Notification_NotChangedStatus()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.Notifications, GetNotifications());

            var controller = new NotificationsController(mockRepo.Object);

            Assert.Equal(NotificationStatus.Viewed, mockRepo.Object.Notifications.First(n => n.Id == 2).Status);

            var result = controller.Notification(2);
            Assert.IsType<ViewResult>(result);

            Assert.Equal(NotificationStatus.Viewed, mockRepo.Object.Notifications.First(n => n.Id == 2).Status);
            mockRepo.Verify(db => db.SaveChanges(), Times.Never);
        }

        private List<Notification> GetNotifications()
        {
            var result = new List<Notification>
            {
                new Notification
                {
                    Id = 1,
                    Name = "test 1",
                    Email = "email 1",
                    Message = "message 1",
                    Date = DateTime.UtcNow.AddDays(-1),
                    Status = NotificationStatus.New
                },
                new Notification
                {
                    Id = 2,
                    Name = "test 2",
                    Email = "email 2",
                    Message = "message 2",
                    Date = DateTime.UtcNow.AddDays(-2),
                    Status = NotificationStatus.Viewed
                }
            };
            return result;
        }
    }
}
