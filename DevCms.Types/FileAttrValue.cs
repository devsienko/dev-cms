namespace DevCms.ContentTypes
{
    public class FileAttrValue
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] Bytes { get; set; }
    }
}
