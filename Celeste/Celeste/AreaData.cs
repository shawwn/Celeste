// Decompiled with JetBrains decompiler
// Type: Celeste.AreaData
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Celeste
{
  public class AreaData
  {
    public int CassetteCheckpointIndex = -1;
    public Color TitleBaseColor = Color.White;
    public Color TitleAccentColor = Color.Gray;
    public Color TitleTextColor = Color.White;
    public float DarknessAlpha = 0.05f;
    public float BloomBase = 0.0f;
    public float BloomStrength = 1f;
    public string Jumpthru = "wood";
    public string Spike = "default";
    public string CrumbleBlock = "default";
    public string WoodPlatform = "default";
    public Color CassseteNoteColor = Color.White;
    public Color CobwebColor = Calc.HexToColor("696a6a");
    public string CassetteSong = "event:/music/cassette/01_forsaken_city";
    public Session.CoreModes CoreMode = Session.CoreModes.None;
    public int MountainState = 0;
    public static List<AreaData> Areas;
    public string Name;
    public string Icon;
    public int ID;
    public bool Interlude;
    public string CompleteScreenName;
    public ModeProperties[] Mode;
    public Player.IntroTypes IntroType;
    public bool Dreaming;
    public string ColorGrade;
    public Action<Scene, bool, Action> Wipe;
    public Action<Level> OnLevelBegin;
    public MountainCamera MountainIdle;
    public MountainCamera MountainSelect;
    public MountainCamera MountainZoom;
    public Vector3 MountainCursor;
    public float MountainCursorScale;

    public static ModeProperties GetMode(AreaKey area)
    {
      return AreaData.GetMode(area.ID, area.Mode);
    }

    public static ModeProperties GetMode(int id, AreaMode mode = AreaMode.Normal)
    {
      return AreaData.Areas[id].Mode[(int) mode];
    }

    public static void Load()
    {
      AreaData.Areas = new List<AreaData>();
      List<AreaData> areas1 = AreaData.Areas;
      AreaData areaData1 = new AreaData();
      areaData1.Name = "area_0";
      areaData1.Icon = "areas/intro";
      areaData1.Interlude = true;
      areaData1.CompleteScreenName = (string) null;
      areaData1.Mode = new ModeProperties[3]
      {
        new ModeProperties()
        {
          PoemID = (string) null,
          Path = "0-Intro",
          Checkpoints = (CheckpointData[]) null,
          Inventory = PlayerInventory.Prologue,
          AudioState = new AudioState("event:/music/lvl0/intro", "event:/env/amb/00_prologue")
        },
        null,
        null
      };
      areaData1.TitleBaseColor = Calc.HexToColor("383838");
      areaData1.TitleAccentColor = Calc.HexToColor("50AFAE");
      areaData1.TitleTextColor = Color.White;
      areaData1.IntroType = Player.IntroTypes.WalkInRight;
      areaData1.Dreaming = false;
      areaData1.ColorGrade = (string) null;
      CurtainWipe curtainWipe1;
      areaData1.Wipe = (Action<Scene, bool, Action>) ((scene, wipeIn, onComplete) => curtainWipe1 = new CurtainWipe(scene, wipeIn, onComplete));
      areaData1.DarknessAlpha = 0.05f;
      areaData1.BloomBase = 0.0f;
      areaData1.BloomStrength = 1f;
      areaData1.OnLevelBegin = (Action<Level>) null;
      areaData1.Jumpthru = "wood";
      AreaData areaData2 = areaData1;
      areas1.Add(areaData2);
      List<AreaData> areas2 = AreaData.Areas;
      AreaData areaData3 = new AreaData();
      areaData3.Name = "area_1";
      areaData3.Icon = "areas/city";
      areaData3.Interlude = false;
      areaData3.CompleteScreenName = "ForsakenCity";
      areaData3.CassetteCheckpointIndex = 2;
      areaData3.Mode = new ModeProperties[3]
      {
        new ModeProperties()
        {
          PoemID = "fc",
          Path = "1-ForsakenCity",
          Checkpoints = new CheckpointData[2]
          {
            new CheckpointData("6", "checkpoint_1_0", new PlayerInventory?(), false, (AudioState) null),
            new CheckpointData("9b", "checkpoint_1_1", new PlayerInventory?(), false, (AudioState) null)
          },
          Inventory = PlayerInventory.Default,
          AudioState = new AudioState("event:/music/lvl1/main", "event:/env/amb/01_main")
        },
        new ModeProperties()
        {
          PoemID = "fcr",
          Path = "1H-ForsakenCity",
          Checkpoints = new CheckpointData[2]
          {
            new CheckpointData("04", "checkpoint_1h_0", new PlayerInventory?(), false, (AudioState) null),
            new CheckpointData("08", "checkpoint_1h_1", new PlayerInventory?(), false, (AudioState) null)
          },
          Inventory = PlayerInventory.Default,
          AudioState = new AudioState("event:/music/remix/01_forsaken_city", "event:/env/amb/01_main")
        },
        new ModeProperties()
        {
          Path = "1X-ForsakenCity",
          Checkpoints = (CheckpointData[]) null,
          Inventory = PlayerInventory.Default,
          AudioState = new AudioState("event:/music/remix/01_forsaken_city", "event:/env/amb/01_main")
        }
      };
      areaData3.TitleBaseColor = Calc.HexToColor("6c7c81");
      areaData3.TitleAccentColor = Calc.HexToColor("2f344b");
      areaData3.TitleTextColor = Color.White;
      areaData3.IntroType = Player.IntroTypes.Jump;
      areaData3.Dreaming = false;
      areaData3.ColorGrade = (string) null;
      AngledWipe angledWipe;
      areaData3.Wipe = (Action<Scene, bool, Action>) ((scene, wipeIn, onComplete) => angledWipe = new AngledWipe(scene, wipeIn, onComplete));
      areaData3.DarknessAlpha = 0.05f;
      areaData3.BloomBase = 0.0f;
      areaData3.BloomStrength = 1f;
      areaData3.OnLevelBegin = (Action<Level>) null;
      areaData3.Jumpthru = "wood";
      areaData3.CassseteNoteColor = Calc.HexToColor("33a9ee");
      areaData3.CassetteSong = "event:/music/cassette/01_forsaken_city";
      AreaData areaData4 = areaData3;
      areas2.Add(areaData4);
      List<AreaData> areas3 = AreaData.Areas;
      AreaData areaData5 = new AreaData();
      areaData5.Name = "area_2";
      areaData5.Icon = "areas/oldsite";
      areaData5.Interlude = false;
      areaData5.CompleteScreenName = "OldSite";
      areaData5.CassetteCheckpointIndex = 0;
      areaData5.Mode = new ModeProperties[3]
      {
        new ModeProperties()
        {
          PoemID = "os",
          Path = "2-OldSite",
          Checkpoints = new CheckpointData[2]
          {
            new CheckpointData("3", "checkpoint_2_0", new PlayerInventory?(PlayerInventory.Default), true, (AudioState) null),
            new CheckpointData("end_3", "checkpoint_2_1", new PlayerInventory?(), false, (AudioState) null)
          },
          Inventory = PlayerInventory.OldSite,
          AudioState = new AudioState("event:/music/lvl2/beginning", "event:/env/amb/02_dream")
        },
        new ModeProperties()
        {
          PoemID = "osr",
          Path = "2H-OldSite",
          Checkpoints = new CheckpointData[2]
          {
            new CheckpointData("03", "checkpoint_2h_0", new PlayerInventory?(), true, (AudioState) null),
            new CheckpointData("08b", "checkpoint_2h_1", new PlayerInventory?(), true, (AudioState) null)
          },
          Inventory = PlayerInventory.Default,
          AudioState = new AudioState("event:/music/remix/02_old_site", "event:/env/amb/02_dream")
        },
        new ModeProperties()
        {
          Path = "2X-OldSite",
          Checkpoints = (CheckpointData[]) null,
          Inventory = PlayerInventory.Default,
          AudioState = new AudioState("event:/music/remix/02_old_site", "event:/env/amb/02_dream")
        }
      };
      areaData5.TitleBaseColor = Calc.HexToColor("247F35");
      areaData5.TitleAccentColor = Calc.HexToColor("E4EF69");
      areaData5.TitleTextColor = Color.White;
      areaData5.IntroType = Player.IntroTypes.WakeUp;
      areaData5.Dreaming = true;
      areaData5.ColorGrade = "oldsite";
      DreamWipe dreamWipe;
      areaData5.Wipe = (Action<Scene, bool, Action>) ((scene, wipeIn, onComplete) => dreamWipe = new DreamWipe(scene, wipeIn, onComplete));
      areaData5.DarknessAlpha = 0.15f;
      areaData5.BloomBase = 0.5f;
      areaData5.BloomStrength = 1f;
      areaData5.OnLevelBegin = (Action<Level>) (level =>
      {
        if (level.Session.Area.Mode != AreaMode.Normal)
          return;
        level.Add((Entity) new OldSiteChaseMusicHandler());
      });
      areaData5.Jumpthru = "wood";
      areaData5.CassseteNoteColor = Calc.HexToColor("33eea2");
      areaData5.CassetteSong = "event:/music/cassette/02_old_site";
      AreaData areaData6 = areaData5;
      areas3.Add(areaData6);
      List<AreaData> areas4 = AreaData.Areas;
      AreaData areaData7 = new AreaData();
      areaData7.Name = "area_3";
      areaData7.Icon = "areas/resort";
      areaData7.Interlude = false;
      areaData7.CompleteScreenName = "CelestialResort";
      areaData7.CassetteCheckpointIndex = 2;
      AreaData areaData8 = areaData7;
      ModeProperties[] modePropertiesArray1 = new ModeProperties[3];
      ModeProperties modeProperties1 = new ModeProperties();
      modeProperties1.PoemID = "cr";
      modeProperties1.Path = "3-CelestialResort";
      modeProperties1.Checkpoints = new CheckpointData[3]
      {
        new CheckpointData("08-a", "checkpoint_3_0", new PlayerInventory?(), false, (AudioState) null)
        {
          AudioState = new AudioState(new AudioTrackState("event:/music/lvl3/explore").SetProgress(3), new AudioTrackState("event:/env/amb/03_interior"))
        },
        new CheckpointData("09-d", "checkpoint_3_1", new PlayerInventory?(), false, (AudioState) null)
        {
          AudioState = new AudioState(new AudioTrackState("event:/music/lvl3/clean").SetProgress(3), new AudioTrackState("event:/env/amb/03_interior"))
        },
        new CheckpointData("00-d", "checkpoint_3_2", new PlayerInventory?(), false, (AudioState) null)
      };
      modeProperties1.Inventory = PlayerInventory.Default;
      modeProperties1.AudioState = new AudioState("event:/music/lvl3/intro", "event:/env/amb/03_exterior");
      modePropertiesArray1[0] = modeProperties1;
      modePropertiesArray1[1] = new ModeProperties()
      {
        PoemID = "crr",
        Path = "3H-CelestialResort",
        Checkpoints = new CheckpointData[3]
        {
          new CheckpointData("06", "checkpoint_3h_0", new PlayerInventory?(), false, (AudioState) null),
          new CheckpointData("11", "checkpoint_3h_1", new PlayerInventory?(), false, (AudioState) null),
          new CheckpointData("16", "checkpoint_3h_2", new PlayerInventory?(), false, (AudioState) null)
        },
        Inventory = PlayerInventory.Default,
        AudioState = new AudioState("event:/music/remix/03_resort", "event:/env/amb/03_exterior")
      };
      modePropertiesArray1[2] = new ModeProperties()
      {
        Path = "3X-CelestialResort",
        Checkpoints = (CheckpointData[]) null,
        Inventory = PlayerInventory.Default,
        AudioState = new AudioState("event:/music/remix/03_resort", "event:/env/amb/03_exterior")
      };
      areaData8.Mode = modePropertiesArray1;
      areaData7.TitleBaseColor = Calc.HexToColor("b93c27");
      areaData7.TitleAccentColor = Calc.HexToColor("ffdd42");
      areaData7.TitleTextColor = Color.White;
      areaData7.IntroType = Player.IntroTypes.WalkInRight;
      areaData7.Dreaming = false;
      areaData7.ColorGrade = (string) null;
      KeyDoorWipe keyDoorWipe;
      areaData7.Wipe = (Action<Scene, bool, Action>) ((scene, wipeIn, onComplete) => keyDoorWipe = new KeyDoorWipe(scene, wipeIn, onComplete));
      areaData7.DarknessAlpha = 0.15f;
      areaData7.BloomBase = 0.0f;
      areaData7.BloomStrength = 1f;
      areaData7.OnLevelBegin = (Action<Level>) null;
      areaData7.Jumpthru = "wood";
      areaData7.CassseteNoteColor = Calc.HexToColor("eed933");
      areaData7.CassetteSong = "event:/music/cassette/03_resort";
      AreaData areaData9 = areaData7;
      areas4.Add(areaData9);
      List<AreaData> areas5 = AreaData.Areas;
      AreaData areaData10 = new AreaData();
      areaData10.Name = "area_4";
      areaData10.Icon = "areas/cliffside";
      areaData10.Interlude = false;
      areaData10.CompleteScreenName = "Cliffside";
      areaData10.CassetteCheckpointIndex = 0;
      areaData10.TitleBaseColor = Calc.HexToColor("FF7F83");
      areaData10.TitleAccentColor = Calc.HexToColor("6D54B7");
      areaData10.TitleTextColor = Color.White;
      areaData10.Mode = new ModeProperties[3]
      {
        new ModeProperties()
        {
          PoemID = "cs",
          Path = "4-GoldenRidge",
          Checkpoints = new CheckpointData[3]
          {
            new CheckpointData("b-00", "checkpoint_4_0", new PlayerInventory?(), false, (AudioState) null),
            new CheckpointData("c-00", "checkpoint_4_1", new PlayerInventory?(), false, (AudioState) null),
            new CheckpointData("d-00", "checkpoint_4_2", new PlayerInventory?(), false, (AudioState) null)
          },
          Inventory = PlayerInventory.Default,
          AudioState = new AudioState("event:/music/lvl4/main", "event:/env/amb/04_main")
        },
        new ModeProperties()
        {
          PoemID = "csr",
          Path = "4H-GoldenRidge",
          Checkpoints = new CheckpointData[3]
          {
            new CheckpointData("b-00", "checkpoint_4h_0", new PlayerInventory?(), false, (AudioState) null),
            new CheckpointData("c-00", "checkpoint_4h_1", new PlayerInventory?(), false, (AudioState) null),
            new CheckpointData("d-00", "checkpoint_4h_2", new PlayerInventory?(), false, (AudioState) null)
          },
          Inventory = PlayerInventory.Default,
          AudioState = new AudioState("event:/music/remix/04_cliffside", "event:/env/amb/04_main")
        },
        new ModeProperties()
        {
          Path = "4X-GoldenRidge",
          Checkpoints = (CheckpointData[]) null,
          Inventory = PlayerInventory.Default,
          AudioState = new AudioState("event:/music/remix/04_cliffside", "event:/env/amb/04_main")
        }
      };
      areaData10.IntroType = Player.IntroTypes.WalkInRight;
      areaData10.Dreaming = false;
      areaData10.ColorGrade = (string) null;
      WindWipe windWipe;
      areaData10.Wipe = (Action<Scene, bool, Action>) ((scene, wipeIn, onComplete) => windWipe = new WindWipe(scene, wipeIn, onComplete));
      areaData10.DarknessAlpha = 0.1f;
      areaData10.BloomBase = 0.25f;
      areaData10.BloomStrength = 1f;
      areaData10.OnLevelBegin = (Action<Level>) null;
      areaData10.Jumpthru = "cliffside";
      areaData10.Spike = "cliffside";
      areaData10.CrumbleBlock = "cliffside";
      areaData10.WoodPlatform = "cliffside";
      areaData10.CassseteNoteColor = Calc.HexToColor("eb4bd9");
      areaData10.CassetteSong = "event:/music/cassette/04_cliffside";
      AreaData areaData11 = areaData10;
      areas5.Add(areaData11);
      List<AreaData> areas6 = AreaData.Areas;
      AreaData areaData12 = new AreaData();
      areaData12.Name = "area_5";
      areaData12.Icon = "areas/temple";
      areaData12.Interlude = false;
      areaData12.CompleteScreenName = "MirrorTemple";
      areaData12.CassetteCheckpointIndex = 1;
      areaData12.Mode = new ModeProperties[3]
      {
        new ModeProperties()
        {
          PoemID = "t",
          Path = "5-MirrorTemple",
          Checkpoints = new CheckpointData[4]
          {
            new CheckpointData("b-00", "checkpoint_5_0", new PlayerInventory?(), false, (AudioState) null),
            new CheckpointData("c-00", "checkpoint_5_1", new PlayerInventory?(), true, new AudioState("event:/music/lvl5/middle_temple", "event:/env/amb/05_interior_dark")),
            new CheckpointData("d-00", "checkpoint_5_2", new PlayerInventory?(), true, new AudioState("event:/music/lvl5/middle_temple", "event:/env/amb/05_interior_dark")),
            new CheckpointData("e-00", "checkpoint_5_3", new PlayerInventory?(), true, new AudioState("event:/music/lvl5/mirror", "event:/env/amb/05_interior_dark"))
          },
          Inventory = PlayerInventory.Default,
          AudioState = new AudioState("event:/music/lvl5/normal", "event:/env/amb/05_interior_main")
        },
        new ModeProperties()
        {
          PoemID = "tr",
          Path = "5H-MirrorTemple",
          Checkpoints = new CheckpointData[3]
          {
            new CheckpointData("b-00", "checkpoint_5h_0", new PlayerInventory?(), false, (AudioState) null),
            new CheckpointData("c-00", "checkpoint_5h_1", new PlayerInventory?(), false, (AudioState) null),
            new CheckpointData("d-00", "checkpoint_5h_2", new PlayerInventory?(), false, (AudioState) null)
          },
          Inventory = PlayerInventory.Default,
          AudioState = new AudioState("event:/music/remix/05_mirror_temple", "event:/env/amb/05_interior_main")
        },
        new ModeProperties()
        {
          Path = "5X-MirrorTemple",
          Checkpoints = (CheckpointData[]) null,
          Inventory = PlayerInventory.Default,
          AudioState = new AudioState("event:/music/remix/05_mirror_temple", "event:/env/amb/05_interior_main")
        }
      };
      areaData12.TitleBaseColor = Calc.HexToColor("8314bc");
      areaData12.TitleAccentColor = Calc.HexToColor("df72f9");
      areaData12.TitleTextColor = Color.White;
      areaData12.IntroType = Player.IntroTypes.WakeUp;
      areaData12.Dreaming = false;
      areaData12.ColorGrade = (string) null;
      DropWipe dropWipe;
      areaData12.Wipe = (Action<Scene, bool, Action>) ((scene, wipeIn, onComplete) => dropWipe = new DropWipe(scene, wipeIn, onComplete));
      areaData12.DarknessAlpha = 0.15f;
      areaData12.BloomBase = 0.0f;
      areaData12.BloomStrength = 1f;
      areaData12.OnLevelBegin = (Action<Level>) (level =>
      {
        level.Add((Entity) new SeekerEffectsController());
        if (level.Session.Area.Mode != AreaMode.Normal)
          return;
        level.Add((Entity) new TempleEndingMusicHandler());
      });
      areaData12.Jumpthru = "temple";
      areaData12.CassseteNoteColor = Calc.HexToColor("5a56e6");
      areaData12.CobwebColor = Calc.HexToColor("9f2166");
      areaData12.CassetteSong = "event:/music/cassette/05_mirror_temple";
      AreaData areaData13 = areaData12;
      areas6.Add(areaData13);
      List<AreaData> areas7 = AreaData.Areas;
      AreaData areaData14 = new AreaData();
      areaData14.Name = "area_6";
      areaData14.Icon = "areas/reflection";
      areaData14.Interlude = false;
      areaData14.CompleteScreenName = "Fall";
      areaData14.CassetteCheckpointIndex = 2;
      AreaData areaData15 = areaData14;
      ModeProperties[] modePropertiesArray2 = new ModeProperties[3];
      ModeProperties modeProperties2 = new ModeProperties();
      modeProperties2.PoemID = "tf";
      modeProperties2.Path = "6-Reflection";
      modeProperties2.Checkpoints = new CheckpointData[5]
      {
        new CheckpointData("00", "checkpoint_6_0", new PlayerInventory?(), false, (AudioState) null),
        new CheckpointData("04", "checkpoint_6_1", new PlayerInventory?(), false, (AudioState) null),
        new CheckpointData("b-00", "checkpoint_6_2", new PlayerInventory?(), false, (AudioState) null),
        new CheckpointData("boss-00", "checkpoint_6_3", new PlayerInventory?(), false, (AudioState) null),
        new CheckpointData("after-00", "checkpoint_6_4", new PlayerInventory?(PlayerInventory.CH6End), false, (AudioState) null)
        {
          Flags = new HashSet<string>()
          {
            "badeline_connection"
          },
          AudioState = new AudioState(new AudioTrackState("event:/music/lvl6/badeline_acoustic").Param("levelup", 2f), new AudioTrackState("event:/env/amb/06_main"))
        }
      };
      modeProperties2.Inventory = PlayerInventory.Default;
      modeProperties2.AudioState = new AudioState("event:/music/lvl6/main", "event:/env/amb/06_main");
      modePropertiesArray2[0] = modeProperties2;
      modePropertiesArray2[1] = new ModeProperties()
      {
        PoemID = "tfr",
        Path = "6H-Reflection",
        Checkpoints = new CheckpointData[3]
        {
          new CheckpointData("b-00", "checkpoint_6h_0", new PlayerInventory?(), false, (AudioState) null),
          new CheckpointData("c-00", "checkpoint_6h_1", new PlayerInventory?(), false, (AudioState) null),
          new CheckpointData("d-00", "checkpoint_6h_2", new PlayerInventory?(), false, (AudioState) null)
        },
        Inventory = PlayerInventory.Default,
        AudioState = new AudioState("event:/music/remix/06_reflection", "event:/env/amb/06_main")
      };
      modePropertiesArray2[2] = new ModeProperties()
      {
        Path = "6X-Reflection",
        Checkpoints = (CheckpointData[]) null,
        Inventory = PlayerInventory.Default,
        AudioState = new AudioState("event:/music/remix/06_reflection", "event:/env/amb/06_main")
      };
      areaData15.Mode = modePropertiesArray2;
      areaData14.TitleBaseColor = Calc.HexToColor("359FE0");
      areaData14.TitleAccentColor = Calc.HexToColor("3C5CBC");
      areaData14.TitleTextColor = Color.White;
      areaData14.IntroType = Player.IntroTypes.None;
      areaData14.Dreaming = false;
      areaData14.ColorGrade = "reflection";
      FallWipe fallWipe;
      areaData14.Wipe = (Action<Scene, bool, Action>) ((scene, wipeIn, onComplete) => fallWipe = new FallWipe(scene, wipeIn, onComplete));
      areaData14.DarknessAlpha = 0.2f;
      areaData14.BloomBase = 0.2f;
      areaData14.BloomStrength = 1f;
      areaData14.OnLevelBegin = (Action<Level>) null;
      areaData14.Jumpthru = "reflection";
      areaData14.Spike = "reflection";
      areaData14.CassseteNoteColor = Calc.HexToColor("56e6dd");
      areaData14.CassetteSong = "event:/music/cassette/06_reflection";
      AreaData areaData16 = areaData14;
      areas7.Add(areaData16);
      List<AreaData> areas8 = AreaData.Areas;
      AreaData areaData17 = new AreaData();
      areaData17.Name = "area_7";
      areaData17.Icon = "areas/summit";
      areaData17.Interlude = false;
      areaData17.CompleteScreenName = "Summit";
      areaData17.CassetteCheckpointIndex = 3;
      areaData17.Mode = new ModeProperties[3]
      {
        new ModeProperties()
        {
          PoemID = "ts",
          Path = "7-Summit",
          Checkpoints = new CheckpointData[6]
          {
            new CheckpointData("b-00", "checkpoint_7_0", new PlayerInventory?(), false, new AudioState(new AudioTrackState("event:/music/lvl7/main").SetProgress(1), (AudioTrackState) null)),
            new CheckpointData("c-00", "checkpoint_7_1", new PlayerInventory?(), false, new AudioState(new AudioTrackState("event:/music/lvl7/main").SetProgress(2), (AudioTrackState) null)),
            new CheckpointData("d-00", "checkpoint_7_2", new PlayerInventory?(), false, new AudioState(new AudioTrackState("event:/music/lvl7/main").SetProgress(3), (AudioTrackState) null)),
            new CheckpointData("e-00b", "checkpoint_7_3", new PlayerInventory?(), false, new AudioState(new AudioTrackState("event:/music/lvl7/main").SetProgress(4), (AudioTrackState) null)),
            new CheckpointData("f-00", "checkpoint_7_4", new PlayerInventory?(), false, new AudioState(new AudioTrackState("event:/music/lvl7/main").SetProgress(5), (AudioTrackState) null)),
            new CheckpointData("g-00", "checkpoint_7_5", new PlayerInventory?(), false, new AudioState("event:/music/lvl7/final_ascent", (string) null))
          },
          Inventory = PlayerInventory.TheSummit,
          AudioState = new AudioState("event:/music/lvl7/main", (string) null)
        },
        new ModeProperties()
        {
          PoemID = "tsr",
          Path = "7H-Summit",
          Checkpoints = new CheckpointData[6]
          {
            new CheckpointData("b-00", "checkpoint_7H_0", new PlayerInventory?(), false, (AudioState) null),
            new CheckpointData("c-01", "checkpoint_7H_1", new PlayerInventory?(), false, (AudioState) null),
            new CheckpointData("d-00", "checkpoint_7H_2", new PlayerInventory?(), false, (AudioState) null),
            new CheckpointData("e-00", "checkpoint_7H_3", new PlayerInventory?(), false, (AudioState) null),
            new CheckpointData("f-00", "checkpoint_7H_4", new PlayerInventory?(), false, (AudioState) null),
            new CheckpointData("g-00", "checkpoint_7H_5", new PlayerInventory?(), false, (AudioState) null)
          },
          Inventory = PlayerInventory.TheSummit,
          AudioState = new AudioState("event:/music/remix/07_summit", (string) null)
        },
        new ModeProperties()
        {
          Path = "7X-Summit",
          Checkpoints = (CheckpointData[]) null,
          Inventory = PlayerInventory.TheSummit,
          AudioState = new AudioState("event:/music/remix/07_summit", (string) null)
        }
      };
      areaData17.TitleBaseColor = Calc.HexToColor("FFD819");
      areaData17.TitleAccentColor = Calc.HexToColor("197DB7");
      areaData17.TitleTextColor = Color.Black;
      areaData17.IntroType = Player.IntroTypes.None;
      areaData17.Dreaming = false;
      areaData17.ColorGrade = (string) null;
      MountainWipe mountainWipe;
      areaData17.Wipe = (Action<Scene, bool, Action>) ((scene, wipeIn, onComplete) => mountainWipe = new MountainWipe(scene, wipeIn, onComplete));
      areaData17.DarknessAlpha = 0.05f;
      areaData17.BloomBase = 0.2f;
      areaData17.BloomStrength = 1f;
      areaData17.OnLevelBegin = (Action<Level>) null;
      areaData17.Jumpthru = "temple";
      areaData17.Spike = "outline";
      areaData17.CassseteNoteColor = Calc.HexToColor("e69156");
      areaData17.CassetteSong = "event:/music/cassette/07_summit";
      AreaData areaData18 = areaData17;
      areas8.Add(areaData18);
      List<AreaData> areas9 = AreaData.Areas;
      AreaData areaData19 = new AreaData();
      areaData19.Name = "area_8";
      areaData19.Icon = "areas/intro";
      areaData19.Interlude = true;
      areaData19.CompleteScreenName = (string) null;
      areaData19.CassetteCheckpointIndex = 1;
      areaData19.Mode = new ModeProperties[3]
      {
        new ModeProperties()
        {
          PoemID = (string) null,
          Path = "8-Epilogue",
          Checkpoints = (CheckpointData[]) null,
          Inventory = PlayerInventory.TheSummit,
          AudioState = new AudioState("event:/music/lvl8/main", "event:/env/amb/00_prologue")
        },
        null,
        null
      };
      areaData19.TitleBaseColor = Calc.HexToColor("383838");
      areaData19.TitleAccentColor = Calc.HexToColor("50AFAE");
      areaData19.TitleTextColor = Color.White;
      areaData19.IntroType = Player.IntroTypes.WalkInLeft;
      areaData19.Dreaming = false;
      areaData19.ColorGrade = (string) null;
      CurtainWipe curtainWipe2;
      areaData19.Wipe = (Action<Scene, bool, Action>) ((scene, wipeIn, onComplete) => curtainWipe2 = new CurtainWipe(scene, wipeIn, onComplete));
      areaData19.DarknessAlpha = 0.05f;
      areaData19.BloomBase = 0.0f;
      areaData19.BloomStrength = 1f;
      areaData19.OnLevelBegin = (Action<Level>) null;
      areaData19.Jumpthru = "wood";
      AreaData areaData20 = areaData19;
      areas9.Add(areaData20);
      List<AreaData> areas10 = AreaData.Areas;
      AreaData areaData21 = new AreaData();
      areaData21.Name = "area_9";
      areaData21.Icon = "areas/core";
      areaData21.Interlude = false;
      areaData21.CompleteScreenName = "Core";
      areaData21.CassetteCheckpointIndex = 3;
      AreaData areaData22 = areaData21;
      ModeProperties[] modePropertiesArray3 = new ModeProperties[3];
      ModeProperties modeProperties3 = new ModeProperties();
      modeProperties3.PoemID = "mc";
      modeProperties3.Path = "9-Core";
      modeProperties3.Checkpoints = new CheckpointData[3]
      {
        new CheckpointData("a-00", "checkpoint_8_0", new PlayerInventory?(), false, (AudioState) null),
        new CheckpointData("c-00", "checkpoint_8_1", new PlayerInventory?(), false, (AudioState) null)
        {
          CoreMode = new Session.CoreModes?(Session.CoreModes.Cold)
        },
        new CheckpointData("d-00", "checkpoint_8_2", new PlayerInventory?(), false, (AudioState) null)
      };
      modeProperties3.Inventory = PlayerInventory.Core;
      modeProperties3.AudioState = new AudioState("event:/music/lvl9/main", "event:/env/amb/09_main");
      modeProperties3.IgnoreLevelAudioLayerData = true;
      modePropertiesArray3[0] = modeProperties3;
      modePropertiesArray3[1] = new ModeProperties()
      {
        PoemID = "mcr",
        Path = "9H-Core",
        Checkpoints = new CheckpointData[3]
        {
          new CheckpointData("a-00", "checkpoint_8H_0", new PlayerInventory?(), false, (AudioState) null),
          new CheckpointData("b-00", "checkpoint_8H_1", new PlayerInventory?(), false, (AudioState) null),
          new CheckpointData("c-01", "checkpoint_8H_2", new PlayerInventory?(), false, (AudioState) null)
        },
        Inventory = PlayerInventory.Core,
        AudioState = new AudioState("event:/music/remix/09_core", "event:/env/amb/09_main")
      };
      modePropertiesArray3[2] = new ModeProperties()
      {
        Path = "9X-Core",
        Checkpoints = (CheckpointData[]) null,
        Inventory = PlayerInventory.Core,
        AudioState = new AudioState("event:/music/remix/09_core", "event:/env/amb/09_main")
      };
      areaData22.Mode = modePropertiesArray3;
      areaData21.TitleBaseColor = Calc.HexToColor("761008");
      areaData21.TitleAccentColor = Calc.HexToColor("E0201D");
      areaData21.TitleTextColor = Color.White;
      areaData21.IntroType = Player.IntroTypes.WalkInRight;
      areaData21.Dreaming = false;
      areaData21.ColorGrade = (string) null;
      HeartWipe heartWipe;
      areaData21.Wipe = (Action<Scene, bool, Action>) ((scene, wipeIn, onComplete) => heartWipe = new HeartWipe(scene, wipeIn, onComplete));
      areaData21.DarknessAlpha = 0.05f;
      areaData21.BloomBase = 0.0f;
      areaData21.BloomStrength = 1f;
      areaData21.OnLevelBegin = (Action<Level>) null;
      areaData21.Jumpthru = "core";
      areaData21.CassseteNoteColor = Calc.HexToColor("e6566a");
      areaData21.CassetteSong = "event:/music/cassette/09_core";
      areaData21.CoreMode = Session.CoreModes.Hot;
      AreaData areaData23 = areaData21;
      areas10.Add(areaData23);
      int length = Enum.GetNames(typeof (AreaMode)).Length;
      for (int id = 0; id < AreaData.Areas.Count; ++id)
      {
        AreaData.Areas[id].ID = id;
        AreaData.Areas[id].Mode[0].MapData = new MapData(new AreaKey(id, AreaMode.Normal));
        if (!AreaData.Areas[id].Interlude)
        {
          for (int index = 1; index < length; ++index)
          {
            if (AreaData.Areas[id].HasMode((AreaMode) index))
              AreaData.Areas[id].Mode[index].MapData = new MapData(new AreaKey(id, (AreaMode) index));
          }
        }
      }
      AreaData.ReloadMountainViews();
    }

    public static void ReloadMountainViews()
    {
      foreach (XmlElement xml in (XmlNode) Calc.LoadXML(Path.Combine(Engine.ContentDirectory, "Overworld", "AreaViews.xml"))["Views"])
      {
        int index = xml.AttrInt("id");
        if (index >= 0 && index < AreaData.Areas.Count)
        {
          Vector3 pos1 = xml["Idle"].AttrVector3("position");
          Vector3 target1 = xml["Idle"].AttrVector3("target");
          AreaData.Areas[index].MountainIdle = new MountainCamera(pos1, target1);
          Vector3 pos2 = xml["Select"].AttrVector3("position");
          Vector3 target2 = xml["Select"].AttrVector3("target");
          AreaData.Areas[index].MountainSelect = new MountainCamera(pos2, target2);
          Vector3 pos3 = xml["Zoom"].AttrVector3("position");
          Vector3 target3 = xml["Zoom"].AttrVector3("target");
          AreaData.Areas[index].MountainZoom = new MountainCamera(pos3, target3);
          if (xml["Cursor"] != null)
            AreaData.Areas[index].MountainCursor = xml["Cursor"].AttrVector3("position");
          AreaData.Areas[index].MountainState = xml.AttrInt("state", 0);
        }
      }
    }

    public static bool IsPoemRemix(string id)
    {
      foreach (AreaData area in AreaData.Areas)
      {
        if (area.Mode[1] != null && !string.IsNullOrEmpty(area.Mode[1].PoemID) && area.Mode[1].PoemID.Equals(id, StringComparison.OrdinalIgnoreCase))
          return true;
      }
      return false;
    }

    public static int GetCheckpointID(AreaKey area, string level)
    {
      CheckpointData[] checkpoints = AreaData.Areas[area.ID].Mode[(int) area.Mode].Checkpoints;
      if (checkpoints != null)
      {
        for (int index = 0; index < checkpoints.Length; ++index)
        {
          if (checkpoints[index].Level.Equals(level))
            return index;
        }
      }
      return -1;
    }

    public static CheckpointData GetCheckpoint(AreaKey area, string level)
    {
      CheckpointData[] checkpoints = AreaData.Areas[area.ID].Mode[(int) area.Mode].Checkpoints;
      if (level != null && checkpoints != null)
      {
        foreach (CheckpointData checkpointData in checkpoints)
        {
          if (checkpointData.Level.Equals(level))
            return checkpointData;
        }
      }
      return (CheckpointData) null;
    }

    public static string GetCheckpointName(AreaKey area, string level)
    {
      if (string.IsNullOrEmpty(level))
        return "START";
      CheckpointData checkpoint = AreaData.GetCheckpoint(area, level);
      if (checkpoint != null)
        return Dialog.Clean(checkpoint.Name, (Language) null);
      return (string) null;
    }

    public static PlayerInventory GetCheckpointInventory(AreaKey area, string level)
    {
      CheckpointData checkpoint = AreaData.GetCheckpoint(area, level);
      if (checkpoint != null && checkpoint.Inventory.HasValue)
        return checkpoint.Inventory.Value;
      return AreaData.Areas[area.ID].Mode[(int) area.Mode].Inventory;
    }

    public static bool GetCheckpointDreaming(AreaKey area, string level)
    {
      CheckpointData checkpoint = AreaData.GetCheckpoint(area, level);
      if (checkpoint != null)
        return checkpoint.Dreaming;
      return AreaData.Areas[area.ID].Dreaming;
    }

    public static Session.CoreModes GetCheckpointCoreMode(AreaKey area, string level)
    {
      CheckpointData checkpoint = AreaData.GetCheckpoint(area, level);
      if (checkpoint != null && checkpoint.CoreMode.HasValue)
        return checkpoint.CoreMode.Value;
      return AreaData.Areas[area.ID].CoreMode;
    }

    public static AudioState GetCheckpointAudioState(AreaKey area, string level)
    {
      CheckpointData checkpoint = AreaData.GetCheckpoint(area, level);
      if (checkpoint != null)
        return checkpoint.AudioState;
      return (AudioState) null;
    }

    public static void Unload()
    {
      AreaData.Areas = (List<AreaData>) null;
    }

    public static AreaData Get(Scene scene)
    {
      if (scene != null && scene is Level)
        return AreaData.Areas[(scene as Level).Session.Area.ID];
      return (AreaData) null;
    }

    public static AreaData Get(Session session)
    {
      if (session != null)
        return AreaData.Areas[session.Area.ID];
      return (AreaData) null;
    }

    public static AreaData Get(AreaKey area)
    {
      return AreaData.Areas[area.ID];
    }

    public static AreaData Get(int id)
    {
      return AreaData.Areas[id];
    }

    public XmlElement CompleteScreenXml
    {
      get
      {
        return GFX.CompleteScreensXml["Screens"][this.CompleteScreenName];
      }
    }

    public void DoScreenWipe(Scene scene, bool wipeIn, Action onComplete = null)
    {
      if (this.Wipe == null)
      {
        WindWipe windWipe = new WindWipe(scene, wipeIn, onComplete);
      }
      else
        this.Wipe(scene, wipeIn, onComplete);
    }

    public bool HasMode(AreaMode mode)
    {
      return (AreaMode) this.Mode.Length > mode && this.Mode[(int) mode] != null && this.Mode[(int) mode].Path != null;
    }
  }
}

