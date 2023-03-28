// Decompiled with JetBrains decompiler
// Type: Monocle.MTexture
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Monocle
{
  public class MTexture
  {
    public string AtlasPath;

    public MTexture()
    {
    }

    public MTexture(VirtualTexture texture)
    {
      this.Texture = texture;
      this.AtlasPath = (string) null;
      this.ClipRect = new Rectangle(0, 0, this.Texture.Width, this.Texture.Height);
      this.DrawOffset = Vector2.Zero;
      this.Width = this.ClipRect.Width;
      this.Height = this.ClipRect.Height;
      this.SetUtil();
    }

    public MTexture(MTexture parent, int x, int y, int width, int height)
    {
      this.Texture = parent.Texture;
      this.AtlasPath = (string) null;
      this.ClipRect = parent.GetRelativeRect(x, y, width, height);
      this.DrawOffset = new Vector2(-Math.Min((float) x - parent.DrawOffset.X, 0.0f), -Math.Min((float) y - parent.DrawOffset.Y, 0.0f));
      this.Width = width;
      this.Height = height;
      this.SetUtil();
    }

    public MTexture(MTexture parent, Rectangle clipRect)
      : this(parent, clipRect.X, clipRect.Y, clipRect.Width, clipRect.Height)
    {
    }

    public MTexture(
      MTexture parent,
      string atlasPath,
      Rectangle clipRect,
      Vector2 drawOffset,
      int width,
      int height)
    {
      this.Texture = parent.Texture;
      this.AtlasPath = atlasPath;
      this.ClipRect = parent.GetRelativeRect(clipRect);
      this.DrawOffset = drawOffset;
      this.Width = width;
      this.Height = height;
      this.SetUtil();
    }

    public MTexture(MTexture parent, string atlasPath, Rectangle clipRect)
      : this(parent, clipRect)
    {
      this.AtlasPath = atlasPath;
    }

    public MTexture(VirtualTexture texture, Vector2 drawOffset, int frameWidth, int frameHeight)
    {
      this.Texture = texture;
      this.ClipRect = new Rectangle(0, 0, texture.Width, texture.Height);
      this.DrawOffset = drawOffset;
      this.Width = frameWidth;
      this.Height = frameHeight;
      this.SetUtil();
    }

    private void SetUtil()
    {
      this.Center = new Vector2((float) this.Width, (float) this.Height) * 0.5f;
      this.LeftUV = (float) this.ClipRect.Left / (float) this.Texture.Width;
      this.RightUV = (float) this.ClipRect.Right / (float) this.Texture.Width;
      this.TopUV = (float) this.ClipRect.Top / (float) this.Texture.Height;
      this.BottomUV = (float) this.ClipRect.Bottom / (float) this.Texture.Height;
    }

    public void Unload()
    {
      this.Texture.Dispose();
      this.Texture = (VirtualTexture) null;
    }

    public MTexture GetSubtexture(int x, int y, int width, int height, MTexture applyTo = null)
    {
      if (applyTo == null)
        return new MTexture(this, x, y, width, height);
      applyTo.Texture = this.Texture;
      applyTo.AtlasPath = (string) null;
      applyTo.ClipRect = this.GetRelativeRect(x, y, width, height);
      applyTo.DrawOffset = new Vector2(-Math.Min((float) x - this.DrawOffset.X, 0.0f), -Math.Min((float) y - this.DrawOffset.Y, 0.0f));
      applyTo.Width = width;
      applyTo.Height = height;
      applyTo.SetUtil();
      return applyTo;
    }

    public MTexture GetSubtexture(Rectangle rect) => new MTexture(this, rect);

    public VirtualTexture Texture { get; private set; }

    public Rectangle ClipRect { get; private set; }

    public Vector2 DrawOffset { get; private set; }

    public int Width { get; private set; }

    public int Height { get; private set; }

    public Vector2 Center { get; private set; }

    public float LeftUV { get; private set; }

    public float RightUV { get; private set; }

    public float TopUV { get; private set; }

    public float BottomUV { get; private set; }

    public override string ToString()
    {
      if (this.AtlasPath != null)
        return this.AtlasPath;
      if (this.Texture.Path != null)
        return this.Texture.Path;
      return "Texture [" + (object) this.Texture.Width + ", " + (object) this.Texture.Height + "]";
    }

    public Rectangle GetRelativeRect(Rectangle rect) => this.GetRelativeRect(rect.X, rect.Y, rect.Width, rect.Height);

    public Rectangle GetRelativeRect(int x, int y, int width, int height)
    {
      int num1 = (int) ((double) this.ClipRect.X - (double) this.DrawOffset.X + (double) x);
      int num2 = (int) ((double) this.ClipRect.Y - (double) this.DrawOffset.Y + (double) y);
      double num3 = (double) num1;
      Rectangle clipRect1 = this.ClipRect;
      double left = (double) clipRect1.Left;
      clipRect1 = this.ClipRect;
      double right = (double) clipRect1.Right;
      int x1 = (int) MathHelper.Clamp((float) num3, (float) left, (float) right);
      double num4 = (double) num2;
      Rectangle clipRect2 = this.ClipRect;
      double top = (double) clipRect2.Top;
      clipRect2 = this.ClipRect;
      double bottom = (double) clipRect2.Bottom;
      int y1 = (int) MathHelper.Clamp((float) num4, (float) top, (float) bottom);
      int width1 = Math.Max(0, Math.Min(num1 + width, this.ClipRect.Right) - x1);
      int height1 = Math.Max(0, Math.Min(num2 + height, this.ClipRect.Bottom) - y1);
      return new Rectangle(x1, y1, width1, height1);
    }

    public int TotalPixels => this.Width * this.Height;

    public void Draw(Vector2 position) => Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), Color.White, 0.0f, -this.DrawOffset, 1f, SpriteEffects.None, 0.0f);

    public void Draw(Vector2 position, Vector2 origin) => Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), Color.White, 0.0f, origin - this.DrawOffset, 1f, SpriteEffects.None, 0.0f);

    public void Draw(Vector2 position, Vector2 origin, Color color) => Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, 0.0f, origin - this.DrawOffset, 1f, SpriteEffects.None, 0.0f);

    public void Draw(Vector2 position, Vector2 origin, Color color, float scale) => Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, 0.0f, origin - this.DrawOffset, scale, SpriteEffects.None, 0.0f);

    public void Draw(Vector2 position, Vector2 origin, Color color, float scale, float rotation) => Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, origin - this.DrawOffset, scale, SpriteEffects.None, 0.0f);

    public void Draw(
      Vector2 position,
      Vector2 origin,
      Color color,
      float scale,
      float rotation,
      SpriteEffects flip)
    {
      Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, origin - this.DrawOffset, scale, flip, 0.0f);
    }

    public void Draw(Vector2 position, Vector2 origin, Color color, Vector2 scale) => Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, 0.0f, origin - this.DrawOffset, scale, SpriteEffects.None, 0.0f);

    public void Draw(
      Vector2 position,
      Vector2 origin,
      Color color,
      Vector2 scale,
      float rotation)
    {
      Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, origin - this.DrawOffset, scale, SpriteEffects.None, 0.0f);
    }

    public void Draw(
      Vector2 position,
      Vector2 origin,
      Color color,
      Vector2 scale,
      float rotation,
      SpriteEffects flip)
    {
      Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, origin - this.DrawOffset, scale, flip, 0.0f);
    }

    public void Draw(
      Vector2 position,
      Vector2 origin,
      Color color,
      Vector2 scale,
      float rotation,
      Rectangle clip)
    {
      Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.GetRelativeRect(clip)), color, rotation, origin - this.DrawOffset, scale, SpriteEffects.None, 0.0f);
    }

    public void DrawCentered(Vector2 position) => Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), Color.White, 0.0f, this.Center - this.DrawOffset, 1f, SpriteEffects.None, 0.0f);

    public void DrawCentered(Vector2 position, Color color) => Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, 0.0f, this.Center - this.DrawOffset, 1f, SpriteEffects.None, 0.0f);

    public void DrawCentered(Vector2 position, Color color, float scale) => Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, 0.0f, this.Center - this.DrawOffset, scale, SpriteEffects.None, 0.0f);

    public void DrawCentered(Vector2 position, Color color, float scale, float rotation) => Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, this.Center - this.DrawOffset, scale, SpriteEffects.None, 0.0f);

    public void DrawCentered(
      Vector2 position,
      Color color,
      float scale,
      float rotation,
      SpriteEffects flip)
    {
      Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, this.Center - this.DrawOffset, scale, flip, 0.0f);
    }

    public void DrawCentered(Vector2 position, Color color, Vector2 scale) => Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, 0.0f, this.Center - this.DrawOffset, scale, SpriteEffects.None, 0.0f);

    public void DrawCentered(Vector2 position, Color color, Vector2 scale, float rotation) => Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, this.Center - this.DrawOffset, scale, SpriteEffects.None, 0.0f);

    public void DrawCentered(
      Vector2 position,
      Color color,
      Vector2 scale,
      float rotation,
      SpriteEffects flip)
    {
      Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, this.Center - this.DrawOffset, scale, flip, 0.0f);
    }

    public void DrawJustified(Vector2 position, Vector2 justify) => Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), Color.White, 0.0f, new Vector2((float) this.Width * justify.X, (float) this.Height * justify.Y) - this.DrawOffset, 1f, SpriteEffects.None, 0.0f);

    public void DrawJustified(Vector2 position, Vector2 justify, Color color) => Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, 0.0f, new Vector2((float) this.Width * justify.X, (float) this.Height * justify.Y) - this.DrawOffset, 1f, SpriteEffects.None, 0.0f);

    public void DrawJustified(Vector2 position, Vector2 justify, Color color, float scale) => Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, 0.0f, new Vector2((float) this.Width * justify.X, (float) this.Height * justify.Y) - this.DrawOffset, scale, SpriteEffects.None, 0.0f);

    public void DrawJustified(
      Vector2 position,
      Vector2 justify,
      Color color,
      float scale,
      float rotation)
    {
      Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, new Vector2((float) this.Width * justify.X, (float) this.Height * justify.Y) - this.DrawOffset, scale, SpriteEffects.None, 0.0f);
    }

    public void DrawJustified(
      Vector2 position,
      Vector2 justify,
      Color color,
      float scale,
      float rotation,
      SpriteEffects flip)
    {
      Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, new Vector2((float) this.Width * justify.X, (float) this.Height * justify.Y) - this.DrawOffset, scale, flip, 0.0f);
    }

    public void DrawJustified(Vector2 position, Vector2 justify, Color color, Vector2 scale) => Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, 0.0f, new Vector2((float) this.Width * justify.X, (float) this.Height * justify.Y) - this.DrawOffset, scale, SpriteEffects.None, 0.0f);

    public void DrawJustified(
      Vector2 position,
      Vector2 justify,
      Color color,
      Vector2 scale,
      float rotation)
    {
      Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, new Vector2((float) this.Width * justify.X, (float) this.Height * justify.Y) - this.DrawOffset, scale, SpriteEffects.None, 0.0f);
    }

    public void DrawJustified(
      Vector2 position,
      Vector2 justify,
      Color color,
      Vector2 scale,
      float rotation,
      SpriteEffects flip)
    {
      Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, new Vector2((float) this.Width * justify.X, (float) this.Height * justify.Y) - this.DrawOffset, scale, flip, 0.0f);
    }

    public void DrawOutline(Vector2 position)
    {
      for (int x = -1; x <= 1; ++x)
      {
        for (int y = -1; y <= 1; ++y)
        {
          if (x != 0 || y != 0)
            Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position + new Vector2((float) x, (float) y), new Rectangle?(this.ClipRect), Color.Black, 0.0f, -this.DrawOffset, 1f, SpriteEffects.None, 0.0f);
        }
      }
      Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), Color.White, 0.0f, -this.DrawOffset, 1f, SpriteEffects.None, 0.0f);
    }

    public void DrawOutline(Vector2 position, Vector2 origin)
    {
      for (int x = -1; x <= 1; ++x)
      {
        for (int y = -1; y <= 1; ++y)
        {
          if (x != 0 || y != 0)
            Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position + new Vector2((float) x, (float) y), new Rectangle?(this.ClipRect), Color.Black, 0.0f, origin - this.DrawOffset, 1f, SpriteEffects.None, 0.0f);
        }
      }
      Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), Color.White, 0.0f, origin - this.DrawOffset, 1f, SpriteEffects.None, 0.0f);
    }

    public void DrawOutline(Vector2 position, Vector2 origin, Color color)
    {
      for (int x = -1; x <= 1; ++x)
      {
        for (int y = -1; y <= 1; ++y)
        {
          if (x != 0 || y != 0)
            Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position + new Vector2((float) x, (float) y), new Rectangle?(this.ClipRect), Color.Black, 0.0f, origin - this.DrawOffset, 1f, SpriteEffects.None, 0.0f);
        }
      }
      Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, 0.0f, origin - this.DrawOffset, 1f, SpriteEffects.None, 0.0f);
    }

    public void DrawOutline(Vector2 position, Vector2 origin, Color color, float scale)
    {
      for (int x = -1; x <= 1; ++x)
      {
        for (int y = -1; y <= 1; ++y)
        {
          if (x != 0 || y != 0)
            Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position + new Vector2((float) x, (float) y), new Rectangle?(this.ClipRect), Color.Black, 0.0f, origin - this.DrawOffset, scale, SpriteEffects.None, 0.0f);
        }
      }
      Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, 0.0f, origin - this.DrawOffset, scale, SpriteEffects.None, 0.0f);
    }

    public void DrawOutline(
      Vector2 position,
      Vector2 origin,
      Color color,
      float scale,
      float rotation)
    {
      for (int x = -1; x <= 1; ++x)
      {
        for (int y = -1; y <= 1; ++y)
        {
          if (x != 0 || y != 0)
            Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position + new Vector2((float) x, (float) y), new Rectangle?(this.ClipRect), Color.Black, rotation, origin - this.DrawOffset, scale, SpriteEffects.None, 0.0f);
        }
      }
      Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, origin - this.DrawOffset, scale, SpriteEffects.None, 0.0f);
    }

    public void DrawOutline(
      Vector2 position,
      Vector2 origin,
      Color color,
      float scale,
      float rotation,
      SpriteEffects flip)
    {
      for (int x = -1; x <= 1; ++x)
      {
        for (int y = -1; y <= 1; ++y)
        {
          if (x != 0 || y != 0)
            Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position + new Vector2((float) x, (float) y), new Rectangle?(this.ClipRect), Color.Black, rotation, origin - this.DrawOffset, scale, flip, 0.0f);
        }
      }
      Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, origin - this.DrawOffset, scale, flip, 0.0f);
    }

    public void DrawOutline(Vector2 position, Vector2 origin, Color color, Vector2 scale)
    {
      for (int x = -1; x <= 1; ++x)
      {
        for (int y = -1; y <= 1; ++y)
        {
          if (x != 0 || y != 0)
            Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position + new Vector2((float) x, (float) y), new Rectangle?(this.ClipRect), Color.Black, 0.0f, origin - this.DrawOffset, scale, SpriteEffects.None, 0.0f);
        }
      }
      Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, 0.0f, origin - this.DrawOffset, scale, SpriteEffects.None, 0.0f);
    }

    public void DrawOutline(
      Vector2 position,
      Vector2 origin,
      Color color,
      Vector2 scale,
      float rotation)
    {
      for (int x = -1; x <= 1; ++x)
      {
        for (int y = -1; y <= 1; ++y)
        {
          if (x != 0 || y != 0)
            Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position + new Vector2((float) x, (float) y), new Rectangle?(this.ClipRect), Color.Black, rotation, origin - this.DrawOffset, scale, SpriteEffects.None, 0.0f);
        }
      }
      Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, origin - this.DrawOffset, scale, SpriteEffects.None, 0.0f);
    }

    public void DrawOutline(
      Vector2 position,
      Vector2 origin,
      Color color,
      Vector2 scale,
      float rotation,
      SpriteEffects flip)
    {
      for (int x = -1; x <= 1; ++x)
      {
        for (int y = -1; y <= 1; ++y)
        {
          if (x != 0 || y != 0)
            Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position + new Vector2((float) x, (float) y), new Rectangle?(this.ClipRect), Color.Black, rotation, origin - this.DrawOffset, scale, flip, 0.0f);
        }
      }
      Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, origin - this.DrawOffset, scale, flip, 0.0f);
    }

    public void DrawOutlineCentered(Vector2 position)
    {
      for (int x = -1; x <= 1; ++x)
      {
        for (int y = -1; y <= 1; ++y)
        {
          if (x != 0 || y != 0)
            Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position + new Vector2((float) x, (float) y), new Rectangle?(this.ClipRect), Color.Black, 0.0f, this.Center - this.DrawOffset, 1f, SpriteEffects.None, 0.0f);
        }
      }
      Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), Color.White, 0.0f, this.Center - this.DrawOffset, 1f, SpriteEffects.None, 0.0f);
    }

    public void DrawOutlineCentered(Vector2 position, Color color)
    {
      for (int x = -1; x <= 1; ++x)
      {
        for (int y = -1; y <= 1; ++y)
        {
          if (x != 0 || y != 0)
            Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position + new Vector2((float) x, (float) y), new Rectangle?(this.ClipRect), Color.Black, 0.0f, this.Center - this.DrawOffset, 1f, SpriteEffects.None, 0.0f);
        }
      }
      Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, 0.0f, this.Center - this.DrawOffset, 1f, SpriteEffects.None, 0.0f);
    }

    public void DrawOutlineCentered(Vector2 position, Color color, float scale)
    {
      for (int x = -1; x <= 1; ++x)
      {
        for (int y = -1; y <= 1; ++y)
        {
          if (x != 0 || y != 0)
            Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position + new Vector2((float) x, (float) y), new Rectangle?(this.ClipRect), Color.Black, 0.0f, this.Center - this.DrawOffset, scale, SpriteEffects.None, 0.0f);
        }
      }
      Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, 0.0f, this.Center - this.DrawOffset, scale, SpriteEffects.None, 0.0f);
    }

    public void DrawOutlineCentered(Vector2 position, Color color, float scale, float rotation)
    {
      for (int x = -1; x <= 1; ++x)
      {
        for (int y = -1; y <= 1; ++y)
        {
          if (x != 0 || y != 0)
            Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position + new Vector2((float) x, (float) y), new Rectangle?(this.ClipRect), Color.Black, rotation, this.Center - this.DrawOffset, scale, SpriteEffects.None, 0.0f);
        }
      }
      Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, this.Center - this.DrawOffset, scale, SpriteEffects.None, 0.0f);
    }

    public void DrawOutlineCentered(
      Vector2 position,
      Color color,
      float scale,
      float rotation,
      SpriteEffects flip)
    {
      for (int x = -1; x <= 1; ++x)
      {
        for (int y = -1; y <= 1; ++y)
        {
          if (x != 0 || y != 0)
            Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position + new Vector2((float) x, (float) y), new Rectangle?(this.ClipRect), Color.Black, rotation, this.Center - this.DrawOffset, scale, flip, 0.0f);
        }
      }
      Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, this.Center - this.DrawOffset, scale, flip, 0.0f);
    }

    public void DrawOutlineCentered(Vector2 position, Color color, Vector2 scale)
    {
      for (int x = -1; x <= 1; ++x)
      {
        for (int y = -1; y <= 1; ++y)
        {
          if (x != 0 || y != 0)
            Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position + new Vector2((float) x, (float) y), new Rectangle?(this.ClipRect), Color.Black, 0.0f, this.Center - this.DrawOffset, scale, SpriteEffects.None, 0.0f);
        }
      }
      Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, 0.0f, this.Center - this.DrawOffset, scale, SpriteEffects.None, 0.0f);
    }

    public void DrawOutlineCentered(Vector2 position, Color color, Vector2 scale, float rotation)
    {
      for (int x = -1; x <= 1; ++x)
      {
        for (int y = -1; y <= 1; ++y)
        {
          if (x != 0 || y != 0)
            Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position + new Vector2((float) x, (float) y), new Rectangle?(this.ClipRect), Color.Black, rotation, this.Center - this.DrawOffset, scale, SpriteEffects.None, 0.0f);
        }
      }
      Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, this.Center - this.DrawOffset, scale, SpriteEffects.None, 0.0f);
    }

    public void DrawOutlineCentered(
      Vector2 position,
      Color color,
      Vector2 scale,
      float rotation,
      SpriteEffects flip)
    {
      for (int x = -1; x <= 1; ++x)
      {
        for (int y = -1; y <= 1; ++y)
        {
          if (x != 0 || y != 0)
            Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position + new Vector2((float) x, (float) y), new Rectangle?(this.ClipRect), Color.Black, rotation, this.Center - this.DrawOffset, scale, flip, 0.0f);
        }
      }
      Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, this.Center - this.DrawOffset, scale, flip, 0.0f);
    }

    public void DrawOutlineJustified(Vector2 position, Vector2 justify)
    {
      for (int x = -1; x <= 1; ++x)
      {
        for (int y = -1; y <= 1; ++y)
        {
          if (x != 0 || y != 0)
            Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position + new Vector2((float) x, (float) y), new Rectangle?(this.ClipRect), Color.Black, 0.0f, new Vector2((float) this.Width * justify.X, (float) this.Height * justify.Y) - this.DrawOffset, 1f, SpriteEffects.None, 0.0f);
        }
      }
      Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), Color.White, 0.0f, new Vector2((float) this.Width * justify.X, (float) this.Height * justify.Y) - this.DrawOffset, 1f, SpriteEffects.None, 0.0f);
    }

    public void DrawOutlineJustified(Vector2 position, Vector2 justify, Color color)
    {
      for (int x = -1; x <= 1; ++x)
      {
        for (int y = -1; y <= 1; ++y)
        {
          if (x != 0 || y != 0)
            Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position + new Vector2((float) x, (float) y), new Rectangle?(this.ClipRect), Color.Black, 0.0f, new Vector2((float) this.Width * justify.X, (float) this.Height * justify.Y) - this.DrawOffset, 1f, SpriteEffects.None, 0.0f);
        }
      }
      Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, 0.0f, new Vector2((float) this.Width * justify.X, (float) this.Height * justify.Y) - this.DrawOffset, 1f, SpriteEffects.None, 0.0f);
    }

    public void DrawOutlineJustified(Vector2 position, Vector2 justify, Color color, float scale)
    {
      for (int x = -1; x <= 1; ++x)
      {
        for (int y = -1; y <= 1; ++y)
        {
          if (x != 0 || y != 0)
            Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position + new Vector2((float) x, (float) y), new Rectangle?(this.ClipRect), Color.Black, 0.0f, new Vector2((float) this.Width * justify.X, (float) this.Height * justify.Y) - this.DrawOffset, scale, SpriteEffects.None, 0.0f);
        }
      }
      Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, 0.0f, new Vector2((float) this.Width * justify.X, (float) this.Height * justify.Y) - this.DrawOffset, scale, SpriteEffects.None, 0.0f);
    }

    public void DrawOutlineJustified(
      Vector2 position,
      Vector2 justify,
      Color color,
      float scale,
      float rotation)
    {
      for (int x = -1; x <= 1; ++x)
      {
        for (int y = -1; y <= 1; ++y)
        {
          if (x != 0 || y != 0)
            Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position + new Vector2((float) x, (float) y), new Rectangle?(this.ClipRect), Color.Black, rotation, new Vector2((float) this.Width * justify.X, (float) this.Height * justify.Y) - this.DrawOffset, scale, SpriteEffects.None, 0.0f);
        }
      }
      Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, new Vector2((float) this.Width * justify.X, (float) this.Height * justify.Y) - this.DrawOffset, scale, SpriteEffects.None, 0.0f);
    }

    public void DrawOutlineJustified(
      Vector2 position,
      Vector2 justify,
      Color color,
      float scale,
      float rotation,
      SpriteEffects flip)
    {
      for (int x = -1; x <= 1; ++x)
      {
        for (int y = -1; y <= 1; ++y)
        {
          if (x != 0 || y != 0)
            Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position + new Vector2((float) x, (float) y), new Rectangle?(this.ClipRect), Color.Black, rotation, new Vector2((float) this.Width * justify.X, (float) this.Height * justify.Y) - this.DrawOffset, scale, flip, 0.0f);
        }
      }
      Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, new Vector2((float) this.Width * justify.X, (float) this.Height * justify.Y) - this.DrawOffset, scale, flip, 0.0f);
    }

    public void DrawOutlineJustified(
      Vector2 position,
      Vector2 justify,
      Color color,
      Vector2 scale)
    {
      for (int x = -1; x <= 1; ++x)
      {
        for (int y = -1; y <= 1; ++y)
        {
          if (x != 0 || y != 0)
            Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position + new Vector2((float) x, (float) y), new Rectangle?(this.ClipRect), Color.Black, 0.0f, new Vector2((float) this.Width * justify.X, (float) this.Height * justify.Y) - this.DrawOffset, scale, SpriteEffects.None, 0.0f);
        }
      }
      Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, 0.0f, new Vector2((float) this.Width * justify.X, (float) this.Height * justify.Y) - this.DrawOffset, scale, SpriteEffects.None, 0.0f);
    }

    public void DrawOutlineJustified(
      Vector2 position,
      Vector2 justify,
      Color color,
      Vector2 scale,
      float rotation)
    {
      for (int x = -1; x <= 1; ++x)
      {
        for (int y = -1; y <= 1; ++y)
        {
          if (x != 0 || y != 0)
            Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position + new Vector2((float) x, (float) y), new Rectangle?(this.ClipRect), Color.Black, rotation, new Vector2((float) this.Width * justify.X, (float) this.Height * justify.Y) - this.DrawOffset, scale, SpriteEffects.None, 0.0f);
        }
      }
      Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, new Vector2((float) this.Width * justify.X, (float) this.Height * justify.Y) - this.DrawOffset, scale, SpriteEffects.None, 0.0f);
    }

    public void DrawOutlineJustified(
      Vector2 position,
      Vector2 justify,
      Color color,
      Vector2 scale,
      float rotation,
      SpriteEffects flip)
    {
      for (int x = -1; x <= 1; ++x)
      {
        for (int y = -1; y <= 1; ++y)
        {
          if (x != 0 || y != 0)
            Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position + new Vector2((float) x, (float) y), new Rectangle?(this.ClipRect), Color.Black, rotation, new Vector2((float) this.Width * justify.X, (float) this.Height * justify.Y) - this.DrawOffset, scale, flip, 0.0f);
        }
      }
      Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, new Vector2((float) this.Width * justify.X, (float) this.Height * justify.Y) - this.DrawOffset, scale, flip, 0.0f);
    }
  }
}
