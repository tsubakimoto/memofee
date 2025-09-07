namespace Memofee.Dtos;

/// <summary>
/// DTO for creating a new feed.
/// </summary>
public class CreateFeedRequest
{
    /// <summary>
    /// Gets or sets the URL of the RSS/Atom feed.
    /// </summary>
    public string Url { get; set; } = string.Empty;
}

/// <summary>
/// DTO for feed response.
/// </summary>
public class FeedResponse
{
    /// <summary>
    /// Gets or sets the unique identifier for this feed.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the URL of the RSS/Atom feed.
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the title of the feed.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets when this feed was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets when this feed was last fetched.
    /// </summary>
    public DateTimeOffset? LastFetchedAt { get; set; }

    /// <summary>
    /// Gets or sets the last error message if any.
    /// </summary>
    public string? LastError { get; set; }
}