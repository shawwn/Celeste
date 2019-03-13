// Decompiled with JetBrains decompiler
// Type: Celeste.CS01_Ending
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class CS01_Ending : CutsceneEntity
  {
    private Player player;
    private Bonfire bonfire;

    public CS01_Ending(Player player)
      : base(false, true)
    {
      this.player = player;
    }

    public override void OnBegin(Level level)
    {
      level.RegisterAreaComplete();
      this.bonfire = this.Scene.Tracker.GetEntity<Bonfire>();
      this.Add((Component) new Coroutine(this.Cutscene(level), true));
    }

    private IEnumerator Cutscene(Level level)
    {
      this.player.StateMachine.State = Player.StDummy;
      this.player.Dashes = 1;
      level.Session.Audio.Music.Layer(3, false);
      level.Session.Audio.Apply();
      yield return (object) 0.5f;
      yield return (object) this.player.DummyWalkTo(this.bonfire.X + 40f, false, 1f, false);
      yield return (object) 1.5f;
      this.player.Facing = Facings.Left;
      yield return (object) 0.5f;
      yield return (object) Textbox.Say("CH1_END", new Func<IEnumerator>(this.EndCityTrigger));
      yield return (object) 0.3f;
      this.EndCutscene(level, true);
    }

    private IEnumerator EndCityTrigger()
    {
      yield return (object) 0.2f;
      yield return (object) this.player.DummyWalkTo(this.bonfire.X - 12f, false, 1f, false);
      yield return (object) 0.2f;
      this.player.Facing = Facings.Right;
      this.player.DummyAutoAnimate = false;
      this.player.Sprite.Play("duck", false, false);
      yield return (object) 0.5f;
      if (this.bonfire != null)
        this.bonfire.SetMode(Bonfire.Mode.Lit);
      yield return (object) 1f;
      this.player.Sprite.Play("idle", false, false);
      yield return (object) 0.4f;
      this.player.DummyAutoAnimate = true;
      yield return (object) this.player.DummyWalkTo(this.bonfire.X - 24f, false, 1f, false);
      yield return (object) 0.4f;
      this.player.DummyAutoAnimate = false;
      this.player.Facing = Facings.Right;
      this.player.Sprite.Play("sleep", false, false);
      Audio.Play("event:/char/madeline/campfire_sit", this.player.Position);
      yield return (object) 4f;
      BirdNPC bird = new BirdNPC(this.player.Position + new Vector2(88f, -200f), BirdNPC.Modes.None);
      this.Scene.Add((Entity) bird);
      EventInstance instance = Audio.Play("event:/game/general/bird_in", bird.Position);
      bird.Facing = Facings.Left;
      bird.Sprite.Play("fall", false, false);
      Vector2 from = bird.Position;
      Vector2 to = this.player.Position + new Vector2(1f, -12f);
      float percent = 0.0f;
      while ((double) percent < 1.0)
      {
        bird.Position = from + (to - from) * Ease.QuadOut(percent);
        Audio.Position(instance, bird.Position);
        if ((double) percent > 0.5)
          bird.Sprite.Play("fly", false, false);
        percent += Engine.DeltaTime * 0.5f;
        yield return (object) null;
      }
      bird.Position = to;
      bird.Sprite.Play("idle", false, false);
      yield return (object) 0.5f;
      bird.Sprite.Play("croak", false, false);
      yield return (object) 0.6f;
      Audio.Play("event:/game/general/bird_squawk", bird.Position);
      yield return (object) 0.9f;
      bird.Sprite.Play("sleep", false, false);
      yield return (object) null;
      bird = (BirdNPC) null;
      instance = (EventInstance) null;
      from = new Vector2();
      to = new Vector2();
      yield return (object) 2f;
    }

    public override void OnEnd(Level level)
    {
      level.CompleteArea(true, false);
    }
  }
}

