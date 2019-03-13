// Decompiled with JetBrains decompiler
// Type: Celeste.CS06_StarJumpEnd
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
  public class CS06_StarJumpEnd : CutsceneEntity
  {
    private bool waiting = true;
    private List<ReflectionTentacles> tentacles = new List<ReflectionTentacles>();
    public const string Flag = "plateau_2";
    private bool shaking;
    private NPC theo;
    private Player player;
    private Bonfire bonfire;
    private BadelineDummy badeline;
    private Plateau plateau;
    private BreathingMinigame breathing;
    private Vector2 playerStart;
    private Vector2 cameraStart;
    private float anxietyFade;
    private SineWave anxietySine;
    private float anxietyJitter;
    private bool hidingNorthingLights;
    private bool charactersSpinning;
    private float maddySine;
    private float maddySineTarget;
    private float maddySineAnchorY;
    private SoundSource shakingLoopSfx;
    private bool baddyCircling;
    private BreathingRumbler rumbler;
    private int tentacleIndex;

    public CS06_StarJumpEnd(NPC theo, Player player, Vector2 playerStart, Vector2 cameraStart)
      : base(true, false)
    {
      this.Depth = 10100;
      this.theo = theo;
      this.player = player;
      this.playerStart = playerStart;
      this.cameraStart = cameraStart;
      this.Add((Component) (this.anxietySine = new SineWave(0.3f)));
    }

    public override void Added(Scene scene)
    {
      this.Level = scene as Level;
      this.bonfire = scene.Entities.FindFirst<Bonfire>();
      this.plateau = scene.Entities.FindFirst<Plateau>();
    }

    public override void Update()
    {
      base.Update();
      if (this.waiting)
      {
        double y = (double) this.player.Y;
        Rectangle bounds = this.Level.Bounds;
        double num = (double) (((Rectangle) ref bounds).get_Top() + 160);
        if (y <= num)
        {
          this.waiting = false;
          this.Start();
        }
      }
      if (this.shaking)
        this.Level.Shake(0.2f);
      if (this.Level != null && this.Level.OnInterval(0.1f))
        this.anxietyJitter = Calc.Random.Range(-0.1f, 0.1f);
      Distort.Anxiety = this.anxietyFade * Math.Max(0.0f, (float) (0.0 + (double) this.anxietyJitter + (double) this.anxietySine.Value * 0.600000023841858));
      this.maddySine = Calc.Approach(this.maddySine, this.maddySineTarget, 12f * Engine.DeltaTime);
      if ((double) this.maddySine <= 0.0)
        return;
      this.player.Y = this.maddySineAnchorY + (float) (Math.Sin((double) this.Level.TimeActive * 2.0) * 3.0) * this.maddySine;
    }

    public override void OnBegin(Level level)
    {
      this.Add((Component) new Coroutine(this.Cutscene(level), true));
    }

    private IEnumerator Cutscene(Level level)
    {
      CS06_StarJumpEnd cs06StarJumpEnd = this;
      level.Entities.FindFirst<StarJumpController>()?.RemoveSelf();
      foreach (Entity entity in level.Entities.FindAll<StarJumpBlock>())
        entity.Collidable = false;
      int center = level.Bounds.X + 160;
      double num1 = (double) center;
      Rectangle bounds1 = level.Bounds;
      double num2 = (double) (((Rectangle) ref bounds1).get_Top() + 150);
      Vector2 cutsceneCenter = new Vector2((float) num1, (float) num2);
      NorthernLights bg = level.Background.Get<NorthernLights>();
      level.CameraOffset.Y = (__Null) -30.0;
      cs06StarJumpEnd.Add((Component) new Coroutine(CutsceneEntity.CameraTo(Vector2.op_Addition(cutsceneCenter, new Vector2(-160f, -70f)), 1.5f, Ease.CubeOut, 0.0f), true));
      cs06StarJumpEnd.Add((Component) new Coroutine(CutsceneEntity.CameraTo(Vector2.op_Addition(cutsceneCenter, new Vector2(-160f, -120f)), 2f, Ease.CubeInOut, 1.5f), true));
      Tween.Set((Entity) cs06StarJumpEnd, Tween.TweenMode.Oneshot, 3f, Ease.CubeInOut, (Action<Tween>) (t => bg.OffsetY = t.Eased * 32f), (Action<Tween>) null);
      if (cs06StarJumpEnd.player.StateMachine.State == 19)
        Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
      cs06StarJumpEnd.player.Dashes = 0;
      cs06StarJumpEnd.player.StateMachine.State = 11;
      cs06StarJumpEnd.player.DummyGravity = false;
      cs06StarJumpEnd.player.DummyAutoAnimate = false;
      cs06StarJumpEnd.player.Sprite.Play("fallSlow", false, false);
      cs06StarJumpEnd.player.Dashes = 1;
      cs06StarJumpEnd.player.Speed = new Vector2(0.0f, -80f);
      cs06StarJumpEnd.player.Facing = Facings.Right;
      cs06StarJumpEnd.player.ForceCameraUpdate = false;
      while ((double) ((Vector2) ref cs06StarJumpEnd.player.Speed).Length() > 0.0 || Vector2.op_Inequality(cs06StarJumpEnd.player.Position, cutsceneCenter))
      {
        cs06StarJumpEnd.player.Speed = Calc.Approach(cs06StarJumpEnd.player.Speed, Vector2.get_Zero(), 200f * Engine.DeltaTime);
        cs06StarJumpEnd.player.Position = Calc.Approach(cs06StarJumpEnd.player.Position, cutsceneCenter, 64f * Engine.DeltaTime);
        yield return (object) null;
      }
      cs06StarJumpEnd.player.Sprite.Play("spin", false, false);
      yield return (object) 3.5f;
      cs06StarJumpEnd.player.Facing = Facings.Right;
      level.Add((Entity) (cs06StarJumpEnd.badeline = new BadelineDummy(cs06StarJumpEnd.player.Position)));
      level.Displacement.AddBurst(cs06StarJumpEnd.player.Position, 0.5f, 8f, 48f, 0.5f, (Ease.Easer) null, (Ease.Easer) null);
      Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
      cs06StarJumpEnd.player.CreateSplitParticles();
      Audio.Play("event:/char/badeline/maddy_split");
      cs06StarJumpEnd.badeline.Sprite.Scale.X = (__Null) -1.0;
      Vector2 start = cs06StarJumpEnd.player.Position;
      Vector2 target = Vector2.op_Addition(cutsceneCenter, new Vector2(-30f, 0.0f));
      cs06StarJumpEnd.maddySineAnchorY = (float) cutsceneCenter.Y;
      float p;
      for (p = 0.0f; (double) p <= 1.0; p += 2f * Engine.DeltaTime)
      {
        yield return (object) null;
        if ((double) p > 1.0)
          p = 1f;
        cs06StarJumpEnd.player.Position = Vector2.Lerp(start, target, Ease.CubeOut(p));
        cs06StarJumpEnd.badeline.Position = new Vector2((float) center + ((float) center - cs06StarJumpEnd.player.X), cs06StarJumpEnd.player.Y);
      }
      start = (Vector2) null;
      target = (Vector2) null;
      cs06StarJumpEnd.charactersSpinning = true;
      cs06StarJumpEnd.Add((Component) new Coroutine(cs06StarJumpEnd.SpinCharacters(), true));
      cs06StarJumpEnd.SetMusicLayer(2);
      yield return (object) 1f;
      yield return (object) Textbox.Say("ch6_dreaming", new Func<IEnumerator>(cs06StarJumpEnd.TentaclesAppear), new Func<IEnumerator>(cs06StarJumpEnd.TentaclesGrab), new Func<IEnumerator>(cs06StarJumpEnd.FeatherMinigame), new Func<IEnumerator>(cs06StarJumpEnd.EndFeatherMinigame), new Func<IEnumerator>(cs06StarJumpEnd.StartCirclingPlayer));
      Audio.Play("event:/game/06_reflection/badeline_pull_whooshdown");
      cs06StarJumpEnd.Add((Component) new Coroutine(cs06StarJumpEnd.BadelineFlyDown(), true));
      yield return (object) 0.7f;
      foreach (Entity entity in level.Entities.FindAll<FlyFeather>())
        entity.RemoveSelf();
      foreach (Entity entity in level.Entities.FindAll<StarJumpBlock>())
        entity.RemoveSelf();
      foreach (Entity entity in level.Entities.FindAll<JumpThru>())
        entity.RemoveSelf();
      level.Shake(0.3f);
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Short);
      level.CameraOffset.Y = (__Null) 0.0;
      cs06StarJumpEnd.player.Sprite.Play("tentacle_pull", false, false);
      cs06StarJumpEnd.player.Speed.Y = (__Null) 160.0;
      FallEffects.Show(true);
      for (p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime / 3f)
      {
        ref __Null local1 = ref cs06StarJumpEnd.player.Speed.Y;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(float&) ref local1 = ^(float&) ref local1 + Engine.DeltaTime * 100f;
        if ((double) cs06StarJumpEnd.player.X < (double) (level.Bounds.X + 32))
          cs06StarJumpEnd.player.X = (float) (level.Bounds.X + 32);
        double x = (double) cs06StarJumpEnd.player.X;
        Rectangle bounds2 = level.Bounds;
        double num3 = (double) (((Rectangle) ref bounds2).get_Right() - 32);
        Rectangle bounds3;
        if (x > num3)
        {
          Player player = cs06StarJumpEnd.player;
          bounds3 = level.Bounds;
          double num4 = (double) (((Rectangle) ref bounds3).get_Right() - 32);
          player.X = (float) num4;
        }
        if ((double) p > 0.699999988079071)
        {
          ref __Null local2 = ref level.CameraOffset.Y;
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          ^(float&) ref local2 = ^(float&) ref local2 - 100f * Engine.DeltaTime;
        }
        foreach (ReflectionTentacles tentacle in cs06StarJumpEnd.tentacles)
        {
          List<Vector2> nodes1 = tentacle.Nodes;
          bounds3 = level.Bounds;
          Vector2 vector2_1 = new Vector2((float) ((Rectangle) ref bounds3).get_Center().X, cs06StarJumpEnd.player.Y + 300f);
          nodes1[0] = vector2_1;
          List<Vector2> nodes2 = tentacle.Nodes;
          bounds3 = level.Bounds;
          Vector2 vector2_2 = new Vector2((float) ((Rectangle) ref bounds3).get_Center().X, cs06StarJumpEnd.player.Y + 600f);
          nodes2[1] = vector2_2;
        }
        FallEffects.SpeedMultiplier += Engine.DeltaTime * 0.75f;
        Input.Rumble(RumbleStrength.Strong, RumbleLength.Short);
        yield return (object) null;
      }
      Audio.Play("event:/game/06_reflection/badeline_pull_impact");
      FallEffects.Show(false);
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
      level.Flash(Color.get_White(), false);
      level.Session.Dreaming = false;
      level.CameraOffset.Y = (__Null) 0.0;
      level.Camera.Position = cs06StarJumpEnd.cameraStart;
      cs06StarJumpEnd.SetBloom(0.0f);
      cs06StarJumpEnd.bonfire.SetMode(Bonfire.Mode.Smoking);
      cs06StarJumpEnd.plateau.Depth = cs06StarJumpEnd.player.Depth + 10;
      cs06StarJumpEnd.plateau.Remove((Component) cs06StarJumpEnd.plateau.Occluder);
      cs06StarJumpEnd.player.Position = Vector2.op_Addition(cs06StarJumpEnd.playerStart, new Vector2(0.0f, 8f));
      cs06StarJumpEnd.player.Speed = Vector2.get_Zero();
      cs06StarJumpEnd.player.Sprite.Play("tentacle_dangling", false, false);
      cs06StarJumpEnd.player.Facing = Facings.Left;
      ref __Null local3 = ref cs06StarJumpEnd.theo.Position.X;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local3 = ^(float&) ref local3 - 24f;
      cs06StarJumpEnd.theo.Sprite.Play("alert", false, false);
      foreach (ReflectionTentacles tentacle in cs06StarJumpEnd.tentacles)
      {
        tentacle.Index = 0;
        List<Vector2> nodes1 = tentacle.Nodes;
        Rectangle bounds2 = level.Bounds;
        Vector2 vector2_1 = new Vector2((float) ((Rectangle) ref bounds2).get_Center().X, cs06StarJumpEnd.player.Y + 32f);
        nodes1[0] = vector2_1;
        List<Vector2> nodes2 = tentacle.Nodes;
        Rectangle bounds3 = level.Bounds;
        Vector2 vector2_2 = new Vector2((float) ((Rectangle) ref bounds3).get_Center().X, cs06StarJumpEnd.player.Y + 400f);
        nodes2[1] = vector2_2;
        tentacle.SnapTentacles();
      }
      cs06StarJumpEnd.shaking = true;
      cs06StarJumpEnd.Add((Component) (cs06StarJumpEnd.shakingLoopSfx = new SoundSource()));
      cs06StarJumpEnd.shakingLoopSfx.Play("event:/game/06_reflection/badeline_pull_rumble_loop", (string) null, 0.0f);
      yield return (object) Textbox.Say("ch6_theo_watchout");
      Audio.Play("event:/game/06_reflection/badeline_pull_cliffbreak");
      Input.Rumble(RumbleStrength.Medium, RumbleLength.Long);
      cs06StarJumpEnd.shakingLoopSfx.Stop(true);
      cs06StarJumpEnd.shaking = false;
      for (int index = 0; (double) index < (double) cs06StarJumpEnd.plateau.Width; index += 8)
      {
        level.Add((Entity) Engine.Pooler.Create<Debris>().Init(Vector2.op_Addition(cs06StarJumpEnd.plateau.Position, new Vector2((float) index + Calc.Random.NextFloat(8f), Calc.Random.NextFloat(8f))), '3').BlastFrom(Vector2.op_Addition(cs06StarJumpEnd.plateau.Center, new Vector2(0.0f, 8f))));
        level.Add((Entity) Engine.Pooler.Create<Debris>().Init(Vector2.op_Addition(cs06StarJumpEnd.plateau.Position, new Vector2((float) index + Calc.Random.NextFloat(8f), Calc.Random.NextFloat(8f))), '3').BlastFrom(Vector2.op_Addition(cs06StarJumpEnd.plateau.Center, new Vector2(0.0f, 8f))));
      }
      cs06StarJumpEnd.plateau.RemoveSelf();
      cs06StarJumpEnd.bonfire.RemoveSelf();
      level.Shake(0.3f);
      cs06StarJumpEnd.player.Speed.Y = (__Null) 160.0;
      cs06StarJumpEnd.player.Sprite.Play("tentacle_pull", false, false);
      cs06StarJumpEnd.player.ForceCameraUpdate = false;
      FadeWipe wipe = new FadeWipe((Scene) level, false, (Action) (() => this.EndCutscene(level, true)));
      wipe.Duration = 3f;
      target = level.Camera.Position;
      start = Vector2.op_Addition(level.Camera.Position, new Vector2(0.0f, 400f));
      while ((double) wipe.Percent < 1.0)
      {
        level.Camera.Position = Vector2.Lerp(target, start, Ease.CubeIn(wipe.Percent));
        ref __Null local1 = ref cs06StarJumpEnd.player.Speed.Y;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(float&) ref local1 = ^(float&) ref local1 + 400f * Engine.DeltaTime;
        foreach (ReflectionTentacles tentacle in cs06StarJumpEnd.tentacles)
        {
          List<Vector2> nodes1 = tentacle.Nodes;
          Rectangle bounds2 = level.Bounds;
          Vector2 vector2_1 = new Vector2((float) ((Rectangle) ref bounds2).get_Center().X, cs06StarJumpEnd.player.Y + 300f);
          nodes1[0] = vector2_1;
          List<Vector2> nodes2 = tentacle.Nodes;
          Rectangle bounds3 = level.Bounds;
          Vector2 vector2_2 = new Vector2((float) ((Rectangle) ref bounds3).get_Center().X, cs06StarJumpEnd.player.Y + 600f);
          nodes2[1] = vector2_2;
        }
        yield return (object) null;
      }
      wipe = (FadeWipe) null;
      target = (Vector2) null;
      start = (Vector2) null;
    }

    private void SetMusicLayer(int index)
    {
      for (int index1 = 1; index1 <= 3; ++index1)
        this.Level.Session.Audio.Music.Layer(index1, index == index1);
      this.Level.Session.Audio.Apply();
    }

    private IEnumerator TentaclesAppear()
    {
      CS06_StarJumpEnd cs06StarJumpEnd = this;
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
      if (cs06StarJumpEnd.tentacleIndex == 0)
        Audio.Play("event:/game/06_reflection/badeline_freakout_1");
      else if (cs06StarJumpEnd.tentacleIndex == 1)
        Audio.Play("event:/game/06_reflection/badeline_freakout_2");
      else if (cs06StarJumpEnd.tentacleIndex == 2)
        Audio.Play("event:/game/06_reflection/badeline_freakout_3");
      else
        Audio.Play("event:/game/06_reflection/badeline_freakout_4");
      if (!cs06StarJumpEnd.hidingNorthingLights)
      {
        cs06StarJumpEnd.Add((Component) new Coroutine(cs06StarJumpEnd.NothernLightsDown(), true));
        cs06StarJumpEnd.hidingNorthingLights = true;
      }
      cs06StarJumpEnd.Level.Shake(0.3f);
      cs06StarJumpEnd.anxietyFade += 0.1f;
      if (cs06StarJumpEnd.tentacleIndex == 0)
        cs06StarJumpEnd.SetMusicLayer(3);
      int num1 = 400;
      int num2 = 140;
      List<Vector2> startNodes = new List<Vector2>();
      startNodes.Add(new Vector2(cs06StarJumpEnd.Level.Camera.X + 160f, cs06StarJumpEnd.Level.Camera.Y + (float) num1));
      startNodes.Add(new Vector2(cs06StarJumpEnd.Level.Camera.X + 160f, (float) ((double) cs06StarJumpEnd.Level.Camera.Y + (double) num1 + 200.0)));
      ReflectionTentacles reflectionTentacles = new ReflectionTentacles();
      reflectionTentacles.Create(0.0f, 0, cs06StarJumpEnd.tentacles.Count, startNodes);
      reflectionTentacles.Nodes[0] = new Vector2((float) reflectionTentacles.Nodes[0].X, cs06StarJumpEnd.Level.Camera.Y + (float) num2);
      cs06StarJumpEnd.Level.Add((Entity) reflectionTentacles);
      cs06StarJumpEnd.tentacles.Add(reflectionTentacles);
      cs06StarJumpEnd.charactersSpinning = false;
      ++cs06StarJumpEnd.tentacleIndex;
      cs06StarJumpEnd.badeline.Sprite.Play("angry", false, false);
      cs06StarJumpEnd.maddySineTarget = 1f;
      yield return (object) null;
    }

    private IEnumerator TentaclesGrab()
    {
      // ISSUE: reference to a compiler-generated field
      int num = this.\u003C\u003E1__state;
      CS06_StarJumpEnd cs06StarJumpEnd = this;
      if (num != 0)
      {
        if (num != 1)
          return false;
        // ISSUE: reference to a compiler-generated field
        this.\u003C\u003E1__state = -1;
        Input.Rumble(RumbleStrength.Strong, RumbleLength.Long);
        cs06StarJumpEnd.Level.Shake(0.3f);
        cs06StarJumpEnd.rumbler = new BreathingRumbler();
        cs06StarJumpEnd.Level.Add((Entity) cs06StarJumpEnd.rumbler);
        return false;
      }
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      cs06StarJumpEnd.maddySineTarget = 0.0f;
      Audio.Play("event:/game/06_reflection/badeline_freakout_5");
      cs06StarJumpEnd.player.Sprite.Play("tentacle_grab", false, false);
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E2__current = (object) 0.1f;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = 1;
      return true;
    }

    private IEnumerator StartCirclingPlayer()
    {
      // ISSUE: reference to a compiler-generated field
      int num = this.\u003C\u003E1__state;
      CS06_StarJumpEnd cs06StarJumpEnd = this;
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
      cs06StarJumpEnd.Add((Component) new Coroutine(cs06StarJumpEnd.BadelineCirclePlayer(), true));
      Vector2 from = cs06StarJumpEnd.player.Position;
      Rectangle bounds = cs06StarJumpEnd.Level.Bounds;
      Vector2 to = new Vector2((float) ((Rectangle) ref bounds).get_Center().X, cs06StarJumpEnd.player.Y);
      Tween.Set((Entity) cs06StarJumpEnd, Tween.TweenMode.Oneshot, 0.5f, Ease.CubeOut, (Action<Tween>) (t => this.player.Position = Vector2.Lerp(from, to, t.Eased)), (Action<Tween>) null);
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E2__current = (object) null;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = 1;
      return true;
    }

    private IEnumerator EndCirclingPlayer()
    {
      this.baddyCircling = false;
      yield return (object) null;
    }

    private IEnumerator BadelineCirclePlayer()
    {
      CS06_StarJumpEnd cs06StarJumpEnd = this;
      float offset = 0.0f;
      Vector2 vector2 = Vector2.op_Subtraction(cs06StarJumpEnd.badeline.Position, cs06StarJumpEnd.player.Position);
      float dist = ((Vector2) ref vector2).Length();
      cs06StarJumpEnd.baddyCircling = true;
      while (cs06StarJumpEnd.baddyCircling)
      {
        offset -= Engine.DeltaTime * 4f;
        dist = Calc.Approach(dist, 24f, Engine.DeltaTime * 32f);
        cs06StarJumpEnd.badeline.Position = Vector2.op_Addition(cs06StarJumpEnd.player.Position, Calc.AngleToVector(offset, dist));
        int num = Math.Sign(cs06StarJumpEnd.player.X - cs06StarJumpEnd.badeline.X);
        if (num != 0)
          cs06StarJumpEnd.badeline.Sprite.Scale.X = (__Null) (double) num;
        if (cs06StarJumpEnd.Level.OnInterval(0.1f))
          TrailManager.Add((Entity) cs06StarJumpEnd.badeline, Player.NormalHairColor, 1f);
        yield return (object) null;
      }
      cs06StarJumpEnd.badeline.Sprite.Scale.X = (__Null) -1.0;
      yield return (object) cs06StarJumpEnd.badeline.FloatTo(Vector2.op_Addition(cs06StarJumpEnd.player.Position, new Vector2(40f, -16f)), new int?(-1), false, false);
    }

    private IEnumerator FeatherMinigame()
    {
      CS06_StarJumpEnd cs06StarJumpEnd = this;
      cs06StarJumpEnd.breathing = new BreathingMinigame(false, cs06StarJumpEnd.rumbler);
      cs06StarJumpEnd.Level.Add((Entity) cs06StarJumpEnd.breathing);
      while (!cs06StarJumpEnd.breathing.Pausing)
        yield return (object) null;
    }

    private IEnumerator EndFeatherMinigame()
    {
      this.baddyCircling = false;
      this.breathing.Pausing = false;
      while (!this.breathing.Completed)
        yield return (object) null;
      this.breathing = (BreathingMinigame) null;
    }

    private IEnumerator BadelineFlyDown()
    {
      // ISSUE: reference to a compiler-generated field
      int num = this.\u003C\u003E1__state;
      CS06_StarJumpEnd cs06StarJumpEnd = this;
      if (num != 0)
      {
        if (num != 1)
          return false;
        // ISSUE: reference to a compiler-generated field
        this.\u003C\u003E1__state = -1;
        cs06StarJumpEnd.badeline.RemoveSelf();
        return false;
      }
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      cs06StarJumpEnd.badeline.Sprite.Play("fallFast", false, false);
      cs06StarJumpEnd.badeline.FloatSpeed = 600f;
      cs06StarJumpEnd.badeline.FloatAccel = 1200f;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E2__current = (object) cs06StarJumpEnd.badeline.FloatTo(new Vector2(cs06StarJumpEnd.badeline.X, cs06StarJumpEnd.Level.Camera.Y + 200f), new int?(), true, true);
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = 1;
      return true;
    }

    private IEnumerator NothernLightsDown()
    {
      NorthernLights bg = this.Level.Background.Get<NorthernLights>();
      if (bg != null)
      {
        while ((double) bg.NorthernLightsAlpha > 0.0)
        {
          bg.NorthernLightsAlpha -= Engine.DeltaTime * 0.5f;
          yield return (object) null;
        }
      }
    }

    private IEnumerator SpinCharacters()
    {
      Vector2 maddyStart = this.player.Position;
      Vector2 baddyStart = this.badeline.Position;
      Vector2 center = Vector2.op_Division(Vector2.op_Addition(maddyStart, baddyStart), 2f);
      float dist = Math.Abs((float) (maddyStart.X - center.X));
      float timer = 1.570796f;
      this.player.Sprite.Play("spin", false, false);
      this.badeline.Sprite.Play("spin", false, false);
      this.badeline.Sprite.Scale.X = (__Null) 1.0;
      while (this.charactersSpinning)
      {
        int frame = (int) ((double) timer / 6.28318548202515 * 14.0 + 10.0);
        this.player.Sprite.SetAnimationFrame(frame);
        this.badeline.Sprite.SetAnimationFrame(frame + 7);
        float num1 = (float) Math.Sin((double) timer);
        float num2 = (float) Math.Cos((double) timer);
        this.player.Position = Vector2.op_Subtraction(center, new Vector2(num1 * dist, num2 * 8f));
        this.badeline.Position = Vector2.op_Addition(center, new Vector2(num1 * dist, num2 * 8f));
        timer += Engine.DeltaTime * 2f;
        yield return (object) null;
      }
      this.player.Facing = Facings.Right;
      this.player.Sprite.Play("fallSlow", false, false);
      this.badeline.Sprite.Scale.X = (__Null) -1.0;
      this.badeline.Sprite.Play("angry", false, false);
      this.badeline.AutoAnimator.Enabled = false;
      Vector2 maddyFrom = this.player.Position;
      Vector2 baddyFrom = this.badeline.Position;
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime * 3f)
      {
        this.player.Position = Vector2.Lerp(maddyFrom, maddyStart, Ease.CubeOut(p));
        this.badeline.Position = Vector2.Lerp(baddyFrom, baddyStart, Ease.CubeOut(p));
        yield return (object) null;
      }
    }

    public override void OnEnd(Level level)
    {
      if (this.rumbler != null)
      {
        this.rumbler.RemoveSelf();
        this.rumbler = (BreathingRumbler) null;
      }
      if (this.breathing != null)
        this.breathing.RemoveSelf();
      this.SetBloom(0.0f);
      level.Session.Audio.Music.Event = (string) null;
      level.Session.Audio.Apply();
      level.Remove((Entity) this.player);
      level.UnloadLevel();
      level.EndCutscene();
      level.Session.SetFlag("plateau_2", true);
      level.SnapColorGrade(AreaData.Get((Scene) level).ColorGrade);
      level.Session.Dreaming = false;
      level.Session.FirstLevel = false;
      if (this.WasSkipped)
        level.OnEndOfFrame += (Action) (() =>
        {
          level.Session.Level = "00";
          Session session = level.Session;
          Level level1 = level;
          Rectangle bounds = level.Bounds;
          double left = (double) ((Rectangle) ref bounds).get_Left();
          bounds = level.Bounds;
          double bottom = (double) ((Rectangle) ref bounds).get_Bottom();
          Vector2 from = new Vector2((float) left, (float) bottom);
          Vector2? nullable = new Vector2?(level1.GetSpawnPoint(from));
          session.RespawnPoint = nullable;
          level.LoadLevel(Player.IntroTypes.None, false);
          FallEffects.Show(false);
          level.Session.Audio.Music.Event = "event:/music/lvl6/main";
          level.Session.Audio.Apply();
        });
      else
        Engine.Scene = (Scene) new OverworldReflectionsFall(level, (Action) (() =>
        {
          Audio.SetAmbience((string) null, true);
          level.Session.Level = "04";
          Session session = level.Session;
          Level level1 = level;
          Rectangle bounds = level.Bounds;
          double x = (double) (float) ((Rectangle) ref bounds).get_Center().X;
          bounds = level.Bounds;
          double top = (double) ((Rectangle) ref bounds).get_Top();
          Vector2 from = new Vector2((float) x, (float) top);
          Vector2? nullable = new Vector2?(level1.GetSpawnPoint(from));
          session.RespawnPoint = nullable;
          level.LoadLevel(Player.IntroTypes.Fall, false);
          level.Add((Entity) new BackgroundFadeIn(Color.get_Black(), 2f, 30f));
          level.Entities.UpdateLists();
          foreach (CrystalStaticSpinner entity in level.Tracker.GetEntities<CrystalStaticSpinner>())
            entity.ForceInstantiate();
        }));
    }

    private void SetBloom(float add)
    {
      this.Level.Session.BloomBaseAdd = add;
      this.Level.Bloom.Base = AreaData.Get((Scene) this.Level).BloomBase + add;
    }
  }
}
