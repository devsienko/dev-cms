namespace DevCms.ContentTypes
{
    public class Attribute
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public AttrType AttrType { get; set; }
        public EntityType EntityType { get; set; }
        public int ContentTypeId { get; set; }
        public bool Required { get; set; }
    }
}
