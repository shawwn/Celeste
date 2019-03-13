// Decompiled with JetBrains decompiler
// Type: Celeste.AreaComplete
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Monocle;
using System;
using System.Xml;

namespace Celeste
{
  public class AreaComplete : Scene
  {
    private bool canConfirm = true;
    private float speedrunTimerDelay = 1.1f;
    private string chapterSpeedrunText = Dialog.Get("OPTIONS_SPEEDRUN_CHAPTER", (Language) null) + ":";
    public Session Session;
    private bool finishedSlide;
    private HiresSnow snow;
    private float speedrunTimerEase;
    private string speedrunTimerChapterString;
    private string speedrunTimerFileString;
    private AreaCompleteTitle title;
    private CompleteRenderer complete;
    private string version;

    public AreaComplete(Session session, XmlElement xml, Atlas atlas, HiresSnow snow)
    {
      this.Session = session;
      this.version = Celeste.Instance.Version.ToString();
      if (session.Area.ID != 7)
      {
        string text = Dialog.Clean("areacomplete_" + (object) session.Area.Mode + (session.FullClear ? (object) "_fullclear" : (object) ""), (Language) null);
        Vector2 origin = new Vector2(960f, 200f);
        float scale = Math.Min(1600f / ActiveFont.Measure(text).X, 3f);
        this.title = new AreaCompleteTitle(origin, text, scale);
      }
      this.Add((Monocle.Renderer) (this.complete = new CompleteRenderer(xml, atlas, 1f, (Action) (() => this.finishedSlide = true))));
      if (this.title != null)
        this.complete.RenderUI = (Action<Vector2>) (v => this.title.DrawLineUI());
      this.complete.RenderPostUI = new Action(this.RenderUI);
      this.speedrunTimerChapterString = TimeSpan.FromTicks(this.Session.Time).ShortGameplayFormat();
      this.speedrunTimerFileString = Dialog.FileTime(SaveData.Instance.Time);
      SpeedrunTimerDisplay.CalculateBaseSizes();
      this.Add((Monocle.Renderer) (this.snow = snow));
      this.RendererList.UpdateLists();
      AreaKey area = session.Area;
      if (area.Mode != AreaMode.Normal)
        return;
      if (area.ID == 1)
        Achievements.Register(Achievement.CH1);
      else if (area.ID == 2)
        Achievements.Register(Achievement.CH2);
      else if (area.ID == 3)
        Achievements.Register(Achievement.CH3);
      else if (area.ID == 4)
        Achievements.Register(Achievement.CH4);
      else if (area.ID == 5)
        Achievements.Register(Achievement.CH5);
      else if (area.ID == 6)
        Achievements.Register(Achievement.CH6);
      else if (area.ID == 7)
        Achievements.Register(Achievement.CH7);
    }

    public override void End()
    {
      base.End();
      this.complete.Dispose();
    }

    public override void Update()
    {
      base.Update();
      if (Input.MenuConfirm.Pressed && this.finishedSlide && this.canConfirm)
      {
        this.canConfirm = false;
        if (this.Session.Area.ID == 7 && this.Session.Area.Mode == AreaMode.Normal)
        {
          FadeWipe fadeWipe = new FadeWipe((Scene) this, false, (Action) (() =>
          {
            this.Session.RespawnPoint = new Vector2?();
            this.Session.FirstLevel = false;
            this.Session.Level = "credits-summit";
            this.Session.Audio.Music.Event = "event:/music/lvl8/main";
            this.Session.Audio.Apply();
            Engine.Scene = (Scene) new LevelLoader(this.Session, new Vector2?())
            {
              PlayerIntroTypeOverride = new Player.IntroTypes?(Player.IntroTypes.None),
              Level = {
                (Entity) new CS07_Credits()
              }
            };
          }));
        }
        else
        {
          HiresSnow outSnow = new HiresSnow(0.45f)
          {
            Alpha = 0.0f
          };
          outSnow.AttachAlphaTo = (ScreenWipe) new FadeWipe((Scene) this, false, (Action) (() => Engine.Scene = (Scene) new OverworldLoader(Overworld.StartMode.AreaComplete, outSnow)));
          this.Add((Monocle.Renderer) outSnow);
        }
      }
      this.snow.Alpha = Calc.Approach(this.snow.Alpha, 0.0f, Engine.DeltaTime * 0.5f);
      this.snow.Direction.Y = Calc.Approach(this.snow.Direction.Y, 1f, Engine.DeltaTime * 24f);
      this.speedrunTimerDelay -= Engine.DeltaTime;
      if ((double) this.speedrunTimerDelay <= 0.0)
        this.speedrunTimerEase = Calc.Approach(this.speedrunTimerEase, 1f, Engine.DeltaTime * 2f);
      if (this.title != null)
        this.title.Update();
      if (Celeste.PlayMode != Celeste.PlayModes.Debug)
        return;
      if (MInput.Keyboard.Pressed(Keys.F2))
      {
        Celeste.ReloadAssets(false, true, false, new AreaKey?());
        Engine.Scene = (Scene) new LevelExit(LevelExit.Mode.Completed, this.Session, (HiresSnow) null);
      }
      else if (MInput.Keyboard.Pressed(Keys.F3))
      {
        Celeste.ReloadAssets(false, true, true, new AreaKey?());
        Engine.Scene = (Scene) new LevelExit(LevelExit.Mode.Completed, this.Session, (HiresSnow) null);
      }
    }

    private void RenderUI()
    {
      this.Entities.Render();
      if ((double) this.speedrunTimerEase > 0.0 && (uint) Settings.Instance.SpeedrunClock > 0U)
      {
        Vector2 position = new Vector2((float) (80.0 - 300.0 * (1.0 - (double) Ease.CubeOut(this.speedrunTimerEase))), 1000f);
        if (Settings.Instance.SpeedrunClock == SpeedrunType.Chapter)
        {
          SpeedrunTimerDisplay.DrawTime(position, this.speedrunTimerChapterString, 1f, true, false, false, 1f);
        }
        else
        {
          position.Y -= 16f;
          SpeedrunTimerDisplay.DrawTime(position, this.speedrunTimerFileString, 1f, true, false, false, 1f);
          ActiveFont.DrawOutline(this.chapterSpeedrunText, position + new Vector2(0.0f, 40f), new Vector2(0.0f, 1f), Vector2.One * 0.6f, Color.White, 2f, Color.Black);
          SpeedrunTimerDisplay.DrawTime(position + new Vector2((float) ((double) ActiveFont.Measure(this.chapterSpeedrunText).X * 0.600000023841858 + 8.0), 40f), this.speedrunTimerChapterString, 0.6f, true, false, false, 1f);
        }
        AreaComplete.VersionNumberAndVariants(this.version, this.speedrunTimerEase, 1f);
      }
      if (!this.complete.HasUI || this.title == null)
        return;
      this.title.Render();
    }

    public static void VersionNumberAndVariants(string version, float ease, float alpha)
    {
      Vector2 position = new Vector2((float) (1820.0 + 300.0 * (1.0 - (double) Ease.CubeOut(ease))), 1020f);
      if (SaveData.Instance.AssistMode || SaveData.Instance.VariantMode)
      {
        MTexture mtexture = GFX.Gui[SaveData.Instance.AssistMode ? "cs_assistmode" : "cs_variantmode"];
        position.Y -= 32f;
        mtexture.DrawJustified(position + new Vector2(0.0f, -8f), new Vector2(0.5f, 1f), Color.White, 0.6f);
        ActiveFont.DrawOutline(version, position, new Vector2(0.5f, 0.0f), Vector2.One * 0.5f, Color.White, 2f, Color.Black);
      }
      else
        ActiveFont.DrawOutline(version, position, new Vector2(0.5f, 0.5f), Vector2.One * 0.5f, Color.White, 2f, Color.Black);
    }
  }
}

