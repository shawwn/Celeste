// Decompiled with JetBrains decompiler
// Type: Celeste.GlassBlock
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste
{
  [Tracked(false)]
  public class GlassBlock : Solid
  {
    private List<GlassBlock.Line> lines = new List<GlassBlock.Line>();
    private Color lineColor = Color.get_White();
    private bool sinks;
    private float startY;

    public GlassBlock(Vector2 position, float width, float height, bool sinks)
      : base(position, width, height, false)
    {
      this.sinks = sinks;
      this.startY = this.Y;
      this.Depth = -10000;
      this.Add((Component) new LightOcclude(1f));
      this.Add((Component) new MirrorSurface((Action) null));
      this.SurfaceSoundIndex = 32;
    }

    public GlassBlock(EntityData data, Vector2 offset)
      : this(Vector2.op_Addition(data.Position, offset), (float) data.Width, (float) data.Height, data.Bool(nameof (sinks), false))
    {
    }

    public override void Awake(Scene scene)
    {
      base.Awake(scene);
      int tiles1 = (int) this.Width / 8;
      int tiles2 = (int) this.Height / 8;
      this.AddSide(new Vector2(0.0f, 0.0f), new Vector2(0.0f, -1f), tiles1);
      this.AddSide(new Vector2((float) (tiles1 - 1), 0.0f), new Vector2(1f, 0.0f), tiles2);
      this.AddSide(new Vector2((float) (tiles1 - 1), (float) (tiles2 - 1)), new Vector2(0.0f, 1f), tiles1);
      this.AddSide(new Vector2(0.0f, (float) (tiles2 - 1)), new Vector2(-1f, 0.0f), tiles2);
    }

    private float Mod(float x, float m)
    {
      return (x % m + m) % m;
    }

    private void AddSide(Vector2 start, Vector2 normal, int tiles)
    {
      Vector2 vector2;
      ((Vector2) ref vector2).\u002Ector((float) -normal.Y, (float) normal.X);
      for (int index = 0; index < tiles; ++index)
      {
        if (this.Open(Vector2.op_Addition(Vector2.op_Addition(start, Vector2.op_Multiply(vector2, (float) index)), normal)))
        {
          Vector2 a = Vector2.op_Addition(Vector2.op_Subtraction(Vector2.op_Addition(Vector2.op_Multiply(Vector2.op_Addition(start, Vector2.op_Multiply(vector2, (float) index)), 8f), new Vector2(4f)), Vector2.op_Multiply(vector2, 4f)), Vector2.op_Multiply(normal, 4f));
          if (!this.Open(Vector2.op_Addition(start, Vector2.op_Multiply(vector2, (float) (index - 1)))))
            a = Vector2.op_Subtraction(a, vector2);
          while (index < tiles && this.Open(Vector2.op_Addition(Vector2.op_Addition(start, Vector2.op_Multiply(vector2, (float) index)), normal)))
            ++index;
          Vector2 b = Vector2.op_Addition(Vector2.op_Subtraction(Vector2.op_Addition(Vector2.op_Multiply(Vector2.op_Addition(start, Vector2.op_Multiply(vector2, (float) index)), 8f), new Vector2(4f)), Vector2.op_Multiply(vector2, 4f)), Vector2.op_Multiply(normal, 4f));
          if (!this.Open(Vector2.op_Addition(start, Vector2.op_Multiply(vector2, (float) index))))
            b = Vector2.op_Addition(b, vector2);
          this.lines.Add(new GlassBlock.Line(a, b));
        }
      }
    }

    private bool Open(Vector2 tile)
    {
      Vector2 point;
      ((Vector2) ref point).\u002Ector((float) ((double) this.X + tile.X * 8.0 + 4.0), (float) ((double) this.Y + tile.Y * 8.0 + 4.0));
      if (!this.Scene.CollideCheck<SolidTiles>(point))
        return !this.Scene.CollideCheck<GlassBlock>(point);
      return false;
    }

    public override void Render()
    {
      foreach (GlassBlock.Line line in this.lines)
        Draw.Line(Vector2.op_Addition(this.Position, line.A), Vector2.op_Addition(this.Position, line.B), this.lineColor);
    }

    private struct Line
    {
      public Vector2 A;
      public Vector2 B;

      public Line(Vector2 a, Vector2 b)
      {
        this.A = a;
        this.B = b;
      }
    }
  }
}
