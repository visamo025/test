using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace WebApplication3.Models
{
    public partial class Notice
    {
        public int Nid { get; set; }
        public string Details { get; set; }
        public DateTime Ndate { get; set; }
        public int Uid { get; set; }
        public string Type { get; set; }

        public virtual Users U { get; set; }
    }
}
