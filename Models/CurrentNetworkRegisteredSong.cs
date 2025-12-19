using System;
using System.Collections.Generic;

namespace SyncMP3.Models;

public partial class CurrentNetworkRegisteredSong
{
    public int Id { get; set; }

    public string OriginUuid { get; set; } = null!;

    public string SongGuid { get; set; } = null!;

    public string SongName { get; set; } = null!;

    public DateTime? Created { get; set; }

    public virtual User OriginUu { get; set; } = null!;
}
