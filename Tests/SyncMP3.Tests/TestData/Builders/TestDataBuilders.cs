public static class TestDataBuilders
{
    public static DomainUser CreateDomainUser(Guid? networkId = null, bool premium = false)
    {
        return new DomainUser
        {
            Id = Guid.NewGuid(),
            NetworkId = networkId,
            Premium = premium
        };
    }

    public static Network CreateNetwork(Guid ownerId)
    {
        return new Network
        {
            Id = Guid.NewGuid(),
            OwnerId = ownerId
        };
    }

    public static Song CreateSong(Guid networkId, string? name = null, int durationSeconds = 180)
    {
        return new Song
        {
            Id = Guid.NewGuid(),
            NetworkId = networkId,
            Name = name ?? "TestSong",
            DurationSeconds = durationSeconds
        };
    }

    public static SongRequest CreateSongRequest(Guid songId, Guid networkId, Guid requestedById)
    {
        return new SongRequest
        {
            Id = Guid.NewGuid(),
            SongId = songId,
            NetworkId = networkId,
            RequestedById = requestedById
        };
    }

    public static DownloadedSong CreateDownloadedSong(
        Song song, Guid networkId, Guid uploadedBy)
    {
        var path = Path.Combine("C:", "Repos", "PROJECTS", "SyncMP3", "Music", networkId.ToString());

        var finalPath = Path.Combine(path, song.Name!);

        return new DownloadedSong
        {
            SongId = song.Id,
            NetworkId = networkId,
            UploadedBy = uploadedBy,
            FilePath = finalPath
        };
    }
    public static NetworkKey CreateNetworkKey(
        Guid networkId, int minuTilExpiration = 60
    )
    {
        return new NetworkKey
        {
            Id = Guid.NewGuid(),
            Code = NetworkKeyGenerator.GenerateCode(),
            NetworkId = networkId,
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(minuTilExpiration)
        };
    }
}