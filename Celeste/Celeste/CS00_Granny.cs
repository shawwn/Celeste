// Decompiled with JetBrains decompiler
// Type: Celeste.CS00_Granny
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class CS00_Granny : CutsceneEntity
  {
    public const string Flag = "granny";
    private NPC00_Granny granny;
    private Player player;
    private Vector2 endPlayerPosition;
    private Coroutine zoomCoroutine;

    public CS00_Granny(NPC00_Granny granny, Player player)
      : base(true, false)
    {
      this.granny = granny;
      this.player = player;
      this.endPlayerPosition = granny.Position + new Vector2(48f, 0.0f);
    }

    public override void OnBegin(Level level)
    {
      this.Add((Component) new Coroutine(this.Cutscene(), true));
    }

    private IEnumerator Cutscene()
    {
      this.player.StateMachine.State = Player.StDummy;
      if ((double) Math.Abs(this.player.X - this.granny.X) < 20.0)
        yield return (object) this.player.DummyWalkTo(this.granny.X - 48f, false, 1f, false);
      this.player.Facing = Facings.Right;
      yield return (object) 0.5f;
      yield return (object) Textbox.Say("CH0_GRANNY", new Func<IEnumerator>(this.Meet), new Func<IEnumerator>(this.RunAlong), new Func<IEnumerator>(this.LaughAndAirQuotes), new Func<IEnumerator>(this.Laugh), new Func<IEnumerator>(this.StopLaughing), new Func<IEnumerator>(this.OminousZoom), new Func<IEnumerator>(this.PanToMaddy));
      yield return (object) this.Level.ZoomBack(0.5f);
      this.EndCutscene(this.Level, true);
    }

    private IEnumerator Meet()
    {
      yield return (object) 0.25f;
      this.granny.Sprite.Scale.X = (float) Math.Sign(this.player.X - this.granny.X);
      yield return (object) this.player.DummyWalkTo(this.granny.X - 20f, false, 1f, false);
      this.player.Facing = Facings.Right;
      yield return (object) 0.8f;
    }

    private IEnumerator RunAlong()
    {
      yield return (object) this.player.DummyWalkToExact((int) this.endPlayerPosition.X, false, 1f);
      yield return (object) 0.8f;
      this.player.Facing = Facings.Left;
      yield return (object) 0.4f;
      this.granny.Sprite.Scale.X = 1f;
      yield return (object) this.Level.ZoomTo(new Vector2(210f, 90f), 2f, 0.5f);
      yield return (object) 0.2f;
    }

    private IEnumerator LaughAndAirQuotes()
    {
      yield return (object) 0.6f;
      this.granny.LaughSfx.FirstPlay = true;
      this.granny.Sprite.Play("laugh", false, false);
      yield return (object) 2f;
      this.granny.Sprite.Play("airQuotes", false, false);
    }

    private IEnumerator Laugh()
    {
      this.granny.LaughSfx.FirstPlay = false;
      yield return (object) null;
      this.granny.Sprite.Play("laugh", false, false);
    }

    private IEnumerator StopLaughing()
    {
      this.granny.Sprite.Play("idle", false, false);
      yield break;
    }

    private IEnumerator OminousZoom()
    {
      Vector2 zoomAt = new Vector2(210f, 100f);
      this.zoomCoroutine = new Coroutine(this.Level.ZoomAcross(zoomAt, 4f, 3f), true);
      this.Add((Component) this.zoomCoroutine);
      this.granny.Sprite.Play("idle", false, false);
      yield return (object) 0.2f;
    }

    private IEnumerator PanToMaddy()
    {
      while (this.zoomCoroutine != null && this.zoomCoroutine.Active)
        yield return (object) null;
      yield return (object) 0.2f;
      yield return (object) this.Level.ZoomAcross(new Vector2(210f, 90f), 2f, 0.5f);
      yield return (object) 0.2f;
    }

    public override void OnEnd(Level level)
    {
      this.granny.Hahaha.Enabled = true;
      this.granny.Sprite.Play("laugh", false, false);
      this.granny.Sprite.Scale.X = 1f;
      this.player.Position.X = this.endPlayerPosition.X;
      this.player.Facing = Facings.Left;
      this.player.StateMachine.State = Player.StNormal;
      level.Session.SetFlag("granny", true);
      level.ResetZoom();
    }
  }
}

