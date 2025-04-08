using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity.Model;

namespace Entity.DTOs
{
    public class BranchDto
    {
        public int Id { get; set; }

        public int EmpresaId { get; set; }  // Referencia a la empresa

        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }

        public string Email { get; set; }
   
        public List<Zone> Zone { get; set; } = new List<Zone>(); // Solo IDs de zonas
    }
}
