// Decompiled with JetBrains decompiler
// Type: Celeste.CS02_BadelineIntro
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class CS02_BadelineIntro : CutsceneEntity
  {
    public const string Flag = "evil_maddy_intro";
    private Player player;
    private BadelineOldsite badeline;
    private Vector2 badelineEndPosition;
    private float anxietyFade;
    private float anxietyFadeTarget;
    private SineWave anxietySine;
    private float anxietyJitter;

    public CS02_BadelineIntro(BadelineOldsite badeline)
      : base(true, false)
    {
      this.badeline = badeline;
      this.badelineEndPosition = Vector2.op_Addition(badeline.Position, new Vector2(8f, -24f));
      this.Add((Component) (this.anxietySine = new SineWave(0.3f)));
      Distort.AnxietyOrigin = new Vector2(0.5f, 0.75f);
    }

    public override void OnBegin(Level level)
    {
      this.Add((Component) new Coroutine(this.Cutscene(level), true));
    }

    public override void Update()
    {
      base.Update();
      this.anxietyFade = Calc.Approach(this.anxietyFade, this.anxietyFadeTarget, 2.5f * Engine.DeltaTime);
      if (this.Scene.OnInterval(0.1f))
        this.anxietyJitter = Calc.Random.Range(-0.1f, 0.1f);
      Distort.Anxiety = this.anxietyFade * Math.Max(0.0f, (float) (0.0 + (double) this.anxietyJitter + (double) this.anxietySine.Value * 0.300000011920929));
    }

    private IEnumerator Cutscene(Level level)
    {
      CS02_BadelineIntro cs02BadelineIntro = this;
      cs02BadelineIntro.anxietyFadeTarget = 1f;
      while (true)
      {
        cs02BadelineIntro.player = level.Tracker.GetEntity<Player>();
        if (cs02BadelineIntro.player == null)
          yield return (object) null;
        else
          break;
      }
      while (!cs02BadelineIntro.player.OnGround(1))
        yield return (object) null;
      cs02BadelineIntro.player.StateMachine.State = 11;
      cs02BadelineIntro.player.StateMachine.Locked = true;
      yield return (object) 1f;
      if (level.Session.Area.Mode == AreaMode.Normal)
        Audio.SetMusic("event:/music/lvl2/evil_madeline", true, true);
      yield return (object) Textbox.Say("CH2_BADELINE_INTRO", new Func<IEnumerator>(cs02BadelineIntro.TurnAround), new Func<IEnumerator>(cs02BadelineIntro.RevealBadeline), new Func<IEnumerator>(cs02BadelineIntro.StartLaughing), new Func<IEnumerator>(cs02BadelineIntro.StopLaughing));
      cs02BadelineIntro.anxietyFadeTarget = 0.0f;
      yield return (object) cs02BadelineIntro.Level.ZoomBack(0.5f);
      cs02BadelineIntro.EndCutscene(level, true);
    }

    private IEnumerator TurnAround()
    {
      CS02_BadelineIntro cs02BadelineIntro = this;
      cs02BadelineIntro.player.Facing = Facings.Left;
      yield return (object) 0.2f;
      cs02BadelineIntro.Add((Component) new Coroutine(CutsceneEntity.CameraTo(new Vector2((float) cs02BadelineIntro.Level.Bounds.X, cs02BadelineIntro.Level.Camera.Y), 0.5f, (Ease.Easer) null, 0.0f), true));
      yield return (object) cs02BadelineIntro.Level.ZoomTo(new Vector2(84f, 135f), 2f, 0.5f);
      yield return (object) 0.2f;
    }

    private IEnumerator RevealBadeline()
    {
      CS02_BadelineIntro cs02BadelineIntro = this;
      Audio.Play("event:/game/02_old_site/sequence_badeline_intro", cs02BadelineIntro.badeline.Position);
      yield return (object) 0.1f;
      cs02BadelineIntro.Level.Displacement.AddBurst(Vector2.op_Addition(cs02BadelineIntro.badeline.Position, new Vector2(0.0f, -4f)), 0.8f, 8f, 48f, 0.5f, (Ease.Easer) null, (Ease.Easer) null);
      Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
      yield return (object) 0.1f;
      cs02BadelineIntro.badeline.Hovering = true;
      cs02BadelineIntro.badeline.Hair.Visible = true;
      cs02BadelineIntro.badeline.Sprite.Play("fallSlow", false, false);
      Vector2 from = cs02BadelineIntro.badeline.Position;
      Vector2 to = cs02BadelineIntro.badelineEndPosition;
      for (float t = 0.0f; (double) t < 1.0; t += Engine.DeltaTime)
      {
        cs02BadelineIntro.badeline.Position = Vector2.op_Addition(from, Vector2.op_Multiply(Vector2.op_Subtraction(to, from), Ease.CubeInOut(t)));
        yield return (object) null;
      }
      cs02BadelineIntro.player.Facing = (Facings) Math.Sign(cs02BadelineIntro.badeline.X - cs02BadelineIntro.player.X);
      yield return (object) 1f;
    }

    private IEnumerator StartLaughing()
    {
      yield return (object) 0.2f;
      this.badeline.Sprite.Play("laugh", true, false);
      yield return (object) null;
    }

    private IEnumerator StopLaughing()
    {
      this.badeline.Sprite.Play("fallSlow", true, false);
      yield return (object) null;
    }

    public override void OnEnd(Level level)
    {
      Audio.SetMusic((string) null, true, true);
      Distort.Anxiety = 0.0f;
      if (this.player != null)
      {
        this.player.StateMachine.Locked = false;
        this.player.Facing = Facings.Left;
        this.player.StateMachine.State = 0;
        this.player.JustRespawned = true;
      }
      this.badeline.Position = this.badelineEndPosition;
      this.badeline.Visible = true;
      this.badeline.Hair.Visible = true;
      this.badeline.Sprite.Play("fallSlow", false, false);
      this.badeline.Hovering = false;
      this.badeline.Add((Component) new Coroutine(this.badeline.StartChasingRoutine(level), true));
      level.Session.SetFlag("evil_maddy_intro", true);
    }
  }
}
