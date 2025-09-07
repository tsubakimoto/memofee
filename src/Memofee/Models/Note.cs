using System.ComponentModel.DataAnnotations;

namespace Memofee.Models;

/// <summary>
/// Represents a personal note that a user has added to an article.
/// </summary>
public class Note
{
    /// <summary>
    /// Gets or sets the unique identifier for this note.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Gets or sets the article ID this note is associated with.
    /// </summary>
    [Required]
    public string ArticleId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the text content of the note.
    /// </summary>
    [Required]
    public string Body { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets tags associated with this note.
    /// Stored as JSON array.
    /// </summary>
    public string Tags { get; set; } = "[]";

    /// <summary>
    /// Gets or sets whether this article is starred/pinned.
    /// </summary>
    public bool Starred { get; set; }

    /// <summary>
    /// Gets or sets when this note was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets when this note was last updated.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Navigation property for the article this note belongs to.
    /// </summary>
    public Article Article { get; set; } = null!;
}