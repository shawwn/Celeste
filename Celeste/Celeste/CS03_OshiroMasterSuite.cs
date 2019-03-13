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
      while (true)
      {
        this.player = this.Scene.Tracker.GetEntity<Player>();
        if (this.player == null)
          yield return (object) null;
        else
          break;
      }
      Audio.SetMusic((string) null, true, true);
      yield return (object) 0.4f;
      this.player.StateMachine.State = Player.StDummy;
      this.player.StateMachine.Locked = true;
      this.Add((Component) new Coroutine(this.player.DummyWalkTo(this.oshiro.X + 32f, false, 1f, false), true));
      yield return (object) 1f;
      Audio.SetMusic("event:/music/lvl3/oshiro_theme", true, true);
      yield return (object) Textbox.Say("CH3_OSHIRO_SUITE", new Func<IEnumerator>(this.SuiteShadowAppear), new Func<IEnumerator>(this.SuiteShadowDisrupt), new Func<IEnumerator>(this.SuiteShadowCeiling), new Func<IEnumerator>(this.Wander), new Func<IEnumerator>(this.Console), new Func<IEnumerator>(this.JumpBack), new Func<IEnumerator>(this.Collapse), new Func<IEnumerator>(this.AwkwardPause));
      this.evil.Add((Component) new SoundSource(Vector2.Zero, "event:/game/03_resort/suite_bad_exittop"));
      yield return (object) this.evil.FloatTo(new Vector2(this.evil.X, (float) (level.Bounds.Top - 32)), new int?(), true, false);
      this.Scene.Remove((Entity) this.evil);
      while ((double) level.Lighting.Alpha != (double) level.BaseLightingAlpha)
      {
        level.Lighting.Alpha = Calc.Approach(level.Lighting.Alpha, level.BaseLightingAlpha, Engine.DeltaTime * 0.5f);
        yield return (object) null;
      }
      this.EndCutscene(level, true);
    }

    private IEnumerator Wander()
    {
      yield return (object) 0.5f;
      this.player.Facing = Facings.Right;
      yield return (object) 0.1f;
      yield return (object) this.player.DummyWalkToExact((int) this.oshiro.X + 48, false, 1f);
      yield return (object) 1f;
      this.player.Facing = Facings.Left;
      yield return (object) 0.2f;
      yield return (object) this.player.DummyWalkToExact((int) this.oshiro.X - 32, false, 1f);
      yield return (object) 0.1f;
      this.oshiro.Sprite.Scale.X = -1f;
      yield return (object) 0.2f;
      this.player.DummyAutoAnimate = false;
      this.player.Sprite.Play("lookUp", false, false);
      yield return (object) 1f;
      this.player.DummyAutoAnimate = true;
      yield return (object) 0.4f;
      this.player.Facing = Facings.Right;
      yield return (object) 0.2f;
      yield return (object) this.player.DummyWalkToExact((int) this.oshiro.X - 24, false, 1f);
      yield return (object) 0.5f;
      Level level = this.SceneAs<Level>();
      yield return (object) level.ZoomTo(new Vector2(190f, 110f), 2f, 0.5f);
    }

    private IEnumerator AwkwardPause()
    {
      yield return (object) 2f;
    }

    private IEnumerator SuiteShadowAppear()
    {
      if (this.mirror != null)
      {
        this.mirror.EvilAppear();
        this.SetMusic();
        Audio.Play("event:/game/03_resort/suite_bad_intro", this.mirror.Position);
        Vector2 from = this.Level.ZoomFocusPoint;
        Vector2 to = new Vector2(216f, 110f);
        for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime * 2f)
        {
          this.Level.ZoomFocusPoint = from + (to - from) * Ease.SineInOut(p);
          yield return (object) null;
        }
        yield return (object) null;
      }
    }

    private IEnumerator SuiteShadowDisrupt()
    {
      if (this.mirror != null)
      {
        Audio.Play("event:/game/03_resort/suite_bad_mirrorbreak", this.mirror.Position);
        yield return (object) this.mirror.SmashRoutine();
        this.evil = new BadelineDummy(this.mirror.Position + new Vector2(0.0f, -8f));
        this.Scene.Add((Entity) this.evil);
        yield return (object) 1.2f;
        this.oshiro.Sprite.Scale.X = 1f;
        yield return (object) this.evil.FloatTo(this.oshiro.Position + new Vector2(32f, -24f), new int?(), true, false);
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
      yield return (object) this.SceneAs<Level>().ZoomBack(0.5f);
      this.evil.Add((Component) new SoundSource(Vector2.Zero, "event:/game/03_resort/suite_bad_movestageleft"));
      yield return (object) this.evil.FloatTo(new Vector2((float) (this.Level.Bounds.Left + 96), this.evil.Y - 16f), new int?(1), true, false);
      this.player.Facing = Facings.Left;
      yield return (object) 0.25f;
      this.evil.Add((Component) new SoundSource(Vector2.Zero, "event:/game/03_resort/suite_bad_ceilingbreak"));
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Long);
      this.Level.DirectionalShake(-Vector2.UnitY, 0.3f);
      yield return (object) this.evil.SmashBlock(this.evil.Position + new Vector2(0.0f, -32f));
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
        DashBlock first = this.Scene.Entities.FindFirst<DashBlock>();
        if (first != null)
          first.RemoveAndFlagAsGone();
        this.oshiro.Sprite.Play("idle_ground", false, false);
      }
      this.oshiro.Talker.Enabled = true;
      if (this.player != null)
      {
        this.player.StateMachine.Locked = false;
        this.player.StateMachine.State = Player.StNormal;
      }
      level.Lighting.Alpha = level.BaseLightingAlpha;
      level.Session.SetFlag("oshiro_resort_suite", true);
      this.SetMusic();
    }
  }
}

