using System;
using System.Collections.Generic;
using System.Linq;
using DevCms.Areas.Admin.Controllers;
using DevCms.ContentTypes;
using DevCms.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace DevCms.Tests
{
    public class DictionariesControllerTest
    {
        [Fact]
        public void CreateDictionary_Test()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.Dictionaries, GetDictionaries());

            var model = new EditDictionaryDto
            {
                Name = "Test",
                Items = null
            };

            var controller = new DictionariesController(mockRepo.Object);

            Assert.Equal(1, mockRepo.Object.Dictionaries.Count());

            var result = controller.Index(model);
            var viewResult = Assert.IsType<ViewResult>(result);
            var resultModel = Assert.IsAssignableFrom<EditDictionaryDto>(
                viewResult.ViewData.Model);
            Assert.Equal("Edit", viewResult.ViewName);

            mockRepo.Verify();
            mockRepo.Verify(db => db.SaveChanges(), Times.Once());

            Assert.Equal(2, mockRepo.Object.Dictionaries.Count());
            var newDictionary = mockRepo.Object.Dictionaries.Last();
            Assert.Equal("Test", newDictionary.Name);

            Assert.Empty(resultModel.Items);
        }

        [Fact]
        public void CreateDictionary_Test_InvalidModel()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.Dictionaries, GetDictionaries());

            var model = new EditDictionaryDto();

            var controller = new DictionariesController(mockRepo.Object);
            controller.ModelState.AddModelError("Name", "Required");
            Assert.Equal(1, mockRepo.Object.Dictionaries.Count());
            var result = controller.Index(model);
            var viewResult = Assert.IsType<ViewResult>(result);
            var resultModel = Assert.IsAssignableFrom<EditDictionaryDto>(viewResult.ViewData.Model);

            mockRepo.Verify();
            mockRepo.Verify(db => db.SaveChanges(), Times.Never);

            Assert.Equal(1, mockRepo.Object.Dictionaries.Count());

            Assert.Null(resultModel.Name);
        }

        [Fact]
        public void Delete_Test()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.Dictionaries, GetDictionaries());

            Assert.Equal(1, mockRepo.Object.Dictionaries.Count());

            var controller = new DictionariesController(mockRepo.Object);
            var result = controller.Delete(1);
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", viewResult.ActionName);

            mockRepo.Verify();
            mockRepo.Verify(db => db.SaveChanges(), Times.Once());

            Assert.Equal(0, mockRepo.Object.Dictionaries.Count());
        }

        [Fact]
        public void Delete_Test_NegativeParameter()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.Dictionaries, GetDictionaries());

            var controller = new DictionariesController(mockRepo.Object);
            var result = controller.Delete(-1);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Delete_Test_ZeroParameter()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.Dictionaries, GetDictionaries());

            var controller = new DictionariesController(mockRepo.Object);
            var result = controller.Delete(0);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Delete_Test_NotExistedId()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.Dictionaries, GetDictionaries());

            var controller = new DictionariesController(mockRepo.Object);
            var result = controller.Delete(222);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Edit_Test_Get_ZeroParameter()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.Dictionaries, GetDictionaries());

            var controller = new DictionariesController(mockRepo.Object);
            var result = controller.Edit(0, null);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Edit_Test_Get_NotFound()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.Dictionaries, GetDictionaries());

            var controller = new DictionariesController(mockRepo.Object);
            var result = controller.Edit(222, null);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Edit_Test_Get()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.Dictionaries, GetDictionaries());

            var controller = new DictionariesController(mockRepo.Object);
            var result = controller.Edit(1, null);

            var viewResult = Assert.IsType<ViewResult>(result);
            var resultModel = Assert.IsAssignableFrom<EditDictionaryDto>(
                viewResult.ViewData.Model);

            Assert.Equal(2, resultModel.Items.Count());
            Assert.Equal("Male", resultModel.Items.First().Name);
            Assert.Equal(1, resultModel.Id);
            Assert.Equal("Sex", resultModel.Name);
            Assert.Null(resultModel.AddedOrEditedItem);
        }

        [Fact]
        public void Edit_Test_Get_EditItem_ZeroArgument()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.Dictionaries, GetDictionaries());

            var controller = new DictionariesController(mockRepo.Object);
            var result = controller.Edit(1, 0);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Edit_Test_Get_EditItem_NotFound()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.Dictionaries, GetDictionaries());

            var controller = new DictionariesController(mockRepo.Object);
            var result = controller.Edit(1, 11);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Edit_Test_Get_EditItem()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.Dictionaries, GetDictionaries());

            var controller = new DictionariesController(mockRepo.Object);
            var result = controller.Edit(1, 1);

            var viewResult = Assert.IsType<ViewResult>(result);
            var resultModel = Assert.IsAssignableFrom<EditDictionaryDto>(
                viewResult.ViewData.Model);

            Assert.Equal(2, resultModel.Items.Count());
            Assert.Equal("Male", resultModel.Items.First().Name);
            Assert.Equal(1, resultModel.Id);
            Assert.Equal("Sex", resultModel.Name);
            Assert.NotNull(resultModel.AddedOrEditedItem);
            Assert.Equal("Male", resultModel.AddedOrEditedItem.Name);
            Assert.Equal(1, resultModel.AddedOrEditedItem.Id);
        }

        [Fact]
        public void Edit_Post_Test_InvalidDictionaryId()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.Dictionaries, GetDictionaries());

            var controller = new DictionariesController(mockRepo.Object);
            var model = new EditDictionaryDto
            {
                Name = "mmm",
                Id = 11,
            };
            var result = controller.Edit(model);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Edit_Post_Test()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.Dictionaries, GetDictionaries());

            var controller = new DictionariesController(mockRepo.Object);
            var model = new EditDictionaryDto
            {
                Name = "Sex2",
                Id = 1,
                AddedOrEditedItem = new EditDictionaryItemDto
                {
                    Id = 1,
                    Name = "Male2"
                }
            };
            var result = controller.Edit(model);
            var viewResult = Assert.IsType<ViewResult>(result);

            mockRepo.Verify(db => db.SaveChanges(), Times.Once());
            Assert.Equal("Sex2", mockRepo.Object.Dictionaries.First().Name);
            Assert.Equal("Male2", mockRepo.Object.Dictionaries.First().Items.First().Name);

            var resultModel = Assert.IsAssignableFrom<EditDictionaryDto>(
                viewResult.ViewData.Model);
            Assert.Equal("Sex2", resultModel.Name);
            Assert.Equal("Male2", resultModel.Items.First().Name);
        }

        [Fact]
        public void EditDictionaryName_Post_Test()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.Dictionaries, GetDictionaries());

            var controller = new DictionariesController(mockRepo.Object);
            var model = new EditDictionaryDto
            {
                Name = "Sex2",
                Id = 1
            };
            var result = controller.Edit(model);
            var viewResult = Assert.IsType<ViewResult>(result);

            mockRepo.Verify(db => db.SaveChanges(), Times.Once());
            Assert.Equal("Sex2", mockRepo.Object.Dictionaries.First().Name);

            var resultModel = Assert.IsAssignableFrom<EditDictionaryDto>(
                viewResult.ViewData.Model);
            Assert.Equal("Sex2", resultModel.Name);
        }

        [Fact]
        public void Edit_Post_Test_InvalidItemId()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.Dictionaries, GetDictionaries());

            var controller = new DictionariesController(mockRepo.Object);
            var model = new EditDictionaryDto
            {
                Name = "mmm",
                Id = 1,
                AddedOrEditedItem = new EditDictionaryItemDto
                {
                    Id = 11,
                    Name = "Item mmm"
                }
            };

            Assert.Throws<InvalidOperationException>(() => controller.Edit(model));
            mockRepo.Verify(db => db.SaveChanges(), Times.Never);
        }

        [Fact]
        public void Edit_Post_Test_InvalidModelState()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.Dictionaries, GetDictionaries());

            var controller = new DictionariesController(mockRepo.Object);
            var model = new EditDictionaryDto
            {
                Id = 1,
                AddedOrEditedItem = new EditDictionaryItemDto
                {
                    Id = 1,
                    Name = "Item mmm"
                }
            };
            controller.ModelState.AddModelError("Name", "Required");
            controller.Edit(model);

            mockRepo.Verify(db => db.SaveChanges(), Times.Never);
            Assert.Equal("Sex", mockRepo.Object.Dictionaries.First().Name);
            Assert.Equal("Male", mockRepo.Object.Dictionaries.First().Items.First().Name);
        }

        [Fact]
        public void Edit_Post_Test_AddItem()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.Dictionaries, GetDictionaries());

            var controller = new DictionariesController(mockRepo.Object);
            var model = new EditDictionaryDto
            {
                Id = 1,
                AddedOrEditedItem = new EditDictionaryItemDto
                {
                    Name = "Unknown",
                }
            };
            controller.ModelState.AddModelError("Name", "Required");
            Assert.Single(controller.ModelState);

            Assert.Equal(2, mockRepo.Object.Dictionaries.First().Items.Count);

            var result = controller.Edit(model);

            var viewResult = Assert.IsType<ViewResult>(result);
            var resultModel = Assert.IsAssignableFrom<EditDictionaryDto>(
                viewResult.ViewData.Model);

            mockRepo.Verify(db => db.SaveChanges(), Times.Once);
            Assert.Empty(controller.ModelState);
            Assert.Equal(3, mockRepo.Object.Dictionaries.First().Items.Count);

            Assert.Equal(3, resultModel.Items.Count());
            var items = mockRepo.Object.Dictionaries.First().Items.Where(a => a.Name == "Unknown");
            Assert.Single(items);
        }

        [Fact]
        public void Edit_Post_Test_AddItem_DictionaryNotFound()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.Dictionaries, GetDictionaries());

            var controller = new DictionariesController(mockRepo.Object);
            var model = new EditDictionaryDto
            {
                Id = 11,
                AddedOrEditedItem = new EditDictionaryItemDto//todo: not exist
                {
                    Name = "Third"
                }
            };
            controller.ModelState.AddModelError("Name", "Required");

            var result = controller.Edit(model);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void DeleteItem_Test_NotFound()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.DictionaryItems, GetItemsList());

            var controller = new DictionariesController(mockRepo.Object);

            var result = controller.DeleteItem(-1);
            Assert.IsType<NotFoundResult>(result);

            result = controller.DeleteItem(11);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void DeleteItem_Test()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.DictionaryItems, GetItemsList());

            Assert.Equal(1, mockRepo.Object.DictionaryItems.Count());

            var controller = new DictionariesController(mockRepo.Object);
            var result = controller.DeleteItem(1);
            var viewResult = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Edit", viewResult.ActionName);
            mockRepo.Verify(db => db.SaveChanges(), Times.Once);
            Assert.Equal(0, mockRepo.Object.DictionaryItems.Count());
        }

        private List<Dictionary> GetDictionaries()
        {
            var result = new List<Dictionary>
            {
                new Dictionary
                {
                    Id = 1,
                    Name = "Sex",
                    Items = new List<DictionaryItem>
                    {
                        new DictionaryItem
                        {
                            Id = 1,
                            Name = "Male"
                        },
                        new DictionaryItem
                        {
                            Id = 2,
                            Name = "Female"
                        }
                    }
                }
            };
            return result;
        }

        private List<DictionaryItem> GetItemsList()
        {
            var result = new List<DictionaryItem>
            {
                new DictionaryItem
                {
                    Id = 1,
                    Name = "Item1",
                    Dictionary = new Dictionary
                    {
                        Id = 1,
                        Name = "Dict1"
                    }
                }
            };
            return result;
        }
    }
}
