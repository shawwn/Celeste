// Decompiled with JetBrains decompiler
// Type: Celeste.CS06_BossMid
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System.Collections;

namespace Celeste
{
  public class CS06_BossMid : CutsceneEntity
  {
    public const string Flag = "boss_mid";
    private Player player;

    public CS06_BossMid()
      : base(true, false)
    {
    }

    public override void OnBegin(Level level)
    {
      this.Add((Component) new Coroutine(this.Cutscene(level), true));
    }

    private IEnumerator Cutscene(Level level)
    {
      while (this.player == null)
      {
        this.player = this.Scene.Tracker.GetEntity<Player>();
        yield return (object) null;
      }
      this.player.StateMachine.State = Player.StDummy;
      this.player.StateMachine.Locked = true;
      while (!this.player.OnGround(1))
        yield return (object) null;
      yield return (object) this.player.DummyWalkToExact((int) this.player.X + 20, false, 1f);
      yield return (object) level.ZoomTo(new Vector2(80f, 110f), 2f, 0.5f);
      yield return (object) Textbox.Say("ch6_boss_middle");
      yield return (object) 0.1f;
      yield return (object) level.ZoomBack(0.4f);
      this.EndCutscene(level, true);
    }

    public override void OnEnd(Level level)
    {
      Player player;
      if (this.WasSkipped && this.player != null)
      {
        for (; !this.player.OnGround(1) && (double) this.player.Y < (double) level.Bounds.Bottom; ++player.Y)
          player = this.player;
      }
      if (this.player != null)
      {
        this.player.StateMachine.Locked = false;
        this.player.StateMachine.State = Player.StNormal;
      }
      FinalBoss first = level.Entities.FindFirst<FinalBoss>();
      if (first != null)
        first.OnPlayer((Player) null);
      level.Session.SetFlag("boss_mid", true);
    }
  }
}

