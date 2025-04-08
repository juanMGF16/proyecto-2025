using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOs
{
    class InventaryDetailsDto
    {
        public int Id { get; set; }
        public string StatusPrevious { get; set; }
        public string StatusNew { get; set; }
        public string Observations { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public int ZoneId { get; set; } 
        public string ZoneName { get; set; } 

    }
}
