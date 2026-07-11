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
            throw new BadRequestException("Missing 'sub' claim in token.");

        if (!Guid.TryParse(requestUserId, out Guid userId))
            throw new BadRequestException("Invalid user id format.");


        if (!await userService.UserExists(userId))
            await userService.CreateUser(userId);

        var networkIdHeader = httpContext.Request.Headers["X-Network-Id"].FirstOrDefault();

        if (networkIdHeader != null)
        {
            if (!Guid.TryParse(networkIdHeader, out Guid networkId))
                throw new BadRequestException("Invalid network id format.");


            if (!await userService.UserNetworkMatchesUser(userId, networkId.ToString()))
                throw new BadRequestException("Network does not match user");

            await _next(httpContext);
            return;
        }

        if (!await userService.UserNetworkMatchesUser(userId, networkIdHeader))
            throw new BadRequestException("Network does not match user");

        await _next(httpContext);
    }
}