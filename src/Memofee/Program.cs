using Memofee.Client.Pages;
using Memofee.Components;
using Memofee.Data;
using Memofee.Services;
using Memofee.Models;
using Memofee.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// Add Entity Framework
builder.Services.AddDbContext<MemoFeeDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? "Server=(localdb)\\mssqllocaldb;Database=MemoFee;Trusted_Connection=true;MultipleActiveResultSets=true";
    options.UseSqlServer(connectionString);
});

// Add HTTP client for feed parsing
builder.Services.AddHttpClient<IFeedParserService, FeedParserService>();

// Add custom services
builder.Services.AddScoped<IFeedParserService, FeedParserService>();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found");

app.UseHttpsRedirection();

// Add Easy Auth middleware
app.UseMiddleware<EasyAuthMiddleware>();

app.UseAntiforgery();

app.UseStaticFiles();

// API Routes
app.MapApiRoutes();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Memofee.Client._Imports).Assembly);

app.Run();

/// <summary>
/// Extension method to map API routes.
/// </summary>
public static class ApiRouteExtensions
{
    public static void MapApiRoutes(this WebApplication app)
    {
        var api = app.MapGroup("/api");

        // Feed APIs
        api.MapGet("/feeds", GetFeeds);
        api.MapPost("/feeds", CreateFeed);
        api.MapDelete("/feeds/{id}", DeleteFeed);
        api.MapPost("/feeds/{id}/refresh", RefreshFeed);

        // Article APIs
        api.MapGet("/articles", GetArticles);
        api.MapGet("/articles/{id}", GetArticle);

        // Note APIs
        api.MapGet("/articles/{articleId}/notes", GetNotes);
        api.MapPut("/articles/{articleId}/notes", CreateOrUpdateNote);
        api.MapDelete("/articles/{articleId}/notes/{noteId}", DeleteNote);
    }

    static async Task<IResult> GetFeeds(MemoFeeDbContext db)
    {
        var feeds = await db.Feeds
            .OrderByDescending(f => f.CreatedAt)
            .Select(f => new FeedResponse
            {
                Id = f.Id,
                Url = f.Url,
                Title = f.Title,
                CreatedAt = f.CreatedAt,
                LastFetchedAt = f.LastFetchedAt,
                LastError = f.LastError
            })
            .ToListAsync();

        return Results.Ok(feeds);
    }

    static async Task<IResult> CreateFeed(CreateFeedRequest request, MemoFeeDbContext db, IFeedParserService feedParser)
    {
        if (string.IsNullOrWhiteSpace(request.Url))
        {
            return Results.BadRequest("Feed URL is required");
        }

        // Check if feed already exists
        var existingFeed = await db.Feeds.FirstOrDefaultAsync(f => f.Url == request.Url);
        if (existingFeed is not null)
        {
            return Results.Conflict("Feed with this URL already exists");
        }

        var feed = new Feed
        {
            Url = request.Url
        };

        try
        {
            // Parse the feed to get title and initial articles
            var (feedTitle, articles) = await feedParser.ParseFeedAsync(request.Url, feed.Id);
            feed.Title = feedTitle;
            feed.LastFetchedAt = DateTimeOffset.UtcNow;

            db.Feeds.Add(feed);

            // Add articles that don't already exist
            foreach (var article in articles)
            {
                var existingArticle = await db.Articles.FirstOrDefaultAsync(a => a.Id == article.Id);
                if (existingArticle is null)
                {
                    db.Articles.Add(article);
                }
            }

            await db.SaveChangesAsync();

            var response = new FeedResponse
            {
                Id = feed.Id,
                Url = feed.Url,
                Title = feed.Title,
                CreatedAt = feed.CreatedAt,
                LastFetchedAt = feed.LastFetchedAt,
                LastError = feed.LastError
            };

            return Results.Created($"/api/feeds/{feed.Id}", response);
        }
        catch (Exception ex)
        {
            feed.LastError = ex.Message;
            db.Feeds.Add(feed);
            await db.SaveChangesAsync();

            return Results.BadRequest($"Failed to parse feed: {ex.Message}");
        }
    }

    static async Task<IResult> DeleteFeed(string id, MemoFeeDbContext db)
    {
        var feed = await db.Feeds.FindAsync(id);
        if (feed is null)
        {
            return Results.NotFound();
        }

        db.Feeds.Remove(feed);
        await db.SaveChangesAsync();

        return Results.NoContent();
    }

    static async Task<IResult> RefreshFeed(string id, MemoFeeDbContext db, IFeedParserService feedParser)
    {
        var feed = await db.Feeds.FindAsync(id);
        if (feed is null)
        {
            return Results.NotFound();
        }

        try
        {
            var (feedTitle, articles) = await feedParser.ParseFeedAsync(feed.Url, feed.Id);
            
            if (!string.IsNullOrEmpty(feedTitle))
            {
                feed.Title = feedTitle;
            }
            feed.LastFetchedAt = DateTimeOffset.UtcNow;
            feed.LastError = null;

            // Add new articles
            foreach (var article in articles)
            {
                var existingArticle = await db.Articles.FirstOrDefaultAsync(a => a.Id == article.Id);
                if (existingArticle is null)
                {
                    db.Articles.Add(article);
                }
            }

            await db.SaveChangesAsync();

            return Results.Ok(new { message = "Feed refreshed successfully", articlesAdded = articles.Count });
        }
        catch (Exception ex)
        {
            feed.LastError = ex.Message;
            await db.SaveChangesAsync();

            return Results.BadRequest($"Failed to refresh feed: {ex.Message}");
        }
    }

    static async Task<IResult> GetArticles(MemoFeeDbContext db, 
        string? q = null, string? tag = null, bool? starred = null, string? feedId = null, 
        int page = 1, int pageSize = 20)
    {
        var query = db.Articles.AsQueryable();

        if (!string.IsNullOrEmpty(feedId))
        {
            query = query.Where(a => a.FeedId == feedId);
        }

        if (!string.IsNullOrEmpty(q))
        {
            query = query.Where(a => a.Title.Contains(q) || 
                                   (a.Summary != null && a.Summary.Contains(q)) ||
                                   a.LinkUrl.Contains(q));
        }

        if (!string.IsNullOrEmpty(tag))
        {
            query = query.Where(a => a.Tags.Contains(tag));
        }

        if (starred.HasValue)
        {
            if (starred.Value)
            {
                query = query.Where(a => a.Notes.Any(n => n.Starred));
            }
            else
            {
                query = query.Where(a => !a.Notes.Any(n => n.Starred));
            }
        }

        var totalCount = await query.CountAsync();
        
        var articles = await query
            .Include(a => a.Notes)
            .OrderByDescending(a => a.PublishedAt ?? a.FetchedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new ArticleResponse
            {
                Id = a.Id,
                FeedId = a.FeedId,
                Title = a.Title,
                LinkUrl = a.LinkUrl,
                PublishedAt = a.PublishedAt,
                Summary = a.Summary,
                Author = a.Author,
                Tags = DeserializeTags(a.Tags),
                FetchedAt = a.FetchedAt,
                HasNotes = a.Notes.Any(),
                Starred = a.Notes.Any(n => n.Starred)
            })
            .ToListAsync();

        return Results.Ok(new { articles, totalCount, page, pageSize });
    }

    static async Task<IResult> GetArticle(string id, MemoFeeDbContext db)
    {
        var article = await db.Articles
            .Include(a => a.Notes)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (article is null)
        {
            return Results.NotFound();
        }

        var response = new ArticleResponse
        {
            Id = article.Id,
            FeedId = article.FeedId,
            Title = article.Title,
            LinkUrl = article.LinkUrl,
            PublishedAt = article.PublishedAt,
            Summary = article.Summary,
            Author = article.Author,
            Tags = DeserializeTags(article.Tags),
            FetchedAt = article.FetchedAt,
            HasNotes = article.Notes.Any(),
            Starred = article.Notes.Any(n => n.Starred)
        };

        return Results.Ok(response);
    }

    static async Task<IResult> GetNotes(string articleId, MemoFeeDbContext db)
    {
        var notes = await db.Notes
            .Where(n => n.ArticleId == articleId)
            .OrderByDescending(n => n.UpdatedAt)
            .Select(n => new NoteResponse
            {
                Id = n.Id,
                ArticleId = n.ArticleId,
                Body = n.Body,
                Tags = DeserializeTags(n.Tags),
                Starred = n.Starred,
                CreatedAt = n.CreatedAt,
                UpdatedAt = n.UpdatedAt
            })
            .ToListAsync();

        return Results.Ok(notes);
    }

    static async Task<IResult> CreateOrUpdateNote(string articleId, CreateUpdateNoteRequest request, MemoFeeDbContext db)
    {
        var article = await db.Articles.FindAsync(articleId);
        if (article is null)
        {
            return Results.NotFound("Article not found");
        }

        // For single user app, we assume one note per article
        var existingNote = await db.Notes.FirstOrDefaultAsync(n => n.ArticleId == articleId);

        if (existingNote is not null)
        {
            // Update existing note
            existingNote.Body = request.Body;
            existingNote.Tags = SerializeTags(request.Tags);
            existingNote.Starred = request.Starred;
            existingNote.UpdatedAt = DateTimeOffset.UtcNow;
        }
        else
        {
            // Create new note
            var note = new Note
            {
                ArticleId = articleId,
                Body = request.Body,
                Tags = SerializeTags(request.Tags),
                Starred = request.Starred
            };
            db.Notes.Add(note);
        }

        await db.SaveChangesAsync();

        var responseNote = existingNote ?? await db.Notes.FirstAsync(n => n.ArticleId == articleId);
        var response = new NoteResponse
        {
            Id = responseNote.Id,
            ArticleId = responseNote.ArticleId,
            Body = responseNote.Body,
            Tags = DeserializeTags(responseNote.Tags),
            Starred = responseNote.Starred,
            CreatedAt = responseNote.CreatedAt,
            UpdatedAt = responseNote.UpdatedAt
        };

        return Results.Ok(response);
    }

    static async Task<IResult> DeleteNote(string articleId, string noteId, MemoFeeDbContext db)
    {
        var note = await db.Notes.FirstOrDefaultAsync(n => n.Id == noteId && n.ArticleId == articleId);
        if (note is null)
        {
            return Results.NotFound();
        }

        db.Notes.Remove(note);
        await db.SaveChangesAsync();

        return Results.NoContent();
    }

    private static string[] DeserializeTags(string tagsJson)
    {
        try
        {
            return JsonSerializer.Deserialize<string[]>(tagsJson) ?? Array.Empty<string>();
        }
        catch
        {
            return Array.Empty<string>();
        }
    }

    private static string SerializeTags(string[] tags)
    {
        return JsonSerializer.Serialize(tags ?? Array.Empty<string>());
    }
}
