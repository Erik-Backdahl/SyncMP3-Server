using System.ComponentModel.DataAnnotations;
using Azure.Core;

internal static class CreateScenarios
{
    public record SuccessfulCreationScenario(
        DomainUser User
    );
    public static async Task<SuccessfulCreationScenario> SuccessfulCreation(SyncMp3DbContext db)
    {
        var user = await TestUserDataCreator.CreateEmptyUser(db);

        return new SuccessfulCreationScenario(user);
    }
    public record AlreadyInNetworkScenario(
        DomainUser User
    );
    public static async Task<AlreadyInNetworkScenario> AlreadyInNetwork(SyncMp3DbContext db)
    {
        var (owner, otherMembers) = await TestUserDataCreator.CreateEmptyNetworkOwnerAndMembers(db);

        return new AlreadyInNetworkScenario(owner);
    }



}
