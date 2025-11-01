using System;
using System.Collections.Generic;

namespace JoyLeeWrite.Models;

public partial class UserSetting
{
    public int UserId { get; set; }

    public string SettingKey { get; set; } = null!;

    public string? SettingValue { get; set; }

    public virtual User User { get; set; } = null!;
}
