public static class UploadScenarios
{
    public record RegularUploadScenario(
        DomainUser RequestingSong,
        DomainUser HasSong,
        Network Network,
        Song UploadSong
    );
    public static async Task<RegularUploadScenario> RegularUpload(SyncMp3DbContext db)
    {
        var (owner, members) = await TestUserDataCreator.CreateEmptyNetworkOwnerAndMembers(db);

        var songRequest = await TestUserDataCreator.CreateSongRequestForSong(db, (Guid)owner.NetworkId!, owner);

        await TestUserDataCreator.AddSongsToUser(db, members[0], [songRequest.SongNavigation]);

        return new RegularUploadScenario(owner, members[0], owner.NetworkNavigation!, songRequest.SongNavigation);
    }
    public record DuplicateSongUploadScenario( // this can happen incase two user try to upload at the same time
        DomainUser RequestingSong,
        DomainUser HasSong,
        Network Network,
        Song UploadSong
    );
    public static async Task<DuplicateSongUploadScenario> DuplicateSongUpload(SyncMp3DbContext db)
    {
        var (owner, members) = await TestUserDataCreator.CreateEmptyNetworkOwnerAndMembers(db);

        var songRequest = await TestUserDataCreator.CreateSongRequestForSong(db, (Guid)owner.NetworkId!, owner);

        await TestUserDataCreator.AddSongsToUser(db, members[0], [songRequest.SongNavigation]);

        await TestUserDataCreator.AddDownloadedSongToDb(db, owner.NetworkId, songRequest.SongNavigation, owner.Id);

        return new DuplicateSongUploadScenario(owner, members[0], owner.NetworkNavigation!, songRequest.SongNavigation);
    }
}