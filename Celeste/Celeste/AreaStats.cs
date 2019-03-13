// Decompiled with JetBrains decompiler
// Type: Celeste.AreaStats
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Celeste
{
  [Serializable]
  public class AreaStats
  {
    [XmlAttribute]
    public int ID;
    [XmlAttribute]
    public bool Cassette;
    public AreaModeStats[] Modes;

    public int TotalStrawberries
    {
      get
      {
        int num = 0;
        for (int index = 0; index < this.Modes.Length; ++index)
          num += this.Modes[index].TotalStrawberries;
        return num;
      }
    }

    public int TotalDeaths
    {
      get
      {
        int num = 0;
        for (int index = 0; index < this.Modes.Length; ++index)
          num += this.Modes[index].Deaths;
        return num;
      }
    }

    public long TotalTimePlayed
    {
      get
      {
        long num = 0;
        for (int index = 0; index < this.Modes.Length; ++index)
          num += this.Modes[index].TimePlayed;
        return num;
      }
    }

    public int BestTotalDeaths
    {
      get
      {
        int num = 0;
        for (int index = 0; index < this.Modes.Length; ++index)
          num += this.Modes[index].BestDeaths;
        return num;
      }
    }

    public int BestTotalDashes
    {
      get
      {
        int num = 0;
        for (int index = 0; index < this.Modes.Length; ++index)
          num += this.Modes[index].BestDashes;
        return num;
      }
    }

    public long BestTotalTime
    {
      get
      {
        long num = 0;
        for (int index = 0; index < this.Modes.Length; ++index)
          num += this.Modes[index].BestTime;
        return num;
      }
    }

    public AreaStats(int id)
    {
      this.ID = id;
      this.Modes = new AreaModeStats[Enum.GetValues(typeof (AreaMode)).Length];
      for (int index = 0; index < this.Modes.Length; ++index)
        this.Modes[index] = new AreaModeStats();
    }

    private AreaStats()
    {
      int length = Enum.GetValues(typeof (AreaMode)).Length;
      this.Modes = new AreaModeStats[length];
      for (int index = 0; index < length; ++index)
        this.Modes[index] = new AreaModeStats();
    }

    public AreaStats Clone()
    {
      AreaStats areaStats = new AreaStats()
      {
        ID = this.ID,
        Cassette = this.Cassette
      };
      for (int index = 0; index < this.Modes.Length; ++index)
        areaStats.Modes[index] = this.Modes[index].Clone();
      return areaStats;
    }

    public void CleanCheckpoints()
    {
      foreach (AreaMode areaMode in Enum.GetValues(typeof (AreaMode)))
      {
        if ((AreaMode) AreaData.Get(this.ID).Mode.Length > areaMode)
        {
          AreaModeStats mode = this.Modes[(int) areaMode];
          ModeProperties modeProperties = AreaData.Get(this.ID).Mode[(int) areaMode];
          HashSet<string> stringSet = new HashSet<string>((IEnumerable<string>) mode.Checkpoints);
          mode.Checkpoints.Clear();
          if (modeProperties != null && modeProperties.Checkpoints != null)
          {
            foreach (CheckpointData checkpoint in modeProperties.Checkpoints)
            {
              if (stringSet.Contains(checkpoint.Level))
                mode.Checkpoints.Add(checkpoint.Level);
            }
          }
        }
      }
    }
  }
}
