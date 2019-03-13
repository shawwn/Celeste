// Decompiled with JetBrains decompiler
// Type: Celeste.NPC08_Theo
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  public class NPC08_Theo : NPC
  {
    public NPC08_Theo(EntityData data, Vector2 position)
      : base(data.Position + position)
    {
      this.Add((Component) (this.Sprite = GFX.SpriteBank.Create("theo")));
      this.Sprite.Scale.X = -1f;
      this.Sprite.Play("idle", false, false);
      this.IdleAnim = "idle";
      this.MoveAnim = "walk";
      this.Maxspeed = 30f;
    }
  }
}

