using System.Text.Json;

namespace Memofee.Services;

/// <summary>
/// Middleware to handle Azure App Service Easy Auth integration.
/// Validates the authenticated user against the allowed UPN/OID configuration.
/// </summary>
public class EasyAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<EasyAuthMiddleware> _logger;
    private readonly IConfiguration _configuration;

    public EasyAuthMiddleware(RequestDelegate next, ILogger<EasyAuthMiddleware> logger, IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip authentication check for health checks and static files
        if (IsPublicPath(context.Request.Path))
        {
            await _next(context);
            return;
        }

        var allowedUpn = _configuration["ALLOWED_UPN"];
        var allowedOid = _configuration["ALLOWED_OID"];

        if (string.IsNullOrEmpty(allowedUpn) && string.IsNullOrEmpty(allowedOid))
        {
            _logger.LogWarning("No ALLOWED_UPN or ALLOWED_OID configured. All authenticated users will be allowed.");
            await _next(context);
            return;
        }

        // Check for Easy Auth headers
        var clientPrincipalHeader = context.Request.Headers["X-MS-CLIENT-PRINCIPAL"].FirstOrDefault();
        var clientPrincipalNameHeader = context.Request.Headers["X-MS-CLIENT-PRINCIPAL-NAME"].FirstOrDefault();

        string? userPrincipalName = null;
        string? objectId = null;

        if (!string.IsNullOrEmpty(clientPrincipalHeader))
        {
            try
            {
                var principalJson = Convert.FromBase64String(clientPrincipalHeader);
                var principal = JsonSerializer.Deserialize<ClientPrincipal>(principalJson);
                
                userPrincipalName = principal?.UserDetails;
                objectId = principal?.UserId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to parse X-MS-CLIENT-PRINCIPAL header");
            }
        }
        else if (!string.IsNullOrEmpty(clientPrincipalNameHeader))
        {
            userPrincipalName = clientPrincipalNameHeader;
        }

        if (string.IsNullOrEmpty(userPrincipalName) && string.IsNullOrEmpty(objectId))
        {
            _logger.LogWarning("No authenticated user found in Easy Auth headers");
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Unauthorized: No authenticated user found");
            return;
        }

        // Check if user is authorized
        var isAuthorized = false;

        if (!string.IsNullOrEmpty(allowedUpn) && !string.IsNullOrEmpty(userPrincipalName))
        {
            isAuthorized = string.Equals(allowedUpn, userPrincipalName, StringComparison.OrdinalIgnoreCase);
        }

        if (!isAuthorized && !string.IsNullOrEmpty(allowedOid) && !string.IsNullOrEmpty(objectId))
        {
            isAuthorized = string.Equals(allowedOid, objectId, StringComparison.OrdinalIgnoreCase);
        }

        if (!isAuthorized)
        {
            _logger.LogWarning("User {UserPrincipalName} (OID: {ObjectId}) is not authorized", userPrincipalName, objectId);
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync("Forbidden: User not authorized to access this application");
            return;
        }

        _logger.LogDebug("Authorized user {UserPrincipalName} (OID: {ObjectId})", userPrincipalName, objectId);
        
        // Add user info to context
        context.Items["UserPrincipalName"] = userPrincipalName;
        context.Items["ObjectId"] = objectId;

        await _next(context);
    }

    private static bool IsPublicPath(PathString path)
    {
        var publicPaths = new[]
        {
            "/health",
            "/_health",
            "/favicon.ico",
            "/.well-known",
            "/css",
            "/js",
            "/_framework",
            "/lib"
        };

        return publicPaths.Any(publicPath => path.StartsWithSegments(publicPath, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Represents the client principal data from Easy Auth.
    /// </summary>
    public class ClientPrincipal
    {
        public string? IdentityProvider { get; set; }
        public string? UserId { get; set; }
        public string? UserDetails { get; set; }
        public string? UserRoles { get; set; }
    }
}