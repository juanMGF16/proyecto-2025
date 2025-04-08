namespace Entity.Model
{
    public class Item
    {
        public int Id { get; set; }
        public string Code { get;set; }
        public string CodeQr { get;set; }
        public string Name { get;set; }
        public string Description { get;set; }
        public int CategoryId { get;set; }
        public string Status { get;set; }
        public DateTime CreatedAt { get;set; }

        public int ZoneId { get; set; }
        public Zone Zone { get; set; }
        public Category Category { get; set; }
        public List<ImageItem>ImageItems { get; set; }
    }
}