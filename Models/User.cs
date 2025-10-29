using System;
using System.Collections.Generic;

namespace JoyLeeWrite.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? FullName { get; set; }

    public string? Email { get; set; }

    public string? AvatarUrl { get; set; }

    public string? Bio { get; set; }

    public string Role { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? LastLogin { get; set; }

    public virtual ICollection<Series> Series { get; set; } = new List<Series>();

    public virtual ICollection<UserSetting> UserSettings { get; set; } = new List<UserSetting>();
}
