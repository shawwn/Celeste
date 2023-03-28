// Decompiled with JetBrains decompiler
// Type: Celeste.CS10_Ending
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Celeste
{
  public class CS10_Ending : CutsceneEntity
  {
    private const int FPS = 12;
    private const float DELAY = 0.083333336f;
    private Atlas Atlas;
    private List<MTexture> Frames;
    private int frame;
    private float fade = 1f;
    private float zoom = 1f;
    private float computerFade;
    private Coroutine talkingLoop;
    private Vector2 center = Celeste.TargetCenter;
    private Coroutine cutscene;
    private Color fadeColor = Color.White;
    private Monocle.Image attachment;
    private Monocle.Image cursor;
    private Monocle.Image ok;
    private Monocle.Image picture;
    private const float PictureIdleScale = 0.9f;
    private float speedrunTimerEase;
    private string speedrunTimerChapterString;
    private string speedrunTimerFileString;
    private string chapterSpeedrunText = Dialog.Get("OPTIONS_SPEEDRUN_CHAPTER") + ":";
    private string version = Celeste.Instance.Version.ToString();
    private bool showTimer;
    private EventInstance endAmbience;
    private EventInstance cinIntro;
    private bool counting;
    private float timer;

    public CS10_Ending(Player player)
      : base(false, true)
    {
      this.Tag = (int) Tags.HUD;
      player.StateMachine.State = 11;
      player.DummyAutoAnimate = false;
      player.Sprite.Rate = 0.0f;
      this.RemoveOnSkipped = false;
      this.Add((Component) new LevelEndingHook((Action) (() => Audio.Stop(this.cinIntro))));
    }

    public override void Awake(Scene scene)
    {
      base.Awake(scene);
      Level level = scene as Level;
      level.TimerStopped = true;
      level.TimerHidden = true;
      level.SaveQuitDisabled = true;
      level.PauseLock = true;
      level.AllowHudHide = false;
    }

    public override void OnBegin(Level level)
    {
      Audio.SetAmbience((string) null);
      level.AutoSave();
      this.speedrunTimerChapterString = TimeSpan.FromTicks(level.Session.Time).ShortGameplayFormat();
      this.speedrunTimerFileString = Dialog.FileTime(SaveData.Instance.Time);
      SpeedrunTimerDisplay.CalculateBaseSizes();
      this.Add((Component) (this.cutscene = new Coroutine(this.Cutscene(level))));
    }

    private IEnumerator Cutscene(Level level)
    {
      CS10_Ending cs10Ending = this;
      if (level.Wipe != null)
        level.Wipe.Cancel();
      while (level.IsAutoSaving())
        yield return (object) null;
      yield return (object) 1f;
      cs10Ending.Atlas = Atlas.FromAtlas(Path.Combine("Graphics", "Atlases", "Farewell"), Atlas.AtlasDataFormat.PackerNoAtlas);
      cs10Ending.Frames = cs10Ending.Atlas.GetAtlasSubtextures("");
      cs10Ending.Add((Component) (cs10Ending.attachment = new Monocle.Image(cs10Ending.Atlas["21-window"])));
      cs10Ending.Add((Component) (cs10Ending.picture = new Monocle.Image(cs10Ending.Atlas["21-picture"])));
      cs10Ending.Add((Component) (cs10Ending.ok = new Monocle.Image(cs10Ending.Atlas["21-button"])));
      cs10Ending.Add((Component) (cs10Ending.cursor = new Monocle.Image(cs10Ending.Atlas["21-cursor"])));
      cs10Ending.attachment.Visible = false;
      cs10Ending.picture.Visible = false;
      cs10Ending.ok.Visible = false;
      cs10Ending.cursor.Visible = false;
      level.PauseLock = false;
      yield return (object) 2f;
      cs10Ending.cinIntro = Audio.Play("event:/new_content/music/lvl10/cinematic/end_intro");
      Audio.SetAmbience((string) null);
      cs10Ending.counting = true;
      cs10Ending.Add((Component) new Coroutine(cs10Ending.Fade(1f, 0.0f, 4f)));
      cs10Ending.Add((Component) new Coroutine(cs10Ending.Zoom(1.38f, 1.2f, 4f)));
      yield return (object) cs10Ending.Loop("0", 2f);
      Input.Rumble(RumbleStrength.Climb, RumbleLength.TwoSeconds);
      yield return (object) cs10Ending.Loop("0,1,1,0,0,1,1,0*8", 2f);
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Short);
      Audio.SetMusic("event:/new_content/music/lvl10/cinematic/end", allowFadeOut: false);
      cs10Ending.endAmbience = Audio.Play("event:/new_content/env/10_endscene");
      cs10Ending.Add((Component) new Coroutine(cs10Ending.Zoom(1.2f, 1.05f, 0.06f, Ease.CubeOut)));
      yield return (object) cs10Ending.Play("2-7");
      yield return (object) cs10Ending.Loop("7", 1.5f);
      yield return (object) cs10Ending.Play("8-10,10*20");
      List<int> frameData = cs10Ending.GetFrameData("10-13,13*16,14*28,14-17,14*24");
      float duration = (float) (frameData.Count + 3) * 0.083333336f;
      cs10Ending.fadeColor = Color.Black;
      cs10Ending.Add((Component) new Coroutine(cs10Ending.Zoom(1.05f, 1f, duration, Ease.Linear)));
      cs10Ending.Add((Component) new Coroutine(cs10Ending.Fade(0.0f, 1f, duration * 0.1f, duration * 0.85f)));
      cs10Ending.Add((Component) Alarm.Create(Alarm.AlarmMode.Oneshot, (Action) (() => Audio.Play("event:/new_content/game/10_farewell/endscene_dial_theo")), duration, true));
      yield return (object) cs10Ending.Play(frameData);
      cs10Ending.frame = 18;
      cs10Ending.fade = 1f;
      yield return (object) 0.5f;
      yield return (object) cs10Ending.Fade(1f, 0.0f, 1.2f);
      cs10Ending.Add((Component) (cs10Ending.talkingLoop = new Coroutine(cs10Ending.Loop("18*24,19,19,18*6,20,20"))));
      yield return (object) 1f;
      yield return (object) Textbox.Say("CH9_END_CINEMATIC", new Func<IEnumerator>(cs10Ending.ShowPicture));
      Audio.SetMusicParam("end", 1f);
      Audio.Play("event:/new_content/game/10_farewell/endscene_photo_zoom");
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime / 4f)
      {
        Audio.SetParameter(cs10Ending.endAmbience, "fade", 1f - p);
        cs10Ending.computerFade = p;
        cs10Ending.picture.Scale = Vector2.One * (float) (0.8999999761581421 + 0.10000002384185791 * (double) p);
        yield return (object) null;
      }
      cs10Ending.EndCutscene(level, false);
    }

    private IEnumerator ShowPicture()
    {
      CS10_Ending cs10Ending = this;
      cs10Ending.center = new Vector2(1230f, 312f);
      cs10Ending.Add((Component) new Coroutine(cs10Ending.Fade(0.0f, 1f, 0.25f)));
      cs10Ending.Add((Component) new Coroutine(cs10Ending.Zoom(1f, 1.1f, 0.25f)));
      yield return (object) 0.25f;
      if (cs10Ending.talkingLoop != null)
        cs10Ending.talkingLoop.RemoveSelf();
      cs10Ending.talkingLoop = (Coroutine) null;
      yield return (object) null;
      cs10Ending.frame = 21;
      cs10Ending.cursor.Visible = true;
      cs10Ending.center = Celeste.TargetCenter;
      cs10Ending.Add((Component) new Coroutine(cs10Ending.Fade(1f, 0.0f, 0.25f)));
      cs10Ending.Add((Component) new Coroutine(cs10Ending.Zoom(1.1f, 1f, 0.25f)));
      yield return (object) 0.25f;
      Audio.Play("event:/new_content/game/10_farewell/endscene_attachment_notify");
      cs10Ending.attachment.Origin = Celeste.TargetCenter;
      cs10Ending.attachment.Position = Celeste.TargetCenter;
      cs10Ending.attachment.Visible = true;
      cs10Ending.attachment.Scale = Vector2.Zero;
      float p;
      for (p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime / 0.3f)
      {
        cs10Ending.attachment.Scale.Y = (float) (0.25 + 0.75 * (double) Ease.BigBackOut(p));
        cs10Ending.attachment.Scale.X = (float) (1.5 - 0.5 * (double) Ease.BigBackOut(p));
        yield return (object) null;
      }
      yield return (object) 0.25f;
      cs10Ending.ok.Position = new Vector2(1208f, 620f);
      cs10Ending.ok.Origin = cs10Ending.ok.Position;
      cs10Ending.ok.Visible = true;
      for (p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime / 0.25f)
      {
        cs10Ending.ok.Scale.Y = (float) (0.25 + 0.75 * (double) Ease.BigBackOut(p));
        cs10Ending.ok.Scale.X = (float) (1.5 - 0.5 * (double) Ease.BigBackOut(p));
        yield return (object) null;
      }
      yield return (object) 0.8f;
      Vector2 from = cs10Ending.cursor.Position;
      Vector2 to = from + new Vector2(-160f, -190f);
      for (p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime / 0.5f)
      {
        cs10Ending.cursor.Position = from + (to - from) * Ease.CubeInOut(p);
        yield return (object) null;
      }
      yield return (object) 0.2f;
      from = new Vector2();
      to = new Vector2();
      Audio.Play("event:/new_content/game/10_farewell/endscene_attachment_click");
      for (p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime / 0.25f)
      {
        cs10Ending.ok.Scale.Y = 1f - Ease.BigBackIn(p);
        cs10Ending.ok.Scale.X = 1f - Ease.BigBackIn(p);
        yield return (object) null;
      }
      cs10Ending.ok.Visible = false;
      yield return (object) 0.1f;
      cs10Ending.picture.Origin = Celeste.TargetCenter;
      cs10Ending.picture.Position = Celeste.TargetCenter;
      cs10Ending.picture.Visible = true;
      for (p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime / 0.4f)
      {
        cs10Ending.picture.Scale.Y = (float) ((0.8999999761581421 + 0.10000000149011612 * (double) Ease.BigBackOut(p)) * 0.8999999761581421);
        cs10Ending.picture.Scale.X = (float) ((1.100000023841858 - 0.10000000149011612 * (double) Ease.BigBackOut(p)) * 0.8999999761581421);
        cs10Ending.picture.Position = Celeste.TargetCenter + Vector2.UnitY * 120f * (1f - Ease.CubeOut(p));
        cs10Ending.picture.Color = Color.White * p;
        yield return (object) null;
      }
      cs10Ending.picture.Position = Celeste.TargetCenter;
      cs10Ending.attachment.Visible = false;
      to = cs10Ending.cursor.Position;
      from = new Vector2(120f, 30f);
      for (p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime / 0.5f)
      {
        cs10Ending.cursor.Position = to + (from - to) * Ease.CubeInOut(p);
        yield return (object) null;
      }
      cs10Ending.cursor.Visible = false;
      to = new Vector2();
      from = new Vector2();
      yield return (object) 2f;
    }

    public override void OnEnd(Level level)
    {
      ScreenWipe.WipeColor = Color.Black;
      if ((HandleBase) Audio.CurrentMusicEventInstance == (HandleBase) null)
        Audio.SetMusic("event:/new_content/music/lvl10/cinematic/end");
      Audio.SetMusicParam("end", 1f);
      this.frame = 21;
      this.zoom = 1f;
      this.fade = 0.0f;
      this.fadeColor = Color.Black;
      this.center = Celeste.TargetCenter;
      this.picture.Scale = Vector2.One;
      this.picture.Visible = true;
      this.picture.Position = Celeste.TargetCenter;
      this.picture.Origin = Celeste.TargetCenter;
      this.computerFade = 1f;
      this.attachment.Visible = false;
      this.cursor.Visible = false;
      this.ok.Visible = false;
      Audio.Stop(this.cinIntro);
      this.cinIntro = (EventInstance) null;
      Audio.Stop(this.endAmbience);
      this.endAmbience = (EventInstance) null;
      List<Coroutine> coroutineList = new List<Coroutine>();
      foreach (Coroutine coroutine in this.Components.GetAll<Coroutine>())
        coroutineList.Add(coroutine);
      foreach (Component component in coroutineList)
        component.RemoveSelf();
      this.Scene.Entities.FindFirst<Textbox>()?.RemoveSelf();
      this.Level.InCutscene = true;
      this.Level.PauseLock = true;
      this.Level.TimerHidden = true;
      this.Add((Component) new Coroutine(this.EndingRoutine()));
    }

    private IEnumerator EndingRoutine()
    {
      CS10_Ending cs10Ending = this;
      cs10Ending.Level.InCutscene = true;
      cs10Ending.Level.PauseLock = true;
      cs10Ending.Level.TimerHidden = true;
      yield return (object) 0.5f;
      if (Settings.Instance.SpeedrunClock != SpeedrunType.Off)
        cs10Ending.showTimer = true;
      while (!Input.MenuConfirm.Pressed)
        yield return (object) null;
      Audio.Play("event:/new_content/game/10_farewell/endscene_final_input");
      cs10Ending.showTimer = false;
      cs10Ending.Add((Component) new Coroutine(cs10Ending.Zoom(1f, 0.75f, 5f, Ease.CubeIn)));
      cs10Ending.Add((Component) new Coroutine(cs10Ending.Fade(0.0f, 1f, 5f)));
      yield return (object) 4f;
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime / 3f)
      {
        Audio.SetMusicParam("fade", 1f - p);
        yield return (object) null;
      }
      Audio.SetMusic((string) null);
      yield return (object) 1f;
      if (cs10Ending.Atlas != null)
        cs10Ending.Atlas.Dispose();
      cs10Ending.Atlas = (Atlas) null;
      cs10Ending.Level.CompleteArea(false, true, true);
    }

    public override void Update()
    {
      if (this.counting)
        this.timer += Engine.DeltaTime;
      this.speedrunTimerEase = Calc.Approach(this.speedrunTimerEase, this.showTimer ? 1f : 0.0f, Engine.DeltaTime * 4f);
      base.Update();
    }

    public override void Render()
    {
      Draw.Rect(-100f, -100f, 2120f, 1280f, Color.Black);
      if (this.Atlas != null && this.Frames != null && this.frame < this.Frames.Count)
      {
        MTexture frame = this.Frames[this.frame];
        MTexture linkedTexture = this.Atlas.GetLinkedTexture(frame.AtlasPath);
        linkedTexture?.DrawJustified(this.center, new Vector2(this.center.X / (float) linkedTexture.Width, this.center.Y / (float) linkedTexture.Height), Color.White, this.zoom);
        frame.DrawJustified(this.center, new Vector2(this.center.X / (float) frame.Width, this.center.Y / (float) frame.Height), Color.White, this.zoom);
        if ((double) this.computerFade > 0.0)
          Draw.Rect(0.0f, 0.0f, 1920f, 1080f, Color.Black * this.computerFade);
        base.Render();
        AreaComplete.Info(this.speedrunTimerEase, this.speedrunTimerChapterString, this.speedrunTimerFileString, this.chapterSpeedrunText, this.version);
      }
      Draw.Rect(0.0f, 0.0f, 1920f, 1080f, this.fadeColor * this.fade);
      if (!(this.Scene as Level).Paused)
        return;
      Draw.Rect(0.0f, 0.0f, 1920f, 1080f, Color.Black * 0.5f);
    }

    private List<int> GetFrameData(string data)
    {
      List<int> frameData = new List<int>();
      string[] strArray1 = data.Split(',');
      for (int index1 = 0; index1 < strArray1.Length; ++index1)
      {
        if (strArray1[index1].Contains<char>('*'))
        {
          string[] strArray2 = strArray1[index1].Split('*');
          int num1 = int.Parse(strArray2[0]);
          int num2 = int.Parse(strArray2[1]);
          for (int index2 = 0; index2 < num2; ++index2)
            frameData.Add(num1);
        }
        else if (strArray1[index1].Contains<char>('-'))
        {
          string[] strArray3 = strArray1[index1].Split('-');
          int num3 = int.Parse(strArray3[0]);
          int num4 = int.Parse(strArray3[1]);
          for (int index3 = num3; index3 <= num4; ++index3)
            frameData.Add(index3);
        }
        else
          frameData.Add(int.Parse(strArray1[index1]));
      }
      return frameData;
    }

    private IEnumerator Zoom(float from, float to, float duration, Ease.Easer ease = null)
    {
      if (ease == null)
        ease = Ease.Linear;
      this.zoom = from;
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime / duration)
      {
        this.zoom = from + (to - from) * ease(p);
        if (this.picture != null)
          this.picture.Scale = Vector2.One * this.zoom;
        yield return (object) null;
      }
      this.zoom = to;
    }

    private IEnumerator Play(string data) => this.Play(this.GetFrameData(data));

    private IEnumerator Play(List<int> frames)
    {
      for (int i = 0; i < frames.Count; ++i)
      {
        this.frame = frames[i];
        yield return (object) 0.083333336f;
      }
    }

    private IEnumerator Loop(string data, float duration = -1f)
    {
      List<int> frames = this.GetFrameData(data);
      float time = 0.0f;
      while ((double) time < (double) duration || (double) duration < 0.0)
      {
        this.frame = frames[(int) ((double) time / 0.0833333358168602) % frames.Count];
        time += Engine.DeltaTime;
        yield return (object) null;
      }
    }

    private IEnumerator Fade(float from, float to, float duration, float delay = 0.0f)
    {
      this.fade = from;
      yield return (object) delay;
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime / duration)
      {
        this.fade = from + (to - from) * p;
        yield return (object) null;
      }
      this.fade = to;
    }
  }
}
