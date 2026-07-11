using Microsoft.AspNetCore.Http;

public static class AppliedTestSongDataCreator
{
    public static async Task<List<DownloadedSong>> CreateRandomDownloadedSongs(
            SyncMp3DbContext db,
            Guid networkId,
            Guid uploadedBy,
            int ammount = 1
        )
    {
        var songs = new List<Song>();
        for (int i = 0; i < ammount; i++)
        {
            songs.Add(TestDataBuilders.CreateSong(networkId));
        }

        var downloaded = songs.Select(song => new DownloadedSong
        {
            SongId = song.Id,
            NetworkId = networkId,
            UploadedBy = uploadedBy,
            FilePath = $"/songs/{Guid.NewGuid()}.mp3"
        }).ToList();

        db.Songs.AddRange(songs);
        db.DownloadedSongs.AddRange(downloaded);
        await db.SaveChangesAsync();

        return downloaded;
    }

    internal static async Task<Network> CreateNetworkWithUploadRequests(
    SyncMp3DbContext db,
    Network network,
    DomainUser userHasSong,
    DomainUser userNeedsSong)
    {
        var networkSongs = await TestSongRepository.AddRandomSongsToNetwork(network.Id, amount: 3);

        db.Songs.AddRange(networkSongs);           // explicitly tell EF these are new
        network.NetworkSongs.AddRange(networkSongs);
        userHasSong.LocalSongs.AddRange(networkSongs); // or LocalSongs — check actual property name

        var songRequests = await TestSongRepository.AddSongRequestsToUser(userNeedsSong.Id, network.Id, networkSongs);

        db.SongRequests.AddRange(songRequests);    // same here
        userNeedsSong.SongRequests.AddRange(songRequests);

        await db.SaveChangesAsync();

        return network;
    }
}