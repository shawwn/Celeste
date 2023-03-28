// Decompiled with JetBrains decompiler
// Type: Celeste.BirdNPC
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class BirdNPC : Actor
  {
    public static ParticleType P_Feather;
    private static string FlownFlag = "bird_fly_away_";
    public Facings Facing = Facings.Left;
    public Sprite Sprite;
    public Vector2 StartPosition;
    public VertexLight Light;
    public bool AutoFly;
    public EntityID EntityID;
    public bool FlyAwayUp = true;
    public float WaitForLightningPostDelay;
    public bool DisableFlapSfx;
    private Coroutine tutorialRoutine;
    private BirdNPC.Modes mode;
    private BirdTutorialGui gui;
    private Level level;
    private Vector2[] nodes;
    private StaticMover staticMover;
    private bool onlyOnce;
    private bool onlyIfPlayerLeft;

    public BirdNPC(Vector2 position, BirdNPC.Modes mode)
      : base(position)
    {
      this.Add((Component) (this.Sprite = GFX.SpriteBank.Create("bird")));
      this.Sprite.Scale.X = (float) this.Facing;
      this.Sprite.UseRawDeltaTime = true;
      this.Sprite.OnFrameChange = (Action<string>) (spr =>
      {
        if (this.level != null && (double) this.X > (double) this.level.Camera.Left + 64.0 && (double) this.X < (double) this.level.Camera.Right - 64.0 && (spr.Equals("peck") || spr.Equals("peckRare")) && this.Sprite.CurrentAnimationFrame == 6)
          Audio.Play("event:/game/general/bird_peck", this.Position);
        if (this.level == null || this.level.Session.Area.ID != 10 || this.DisableFlapSfx)
          return;
        BirdNPC.FlapSfxCheck(this.Sprite);
      });
      this.Add((Component) (this.Light = new VertexLight(new Vector2(0.0f, -8f), Color.White, 1f, 8, 32)));
      this.StartPosition = this.Position;
      this.SetMode(mode);
    }

    public BirdNPC(EntityData data, Vector2 offset)
      : this(data.Position + offset, data.Enum<BirdNPC.Modes>(nameof (mode), BirdNPC.Modes.None))
    {
      this.EntityID = new EntityID(data.Level.Name, data.ID);
      this.nodes = data.NodesOffset(offset);
      this.onlyOnce = data.Bool(nameof (onlyOnce));
      this.onlyIfPlayerLeft = data.Bool(nameof (onlyIfPlayerLeft));
    }

    public void SetMode(BirdNPC.Modes mode)
    {
      this.mode = mode;
      if (this.tutorialRoutine != null)
        this.tutorialRoutine.RemoveSelf();
      switch (mode)
      {
        case BirdNPC.Modes.ClimbingTutorial:
          this.Add((Component) (this.tutorialRoutine = new Coroutine(this.ClimbingTutorial())));
          break;
        case BirdNPC.Modes.DashingTutorial:
          this.Add((Component) (this.tutorialRoutine = new Coroutine(this.DashingTutorial())));
          break;
        case BirdNPC.Modes.DreamJumpTutorial:
          this.Add((Component) (this.tutorialRoutine = new Coroutine(this.DreamJumpTutorial())));
          break;
        case BirdNPC.Modes.SuperWallJumpTutorial:
          this.Add((Component) (this.tutorialRoutine = new Coroutine(this.SuperWallJumpTutorial())));
          break;
        case BirdNPC.Modes.HyperJumpTutorial:
          this.Add((Component) (this.tutorialRoutine = new Coroutine(this.HyperJumpTutorial())));
          break;
        case BirdNPC.Modes.FlyAway:
          this.Add((Component) (this.tutorialRoutine = new Coroutine(this.WaitRoutine())));
          break;
        case BirdNPC.Modes.Sleeping:
          this.Sprite.Play("sleep");
          this.Facing = Facings.Right;
          break;
        case BirdNPC.Modes.MoveToNodes:
          this.Add((Component) (this.tutorialRoutine = new Coroutine(this.MoveToNodesRoutine())));
          break;
        case BirdNPC.Modes.WaitForLightningOff:
          this.Add((Component) (this.tutorialRoutine = new Coroutine(this.WaitForLightningOffRoutine())));
          break;
      }
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
      if (this.mode == BirdNPC.Modes.SuperWallJumpTutorial)
      {
        Player entity = scene.Tracker.GetEntity<Player>();
        if (entity != null && (double) entity.Y < (double) this.Y + 32.0)
          this.RemoveSelf();
      }
      if (!this.onlyIfPlayerLeft)
        return;
      Player entity1 = this.level.Tracker.GetEntity<Player>();
      if (entity1 == null || (double) entity1.X <= (double) this.X)
        return;
      this.RemoveSelf();
    }

    public override bool IsRiding(Solid solid) => this.Scene.CollideCheck(new Rectangle((int) this.X - 4, (int) this.Y, 8, 2), (Entity) solid);

    public override void Update()
    {
      this.Sprite.Scale.X = (float) this.Facing;
      base.Update();
    }

    public IEnumerator Caw()
    {
      BirdNPC birdNpc = this;
      birdNpc.Sprite.Play("croak");
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
      birdNpc.level.Session.SetFlag(BirdNPC.FlownFlag + birdNpc.level.Session.Level);
      yield return (object) birdNpc.Startle("event:/game/general/bird_startle");
      yield return (object) birdNpc.FlyAway();
    }

    public IEnumerator FlyAway(float upwardsMultiplier = 1f)
    {
      BirdNPC birdNpc = this;
      if (birdNpc.staticMover != null)
      {
        birdNpc.staticMover.RemoveSelf();
        birdNpc.staticMover = (StaticMover) null;
      }
      birdNpc.Sprite.Play("fly");
      birdNpc.Facing = ToFacing.Convert(-(int)birdNpc.Facing);
      Vector2 speed = new Vector2((float) ((int) birdNpc.Facing * 20), -40f * upwardsMultiplier);
      while ((double) birdNpc.Y > (double) birdNpc.level.Bounds.Top)
      {
        speed += new Vector2((float) ((int) birdNpc.Facing * 140), -120f * upwardsMultiplier) * Engine.DeltaTime;
        birdNpc.Position = birdNpc.Position + speed * Engine.DeltaTime;
        yield return (object) null;
      }
      birdNpc.RemoveSelf();
    }

    private IEnumerator ClimbingTutorial()
    {
      BirdNPC birdNpc = this;
      yield return (object) 0.25f;
      Player p = birdNpc.Scene.Tracker.GetEntity<Player>();
      while ((double) Math.Abs(p.X - birdNpc.X) > 120.0)
        yield return (object) null;
      BirdTutorialGui tut1 = new BirdTutorialGui((Entity) birdNpc, new Vector2(0.0f, -16f), (object) Dialog.Clean("tutorial_climb"), new object[2]
      {
        (object) Dialog.Clean("tutorial_hold"),
        (object) BirdTutorialGui.ButtonPrompt.Grab
      });
      BirdTutorialGui tut2 = new BirdTutorialGui((Entity) birdNpc, new Vector2(0.0f, -16f), (object) Dialog.Clean("tutorial_climb"), new object[3]
      {
        (object) BirdTutorialGui.ButtonPrompt.Grab,
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
          yield return (object) birdNpc.ShowTutorial(tut2);
        }
        while (p.Scene != null && (!p.OnGround() || p.StateMachine.State == 1))
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
      bird.Y = (float) bird.level.Bounds.Top;
      bird.X += 32f;
      yield return (object) 1f;
      Player player = bird.Scene.Tracker.GetEntity<Player>();
      Bridge bridge = bird.Scene.Entities.FindFirst<Bridge>();
      while ((player == null || (double) player.X <= (double) bird.StartPosition.X - 92.0 || (double) player.Y <= (double) bird.StartPosition.Y - 20.0 || (double) player.Y >= (double) bird.StartPosition.Y - 10.0) && (!SaveData.Instance.Assists.Invincible || player == null || (double) player.X <= (double) bird.StartPosition.X - 60.0 || (double) player.Y <= (double) bird.StartPosition.Y || (double) player.Y >= (double) bird.StartPosition.Y + 34.0))
        yield return (object) null;
      bird.Scene.Add((Entity) new CS00_Ending(player, bird, bridge));
    }

    private IEnumerator DreamJumpTutorial()
    {
      BirdNPC birdNpc = this;
      yield return (object) birdNpc.ShowTutorial(new BirdTutorialGui((Entity) birdNpc, new Vector2(0.0f, -16f), (object) Dialog.Clean("tutorial_dreamjump"), new object[3]
      {
        (object) new Vector2(1f, 0.0f),
        (object) "+",
        (object) BirdTutorialGui.ButtonPrompt.Jump
      }), true);
      while (true)
      {
        Player entity = birdNpc.Scene.Tracker.GetEntity<Player>();
        if (entity == null || (double) entity.X <= (double) birdNpc.X && (double) (birdNpc.Position - entity.Position).Length() >= 32.0)
          yield return (object) null;
        else
          break;
      }
      yield return (object) birdNpc.HideTutorial();
      while (true)
      {
        Player entity = birdNpc.Scene.Tracker.GetEntity<Player>();
        if (entity == null || (double) (birdNpc.Position - entity.Position).Length() >= 24.0)
          yield return (object) null;
        else
          break;
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
        (object) Dialog.Clean("TUTORIAL_DASH"),
        (object) new Vector2(0.0f, -1f)
      });
      BirdTutorialGui tut2 = new BirdTutorialGui((Entity) birdNpc, new Vector2(0.0f, -16f), (object) GFX.Gui["hyperjump/tutorial01"], new object[1]
      {
        (object) Dialog.Clean("TUTORIAL_DREAMJUMP")
      });
      Player entity;
      do
      {
        yield return (object) birdNpc.ShowTutorial(tut1, caw);
        birdNpc.Sprite.Play("idleRarePeck");
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
      BirdTutorialGui tut = new BirdTutorialGui((Entity) birdNpc, new Vector2(0.0f, -16f), (object) Dialog.Clean("TUTORIAL_DREAMJUMP"), new object[5]
      {
        (object) new Vector2(1f, 1f),
        (object) "+",
        (object) BirdTutorialGui.ButtonPrompt.Dash,
        (object) GFX.Gui["tinyarrow"],
        (object) BirdTutorialGui.ButtonPrompt.Jump
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
        if (entity == null || (double) (entity.Center - birdNpc.Position).Length() >= 32.0)
          yield return (object) null;
        else
          break;
      }
      yield return (object) birdNpc.StartleAndFlyAway();
    }

    public IEnumerator Startle(string startleSound, float duration = 0.8f, Vector2? multiplier = null)
    {
      BirdNPC birdNpc = this;
      if (!multiplier.HasValue)
        multiplier = new Vector2?(new Vector2(1f, 1f));
      if (!string.IsNullOrWhiteSpace(startleSound))
        Audio.Play(startleSound, birdNpc.Position);
      Dust.Burst(birdNpc.Position, -1.5707964f, 8);
      birdNpc.Sprite.Play("jump");
      Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeOut, duration, true);
      tween.OnUpdate = (Action<Tween>) (t =>
      {
        if ((double) t.Eased < 0.5 && this.Scene.OnInterval(0.05f))
          this.level.Particles.Emit(BirdNPC.P_Feather, 2, this.Position + Vector2.UnitY * -6f, Vector2.One * 4f);
        Vector2 vector2 = Vector2.Lerp(new Vector2(100f, -100f) * multiplier.Value, new Vector2(20f, -20f) * multiplier.Value, t.Eased);
        vector2.X *= (float) -(int) this.Facing;
        this.Position = this.Position + vector2 * Engine.DeltaTime;
      });
      birdNpc.Add((Component) tween);
      while (tween.Active)
        yield return (object) null;
    }

    public IEnumerator FlyTo(Vector2 target, float durationMult = 1f, bool relocateSfx = true)
    {
      BirdNPC birdNpc = this;
      birdNpc.Sprite.Play("fly");
      if (relocateSfx)
        birdNpc.Add((Component) new SoundSource().Play("event:/new_content/game/10_farewell/bird_relocate"));
      int num = Math.Sign(target.X - birdNpc.X);
      if (num != 0)
        birdNpc.Facing = (Facings) num;
      Vector2 position = birdNpc.Position;
      Vector2 end = target;
      SimpleCurve curve = new SimpleCurve(position, end, position + (end - position) * 0.75f - Vector2.UnitY * 30f);
      float duration = (end - position).Length() / 100f * durationMult;
      for (float p = 0.0f; (double) p < 0.949999988079071; p += Engine.DeltaTime / duration)
      {
        birdNpc.Position = curve.GetPoint(Ease.SineInOut(p)).Floor();
        birdNpc.Sprite.Rate = (float) (1.0 - (double) p * 0.5);
        yield return (object) null;
      }
      Dust.Burst(birdNpc.Position, -1.5707964f, 8);
      birdNpc.Position = target;
      birdNpc.Facing = Facings.Left;
      birdNpc.Sprite.Rate = 1f;
      birdNpc.Sprite.Play("idle");
    }

    private IEnumerator MoveToNodesRoutine()
    {
      BirdNPC birdNpc = this;
      int index = 0;
      while (true)
      {
        Player entity = birdNpc.Scene.Tracker.GetEntity<Player>();
        if (entity == null || (double) (entity.Center - birdNpc.Position).Length() >= 80.0)
        {
          yield return (object) null;
        }
        else
        {
          birdNpc.Depth = -1000000;
          yield return (object) birdNpc.Startle("event:/new_content/game/10_farewell/bird_startle", 0.2f);
          if (index < birdNpc.nodes.Length)
          {
            yield return (object) birdNpc.FlyTo(birdNpc.nodes[index], 0.6f);
            ++index;
          }
          else
          {
            birdNpc.Tag = (int) Tags.Persistent;
            birdNpc.Add((Component) new SoundSource().Play("event:/new_content/game/10_farewell/bird_relocate"));
            if (birdNpc.onlyOnce)
              birdNpc.level.Session.DoNotLoad.Add(birdNpc.EntityID);
            birdNpc.Sprite.Play("fly");
            birdNpc.Facing = Facings.Right;
            Vector2 speed = new Vector2((float) ((int) birdNpc.Facing * 20), -40f);
            while ((double) birdNpc.Y > (double) (birdNpc.level.Bounds.Top - 200))
            {
              speed += new Vector2((float) ((int) birdNpc.Facing * 140), -60f) * Engine.DeltaTime;
              birdNpc.Position = birdNpc.Position + speed * Engine.DeltaTime;
              yield return (object) null;
            }
            birdNpc.RemoveSelf();
            speed = new Vector2();
          }
        }
      }
    }

    private IEnumerator WaitForLightningOffRoutine()
    {
      BirdNPC birdNpc = this;
      birdNpc.Sprite.Play("hoverStressed");
      while (birdNpc.Scene.Entities.FindFirst<Lightning>() != null)
        yield return (object) null;
      if ((double) birdNpc.WaitForLightningPostDelay > 0.0)
        yield return (object) birdNpc.WaitForLightningPostDelay;
      Vector2 speed;
      if (!birdNpc.FlyAwayUp)
      {
        birdNpc.Sprite.Play("fly");
        speed = new Vector2((float) ((int) birdNpc.Facing * 20), -10f);
        while ((double) birdNpc.Y > (double) birdNpc.level.Bounds.Top)
        {
          speed += new Vector2((float) ((int) birdNpc.Facing * 140), -10f) * Engine.DeltaTime;
          birdNpc.Position = birdNpc.Position + speed * Engine.DeltaTime;
          yield return (object) null;
        }
        speed = new Vector2();
      }
      else
      {
        birdNpc.Sprite.Play("flyup");
        speed = new Vector2(0.0f, -32f);
        while ((double) birdNpc.Y > (double) birdNpc.level.Bounds.Top)
        {
          speed += new Vector2(0.0f, -100f) * Engine.DeltaTime;
          birdNpc.Position = birdNpc.Position + speed * Engine.DeltaTime;
          yield return (object) null;
        }
        speed = new Vector2();
      }
      birdNpc.RemoveSelf();
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
      float right1 = (float) this.level.Bounds.Right;
      float y1 = this.StartPosition.Y - 20f;
      float y2 = this.StartPosition.Y - 10f;
      Draw.Line(new Vector2(x1, y1), new Vector2(x1, y2), Color.Aqua);
      Draw.Line(new Vector2(x1, y1), new Vector2(right1, y1), Color.Aqua);
      Draw.Line(new Vector2(right1, y1), new Vector2(right1, y2), Color.Aqua);
      Draw.Line(new Vector2(x1, y2), new Vector2(right1, y2), Color.Aqua);
      float x2 = this.StartPosition.X - 60f;
      float right2 = (float) this.level.Bounds.Right;
      float y3 = this.StartPosition.Y;
      float y4 = this.StartPosition.Y + 34f;
      Draw.Line(new Vector2(x2, y3), new Vector2(x2, y4), Color.Aqua);
      Draw.Line(new Vector2(x2, y3), new Vector2(right2, y3), Color.Aqua);
      Draw.Line(new Vector2(right2, y3), new Vector2(right2, y4), Color.Aqua);
      Draw.Line(new Vector2(x2, y4), new Vector2(right2, y4), Color.Aqua);
    }

    public static void FlapSfxCheck(Sprite sprite)
    {
      if (sprite.Entity != null && sprite.Entity.Scene != null)
      {
        Camera camera = (sprite.Entity.Scene as Level).Camera;
        Vector2 renderPosition = sprite.RenderPosition;
        if ((double) renderPosition.X < (double) camera.X - 32.0 || (double) renderPosition.Y < (double) camera.Y - 32.0 || (double) renderPosition.X > (double) camera.X + 320.0 + 32.0 || (double) renderPosition.Y > (double) camera.Y + 180.0 + 32.0)
          return;
      }
      string currentAnimationId = sprite.CurrentAnimationID;
      int currentAnimationFrame = sprite.CurrentAnimationFrame;
      if ((!(currentAnimationId == "hover") || currentAnimationFrame != 0) && (!(currentAnimationId == "hoverStressed") || currentAnimationFrame != 0) && (!(currentAnimationId == "fly") || currentAnimationFrame != 0))
        return;
      Audio.Play("event:/new_content/game/10_farewell/bird_wingflap", sprite.RenderPosition);
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
      MoveToNodes,
      WaitForLightningOff,
    }
  }
}
