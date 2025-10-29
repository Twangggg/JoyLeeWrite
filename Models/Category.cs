using System;
using System.Collections.Generic;

namespace JoyLeeWrite.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public virtual ICollection<Series> Series { get; set; } = new List<Series>();
}
