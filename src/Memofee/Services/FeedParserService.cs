using System.Security.Cryptography;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using Memofee.Models;

namespace Memofee.Services;

/// <summary>
/// Service for parsing RSS/Atom feeds and converting them to Article models.
/// </summary>
public interface IFeedParserService
{
    /// <summary>
    /// Fetches and parses a feed from the given URL.
    /// </summary>
    /// <param name="feedUrl">The URL of the feed to parse</param>
    /// <param name="feedId">The ID of the feed in the database</param>
    /// <returns>A tuple containing the feed title and list of articles</returns>
    Task<(string? feedTitle, IList<Article> articles)> ParseFeedAsync(string feedUrl, string feedId);

    /// <summary>
    /// Generates a stable ID for an article based on feed URL and entry identifiers.
    /// </summary>
    /// <param name="feedUrl">The feed URL</param>
    /// <param name="entryId">The entry ID from the feed</param>
    /// <param name="linkUrl">The link URL as fallback</param>
    /// <returns>A stable article ID</returns>
    string GenerateArticleId(string feedUrl, string? entryId, string linkUrl);
}

/// <summary>
/// Implementation of feed parsing service using System.ServiceModel.Syndication.
/// </summary>
public class FeedParserService : IFeedParserService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<FeedParserService> _logger;

    public FeedParserService(HttpClient httpClient, ILogger<FeedParserService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<(string? feedTitle, IList<Article> articles)> ParseFeedAsync(string feedUrl, string feedId)
    {
        try
        {
            _logger.LogDebug("Fetching feed from {FeedUrl}", feedUrl);

            // Fetch the feed content
            var response = await _httpClient.GetAsync(feedUrl);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            
            // Parse the syndication feed
            using var stringReader = new StringReader(content);
            using var xmlReader = XmlReader.Create(stringReader);
            
            var feed = SyndicationFeed.Load(xmlReader);
            
            var articles = new List<Article>();
            var feedTitle = feed.Title?.Text;

            foreach (var item in feed.Items)
            {
                var linkUrl = item.Links.FirstOrDefault()?.Uri?.ToString();
                if (string.IsNullOrEmpty(linkUrl))
                {
                    _logger.LogWarning("Skipping feed item without link URL from {FeedUrl}", feedUrl);
                    continue;
                }

                var entryId = item.Id;
                var articleId = GenerateArticleId(feedUrl, entryId, linkUrl);

                var article = new Article
                {
                    Id = articleId,
                    FeedId = feedId,
                    Title = item.Title?.Text ?? "No Title",
                    LinkUrl = linkUrl,
                    PublishedAt = item.PublishDate == DateTimeOffset.MinValue ? null : item.PublishDate,
                    Summary = item.Summary?.Text ?? item.Content?.ToString(),
                    Author = item.Authors.FirstOrDefault()?.Name,
                    Tags = SerializeTags(item.Categories.Select(c => c.Name).ToArray()),
                    FetchedAt = DateTimeOffset.UtcNow
                };

                articles.Add(article);
            }

            _logger.LogInformation("Successfully parsed {ArticleCount} articles from feed {FeedUrl}", articles.Count, feedUrl);
            return (feedTitle, articles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse feed from {FeedUrl}", feedUrl);
            throw;
        }
    }

    public string GenerateArticleId(string feedUrl, string? entryId, string linkUrl)
    {
        // Use entry ID if available, otherwise fall back to link URL
        var identifier = !string.IsNullOrEmpty(entryId) ? entryId : linkUrl;
        
        // Create a stable hash from feed URL and identifier
        var combined = $"{feedUrl}|{identifier}";
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combined));
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }

    private static string SerializeTags(string[] tags)
    {
        if (tags == null || tags.Length == 0)
            return "[]";

        return System.Text.Json.JsonSerializer.Serialize(tags);
    }
}