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
    private string version = Celeste.Celeste.Instance.Version.ToString();
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
      this.vignette.Position = Celeste.Celeste.TargetCenter;
      this.Add((Component) (this.cutscene = new Coroutine(this.Cutscene(level), true)));
    }

    private IEnumerator Cutscene(Level level)
    {
      CS08_Ending cs08Ending = this;
      level.ZoomSnap(new Vector2(164f, 120f), 2f);
      level.Wipe.Cancel();
      FadeWipe fadeWipe1 = new FadeWipe((Scene) level, true, (Action) null);
      while (cs08Ending.player == null)
      {
        cs08Ending.granny = level.Entities.FindFirst<NPC08_Granny>();
        cs08Ending.theo = level.Entities.FindFirst<NPC08_Theo>();
        cs08Ending.player = level.Tracker.GetEntity<Player>();
        yield return (object) null;
      }
      cs08Ending.player.StateMachine.State = 11;
      yield return (object) 1f;
      yield return (object) cs08Ending.player.DummyWalkToExact((int) cs08Ending.player.X + 16, false, 1f);
      yield return (object) 0.25f;
      yield return (object) Textbox.Say("EP_CABIN", new Func<IEnumerator>(cs08Ending.BadelineEmerges), new Func<IEnumerator>(cs08Ending.OshiroEnters), new Func<IEnumerator>(cs08Ending.OshiroSettles), new Func<IEnumerator>(cs08Ending.MaddyTurns));
      FadeWipe fadeWipe2 = new FadeWipe((Scene) cs08Ending.Level, false, (Action) null);
      fadeWipe2.Duration = 1.5f;
      yield return (object) fadeWipe2.Wait();
      cs08Ending.fade = 1f;
      yield return (object) Textbox.Say("EP_PIE_START");
      yield return (object) 0.5f;
      cs08Ending.vignettebg.Visible = true;
      cs08Ending.vignette.Visible = true;
      cs08Ending.vignettebg.Color = Color.get_Black();
      cs08Ending.vignette.Color = Color.op_Multiply(Color.get_White(), 0.0f);
      cs08Ending.Add((Component) cs08Ending.vignette);
      float p1;
      for (p1 = 0.0f; (double) p1 < 1.0; p1 += Engine.DeltaTime)
      {
        cs08Ending.vignette.Color = Color.op_Multiply(Color.get_White(), Ease.CubeIn(p1));
        cs08Ending.vignette.Scale = Vector2.op_Multiply(Vector2.get_One(), (float) (1.0 + 0.25 * (1.0 - (double) p1)));
        cs08Ending.vignette.Rotation = (float) (0.0500000007450581 * (1.0 - (double) p1));
        yield return (object) null;
      }
      cs08Ending.vignette.Color = Color.get_White();
      cs08Ending.vignettebg.Color = Color.get_White();
      yield return (object) 2f;
      p1 = 1f;
      float p2;
      for (p2 = 0.0f; (double) p2 < 1.0; p2 += Engine.DeltaTime / p1)
      {
        float num = Ease.CubeOut(p2);
        cs08Ending.vignette.Position = Vector2.Lerp(Celeste.Celeste.TargetCenter, Vector2.op_Addition(Celeste.Celeste.TargetCenter, new Vector2(0.0f, 140f)), num);
        cs08Ending.vignette.Scale = Vector2.op_Multiply(Vector2.get_One(), (float) (0.649999976158142 + 0.349999994039536 * (1.0 - (double) num)));
        cs08Ending.vignette.Rotation = -0.025f * num;
        yield return (object) null;
      }
      yield return (object) Textbox.Say(cs08Ending.endingDialog);
      yield return (object) 0.25f;
      p1 = 2f;
      Vector2 posFrom = cs08Ending.vignette.Position;
      p2 = cs08Ending.vignette.Rotation;
      float scaleFrom = (float) cs08Ending.vignette.Scale.X;
      for (float p3 = 0.0f; (double) p3 < 1.0; p3 += Engine.DeltaTime / p1)
      {
        float num = Ease.CubeOut(p3);
        cs08Ending.vignette.Position = Vector2.Lerp(posFrom, Celeste.Celeste.TargetCenter, num);
        cs08Ending.vignette.Scale = Vector2.op_Multiply(Vector2.get_One(), MathHelper.Lerp(scaleFrom, 1f, num));
        cs08Ending.vignette.Rotation = MathHelper.Lerp(p2, 0.0f, num);
        yield return (object) null;
      }
      posFrom = (Vector2) null;
      cs08Ending.EndCutscene(level, false);
    }

    public override void OnEnd(Level level)
    {
      this.vignette.Visible = true;
      this.vignette.Color = Color.get_White();
      this.vignette.Position = Celeste.Celeste.TargetCenter;
      this.vignette.Scale = Vector2.get_One();
      this.vignette.Rotation = 0.0f;
      if (this.player != null)
        this.player.Speed = Vector2.get_Zero();
      this.Scene.Entities.FindFirst<Textbox>()?.RemoveSelf();
      this.cutscene.RemoveSelf();
      this.Add((Component) new Coroutine(this.EndingRoutine(), true));
    }

    private IEnumerator EndingRoutine()
    {
      CS08_Ending cs08Ending = this;
      cs08Ending.Level.InCutscene = true;
      cs08Ending.Level.PauseLock = true;
      yield return (object) 0.5f;
      TimeSpan timeSpan = TimeSpan.FromTicks(SaveData.Instance.Time);
      string str = ((int) timeSpan.TotalHours).ToString() + timeSpan.ToString("\\:mm\\:ss\\.fff");
      StrawberriesCounter strawbs = new StrawberriesCounter(true, SaveData.Instance.TotalStrawberries, 175, true);
      DeathsCounter deaths = new DeathsCounter(AreaMode.Normal, true, SaveData.Instance.TotalDeaths, 0);
      CS08_Ending.TimeDisplay time = new CS08_Ending.TimeDisplay(str);
      float timeWidth = SpeedrunTimerDisplay.GetTimeWidth(str, 1f);
      cs08Ending.Add((Component) strawbs);
      cs08Ending.Add((Component) deaths);
      cs08Ending.Add((Component) time);
      Vector2 from = new Vector2(960f, 1180f);
      Vector2 to = new Vector2(960f, 940f);
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime / 0.5f)
      {
        Vector2 vector2 = Vector2.Lerp(from, to, Ease.CubeOut(p));
        strawbs.Position = Vector2.op_Addition(vector2, new Vector2(-170f, 0.0f));
        deaths.Position = Vector2.op_Addition(vector2, new Vector2(170f, 0.0f));
        time.Position = Vector2.op_Addition(vector2, new Vector2((float) (-(double) timeWidth / 2.0), 100f));
        yield return (object) null;
      }
      strawbs = (StrawberriesCounter) null;
      deaths = (DeathsCounter) null;
      time = (CS08_Ending.TimeDisplay) null;
      from = (Vector2) null;
      to = (Vector2) null;
      cs08Ending.showVersion = true;
      yield return (object) 0.25f;
      while (!Input.MenuConfirm.Pressed)
        yield return (object) null;
      cs08Ending.showVersion = false;
      yield return (object) 0.25f;
      cs08Ending.Level.CompleteArea(false, false);
    }

    private IEnumerator MaddyTurns()
    {
      yield return (object) 0.1f;
      this.player.Facing = (Facings) -(int) this.player.Facing;
      yield return (object) 0.1f;
    }

    private IEnumerator BadelineEmerges()
    {
      // ISSUE: reference to a compiler-generated field
      int num = this.\u003C\u003E1__state;
      CS08_Ending cs08Ending = this;
      if (num != 0)
      {
        if (num != 1)
          return false;
        // ISSUE: reference to a compiler-generated field
        this.\u003C\u003E1__state = -1;
        return false;
      }
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      cs08Ending.Level.Displacement.AddBurst(cs08Ending.player.Center, 0.5f, 8f, 32f, 0.5f, (Ease.Easer) null, (Ease.Easer) null);
      cs08Ending.Level.Session.Inventory.Dashes = 1;
      cs08Ending.player.Dashes = 1;
      cs08Ending.Level.Add((Entity) (cs08Ending.badeline = new BadelineDummy(cs08Ending.player.Position)));
      Audio.Play("event:/char/badeline/maddy_split", cs08Ending.player.Position);
      cs08Ending.badeline.Sprite.Scale.X = (__Null) 1.0;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E2__current = (object) cs08Ending.badeline.FloatTo(Vector2.op_Addition(cs08Ending.player.Position, new Vector2(-12f, -16f)), new int?(1), false, false);
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = 1;
      return true;
    }

    private IEnumerator OshiroEnters()
    {
      CS08_Ending cs08Ending = this;
      FadeWipe fadeWipe = new FadeWipe((Scene) cs08Ending.Level, false, (Action) null);
      fadeWipe.Duration = 1.5f;
      yield return (object) fadeWipe.Wait();
      cs08Ending.fade = 1f;
      yield return (object) 0.25f;
      float x = cs08Ending.player.X;
      cs08Ending.player.X = cs08Ending.granny.X + 8f;
      cs08Ending.badeline.X = cs08Ending.player.X + 12f;
      cs08Ending.player.Facing = Facings.Left;
      cs08Ending.badeline.Sprite.Scale.X = (__Null) -1.0;
      cs08Ending.granny.X = x + 8f;
      cs08Ending.theo.X += 16f;
      cs08Ending.Level.Add(cs08Ending.oshiro = new Entity(new Vector2(cs08Ending.granny.X - 24f, cs08Ending.granny.Y + 4f)));
      OshiroSprite oshiroSprite = new OshiroSprite(1);
      cs08Ending.oshiro.Add((Component) oshiroSprite);
      cs08Ending.fade = 0.0f;
      new FadeWipe((Scene) cs08Ending.Level, true, (Action) null).Duration = 1f;
      yield return (object) 0.25f;
      while ((double) cs08Ending.oshiro.Y > (double) cs08Ending.granny.Y - 4.0)
      {
        cs08Ending.oshiro.Y -= Engine.DeltaTime * 32f;
        yield return (object) null;
      }
    }

    private IEnumerator OshiroSettles()
    {
      Vector2 from = this.oshiro.Position;
      Vector2 to = Vector2.op_Addition(this.oshiro.Position, new Vector2(40f, 8f));
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime)
      {
        this.oshiro.Position = Vector2.Lerp(from, to, p);
        yield return (object) null;
      }
      this.granny.Sprite.Scale.X = (__Null) 1.0;
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
        Draw.Rect(-10f, -10f, 1940f, 1100f, Color.op_Multiply(Color.get_Black(), this.fade));
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
          return Vector2.op_Addition(this.Entity != null ? this.Entity.Position : Vector2.get_Zero(), this.Position).Round();
        }
      }
    }
  }
}
