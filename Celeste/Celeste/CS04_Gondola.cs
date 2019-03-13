// Decompiled with JetBrains decompiler
// Type: Celeste.CS04_Gondola
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
  public class CS04_Gondola : CutsceneEntity
  {
    private List<ReflectionTentacles> tentacles = new List<ReflectionTentacles>();
    private float gondolaPercent = 0.0f;
    private float gondolaSpeed = 0.0f;
    private float shakeTimer = 0.0f;
    private float anxiety = 0.0f;
    private float anxietyStutter = 0.0f;
    private float anxietyRumble = 0.0f;
    private CS04_Gondola.GondolaStates gondolaState = CS04_Gondola.GondolaStates.Stopped;
    private NPC theo;
    private Gondola gondola;
    private Player player;
    private BadelineDummy evil;
    private Parallax loopingCloud;
    private Parallax bottomCloud;
    private WindSnowFG windSnowFg;
    private float LoopCloudsAt;
    private SoundSource moveLoopSfx;
    private SoundSource haltLoopSfx;
    private bool AutoSnapCharacters;
    private float theoXOffset;
    private float playerXOffset;
    private const float gondolaMaxSpeed = 64f;
    private BreathingRumbler rumbler;

    public CS04_Gondola(NPC theo, Gondola gondola, Player player)
      : base(false, true)
    {
      this.theo = theo;
      this.gondola = gondola;
      this.player = player;
    }

    public override void OnBegin(Level level)
    {
      level.RegisterAreaComplete();
      foreach (Backdrop backdrop in level.Foreground.Backdrops)
      {
        if (backdrop is WindSnowFG)
          this.windSnowFg = backdrop as WindSnowFG;
      }
      this.Add((Component) (this.moveLoopSfx = new SoundSource()));
      this.Add((Component) (this.haltLoopSfx = new SoundSource()));
      this.Add((Component) new Coroutine(this.Cutscene(), true));
    }

    private IEnumerator Cutscene()
    {
      this.player.StateMachine.State = Player.StDummy;
      yield return (object) this.player.DummyWalkToExact((int) this.gondola.X + 16, false, 1f);
      while (!this.player.OnGround(1))
        yield return (object) null;
      Audio.SetMusic("event:/music/lvl1/theo", true, true);
      yield return (object) Textbox.Say("CH4_GONDOLA", new Func<IEnumerator>(this.EnterTheo), new Func<IEnumerator>(this.CheckOnTheo), new Func<IEnumerator>(this.GetUpTheo), new Func<IEnumerator>(this.LookAtLever), new Func<IEnumerator>(this.PullLever), new Func<IEnumerator>(this.WaitABit), new Func<IEnumerator>(this.WaitForCenter), new Func<IEnumerator>(this.SelfieThenStallsOut), new Func<IEnumerator>(this.MovePlayerLeft), new Func<IEnumerator>(this.SnapLeverOff), new Func<IEnumerator>(this.DarknessAppears), new Func<IEnumerator>(this.DarknessConsumes), new Func<IEnumerator>(this.CantBreath), new Func<IEnumerator>(this.StartBreathing), new Func<IEnumerator>(this.Ascend), new Func<IEnumerator>(this.WaitABit), new Func<IEnumerator>(this.TheoTakesOutPhone), new Func<IEnumerator>(this.FaceTheo));
      yield return (object) this.ShowPhoto();
      this.EndCutscene(this.Level, true);
    }

    public override void OnEnd(Level level)
    {
      if (this.rumbler != null)
      {
        this.rumbler.RemoveSelf();
        this.rumbler = (BreathingRumbler) null;
      }
      level.CompleteArea(true, false);
      if (this.WasSkipped)
        return;
      SpotlightWipe.Modifier = 120f;
      SpotlightWipe.FocusPoint = new Vector2(320f, 180f) / 2f;
    }

    private IEnumerator EnterTheo()
    {
      this.player.Facing = Facings.Left;
      yield return (object) 0.2f;
      yield return (object) this.PanCamera(new Vector2((float) this.Level.Bounds.Left, this.theo.Y - 90f), 1f, (Ease.Easer) null);
      this.theo.Visible = true;
      float theoStartX = this.theo.X;
      yield return (object) this.theo.MoveTo(new Vector2(theoStartX + 35f, this.theo.Y), false, new int?(), false);
      yield return (object) 0.6f;
      yield return (object) this.theo.MoveTo(new Vector2(theoStartX + 60f, this.theo.Y), false, new int?(), false);
      Audio.Play("event:/game/04_cliffside/gondola_theo_fall", this.theo.Position);
      this.theo.Sprite.Play("idleEdge", false, false);
      yield return (object) 1f;
      this.theo.Sprite.Play("falling", false, false);
      this.theo.X += 4f;
      this.theo.Depth = -10010;
      float speed = 80f;
      while ((double) this.theo.Y < (double) this.player.Y)
      {
        this.theo.Y += speed * Engine.DeltaTime;
        speed += 120f * Engine.DeltaTime;
        yield return (object) null;
      }
      this.Level.DirectionalShake(new Vector2(0.0f, 1f), 0.3f);
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
      this.theo.Y = this.player.Y;
      this.theo.Sprite.Play("hitGround", false, false);
      this.theo.Sprite.Rate = 0.0f;
      this.theo.Depth = 1000;
      this.theo.Sprite.Scale = new Vector2(1.3f, 0.8f);
      yield return (object) 0.5f;
      Vector2 start = this.theo.Sprite.Scale;
      Tween tween = Tween.Create(Tween.TweenMode.Oneshot, (Ease.Easer) null, 2f, true);
      tween.OnUpdate = (Action<Tween>) (t =>
      {
        this.theo.Sprite.Scale.X = MathHelper.Lerp(start.X, 1f, t.Eased);
        this.theo.Sprite.Scale.Y = MathHelper.Lerp(start.Y, 1f, t.Eased);
      });
      this.Add((Component) tween);
      yield return (object) this.PanCamera(new Vector2((float) this.Level.Bounds.Left, this.theo.Y - 120f), 1f, (Ease.Easer) null);
      yield return (object) 0.6f;
    }

    private IEnumerator CheckOnTheo()
    {
      yield return (object) this.player.DummyWalkTo(this.gondola.X - 18f, false, 1f, false);
    }

    private IEnumerator GetUpTheo()
    {
      yield return (object) 1.4f;
      Audio.Play("event:/game/04_cliffside/gondola_theo_recover", this.theo.Position);
      this.theo.Sprite.Rate = 1f;
      this.theo.Sprite.Play("recoverGround", false, false);
      yield return (object) 1.6f;
      yield return (object) this.theo.MoveTo(new Vector2(this.gondola.X - 50f, this.player.Y), false, new int?(), false);
      yield return (object) 0.2f;
    }

    private IEnumerator LookAtLever()
    {
      yield return (object) this.theo.MoveTo(new Vector2(this.gondola.X + 7f, this.theo.Y), false, new int?(), false);
      this.player.Facing = Facings.Right;
      this.theo.Sprite.Scale.X = -1f;
    }

    private IEnumerator PullLever()
    {
      this.Add((Component) new Coroutine(this.player.DummyWalkToExact((int) this.gondola.X - 7, false, 1f), true));
      this.theo.Sprite.Scale.X = -1f;
      yield return (object) 0.2f;
      Audio.Play("event:/game/04_cliffside/gondola_theo_lever_start", this.theo.Position);
      this.theo.Sprite.Play("pullVent", false, false);
      yield return (object) 1f;
      Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
      this.gondola.Lever.Play("pulled", false, false);
      this.theo.Sprite.Play("fallVent", false, false);
      yield return (object) 0.6f;
      this.Level.Shake(0.3f);
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Long);
      yield return (object) 0.5f;
      yield return (object) this.PanCamera(this.gondola.Position + new Vector2(-160f, -120f), 1f, (Ease.Easer) null);
      yield return (object) 0.5f;
      this.Level.Background.Backdrops.Add((Backdrop) (this.loopingCloud = new Parallax(GFX.Game["bgs/04/bgCloudLoop"])));
      this.Level.Background.Backdrops.Add((Backdrop) (this.bottomCloud = new Parallax(GFX.Game["bgs/04/bgCloud"])));
      this.loopingCloud.LoopX = this.bottomCloud.LoopX = true;
      this.loopingCloud.LoopY = this.bottomCloud.LoopY = false;
      this.loopingCloud.Position.Y = this.Level.Camera.Top - (float) this.loopingCloud.Texture.Height - (float) this.bottomCloud.Texture.Height;
      this.bottomCloud.Position.Y = this.Level.Camera.Top - (float) this.bottomCloud.Texture.Height;
      this.LoopCloudsAt = this.bottomCloud.Position.Y;
      this.AutoSnapCharacters = true;
      this.theoXOffset = this.theo.X - this.gondola.X;
      this.playerXOffset = this.player.X - this.gondola.X;
      this.player.StateMachine.State = Player.StFrozen;
      Tween tween = Tween.Create(Tween.TweenMode.Oneshot, (Ease.Easer) null, 16f, true);
      tween.OnUpdate = (Action<Tween>) (t =>
      {
        if (!(Audio.CurrentMusic == "event:/music/lvl1/theo"))
          return;
        Audio.SetMusicParam("fade", 1f - t.Eased);
      });
      this.Add((Component) tween);
      SoundSource sfx = new SoundSource();
      sfx.Position = this.gondola.LeftCliffside.Position;
      sfx.Play("event:/game/04_cliffside/gondola_cliffmechanism_start", (string) null, 0.0f);
      this.Add((Component) sfx);
      this.moveLoopSfx.Play("event:/game/04_cliffside/gondola_movement_loop", (string) null, 0.0f);
      this.Level.Shake(0.3f);
      Input.Rumble(RumbleStrength.Strong, RumbleLength.FullSecond);
      this.gondolaSpeed = 32f;
      this.gondola.RotationSpeed = 1f;
      this.gondolaState = CS04_Gondola.GondolaStates.MovingToCenter;
      yield return (object) 1f;
      yield return (object) this.MoveTheoOnGondola(12f, false);
      yield return (object) 0.2f;
      this.theo.Sprite.Scale.X = -1f;
    }

    private IEnumerator WaitABit()
    {
      yield return (object) 1f;
    }

    private IEnumerator WaitForCenter()
    {
      while (this.gondolaState != CS04_Gondola.GondolaStates.InCenter)
        yield return (object) null;
      this.theo.Sprite.Scale.X = 1f;
      yield return (object) 1f;
      yield return (object) this.MovePlayerOnGondola(-20f);
      yield return (object) 0.5f;
    }

    private IEnumerator SelfieThenStallsOut()
    {
      Audio.SetMusic("event:/music/lvl4/minigame", true, true);
      this.Add((Component) new Coroutine(this.Level.ZoomTo(new Vector2(160f, 110f), 2f, 0.5f), true));
      yield return (object) 0.3f;
      this.theo.Sprite.Scale.X = 1f;
      yield return (object) 0.2f;
      this.Add((Component) new Coroutine(this.MovePlayerOnGondola(this.theoXOffset - 8f), true));
      yield return (object) 0.4f;
      Audio.Play("event:/game/04_cliffside/gondola_theoselfie_halt", this.theo.Position);
      this.theo.Sprite.Play("holdOutPhone", false, false);
      yield return (object) 1.5f;
      this.theoXOffset += 4f;
      this.playerXOffset += 4f;
      this.gondola.RotationSpeed = -1f;
      this.gondolaState = CS04_Gondola.GondolaStates.Stopped;
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Long);
      this.theo.Sprite.Play("takeSelfieImmediate", false, false);
      this.Add((Component) new Coroutine(this.PanCamera(this.gondola.Position + (this.gondola.Destination - this.gondola.Position).SafeNormalize() * 32f + new Vector2(-160f, -120f), 0.3f, Ease.CubeOut), true));
      yield return (object) 0.5f;
      this.Level.Flash(Color.White, false);
      this.Level.Add((Entity) (this.evil = new BadelineDummy(Vector2.Zero)));
      this.evil.Appear(this.Level, false);
      this.evil.Floatness = 0.0f;
      this.evil.Depth = -1000000;
      this.moveLoopSfx.Stop(true);
      this.haltLoopSfx.Play("event:/game/04_cliffside/gondola_halted_loop", (string) null, 0.0f);
      this.gondolaState = CS04_Gondola.GondolaStates.Shaking;
      yield return (object) this.PanCamera(this.gondola.Position + new Vector2(-160f, -120f), 1f, (Ease.Easer) null);
      yield return (object) 1f;
    }

    private IEnumerator MovePlayerLeft()
    {
      yield return (object) this.MovePlayerOnGondola(-20f);
      this.theo.Sprite.Scale.X = -1f;
      yield return (object) 0.5f;
      yield return (object) this.MovePlayerOnGondola(20f);
      yield return (object) 0.5f;
      yield return (object) this.MovePlayerOnGondola(-10f);
      yield return (object) 0.5f;
      this.player.Facing = Facings.Right;
    }

    private IEnumerator SnapLeverOff()
    {
      yield return (object) this.MoveTheoOnGondola(7f, true);
      Audio.Play("event:/game/04_cliffside/gondola_theo_lever_fail", this.theo.Position);
      this.theo.Sprite.Play("pullVent", false, false);
      yield return (object) 1f;
      this.theo.Sprite.Play("fallVent", false, false);
      yield return (object) 1f;
      this.gondola.BreakLever();
      Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
      this.Level.Shake(0.3f);
      yield return (object) 2.5f;
    }

    private IEnumerator DarknessAppears()
    {
      Audio.SetMusicParam("calm", 0.0f);
      yield return (object) 0.25f;
      this.player.Sprite.Play("tired", false, false);
      yield return (object) 0.25f;
      this.evil.Vanish();
      this.evil = (BadelineDummy) null;
      yield return (object) 0.3f;
      this.Level.NextColorGrade("panicattack");
      this.Level.Shake(0.3f);
      Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
      this.BurstTentacles(3, 90f, 200f);
      Audio.Play("event:/game/04_cliffside/gondola_scaryhair_01", this.gondola.Position);
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime / 2f)
      {
        yield return (object) null;
        this.Level.Background.Fade = p;
        this.anxiety = p;
        if (this.windSnowFg != null)
          this.windSnowFg.Alpha = 1f - p;
      }
      yield return (object) 0.25f;
    }

    private IEnumerator DarknessConsumes()
    {
      this.Level.Shake(0.3f);
      Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
      Audio.Play("event:/game/04_cliffside/gondola_scaryhair_02", this.gondola.Position);
      this.BurstTentacles(2, 60f, 200f);
      yield return (object) this.MoveTheoOnGondola(0.0f, true);
      this.theo.Sprite.Play("comfortStart", false, false);
    }

    private IEnumerator CantBreath()
    {
      this.Level.Shake(0.3f);
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
      Audio.Play("event:/game/04_cliffside/gondola_scaryhair_03", this.gondola.Position);
      this.BurstTentacles(1, 30f, 200f);
      this.BurstTentacles(0, 0.0f, 100f);
      this.rumbler = new BreathingRumbler();
      this.Scene.Add((Entity) this.rumbler);
      yield return (object) null;
    }

    private IEnumerator StartBreathing()
    {
      BreathingMinigame breathing = new BreathingMinigame(true, this.rumbler);
      this.Scene.Add((Entity) breathing);
      while (!breathing.Completed)
        yield return (object) null;
      foreach (ReflectionTentacles tentacle in this.tentacles)
      {
        ReflectionTentacles t = tentacle;
        t.RemoveSelf();
        t = (ReflectionTentacles) null;
      }
      this.anxiety = 0.0f;
      this.Level.Background.Fade = 0.0f;
      this.Level.SnapColorGrade((string) null);
      this.gondola.CancelPullSides();
      this.Level.ResetZoom();
      yield return (object) 0.5f;
      Audio.Play("event:/game/04_cliffside/gondola_restart", this.gondola.Position);
      yield return (object) 1f;
      this.moveLoopSfx.Play("event:/game/04_cliffside/gondola_movement_loop", (string) null, 0.0f);
      this.haltLoopSfx.Stop(true);
      this.Level.Shake(0.3f);
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Long);
      this.gondolaState = CS04_Gondola.GondolaStates.InCenter;
      this.gondola.RotationSpeed = 0.5f;
      yield return (object) 1.2f;
    }

    private IEnumerator Ascend()
    {
      this.gondolaState = CS04_Gondola.GondolaStates.MovingToEnd;
      while ((uint) this.gondolaState > 0U)
        yield return (object) null;
      this.Level.Shake(0.3f);
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Long);
      this.moveLoopSfx.Stop(true);
      Audio.Play("event:/game/04_cliffside/gondola_finish", this.gondola.Position);
      this.gondola.RotationSpeed = 0.5f;
      yield return (object) 0.1f;
      while ((double) this.gondola.Rotation > 0.0)
        yield return (object) null;
      this.gondola.Rotation = this.gondola.RotationSpeed = 0.0f;
      this.Level.Shake(0.3f);
      this.AutoSnapCharacters = false;
      this.player.StateMachine.State = Player.StDummy;
      this.player.Position = this.player.Position.Floor();
      while (this.player.CollideCheck<Solid>())
        --this.player.Y;
      this.theo.Position.Y = this.player.Position.Y;
      this.theo.Sprite.Play("comfortRecover", false, false);
      this.theo.Sprite.Scale.X = 1f;
      yield return (object) this.player.DummyWalkTo(this.gondola.X + 80f, false, 1f, false);
      this.player.DummyAutoAnimate = false;
      this.player.Sprite.Play("tired", false, false);
      yield return (object) this.theo.MoveTo(new Vector2(this.gondola.X + 64f, this.theo.Y), false, new int?(), false);
      yield return (object) 0.5f;
    }

    private IEnumerator TheoTakesOutPhone()
    {
      this.player.Facing = Facings.Right;
      yield return (object) 0.25f;
      this.theo.Sprite.Play("usePhone", false, false);
      yield return (object) 2f;
    }

    private IEnumerator FaceTheo()
    {
      this.player.DummyAutoAnimate = true;
      yield return (object) 0.2f;
      this.player.Facing = Facings.Left;
      yield return (object) 0.2f;
    }

    private IEnumerator ShowPhoto()
    {
      this.theo.Sprite.Scale.X = -1f;
      yield return (object) 0.25f;
      yield return (object) this.player.DummyWalkTo(this.theo.X + 5f, false, 1f, false);
      yield return (object) 1f;
      Selfie selfie = new Selfie(this.SceneAs<Level>());
      this.Scene.Add((Entity) selfie);
      yield return (object) selfie.OpenRoutine("selfieGondola");
      yield return (object) selfie.WaitForInput();
    }

    public override void Update()
    {
      base.Update();
      if ((double) this.anxietyRumble > 0.0)
        Input.RumbleSpecific(this.anxietyRumble, 0.1f);
      if (this.Scene.OnInterval(0.05f))
        this.anxietyStutter = Calc.Random.NextFloat(0.1f);
      Distort.AnxietyOrigin = new Vector2(0.5f, 0.5f);
      Distort.Anxiety = (float) ((double) this.anxiety * 0.200000002980232 + (double) this.anxietyStutter * (double) this.anxiety);
      if (this.moveLoopSfx != null && this.gondola != null)
        this.moveLoopSfx.Position = this.gondola.Position;
      if (this.haltLoopSfx != null && this.gondola != null)
        this.haltLoopSfx.Position = this.gondola.Position;
      if (this.gondolaState == CS04_Gondola.GondolaStates.MovingToCenter)
      {
        this.MoveGondolaTowards(0.5f);
        if ((double) this.gondolaPercent >= 0.5)
          this.gondolaState = CS04_Gondola.GondolaStates.InCenter;
      }
      else if (this.gondolaState == CS04_Gondola.GondolaStates.InCenter)
      {
        Vector2 vector2 = (this.gondola.Destination - this.gondola.Position).SafeNormalize() * this.gondolaSpeed;
        this.loopingCloud.CameraOffset.X += vector2.X * Engine.DeltaTime;
        this.loopingCloud.CameraOffset.Y += vector2.Y * Engine.DeltaTime;
        this.windSnowFg.CameraOffset = this.loopingCloud.CameraOffset;
        this.loopingCloud.LoopY = true;
      }
      else if (this.gondolaState != CS04_Gondola.GondolaStates.Stopped)
      {
        if (this.gondolaState == CS04_Gondola.GondolaStates.Shaking)
        {
          this.Level.Wind.X = -400f;
          if ((double) this.shakeTimer <= 0.0 && ((double) this.gondola.Rotation == 0.0 || (double) this.gondola.Rotation < -0.25))
          {
            this.shakeTimer = 1f;
            this.gondola.RotationSpeed = 0.5f;
          }
          this.shakeTimer -= Engine.DeltaTime;
        }
        else if (this.gondolaState == CS04_Gondola.GondolaStates.MovingToEnd)
        {
          this.MoveGondolaTowards(1f);
          if ((double) this.gondolaPercent >= 1.0)
            this.gondolaState = CS04_Gondola.GondolaStates.Stopped;
        }
      }
      if (this.loopingCloud != null && !this.loopingCloud.LoopY && (double) this.Level.Camera.Bottom < (double) this.LoopCloudsAt)
        this.loopingCloud.LoopY = true;
      if (!this.AutoSnapCharacters)
        return;
      this.theo.Position = this.gondola.GetRotatedFloorPositionAt(this.theoXOffset, 52f);
      this.player.Position = this.gondola.GetRotatedFloorPositionAt(this.playerXOffset, 52f);
      if (this.evil != null)
        this.evil.Position = this.gondola.GetRotatedFloorPositionAt(-24f, 20f);
    }

    private void MoveGondolaTowards(float percent)
    {
      float num = (this.gondola.Start - this.gondola.Destination).Length();
      this.gondolaSpeed = Calc.Approach(this.gondolaSpeed, 64f, 120f * Engine.DeltaTime);
      this.gondolaPercent = Calc.Approach(this.gondolaPercent, percent, this.gondolaSpeed / num * Engine.DeltaTime);
      this.gondola.Position = (this.gondola.Start + (this.gondola.Destination - this.gondola.Start) * this.gondolaPercent).Floor();
      this.Level.Camera.Position = this.gondola.Position + new Vector2(-160f, -120f);
    }

    private IEnumerator PanCamera(Vector2 to, float duration, Ease.Easer ease = null)
    {
      if (ease == null)
        ease = Ease.CubeInOut;
      Vector2 from = this.Level.Camera.Position;
      for (float t = 0.0f; (double) t < 1.0; t += Engine.DeltaTime / duration)
      {
        yield return (object) null;
        this.Level.Camera.Position = from + (to - from) * ease(Math.Min(t, 1f));
      }
    }

    private IEnumerator MovePlayerOnGondola(float x)
    {
      this.player.Sprite.Play("walk", false, false);
      this.player.Facing = (Facings) Math.Sign(x - this.playerXOffset);
      while ((double) this.playerXOffset != (double) x)
      {
        this.playerXOffset = Calc.Approach(this.playerXOffset, x, 48f * Engine.DeltaTime);
        yield return (object) null;
      }
      this.player.Sprite.Play("idle", false, false);
    }

    private IEnumerator MoveTheoOnGondola(float x, bool changeFacing = true)
    {
      this.theo.Sprite.Play("walk", false, false);
      if (changeFacing)
        this.theo.Sprite.Scale.X = (float) Math.Sign(x - this.theoXOffset);
      while ((double) this.theoXOffset != (double) x)
      {
        this.theoXOffset = Calc.Approach(this.theoXOffset, x, 48f * Engine.DeltaTime);
        yield return (object) null;
      }
      this.theo.Sprite.Play("idle", false, false);
    }

    private void BurstTentacles(int layer, float dist, float from = 200f)
    {
      Vector2 vector2 = this.Level.Camera.Position + new Vector2(160f, 90f);
      ReflectionTentacles reflectionTentacles1 = new ReflectionTentacles();
      reflectionTentacles1.Create(0.0f, 0, layer, new List<Vector2>()
      {
        vector2 + new Vector2(-from, 0.0f),
        vector2 + new Vector2(-800f, 0.0f)
      });
      reflectionTentacles1.SnapTentacles();
      reflectionTentacles1.Nodes[0] = vector2 + new Vector2(-dist, 0.0f);
      ReflectionTentacles reflectionTentacles2 = new ReflectionTentacles();
      reflectionTentacles2.Create(0.0f, 0, layer, new List<Vector2>()
      {
        vector2 + new Vector2(from, 0.0f),
        vector2 + new Vector2(800f, 0.0f)
      });
      reflectionTentacles2.SnapTentacles();
      reflectionTentacles2.Nodes[0] = vector2 + new Vector2(dist, 0.0f);
      this.tentacles.Add(reflectionTentacles1);
      this.tentacles.Add(reflectionTentacles2);
      this.Level.Add((Entity) reflectionTentacles1);
      this.Level.Add((Entity) reflectionTentacles2);
    }

    private enum GondolaStates
    {
      Stopped,
      MovingToCenter,
      InCenter,
      Shaking,
      MovingToEnd,
    }
  }
}

