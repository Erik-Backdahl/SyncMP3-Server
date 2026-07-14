using System.Data;

public static class DownloadScenario
{
    public record AvailibleSongScenario(
        DomainUser User,
        DownloadedSong SongToDownload);
    public static async Task<AvailibleSongScenario> AvailibleSong(SyncMp3DbContext db)
    {
        var (owner, members) = await TestUserDataCreator.CreateEmptyNetworkOwnerAndMembers(db);

        var downloadedSong = await TestUserDataCreator.CreateDownloadedSongAndRequest(db, owner, members[0]);

        return new AvailibleSongScenario(owner, downloadedSong);
    }
    public record UnavailibleSongScenario(
        DomainUser User);
    public static async Task<UnavailibleSongScenario> UnavailibleSong(SyncMp3DbContext db)
    {
        var (owner, members) = await TestUserDataCreator.CreateEmptyNetworkOwnerAndMembers(db);

        return new UnavailibleSongScenario(owner);
    }
}
