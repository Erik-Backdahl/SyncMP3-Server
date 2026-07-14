using System.Net.Http.Headers;

internal static class GenerateKeyScenarios
{
    internal record ExpiredKeyScenario(
        DomainUser User
    );

    internal static async Task<ExpiredKeyScenario> ExpiredKey(SyncMp3DbContext db)
    {
        var (owner, otherMembers) = await TestUserDataCreator.CreateEmptyNetworkOwnerAndMembers(db);

        var networkKey = await TestUserDataCreator.AddNetworkKeyToNetwork(db, (Guid)owner.NetworkId!, 0);

        return new ExpiredKeyScenario(owner);
    }
    internal record TooRecentKeyScenario(
        DomainUser User,
        NetworkKey NetworkKey
    );

    internal static async Task<TooRecentKeyScenario> TooRecentKey(SyncMp3DbContext db)
    {
        var (owner, otherMembers) = await TestUserDataCreator.CreateEmptyNetworkOwnerAndMembers(db);

        var networkKey = await TestUserDataCreator.AddNetworkKeyToNetwork(db, (Guid)owner.NetworkId!, 60);
        return new TooRecentKeyScenario(owner, networkKey);
    }
}
