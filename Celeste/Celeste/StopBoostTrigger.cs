// Decompiled with JetBrains decompiler
// Type: Celeste.StopBoostTrigger
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;

namespace Celeste
{
  public class StopBoostTrigger : Trigger
  {
    public StopBoostTrigger(EntityData data, Vector2 offset)
      : base(data, offset)
    {
    }

    public override void OnEnter(Player player)
    {
      base.OnEnter(player);
      if (player.StateMachine.State != 10)
        return;
      player.StopSummitLaunch();
    }
  }
}
