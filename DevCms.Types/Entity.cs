using System;
using System.Collections.Generic;
using System.Linq;

namespace DevCms.ContentTypes
{
    public class Entity
    {
        public int Id { get; set; }
        public List<AttrValue> AttrValues { get; set; }
        public EntityType EntityType { get; set; }
        public int EntityTypeId { get; set; }

        public AttrValue this[string attributeName] => GetAttributeValue(attributeName);

        public AttrValue GetAttributeValue(string attributeName)
        {
            if (string.IsNullOrEmpty(attributeName))
                throw new ArgumentNullException(nameof(attributeName));
            var result = AttrValues.FirstOrDefault(av => av.Attr.Name == attributeName);
            return result;
        }
    }
}
