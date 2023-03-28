// Decompiled with JetBrains decompiler
// Type: Celeste.CS10_Farewell
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
  public class CS10_Farewell : CutsceneEntity
  {
    private Player player;
    private NPC granny;
    private float fade;
    private Coroutine grannyWalk;
    private EventInstance snapshot;
    private EventInstance dissipate;

    public CS10_Farewell(Player player)
      : base(false)
    {
      this.player = player;
      this.Depth = -1000000;
    }

    public override void Awake(Scene scene)
    {
      base.Awake(scene);
      Level level = scene as Level;
      level.TimerStopped = true;
      level.TimerHidden = true;
      level.SaveQuitDisabled = true;
      level.SnapColorGrade("none");
      this.snapshot = Audio.CreateSnapshot("snapshot:/game_10_granny_clouds_dialogue");
    }

    public override void OnBegin(Level level) => this.Add((Component) new Coroutine(this.Cutscene(level)));

    private IEnumerator Cutscene(Level level)
    {
      CS10_Farewell cs10Farewell = this;
      cs10Farewell.player.Dashes = 1;
      cs10Farewell.player.StateMachine.State = 11;
      cs10Farewell.player.Sprite.Play("idle");
      cs10Farewell.player.Visible = false;
      Audio.SetMusic("event:/new_content/music/lvl10/granny_farewell");
      FadeWipe fadeWipe = new FadeWipe((Scene) cs10Farewell.Level, true);
      fadeWipe.Duration = 2f;
      ScreenWipe.WipeColor = Color.White;
      yield return (object) fadeWipe.Duration;
      yield return (object) 1.5f;
      cs10Farewell.Add((Component) new Coroutine(cs10Farewell.Level.ZoomTo(new Vector2(160f, 125f), 2f, 5f)));
      yield return (object) 0.2f;
      Audio.Play("event:/new_content/char/madeline/screenentry_gran");
      yield return (object) 0.3f;
      Vector2 position = cs10Farewell.player.Position;
      cs10Farewell.player.Position = new Vector2(cs10Farewell.player.X, (float) (level.Bounds.Bottom + 8));
      cs10Farewell.player.Speed.Y = -160f;
      cs10Farewell.player.Visible = true;
      cs10Farewell.player.DummyGravity = false;
      cs10Farewell.player.MuffleLanding = true;
      Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
      while (!cs10Farewell.player.OnGround() || (double) cs10Farewell.player.Speed.Y < 0.0)
      {
        double y = (double) cs10Farewell.player.Speed.Y;
        cs10Farewell.player.Speed.Y += (float) ((double) Engine.DeltaTime * 900.0 * 0.20000000298023224);
        if (y < 0.0 && (double) cs10Farewell.player.Speed.Y >= 0.0)
        {
          cs10Farewell.player.Speed.Y = 0.0f;
          yield return (object) 0.2f;
        }
        yield return (object) null;
      }
      Input.Rumble(RumbleStrength.Light, RumbleLength.Short);
      Audio.Play("event:/new_content/char/madeline/screenentry_gran_landing", cs10Farewell.player.Position);
      cs10Farewell.granny = new NPC(cs10Farewell.player.Position + new Vector2(164f, 0.0f));
      cs10Farewell.granny.IdleAnim = "idle";
      cs10Farewell.granny.MoveAnim = "walk";
      cs10Farewell.granny.Maxspeed = 15f;
      cs10Farewell.granny.Add((Component) (cs10Farewell.granny.Sprite = GFX.SpriteBank.Create("granny")));
      cs10Farewell.granny.Add((Component) new GrannyLaughSfx(cs10Farewell.granny.Sprite)
      {
        FirstPlay = false
      });
      // ISSUE: reference to a compiler-generated method
      // cs10Farewell.granny.Sprite.OnFrameChange = new Action<string>(cs10Farewell.\u003CCutscene\u003Eb__9_0);
      cs10Farewell.granny.Sprite.OnFrameChange = new Action<string>((x) =>
      {
        Console.WriteLine("TKTK TODO: CS10_Farewell granny.Sprite.OnFrameChange");
      });
      cs10Farewell.Scene.Add((Entity) cs10Farewell.granny);
      cs10Farewell.grannyWalk = new Coroutine(cs10Farewell.granny.MoveTo(cs10Farewell.player.Position + new Vector2(32f, 0.0f)));
      cs10Farewell.Add((Component) cs10Farewell.grannyWalk);
      yield return (object) 2f;
      cs10Farewell.player.Facing = Facings.Left;
      yield return (object) 1.6f;
      cs10Farewell.player.Facing = Facings.Right;
      yield return (object) 0.8f;
      yield return (object) cs10Farewell.player.DummyWalkToExact((int) cs10Farewell.player.X + 4, speedMultiplier: 0.4f);
      yield return (object) 0.8f;
      yield return (object) Textbox.Say("CH9_FAREWELL", new Func<IEnumerator>(cs10Farewell.Laugh), new Func<IEnumerator>(cs10Farewell.StopLaughing), new Func<IEnumerator>(cs10Farewell.StepForward), new Func<IEnumerator>(cs10Farewell.GrannyDisappear), new Func<IEnumerator>(cs10Farewell.FadeToWhite), new Func<IEnumerator>(cs10Farewell.WaitForGranny));
      yield return (object) 2f;
      while ((double) cs10Farewell.fade < 1.0)
        yield return (object) null;
      cs10Farewell.EndCutscene(level);
    }

    private IEnumerator WaitForGranny()
    {
      while (this.grannyWalk != null && !this.grannyWalk.Finished)
        yield return (object) null;
    }

    private IEnumerator Laugh()
    {
      this.granny.Sprite.Play("laugh");
      yield break;
    }

    private IEnumerator StopLaughing()
    {
      this.granny.Sprite.Play("idle");
      yield break;
    }

    private IEnumerator StepForward()
    {
      yield return (object) this.player.DummyWalkToExact((int) this.player.X + 8, speedMultiplier: 0.4f);
    }

    private IEnumerator GrannyDisappear()
    {
      CS10_Farewell cs10Farewell = this;
      Audio.SetMusicParam("end", 1f);
      cs10Farewell.Add((Component) new Coroutine(cs10Farewell.player.DummyWalkToExact((int) cs10Farewell.player.X + 8, speedMultiplier: 0.4f)));
      yield return (object) 0.1f;
      cs10Farewell.dissipate = Audio.Play("event:/new_content/char/granny/dissipate", cs10Farewell.granny.Position);
      MTexture frame = cs10Farewell.granny.Sprite.GetFrame(cs10Farewell.granny.Sprite.CurrentAnimationID, cs10Farewell.granny.Sprite.CurrentAnimationFrame);
      cs10Farewell.Level.Add((Entity) new DisperseImage(cs10Farewell.granny.Position, new Vector2(1f, -0.1f), cs10Farewell.granny.Sprite.Origin, cs10Farewell.granny.Sprite.Scale, frame));
      yield return (object) null;
      cs10Farewell.granny.Visible = false;
      yield return (object) 3.5f;
    }

    // private IEnumerator FadeToWhite()
    // {
    //   // ISSUE: reference to a compiler-generated field
    //   int num = this.\u003C\u003E1__state;
    //   CS10_Farewell cs10Farewell = this;
    //   if (num != 0)
    //     return false;
    //   // ISSUE: reference to a compiler-generated field
    //   this.\u003C\u003E1__state = -1;
    //   cs10Farewell.Add((Component) new Coroutine(cs10Farewell.DoFadeToWhite()));
    //   return false;
    // }
    
    private IEnumerator FadeToWhite()
    {
      Add((Component) new Coroutine(DoFadeToWhite()));
      yield break;
    }

    private IEnumerator DoFadeToWhite()
    {
      CS10_Farewell cs10Farewell = this;
      cs10Farewell.Add((Component) new Coroutine(cs10Farewell.Level.ZoomBack(8f)));
      while ((double) cs10Farewell.fade < 1.0)
      {
        cs10Farewell.fade = Calc.Approach(cs10Farewell.fade, 1f, Engine.DeltaTime / 8f);
        yield return (object) null;
      }
    }

    public override void OnEnd(Level level)
    {
      this.Dispose();
      if (this.WasSkipped)
        Audio.Stop(this.dissipate);
      this.Level.OnEndOfFrame += (Action) (() =>
      {
        Achievements.Register(Achievement.FAREWELL);
        this.Level.TeleportTo(this.player, "end-cinematic", Player.IntroTypes.Transition);
      });
    }

    public override void SceneEnd(Scene scene)
    {
      base.SceneEnd(scene);
      this.Dispose();
    }

    public override void Removed(Scene scene)
    {
      base.Removed(scene);
      this.Dispose();
    }

    private void Dispose()
    {
      Audio.ReleaseSnapshot(this.snapshot);
      this.snapshot = (EventInstance) null;
    }

    public override void Render()
    {
      if ((double) this.fade <= 0.0)
        return;
      Draw.Rect(this.Level.Camera.X - 1f, this.Level.Camera.Y - 1f, 322f, 182f, Color.White * this.fade);
    }
  }
}
