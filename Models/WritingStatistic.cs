using System;
using System.Collections.Generic;

namespace JoyLeeWrite.Models;

public partial class WritingStatistic
{
    public long StatId { get; set; }

    public int UserId { get; set; }

    public DateOnly RecordDate { get; set; }

    public int WordsWritten { get; set; }

    public int ChaptersCreated { get; set; }

    public int ChaptersModified { get; set; }

    public int WritingTimeMinutes { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User Author { get; set; } = null!;
}
