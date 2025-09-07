using System.ComponentModel.DataAnnotations;

namespace Memofee.Models;

/// <summary>
/// Represents an RSS/Atom feed that the user has registered for monitoring.
/// </summary>
public class Feed
{
    /// <summary>
    /// Gets or sets the unique identifier for this feed.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Gets or sets the URL of the RSS/Atom feed.
    /// </summary>
    [Required]
    [Url]
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the title of the feed, if discovered during parsing.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets when this feed was created/registered.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets when this feed was last successfully fetched.
    /// </summary>
    public DateTimeOffset? LastFetchedAt { get; set; }

    /// <summary>
    /// Gets or sets the last error message if feed fetching failed.
    /// </summary>
    public string? LastError { get; set; }

    /// <summary>
    /// Navigation property for articles from this feed.
    /// </summary>
    public ICollection<Article> Articles { get; set; } = new List<Article>();
}