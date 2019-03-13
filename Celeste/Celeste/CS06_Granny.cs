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
      this.player.StateMachine.State = Player.StDummy;
      this.player.StateMachine.Locked = true;
      this.player.ForceCameraUpdate = true;
      if (this.index == 0)
      {
        yield return (object) this.player.DummyWalkTo(this.granny.X - 40f, false, 1f, false);
        this.startX = this.player.X;
        this.player.Facing = Facings.Right;
        this.firstLaugh = true;
        yield return (object) Textbox.Say("ch6_oldlady", new Func<IEnumerator>(this.ZoomIn), new Func<IEnumerator>(this.Laughs), new Func<IEnumerator>(this.StopLaughing), new Func<IEnumerator>(this.MaddyWalksRight), new Func<IEnumerator>(this.MaddyWalksLeft), new Func<IEnumerator>(this.WaitABit), new Func<IEnumerator>(this.MaddyTurnsRight));
      }
      else if (this.index == 1)
      {
        yield return (object) this.ZoomIn();
        yield return (object) this.player.DummyWalkTo(this.granny.X - 20f, false, 1f, false);
        this.player.Facing = Facings.Right;
        yield return (object) Textbox.Say("ch6_oldlady_b");
      }
      else if (this.index == 2)
      {
        yield return (object) this.ZoomIn();
        yield return (object) this.player.DummyWalkTo(this.granny.X - 20f, false, 1f, false);
        this.player.Facing = Facings.Right;
        yield return (object) Textbox.Say("ch6_oldlady_c");
      }
      yield return (object) this.Level.ZoomBack(0.5f);
      this.EndCutscene(level, true);
    }

    private IEnumerator ZoomIn()
    {
      Vector2 center = Vector2.Lerp(this.granny.Position, this.player.Position, 0.5f) - this.Level.Camera.Position + new Vector2(0.0f, -20f);
      yield return (object) this.Level.ZoomTo(center, 2f, 0.5f);
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
      this.player.StateMachine.State = Player.StNormal;
      this.player.ForceCameraUpdate = false;
      this.granny.Sprite.Play("idle", false, false);
      level.Session.SetFlag("granny_" + (object) this.index, true);
    }
  }
}

