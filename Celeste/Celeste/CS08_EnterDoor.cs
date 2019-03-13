// Decompiled with JetBrains decompiler
// Type: Celeste.CS08_EnterDoor
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class CS08_EnterDoor : CutsceneEntity
  {
    private Player player;
    private float targetX;

    public CS08_EnterDoor(Player player, float targetX)
      : base(true, false)
    {
      this.player = player;
      this.targetX = targetX;
    }

    public override void OnBegin(Level level)
    {
      this.Add((Component) new Coroutine(this.Cutscene(level), true));
    }

    private IEnumerator Cutscene(Level level)
    {
      this.player.StateMachine.State = Player.StDummy;
      this.Add((Component) new Coroutine(this.player.DummyWalkToExact((int) this.targetX, false, 0.7f), true));
      this.Add((Component) new Coroutine(level.ZoomTo(new Vector2(this.targetX - level.Camera.X, 90f), 2f, 2f), true));
      FadeWipe wipe = new FadeWipe((Scene) level, false, (Action) null);
      wipe.Duration = 2f;
      yield return (object) wipe.Wait();
      this.EndCutscene(level, true);
    }

    public override void OnEnd(Level level)
    {
      level.OnEndOfFrame += (Action) (() =>
      {
        level.Remove((Entity) this.player);
        level.UnloadLevel();
        level.Session.Level = "inside";
        Session session = level.Session;
        Level level1 = level;
        Rectangle bounds = level.Bounds;
        double left = (double) bounds.Left;
        bounds = level.Bounds;
        double top = (double) bounds.Top;
        Vector2 from = new Vector2((float) left, (float) top);
        Vector2? nullable = new Vector2?(level1.GetSpawnPoint(from));
        session.RespawnPoint = nullable;
        level.LoadLevel(Player.IntroTypes.None, false);
        level.Add((Entity) new CS08_Ending());
      });
    }
  }
}

