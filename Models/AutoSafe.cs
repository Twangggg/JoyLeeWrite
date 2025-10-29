using System;
using System.Collections.Generic;

namespace JoyLeeWrite.Models;

public partial class AutoSafe
{
    public int AutoSaveId { get; set; }

    public int ChapterId { get; set; }

    public string? Content { get; set; }

    public DateTime SavedAt { get; set; }

    public virtual Chapter Chapter { get; set; } = null!;
}
