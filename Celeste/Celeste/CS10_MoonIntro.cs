// Decompiled with JetBrains decompiler
// Type: Celeste.CS10_MoonIntro
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class CS10_MoonIntro : CutsceneEntity
  {
    public const string Flag = "moon_intro";
    private Player player;
    private BadelineDummy badeline;
    private BirdNPC bird;
    private float fade = 1f;
    private float targetX;

    public CS10_MoonIntro(Player player)
      : base()
    {
      this.Depth = -8500;
      this.player = player;
      this.targetX = player.CameraTarget.X + 8f;
    }

    public override void OnBegin(Level level)
    {
      this.bird = this.Scene.Entities.FindFirst<BirdNPC>();
      this.player.StateMachine.State = 11;
      if (level.Wipe != null)
        level.Wipe.Cancel();
      level.Wipe = (ScreenWipe) new FadeWipe((Scene) level, true);
      this.Add((Component) new Coroutine(this.Cutscene(level)));
    }

    private IEnumerator Cutscene(Level level)
    {
      CS10_MoonIntro cs10MoonIntro = this;
      cs10MoonIntro.player.StateMachine.State = 11;
      cs10MoonIntro.player.Visible = false;
      cs10MoonIntro.player.Active = false;
      cs10MoonIntro.player.Dashes = 2;
      for (float t = 0.0f; (double) t < 1.0; t += Engine.DeltaTime / 0.9f)
      {
        level.Wipe.Percent = 0.0f;
        yield return (object) null;
      }
      cs10MoonIntro.Add((Component) new Coroutine(cs10MoonIntro.FadeIn(5f)));
      level.Camera.Position = level.LevelOffset + new Vector2(-100f, 0.0f);
      yield return (object) CutsceneEntity.CameraTo(new Vector2(cs10MoonIntro.targetX, level.Camera.Y), 6f, Ease.SineOut);
      level.Camera.Position = new Vector2(cs10MoonIntro.targetX, level.Camera.Y);
      if (cs10MoonIntro.bird != null)
      {
        yield return (object) cs10MoonIntro.bird.StartleAndFlyAway();
        level.Session.DoNotLoad.Add(cs10MoonIntro.bird.EntityID);
        cs10MoonIntro.bird = (BirdNPC) null;
      }
      yield return (object) 0.5f;
      cs10MoonIntro.player.Speed = Vector2.Zero;
      cs10MoonIntro.player.Position = level.GetSpawnPoint(cs10MoonIntro.player.Position);
      cs10MoonIntro.player.Active = true;
      cs10MoonIntro.player.StateMachine.State = 23;
      while ((double) cs10MoonIntro.player.Top > (double) level.Bounds.Bottom)
        yield return (object) null;
      yield return (object) 0.2f;
      Audio.Play("event:/new_content/char/madeline/screenentry_lowgrav", cs10MoonIntro.player.Position);
      while (cs10MoonIntro.player.StateMachine.State == 23)
        yield return (object) null;
      cs10MoonIntro.player.X = (float) (int) cs10MoonIntro.player.X;
      cs10MoonIntro.player.Y = (float) (int) cs10MoonIntro.player.Y;
      while (!cs10MoonIntro.player.OnGround() && (double) cs10MoonIntro.player.Bottom < (double) level.Bounds.Bottom)
        cs10MoonIntro.player.MoveVExact(16);
      cs10MoonIntro.player.StateMachine.State = 11;
      yield return (object) 0.5f;
      yield return (object) cs10MoonIntro.BadelineAppears();
      yield return (object) Textbox.Say("CH9_LANDING", new Func<IEnumerator>(cs10MoonIntro.BadelineTurns), new Func<IEnumerator>(cs10MoonIntro.BadelineVanishes));
      cs10MoonIntro.EndCutscene(level);
    }

    private IEnumerator BadelineTurns()
    {
      CS10_MoonIntro cs10MoonIntro = this;
      yield return (object) 0.1f;
      int target = Math.Sign(cs10MoonIntro.badeline.Sprite.Scale.X) * -1;
      Wiggler wiggler = Wiggler.Create(0.5f, 3f, (Action<float>) (v => this.badeline.Sprite.Scale = new Vector2((float) target, 1f) * (float) (1.0 + 0.20000000298023224 * (double) v)), true, true);
      cs10MoonIntro.Add((Component) wiggler);
      Audio.Play(target < 0 ? "event:/char/badeline/jump_wall_left" : "event:/char/badeline/jump_wall_left", cs10MoonIntro.badeline.Position);
      yield return (object) 0.6f;
    }

    private IEnumerator BadelineAppears()
    {
      CS10_MoonIntro cs10MoonIntro = this;
      cs10MoonIntro.Level.Session.Inventory.Dashes = 1;
      cs10MoonIntro.player.Dashes = 1;
      cs10MoonIntro.Level.Add((Entity) (cs10MoonIntro.badeline = new BadelineDummy(cs10MoonIntro.player.Position)));
      cs10MoonIntro.Level.Displacement.AddBurst(cs10MoonIntro.player.Center, 0.5f, 8f, 32f, 0.5f);
      Audio.Play("event:/char/badeline/maddy_split", cs10MoonIntro.player.Position);
      cs10MoonIntro.badeline.Sprite.Scale.X = 1f;
      yield return (object) cs10MoonIntro.badeline.FloatTo(cs10MoonIntro.player.Position + new Vector2(-16f, -16f), new int?(1), false);
      cs10MoonIntro.player.Facing = Facings.Left;
      yield return (object) null;
    }

    private IEnumerator BadelineVanishes()
    {
      yield return (object) 0.5f;
      this.badeline.Vanish();
      Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
      this.badeline = (BadelineDummy) null;
      yield return (object) 0.8f;
      this.player.Facing = Facings.Right;
      yield return (object) 0.6f;
    }

    private IEnumerator FadeIn(float duration)
    {
      while ((double) this.fade > 0.0)
      {
        this.fade = Calc.Approach(this.fade, 0.0f, Engine.DeltaTime / duration);
        yield return (object) null;
      }
    }

    public override void OnEnd(Level level)
    {
      level.Session.Inventory.Dashes = 1;
      this.player.Dashes = 1;
      this.player.Depth = 0;
      this.player.Speed = Vector2.Zero;
      this.player.Position = level.GetSpawnPoint(this.player.Position) + new Vector2(0.0f, -32f);
      this.player.Active = true;
      this.player.Visible = true;
      this.player.StateMachine.State = 0;
      this.player.X = (float) (int) this.player.X;
      this.player.Y = (float) (int) this.player.Y;
      while (!this.player.OnGround() && (double) this.player.Bottom < (double) level.Bounds.Bottom)
        this.player.MoveVExact(16);
      if (this.badeline != null)
        this.badeline.RemoveSelf();
      if (this.bird != null)
      {
        this.bird.RemoveSelf();
        level.Session.DoNotLoad.Add(this.bird.EntityID);
      }
      level.Camera.Position = new Vector2(this.targetX, level.Camera.Y);
      level.Session.SetFlag("moon_intro");
    }

    public override void Render()
    {
      Camera camera = (this.Scene as Level).Camera;
      Draw.Rect(camera.X - 10f, camera.Y - 10f, 340f, 200f, Color.Black * this.fade);
    }
  }
}
