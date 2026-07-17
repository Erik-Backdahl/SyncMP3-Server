internal static class JoinScenarios
{
    public record SuccessJoinScenario(
        DomainUser User,
        string NetworkId, // For asserting later
        NetworkKey NetworkKey
    );
    internal static async Task<SuccessJoinScenario> SuccessJoin(SyncMp3DbContext db)
    {
        var (owner, otherMembers) = await TestUserDataCreator.CreateEmptyNetworkOwnerAndMembers(db);
        var userNotInNetwork = await TestUserDataCreator.CreateEmptyUser(db);

        var networkKey = await TestUserDataCreator.AddNetworkKeyToNetwork(db, (Guid)owner.NetworkId!, 60);
        return new SuccessJoinScenario(userNotInNetwork, owner.NetworkId.ToString()!, networkKey);
    }
    public record OldCodeScenario(
        DomainUser User,
        string NetworkId, // For asserting later
        NetworkKey NetworkKey
    );
    internal static async Task<OldCodeScenario> OldCode(SyncMp3DbContext db)
    {
        var (owner, otherMembers) = await TestUserDataCreator.CreateEmptyNetworkOwnerAndMembers(db);
        var userNotInNetwork = await TestUserDataCreator.CreateEmptyUser(db);

        var networkKey = await TestUserDataCreator.AddNetworkKeyToNetwork(db, (Guid)owner.NetworkId!, 0);
        return new OldCodeScenario(userNotInNetwork, owner.NetworkId.ToString()!, networkKey);
    }
    public record UserAlreadyInNetworkScenario(
        DomainUser UserAlreadyInOtherNetwork,
        string NewNetwork, 
        NetworkKey NetworkKey
    );
    internal static async Task<UserAlreadyInNetworkScenario> UserAlreadyInNetwork(SyncMp3DbContext db)
    {
        var (owner, otherMembers) = await TestUserDataCreator.CreateEmptyNetworkOwnerAndMembers(db);
        var (userInOtherNetwork, otherMembersUnused) = await TestUserDataCreator.CreateEmptyNetworkOwnerAndMembers(db);
    

        var networkKey = await TestUserDataCreator.AddNetworkKeyToNetwork(db, (Guid)owner.NetworkId!, 60);
        return new UserAlreadyInNetworkScenario(userInOtherNetwork, owner.NetworkId.ToString()!, networkKey);
    }
}