using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace JoyLeeWrite.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public virtual ICollection<Series> Series { get; set; } = new List<Series>();
    [NotMapped]
    public bool IsSelected { get; set; }
    public override string ToString()
    {
        return CategoryName;
    }
}
