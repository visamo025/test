using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace WebApplication3.Models
{
    public partial class Complain
    {
        public int Cid { get; set; }
        public string Details { get; set; }
        public string Personname { get; set; }
        public string Status { get; set; }
        public int Uid { get; set; }
        public DateTime Cdate { get; set; }
        public DateTime Rdate { get; set; }
        public string Type { get; set; }

        public virtual Users U { get; set; }
    }
}
