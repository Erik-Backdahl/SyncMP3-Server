internal static class TransferTitleScenarios
{
    public record RegularTransferScenario(
        DomainUser Owner,
        DomainUser NewOwner
    );
    public static async Task<RegularTransferScenario> RegularTransfer(SyncMp3DbContext db)
    {
        var (owner, members) = await TestUserDataCreator.CreateEmptyNetworkOwnerAndMembers(db);

        return new RegularTransferScenario(owner, members[0]);
    }
    public record NotOwnerScenario(
        DomainUser RequestingUser,
        DomainUser NotOwnerUser
    );
    public static async Task<NotOwnerScenario> NotOwner(SyncMp3DbContext db)
    {
        var (owner, members) = await TestUserDataCreator.CreateEmptyNetworkOwnerAndMembers(db, 3);

        return new NotOwnerScenario(owner, members[0]);
    }
}