namespace DevCms.ContentTypes
{
    public class AttrValue
    {
        public int Id { get; set; }
        public string Value { get; set; }

        public FileAttrValue ValueAsFile { get; set; }
        public int? ValueAsFileId { get; set; }

        public Attribute Attr { get; set; }
        public int AttrId { get; set; }
        public Entity Entity { get; set; }
        public int EntityId { get; set; }

        public override string ToString()
        {
            return Value;
        }
    }
}
