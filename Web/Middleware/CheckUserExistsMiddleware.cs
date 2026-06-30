using Microsoft.AspNetCore.Mvc;

public class CheckUserExistsMiddleware
{
    private readonly RequestDelegate _next;

    public CheckUserExistsMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    public async Task InvokeAsync(HttpContext httpContext, IUserService userService)
    {
        if (httpContext.Request.Path.StartsWithSegments("/api/auth") ||
            httpContext.Request.Path.StartsWithSegments("/openapi") ||
            httpContext.Request.Path.StartsWithSegments("/swagger"))
        {
            await _next(httpContext);
            return;
        }

        var requestUserId = httpContext.User.FindFirst("sub")?.Value;

        if (requestUserId == null)
        {
            httpContext.Response.StatusCode = 400;
            await httpContext.Response.WriteAsync("Missing 'sub' claim in token.");
            return;
        }

        if (!Guid.TryParse(requestUserId, out Guid userId))
        {
            httpContext.Response.StatusCode = 400;
            await httpContext.Response.WriteAsync("Invalid user id format.");
            return;
        }

        if (!await userService.UserExists(userId))
            await userService.CreateUser(userId);

        var networkIdHeader = httpContext.Request.Headers["X-Network-Id"].FirstOrDefault();

        if (networkIdHeader != null)
        {
            if (!Guid.TryParse(networkIdHeader, out Guid networkId))
            {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsync("Invalid network id format.");
                return;
            }

            if (!await userService.UserNetworkMatchesUser(userId, networkId.ToString()))
            {
                httpContext.Response.StatusCode = 403;
                await httpContext.Response.WriteAsync("Network does not match user");
                return;
            }

            await _next(httpContext);
            return;
        }

        if (!await userService.UserNetworkMatchesUser(userId, networkIdHeader))
        {
            httpContext.Response.StatusCode = 403;
            await httpContext.Response.WriteAsync("Network does not match user");
            return;
        }

        await _next(httpContext);
    }
}
