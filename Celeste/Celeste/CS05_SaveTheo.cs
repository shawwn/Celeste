// Decompiled with JetBrains decompiler
// Type: Celeste.CS05_SaveTheo
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Celeste
{
  public class CS05_SaveTheo : CutsceneEntity
  {
    public const string Flag = "foundTheoInCrystal";
    private Player player;
    private TheoCrystal theo;
    private Vector2 playerEndPosition;

    public CS05_SaveTheo(Player player)
      : base(true, false)
    {
      this.player = player;
    }

    public override void OnBegin(Level level)
    {
      this.theo = level.Tracker.GetEntity<TheoCrystal>();
      this.playerEndPosition = Vector2.op_Addition(this.theo.Position, new Vector2(-24f, 0.0f));
      this.Add((Component) new Coroutine(this.Cutscene(level), true));
    }

    private IEnumerator Cutscene(Level level)
    {
      CS05_SaveTheo cs05SaveTheo = this;
      cs05SaveTheo.player.StateMachine.State = 11;
      cs05SaveTheo.player.StateMachine.Locked = true;
      cs05SaveTheo.player.ForceCameraUpdate = true;
      level.Session.Audio.Music.Layer(6, 0.0f);
      level.Session.Audio.Apply();
      yield return (object) cs05SaveTheo.player.DummyWalkTo(cs05SaveTheo.theo.X - 18f, false, 1f, false);
      cs05SaveTheo.player.Facing = Facings.Right;
      yield return (object) Textbox.Say("ch5_found_theo", new Func<IEnumerator>(cs05SaveTheo.TryToBreakCrystal));
      yield return (object) 0.25f;
      yield return (object) cs05SaveTheo.Level.ZoomBack(0.5f);
      cs05SaveTheo.EndCutscene(level, true);
    }

    private IEnumerator TryToBreakCrystal()
    {
      CS05_SaveTheo cs05SaveTheo = this;
      cs05SaveTheo.Scene.Entities.FindFirst<TheoCrystalPedestal>().Collidable = true;
      yield return (object) cs05SaveTheo.player.DummyWalkTo(cs05SaveTheo.theo.X, false, 1f, false);
      yield return (object) 0.1f;
      yield return (object) cs05SaveTheo.Level.ZoomTo(new Vector2(160f, 90f), 2f, 0.5f);
      cs05SaveTheo.player.DummyAutoAnimate = false;
      cs05SaveTheo.player.Sprite.Play("lookUp", false, false);
      yield return (object) 1f;
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
      MInput.Disabled = true;
      cs05SaveTheo.player.OverrideDashDirection = new Vector2?(new Vector2(0.0f, -1f));
      cs05SaveTheo.player.StateMachine.Locked = false;
      cs05SaveTheo.player.StateMachine.State = cs05SaveTheo.player.StartDash();
      cs05SaveTheo.player.Dashes = 0;
      yield return (object) 0.1f;
      while (!cs05SaveTheo.player.OnGround(1) || cs05SaveTheo.player.Speed.Y < 0.0)
      {
        cs05SaveTheo.player.Dashes = 0;
        Input.MoveY.Value = -1;
        Input.MoveX.Value = 0;
        yield return (object) null;
      }
      cs05SaveTheo.player.OverrideDashDirection = new Vector2?();
      cs05SaveTheo.player.StateMachine.State = 11;
      cs05SaveTheo.player.StateMachine.Locked = true;
      MInput.Disabled = false;
      cs05SaveTheo.player.DummyAutoAnimate = true;
      yield return (object) cs05SaveTheo.player.DummyWalkToExact((int) cs05SaveTheo.playerEndPosition.X, true, 1f);
      yield return (object) 1.5f;
    }

    public override void OnEnd(Level level)
    {
      this.player.Position = this.playerEndPosition;
      while (!this.player.OnGround(1))
        this.player.MoveV(1f, (Collision) null, (Solid) null);
      level.Camera.Position = this.player.CameraTarget;
      level.Session.SetFlag("foundTheoInCrystal", true);
      level.ResetZoom();
      level.Session.Audio.Music.Layer(6, 1f);
      level.Session.Audio.Apply();
      List<Follower> followerList = new List<Follower>((IEnumerable<Follower>) this.player.Leader.Followers);
      this.player.RemoveSelf();
      level.Add((Entity) (this.player = new Player(this.player.Position, PlayerSpriteMode.Madeline)));
      foreach (Follower follower in followerList)
      {
        this.player.Leader.Followers.Add(follower);
        follower.Leader = this.player.Leader;
      }
      this.player.Facing = Facings.Right;
      this.player.IntroType = Player.IntroTypes.None;
      TheoCrystalPedestal first = this.Scene.Entities.FindFirst<TheoCrystalPedestal>();
      first.Collidable = false;
      first.DroppedTheo = true;
      this.theo.Depth = 100;
      this.theo.OnPedestal = false;
      this.theo.Speed = Vector2.get_Zero();
      while (!this.theo.OnGround(1))
        this.theo.MoveV(1f, (Collision) null, (Solid) null);
    }
  }
}
