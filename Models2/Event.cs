using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace WebApplication3.Models2
{
    public partial class Event
    {
        public int Eid { get; set; }
        public string Eventname { get; set; }
        public int Amount { get; set; }
        public DateTime Reqdate { get; set; }
        public DateTime Bookdate { get; set; }
        public int Ndays { get; set; }
        public int Uid { get; set; }

        public virtual Users U { get; set; }
    }
}
