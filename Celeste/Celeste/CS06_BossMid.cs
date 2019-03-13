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
      CS06_BossMid cs06BossMid = this;
      while (cs06BossMid.player == null)
      {
        cs06BossMid.player = cs06BossMid.Scene.Tracker.GetEntity<Player>();
        yield return (object) null;
      }
      cs06BossMid.player.StateMachine.State = 11;
      cs06BossMid.player.StateMachine.Locked = true;
      while (!cs06BossMid.player.OnGround(1))
        yield return (object) null;
      yield return (object) cs06BossMid.player.DummyWalkToExact((int) cs06BossMid.player.X + 20, false, 1f);
      yield return (object) level.ZoomTo(new Vector2(80f, 110f), 2f, 0.5f);
      yield return (object) Textbox.Say("ch6_boss_middle");
      yield return (object) 0.1f;
      yield return (object) level.ZoomBack(0.4f);
      cs06BossMid.EndCutscene(level, true);
    }

    public override void OnEnd(Level level)
    {
      if (this.WasSkipped && this.player != null)
      {
        while (!this.player.OnGround(1))
        {
          double y = (double) this.player.Y;
          Rectangle bounds = level.Bounds;
          double bottom = (double) ((Rectangle) ref bounds).get_Bottom();
          if (y < bottom)
            ++this.player.Y;
          else
            break;
        }
      }
      if (this.player != null)
      {
        this.player.StateMachine.Locked = false;
        this.player.StateMachine.State = 0;
      }
      level.Entities.FindFirst<FinalBoss>()?.OnPlayer((Player) null);
      level.Session.SetFlag("boss_mid", true);
    }
  }
}
