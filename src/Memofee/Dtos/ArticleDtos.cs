namespace Memofee.Dtos;

/// <summary>
/// DTO for article response.
/// </summary>
public class ArticleResponse
{
    /// <summary>
    /// Gets or sets the article ID.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the feed ID.
    /// </summary>
    public string FeedId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the article title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the link URL.
    /// </summary>
    public string LinkUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the published date.
    /// </summary>
    public DateTimeOffset? PublishedAt { get; set; }

    /// <summary>
    /// Gets or sets the summary.
    /// </summary>
    public string? Summary { get; set; }

    /// <summary>
    /// Gets or sets the author.
    /// </summary>
    public string? Author { get; set; }

    /// <summary>
    /// Gets or sets the tags.
    /// </summary>
    public string[] Tags { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Gets or sets when this article was fetched.
    /// </summary>
    public DateTimeOffset FetchedAt { get; set; }

    /// <summary>
    /// Gets or sets whether this article has any notes.
    /// </summary>
    public bool HasNotes { get; set; }

    /// <summary>
    /// Gets or sets whether this article is starred.
    /// </summary>
    public bool Starred { get; set; }
}