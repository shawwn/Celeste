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
      CS01_Ending cs01Ending = this;
      cs01Ending.player.StateMachine.State = 11;
      cs01Ending.player.Dashes = 1;
      level.Session.Audio.Music.Layer(3, false);
      level.Session.Audio.Apply();
      yield return (object) 0.5f;
      yield return (object) cs01Ending.player.DummyWalkTo(cs01Ending.bonfire.X + 40f, false, 1f, false);
      yield return (object) 1.5f;
      cs01Ending.player.Facing = Facings.Left;
      yield return (object) 0.5f;
      yield return (object) Textbox.Say("CH1_END", new Func<IEnumerator>(cs01Ending.EndCityTrigger));
      yield return (object) 0.3f;
      cs01Ending.EndCutscene(level, true);
    }

    private IEnumerator EndCityTrigger()
    {
      CS01_Ending cs01Ending = this;
      yield return (object) 0.2f;
      yield return (object) cs01Ending.player.DummyWalkTo(cs01Ending.bonfire.X - 12f, false, 1f, false);
      yield return (object) 0.2f;
      cs01Ending.player.Facing = Facings.Right;
      cs01Ending.player.DummyAutoAnimate = false;
      cs01Ending.player.Sprite.Play("duck", false, false);
      yield return (object) 0.5f;
      if (cs01Ending.bonfire != null)
        cs01Ending.bonfire.SetMode(Bonfire.Mode.Lit);
      yield return (object) 1f;
      cs01Ending.player.Sprite.Play("idle", false, false);
      yield return (object) 0.4f;
      cs01Ending.player.DummyAutoAnimate = true;
      yield return (object) cs01Ending.player.DummyWalkTo(cs01Ending.bonfire.X - 24f, false, 1f, false);
      yield return (object) 0.4f;
      cs01Ending.player.DummyAutoAnimate = false;
      cs01Ending.player.Facing = Facings.Right;
      cs01Ending.player.Sprite.Play("sleep", false, false);
      Audio.Play("event:/char/madeline/campfire_sit", cs01Ending.player.Position);
      yield return (object) 4f;
      BirdNPC bird = new BirdNPC(Vector2.op_Addition(cs01Ending.player.Position, new Vector2(88f, -200f)), BirdNPC.Modes.None);
      cs01Ending.Scene.Add((Entity) bird);
      EventInstance instance = Audio.Play("event:/game/general/bird_in", bird.Position);
      bird.Facing = Facings.Left;
      bird.Sprite.Play("fall", false, false);
      Vector2 from = bird.Position;
      Vector2 to = Vector2.op_Addition(cs01Ending.player.Position, new Vector2(1f, -12f));
      float percent = 0.0f;
      while ((double) percent < 1.0)
      {
        bird.Position = Vector2.op_Addition(from, Vector2.op_Multiply(Vector2.op_Subtraction(to, from), Ease.QuadOut(percent)));
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
      from = (Vector2) null;
      to = (Vector2) null;
      yield return (object) 2f;
    }

    public override void OnEnd(Level level)
    {
      level.CompleteArea(true, false);
    }
  }
}
