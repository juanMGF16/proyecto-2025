using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity.Model;

namespace Entity.DTOs
{
    public class RolUserDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public int RolId { get; set; }
        //public string RolName { get; set; }
        //public string UserName { get; set; }
        public bool Active { get; set; }

    }
}
