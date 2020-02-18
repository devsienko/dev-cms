using System.Collections.Generic;

namespace DevCms.ContentTypes
{
    public class Dictionary
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<DictionaryItem> Items { get; set; } = new List<DictionaryItem>();
    }
}
