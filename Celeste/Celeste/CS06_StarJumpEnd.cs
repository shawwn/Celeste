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
    private float anxietyFade = 0.0f;
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
      if (this.waiting && (double) this.player.Y <= (double) (this.Level.Bounds.Top + 160))
      {
        this.waiting = false;
        this.Start();
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
      StarJumpController controller = level.Entities.FindFirst<StarJumpController>();
      if (controller != null)
        controller.RemoveSelf();
      foreach (StarJumpBlock starJumpBlock in level.Entities.FindAll<StarJumpBlock>())
      {
        StarJumpBlock entity = starJumpBlock;
        entity.Collidable = false;
        entity = (StarJumpBlock) null;
      }
      int center = level.Bounds.X + 160;
      Vector2 cutsceneCenter = new Vector2((float) center, (float) (level.Bounds.Top + 150));
      NorthernLights bg = level.Background.Get<NorthernLights>();
      level.CameraOffset.Y = -30f;
      this.Add((Component) new Coroutine(CutsceneEntity.CameraTo(cutsceneCenter + new Vector2(-160f, -70f), 1.5f, Ease.CubeOut, 0.0f), true));
      this.Add((Component) new Coroutine(CutsceneEntity.CameraTo(cutsceneCenter + new Vector2(-160f, -120f), 2f, Ease.CubeInOut, 1.5f), true));
      Tween.Set((Entity) this, Tween.TweenMode.Oneshot, 3f, Ease.CubeInOut, (Action<Tween>) (t => bg.OffsetY = t.Eased * 32f), (Action<Tween>) null);
      if (this.player.StateMachine.State == 19)
        Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
      this.player.Dashes = 0;
      this.player.StateMachine.State = Player.StDummy;
      this.player.DummyGravity = false;
      this.player.DummyAutoAnimate = false;
      this.player.Sprite.Play("fallSlow", false, false);
      this.player.Dashes = 1;
      this.player.Speed = new Vector2(0.0f, -80f);
      this.player.Facing = Facings.Right;
      this.player.ForceCameraUpdate = false;
      while ((double) this.player.Speed.Length() > 0.0 || this.player.Position != cutsceneCenter)
      {
        this.player.Speed = Calc.Approach(this.player.Speed, Vector2.Zero, 200f * Engine.DeltaTime);
        this.player.Position = Calc.Approach(this.player.Position, cutsceneCenter, 64f * Engine.DeltaTime);
        yield return (object) null;
      }
      this.player.Sprite.Play("spin", false, false);
      yield return (object) 3.5f;
      this.player.Facing = Facings.Right;
      level.Add((Entity) (this.badeline = new BadelineDummy(this.player.Position)));
      level.Displacement.AddBurst(this.player.Position, 0.5f, 8f, 48f, 0.5f, (Ease.Easer) null, (Ease.Easer) null);
      Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
      this.player.CreateSplitParticles();
      Audio.Play("event:/char/badeline/maddy_split");
      this.badeline.Sprite.Scale.X = -1f;
      Vector2 start = this.player.Position;
      Vector2 target = cutsceneCenter + new Vector2(-30f, 0.0f);
      this.maddySineAnchorY = cutsceneCenter.Y;
      for (float p = 0.0f; (double) p <= 1.0; p += 2f * Engine.DeltaTime)
      {
        yield return (object) null;
        if ((double) p > 1.0)
          p = 1f;
        this.player.Position = Vector2.Lerp(start, target, Ease.CubeOut(p));
        this.badeline.Position = new Vector2((float) center + ((float) center - this.player.X), this.player.Y);
      }
      start = new Vector2();
      target = new Vector2();
      this.charactersSpinning = true;
      this.Add((Component) new Coroutine(this.SpinCharacters(), true));
      this.SetMusicLayer(2);
      yield return (object) 1f;
      yield return (object) Textbox.Say("ch6_dreaming", new Func<IEnumerator>(this.TentaclesAppear), new Func<IEnumerator>(this.TentaclesGrab), new Func<IEnumerator>(this.FeatherMinigame), new Func<IEnumerator>(this.EndFeatherMinigame), new Func<IEnumerator>(this.StartCirclingPlayer));
      Audio.Play("event:/game/06_reflection/badeline_pull_whooshdown");
      this.Add((Component) new Coroutine(this.BadelineFlyDown(), true));
      yield return (object) 0.7f;
      foreach (FlyFeather flyFeather in level.Entities.FindAll<FlyFeather>())
      {
        FlyFeather entity = flyFeather;
        entity.RemoveSelf();
        entity = (FlyFeather) null;
      }
      foreach (StarJumpBlock starJumpBlock in level.Entities.FindAll<StarJumpBlock>())
      {
        StarJumpBlock entity = starJumpBlock;
        entity.RemoveSelf();
        entity = (StarJumpBlock) null;
      }
      foreach (JumpThru jumpThru in level.Entities.FindAll<JumpThru>())
      {
        JumpThru entity = jumpThru;
        entity.RemoveSelf();
        entity = (JumpThru) null;
      }
      level.Shake(0.3f);
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Short);
      level.CameraOffset.Y = 0.0f;
      this.player.Sprite.Play("tentacle_pull", false, false);
      this.player.Speed.Y = 160f;
      FallEffects.Show(true);
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime / 3f)
      {
        this.player.Speed.Y += Engine.DeltaTime * 100f;
        if ((double) this.player.X < (double) (level.Bounds.X + 32))
          this.player.X = (float) (level.Bounds.X + 32);
        double x = (double) this.player.X;
        Rectangle bounds = level.Bounds;
        double num1 = (double) (bounds.Right - 32);
        if (x > num1)
        {
          Player player = this.player;
          bounds = level.Bounds;
          double num2 = (double) (bounds.Right - 32);
          player.X = (float) num2;
        }
        if ((double) p > 0.699999988079071)
          level.CameraOffset.Y -= 100f * Engine.DeltaTime;
        foreach (ReflectionTentacles tentacle1 in this.tentacles)
        {
          ReflectionTentacles tentacle = tentacle1;
          List<Vector2> nodes1 = tentacle.Nodes;
          bounds = level.Bounds;
          Vector2 vector2_1 = new Vector2((float) bounds.Center.X, this.player.Y + 300f);
          nodes1[0] = vector2_1;
          List<Vector2> nodes2 = tentacle.Nodes;
          bounds = level.Bounds;
          Vector2 vector2_2 = new Vector2((float) bounds.Center.X, this.player.Y + 600f);
          nodes2[1] = vector2_2;
          tentacle = (ReflectionTentacles) null;
        }
        FallEffects.SpeedMultiplier += Engine.DeltaTime * 0.75f;
        Input.Rumble(RumbleStrength.Strong, RumbleLength.Short);
        yield return (object) null;
      }
      Audio.Play("event:/game/06_reflection/badeline_pull_impact");
      FallEffects.Show(false);
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
      level.Flash(Color.White, false);
      level.Session.Dreaming = false;
      level.CameraOffset.Y = 0.0f;
      level.Camera.Position = this.cameraStart;
      this.SetBloom(0.0f);
      this.bonfire.SetMode(Bonfire.Mode.Smoking);
      this.plateau.Depth = this.player.Depth + 10;
      this.plateau.Remove((Component) this.plateau.Occluder);
      this.player.Position = this.playerStart + new Vector2(0.0f, 8f);
      this.player.Speed = Vector2.Zero;
      this.player.Sprite.Play("tentacle_dangling", false, false);
      this.player.Facing = Facings.Left;
      this.theo.Position.X -= 24f;
      this.theo.Sprite.Play("alert", false, false);
      foreach (ReflectionTentacles tentacle1 in this.tentacles)
      {
        ReflectionTentacles tentacle = tentacle1;
        tentacle.Index = 0;
        List<Vector2> nodes1 = tentacle.Nodes;
        Rectangle bounds = level.Bounds;
        Vector2 vector2_1 = new Vector2((float) bounds.Center.X, this.player.Y + 32f);
        nodes1[0] = vector2_1;
        List<Vector2> nodes2 = tentacle.Nodes;
        bounds = level.Bounds;
        Vector2 vector2_2 = new Vector2((float) bounds.Center.X, this.player.Y + 400f);
        nodes2[1] = vector2_2;
        tentacle.SnapTentacles();
        tentacle = (ReflectionTentacles) null;
      }
      this.shaking = true;
      this.Add((Component) (this.shakingLoopSfx = new SoundSource()));
      this.shakingLoopSfx.Play("event:/game/06_reflection/badeline_pull_rumble_loop", (string) null, 0.0f);
      yield return (object) Textbox.Say("ch6_theo_watchout");
      Audio.Play("event:/game/06_reflection/badeline_pull_cliffbreak");
      Input.Rumble(RumbleStrength.Medium, RumbleLength.Long);
      this.shakingLoopSfx.Stop(true);
      this.shaking = false;
      for (int x = 0; (double) x < (double) this.plateau.Width; x += 8)
      {
        level.Add((Entity) Engine.Pooler.Create<Debris>().Init(this.plateau.Position + new Vector2((float) x + Calc.Random.NextFloat(8f), Calc.Random.NextFloat(8f)), '3').BlastFrom(this.plateau.Center + new Vector2(0.0f, 8f)));
        level.Add((Entity) Engine.Pooler.Create<Debris>().Init(this.plateau.Position + new Vector2((float) x + Calc.Random.NextFloat(8f), Calc.Random.NextFloat(8f)), '3').BlastFrom(this.plateau.Center + new Vector2(0.0f, 8f)));
      }
      this.plateau.RemoveSelf();
      this.bonfire.RemoveSelf();
      level.Shake(0.3f);
      this.player.Speed.Y = 160f;
      this.player.Sprite.Play("tentacle_pull", false, false);
      this.player.ForceCameraUpdate = false;
      FadeWipe wipe = new FadeWipe((Scene) level, false, (Action) (() => this.EndCutscene(level, true)));
      wipe.Duration = 3f;
      Vector2 camFrom = level.Camera.Position;
      Vector2 camTo = level.Camera.Position + new Vector2(0.0f, 400f);
      while ((double) wipe.Percent < 1.0)
      {
        level.Camera.Position = Vector2.Lerp(camFrom, camTo, Ease.CubeIn(wipe.Percent));
        this.player.Speed.Y += 400f * Engine.DeltaTime;
        foreach (ReflectionTentacles tentacle1 in this.tentacles)
        {
          ReflectionTentacles tentacle = tentacle1;
          List<Vector2> nodes1 = tentacle.Nodes;
          Rectangle bounds = level.Bounds;
          Vector2 vector2_1 = new Vector2((float) bounds.Center.X, this.player.Y + 300f);
          nodes1[0] = vector2_1;
          List<Vector2> nodes2 = tentacle.Nodes;
          bounds = level.Bounds;
          Vector2 vector2_2 = new Vector2((float) bounds.Center.X, this.player.Y + 600f);
          nodes2[1] = vector2_2;
          tentacle = (ReflectionTentacles) null;
        }
        yield return (object) null;
      }
      wipe = (FadeWipe) null;
      camFrom = new Vector2();
      camTo = new Vector2();
    }

    private void SetMusicLayer(int index)
    {
      for (int index1 = 1; index1 <= 3; ++index1)
        this.Level.Session.Audio.Music.Layer(index1, index == index1);
      this.Level.Session.Audio.Apply();
    }

    private IEnumerator TentaclesAppear()
    {
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
      if (this.tentacleIndex == 0)
        Audio.Play("event:/game/06_reflection/badeline_freakout_1");
      else if (this.tentacleIndex == 1)
        Audio.Play("event:/game/06_reflection/badeline_freakout_2");
      else if (this.tentacleIndex == 2)
        Audio.Play("event:/game/06_reflection/badeline_freakout_3");
      else
        Audio.Play("event:/game/06_reflection/badeline_freakout_4");
      if (!this.hidingNorthingLights)
      {
        this.Add((Component) new Coroutine(this.NothernLightsDown(), true));
        this.hidingNorthingLights = true;
      }
      this.Level.Shake(0.3f);
      this.anxietyFade += 0.1f;
      if (this.tentacleIndex == 0)
        this.SetMusicLayer(3);
      int from = 400;
      int to = 140;
      List<Vector2> nodes = new List<Vector2>();
      nodes.Add(new Vector2(this.Level.Camera.X + 160f, this.Level.Camera.Y + (float) from));
      nodes.Add(new Vector2(this.Level.Camera.X + 160f, (float) ((double) this.Level.Camera.Y + (double) from + 200.0)));
      ReflectionTentacles t = new ReflectionTentacles();
      t.Create(0.0f, 0, this.tentacles.Count, nodes);
      t.Nodes[0] = new Vector2(t.Nodes[0].X, this.Level.Camera.Y + (float) to);
      this.Level.Add((Entity) t);
      this.tentacles.Add(t);
      this.charactersSpinning = false;
      ++this.tentacleIndex;
      this.badeline.Sprite.Play("angry", false, false);
      this.maddySineTarget = 1f;
      yield return (object) null;
    }

    private IEnumerator TentaclesGrab()
    {
      this.maddySineTarget = 0.0f;
      Audio.Play("event:/game/06_reflection/badeline_freakout_5");
      this.player.Sprite.Play("tentacle_grab", false, false);
      yield return (object) 0.1f;
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Long);
      this.Level.Shake(0.3f);
      this.rumbler = new BreathingRumbler();
      this.Level.Add((Entity) this.rumbler);
    }

    private IEnumerator StartCirclingPlayer()
    {
      this.Add((Component) new Coroutine(this.BadelineCirclePlayer(), true));
      Vector2 from = this.player.Position;
      Vector2 to = new Vector2((float) this.Level.Bounds.Center.X, this.player.Y);
      Tween.Set((Entity) this, Tween.TweenMode.Oneshot, 0.5f, Ease.CubeOut, (Action<Tween>) (t => this.player.Position = Vector2.Lerp(from, to, t.Eased)), (Action<Tween>) null);
      yield return (object) null;
    }

    private IEnumerator EndCirclingPlayer()
    {
      this.baddyCircling = false;
      yield return (object) null;
    }

    private IEnumerator BadelineCirclePlayer()
    {
      float offset = 0.0f;
      float dist = (this.badeline.Position - this.player.Position).Length();
      this.baddyCircling = true;
      while (this.baddyCircling)
      {
        offset -= Engine.DeltaTime * 4f;
        dist = Calc.Approach(dist, 24f, Engine.DeltaTime * 32f);
        this.badeline.Position = this.player.Position + Calc.AngleToVector(offset, dist);
        int sign = Math.Sign(this.player.X - this.badeline.X);
        if ((uint) sign > 0U)
          this.badeline.Sprite.Scale.X = (float) sign;
        if (this.Level.OnInterval(0.1f))
          TrailManager.Add((Entity) this.badeline, Player.NormalHairColor, 1f);
        yield return (object) null;
      }
      this.badeline.Sprite.Scale.X = -1f;
      yield return (object) this.badeline.FloatTo(this.player.Position + new Vector2(40f, -16f), new int?(-1), false, false);
    }

    private IEnumerator FeatherMinigame()
    {
      this.breathing = new BreathingMinigame(false, this.rumbler);
      this.Level.Add((Entity) this.breathing);
      while (!this.breathing.Pausing)
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
      this.badeline.Sprite.Play("fallFast", false, false);
      this.badeline.FloatSpeed = 600f;
      this.badeline.FloatAccel = 1200f;
      yield return (object) this.badeline.FloatTo(new Vector2(this.badeline.X, this.Level.Camera.Y + 200f), new int?(), true, true);
      this.badeline.RemoveSelf();
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
      Vector2 center = (maddyStart + baddyStart) / 2f;
      float dist = Math.Abs(maddyStart.X - center.X);
      float timer = 1.570796f;
      this.player.Sprite.Play("spin", false, false);
      this.badeline.Sprite.Play("spin", false, false);
      this.badeline.Sprite.Scale.X = 1f;
      while (this.charactersSpinning)
      {
        int frame = (int) ((double) timer / 6.28318548202515 * 14.0 + 10.0);
        this.player.Sprite.SetAnimationFrame(frame);
        this.badeline.Sprite.SetAnimationFrame(frame + 7);
        float sin = (float) Math.Sin((double) timer);
        float cos = (float) Math.Cos((double) timer);
        this.player.Position = center - new Vector2(sin * dist, cos * 8f);
        this.badeline.Position = center + new Vector2(sin * dist, cos * 8f);
        timer += Engine.DeltaTime * 2f;
        yield return (object) null;
      }
      this.player.Facing = Facings.Right;
      this.player.Sprite.Play("fallSlow", false, false);
      this.badeline.Sprite.Scale.X = -1f;
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
          double left = (double) bounds.Left;
          bounds = level.Bounds;
          double bottom = (double) bounds.Bottom;
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
          double x = (double) bounds.Center.X;
          bounds = level.Bounds;
          double top = (double) bounds.Top;
          Vector2 from = new Vector2((float) x, (float) top);
          Vector2? nullable = new Vector2?(level1.GetSpawnPoint(from));
          session.RespawnPoint = nullable;
          level.LoadLevel(Player.IntroTypes.Fall, false);
          level.Add((Entity) new BackgroundFadeIn(Color.Black, 2f, 30f));
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

