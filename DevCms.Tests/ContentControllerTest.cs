using System.Collections.Generic;
using System.IO;
using System.Linq;
using DevCms.Areas.Admin.Controllers;
using DevCms.ContentTypes;
using DevCms.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace DevCms.Tests
{
    public class ContentControllerTest
    {
        [Fact]
        public void Index_Test_InvalidId()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.ContentTypes, GetContentTypeList());

            var controller = new ContentController(mockRepo.Object);

            var result = controller.Index(-11);
            Assert.IsType<NotFoundResult>(result);

            result = controller.Index(0);
            Assert.IsType<NotFoundResult>(result);

            result = controller.Index(11);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Index_Test()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.ContentTypes, GetContentTypeList());

            var controller = new ContentController(mockRepo.Object);

            var result = controller.Index(1);
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<AddEntityDto>(
                viewResult.ViewData.Model);

            Assert.Equal(1, model.ContentTypeId);
            Assert.Single(model.Attrs);
            Assert.Equal(1, model.Attrs.First().AttributeId);
            Assert.Null(model.Attrs.First().Value);
            Assert.Null(model.Attrs.First().Id);
            Assert.Null(model.Id);
        }

        [Fact]
        public void Index_Test_Post()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.Content, GetContentList());
            mockRepo.SetupDbSetMock(db => db.ContentTypes, GetContentTypeList());

            var controller = new ContentController(mockRepo.Object);

            var model = new AddEntityDto
            {
                Id = null,
                ContentTypeId = 1,
                Attrs = new List<AttributeValueDto>
                {
                    new AttributeValueDto
                    {
                        AttributeId = 1,
                        Value = "333",
                        AttributeType = AttrType.String
                    }
                }
            };

            Assert.Single(mockRepo.Object.Content);

            var result = controller.Index(model);
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", viewResult.ActionName);

            mockRepo.Verify(db => db.SaveChanges(), Times.Once());
            Assert.Equal(2, mockRepo.Object.Content.Count());
            Assert.Single(mockRepo.Object.Content.First().AttrValues);
        }

        [Fact]
        public void AddEntityWithDictionaryAttributeValue()
        {
            var dbMock = GetContentTypeListWithDictionaries();
            var controller = new ContentController(dbMock.Object);

            var model = new AddEntityDto
            {
                Id = null,//new entity
                ContentTypeId = 1,
                Attrs = new List<AttributeValueDto>
                {
                    new AttributeValueDto
                    {
                        AttributeId = 1,
                        DictionaryItemId = 1,
                        AttributeType = AttrType.Dictionary
                    }
                }
            };

            Assert.Equal(0,dbMock.Object.Content.Count());

            var result = controller.Index(model);
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", viewResult.ActionName);

            dbMock.Verify(db => db.SaveChanges(), Times.Once());
            Assert.Equal(1, dbMock.Object.Content.Count());

            var attrValues = dbMock.Object.Content.First().AttrValues;
            Assert.Single(attrValues);
            Assert.Equal(1, attrValues.First().DictionaryItemId);
            Assert.Null(attrValues.First().Value);
        }
        
        [Fact]
        public void Index_Test_Post_InvalidModel()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.Content, GetContentList());
            mockRepo.SetupDbSetMock(db => db.ContentTypes, GetContentTypeList());

            var controller = new ContentController(mockRepo.Object);
            controller.ModelState.AddModelError("Name", "Required");

            var model = new AddEntityDto
            {
                Id = null,
                ContentTypeId = 1,
                Attrs = new List<AttributeValueDto>
                {
                    new AttributeValueDto
                    {
                        AttributeId = 1,
                        Value = "attr value 1"
                    }
                }
            };

            var result = controller.Index(model);
            var viewResult = Assert.IsType<ViewResult>(result);
            var resultModel = Assert.IsAssignableFrom<AddEntityDto>(
                viewResult.ViewData.Model);

            Assert.Null(resultModel.Id);
            Assert.Equal(1, resultModel.ContentTypeId);
            Assert.Equal("attr value 1", resultModel.Attrs.First().Value);
        }

        [Fact]
        public void Index_Test_Post_InvalidContentTypeOrAttributeId()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.Content, GetContentList());
            mockRepo.SetupDbSetMock(db => db.ContentTypes, GetContentTypeList());

            var controller = new ContentController(mockRepo.Object);

            var model = new AddEntityDto
            {
                ContentTypeId = 11
            };

            var result = controller.Index(model);
            Assert.IsType<NotFoundResult>(result);

            model = new AddEntityDto
            {
                ContentTypeId = 1,
                Attrs = new List<AttributeValueDto>
                {
                    new AttributeValueDto
                    {
                        AttributeId = 44
                    }
                }
            };

            result = controller.Index(model);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void DeleteAttr_Test_InvalidId()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.Content, GetContentList());

            var controller = new ContentController(mockRepo.Object);

            var result = controller.Delete(-11);
            Assert.IsType<NotFoundResult>(result);

            result = controller.Delete(0);
            Assert.IsType<NotFoundResult>(result);

            result = controller.Delete(11);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Delete_Test()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.Content, GetContentList());

            var controller = new ContentController(mockRepo.Object);
            var result = controller.Delete(1);
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", viewResult.ActionName);

            mockRepo.Verify();
            mockRepo.Verify(db => db.SaveChanges(), Times.Once());
        }

        [Fact]
        public void Edit_Test_InvalidId()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.Content, GetContentList());

            var controller = new ContentController(mockRepo.Object);

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
            mockRepo.SetupDbSetMock(db => db.Content, GetContentList());
            mockRepo.SetupDbSetMock(db => db.ContentTypes, GetContentTypeList2());

            var controller = new ContentController(mockRepo.Object);

            var result = controller.Edit(1);
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<EditContentDto>(
                viewResult.ViewData.Model);

            Assert.Equal(1, model.ContentTypeId);
            Assert.Equal(2, model.Attrs.Count);
            Assert.Equal(11, model.Attrs.First().AttributeId);
            Assert.Equal("test", model.Attrs.First().Value);
            Assert.Equal(1, model.Attrs.First().Id);
            Assert.Equal(1, model.Id);
        }

        [Fact]
        public void Edit_Test_WithoutAttributes()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.Content, GetContentList());
            mockRepo.SetupDbSetMock(db => db.ContentTypes, GetContentTypeWithoutAttributes());

            var controller = new ContentController(mockRepo.Object);

            var result = controller.Edit(1);
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<EditContentDto>(
                viewResult.ViewData.Model);

            Assert.Equal(1, model.ContentTypeId);
            Assert.Empty(model.Attrs);
            Assert.Equal(1, model.Id);
        }

        [Fact]
        public void Edit_Test_Post()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.Content, GetContentList());
            mockRepo.SetupDbSetMock(db => db.ContentTypes, GetContentTypeList1());

            var controller = new ContentController(mockRepo.Object);

            var model = new EditContentDto
            {
                Id = 1,
                ContentTypeId = 1,
                Attrs = new List<AttributeValueDto>
                {
                    new AttributeValueDto
                    {
                        Id = 1,
                        AttributeId = 11,
                        Value = "333",
                        AttributeType = AttrType.String
                    }
                }
            };

            Assert.Single(mockRepo.Object.Content);
            var attrValues = mockRepo.Object.Content.First().AttrValues;
            Assert.Single(mockRepo.Object.Content.First().AttrValues);
            Assert.Equal("test", attrValues.First().Value);

            var result = controller.Edit(model);
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", viewResult.ActionName);

            mockRepo.Verify(db => db.SaveChanges(), Times.Once());
            Assert.Equal(1, mockRepo.Object.Content.Count());
            Assert.Single(mockRepo.Object.Content.First().AttrValues);
            Assert.Equal("333", attrValues.First().Value);
        }

        [Fact]
        public void Edit_Test_Post_RequireImage()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.Content, GetContentList());
            mockRepo.SetupDbSetMock(db => db.ContentTypes, GetContentTypeList2());

            var controller = new ContentController(mockRepo.Object);

            var fileName = "test.png";
            var model = new EditContentDto
            {
                Id = 1,
                ContentTypeId = 1,
                Attrs = new List<AttributeValueDto>
                {
                    new AttributeValueDto
                    {
                        Id = 2,
                        AttributeId = 12,
                        AttributeType = AttrType.Image,
                        ValueAsImage = CreateFile(fileName),
                        Required = true
                    }
                }
            };

            Assert.False(mockRepo.Object.Content
                .First()
                .AttrValues
                .Any(av => av.AttrId == 12 && av.ValueAsFile.FileName == fileName));

            var result = controller.Edit(model);
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", viewResult.ActionName);

            mockRepo.Verify(db => db.SaveChanges(), Times.Once());
            Assert.Equal(1, mockRepo.Object.Content.Count());
            Assert.True(mockRepo.Object.Content
                .First()
                .AttrValues
                .Any(av => av.AttrId == 12 && av.ValueAsFile.FileName == fileName));
        }

        private IFormFile CreateFile(string fileName)
        {
            var fileMock = new Mock<IFormFile>();

            var content = "Hello World from a Fake File";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;

            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);

            return fileMock.Object;
        }

        [Fact]
        public void EditAfterNewAttrsAdded_Test_Post()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.Content, GetContentList());
            mockRepo.SetupDbSetMock(db => db.ContentTypes, GetContentTypeList2());

            var controller = new ContentController(mockRepo.Object);

            var model = new EditContentDto
            {
                Id = 1,
                ContentTypeId = 1,
                Attrs = new List<AttributeValueDto>
                {
                    new AttributeValueDto
                    {
                        Id = 1,
                        AttributeId = 11,
                        Value = "333",
                        AttributeType = AttrType.String
                    },
                    new AttributeValueDto
                    {
                        AttributeId = 12,
                        AttributeType = AttrType.Image
                    }
                }
            };

            Assert.Single(mockRepo.Object.Content);

            var result = controller.Edit(model);
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", viewResult.ActionName);

            mockRepo.Verify(db => db.SaveChanges(), Times.Once());
            Assert.Equal(1, mockRepo.Object.Content.Count());
            Assert.Equal(2, mockRepo.Object.Content.First().AttrValues.Count);
        }

        [Fact]
        public void Edit_Test_Post_InvalidModel()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.Content, GetContentList());
            mockRepo.SetupDbSetMock(db => db.ContentTypes, GetContentTypeList());

            var controller = new ContentController(mockRepo.Object);
            controller.ModelState.AddModelError("Name", "Required");

            var model = new EditContentDto
            {
                Id = 1,
                ContentTypeId = 1,
                Attrs = new List<AttributeValueDto>
                {
                    new AttributeValueDto
                    {
                        Id = 1,
                        AttributeId = 1,
                        Value = "attr value 1"
                    }
                }
            };

            var result = controller.Edit(model);
            var viewResult = Assert.IsType<ViewResult>(result);
            var resultModel = Assert.IsAssignableFrom<EditContentDto>(
                viewResult.ViewData.Model);

            Assert.Equal(1, resultModel.Id);
            Assert.Equal(1, resultModel.ContentTypeId);
            Assert.Equal("attr value 1", resultModel.Attrs.First().Value);
        }

        [Fact]
        public void Edit_Test_Post_InvalidContentTypeOrAttributeId()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.Content, GetContentList());
            mockRepo.SetupDbSetMock(db => db.ContentTypes, GetContentTypeList());

            var controller = new ContentController(mockRepo.Object);

            var model = new EditContentDto
            {
                ContentTypeId = 11
            };

            var result = controller.Edit(model);
            Assert.IsType<NotFoundResult>(result);

            model = new EditContentDto
            {
                ContentTypeId = 1,
                Attrs = new List<AttributeValueDto>
                {
                    new AttributeValueDto
                    {
                        AttributeId = 44
                    }
                }
            };

            result = controller.Edit(model);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void AddAttribute_Test()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.Content, GetContentList());
            mockRepo.SetupDbSetMock(db => db.ContentTypes, GetContentTypeList1());

            var controller = new ContentController(mockRepo.Object);

            var result = controller.Edit(1);
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<EditContentDto>(
                viewResult.ViewData.Model);

            Assert.Equal(1, model.ContentTypeId);
            Assert.Single(model.Attrs);
            Assert.Equal(11, model.Attrs.First().AttributeId);
            Assert.Equal("test", model.Attrs.First().Value);
            Assert.Equal(1, model.Attrs.First().Id);
            Assert.Equal(1, model.Id);

            mockRepo.SetupDbSetMock(db => db.ContentTypes, GetContentTypeList2());

            result = controller.Edit(1);
            viewResult = Assert.IsType<ViewResult>(result);
            model = Assert.IsAssignableFrom<EditContentDto>(
                viewResult.ViewData.Model);

            Assert.Equal(1, model.ContentTypeId);
            Assert.Equal(2, model.Attrs.Count);
            Assert.Equal(11, model.Attrs[0].AttributeId);
            Assert.Equal(12, model.Attrs[1].AttributeId);
            Assert.Equal("test", model.Attrs[0].Value);
            Assert.Null(model.Attrs[1].Value);
            Assert.Equal(1, model.Attrs[0].Id);
            Assert.Null(model.Attrs[1].Id);
            Assert.Equal(1, model.Id);
        }

        private List<EntityType> GetContentTypeList()
        {
            var result = new List<EntityType>
            {
                new EntityType
                {
                    Id = 1,
                    Name = "Test name",
                    Attrs = new List<Attribute>
                    {
                        new Attribute
                        {
                            Id = 1,
                            AttrType = AttrType.String,
                            Name = "Attr1",
                            ContentTypeId = 1
                        }
                    }
                }
            };
            return result;
        }

        private Mock<DevCmsDb> GetContentTypeListWithDictionaries()
        {
            var result = new Mock<DevCmsDb>();
            result.SetupDbSetMock(db => db.Content, new List<Entity>());

            var entityTypes = new List<EntityType>
            {
                new EntityType
                {
                    Id = 1,
                    Name = "Test name",
                    Attrs = new List<Attribute>
                    {
                        new Attribute
                        {
                            Id = 1,
                            AttrType = AttrType.Dictionary,
                            Name = "Attr1",
                            ContentTypeId = 1,
                            DictionaryId = 1
                        }
                    }
                }
            };
            result.SetupDbSetMock(db => db.ContentTypes, entityTypes);

            var dictionaries = new List<Dictionary>
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
                            Name = "Male",
                        },
                        new DictionaryItem
                        {
                            Id = 2,
                            Name = "Female",
                        }
                    }
                }
            };
            result.SetupDbSetMock(db => db.Dictionaries, dictionaries);

            return result;
        }

        private List<EntityType> GetContentTypeList1()
        {
            var result = new List<EntityType>
            {
                new EntityType
                {
                    Id = 1,
                    Name = "Test name",
                    Attrs = new List<Attribute>
                    {
                        new Attribute
                        {
                            Id = 11,
                            AttrType = AttrType.String,
                            Name = "Attr1",
                            ContentTypeId = 1
                        }
                    }
                }
            };
            return result;
        }

        private List<EntityType> GetContentTypeList2()
        {
            var result = new List<EntityType>
            {
                new EntityType
                {
                    Id = 1,
                    Name = "Test name",
                    Attrs = new List<Attribute>
                    {
                        new Attribute
                        {
                            Id = 11,
                            AttrType = AttrType.String,
                            Name = "Attr1",
                            ContentTypeId = 1
                        },
                        new Attribute
                        {
                            Id = 12,
                            AttrType = AttrType.Image,
                            Name = "Attr2",
                            ContentTypeId = 1,
                            Required = true
                        }
                    }
                }
            };
            return result;
        }

        private List<EntityType> GetContentTypeWithoutAttributes()
        {
            var result = new List<EntityType>
            {
                new EntityType
                {
                    Id = 1,
                    Name = "Test name",
                    Attrs = new List<Attribute>()
                }
            };
            return result;
        }

        private List<Entity> GetContentList()
        {
            var result = new List<Entity>
            {
                new Entity
                {
                    Id = 1,
                    EntityTypeId = 1,
                    AttrValues = new List<AttrValue>
                    {
                        new AttrValue
                        {
                            Id = 1,
                            Value = "test",
                            AttrId = 11,
                            Attr = new Attribute
                            {
                                Id = 11,
                                Name = "test attr"
                            }
                        }
                    }
                }
            };
            return result;
        }
    }
}