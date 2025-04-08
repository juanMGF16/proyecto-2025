using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity.Model;

namespace Entity.DTOs
{
    class ZoneDto
    {
        public int Id { get; set; }

        public int BranchId { get; set; } 

        public int InventoryDetailsId { get; set; }
        public List<InventaryDetails> InventoryDetailsIds { get; set; } = new List<InventaryDetails>(); 
        public List<Item> ItemIds { get; set; } = new List<Item>(); 
    }
}
