public interface IUserRepository
{
    Task<bool> UserNetworkMatchesUser(Guid userId, string? networkId);
    Task CreateUser(DomainUser newUser);
    Task<bool> UserExists(Guid id);
    Task<DomainUser> GetDomainUser(Guid id);
}