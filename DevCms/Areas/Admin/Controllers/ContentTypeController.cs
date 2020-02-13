using System.Linq;
using Microsoft.AspNetCore.Mvc;
using DevCms.Models;
using DevCms.ContentTypes;
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
                var editModel = EditContentTypeModel.GetViewModelFrom(contentType);
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
            var model = EditContentTypeModel.GetViewModelFrom(contentType);
            if (attr.HasValue)
            {
                model.EditedAttr = model.Attrs.FirstOrDefault(at => at.Id == attr);
                if (model.EditedAttr == null)
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
                    Name = model.EditedAttr.Name,
                    AttrType = model.EditedAttr.AttributeType,
                    ContentTypeId = contentType.Id,
                    Required = model.EditedAttr.Required
                });
                _db.SaveChanges();
                ModelState.Clear();
                model = EditContentTypeModel.GetViewModelFrom(contentType);
            }
            else if (ModelState.IsValid)
            {
                var contentType = _db.ContentTypes
                    .Include(ct => ct.Attrs)
                    .FirstOrDefault(t => t.Id == model.Id);
                if (contentType == null)
                    return NotFound();
                contentType.Name = model.Name;
                if (model.EditedAttr != null && model.EditedAttr.Id.HasValue)
                {
                    var editedAttr = contentType.Attrs.First(at => at.Id == model.EditedAttr.Id);
                    editedAttr.Name = model.EditedAttr.Name;
                    editedAttr.AttrType = model.EditedAttr.AttributeType;
                    editedAttr.Required = model.EditedAttr.Required;
                }
                _db.SaveChanges();
                ModelState.Clear();
                model = EditContentTypeModel.GetViewModelFrom(contentType);
            }
            return View(model);

        }

        private bool IsAdditionNewAttribute(EditContentTypeModel model)
        {
            var result = model.EditedAttr != null && !model.EditedAttr.Id.HasValue;
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
