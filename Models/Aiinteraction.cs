using System;
using System.Collections.Generic;

namespace JoyLeeWrite.Models;

public partial class Aiinteraction
{
    public long InteractionId { get; set; }

    public int? ChapterId { get; set; }

    public string? Prompt { get; set; }

    public string? Response { get; set; }

    public string? ModelUsed { get; set; }

    public string? Role { get; set; }

    public DateTime Timestamp { get; set; }

    public virtual Chapter? Chapter { get; set; }
}
