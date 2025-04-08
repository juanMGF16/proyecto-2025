using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOs
{

    //DTOs para la entidad Rol sirve para transferir datos entre capas de la aplicacion y no exponer la entidad directamente
    public class RolDto
    {

        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
    }
}
