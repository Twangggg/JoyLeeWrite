using System;
using System.Collections.Generic;
using JoyLeeWrite.Models;
using Microsoft.EntityFrameworkCore;

namespace JoyLeeWrite.Data;

public partial class JoyLeeWriteDbContext : DbContext
{
    public JoyLeeWriteDbContext()
    {
    }

    public JoyLeeWriteDbContext(DbContextOptions<JoyLeeWriteDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Aiinteraction> Aiinteractions { get; set; }

    public virtual DbSet<AutoSafe> AutoSaves { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Chapter> Chapters { get; set; }

    public virtual DbSet<Series> Series { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserSetting> UserSettings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=.;Database=JoyLeeWrite;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Aiinteraction>(entity =>
        {
            entity.HasKey(e => e.InteractionId).HasName("PK__AIIntera__922C04960D77007F");

            entity.ToTable("AIInteractions", tb => tb.HasTrigger("trg_AIInteractions_SetTimestamp"));

            entity.Property(e => e.ModelUsed).HasMaxLength(200);
            entity.Property(e => e.Role).HasMaxLength(50);
            entity.Property(e => e.Timestamp).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Chapter).WithMany(p => p.Aiinteractions)
                .HasForeignKey(d => d.ChapterId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_AIInteractions_Chapter");
        });

        modelBuilder.Entity<AutoSafe>(entity =>
        {
            entity.HasKey(e => e.AutoSaveId).HasName("PK__AutoSave__558F77CD9CB69C9F");

            entity.Property(e => e.SavedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Chapter).WithMany(p => p.AutoSaves)
                .HasForeignKey(d => d.ChapterId)
                .HasConstraintName("FK_AutoSaves_Chapter");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__19093A0BD6DA4800");

            entity.HasIndex(e => e.CategoryName, "UQ__Categori__8517B2E024A88B2F").IsUnique();

            entity.Property(e => e.CategoryName).HasMaxLength(100);
        });

        modelBuilder.Entity<Chapter>(entity =>
        {
            entity.HasKey(e => e.ChapterId).HasName("PK__Chapters__0893A36A30020EAA");

            entity.ToTable(tb =>
                {
                    tb.HasTrigger("trg_Chapters_DeleteWordCount");
                    tb.HasTrigger("trg_Chapters_UpdateWordCount");
                });

            entity.HasIndex(e => new { e.SeriesId, e.ChapterNumber }, "UX_Chapters_Series_ChapterNumber").IsUnique();

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.LastModified).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("draft");
            entity.Property(e => e.Title).HasMaxLength(400);

            entity.HasOne(d => d.Series).WithMany(p => p.Chapters)
                .HasForeignKey(d => d.SeriesId)
                .HasConstraintName("FK_Chapters_Series");
        });

        modelBuilder.Entity<Series>(entity =>
        {
            entity.HasKey(e => e.SeriesId).HasName("PK__Series__F3A1C161033BE287");

            entity.ToTable(tb => tb.HasTrigger("trg_Series_UpdateTimestamp"));

            entity.Property(e => e.CoverImgUrl).HasMaxLength(1000);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.LastModified).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("draft");
            entity.Property(e => e.Tags).HasMaxLength(500);
            entity.Property(e => e.Title).HasMaxLength(400);
            entity.Property(e => e.UpdatedDate).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Author).WithMany(p => p.Series)
                .HasForeignKey(d => d.AuthorId)
                .HasConstraintName("FK_Series_Author");

            entity.HasMany(d => d.Categories).WithMany(p => p.Series)
                .UsingEntity<Dictionary<string, object>>(
                    "SeriesCategory",
                    r => r.HasOne<Category>().WithMany()
                        .HasForeignKey("CategoryId")
                        .HasConstraintName("FK_SC_Category"),
                    l => l.HasOne<Series>().WithMany()
                        .HasForeignKey("SeriesId")
                        .HasConstraintName("FK_SC_Series"),
                    j =>
                    {
                        j.HasKey("SeriesId", "CategoryId").HasName("PK__SeriesCa__523152C15871E890");
                        j.ToTable("SeriesCategories");
                    });
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4CF81CC078");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E4491C2E2D").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534E0BB7C7B").IsUnique();

            entity.Property(e => e.AvatarUrl).HasMaxLength(1000);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.FullName).HasMaxLength(200);
            entity.Property(e => e.PasswordHash).HasMaxLength(256);
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .HasDefaultValue("AUTHOR");
            entity.Property(e => e.Username).HasMaxLength(100);
        });

        modelBuilder.Entity<UserSetting>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.SettingKey }).HasName("PK__UserSett__C796BDD628DE1B7A");

            entity.Property(e => e.SettingKey).HasMaxLength(200);

            entity.HasOne(d => d.User).WithMany(p => p.UserSettings)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UserSettings_User");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
