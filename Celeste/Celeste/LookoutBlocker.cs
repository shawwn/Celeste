// Decompiled with JetBrains decompiler
// Type: Celeste.LookoutBlocker
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  [Tracked(false)]
  public class LookoutBlocker : Entity
  {
    public LookoutBlocker(EntityData data, Vector2 offset)
      : base(data.Position + offset)
    {
      this.Collider = (Collider) new Hitbox((float) data.Width, (float) data.Height, 0.0f, 0.0f);
    }
  }
}

