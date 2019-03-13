// Decompiled with JetBrains decompiler
// Type: Celeste.CS06_Granny
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class CS06_Granny : CutsceneEntity
  {
    public const string FlagPrefix = "granny_";
    private NPC06_Granny granny;
    private Player player;
    private float startX;
    private int index;
    private bool firstLaugh;

    public CS06_Granny(NPC06_Granny granny, Player player, int index)
      : base(true, false)
    {
      this.granny = granny;
      this.player = player;
      this.index = index;
    }

    public override void OnBegin(Level level)
    {
      this.Add((Component) new Coroutine(this.Cutscene(level), true));
    }

    private IEnumerator Cutscene(Level level)
    {
      CS06_Granny cs06Granny = this;
      cs06Granny.player.StateMachine.State = 11;
      cs06Granny.player.StateMachine.Locked = true;
      cs06Granny.player.ForceCameraUpdate = true;
      if (cs06Granny.index == 0)
      {
        yield return (object) cs06Granny.player.DummyWalkTo(cs06Granny.granny.X - 40f, false, 1f, false);
        cs06Granny.startX = cs06Granny.player.X;
        cs06Granny.player.Facing = Facings.Right;
        cs06Granny.firstLaugh = true;
        yield return (object) Textbox.Say("ch6_oldlady", new Func<IEnumerator>(cs06Granny.ZoomIn), new Func<IEnumerator>(cs06Granny.Laughs), new Func<IEnumerator>(cs06Granny.StopLaughing), new Func<IEnumerator>(cs06Granny.MaddyWalksRight), new Func<IEnumerator>(cs06Granny.MaddyWalksLeft), new Func<IEnumerator>(cs06Granny.WaitABit), new Func<IEnumerator>(cs06Granny.MaddyTurnsRight));
      }
      else if (cs06Granny.index == 1)
      {
        yield return (object) cs06Granny.ZoomIn();
        yield return (object) cs06Granny.player.DummyWalkTo(cs06Granny.granny.X - 20f, false, 1f, false);
        cs06Granny.player.Facing = Facings.Right;
        yield return (object) Textbox.Say("ch6_oldlady_b");
      }
      else if (cs06Granny.index == 2)
      {
        yield return (object) cs06Granny.ZoomIn();
        yield return (object) cs06Granny.player.DummyWalkTo(cs06Granny.granny.X - 20f, false, 1f, false);
        cs06Granny.player.Facing = Facings.Right;
        yield return (object) Textbox.Say("ch6_oldlady_c");
      }
      yield return (object) cs06Granny.Level.ZoomBack(0.5f);
      cs06Granny.EndCutscene(level, true);
    }

    private IEnumerator ZoomIn()
    {
      // ISSUE: reference to a compiler-generated field
      int num = this.\u003C\u003E1__state;
      CS06_Granny cs06Granny = this;
      if (num != 0)
      {
        if (num != 1)
          return false;
        // ISSUE: reference to a compiler-generated field
        this.\u003C\u003E1__state = -1;
        return false;
      }
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      Vector2 screenSpaceFocusPoint = Vector2.op_Addition(Vector2.op_Subtraction(Vector2.Lerp(cs06Granny.granny.Position, cs06Granny.player.Position, 0.5f), cs06Granny.Level.Camera.Position), new Vector2(0.0f, -20f));
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E2__current = (object) cs06Granny.Level.ZoomTo(screenSpaceFocusPoint, 2f, 0.5f);
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = 1;
      return true;
    }

    private IEnumerator Laughs()
    {
      if (this.firstLaugh)
      {
        this.firstLaugh = false;
        yield return (object) 0.5f;
      }
      this.granny.Sprite.Play("laugh", false, false);
      yield return (object) 1f;
    }

    private IEnumerator StopLaughing()
    {
      this.granny.Sprite.Play("idle", false, false);
      yield return (object) 0.25f;
    }

    private IEnumerator MaddyWalksLeft()
    {
      yield return (object) 0.1f;
      this.player.Facing = Facings.Left;
      yield return (object) this.player.DummyWalkToExact((int) this.player.X - 8, false, 1f);
      yield return (object) 0.1f;
    }

    private IEnumerator MaddyWalksRight()
    {
      yield return (object) 0.1f;
      this.player.Facing = Facings.Right;
      yield return (object) this.player.DummyWalkToExact((int) this.player.X + 8, false, 1f);
      yield return (object) 0.1f;
    }

    private IEnumerator WaitABit()
    {
      yield return (object) 0.8f;
    }

    private IEnumerator MaddyTurnsRight()
    {
      yield return (object) 0.1f;
      this.player.Facing = Facings.Right;
      yield return (object) 0.1f;
    }

    public override void OnEnd(Level level)
    {
      this.player.StateMachine.Locked = false;
      this.player.StateMachine.State = 0;
      this.player.ForceCameraUpdate = false;
      this.granny.Sprite.Play("idle", false, false);
      level.Session.SetFlag("granny_" + (object) this.index, true);
    }
  }
}
