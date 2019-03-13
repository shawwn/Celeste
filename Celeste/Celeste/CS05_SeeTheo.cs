// Decompiled with JetBrains decompiler
// Type: Celeste.CS05_SeeTheo
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class CS05_SeeTheo : CutsceneEntity
  {
    private const float NewDarknessAlpha = 0.3f;
    public const string Flag = "seeTheoInCrystal";
    private int index;
    private Player player;
    private TheoCrystal theo;

    public CS05_SeeTheo(Player player, int index)
      : base(true, false)
    {
      this.player = player;
      this.index = index;
    }

    public override void OnBegin(Level level)
    {
      this.Add((Component) new Coroutine(this.Cutscene(level), true));
    }

    private IEnumerator Cutscene(Level level)
    {
      while (this.player.Scene == null || !this.player.OnGround(1))
        yield return (object) null;
      this.player.StateMachine.State = Player.StDummy;
      this.player.StateMachine.Locked = true;
      yield return (object) 0.25f;
      this.theo = this.Scene.Tracker.GetEntity<TheoCrystal>();
      if (this.theo != null && (uint) Math.Sign(this.player.X - this.theo.X) > 0U)
        this.player.Facing = (Facings) Math.Sign(this.theo.X - this.player.X);
      yield return (object) 0.25f;
      if (this.index == 0)
        yield return (object) Textbox.Say("ch5_see_theo", new Func<IEnumerator>(this.ZoomIn), new Func<IEnumerator>(this.MadelineTurnsAround), new Func<IEnumerator>(this.WaitABit), new Func<IEnumerator>(this.MadelineTurnsBackAndBrighten));
      else if (this.index == 1)
        yield return (object) Textbox.Say("ch5_see_theo_b");
      yield return (object) this.Level.ZoomBack(0.5f);
      this.EndCutscene(level, true);
    }

    private IEnumerator ZoomIn()
    {
      yield return (object) this.Level.ZoomTo(Vector2.Lerp(this.player.Position, this.theo.Position, 0.5f) - this.Level.Camera.Position + new Vector2(0.0f, -20f), 2f, 0.5f);
    }

    private IEnumerator MadelineTurnsAround()
    {
      yield return (object) 0.3f;
      this.player.Facing = Facings.Left;
      yield return (object) 0.1f;
    }

    private IEnumerator WaitABit()
    {
      yield return (object) 1f;
    }

    private IEnumerator MadelineTurnsBackAndBrighten()
    {
      yield return (object) 0.1f;
      Coroutine coroutine = new Coroutine(this.Brighten(), true);
      this.Add((Component) coroutine);
      yield return (object) 0.2f;
      this.player.Facing = Facings.Right;
      yield return (object) 0.1f;
      while (coroutine.Active)
        yield return (object) null;
    }

    private IEnumerator Brighten()
    {
      yield return (object) this.Level.ZoomBack(0.5f);
      yield return (object) 0.3f;
      this.Level.Session.DarkRoomAlpha = 0.3f;
      float darkness = this.Level.Session.DarkRoomAlpha;
      while ((double) this.Level.Lighting.Alpha != (double) darkness)
      {
        this.Level.Lighting.Alpha = Calc.Approach(this.Level.Lighting.Alpha, darkness, Engine.DeltaTime * 0.5f);
        yield return (object) null;
      }
    }

    public override void OnEnd(Level level)
    {
      this.player.StateMachine.Locked = false;
      this.player.StateMachine.State = Player.StNormal;
      this.player.ForceCameraUpdate = false;
      this.player.DummyAutoAnimate = true;
      level.Session.DarkRoomAlpha = 0.3f;
      level.Lighting.Alpha = level.Session.DarkRoomAlpha;
      level.Session.SetFlag("seeTheoInCrystal", true);
    }
  }
}

