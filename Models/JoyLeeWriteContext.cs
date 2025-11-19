using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace JoyLeeWrite.Models;

public partial class JoyLeeWriteContext : DbContext
{
    public JoyLeeWriteContext()
    {
    }

    public JoyLeeWriteContext(DbContextOptions<JoyLeeWriteContext> options)
        : base(options)
    {
    }

    public virtual DbSet<WritingStatistic> WritingStatistics { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.;Database=JoyLeeWrite;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WritingStatistic>(entity =>
        {
            entity.HasKey(e => e.StatId).HasName("PK__WritingS__3A162D3EE01F2E63");

            entity.HasIndex(e => new { e.UserId, e.RecordDate }, "IX_WritingStats_UserDate").IsDescending(false, true);

            entity.HasIndex(e => new { e.UserId, e.RecordDate }, "UX_WritingStats_UserDate").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
