using System.Collections.Generic;
using DevCms.ContentTypes;

namespace DevCms.Models
{
    public class CreateInstanceRequest
    {
        public int TypeId { get; set; }
        public List<AttrValue> AttrValues { get; set; }
    }

    public class SaveInstanceRequest
    {
        public int InstanseId { get; set; }
        public List<AttrValue> AttrValues { get; set; }
    }
}