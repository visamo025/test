using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace WebApplication3.Models
{
    public partial class Visitor
    {
        public int Vid { get; set; }
        public string Vname { get; set; }
        public string Numberofvisitor { get; set; }
        public string Type { get; set; }
        public DateTime Datetime { get; set; }
        public int Uid { get; set; }
        public string Wingno { get; set; }
        public string Flatno { get; set; }

        public virtual Users U { get; set; }
    }
}
