using System;

namespace LemonBot.Models;

public class SaveData
{
    public static SaveData? Instance = null;

    public long NextSquawk { get; set; } = 0;
}
