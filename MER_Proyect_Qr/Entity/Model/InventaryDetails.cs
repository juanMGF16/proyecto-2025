namespace Entity.Model
{
    public class InventaryDetails
    {
        public int Id { get; set; }
        public string StatusPrevious { get; set; }
        public string StatusNew { get; set; }
        public string Observations { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public int ZoneId { get; set; }
        public Zone Zone { get; set; }
    }
}