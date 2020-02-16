namespace DevCms.ContentTypes
{
    public class DictionaryItem
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Dictionary Dictionary { get; set; }
        public int DictionaryId { get; set; }
    }
}
