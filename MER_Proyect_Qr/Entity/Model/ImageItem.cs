namespace Entity.Model
{
    public class ImageItem
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public string UrlImage { get; set; }
        public DateTime DateRegistry { get;set; }
        public Item Item { get; set; }
    }
}