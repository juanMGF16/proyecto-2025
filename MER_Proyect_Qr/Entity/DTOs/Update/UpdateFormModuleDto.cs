using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity.Model;

namespace Entity.DTOs.Update
{
    public class UpdateFormModuleDto
    {
        public int Id { get; set; }
        public int ModuleId { get; set; }
        public int FormId { get; set; }
        public bool Active { get; set; }

    }
}
