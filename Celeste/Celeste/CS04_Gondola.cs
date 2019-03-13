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
    private float gondolaPercent;
    private bool AutoSnapCharacters;
    private float theoXOffset;
    private float playerXOffset;
    private float gondolaSpeed;
    private float shakeTimer;
    private const float gondolaMaxSpeed = 64f;
    private float anxiety;
    private float anxietyStutter;
    private float anxietyRumble;
    private BreathingRumbler rumbler;
    private CS04_Gondola.GondolaStates gondolaState;

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
      CS04_Gondola cs04Gondola = this;
      cs04Gondola.player.StateMachine.State = 11;
      yield return (object) cs04Gondola.player.DummyWalkToExact((int) cs04Gondola.gondola.X + 16, false, 1f);
      while (!cs04Gondola.player.OnGround(1))
        yield return (object) null;
      Audio.SetMusic("event:/music/lvl1/theo", true, true);
      yield return (object) Textbox.Say("CH4_GONDOLA", new Func<IEnumerator>(cs04Gondola.EnterTheo), new Func<IEnumerator>(cs04Gondola.CheckOnTheo), new Func<IEnumerator>(cs04Gondola.GetUpTheo), new Func<IEnumerator>(cs04Gondola.LookAtLever), new Func<IEnumerator>(cs04Gondola.PullLever), new Func<IEnumerator>(cs04Gondola.WaitABit), new Func<IEnumerator>(cs04Gondola.WaitForCenter), new Func<IEnumerator>(cs04Gondola.SelfieThenStallsOut), new Func<IEnumerator>(cs04Gondola.MovePlayerLeft), new Func<IEnumerator>(cs04Gondola.SnapLeverOff), new Func<IEnumerator>(cs04Gondola.DarknessAppears), new Func<IEnumerator>(cs04Gondola.DarknessConsumes), new Func<IEnumerator>(cs04Gondola.CantBreath), new Func<IEnumerator>(cs04Gondola.StartBreathing), new Func<IEnumerator>(cs04Gondola.Ascend), new Func<IEnumerator>(cs04Gondola.WaitABit), new Func<IEnumerator>(cs04Gondola.TheoTakesOutPhone), new Func<IEnumerator>(cs04Gondola.FaceTheo));
      yield return (object) cs04Gondola.ShowPhoto();
      cs04Gondola.EndCutscene(cs04Gondola.Level, true);
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
      SpotlightWipe.FocusPoint = Vector2.op_Division(new Vector2(320f, 180f), 2f);
    }

    private IEnumerator EnterTheo()
    {
      CS04_Gondola cs04Gondola1 = this;
      cs04Gondola1.player.Facing = Facings.Left;
      yield return (object) 0.2f;
      CS04_Gondola cs04Gondola2 = cs04Gondola1;
      Rectangle bounds1 = cs04Gondola1.Level.Bounds;
      Vector2 to1 = new Vector2((float) ((Rectangle) ref bounds1).get_Left(), cs04Gondola1.theo.Y - 90f);
      yield return (object) cs04Gondola2.PanCamera(to1, 1f, (Ease.Easer) null);
      cs04Gondola1.theo.Visible = true;
      float theoStartX = cs04Gondola1.theo.X;
      yield return (object) cs04Gondola1.theo.MoveTo(new Vector2(theoStartX + 35f, cs04Gondola1.theo.Y), false, new int?(), false);
      yield return (object) 0.6f;
      yield return (object) cs04Gondola1.theo.MoveTo(new Vector2(theoStartX + 60f, cs04Gondola1.theo.Y), false, new int?(), false);
      Audio.Play("event:/game/04_cliffside/gondola_theo_fall", cs04Gondola1.theo.Position);
      cs04Gondola1.theo.Sprite.Play("idleEdge", false, false);
      yield return (object) 1f;
      cs04Gondola1.theo.Sprite.Play("falling", false, false);
      cs04Gondola1.theo.X += 4f;
      cs04Gondola1.theo.Depth = -10010;
      float speed = 80f;
      while ((double) cs04Gondola1.theo.Y < (double) cs04Gondola1.player.Y)
      {
        cs04Gondola1.theo.Y += speed * Engine.DeltaTime;
        speed += 120f * Engine.DeltaTime;
        yield return (object) null;
      }
      cs04Gondola1.Level.DirectionalShake(new Vector2(0.0f, 1f), 0.3f);
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
      cs04Gondola1.theo.Y = cs04Gondola1.player.Y;
      cs04Gondola1.theo.Sprite.Play("hitGround", false, false);
      cs04Gondola1.theo.Sprite.Rate = 0.0f;
      cs04Gondola1.theo.Depth = 1000;
      cs04Gondola1.theo.Sprite.Scale = new Vector2(1.3f, 0.8f);
      yield return (object) 0.5f;
      Vector2 start = cs04Gondola1.theo.Sprite.Scale;
      Tween tween = Tween.Create(Tween.TweenMode.Oneshot, (Ease.Easer) null, 2f, true);
      tween.OnUpdate = (Action<Tween>) (t =>
      {
        this.theo.Sprite.Scale.X = (__Null) (double) MathHelper.Lerp((float) start.X, 1f, t.Eased);
        this.theo.Sprite.Scale.Y = (__Null) (double) MathHelper.Lerp((float) start.Y, 1f, t.Eased);
      });
      cs04Gondola1.Add((Component) tween);
      CS04_Gondola cs04Gondola3 = cs04Gondola1;
      Rectangle bounds2 = cs04Gondola1.Level.Bounds;
      Vector2 to2 = new Vector2((float) ((Rectangle) ref bounds2).get_Left(), cs04Gondola1.theo.Y - 120f);
      yield return (object) cs04Gondola3.PanCamera(to2, 1f, (Ease.Easer) null);
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
      this.theo.Sprite.Scale.X = (__Null) -1.0;
    }

    private IEnumerator PullLever()
    {
      CS04_Gondola cs04Gondola = this;
      cs04Gondola.Add((Component) new Coroutine(cs04Gondola.player.DummyWalkToExact((int) cs04Gondola.gondola.X - 7, false, 1f), true));
      cs04Gondola.theo.Sprite.Scale.X = (__Null) -1.0;
      yield return (object) 0.2f;
      Audio.Play("event:/game/04_cliffside/gondola_theo_lever_start", cs04Gondola.theo.Position);
      cs04Gondola.theo.Sprite.Play("pullVent", false, false);
      yield return (object) 1f;
      Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
      cs04Gondola.gondola.Lever.Play("pulled", false, false);
      cs04Gondola.theo.Sprite.Play("fallVent", false, false);
      yield return (object) 0.6f;
      cs04Gondola.Level.Shake(0.3f);
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Long);
      yield return (object) 0.5f;
      yield return (object) cs04Gondola.PanCamera(Vector2.op_Addition(cs04Gondola.gondola.Position, new Vector2(-160f, -120f)), 1f, (Ease.Easer) null);
      yield return (object) 0.5f;
      cs04Gondola.Level.Background.Backdrops.Add((Backdrop) (cs04Gondola.loopingCloud = new Parallax(GFX.Game["bgs/04/bgCloudLoop"])));
      cs04Gondola.Level.Background.Backdrops.Add((Backdrop) (cs04Gondola.bottomCloud = new Parallax(GFX.Game["bgs/04/bgCloud"])));
      cs04Gondola.loopingCloud.LoopX = cs04Gondola.bottomCloud.LoopX = true;
      cs04Gondola.loopingCloud.LoopY = cs04Gondola.bottomCloud.LoopY = false;
      cs04Gondola.loopingCloud.Position.Y = (__Null) ((double) cs04Gondola.Level.Camera.Top - (double) cs04Gondola.loopingCloud.Texture.Height - (double) cs04Gondola.bottomCloud.Texture.Height);
      cs04Gondola.bottomCloud.Position.Y = (__Null) ((double) cs04Gondola.Level.Camera.Top - (double) cs04Gondola.bottomCloud.Texture.Height);
      cs04Gondola.LoopCloudsAt = (float) cs04Gondola.bottomCloud.Position.Y;
      cs04Gondola.AutoSnapCharacters = true;
      cs04Gondola.theoXOffset = cs04Gondola.theo.X - cs04Gondola.gondola.X;
      cs04Gondola.playerXOffset = cs04Gondola.player.X - cs04Gondola.gondola.X;
      cs04Gondola.player.StateMachine.State = 17;
      Tween tween = Tween.Create(Tween.TweenMode.Oneshot, (Ease.Easer) null, 16f, true);
      tween.OnUpdate = (Action<Tween>) (t =>
      {
        if (!(Audio.CurrentMusic == "event:/music/lvl1/theo"))
          return;
        Audio.SetMusicParam("fade", 1f - t.Eased);
      });
      cs04Gondola.Add((Component) tween);
      SoundSource soundSource = new SoundSource();
      soundSource.Position = cs04Gondola.gondola.LeftCliffside.Position;
      soundSource.Play("event:/game/04_cliffside/gondola_cliffmechanism_start", (string) null, 0.0f);
      cs04Gondola.Add((Component) soundSource);
      cs04Gondola.moveLoopSfx.Play("event:/game/04_cliffside/gondola_movement_loop", (string) null, 0.0f);
      cs04Gondola.Level.Shake(0.3f);
      Input.Rumble(RumbleStrength.Strong, RumbleLength.FullSecond);
      cs04Gondola.gondolaSpeed = 32f;
      cs04Gondola.gondola.RotationSpeed = 1f;
      cs04Gondola.gondolaState = CS04_Gondola.GondolaStates.MovingToCenter;
      yield return (object) 1f;
      yield return (object) cs04Gondola.MoveTheoOnGondola(12f, false);
      yield return (object) 0.2f;
      cs04Gondola.theo.Sprite.Scale.X = (__Null) -1.0;
    }

    private IEnumerator WaitABit()
    {
      yield return (object) 1f;
    }

    private IEnumerator WaitForCenter()
    {
      while (this.gondolaState != CS04_Gondola.GondolaStates.InCenter)
        yield return (object) null;
      this.theo.Sprite.Scale.X = (__Null) 1.0;
      yield return (object) 1f;
      yield return (object) this.MovePlayerOnGondola(-20f);
      yield return (object) 0.5f;
    }

    private IEnumerator SelfieThenStallsOut()
    {
      CS04_Gondola cs04Gondola = this;
      Audio.SetMusic("event:/music/lvl4/minigame", true, true);
      cs04Gondola.Add((Component) new Coroutine(cs04Gondola.Level.ZoomTo(new Vector2(160f, 110f), 2f, 0.5f), true));
      yield return (object) 0.3f;
      cs04Gondola.theo.Sprite.Scale.X = (__Null) 1.0;
      yield return (object) 0.2f;
      cs04Gondola.Add((Component) new Coroutine(cs04Gondola.MovePlayerOnGondola(cs04Gondola.theoXOffset - 8f), true));
      yield return (object) 0.4f;
      Audio.Play("event:/game/04_cliffside/gondola_theoselfie_halt", cs04Gondola.theo.Position);
      cs04Gondola.theo.Sprite.Play("holdOutPhone", false, false);
      yield return (object) 1.5f;
      cs04Gondola.theoXOffset += 4f;
      cs04Gondola.playerXOffset += 4f;
      cs04Gondola.gondola.RotationSpeed = -1f;
      cs04Gondola.gondolaState = CS04_Gondola.GondolaStates.Stopped;
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Long);
      cs04Gondola.theo.Sprite.Play("takeSelfieImmediate", false, false);
      cs04Gondola.Add((Component) new Coroutine(cs04Gondola.PanCamera(Vector2.op_Addition(Vector2.op_Addition(cs04Gondola.gondola.Position, Vector2.op_Multiply(Vector2.op_Subtraction(cs04Gondola.gondola.Destination, cs04Gondola.gondola.Position).SafeNormalize(), 32f)), new Vector2(-160f, -120f)), 0.3f, Ease.CubeOut), true));
      yield return (object) 0.5f;
      cs04Gondola.Level.Flash(Color.get_White(), false);
      cs04Gondola.Level.Add((Entity) (cs04Gondola.evil = new BadelineDummy(Vector2.get_Zero())));
      cs04Gondola.evil.Appear(cs04Gondola.Level, false);
      cs04Gondola.evil.Floatness = 0.0f;
      cs04Gondola.evil.Depth = -1000000;
      cs04Gondola.moveLoopSfx.Stop(true);
      cs04Gondola.haltLoopSfx.Play("event:/game/04_cliffside/gondola_halted_loop", (string) null, 0.0f);
      cs04Gondola.gondolaState = CS04_Gondola.GondolaStates.Shaking;
      yield return (object) cs04Gondola.PanCamera(Vector2.op_Addition(cs04Gondola.gondola.Position, new Vector2(-160f, -120f)), 1f, (Ease.Easer) null);
      yield return (object) 1f;
    }

    private IEnumerator MovePlayerLeft()
    {
      yield return (object) this.MovePlayerOnGondola(-20f);
      this.theo.Sprite.Scale.X = (__Null) -1.0;
      yield return (object) 0.5f;
      yield return (object) this.MovePlayerOnGondola(20f);
      yield return (object) 0.5f;
      yield return (object) this.MovePlayerOnGondola(-10f);
      yield return (object) 0.5f;
      this.player.Facing = Facings.Right;
    }

    private IEnumerator SnapLeverOff()
    {
      CS04_Gondola cs04Gondola = this;
      yield return (object) cs04Gondola.MoveTheoOnGondola(7f, true);
      Audio.Play("event:/game/04_cliffside/gondola_theo_lever_fail", cs04Gondola.theo.Position);
      cs04Gondola.theo.Sprite.Play("pullVent", false, false);
      yield return (object) 1f;
      cs04Gondola.theo.Sprite.Play("fallVent", false, false);
      yield return (object) 1f;
      cs04Gondola.gondola.BreakLever();
      Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
      cs04Gondola.Level.Shake(0.3f);
      yield return (object) 2.5f;
    }

    private IEnumerator DarknessAppears()
    {
      CS04_Gondola cs04Gondola = this;
      Audio.SetMusicParam("calm", 0.0f);
      yield return (object) 0.25f;
      cs04Gondola.player.Sprite.Play("tired", false, false);
      yield return (object) 0.25f;
      cs04Gondola.evil.Vanish();
      cs04Gondola.evil = (BadelineDummy) null;
      yield return (object) 0.3f;
      cs04Gondola.Level.NextColorGrade("panicattack");
      cs04Gondola.Level.Shake(0.3f);
      Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
      cs04Gondola.BurstTentacles(3, 90f, 200f);
      Audio.Play("event:/game/04_cliffside/gondola_scaryhair_01", cs04Gondola.gondola.Position);
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime / 2f)
      {
        yield return (object) null;
        cs04Gondola.Level.Background.Fade = p;
        cs04Gondola.anxiety = p;
        if (cs04Gondola.windSnowFg != null)
          cs04Gondola.windSnowFg.Alpha = 1f - p;
      }
      yield return (object) 0.25f;
    }

    private IEnumerator DarknessConsumes()
    {
      // ISSUE: reference to a compiler-generated field
      int num = this.\u003C\u003E1__state;
      CS04_Gondola cs04Gondola = this;
      if (num != 0)
      {
        if (num != 1)
          return false;
        // ISSUE: reference to a compiler-generated field
        this.\u003C\u003E1__state = -1;
        cs04Gondola.theo.Sprite.Play("comfortStart", false, false);
        return false;
      }
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      cs04Gondola.Level.Shake(0.3f);
      Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
      Audio.Play("event:/game/04_cliffside/gondola_scaryhair_02", cs04Gondola.gondola.Position);
      cs04Gondola.BurstTentacles(2, 60f, 200f);
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E2__current = (object) cs04Gondola.MoveTheoOnGondola(0.0f, true);
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = 1;
      return true;
    }

    private IEnumerator CantBreath()
    {
      // ISSUE: reference to a compiler-generated field
      int num = this.\u003C\u003E1__state;
      CS04_Gondola cs04Gondola = this;
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
      cs04Gondola.Level.Shake(0.3f);
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
      Audio.Play("event:/game/04_cliffside/gondola_scaryhair_03", cs04Gondola.gondola.Position);
      cs04Gondola.BurstTentacles(1, 30f, 200f);
      cs04Gondola.BurstTentacles(0, 0.0f, 100f);
      cs04Gondola.rumbler = new BreathingRumbler();
      cs04Gondola.Scene.Add((Entity) cs04Gondola.rumbler);
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E2__current = (object) null;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = 1;
      return true;
    }

    private IEnumerator StartBreathing()
    {
      CS04_Gondola cs04Gondola = this;
      BreathingMinigame breathing = new BreathingMinigame(true, cs04Gondola.rumbler);
      cs04Gondola.Scene.Add((Entity) breathing);
      while (!breathing.Completed)
        yield return (object) null;
      foreach (Entity tentacle in cs04Gondola.tentacles)
        tentacle.RemoveSelf();
      cs04Gondola.anxiety = 0.0f;
      cs04Gondola.Level.Background.Fade = 0.0f;
      cs04Gondola.Level.SnapColorGrade((string) null);
      cs04Gondola.gondola.CancelPullSides();
      cs04Gondola.Level.ResetZoom();
      yield return (object) 0.5f;
      Audio.Play("event:/game/04_cliffside/gondola_restart", cs04Gondola.gondola.Position);
      yield return (object) 1f;
      cs04Gondola.moveLoopSfx.Play("event:/game/04_cliffside/gondola_movement_loop", (string) null, 0.0f);
      cs04Gondola.haltLoopSfx.Stop(true);
      cs04Gondola.Level.Shake(0.3f);
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Long);
      cs04Gondola.gondolaState = CS04_Gondola.GondolaStates.InCenter;
      cs04Gondola.gondola.RotationSpeed = 0.5f;
      yield return (object) 1.2f;
    }

    private IEnumerator Ascend()
    {
      CS04_Gondola cs04Gondola = this;
      cs04Gondola.gondolaState = CS04_Gondola.GondolaStates.MovingToEnd;
      while (cs04Gondola.gondolaState != CS04_Gondola.GondolaStates.Stopped)
        yield return (object) null;
      cs04Gondola.Level.Shake(0.3f);
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Long);
      cs04Gondola.moveLoopSfx.Stop(true);
      Audio.Play("event:/game/04_cliffside/gondola_finish", cs04Gondola.gondola.Position);
      cs04Gondola.gondola.RotationSpeed = 0.5f;
      yield return (object) 0.1f;
      while ((double) cs04Gondola.gondola.Rotation > 0.0)
        yield return (object) null;
      cs04Gondola.gondola.Rotation = cs04Gondola.gondola.RotationSpeed = 0.0f;
      cs04Gondola.Level.Shake(0.3f);
      cs04Gondola.AutoSnapCharacters = false;
      cs04Gondola.player.StateMachine.State = 11;
      cs04Gondola.player.Position = cs04Gondola.player.Position.Floor();
      while (cs04Gondola.player.CollideCheck<Solid>())
        --cs04Gondola.player.Y;
      cs04Gondola.theo.Position.Y = cs04Gondola.player.Position.Y;
      cs04Gondola.theo.Sprite.Play("comfortRecover", false, false);
      cs04Gondola.theo.Sprite.Scale.X = (__Null) 1.0;
      yield return (object) cs04Gondola.player.DummyWalkTo(cs04Gondola.gondola.X + 80f, false, 1f, false);
      cs04Gondola.player.DummyAutoAnimate = false;
      cs04Gondola.player.Sprite.Play("tired", false, false);
      yield return (object) cs04Gondola.theo.MoveTo(new Vector2(cs04Gondola.gondola.X + 64f, cs04Gondola.theo.Y), false, new int?(), false);
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
      CS04_Gondola cs04Gondola = this;
      cs04Gondola.theo.Sprite.Scale.X = (__Null) -1.0;
      yield return (object) 0.25f;
      yield return (object) cs04Gondola.player.DummyWalkTo(cs04Gondola.theo.X + 5f, false, 1f, false);
      yield return (object) 1f;
      Selfie selfie = new Selfie(cs04Gondola.SceneAs<Level>());
      cs04Gondola.Scene.Add((Entity) selfie);
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
        Vector2 vector2 = Vector2.op_Multiply(Vector2.op_Subtraction(this.gondola.Destination, this.gondola.Position).SafeNormalize(), this.gondolaSpeed);
        ref __Null local1 = ref this.loopingCloud.CameraOffset.X;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(float&) ref local1 = ^(float&) ref local1 + (float) vector2.X * Engine.DeltaTime;
        ref __Null local2 = ref this.loopingCloud.CameraOffset.Y;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(float&) ref local2 = ^(float&) ref local2 + (float) vector2.Y * Engine.DeltaTime;
        this.windSnowFg.CameraOffset = this.loopingCloud.CameraOffset;
        this.loopingCloud.LoopY = true;
      }
      else if (this.gondolaState != CS04_Gondola.GondolaStates.Stopped)
      {
        if (this.gondolaState == CS04_Gondola.GondolaStates.Shaking)
        {
          this.Level.Wind.X = (__Null) -400.0;
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
      if (this.evil == null)
        return;
      this.evil.Position = this.gondola.GetRotatedFloorPositionAt(-24f, 20f);
    }

    private void MoveGondolaTowards(float percent)
    {
      Vector2 vector2 = Vector2.op_Subtraction(this.gondola.Start, this.gondola.Destination);
      float num = ((Vector2) ref vector2).Length();
      this.gondolaSpeed = Calc.Approach(this.gondolaSpeed, 64f, 120f * Engine.DeltaTime);
      this.gondolaPercent = Calc.Approach(this.gondolaPercent, percent, this.gondolaSpeed / num * Engine.DeltaTime);
      this.gondola.Position = Vector2.op_Addition(this.gondola.Start, Vector2.op_Multiply(Vector2.op_Subtraction(this.gondola.Destination, this.gondola.Start), this.gondolaPercent)).Floor();
      this.Level.Camera.Position = Vector2.op_Addition(this.gondola.Position, new Vector2(-160f, -120f));
    }

    private IEnumerator PanCamera(Vector2 to, float duration, Ease.Easer ease = null)
    {
      CS04_Gondola cs04Gondola = this;
      if (ease == null)
        ease = Ease.CubeInOut;
      Vector2 from = cs04Gondola.Level.Camera.Position;
      for (float t = 0.0f; (double) t < 1.0; t += Engine.DeltaTime / duration)
      {
        yield return (object) null;
        cs04Gondola.Level.Camera.Position = Vector2.op_Addition(from, Vector2.op_Multiply(Vector2.op_Subtraction(to, from), ease(Math.Min(t, 1f))));
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
        this.theo.Sprite.Scale.X = (__Null) (double) Math.Sign(x - this.theoXOffset);
      while ((double) this.theoXOffset != (double) x)
      {
        this.theoXOffset = Calc.Approach(this.theoXOffset, x, 48f * Engine.DeltaTime);
        yield return (object) null;
      }
      this.theo.Sprite.Play("idle", false, false);
    }

    private void BurstTentacles(int layer, float dist, float from = 200f)
    {
      Vector2 vector2 = Vector2.op_Addition(this.Level.Camera.Position, new Vector2(160f, 90f));
      ReflectionTentacles reflectionTentacles1 = new ReflectionTentacles();
      ReflectionTentacles reflectionTentacles2 = reflectionTentacles1;
      int layer1 = layer;
      List<Vector2> startNodes1 = new List<Vector2>();
      startNodes1.Add(Vector2.op_Addition(vector2, new Vector2(-from, 0.0f)));
      startNodes1.Add(Vector2.op_Addition(vector2, new Vector2(-800f, 0.0f)));
      reflectionTentacles2.Create(0.0f, 0, layer1, startNodes1);
      reflectionTentacles1.SnapTentacles();
      reflectionTentacles1.Nodes[0] = Vector2.op_Addition(vector2, new Vector2(-dist, 0.0f));
      ReflectionTentacles reflectionTentacles3 = new ReflectionTentacles();
      ReflectionTentacles reflectionTentacles4 = reflectionTentacles3;
      int layer2 = layer;
      List<Vector2> startNodes2 = new List<Vector2>();
      startNodes2.Add(Vector2.op_Addition(vector2, new Vector2(from, 0.0f)));
      startNodes2.Add(Vector2.op_Addition(vector2, new Vector2(800f, 0.0f)));
      reflectionTentacles4.Create(0.0f, 0, layer2, startNodes2);
      reflectionTentacles3.SnapTentacles();
      reflectionTentacles3.Nodes[0] = Vector2.op_Addition(vector2, new Vector2(dist, 0.0f));
      this.tentacles.Add(reflectionTentacles1);
      this.tentacles.Add(reflectionTentacles3);
      this.Level.Add((Entity) reflectionTentacles1);
      this.Level.Add((Entity) reflectionTentacles3);
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
