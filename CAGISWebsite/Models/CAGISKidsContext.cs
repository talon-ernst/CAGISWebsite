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
        public virtual DbSet<Archives> Archives { get; set; }
        public virtual DbSet<AspNetRoleClaims> AspNetRoleClaims { get; set; }
        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUserRoles> AspNetUserRoles { get; set; }
        public virtual DbSet<AspNetUserTokens> AspNetUserTokens { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<Blogs> Blogs { get; set; }
        public virtual DbSet<Categories> Categories { get; set; }
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

                entity.Property(e => e.ActivityCategory).HasColumnName("activityCategory");

                entity.Property(e => e.ActivityEditDate)
                    .HasColumnName("activityEditDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.ActivityImageId).HasColumnName("activityImageID");

                entity.Property(e => e.ActivityText)
                    .IsRequired()
                    .HasColumnName("activityText")
                    .HasMaxLength(4000);

                entity.Property(e => e.ActivityTitle)
                    .IsRequired()
                    .HasColumnName("activityTitle")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ActivityUploadDate)
                    .HasColumnName("activityUploadDate")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.Activity)
                    .WithOne(p => p.Activities)
                    .HasForeignKey<Activities>(d => d.ActivityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Activities_Categories");

                entity.HasOne(d => d.ActivityImage)
                    .WithMany(p => p.Activities)
                    .HasForeignKey(d => d.ActivityImageId)
                    .HasConstraintName("FK_Activities_Images");
            });

            modelBuilder.Entity<Archives>(entity =>
            {
                entity.HasKey(e => e.PostId)
                    .HasName("PK__Archives__DD0C739AAFBD01A2");

                entity.Property(e => e.PostId)
                    .HasColumnName("postId")
                    .ValueGeneratedNever();

                entity.Property(e => e.PostArchivedDate)
                    .HasColumnName("postArchivedDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.PostCategory).HasColumnName("postCategory");

                entity.Property(e => e.PostLastEditedDate)
                    .HasColumnName("postLastEditedDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.PostText)
                    .IsRequired()
                    .HasColumnName("postText");

                entity.Property(e => e.PostTitle)
                    .IsRequired()
                    .HasColumnName("postTitle");

                entity.Property(e => e.PostUploadDate)
                    .HasColumnName("postUploadDate")
                    .HasColumnType("datetime");
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

                entity.Property(e => e.BlogCategory).HasColumnName("blogCategory");

                entity.Property(e => e.BlogEditDate)
                    .HasColumnName("blogEditDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.BlogImageId).HasColumnName("blogImageID");

                entity.Property(e => e.BlogText)
                    .IsRequired()
                    .HasColumnName("blogText")
                    .HasMaxLength(4000);

                entity.Property(e => e.BlogTitle)
                    .IsRequired()
                    .HasColumnName("blogTitle")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.BlogUploadDate)
                    .HasColumnName("blogUploadDate")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.BlogCategoryNavigation)
                    .WithMany(p => p.Blogs)
                    .HasForeignKey(d => d.BlogCategory)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Blogs_Categories");

                entity.HasOne(d => d.BlogImage)
                    .WithMany(p => p.Blogs)
                    .HasForeignKey(d => d.BlogImageId)
                    .HasConstraintName("FK_Blogs_Images");
            });

            modelBuilder.Entity<Categories>(entity =>
            {
                entity.HasKey(e => e.CategoryId);

                entity.Property(e => e.CategoryId)
                    .HasColumnName("categoryId")
                    .ValueGeneratedNever();

                entity.Property(e => e.CategoryName)
                    .IsRequired()
                    .HasColumnName("categoryName")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Contests>(entity =>
            {
                entity.HasKey(e => e.ContestId)
                    .HasName("PK__Contests__C5A32706096CBC51");

                entity.Property(e => e.ContestId)
                    .HasColumnName("contestID")
                    .ValueGeneratedNever();

                entity.Property(e => e.ContestEditDate)
                    .HasColumnName("contestEditDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.ContestEndDate)
                    .HasColumnName("contestEndDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.ContestImageId).HasColumnName("contestImageId");

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

                entity.Property(e => e.ContestUploadDate)
                    .HasColumnName("contestUploadDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasMaxLength(65);

                entity.HasOne(d => d.ContestImage)
                    .WithMany(p => p.Contests)
                    .HasForeignKey(d => d.ContestImageId)
                    .HasConstraintName("FK_Contests_Images");
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

                entity.Property(e => e.Dykcategory).HasColumnName("DYKCategory");

                entity.Property(e => e.DykeditDate)
                    .HasColumnName("DYKEditDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.DykimageId).HasColumnName("DYKImageID");

                entity.Property(e => e.Dyktext)
                    .IsRequired()
                    .HasColumnName("DYKText")
                    .HasMaxLength(4000);

                entity.Property(e => e.Dyktitle)
                    .IsRequired()
                    .HasColumnName("DYKTitle")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DykuploadDate)
                    .HasColumnName("DYKUploadDate")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.DykcategoryNavigation)
                    .WithMany(p => p.Facts)
                    .HasForeignKey(d => d.Dykcategory)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Facts_Categories");

                entity.HasOne(d => d.Dykimage)
                    .WithMany(p => p.Facts)
                    .HasForeignKey(d => d.DykimageId)
                    .HasConstraintName("FK_Facts_Images");
            });

            modelBuilder.Entity<Images>(entity =>
            {
                entity.HasKey(e => e.ImageId)
                    .HasName("PK__Images__336E9B7586C1622E");

                entity.Property(e => e.ImageId)
                    .HasColumnName("imageID")
                    .ValueGeneratedNever();

                entity.Property(e => e.ImagePath)
                    .IsRequired()
                    .HasColumnName("imagePath");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
