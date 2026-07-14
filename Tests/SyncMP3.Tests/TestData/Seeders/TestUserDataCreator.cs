using System.Collections.Specialized;
using System.ComponentModel;

public static class TestUserDataCreator
{
    public static async Task<(DomainUser owner, List<DomainUser> otherMembers)> CreateEmptyNetworkOwnerAndMembers(
    SyncMp3DbContext db, int memberAmount = 1
    )
    {
        var masterUser = new DomainUser { Id = Guid.NewGuid() };
        await db.DomainUsers.AddAsync(masterUser);
        await db.SaveChangesAsync();

        var network = new Network
        {
            Id = Guid.NewGuid(),
            OwnerId = masterUser.Id
        };
        await db.Networks.AddAsync(network);
        await db.SaveChangesAsync();

        masterUser.NetworkId = network.Id;

        var allUsers = Enumerable.Range(0, memberAmount)
            .Select(_ => new DomainUser { Id = Guid.NewGuid(), NetworkId = network.Id })
            .ToList();

        await db.DomainUsers.AddRangeAsync(allUsers);
        await db.SaveChangesAsync(); 

        return (masterUser, allUsers);
    }
    public static async Task<SongRequest> CreateSongRequestForSong(
        SyncMp3DbContext db,
        Guid networkId,
        DomainUser requestee
        )
    {
        var song = TestDataBuilders.CreateSong(networkId, durationSeconds: 120);

        await db.Songs.AddAsync(song);

        var songRequest = TestDataBuilders.CreateSongRequest(song.Id, networkId, requestee.Id);

        await db.SongRequests.AddAsync(songRequest);

        await db.SaveChangesAsync();

        return songRequest;
    }
    internal static async Task AddSongsToUser(
        SyncMp3DbContext db,
        DomainUser user,
        List<Song> song)
    {
        user.LocalSongs.AddRange(song);

        await db.SaveChangesAsync();
    }

    internal static async Task AddDownloadedSongToDb(
        SyncMp3DbContext db,
        Guid? networkId,
        Song song,
        Guid userId)
    {
        var downloadedSong = TestDataBuilders.CreateDownloadedSong(song, (Guid)networkId!, userId);

        await db.DownloadedSongs.AddAsync(downloadedSong);

        await db.SaveChangesAsync();
    }
    internal static async Task<DownloadedSong> CreateDownloadedSongAndRequest(
        SyncMp3DbContext db,
        DomainUser user,
        DomainUser uploadedByUser
    )
    {
        var song = TestDataBuilders.CreateSong((Guid)user.NetworkId!);

        var downloadedSong = TestDataBuilders.CreateDownloadedSong(song, (Guid)user.NetworkId, uploadedByUser.Id);

        await db.Songs.AddAsync(song);
        await db.SaveChangesAsync();

        await db.DownloadedSongs.AddAsync(downloadedSong);
        await db.SaveChangesAsync();

        return downloadedSong;
    }
    internal static async Task<DomainUser> CreateEmptyUser(SyncMp3DbContext db)
    {
        var user = TestDataBuilders.CreateDomainUser();

        await db.DomainUsers.AddAsync(user);
        await db.SaveChangesAsync();

        return user;
    }
}