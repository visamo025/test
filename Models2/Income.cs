using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace WebApplication3.Models2
{
    public partial class Income
    {
        public int Id { get; set; }
        public int Amount { get; set; }
        public string Description { get; set; }
        public string Flatno { get; set; }
        public string Wingno { get; set; }
        public string Status { get; set; }
        public DateTime Date { get; set; }
        public int Uid { get; set; }
        public string Details { get; set; }

        public virtual Users U { get; set; }
    }
}
