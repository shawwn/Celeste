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
      CS00_Ending cs00Ending = this;
      for (; (double) Engine.TimeRate > 0.0; Engine.TimeRate -= Engine.RawDeltaTime * 2f)
      {
        yield return (object) null;
        if ((double) Engine.TimeRate < 0.5 && cs00Ending.bridge != null)
          cs00Ending.bridge.StopCollapseLoop();
        level.StopShake();
        MInput.GamePads[Input.Gamepad].StopRumble();
      }
      Engine.TimeRate = 0.0f;
      cs00Ending.player.StateMachine.State = 11;
      cs00Ending.player.Facing = Facings.Right;
      yield return (object) cs00Ending.WaitFor(1f);
      EventInstance instance = Audio.Play("event:/game/general/bird_in", cs00Ending.bird.Position);
      cs00Ending.bird.Facing = Facings.Left;
      cs00Ending.bird.Sprite.Play("fall", false, false);
      float percent = 0.0f;
      Vector2 from = cs00Ending.bird.Position;
      Vector2 to = cs00Ending.bird.StartPosition;
      while ((double) percent < 1.0)
      {
        cs00Ending.bird.Position = Vector2.op_Addition(from, Vector2.op_Multiply(Vector2.op_Subtraction(to, from), Ease.QuadOut(percent)));
        Audio.Position(instance, cs00Ending.bird.Position);
        if ((double) percent > 0.5)
          cs00Ending.bird.Sprite.Play("fly", false, false);
        percent += Engine.RawDeltaTime * 0.5f;
        yield return (object) null;
      }
      cs00Ending.bird.Position = to;
      instance = (EventInstance) null;
      from = (Vector2) null;
      to = (Vector2) null;
      Audio.Play("event:/game/general/bird_land_dirt", cs00Ending.bird.Position);
      Dust.Burst(cs00Ending.bird.Position, -1.570796f, 12);
      cs00Ending.bird.Sprite.Play("idle", false, false);
      yield return (object) cs00Ending.WaitFor(0.5f);
      cs00Ending.bird.Sprite.Play("peck", false, false);
      yield return (object) cs00Ending.WaitFor(1.1f);
      yield return (object) cs00Ending.bird.ShowTutorial(new BirdTutorialGui((Entity) cs00Ending.bird, new Vector2(0.0f, -16f), (object) Dialog.Clean("tutorial_dash", (Language) null), new object[3]
      {
        (object) new Vector2(1f, -1f),
        (object) "+",
        (object) Input.Dash
      }), true);
      while (true)
      {
        Vector2 aimVector = Input.GetAimVector(Facings.Right);
        if (aimVector.X <= 0.0 || aimVector.Y >= 0.0 || !Input.Dash.Pressed)
          yield return (object) null;
        else
          break;
      }
      cs00Ending.player.StateMachine.State = 16;
      cs00Ending.player.Dashes = 0;
      level.Session.Inventory.Dashes = 1;
      Engine.TimeRate = 1f;
      cs00Ending.keyOffed = true;
      int num1 = (int) Audio.CurrentMusicEventInstance.triggerCue();
      cs00Ending.bird.Add((Component) new Coroutine(cs00Ending.bird.HideTutorial(), true));
      yield return (object) 0.25f;
      cs00Ending.bird.Add((Component) new Coroutine(cs00Ending.bird.StartleAndFlyAway(), true));
      while (!cs00Ending.player.Dead && !cs00Ending.player.OnGround(1))
        yield return (object) null;
      yield return (object) 2f;
      Audio.SetMusic("event:/music/lvl0/title_ping", true, true);
      yield return (object) 2f;
      cs00Ending.endingText = new PrologueEndingText(false);
      cs00Ending.Scene.Add((Entity) cs00Ending.endingText);
      Snow bgSnow = level.Background.Get<Snow>();
      Snow fgSnow = level.Foreground.Get<Snow>();
      level.Add((Monocle.Renderer) (level.HiresSnow = new HiresSnow(0.45f)));
      level.HiresSnow.Alpha = 0.0f;
      float ease = 0.0f;
      while ((double) ease < 1.0)
      {
        ease += Engine.DeltaTime * 0.25f;
        float num2 = Ease.CubeInOut(ease);
        if (fgSnow != null)
          fgSnow.Alpha -= Engine.DeltaTime * 0.5f;
        if (bgSnow != null)
          bgSnow.Alpha -= Engine.DeltaTime * 0.5f;
        level.HiresSnow.Alpha = Calc.Approach(level.HiresSnow.Alpha, 1f, Engine.DeltaTime * 0.5f);
        cs00Ending.endingText.Position = new Vector2(960f, (float) (540.0 - 1080.0 * (1.0 - (double) num2)));
        Camera camera = level.Camera;
        Rectangle bounds = level.Bounds;
        double num3 = (double) ((Rectangle) ref bounds).get_Top() - 3900.0 * (double) num2;
        camera.Y = (float) num3;
        yield return (object) null;
      }
      cs00Ending.EndCutscene(level, true);
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
          this.player.StateMachine.State = 11;
          this.player.DummyAutoAnimate = false;
          this.player.Sprite.Play("tired", false, false);
          this.player.Speed = Vector2.get_Zero();
        }
        if (!this.keyOffed)
        {
          int num1 = (int) Audio.CurrentMusicEventInstance.triggerCue();
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
        Camera camera = level.Camera;
        Rectangle bounds = level.Bounds;
        double num2 = (double) (((Rectangle) ref bounds).get_Top() - 3900);
        camera.Y = (float) num2;
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
        // ISSUE: reference to a compiler-generated field
        int num = this.\u003C\u003E1__state;
        CS00_Ending.EndingCutsceneDelay endingCutsceneDelay = this;
        if (num != 0)
        {
          if (num != 1)
            return false;
          // ISSUE: reference to a compiler-generated field
          this.\u003C\u003E1__state = -1;
          (endingCutsceneDelay.Scene as Level).CompleteArea(false, false);
          return false;
        }
        // ISSUE: reference to a compiler-generated field
        this.\u003C\u003E1__state = -1;
        // ISSUE: reference to a compiler-generated field
        this.\u003C\u003E2__current = (object) 3f;
        // ISSUE: reference to a compiler-generated field
        this.\u003C\u003E1__state = 1;
        return true;
      }
    }
  }
}
