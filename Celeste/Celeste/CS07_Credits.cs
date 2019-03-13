// Decompiled with JetBrains decompiler
// Type: Celeste.CS07_Credits
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Celeste
{
  public class CS07_Credits : CutsceneEntity
  {
    public string Event = (string) null;
    private MTexture gradient = GFX.Gui["creditsgradient"].GetSubtexture(0, 1, 1920, 1, (MTexture) null);
    private bool autoWalk = true;
    private bool autoUpdateCamera = true;
    private bool badelineAutoFloat = true;
    private float badelineWalkApproach = 0.0f;
    private float fade = 1f;
    public const float CameraXOffset = 70f;
    public const float CameraYOffset = -24f;
    public static CS07_Credits Instance;
    private Credits credits;
    private Player player;
    private BadelineDummy badeline;
    private bool badelineAutoWalk;
    private Vector2 badelineWalkApproachFrom;
    private float walkOffset;
    private CS07_Credits.Fill fillbg;
    private HiresSnow snow;
    private bool gotoEpilogue;

    public CS07_Credits()
      : base(true, false)
    {
      MInput.Disabled = true;
      CS07_Credits.Instance = this;
      this.Tag = (int) Tags.Global | (int) Tags.HUD;
    }

    public override void OnBegin(Level level)
    {
      Audio.BusMuted("bus:/gameplay_sfx", new bool?(true));
      this.gotoEpilogue = level.Session.OldStats.Modes[0].Completed;
      this.gotoEpilogue = true;
      this.Add((Component) new Coroutine(this.Routine(), true));
      this.Add((Component) new PostUpdateHook(new Action(this.PostUpdate)));
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      (this.Scene as Level).InCredits = true;
    }

    private IEnumerator Routine()
    {
      this.Level.Background.Backdrops.Add((Backdrop) (this.fillbg = new CS07_Credits.Fill()));
      this.Level.Completed = true;
      SpeedrunTimerDisplay timer = this.Level.Entities.FindFirst<SpeedrunTimerDisplay>();
      if (timer != null)
        timer.RemoveSelf();
      TotalStrawberriesDisplay strawbs = this.Level.Entities.FindFirst<TotalStrawberriesDisplay>();
      if (strawbs != null)
        strawbs.RemoveSelf();
      GameplayStats stats = this.Level.Entities.FindFirst<GameplayStats>();
      if (stats != null)
        stats.RemoveSelf();
      timer = (SpeedrunTimerDisplay) null;
      strawbs = (TotalStrawberriesDisplay) null;
      stats = (GameplayStats) null;
      yield return (object) null;
      this.Level.Wipe.Cancel();
      yield return (object) 0.5f;
      float alignment = 1f;
      if (SaveData.Instance != null && SaveData.Instance.Assists.MirrorMode)
        alignment = 0.0f;
      this.credits = new Credits(alignment, 0.6f, false, true);
      this.credits.AllowInput = false;
      yield return (object) 3f;
      this.SetBgFade(0.0f);
      this.Add((Component) new Coroutine(this.FadeTo(0.0f), true));
      yield return (object) this.SetupLevel();
      yield return (object) this.WaitForPlayer();
      yield return (object) this.FadeTo(1f);
      yield return (object) 1f;
      this.SetBgFade(0.1f);
      yield return (object) this.NextLevel("credits-dashes");
      yield return (object) this.SetupLevel();
      this.Add((Component) new Coroutine(this.FadeTo(0.0f), true));
      yield return (object) this.WaitForPlayer();
      yield return (object) this.FadeTo(1f);
      yield return (object) 1f;
      this.SetBgFade(0.2f);
      yield return (object) this.NextLevel("credits-walking");
      yield return (object) this.SetupLevel();
      this.Add((Component) new Coroutine(this.FadeTo(0.0f), true));
      yield return (object) 5.8f;
      this.badelineAutoFloat = false;
      yield return (object) 0.5f;
      this.badeline.Sprite.Scale.X = 1f;
      yield return (object) 0.5f;
      this.autoWalk = false;
      this.player.Speed = Vector2.Zero;
      this.player.Facing = Facings.Right;
      yield return (object) 1.5f;
      this.badeline.Sprite.Scale.X = -1f;
      yield return (object) 1f;
      this.badeline.Sprite.Scale.X = -1f;
      this.badelineAutoWalk = true;
      this.badelineWalkApproachFrom = this.badeline.Position;
      this.Add((Component) new Coroutine(this.BadelineApproachWalking(), true));
      yield return (object) 0.7f;
      this.autoWalk = true;
      this.player.Facing = Facings.Left;
      yield return (object) this.WaitForPlayer();
      yield return (object) this.FadeTo(1f);
      yield return (object) 1f;
      this.SetBgFade(0.3f);
      yield return (object) this.NextLevel("credits-tree");
      yield return (object) this.SetupLevel();
      Petals petals = new Petals();
      this.Level.Foreground.Backdrops.Add((Backdrop) petals);
      this.autoUpdateCamera = false;
      Vector2 cameraTo1 = this.Level.Camera.Position + new Vector2(-220f, 32f);
      this.Level.Camera.Position += new Vector2(-100f, 0.0f);
      this.badelineWalkApproach = 1f;
      this.badelineAutoFloat = false;
      this.badelineAutoWalk = true;
      this.badeline.Floatness = 0.0f;
      this.Add((Component) new Coroutine(this.FadeTo(0.0f), true));
      this.Add((Component) new Coroutine(CutsceneEntity.CameraTo(cameraTo1, 12f, Ease.Linear, 0.0f), true));
      yield return (object) 3.5f;
      this.badeline.Sprite.Play("idle", false, false);
      this.badelineAutoWalk = false;
      yield return (object) 0.25f;
      this.autoWalk = false;
      this.player.Sprite.Play("idle", false, false);
      this.player.Speed = Vector2.Zero;
      this.player.DummyAutoAnimate = false;
      this.player.Facing = Facings.Right;
      yield return (object) 0.5f;
      this.player.Sprite.Play("sitDown", false, false);
      yield return (object) 4f;
      this.badeline.Sprite.Play("laugh", false, false);
      yield return (object) 1.75f;
      yield return (object) this.FadeTo(1f);
      this.Level.Foreground.Backdrops.Remove((Backdrop) petals);
      petals = (Petals) null;
      cameraTo1 = new Vector2();
      yield return (object) 1f;
      this.SetBgFade(0.4f);
      yield return (object) this.NextLevel("credits-clouds");
      yield return (object) this.SetupLevel();
      this.autoWalk = false;
      this.player.Speed = Vector2.Zero;
      this.autoUpdateCamera = false;
      this.player.ForceCameraUpdate = false;
      this.badeline.Visible = false;
      Player other = (Player) null;
      foreach (CreditsTrigger entity in this.Scene.Tracker.GetEntities<CreditsTrigger>())
      {
        CreditsTrigger trigger = entity;
        if (trigger.Event == "BadelineOffset")
        {
          other = new Player(trigger.Position, PlayerSpriteMode.Badeline);
          other.OverrideHairColor = new Color?(BadelineOldsite.HairColor);
          yield return (object) null;
          other.StateMachine.State = Player.StDummy;
          other.Facing = Facings.Left;
          this.Scene.Add((Entity) other);
        }
        trigger = (CreditsTrigger) null;
      }
      this.Add((Component) new Coroutine(this.FadeTo(0.0f), true));
      this.Level.Camera.Position += new Vector2(0.0f, -100f);
      Vector2 cameraTo2 = this.Level.Camera.Position + new Vector2(0.0f, 160f);
      this.Add((Component) new Coroutine(CutsceneEntity.CameraTo(cameraTo2, 12f, Ease.Linear, 0.0f), true));
      float playerHighJump = 0.0f;
      float baddyHighJump = 0.0f;
      for (float p = 0.0f; (double) p < 10.0; p += Engine.DeltaTime)
      {
        if (((double) p > 3.0 && (double) p < 6.0 || (double) p > 9.0) && (double) this.player.Speed.Y < 0.0 && this.player.OnGround(4))
          playerHighJump = 0.25f;
        if ((double) p > 5.0 && (double) p < 8.0 && (double) other.Speed.Y < 0.0 && other.OnGround(4))
          baddyHighJump = 0.25f;
        if ((double) playerHighJump > 0.0)
        {
          playerHighJump -= Engine.DeltaTime;
          this.player.Speed.Y = -200f;
        }
        if ((double) baddyHighJump > 0.0)
        {
          baddyHighJump -= Engine.DeltaTime;
          other.Speed.Y = -200f;
        }
        yield return (object) null;
      }
      yield return (object) this.FadeTo(1f);
      other = (Player) null;
      cameraTo2 = new Vector2();
      yield return (object) 1f;
      this.SetBgFade(0.5f);
      yield return (object) this.NextLevel("credits-resort");
      yield return (object) this.SetupLevel();
      this.Add((Component) new Coroutine(this.FadeTo(0.0f), true));
      this.badelineWalkApproach = 1f;
      this.badelineAutoFloat = false;
      this.badelineAutoWalk = true;
      this.badeline.Floatness = 0.0f;
      Vector2 point = Vector2.Zero;
      foreach (CreditsTrigger creditsTrigger in this.Scene.Entities.FindAll<CreditsTrigger>())
      {
        CreditsTrigger credit = creditsTrigger;
        if (credit.Event == "Oshiro")
          point = credit.Position;
        credit = (CreditsTrigger) null;
      }
      NPC oshiro = new NPC(point + new Vector2(0.0f, 4f));
      oshiro.Add((Component) (oshiro.Sprite = (Sprite) new OshiroSprite(1)));
      oshiro.MoveAnim = "sweeping";
      oshiro.IdleAnim = "sweeping";
      oshiro.Sprite.Play("sweeping", false, false);
      oshiro.Maxspeed = 10f;
      oshiro.Depth = -60;
      this.Scene.Add((Entity) oshiro);
      this.Add((Component) new Coroutine(this.DustyRoutine((Entity) oshiro), true));
      yield return (object) 4.8f;
      Vector2 oshiroTarget = oshiro.Position + new Vector2(116f, 0.0f);
      Coroutine oshiroRoutine = new Coroutine(oshiro.MoveTo(oshiroTarget, false, new int?(), false), true);
      this.Add((Component) oshiroRoutine);
      yield return (object) 2f;
      this.autoUpdateCamera = false;
      Rectangle bounds = this.Level.Bounds;
      double num = (double) (bounds.Left + 64);
      bounds = this.Level.Bounds;
      double top = (double) bounds.Top;
      yield return (object) CutsceneEntity.CameraTo(new Vector2((float) num, (float) top), 2f, (Ease.Easer) null, 0.0f);
      yield return (object) 5f;
      BirdNPC bird1 = new BirdNPC(oshiro.Position + new Vector2(280f, -160f), BirdNPC.Modes.None);
      bird1.Depth = 10010;
      bird1.Light.Visible = false;
      this.Scene.Add((Entity) bird1);
      bird1.Facing = Facings.Left;
      bird1.Sprite.Play("fall", false, false);
      Vector2 from = bird1.Position;
      Vector2 to = oshiroTarget + new Vector2(50f, -12f);
      float percent = 0.0f;
      while ((double) percent < 1.0)
      {
        bird1.Position = from + (to - from) * Ease.QuadOut(percent);
        if ((double) percent > 0.5)
        {
          bird1.Sprite.Play("fly", false, false);
          bird1.Depth = -1000000;
          bird1.Light.Visible = true;
        }
        percent += Engine.DeltaTime * 0.5f;
        yield return (object) null;
      }
      bird1.Position = to;
      oshiroRoutine.RemoveSelf();
      oshiro.Sprite.Play("putBroomAway", false, false);
      oshiro.Sprite.OnFrameChange = (Action<string>) (anim =>
      {
        if (oshiro.Sprite.CurrentAnimationFrame != 10)
          return;
        Entity entity = new Entity(oshiro.Position);
        entity.Depth = oshiro.Depth + 1;
        this.Scene.Add(entity);
        entity.Add((Component) new Monocle.Image(GFX.Game["characters/oshiro/broom"])
        {
          Origin = oshiro.Sprite.Origin
        });
        oshiro.Sprite.OnFrameChange = (Action<string>) null;
      });
      bird1.Sprite.Play("idle", false, false);
      yield return (object) 0.5f;
      bird1.Sprite.Play("croak", false, false);
      yield return (object) 0.6f;
      from = new Vector2();
      to = new Vector2();
      oshiro.Maxspeed = 40f;
      oshiro.MoveAnim = "move";
      oshiro.IdleAnim = "idle";
      yield return (object) oshiro.MoveTo(oshiroTarget + new Vector2(14f, 0.0f), false, new int?(), false);
      yield return (object) 2f;
      this.Add((Component) new Coroutine(bird1.StartleAndFlyAway(), true));
      yield return (object) 0.75f;
      bird1.Light.Visible = false;
      bird1.Depth = 10010;
      oshiro.Sprite.Scale.X = -1f;
      yield return (object) this.FadeTo(1f);
      point = new Vector2();
      oshiroTarget = new Vector2();
      oshiroRoutine = (Coroutine) null;
      bird1 = (BirdNPC) null;
      yield return (object) 1f;
      this.SetBgFade(0.6f);
      yield return (object) this.NextLevel("credits-wallslide");
      yield return (object) this.SetupLevel();
      this.badelineAutoFloat = false;
      this.badeline.Floatness = 0.0f;
      this.badeline.Sprite.Play("idle", false, false);
      this.badeline.Sprite.Scale.X = 1f;
      foreach (CreditsTrigger entity in this.Scene.Tracker.GetEntities<CreditsTrigger>())
      {
        CreditsTrigger trigger = entity;
        if (trigger.Event == "BadelineOffset")
          this.badeline.Position = trigger.Position + new Vector2(8f, 16f);
        trigger = (CreditsTrigger) null;
      }
      this.Add((Component) new Coroutine(this.FadeTo(0.0f), true));
      this.Add((Component) new Coroutine(this.WaitForPlayer(), true));
      while ((double) this.player.X > (double) this.badeline.X - 16.0)
        yield return (object) null;
      this.badeline.Sprite.Scale.X = -1f;
      yield return (object) 0.1f;
      this.badelineAutoWalk = true;
      this.badelineWalkApproachFrom = this.badeline.Position;
      this.badelineWalkApproach = 0.0f;
      this.badeline.Sprite.Play("walk", false, false);
      while ((double) this.badelineWalkApproach != 1.0)
      {
        this.badelineWalkApproach = Calc.Approach(this.badelineWalkApproach, 1f, Engine.DeltaTime * 4f);
        yield return (object) null;
      }
      while ((double) this.player.X > (double) (this.Level.Bounds.X + 160))
        yield return (object) null;
      yield return (object) this.FadeTo(1f);
      yield return (object) 1f;
      this.SetBgFade(0.7f);
      yield return (object) this.NextLevel("credits-payphone");
      yield return (object) this.SetupLevel();
      this.player.Speed = Vector2.Zero;
      this.player.Facing = Facings.Left;
      this.autoWalk = false;
      this.badeline.Sprite.Play("idle", false, false);
      this.badeline.Floatness = 0.0f;
      this.badeline.Y = this.player.Y;
      this.badeline.Sprite.Scale.X = 1f;
      this.badelineAutoFloat = false;
      this.autoUpdateCamera = false;
      this.Level.Camera.X += 100f;
      Vector2 cameraTo3 = this.Level.Camera.Position + new Vector2(-200f, 0.0f);
      this.Add((Component) new Coroutine(CutsceneEntity.CameraTo(cameraTo3, 14f, Ease.Linear, 0.0f), true));
      this.Add((Component) new Coroutine(this.FadeTo(0.0f), true));
      yield return (object) 1.5f;
      this.badeline.Sprite.Scale.X = -1f;
      yield return (object) 0.5f;
      this.Add((Component) new Coroutine(this.badeline.FloatTo(this.badeline.Position + new Vector2(16f, -12f), new int?(-1), false, false), true));
      yield return (object) 0.5f;
      this.player.Facing = Facings.Right;
      yield return (object) 1.5f;
      Vector2 start = this.badeline.Position;
      Vector2 around = this.player.Center;
      this.Add((Component) new Coroutine(this.BadelineAround(start, around, this.badeline), true));
      yield return (object) 0.5f;
      this.Add((Component) new Coroutine(this.BadelineAround(start, around, (BadelineDummy) null), true));
      yield return (object) 0.5f;
      this.Add((Component) new Coroutine(this.BadelineAround(start, around, (BadelineDummy) null), true));
      yield return (object) 3f;
      this.badeline.Sprite.Play("laugh", false, false);
      yield return (object) 0.5f;
      this.player.Facing = Facings.Left;
      yield return (object) 0.5f;
      this.player.DummyAutoAnimate = false;
      this.player.Sprite.Play("sitDown", false, false);
      yield return (object) 3f;
      yield return (object) this.FadeTo(1f);
      cameraTo3 = new Vector2();
      start = new Vector2();
      around = new Vector2();
      yield return (object) 1f;
      this.SetBgFade(0.8f);
      yield return (object) this.NextLevel("credits-city");
      yield return (object) this.SetupLevel();
      BirdNPC bird2 = this.Scene.Entities.FindFirst<BirdNPC>();
      if (bird2 != null)
        bird2.Facing = Facings.Right;
      this.badelineWalkApproach = 1f;
      this.badelineAutoFloat = false;
      this.badelineAutoWalk = true;
      this.badeline.Floatness = 0.0f;
      this.Add((Component) new Coroutine(this.FadeTo(0.0f), true));
      yield return (object) this.WaitForPlayer();
      yield return (object) this.FadeTo(1f);
      bird2 = (BirdNPC) null;
      yield return (object) 1f;
      this.SetBgFade(0.0f);
      yield return (object) this.NextLevel("credits-prologue");
      yield return (object) this.SetupLevel();
      this.badelineWalkApproach = 1f;
      this.badelineAutoFloat = false;
      this.badelineAutoWalk = true;
      this.badeline.Floatness = 0.0f;
      this.Add((Component) new Coroutine(this.FadeTo(0.0f), true));
      yield return (object) this.WaitForPlayer();
      yield return (object) this.FadeTo(1f);
      while ((double) this.credits.BottomTimer < 2.0)
        yield return (object) null;
      if (!this.gotoEpilogue)
      {
        this.snow = new HiresSnow(0.45f);
        this.snow.Alpha = 0.0f;
        this.snow.AttachAlphaTo = (ScreenWipe) new FadeWipe((Scene) this.Level, false, (Action) (() => this.EndCutscene(this.Level, true)));
        this.Level.Add((Monocle.Renderer) (this.Level.HiresSnow = this.snow));
      }
      else
      {
        FadeWipe fadeWipe = new FadeWipe((Scene) this.Level, false, (Action) (() => this.EndCutscene(this.Level, true)));
      }
    }

    private IEnumerator SetupLevel()
    {
      this.Level.SnapColorGrade("credits");
      this.player = (Player) null;
      while ((this.player = this.Scene.Tracker.GetEntity<Player>()) == null)
        yield return (object) null;
      this.Level.Add((Entity) (this.badeline = new BadelineDummy(this.player.Position + new Vector2(16f, -16f))));
      this.badeline.Floatness = 4f;
      this.badelineAutoFloat = true;
      this.badelineAutoWalk = false;
      this.badelineWalkApproach = 0.0f;
      this.Level.Session.Inventory.Dashes = 1;
      this.player.Dashes = 1;
      this.player.StateMachine.State = Player.StDummy;
      this.player.DummyFriction = false;
      this.player.DummyMaxspeed = false;
      this.player.Facing = Facings.Left;
      this.autoWalk = true;
      this.autoUpdateCamera = true;
      this.Level.CameraOffset.X = 70f;
      this.Level.CameraOffset.Y = -24f;
      this.Level.Camera.Position = this.player.CameraTarget;
    }

    private IEnumerator WaitForPlayer()
    {
      while ((double) this.player.X > (double) (this.Level.Bounds.X + 160))
      {
        if (this.Event != null)
          yield return (object) this.DoEvent(this.Event);
        this.Event = (string) null;
        yield return (object) null;
      }
    }

    private IEnumerator NextLevel(string name)
    {
      if (this.player != null)
        this.player.RemoveSelf();
      this.player = (Player) null;
      this.Level.OnEndOfFrame += (Action) (() =>
      {
        this.Level.UnloadLevel();
        this.Level.Session.Level = name;
        Session session = this.Level.Session;
        Level level = this.Level;
        Rectangle bounds = this.Level.Bounds;
        double left = (double) bounds.Left;
        bounds = this.Level.Bounds;
        double top = (double) bounds.Top;
        Vector2 from = new Vector2((float) left, (float) top);
        Vector2? nullable = new Vector2?(level.GetSpawnPoint(from));
        session.RespawnPoint = nullable;
        this.Level.LoadLevel(Player.IntroTypes.None, false);
        this.Level.Wipe.Cancel();
      });
      yield return (object) null;
      yield return (object) null;
    }

    private IEnumerator FadeTo(float value)
    {
      while ((double) (this.fade = Calc.Approach(this.fade, value, Engine.DeltaTime * 0.5f)) != (double) value)
        yield return (object) null;
      this.fade = value;
    }

    private IEnumerator BadelineApproachWalking()
    {
      while ((double) this.badelineWalkApproach < 1.0)
      {
        this.badeline.Floatness = Calc.Approach(this.badeline.Floatness, 0.0f, Engine.DeltaTime * 8f);
        this.badelineWalkApproach = Calc.Approach(this.badelineWalkApproach, 1f, Engine.DeltaTime * 0.6f);
        yield return (object) null;
      }
    }

    private IEnumerator DustyRoutine(Entity oshiro)
    {
      List<Entity> dusty = new List<Entity>();
      float timer = 0.0f;
      Vector2 offset = oshiro.Position + new Vector2(220f, -24f);
      Vector2 start = offset;
      for (int i = 0; i < 3; ++i)
      {
        Entity dust = new Entity(offset + new Vector2((float) (i * 24), 0.0f))
        {
          Depth = -50
        };
        dust.Add((Component) new DustGraphic(true, false, true));
        Monocle.Image img = new Monocle.Image(GFX.Game["decals/3-resort/brokenbox_" + ((char) (97 + i)).ToString()]);
        img.JustifyOrigin(0.5f, 1f);
        img.Position = new Vector2(0.0f, -4f);
        dust.Add((Component) img);
        this.Scene.Add(dust);
        dusty.Add(dust);
        dust = (Entity) null;
        img = (Monocle.Image) null;
      }
      yield return (object) 3.8f;
      while (true)
      {
        for (int i = 0; i < dusty.Count; ++i)
        {
          Entity dust = dusty[i];
          dust.X = offset.X + (float) (i * 24);
          dust.Y = offset.Y + (float) Math.Sin((double) timer * 4.0 + (double) i * 0.800000011920929) * 4f;
          dust = (Entity) null;
        }
        if ((double) offset.X < (double) (this.Level.Bounds.Left + 120))
          offset.Y = Calc.Approach(offset.Y, start.Y + 16f, Engine.DeltaTime * 16f);
        offset.X -= 26f * Engine.DeltaTime;
        timer += Engine.DeltaTime;
        yield return (object) null;
      }
    }

    private IEnumerator BadelineAround(
      Vector2 start,
      Vector2 around,
      BadelineDummy badeline = null)
    {
      bool removeAtEnd = badeline == null;
      if (badeline == null)
        this.Scene.Add((Entity) (badeline = new BadelineDummy(start)));
      badeline.Sprite.Play("fallSlow", false, false);
      float angle = Calc.Angle(around, start);
      float dist = (around - start).Length();
      float duration = 3f;
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime / duration)
      {
        float a = p * 2f;
        badeline.Position = around + Calc.AngleToVector(angle - a * 6.283185f, (float) ((double) dist + (double) Calc.YoYo(p) * 16.0 + Math.Sin((double) p * 6.28318548202515 * 4.0) * 5.0));
        badeline.Sprite.Scale.X = (float) Math.Sign(around.X - badeline.X);
        if (!removeAtEnd)
          this.player.Facing = (Facings) Math.Sign(badeline.X - this.player.X);
        if (this.Scene.OnInterval(0.1f))
          TrailManager.Add((Entity) badeline, Player.NormalHairColor, 1f);
        yield return (object) null;
      }
      if (removeAtEnd)
        badeline.Vanish();
      else
        badeline.Sprite.Play("laugh", false, false);
    }

    private IEnumerator DoEvent(string e)
    {
      if (e == "WaitJumpDash")
        yield return (object) this.EventWaitJumpDash();
      else if (e == "WaitJumpDoubleDash")
        yield return (object) this.EventWaitJumpDoubleDash();
      else if (e == "ClimbDown")
        yield return (object) this.EventClimbDown();
      else if (e == "Wait")
        yield return (object) this.EventWait();
    }

    private IEnumerator EventWaitJumpDash()
    {
      this.autoWalk = false;
      this.player.DummyFriction = true;
      yield return (object) 0.1f;
      this.PlayerJump(-1);
      yield return (object) 0.2f;
      this.player.OverrideDashDirection = new Vector2?(new Vector2(-1f, -1f));
      this.player.StateMachine.State = this.player.StartDash();
      yield return (object) 0.6f;
      this.player.OverrideDashDirection = new Vector2?();
      this.player.StateMachine.State = Player.StDummy;
      this.autoWalk = true;
    }

    private IEnumerator EventWaitJumpDoubleDash()
    {
      this.autoWalk = false;
      this.player.DummyFriction = true;
      yield return (object) 0.1f;
      this.player.Facing = Facings.Right;
      yield return (object) 0.25f;
      yield return (object) this.BadelineCombine();
      this.player.Dashes = 2;
      yield return (object) 0.5f;
      this.player.Facing = Facings.Left;
      yield return (object) 0.7f;
      this.PlayerJump(-1);
      yield return (object) 0.4f;
      this.player.OverrideDashDirection = new Vector2?(new Vector2(-1f, -1f));
      this.player.StateMachine.State = this.player.StartDash();
      yield return (object) 0.6f;
      this.player.OverrideDashDirection = new Vector2?(new Vector2(-1f, 0.0f));
      this.player.StateMachine.State = this.player.StartDash();
      yield return (object) 0.6f;
      this.player.OverrideDashDirection = new Vector2?();
      this.player.StateMachine.State = Player.StDummy;
      this.autoWalk = true;
      while (!this.player.OnGround(1))
        yield return (object) null;
      this.autoWalk = false;
      this.player.DummyFriction = true;
      this.player.Dashes = 2;
      yield return (object) 0.5f;
      this.player.Facing = Facings.Right;
      yield return (object) 1f;
      this.Level.Displacement.AddBurst(this.player.Position, 0.4f, 8f, 32f, 0.5f, (Ease.Easer) null, (Ease.Easer) null);
      this.badeline.Position = this.player.Position;
      this.badeline.Visible = true;
      this.badelineAutoFloat = true;
      this.player.Dashes = 1;
      yield return (object) 0.8f;
      this.player.Facing = Facings.Left;
      this.autoWalk = true;
      this.player.DummyFriction = false;
    }

    private IEnumerator EventClimbDown()
    {
      this.autoWalk = false;
      this.player.DummyFriction = true;
      yield return (object) 0.1f;
      this.PlayerJump(-1);
      yield return (object) 0.4f;
      while (!this.player.CollideCheck<Solid>(this.player.Position + new Vector2(-1f, 0.0f)))
        yield return (object) null;
      this.player.DummyAutoAnimate = false;
      this.player.Sprite.Play("wallslide", false, false);
      while (this.player.CollideCheck<Solid>(this.player.Position + new Vector2(-1f, 32f)))
      {
        this.player.CreateWallSlideParticles(-1);
        this.player.Speed.Y = Math.Min(this.player.Speed.Y, 40f);
        yield return (object) null;
      }
      this.PlayerJump(1);
      yield return (object) 0.4f;
      while (!this.player.CollideCheck<Solid>(this.player.Position + new Vector2(1f, 0.0f)))
        yield return (object) null;
      this.player.DummyAutoAnimate = false;
      this.player.Sprite.Play("wallslide", false, false);
      while (!this.player.CollideCheck<Solid>(this.player.Position + new Vector2(0.0f, 32f)))
      {
        this.player.CreateWallSlideParticles(1);
        this.player.Speed.Y = Math.Min(this.player.Speed.Y, 40f);
        yield return (object) null;
      }
      this.PlayerJump(-1);
      yield return (object) 0.4f;
      this.autoWalk = true;
    }

    private IEnumerator EventWait()
    {
      this.badeline.Sprite.Play("idle", false, false);
      this.badelineAutoWalk = false;
      this.autoWalk = false;
      this.player.DummyFriction = true;
      yield return (object) 0.1f;
      this.player.DummyAutoAnimate = false;
      this.player.Speed = Vector2.Zero;
      yield return (object) 0.5f;
      this.player.Sprite.Play("lookUp", false, false);
      yield return (object) 2f;
      BirdNPC bird = this.Scene.Entities.FindFirst<BirdNPC>();
      if (bird != null)
        bird.AutoFly = true;
      yield return (object) 0.1f;
      this.player.Sprite.Play("idle", false, false);
      yield return (object) 1f;
      this.autoWalk = true;
      this.player.DummyFriction = false;
      this.player.DummyAutoAnimate = true;
      this.badelineAutoWalk = true;
      this.badelineWalkApproach = 0.0f;
      this.badelineWalkApproachFrom = this.badeline.Position;
      this.badeline.Sprite.Play("walk", false, false);
      while ((double) this.badelineWalkApproach < 1.0)
      {
        this.badelineWalkApproach += Engine.DeltaTime * 4f;
        yield return (object) null;
      }
    }

    private IEnumerator BadelineCombine()
    {
      Vector2 from = this.badeline.Position;
      this.badelineAutoFloat = false;
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime / 0.25f)
      {
        this.badeline.Position = Vector2.Lerp(from, this.player.Position, Ease.CubeIn(p));
        yield return (object) null;
      }
      this.badeline.Visible = false;
      this.Level.Displacement.AddBurst(this.player.Position, 0.4f, 8f, 32f, 0.5f, (Ease.Easer) null, (Ease.Easer) null);
    }

    private void PlayerJump(int direction)
    {
      this.player.Facing = (Facings) direction;
      this.player.DummyFriction = false;
      this.player.DummyAutoAnimate = true;
      this.player.Speed.X = (float) (direction * 120);
      this.player.Jump(true, true);
      this.player.AutoJump = true;
      this.player.AutoJumpTimer = 2f;
    }

    private void SetBgFade(float alpha)
    {
      this.fillbg.Color = Color.Black * alpha;
    }

    public override void Update()
    {
      MInput.Disabled = false;
      if (this.Level.CanPause && (Input.Pause.Pressed || Input.ESC.Pressed))
        this.Level.Pause(0, true, false);
      MInput.Disabled = true;
      if (this.player != null && this.player.Scene != null)
      {
        if (this.player.OverrideDashDirection.HasValue)
        {
          Input.MoveX.Value = (int) this.player.OverrideDashDirection.Value.X;
          Input.MoveY.Value = (int) this.player.OverrideDashDirection.Value.Y;
        }
        if (this.autoWalk)
        {
          if (this.player.OnGround(1))
          {
            this.player.Speed.X = -44.8f;
            bool flag1 = this.player.CollideCheck<Solid>(this.player.Position + new Vector2(-20f, 0.0f));
            bool flag2 = !this.player.CollideCheck<Solid>(this.player.Position + new Vector2(-8f, 1f)) && !this.player.CollideCheck<Solid>(this.player.Position + new Vector2(-8f, 32f));
            if (flag1 | flag2)
            {
              this.player.Jump(true, true);
              this.player.AutoJump = true;
              this.player.AutoJumpTimer = flag1 ? 0.6f : 2f;
            }
          }
          else
            this.player.Speed.X = -64f;
        }
        if (this.badeline != null && this.badelineAutoFloat)
        {
          Vector2 position = this.badeline.Position;
          Vector2 vector2 = this.player.Position + new Vector2(16f, -16f);
          this.badeline.Position = position + (vector2 - position) * (1f - (float) Math.Pow(0.00999999977648258, (double) Engine.DeltaTime));
          this.badeline.Sprite.Scale.X = -1f;
        }
        if (this.badeline != null && this.badelineAutoWalk)
        {
          Player.ChaserState chaseState;
          this.player.GetChasePosition(this.Scene.TimeActive, (float) (0.349999994039536 + Math.Sin((double) this.walkOffset) * 0.100000001490116), out chaseState);
          if (chaseState.OnGround)
            this.walkOffset += Engine.DeltaTime;
          if ((double) this.badelineWalkApproach >= 1.0)
          {
            this.badeline.Position = chaseState.Position;
            if (this.badeline.Sprite.Has(chaseState.Animation))
              this.badeline.Sprite.Play(chaseState.Animation, false, false);
            this.badeline.Sprite.Scale.X = (float) chaseState.Facing;
          }
          else
            this.badeline.Position = Vector2.Lerp(this.badelineWalkApproachFrom, chaseState.Position, this.badelineWalkApproach);
        }
        if ((double) Math.Abs(this.player.Speed.X) > 90.0)
          this.player.Speed.X = Calc.Approach(this.player.Speed.X, 90f * (float) Math.Sign(this.player.Speed.X), 1000f * Engine.DeltaTime);
      }
      if (this.credits != null)
        this.credits.Update();
      base.Update();
    }

    public void PostUpdate()
    {
      if (this.player == null || this.player.Scene == null || !this.autoUpdateCamera)
        return;
      Vector2 position = this.Level.Camera.Position;
      Vector2 cameraTarget = this.player.CameraTarget;
      if (!this.player.OnGround(1))
        cameraTarget.Y = (float) (((double) this.Level.Camera.Y * 2.0 + (double) cameraTarget.Y) / 3.0);
      this.Level.Camera.Position = position + (cameraTarget - position) * (1f - (float) Math.Pow(0.00999999977648258, (double) Engine.DeltaTime));
      this.Level.Camera.X = (float) (int) cameraTarget.X;
    }

    public override void Render()
    {
      bool flag = SaveData.Instance != null && SaveData.Instance.Assists.MirrorMode;
      if (!this.Level.Paused)
      {
        if (flag)
          this.gradient.Draw(new Vector2(1720f, -10f), Vector2.Zero, Color.White * 0.6f, new Vector2(-1f, 1100f));
        else
          this.gradient.Draw(new Vector2(200f, -10f), Vector2.Zero, Color.White * 0.6f, new Vector2(1f, 1100f));
      }
      if ((double) this.fade > 0.0)
        Draw.Rect(-10f, -10f, 1940f, 1100f, Color.Black * Ease.CubeInOut(this.fade));
      if (this.credits != null && !this.Level.Paused)
        this.credits.Render(new Vector2(flag ? 100f : 1820f, 0.0f));
      base.Render();
    }

    public override void OnEnd(Level level)
    {
      Audio.BusMuted("bus:/gameplay_sfx", new bool?(false));
      CS07_Credits.Instance = (CS07_Credits) null;
      MInput.Disabled = false;
      if (!this.gotoEpilogue)
        Engine.Scene = (Scene) new OverworldLoader(Overworld.StartMode.AreaComplete, this.snow);
      else
        LevelEnter.Go(new Session(new AreaKey(8, AreaMode.Normal), (string) null, (AreaStats) null), false);
    }

    private class Fill : Backdrop
    {
      public override void Render(Scene scene)
      {
        Draw.Rect(-10f, -10f, 340f, 200f, this.Color);
      }
    }
  }
}

