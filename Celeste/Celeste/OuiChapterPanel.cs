// Decompiled with JetBrains decompiler
// Type: Celeste.OuiChapterPanel
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using FMOD.Studio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Celeste
{
  public class OuiChapterPanel : Oui
  {
    public AreaKey Area;
    public AreaStats RealStats;
    public AreaStats DisplayedStats;
    public AreaData Data;
    public Overworld.StartMode OverworldStartMode;
    public bool EnteringChapter;
    public const int ContentOffsetX = 440;
    public const int NoStatsHeight = 300;
    public const int StatsHeight = 540;
    public const int CheckpointsHeight = 730;
    private bool initialized;
    private string chapter = "";
    private bool selectingMode = true;
    private float height;
    private bool resizing;
    private Wiggler wiggler;
    private Wiggler modeAppearWiggler;
    private MTexture card = new MTexture();
    private Vector2 contentOffset;
    private float spotlightRadius;
    private float spotlightAlpha;
    private Vector2 spotlightPosition;
    private AreaCompleteTitle remixUnlockText;
    private StrawberriesCounter strawberries = new StrawberriesCounter(true, 0, showOutOf: true);
    private Vector2 strawberriesOffset;
    private DeathsCounter deaths = new DeathsCounter(AreaMode.Normal, true, 0);
    private Vector2 deathsOffset;
    private HeartGemDisplay heart = new HeartGemDisplay(0, false);
    private Vector2 heartOffset;
    private int checkpoint;
    private List<OuiChapterPanel.Option> modes = new List<OuiChapterPanel.Option>();
    private List<OuiChapterPanel.Option> checkpoints = new List<OuiChapterPanel.Option>();
    private EventInstance bSideUnlockSfx;

    public Vector2 OpenPosition => new Vector2(1070f, 100f);

    public Vector2 ClosePosition => new Vector2(2220f, 100f);

    public Vector2 IconOffset => new Vector2(690f, 86f);

    private Vector2 OptionsRenderPosition => this.Position + new Vector2(this.contentOffset.X, 128f + this.height);

    private int option
    {
      get => !this.selectingMode ? this.checkpoint : (int) this.Area.Mode;
      set
      {
        if (this.selectingMode)
          this.Area.Mode = (AreaMode) value;
        else
          this.checkpoint = value;
      }
    }

    private List<OuiChapterPanel.Option> options => !this.selectingMode ? this.checkpoints : this.modes;

    public OuiChapterPanel()
    {
      this.Add((Component) this.strawberries);
      this.Add((Component) this.deaths);
      this.Add((Component) this.heart);
      this.deaths.CanWiggle = false;
      this.strawberries.CanWiggle = false;
      this.strawberries.OverworldSfx = true;
      this.Add((Component) (this.wiggler = Wiggler.Create(0.4f, 4f)));
      this.Add((Component) (this.modeAppearWiggler = Wiggler.Create(0.4f, 4f)));
    }

    public override bool IsStart(Overworld overworld, Overworld.StartMode start)
    {
      if (start == Overworld.StartMode.AreaComplete || start == Overworld.StartMode.AreaQuit)
      {
        bool shouldAdvance = start == Overworld.StartMode.AreaComplete && (Celeste.PlayMode == Celeste.PlayModes.Event || SaveData.Instance.CurrentSession != null && SaveData.Instance.CurrentSession.ShouldAdvance);
        this.Position = this.OpenPosition;
        this.Reset();
        this.Add((Component) new Coroutine(this.IncrementStats(shouldAdvance)));
        overworld.ShowInputUI = false;
        overworld.Mountain.SnapState(this.Data.MountainState);
        overworld.Mountain.SnapCamera(this.Area.ID, this.Data.MountainZoom);
        double num = (double) overworld.Mountain.EaseCamera(this.Area.ID, this.Data.MountainSelect, new float?(1f));
        this.OverworldStartMode = start;
        return true;
      }
      this.Position = this.ClosePosition;
      return false;
    }

    public override IEnumerator Enter(Oui from)
    {
      OuiChapterPanel ouiChapterPanel = this;
      ouiChapterPanel.Visible = true;
      ouiChapterPanel.Area.Mode = AreaMode.Normal;
      ouiChapterPanel.Reset();
      double num = (double) ouiChapterPanel.Overworld.Mountain.EaseCamera(ouiChapterPanel.Area.ID, ouiChapterPanel.Data.MountainSelect);
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime * 4f)
      {
        yield return (object) null;
        ouiChapterPanel.Position = ouiChapterPanel.ClosePosition + (ouiChapterPanel.OpenPosition - ouiChapterPanel.ClosePosition) * Ease.CubeOut(p);
      }
      ouiChapterPanel.Position = ouiChapterPanel.OpenPosition;
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
      bool flag = false;
      if (!this.Data.Interlude && this.Data.HasMode(AreaMode.BSide) && (this.DisplayedStats.Cassette || (SaveData.Instance.DebugMode || SaveData.Instance.CheatMode) && this.DisplayedStats.Cassette == this.RealStats.Cassette))
        flag = true;
      int num = this.Data.Interlude || !this.Data.HasMode(AreaMode.CSide) || SaveData.Instance.UnlockedModes < 3 ? 0 : (Celeste.PlayMode != Celeste.PlayModes.Event ? 1 : 0);
      this.modes.Add(new OuiChapterPanel.Option()
      {
        Label = Dialog.Clean(this.Data.Interlude ? "FILE_BEGIN" : "overworld_normal").ToUpper(),
        Icon = GFX.Gui["menu/play"],
        ID = "A"
      });
      if (flag)
        this.AddRemixButton();
      if (num != 0)
        this.modes.Add(new OuiChapterPanel.Option()
        {
          Label = Dialog.Clean("overworld_remix2"),
          Icon = GFX.Gui["menu/rmx2"],
          ID = "C"
        });
      this.selectingMode = true;
      this.UpdateStats(false);
      this.SetStatsPosition(false);
      for (int index = 0; index < this.options.Count; ++index)
        this.options[index].SlideTowards(index, this.options.Count, true);
      this.chapter = Dialog.Get("area_chapter").Replace("{x}", this.Area.ChapterIndex.ToString().PadLeft(2));
      this.contentOffset = new Vector2(440f, 120f);
      this.initialized = true;
    }

    private int GetModeHeight()
    {
      AreaModeStats mode = this.RealStats.Modes[(int) this.Area.Mode];
      bool flag = mode.Strawberries.Count <= 0;
      if (!this.Data.Interlude && (mode.Deaths > 0 && this.Area.Mode != AreaMode.Normal || mode.Completed || mode.HeartGem))
        flag = false;
      return !flag ? 540 : 300;
    }

    private OuiChapterPanel.Option AddRemixButton()
    {
      OuiChapterPanel.Option option = new OuiChapterPanel.Option()
      {
        Label = Dialog.Clean("overworld_remix"),
        Icon = GFX.Gui["menu/remix"],
        ID = "B"
      };
      this.modes.Insert(1, option);
      return option;
    }

    // public override IEnumerator Leave(Oui next)
    // {
    //   // ISSUE: reference to a compiler-generated field
    //   int num1 = this.\u003C\u003E1__state;
    //   OuiChapterPanel ouiChapterPanel = this;
    //   if (num1 != 0)
    //     return false;
    //   // ISSUE: reference to a compiler-generated field
    //   this.\u003C\u003E1__state = -1;
    //   ouiChapterPanel.Overworld.Mountain.EaseCamera(ouiChapterPanel.Area.ID, ouiChapterPanel.Data.MountainIdle);
    //   ouiChapterPanel.Add((Component) new Coroutine(ouiChapterPanel.EaseOut()));
    //   return false;
    // }
    public override IEnumerator Leave(Oui next)
    {
      double num = (double) this.Overworld.Mountain.EaseCamera(this.Area.ID, this.Data.MountainIdle, new float?(), true, false);
      this.Add((Component) new Coroutine(this.EaseOut(true), true));
      yield break;
    }

    public IEnumerator EaseOut(bool removeChildren = true)
    {
      OuiChapterPanel ouiChapterPanel = this;
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime * 4f)
      {
        ouiChapterPanel.Position = ouiChapterPanel.OpenPosition + (ouiChapterPanel.ClosePosition - ouiChapterPanel.OpenPosition) * Ease.CubeIn(p);
        yield return (object) null;
      }
      if (!ouiChapterPanel.Selected)
        ouiChapterPanel.Visible = false;
    }

    public void Start(string checkpoint = null)
    {
      this.Focused = false;
      Audio.Play("event:/ui/world_map/chapter/checkpoint_start");
      this.Add((Component) new Coroutine(this.StartRoutine(checkpoint)));
    }

    private IEnumerator StartRoutine(string checkpoint = null)
    {
      OuiChapterPanel ouiChapterPanel = this;
      ouiChapterPanel.EnteringChapter = true;
      ouiChapterPanel.Overworld.Maddy.Hide(false);
      double num = (double) ouiChapterPanel.Overworld.Mountain.EaseCamera(ouiChapterPanel.Area.ID, ouiChapterPanel.Data.MountainZoom, new float?(1f));
      ouiChapterPanel.Add((Component) new Coroutine(ouiChapterPanel.EaseOut(false)));
      yield return (object) 0.2f;
      ScreenWipe.WipeColor = Color.Black;
      AreaData.Get(ouiChapterPanel.Area).Wipe((Scene) ouiChapterPanel.Overworld, false, (Action) null);
      Audio.SetMusic((string) null);
      Audio.SetAmbience((string) null);
      if ((ouiChapterPanel.Area.ID == 0 || ouiChapterPanel.Area.ID == 9) && checkpoint == null && ouiChapterPanel.Area.Mode == AreaMode.Normal)
      {
        ouiChapterPanel.Overworld.RendererList.UpdateLists();
        ouiChapterPanel.Overworld.RendererList.MoveToFront((Monocle.Renderer) ouiChapterPanel.Overworld.Snow);
      }
      yield return (object) 0.5f;
      LevelEnter.Go(new Session(ouiChapterPanel.Area, checkpoint), false);
    }

    private void Swap()
    {
      this.Focused = false;
      this.Overworld.ShowInputUI = !this.selectingMode;
      this.Add((Component) new Coroutine(this.SwapRoutine()));
    }

    private IEnumerator SwapRoutine()
    {
      OuiChapterPanel ouiChapterPanel = this;
      float fromHeight = ouiChapterPanel.height;
      int toHeight = ouiChapterPanel.selectingMode ? 730 : ouiChapterPanel.GetModeHeight();
      ouiChapterPanel.resizing = true;
      ouiChapterPanel.PlayExpandSfx(fromHeight, (float) toHeight);
      float offset = 800f;
      float p;
      for (p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime * 4f)
      {
        yield return (object) null;
        ouiChapterPanel.contentOffset.X = (float) (440.0 + (double) offset * (double) Ease.CubeIn(p));
        ouiChapterPanel.height = MathHelper.Lerp(fromHeight, (float) toHeight, Ease.CubeOut(p * 0.5f));
      }
      ouiChapterPanel.selectingMode = !ouiChapterPanel.selectingMode;
      if (!ouiChapterPanel.selectingMode)
      {
        HashSet<string> checkpoints = SaveData.Instance.GetCheckpoints(ouiChapterPanel.Area);
        int num = checkpoints.Count + 1;
        ouiChapterPanel.checkpoints.Clear();
        ouiChapterPanel.checkpoints.Add(new OuiChapterPanel.Option()
        {
          Label = Dialog.Clean("overworld_start"),
          BgColor = Calc.HexToColor("eabe26"),
          Icon = GFX.Gui["areaselect/startpoint"],
          CheckpointLevelName = (string) null,
          CheckpointRotation = (float) Calc.Random.Choose<int>(-1, 1) * Calc.Random.Range(0.05f, 0.2f),
          CheckpointOffset = new Vector2((float) Calc.Random.Range(-16, 16), (float) Calc.Random.Range(-16, 16)),
          Large = false,
          Siblings = num
        });
        foreach (string level in checkpoints)
          ouiChapterPanel.checkpoints.Add(new OuiChapterPanel.Option()
          {
            Label = AreaData.GetCheckpointName(ouiChapterPanel.Area, level),
            Icon = GFX.Gui["areaselect/checkpoint"],
            CheckpointLevelName = level,
            CheckpointRotation = (float) Calc.Random.Choose<int>(-1, 1) * Calc.Random.Range(0.05f, 0.2f),
            CheckpointOffset = new Vector2((float) Calc.Random.Range(-16, 16), (float) Calc.Random.Range(-16, 16)),
            Large = false,
            Siblings = num
          });
        if (!ouiChapterPanel.RealStats.Modes[(int) ouiChapterPanel.Area.Mode].Completed && !SaveData.Instance.DebugMode && !SaveData.Instance.CheatMode)
        {
          ouiChapterPanel.option = ouiChapterPanel.checkpoints.Count - 1;
          for (int index = 0; index < ouiChapterPanel.checkpoints.Count - 1; ++index)
            ouiChapterPanel.options[index].CheckpointSlideOut = 1f;
        }
        else
          ouiChapterPanel.option = 0;
        for (int index = 0; index < ouiChapterPanel.options.Count; ++index)
          ouiChapterPanel.options[index].SlideTowards(index, ouiChapterPanel.options.Count, true);
      }
      ouiChapterPanel.options[ouiChapterPanel.option].Pop = 1f;
      for (p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime * 4f)
      {
        yield return (object) null;
        ouiChapterPanel.height = MathHelper.Lerp(fromHeight, (float) toHeight, Ease.CubeOut(Math.Min(1f, (float) (0.5 + (double) p * 0.5))));
        ouiChapterPanel.contentOffset.X = (float) (440.0 + (double) offset * (1.0 - (double) Ease.CubeOut(p)));
      }
      ouiChapterPanel.contentOffset.X = 440f;
      ouiChapterPanel.height = (float) toHeight;
      ouiChapterPanel.Focused = true;
      ouiChapterPanel.resizing = false;
    }

    public override void Update()
    {
      if (!this.initialized)
        return;
      base.Update();
      for (int index = 0; index < this.options.Count; ++index)
      {
        OuiChapterPanel.Option option = this.options[index];
        option.Pop = Calc.Approach(option.Pop, this.option == index ? 1f : 0.0f, Engine.DeltaTime * 4f);
        option.Appear = Calc.Approach(option.Appear, 1f, Engine.DeltaTime * 3f);
        option.CheckpointSlideOut = Calc.Approach(option.CheckpointSlideOut, this.option > index ? 1f : 0.0f, Engine.DeltaTime * 4f);
        option.Faded = Calc.Approach(option.Faded, this.option == index || option.Appeared ? 0.0f : 1f, Engine.DeltaTime * 4f);
        option.SlideTowards(index, this.options.Count, false);
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
            this.UpdateStats();
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
            this.UpdateStats();
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
              this.Start();
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
      bool flag = false;
      if (this.RealStats.Modes[(int) this.Area.Mode].Completed)
      {
        int mode = (int) this.Area.Mode;
        foreach (EntityData goldenberry in AreaData.Areas[this.Area.ID].Mode[mode].MapData.Goldenberries)
        {
          EntityID entityId = new EntityID(goldenberry.Level.Name, goldenberry.ID);
          if (this.RealStats.Modes[mode].Strawberries.Contains(entityId))
          {
            flag = true;
            break;
          }
        }
      }
      MTexture mtexture1 = GFX.Gui[!flag ? "areaselect/cardtop" : "areaselect/cardtop_golden"];
      mtexture1.Draw(this.Position + new Vector2(0.0f, -32f));
      MTexture mtexture2 = GFX.Gui[!flag ? "areaselect/card" : "areaselect/card_golden"];
      this.card = mtexture2.GetSubtexture(0, mtexture2.Height - (int) this.height, mtexture2.Width, (int) this.height, this.card);
      this.card.Draw(this.Position + new Vector2(0.0f, (float) (mtexture1.Height - 32)));
      for (int index = 0; index < this.options.Count; ++index)
      {
        if (this.options[index].OnTopOfUI)
          this.options[index].Render(optionsRenderPosition, this.option == index, this.wiggler, this.modeAppearWiggler);
      }
      ActiveFont.Draw(this.options[this.option].Label, optionsRenderPosition + new Vector2(0.0f, -140f), Vector2.One * 0.5f, Vector2.One * (float) (1.0 + (double) this.wiggler.Value * 0.10000000149011612), Color.Black * 0.8f);
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
        for (int index = this.options.Count - 1; index >= 0; --index)
          this.DrawCheckpoint(center, this.options[index], index);
      }
      GFX.Gui["areaselect/title"].Draw(this.Position + new Vector2(-60f, 0.0f), Vector2.Zero, this.Data.TitleBaseColor);
      GFX.Gui["areaselect/accent"].Draw(this.Position + new Vector2(-60f, 0.0f), Vector2.Zero, this.Data.TitleAccentColor);
      string text = Dialog.Clean(AreaData.Get(this.Area).Name);
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
      HiresRenderer.BeginRender();
    }

    private void DrawCheckpoint(Vector2 center, OuiChapterPanel.Option option, int checkpointIndex)
    {
      MTexture checkpointPreview = this.GetCheckpointPreview(this.Area, option.CheckpointLevelName);
      MTexture checkpoint = MTN.Checkpoints["polaroid"];
      float checkpointRotation = option.CheckpointRotation;
      Vector2 position1 = center + option.CheckpointOffset + Vector2.UnitX * 800f * Ease.CubeIn(option.CheckpointSlideOut);
      Vector2 position2 = position1;
      Color white = Color.White;
      double rotation = (double) checkpointRotation;
      checkpoint.DrawCentered(position2, white, 0.75f, (float) rotation);
      MTexture mtexture = GFX.Gui["collectables/strawberry"];
      if (checkpointPreview != null)
      {
        Vector2 scale = Vector2.One * 0.75f;
        if (SaveData.Instance.Assists.MirrorMode)
          scale.X = -scale.X;
        scale *= 720f / (float) checkpointPreview.Width;
        HiresRenderer.EndRender();
        HiresRenderer.BeginRender(BlendState.AlphaBlend, SamplerState.PointClamp);
        checkpointPreview.DrawCentered(position1, Color.White, scale, checkpointRotation);
        HiresRenderer.EndRender();
        HiresRenderer.BeginRender();
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
      Vector2 position3 = vector2 - vector * (float) length * 44f;
      if (this.Area.Mode == AreaMode.Normal && this.Data.CassetteCheckpointIndex == checkpointIndex)
      {
        Vector2 position4 = position3 - vector * 60f;
        if (this.RealStats.Cassette)
          MTN.Journal["cassette"].DrawCentered(position4, Color.White, 1f, checkpointRotation);
        else
          MTN.Journal["cassette_outline"].DrawCentered(position4, Color.DarkGray, 1f, checkpointRotation);
      }
      for (int index = 0; index < length; ++index)
      {
        mtexture.DrawCentered(position3, flagArray[index] ? Color.White : Color.Black * 0.3f, 0.5f, checkpointRotation);
        position3 += vector * 44f;
      }
    }

    private void UpdateStats(
      bool wiggle = true,
      bool? overrideStrawberryWiggle = null,
      bool? overrideDeathWiggle = null,
      bool? overrideHeartWiggle = null)
    {
      AreaModeStats mode = this.DisplayedStats.Modes[(int) this.Area.Mode];
      AreaData areaData = AreaData.Get(this.Area);
      this.deaths.Visible = mode.Deaths > 0 && (this.Area.Mode != AreaMode.Normal || this.RealStats.Modes[(int) this.Area.Mode].Completed) && !AreaData.Get(this.Area).Interlude;
      this.deaths.Amount = mode.Deaths;
      this.deaths.SetMode(areaData.IsFinal ? AreaMode.CSide : this.Area.Mode);
      this.heart.Visible = mode.HeartGem && !areaData.Interlude && areaData.CanFullClear;
      this.heart.SetCurrentMode(this.Area.Mode, mode.HeartGem);
      this.strawberries.Visible = (mode.TotalStrawberries > 0 || mode.Completed && this.Area.Mode == AreaMode.Normal && AreaData.Get(this.Area).Mode[0].TotalStrawberries > 0) && !AreaData.Get(this.Area).Interlude;
      this.strawberries.Amount = mode.TotalStrawberries;
      this.strawberries.OutOf = this.Data.Mode[0].TotalStrawberries;
      this.strawberries.ShowOutOf = mode.Completed && this.Area.Mode == AreaMode.Normal;
      this.strawberries.Golden = this.Area.Mode != 0;
      if (!wiggle)
        return;
      if (this.strawberries.Visible && (!overrideStrawberryWiggle.HasValue || overrideStrawberryWiggle.Value))
        this.strawberries.Wiggle();
      if (this.heart.Visible && (!overrideHeartWiggle.HasValue || overrideHeartWiggle.Value))
        this.heart.Wiggle();
      if (!this.deaths.Visible || overrideDeathWiggle.HasValue && !overrideDeathWiggle.Value)
        return;
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

    private Vector2 Approach(Vector2 from, Vector2 to, bool snap) => snap ? to : (from += (to - from) * (1f - (float) Math.Pow(1.0 / 1000.0, (double) Engine.DeltaTime)));

    private IEnumerator IncrementStatsDisplay(
      AreaModeStats modeStats,
      AreaModeStats newModeStats,
      bool doHeartGem,
      bool doStrawberries,
      bool doDeaths,
      bool doRemixUnlock)
    {
      OuiChapterPanel ouiChapterPanel = this;
      if (doHeartGem)
      {
        Audio.Play("event:/ui/postgame/crystal_heart");
        ouiChapterPanel.heart.Visible = true;
        ouiChapterPanel.heart.SetCurrentMode(ouiChapterPanel.Area.Mode, true);
        ouiChapterPanel.heart.Appear(ouiChapterPanel.Area.Mode);
        yield return (object) 1.8f;
      }
      if (doStrawberries)
      {
        ouiChapterPanel.strawberries.CanWiggle = true;
        ouiChapterPanel.strawberries.Visible = true;
        while (newModeStats.TotalStrawberries > modeStats.TotalStrawberries)
        {
          int num = newModeStats.TotalStrawberries - modeStats.TotalStrawberries;
          if (num < 3)
            yield return (object) 0.3f;
          else if (num < 8)
          {
            yield return (object) 0.2f;
          }
          else
          {
            yield return (object) 0.1f;
            ++modeStats.TotalStrawberries;
          }
          ++modeStats.TotalStrawberries;
          ouiChapterPanel.strawberries.Amount = modeStats.TotalStrawberries;
          Input.Rumble(RumbleStrength.Light, RumbleLength.Short);
        }
        ouiChapterPanel.strawberries.CanWiggle = false;
        yield return (object) 0.5f;
        if (newModeStats.Completed && !modeStats.Completed && ouiChapterPanel.Area.Mode == AreaMode.Normal)
        {
          yield return (object) 0.25f;
          Audio.Play(ouiChapterPanel.strawberries.Amount >= ouiChapterPanel.Data.Mode[0].TotalStrawberries ? "event:/ui/postgame/strawberry_total_all" : "event:/ui/postgame/strawberry_total");
          ouiChapterPanel.strawberries.OutOf = ouiChapterPanel.Data.Mode[0].TotalStrawberries;
          ouiChapterPanel.strawberries.ShowOutOf = true;
          ouiChapterPanel.strawberries.Wiggle();
          modeStats.Completed = true;
          Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
          yield return (object) 0.6f;
        }
      }
      if (doDeaths)
      {
        Audio.Play("event:/ui/postgame/death_appear");
        ouiChapterPanel.deaths.CanWiggle = true;
        ouiChapterPanel.deaths.Visible = true;
        while (newModeStats.Deaths > modeStats.Deaths)
        {
          int add;
          yield return (object) ouiChapterPanel.HandleDeathTick(modeStats.Deaths, newModeStats.Deaths, out add);
          modeStats.Deaths += add;
          ouiChapterPanel.deaths.Amount = modeStats.Deaths;
          if (modeStats.Deaths >= newModeStats.Deaths)
            Audio.Play("event:/ui/postgame/death_final");
          else
            Audio.Play("event:/ui/postgame/death_count");
          Input.Rumble(RumbleStrength.Light, RumbleLength.Short);
        }
        yield return (object) 0.8f;
        ouiChapterPanel.deaths.CanWiggle = false;
      }
      if (doRemixUnlock)
      {
        ouiChapterPanel.bSideUnlockSfx = Audio.Play("event:/ui/postgame/unlock_bside");
        OuiChapterPanel.Option o = ouiChapterPanel.AddRemixButton();
        o.Appear = 0.0f;
        o.IconEase = 0.0f;
        o.Appeared = true;
        yield return (object) 0.5f;
        ouiChapterPanel.spotlightPosition = o.GetRenderPosition(ouiChapterPanel.OptionsRenderPosition);
        float t;
        for (t = 0.0f; (double) t < 1.0; t += Engine.DeltaTime / 0.5f)
        {
          ouiChapterPanel.spotlightAlpha = t * 0.9f;
          ouiChapterPanel.spotlightRadius = 128f * Ease.CubeOut(t);
          yield return (object) null;
        }
        yield return (object) 0.3f;
        while ((double) (o.IconEase += Engine.DeltaTime * 2f) < 1.0)
          yield return (object) null;
        o.IconEase = 1f;
        ouiChapterPanel.modeAppearWiggler.Start();
        Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
        ouiChapterPanel.remixUnlockText = new AreaCompleteTitle(ouiChapterPanel.spotlightPosition + new Vector2(0.0f, 80f), Dialog.Clean("OVERWORLD_REMIX_UNLOCKED"), 1f);
        ouiChapterPanel.remixUnlockText.Tag = (int) Tags.HUD;
        ouiChapterPanel.Overworld.Add((Entity) ouiChapterPanel.remixUnlockText);
        yield return (object) 1.5f;
        for (t = 0.0f; (double) t < 1.0; t += Engine.DeltaTime / 0.5f)
        {
          ouiChapterPanel.spotlightAlpha = (float) ((1.0 - (double) t) * 0.5);
          ouiChapterPanel.spotlightRadius = (float) (128.0 + 128.0 * (double) Ease.CubeOut(t));
          ouiChapterPanel.remixUnlockText.Alpha = 1f - Ease.CubeOut(t);
          yield return (object) null;
        }
        ouiChapterPanel.remixUnlockText.RemoveSelf();
        ouiChapterPanel.remixUnlockText = (AreaCompleteTitle) null;
        o.Appeared = false;
        o = (OuiChapterPanel.Option) null;
      }
    }

    public IEnumerator IncrementStats(bool shouldAdvance)
    {
      OuiChapterPanel ouiChapterPanel = this;
      ouiChapterPanel.Focused = false;
      ouiChapterPanel.Overworld.ShowInputUI = false;
      if (ouiChapterPanel.Data.Interlude)
      {
        if (shouldAdvance && ouiChapterPanel.OverworldStartMode == Overworld.StartMode.AreaComplete)
        {
          yield return (object) 1.2f;
          ouiChapterPanel.Overworld.Goto<OuiChapterSelect>().AdvanceToNext();
        }
        else
          ouiChapterPanel.Focused = true;
        yield return (object) null;
      }
      else
      {
        AreaData data = ouiChapterPanel.Data;
        AreaStats displayedStats = ouiChapterPanel.DisplayedStats;
        AreaStats area = SaveData.Instance.Areas[data.ID];
        AreaModeStats modeStats = displayedStats.Modes[(int) ouiChapterPanel.Area.Mode];
        AreaModeStats newModeStats = area.Modes[(int) ouiChapterPanel.Area.Mode];
        bool doStrawberries = newModeStats.TotalStrawberries > modeStats.TotalStrawberries;
        bool doHeartGem = newModeStats.HeartGem && !modeStats.HeartGem;
        bool doDeaths = newModeStats.Deaths > modeStats.Deaths && (ouiChapterPanel.Area.Mode != AreaMode.Normal || newModeStats.Completed);
        bool doRemixUnlock = ouiChapterPanel.Area.Mode == AreaMode.Normal && data.HasMode(AreaMode.BSide) && area.Cassette && !displayedStats.Cassette;
        if (doStrawberries | doHeartGem | doDeaths | doRemixUnlock)
          yield return (object) 0.8f;
        bool skipped = false;
        Coroutine routine = new Coroutine(ouiChapterPanel.IncrementStatsDisplay(modeStats, newModeStats, doHeartGem, doStrawberries, doDeaths, doRemixUnlock));
        ouiChapterPanel.Add((Component) routine);
        yield return (object) null;
        while (!routine.Finished)
        {
          if (MInput.GamePads[0].Pressed(Buttons.Start) || MInput.Keyboard.Pressed(Keys.Enter))
          {
            routine.Active = false;
            routine.RemoveSelf();
            skipped = true;
            Audio.Stop(ouiChapterPanel.bSideUnlockSfx);
            Audio.Play("event:/new_content/ui/skip_all");
            break;
          }
          yield return (object) null;
        }
        if (skipped & doRemixUnlock)
        {
          ouiChapterPanel.spotlightAlpha = 0.0f;
          ouiChapterPanel.spotlightRadius = 0.0f;
          if (ouiChapterPanel.remixUnlockText != null)
          {
            ouiChapterPanel.remixUnlockText.RemoveSelf();
            ouiChapterPanel.remixUnlockText = (AreaCompleteTitle) null;
          }
          if (ouiChapterPanel.modes.Count <= 1 || ouiChapterPanel.modes[1].ID != "B")
          {
            ouiChapterPanel.AddRemixButton();
          }
          else
          {
            OuiChapterPanel.Option mode = ouiChapterPanel.modes[1];
            mode.IconEase = 1f;
            mode.Appear = 1f;
            mode.Appeared = false;
          }
        }
        ouiChapterPanel.DisplayedStats = ouiChapterPanel.RealStats;
        if (skipped)
        {
          doStrawberries = doStrawberries && modeStats.TotalStrawberries != newModeStats.TotalStrawberries;
          doDeaths &= modeStats.Deaths != newModeStats.Deaths;
          doHeartGem = doHeartGem && !ouiChapterPanel.heart.Visible;
          ouiChapterPanel.UpdateStats(overrideStrawberryWiggle: new bool?(doStrawberries), overrideDeathWiggle: new bool?(doDeaths), overrideHeartWiggle: new bool?(doHeartGem));
        }
        yield return (object) null;
        routine = (Coroutine) null;
        if (shouldAdvance && ouiChapterPanel.OverworldStartMode == Overworld.StartMode.AreaComplete)
        {
          if (!doDeaths && !doStrawberries && !doHeartGem || Settings.Instance.SpeedrunClock != SpeedrunType.Off)
            yield return (object) 1.2f;
          ouiChapterPanel.Overworld.Goto<OuiChapterSelect>().AdvanceToNext();
        }
        else
        {
          ouiChapterPanel.Focused = true;
          ouiChapterPanel.Overworld.ShowInputUI = true;
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

    public static string GetCheckpointPreviewName(AreaKey area, string level) => level == null ? area.ToString() : area.ToString() + "_" + level;

    private MTexture GetCheckpointPreview(AreaKey area, string level)
    {
      string checkpointPreviewName = OuiChapterPanel.GetCheckpointPreviewName(area, level);
      return MTN.Checkpoints.Has(checkpointPreviewName) ? MTN.Checkpoints[checkpointPreviewName] : (MTexture) null;
    }

    private class Option
    {
      public string Label;
      public string ID;
      public MTexture Icon;
      public MTexture Bg = GFX.Gui["areaselect/tab"];
      public Color BgColor = Calc.HexToColor("3c6180");
      public float Pop;
      public bool Large = true;
      public int Siblings;
      public float Slide;
      public float Appear = 1f;
      public float IconEase = 1f;
      public bool Appeared;
      public float Faded;
      public float CheckpointSlideOut;
      public string CheckpointLevelName;
      public float CheckpointRotation;
      public Vector2 CheckpointOffset;

      public float Scale => this.Siblings < 5 ? 1f : 0.8f;

      public bool OnTopOfUI => (double) this.Pop > 0.5;

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
        float num = (this.Large ? 170f : 130f) * this.Scale;
        if (this.Siblings > 0 && (double) num * (double) this.Siblings > 750.0)
          num = (float) (750 / this.Siblings);
        Vector2 renderPosition = center + new Vector2(this.Slide * num, (float) (Math.Sin((double) this.Pop * 3.1415927410125732) * 70.0 - (double) this.Pop * 12.0));
        renderPosition.Y += (float) ((1.0 - (double) Ease.CubeOut(this.Appear)) * -200.0);
        renderPosition.Y -= (float) ((1.0 - (double) this.Scale) * 80.0);
        return renderPosition;
      }

      public void Render(Vector2 center, bool selected, Wiggler wiggler, Wiggler appearWiggler)
      {
        float num1 = (float) ((double) this.Scale + (selected ? (double) wiggler.Value * 0.25 : 0.0) + (this.Appeared ? (double) appearWiggler.Value * 0.25 : 0.0));
        Vector2 renderPosition = this.GetRenderPosition(center);
        Color color1 = Color.Lerp(this.BgColor, Color.Black, (float) ((1.0 - (double) this.Pop) * 0.6000000238418579));
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
