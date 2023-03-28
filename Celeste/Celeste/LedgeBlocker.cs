// Decompiled with JetBrains decompiler
// Type: Celeste.LedgeBlocker
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

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
            if (!this.Blocking || !player.CollideCheck(this.Entity, player.Position + Vector2.UnitX * (float) player.Facing * 8f))
                return false;
            return this.BlockChecker == null || this.BlockChecker(player);
        }

        public bool JumpThruBoostCheck(Player player)
        {
            if (!this.Blocking || !player.CollideCheck(this.Entity, player.Position - Vector2.UnitY * 2f))
                return false;
            return this.BlockChecker == null || this.BlockChecker(player);
        }

        public bool DashCorrectCheck(Player player)
        {
            if (!this.Blocking || !player.CollideCheck(this.Entity, player.Position))
                return false;
            return this.BlockChecker == null || this.BlockChecker(player);
        }
    }
}