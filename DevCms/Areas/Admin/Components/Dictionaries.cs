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
        private static List<Dictionary> _dicts { get; set; }

        public static List<Dictionary> Dicts
        {
            get
            {
                if (_dicts == null)
                {
                    InitDicts();
                }

                return _dicts;
            }
        }

        public IViewComponentResult Invoke()
        {
            return View(Dicts);
        }

        private static void InitDicts()
        {
            if (_dicts == null)
                _dicts = new List<Dictionary>
                {
                    new Dictionary
                    {
                        Id = 1,
                        Name = "Sex",
                        Items = new List<DictionaryItem>
                        {
                            new DictionaryItem
                            {
                                Id = 1,
                                Name = "Male"
                            },
                            new DictionaryItem
                            {
                                Id = 2,
                                Name = "Female"
                            }
                        }
                    }
                };
        }
    }
}
