using System;
using System.Collections.Generic;

namespace JoyLeeWrite.Models;

public partial class Series
{
    public int SeriesId { get; set; }

    public int AuthorId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? CoverImgUrl { get; set; }

    public string? Tags { get; set; }

    public int WordCount { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }

    public DateTime LastModified { get; set; }

    public virtual User Author { get; set; } = null!;

    public virtual ICollection<Chapter> Chapters { get; set; } = new List<Chapter>();

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
}
