using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using DevCms.Models;
using DevCms.ContentTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Attribute = DevCms.ContentTypes.Attribute;

namespace DevCms.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class ContentController : Controller
    {
        private readonly DevCmsDb _db;

        public ContentController(DevCmsDb context)
        {
            _db = context;
        }

        public IActionResult Index(int id)
        {
            if (id < 1)
                return NotFound();
            var contentType = _db.ContentTypes
                .Include(t => t.Attrs)
                .FirstOrDefault(t => t.Id == id);
            if (contentType == null)
                return NotFound();
            var model = new AddContentDto
            {
                ContentTypeId = contentType.Id,
                Attrs = contentType.Attrs.Any()
                    ? contentType.Attrs.Select(a => new AttributeValueDto
                    {
                        AttributeName = a.Name,
                        AttributeType = a.AttrType,
                        AttributeId = a.Id,
                        Required = a.Required
                    }).ToList()
                    : new List<AttributeValueDto>()
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Index(AddContentDto model)
        {
            //todo: check all required attribute
            if (ModelState.IsValid)
            {
                var contentType = _db.ContentTypes
                    .Include(t => t.Attrs)
                    .FirstOrDefault(t => t.Id == model.ContentTypeId);
                if (contentType == null)
                {
                    //todo: to log
                    return NotFound();
                }

                if (!AllAttrsExests(model.Attrs, contentType)) 
                {
                    //todo: to log
                    return NotFound();
                }
                var content = new Entity
                {
                    EntityTypeId = model.ContentTypeId,
                    AttrValues = model.Attrs.Select(CreateAttribute).ToList()
                };
                _db.Content.Add(content);
                _db.SaveChanges();
                return RedirectToAction(nameof(Index), new {id = model.ContentTypeId});
            }
           
            return View(model);
        }

        private AttrValue CreateAttribute(AttributeValueDto avDto)
        {
            var result = new AttrValue
            {
                AttrId = avDto.AttributeId
            };
            if (avDto.AttributeType == AttrType.String)
            {
                result.Value = avDto.Value;
            }
            else if (avDto.AttributeType == AttrType.Image)
            {
                if (avDto.Required || avDto.ValueAsImage != null)
                {
                    byte[] imageData;
                    using (var binaryReader = new BinaryReader(avDto.ValueAsImage.OpenReadStream()))
                    {
                        imageData = binaryReader.ReadBytes((int)avDto.ValueAsImage.Length);
                    }

                    result.ValueAsFile = new FileAttrValue
                    {
                        FileName = avDto.ValueAsImage.FileName,
                        ContentType = avDto.ValueAsImage.ContentType,
                        Bytes = imageData
                    };
                }

            }
            else
            {
                throw new NotImplementedException();
            }
            return result;
        }

        private void EditAttribute(AttrValue dbAv, AttributeValueDto modelAv)
        {
            if (modelAv.AttributeType == AttrType.String)
            {
                dbAv.Value = modelAv.Value;
            }
            else if (modelAv.AttributeType == AttrType.Image)
            {
                if (modelAv.Required || modelAv.ValueAsImage != null)
                {
                    byte[] imageData;
                    using (var binaryReader = new BinaryReader(modelAv.ValueAsImage.OpenReadStream()))
                    {
                        imageData = binaryReader.ReadBytes((int)modelAv.ValueAsImage.Length);
                    }

                    dbAv.ValueAsFile = new FileAttrValue
                    {
                        FileName = modelAv.ValueAsImage.FileName,
                        ContentType = modelAv.ValueAsImage.ContentType,
                        Bytes = imageData
                    };
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public IActionResult Edit(int id)
        {
            if (id < 1)
                return NotFound();
            var content = _db.Content
                .Include(t => t.AttrValues)
                .ThenInclude(av => av.Attr)
                .FirstOrDefault(t => t.Id == id);
            if (content == null)
                return NotFound();
            var contentType = _db.ContentTypes
                .Include(t => t.Attrs)
                .First(t => t.Id == content.EntityTypeId);
            var model = new EditContentDto
            {
                Id = content.Id,
                ContentTypeId = content.EntityTypeId,
                Attrs = contentType.Attrs == null || !contentType.Attrs.Any()
                    ? new List<AttributeValueDto>()
                    : contentType.Attrs
                        .Select(a => CreateAttributeValue(a, content))
                        .ToList()
            };
            return View(model);
        }

        private AttributeValueDto CreateAttributeValue(Attribute attr, Entity entity)
        {
            var result = new AttributeValueDto
            {
                AttributeName = attr.Name,
                AttributeId = attr.Id,
                AttributeType = attr.AttrType,
                Required = attr.Required
            };
            var av = entity.AttrValues.FirstOrDefault(attrValue => attrValue.AttrId == attr.Id);
            if (av == null)
                return result;
            result.Id = av.Id;
            if (attr.AttrType == AttrType.String)
            {
                result.Value = av.Value;
            }
            else if (attr.AttrType == AttrType.Image)
            {
                //we just show the image
            }
            else
            {
                throw new ArgumentException("attr.AttrType");
            }

            return result;
        }

        [HttpPost]
        public IActionResult Edit(EditContentDto model)
        {
            if (ModelState.IsValid)
            {
                var content = _db.Content
                    .Include(i => i.AttrValues)
                    .FirstOrDefault(i => i.Id == model.Id);
                if (content == null)
                {
                    //todo: to log
                    return NotFound();
                }

                //todo: I suppose to remove this validation:
                var contentType = _db.ContentTypes
                    .Include(t => t.Attrs)
                    .FirstOrDefault(t => t.Id == model.ContentTypeId);

                if (!AllAttrsExests(model.Attrs, contentType))
                    return NotFound();

                foreach (var modelAv in model.Attrs)
                {
                    var av = content.AttrValues
                        .FirstOrDefault(dbAv => dbAv.AttrId == modelAv.AttributeId);
                    if(av == null)
                        content.AttrValues.Add(CreateAttribute(modelAv));
                    else
                        EditAttribute(av, modelAv);
                }

                _db.SaveChanges();
                return RedirectToAction(nameof(Index), new { id = model.ContentTypeId });

            }
            return View(model);
              
        }

        /// <summary>
        /// all model attributes exists in the db
        /// </summary>
        private bool AllAttrsExests(List<AttributeValueDto> attrs, EntityType entityType)
        {
            var result = attrs
                .Any(modelAttr => entityType.Attrs.Any(dbAttr => dbAttr.Id == modelAttr.AttributeId));

            return result;
        }

        public IActionResult Delete(int id)
        {
            if (id < 1)
                return NotFound();
            var content = _db.Content.FirstOrDefault(t => t.Id == id);
            if(content == null)
                return NotFound();
            var typeId = content.EntityTypeId;
            _db.Content.Remove(content);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index), new { id = typeId });
        }
    }
}
