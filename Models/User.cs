using System;
using System.Collections.Generic;

namespace SyncMP3.Models;

public partial class User
{
    public int Id { get; set; }

    public string UserUuid { get; set; } = null!;

    public string? NetworkGuid { get; set; }

    public bool? Permium { get; set; }

    public DateTime? Created { get; set; }

    public string? PublicKey { get; set; }

    public virtual ICollection<CurrentNetworkRegisteredSong> CurrentNetworkRegisteredSongs { get; set; } = new List<CurrentNetworkRegisteredSong>();

    public virtual ICollection<CurrentUserRegisteredSong> CurrentUserRegisteredSongs { get; set; } = new List<CurrentUserRegisteredSong>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual Network? Network { get; set; }

    public virtual NetworkPassword? NetworkPassword { get; set; }
}
