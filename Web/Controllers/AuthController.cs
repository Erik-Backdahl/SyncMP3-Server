using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly IUserService _userService;

    public AuthController(ITokenService tokenService, IUserService userService)
    {
        _tokenService = tokenService;
        _userService = userService;
    }

    [HttpPost("anonymous")]
    public async Task<IActionResult> CreateAnonymousUser()
    {
        var guid = Guid.NewGuid();
        var token = _tokenService.GenerateToken(guid, UserType.Anonymous);

        await _userService.CreateUser(guid);
        return Ok(new { token });
    }
}