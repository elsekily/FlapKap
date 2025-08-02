using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace FlapKap.API.Authorization;

public class MatchUsernameRequirement : IAuthorizationRequirement { }
public class MatchUsernameHandler : AuthorizationHandler<MatchUsernameRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public MatchUsernameHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MatchUsernameRequirement requirement)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
            return Task.CompletedTask;

        // Extract username from token claims
        var usernameInToken = context.User.FindFirst(ClaimTypes.Name)?.Value;

        // Extract username from route parameter
        var queryUsername = httpContext.Request.Query["username"].ToString();

        if (!string.IsNullOrWhiteSpace(usernameInToken) &&
            !string.IsNullOrWhiteSpace(queryUsername) &&
            string.Equals(usernameInToken, queryUsername, StringComparison.OrdinalIgnoreCase))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}