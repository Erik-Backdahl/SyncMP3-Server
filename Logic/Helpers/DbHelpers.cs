using SyncMP3.Models;
using SyncMP3.Data;

class DbHelpers
{
    public static Dictionary<string, string> GetAllHeaders()
    {
        throw new NotImplementedException();
    }
    public static bool CheckUserInNetwork(SyncMp3Context dbContext, string uuid)
    {
        var user = dbContext.Users.FirstOrDefault(u => u.UserUuid == uuid);
        
        return user != null && !string.IsNullOrEmpty(user.NetworkGuid);
    }
}