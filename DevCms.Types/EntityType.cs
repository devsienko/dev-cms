using System.Collections.Generic;

namespace DevCms.ContentTypes
{
    public class EntityType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Attribute> Attrs { get; set; } = new List<Attribute>();
    }
}
