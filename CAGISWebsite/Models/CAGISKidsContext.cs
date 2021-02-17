using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

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

        public virtual DbSet<Activities> Activities { get; set; }
        public virtual DbSet<AspNetRoleClaims> AspNetRoleClaims { get; set; }
        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUserRoles> AspNetUserRoles { get; set; }
        public virtual DbSet<AspNetUserTokens> AspNetUserTokens { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<Blogs> Blogs { get; set; }
        public virtual DbSet<Contests> Contests { get; set; }
        public virtual DbSet<Employees> Employees { get; set; }
        public virtual DbSet<Facts> Facts { get; set; }
        public virtual DbSet<Images> Images { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=DESKTOP-4MMOC5A\\SQLEXPRESS;Database=CAGISKids;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Activities>(entity =>
            {
                entity.HasKey(e => e.ActivityId)
                    .HasName("PK__Activiti__0FC9CBCC63BA24DC");

                entity.Property(e => e.ActivityId)
                    .HasColumnName("activityID")
                    .ValueGeneratedNever();

                entity.Property(e => e.ActivityText)
                    .IsRequired()
                    .HasColumnName("activityText")
                    .HasMaxLength(4000);

                entity.Property(e => e.ActivityTitle)
                    .IsRequired()
                    .HasColumnName("activityTitle")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<AspNetRoleClaims>(entity =>
            {
                entity.HasIndex(e => e.RoleId);

                entity.Property(e => e.RoleId).IsRequired();

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetRoleClaims)
                    .HasForeignKey(d => d.RoleId);
            });

            modelBuilder.Entity<AspNetRoles>(entity =>
            {
                entity.HasIndex(e => e.NormalizedName)
                    .HasName("RoleNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedName] IS NOT NULL)");

                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.NormalizedName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetUserClaims>(entity =>
            {
                entity.HasIndex(e => e.UserId);

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserClaims)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserLogins>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

                entity.HasIndex(e => e.UserId);

                entity.Property(e => e.LoginProvider).HasMaxLength(128);

                entity.Property(e => e.ProviderKey).HasMaxLength(128);

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserLogins)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserRoles>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.HasIndex(e => e.RoleId);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.RoleId);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserTokens>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

                entity.Property(e => e.LoginProvider).HasMaxLength(128);

                entity.Property(e => e.Name).HasMaxLength(128);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserTokens)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUsers>(entity =>
            {
                entity.HasIndex(e => e.NormalizedEmail)
                    .HasName("EmailIndex");

                entity.HasIndex(e => e.NormalizedUserName)
                    .HasName("UserNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedUserName] IS NOT NULL)");

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

                entity.Property(e => e.UserName).HasMaxLength(256);
            });

            modelBuilder.Entity<Blogs>(entity =>
            {
                entity.HasKey(e => e.BlogId)
                    .HasName("PK__Blogs__FA0AA70D59BCB166");

                entity.Property(e => e.BlogId)
                    .HasColumnName("blogID")
                    .ValueGeneratedNever();

                entity.Property(e => e.BlogText)
                    .IsRequired()
                    .HasColumnName("blogText")
                    .HasMaxLength(4000);

                entity.Property(e => e.BlogTitle)
                    .IsRequired()
                    .HasColumnName("blogTitle")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Contests>(entity =>
            {
                entity.HasKey(e => e.ContestId)
                    .HasName("PK__Contests__C5A32706096CBC51");

                entity.Property(e => e.ContestId)
                    .HasColumnName("contestID")
                    .ValueGeneratedNever();

                entity.Property(e => e.ContestEndDate)
                    .HasColumnName("contestEndDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.ContestStartDate)
                    .HasColumnName("contestStartDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.ContestText)
                    .IsRequired()
                    .HasColumnName("contestText")
                    .HasMaxLength(4000);

                entity.Property(e => e.ContestTitle)
                    .IsRequired()
                    .HasColumnName("contestTitle")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasMaxLength(65);
            });

            modelBuilder.Entity<Employees>(entity =>
            {
                entity.HasKey(e => e.AdminId)
                    .HasName("PK__Employee__AD050086F6C0229E");

                entity.Property(e => e.AdminId)
                    .HasColumnName("adminID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Facts>(entity =>
            {
                entity.HasKey(e => e.Dykid)
                    .HasName("PK__Facts__EB2F82E9E246327C");

                entity.Property(e => e.Dykid)
                    .HasColumnName("DYKID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Dyktext)
                    .IsRequired()
                    .HasColumnName("DYKText")
                    .HasMaxLength(4000);

                entity.Property(e => e.Dyktitle)
                    .IsRequired()
                    .HasColumnName("DYKTitle")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Images>(entity =>
            {
                entity.HasKey(e => e.ImageId)
                    .HasName("PK__Images__336E9B7586C1622E");

                entity.Property(e => e.ImageId)
                    .HasColumnName("imageID")
                    .ValueGeneratedNever();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
