namespace Memofee.Dtos;

/// <summary>
/// DTO for creating/updating a note.
/// </summary>
public class CreateUpdateNoteRequest
{
    /// <summary>
    /// Gets or sets the note body/content.
    /// </summary>
    public string Body { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the tags.
    /// </summary>
    public string[] Tags { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Gets or sets whether the article is starred.
    /// </summary>
    public bool Starred { get; set; }
}

/// <summary>
/// DTO for note response.
/// </summary>
public class NoteResponse
{
    /// <summary>
    /// Gets or sets the note ID.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the article ID.
    /// </summary>
    public string ArticleId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the note body.
    /// </summary>
    public string Body { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the tags.
    /// </summary>
    public string[] Tags { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Gets or sets whether starred.
    /// </summary>
    public bool Starred { get; set; }

    /// <summary>
    /// Gets or sets when created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets when last updated.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }
}