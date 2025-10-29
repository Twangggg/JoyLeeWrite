using System;
using System.Collections.Generic;

namespace JoyLeeWrite.Models;

public partial class Chapter
{
    public int ChapterId { get; set; }

    public int SeriesId { get; set; }

    public int ChapterNumber { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }

    public int WordCount { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public DateTime LastModified { get; set; }

    public virtual ICollection<Aiinteraction> Aiinteractions { get; set; } = new List<Aiinteraction>();

    public virtual ICollection<AutoSafe> AutoSaves { get; set; } = new List<AutoSafe>();

    public virtual Series Series { get; set; } = null!;
}
