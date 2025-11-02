using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace JoyLeeWrite.Models;

public partial class Chapter : INotifyPropertyChanged
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
    [NotMapped]
    private string _colorStatus = string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotMapped]
    public string ColorStatus
    {
        get
        {
            return Status == "Published" ? "#4CAF50" : "Gray";
        }
        set
        {
            _colorStatus = Status == "Published" ? "#4CAF50" : "Gray";
            OnPropertyChanged(nameof(ColorStatus));
            OnPropertyChanged(nameof(Status));
        }
    }
    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
