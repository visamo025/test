using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace WebApplication3.Models2
{
    public partial class visamodbContext : DbContext
    {
        public visamodbContext()
        {
        }

        public visamodbContext(DbContextOptions<visamodbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Complain> Complain { get; set; }
        public virtual DbSet<Event> Event { get; set; }
        public virtual DbSet<Expense> Expense { get; set; }
        public virtual DbSet<Gatepass> Gatepass { get; set; }
        public virtual DbSet<Income> Income { get; set; }
        public virtual DbSet<Notice> Notice { get; set; }
        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<Service> Service { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<Visitor> Visitor { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Data Source=LAPTOP-ETV69QIM\\SQLEXPRESS01;Initial Catalog=visamodb;Integrated Security=True;Encrypt=False");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Complain>(entity =>
            {
                entity.HasKey(e => e.Cid);

                entity.ToTable("complain");

                entity.Property(e => e.Cid).HasColumnName("cid");

                entity.Property(e => e.Cdate)
                    .HasColumnName("cdate")
                    .HasColumnType("date");

                entity.Property(e => e.Details)
                    .IsRequired()
                    .HasColumnName("details")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Personname)
                    .IsRequired()
                    .HasColumnName("personname")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Rdate)
                    .HasColumnName("rdate")
                    .HasColumnType("date");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnName("status")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnName("type")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Uid).HasColumnName("uid");

                entity.HasOne(d => d.U)
                    .WithMany(p => p.Complain)
                    .HasForeignKey(d => d.Uid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_complain_users");
            });

            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.Eid);

                entity.ToTable("event");

                entity.Property(e => e.Eid).HasColumnName("eid");

                entity.Property(e => e.Amount).HasColumnName("amount");

                entity.Property(e => e.Bookdate)
                    .HasColumnName("bookdate")
                    .HasColumnType("date");

                entity.Property(e => e.Eventname)
                    .IsRequired()
                    .HasColumnName("eventname")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Ndays).HasColumnName("ndays");

                entity.Property(e => e.Reqdate)
                    .HasColumnName("reqdate")
                    .HasColumnType("date");

                entity.Property(e => e.Uid).HasColumnName("uid");

                entity.HasOne(d => d.U)
                    .WithMany(p => p.Event)
                    .HasForeignKey(d => d.Uid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_event_users");
            });

            modelBuilder.Entity<Expense>(entity =>
            {
                entity.HasKey(e => e.Eid);

                entity.ToTable("expense");

                entity.Property(e => e.Eid).HasColumnName("eid");

                entity.Property(e => e.Amount).HasColumnName("amount");

                entity.Property(e => e.Edate)
                    .HasColumnName("edate")
                    .HasColumnType("date");

                entity.Property(e => e.Paymentmode)
                    .IsRequired()
                    .HasColumnName("paymentmode")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnName("type")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Uid).HasColumnName("uid");

                entity.HasOne(d => d.U)
                    .WithMany(p => p.Expense)
                    .HasForeignKey(d => d.Uid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_expense_users");
            });

            modelBuilder.Entity<Gatepass>(entity =>
            {
                entity.HasKey(e => e.Gid);

                entity.ToTable("gatepass");

                entity.Property(e => e.Gid).HasColumnName("gid");

                entity.Property(e => e.Date)
                    .HasColumnName("date")
                    .HasColumnType("date");

                entity.Property(e => e.Npeople)
                    .IsRequired()
                    .HasColumnName("npeople")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Time).HasColumnName("time");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnName("type")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Uid).HasColumnName("uid");

                entity.Property(e => e.Visitorname)
                    .IsRequired()
                    .HasColumnName("visitorname")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.U)
                    .WithMany(p => p.Gatepass)
                    .HasForeignKey(d => d.Uid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_gatepass_users");
            });

            modelBuilder.Entity<Income>(entity =>
            {
                entity.ToTable("income");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Amount).HasColumnName("amount");

                entity.Property(e => e.Date)
                    .HasColumnName("date")
                    .HasColumnType("date");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Details)
                    .IsRequired()
                    .HasColumnName("details")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Flatno)
                    .IsRequired()
                    .HasColumnName("flatno")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnName("status")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Uid).HasColumnName("uid");

                entity.Property(e => e.Wingno)
                    .IsRequired()
                    .HasColumnName("wingno")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.U)
                    .WithMany(p => p.Income)
                    .HasForeignKey(d => d.Uid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_income_users");
            });

            modelBuilder.Entity<Notice>(entity =>
            {
                entity.HasKey(e => e.Nid);

                entity.ToTable("notice");

                entity.Property(e => e.Nid).HasColumnName("nid");

                entity.Property(e => e.Details)
                    .IsRequired()
                    .HasColumnName("details")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Ndate)
                    .HasColumnName("ndate")
                    .HasColumnType("date");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnName("type")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Uid).HasColumnName("uid");

                entity.HasOne(d => d.U)
                    .WithMany(p => p.Notice)
                    .HasForeignKey(d => d.Uid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_notice_users");
            });

            modelBuilder.Entity<Roles>(entity =>
            {
                entity.HasKey(e => e.Rid);

                entity.ToTable("roles");

                entity.Property(e => e.Rid).HasColumnName("rid");

                entity.Property(e => e.Roles1)
                    .IsRequired()
                    .HasColumnName("roles")
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Service>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("service");

                entity.Property(e => e.Sid).HasColumnName("sid");

                entity.Property(e => e.Completedatetime)
                    .HasColumnName("completedatetime")
                    .HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Flatno)
                    .IsRequired()
                    .HasColumnName("flatno")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Nameofworker)
                    .IsRequired()
                    .HasColumnName("nameofworker")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Requestdatetime)
                    .HasColumnName("requestdatetime")
                    .HasColumnType("datetime");

                entity.Property(e => e.Typeofservice)
                    .IsRequired()
                    .HasColumnName("typeofservice")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Uid).HasColumnName("uid");

                entity.Property(e => e.Wingno)
                    .IsRequired()
                    .HasColumnName("wingno")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.U)
                    .WithMany(p => p.Service)
                    .HasForeignKey(d => d.Uid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_service_users");
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasKey(e => e.Uid);

                entity.ToTable("users");

                entity.Property(e => e.Uid).HasColumnName("uid");

                entity.Property(e => e.Contactno)
                    .IsRequired()
                    .HasColumnName("contactno")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Flatno)
                    .HasColumnName("flatno")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Photo)
                    .IsRequired()
                    .HasColumnName("photo")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Rid).HasColumnName("rid");

                entity.Property(e => e.Wingno)
                    .HasColumnName("wingno")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.R)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.Rid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_users_roles");
            });

            modelBuilder.Entity<Visitor>(entity =>
            {
                entity.HasKey(e => e.Vid);

                entity.ToTable("visitor");

                entity.Property(e => e.Vid).HasColumnName("vid");

                entity.Property(e => e.Datetime)
                    .HasColumnName("datetime")
                    .HasColumnType("datetime");

                entity.Property(e => e.Flatno)
                    .IsRequired()
                    .HasColumnName("flatno")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Numberofvisitor)
                    .IsRequired()
                    .HasColumnName("numberofvisitor")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnName("type")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Uid).HasColumnName("uid");

                entity.Property(e => e.Vname)
                    .IsRequired()
                    .HasColumnName("vname")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Wingno)
                    .IsRequired()
                    .HasColumnName("wingno")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.U)
                    .WithMany(p => p.Visitor)
                    .HasForeignKey(d => d.Uid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_visitor_users");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
