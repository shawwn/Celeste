// Decompiled with JetBrains decompiler
// Type: Celeste.TempleMirror
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  public class TempleMirror : Entity
  {
    private readonly Color color = Calc.HexToColor("05070e");
    private MTexture[,] frame = new MTexture[3, 3];
    private readonly Vector2 size;
    private MirrorSurface surface;

    public TempleMirror(EntityData e, Vector2 offset)
      : base(Vector2.op_Addition(e.Position, offset))
    {
      this.size = new Vector2((float) e.Width, (float) e.Height);
      this.Depth = 9500;
      this.Collider = (Collider) new Hitbox((float) e.Width, (float) e.Height, 0.0f, 0.0f);
      this.Add((Component) (this.surface = new MirrorSurface((Action) null)));
      this.surface.ReflectionOffset = new Vector2(e.Float("reflectX", 0.0f), e.Float("reflectY", 0.0f));
      this.surface.OnRender = (Action) (() => Draw.Rect(this.X + 2f, this.Y + 2f, (float) (this.size.X - 4.0), (float) (this.size.Y - 4.0), this.surface.ReflectionColor));
      MTexture mtexture = GFX.Game["scenery/templemirror"];
      for (int index1 = 0; index1 < mtexture.Width / 8; ++index1)
      {
        for (int index2 = 0; index2 < mtexture.Height / 8; ++index2)
          this.frame[index1, index2] = mtexture.GetSubtexture(new Rectangle(index1 * 8, index2 * 8, 8, 8));
      }
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      scene.Add((Entity) new TempleMirror.Frame(this));
    }

    public override void Render()
    {
      Draw.Rect(this.X + 3f, this.Y + 3f, (float) (this.size.X - 6.0), (float) (this.size.Y - 6.0), this.color);
    }

    private class Frame : Entity
    {
      private TempleMirror mirror;

      public Frame(TempleMirror mirror)
      {
        this.mirror = mirror;
        this.Depth = 8995;
      }

      public override void Render()
      {
        this.Position = this.mirror.Position;
        MTexture[,] frame = this.mirror.frame;
        Vector2 size = this.mirror.size;
        frame[0, 0].Draw(Vector2.op_Addition(this.Position, new Vector2(0.0f, 0.0f)));
        frame[2, 0].Draw(Vector2.op_Addition(this.Position, new Vector2((float) (size.X - 8.0), 0.0f)));
        frame[0, 2].Draw(Vector2.op_Addition(this.Position, new Vector2(0.0f, (float) (size.Y - 8.0))));
        frame[2, 2].Draw(Vector2.op_Addition(this.Position, new Vector2((float) (size.X - 8.0), (float) (size.Y - 8.0))));
        for (int index = 1; (double) index < size.X / 8.0 - 1.0; ++index)
        {
          frame[1, 0].Draw(Vector2.op_Addition(this.Position, new Vector2((float) (index * 8), 0.0f)));
          frame[1, 2].Draw(Vector2.op_Addition(this.Position, new Vector2((float) (index * 8), (float) (size.Y - 8.0))));
        }
        for (int index = 1; (double) index < size.Y / 8.0 - 1.0; ++index)
        {
          frame[0, 1].Draw(Vector2.op_Addition(this.Position, new Vector2(0.0f, (float) (index * 8))));
          frame[2, 1].Draw(Vector2.op_Addition(this.Position, new Vector2((float) (size.X - 8.0), (float) (index * 8))));
        }
      }
    }
  }
}
