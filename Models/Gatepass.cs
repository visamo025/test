using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace WebApplication3.Models
{
    public partial class Gatepass
    {
        public int Gid { get; set; }
        public string Visitorname { get; set; }
        public string Type { get; set; }
        public string Npeople { get; set; }
        public int Uid { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public string Email { get; set; }
        public string Flatno { get; set; }
        public string Wingno { get; set; }

        public virtual Users U { get; set; }
    }
}
