using System.Linq;
using DevCms.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevCms.Areas.Admin.Components
{
    public class ContentTypes : ViewComponent
    {
        private readonly DevCmsDb _db;
        public ContentTypes(DevCmsDb context)
        {
            _db = context;
        }

        public IViewComponentResult Invoke(int objTypeId)
        {
            var model = _db.ContentTypes
                .Include(t => t.Attrs).ToList();
            return View(model);
        }
    }
}
