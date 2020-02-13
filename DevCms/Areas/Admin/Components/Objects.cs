using System.Linq;
using DevCms.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevCms.Areas.Admin.Components
{
    public class Objects : ViewComponent
    {
        private readonly DevCmsDb _db;
        public Objects(DevCmsDb context)
        {
            _db = context;
        }

        public IViewComponentResult Invoke(int objTypeId)
        {
            var model = _db.Content
                .Include(t => t.EntityType)
                .Include(t => t.AttrValues)
                .ThenInclude(av => av.Attr)
                .OrderByDescending(n => n.Id)
                .Where(i => i.EntityTypeId == objTypeId)
                .ToList();
            return View(model);
        }
    }
}
