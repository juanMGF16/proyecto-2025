﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class Rol
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool Active { get; set; }
        public string Description { get; set; }

        public List<RolUser> RolUsers { get; set; } = new List<RolUser>();
        public List<RolFormPermission> rolFormPermissions { get; set; }        
    }
}
