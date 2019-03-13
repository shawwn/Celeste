// Decompiled with JetBrains decompiler
// Type: Celeste.LedgeBlocker
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  [Tracked(false)]
  public class LedgeBlocker : Component
  {
    public bool Blocking = true;
    public Func<Player, bool> BlockChecker;

    public LedgeBlocker(Func<Player, bool> blockChecker = null)
      : base(false, false)
    {
      this.BlockChecker = blockChecker;
    }

    public bool HopBlockCheck(Player player)
    {
      if (!this.Blocking || !player.CollideCheck(this.Entity, Vector2.op_Addition(player.Position, Vector2.op_Multiply(Vector2.op_Multiply(Vector2.get_UnitX(), (float) player.Facing), 8f))))
        return false;
      if (this.BlockChecker != null)
        return this.BlockChecker(player);
      return true;
    }

    public bool JumpThruBoostCheck(Player player)
    {
      if (!this.Blocking || !player.CollideCheck(this.Entity, Vector2.op_Subtraction(player.Position, Vector2.op_Multiply(Vector2.get_UnitY(), 2f))))
        return false;
      if (this.BlockChecker != null)
        return this.BlockChecker(player);
      return true;
    }
  }
}
