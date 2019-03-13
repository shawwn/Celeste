// Decompiled with JetBrains decompiler
// Type: Celeste.BirdNPC
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class BirdNPC : Actor
  {
    private static string FlownFlag = "bird_fly_away_";
    public Facings Facing = Facings.Left;
    public static ParticleType P_Feather;
    public Sprite Sprite;
    public Vector2 StartPosition;
    public VertexLight Light;
    public bool AutoFly;
    private Coroutine tutorialRoutine;
    private BirdNPC.Modes mode;
    private BirdTutorialGui gui;
    private Level level;

    public BirdNPC(Vector2 position, BirdNPC.Modes mode)
      : base(position)
    {
      this.Add((Component) (this.Sprite = GFX.SpriteBank.Create("bird")));
      this.Sprite.Scale.X = (__Null) (double) this.Facing;
      this.Sprite.UseRawDeltaTime = true;
      this.Sprite.OnFrameChange = (Action<string>) (spr =>
      {
        if (this.level == null || (double) this.X <= (double) this.level.Camera.Left + 64.0 || (double) this.X >= (double) this.level.Camera.Right - 64.0 || (!spr.Equals("peck") && !spr.Equals("peckRare") || this.Sprite.CurrentAnimationFrame != 6))
          return;
        Audio.Play("event:/game/general/bird_peck", this.Position);
      });
      this.Add((Component) (this.Light = new VertexLight(new Vector2(0.0f, -8f), Color.get_White(), 1f, 8, 32)));
      this.StartPosition = this.Position;
      this.mode = mode;
      switch (mode)
      {
        case BirdNPC.Modes.ClimbingTutorial:
          this.Add((Component) (this.tutorialRoutine = new Coroutine(this.ClimbingTutorial(), true)));
          break;
        case BirdNPC.Modes.DashingTutorial:
          this.Add((Component) (this.tutorialRoutine = new Coroutine(this.DashingTutorial(), true)));
          break;
        case BirdNPC.Modes.DreamJumpTutorial:
          this.Add((Component) (this.tutorialRoutine = new Coroutine(this.DreamJumpTutorial(), true)));
          break;
        case BirdNPC.Modes.SuperWallJumpTutorial:
          this.Add((Component) (this.tutorialRoutine = new Coroutine(this.SuperWallJumpTutorial(), true)));
          break;
        case BirdNPC.Modes.HyperJumpTutorial:
          this.Add((Component) (this.tutorialRoutine = new Coroutine(this.HyperJumpTutorial(), true)));
          break;
        case BirdNPC.Modes.FlyAway:
          this.Add((Component) (this.tutorialRoutine = new Coroutine(this.WaitRoutine(), true)));
          break;
        case BirdNPC.Modes.Sleeping:
          this.Sprite.Play("sleep", false, false);
          this.Facing = Facings.Right;
          break;
      }
    }

    public BirdNPC(EntityData data, Vector2 offset)
      : this(Vector2.op_Addition(data.Position, offset), data.Enum<BirdNPC.Modes>(nameof (mode), BirdNPC.Modes.None))
    {
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      this.level = scene as Level;
      if (this.mode == BirdNPC.Modes.ClimbingTutorial && this.level.Session.GetLevelFlag("2"))
      {
        this.RemoveSelf();
      }
      else
      {
        if (this.mode != BirdNPC.Modes.FlyAway || !this.level.Session.GetFlag(BirdNPC.FlownFlag + this.level.Session.Level))
          return;
        this.RemoveSelf();
      }
    }

    public override void Awake(Scene scene)
    {
      base.Awake(scene);
      if (this.mode != BirdNPC.Modes.SuperWallJumpTutorial)
        return;
      Player entity = scene.Tracker.GetEntity<Player>();
      if (entity == null || (double) entity.Y >= (double) this.Y + 32.0)
        return;
      this.RemoveSelf();
    }

    public override void Update()
    {
      this.Sprite.Scale.X = (__Null) (double) this.Facing;
      base.Update();
    }

    public IEnumerator Caw()
    {
      BirdNPC birdNpc = this;
      birdNpc.Sprite.Play("croak", false, false);
      while (birdNpc.Sprite.CurrentAnimationFrame < 9)
        yield return (object) null;
      Audio.Play("event:/game/general/bird_squawk", birdNpc.Position);
    }

    public IEnumerator ShowTutorial(BirdTutorialGui gui, bool caw = false)
    {
      BirdNPC birdNpc = this;
      if (caw)
        yield return (object) birdNpc.Caw();
      birdNpc.gui = gui;
      gui.Open = true;
      birdNpc.Scene.Add((Entity) gui);
      while ((double) gui.Scale < 1.0)
        yield return (object) null;
    }

    public IEnumerator HideTutorial()
    {
      BirdNPC birdNpc = this;
      if (birdNpc.gui != null)
      {
        birdNpc.gui.Open = false;
        while ((double) birdNpc.gui.Scale > 0.0)
          yield return (object) null;
        birdNpc.Scene.Remove((Entity) birdNpc.gui);
        birdNpc.gui = (BirdTutorialGui) null;
      }
    }

    public IEnumerator StartleAndFlyAway()
    {
      BirdNPC birdNpc = this;
      birdNpc.Depth = -1000000;
      birdNpc.level.Session.SetFlag(BirdNPC.FlownFlag + birdNpc.level.Session.Level, true);
      Audio.Play("event:/game/general/bird_startle", birdNpc.Position);
      Dust.Burst(birdNpc.Position, -1.570796f, 8);
      birdNpc.Sprite.Play("jump", false, false);
      Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeOut, 0.8f, true);
      // ISSUE: reference to a compiler-generated method
      tween.OnUpdate = new Action<Tween>(birdNpc.\u003CStartleAndFlyAway\u003Eb__20_0);
      birdNpc.Add((Component) tween);
      while (tween.Active)
        yield return (object) null;
      tween = (Tween) null;
      birdNpc.Sprite.Play("fly", false, false);
      birdNpc.Facing = (Facings) -(int) birdNpc.Facing;
      Vector2 speed = new Vector2((float) ((int) birdNpc.Facing * 20), -40f);
      while (true)
      {
        double y = (double) birdNpc.Y;
        Rectangle bounds = birdNpc.level.Bounds;
        double top = (double) ((Rectangle) ref bounds).get_Top();
        if (y > top)
        {
          speed = Vector2.op_Addition(speed, Vector2.op_Multiply(new Vector2((float) ((int) birdNpc.Facing * 140), -120f), Engine.DeltaTime));
          birdNpc.Position = Vector2.op_Addition(birdNpc.Position, Vector2.op_Multiply(speed, Engine.DeltaTime));
          yield return (object) null;
        }
        else
          break;
      }
      speed = (Vector2) null;
      birdNpc.RemoveSelf();
    }

    private IEnumerator ClimbingTutorial()
    {
      BirdNPC birdNpc = this;
      yield return (object) 0.25f;
      Player p = birdNpc.Scene.Tracker.GetEntity<Player>();
      while ((double) Math.Abs(p.X - birdNpc.X) > 120.0)
        yield return (object) null;
      BirdTutorialGui tut1 = new BirdTutorialGui((Entity) birdNpc, new Vector2(0.0f, -16f), (object) Dialog.Clean("tutorial_climb", (Language) null), new object[2]
      {
        (object) Dialog.Clean("tutorial_hold", (Language) null),
        (object) Input.Grab
      });
      BirdTutorialGui tut2 = new BirdTutorialGui((Entity) birdNpc, new Vector2(0.0f, -16f), (object) Dialog.Clean("tutorial_climb", (Language) null), new object[3]
      {
        (object) Input.Grab,
        (object) "+",
        (object) new Vector2(0.0f, -1f)
      });
      bool first = true;
      bool willEnd;
      do
      {
        yield return (object) birdNpc.ShowTutorial(tut1, first);
        first = false;
        while (p.StateMachine.State != 1 && (double) p.Y > (double) birdNpc.Y)
          yield return (object) null;
        if ((double) p.Y > (double) birdNpc.Y)
        {
          Audio.Play("event:/ui/game/tutorial_note_flip_back");
          yield return (object) birdNpc.HideTutorial();
          yield return (object) birdNpc.ShowTutorial(tut2, false);
        }
        while (p.Scene != null && (!p.OnGround(1) || p.StateMachine.State == 1))
          yield return (object) null;
        willEnd = (double) p.Y <= (double) birdNpc.Y + 4.0;
        if (!willEnd)
          Audio.Play("event:/ui/game/tutorial_note_flip_front");
        yield return (object) birdNpc.HideTutorial();
      }
      while (!willEnd);
      yield return (object) birdNpc.StartleAndFlyAway();
    }

    private IEnumerator DashingTutorial()
    {
      BirdNPC bird = this;
      BirdNPC birdNpc = bird;
      Rectangle bounds = bird.level.Bounds;
      double top = (double) ((Rectangle) ref bounds).get_Top();
      birdNpc.Y = (float) top;
      bird.X += 32f;
      yield return (object) 1f;
      Player player = bird.Scene.Tracker.GetEntity<Player>();
      Bridge bridge = bird.Scene.Entities.FindFirst<Bridge>();
      while ((player == null || (double) player.X <= bird.StartPosition.X - 92.0 || ((double) player.Y <= bird.StartPosition.Y - 20.0 || (double) player.Y >= bird.StartPosition.Y - 10.0)) && (!SaveData.Instance.Assists.Invincible || player == null || ((double) player.X <= bird.StartPosition.X - 60.0 || (double) player.Y <= bird.StartPosition.Y) || (double) player.Y >= bird.StartPosition.Y + 34.0))
        yield return (object) null;
      bird.Scene.Add((Entity) new CS00_Ending(player, bird, bridge));
    }

    private IEnumerator DreamJumpTutorial()
    {
      BirdNPC birdNpc = this;
      yield return (object) birdNpc.ShowTutorial(new BirdTutorialGui((Entity) birdNpc, new Vector2(0.0f, -16f), (object) Dialog.Clean("tutorial_dreamjump", (Language) null), new object[3]
      {
        (object) new Vector2(1f, 0.0f),
        (object) "+",
        (object) Input.Jump
      }), true);
      while (true)
      {
        Player entity = birdNpc.Scene.Tracker.GetEntity<Player>();
        if (entity != null)
        {
          if ((double) entity.X <= (double) birdNpc.X)
          {
            Vector2 vector2 = Vector2.op_Subtraction(birdNpc.Position, entity.Position);
            if ((double) ((Vector2) ref vector2).Length() < 32.0)
              break;
          }
          else
            break;
        }
        yield return (object) null;
      }
      yield return (object) birdNpc.HideTutorial();
      while (true)
      {
        Player entity = birdNpc.Scene.Tracker.GetEntity<Player>();
        if (entity != null)
        {
          Vector2 vector2 = Vector2.op_Subtraction(birdNpc.Position, entity.Position);
          if ((double) ((Vector2) ref vector2).Length() < 24.0)
            break;
        }
        yield return (object) null;
      }
      yield return (object) birdNpc.StartleAndFlyAway();
    }

    private IEnumerator SuperWallJumpTutorial()
    {
      BirdNPC birdNpc = this;
      birdNpc.Facing = Facings.Right;
      yield return (object) 0.25f;
      bool caw = true;
      BirdTutorialGui tut1 = new BirdTutorialGui((Entity) birdNpc, new Vector2(0.0f, -16f), (object) GFX.Gui["hyperjump/tutorial00"], new object[2]
      {
        (object) Dialog.Clean("TUTORIAL_DASH", (Language) null),
        (object) new Vector2(0.0f, -1f)
      });
      BirdTutorialGui tut2 = new BirdTutorialGui((Entity) birdNpc, new Vector2(0.0f, -16f), (object) GFX.Gui["hyperjump/tutorial01"], new object[1]
      {
        (object) Dialog.Clean("TUTORIAL_DREAMJUMP", (Language) null)
      });
      Player entity;
      do
      {
        yield return (object) birdNpc.ShowTutorial(tut1, caw);
        birdNpc.Sprite.Play("idleRarePeck", false, false);
        yield return (object) 2f;
        birdNpc.gui = tut2;
        birdNpc.gui.Open = true;
        birdNpc.gui.Scale = 1f;
        birdNpc.Scene.Add((Entity) birdNpc.gui);
        yield return (object) null;
        tut1.Open = false;
        tut1.Scale = 0.0f;
        birdNpc.Scene.Remove((Entity) tut1);
        yield return (object) 2f;
        yield return (object) birdNpc.HideTutorial();
        yield return (object) 2f;
        caw = false;
        entity = birdNpc.Scene.Tracker.GetEntity<Player>();
      }
      while (entity == null || (double) entity.Y > (double) birdNpc.Y || (double) entity.X <= (double) birdNpc.X + 144.0);
      yield return (object) birdNpc.StartleAndFlyAway();
    }

    private IEnumerator HyperJumpTutorial()
    {
      BirdNPC birdNpc = this;
      birdNpc.Facing = Facings.Left;
      BirdTutorialGui tut = new BirdTutorialGui((Entity) birdNpc, new Vector2(0.0f, -16f), (object) Dialog.Clean("TUTORIAL_DREAMJUMP", (Language) null), new object[5]
      {
        (object) new Vector2(1f, 1f),
        (object) "+",
        (object) Input.Dash,
        (object) GFX.Gui["tinyarrow"],
        (object) Input.Jump
      });
      yield return (object) 0.3f;
      yield return (object) birdNpc.ShowTutorial(tut, true);
    }

    private IEnumerator WaitRoutine()
    {
      BirdNPC birdNpc = this;
      while (!birdNpc.AutoFly)
      {
        Player entity = birdNpc.Scene.Tracker.GetEntity<Player>();
        if (entity == null || (double) Math.Abs(entity.X - birdNpc.X) >= 120.0)
          yield return (object) null;
        else
          break;
      }
      yield return (object) birdNpc.Caw();
      while (!birdNpc.AutoFly)
      {
        Player entity = birdNpc.Scene.Tracker.GetEntity<Player>();
        if (entity != null)
        {
          Vector2 vector2 = Vector2.op_Subtraction(entity.Center, birdNpc.Position);
          if ((double) ((Vector2) ref vector2).Length() < 32.0)
            break;
        }
        yield return (object) null;
      }
      yield return (object) birdNpc.StartleAndFlyAway();
    }

    public override void SceneEnd(Scene scene)
    {
      Engine.TimeRate = 1f;
      base.SceneEnd(scene);
    }

    public override void DebugRender(Camera camera)
    {
      base.DebugRender(camera);
      if (this.mode != BirdNPC.Modes.DashingTutorial)
        return;
      float num1 = (float) (this.StartPosition.X - 92.0);
      Rectangle bounds1 = this.level.Bounds;
      float right1 = (float) ((Rectangle) ref bounds1).get_Right();
      float num2 = (float) (this.StartPosition.Y - 20.0);
      float num3 = (float) (this.StartPosition.Y - 10.0);
      Draw.Line(new Vector2(num1, num2), new Vector2(num1, num3), Color.get_Aqua());
      Draw.Line(new Vector2(num1, num2), new Vector2(right1, num2), Color.get_Aqua());
      Draw.Line(new Vector2(right1, num2), new Vector2(right1, num3), Color.get_Aqua());
      Draw.Line(new Vector2(num1, num3), new Vector2(right1, num3), Color.get_Aqua());
      float num4 = (float) (this.StartPosition.X - 60.0);
      Rectangle bounds2 = this.level.Bounds;
      float right2 = (float) ((Rectangle) ref bounds2).get_Right();
      float y = (float) this.StartPosition.Y;
      float num5 = (float) (this.StartPosition.Y + 34.0);
      Draw.Line(new Vector2(num4, y), new Vector2(num4, num5), Color.get_Aqua());
      Draw.Line(new Vector2(num4, y), new Vector2(right2, y), Color.get_Aqua());
      Draw.Line(new Vector2(right2, y), new Vector2(right2, num5), Color.get_Aqua());
      Draw.Line(new Vector2(num4, num5), new Vector2(right2, num5), Color.get_Aqua());
    }

    public enum Modes
    {
      ClimbingTutorial,
      DashingTutorial,
      DreamJumpTutorial,
      SuperWallJumpTutorial,
      HyperJumpTutorial,
      FlyAway,
      None,
      Sleeping,
    }
  }
}
