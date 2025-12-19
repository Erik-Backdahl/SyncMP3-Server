using System;
using System.Collections.Generic;

namespace SyncMP3.Models;

public partial class Network
{
    public int Id { get; set; }

    public string NetworkGuid { get; set; } = null!;

    public string MasterUser { get; set; } = null!;

    public int TotalUsers { get; set; }

    public int? MaxUsers { get; set; }

    public DateTime? Created { get; set; }

    public virtual NetworkPassword? NetworkPassword { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
