// Decompiled with JetBrains decompiler
// Type: Celeste.NoRefillTrigger
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;

namespace Celeste
{
  public class NoRefillTrigger : Trigger
  {
    public bool State;

    public NoRefillTrigger(EntityData data, Vector2 offset)
      : base(data, offset)
    {
      this.State = data.Bool("state", false);
    }

    public override void OnEnter(Player player)
    {
      base.OnEnter(player);
      this.SceneAs<Level>().Session.Inventory.NoRefills = this.State;
    }
  }
}
