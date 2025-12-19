using System;
using System.Collections.Generic;

namespace SyncMP3.Models;

public partial class NetworkPassword
{
    public int Id { get; set; }

    public string CreatedBy { get; set; } = null!;

    public string NetworkGuid { get; set; } = null!;

    public string Password { get; set; } = null!;

    public DateTime? Created { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual Network Network { get; set; } = null!;
}
