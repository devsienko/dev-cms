using System.Collections.Generic;
using System.Linq;
using DevCms.ContentTypes;
using DevCms.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevCms.Areas.Admin.Components
{
    public class Dictionaries : ViewComponent
    {
        private readonly DevCmsDb _db;

        public Dictionaries(DevCmsDb context)
        {
            _db = context;
        }

        public IViewComponentResult Invoke()
        {
            return View(_db.Dictionaries.ToList());
        }
    }
}
