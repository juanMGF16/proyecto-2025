namespace Entity.Model
{
    public class Branch
    {
        public int Id { get; set; }
        public int EmpresaId { get; set; }
        public string Name { get; set; }
        public int Address { get; set; }
        public int Phone { get; set; }
        public string Email { get; set; }
        public int Status { get; set; }
        public Company company { get; set; }
        public List<Zone> Zones { get; set; } = new List<Zone>();
    }
}