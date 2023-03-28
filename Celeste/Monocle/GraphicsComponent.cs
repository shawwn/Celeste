// Decompiled with JetBrains decompiler
// Type: Monocle.GraphicsComponent
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monocle
{
  public abstract class GraphicsComponent : Component
  {
    public Vector2 Position;
    public Vector2 Origin;
    public Vector2 Scale = Vector2.One;
    public float Rotation;
    public Color Color = Color.White;
    public SpriteEffects Effects;

    public GraphicsComponent(bool active)
      : base(active, true)
    {
    }

    public float X
    {
      get => this.Position.X;
      set => this.Position.X = value;
    }

    public float Y
    {
      get => this.Position.Y;
      set => this.Position.Y = value;
    }

    public bool FlipX
    {
      get => (this.Effects & SpriteEffects.FlipHorizontally) == SpriteEffects.FlipHorizontally;
      set => this.Effects = value ? this.Effects | SpriteEffects.FlipHorizontally : this.Effects & ~SpriteEffects.FlipHorizontally;
    }

    public bool FlipY
    {
      get => (this.Effects & SpriteEffects.FlipVertically) == SpriteEffects.FlipVertically;
      set => this.Effects = value ? this.Effects | SpriteEffects.FlipVertically : this.Effects & ~SpriteEffects.FlipVertically;
    }

    public Vector2 RenderPosition
    {
      get => (this.Entity == null ? Vector2.Zero : this.Entity.Position) + this.Position;
      set => this.Position = value - (this.Entity == null ? Vector2.Zero : this.Entity.Position);
    }

    public void DrawOutline(int offset = 1) => this.DrawOutline(Color.Black, offset);

    public void DrawOutline(Color color, int offset = 1)
    {
      Vector2 position = this.Position;
      Color color1 = this.Color;
      this.Color = color;
      for (int index1 = -1; index1 < 2; ++index1)
      {
        for (int index2 = -1; index2 < 2; ++index2)
        {
          if (index1 != 0 || index2 != 0)
          {
            this.Position = position + new Vector2((float) (index1 * offset), (float) (index2 * offset));
            this.Render();
          }
        }
      }
      this.Position = position;
      this.Color = color1;
    }

    public void DrawSimpleOutline()
    {
      Vector2 position = this.Position;
      Color color = this.Color;
      this.Color = Color.Black;
      this.Position = position + new Vector2(-1f, 0.0f);
      this.Render();
      this.Position = position + new Vector2(0.0f, -1f);
      this.Render();
      this.Position = position + new Vector2(1f, 0.0f);
      this.Render();
      this.Position = position + new Vector2(0.0f, 1f);
      this.Render();
      this.Position = position;
      this.Color = color;
    }
  }
}
