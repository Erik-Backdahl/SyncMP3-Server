using SyncMP3.Data;
using SyncMP3.Models;
using System.Net;
using System.Security.Cryptography;

class GetNetworkPassword
{
    internal static async Task Maintain()
    {
        
    }
    internal static async Task CheckUserBeforeGivingPassword()
    {
        
    }
    internal static async Task CreatePassword(HttpListenerResponse response, SyncMp3Context dbContext, string createdByUuid, string networkGuid)
    {
        string password = "";
        bool invalidInput = true;
        while (invalidInput)
        {
            var random = new Random();

            string num1 = random.Next(0, 10).ToString();
            string num2 = random.Next(0, 10).ToString();
            string num3 = random.Next(0, 10).ToString();
            string num4 = random.Next(0, 10).ToString();
            string num5 = random.Next(0, 10).ToString();
            string num6 = random.Next(0, 10).ToString();

            password = num1 + num2 + num3 + num4 + num5 + num6;

            if(!dbContext.NetworkPasswords.Any(n => n.Password == password))
            {
                invalidInput = false;
            }
        }

        var NetworkPassword = new NetworkPassword()
        {
            CreatedBy = createdByUuid,
            NetworkGuid = networkGuid,
            Password = password
        };

        response.AddHeader("Password", password);
        
        dbContext.NetworkPasswords.Add(NetworkPassword);
    }
}