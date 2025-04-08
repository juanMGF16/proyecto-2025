using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity.Model;

namespace Entity.DTOs
{
    public class CompanyDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Logo { get; set; }
        public string Status { get; set; }
        public DateTime DataRegistry { get; set; }

        public List<User> User { get; set; } = new List<User>();

        public List<Branch> Branch { get; set; } = new List<Branch>();
    }
}
