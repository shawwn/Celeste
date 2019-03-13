// Decompiled with JetBrains decompiler
// Type: Monocle.MTexture
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Monocle
{
  public class MTexture
  {
    public MTexture()
    {
    }

    public MTexture(VirtualTexture texture)
    {
      this.Texture = texture;
      this.AtlasPath = (string) null;
      this.ClipRect = new Rectangle(0, 0, this.Texture.Width, this.Texture.Height);
      this.DrawOffset = Vector2.get_Zero();
      this.Width = (int) this.ClipRect.Width;
      this.Height = (int) this.ClipRect.Height;
      this.SetUtil();
    }

    public MTexture(MTexture parent, int x, int y, int width, int height)
    {
      this.Texture = parent.Texture;
      this.AtlasPath = (string) null;
      this.ClipRect = parent.GetRelativeRect(x, y, width, height);
      this.DrawOffset = new Vector2(-Math.Min((float) x - (float) parent.DrawOffset.X, 0.0f), -Math.Min((float) y - (float) parent.DrawOffset.Y, 0.0f));
      this.Width = width;
      this.Height = height;
      this.SetUtil();
    }

    public MTexture(MTexture parent, Rectangle clipRect)
      : this(parent, (int) clipRect.X, (int) clipRect.Y, (int) clipRect.Width, (int) clipRect.Height)
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
      this.Center = Vector2.op_Multiply(new Vector2((float) this.Width, (float) this.Height), 0.5f);
      Rectangle clipRect1 = this.ClipRect;
      this.LeftUV = (float) ((Rectangle) ref clipRect1).get_Left() / (float) this.Texture.Width;
      Rectangle clipRect2 = this.ClipRect;
      this.RightUV = (float) ((Rectangle) ref clipRect2).get_Right() / (float) this.Texture.Width;
      Rectangle clipRect3 = this.ClipRect;
      this.TopUV = (float) ((Rectangle) ref clipRect3).get_Top() / (float) this.Texture.Height;
      Rectangle clipRect4 = this.ClipRect;
      this.BottomUV = (float) ((Rectangle) ref clipRect4).get_Bottom() / (float) this.Texture.Height;
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
      applyTo.DrawOffset = new Vector2(-Math.Min((float) x - (float) this.DrawOffset.X, 0.0f), -Math.Min((float) y - (float) this.DrawOffset.Y, 0.0f));
      applyTo.Width = width;
      applyTo.Height = height;
      applyTo.SetUtil();
      return applyTo;
    }

    public MTexture GetSubtexture(Rectangle rect)
    {
      return new MTexture(this, rect);
    }

    public VirtualTexture Texture { get; private set; }

    public Rectangle ClipRect { get; private set; }

    public string AtlasPath { get; private set; }

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

    public Rectangle GetRelativeRect(Rectangle rect)
    {
      return this.GetRelativeRect((int) rect.X, (int) rect.Y, (int) rect.Width, (int) rect.Height);
    }

    public Rectangle GetRelativeRect(int x, int y, int width, int height)
    {
      int num1 = (int) ((double) (float) this.ClipRect.X - this.DrawOffset.X + (double) x);
      int num2 = (int) ((double) (float) this.ClipRect.Y - this.DrawOffset.Y + (double) y);
      double num3 = (double) num1;
      Rectangle clipRect1 = this.ClipRect;
      double left = (double) ((Rectangle) ref clipRect1).get_Left();
      clipRect1 = this.ClipRect;
      double right1 = (double) ((Rectangle) ref clipRect1).get_Right();
      int num4 = (int) MathHelper.Clamp((float) num3, (float) left, (float) right1);
      double num5 = (double) num2;
      Rectangle clipRect2 = this.ClipRect;
      double top = (double) ((Rectangle) ref clipRect2).get_Top();
      clipRect2 = this.ClipRect;
      double bottom1 = (double) ((Rectangle) ref clipRect2).get_Bottom();
      int num6 = (int) MathHelper.Clamp((float) num5, (float) top, (float) bottom1);
      int val1_1 = num1 + width;
      Rectangle clipRect3 = this.ClipRect;
      int right2 = ((Rectangle) ref clipRect3).get_Right();
      int num7 = Math.Max(0, Math.Min(val1_1, right2) - num4);
      int val1_2 = num2 + height;
      Rectangle clipRect4 = this.ClipRect;
      int bottom2 = ((Rectangle) ref clipRect4).get_Bottom();
      int num8 = Math.Max(0, Math.Min(val1_2, bottom2) - num6);
      return new Rectangle(num4, num6, num7, num8);
    }

    public int TotalPixels
    {
      get
      {
        return this.Width * this.Height;
      }
    }

    public void Draw(Vector2 position)
    {
      Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), Color.get_White(), 0.0f, Vector2.op_UnaryNegation(this.DrawOffset), 1f, (SpriteEffects) 0, 0.0f);
    }

    public void Draw(Vector2 position, Vector2 origin)
    {
      Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), Color.get_White(), 0.0f, Vector2.op_Subtraction(origin, this.DrawOffset), 1f, (SpriteEffects) 0, 0.0f);
    }

    public void Draw(Vector2 position, Vector2 origin, Color color)
    {
      Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, 0.0f, Vector2.op_Subtraction(origin, this.DrawOffset), 1f, (SpriteEffects) 0, 0.0f);
    }

    public void Draw(Vector2 position, Vector2 origin, Color color, float scale)
    {
      Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, 0.0f, Vector2.op_Subtraction(origin, this.DrawOffset), scale, (SpriteEffects) 0, 0.0f);
    }

    public void Draw(Vector2 position, Vector2 origin, Color color, float scale, float rotation)
    {
      Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, Vector2.op_Subtraction(origin, this.DrawOffset), scale, (SpriteEffects) 0, 0.0f);
    }

    public void Draw(
      Vector2 position,
      Vector2 origin,
      Color color,
      float scale,
      float rotation,
      SpriteEffects flip)
    {
      Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, Vector2.op_Subtraction(origin, this.DrawOffset), scale, flip, 0.0f);
    }

    public void Draw(Vector2 position, Vector2 origin, Color color, Vector2 scale)
    {
      Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, 0.0f, Vector2.op_Subtraction(origin, this.DrawOffset), scale, (SpriteEffects) 0, 0.0f);
    }

    public void Draw(
      Vector2 position,
      Vector2 origin,
      Color color,
      Vector2 scale,
      float rotation)
    {
      Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, Vector2.op_Subtraction(origin, this.DrawOffset), scale, (SpriteEffects) 0, 0.0f);
    }

    public void Draw(
      Vector2 position,
      Vector2 origin,
      Color color,
      Vector2 scale,
      float rotation,
      SpriteEffects flip)
    {
      Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, Vector2.op_Subtraction(origin, this.DrawOffset), scale, flip, 0.0f);
    }

    public void Draw(
      Vector2 position,
      Vector2 origin,
      Color color,
      Vector2 scale,
      float rotation,
      Rectangle clip)
    {
      Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.GetRelativeRect(clip)), color, rotation, Vector2.op_Subtraction(origin, this.DrawOffset), scale, (SpriteEffects) 0, 0.0f);
    }

    public void DrawCentered(Vector2 position)
    {
      Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), Color.get_White(), 0.0f, Vector2.op_Subtraction(this.Center, this.DrawOffset), 1f, (SpriteEffects) 0, 0.0f);
    }

    public void DrawCentered(Vector2 position, Color color)
    {
      Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, 0.0f, Vector2.op_Subtraction(this.Center, this.DrawOffset), 1f, (SpriteEffects) 0, 0.0f);
    }

    public void DrawCentered(Vector2 position, Color color, float scale)
    {
      Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, 0.0f, Vector2.op_Subtraction(this.Center, this.DrawOffset), scale, (SpriteEffects) 0, 0.0f);
    }

    public void DrawCentered(Vector2 position, Color color, float scale, float rotation)
    {
      Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, Vector2.op_Subtraction(this.Center, this.DrawOffset), scale, (SpriteEffects) 0, 0.0f);
    }

    public void DrawCentered(
      Vector2 position,
      Color color,
      float scale,
      float rotation,
      SpriteEffects flip)
    {
      Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, Vector2.op_Subtraction(this.Center, this.DrawOffset), scale, flip, 0.0f);
    }

    public void DrawCentered(Vector2 position, Color color, Vector2 scale)
    {
      Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, 0.0f, Vector2.op_Subtraction(this.Center, this.DrawOffset), scale, (SpriteEffects) 0, 0.0f);
    }

    public void DrawCentered(Vector2 position, Color color, Vector2 scale, float rotation)
    {
      Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, Vector2.op_Subtraction(this.Center, this.DrawOffset), scale, (SpriteEffects) 0, 0.0f);
    }

    public void DrawCentered(
      Vector2 position,
      Color color,
      Vector2 scale,
      float rotation,
      SpriteEffects flip)
    {
      Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, Vector2.op_Subtraction(this.Center, this.DrawOffset), scale, flip, 0.0f);
    }

    public void DrawJustified(Vector2 position, Vector2 justify)
    {
      Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), Color.get_White(), 0.0f, Vector2.op_Subtraction(new Vector2((float) this.Width * (float) justify.X, (float) this.Height * (float) justify.Y), this.DrawOffset), 1f, (SpriteEffects) 0, 0.0f);
    }

    public void DrawJustified(Vector2 position, Vector2 justify, Color color)
    {
      Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, 0.0f, Vector2.op_Subtraction(new Vector2((float) this.Width * (float) justify.X, (float) this.Height * (float) justify.Y), this.DrawOffset), 1f, (SpriteEffects) 0, 0.0f);
    }

    public void DrawJustified(Vector2 position, Vector2 justify, Color color, float scale)
    {
      Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, 0.0f, Vector2.op_Subtraction(new Vector2((float) this.Width * (float) justify.X, (float) this.Height * (float) justify.Y), this.DrawOffset), scale, (SpriteEffects) 0, 0.0f);
    }

    public void DrawJustified(
      Vector2 position,
      Vector2 justify,
      Color color,
      float scale,
      float rotation)
    {
      Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, Vector2.op_Subtraction(new Vector2((float) this.Width * (float) justify.X, (float) this.Height * (float) justify.Y), this.DrawOffset), scale, (SpriteEffects) 0, 0.0f);
    }

    public void DrawJustified(
      Vector2 position,
      Vector2 justify,
      Color color,
      float scale,
      float rotation,
      SpriteEffects flip)
    {
      Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, Vector2.op_Subtraction(new Vector2((float) this.Width * (float) justify.X, (float) this.Height * (float) justify.Y), this.DrawOffset), scale, flip, 0.0f);
    }

    public void DrawJustified(Vector2 position, Vector2 justify, Color color, Vector2 scale)
    {
      Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, 0.0f, Vector2.op_Subtraction(new Vector2((float) this.Width * (float) justify.X, (float) this.Height * (float) justify.Y), this.DrawOffset), scale, (SpriteEffects) 0, 0.0f);
    }

    public void DrawJustified(
      Vector2 position,
      Vector2 justify,
      Color color,
      Vector2 scale,
      float rotation)
    {
      Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, Vector2.op_Subtraction(new Vector2((float) this.Width * (float) justify.X, (float) this.Height * (float) justify.Y), this.DrawOffset), scale, (SpriteEffects) 0, 0.0f);
    }

    public void DrawJustified(
      Vector2 position,
      Vector2 justify,
      Color color,
      Vector2 scale,
      float rotation,
      SpriteEffects flip)
    {
      Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, Vector2.op_Subtraction(new Vector2((float) this.Width * (float) justify.X, (float) this.Height * (float) justify.Y), this.DrawOffset), scale, flip, 0.0f);
    }

    public void DrawOutline(Vector2 position)
    {
      for (int index1 = -1; index1 <= 1; ++index1)
      {
        for (int index2 = -1; index2 <= 1; ++index2)
        {
          if (index1 != 0 || index2 != 0)
            Draw.SpriteBatch.Draw(this.Texture.Texture, Vector2.op_Addition(position, new Vector2((float) index1, (float) index2)), new Rectangle?(this.ClipRect), Color.get_Black(), 0.0f, Vector2.op_UnaryNegation(this.DrawOffset), 1f, (SpriteEffects) 0, 0.0f);
        }
      }
      Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), Color.get_White(), 0.0f, Vector2.op_UnaryNegation(this.DrawOffset), 1f, (SpriteEffects) 0, 0.0f);
    }

    public void DrawOutline(Vector2 position, Vector2 origin)
    {
      for (int index1 = -1; index1 <= 1; ++index1)
      {
        for (int index2 = -1; index2 <= 1; ++index2)
        {
          if (index1 != 0 || index2 != 0)
            Draw.SpriteBatch.Draw(this.Texture.Texture, Vector2.op_Addition(position, new Vector2((float) index1, (float) index2)), new Rectangle?(this.ClipRect), Color.get_Black(), 0.0f, Vector2.op_Subtraction(origin, this.DrawOffset), 1f, (SpriteEffects) 0, 0.0f);
        }
      }
      Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), Color.get_White(), 0.0f, Vector2.op_Subtraction(origin, this.DrawOffset), 1f, (SpriteEffects) 0, 0.0f);
    }

    public void DrawOutline(Vector2 position, Vector2 origin, Color color)
    {
      for (int index1 = -1; index1 <= 1; ++index1)
      {
        for (int index2 = -1; index2 <= 1; ++index2)
        {
          if (index1 != 0 || index2 != 0)
            Draw.SpriteBatch.Draw(this.Texture.Texture, Vector2.op_Addition(position, new Vector2((float) index1, (float) index2)), new Rectangle?(this.ClipRect), Color.get_Black(), 0.0f, Vector2.op_Subtraction(origin, this.DrawOffset), 1f, (SpriteEffects) 0, 0.0f);
        }
      }
      Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, 0.0f, Vector2.op_Subtraction(origin, this.DrawOffset), 1f, (SpriteEffects) 0, 0.0f);
    }

    public void DrawOutline(Vector2 position, Vector2 origin, Color color, float scale)
    {
      for (int index1 = -1; index1 <= 1; ++index1)
      {
        for (int index2 = -1; index2 <= 1; ++index2)
        {
          if (index1 != 0 || index2 != 0)
            Draw.SpriteBatch.Draw(this.Texture.Texture, Vector2.op_Addition(position, new Vector2((float) index1, (float) index2)), new Rectangle?(this.ClipRect), Color.get_Black(), 0.0f, Vector2.op_Subtraction(origin, this.DrawOffset), scale, (SpriteEffects) 0, 0.0f);
        }
      }
      Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, 0.0f, Vector2.op_Subtraction(origin, this.DrawOffset), scale, (SpriteEffects) 0, 0.0f);
    }

    public void DrawOutline(
      Vector2 position,
      Vector2 origin,
      Color color,
      float scale,
      float rotation)
    {
      for (int index1 = -1; index1 <= 1; ++index1)
      {
        for (int index2 = -1; index2 <= 1; ++index2)
        {
          if (index1 != 0 || index2 != 0)
            Draw.SpriteBatch.Draw(this.Texture.Texture, Vector2.op_Addition(position, new Vector2((float) index1, (float) index2)), new Rectangle?(this.ClipRect), Color.get_Black(), rotation, Vector2.op_Subtraction(origin, this.DrawOffset), scale, (SpriteEffects) 0, 0.0f);
        }
      }
      Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, Vector2.op_Subtraction(origin, this.DrawOffset), scale, (SpriteEffects) 0, 0.0f);
    }

    public void DrawOutline(
      Vector2 position,
      Vector2 origin,
      Color color,
      float scale,
      float rotation,
      SpriteEffects flip)
    {
      for (int index1 = -1; index1 <= 1; ++index1)
      {
        for (int index2 = -1; index2 <= 1; ++index2)
        {
          if (index1 != 0 || index2 != 0)
            Draw.SpriteBatch.Draw(this.Texture.Texture, Vector2.op_Addition(position, new Vector2((float) index1, (float) index2)), new Rectangle?(this.ClipRect), Color.get_Black(), rotation, Vector2.op_Subtraction(origin, this.DrawOffset), scale, flip, 0.0f);
        }
      }
      Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, Vector2.op_Subtraction(origin, this.DrawOffset), scale, flip, 0.0f);
    }

    public void DrawOutline(Vector2 position, Vector2 origin, Color color, Vector2 scale)
    {
      for (int index1 = -1; index1 <= 1; ++index1)
      {
        for (int index2 = -1; index2 <= 1; ++index2)
        {
          if (index1 != 0 || index2 != 0)
            Draw.SpriteBatch.Draw(this.Texture.Texture, Vector2.op_Addition(position, new Vector2((float) index1, (float) index2)), new Rectangle?(this.ClipRect), Color.get_Black(), 0.0f, Vector2.op_Subtraction(origin, this.DrawOffset), scale, (SpriteEffects) 0, 0.0f);
        }
      }
      Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, 0.0f, Vector2.op_Subtraction(origin, this.DrawOffset), scale, (SpriteEffects) 0, 0.0f);
    }

    public void DrawOutline(
      Vector2 position,
      Vector2 origin,
      Color color,
      Vector2 scale,
      float rotation)
    {
      for (int index1 = -1; index1 <= 1; ++index1)
      {
        for (int index2 = -1; index2 <= 1; ++index2)
        {
          if (index1 != 0 || index2 != 0)
            Draw.SpriteBatch.Draw(this.Texture.Texture, Vector2.op_Addition(position, new Vector2((float) index1, (float) index2)), new Rectangle?(this.ClipRect), Color.get_Black(), rotation, Vector2.op_Subtraction(origin, this.DrawOffset), scale, (SpriteEffects) 0, 0.0f);
        }
      }
      Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, Vector2.op_Subtraction(origin, this.DrawOffset), scale, (SpriteEffects) 0, 0.0f);
    }

    public void DrawOutline(
      Vector2 position,
      Vector2 origin,
      Color color,
      Vector2 scale,
      float rotation,
      SpriteEffects flip)
    {
      for (int index1 = -1; index1 <= 1; ++index1)
      {
        for (int index2 = -1; index2 <= 1; ++index2)
        {
          if (index1 != 0 || index2 != 0)
            Draw.SpriteBatch.Draw(this.Texture.Texture, Vector2.op_Addition(position, new Vector2((float) index1, (float) index2)), new Rectangle?(this.ClipRect), Color.get_Black(), rotation, Vector2.op_Subtraction(origin, this.DrawOffset), scale, flip, 0.0f);
        }
      }
      Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, Vector2.op_Subtraction(origin, this.DrawOffset), scale, flip, 0.0f);
    }

    public void DrawOutlineCentered(Vector2 position)
    {
      for (int index1 = -1; index1 <= 1; ++index1)
      {
        for (int index2 = -1; index2 <= 1; ++index2)
        {
          if (index1 != 0 || index2 != 0)
            Draw.SpriteBatch.Draw(this.Texture.Texture, Vector2.op_Addition(position, new Vector2((float) index1, (float) index2)), new Rectangle?(this.ClipRect), Color.get_Black(), 0.0f, Vector2.op_Subtraction(this.Center, this.DrawOffset), 1f, (SpriteEffects) 0, 0.0f);
        }
      }
      Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), Color.get_White(), 0.0f, Vector2.op_Subtraction(this.Center, this.DrawOffset), 1f, (SpriteEffects) 0, 0.0f);
    }

    public void DrawOutlineCentered(Vector2 position, Color color)
    {
      for (int index1 = -1; index1 <= 1; ++index1)
      {
        for (int index2 = -1; index2 <= 1; ++index2)
        {
          if (index1 != 0 || index2 != 0)
            Draw.SpriteBatch.Draw(this.Texture.Texture, Vector2.op_Addition(position, new Vector2((float) index1, (float) index2)), new Rectangle?(this.ClipRect), Color.get_Black(), 0.0f, Vector2.op_Subtraction(this.Center, this.DrawOffset), 1f, (SpriteEffects) 0, 0.0f);
        }
      }
      Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, 0.0f, Vector2.op_Subtraction(this.Center, this.DrawOffset), 1f, (SpriteEffects) 0, 0.0f);
    }

    public void DrawOutlineCentered(Vector2 position, Color color, float scale)
    {
      for (int index1 = -1; index1 <= 1; ++index1)
      {
        for (int index2 = -1; index2 <= 1; ++index2)
        {
          if (index1 != 0 || index2 != 0)
            Draw.SpriteBatch.Draw(this.Texture.Texture, Vector2.op_Addition(position, new Vector2((float) index1, (float) index2)), new Rectangle?(this.ClipRect), Color.get_Black(), 0.0f, Vector2.op_Subtraction(this.Center, this.DrawOffset), scale, (SpriteEffects) 0, 0.0f);
        }
      }
      Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, 0.0f, Vector2.op_Subtraction(this.Center, this.DrawOffset), scale, (SpriteEffects) 0, 0.0f);
    }

    public void DrawOutlineCentered(Vector2 position, Color color, float scale, float rotation)
    {
      for (int index1 = -1; index1 <= 1; ++index1)
      {
        for (int index2 = -1; index2 <= 1; ++index2)
        {
          if (index1 != 0 || index2 != 0)
            Draw.SpriteBatch.Draw(this.Texture.Texture, Vector2.op_Addition(position, new Vector2((float) index1, (float) index2)), new Rectangle?(this.ClipRect), Color.get_Black(), rotation, Vector2.op_Subtraction(this.Center, this.DrawOffset), scale, (SpriteEffects) 0, 0.0f);
        }
      }
      Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, Vector2.op_Subtraction(this.Center, this.DrawOffset), scale, (SpriteEffects) 0, 0.0f);
    }

    public void DrawOutlineCentered(
      Vector2 position,
      Color color,
      float scale,
      float rotation,
      SpriteEffects flip)
    {
      for (int index1 = -1; index1 <= 1; ++index1)
      {
        for (int index2 = -1; index2 <= 1; ++index2)
        {
          if (index1 != 0 || index2 != 0)
            Draw.SpriteBatch.Draw(this.Texture.Texture, Vector2.op_Addition(position, new Vector2((float) index1, (float) index2)), new Rectangle?(this.ClipRect), Color.get_Black(), rotation, Vector2.op_Subtraction(this.Center, this.DrawOffset), scale, flip, 0.0f);
        }
      }
      Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, Vector2.op_Subtraction(this.Center, this.DrawOffset), scale, flip, 0.0f);
    }

    public void DrawOutlineCentered(Vector2 position, Color color, Vector2 scale)
    {
      for (int index1 = -1; index1 <= 1; ++index1)
      {
        for (int index2 = -1; index2 <= 1; ++index2)
        {
          if (index1 != 0 || index2 != 0)
            Draw.SpriteBatch.Draw(this.Texture.Texture, Vector2.op_Addition(position, new Vector2((float) index1, (float) index2)), new Rectangle?(this.ClipRect), Color.get_Black(), 0.0f, Vector2.op_Subtraction(this.Center, this.DrawOffset), scale, (SpriteEffects) 0, 0.0f);
        }
      }
      Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, 0.0f, Vector2.op_Subtraction(this.Center, this.DrawOffset), scale, (SpriteEffects) 0, 0.0f);
    }

    public void DrawOutlineCentered(Vector2 position, Color color, Vector2 scale, float rotation)
    {
      for (int index1 = -1; index1 <= 1; ++index1)
      {
        for (int index2 = -1; index2 <= 1; ++index2)
        {
          if (index1 != 0 || index2 != 0)
            Draw.SpriteBatch.Draw(this.Texture.Texture, Vector2.op_Addition(position, new Vector2((float) index1, (float) index2)), new Rectangle?(this.ClipRect), Color.get_Black(), rotation, Vector2.op_Subtraction(this.Center, this.DrawOffset), scale, (SpriteEffects) 0, 0.0f);
        }
      }
      Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, Vector2.op_Subtraction(this.Center, this.DrawOffset), scale, (SpriteEffects) 0, 0.0f);
    }

    public void DrawOutlineCentered(
      Vector2 position,
      Color color,
      Vector2 scale,
      float rotation,
      SpriteEffects flip)
    {
      for (int index1 = -1; index1 <= 1; ++index1)
      {
        for (int index2 = -1; index2 <= 1; ++index2)
        {
          if (index1 != 0 || index2 != 0)
            Draw.SpriteBatch.Draw(this.Texture.Texture, Vector2.op_Addition(position, new Vector2((float) index1, (float) index2)), new Rectangle?(this.ClipRect), Color.get_Black(), rotation, Vector2.op_Subtraction(this.Center, this.DrawOffset), scale, flip, 0.0f);
        }
      }
      Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, Vector2.op_Subtraction(this.Center, this.DrawOffset), scale, flip, 0.0f);
    }

    public void DrawOutlineJustified(Vector2 position, Vector2 justify)
    {
      for (int index1 = -1; index1 <= 1; ++index1)
      {
        for (int index2 = -1; index2 <= 1; ++index2)
        {
          if (index1 != 0 || index2 != 0)
            Draw.SpriteBatch.Draw(this.Texture.Texture, Vector2.op_Addition(position, new Vector2((float) index1, (float) index2)), new Rectangle?(this.ClipRect), Color.get_Black(), 0.0f, Vector2.op_Subtraction(new Vector2((float) this.Width * (float) justify.X, (float) this.Height * (float) justify.Y), this.DrawOffset), 1f, (SpriteEffects) 0, 0.0f);
        }
      }
      Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), Color.get_White(), 0.0f, Vector2.op_Subtraction(new Vector2((float) this.Width * (float) justify.X, (float) this.Height * (float) justify.Y), this.DrawOffset), 1f, (SpriteEffects) 0, 0.0f);
    }

    public void DrawOutlineJustified(Vector2 position, Vector2 justify, Color color)
    {
      for (int index1 = -1; index1 <= 1; ++index1)
      {
        for (int index2 = -1; index2 <= 1; ++index2)
        {
          if (index1 != 0 || index2 != 0)
            Draw.SpriteBatch.Draw(this.Texture.Texture, Vector2.op_Addition(position, new Vector2((float) index1, (float) index2)), new Rectangle?(this.ClipRect), Color.get_Black(), 0.0f, Vector2.op_Subtraction(new Vector2((float) this.Width * (float) justify.X, (float) this.Height * (float) justify.Y), this.DrawOffset), 1f, (SpriteEffects) 0, 0.0f);
        }
      }
      Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, 0.0f, Vector2.op_Subtraction(new Vector2((float) this.Width * (float) justify.X, (float) this.Height * (float) justify.Y), this.DrawOffset), 1f, (SpriteEffects) 0, 0.0f);
    }

    public void DrawOutlineJustified(Vector2 position, Vector2 justify, Color color, float scale)
    {
      for (int index1 = -1; index1 <= 1; ++index1)
      {
        for (int index2 = -1; index2 <= 1; ++index2)
        {
          if (index1 != 0 || index2 != 0)
            Draw.SpriteBatch.Draw(this.Texture.Texture, Vector2.op_Addition(position, new Vector2((float) index1, (float) index2)), new Rectangle?(this.ClipRect), Color.get_Black(), 0.0f, Vector2.op_Subtraction(new Vector2((float) this.Width * (float) justify.X, (float) this.Height * (float) justify.Y), this.DrawOffset), scale, (SpriteEffects) 0, 0.0f);
        }
      }
      Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, 0.0f, Vector2.op_Subtraction(new Vector2((float) this.Width * (float) justify.X, (float) this.Height * (float) justify.Y), this.DrawOffset), scale, (SpriteEffects) 0, 0.0f);
    }

    public void DrawOutlineJustified(
      Vector2 position,
      Vector2 justify,
      Color color,
      float scale,
      float rotation)
    {
      for (int index1 = -1; index1 <= 1; ++index1)
      {
        for (int index2 = -1; index2 <= 1; ++index2)
        {
          if (index1 != 0 || index2 != 0)
            Draw.SpriteBatch.Draw(this.Texture.Texture, Vector2.op_Addition(position, new Vector2((float) index1, (float) index2)), new Rectangle?(this.ClipRect), Color.get_Black(), rotation, Vector2.op_Subtraction(new Vector2((float) this.Width * (float) justify.X, (float) this.Height * (float) justify.Y), this.DrawOffset), scale, (SpriteEffects) 0, 0.0f);
        }
      }
      Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, Vector2.op_Subtraction(new Vector2((float) this.Width * (float) justify.X, (float) this.Height * (float) justify.Y), this.DrawOffset), scale, (SpriteEffects) 0, 0.0f);
    }

    public void DrawOutlineJustified(
      Vector2 position,
      Vector2 justify,
      Color color,
      float scale,
      float rotation,
      SpriteEffects flip)
    {
      for (int index1 = -1; index1 <= 1; ++index1)
      {
        for (int index2 = -1; index2 <= 1; ++index2)
        {
          if (index1 != 0 || index2 != 0)
            Draw.SpriteBatch.Draw(this.Texture.Texture, Vector2.op_Addition(position, new Vector2((float) index1, (float) index2)), new Rectangle?(this.ClipRect), Color.get_Black(), rotation, Vector2.op_Subtraction(new Vector2((float) this.Width * (float) justify.X, (float) this.Height * (float) justify.Y), this.DrawOffset), scale, flip, 0.0f);
        }
      }
      Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, Vector2.op_Subtraction(new Vector2((float) this.Width * (float) justify.X, (float) this.Height * (float) justify.Y), this.DrawOffset), scale, flip, 0.0f);
    }

    public void DrawOutlineJustified(
      Vector2 position,
      Vector2 justify,
      Color color,
      Vector2 scale)
    {
      for (int index1 = -1; index1 <= 1; ++index1)
      {
        for (int index2 = -1; index2 <= 1; ++index2)
        {
          if (index1 != 0 || index2 != 0)
            Draw.SpriteBatch.Draw(this.Texture.Texture, Vector2.op_Addition(position, new Vector2((float) index1, (float) index2)), new Rectangle?(this.ClipRect), Color.get_Black(), 0.0f, Vector2.op_Subtraction(new Vector2((float) this.Width * (float) justify.X, (float) this.Height * (float) justify.Y), this.DrawOffset), scale, (SpriteEffects) 0, 0.0f);
        }
      }
      Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, 0.0f, Vector2.op_Subtraction(new Vector2((float) this.Width * (float) justify.X, (float) this.Height * (float) justify.Y), this.DrawOffset), scale, (SpriteEffects) 0, 0.0f);
    }

    public void DrawOutlineJustified(
      Vector2 position,
      Vector2 justify,
      Color color,
      Vector2 scale,
      float rotation)
    {
      for (int index1 = -1; index1 <= 1; ++index1)
      {
        for (int index2 = -1; index2 <= 1; ++index2)
        {
          if (index1 != 0 || index2 != 0)
            Draw.SpriteBatch.Draw(this.Texture.Texture, Vector2.op_Addition(position, new Vector2((float) index1, (float) index2)), new Rectangle?(this.ClipRect), Color.get_Black(), rotation, Vector2.op_Subtraction(new Vector2((float) this.Width * (float) justify.X, (float) this.Height * (float) justify.Y), this.DrawOffset), scale, (SpriteEffects) 0, 0.0f);
        }
      }
      Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, Vector2.op_Subtraction(new Vector2((float) this.Width * (float) justify.X, (float) this.Height * (float) justify.Y), this.DrawOffset), scale, (SpriteEffects) 0, 0.0f);
    }

    public void DrawOutlineJustified(
      Vector2 position,
      Vector2 justify,
      Color color,
      Vector2 scale,
      float rotation,
      SpriteEffects flip)
    {
      for (int index1 = -1; index1 <= 1; ++index1)
      {
        for (int index2 = -1; index2 <= 1; ++index2)
        {
          if (index1 != 0 || index2 != 0)
            Draw.SpriteBatch.Draw(this.Texture.Texture, Vector2.op_Addition(position, new Vector2((float) index1, (float) index2)), new Rectangle?(this.ClipRect), Color.get_Black(), rotation, Vector2.op_Subtraction(new Vector2((float) this.Width * (float) justify.X, (float) this.Height * (float) justify.Y), this.DrawOffset), scale, flip, 0.0f);
        }
      }
      Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, Vector2.op_Subtraction(new Vector2((float) this.Width * (float) justify.X, (float) this.Height * (float) justify.Y), this.DrawOffset), scale, flip, 0.0f);
    }
  }
}
