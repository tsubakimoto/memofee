using Microsoft.EntityFrameworkCore;
using Memofee.Models;

namespace Memofee.Data;

/// <summary>
/// Entity Framework DbContext for the Memofee application.
/// </summary>
public class MemoFeeDbContext : DbContext
{
    public MemoFeeDbContext(DbContextOptions<MemoFeeDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the Feeds DbSet.
    /// </summary>
    public DbSet<Feed> Feeds { get; set; } = null!;

    /// <summary>
    /// Gets or sets the Articles DbSet.
    /// </summary>
    public DbSet<Article> Articles { get; set; } = null!;

    /// <summary>
    /// Gets or sets the Notes DbSet.
    /// </summary>
    public DbSet<Note> Notes { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Feed entity
        modelBuilder.Entity<Feed>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Url).IsUnique();
            entity.Property(e => e.Url).IsRequired().HasMaxLength(2048);
            entity.Property(e => e.Title).HasMaxLength(500);
            entity.Property(e => e.LastError).HasMaxLength(1000);
        });

        // Configure Article entity
        modelBuilder.Entity<Article>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(128);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(500);
            entity.Property(e => e.LinkUrl).IsRequired().HasMaxLength(2048);
            entity.Property(e => e.Summary).HasMaxLength(2000);
            entity.Property(e => e.Author).HasMaxLength(200);
            entity.Property(e => e.Tags).HasMaxLength(1000);

            // Set up relationship with Feed
            entity.HasOne(e => e.Feed)
                  .WithMany(f => f.Articles)
                  .HasForeignKey(e => e.FeedId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Note entity
        modelBuilder.Entity<Note>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Body).IsRequired().HasMaxLength(5000);
            entity.Property(e => e.Tags).HasMaxLength(1000);

            // Set up relationship with Article
            entity.HasOne(e => e.Article)
                  .WithMany(a => a.Notes)
                  .HasForeignKey(e => e.ArticleId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}