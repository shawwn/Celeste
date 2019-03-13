// Decompiled with JetBrains decompiler
// Type: Celeste.NegaBlock
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  [Tracked(false)]
  public class NegaBlock : Solid
  {
    public NegaBlock(Vector2 position, float width, float height)
      : base(position, width, height, false)
    {
    }

    public NegaBlock(EntityData data, Vector2 offset)
      : this(data.Position + offset, (float) data.Width, (float) data.Height)
    {
    }

    public override void Render()
    {
      base.Render();
      Draw.Rect(this.Collider, Color.Red);
    }
  }
}

