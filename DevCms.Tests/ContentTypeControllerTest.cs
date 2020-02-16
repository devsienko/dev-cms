using System;
using System.Collections.Generic;
using System.Linq;
using DevCms.Areas.Admin.Controllers;
using DevCms.ContentTypes;
using DevCms.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Attribute = DevCms.ContentTypes.Attribute;

namespace DevCms.Tests
{
    public class ContentTypeControllerTest
    {
        [Fact]
        public void Index_Test()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.ContentTypes, GetContentTypeList());

            var model = new CreateContentTypeModel
            {
                Name = "Test",
                Attributes = null
            };

            var controller = new ContentTypeController(mockRepo.Object);
            var result = controller.Index(model);
            var viewResult = Assert.IsType<ViewResult>(result);
            var resultModel = Assert.IsAssignableFrom<EditContentTypeModel>(
                viewResult.ViewData.Model);
            Assert.Equal("Edit", viewResult.ViewName);

            mockRepo.Verify();
            mockRepo.Verify(db => db.SaveChanges(), Times.Once());

            Assert.Equal(1, resultModel.Id);
            Assert.Empty(resultModel.Attrs);
        }

        [Fact]
        public void Index_Test_InvalidModel()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.ContentTypes, GetContentTypeList());

            var model = new CreateContentTypeModel
            {
                Name = "Test",
                Attributes = null
            };

            var controller = new ContentTypeController(mockRepo.Object);
            controller.ModelState.AddModelError("Name", "Required");
            var result = controller.Index(model);
            var viewResult = Assert.IsType<ViewResult>(result);
            var resultModel = Assert.IsAssignableFrom<CreateContentTypeModel>(viewResult.ViewData.Model);

            mockRepo.Verify();

            Assert.Equal("Test", resultModel.Name);
            Assert.Null(resultModel.Attributes);
        }

        [Fact]
        public void Delete_Test()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.ContentTypes, GetContentTypeList());

            var controller = new ContentTypeController(mockRepo.Object);
            var result = controller.Delete(1);
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", viewResult.ActionName);

            mockRepo.Verify();
            mockRepo.Verify(db => db.SaveChanges(), Times.Once());
        }

        [Fact]
        public void Delete_Test_NegativeParameter()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.ContentTypes, GetContentTypeList());

            var controller = new ContentTypeController(mockRepo.Object);
            var result = controller.Delete(-1);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Delete_Test_ZeroParameter()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.ContentTypes, GetContentTypeList());

            var controller = new ContentTypeController(mockRepo.Object);
            var result = controller.Delete(0);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Delete_Test_NotExistedId()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.ContentTypes, GetContentTypeList());

            var controller = new ContentTypeController(mockRepo.Object);
            var result = controller.Delete(222);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Edit_Test_Get_ZeroParameter()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.ContentTypes, GetContentTypeList());

            var controller = new ContentTypeController(mockRepo.Object);
            var result = controller.Edit(0, null);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Edit_Test_Get_NotFound()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.ContentTypes, GetContentTypeList());

            var controller = new ContentTypeController(mockRepo.Object);
            var result = controller.Edit(222, null);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Edit_Test_Get()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.ContentTypes, GetContentTypeList());

            var controller = new ContentTypeController(mockRepo.Object);
            var result = controller.Edit(1, null);

            var viewResult = Assert.IsType<ViewResult>(result);
            var resultModel = Assert.IsAssignableFrom<EditContentTypeModel>(
                viewResult.ViewData.Model);

            Assert.Single(resultModel.Attrs);
            Assert.Equal("Attr1", resultModel.Attrs.First().Name);
            Assert.Equal(1, resultModel.Id);
            Assert.Equal("Test name", resultModel.Name);
            Assert.Null(resultModel.AddedOrEditedAttr);
        }

        [Fact]
        public void Edit_Test_Get_EditAttr_ZeroArgument()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.ContentTypes, GetContentTypeList());

            var controller = new ContentTypeController(mockRepo.Object);
            var result = controller.Edit(1, 0);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Edit_Test_Get_EditAttr_NotFound()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.ContentTypes, GetContentTypeList());

            var controller = new ContentTypeController(mockRepo.Object);
            var result = controller.Edit(1, 11);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Edit_Test_Get_EditAttr()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.ContentTypes, GetContentTypeList());

            var controller = new ContentTypeController(mockRepo.Object);
            var result = controller.Edit(1, 1);

            var viewResult = Assert.IsType<ViewResult>(result);
            var resultModel = Assert.IsAssignableFrom<EditContentTypeModel>(
                viewResult.ViewData.Model);

            Assert.Single(resultModel.Attrs);
            Assert.Equal("Attr1", resultModel.Attrs.First().Name);
            Assert.Equal(1, resultModel.Id);
            Assert.Equal("Test name", resultModel.Name);
            Assert.NotNull(resultModel.AddedOrEditedAttr);
            Assert.Equal("Attr1", resultModel.AddedOrEditedAttr.Name);
            Assert.True(resultModel.AddedOrEditedAttr.Required);
            Assert.Equal(1, resultModel.AddedOrEditedAttr.Id);
        }

        [Fact]
        public void Edit_Post_Test_InvalidContentTypeId()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.ContentTypes, GetContentTypeList());

            var controller = new ContentTypeController(mockRepo.Object);
            var model = new EditContentTypeModel
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
            mockRepo.SetupDbSetMock(db => db.ContentTypes, GetContentTypeList());

            var controller = new ContentTypeController(mockRepo.Object);
            var model = new EditContentTypeModel
            {
                Name = "mmm",
                Id = 1,
                AddedOrEditedAttr = new ContentAttributeModel
                {
                    Id = 1,
                    Name = "attr mmm",
                    Required = true,
                    AttributeType = AttrType.Image
                }
            };
            var result = controller.Edit(model);
            var viewResult = Assert.IsType<ViewResult>(result);

            mockRepo.Verify(db => db.SaveChanges(), Times.Once());
            Assert.Equal("mmm", mockRepo.Object.ContentTypes.First().Name);
            Assert.Equal("attr mmm", mockRepo.Object.ContentTypes.First().Attrs.First().Name);

            var resultModel = Assert.IsAssignableFrom<EditContentTypeModel>(
                viewResult.ViewData.Model);
            Assert.Equal("mmm", resultModel.Name);
            Assert.Equal("attr mmm", resultModel.Attrs.First().Name);
            Assert.True(resultModel.Attrs.First().Required);
            Assert.Equal(AttrType.Image, resultModel.Attrs.First().AttributeType);
        }

        [Fact]
        public void Edit_Post_Test_InvalidAttrId()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.ContentTypes, GetContentTypeList());

            var controller = new ContentTypeController(mockRepo.Object);
            var model = new EditContentTypeModel
            {
                Name = "mmm",
                Id = 1,
                AddedOrEditedAttr = new ContentAttributeModel//todo: not exist
                {
                    Id = 11,
                    Name = "attr mmm"
                }
            };

            Assert.Throws<InvalidOperationException>(() => controller.Edit(model));
            mockRepo.Verify(db => db.SaveChanges(), Times.Never);
        }

        [Fact]
        public void Edit_Post_Test_InvalidModelState()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.ContentTypes, GetContentTypeList());

            var controller = new ContentTypeController(mockRepo.Object);
            var model = new EditContentTypeModel
            {
                Name = "mmm",
                Id = 1,
                AddedOrEditedAttr = new ContentAttributeModel//todo: not exist
                {
                    Id = 1,
                    Name = "attr mmm"
                }
            };
            controller.ModelState.AddModelError("Name", "Required");
            controller.Edit(model);

            mockRepo.Verify(db => db.SaveChanges(), Times.Never);
            Assert.Equal("Test name", mockRepo.Object.ContentTypes.First().Name);
            Assert.Equal("Attr1", mockRepo.Object.ContentTypes.First().Attrs.First().Name);
        }

        [Fact]
        public void Edit_Post_Test_AddAttribute()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.ContentTypes, GetContentTypeList());

            var controller = new ContentTypeController(mockRepo.Object);
            var model = new EditContentTypeModel
            {
                Id = 1,
                AddedOrEditedAttr = new ContentAttributeModel//todo: not exist
                {
                    Name = "attr mmm",
                    Required = true,
                    AttributeType = AttrType.Image
                }
            };
            controller.ModelState.AddModelError("Name", "Required");

            Assert.Single(controller.ModelState);
            Assert.Single(mockRepo.Object.ContentTypes.First().Attrs);

            var result = controller.Edit(model);

            var viewResult = Assert.IsType<ViewResult>(result);
            var resultModel = Assert.IsAssignableFrom<EditContentTypeModel>(
                viewResult.ViewData.Model);

            mockRepo.Verify(db => db.SaveChanges(), Times.Once);
            Assert.Empty(controller.ModelState);
            Assert.Equal(2, mockRepo.Object.ContentTypes.First().Attrs.Count);

            Assert.Equal(2, resultModel.Attrs.Count);
            var attrs = mockRepo.Object.ContentTypes.First().Attrs.Where(a => a.Name == "attr mmm");
            Assert.Single(attrs);
            Assert.True(attrs.First().Required);
            Assert.Equal(AttrType.Image, attrs.First().AttrType);
        }

        [Fact]
        public void Edit_Post_Test_AddAttribute_ContentTypeNotFound()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.ContentTypes, GetContentTypeList());

            var controller = new ContentTypeController(mockRepo.Object);
            var model = new EditContentTypeModel
            {
                Id = 11,
                AddedOrEditedAttr = new ContentAttributeModel//todo: not exist
                {
                    Name = "attr mmm"
                }
            };
            controller.ModelState.AddModelError("Name", "Required");

            var result = controller.Edit(model);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void DeleteAttr_Test_NotFound()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.ContentAttrs, GetAttributeList());

            var controller = new ContentTypeController(mockRepo.Object);

            var result = controller.DeleteAttr(-1);
            Assert.IsType<NotFoundResult>(result);

            result = controller.DeleteAttr(11);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void DeleteAttr_Test()
        {
            var mockRepo = new Mock<DevCmsDb>();
            mockRepo.SetupDbSetMock(db => db.ContentAttrs, GetAttributeList());

            var controller = new ContentTypeController(mockRepo.Object);

            var result = controller.DeleteAttr(1);
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Edit", viewResult.ActionName);
            mockRepo.Verify(db => db.SaveChanges(), Times.Once);
            Assert.Equal(0, mockRepo.Object.ContentAttrs.Count());
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
                            ContentTypeId = 1,
                            Required = true
                        }
                    }
                }
            };
            return result;
        }

        private List<Attribute> GetAttributeList()
        {
            var result = new List<Attribute>
            {
                new Attribute
                {
                    Id = 1,
                    AttrType = AttrType.String,
                    Name = "Attr1",
                    ContentTypeId = 1
                }
            };
            return result;
        }
    }
}
