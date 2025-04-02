using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace WebApplication3.Models
{
    public partial class Users
    {
        public Users()
        {
            Complain = new HashSet<Complain>();
            Event = new HashSet<Event>();
            Expense = new HashSet<Expense>();
            Gatepass = new HashSet<Gatepass>();
            Income = new HashSet<Income>();
            Notice = new HashSet<Notice>();
            Service = new HashSet<Service>();
            Visitor = new HashSet<Visitor>();
        }

        public int Uid { get; set; }
        public string Flatno { get; set; }
        public string Wingno { get; set; }
        public string Name { get; set; }
        public string Contactno { get; set; }
        public string Email { get; set; }
        public string Photo { get; set; }
        public int Rid { get; set; }
        public string Password { get; set; }

        public virtual Roles R { get; set; }
        public virtual ICollection<Complain> Complain { get; set; }
        public virtual ICollection<Event> Event { get; set; }
        public virtual ICollection<Expense> Expense { get; set; }
        public virtual ICollection<Gatepass> Gatepass { get; set; }
        public virtual ICollection<Income> Income { get; set; }
        public virtual ICollection<Notice> Notice { get; set; }
        public virtual ICollection<Service> Service { get; set; }
        public virtual ICollection<Visitor> Visitor { get; set; }
    }
}
