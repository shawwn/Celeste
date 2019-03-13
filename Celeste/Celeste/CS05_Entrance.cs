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
      this.player = level.Tracker.GetEntity<Player>();
      this.player.StateMachine.State = Player.StDummy;
      this.player.StateMachine.Locked = true;
      this.player.X = this.theo.X - 32f;
      this.playerMoveTo = new Vector2(this.theo.X - 32f, this.player.Y);
      this.player.Facing = Facings.Left;
      SpotlightWipe.FocusPoint = this.theo.TopCenter - Vector2.UnitX * 16f - level.Camera.Position;
      yield return (object) 2f;
      this.player.Facing = Facings.Right;
      yield return (object) 0.3f;
      yield return (object) this.theo.MoveTo(new Vector2(this.theo.X + 48f, this.theo.Y), false, new int?(), false);
      yield return (object) Textbox.Say("ch5_entrance", new Func<IEnumerator>(this.MaddyTurnsRight), new Func<IEnumerator>(this.TheoTurns), new Func<IEnumerator>(this.TheoLeaves));
      this.EndCutscene(level, true);
    }

    private IEnumerator MaddyTurnsRight()
    {
      this.player.Facing = Facings.Right;
      yield break;
    }

    private IEnumerator TheoTurns()
    {
      this.theo.Sprite.Scale.X *= -1f;
      yield break;
    }

    private IEnumerator TheoLeaves()
    {
      yield return (object) this.theo.MoveTo(new Vector2((float) (this.Level.Bounds.Right + 32), this.theo.Y), false, new int?(), false);
    }

    public override void OnEnd(Level level)
    {
      if (this.player != null)
      {
        this.player.StateMachine.Locked = false;
        this.player.StateMachine.State = Player.StNormal;
        this.player.ForceCameraUpdate = false;
        this.player.Position = this.playerMoveTo;
        this.player.Facing = Facings.Right;
      }
      this.Scene.Remove((Entity) this.theo);
      level.Session.SetFlag("entrance", true);
    }
  }
}

