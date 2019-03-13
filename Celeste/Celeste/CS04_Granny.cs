// Decompiled with JetBrains decompiler
// Type: Celeste.CS04_Granny
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class CS04_Granny : CutsceneEntity
  {
    public const string Flag = "granny_1";
    private NPC04_Granny granny;
    private Player player;

    public CS04_Granny(NPC04_Granny granny, Player player)
      : base(true, false)
    {
      this.granny = granny;
      this.player = player;
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
      yield return (object) this.player.DummyWalkTo(this.granny.X - 30f, false, 1f, false);
      this.player.Facing = Facings.Right;
      yield return (object) Textbox.Say("CH4_GRANNY_1", new Func<IEnumerator>(this.Laughs), new Func<IEnumerator>(this.StopLaughing), new Func<IEnumerator>(this.WaitABeat), new Func<IEnumerator>(this.ZoomIn), new Func<IEnumerator>(this.MaddyTurnsAround), new Func<IEnumerator>(this.MaddyApproaches), new Func<IEnumerator>(this.MaddyWalksPastGranny));
      yield return (object) this.Level.ZoomBack(0.5f);
      this.EndCutscene(level, true);
    }

    private IEnumerator Laughs()
    {
      this.granny.Sprite.Play("laugh", false, false);
      yield return (object) 1f;
    }

    private IEnumerator StopLaughing()
    {
      this.granny.Sprite.Play("idle", false, false);
      yield return (object) 0.25f;
    }

    private IEnumerator WaitABeat()
    {
      yield return (object) 1.2f;
    }

    private IEnumerator ZoomIn()
    {
      yield return (object) this.Level.ZoomTo(new Vector2(123f, 116f), 2f, 0.5f);
    }

    private IEnumerator MaddyTurnsAround()
    {
      yield return (object) 0.2f;
      this.player.Facing = Facings.Left;
      yield return (object) 0.1f;
    }

    private IEnumerator MaddyApproaches()
    {
      yield return (object) this.player.DummyWalkTo(this.granny.X - 20f, false, 1f, false);
    }

    private IEnumerator MaddyWalksPastGranny()
    {
      yield return (object) this.player.DummyWalkToExact((int) this.granny.X + 30, false, 1f);
    }

    public override void OnEnd(Level level)
    {
      this.player.X = this.granny.X + 30f;
      this.player.StateMachine.Locked = false;
      this.player.StateMachine.State = Player.StNormal;
      this.player.ForceCameraUpdate = false;
      if (this.WasSkipped)
        level.Camera.Position = this.player.CameraTarget;
      this.granny.Sprite.Play("laugh", false, false);
      level.Session.SetFlag("granny_1", true);
    }
  }
}

