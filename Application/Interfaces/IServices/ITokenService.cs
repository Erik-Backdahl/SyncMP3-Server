public interface ITokenService
{
    string GenerateToken(Guid guid, UserType accountStatus);
}