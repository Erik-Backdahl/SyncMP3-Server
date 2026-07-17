internal class RemoveUserScenarios
{
    public record RegularRemovalScenario(
        DomainUser OwnerUser,
        DomainUser RemoveUser
    );
    public static async Task<RegularRemovalScenario> RegularRemoval(SyncMp3DbContext db)
    {
        var (owner, members) = await TestUserDataCreator.CreateEmptyNetworkOwnerAndMembers(db);

        return new RegularRemovalScenario(owner, members[0]);
    }
    public record NotOwnerOfNetworkScenario(
        DomainUser OwnerUser,
        DomainUser RegularUser
    );
    public static async Task<NotOwnerOfNetworkScenario> NotOwnerOfNetwork(SyncMp3DbContext db)
    {
        var (owner, members) = await TestUserDataCreator.CreateEmptyNetworkOwnerAndMembers(db);

        return new NotOwnerOfNetworkScenario(owner, members[0]);
    }
    public record UserToBeRemovedIsNotInNetworkScenario(
        DomainUser OwnerUser,
        DomainUser UserApartOfOtherNetwork
    );
    public static async Task<UserToBeRemovedIsNotInNetworkScenario> UserToBeRemovedIsNotInNetwork(SyncMp3DbContext db)
    {
        var (owner, members) = await TestUserDataCreator.CreateEmptyNetworkOwnerAndMembers(db);
        var (userApartOfOtherNetwork, membersNotUsed) = await TestUserDataCreator.CreateEmptyNetworkOwnerAndMembers(db);

        return new UserToBeRemovedIsNotInNetworkScenario(owner, userApartOfOtherNetwork);
    }
}