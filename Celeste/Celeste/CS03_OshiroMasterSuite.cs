// Decompiled with JetBrains decompiler
// Type: Celeste.CS03_OshiroMasterSuite
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class CS03_OshiroMasterSuite : CutsceneEntity
  {
    public const string Flag = "oshiro_resort_suite";
    private Player player;
    private NPC oshiro;
    private BadelineDummy evil;
    private ResortMirror mirror;

    public CS03_OshiroMasterSuite(NPC oshiro)
      : base(true, false)
    {
      this.oshiro = oshiro;
    }

    public override void OnBegin(Level level)
    {
      this.mirror = this.Scene.Entities.FindFirst<ResortMirror>();
      this.Add((Component) new Coroutine(this.Cutscene(level), true));
    }

    private IEnumerator Cutscene(Level level)
    {
      CS03_OshiroMasterSuite oshiroMasterSuite = this;
      while (true)
      {
        oshiroMasterSuite.player = oshiroMasterSuite.Scene.Tracker.GetEntity<Player>();
        if (oshiroMasterSuite.player == null)
          yield return (object) null;
        else
          break;
      }
      Audio.SetMusic((string) null, true, true);
      yield return (object) 0.4f;
      oshiroMasterSuite.player.StateMachine.State = 11;
      oshiroMasterSuite.player.StateMachine.Locked = true;
      oshiroMasterSuite.Add((Component) new Coroutine(oshiroMasterSuite.player.DummyWalkTo(oshiroMasterSuite.oshiro.X + 32f, false, 1f, false), true));
      yield return (object) 1f;
      Audio.SetMusic("event:/music/lvl3/oshiro_theme", true, true);
      yield return (object) Textbox.Say("CH3_OSHIRO_SUITE", new Func<IEnumerator>(oshiroMasterSuite.SuiteShadowAppear), new Func<IEnumerator>(oshiroMasterSuite.SuiteShadowDisrupt), new Func<IEnumerator>(oshiroMasterSuite.SuiteShadowCeiling), new Func<IEnumerator>(oshiroMasterSuite.Wander), new Func<IEnumerator>(oshiroMasterSuite.Console), new Func<IEnumerator>(oshiroMasterSuite.JumpBack), new Func<IEnumerator>(oshiroMasterSuite.Collapse), new Func<IEnumerator>(oshiroMasterSuite.AwkwardPause));
      oshiroMasterSuite.evil.Add((Component) new SoundSource(Vector2.get_Zero(), "event:/game/03_resort/suite_bad_exittop"));
      BadelineDummy evil = oshiroMasterSuite.evil;
      double x = (double) oshiroMasterSuite.evil.X;
      Rectangle bounds = level.Bounds;
      double num = (double) (((Rectangle) ref bounds).get_Top() - 32);
      Vector2 target = new Vector2((float) x, (float) num);
      int? turnAtEndTo = new int?();
      yield return (object) evil.FloatTo(target, turnAtEndTo, true, false);
      oshiroMasterSuite.Scene.Remove((Entity) oshiroMasterSuite.evil);
      while ((double) level.Lighting.Alpha != (double) level.BaseLightingAlpha)
      {
        level.Lighting.Alpha = Calc.Approach(level.Lighting.Alpha, level.BaseLightingAlpha, Engine.DeltaTime * 0.5f);
        yield return (object) null;
      }
      oshiroMasterSuite.EndCutscene(level, true);
    }

    private IEnumerator Wander()
    {
      CS03_OshiroMasterSuite oshiroMasterSuite = this;
      yield return (object) 0.5f;
      oshiroMasterSuite.player.Facing = Facings.Right;
      yield return (object) 0.1f;
      yield return (object) oshiroMasterSuite.player.DummyWalkToExact((int) oshiroMasterSuite.oshiro.X + 48, false, 1f);
      yield return (object) 1f;
      oshiroMasterSuite.player.Facing = Facings.Left;
      yield return (object) 0.2f;
      yield return (object) oshiroMasterSuite.player.DummyWalkToExact((int) oshiroMasterSuite.oshiro.X - 32, false, 1f);
      yield return (object) 0.1f;
      oshiroMasterSuite.oshiro.Sprite.Scale.X = (__Null) -1.0;
      yield return (object) 0.2f;
      oshiroMasterSuite.player.DummyAutoAnimate = false;
      oshiroMasterSuite.player.Sprite.Play("lookUp", false, false);
      yield return (object) 1f;
      oshiroMasterSuite.player.DummyAutoAnimate = true;
      yield return (object) 0.4f;
      oshiroMasterSuite.player.Facing = Facings.Right;
      yield return (object) 0.2f;
      yield return (object) oshiroMasterSuite.player.DummyWalkToExact((int) oshiroMasterSuite.oshiro.X - 24, false, 1f);
      yield return (object) 0.5f;
      yield return (object) oshiroMasterSuite.SceneAs<Level>().ZoomTo(new Vector2(190f, 110f), 2f, 0.5f);
    }

    private IEnumerator AwkwardPause()
    {
      yield return (object) 2f;
    }

    private IEnumerator SuiteShadowAppear()
    {
      CS03_OshiroMasterSuite oshiroMasterSuite = this;
      if (oshiroMasterSuite.mirror != null)
      {
        oshiroMasterSuite.mirror.EvilAppear();
        oshiroMasterSuite.SetMusic();
        Audio.Play("event:/game/03_resort/suite_bad_intro", oshiroMasterSuite.mirror.Position);
        Vector2 from = oshiroMasterSuite.Level.ZoomFocusPoint;
        Vector2 to = new Vector2(216f, 110f);
        for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime * 2f)
        {
          oshiroMasterSuite.Level.ZoomFocusPoint = Vector2.op_Addition(from, Vector2.op_Multiply(Vector2.op_Subtraction(to, from), Ease.SineInOut(p)));
          yield return (object) null;
        }
        yield return (object) null;
      }
    }

    private IEnumerator SuiteShadowDisrupt()
    {
      CS03_OshiroMasterSuite oshiroMasterSuite = this;
      if (oshiroMasterSuite.mirror != null)
      {
        Audio.Play("event:/game/03_resort/suite_bad_mirrorbreak", oshiroMasterSuite.mirror.Position);
        yield return (object) oshiroMasterSuite.mirror.SmashRoutine();
        oshiroMasterSuite.evil = new BadelineDummy(Vector2.op_Addition(oshiroMasterSuite.mirror.Position, new Vector2(0.0f, -8f)));
        oshiroMasterSuite.Scene.Add((Entity) oshiroMasterSuite.evil);
        yield return (object) 1.2f;
        oshiroMasterSuite.oshiro.Sprite.Scale.X = (__Null) 1.0;
        yield return (object) oshiroMasterSuite.evil.FloatTo(Vector2.op_Addition(oshiroMasterSuite.oshiro.Position, new Vector2(32f, -24f)), new int?(), true, false);
      }
    }

    private IEnumerator Collapse()
    {
      this.oshiro.Sprite.Play("fall", false, false);
      Audio.Play("event:/char/oshiro/chat_collapse", this.oshiro.Position);
      yield return (object) null;
    }

    private IEnumerator Console()
    {
      yield return (object) this.player.DummyWalkToExact((int) this.oshiro.X - 16, false, 1f);
    }

    private IEnumerator JumpBack()
    {
      yield return (object) this.player.DummyWalkToExact((int) this.oshiro.X - 24, true, 1f);
      yield return (object) 0.8f;
    }

    private IEnumerator SuiteShadowCeiling()
    {
      CS03_OshiroMasterSuite oshiroMasterSuite = this;
      yield return (object) oshiroMasterSuite.SceneAs<Level>().ZoomBack(0.5f);
      oshiroMasterSuite.evil.Add((Component) new SoundSource(Vector2.get_Zero(), "event:/game/03_resort/suite_bad_movestageleft"));
      BadelineDummy evil = oshiroMasterSuite.evil;
      Rectangle bounds = oshiroMasterSuite.Level.Bounds;
      Vector2 target = new Vector2((float) (((Rectangle) ref bounds).get_Left() + 96), oshiroMasterSuite.evil.Y - 16f);
      int? turnAtEndTo = new int?(1);
      yield return (object) evil.FloatTo(target, turnAtEndTo, true, false);
      oshiroMasterSuite.player.Facing = Facings.Left;
      yield return (object) 0.25f;
      oshiroMasterSuite.evil.Add((Component) new SoundSource(Vector2.get_Zero(), "event:/game/03_resort/suite_bad_ceilingbreak"));
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Long);
      oshiroMasterSuite.Level.DirectionalShake(Vector2.op_UnaryNegation(Vector2.get_UnitY()), 0.3f);
      yield return (object) oshiroMasterSuite.evil.SmashBlock(Vector2.op_Addition(oshiroMasterSuite.evil.Position, new Vector2(0.0f, -32f)));
      yield return (object) 0.8f;
    }

    private void SetMusic()
    {
      if (this.Level.Session.Area.Mode != AreaMode.Normal)
        return;
      this.Level.Session.Audio.Music.Event = "event:/music/lvl2/evil_madeline";
      this.Level.Session.Audio.Apply();
    }

    public override void OnEnd(Level level)
    {
      if (this.WasSkipped)
      {
        if (this.evil != null)
          this.Scene.Remove((Entity) this.evil);
        if (this.mirror != null)
          this.mirror.Broken();
        this.Scene.Entities.FindFirst<DashBlock>()?.RemoveAndFlagAsGone();
        this.oshiro.Sprite.Play("idle_ground", false, false);
      }
      this.oshiro.Talker.Enabled = true;
      if (this.player != null)
      {
        this.player.StateMachine.Locked = false;
        this.player.StateMachine.State = 0;
      }
      level.Lighting.Alpha = level.BaseLightingAlpha;
      level.Session.SetFlag("oshiro_resort_suite", true);
      this.SetMusic();
    }
  }
}
