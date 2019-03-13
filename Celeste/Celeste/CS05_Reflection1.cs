// Decompiled with JetBrains decompiler
// Type: Celeste.CS05_Reflection1
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class CS05_Reflection1 : CutsceneEntity
  {
    public const string Flag = "reflection";
    private Player player;

    public CS05_Reflection1(Player player)
      : base(true, false)
    {
      this.player = player;
    }

    public override void OnBegin(Level level)
    {
      this.Add((Component) new Coroutine(this.Cutscene(level), true));
    }

    private IEnumerator Cutscene(Level level)
    {
      CS05_Reflection1 cs05Reflection1 = this;
      cs05Reflection1.player.StateMachine.State = Player.StDummy;
      cs05Reflection1.player.StateMachine.Locked = true;
      cs05Reflection1.player.ForceCameraUpdate = true;
      TempleMirror first = cs05Reflection1.Scene.Entities.FindFirst<TempleMirror>();
      yield return (object) cs05Reflection1.player.DummyWalkTo((float) (first.Center.X + 8.0), false, 1f, false);
      yield return (object) 0.2f;
      cs05Reflection1.player.Facing = Facings.Left;
      yield return (object) 0.3f;
      if (!cs05Reflection1.player.Dead)
        yield return (object) Textbox.Say("ch5_reflection", new Func<IEnumerator>(cs05Reflection1.MadelineFallsToKnees), new Func<IEnumerator>(cs05Reflection1.MadelineStopsPanicking), new Func<IEnumerator>(cs05Reflection1.MadelineGetsUp));
      else
        yield return (object) 100f;
      yield return (object) cs05Reflection1.Level.ZoomBack(0.5f);
      cs05Reflection1.EndCutscene(level, true);
    }

    private IEnumerator MadelineFallsToKnees()
    {
      CS05_Reflection1 cs05Reflection1 = this;
      yield return (object) 0.2f;
      cs05Reflection1.player.DummyAutoAnimate = false;
      cs05Reflection1.player.Sprite.Play("tired", false, false);
      yield return (object) 0.2f;
      yield return (object) cs05Reflection1.Level.ZoomTo(new Vector2(90f, 116f), 2f, 0.5f);
      yield return (object) 0.2f;
    }

    private IEnumerator MadelineStopsPanicking()
    {
      yield return (object) 0.8f;
      this.player.Sprite.Play("tiredStill", false, false);
      yield return (object) 0.4f;
    }

    private IEnumerator MadelineGetsUp()
    {
      this.player.DummyAutoAnimate = true;
      this.player.Sprite.Play("idle", false, false);
      yield break;
    }

    public override void OnEnd(Level level)
    {
      this.player.StateMachine.Locked = false;
      this.player.StateMachine.State = Player.StNormal;
      this.player.ForceCameraUpdate = false;
      this.player.FlipInReflection = false;
      level.Session.SetFlag("reflection", true);
    }
  }
}
