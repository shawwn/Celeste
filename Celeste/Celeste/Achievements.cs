﻿// Decompiled with JetBrains decompiler
// Type: Celeste.Achievements
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Steamworks;

namespace Celeste
{
  public static class Achievements
  {
    public static string ID(Achievement achievement)
    {
      return achievement.ToString();
    }

    public static bool Has(Achievement achievement)
    {
      bool pbAchieved;
      return SteamUserStats.GetAchievement(Achievements.ID(achievement), out pbAchieved) & pbAchieved;
    }

    public static void Register(Achievement achievement)
    {
      if (Achievements.Has(achievement))
        return;
      SteamUserStats.SetAchievement(Achievements.ID(achievement));
      Stats.Store();
    }
  }
}

