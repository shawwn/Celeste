// Decompiled with JetBrains decompiler
// Type: Celeste.CS05_Entrance
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class CS05_Entrance : CutsceneEntity
  {
    public const string Flag = "entrance";
    private NPC theo;
    private Player player;
    private Vector2 playerMoveTo;

    public CS05_Entrance(NPC theo)
      : base(true, false)
    {
      this.theo = theo;
    }

    public override void OnBegin(Level level)
    {
      this.Add((Component) new Coroutine(this.Cutscene(level), true));
    }

    private IEnumerator Cutscene(Level level)
    {
      CS05_Entrance cs05Entrance = this;
      cs05Entrance.player = level.Tracker.GetEntity<Player>();
      cs05Entrance.player.StateMachine.State = 11;
      cs05Entrance.player.StateMachine.Locked = true;
      cs05Entrance.player.X = cs05Entrance.theo.X - 32f;
      cs05Entrance.playerMoveTo = new Vector2(cs05Entrance.theo.X - 32f, cs05Entrance.player.Y);
      cs05Entrance.player.Facing = Facings.Left;
      SpotlightWipe.FocusPoint = Vector2.op_Subtraction(Vector2.op_Subtraction(cs05Entrance.theo.TopCenter, Vector2.op_Multiply(Vector2.get_UnitX(), 16f)), level.Camera.Position);
      yield return (object) 2f;
      cs05Entrance.player.Facing = Facings.Right;
      yield return (object) 0.3f;
      yield return (object) cs05Entrance.theo.MoveTo(new Vector2(cs05Entrance.theo.X + 48f, cs05Entrance.theo.Y), false, new int?(), false);
      yield return (object) Textbox.Say("ch5_entrance", new Func<IEnumerator>(cs05Entrance.MaddyTurnsRight), new Func<IEnumerator>(cs05Entrance.TheoTurns), new Func<IEnumerator>(cs05Entrance.TheoLeaves));
      cs05Entrance.EndCutscene(level, true);
    }

    private IEnumerator MaddyTurnsRight()
    {
      this.player.Facing = Facings.Right;
      yield break;
    }

    private IEnumerator TheoTurns()
    {
      ref __Null local = ref this.theo.Sprite.Scale.X;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local = ^(float&) ref local * -1f;
      yield break;
    }

    private IEnumerator TheoLeaves()
    {
      // ISSUE: reference to a compiler-generated field
      int num = this.\u003C\u003E1__state;
      CS05_Entrance cs05Entrance = this;
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
      NPC theo = cs05Entrance.theo;
      Rectangle bounds = cs05Entrance.Level.Bounds;
      Vector2 target = new Vector2((float) (((Rectangle) ref bounds).get_Right() + 32), cs05Entrance.theo.Y);
      int? turnAtEndTo = new int?();
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E2__current = (object) theo.MoveTo(target, false, turnAtEndTo, false);
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = 1;
      return true;
    }

    public override void OnEnd(Level level)
    {
      if (this.player != null)
      {
        this.player.StateMachine.Locked = false;
        this.player.StateMachine.State = 0;
        this.player.ForceCameraUpdate = false;
        this.player.Position = this.playerMoveTo;
        this.player.Facing = Facings.Right;
      }
      this.Scene.Remove((Entity) this.theo);
      level.Session.SetFlag("entrance", true);
    }
  }
}
