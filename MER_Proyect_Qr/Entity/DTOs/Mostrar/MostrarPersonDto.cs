using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOs.Mostrar
{
    public class MostrarPersonDto
    {
        public int Id { get; set; }
        public string NameCompleted { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool Active { get; set; }
    }
}
