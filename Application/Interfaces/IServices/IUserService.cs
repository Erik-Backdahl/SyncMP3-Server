public interface IUserService
{
    Task<bool> UserNetworkMatchesUser(Guid userId, string? networkId);
    Task<bool> UserExists(Guid id);
    Task CreateUser(Guid id);
}