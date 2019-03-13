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
      this.playerEndPosition = this.theo.Position + new Vector2(-24f, 0.0f);
      this.Add((Component) new Coroutine(this.Cutscene(level), true));
    }

    private IEnumerator Cutscene(Level level)
    {
      this.player.StateMachine.State = Player.StDummy;
      this.player.StateMachine.Locked = true;
      this.player.ForceCameraUpdate = true;
      level.Session.Audio.Music.Layer(6, 0.0f);
      level.Session.Audio.Apply();
      yield return (object) this.player.DummyWalkTo(this.theo.X - 18f, false, 1f, false);
      this.player.Facing = Facings.Right;
      yield return (object) Textbox.Say("ch5_found_theo", new Func<IEnumerator>(this.TryToBreakCrystal));
      yield return (object) 0.25f;
      yield return (object) this.Level.ZoomBack(0.5f);
      this.EndCutscene(level, true);
    }

    private IEnumerator TryToBreakCrystal()
    {
      TheoCrystalPedestal pedestal = this.Scene.Entities.FindFirst<TheoCrystalPedestal>();
      pedestal.Collidable = true;
      yield return (object) this.player.DummyWalkTo(this.theo.X, false, 1f, false);
      yield return (object) 0.1f;
      yield return (object) this.Level.ZoomTo(new Vector2(160f, 90f), 2f, 0.5f);
      this.player.DummyAutoAnimate = false;
      this.player.Sprite.Play("lookUp", false, false);
      yield return (object) 1f;
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
      MInput.Disabled = true;
      this.player.OverrideDashDirection = new Vector2?(new Vector2(0.0f, -1f));
      this.player.StateMachine.Locked = false;
      this.player.StateMachine.State = this.player.StartDash();
      this.player.Dashes = 0;
      yield return (object) 0.1f;
      while (!this.player.OnGround(1) || (double) this.player.Speed.Y < 0.0)
      {
        this.player.Dashes = 0;
        Input.MoveY.Value = -1;
        Input.MoveX.Value = 0;
        yield return (object) null;
      }
      this.player.OverrideDashDirection = new Vector2?();
      this.player.StateMachine.State = Player.StDummy;
      this.player.StateMachine.Locked = true;
      MInput.Disabled = false;
      this.player.DummyAutoAnimate = true;
      yield return (object) this.player.DummyWalkToExact((int) this.playerEndPosition.X, true, 1f);
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
      this.theo.Speed = Vector2.Zero;
      while (!this.theo.OnGround(1))
        this.theo.MoveV(1f, (Collision) null, (Solid) null);
    }
  }
}

