using System.Collections.Specialized;
using System.ComponentModel;

public static class TestUserDataCreator
{
    public static async Task<(DomainUser owner, List<DomainUser> otherMembers)> CreateEmptyNetworkOwnerAndMembers(
    SyncMp3DbContext db, int memberAmount = 1
    )
    {
        // 1. Create the owner with no network yet (NetworkId nullable)
        var masterUser = new DomainUser { Id = Guid.NewGuid() };
        await db.DomainUsers.AddAsync(masterUser);
        await db.SaveChangesAsync();

        // 2. Create the network, now that masterUser.Id exists to satisfy OwnerId FK
        var network = new Network
        {
            Id = Guid.NewGuid(),
            OwnerId = masterUser.Id
        };
        await db.Networks.AddAsync(network);
        await db.SaveChangesAsync();

        // 3. Go back and attach the owner to the network
        masterUser.NetworkId = network.Id;

        // 4. Other members can be created directly with NetworkId set — no cycle here,
        //    since Network already exists and DomainUser -> Network is a one-way FK for them
        var allUsers = Enumerable.Range(0, memberAmount)
            .Select(_ => new DomainUser { Id = Guid.NewGuid(), NetworkId = network.Id })
            .ToList();

        await db.DomainUsers.AddRangeAsync(allUsers);
        await db.SaveChangesAsync(); // saves both masterUser's NetworkId update and the new members

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
}