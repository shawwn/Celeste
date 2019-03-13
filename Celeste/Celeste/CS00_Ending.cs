// Decompiled with JetBrains decompiler
// Type: Celeste.CS00_Ending
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections;

namespace Celeste
{
  public class CS00_Ending : CutsceneEntity
  {
    private Player player;
    private BirdNPC bird;
    private Bridge bridge;
    private bool keyOffed;
    private PrologueEndingText endingText;

    public CS00_Ending(Player player, BirdNPC bird, Bridge bridge)
      : base(false, true)
    {
      this.player = player;
      this.bird = bird;
      this.bridge = bridge;
    }

    public override void OnBegin(Level level)
    {
      this.Add((Component) new Coroutine(this.Cutscene(level), true));
    }

    private IEnumerator Cutscene(Level level)
    {
      for (; (double) Engine.TimeRate > 0.0; Engine.TimeRate -= Engine.RawDeltaTime * 2f)
      {
        yield return (object) null;
        if ((double) Engine.TimeRate < 0.5 && this.bridge != null)
          this.bridge.StopCollapseLoop();
        level.StopShake();
        MInput.GamePads[Input.Gamepad].StopRumble();
      }
      Engine.TimeRate = 0.0f;
      this.player.StateMachine.State = Player.StDummy;
      this.player.Facing = Facings.Right;
      yield return (object) this.WaitFor(1f);
      EventInstance instance = Audio.Play("event:/game/general/bird_in", this.bird.Position);
      this.bird.Facing = Facings.Left;
      this.bird.Sprite.Play("fall", false, false);
      float percent = 0.0f;
      Vector2 from = this.bird.Position;
      Vector2 to = this.bird.StartPosition;
      while ((double) percent < 1.0)
      {
        this.bird.Position = from + (to - from) * Ease.QuadOut(percent);
        Audio.Position(instance, this.bird.Position);
        if ((double) percent > 0.5)
          this.bird.Sprite.Play("fly", false, false);
        percent += Engine.RawDeltaTime * 0.5f;
        yield return (object) null;
      }
      this.bird.Position = to;
      instance = (EventInstance) null;
      from = new Vector2();
      to = new Vector2();
      Audio.Play("event:/game/general/bird_land_dirt", this.bird.Position);
      Dust.Burst(this.bird.Position, -1.570796f, 12);
      this.bird.Sprite.Play("idle", false, false);
      yield return (object) this.WaitFor(0.5f);
      this.bird.Sprite.Play("peck", false, false);
      yield return (object) this.WaitFor(1.1f);
      yield return (object) this.bird.ShowTutorial(new BirdTutorialGui((Entity) this.bird, new Vector2(0.0f, -16f), (object) Dialog.Clean("tutorial_dash", (Language) null), new object[3]
      {
        (object) new Vector2(1f, -1f),
        (object) "+",
        (object) Input.Dash
      }), true);
      while (true)
      {
        Vector2 aim = Input.GetAimVector(Facings.Right);
        if ((double) aim.X <= 0.0 || (double) aim.Y >= 0.0 || !Input.Dash.Pressed)
        {
          yield return (object) null;
          aim = new Vector2();
        }
        else
          break;
      }
      this.player.StateMachine.State = Player.StBirdDashTutorial;
      this.player.Dashes = 0;
      level.Session.Inventory.Dashes = 1;
      Engine.TimeRate = 1f;
      this.keyOffed = true;
      int num = (int) Audio.CurrentMusicEventInstance.triggerCue();
      this.bird.Add((Component) new Coroutine(this.bird.HideTutorial(), true));
      yield return (object) 0.25f;
      this.bird.Add((Component) new Coroutine(this.bird.StartleAndFlyAway(), true));
      while (!this.player.Dead && !this.player.OnGround(1))
        yield return (object) null;
      yield return (object) 2f;
      Audio.SetMusic("event:/music/lvl0/title_ping", true, true);
      yield return (object) 2f;
      this.endingText = new PrologueEndingText(false);
      this.Scene.Add((Entity) this.endingText);
      Snow bgSnow = level.Background.Get<Snow>();
      Snow fgSnow = level.Foreground.Get<Snow>();
      level.Add((Monocle.Renderer) (level.HiresSnow = new HiresSnow(0.45f)));
      level.HiresSnow.Alpha = 0.0f;
      float ease = 0.0f;
      while ((double) ease < 1.0)
      {
        ease += Engine.DeltaTime * 0.25f;
        float e = Ease.CubeInOut(ease);
        if (fgSnow != null)
          fgSnow.Alpha -= Engine.DeltaTime * 0.5f;
        if (bgSnow != null)
          bgSnow.Alpha -= Engine.DeltaTime * 0.5f;
        level.HiresSnow.Alpha = Calc.Approach(level.HiresSnow.Alpha, 1f, Engine.DeltaTime * 0.5f);
        this.endingText.Position = new Vector2(960f, (float) (540.0 - 1080.0 * (1.0 - (double) e)));
        level.Camera.Y = (float) level.Bounds.Top - 3900f * e;
        yield return (object) null;
      }
      this.EndCutscene(level, true);
    }

    private IEnumerator WaitFor(float time)
    {
      for (float t = 0.0f; (double) t < (double) time; t += Engine.RawDeltaTime)
        yield return (object) null;
    }

    public override void OnEnd(Level level)
    {
      if (this.WasSkipped)
      {
        if (this.bird != null)
          this.bird.Visible = false;
        if (this.player != null)
        {
          this.player.Position = new Vector2(2120f, 40f);
          this.player.StateMachine.State = Player.StDummy;
          this.player.DummyAutoAnimate = false;
          this.player.Sprite.Play("tired", false, false);
          this.player.Speed = Vector2.Zero;
        }
        if (!this.keyOffed)
        {
          int num = (int) Audio.CurrentMusicEventInstance.triggerCue();
        }
        if (level.HiresSnow == null)
          level.Add((Monocle.Renderer) (level.HiresSnow = new HiresSnow(0.45f)));
        level.HiresSnow.Alpha = 1f;
        Snow snow1 = level.Background.Get<Snow>();
        if (snow1 != null)
          snow1.Alpha = 0.0f;
        Snow snow2 = level.Foreground.Get<Snow>();
        if (snow2 != null)
          snow2.Alpha = 0.0f;
        if (this.endingText != null)
          level.Remove((Entity) this.endingText);
        level.Add((Entity) (this.endingText = new PrologueEndingText(true)));
        this.endingText.Position = new Vector2(960f, 540f);
        level.Camera.Y = (float) (level.Bounds.Top - 3900);
      }
      Engine.TimeRate = 1f;
      level.PauseLock = true;
      level.Entities.FindFirst<SpeedrunTimerDisplay>().CompleteTimer = 10f;
      level.Add((Entity) new CS00_Ending.EndingCutsceneDelay());
    }

    private class EndingCutsceneDelay : Entity
    {
      public EndingCutsceneDelay()
      {
        this.Add((Component) new Coroutine(this.Routine(), true));
      }

      private IEnumerator Routine()
      {
        yield return (object) 3f;
        Level level = this.Scene as Level;
        level.CompleteArea(false, false);
      }
    }
  }
}

