// Decompiled with JetBrains decompiler
// Type: Celeste.Stats
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

#if STEAM
using Steamworks;
#endif
using System.Collections.Generic;

namespace Celeste
{
  public static class Stats
  {
    private static Dictionary<Stat, string> statToString = new Dictionary<Stat, string>();
    private static bool ready = false;

    public static void MakeRequest()
    {
#if STEAM
      Stats.ready = SteamUserStats.RequestCurrentStats();
      SteamUserStats.RequestGlobalStats(0);
#endif
    }

    public static bool Has()
    {
      return Stats.ready;
    }

    public static void Increment(Stat stat, int increment = 1)
    {
      if (!Stats.ready)
        return;
      string pchName = (string) null;
      if (!Stats.statToString.TryGetValue(stat, out pchName))
        Stats.statToString.Add(stat, pchName = stat.ToString());
#if STEAM
      int pData;
      if (SteamUserStats.GetStat(pchName, out pData))
        SteamUserStats.SetStat(pchName, pData + increment);
#endif
    }

    public static int Local(Stat stat)
    {
      int pData = 0;
      if (Stats.ready)
      {
        string pchName = (string) null;
        if (!Stats.statToString.TryGetValue(stat, out pchName))
          Stats.statToString.Add(stat, pchName = stat.ToString());
#if STEAM
        SteamUserStats.GetStat(pchName, out pData);
#endif
      }
      return pData;
    }

    public static long Global(Stat stat)
    {
      long pData = 0;
      if (Stats.ready)
      {
        string pchStatName = (string) null;
        if (!Stats.statToString.TryGetValue(stat, out pchStatName))
          Stats.statToString.Add(stat, pchStatName = stat.ToString());
#if STEAM
        SteamUserStats.GetGlobalStat(pchStatName, out pData);
#endif
      }
      return pData;
    }

    public static void Store()
    {
      if (!Stats.ready)
        return;
#if STEAM
      SteamUserStats.StoreStats();
#endif
    }

    public static string Name(Stat stat)
    {
      switch (stat)
      {
        case Stat.DEATHS:
          return "Deaths";
        case Stat.DASHES:
          return "Dashes";
        case Stat.BERRIES:
          return "Strawberries";
        case Stat.GOLDBERRIES:
          return "Golden Berries";
        case Stat.PICO_DEATHS:
          return "PICO-8 Deaths";
        case Stat.PICO_COMPLETES:
          return "PICO-8 Completes";
        case Stat.PICO_BERRIES:
          return "PICO-8 Strawberries";
        default:
          return "";
      }
    }
  }
}

