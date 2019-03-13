// Decompiled with JetBrains decompiler
// Type: Celeste.SaveData
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Celeste
{
  [Serializable]
  public class SaveData
  {
    public string Name = "Madeline";
    public Assists Assists = Assists.Default;
    public HashSet<string> Flags = new HashSet<string>();
    public List<string> Poem = new List<string>();
    public List<AreaStats> Areas = new List<AreaStats>();
    public const int MaxStrawberries = 175;
    public const int MaxGoldenStrawberries = 25;
    public const int MaxHeartGems = 24;
    public const int MaxCassettes = 8;
    public const int MaxCompletions = 8;
    public static SaveData Instance;
    public string Version;
    public long Time;
    public DateTime LastSave;
    public bool CheatMode;
    public bool AssistMode;
    public bool VariantMode;
    public string TheoSisterName;
    public int UnlockedAreas;
    public int TotalDeaths;
    public int TotalStrawberries;
    public int TotalGoldenStrawberries;
    public int TotalJumps;
    public int TotalWallJumps;
    public int TotalDashes;
    public bool[] SummitGems;
    public AreaKey LastArea;
    public Session CurrentSession;
    [XmlIgnore]
    [NonSerialized]
    public int FileSlot;
    [XmlIgnore]
    [NonSerialized]
    public bool DoNotSave;
    [XmlIgnore]
    [NonSerialized]
    public bool DebugMode;

    public static void Start(SaveData data, int slot)
    {
      SaveData.Instance = data;
      SaveData.Instance.FileSlot = slot;
      SaveData.Instance.AfterInitialize();
    }

    public static string GetFilename(int slot)
    {
      if (slot == 4)
        return "debug";
      return slot.ToString();
    }

    public static string GetFilename()
    {
      return SaveData.GetFilename(SaveData.Instance.FileSlot);
    }

    public static void InitializeDebugMode(bool loadExisting = true)
    {
      SaveData data = (SaveData) null;
      if (loadExisting && UserIO.Open(UserIO.Mode.Read))
      {
        data = UserIO.Load<SaveData>(SaveData.GetFilename(4), false);
        UserIO.Close();
      }
      if (data == null)
        data = new SaveData();
      data.DebugMode = true;
      data.CurrentSession = (Session) null;
      SaveData.Start(data, 4);
    }

    public static bool TryDelete(int slot)
    {
      return UserIO.Delete(SaveData.GetFilename(slot));
    }

    public void AfterInitialize()
    {
      while (this.Areas.Count < AreaData.Areas.Count)
        this.Areas.Add(new AreaStats(this.Areas.Count));
      while (this.Areas.Count > AreaData.Areas.Count)
        this.Areas.RemoveAt(this.Areas.Count - 1);
      int num = -1;
      for (int index = 0; index < this.Areas.Count; ++index)
      {
        if (this.Areas[index].Modes[0].Completed)
          num = index;
      }
      if (this.UnlockedAreas < num + 1 && this.MaxArea >= num + 1)
        this.UnlockedAreas = num + 1;
      if (this.DebugMode)
      {
        this.CurrentSession = (Session) null;
        this.UnlockedAreas = this.MaxArea;
      }
      if (string.IsNullOrEmpty(this.TheoSisterName))
      {
        this.TheoSisterName = Dialog.Clean("THEO_SISTER_NAME", (Language) null);
        if (this.Name.IndexOf(this.TheoSisterName, StringComparison.InvariantCultureIgnoreCase) >= 0)
          this.TheoSisterName = Dialog.Clean("THEO_SISTER_ALT_NAME", (Language) null);
      }
      this.AssistModeChecks();
      foreach (AreaStats area in this.Areas)
        area.CleanCheckpoints();
      if (this.Version == null || !(new System.Version(this.Version) < new System.Version(1, 2, 1, 1)))
        return;
      for (int index1 = 0; index1 < this.Areas.Count; ++index1)
      {
        if (this.Areas[index1] != null)
        {
          for (int index2 = 0; index2 < this.Areas[index1].Modes.Length; ++index2)
          {
            if (this.Areas[index1].Modes[index2] != null)
            {
              if (this.Areas[index1].Modes[index2].BestTime > 0L)
                this.Areas[index1].Modes[index2].SingleRunCompleted = true;
              this.Areas[index1].Modes[index2].BestTime = 0L;
              this.Areas[index1].Modes[index2].BestFullClearTime = 0L;
            }
          }
        }
      }
    }

    public void AssistModeChecks()
    {
      if (!this.VariantMode && !this.AssistMode)
        this.Assists = new Assists();
      else if (!this.VariantMode)
        this.Assists.EnfornceAssistMode();
      if (this.Assists.GameSpeed < 5 || this.Assists.GameSpeed > 20)
        this.Assists.GameSpeed = 10;
      Input.MoveX.Inverted = Input.Aim.InvertedX = this.Assists.MirrorMode;
    }

    public static void NoFileAssistChecks()
    {
      Input.MoveX.Inverted = Input.Aim.InvertedX = false;
    }

    public void BeforeSave()
    {
      SaveData.Instance.Version = Celeste.Celeste.Instance.Version.ToString();
    }

    public void StartSession(Session session)
    {
      this.LastArea = session.Area;
      this.CurrentSession = session;
      if (!this.DebugMode)
        return;
      AreaModeStats mode1 = this.Areas[session.Area.ID].Modes[(int) session.Area.Mode];
      AreaModeStats mode2 = session.OldStats.Modes[(int) session.Area.Mode];
      SaveData.Instance.TotalStrawberries -= mode1.TotalStrawberries;
      mode1.Strawberries.Clear();
      mode1.TotalStrawberries = 0;
      mode2.Strawberries.Clear();
      mode2.TotalStrawberries = 0;
    }

    public void AddDeath(AreaKey area)
    {
      ++this.TotalDeaths;
      ++this.Areas[area.ID].Modes[(int) area.Mode].Deaths;
      Stats.Increment(Stat.DEATHS, 1);
    }

    public void AddStrawberry(AreaKey area, EntityID strawberry, bool golden)
    {
      AreaModeStats mode = this.Areas[area.ID].Modes[(int) area.Mode];
      if (!mode.Strawberries.Contains(strawberry))
      {
        mode.Strawberries.Add(strawberry);
        ++mode.TotalStrawberries;
        ++this.TotalStrawberries;
        if (golden)
          ++this.TotalGoldenStrawberries;
        if (this.TotalStrawberries >= 30)
          Achievements.Register(Achievement.STRB1);
        if (this.TotalStrawberries >= 80)
          Achievements.Register(Achievement.STRB2);
        if (this.TotalStrawberries >= 175)
          Achievements.Register(Achievement.STRB3);
      }
      Stats.Increment(golden ? Stat.GOLDBERRIES : Stat.BERRIES, 1);
    }

    public void AddStrawberry(EntityID strawberry, bool golden)
    {
      this.AddStrawberry(this.CurrentSession.Area, strawberry, golden);
    }

    public bool CheckStrawberry(AreaKey area, EntityID strawberry)
    {
      return this.Areas[area.ID].Modes[(int) area.Mode].Strawberries.Contains(strawberry);
    }

    public bool CheckStrawberry(EntityID strawberry)
    {
      return this.CheckStrawberry(this.CurrentSession.Area, strawberry);
    }

    public void AddTime(AreaKey area, long time)
    {
      this.Time += time;
      this.Areas[area.ID].Modes[(int) area.Mode].TimePlayed += time;
    }

    public void RegisterHeartGem(AreaKey area)
    {
      this.Areas[area.ID].Modes[(int) area.Mode].HeartGem = true;
      if (area.Mode == AreaMode.Normal)
      {
        if (area.ID == 1)
          Achievements.Register(Achievement.HEART1);
        else if (area.ID == 2)
          Achievements.Register(Achievement.HEART2);
        else if (area.ID == 3)
          Achievements.Register(Achievement.HEART3);
        else if (area.ID == 4)
          Achievements.Register(Achievement.HEART4);
        else if (area.ID == 5)
          Achievements.Register(Achievement.HEART5);
        else if (area.ID == 6)
          Achievements.Register(Achievement.HEART6);
        else if (area.ID == 7)
        {
          Achievements.Register(Achievement.HEART7);
        }
        else
        {
          if (area.ID != 9)
            return;
          Achievements.Register(Achievement.HEART8);
        }
      }
      else
      {
        if (area.Mode != AreaMode.BSide)
          return;
        if (area.ID == 1)
          Achievements.Register(Achievement.BSIDE1);
        else if (area.ID == 2)
          Achievements.Register(Achievement.BSIDE2);
        else if (area.ID == 3)
          Achievements.Register(Achievement.BSIDE3);
        else if (area.ID == 4)
          Achievements.Register(Achievement.BSIDE4);
        else if (area.ID == 5)
          Achievements.Register(Achievement.BSIDE5);
        else if (area.ID == 6)
          Achievements.Register(Achievement.BSIDE6);
        else if (area.ID == 7)
        {
          Achievements.Register(Achievement.BSIDE7);
        }
        else
        {
          if (area.ID != 9)
            return;
          Achievements.Register(Achievement.BSIDE8);
        }
      }
    }

    public void RegisterCassette(AreaKey area)
    {
      this.Areas[area.ID].Cassette = true;
      Achievements.Register(Achievement.CASS);
    }

    public bool RegisterPoemEntry(string id)
    {
      id = id.ToLower();
      if (this.Poem.Contains(id))
        return false;
      this.Poem.Add(id);
      return true;
    }

    public void RegisterSummitGem(int id)
    {
      if (this.SummitGems == null)
        this.SummitGems = new bool[6];
      this.SummitGems[id] = true;
    }

    public void RegisterCompletion(Session session)
    {
      AreaKey area = session.Area;
      AreaModeStats mode = this.Areas[area.ID].Modes[(int) area.Mode];
      if (session.GrabbedGolden)
        mode.BestDeaths = 0;
      if (session.StartedFromBeginning)
      {
        mode.SingleRunCompleted = true;
        if (mode.BestTime <= 0L || session.Deaths < mode.BestDeaths)
          mode.BestDeaths = session.Deaths;
        if (mode.BestTime <= 0L || session.Dashes < mode.BestDashes)
          mode.BestDashes = session.Dashes;
        if (mode.BestTime <= 0L || session.Time < mode.BestTime)
        {
          if (mode.BestTime > 0L)
            session.BeatBestTime = true;
          mode.BestTime = session.Time;
        }
        if (area.Mode == AreaMode.Normal && session.FullClear)
        {
          mode.FullClear = true;
          if (session.StartedFromBeginning && (mode.BestFullClearTime <= 0L || session.Time < mode.BestFullClearTime))
            mode.BestFullClearTime = session.Time;
        }
      }
      if (area.ID + 1 > this.UnlockedAreas && area.ID < this.MaxArea)
        this.UnlockedAreas = area.ID + 1;
      mode.Completed = true;
      session.InArea = false;
    }

    public bool SetCheckpoint(AreaKey area, string level)
    {
      AreaModeStats mode = this.Areas[area.ID].Modes[(int) area.Mode];
      if (mode.Checkpoints.Contains(level))
        return false;
      mode.Checkpoints.Add(level);
      return true;
    }

    public bool FoundAnyCheckpoints(AreaKey area)
    {
      if (Celeste.Celeste.PlayMode == Celeste.Celeste.PlayModes.Event)
        return false;
      if (!this.DebugMode && !this.CheatMode)
        return this.Areas[area.ID].Modes[(int) area.Mode].Checkpoints.Count > 0;
      ModeProperties modeProperties = AreaData.Areas[area.ID].Mode[(int) area.Mode];
      if (modeProperties != null && modeProperties.Checkpoints != null)
        return (uint) modeProperties.Checkpoints.Length > 0U;
      return false;
    }

    public HashSet<string> GetCheckpoints(AreaKey area)
    {
      if (Celeste.Celeste.PlayMode == Celeste.Celeste.PlayModes.Event)
        return new HashSet<string>();
      if (!this.DebugMode && !this.CheatMode)
        return this.Areas[area.ID].Modes[(int) area.Mode].Checkpoints;
      HashSet<string> stringSet = new HashSet<string>();
      ModeProperties modeProperties = AreaData.Areas[area.ID].Mode[(int) area.Mode];
      if (modeProperties.Checkpoints != null)
      {
        foreach (CheckpointData checkpoint in modeProperties.Checkpoints)
          stringSet.Add(checkpoint.Level);
      }
      return stringSet;
    }

    public bool HasFlag(string flag)
    {
      return this.Flags.Contains(flag);
    }

    public void SetFlag(string flag)
    {
      if (this.HasFlag(flag))
        return;
      this.Flags.Add(flag);
    }

    public int UnlockedModes
    {
      get
      {
        if (this.DebugMode || this.CheatMode || this.TotalHeartGems >= 16)
          return 3;
        for (int index = 1; index <= this.MaxArea; ++index)
        {
          if (this.Areas[index].Cassette)
            return 2;
        }
        return 1;
      }
    }

    public int MaxArea
    {
      get
      {
        if (Celeste.Celeste.PlayMode == Celeste.Celeste.PlayModes.Event)
          return 2;
        return AreaData.Areas.Count - 1;
      }
    }

    public int TotalHeartGems
    {
      get
      {
        int num = 0;
        foreach (AreaStats area in this.Areas)
        {
          for (int index = 0; index < area.Modes.Length; ++index)
          {
            if (area.Modes[index] != null && area.Modes[index].HeartGem)
              ++num;
          }
        }
        return num;
      }
    }

    public int TotalCassettes
    {
      get
      {
        int num = 0;
        for (int id = 0; id <= this.MaxArea; ++id)
        {
          if (!AreaData.Get(id).Interlude && this.Areas[id].Cassette)
            ++num;
        }
        return num;
      }
    }

    public int TotalCompletions
    {
      get
      {
        int num = 0;
        for (int id = 0; id <= this.MaxArea; ++id)
        {
          if (!AreaData.Get(id).Interlude && this.Areas[id].Modes[0].Completed)
            ++num;
        }
        return num;
      }
    }

    public int CompletionPercent
    {
      get
      {
        float num1 = 0.0f;
        float num2 = this.TotalHeartGems < 24 ? num1 + (float) ((double) this.TotalHeartGems / 24.0 * 24.0) : num1 + 24f;
        float num3 = this.TotalStrawberries < 175 ? num2 + (float) ((double) this.TotalStrawberries / 175.0 * 55.0) : num2 + 55f;
        float num4 = this.TotalCassettes < 8 ? num3 + (float) ((double) this.TotalCassettes / 8.0 * 7.0) : num3 + 7f;
        float num5 = this.TotalCompletions < 8 ? num4 + (float) ((double) this.TotalCompletions / 8.0 * 14.0) : num4 + 14f;
        if ((double) num5 < 0.0)
          num5 = 0.0f;
        else if ((double) num5 > 100.0)
          num5 = 100f;
        return (int) num5;
      }
    }
  }
}
