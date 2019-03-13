// Decompiled with JetBrains decompiler
// Type: Celeste.SinkingPlatformLine
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  public class SinkingPlatformLine : Entity
  {
    private Color lineEdgeColor = Calc.HexToColor("2a1923");
    private Color lineInnerColor = Calc.HexToColor("160b12");
    private float height;

    public SinkingPlatformLine(Vector2 position)
    {
      this.Position = position;
      this.Depth = 9001;
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      this.height = (float) this.SceneAs<Level>().Bounds.Height - (this.Y - (float) this.SceneAs<Level>().Bounds.Y);
    }

    public override void Render()
    {
      Draw.Rect(this.X - 1f, this.Y, 3f, this.height, this.lineEdgeColor);
      Draw.Rect(this.X, this.Y + 1f, 1f, this.height, this.lineInnerColor);
    }
  }
}
