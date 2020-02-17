using System;
using System.Collections.Generic;
using System.Linq;
using DevCms.Areas.Admin.Components;
using DevCms.ContentTypes;
using Microsoft.AspNetCore.Mvc;
using DevCms.Models;
using Microsoft.AspNetCore.Authorization;

namespace DevCms.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class DictionariesController : Controller
    {
        private readonly DevCmsDb _context;

        public DictionariesController(DevCmsDb context)
        {
            _context = context;
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
                var lastId = Dictionaries.Dicts.Max(d => d.Id);
                var dictionary = new Dictionary
                {
                    Id = lastId + 1,
                    Name = model.Name,
                    Items = new List<DictionaryItem>()
                };
                Dictionaries.Dicts.Add(dictionary);
                return View(nameof(Edit), new EditDictionaryDto
                {
                    Id = dictionary.Id,
                    Name = dictionary.Name,
                    Items = dictionary.Items.Select(i => new EditDictionaryItemDto
                    {
                        Id = i.Id,
                        Name = i.Name
                    })
                });
            }
            return View(model);
        }

        public IActionResult Edit(int id, int? itemId)
        {
            if (id < 0)
                return NotFound();
            if (itemId.HasValue && itemId < 0)
                return NotFound();
            var dictionary = Dictionaries.Dicts
                .FirstOrDefault(t => t.Id == id);
            if (dictionary == null)
                return NotFound();
            var model = new EditDictionaryDto
            {
                Id = dictionary.Id,
                Name = dictionary.Name,
                Items = dictionary.Items.Select(i => new EditDictionaryItemDto
                {
                    Id = i.Id,
                    Name = i.Name
                })
            };
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
                var dictionary = Dictionaries.Dicts
                    .FirstOrDefault(t => t.Id == model.Id);
                if (dictionary == null)
                    return NotFound();
                dictionary.Items.Add(new DictionaryItem
                {
                    Id = dictionary.Items.DefaultIfEmpty(new DictionaryItem{ Id = 0 }).Max(i => i.Id) + 1,
                    Name = model.AddedOrEditedItem.Name
                });
                ModelState.Clear();
                model = new EditDictionaryDto
                {
                    Id = dictionary.Id,
                    Name = dictionary.Name,
                    Items = dictionary.Items.Select(i => new EditDictionaryItemDto
                    {
                        Id = i.Id,
                        Name = i.Name
                    })
                };
            }
            else if (ModelState.IsValid)
            {
                var dictionary = Dictionaries.Dicts
                    .FirstOrDefault(t => t.Id == model.Id);
                if (dictionary == null)
                    return NotFound();
                dictionary.Name = model.Name;
                if (model.AddedOrEditedItem != null && model.AddedOrEditedItem.Id.HasValue)
                {
                    var editedAttr = dictionary.Items.First(at => at.Id == model.AddedOrEditedItem.Id);
                    editedAttr.Name = model.AddedOrEditedItem.Name;
                }
                ModelState.Clear();
                model = new EditDictionaryDto
                {
                    Id = dictionary.Id,
                    Name = dictionary.Name,
                    Items = dictionary.Items.Select(i => new EditDictionaryItemDto
                    {
                        Id = i.Id,
                        Name = i.Name
                    })
                };
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
            var dictionary = Dictionaries.Dicts.FirstOrDefault(t => t.Id == id);
            if (dictionary == null)
                return NotFound();
            Dictionaries.Dicts.Remove(dictionary);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult DeleteItem(int id)
        {
            if (id < 1)
                return NotFound();
            var dictionary = Dictionaries.Dicts.FirstOrDefault(t => t.Items.Any(i => i.Id == id));
            if (dictionary == null)
                return NotFound();
            var itemToDeleting = dictionary.Items.First(i => i.Id == id);
            dictionary.Items.Remove(itemToDeleting);
            return RedirectToAction(nameof(Edit), new { id = dictionary.Id });
        }
    }
}
