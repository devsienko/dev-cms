using System.Linq;
using Microsoft.AspNetCore.Mvc;
using DevCms.Models;
using DevCms.ContentTypes;
using DevCms.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace DevCms.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class ContentTypeController : Controller
    {
        private readonly DevCmsDb _db;

        public ContentTypeController(DevCmsDb context)
        {
            _db = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(CreateContentTypeModel model)
        {
            if (ModelState.IsValid)
            {
                var contentType = new EntityType { Name = model.Name };
                _db.ContentTypes.Add(contentType);
                _db.SaveChanges();
                var editModel = DtoHelper.GetEditContentTypeModel(contentType);
                return View(nameof(Edit), editModel);
            }
            return View(model);
        }

        public IActionResult Edit(int id, int? attr)
        {
            if (id < 0)
                return NotFound();
            if (attr.HasValue && attr < 0)
                return NotFound();
            var contentType = _db.ContentTypes
                .Include(ct => ct.Attrs)
                .FirstOrDefault(t => t.Id == id);
            if (contentType == null)
                return NotFound();
            var model = DtoHelper.GetEditContentTypeModel(contentType);
            if (attr.HasValue)
            {
                model.AddedOrEditedAttr = model.Attrs.FirstOrDefault(at => at.Id == attr);
                if (model.AddedOrEditedAttr == null)
                    return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(EditContentTypeModel model)
        {
            // this mess exists because I wanted to have the only one endpoint to edit/create the attributes
            // and to edit the name of the content type
            // I can avoid it by using some RequireId attribute or by creating some another action like 
            // CreateAttr /EditAttr, but I decided to use this approach
            if (IsAdditionNewAttribute(model))
            {
                ModelState["Name"].Errors.Clear();
                var contentType = _db.ContentTypes
                    .Include(ct => ct.Attrs)
                    .FirstOrDefault(t => t.Id == model.Id);
                if (contentType == null)
                    return NotFound();
                contentType.Attrs.Add(new Attribute
                {
                    Name = model.AddedOrEditedAttr.Name,
                    AttrType = model.AddedOrEditedAttr.AttributeType,
                    ContentTypeId = contentType.Id,
                    Required = model.AddedOrEditedAttr.Required
                });
                _db.SaveChanges();
                ModelState.Clear();
                model = DtoHelper.GetEditContentTypeModel(contentType);
            }
            else if (ModelState.IsValid)
            {
                var contentType = _db.ContentTypes
                    .Include(ct => ct.Attrs)
                    .FirstOrDefault(t => t.Id == model.Id);
                if (contentType == null)
                    return NotFound();
                contentType.Name = model.Name;
                if (model.AddedOrEditedAttr != null && model.AddedOrEditedAttr.Id.HasValue)
                {
                    var editedAttr = contentType.Attrs.First(at => at.Id == model.AddedOrEditedAttr.Id);
                    editedAttr.Name = model.AddedOrEditedAttr.Name;
                    editedAttr.AttrType = model.AddedOrEditedAttr.AttributeType;
                    editedAttr.Required = model.AddedOrEditedAttr.Required;
                }
                _db.SaveChanges();
                ModelState.Clear();
                model = DtoHelper.GetEditContentTypeModel(contentType);
            }
            return View(model);

        }

        private bool IsAdditionNewAttribute(EditContentTypeModel model)
        {
            var result = model.AddedOrEditedAttr != null && !model.AddedOrEditedAttr.Id.HasValue;
            return result;
        }

        public IActionResult Delete(int id)
        {
            if (id < 1)
                return NotFound();
            var contentType = _db.ContentTypes.FirstOrDefault(t => t.Id == id);
            if (contentType == null)
                return NotFound();
            _db.ContentTypes.Remove(contentType);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult DeleteAttr(int id)
        {
            if (id < 1)
                return NotFound();
            var attr = _db.ContentAttrs.FirstOrDefault(a => a.Id == id);
            if (attr == null)
                return NotFound();
            var typeId = attr.ContentTypeId;
            _db.ContentAttrs.Remove(attr);
            _db.SaveChanges();
            return RedirectToAction("Edit", new { id = typeId });
        }
    }
}
