using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CAGISWebsite.Models
{
    public partial class CAGISKidsContext : DbContext
    {
        public CAGISKidsContext()
        {
        }

        public CAGISKidsContext(DbContextOptions<CAGISKidsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Admin> Admin { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=CAGISKids;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.AdminFirstName)
                    .IsRequired()
                    .HasColumnName("adminFirstName")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.AdminId).HasColumnName("adminId");

                entity.Property(e => e.AdminLastName)
                    .IsRequired()
                    .HasColumnName("adminLastName")
                    .HasMaxLength(30)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
