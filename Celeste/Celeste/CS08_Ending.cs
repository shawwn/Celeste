// Decompiled with JetBrains decompiler
// Type: Celeste.CS08_Ending
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class CS08_Ending : CutsceneEntity
  {
    private string version = Celeste.Instance.Version.ToString();
    private Player player;
    private NPC08_Granny granny;
    private NPC08_Theo theo;
    private BadelineDummy badeline;
    private Entity oshiro;
    private Monocle.Image vignette;
    private Monocle.Image vignettebg;
    private string endingDialog;
    private float fade;
    private bool showVersion;
    private float versionAlpha;
    private Coroutine cutscene;

    public CS08_Ending()
      : base(false, true)
    {
      this.Tag = (int) Tags.HUD | (int) Tags.PauseUpdate;
      this.RemoveOnSkipped = false;
    }

    public override void OnBegin(Level level)
    {
      level.SaveQuitDisabled = true;
      int totalStrawberries = SaveData.Instance.TotalStrawberries;
      string index;
      if (totalStrawberries < 20)
      {
        index = "final1";
        this.endingDialog = "EP_PIE_DISAPPOINTED";
      }
      else if (totalStrawberries < 50)
      {
        index = "final2";
        this.endingDialog = "EP_PIE_GROSSED_OUT";
      }
      else if (totalStrawberries < 90)
      {
        index = "final3";
        this.endingDialog = "EP_PIE_OKAY";
      }
      else if (totalStrawberries < 150)
      {
        index = "final4";
        this.endingDialog = "EP_PIE_REALLY_GOOD";
      }
      else
      {
        index = "final5";
        this.endingDialog = "EP_PIE_AMAZING";
      }
      this.Add((Component) (this.vignettebg = new Monocle.Image(GFX.Portraits["finalbg"])));
      this.vignettebg.Visible = false;
      this.Add((Component) (this.vignette = new Monocle.Image(GFX.Portraits[index])));
      this.vignette.Visible = false;
      this.vignette.CenterOrigin();
      this.vignette.Position = Celeste.TargetCenter;
      this.Add((Component) (this.cutscene = new Coroutine(this.Cutscene(level), true)));
    }

    private IEnumerator Cutscene(Level level)
    {
      level.ZoomSnap(new Vector2(164f, 120f), 2f);
      level.Wipe.Cancel();
      FadeWipe fadeWipe = new FadeWipe((Scene) level, true, (Action) null);
      while (this.player == null)
      {
        this.granny = level.Entities.FindFirst<NPC08_Granny>();
        this.theo = level.Entities.FindFirst<NPC08_Theo>();
        this.player = level.Tracker.GetEntity<Player>();
        yield return (object) null;
      }
      this.player.StateMachine.State = Player.StDummy;
      yield return (object) 1f;
      yield return (object) this.player.DummyWalkToExact((int) this.player.X + 16, false, 1f);
      yield return (object) 0.25f;
      yield return (object) Textbox.Say("EP_CABIN", new Func<IEnumerator>(this.BadelineEmerges), new Func<IEnumerator>(this.OshiroEnters), new Func<IEnumerator>(this.OshiroSettles), new Func<IEnumerator>(this.MaddyTurns));
      FadeWipe wipe = new FadeWipe((Scene) this.Level, false, (Action) null);
      wipe.Duration = 1.5f;
      yield return (object) wipe.Wait();
      this.fade = 1f;
      yield return (object) Textbox.Say("EP_PIE_START");
      yield return (object) 0.5f;
      this.vignettebg.Visible = true;
      this.vignette.Visible = true;
      this.vignettebg.Color = Color.Black;
      this.vignette.Color = Color.White * 0.0f;
      this.Add((Component) this.vignette);
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime)
      {
        this.vignette.Color = Color.White * Ease.CubeIn(p);
        this.vignette.Scale = Vector2.One * (float) (1.0 + 0.25 * (1.0 - (double) p));
        this.vignette.Rotation = (float) (0.0500000007450581 * (1.0 - (double) p));
        yield return (object) null;
      }
      this.vignette.Color = Color.White;
      this.vignettebg.Color = Color.White;
      yield return (object) 2f;
      float duration1 = 1f;
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime / duration1)
      {
        float e = Ease.CubeOut(p);
        this.vignette.Position = Vector2.Lerp(Celeste.TargetCenter, Celeste.TargetCenter + new Vector2(0.0f, 140f), e);
        this.vignette.Scale = Vector2.One * (float) (0.649999976158142 + 0.349999994039536 * (1.0 - (double) e));
        this.vignette.Rotation = -0.025f * e;
        yield return (object) null;
      }
      yield return (object) Textbox.Say(this.endingDialog);
      yield return (object) 0.25f;
      float duration2 = 2f;
      Vector2 posFrom = this.vignette.Position;
      float rotFrom = this.vignette.Rotation;
      float scaleFrom = this.vignette.Scale.X;
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime / duration2)
      {
        float e = Ease.CubeOut(p);
        this.vignette.Position = Vector2.Lerp(posFrom, Celeste.TargetCenter, e);
        this.vignette.Scale = Vector2.One * MathHelper.Lerp(scaleFrom, 1f, e);
        this.vignette.Rotation = MathHelper.Lerp(rotFrom, 0.0f, e);
        yield return (object) null;
      }
      posFrom = new Vector2();
      this.EndCutscene(level, false);
    }

    public override void OnEnd(Level level)
    {
      this.vignette.Visible = true;
      this.vignette.Color = Color.White;
      this.vignette.Position = Celeste.TargetCenter;
      this.vignette.Scale = Vector2.One;
      this.vignette.Rotation = 0.0f;
      if (this.player != null)
        this.player.Speed = Vector2.Zero;
      Textbox first = this.Scene.Entities.FindFirst<Textbox>();
      if (first != null)
        first.RemoveSelf();
      this.cutscene.RemoveSelf();
      this.Add((Component) new Coroutine(this.EndingRoutine(), true));
    }

    private IEnumerator EndingRoutine()
    {
      this.Level.InCutscene = true;
      this.Level.PauseLock = true;
      yield return (object) 0.5f;
      TimeSpan timespan = TimeSpan.FromTicks(SaveData.Instance.Time);
      int hours = (int) timespan.TotalHours;
      string gameTime = hours.ToString() + timespan.ToString("\\:mm\\:ss\\.fff");
      StrawberriesCounter strawbs = new StrawberriesCounter(true, SaveData.Instance.TotalStrawberries, 175, true);
      DeathsCounter deaths = new DeathsCounter(AreaMode.Normal, true, SaveData.Instance.TotalDeaths, 0);
      CS08_Ending.TimeDisplay time = new CS08_Ending.TimeDisplay(gameTime);
      float timeWidth = SpeedrunTimerDisplay.GetTimeWidth(gameTime, 1f);
      this.Add((Component) strawbs);
      this.Add((Component) deaths);
      this.Add((Component) time);
      Vector2 from = new Vector2(960f, 1180f);
      Vector2 to = new Vector2(960f, 940f);
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime / 0.5f)
      {
        Vector2 lerp = Vector2.Lerp(from, to, Ease.CubeOut(p));
        strawbs.Position = lerp + new Vector2(-170f, 0.0f);
        deaths.Position = lerp + new Vector2(170f, 0.0f);
        time.Position = lerp + new Vector2((float) (-(double) timeWidth / 2.0), 100f);
        yield return (object) null;
        lerp = new Vector2();
      }
      timespan = new TimeSpan();
      gameTime = (string) null;
      strawbs = (StrawberriesCounter) null;
      deaths = (DeathsCounter) null;
      time = (CS08_Ending.TimeDisplay) null;
      from = new Vector2();
      to = new Vector2();
      this.showVersion = true;
      yield return (object) 0.25f;
      while (!Input.MenuConfirm.Pressed)
        yield return (object) null;
      this.showVersion = false;
      yield return (object) 0.25f;
      this.Level.CompleteArea(false, false);
    }

    private IEnumerator MaddyTurns()
    {
      yield return (object) 0.1f;
      this.player.Facing = ToFacing.Convert(-(int) this.player.Facing);
      yield return (object) 0.1f;
    }

    private IEnumerator BadelineEmerges()
    {
      this.Level.Displacement.AddBurst(this.player.Center, 0.5f, 8f, 32f, 0.5f, (Ease.Easer) null, (Ease.Easer) null);
      this.Level.Session.Inventory.Dashes = 1;
      this.player.Dashes = 1;
      this.Level.Add((Entity) (this.badeline = new BadelineDummy(this.player.Position)));
      Audio.Play("event:/char/badeline/maddy_split", this.player.Position);
      this.badeline.Sprite.Scale.X = 1f;
      yield return (object) this.badeline.FloatTo(this.player.Position + new Vector2(-12f, -16f), new int?(1), false, false);
    }

    private IEnumerator OshiroEnters()
    {
      FadeWipe wipe = new FadeWipe((Scene) this.Level, false, (Action) null);
      wipe.Duration = 1.5f;
      yield return (object) wipe.Wait();
      this.fade = 1f;
      yield return (object) 0.25f;
      float playerWas = this.player.X;
      this.player.X = this.granny.X + 8f;
      this.badeline.X = this.player.X + 12f;
      this.player.Facing = Facings.Left;
      this.badeline.Sprite.Scale.X = -1f;
      this.granny.X = playerWas + 8f;
      this.theo.X += 16f;
      this.Level.Add(this.oshiro = new Entity(new Vector2(this.granny.X - 24f, this.granny.Y + 4f)));
      OshiroSprite oshiroSprite = new OshiroSprite(1);
      this.oshiro.Add((Component) oshiroSprite);
      oshiroSprite = (OshiroSprite) null;
      this.fade = 0.0f;
      wipe = new FadeWipe((Scene) this.Level, true, (Action) null);
      wipe.Duration = 1f;
      yield return (object) 0.25f;
      while ((double) this.oshiro.Y > (double) this.granny.Y - 4.0)
      {
        this.oshiro.Y -= Engine.DeltaTime * 32f;
        yield return (object) null;
      }
    }

    private IEnumerator OshiroSettles()
    {
      Vector2 from = this.oshiro.Position;
      Vector2 to = this.oshiro.Position + new Vector2(40f, 8f);
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime)
      {
        this.oshiro.Position = Vector2.Lerp(from, to, p);
        yield return (object) null;
      }
      this.granny.Sprite.Scale.X = 1f;
      yield return (object) null;
    }

    public override void Update()
    {
      this.versionAlpha = Calc.Approach(this.versionAlpha, this.showVersion ? 1f : 0.0f, Engine.DeltaTime * 5f);
      base.Update();
    }

    public override void Render()
    {
      if ((double) this.fade > 0.0)
        Draw.Rect(-10f, -10f, 1940f, 1100f, Color.Black * this.fade);
      base.Render();
      if (Settings.Instance.SpeedrunClock == SpeedrunType.Off || (double) this.versionAlpha <= 0.0)
        return;
      AreaComplete.VersionNumberAndVariants(this.version, this.versionAlpha, 1f);
    }

    public class TimeDisplay : Component
    {
      public Vector2 Position;
      public string Time;

      public TimeDisplay(string time)
        : base(true, true)
      {
        this.Time = time;
      }

      public override void Render()
      {
        SpeedrunTimerDisplay.DrawTime(this.RenderPosition, this.Time, 1f, true, false, false, 1f);
      }

      public Vector2 RenderPosition
      {
        get
        {
          return ((this.Entity != null ? this.Entity.Position : Vector2.Zero) + this.Position).Round();
        }
      }
    }
  }
}

