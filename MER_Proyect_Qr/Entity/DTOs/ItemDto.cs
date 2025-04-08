using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity.Model;

namespace Entity.DTOs
{
    class ItemDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string CodeQr { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } 
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public int ZoneId { get; set; } 

        public string ZoneName { get; set; } 

        public List<ImageItem> ImageItem { get; set; } = new List<ImageItem>(); 
    }
}
