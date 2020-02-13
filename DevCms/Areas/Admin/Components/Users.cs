using System.Linq;
using DevCms.Models;
using Microsoft.AspNetCore.Mvc;

namespace DevCms.Areas.Admin.Components
{
    public class Users : ViewComponent
    {
        private readonly DevCmsDb _db;
        public Users(DevCmsDb context)
        {
            _db = context;
        }

        public IViewComponentResult Invoke()
        {
            var model = _db.Users
                .OrderByDescending(n => n.Email)
                .ToList();
            return View(model);
        }
    }
}
