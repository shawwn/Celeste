// Decompiled with JetBrains decompiler
// Type: Celeste.CS03_Ending
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class CS03_Ending : CutsceneEntity
  {
    public const string Flag = "oshiroEnding";
    private ResortRoofEnding roof;
    private AngryOshiro angryOshiro;
    private Player player;
    private Entity oshiro;
    private Sprite oshiroSprite;
    private EventInstance smashSfx;
    private bool smashRumble;

    public CS03_Ending(ResortRoofEnding roof, Player player)
      : base(false, true)
    {
      this.roof = roof;
      this.player = player;
      this.Depth = -1000000;
    }

    public override void OnBegin(Level level)
    {
      level.RegisterAreaComplete();
      this.Add((Component) new Coroutine(this.Cutscene(level), true));
    }

    public override void Update()
    {
      base.Update();
      if (!this.smashRumble)
        return;
      Input.Rumble(RumbleStrength.Light, RumbleLength.Short);
    }

    private IEnumerator Cutscene(Level level)
    {
      this.player.StateMachine.State = Player.StDummy;
      this.player.StateMachine.Locked = true;
      this.player.ForceCameraUpdate = false;
      this.Add((Component) new Coroutine(this.player.DummyRunTo((float) ((double) this.roof.X + (double) this.roof.Width - 32.0), true), true));
      yield return (object) null;
      this.player.DummyAutoAnimate = false;
      yield return (object) 0.5f;
      this.angryOshiro = this.Scene.Entities.FindFirst<AngryOshiro>();
      this.Add((Component) new Coroutine(this.MoveGhostTo(new Vector2(this.roof.X + 40f, this.roof.Y - 12f)), true));
      yield return (object) 1f;
      this.player.DummyAutoAnimate = true;
      yield return (object) level.ZoomTo(new Vector2(130f, 60f), 2f, 0.5f);
      this.player.Facing = Facings.Left;
      yield return (object) 0.5f;
      yield return (object) Textbox.Say("CH3_OSHIRO_CHASE_END", new Func<IEnumerator>(this.GhostSmash));
      yield return (object) this.GhostSmash(0.5f, true);
      Audio.SetMusic((string) null, true, true);
      this.oshiroSprite = (Sprite) null;
      CS03_Ending.BgFlash bgFlash = new CS03_Ending.BgFlash();
      bgFlash.Alpha = 1f;
      level.Add((Entity) bgFlash);
      Distort.GameRate = 0.0f;
      Sprite lightning = GFX.SpriteBank.Create("oshiro_boss_lightning");
      lightning.Position = this.angryOshiro.Position + new Vector2(140f, -100f);
      lightning.Rotation = Calc.Angle(lightning.Position, this.angryOshiro.Position + new Vector2(0.0f, 10f));
      lightning.Play("once", false, false);
      this.Add((Component) lightning);
      yield return (object) null;
      Celeste.Freeze(0.3f);
      yield return (object) null;
      level.Shake(0.3f);
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Long);
      this.smashRumble = false;
      yield return (object) 0.2f;
      Distort.GameRate = 1f;
      level.Flash(Color.White, false);
      this.player.DummyGravity = false;
      this.angryOshiro.Sprite.Play("transformBack", false, false);
      this.player.Sprite.Play("fall", false, false);
      this.roof.BeginFalling = true;
      yield return (object) null;
      Engine.TimeRate = 0.01f;
      this.player.Sprite.Play("fallFast", false, false);
      this.player.DummyGravity = true;
      this.player.Speed.Y = -200f;
      this.player.Speed.X = 300f;
      Vector2 oshiroFallSpeed = new Vector2(-100f, -250f);
      Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.SineInOut, 1.5f, true);
      tween.OnUpdate = (Action<Tween>) (t => this.angryOshiro.Sprite.Rotation = (float) ((double) t.Eased * -100.0 * (Math.PI / 180.0)));
      this.Add((Component) tween);
      for (float t = 0.0f; (double) t < 2.0; t += Engine.DeltaTime)
      {
        oshiroFallSpeed.X = Calc.Approach(oshiroFallSpeed.X, 0.0f, Engine.DeltaTime * 400f);
        oshiroFallSpeed.Y += Engine.DeltaTime * 800f;
        AngryOshiro angryOshiro = this.angryOshiro;
        angryOshiro.Position = angryOshiro.Position + oshiroFallSpeed * Engine.DeltaTime;
        bgFlash.Alpha = Calc.Approach(bgFlash.Alpha, 0.0f, Engine.RawDeltaTime);
        Engine.TimeRate = Calc.Approach(Engine.TimeRate, 1f, Engine.RawDeltaTime * 0.6f);
        yield return (object) null;
      }
      level.DirectionalShake(new Vector2(0.0f, -1f), 0.5f);
      Input.Rumble(RumbleStrength.Medium, RumbleLength.Long);
      yield return (object) 1f;
      bgFlash = (CS03_Ending.BgFlash) null;
      lightning = (Sprite) null;
      oshiroFallSpeed = new Vector2();
      tween = (Tween) null;
      while (!this.player.OnGround(1))
        this.player.MoveV(1f, (Collision) null, (Solid) null);
      this.player.DummyAutoAnimate = false;
      this.player.Sprite.Play("tired", false, false);
      this.angryOshiro.RemoveSelf();
      this.Scene.Add(this.oshiro = new Entity(new Vector2((float) (level.Bounds.Left + 110), this.player.Y)));
      this.oshiro.Add((Component) (this.oshiroSprite = GFX.SpriteBank.Create("oshiro")));
      this.oshiroSprite.Play("fall", false, false);
      this.oshiroSprite.Scale.X = 1f;
      this.oshiro.Collider = (Collider) new Hitbox(8f, 8f, -4f, -8f);
      this.oshiro.Add((Component) new VertexLight(new Vector2(0.0f, -8f), Color.White, 1f, 16, 32));
      yield return (object) CutsceneEntity.CameraTo(this.player.CameraTarget + new Vector2(0.0f, 40f), 1f, Ease.CubeOut, 0.0f);
      yield return (object) 1.5f;
      Audio.SetMusic("event:/music/lvl3/intro", true, true);
      yield return (object) 3f;
      Audio.Play("event:/char/oshiro/chat_get_up", this.oshiro.Position);
      this.oshiroSprite.Play("recover", false, false);
      float target = this.oshiro.Y + 4f;
      while ((double) this.oshiro.Y != (double) target)
      {
        this.oshiro.Y = Calc.Approach(this.oshiro.Y, target, 6f * Engine.DeltaTime);
        yield return (object) null;
      }
      yield return (object) 0.6f;
      yield return (object) Textbox.Say("CH3_ENDING", new Func<IEnumerator>(this.OshiroTurns));
      this.Add((Component) new Coroutine(CutsceneEntity.CameraTo(level.Camera.Position + new Vector2(-80f, 0.0f), 3f, (Ease.Easer) null, 0.0f), true));
      yield return (object) 0.5f;
      this.oshiroSprite.Scale.X = -1f;
      yield return (object) 0.2f;
      float timer = 0.0f;
      this.oshiro.Add((Component) new SoundSource("event:/char/oshiro/move_08_roof07_exit"));
      while ((double) this.oshiro.X > (double) (level.Bounds.Left - 16))
      {
        this.oshiro.X -= 40f * Engine.DeltaTime;
        this.oshiroSprite.Y = (float) Math.Sin((double) (timer += Engine.DeltaTime * 2f)) * 2f;
        Door door = this.oshiro.CollideFirst<Door>();
        if (door != null)
          door.Open(this.oshiro.X);
        yield return (object) null;
        door = (Door) null;
      }
      this.Add((Component) new Coroutine(CutsceneEntity.CameraTo(level.Camera.Position + new Vector2(80f, 0.0f), 2f, (Ease.Easer) null, 0.0f), true));
      yield return (object) 1.2f;
      this.player.DummyAutoAnimate = true;
      yield return (object) this.player.DummyWalkTo(this.player.X - 16f, false, 1f, false);
      yield return (object) 2f;
      this.player.Facing = Facings.Right;
      yield return (object) 1f;
      this.player.ForceCameraUpdate = false;
      this.player.Add((Component) new Coroutine(this.RunPlayerRight(), true));
      this.EndCutscene(level, true);
    }

    private IEnumerator OshiroTurns()
    {
      yield return (object) 1f;
      this.oshiroSprite.Scale.X = -1f;
      yield return (object) 0.2f;
    }

    private IEnumerator MoveGhostTo(Vector2 target)
    {
      if (this.angryOshiro != null)
      {
        target.Y -= this.angryOshiro.Height / 2f;
        this.angryOshiro.EnterDummyMode();
        this.angryOshiro.Collidable = false;
        while (this.angryOshiro.Position != target)
        {
          this.angryOshiro.Position = Calc.Approach(this.angryOshiro.Position, target, 64f * Engine.DeltaTime);
          yield return (object) null;
        }
      }
    }

    private IEnumerator GhostSmash()
    {
      yield return (object) this.GhostSmash(0.0f, false);
    }

    private IEnumerator GhostSmash(float topDelay, bool final)
    {
      if (this.angryOshiro != null)
      {
        this.smashSfx = !final ? Audio.Play("event:/char/oshiro/boss_slam_first", this.angryOshiro.Position) : Audio.Play("event:/char/oshiro/boss_slam_final", this.angryOshiro.Position);
        float from = this.angryOshiro.Y;
        float to = this.angryOshiro.Y - 32f;
        for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime * 2f)
        {
          this.angryOshiro.Y = MathHelper.Lerp(from, to, Ease.CubeOut(p));
          yield return (object) null;
        }
        yield return (object) topDelay;
        float ground = from + 20f;
        for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime * 8f)
        {
          this.angryOshiro.Y = MathHelper.Lerp(to, ground, Ease.CubeOut(p));
          yield return (object) null;
        }
        this.angryOshiro.Squish();
        this.Level.Shake(0.5f);
        Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
        this.smashRumble = true;
        this.roof.StartShaking(0.5f);
        if (!final)
        {
          for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime * 16f)
          {
            this.angryOshiro.Y = MathHelper.Lerp(ground, from, Ease.CubeOut(p));
            yield return (object) null;
          }
        }
        else
          this.angryOshiro.Y = (float) (((double) ground + (double) from) / 2.0);
        if (this.angryOshiro != null)
        {
          this.player.DummyAutoAnimate = false;
          this.player.Sprite.Play("shaking", false, false);
          this.roof.Wobble(this.angryOshiro, final);
          if (!final)
            yield return (object) 0.5f;
        }
      }
    }

    private IEnumerator RunPlayerRight()
    {
      yield return (object) 0.75f;
      yield return (object) this.player.DummyRunTo(this.player.X + 128f, false);
    }

    public override void OnEnd(Level level)
    {
      Audio.SetMusic("event:/music/lvl3/intro", true, true);
      Audio.Stop(this.smashSfx, true);
      level.CompleteArea(true, false);
      SpotlightWipe.FocusPoint = new Vector2(192f, 120f);
    }

    private class BgFlash : Entity
    {
      public float Alpha = 0.0f;

      public BgFlash()
      {
        this.Depth = 10100;
      }

      public override void Render()
      {
        Camera camera = (this.Scene as Level).Camera;
        Draw.Rect(camera.X - 10f, camera.Y - 10f, 340f, 200f, Color.Black * this.Alpha);
      }
    }
  }
}

