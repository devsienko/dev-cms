using System.Linq;
using System.Threading.Tasks;
using DevCms.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevCms.Controllers
{
    public class ImagesController : Controller
    {
        private readonly DevCmsDb _db;

        public ImagesController(DevCmsDb context)
        {
            _db = context;
        }

        [HttpGet]
        //todo: async
        public IActionResult Get(int id)
        {
            var av = _db.AttrValues
                .Include(attrValue => attrValue.ValueAsFile)
                .First(attrValue => attrValue.Id == id);
            return File(av.ValueAsFile.Bytes, av.ValueAsFile.ContentType, av.ValueAsFile.FileName);
        }
    }
}
