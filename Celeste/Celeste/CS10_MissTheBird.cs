// Decompiled with JetBrains decompiler
// Type: Celeste.CS10_MissTheBird
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class CS10_MissTheBird : CutsceneEntity
  {
    public const string Flag = "MissTheBird";
    private Player player;
    private FlingBirdIntro flingBird;
    private BirdNPC bird;
    private Coroutine zoomRoutine;
    private EventInstance crashMusicSfx;

    public CS10_MissTheBird(Player player, FlingBirdIntro flingBird)
      : base()
    {
      this.player = player;
      this.flingBird = flingBird;
      this.Add((Component) new LevelEndingHook((Action) (() => Audio.Stop(this.crashMusicSfx))));
    }

    public override void OnBegin(Level level) => this.Add((Component) new Coroutine(this.Cutscene(level)));

    private IEnumerator Cutscene(Level level)
    {
      CS10_MissTheBird cs10MissTheBird = this;
      Audio.SetMusicParam("bird_grab", 1f);
      cs10MissTheBird.crashMusicSfx = Audio.Play("event:/new_content/music/lvl10/cinematic/bird_crash_first");
      yield return (object) cs10MissTheBird.flingBird.DoGrabbingRoutine(cs10MissTheBird.player);
      cs10MissTheBird.bird = new BirdNPC(cs10MissTheBird.flingBird.Position, BirdNPC.Modes.None);
      level.Add((Entity) cs10MissTheBird.bird);
      cs10MissTheBird.flingBird.RemoveSelf();
      yield return (object) null;
      level.ResetZoom();
      level.Shake(0.5f);
      cs10MissTheBird.player.Position = cs10MissTheBird.player.Position.Floor();
      cs10MissTheBird.player.DummyGravity = true;
      cs10MissTheBird.player.DummyAutoAnimate = false;
      cs10MissTheBird.player.DummyFriction = false;
      cs10MissTheBird.player.ForceCameraUpdate = true;
      cs10MissTheBird.player.Speed = new Vector2(200f, 200f);
      BirdNPC bird = cs10MissTheBird.bird;
      bird.Position = bird.Position + Vector2.UnitX * 16f;
      cs10MissTheBird.bird.Add((Component) new Coroutine(cs10MissTheBird.bird.Startle((string) null, 0.5f, new Vector2?(new Vector2(3f, 0.25f)))));
      // ISSUE: reference to a compiler-generated method
      // cs10MissTheBird.Add((Component) Alarm.Create(Alarm.AlarmMode.Oneshot, new Action(cs10MissTheBird.\u003CCutscene\u003Eb__8_0), 0.1f, true));
      cs10MissTheBird.Add((Component) Alarm.Create(Alarm.AlarmMode.Oneshot, new Action(() =>
      {
        Console.WriteLine("TKTK TODO: CS10_MissTheBird.cs: What should we do here?");
      }), 0.1f, true));
      while (!cs10MissTheBird.player.OnGround())
        cs10MissTheBird.player.MoveVExact(1);
      Engine.TimeRate = 0.5f;
      cs10MissTheBird.player.Sprite.Play("roll");
      while ((double) cs10MissTheBird.player.Speed.X != 0.0)
      {
        cs10MissTheBird.player.Speed.X = Calc.Approach(cs10MissTheBird.player.Speed.X, 0.0f, 120f * Engine.DeltaTime);
        if (cs10MissTheBird.Scene.OnInterval(0.1f))
          Dust.BurstFG(cs10MissTheBird.player.Position, -1.5707964f, 2);
        yield return (object) null;
      }
      while ((double) Engine.TimeRate < 1.0)
      {
        Engine.TimeRate = Calc.Approach(Engine.TimeRate, 1f, 4f * Engine.DeltaTime);
        yield return (object) null;
      }
      cs10MissTheBird.player.Speed.X = 0.0f;
      cs10MissTheBird.player.DummyFriction = true;
      yield return (object) 0.25f;
      cs10MissTheBird.Add((Component) (cs10MissTheBird.zoomRoutine = new Coroutine(level.ZoomTo(new Vector2(160f, 110f), 1.5f, 6f))));
      yield return (object) 1.5f;
      cs10MissTheBird.player.Sprite.Play("rollGetUp");
      yield return (object) 0.5f;
      cs10MissTheBird.player.ForceCameraUpdate = false;
      yield return (object) Textbox.Say("CH9_MISS_THE_BIRD", new Func<IEnumerator>(cs10MissTheBird.StandUpFaceLeft), new Func<IEnumerator>(cs10MissTheBird.TakeStepLeft), new Func<IEnumerator>(cs10MissTheBird.TakeStepRight), new Func<IEnumerator>(cs10MissTheBird.FlickerBlackhole), new Func<IEnumerator>(cs10MissTheBird.OpenBlackhole));
      cs10MissTheBird.StartMusic();
      cs10MissTheBird.EndCutscene(level);
    }

    private IEnumerator StandUpFaceLeft()
    {
      while (!this.zoomRoutine.Finished)
        yield return (object) null;
      yield return (object) 0.2f;
      Audio.Play("event:/char/madeline/stand", this.player.Position);
      this.player.DummyAutoAnimate = true;
      this.player.Sprite.Play("idle");
      yield return (object) 0.2f;
      this.player.Facing = Facings.Left;
      yield return (object) 0.5f;
    }

    private IEnumerator TakeStepLeft()
    {
      yield return (object) this.player.DummyWalkTo(this.player.X - 16f);
    }

    private IEnumerator TakeStepRight()
    {
      yield return (object) this.player.DummyWalkTo(this.player.X + 32f);
    }

    private IEnumerator FlickerBlackhole()
    {
      yield return (object) 0.5f;
      Audio.Play("event:/new_content/game/10_farewell/glitch_medium");
      yield return (object) MoonGlitchBackgroundTrigger.GlitchRoutine(0.5f, false);
      yield return (object) this.player.DummyWalkTo(this.player.X - 8f, true);
      yield return (object) 0.4f;
    }

    private IEnumerator OpenBlackhole()
    {
      CS10_MissTheBird cs10MissTheBird = this;
      yield return (object) 0.2f;
      cs10MissTheBird.Level.ResetZoom();
      cs10MissTheBird.Level.Flash(Color.White);
      cs10MissTheBird.Level.Shake(0.4f);
      Level level1 = cs10MissTheBird.Level;
      double x1 = (double) cs10MissTheBird.player.X;
      Rectangle bounds = cs10MissTheBird.Level.Bounds;
      double top1 = (double) bounds.Top;
      LightningStrike lightningStrike1 = new LightningStrike(new Vector2((float) x1, (float) top1), 80, 240f);
      level1.Add((Entity) lightningStrike1);
      Level level2 = cs10MissTheBird.Level;
      double x2 = (double) cs10MissTheBird.player.X - 100.0;
      bounds = cs10MissTheBird.Level.Bounds;
      double top2 = (double) bounds.Top;
      LightningStrike lightningStrike2 = new LightningStrike(new Vector2((float) x2, (float) top2), 90, 240f, 0.5f);
      level2.Add((Entity) lightningStrike2);
      Audio.Play("event:/new_content/game/10_farewell/lightning_strike");
      cs10MissTheBird.TriggerEnvironmentalEvents();
      cs10MissTheBird.StartMusic();
      yield return (object) 1.2f;
    }

    private void StartMusic()
    {
      this.Level.Session.Audio.Music.Event = "event:/new_content/music/lvl10/part03";
      this.Level.Session.Audio.Ambience.Event = "event:/new_content/env/10_voidspiral";
      this.Level.Session.Audio.Apply();
    }

    private void TriggerEnvironmentalEvents()
    {
      CutsceneNode cutsceneNode = CutsceneNode.Find("player_skip");
      if (cutsceneNode != null)
        RumbleTrigger.ManuallyTrigger(cutsceneNode.X, 0.0f);
      this.Scene.Entities.FindFirst<MoonGlitchBackgroundTrigger>()?.Invoke();
    }

    public override void OnEnd(Level level)
    {
      Audio.Stop(this.crashMusicSfx);
      Engine.TimeRate = 1f;
      level.Session.SetFlag("MissTheBird");
      if (this.WasSkipped)
      {
        this.player.Sprite.Play("idle");
        CutsceneNode cutsceneNode = CutsceneNode.Find("player_skip");
        if (cutsceneNode != null)
        {
          this.player.Position = cutsceneNode.Position.Floor();
          level.Camera.Position = this.player.CameraTarget;
        }
        if (this.flingBird != null)
        {
          if (this.flingBird.CrashSfxEmitter != null)
            this.flingBird.CrashSfxEmitter.RemoveSelf();
          this.flingBird.RemoveSelf();
        }
        if (this.bird != null)
          this.bird.RemoveSelf();
        this.TriggerEnvironmentalEvents();
        this.StartMusic();
      }
      this.player.Speed = Vector2.Zero;
      this.player.DummyAutoAnimate = true;
      this.player.DummyFriction = true;
      this.player.DummyGravity = true;
      this.player.ForceCameraUpdate = false;
      this.player.StateMachine.State = 0;
    }
  }
}
