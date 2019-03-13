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
      CS05_SeeTheo cs05SeeTheo = this;
      while (cs05SeeTheo.player.Scene == null || !cs05SeeTheo.player.OnGround(1))
        yield return (object) null;
      cs05SeeTheo.player.StateMachine.State = 11;
      cs05SeeTheo.player.StateMachine.Locked = true;
      yield return (object) 0.25f;
      cs05SeeTheo.theo = cs05SeeTheo.Scene.Tracker.GetEntity<TheoCrystal>();
      if (cs05SeeTheo.theo != null && Math.Sign(cs05SeeTheo.player.X - cs05SeeTheo.theo.X) != 0)
        cs05SeeTheo.player.Facing = (Facings) Math.Sign(cs05SeeTheo.theo.X - cs05SeeTheo.player.X);
      yield return (object) 0.25f;
      if (cs05SeeTheo.index == 0)
        yield return (object) Textbox.Say("ch5_see_theo", new Func<IEnumerator>(cs05SeeTheo.ZoomIn), new Func<IEnumerator>(cs05SeeTheo.MadelineTurnsAround), new Func<IEnumerator>(cs05SeeTheo.WaitABit), new Func<IEnumerator>(cs05SeeTheo.MadelineTurnsBackAndBrighten));
      else if (cs05SeeTheo.index == 1)
        yield return (object) Textbox.Say("ch5_see_theo_b");
      yield return (object) cs05SeeTheo.Level.ZoomBack(0.5f);
      cs05SeeTheo.EndCutscene(level, true);
    }

    private IEnumerator ZoomIn()
    {
      // ISSUE: reference to a compiler-generated field
      int num = this.\u003C\u003E1__state;
      CS05_SeeTheo cs05SeeTheo = this;
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
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E2__current = (object) cs05SeeTheo.Level.ZoomTo(Vector2.op_Addition(Vector2.op_Subtraction(Vector2.Lerp(cs05SeeTheo.player.Position, cs05SeeTheo.theo.Position, 0.5f), cs05SeeTheo.Level.Camera.Position), new Vector2(0.0f, -20f)), 2f, 0.5f);
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = 1;
      return true;
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
      CS05_SeeTheo cs05SeeTheo = this;
      yield return (object) 0.1f;
      Coroutine coroutine = new Coroutine(cs05SeeTheo.Brighten(), true);
      cs05SeeTheo.Add((Component) coroutine);
      yield return (object) 0.2f;
      cs05SeeTheo.player.Facing = Facings.Right;
      yield return (object) 0.1f;
      while (coroutine.Active)
        yield return (object) null;
    }

    private IEnumerator Brighten()
    {
      CS05_SeeTheo cs05SeeTheo = this;
      yield return (object) cs05SeeTheo.Level.ZoomBack(0.5f);
      yield return (object) 0.3f;
      cs05SeeTheo.Level.Session.DarkRoomAlpha = 0.3f;
      float darkness = cs05SeeTheo.Level.Session.DarkRoomAlpha;
      while ((double) cs05SeeTheo.Level.Lighting.Alpha != (double) darkness)
      {
        cs05SeeTheo.Level.Lighting.Alpha = Calc.Approach(cs05SeeTheo.Level.Lighting.Alpha, darkness, Engine.DeltaTime * 0.5f);
        yield return (object) null;
      }
    }

    public override void OnEnd(Level level)
    {
      this.player.StateMachine.Locked = false;
      this.player.StateMachine.State = 0;
      this.player.ForceCameraUpdate = false;
      this.player.DummyAutoAnimate = true;
      level.Session.DarkRoomAlpha = 0.3f;
      level.Lighting.Alpha = level.Session.DarkRoomAlpha;
      level.Session.SetFlag("seeTheoInCrystal", true);
    }
  }
}
