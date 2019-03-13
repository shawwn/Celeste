// Decompiled with JetBrains decompiler
// Type: Celeste.OuiChapterPanel
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Celeste
{
  public class OuiChapterPanel : Oui
  {
    private bool initialized = false;
    private string chapter = "";
    private bool selectingMode = true;
    private MTexture card = new MTexture();
    private StrawberriesCounter strawberries = new StrawberriesCounter(true, 0, 0, true);
    private DeathsCounter deaths = new DeathsCounter(AreaMode.Normal, true, 0, 0);
    private HeartGemDisplay heart = new HeartGemDisplay(0, false);
    private int checkpoint = 0;
    private List<OuiChapterPanel.Option> modes = new List<OuiChapterPanel.Option>();
    private List<OuiChapterPanel.Option> checkpoints = new List<OuiChapterPanel.Option>();
    public AreaKey Area;
    public AreaStats RealStats;
    public AreaStats DisplayedStats;
    public AreaData Data;
    public Overworld.StartMode OverworldStartMode;
    public const int ContentOffsetX = 440;
    public const int NoStatsHeight = 300;
    public const int StatsHeight = 540;
    public const int CheckpointsHeight = 730;
    private float height;
    private bool resizing;
    private Wiggler wiggler;
    private Wiggler modeAppearWiggler;
    private Vector2 contentOffset;
    private float spotlightRadius;
    private float spotlightAlpha;
    private Vector2 spotlightPosition;
    private Vector2 strawberriesOffset;
    private Vector2 deathsOffset;
    private Vector2 heartOffset;

    public Vector2 OpenPosition
    {
      get
      {
        return new Vector2(1070f, 100f);
      }
    }

    public Vector2 ClosePosition
    {
      get
      {
        return new Vector2(2220f, 100f);
      }
    }

    public Vector2 IconOffset
    {
      get
      {
        return new Vector2(690f, 86f);
      }
    }

    private Vector2 OptionsRenderPosition
    {
      get
      {
        return this.Position + new Vector2(this.contentOffset.X, 128f + this.height);
      }
    }

    private int option
    {
      get
      {
        return this.selectingMode ? (int) this.Area.Mode : this.checkpoint;
      }
      set
      {
        if (this.selectingMode)
          this.Area.Mode = (AreaMode) value;
        else
          this.checkpoint = value;
      }
    }

    private List<OuiChapterPanel.Option> options
    {
      get
      {
        return this.selectingMode ? this.modes : this.checkpoints;
      }
    }

    public OuiChapterPanel()
    {
      this.Add((Component) this.strawberries);
      this.Add((Component) this.deaths);
      this.Add((Component) this.heart);
      this.deaths.CanWiggle = false;
      this.strawberries.CanWiggle = false;
      this.strawberries.OverworldSfx = true;
      this.Add((Component) (this.wiggler = Wiggler.Create(0.4f, 4f, (Action<float>) null, false, false)));
      this.Add((Component) (this.modeAppearWiggler = Wiggler.Create(0.4f, 4f, (Action<float>) null, false, false)));
    }

    public override bool IsStart(Overworld overworld, Overworld.StartMode start)
    {
      if (start == Overworld.StartMode.AreaComplete || start == Overworld.StartMode.AreaQuit)
      {
        bool shouldAdvance = start == Overworld.StartMode.AreaComplete && (Celeste.PlayMode == Celeste.PlayModes.Event || SaveData.Instance.CurrentSession != null && SaveData.Instance.CurrentSession.ShouldAdvance);
        this.Position = this.OpenPosition;
        this.Reset();
        this.Add((Component) new Coroutine(this.IncrementStats(shouldAdvance), true));
        overworld.ShowInputUI = false;
        overworld.Mountain.SnapState(this.Data.MountainState);
        overworld.Mountain.SnapCamera(this.Area.ID, this.Data.MountainZoom, false);
        double num = (double) overworld.Mountain.EaseCamera(this.Area.ID, this.Data.MountainSelect, new float?(1f), true, false);
        this.OverworldStartMode = start;
        return true;
      }
      this.Position = this.ClosePosition;
      return false;
    }

    public override IEnumerator Enter(Oui from)
    {
      this.Visible = true;
      this.Area.Mode = AreaMode.Normal;
      this.Reset();
      double num = (double) this.Overworld.Mountain.EaseCamera(this.Area.ID, this.Data.MountainSelect, new float?(), true, false);
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime * 4f)
      {
        this.Position = this.ClosePosition + (this.OpenPosition - this.ClosePosition) * Ease.CubeOut(p);
        yield return (object) null;
      }
    }

    private void Reset()
    {
      this.Area = SaveData.Instance.LastArea;
      this.Data = AreaData.Areas[this.Area.ID];
      this.RealStats = SaveData.Instance.Areas[this.Area.ID];
      if (SaveData.Instance.CurrentSession != null && SaveData.Instance.CurrentSession.OldStats != null && SaveData.Instance.CurrentSession.Area.ID == this.Area.ID)
      {
        this.DisplayedStats = SaveData.Instance.CurrentSession.OldStats;
        SaveData.Instance.CurrentSession = (Session) null;
      }
      else
        this.DisplayedStats = this.RealStats;
      this.height = (float) this.GetModeHeight();
      this.modes.Clear();
      bool flag1 = false;
      if (!this.Data.Interlude && this.Data.HasMode(AreaMode.BSide) && (this.DisplayedStats.Cassette || (SaveData.Instance.DebugMode || SaveData.Instance.CheatMode) && this.DisplayedStats.Cassette == this.RealStats.Cassette))
        flag1 = true;
      bool flag2 = !this.Data.Interlude && this.Data.HasMode(AreaMode.CSide) && SaveData.Instance.UnlockedModes >= 3 && Celeste.PlayMode != Celeste.PlayModes.Event;
      this.modes.Add(new OuiChapterPanel.Option()
      {
        Label = Dialog.Clean(this.Data.Interlude ? "FILE_BEGIN" : "overworld_normal", (Language) null).ToUpper(),
        Icon = GFX.Gui["menu/play"]
      });
      if (flag1)
        this.AddRemixButton();
      if (flag2)
        this.modes.Add(new OuiChapterPanel.Option()
        {
          Label = Dialog.Clean("overworld_remix2", (Language) null),
          Icon = GFX.Gui["menu/rmx2"]
        });
      this.selectingMode = true;
      this.UpdateStats(false);
      this.SetStatsPosition(false);
      for (int i = 0; i < this.options.Count; ++i)
        this.options[i].SlideTowards(i, this.options.Count, true);
      this.chapter = Dialog.Get("area_chapter", (Language) null).Replace("{x}", this.Area.ChapterIndex.ToString().PadLeft(2));
      this.contentOffset = new Vector2(440f, 120f);
      this.initialized = true;
    }

    private int GetModeHeight()
    {
      AreaModeStats mode = this.RealStats.Modes[(int) this.Area.Mode];
      bool flag = mode.Strawberries.Count <= 0;
      if (!this.Data.Interlude && (mode.Deaths > 0 && this.Area.Mode != AreaMode.Normal || mode.Completed || mode.HeartGem))
        flag = false;
      return flag ? 300 : 540;
    }

    private OuiChapterPanel.Option AddRemixButton()
    {
      OuiChapterPanel.Option option = new OuiChapterPanel.Option()
      {
        Label = Dialog.Clean("overworld_remix", (Language) null),
        Icon = GFX.Gui["menu/remix"]
      };
      this.modes.Insert(1, option);
      return option;
    }

    public override IEnumerator Leave(Oui next)
    {
      double num = (double) this.Overworld.Mountain.EaseCamera(this.Area.ID, this.Data.MountainIdle, new float?(), true, false);
      this.Add((Component) new Coroutine(this.EaseOut(true), true));
      yield break;
    }

    public IEnumerator EaseOut(bool removeChildren = true)
    {
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime * 4f)
      {
        this.Position = this.OpenPosition + (this.ClosePosition - this.OpenPosition) * Ease.CubeIn(p);
        yield return (object) null;
      }
      if (!this.Selected)
        this.Visible = false;
    }

    public void Start(string checkpoint = null)
    {
      this.Focused = false;
      Audio.Play("event:/ui/world_map/chapter/checkpoint_start");
      this.Add((Component) new Coroutine(this.StartRoutine(checkpoint), true));
    }

    private IEnumerator StartRoutine(string checkpoint = null)
    {
      this.Overworld.Maddy.Hide(false);
      double num = (double) this.Overworld.Mountain.EaseCamera(this.Area.ID, this.Data.MountainZoom, new float?(1f), true, false);
      this.Add((Component) new Coroutine(this.EaseOut(false), true));
      yield return (object) 0.2f;
      AreaData.Get(this.Area).Wipe((Scene) this.Overworld, false, (Action) null);
      Audio.SetMusic((string) null, true, true);
      Audio.SetAmbience((string) null, true);
      if ((this.Area.ID == 0 || this.Area.ID == 9) && checkpoint == null && this.Area.Mode == AreaMode.Normal)
      {
        this.Overworld.RendererList.UpdateLists();
        this.Overworld.RendererList.MoveToFront((Monocle.Renderer) this.Overworld.Snow);
      }
      yield return (object) 0.5f;
      LevelEnter.Go(new Session(this.Area, checkpoint, (AreaStats) null), false);
    }

    private void Swap()
    {
      this.Focused = false;
      this.Overworld.ShowInputUI = !this.selectingMode;
      this.Add((Component) new Coroutine(this.SwapRoutine(), true));
    }

    private IEnumerator SwapRoutine()
    {
      float fromHeight = this.height;
      int toHeight = this.selectingMode ? 730 : this.GetModeHeight();
      this.resizing = true;
      this.PlayExpandSfx(fromHeight, (float) toHeight);
      float offset = 800f;
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime * 4f)
      {
        yield return (object) null;
        this.contentOffset.X = (float) (440.0 + (double) offset * (double) Ease.CubeIn(p));
        this.height = MathHelper.Lerp(fromHeight, (float) toHeight, Ease.CubeOut(p * 0.5f));
      }
      this.selectingMode = !this.selectingMode;
      if (!this.selectingMode)
      {
        HashSet<string> levelsWithCheckpoints = SaveData.Instance.GetCheckpoints(this.Area);
        float scale = levelsWithCheckpoints.Count < 5 ? 1f : 0.75f;
        this.checkpoints.Clear();
        this.checkpoints.Add(new OuiChapterPanel.Option()
        {
          Label = Dialog.Clean("overworld_start", (Language) null),
          BgColor = Calc.HexToColor("eabe26"),
          Icon = GFX.Gui["areaselect/startpoint"],
          CheckpointLevelName = (string) null,
          CheckpointRotation = (float) Calc.Random.Choose<int>(-1, 1) * Calc.Random.Range(0.05f, 0.2f),
          CheckpointOffset = new Vector2((float) Calc.Random.Range(-16, 16), (float) Calc.Random.Range(-16, 16)),
          Large = false,
          Scale = scale
        });
        foreach (string str in levelsWithCheckpoints)
        {
          string level = str;
          this.checkpoints.Add(new OuiChapterPanel.Option()
          {
            Label = AreaData.GetCheckpointName(this.Area, level),
            Icon = GFX.Gui["areaselect/checkpoint"],
            CheckpointLevelName = level,
            CheckpointRotation = (float) Calc.Random.Choose<int>(-1, 1) * Calc.Random.Range(0.05f, 0.2f),
            CheckpointOffset = new Vector2((float) Calc.Random.Range(-16, 16), (float) Calc.Random.Range(-16, 16)),
            Large = false,
            Scale = scale
          });
          level = (string) null;
        }
        if (!this.RealStats.Modes[(int) this.Area.Mode].Completed && !SaveData.Instance.DebugMode && !SaveData.Instance.CheatMode)
        {
          this.option = this.checkpoints.Count - 1;
          for (int i = 0; i < this.checkpoints.Count - 1; ++i)
            this.options[i].CheckpointSlideOut = 1f;
        }
        else
          this.option = 0;
        for (int i = 0; i < this.options.Count; ++i)
          this.options[i].SlideTowards(i, this.options.Count, true);
        levelsWithCheckpoints = (HashSet<string>) null;
      }
      this.options[this.option].Pop = 1f;
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime * 4f)
      {
        yield return (object) null;
        this.height = MathHelper.Lerp(fromHeight, (float) toHeight, Ease.CubeOut(Math.Min(1f, (float) (0.5 + (double) p * 0.5))));
        this.contentOffset.X = (float) (440.0 + (double) offset * (1.0 - (double) Ease.CubeOut(p)));
      }
      this.contentOffset.X = 440f;
      this.height = (float) toHeight;
      this.Focused = true;
      this.resizing = false;
    }

    public override void Update()
    {
      if (!this.initialized)
        return;
      base.Update();
      for (int i = 0; i < this.options.Count; ++i)
      {
        OuiChapterPanel.Option option = this.options[i];
        option.Pop = Calc.Approach(option.Pop, this.option == i ? 1f : 0.0f, Engine.DeltaTime * 4f);
        option.Appear = Calc.Approach(option.Appear, 1f, Engine.DeltaTime * 3f);
        option.CheckpointSlideOut = Calc.Approach(option.CheckpointSlideOut, this.option > i ? 1f : 0.0f, Engine.DeltaTime * 4f);
        option.Faded = Calc.Approach(option.Faded, this.option == i || option.Appeared ? 0.0f : 1f, Engine.DeltaTime * 4f);
        option.SlideTowards(i, this.options.Count, false);
      }
      if (this.selectingMode && !this.resizing)
        this.height = Calc.Approach(this.height, (float) this.GetModeHeight(), Engine.DeltaTime * 1600f);
      if (this.Selected && this.Focused)
      {
        if (Input.MenuLeft.Pressed && this.option > 0)
        {
          Audio.Play("event:/ui/world_map/chapter/tab_roll_left");
          --this.option;
          this.wiggler.Start();
          if (this.selectingMode)
          {
            this.UpdateStats(true);
            this.PlayExpandSfx(this.height, (float) this.GetModeHeight());
          }
          else
            Audio.Play("event:/ui/world_map/chapter/checkpoint_photo_add");
        }
        else if (Input.MenuRight.Pressed && this.option + 1 < this.options.Count)
        {
          Audio.Play("event:/ui/world_map/chapter/tab_roll_right");
          ++this.option;
          this.wiggler.Start();
          if (this.selectingMode)
          {
            this.UpdateStats(true);
            this.PlayExpandSfx(this.height, (float) this.GetModeHeight());
          }
          else
            Audio.Play("event:/ui/world_map/chapter/checkpoint_photo_remove");
        }
        else if (Input.MenuConfirm.Pressed)
        {
          if (this.selectingMode)
          {
            if (!SaveData.Instance.FoundAnyCheckpoints(this.Area))
            {
              this.Start((string) null);
            }
            else
            {
              Audio.Play("event:/ui/world_map/chapter/level_select");
              this.Swap();
            }
          }
          else
            this.Start(this.options[this.option].CheckpointLevelName);
        }
        else if (Input.MenuCancel.Pressed)
        {
          if (this.selectingMode)
          {
            Audio.Play("event:/ui/world_map/chapter/back");
            this.Overworld.Goto<OuiChapterSelect>();
          }
          else
          {
            Audio.Play("event:/ui/world_map/chapter/checkpoint_back");
            this.Swap();
          }
        }
      }
      this.SetStatsPosition(true);
    }

    public override void Render()
    {
      if (!this.initialized)
        return;
      Vector2 optionsRenderPosition = this.OptionsRenderPosition;
      for (int index = 0; index < this.options.Count; ++index)
      {
        if (!this.options[index].OnTopOfUI)
          this.options[index].Render(optionsRenderPosition, this.option == index, this.wiggler, this.modeAppearWiggler);
      }
      MTexture mtexture1 = GFX.Gui["areaselect/cardtop"];
      mtexture1.Draw(this.Position + new Vector2(0.0f, -32f));
      MTexture mtexture2 = GFX.Gui["areaselect/card"];
      this.card = mtexture2.GetSubtexture(0, mtexture2.Height - (int) this.height, mtexture2.Width, (int) this.height, this.card);
      this.card.Draw(this.Position + new Vector2(0.0f, (float) (mtexture1.Height - 32)));
      for (int index = 0; index < this.options.Count; ++index)
      {
        if (this.options[index].OnTopOfUI)
          this.options[index].Render(optionsRenderPosition, this.option == index, this.wiggler, this.modeAppearWiggler);
      }
      ActiveFont.Draw(this.options[this.option].Label, optionsRenderPosition + new Vector2(0.0f, -140f), Vector2.One * 0.5f, Vector2.One * (float) (1.0 + (double) this.wiggler.Value * 0.100000001490116), Color.Black * 0.8f);
      if (this.selectingMode)
      {
        this.strawberries.Position = this.contentOffset + new Vector2(0.0f, 170f) + this.strawberriesOffset;
        this.deaths.Position = this.contentOffset + new Vector2(0.0f, 170f) + this.deathsOffset;
        this.heart.Position = this.contentOffset + new Vector2(0.0f, 170f) + this.heartOffset;
        base.Render();
      }
      if (!this.selectingMode)
      {
        Vector2 center = this.Position + new Vector2(this.contentOffset.X, 340f);
        for (int checkpointIndex = this.options.Count - 1; checkpointIndex >= 0; --checkpointIndex)
          this.DrawCheckpoint(center, this.options[checkpointIndex], checkpointIndex);
      }
      GFX.Gui["areaselect/title"].Draw(this.Position + new Vector2(-60f, 0.0f), Vector2.Zero, this.Data.TitleBaseColor);
      GFX.Gui["areaselect/accent"].Draw(this.Position + new Vector2(-60f, 0.0f), Vector2.Zero, this.Data.TitleAccentColor);
      string text = Dialog.Clean(AreaData.Get(this.Area).Name, (Language) null);
      if (this.Data.Interlude)
      {
        ActiveFont.Draw(text, this.Position + this.IconOffset + new Vector2(-100f, 0.0f), new Vector2(1f, 0.5f), Vector2.One * 1f, this.Data.TitleTextColor * 0.8f);
      }
      else
      {
        ActiveFont.Draw(this.chapter, this.Position + this.IconOffset + new Vector2(-100f, -2f), new Vector2(1f, 1f), Vector2.One * 0.6f, this.Data.TitleAccentColor * 0.8f);
        ActiveFont.Draw(text, this.Position + this.IconOffset + new Vector2(-100f, -18f), new Vector2(1f, 0.0f), Vector2.One * 1f, this.Data.TitleTextColor * 0.8f);
      }
      if ((double) this.spotlightAlpha <= 0.0)
        return;
      HiresRenderer.EndRender();
      SpotlightWipe.DrawSpotlight(this.spotlightPosition, this.spotlightRadius, Color.Black * this.spotlightAlpha);
      HiresRenderer.BeginRender((BlendState) null, (SamplerState) null);
    }

    private void DrawCheckpoint(Vector2 center, OuiChapterPanel.Option option, int checkpointIndex)
    {
      MTexture checkpointPreview = this.GetCheckpointPreview(this.Area, option.CheckpointLevelName);
      MTexture checkpoint = GFX.Checkpoints["polaroid"];
      float checkpointRotation = option.CheckpointRotation;
      Vector2 position1 = center + option.CheckpointOffset + Vector2.UnitX * 800f * Ease.CubeIn(option.CheckpointSlideOut);
      checkpoint.DrawCentered(position1, Color.White, 0.75f, checkpointRotation);
      MTexture mtexture = GFX.Gui["collectables/strawberry"];
      if (checkpointPreview != null)
      {
        Vector2 scale = Vector2.One * 0.75f;
        if (SaveData.Instance.Assists.MirrorMode)
          scale.X = -scale.X;
        checkpointPreview.DrawCentered(position1, Color.White, scale, checkpointRotation);
      }
      int mode = (int) this.Area.Mode;
      if (!this.RealStats.Modes[mode].Completed && !SaveData.Instance.CheatMode && !SaveData.Instance.DebugMode)
        return;
      Vector2 vec = new Vector2(300f, 220f);
      Vector2 vector2 = position1 + vec.Rotate(checkpointRotation);
      int length = checkpointIndex != 0 ? this.Data.Mode[mode].Checkpoints[checkpointIndex - 1].Strawberries : this.Data.Mode[mode].StartStrawberries;
      bool[] flagArray = new bool[length];
      foreach (EntityID strawberry in this.RealStats.Modes[mode].Strawberries)
      {
        for (int index = 0; index < length; ++index)
        {
          EntityData entityData = this.Data.Mode[mode].StrawberriesByCheckpoint[checkpointIndex, index];
          if (entityData != null && entityData.Level.Name == strawberry.Level && entityData.ID == strawberry.ID)
            flagArray[index] = true;
        }
      }
      Vector2 vector = Calc.AngleToVector(checkpointRotation, 1f);
      Vector2 position2 = vector2 - vector * (float) length * 44f;
      if (this.Area.Mode == AreaMode.Normal && this.Data.CassetteCheckpointIndex == checkpointIndex)
      {
        Vector2 position3 = position2 - vector * 60f;
        if (this.RealStats.Cassette)
          GFX.Journal["cassette"].DrawCentered(position3, Color.White, 1f, checkpointRotation);
        else
          GFX.Journal["cassette_outline"].DrawCentered(position3, Color.DarkGray, 1f, checkpointRotation);
      }
      for (int index = 0; index < length; ++index)
      {
        mtexture.DrawCentered(position2, flagArray[index] ? Color.White : Color.Black * 0.3f, 0.5f, checkpointRotation);
        position2 += vector * 44f;
      }
    }

    private void UpdateStats(bool wiggle = true)
    {
      AreaModeStats mode = this.DisplayedStats.Modes[(int) this.Area.Mode];
      this.deaths.Visible = mode.Deaths > 0 && (this.Area.Mode != AreaMode.Normal || this.RealStats.Modes[(int) this.Area.Mode].Completed) && !AreaData.Get(this.Area).Interlude;
      this.deaths.Amount = mode.Deaths;
      this.deaths.SetMode(this.Area.Mode);
      this.heart.Visible = mode.HeartGem && !AreaData.Get(this.Area).Interlude;
      this.heart.SetCurrentMode(this.Area.Mode, mode.HeartGem);
      this.strawberries.Visible = (mode.TotalStrawberries > 0 || mode.Completed && this.Area.Mode == AreaMode.Normal && AreaData.Get(this.Area).Mode[0].TotalStrawberries > 0) && !AreaData.Get(this.Area).Interlude;
      this.strawberries.Amount = mode.TotalStrawberries;
      this.strawberries.OutOf = this.Data.Mode[0].TotalStrawberries;
      this.strawberries.ShowOutOf = mode.Completed && this.Area.Mode == AreaMode.Normal;
      this.strawberries.Golden = (uint) this.Area.Mode > 0U;
      if (!wiggle)
        return;
      if (this.strawberries.Visible)
        this.strawberries.Wiggle();
      if (this.heart.Visible)
        this.heart.Wiggle();
      if (this.deaths.Visible)
        this.deaths.Wiggle();
    }

    private void SetStatsPosition(bool approach)
    {
      if (this.heart.Visible && (this.strawberries.Visible || this.deaths.Visible))
      {
        this.heartOffset = this.Approach(this.heartOffset, new Vector2(-120f, 0.0f), !approach);
        this.strawberriesOffset = this.Approach(this.strawberriesOffset, new Vector2(120f, this.deaths.Visible ? -40f : 0.0f), !approach);
        this.deathsOffset = this.Approach(this.deathsOffset, new Vector2(120f, this.strawberries.Visible ? 40f : 0.0f), !approach);
      }
      else if (this.heart.Visible)
      {
        this.heartOffset = this.Approach(this.heartOffset, Vector2.Zero, !approach);
      }
      else
      {
        this.strawberriesOffset = this.Approach(this.strawberriesOffset, new Vector2(0.0f, this.deaths.Visible ? -40f : 0.0f), !approach);
        this.deathsOffset = this.Approach(this.deathsOffset, new Vector2(0.0f, this.strawberries.Visible ? 40f : 0.0f), !approach);
      }
    }

    private Vector2 Approach(Vector2 from, Vector2 to, bool snap)
    {
      if (snap)
        return to;
      return from += (to - from) * (1f - (float) Math.Pow(1.0 / 1000.0, (double) Engine.DeltaTime));
    }

    public IEnumerator IncrementStats(bool shouldAdvance)
    {
      this.Focused = false;
      this.Overworld.ShowInputUI = false;
      if (this.Data.Interlude)
      {
        if (shouldAdvance && this.OverworldStartMode == Overworld.StartMode.AreaComplete)
        {
          yield return (object) 1.2f;
          this.Overworld.Goto<OuiChapterSelect>().AdvanceToNext();
        }
        else
          this.Focused = true;
        yield return (object) null;
      }
      else
      {
        AreaData data = this.Data;
        AreaStats stats = this.DisplayedStats;
        AreaStats newStats = SaveData.Instance.Areas[data.ID];
        AreaModeStats statsMode = stats.Modes[(int) this.Area.Mode];
        AreaModeStats newStatsMode = newStats.Modes[(int) this.Area.Mode];
        bool doStrawberries = newStatsMode.TotalStrawberries > statsMode.TotalStrawberries;
        bool doHeartGem = newStatsMode.HeartGem && !statsMode.HeartGem;
        bool doDeaths = newStatsMode.Deaths > statsMode.Deaths && (this.Area.Mode != AreaMode.Normal || newStatsMode.Completed);
        bool doRemixUnlock = this.Area.Mode == AreaMode.Normal && data.HasMode(AreaMode.BSide) && newStats.Cassette && !stats.Cassette;
        if (doStrawberries | doHeartGem | doDeaths | doRemixUnlock)
          yield return (object) 0.8f;
        if (Settings.Instance.SpeedrunClock == SpeedrunType.Off)
        {
          if (doHeartGem)
          {
            Audio.Play("event:/ui/postgame/crystal_heart");
            this.heart.Visible = true;
            this.heart.SetCurrentMode(this.Area.Mode, true);
            this.heart.Appear(this.Area.Mode);
            yield return (object) 1.8f;
          }
          if (doStrawberries)
          {
            this.strawberries.CanWiggle = true;
            this.strawberries.Visible = true;
            while (newStatsMode.TotalStrawberries > statsMode.TotalStrawberries)
            {
              int diff = newStatsMode.TotalStrawberries - statsMode.TotalStrawberries;
              if (diff < 3)
                yield return (object) 0.3f;
              else if (diff < 8)
              {
                yield return (object) 0.2f;
              }
              else
              {
                yield return (object) 0.1f;
                ++statsMode.TotalStrawberries;
              }
              ++statsMode.TotalStrawberries;
              this.strawberries.Amount = statsMode.TotalStrawberries;
              Input.Rumble(RumbleStrength.Light, RumbleLength.Short);
            }
            this.strawberries.CanWiggle = false;
            yield return (object) 0.5f;
            if (newStatsMode.Completed && !statsMode.Completed && this.Area.Mode == AreaMode.Normal)
            {
              yield return (object) 0.25f;
              Audio.Play(this.strawberries.Amount >= this.Data.Mode[0].TotalStrawberries ? "event:/ui/postgame/strawberry_total_all" : "event:/ui/postgame/strawberry_total");
              this.strawberries.OutOf = this.Data.Mode[0].TotalStrawberries;
              this.strawberries.ShowOutOf = true;
              this.strawberries.Wiggle();
              statsMode.Completed = true;
              Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
              yield return (object) 0.6f;
            }
          }
          if (doDeaths)
          {
            Audio.Play("event:/ui/postgame/death_appear");
            this.deaths.CanWiggle = true;
            this.deaths.Visible = true;
            while (newStatsMode.Deaths > statsMode.Deaths)
            {
              int add;
              yield return (object) this.HandleDeathTick(statsMode.Deaths, newStatsMode.Deaths, out add);
              statsMode.Deaths += add;
              this.deaths.Amount = statsMode.Deaths;
              if (statsMode.Deaths >= newStatsMode.Deaths)
                Audio.Play("event:/ui/postgame/death_final");
              else
                Audio.Play("event:/ui/postgame/death_count");
              Input.Rumble(RumbleStrength.Light, RumbleLength.Short);
            }
            yield return (object) 0.8f;
            this.deaths.CanWiggle = false;
          }
          if (doRemixUnlock)
          {
            Audio.Play("event:/ui/postgame/unlock_bside");
            OuiChapterPanel.Option o = this.AddRemixButton();
            o.Appear = 0.0f;
            o.IconEase = 0.0f;
            o.Appeared = true;
            yield return (object) 0.5f;
            this.spotlightPosition = o.GetRenderPosition(this.OptionsRenderPosition);
            for (float t = 0.0f; (double) t < 1.0; t += Engine.DeltaTime / 0.5f)
            {
              this.spotlightAlpha = t * 0.9f;
              this.spotlightRadius = 128f * Ease.CubeOut(t);
              yield return (object) null;
            }
            yield return (object) 0.3f;
            while ((double) (o.IconEase += Engine.DeltaTime * 2f) < 1.0)
              yield return (object) null;
            o.IconEase = 1f;
            this.modeAppearWiggler.Start();
            Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
            AreaCompleteTitle text = new AreaCompleteTitle(this.spotlightPosition + new Vector2(0.0f, 80f), Dialog.Clean("OVERWORLD_REMIX_UNLOCKED", (Language) null), 1f);
            text.Tag = (int) Tags.HUD;
            this.Overworld.Add((Entity) text);
            yield return (object) 1.5f;
            for (float t = 0.0f; (double) t < 1.0; t += Engine.DeltaTime / 0.5f)
            {
              this.spotlightAlpha = (float) ((1.0 - (double) t) * 0.5);
              this.spotlightRadius = (float) (128.0 + 128.0 * (double) Ease.CubeOut(t));
              text.Alpha = 1f - Ease.CubeOut(t);
              yield return (object) null;
            }
            text.RemoveSelf();
            o.Appeared = false;
            o = (OuiChapterPanel.Option) null;
            text = (AreaCompleteTitle) null;
          }
          this.DisplayedStats = this.RealStats;
        }
        else
        {
          if (doRemixUnlock)
            this.AddRemixButton();
          this.DisplayedStats = this.RealStats;
          this.UpdateStats(true);
          yield return (object) null;
        }
        if (shouldAdvance && this.OverworldStartMode == Overworld.StartMode.AreaComplete)
        {
          if (!doDeaths && !doStrawberries && !doHeartGem || (uint) Settings.Instance.SpeedrunClock > 0U)
            yield return (object) 1.2f;
          this.Overworld.Goto<OuiChapterSelect>().AdvanceToNext();
        }
        else
        {
          this.Focused = true;
          this.Overworld.ShowInputUI = true;
        }
      }
    }

    private float HandleDeathTick(int oldDeaths, int newDeaths, out int add)
    {
      int num = newDeaths - oldDeaths;
      if (num < 3)
      {
        add = 1;
        return 0.3f;
      }
      if (num < 8)
      {
        add = 2;
        return 0.2f;
      }
      if (num < 30)
      {
        add = 5;
        return 0.1f;
      }
      if (num < 100)
      {
        add = 10;
        return 0.1f;
      }
      if (num < 1000)
      {
        add = 25;
        return 0.1f;
      }
      add = 100;
      return 0.1f;
    }

    private void PlayExpandSfx(float currentHeight, float nextHeight)
    {
      if ((double) nextHeight > (double) currentHeight)
      {
        Audio.Play("event:/ui/world_map/chapter/pane_expand");
      }
      else
      {
        if ((double) nextHeight >= (double) currentHeight)
          return;
        Audio.Play("event:/ui/world_map/chapter/pane_contract");
      }
    }

    public static string GetCheckpointPreviewName(AreaKey area, string level)
    {
      if (level == null)
        return area.ToString();
      return area.ToString() + "_" + level;
    }

    private MTexture GetCheckpointPreview(AreaKey area, string level)
    {
      string checkpointPreviewName = OuiChapterPanel.GetCheckpointPreviewName(area, level);
      if (GFX.Checkpoints.Has(checkpointPreviewName))
        return GFX.Checkpoints[checkpointPreviewName];
      return (MTexture) null;
    }

    private class Option
    {
      public MTexture Bg = GFX.Gui["areaselect/tab"];
      public Color BgColor = Calc.HexToColor("3c6180");
      public bool Large = true;
      public float Scale = 1f;
      public float Slide = 0.0f;
      public float Appear = 1f;
      public float IconEase = 1f;
      public float Faded = 0.0f;
      public float CheckpointSlideOut = 0.0f;
      public string Label;
      public MTexture Icon;
      public float Pop;
      public bool Appeared;
      public string CheckpointLevelName;
      public float CheckpointRotation;
      public Vector2 CheckpointOffset;

      public bool OnTopOfUI
      {
        get
        {
          return (double) this.Pop > 0.5;
        }
      }

      public void SlideTowards(int i, int count, bool snap)
      {
        float num = (float) ((double) count / 2.0 - 0.5);
        float target = (float) i - num;
        if (snap)
          this.Slide = target;
        else
          this.Slide = Calc.Approach(this.Slide, target, Engine.DeltaTime * 4f);
      }

      public Vector2 GetRenderPosition(Vector2 center)
      {
        Vector2 vector2 = center + new Vector2(this.Slide * (this.Large ? 170f : 130f) * this.Scale, (float) (Math.Sin((double) this.Pop * 3.14159274101257) * 70.0 - (double) this.Pop * 12.0));
        vector2.Y += (float) ((1.0 - (double) Ease.CubeOut(this.Appear)) * -200.0);
        vector2.Y -= (float) ((1.0 - (double) this.Scale) * 80.0);
        return vector2;
      }

      public void Render(Vector2 center, bool selected, Wiggler wiggler, Wiggler appearWiggler)
      {
        float num1 = (float) ((double) this.Scale + (selected ? (double) wiggler.Value * 0.25 : 0.0) + (this.Appeared ? (double) appearWiggler.Value * 0.25 : 0.0));
        Vector2 renderPosition = this.GetRenderPosition(center);
        Color color1 = Color.Lerp(this.BgColor, Color.Black, (float) ((1.0 - (double) this.Pop) * 0.600000023841858));
        this.Bg.DrawCentered(renderPosition + new Vector2(0.0f, 10f), color1, (this.Appeared ? this.Scale : num1) * new Vector2(this.Large ? 1f : 0.9f, 1f));
        if ((double) this.IconEase <= 0.0)
          return;
        float num2 = Ease.CubeIn(this.IconEase);
        Color color2 = Color.Lerp(Color.White, Color.Black, this.Faded * 0.6f) * num2;
        this.Icon.DrawCentered(renderPosition, color2, (float) ((double) (this.Bg.Width - 50) / (double) this.Icon.Width * (double) num1 * (2.5 - (double) num2 * 1.5)));
      }
    }
  }
}

