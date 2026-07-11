internal class TestSongRepository
{
    public static async Task<List<Song>> AddRandomSongsToNetwork(
            Guid networkId,
            int amount = 1
        )
    {
        var songs = new List<Song>();
        for (int i = 0; i < amount; i++)
        {
            songs.Add(new Song
            {
                Id = Guid.NewGuid(),
                NetworkId = networkId,
                Name = $"TestSong {i}",
                DurationSeconds = 180
            });
        }

        return songs;
    }
    public static async Task<List<SongRequest>> AddSongRequestsToUser(
            Guid userId,
            Guid networkId,
            List<Song> songs
        )
    {
        var requests = songs.Select(song => new SongRequest
        {
            Id = Guid.NewGuid(),
            SongId = song.Id,
            NetworkId = networkId,
            RequestedById = userId
        }).ToList();

        return requests;
    }

}