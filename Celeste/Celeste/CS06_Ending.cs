// Decompiled with JetBrains decompiler
// Type: Celeste.CS06_Ending
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class CS06_Ending : CutsceneEntity
  {
    private Player player;
    private BadelineDummy badeline;
    private NPC granny;
    private NPC theo;

    public CS06_Ending(Player player, NPC granny)
      : base(false, true)
    {
      this.player = player;
      this.granny = granny;
    }

    public override void OnBegin(Level level)
    {
      level.RegisterAreaComplete();
      this.theo = (NPC) this.Scene.Entities.FindFirst<NPC06_Theo_Ending>();
      this.Add((Component) new Coroutine(this.Cutscene(level), true));
    }

    private IEnumerator Cutscene(Level level)
    {
      CS06_Ending cs06Ending = this;
      cs06Ending.player.StateMachine.State = 11;
      cs06Ending.player.StateMachine.Locked = true;
      yield return (object) 1f;
      cs06Ending.player.Dashes = 1;
      level.Session.Inventory.Dashes = 1;
      level.Add((Entity) (cs06Ending.badeline = new BadelineDummy(cs06Ending.player.Center)));
      cs06Ending.badeline.Appear(level, true);
      cs06Ending.badeline.FloatSpeed = 80f;
      cs06Ending.badeline.Sprite.Scale.X = (__Null) -1.0;
      Audio.Play("event:/char/badeline/maddy_split", cs06Ending.player.Center);
      yield return (object) cs06Ending.badeline.FloatTo(Vector2.op_Addition(cs06Ending.player.Position, new Vector2(24f, -20f)), new int?(-1), false, false);
      yield return (object) level.ZoomTo(new Vector2(160f, 120f), 2f, 1f);
      yield return (object) Textbox.Say("ch6_ending", new Func<IEnumerator>(cs06Ending.GrannyEnter), new Func<IEnumerator>(cs06Ending.TheoEnter), new Func<IEnumerator>(cs06Ending.MaddyTurnsRight), new Func<IEnumerator>(cs06Ending.BadelineTurnsRight), new Func<IEnumerator>(cs06Ending.BadelineTurnsLeft), new Func<IEnumerator>(cs06Ending.WaitAbit), new Func<IEnumerator>(cs06Ending.TurnToLeft), new Func<IEnumerator>(cs06Ending.TheoRaiseFist), new Func<IEnumerator>(cs06Ending.TheoStopTired));
      Audio.Play("event:/char/madeline/backpack_drop", cs06Ending.player.Position);
      cs06Ending.player.DummyAutoAnimate = false;
      cs06Ending.player.Sprite.Play("bagdown", false, false);
      cs06Ending.EndCutscene(level, true);
    }

    private IEnumerator GrannyEnter()
    {
      CS06_Ending cs06Ending = this;
      yield return (object) 0.25f;
      cs06Ending.badeline.Sprite.Scale.X = (__Null) 1.0;
      yield return (object) 0.1f;
      cs06Ending.granny.Visible = true;
      cs06Ending.Add((Component) new Coroutine(cs06Ending.badeline.FloatTo(new Vector2(cs06Ending.badeline.X - 10f, cs06Ending.badeline.Y), new int?(1), false, false), true));
      yield return (object) cs06Ending.granny.MoveTo(Vector2.op_Addition(cs06Ending.player.Position, new Vector2(40f, 0.0f)), false, new int?(), false);
    }

    private IEnumerator TheoEnter()
    {
      CS06_Ending cs06Ending = this;
      cs06Ending.player.Facing = Facings.Left;
      cs06Ending.badeline.Sprite.Scale.X = (__Null) -1.0;
      yield return (object) 0.25f;
      yield return (object) CutsceneEntity.CameraTo(new Vector2(cs06Ending.Level.Camera.X - 40f, cs06Ending.Level.Camera.Y), 1f, (Ease.Easer) null, 0.0f);
      cs06Ending.theo.Visible = true;
      cs06Ending.Add((Component) new Coroutine(CutsceneEntity.CameraTo(new Vector2(cs06Ending.Level.Camera.X + 40f, cs06Ending.Level.Camera.Y), 2f, (Ease.Easer) null, 1f), true));
      cs06Ending.Add((Component) new Coroutine(cs06Ending.badeline.FloatTo(new Vector2(cs06Ending.badeline.X + 6f, cs06Ending.badeline.Y + 4f), new int?(-1), false, false), true));
      yield return (object) cs06Ending.theo.MoveTo(Vector2.op_Addition(cs06Ending.player.Position, new Vector2(-32f, 0.0f)), false, new int?(), false);
      cs06Ending.theo.Sprite.Play("tired", false, false);
    }

    private IEnumerator MaddyTurnsRight()
    {
      yield return (object) 0.1f;
      this.player.Facing = Facings.Right;
      yield return (object) 0.1f;
      yield return (object) this.badeline.FloatTo(Vector2.op_Addition(this.badeline.Position, new Vector2(-2f, 10f)), new int?(-1), false, false);
      yield return (object) 0.1f;
    }

    private IEnumerator BadelineTurnsRight()
    {
      yield return (object) 0.1f;
      this.badeline.Sprite.Scale.X = (__Null) 1.0;
      yield return (object) 0.1f;
    }

    private IEnumerator BadelineTurnsLeft()
    {
      yield return (object) 0.1f;
      this.badeline.Sprite.Scale.X = (__Null) -1.0;
      yield return (object) 0.1f;
    }

    private IEnumerator WaitAbit()
    {
      yield return (object) 0.4f;
    }

    private IEnumerator TurnToLeft()
    {
      yield return (object) 0.1f;
      this.player.Facing = Facings.Left;
      yield return (object) 0.05f;
      this.badeline.Sprite.Scale.X = (__Null) -1.0;
      yield return (object) 0.1f;
    }

    private IEnumerator TheoRaiseFist()
    {
      // ISSUE: reference to a compiler-generated field
      int num = this.\u003C\u003E1__state;
      CS06_Ending cs06Ending = this;
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
      cs06Ending.theo.Sprite.Play("yolo", false, false);
      // ISSUE: reference to a compiler-generated method
      cs06Ending.Add((Component) Alarm.Create(Alarm.AlarmMode.Oneshot, new Action(cs06Ending.\u003CTheoRaiseFist\u003Eb__14_0), 0.8f, true));
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E2__current = (object) null;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = 1;
      return true;
    }

    private IEnumerator TheoStopTired()
    {
      this.theo.Sprite.Play("idle", false, false);
      yield return (object) null;
    }

    public override void OnEnd(Level level)
    {
      level.CompleteArea(true, false);
      SpotlightWipe.FocusPoint = Vector2.op_Addition(SpotlightWipe.FocusPoint, new Vector2(0.0f, -20f));
    }
  }
}
