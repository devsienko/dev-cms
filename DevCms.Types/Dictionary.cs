using System;
using System.Collections.Generic;
using System.Linq;

namespace DevCms.ContentTypes
{
    public class Dictionary
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<DictionaryItem> Items { get; set; }
    }
}
