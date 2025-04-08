namespace Entity.Model
{
    public class Zone
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public int InventaryDetailsId { get; set; }
        public Branch Branch { get; set; }
        public List<InventaryDetails> InventaryDetails { get; set; } = new List<InventaryDetails>();
        public List<Item> Items { get; set; } = new List<Item>();
    }
}