﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOs
{
    class LogActivity
    {
        public int Id { get; set; }
        public string Action { get; set; }

        public string TableAffected { get; set; }

        public string DataPrevious { get; set; }

        public string DataNews { get; set; }

        public DateTime Date { get; set; }
    }
}
