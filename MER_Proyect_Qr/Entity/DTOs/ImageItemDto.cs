using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOs
{
    class ImageItemDto
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public string UrlImage { get; set; }
        public DateTime DateRegistry { get; set; }
    }
}
