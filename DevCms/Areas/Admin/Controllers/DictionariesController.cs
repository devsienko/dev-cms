using System;
using System.Collections.Generic;
using System.Linq;
using DevCms.Areas.Admin.Components;
using DevCms.ContentTypes;
using Microsoft.AspNetCore.Mvc;
using DevCms.Models;
using DevCms.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace DevCms.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class DictionariesController : Controller
    {
        private readonly DevCmsDb _db;

        public DictionariesController(DevCmsDb db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(EditDictionaryDto model)
        {
            if (ModelState.IsValid)
            {
                var dictionary = new Dictionary { Name = model.Name };
                _db.Dictionaries.Add(dictionary);
                _db.SaveChanges();
                return View(nameof(Edit), DtoHelper.GeEditDictionaryDto(dictionary));
            }
            return View(model);
        }

        public IActionResult Edit(int id, int? itemId)
        {
            if (id < 0)
                return NotFound();
            if (itemId.HasValue && itemId < 0)
                return NotFound();
            var dictionary = _db.Dictionaries
                .Include(d => d.Items)
                .FirstOrDefault(t => t.Id == id);
            if (dictionary == null)
                return NotFound();
            var model = DtoHelper.GeEditDictionaryDto(dictionary);
            var isEditItemRequest = itemId.HasValue;
            if (isEditItemRequest)
            {
                model.AddedOrEditedItem = model.Items.FirstOrDefault(at => at.Id == itemId);
                if (model.AddedOrEditedItem == null)
                    return NotFound();
            }
            return View(model);
        }


        [HttpPost]
        public IActionResult Edit(EditDictionaryDto model)
        {
            // this mess exists because I wanted to have the only one endpoint to edit/create the attributes
            // and to edit the name of the content type
            // I can avoid it by using some RequireId attribute or by creating some another action like 
            // CreateAttr /EditAttr, but I decided to use this approach
            if (IsAdditionNewDictionaryItem(model))
            {
                ModelState["Name"].Errors.Clear();
                var dictionary = _db.Dictionaries
                    .Include(d => d.Items)
                    .FirstOrDefault(t => t.Id == model.Id);
                if (dictionary == null)
                    return NotFound();
                dictionary.Items.Add(new DictionaryItem
                {
                    Name = model.AddedOrEditedItem.Name
                });
                _db.SaveChanges();
                ModelState.Clear();
                model = DtoHelper.GeEditDictionaryDto(dictionary);
            }
            else if (ModelState.IsValid)
            {
                var dictionary = _db.Dictionaries
                    .Include(d => d.Items)
                    .FirstOrDefault(t => t.Id == model.Id);
                if (dictionary == null)
                    return NotFound();
                dictionary.Name = model.Name;
                if (model.AddedOrEditedItem != null && model.AddedOrEditedItem.Id.HasValue)
                {
                    var editedItem = dictionary.Items.First(at => at.Id == model.AddedOrEditedItem.Id);
                    editedItem.Name = model.AddedOrEditedItem.Name;
                }
                _db.SaveChanges();
                ModelState.Clear();
                model = DtoHelper.GeEditDictionaryDto(dictionary);
            }

            return View(model);
        }

        private bool IsAdditionNewDictionaryItem(EditDictionaryDto model)
        {
            var result = model.AddedOrEditedItem != null && !model.AddedOrEditedItem.Id.HasValue;
            return result;
        }

        public IActionResult Delete(int id)
        {
            if (id < 1)
                return NotFound();
            var dictionary = _db.Dictionaries.FirstOrDefault(t => t.Id == id);
            if (dictionary == null)
                return NotFound();
            _db.Dictionaries.Remove(dictionary);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult DeleteItem(int id)
        {
            if (id < 1)
                return NotFound();
            var dictionaryItem = _db.DictionaryItems
                .Include(i => i.Dictionary)
                .FirstOrDefault(i => i.Id == id);
            if (dictionaryItem == null)
                return NotFound();
            if (dictionaryItem.Dictionary == null)
                return NotFound();
            var dictionaryId = dictionaryItem.DictionaryId;
            _db.DictionaryItems.Remove(dictionaryItem);
            _db.SaveChanges();
            return RedirectToAction(nameof(Edit), new { id = dictionaryId });
        }
    }
}
