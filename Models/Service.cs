using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace WebApplication3.Models
{
    public partial class Service
    {
        public int Sid { get; set; }
        public string Description { get; set; }
        public string Typeofservice { get; set; }
        public DateTime Requestdatetime { get; set; }
        public DateTime Completedatetime { get; set; }
        public string Nameofworker { get; set; }
        public int Uid { get; set; }
        public string Flatno { get; set; }
        public string Wingno { get; set; }

        public virtual Users U { get; set; }
    }
}
