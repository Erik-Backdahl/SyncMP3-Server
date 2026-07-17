using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

[ApiController]
[Route("api/[controller]")]
public class NetworkController : ControllerBase
{
    private readonly INetworkService _networkService;
    public NetworkController(INetworkService networkService)
    {
        _networkService = networkService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(
    )
    {
        var userId = Guid.Parse(User.FindFirst("sub")!.Value!);

        var networkInfoDTO = await _networkService.CreateNewNetwork(userId);

        //TODO: replace string.Empty with info url 
        return Created(string.Empty, networkInfoDTO);
    }
    [HttpGet("generate-key")]
    public async Task<IActionResult> GenerateKey(
        [FromHeader(Name = "X-Network-Id")] Guid networkId
    )
    {
        var key = await _networkService.GenerateNewNetworkKey(networkId);

        return Ok(new NetworkCreateResponseDTO
        {
            NetworkId = key.NetworkId,
            Code = key.Code,
            Expires = key.ExpiresAtUtc
        });
    }
    [HttpPatch("join")]
    public async Task<IActionResult> Join(
        [FromHeader(Name = "X-Code")] string code
    )
    {
        var userId = Guid.Parse(User.FindFirst("sub")!.Value!);

        var network = await _networkService.TryJoinNetwork(userId, code);

        return Ok(new NetworkInfoDTO
        {
            NetworkId = network.Id,
            TotalMembers = network.Users.Count
        });
    }
    [HttpPatch("remove-user")]
    public async Task<IActionResult> RemoveUser(
        [FromHeader(Name = "X-Network-Id")] Guid networkId,
        [FromHeader(Name = "X-Remove-Id")] Guid removeUserId
    )
    {
        var userId = Guid.Parse(User.FindFirst("sub")!.Value!);

        await _networkService.RemoveUser(userId, removeUserId, networkId);

        return NoContent();
    }
    [HttpPatch("leave")]
    public async Task<IActionResult> LeaveNetwork(
        [FromHeader(Name = "X-Network-Id")] Guid networkId
    )
    {
        var userId = Guid.Parse(User.FindFirst("sub")!.Value!);

        await _networkService.LeaveNetwork(networkId, userId);

        return NoContent();
    }
    [HttpPost("transfer-title")]
    public async Task<IActionResult> TransferTitle(
        [FromHeader(Name = "X-Network-Id")] Guid networkId,
        [FromHeader(Name = "X-New-Owner-Id")] Guid newOwnerId
    )
    {
        var userId = Guid.Parse(User.FindFirst("sub")!.Value!);

        await _networkService.TransferTitle(networkId, userId, newOwnerId);

        return Ok();
    }

    /* 
    [HttpGet("info")]
 */
}