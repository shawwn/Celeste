// Decompiled with JetBrains decompiler
// Type: Celeste.PropLight
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  public class PropLight : Entity
  {
    public PropLight(Vector2 position, Color color, float alpha)
      : base(position)
    {
      this.Add((Component) new VertexLight(color, alpha, 128, 256));
    }

    public PropLight(EntityData data, Vector2 offset)
      : this(data.Position + offset, data.HexColor("color", new Color()), data.Float("alpha", 0.0f))
    {
    }
  }
}

