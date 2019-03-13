// Decompiled with JetBrains decompiler
// Type: Monocle.Draw
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Monocle
{
  public static class Draw
  {
    public static MTexture Particle;
    public static MTexture Pixel;
    private static Rectangle rect;

    public static Renderer Renderer { get; internal set; }

    public static SpriteBatch SpriteBatch { get; private set; }

    public static SpriteFont DefaultFont { get; private set; }

    internal static void Initialize(GraphicsDevice graphicsDevice)
    {
      Draw.SpriteBatch = new SpriteBatch(graphicsDevice);
      Draw.DefaultFont = (SpriteFont) Engine.Instance.get_Content().Load<SpriteFont>("Monocle\\MonocleDefault");
      Draw.UseDebugPixelTexture();
    }

    public static void UseDebugPixelTexture()
    {
      MTexture parent = new MTexture(VirtualContent.CreateTexture("debug-pixel", 3, 3, Color.get_White()));
      Draw.Pixel = new MTexture(parent, 1, 1, 1, 1);
      Draw.Particle = new MTexture(parent, 1, 1, 1, 1);
    }

    public static void Point(Vector2 at, Color color)
    {
      Draw.SpriteBatch.Draw(Draw.Pixel.Texture.Texture, at, new Rectangle?(Draw.Pixel.ClipRect), color, 0.0f, Vector2.get_Zero(), 1f, (SpriteEffects) 0, 0.0f);
    }

    public static void Line(Vector2 start, Vector2 end, Color color)
    {
      Draw.LineAngle(start, Calc.Angle(start, end), Vector2.Distance(start, end), color);
    }

    public static void Line(Vector2 start, Vector2 end, Color color, float thickness)
    {
      Draw.LineAngle(start, Calc.Angle(start, end), Vector2.Distance(start, end), color, thickness);
    }

    public static void Line(float x1, float y1, float x2, float y2, Color color)
    {
      Draw.Line(new Vector2(x1, y1), new Vector2(x2, y2), color);
    }

    public static void LineAngle(Vector2 start, float angle, float length, Color color)
    {
      Draw.SpriteBatch.Draw(Draw.Pixel.Texture.Texture, start, new Rectangle?(Draw.Pixel.ClipRect), color, angle, Vector2.get_Zero(), new Vector2(length, 1f), (SpriteEffects) 0, 0.0f);
    }

    public static void LineAngle(
      Vector2 start,
      float angle,
      float length,
      Color color,
      float thickness)
    {
      Draw.SpriteBatch.Draw(Draw.Pixel.Texture.Texture, start, new Rectangle?(Draw.Pixel.ClipRect), color, angle, new Vector2(0.0f, 0.5f), new Vector2(length, thickness), (SpriteEffects) 0, 0.0f);
    }

    public static void LineAngle(
      float startX,
      float startY,
      float angle,
      float length,
      Color color)
    {
      Draw.LineAngle(new Vector2(startX, startY), angle, length, color);
    }

    public static void Circle(Vector2 position, float radius, Color color, int resolution)
    {
      Vector2 vector1 = Vector2.op_Multiply(Vector2.get_UnitX(), radius);
      Vector2 vector2_1 = vector1.Perpendicular();
      for (int index = 1; index <= resolution; ++index)
      {
        Vector2 vector2 = Calc.AngleToVector((float) index * 1.570796f / (float) resolution, radius);
        Vector2 vector2_2 = vector2.Perpendicular();
        Draw.Line(Vector2.op_Addition(position, vector1), Vector2.op_Addition(position, vector2), color);
        Draw.Line(Vector2.op_Subtraction(position, vector1), Vector2.op_Subtraction(position, vector2), color);
        Draw.Line(Vector2.op_Addition(position, vector2_1), Vector2.op_Addition(position, vector2_2), color);
        Draw.Line(Vector2.op_Subtraction(position, vector2_1), Vector2.op_Subtraction(position, vector2_2), color);
        vector1 = vector2;
        vector2_1 = vector2_2;
      }
    }

    public static void Circle(float x, float y, float radius, Color color, int resolution)
    {
      Draw.Circle(new Vector2(x, y), radius, color, resolution);
    }

    public static void Circle(
      Vector2 position,
      float radius,
      Color color,
      float thickness,
      int resolution)
    {
      Vector2 vector1 = Vector2.op_Multiply(Vector2.get_UnitX(), radius);
      Vector2 vector2_1 = vector1.Perpendicular();
      for (int index = 1; index <= resolution; ++index)
      {
        Vector2 vector2 = Calc.AngleToVector((float) index * 1.570796f / (float) resolution, radius);
        Vector2 vector2_2 = vector2.Perpendicular();
        Draw.Line(Vector2.op_Addition(position, vector1), Vector2.op_Addition(position, vector2), color, thickness);
        Draw.Line(Vector2.op_Subtraction(position, vector1), Vector2.op_Subtraction(position, vector2), color, thickness);
        Draw.Line(Vector2.op_Addition(position, vector2_1), Vector2.op_Addition(position, vector2_2), color, thickness);
        Draw.Line(Vector2.op_Subtraction(position, vector2_1), Vector2.op_Subtraction(position, vector2_2), color, thickness);
        vector1 = vector2;
        vector2_1 = vector2_2;
      }
    }

    public static void Circle(
      float x,
      float y,
      float radius,
      Color color,
      float thickness,
      int resolution)
    {
      Draw.Circle(new Vector2(x, y), radius, color, thickness, resolution);
    }

    public static void Rect(float x, float y, float width, float height, Color color)
    {
      Draw.rect.X = (__Null) (int) x;
      Draw.rect.Y = (__Null) (int) y;
      Draw.rect.Width = (__Null) (int) width;
      Draw.rect.Height = (__Null) (int) height;
      Draw.SpriteBatch.Draw(Draw.Pixel.Texture.Texture, Draw.rect, new Rectangle?(Draw.Pixel.ClipRect), color);
    }

    public static void Rect(Vector2 position, float width, float height, Color color)
    {
      Draw.Rect((float) position.X, (float) position.Y, width, height, color);
    }

    public static void Rect(Rectangle rect, Color color)
    {
      Draw.rect = rect;
      Draw.SpriteBatch.Draw(Draw.Pixel.Texture.Texture, rect, new Rectangle?(Draw.Pixel.ClipRect), color);
    }

    public static void Rect(Collider collider, Color color)
    {
      Draw.Rect(collider.AbsoluteLeft, collider.AbsoluteTop, collider.Width, collider.Height, color);
    }

    public static void HollowRect(float x, float y, float width, float height, Color color)
    {
      Draw.rect.X = (__Null) (int) x;
      Draw.rect.Y = (__Null) (int) y;
      Draw.rect.Width = (__Null) (int) width;
      Draw.rect.Height = (__Null) 1;
      Draw.SpriteBatch.Draw(Draw.Pixel.Texture.Texture, Draw.rect, new Rectangle?(Draw.Pixel.ClipRect), color);
      ref __Null local1 = ref Draw.rect.Y;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(int&) ref local1 = ^(int&) ref local1 + ((int) height - 1);
      Draw.SpriteBatch.Draw(Draw.Pixel.Texture.Texture, Draw.rect, new Rectangle?(Draw.Pixel.ClipRect), color);
      ref __Null local2 = ref Draw.rect.Y;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(int&) ref local2 = ^(int&) ref local2 - ((int) height - 1);
      Draw.rect.Width = (__Null) 1;
      Draw.rect.Height = (__Null) (int) height;
      Draw.SpriteBatch.Draw(Draw.Pixel.Texture.Texture, Draw.rect, new Rectangle?(Draw.Pixel.ClipRect), color);
      ref __Null local3 = ref Draw.rect.X;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(int&) ref local3 = ^(int&) ref local3 + ((int) width - 1);
      Draw.SpriteBatch.Draw(Draw.Pixel.Texture.Texture, Draw.rect, new Rectangle?(Draw.Pixel.ClipRect), color);
    }

    public static void HollowRect(Vector2 position, float width, float height, Color color)
    {
      Draw.HollowRect((float) position.X, (float) position.Y, width, height, color);
    }

    public static void HollowRect(Rectangle rect, Color color)
    {
      Draw.HollowRect((float) rect.X, (float) rect.Y, (float) rect.Width, (float) rect.Height, color);
    }

    public static void HollowRect(Collider collider, Color color)
    {
      Draw.HollowRect(collider.AbsoluteLeft, collider.AbsoluteTop, collider.Width, collider.Height, color);
    }

    public static void Text(SpriteFont font, string text, Vector2 position, Color color)
    {
      Draw.SpriteBatch.DrawString(font, text, position.Floor(), color);
    }

    public static void Text(
      SpriteFont font,
      string text,
      Vector2 position,
      Color color,
      Vector2 origin,
      Vector2 scale,
      float rotation)
    {
      Draw.SpriteBatch.DrawString(font, text, position.Floor(), color, rotation, origin, scale, (SpriteEffects) 0, 0.0f);
    }

    public static void TextJustified(
      SpriteFont font,
      string text,
      Vector2 position,
      Color color,
      Vector2 justify)
    {
      Vector2 vector2 = font.MeasureString(text);
      ref __Null local1 = ref vector2.X;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local1 = ^(float&) ref local1 * (float) justify.X;
      ref __Null local2 = ref vector2.Y;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local2 = ^(float&) ref local2 * (float) justify.Y;
      Draw.SpriteBatch.DrawString(font, text, position.Floor(), color, 0.0f, vector2, 1f, (SpriteEffects) 0, 0.0f);
    }

    public static void TextJustified(
      SpriteFont font,
      string text,
      Vector2 position,
      Color color,
      float scale,
      Vector2 justify)
    {
      Vector2 vector2 = font.MeasureString(text);
      ref __Null local1 = ref vector2.X;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local1 = ^(float&) ref local1 * (float) justify.X;
      ref __Null local2 = ref vector2.Y;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local2 = ^(float&) ref local2 * (float) justify.Y;
      Draw.SpriteBatch.DrawString(font, text, position.Floor(), color, 0.0f, vector2, scale, (SpriteEffects) 0, 0.0f);
    }

    public static void TextCentered(SpriteFont font, string text, Vector2 position)
    {
      Draw.Text(font, text, Vector2.op_Subtraction(position, Vector2.op_Multiply(font.MeasureString(text), 0.5f)), Color.get_White());
    }

    public static void TextCentered(SpriteFont font, string text, Vector2 position, Color color)
    {
      Draw.Text(font, text, Vector2.op_Subtraction(position, Vector2.op_Multiply(font.MeasureString(text), 0.5f)), color);
    }

    public static void TextCentered(
      SpriteFont font,
      string text,
      Vector2 position,
      Color color,
      float scale)
    {
      Draw.Text(font, text, position, color, Vector2.op_Multiply(font.MeasureString(text), 0.5f), Vector2.op_Multiply(Vector2.get_One(), scale), 0.0f);
    }

    public static void TextCentered(
      SpriteFont font,
      string text,
      Vector2 position,
      Color color,
      float scale,
      float rotation)
    {
      Draw.Text(font, text, position, color, Vector2.op_Multiply(font.MeasureString(text), 0.5f), Vector2.op_Multiply(Vector2.get_One(), scale), rotation);
    }

    public static void OutlineTextCentered(
      SpriteFont font,
      string text,
      Vector2 position,
      Color color,
      float scale)
    {
      Vector2 vector2 = Vector2.op_Division(font.MeasureString(text), 2f);
      for (int index1 = -1; index1 < 2; ++index1)
      {
        for (int index2 = -1; index2 < 2; ++index2)
        {
          if (index1 != 0 || index2 != 0)
            Draw.SpriteBatch.DrawString(font, text, Vector2.op_Addition(position.Floor(), new Vector2((float) index1, (float) index2)), Color.get_Black(), 0.0f, vector2, scale, (SpriteEffects) 0, 0.0f);
        }
      }
      Draw.SpriteBatch.DrawString(font, text, position.Floor(), color, 0.0f, vector2, scale, (SpriteEffects) 0, 0.0f);
    }

    public static void OutlineTextCentered(
      SpriteFont font,
      string text,
      Vector2 position,
      Color color,
      Color outlineColor)
    {
      Vector2 vector2 = Vector2.op_Division(font.MeasureString(text), 2f);
      for (int index1 = -1; index1 < 2; ++index1)
      {
        for (int index2 = -1; index2 < 2; ++index2)
        {
          if (index1 != 0 || index2 != 0)
            Draw.SpriteBatch.DrawString(font, text, Vector2.op_Addition(position.Floor(), new Vector2((float) index1, (float) index2)), outlineColor, 0.0f, vector2, 1f, (SpriteEffects) 0, 0.0f);
        }
      }
      Draw.SpriteBatch.DrawString(font, text, position.Floor(), color, 0.0f, vector2, 1f, (SpriteEffects) 0, 0.0f);
    }

    public static void OutlineTextCentered(
      SpriteFont font,
      string text,
      Vector2 position,
      Color color,
      Color outlineColor,
      float scale)
    {
      Vector2 vector2 = Vector2.op_Division(font.MeasureString(text), 2f);
      for (int index1 = -1; index1 < 2; ++index1)
      {
        for (int index2 = -1; index2 < 2; ++index2)
        {
          if (index1 != 0 || index2 != 0)
            Draw.SpriteBatch.DrawString(font, text, Vector2.op_Addition(position.Floor(), new Vector2((float) index1, (float) index2)), outlineColor, 0.0f, vector2, scale, (SpriteEffects) 0, 0.0f);
        }
      }
      Draw.SpriteBatch.DrawString(font, text, position.Floor(), color, 0.0f, vector2, scale, (SpriteEffects) 0, 0.0f);
    }

    public static void OutlineTextJustify(
      SpriteFont font,
      string text,
      Vector2 position,
      Color color,
      Color outlineColor,
      Vector2 justify)
    {
      Vector2 vector2 = Vector2.op_Multiply(font.MeasureString(text), justify);
      for (int index1 = -1; index1 < 2; ++index1)
      {
        for (int index2 = -1; index2 < 2; ++index2)
        {
          if (index1 != 0 || index2 != 0)
            Draw.SpriteBatch.DrawString(font, text, Vector2.op_Addition(position.Floor(), new Vector2((float) index1, (float) index2)), outlineColor, 0.0f, vector2, 1f, (SpriteEffects) 0, 0.0f);
        }
      }
      Draw.SpriteBatch.DrawString(font, text, position.Floor(), color, 0.0f, vector2, 1f, (SpriteEffects) 0, 0.0f);
    }

    public static void OutlineTextJustify(
      SpriteFont font,
      string text,
      Vector2 position,
      Color color,
      Color outlineColor,
      Vector2 justify,
      float scale)
    {
      Vector2 vector2 = Vector2.op_Multiply(font.MeasureString(text), justify);
      for (int index1 = -1; index1 < 2; ++index1)
      {
        for (int index2 = -1; index2 < 2; ++index2)
        {
          if (index1 != 0 || index2 != 0)
            Draw.SpriteBatch.DrawString(font, text, Vector2.op_Addition(position.Floor(), new Vector2((float) index1, (float) index2)), outlineColor, 0.0f, vector2, scale, (SpriteEffects) 0, 0.0f);
        }
      }
      Draw.SpriteBatch.DrawString(font, text, position.Floor(), color, 0.0f, vector2, scale, (SpriteEffects) 0, 0.0f);
    }

    public static void SineTextureH(
      MTexture tex,
      Vector2 position,
      Vector2 origin,
      Vector2 scale,
      float rotation,
      Color color,
      SpriteEffects effects,
      float sineCounter,
      float amplitude = 2f,
      int sliceSize = 2,
      float sliceAdd = 0.7853982f)
    {
      position = position.Floor();
      Rectangle clipRect = tex.ClipRect;
      clipRect.Width = (__Null) sliceSize;
      int num = 0;
      for (; clipRect.X < tex.ClipRect.X + tex.ClipRect.Width; clipRect.Width = (__Null) Math.Min(sliceSize, (int) (tex.ClipRect.X + tex.ClipRect.Width - clipRect.X)))
      {
        Vector2 vector2;
        ((Vector2) ref vector2).\u002Ector((float) (sliceSize * num), (float) Math.Round(Math.Sin((double) sineCounter + (double) sliceAdd * (double) num) * (double) amplitude));
        Draw.SpriteBatch.Draw(tex.Texture.Texture, position, new Rectangle?(clipRect), color, rotation, Vector2.op_Subtraction(origin, vector2), scale, effects, 0.0f);
        ++num;
        ref __Null local = ref clipRect.X;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(int&) ref local = ^(int&) ref local + sliceSize;
      }
    }

    public static void SineTextureV(
      MTexture tex,
      Vector2 position,
      Vector2 origin,
      Vector2 scale,
      float rotation,
      Color color,
      SpriteEffects effects,
      float sineCounter,
      float amplitude = 2f,
      int sliceSize = 2,
      float sliceAdd = 0.7853982f)
    {
      position = position.Floor();
      Rectangle clipRect = tex.ClipRect;
      clipRect.Height = (__Null) sliceSize;
      int num = 0;
      for (; clipRect.Y < tex.ClipRect.Y + tex.ClipRect.Height; clipRect.Height = (__Null) Math.Min(sliceSize, (int) (tex.ClipRect.Y + tex.ClipRect.Height - clipRect.Y)))
      {
        Vector2 vector2;
        ((Vector2) ref vector2).\u002Ector((float) Math.Round(Math.Sin((double) sineCounter + (double) sliceAdd * (double) num) * (double) amplitude), (float) (sliceSize * num));
        Draw.SpriteBatch.Draw(tex.Texture.Texture, position, new Rectangle?(clipRect), color, rotation, Vector2.op_Subtraction(origin, vector2), scale, effects, 0.0f);
        ++num;
        ref __Null local = ref clipRect.Y;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(int&) ref local = ^(int&) ref local + sliceSize;
      }
    }

    public static void TextureBannerV(
      MTexture tex,
      Vector2 position,
      Vector2 origin,
      Vector2 scale,
      float rotation,
      Color color,
      SpriteEffects effects,
      float sineCounter,
      float amplitude = 2f,
      int sliceSize = 2,
      float sliceAdd = 0.7853982f)
    {
      position = position.Floor();
      Rectangle clipRect = tex.ClipRect;
      clipRect.Height = (__Null) sliceSize;
      int num1 = 0;
      while (clipRect.Y < tex.ClipRect.Y + tex.ClipRect.Height)
      {
        float num2 = (float) (clipRect.Y - tex.ClipRect.Y) / (float) tex.ClipRect.Height;
        clipRect.Height = (__Null) (int) MathHelper.Lerp((float) sliceSize, 1f, num2);
        clipRect.Height = (__Null) Math.Min(sliceSize, (int) (tex.ClipRect.Y + tex.ClipRect.Height - clipRect.Y));
        Vector2 vector2;
        ((Vector2) ref vector2).\u002Ector((float) Math.Round(Math.Sin((double) sineCounter + (double) sliceAdd * (double) num1) * (double) amplitude * (double) num2), (float) (clipRect.Y - tex.ClipRect.Y));
        Draw.SpriteBatch.Draw(tex.Texture.Texture, position, new Rectangle?(clipRect), color, rotation, Vector2.op_Subtraction(origin, vector2), scale, effects, 0.0f);
        ++num1;
        ref __Null local = ref clipRect.Y;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(int&) ref local = ^(int&) ref local + clipRect.Height;
      }
    }
  }
}
