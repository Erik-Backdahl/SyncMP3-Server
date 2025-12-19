using System;
using System.Collections.Generic;

namespace SyncMP3.Models;

public partial class Message
{
    public int Id { get; set; }

    public string UserUuid { get; set; } = null!;

    public string MessageType { get; set; } = null!;

    public string Message1 { get; set; } = null!;

    public DateTime? Created { get; set; }

    public virtual User UserUu { get; set; } = null!;
}
