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
    private MTexture gradient = GFX.Gui["creditsgradient"].GetSubtexture(0, 1, 1920, 1, (MTexture) null);
    private bool autoWalk = true;
    private bool autoUpdateCamera = true;
    private bool badelineAutoFloat = true;
    private float fade = 1f;
    public const float CameraXOffset = 70f;
    public const float CameraYOffset = -24f;
    public static CS07_Credits Instance;
    public string Event;
    private Credits credits;
    private Player player;
    private BadelineDummy badeline;
    private bool badelineAutoWalk;
    private float badelineWalkApproach;
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
      CS07_Credits cs07Credits1 = this;
      cs07Credits1.Level.Background.Backdrops.Add((Backdrop) (cs07Credits1.fillbg = new CS07_Credits.Fill()));
      cs07Credits1.Level.Completed = true;
      cs07Credits1.Level.Entities.FindFirst<SpeedrunTimerDisplay>()?.RemoveSelf();
      cs07Credits1.Level.Entities.FindFirst<TotalStrawberriesDisplay>()?.RemoveSelf();
      cs07Credits1.Level.Entities.FindFirst<GameplayStats>()?.RemoveSelf();
      yield return (object) null;
      cs07Credits1.Level.Wipe.Cancel();
      yield return (object) 0.5f;
      float alignment = 1f;
      if (SaveData.Instance != null && SaveData.Instance.Assists.MirrorMode)
        alignment = 0.0f;
      cs07Credits1.credits = new Credits(alignment, 0.6f, false, true);
      cs07Credits1.credits.AllowInput = false;
      yield return (object) 3f;
      cs07Credits1.SetBgFade(0.0f);
      cs07Credits1.Add((Component) new Coroutine(cs07Credits1.FadeTo(0.0f), true));
      yield return (object) cs07Credits1.SetupLevel();
      yield return (object) cs07Credits1.WaitForPlayer();
      yield return (object) cs07Credits1.FadeTo(1f);
      yield return (object) 1f;
      cs07Credits1.SetBgFade(0.1f);
      yield return (object) cs07Credits1.NextLevel("credits-dashes");
      yield return (object) cs07Credits1.SetupLevel();
      cs07Credits1.Add((Component) new Coroutine(cs07Credits1.FadeTo(0.0f), true));
      yield return (object) cs07Credits1.WaitForPlayer();
      yield return (object) cs07Credits1.FadeTo(1f);
      yield return (object) 1f;
      cs07Credits1.SetBgFade(0.2f);
      yield return (object) cs07Credits1.NextLevel("credits-walking");
      yield return (object) cs07Credits1.SetupLevel();
      cs07Credits1.Add((Component) new Coroutine(cs07Credits1.FadeTo(0.0f), true));
      yield return (object) 5.8f;
      cs07Credits1.badelineAutoFloat = false;
      yield return (object) 0.5f;
      cs07Credits1.badeline.Sprite.Scale.X = (__Null) 1.0;
      yield return (object) 0.5f;
      cs07Credits1.autoWalk = false;
      cs07Credits1.player.Speed = Vector2.get_Zero();
      cs07Credits1.player.Facing = Facings.Right;
      yield return (object) 1.5f;
      cs07Credits1.badeline.Sprite.Scale.X = (__Null) -1.0;
      yield return (object) 1f;
      cs07Credits1.badeline.Sprite.Scale.X = (__Null) -1.0;
      cs07Credits1.badelineAutoWalk = true;
      cs07Credits1.badelineWalkApproachFrom = cs07Credits1.badeline.Position;
      cs07Credits1.Add((Component) new Coroutine(cs07Credits1.BadelineApproachWalking(), true));
      yield return (object) 0.7f;
      cs07Credits1.autoWalk = true;
      cs07Credits1.player.Facing = Facings.Left;
      yield return (object) cs07Credits1.WaitForPlayer();
      yield return (object) cs07Credits1.FadeTo(1f);
      yield return (object) 1f;
      cs07Credits1.SetBgFade(0.3f);
      yield return (object) cs07Credits1.NextLevel("credits-tree");
      yield return (object) cs07Credits1.SetupLevel();
      Petals petals = new Petals();
      cs07Credits1.Level.Foreground.Backdrops.Add((Backdrop) petals);
      cs07Credits1.autoUpdateCamera = false;
      Vector2 target1 = Vector2.op_Addition(cs07Credits1.Level.Camera.Position, new Vector2(-220f, 32f));
      Camera camera1 = cs07Credits1.Level.Camera;
      camera1.Position = Vector2.op_Addition(camera1.Position, new Vector2(-100f, 0.0f));
      cs07Credits1.badelineWalkApproach = 1f;
      cs07Credits1.badelineAutoFloat = false;
      cs07Credits1.badelineAutoWalk = true;
      cs07Credits1.badeline.Floatness = 0.0f;
      cs07Credits1.Add((Component) new Coroutine(cs07Credits1.FadeTo(0.0f), true));
      cs07Credits1.Add((Component) new Coroutine(CutsceneEntity.CameraTo(target1, 12f, Ease.Linear, 0.0f), true));
      yield return (object) 3.5f;
      cs07Credits1.badeline.Sprite.Play("idle", false, false);
      cs07Credits1.badelineAutoWalk = false;
      yield return (object) 0.25f;
      cs07Credits1.autoWalk = false;
      cs07Credits1.player.Sprite.Play("idle", false, false);
      cs07Credits1.player.Speed = Vector2.get_Zero();
      cs07Credits1.player.DummyAutoAnimate = false;
      cs07Credits1.player.Facing = Facings.Right;
      yield return (object) 0.5f;
      cs07Credits1.player.Sprite.Play("sitDown", false, false);
      yield return (object) 4f;
      cs07Credits1.badeline.Sprite.Play("laugh", false, false);
      yield return (object) 1.75f;
      yield return (object) cs07Credits1.FadeTo(1f);
      cs07Credits1.Level.Foreground.Backdrops.Remove((Backdrop) petals);
      petals = (Petals) null;
      yield return (object) 1f;
      cs07Credits1.SetBgFade(0.4f);
      yield return (object) cs07Credits1.NextLevel("credits-clouds");
      yield return (object) cs07Credits1.SetupLevel();
      cs07Credits1.autoWalk = false;
      cs07Credits1.player.Speed = Vector2.get_Zero();
      cs07Credits1.autoUpdateCamera = false;
      cs07Credits1.player.ForceCameraUpdate = false;
      cs07Credits1.badeline.Visible = false;
      Player other = (Player) null;
      foreach (CreditsTrigger entity in cs07Credits1.Scene.Tracker.GetEntities<CreditsTrigger>())
      {
        if (entity.Event == "BadelineOffset")
        {
          other = new Player(entity.Position, PlayerSpriteMode.Badeline);
          other.OverrideHairColor = new Color?(BadelineOldsite.HairColor);
          yield return (object) null;
          other.StateMachine.State = 11;
          other.Facing = Facings.Left;
          cs07Credits1.Scene.Add((Entity) other);
        }
      }
      cs07Credits1.Add((Component) new Coroutine(cs07Credits1.FadeTo(0.0f), true));
      Camera camera2 = cs07Credits1.Level.Camera;
      camera2.Position = Vector2.op_Addition(camera2.Position, new Vector2(0.0f, -100f));
      Vector2 target2 = Vector2.op_Addition(cs07Credits1.Level.Camera.Position, new Vector2(0.0f, 160f));
      cs07Credits1.Add((Component) new Coroutine(CutsceneEntity.CameraTo(target2, 12f, Ease.Linear, 0.0f), true));
      float playerHighJump = 0.0f;
      float baddyHighJump = 0.0f;
      for (float p = 0.0f; (double) p < 10.0; p += Engine.DeltaTime)
      {
        if (((double) p > 3.0 && (double) p < 6.0 || (double) p > 9.0) && (cs07Credits1.player.Speed.Y < 0.0 && cs07Credits1.player.OnGround(4)))
          playerHighJump = 0.25f;
        if ((double) p > 5.0 && (double) p < 8.0 && (other.Speed.Y < 0.0 && other.OnGround(4)))
          baddyHighJump = 0.25f;
        if ((double) playerHighJump > 0.0)
        {
          playerHighJump -= Engine.DeltaTime;
          cs07Credits1.player.Speed.Y = (__Null) -200.0;
        }
        if ((double) baddyHighJump > 0.0)
        {
          baddyHighJump -= Engine.DeltaTime;
          other.Speed.Y = (__Null) -200.0;
        }
        yield return (object) null;
      }
      yield return (object) cs07Credits1.FadeTo(1f);
      other = (Player) null;
      yield return (object) 1f;
      CS07_Credits cs07Credits = cs07Credits1;
      cs07Credits1.SetBgFade(0.5f);
      yield return (object) cs07Credits1.NextLevel("credits-resort");
      yield return (object) cs07Credits1.SetupLevel();
      cs07Credits1.Add((Component) new Coroutine(cs07Credits1.FadeTo(0.0f), true));
      cs07Credits1.badelineWalkApproach = 1f;
      cs07Credits1.badelineAutoFloat = false;
      cs07Credits1.badelineAutoWalk = true;
      cs07Credits1.badeline.Floatness = 0.0f;
      Vector2 vector2 = Vector2.get_Zero();
      foreach (CreditsTrigger creditsTrigger in cs07Credits1.Scene.Entities.FindAll<CreditsTrigger>())
      {
        if (creditsTrigger.Event == "Oshiro")
          vector2 = creditsTrigger.Position;
      }
      NPC oshiro = new NPC(Vector2.op_Addition(vector2, new Vector2(0.0f, 4f)));
      oshiro.Add((Component) (oshiro.Sprite = (Sprite) new OshiroSprite(1)));
      oshiro.MoveAnim = "sweeping";
      oshiro.IdleAnim = "sweeping";
      oshiro.Sprite.Play("sweeping", false, false);
      oshiro.Maxspeed = 10f;
      oshiro.Depth = -60;
      cs07Credits1.Scene.Add((Entity) oshiro);
      cs07Credits1.Add((Component) new Coroutine(cs07Credits1.DustyRoutine((Entity) oshiro), true));
      yield return (object) 4.8f;
      Vector2 oshiroTarget = Vector2.op_Addition(oshiro.Position, new Vector2(116f, 0.0f));
      Coroutine oshiroRoutine = new Coroutine(oshiro.MoveTo(oshiroTarget, false, new int?(), false), true);
      cs07Credits1.Add((Component) oshiroRoutine);
      yield return (object) 2f;
      cs07Credits1.autoUpdateCamera = false;
      Rectangle bounds1 = cs07Credits1.Level.Bounds;
      double num = (double) (((Rectangle) ref bounds1).get_Left() + 64);
      Rectangle bounds2 = cs07Credits1.Level.Bounds;
      double top = (double) ((Rectangle) ref bounds2).get_Top();
      yield return (object) CutsceneEntity.CameraTo(new Vector2((float) num, (float) top), 2f, (Ease.Easer) null, 0.0f);
      yield return (object) 5f;
      BirdNPC bird = new BirdNPC(Vector2.op_Addition(oshiro.Position, new Vector2(280f, -160f)), BirdNPC.Modes.None);
      bird.Depth = 10010;
      bird.Light.Visible = false;
      cs07Credits1.Scene.Add((Entity) bird);
      bird.Facing = Facings.Left;
      bird.Sprite.Play("fall", false, false);
      Vector2 from = bird.Position;
      Vector2 to = Vector2.op_Addition(oshiroTarget, new Vector2(50f, -12f));
      baddyHighJump = 0.0f;
      while ((double) baddyHighJump < 1.0)
      {
        bird.Position = Vector2.op_Addition(from, Vector2.op_Multiply(Vector2.op_Subtraction(to, from), Ease.QuadOut(baddyHighJump)));
        if ((double) baddyHighJump > 0.5)
        {
          bird.Sprite.Play("fly", false, false);
          bird.Depth = -1000000;
          bird.Light.Visible = true;
        }
        baddyHighJump += Engine.DeltaTime * 0.5f;
        yield return (object) null;
      }
      bird.Position = to;
      oshiroRoutine.RemoveSelf();
      oshiro.Sprite.Play("putBroomAway", false, false);
      oshiro.Sprite.OnFrameChange = (Action<string>) (anim =>
      {
        if (oshiro.Sprite.CurrentAnimationFrame != 10)
          return;
        Entity entity = new Entity(oshiro.Position);
        entity.Depth = oshiro.Depth + 1;
        cs07Credits.Scene.Add(entity);
        entity.Add((Component) new Monocle.Image(GFX.Game["characters/oshiro/broom"])
        {
          Origin = oshiro.Sprite.Origin
        });
        oshiro.Sprite.OnFrameChange = (Action<string>) null;
      });
      bird.Sprite.Play("idle", false, false);
      yield return (object) 0.5f;
      bird.Sprite.Play("croak", false, false);
      yield return (object) 0.6f;
      from = (Vector2) null;
      to = (Vector2) null;
      oshiro.Maxspeed = 40f;
      oshiro.MoveAnim = "move";
      oshiro.IdleAnim = "idle";
      yield return (object) oshiro.MoveTo(Vector2.op_Addition(oshiroTarget, new Vector2(14f, 0.0f)), false, new int?(), false);
      yield return (object) 2f;
      cs07Credits1.Add((Component) new Coroutine(bird.StartleAndFlyAway(), true));
      yield return (object) 0.75f;
      bird.Light.Visible = false;
      bird.Depth = 10010;
      oshiro.Sprite.Scale.X = (__Null) -1.0;
      yield return (object) cs07Credits1.FadeTo(1f);
      oshiroTarget = (Vector2) null;
      oshiroRoutine = (Coroutine) null;
      bird = (BirdNPC) null;
      yield return (object) 1f;
      cs07Credits1.SetBgFade(0.6f);
      yield return (object) cs07Credits1.NextLevel("credits-wallslide");
      yield return (object) cs07Credits1.SetupLevel();
      cs07Credits1.badelineAutoFloat = false;
      cs07Credits1.badeline.Floatness = 0.0f;
      cs07Credits1.badeline.Sprite.Play("idle", false, false);
      cs07Credits1.badeline.Sprite.Scale.X = (__Null) 1.0;
      foreach (CreditsTrigger entity in cs07Credits1.Scene.Tracker.GetEntities<CreditsTrigger>())
      {
        if (entity.Event == "BadelineOffset")
          cs07Credits1.badeline.Position = Vector2.op_Addition(entity.Position, new Vector2(8f, 16f));
      }
      cs07Credits1.Add((Component) new Coroutine(cs07Credits1.FadeTo(0.0f), true));
      cs07Credits1.Add((Component) new Coroutine(cs07Credits1.WaitForPlayer(), true));
      while ((double) cs07Credits1.player.X > (double) cs07Credits1.badeline.X - 16.0)
        yield return (object) null;
      cs07Credits1.badeline.Sprite.Scale.X = (__Null) -1.0;
      yield return (object) 0.1f;
      cs07Credits1.badelineAutoWalk = true;
      cs07Credits1.badelineWalkApproachFrom = cs07Credits1.badeline.Position;
      cs07Credits1.badelineWalkApproach = 0.0f;
      cs07Credits1.badeline.Sprite.Play("walk", false, false);
      while ((double) cs07Credits1.badelineWalkApproach != 1.0)
      {
        cs07Credits1.badelineWalkApproach = Calc.Approach(cs07Credits1.badelineWalkApproach, 1f, Engine.DeltaTime * 4f);
        yield return (object) null;
      }
      while ((double) cs07Credits1.player.X > (double) (cs07Credits1.Level.Bounds.X + 160))
        yield return (object) null;
      yield return (object) cs07Credits1.FadeTo(1f);
      yield return (object) 1f;
      cs07Credits1.SetBgFade(0.7f);
      yield return (object) cs07Credits1.NextLevel("credits-payphone");
      yield return (object) cs07Credits1.SetupLevel();
      cs07Credits1.player.Speed = Vector2.get_Zero();
      cs07Credits1.player.Facing = Facings.Left;
      cs07Credits1.autoWalk = false;
      cs07Credits1.badeline.Sprite.Play("idle", false, false);
      cs07Credits1.badeline.Floatness = 0.0f;
      cs07Credits1.badeline.Y = cs07Credits1.player.Y;
      cs07Credits1.badeline.Sprite.Scale.X = (__Null) 1.0;
      cs07Credits1.badelineAutoFloat = false;
      cs07Credits1.autoUpdateCamera = false;
      cs07Credits1.Level.Camera.X += 100f;
      Vector2 target3 = Vector2.op_Addition(cs07Credits1.Level.Camera.Position, new Vector2(-200f, 0.0f));
      cs07Credits1.Add((Component) new Coroutine(CutsceneEntity.CameraTo(target3, 14f, Ease.Linear, 0.0f), true));
      cs07Credits1.Add((Component) new Coroutine(cs07Credits1.FadeTo(0.0f), true));
      yield return (object) 1.5f;
      cs07Credits1.badeline.Sprite.Scale.X = (__Null) -1.0;
      yield return (object) 0.5f;
      cs07Credits1.Add((Component) new Coroutine(cs07Credits1.badeline.FloatTo(Vector2.op_Addition(cs07Credits1.badeline.Position, new Vector2(16f, -12f)), new int?(-1), false, false), true));
      yield return (object) 0.5f;
      cs07Credits1.player.Facing = Facings.Right;
      yield return (object) 1.5f;
      oshiroTarget = cs07Credits1.badeline.Position;
      to = cs07Credits1.player.Center;
      cs07Credits1.Add((Component) new Coroutine(cs07Credits1.BadelineAround(oshiroTarget, to, cs07Credits1.badeline), true));
      yield return (object) 0.5f;
      cs07Credits1.Add((Component) new Coroutine(cs07Credits1.BadelineAround(oshiroTarget, to, (BadelineDummy) null), true));
      yield return (object) 0.5f;
      cs07Credits1.Add((Component) new Coroutine(cs07Credits1.BadelineAround(oshiroTarget, to, (BadelineDummy) null), true));
      yield return (object) 3f;
      cs07Credits1.badeline.Sprite.Play("laugh", false, false);
      yield return (object) 0.5f;
      cs07Credits1.player.Facing = Facings.Left;
      yield return (object) 0.5f;
      cs07Credits1.player.DummyAutoAnimate = false;
      cs07Credits1.player.Sprite.Play("sitDown", false, false);
      yield return (object) 3f;
      yield return (object) cs07Credits1.FadeTo(1f);
      oshiroTarget = (Vector2) null;
      to = (Vector2) null;
      yield return (object) 1f;
      cs07Credits1.SetBgFade(0.8f);
      yield return (object) cs07Credits1.NextLevel("credits-city");
      yield return (object) cs07Credits1.SetupLevel();
      BirdNPC first = cs07Credits1.Scene.Entities.FindFirst<BirdNPC>();
      if (first != null)
        first.Facing = Facings.Right;
      cs07Credits1.badelineWalkApproach = 1f;
      cs07Credits1.badelineAutoFloat = false;
      cs07Credits1.badelineAutoWalk = true;
      cs07Credits1.badeline.Floatness = 0.0f;
      cs07Credits1.Add((Component) new Coroutine(cs07Credits1.FadeTo(0.0f), true));
      yield return (object) cs07Credits1.WaitForPlayer();
      yield return (object) cs07Credits1.FadeTo(1f);
      yield return (object) 1f;
      cs07Credits1.SetBgFade(0.0f);
      yield return (object) cs07Credits1.NextLevel("credits-prologue");
      yield return (object) cs07Credits1.SetupLevel();
      cs07Credits1.badelineWalkApproach = 1f;
      cs07Credits1.badelineAutoFloat = false;
      cs07Credits1.badelineAutoWalk = true;
      cs07Credits1.badeline.Floatness = 0.0f;
      cs07Credits1.Add((Component) new Coroutine(cs07Credits1.FadeTo(0.0f), true));
      yield return (object) cs07Credits1.WaitForPlayer();
      yield return (object) cs07Credits1.FadeTo(1f);
      while ((double) cs07Credits1.credits.BottomTimer < 2.0)
        yield return (object) null;
      if (!cs07Credits1.gotoEpilogue)
      {
        cs07Credits1.snow = new HiresSnow(0.45f);
        cs07Credits1.snow.Alpha = 0.0f;
        // ISSUE: reference to a compiler-generated method
        cs07Credits1.snow.AttachAlphaTo = (ScreenWipe) new FadeWipe((Scene) cs07Credits1.Level, false, new Action(cs07Credits1.\u003CRoutine\u003Eb__23_0));
        cs07Credits1.Level.Add((Monocle.Renderer) (cs07Credits1.Level.HiresSnow = cs07Credits1.snow));
      }
      else
      {
        // ISSUE: reference to a compiler-generated method
        FadeWipe fadeWipe = new FadeWipe((Scene) cs07Credits1.Level, false, new Action(cs07Credits1.\u003CRoutine\u003Eb__23_1));
      }
    }

    private IEnumerator SetupLevel()
    {
      CS07_Credits cs07Credits = this;
      cs07Credits.Level.SnapColorGrade("credits");
      cs07Credits.player = (Player) null;
      while ((cs07Credits.player = cs07Credits.Scene.Tracker.GetEntity<Player>()) == null)
        yield return (object) null;
      cs07Credits.Level.Add((Entity) (cs07Credits.badeline = new BadelineDummy(Vector2.op_Addition(cs07Credits.player.Position, new Vector2(16f, -16f)))));
      cs07Credits.badeline.Floatness = 4f;
      cs07Credits.badelineAutoFloat = true;
      cs07Credits.badelineAutoWalk = false;
      cs07Credits.badelineWalkApproach = 0.0f;
      cs07Credits.Level.Session.Inventory.Dashes = 1;
      cs07Credits.player.Dashes = 1;
      cs07Credits.player.StateMachine.State = 11;
      cs07Credits.player.DummyFriction = false;
      cs07Credits.player.DummyMaxspeed = false;
      cs07Credits.player.Facing = Facings.Left;
      cs07Credits.autoWalk = true;
      cs07Credits.autoUpdateCamera = true;
      cs07Credits.Level.CameraOffset.X = (__Null) 70.0;
      cs07Credits.Level.CameraOffset.Y = (__Null) -24.0;
      cs07Credits.Level.Camera.Position = cs07Credits.player.CameraTarget;
    }

    private IEnumerator WaitForPlayer()
    {
      CS07_Credits cs07Credits = this;
      while ((double) cs07Credits.player.X > (double) (cs07Credits.Level.Bounds.X + 160))
      {
        if (cs07Credits.Event != null)
          yield return (object) cs07Credits.DoEvent(cs07Credits.Event);
        cs07Credits.Event = (string) null;
        yield return (object) null;
      }
    }

    private IEnumerator NextLevel(string name)
    {
      CS07_Credits cs07Credits = this;
      if (cs07Credits.player != null)
        cs07Credits.player.RemoveSelf();
      cs07Credits.player = (Player) null;
      cs07Credits.Level.OnEndOfFrame += (Action) (() =>
      {
        this.Level.UnloadLevel();
        this.Level.Session.Level = name;
        Session session = this.Level.Session;
        Level level = this.Level;
        Rectangle bounds = this.Level.Bounds;
        double left = (double) ((Rectangle) ref bounds).get_Left();
        bounds = this.Level.Bounds;
        double top = (double) ((Rectangle) ref bounds).get_Top();
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
      CS07_Credits cs07Credits = this;
      List<Entity> dusty = new List<Entity>();
      float timer = 0.0f;
      Vector2 offset = Vector2.op_Addition(oshiro.Position, new Vector2(220f, -24f));
      Vector2 start = offset;
      for (int index = 0; index < 3; ++index)
      {
        Entity entity = new Entity(Vector2.op_Addition(offset, new Vector2((float) (index * 24), 0.0f)))
        {
          Depth = -50
        };
        entity.Add((Component) new DustGraphic(true, false, true));
        Monocle.Image image = new Monocle.Image(GFX.Game["decals/3-resort/brokenbox_" + ((char) (97 + index)).ToString()]);
        image.JustifyOrigin(0.5f, 1f);
        image.Position = new Vector2(0.0f, -4f);
        entity.Add((Component) image);
        cs07Credits.Scene.Add(entity);
        dusty.Add(entity);
      }
      yield return (object) 3.8f;
      while (true)
      {
        for (int index = 0; index < dusty.Count; ++index)
        {
          Entity entity = dusty[index];
          entity.X = (float) offset.X + (float) (index * 24);
          entity.Y = (float) (offset.Y + Math.Sin((double) timer * 4.0 + (double) index * 0.800000011920929) * 4.0);
        }
        // ISSUE: variable of the null type
        __Null x = offset.X;
        Rectangle bounds = cs07Credits.Level.Bounds;
        double num = (double) (((Rectangle) ref bounds).get_Left() + 120);
        if (x < num)
          offset.Y = (__Null) (double) Calc.Approach((float) offset.Y, (float) (start.Y + 16.0), Engine.DeltaTime * 16f);
        ref __Null local = ref offset.X;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(float&) ref local = ^(float&) ref local - 26f * Engine.DeltaTime;
        timer += Engine.DeltaTime;
        yield return (object) null;
      }
    }

    private IEnumerator BadelineAround(
      Vector2 start,
      Vector2 around,
      BadelineDummy badeline = null)
    {
      CS07_Credits cs07Credits = this;
      bool removeAtEnd = badeline == null;
      if (badeline == null)
        cs07Credits.Scene.Add((Entity) (badeline = new BadelineDummy(start)));
      badeline.Sprite.Play("fallSlow", false, false);
      float angle = Calc.Angle(around, start);
      Vector2 vector2 = Vector2.op_Subtraction(around, start);
      float dist = ((Vector2) ref vector2).Length();
      float duration = 3f;
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime / duration)
      {
        badeline.Position = Vector2.op_Addition(around, Calc.AngleToVector(angle - p * 2f * 6.283185f, (float) ((double) dist + (double) Calc.YoYo(p) * 16.0 + Math.Sin((double) p * 6.28318548202515 * 4.0) * 5.0)));
        badeline.Sprite.Scale.X = (__Null) (double) Math.Sign((float) around.X - badeline.X);
        if (!removeAtEnd)
          cs07Credits.player.Facing = (Facings) Math.Sign(badeline.X - cs07Credits.player.X);
        if (cs07Credits.Scene.OnInterval(0.1f))
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
      this.player.StateMachine.State = 11;
      this.autoWalk = true;
    }

    private IEnumerator EventWaitJumpDoubleDash()
    {
      CS07_Credits cs07Credits = this;
      cs07Credits.autoWalk = false;
      cs07Credits.player.DummyFriction = true;
      yield return (object) 0.1f;
      cs07Credits.player.Facing = Facings.Right;
      yield return (object) 0.25f;
      yield return (object) cs07Credits.BadelineCombine();
      cs07Credits.player.Dashes = 2;
      yield return (object) 0.5f;
      cs07Credits.player.Facing = Facings.Left;
      yield return (object) 0.7f;
      cs07Credits.PlayerJump(-1);
      yield return (object) 0.4f;
      cs07Credits.player.OverrideDashDirection = new Vector2?(new Vector2(-1f, -1f));
      cs07Credits.player.StateMachine.State = cs07Credits.player.StartDash();
      yield return (object) 0.6f;
      cs07Credits.player.OverrideDashDirection = new Vector2?(new Vector2(-1f, 0.0f));
      cs07Credits.player.StateMachine.State = cs07Credits.player.StartDash();
      yield return (object) 0.6f;
      cs07Credits.player.OverrideDashDirection = new Vector2?();
      cs07Credits.player.StateMachine.State = 11;
      cs07Credits.autoWalk = true;
      while (!cs07Credits.player.OnGround(1))
        yield return (object) null;
      cs07Credits.autoWalk = false;
      cs07Credits.player.DummyFriction = true;
      cs07Credits.player.Dashes = 2;
      yield return (object) 0.5f;
      cs07Credits.player.Facing = Facings.Right;
      yield return (object) 1f;
      cs07Credits.Level.Displacement.AddBurst(cs07Credits.player.Position, 0.4f, 8f, 32f, 0.5f, (Ease.Easer) null, (Ease.Easer) null);
      cs07Credits.badeline.Position = cs07Credits.player.Position;
      cs07Credits.badeline.Visible = true;
      cs07Credits.badelineAutoFloat = true;
      cs07Credits.player.Dashes = 1;
      yield return (object) 0.8f;
      cs07Credits.player.Facing = Facings.Left;
      cs07Credits.autoWalk = true;
      cs07Credits.player.DummyFriction = false;
    }

    private IEnumerator EventClimbDown()
    {
      this.autoWalk = false;
      this.player.DummyFriction = true;
      yield return (object) 0.1f;
      this.PlayerJump(-1);
      yield return (object) 0.4f;
      while (!this.player.CollideCheck<Solid>(Vector2.op_Addition(this.player.Position, new Vector2(-1f, 0.0f))))
        yield return (object) null;
      this.player.DummyAutoAnimate = false;
      this.player.Sprite.Play("wallslide", false, false);
      while (this.player.CollideCheck<Solid>(Vector2.op_Addition(this.player.Position, new Vector2(-1f, 32f))))
      {
        this.player.CreateWallSlideParticles(-1);
        this.player.Speed.Y = (__Null) (double) Math.Min((float) this.player.Speed.Y, 40f);
        yield return (object) null;
      }
      this.PlayerJump(1);
      yield return (object) 0.4f;
      while (!this.player.CollideCheck<Solid>(Vector2.op_Addition(this.player.Position, new Vector2(1f, 0.0f))))
        yield return (object) null;
      this.player.DummyAutoAnimate = false;
      this.player.Sprite.Play("wallslide", false, false);
      while (!this.player.CollideCheck<Solid>(Vector2.op_Addition(this.player.Position, new Vector2(0.0f, 32f))))
      {
        this.player.CreateWallSlideParticles(1);
        this.player.Speed.Y = (__Null) (double) Math.Min((float) this.player.Speed.Y, 40f);
        yield return (object) null;
      }
      this.PlayerJump(-1);
      yield return (object) 0.4f;
      this.autoWalk = true;
    }

    private IEnumerator EventWait()
    {
      CS07_Credits cs07Credits = this;
      cs07Credits.badeline.Sprite.Play("idle", false, false);
      cs07Credits.badelineAutoWalk = false;
      cs07Credits.autoWalk = false;
      cs07Credits.player.DummyFriction = true;
      yield return (object) 0.1f;
      cs07Credits.player.DummyAutoAnimate = false;
      cs07Credits.player.Speed = Vector2.get_Zero();
      yield return (object) 0.5f;
      cs07Credits.player.Sprite.Play("lookUp", false, false);
      yield return (object) 2f;
      BirdNPC first = cs07Credits.Scene.Entities.FindFirst<BirdNPC>();
      if (first != null)
        first.AutoFly = true;
      yield return (object) 0.1f;
      cs07Credits.player.Sprite.Play("idle", false, false);
      yield return (object) 1f;
      cs07Credits.autoWalk = true;
      cs07Credits.player.DummyFriction = false;
      cs07Credits.player.DummyAutoAnimate = true;
      cs07Credits.badelineAutoWalk = true;
      cs07Credits.badelineWalkApproach = 0.0f;
      cs07Credits.badelineWalkApproachFrom = cs07Credits.badeline.Position;
      cs07Credits.badeline.Sprite.Play("walk", false, false);
      while ((double) cs07Credits.badelineWalkApproach < 1.0)
      {
        cs07Credits.badelineWalkApproach += Engine.DeltaTime * 4f;
        yield return (object) null;
      }
    }

    private IEnumerator BadelineCombine()
    {
      CS07_Credits cs07Credits = this;
      Vector2 from = cs07Credits.badeline.Position;
      cs07Credits.badelineAutoFloat = false;
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime / 0.25f)
      {
        cs07Credits.badeline.Position = Vector2.Lerp(from, cs07Credits.player.Position, Ease.CubeIn(p));
        yield return (object) null;
      }
      cs07Credits.badeline.Visible = false;
      cs07Credits.Level.Displacement.AddBurst(cs07Credits.player.Position, 0.4f, 8f, 32f, 0.5f, (Ease.Easer) null, (Ease.Easer) null);
    }

    private void PlayerJump(int direction)
    {
      this.player.Facing = (Facings) direction;
      this.player.DummyFriction = false;
      this.player.DummyAutoAnimate = true;
      this.player.Speed.X = (__Null) (double) (direction * 120);
      this.player.Jump(true, true);
      this.player.AutoJump = true;
      this.player.AutoJumpTimer = 2f;
    }

    private void SetBgFade(float alpha)
    {
      this.fillbg.Color = Color.op_Multiply(Color.get_Black(), alpha);
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
            this.player.Speed.X = (__Null) -44.7999992370605;
            bool flag1 = this.player.CollideCheck<Solid>(Vector2.op_Addition(this.player.Position, new Vector2(-20f, 0.0f)));
            bool flag2 = !this.player.CollideCheck<Solid>(Vector2.op_Addition(this.player.Position, new Vector2(-8f, 1f))) && !this.player.CollideCheck<Solid>(Vector2.op_Addition(this.player.Position, new Vector2(-8f, 32f)));
            if (flag1 | flag2)
            {
              this.player.Jump(true, true);
              this.player.AutoJump = true;
              this.player.AutoJumpTimer = flag1 ? 0.6f : 2f;
            }
          }
          else
            this.player.Speed.X = (__Null) -64.0;
        }
        if (this.badeline != null && this.badelineAutoFloat)
        {
          Vector2 position = this.badeline.Position;
          Vector2 vector2 = Vector2.op_Addition(this.player.Position, new Vector2(16f, -16f));
          this.badeline.Position = Vector2.op_Addition(position, Vector2.op_Multiply(Vector2.op_Subtraction(vector2, position), 1f - (float) Math.Pow(0.00999999977648258, (double) Engine.DeltaTime)));
          this.badeline.Sprite.Scale.X = (__Null) -1.0;
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
            this.badeline.Sprite.Scale.X = (__Null) (double) chaseState.Facing;
          }
          else
            this.badeline.Position = Vector2.Lerp(this.badelineWalkApproachFrom, chaseState.Position, this.badelineWalkApproach);
        }
        if ((double) Math.Abs((float) this.player.Speed.X) > 90.0)
          this.player.Speed.X = (__Null) (double) Calc.Approach((float) this.player.Speed.X, 90f * (float) Math.Sign((float) this.player.Speed.X), 1000f * Engine.DeltaTime);
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
        cameraTarget.Y = (__Null) (((double) this.Level.Camera.Y * 2.0 + cameraTarget.Y) / 3.0);
      this.Level.Camera.Position = Vector2.op_Addition(position, Vector2.op_Multiply(Vector2.op_Subtraction(cameraTarget, position), 1f - (float) Math.Pow(0.00999999977648258, (double) Engine.DeltaTime)));
      this.Level.Camera.X = (float) (int) cameraTarget.X;
    }

    public override void Render()
    {
      bool flag = SaveData.Instance != null && SaveData.Instance.Assists.MirrorMode;
      if (!this.Level.Paused)
      {
        if (flag)
          this.gradient.Draw(new Vector2(1720f, -10f), Vector2.get_Zero(), Color.op_Multiply(Color.get_White(), 0.6f), new Vector2(-1f, 1100f));
        else
          this.gradient.Draw(new Vector2(200f, -10f), Vector2.get_Zero(), Color.op_Multiply(Color.get_White(), 0.6f), new Vector2(1f, 1100f));
      }
      if ((double) this.fade > 0.0)
        Draw.Rect(-10f, -10f, 1940f, 1100f, Color.op_Multiply(Color.get_Black(), Ease.CubeInOut(this.fade)));
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
