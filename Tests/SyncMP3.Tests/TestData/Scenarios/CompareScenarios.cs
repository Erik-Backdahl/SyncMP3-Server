using Microsoft.EntityFrameworkCore;

public static class CompareScenarios
{
    public record NetworkWithDownloadedSongsScenario(
        DomainUser Owner,
        List<DomainUser> Members,
        Network Network,
        List<DownloadedSong> DownloadedSongs);

    public static async Task<NetworkWithDownloadedSongsScenario> NetworkWithUndownloadedSongs(
        SyncMp3DbContext db, int memberCount = 2, int songCount = 3)
    {
        var (owner, members) = await TestUserDataCreator.CreateEmptyNetworkOwnerAndMembers(db, memberCount);

        var network = await db.Networks.SingleAsync(n => n.Id == owner.NetworkId);

        // uploaded by a member, not the owner — so they're "new" and downloadable for the owner
        var downloadedSongs = await AppliedTestSongDataCreator.CreateRandomDownloadedSongs(
            db, network.Id, members[0].Id, songCount);

        return new NetworkWithDownloadedSongsScenario(owner, members, network, downloadedSongs);
    }
    public record NetWorkWithUploadRequestsScenario(
        DomainUser UserThatCanUpload,
        DomainUser UserThatNeedsSongs,
        Network Network
        );
    public static async Task<NetWorkWithUploadRequestsScenario> NetWorkWithUploadRequests(
        SyncMp3DbContext db, int memberCount = 2)
    {
        var (owner, members) = await TestUserDataCreator.CreateEmptyNetworkOwnerAndMembers(db, memberCount);

        var network = await db.Networks.SingleAsync(n => n.Id == owner.NetworkId);

        await AppliedTestSongDataCreator.CreateNetworkWithUploadRequests(db, network, userHasSong: owner, userNeedsSong: members[0]);

        return new NetWorkWithUploadRequestsScenario(owner, members[0], network);
    }
}