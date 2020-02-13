using DevCms.Util;
using Microsoft.AspNetCore.Mvc;

namespace DevCms.Components
{
    public class Footer : ViewComponent
    {
        private readonly ContentManager _contentManager;

        public Footer(ContentManager contentManager)
        {
            _contentManager = contentManager;
        }

        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
