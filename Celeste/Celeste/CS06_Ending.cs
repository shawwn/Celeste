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
      this.player.StateMachine.State = Player.StDummy;
      this.player.StateMachine.Locked = true;
      yield return (object) 1f;
      this.player.Dashes = 1;
      level.Session.Inventory.Dashes = 1;
      level.Add((Entity) (this.badeline = new BadelineDummy(this.player.Center)));
      this.badeline.Appear(level, true);
      this.badeline.FloatSpeed = 80f;
      this.badeline.Sprite.Scale.X = -1f;
      Audio.Play("event:/char/badeline/maddy_split", this.player.Center);
      yield return (object) this.badeline.FloatTo(this.player.Position + new Vector2(24f, -20f), new int?(-1), false, false);
      yield return (object) level.ZoomTo(new Vector2(160f, 120f), 2f, 1f);
      yield return (object) Textbox.Say("ch6_ending", new Func<IEnumerator>(this.GrannyEnter), new Func<IEnumerator>(this.TheoEnter), new Func<IEnumerator>(this.MaddyTurnsRight), new Func<IEnumerator>(this.BadelineTurnsRight), new Func<IEnumerator>(this.BadelineTurnsLeft), new Func<IEnumerator>(this.WaitAbit), new Func<IEnumerator>(this.TurnToLeft), new Func<IEnumerator>(this.TheoRaiseFist), new Func<IEnumerator>(this.TheoStopTired));
      Audio.Play("event:/char/madeline/backpack_drop", this.player.Position);
      this.player.DummyAutoAnimate = false;
      this.player.Sprite.Play("bagdown", false, false);
      this.EndCutscene(level, true);
    }

    private IEnumerator GrannyEnter()
    {
      yield return (object) 0.25f;
      this.badeline.Sprite.Scale.X = 1f;
      yield return (object) 0.1f;
      this.granny.Visible = true;
      this.Add((Component) new Coroutine(this.badeline.FloatTo(new Vector2(this.badeline.X - 10f, this.badeline.Y), new int?(1), false, false), true));
      yield return (object) this.granny.MoveTo(this.player.Position + new Vector2(40f, 0.0f), false, new int?(), false);
    }

    private IEnumerator TheoEnter()
    {
      this.player.Facing = Facings.Left;
      this.badeline.Sprite.Scale.X = -1f;
      yield return (object) 0.25f;
      yield return (object) CutsceneEntity.CameraTo(new Vector2(this.Level.Camera.X - 40f, this.Level.Camera.Y), 1f, (Ease.Easer) null, 0.0f);
      this.theo.Visible = true;
      this.Add((Component) new Coroutine(CutsceneEntity.CameraTo(new Vector2(this.Level.Camera.X + 40f, this.Level.Camera.Y), 2f, (Ease.Easer) null, 1f), true));
      this.Add((Component) new Coroutine(this.badeline.FloatTo(new Vector2(this.badeline.X + 6f, this.badeline.Y + 4f), new int?(-1), false, false), true));
      yield return (object) this.theo.MoveTo(this.player.Position + new Vector2(-32f, 0.0f), false, new int?(), false);
      this.theo.Sprite.Play("tired", false, false);
    }

    private IEnumerator MaddyTurnsRight()
    {
      yield return (object) 0.1f;
      this.player.Facing = Facings.Right;
      yield return (object) 0.1f;
      yield return (object) this.badeline.FloatTo(this.badeline.Position + new Vector2(-2f, 10f), new int?(-1), false, false);
      yield return (object) 0.1f;
    }

    private IEnumerator BadelineTurnsRight()
    {
      yield return (object) 0.1f;
      this.badeline.Sprite.Scale.X = 1f;
      yield return (object) 0.1f;
    }

    private IEnumerator BadelineTurnsLeft()
    {
      yield return (object) 0.1f;
      this.badeline.Sprite.Scale.X = -1f;
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
      this.badeline.Sprite.Scale.X = -1f;
      yield return (object) 0.1f;
    }

    private IEnumerator TheoRaiseFist()
    {
      this.theo.Sprite.Play("yolo", false, false);
      this.Add((Component) Alarm.Create(Alarm.AlarmMode.Oneshot, (Action) (() => this.theo.Sprite.Play("yoloEnd", false, false)), 0.8f, true));
      yield return (object) null;
    }

    private IEnumerator TheoStopTired()
    {
      this.theo.Sprite.Play("idle", false, false);
      yield return (object) null;
    }

    public override void OnEnd(Level level)
    {
      level.CompleteArea(true, false);
      SpotlightWipe.FocusPoint += new Vector2(0.0f, -20f);
    }
  }
}

