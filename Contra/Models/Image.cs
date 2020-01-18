namespace Contra.Models
{
    public class Image
    {
        public int Id { get; set; }
        public string OwnerID { get; set; }

        public string Name { get; set; }

        public string ContentType { get; set; }
        public byte[] Content { get; set; }
    }
}
