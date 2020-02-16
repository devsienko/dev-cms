using System;
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
                    Name = model.Name
                };
                Dictionaries.Dicts.Add(dictionary);
                return View(nameof(Edit), new EditDictionaryDto
                {
                    Id = dictionary.Id,
                    Name = dictionary.Name
                });
            }
            return View(model);
        }

        public IActionResult Edit(int id)
        {
            if (id < 0)
                return NotFound();
            var dictionary = Dictionaries.Dicts
                .FirstOrDefault(t => t.Id == id);
            if (dictionary == null)
                return NotFound();
            return View(new EditDictionaryDto
            {
                Id = dictionary.Id,
                Name = dictionary.Name
            });
        }


        [HttpPost]
        public IActionResult Edit(EditDictionaryDto model)
        {
            if (ModelState.IsValid)
            {
                var dictionary = Dictionaries.Dicts
                    .FirstOrDefault(t => t.Id == model.Id);
                if (dictionary == null)
                    return NotFound();
                dictionary.Name = model.Name;
               
                ModelState.Clear();
                model = new EditDictionaryDto
                {
                    Id = dictionary.Id,
                    Name = dictionary.Name
                };
            }
            return View(model);
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

    }
}
