// Decompiled with JetBrains decompiler
// Type: Celeste.ChaserBarrier
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  [Tracked(false)]
  public class ChaserBarrier : Entity
  {
    public ChaserBarrier(Vector2 position, int width, int height)
      : base(position)
    {
      this.Collider = (Collider) new Hitbox((float) width, (float) height, 0.0f, 0.0f);
    }

    public ChaserBarrier(EntityData data, Vector2 offset)
      : this(data.Position + offset, data.Width, data.Height)
    {
    }

    public override void Render()
    {
      base.Render();
      Draw.Rect(this.Collider, Color.Red * 0.3f);
    }
  }
}

