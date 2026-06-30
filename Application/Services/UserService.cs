public class UserService : IUserService
{
    public readonly IUserRepository _userRepository;
    
    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public async Task<bool> UserNetworkMatchesUser(Guid userId, string? networkId)
    {
        return await _userRepository.UserNetworkMatchesUser(userId, networkId);
    }
    public async Task CreateUser(Guid id)
    {
        var newUser = new DomainUser
        {
            Id = id,  
        };
        await _userRepository.CreateUser(newUser);
    }
    public async Task<bool> UserExists(Guid id)
    {
        return await _userRepository.UserExists(id);
    }
}