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
      this.Sprite.Scale.X = (float) this.Facing;
      this.Sprite.UseRawDeltaTime = true;
      this.Sprite.OnFrameChange = (Action<string>) (spr =>
      {
        if (this.level == null || (double) this.X <= (double) this.level.Camera.Left + 64.0 || (double) this.X >= (double) this.level.Camera.Right - 64.0 || !spr.Equals("peck") && !spr.Equals("peckRare") || this.Sprite.CurrentAnimationFrame != 6)
          return;
        Audio.Play("event:/game/general/bird_peck", this.Position);
      });
      this.Add((Component) (this.Light = new VertexLight(new Vector2(0.0f, -8f), Color.White, 1f, 8, 32)));
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
      : this(data.Position + offset, data.Enum<BirdNPC.Modes>(nameof (mode), BirdNPC.Modes.None))
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
      if (entity != null && (double) entity.Y < (double) this.Y + 32.0)
        this.RemoveSelf();
    }

    public override void Update()
    {
      this.Sprite.Scale.X = (float) this.Facing;
      base.Update();
    }

    public IEnumerator Caw()
    {
      this.Sprite.Play("croak", false, false);
      while (this.Sprite.CurrentAnimationFrame < 9)
        yield return (object) null;
      Audio.Play("event:/game/general/bird_squawk", this.Position);
    }

    public IEnumerator ShowTutorial(BirdTutorialGui gui, bool caw = false)
    {
      if (caw)
        yield return (object) this.Caw();
      this.gui = gui;
      gui.Open = true;
      this.Scene.Add((Entity) gui);
      while ((double) gui.Scale < 1.0)
        yield return (object) null;
    }

    public IEnumerator HideTutorial()
    {
      if (this.gui != null)
      {
        this.gui.Open = false;
        while ((double) this.gui.Scale > 0.0)
          yield return (object) null;
        this.Scene.Remove((Entity) this.gui);
        this.gui = (BirdTutorialGui) null;
      }
    }

    public IEnumerator StartleAndFlyAway()
    {
      this.Depth = -1000000;
      this.level.Session.SetFlag(BirdNPC.FlownFlag + this.level.Session.Level, true);
      Audio.Play("event:/game/general/bird_startle", this.Position);
      Dust.Burst(this.Position, -1.570796f, 8);
      this.Sprite.Play("jump", false, false);
      Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeOut, 0.8f, true);
      tween.OnUpdate = (Action<Tween>) (t =>
      {
        if ((double) t.Eased < 0.5 && this.Scene.OnInterval(0.05f))
          this.level.Particles.Emit(BirdNPC.P_Feather, 2, this.Position + Vector2.UnitY * -6f, Vector2.One * 4f);
        Vector2 vector2 = Vector2.Lerp(new Vector2(100f, -100f), new Vector2(20f, -20f), t.Eased);
        vector2.X *= (float) -(int) this.Facing;
        this.Position = this.Position + vector2 * Engine.DeltaTime;
      });
      this.Add((Component) tween);
      while (tween.Active)
        yield return (object) null;
      tween = (Tween) null;
      this.Sprite.Play("fly", false, false);
      this.Facing = (Facings) -(int) this.Facing;
      Vector2 speed = new Vector2((float) ((int) this.Facing * 20), -40f);
      while ((double) this.Y > (double) this.level.Bounds.Top)
      {
        speed += new Vector2((float) ((int) this.Facing * 140), -120f) * Engine.DeltaTime;
        this.Position = this.Position + speed * Engine.DeltaTime;
        yield return (object) null;
      }
      speed = new Vector2();
      this.RemoveSelf();
    }

    private IEnumerator ClimbingTutorial()
    {
      yield return (object) 0.25f;
      Player p = this.Scene.Tracker.GetEntity<Player>();
      while ((double) Math.Abs(p.X - this.X) > 120.0)
        yield return (object) null;
      BirdTutorialGui tut1 = new BirdTutorialGui((Entity) this, new Vector2(0.0f, -16f), (object) Dialog.Clean("tutorial_climb", (Language) null), new object[2]
      {
        (object) Dialog.Clean("tutorial_hold", (Language) null),
        (object) Input.Grab
      });
      BirdTutorialGui tut2 = new BirdTutorialGui((Entity) this, new Vector2(0.0f, -16f), (object) Dialog.Clean("tutorial_climb", (Language) null), new object[3]
      {
        (object) Input.Grab,
        (object) "+",
        (object) new Vector2(0.0f, -1f)
      });
      bool first = true;
      bool willEnd;
      do
      {
        yield return (object) this.ShowTutorial(tut1, first);
        first = false;
        while (p.StateMachine.State != 1 && (double) p.Y > (double) this.Y)
          yield return (object) null;
        if ((double) p.Y > (double) this.Y)
        {
          Audio.Play("event:/ui/game/tutorial_note_flip_back");
          yield return (object) this.HideTutorial();
          yield return (object) this.ShowTutorial(tut2, false);
        }
        while (p.Scene != null && (!p.OnGround(1) || p.StateMachine.State == 1))
          yield return (object) null;
        willEnd = (double) p.Y <= (double) this.Y + 4.0;
        if (!willEnd)
          Audio.Play("event:/ui/game/tutorial_note_flip_front");
        yield return (object) this.HideTutorial();
      }
      while (!willEnd);
      yield return (object) this.StartleAndFlyAway();
    }

    private IEnumerator DashingTutorial()
    {
      this.Y = (float) this.level.Bounds.Top;
      this.X += 32f;
      yield return (object) 1f;
      Player player = this.Scene.Tracker.GetEntity<Player>();
      Bridge bridge = this.Scene.Entities.FindFirst<Bridge>();
      while (true)
      {
        if ((player == null || ((double) player.X <= (double) this.StartPosition.X - 92.0 || (double) player.Y <= (double) this.StartPosition.Y - 20.0) || (double) player.Y >= (double) this.StartPosition.Y - 10.0) && (!SaveData.Instance.Assists.Invincible || player == null || ((double) player.X <= (double) this.StartPosition.X - 60.0 || (double) player.Y <= (double) this.StartPosition.Y) || (double) player.Y >= (double) this.StartPosition.Y + 34.0))
          yield return (object) null;
        else
          break;
      }
      this.Scene.Add((Entity) new CS00_Ending(player, this, bridge));
    }

    private IEnumerator DreamJumpTutorial()
    {
      yield return (object) this.ShowTutorial(new BirdTutorialGui((Entity) this, new Vector2(0.0f, -16f), (object) Dialog.Clean("tutorial_dreamjump", (Language) null), new object[3]
      {
        (object) new Vector2(1f, 0.0f),
        (object) "+",
        (object) Input.Jump
      }), true);
      while (true)
      {
        Player p = this.Scene.Tracker.GetEntity<Player>();
        if (p == null || (double) p.X <= (double) this.X && (double) (this.Position - p.Position).Length() >= 32.0)
        {
          yield return (object) null;
          p = (Player) null;
        }
        else
          break;
      }
      yield return (object) this.HideTutorial();
      while (true)
      {
        Player p = this.Scene.Tracker.GetEntity<Player>();
        if (p == null || (double) (this.Position - p.Position).Length() >= 24.0)
        {
          yield return (object) null;
          p = (Player) null;
        }
        else
          break;
      }
      yield return (object) this.StartleAndFlyAway();
    }

    private IEnumerator SuperWallJumpTutorial()
    {
      this.Facing = Facings.Right;
      yield return (object) 0.25f;
      bool first = true;
      BirdTutorialGui tut1 = new BirdTutorialGui((Entity) this, new Vector2(0.0f, -16f), (object) GFX.Gui["hyperjump/tutorial00"], new object[2]
      {
        (object) Dialog.Clean("TUTORIAL_DASH", (Language) null),
        (object) new Vector2(0.0f, -1f)
      });
      BirdTutorialGui tut2 = new BirdTutorialGui((Entity) this, new Vector2(0.0f, -16f), (object) GFX.Gui["hyperjump/tutorial01"], new object[1]
      {
        (object) Dialog.Clean("TUTORIAL_DREAMJUMP", (Language) null)
      });
      while (true)
      {
        yield return (object) this.ShowTutorial(tut1, first);
        this.Sprite.Play("idleRarePeck", false, false);
        yield return (object) 2f;
        this.gui = tut2;
        this.gui.Open = true;
        this.gui.Scale = 1f;
        this.Scene.Add((Entity) this.gui);
        yield return (object) null;
        tut1.Open = false;
        tut1.Scale = 0.0f;
        this.Scene.Remove((Entity) tut1);
        yield return (object) 2f;
        yield return (object) this.HideTutorial();
        yield return (object) 2f;
        first = false;
        Player player = this.Scene.Tracker.GetEntity<Player>();
        if (player == null || (double) player.Y > (double) this.Y || (double) player.X <= (double) this.X + 144.0)
          player = (Player) null;
        else
          break;
      }
      yield return (object) this.StartleAndFlyAway();
    }

    private IEnumerator HyperJumpTutorial()
    {
      this.Facing = Facings.Left;
      BirdTutorialGui tut = new BirdTutorialGui((Entity) this, new Vector2(0.0f, -16f), (object) Dialog.Clean("TUTORIAL_DREAMJUMP", (Language) null), new object[5]
      {
        (object) new Vector2(1f, 1f),
        (object) "+",
        (object) Input.Dash,
        (object) GFX.Gui["tinyarrow"],
        (object) Input.Jump
      });
      yield return (object) 0.3f;
      yield return (object) this.ShowTutorial(tut, true);
    }

    private IEnumerator WaitRoutine()
    {
      while (!this.AutoFly)
      {
        Player player = this.Scene.Tracker.GetEntity<Player>();
        if (player == null || (double) Math.Abs(player.X - this.X) >= 120.0)
        {
          yield return (object) null;
          player = (Player) null;
        }
        else
          break;
      }
      yield return (object) this.Caw();
      while (!this.AutoFly)
      {
        Player player = this.Scene.Tracker.GetEntity<Player>();
        if (player == null || (double) (player.Center - this.Position).Length() >= 32.0)
        {
          yield return (object) null;
          player = (Player) null;
        }
        else
          break;
      }
      yield return (object) this.StartleAndFlyAway();
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
      float x1 = this.StartPosition.X - 92f;
      Rectangle bounds = this.level.Bounds;
      float right1 = (float) bounds.Right;
      float y1 = this.StartPosition.Y - 20f;
      float y2 = this.StartPosition.Y - 10f;
      Draw.Line(new Vector2(x1, y1), new Vector2(x1, y2), Color.Aqua);
      Draw.Line(new Vector2(x1, y1), new Vector2(right1, y1), Color.Aqua);
      Draw.Line(new Vector2(right1, y1), new Vector2(right1, y2), Color.Aqua);
      Draw.Line(new Vector2(x1, y2), new Vector2(right1, y2), Color.Aqua);
      float x2 = this.StartPosition.X - 60f;
      bounds = this.level.Bounds;
      float right2 = (float) bounds.Right;
      float y3 = this.StartPosition.Y;
      float y4 = this.StartPosition.Y + 34f;
      Draw.Line(new Vector2(x2, y3), new Vector2(x2, y4), Color.Aqua);
      Draw.Line(new Vector2(x2, y3), new Vector2(right2, y3), Color.Aqua);
      Draw.Line(new Vector2(right2, y3), new Vector2(right2, y4), Color.Aqua);
      Draw.Line(new Vector2(x2, y4), new Vector2(right2, y4), Color.Aqua);
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

