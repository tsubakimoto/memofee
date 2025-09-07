using System.ComponentModel.DataAnnotations;

namespace Memofee.Models;

/// <summary>
/// Represents an article retrieved from an RSS/Atom feed.
/// </summary>
public class Article
{
    /// <summary>
    /// Gets or sets the stable unique identifier for this article.
    /// Generated as hash(feed_url, entry_id|link) to ensure consistent identification.
    /// </summary>
    [Key]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the feed ID this article belongs to.
    /// </summary>
    [Required]
    public string FeedId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the title of the article.
    /// </summary>
    [Required]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the URL link to the full article.
    /// </summary>
    [Required]
    [Url]
    public string LinkUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets when the article was published, if available from the feed.
    /// </summary>
    public DateTimeOffset? PublishedAt { get; set; }

    /// <summary>
    /// Gets or sets the summary/description of the article.
    /// </summary>
    public string? Summary { get; set; }

    /// <summary>
    /// Gets or sets the author of the article.
    /// </summary>
    public string? Author { get; set; }

    /// <summary>
    /// Gets or sets tags/categories associated with the article from the feed.
    /// Stored as JSON array.
    /// </summary>
    public string Tags { get; set; } = "[]";

    /// <summary>
    /// Gets or sets when this article was fetched from the feed.
    /// </summary>
    public DateTimeOffset FetchedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Navigation property for the feed this article belongs to.
    /// </summary>
    public Feed Feed { get; set; } = null!;

    /// <summary>
    /// Navigation property for notes associated with this article.
    /// </summary>
    public ICollection<Note> Notes { get; set; } = new List<Note>();
}