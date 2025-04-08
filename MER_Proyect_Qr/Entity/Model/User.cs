using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public DateTime CreationDate { get; set; }  
        public bool Active { get; set; }


        //public int CompanyId { get; set; }
        //public Company Company { get; set; }    

        public int PersonId { get; set; }
        public Person Person { get; set; }
        public ICollection<RolUser> RolUser { get; set; } = new List<RolUser>();

    }
}
