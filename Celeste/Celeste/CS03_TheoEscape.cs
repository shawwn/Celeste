// Decompiled with JetBrains decompiler
// Type: Celeste.CS03_TheoEscape
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class CS03_TheoEscape : CutsceneEntity
  {
    public const string Flag = "resort_theo";
    private NPC03_Theo_Escaping theo;
    private Player player;
    private Vector2 theoStart;

    public CS03_TheoEscape(NPC03_Theo_Escaping theo, Player player)
      : base(true, false)
    {
      this.theo = theo;
      this.theoStart = theo.Position;
      this.player = player;
    }

    public override void OnBegin(Level level)
    {
      this.Add((Component) new Coroutine(this.Cutscene(level), true));
    }

    private IEnumerator Cutscene(Level level)
    {
      CS03_TheoEscape cs03TheoEscape = this;
      cs03TheoEscape.player.StateMachine.State = 11;
      cs03TheoEscape.player.StateMachine.Locked = true;
      yield return (object) cs03TheoEscape.player.DummyWalkTo(cs03TheoEscape.theo.X - 64f, false, 1f, false);
      cs03TheoEscape.player.Facing = Facings.Right;
      yield return (object) cs03TheoEscape.Level.ZoomTo(new Vector2(240f, 135f), 2f, 0.5f);
      Func<IEnumerator>[] funcArray = new Func<IEnumerator>[4]
      {
        new Func<IEnumerator>(cs03TheoEscape.StopRemovingVent),
        new Func<IEnumerator>(cs03TheoEscape.StartRemoveVent),
        new Func<IEnumerator>(cs03TheoEscape.RemoveVent),
        new Func<IEnumerator>(cs03TheoEscape.GivePhone)
      };
      string dialog = "CH3_THEO_INTRO";
      if (!SaveData.Instance.HasFlag("MetTheo"))
        dialog = "CH3_THEO_NEVER_MET";
      else if (!SaveData.Instance.HasFlag("TheoKnowsName"))
        dialog = "CH3_THEO_NEVER_INTRODUCED";
      yield return (object) Textbox.Say(dialog, funcArray);
      cs03TheoEscape.theo.Sprite.Scale.X = (__Null) 1.0;
      yield return (object) 0.2f;
      cs03TheoEscape.theo.Sprite.Play("walk", false, false);
      while (!cs03TheoEscape.theo.CollideCheck<Solid>(Vector2.op_Addition(cs03TheoEscape.theo.Position, new Vector2(2f, 0.0f))))
      {
        yield return (object) null;
        cs03TheoEscape.theo.X += 48f * Engine.DeltaTime;
      }
      cs03TheoEscape.theo.Sprite.Play("idle", false, false);
      yield return (object) 0.2f;
      Audio.Play("event:/char/theo/resort_standtocrawl", cs03TheoEscape.theo.Position);
      cs03TheoEscape.theo.Sprite.Play("duck", false, false);
      yield return (object) 0.5f;
      if (cs03TheoEscape.theo.Talker != null)
        cs03TheoEscape.theo.Talker.Active = false;
      level.Session.SetFlag("resort_theo", true);
      cs03TheoEscape.player.StateMachine.Locked = false;
      cs03TheoEscape.player.StateMachine.State = 0;
      cs03TheoEscape.theo.CrawlUntilOut();
      yield return (object) level.ZoomBack(0.5f);
      cs03TheoEscape.EndCutscene(level, true);
    }

    private IEnumerator StartRemoveVent()
    {
      this.theo.Sprite.Scale.X = (__Null) 1.0;
      yield return (object) 0.1f;
      Audio.Play("event:/char/theo/resort_vent_grab", this.theo.Position);
      this.theo.Sprite.Play("goToVent", false, false);
      yield return (object) 0.25f;
    }

    private IEnumerator StopRemovingVent()
    {
      this.theo.Sprite.Play("idle", false, false);
      yield return (object) 0.1f;
      this.theo.Sprite.Scale.X = (__Null) -1.0;
    }

    private IEnumerator RemoveVent()
    {
      yield return (object) 0.8f;
      Audio.Play("event:/char/theo/resort_vent_rip", this.theo.Position);
      this.theo.Sprite.Play("fallVent", false, false);
      yield return (object) 0.8f;
      this.theo.grate.Fall();
      yield return (object) 0.8f;
      this.theo.Sprite.Scale.X = (__Null) -1.0;
      yield return (object) 0.25f;
    }

    private IEnumerator GivePhone()
    {
      CS03_TheoEscape cs03TheoEscape = this;
      Player player = cs03TheoEscape.Scene.Tracker.GetEntity<Player>();
      if (player != null)
      {
        cs03TheoEscape.theo.Sprite.Play("walk", false, false);
        cs03TheoEscape.theo.Sprite.Scale.X = (__Null) -1.0;
        while ((double) cs03TheoEscape.theo.X > (double) player.X + 24.0)
        {
          cs03TheoEscape.theo.X -= 48f * Engine.DeltaTime;
          yield return (object) null;
        }
      }
      cs03TheoEscape.theo.Sprite.Play("idle", false, false);
      yield return (object) 1f;
    }

    public override void OnEnd(Level level)
    {
      this.player.StateMachine.Locked = false;
      this.player.StateMachine.State = 0;
      level.Session.SetFlag("resort_theo", true);
      SaveData.Instance.SetFlag("MetTheo");
      SaveData.Instance.SetFlag("TheoKnowsName");
      if (this.theo == null || !this.WasSkipped)
        return;
      this.theo.Position = this.theoStart;
      this.theo.CrawlUntilOut();
      if (this.theo.grate == null)
        return;
      this.theo.grate.RemoveSelf();
    }
  }
}
