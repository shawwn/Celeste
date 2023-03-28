// Decompiled with JetBrains decompiler
// Type: Monocle.Draw
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

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
      Draw.DefaultFont = Engine.Instance.Content.Load<SpriteFont>("Monocle\\MonocleDefault");
      Draw.UseDebugPixelTexture();
    }

    public static void UseDebugPixelTexture()
    {
      MTexture parent = new MTexture(VirtualContent.CreateTexture("debug-pixel", 3, 3, Color.White));
      Draw.Pixel = new MTexture(parent, 1, 1, 1, 1);
      Draw.Particle = new MTexture(parent, 1, 1, 1, 1);
    }

    public static void Point(Vector2 at, Color color) => Draw.SpriteBatch.Draw(Draw.Pixel.Texture.Texture, at, new Rectangle?(Draw.Pixel.ClipRect), color, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);

    public static void Line(Vector2 start, Vector2 end, Color color) => Draw.LineAngle(start, Calc.Angle(start, end), Vector2.Distance(start, end), color);

    public static void Line(Vector2 start, Vector2 end, Color color, float thickness) => Draw.LineAngle(start, Calc.Angle(start, end), Vector2.Distance(start, end), color, thickness);

    public static void Line(float x1, float y1, float x2, float y2, Color color) => Draw.Line(new Vector2(x1, y1), new Vector2(x2, y2), color);

    public static void Line(
      float x1,
      float y1,
      float x2,
      float y2,
      Color color,
      float thickness)
    {
      Draw.Line(new Vector2(x1, y1), new Vector2(x2, y2), color, thickness);
    }

    public static void LineAngle(Vector2 start, float angle, float length, Color color) => Draw.SpriteBatch.Draw(Draw.Pixel.Texture.Texture, start, new Rectangle?(Draw.Pixel.ClipRect), color, angle, Vector2.Zero, new Vector2(length, 1f), SpriteEffects.None, 0.0f);

    public static void LineAngle(
      Vector2 start,
      float angle,
      float length,
      Color color,
      float thickness)
    {
      Draw.SpriteBatch.Draw(Draw.Pixel.Texture.Texture, start, new Rectangle?(Draw.Pixel.ClipRect), color, angle, new Vector2(0.0f, 0.5f), new Vector2(length, thickness), SpriteEffects.None, 0.0f);
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
      Vector2 vector1 = Vector2.UnitX * radius;
      Vector2 vector2_1 = vector1.Perpendicular();
      for (int index = 1; index <= resolution; ++index)
      {
        Vector2 vector2 = Calc.AngleToVector((float) index * 1.5707964f / (float) resolution, radius);
        Vector2 vector2_2 = vector2.Perpendicular();
        Draw.Line(position + vector1, position + vector2, color);
        Draw.Line(position - vector1, position - vector2, color);
        Draw.Line(position + vector2_1, position + vector2_2, color);
        Draw.Line(position - vector2_1, position - vector2_2, color);
        vector1 = vector2;
        vector2_1 = vector2_2;
      }
    }

    public static void Circle(float x, float y, float radius, Color color, int resolution) => Draw.Circle(new Vector2(x, y), radius, color, resolution);

    public static void Circle(
      Vector2 position,
      float radius,
      Color color,
      float thickness,
      int resolution)
    {
      Vector2 vector1 = Vector2.UnitX * radius;
      Vector2 vector2_1 = vector1.Perpendicular();
      for (int index = 1; index <= resolution; ++index)
      {
        Vector2 vector2 = Calc.AngleToVector((float) index * 1.5707964f / (float) resolution, radius);
        Vector2 vector2_2 = vector2.Perpendicular();
        Draw.Line(position + vector1, position + vector2, color, thickness);
        Draw.Line(position - vector1, position - vector2, color, thickness);
        Draw.Line(position + vector2_1, position + vector2_2, color, thickness);
        Draw.Line(position - vector2_1, position - vector2_2, color, thickness);
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
      Draw.rect.X = (int) x;
      Draw.rect.Y = (int) y;
      Draw.rect.Width = (int) width;
      Draw.rect.Height = (int) height;
      Draw.SpriteBatch.Draw(Draw.Pixel.Texture.Texture, Draw.rect, new Rectangle?(Draw.Pixel.ClipRect), color);
    }

    public static void Rect(Vector2 position, float width, float height, Color color) => Draw.Rect(position.X, position.Y, width, height, color);

    public static void Rect(Rectangle rect, Color color)
    {
      Draw.rect = rect;
      Draw.SpriteBatch.Draw(Draw.Pixel.Texture.Texture, rect, new Rectangle?(Draw.Pixel.ClipRect), color);
    }

    public static void Rect(Collider collider, Color color) => Draw.Rect(collider.AbsoluteLeft, collider.AbsoluteTop, collider.Width, collider.Height, color);

    public static void HollowRect(float x, float y, float width, float height, Color color)
    {
      Draw.rect.X = (int) x;
      Draw.rect.Y = (int) y;
      Draw.rect.Width = (int) width;
      Draw.rect.Height = 1;
      Draw.SpriteBatch.Draw(Draw.Pixel.Texture.Texture, Draw.rect, new Rectangle?(Draw.Pixel.ClipRect), color);
      Draw.rect.Y += (int) height - 1;
      Draw.SpriteBatch.Draw(Draw.Pixel.Texture.Texture, Draw.rect, new Rectangle?(Draw.Pixel.ClipRect), color);
      Draw.rect.Y -= (int) height - 1;
      Draw.rect.Width = 1;
      Draw.rect.Height = (int) height;
      Draw.SpriteBatch.Draw(Draw.Pixel.Texture.Texture, Draw.rect, new Rectangle?(Draw.Pixel.ClipRect), color);
      Draw.rect.X += (int) width - 1;
      Draw.SpriteBatch.Draw(Draw.Pixel.Texture.Texture, Draw.rect, new Rectangle?(Draw.Pixel.ClipRect), color);
    }

    public static void HollowRect(Vector2 position, float width, float height, Color color) => Draw.HollowRect(position.X, position.Y, width, height, color);

    public static void HollowRect(Rectangle rect, Color color) => Draw.HollowRect((float) rect.X, (float) rect.Y, (float) rect.Width, (float) rect.Height, color);

    public static void HollowRect(Collider collider, Color color) => Draw.HollowRect(collider.AbsoluteLeft, collider.AbsoluteTop, collider.Width, collider.Height, color);

    public static void Text(SpriteFont font, string text, Vector2 position, Color color) => Draw.SpriteBatch.DrawString(font, text, position.Floor(), color);

    public static void Text(
      SpriteFont font,
      string text,
      Vector2 position,
      Color color,
      Vector2 origin,
      Vector2 scale,
      float rotation)
    {
      Draw.SpriteBatch.DrawString(font, text, position.Floor(), color, rotation, origin, scale, SpriteEffects.None, 0.0f);
    }

    public static void TextJustified(
      SpriteFont font,
      string text,
      Vector2 position,
      Color color,
      Vector2 justify)
    {
      Vector2 origin = font.MeasureString(text);
      origin.X *= justify.X;
      origin.Y *= justify.Y;
      Draw.SpriteBatch.DrawString(font, text, position.Floor(), color, 0.0f, origin, 1f, SpriteEffects.None, 0.0f);
    }

    public static void TextJustified(
      SpriteFont font,
      string text,
      Vector2 position,
      Color color,
      float scale,
      Vector2 justify)
    {
      Vector2 origin = font.MeasureString(text);
      origin.X *= justify.X;
      origin.Y *= justify.Y;
      Draw.SpriteBatch.DrawString(font, text, position.Floor(), color, 0.0f, origin, scale, SpriteEffects.None, 0.0f);
    }

    public static void TextCentered(SpriteFont font, string text, Vector2 position) => Draw.Text(font, text, position - font.MeasureString(text) * 0.5f, Color.White);

    public static void TextCentered(SpriteFont font, string text, Vector2 position, Color color) => Draw.Text(font, text, position - font.MeasureString(text) * 0.5f, color);

    public static void TextCentered(
      SpriteFont font,
      string text,
      Vector2 position,
      Color color,
      float scale)
    {
      Draw.Text(font, text, position, color, font.MeasureString(text) * 0.5f, Vector2.One * scale, 0.0f);
    }

    public static void TextCentered(
      SpriteFont font,
      string text,
      Vector2 position,
      Color color,
      float scale,
      float rotation)
    {
      Draw.Text(font, text, position, color, font.MeasureString(text) * 0.5f, Vector2.One * scale, rotation);
    }

    public static void OutlineTextCentered(
      SpriteFont font,
      string text,
      Vector2 position,
      Color color,
      float scale)
    {
      Vector2 origin = font.MeasureString(text) / 2f;
      for (int x = -1; x < 2; ++x)
      {
        for (int y = -1; y < 2; ++y)
        {
          if (x != 0 || y != 0)
            Draw.SpriteBatch.DrawString(font, text, position.Floor() + new Vector2((float) x, (float) y), Color.Black, 0.0f, origin, scale, SpriteEffects.None, 0.0f);
        }
      }
      Draw.SpriteBatch.DrawString(font, text, position.Floor(), color, 0.0f, origin, scale, SpriteEffects.None, 0.0f);
    }

    public static void OutlineTextCentered(
      SpriteFont font,
      string text,
      Vector2 position,
      Color color,
      Color outlineColor)
    {
      Vector2 origin = font.MeasureString(text) / 2f;
      for (int x = -1; x < 2; ++x)
      {
        for (int y = -1; y < 2; ++y)
        {
          if (x != 0 || y != 0)
            Draw.SpriteBatch.DrawString(font, text, position.Floor() + new Vector2((float) x, (float) y), outlineColor, 0.0f, origin, 1f, SpriteEffects.None, 0.0f);
        }
      }
      Draw.SpriteBatch.DrawString(font, text, position.Floor(), color, 0.0f, origin, 1f, SpriteEffects.None, 0.0f);
    }

    public static void OutlineTextCentered(
      SpriteFont font,
      string text,
      Vector2 position,
      Color color,
      Color outlineColor,
      float scale)
    {
      Vector2 origin = font.MeasureString(text) / 2f;
      for (int x = -1; x < 2; ++x)
      {
        for (int y = -1; y < 2; ++y)
        {
          if (x != 0 || y != 0)
            Draw.SpriteBatch.DrawString(font, text, position.Floor() + new Vector2((float) x, (float) y), outlineColor, 0.0f, origin, scale, SpriteEffects.None, 0.0f);
        }
      }
      Draw.SpriteBatch.DrawString(font, text, position.Floor(), color, 0.0f, origin, scale, SpriteEffects.None, 0.0f);
    }

    public static void OutlineTextJustify(
      SpriteFont font,
      string text,
      Vector2 position,
      Color color,
      Color outlineColor,
      Vector2 justify)
    {
      Vector2 origin = font.MeasureString(text) * justify;
      for (int x = -1; x < 2; ++x)
      {
        for (int y = -1; y < 2; ++y)
        {
          if (x != 0 || y != 0)
            Draw.SpriteBatch.DrawString(font, text, position.Floor() + new Vector2((float) x, (float) y), outlineColor, 0.0f, origin, 1f, SpriteEffects.None, 0.0f);
        }
      }
      Draw.SpriteBatch.DrawString(font, text, position.Floor(), color, 0.0f, origin, 1f, SpriteEffects.None, 0.0f);
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
      Vector2 origin = font.MeasureString(text) * justify;
      for (int x = -1; x < 2; ++x)
      {
        for (int y = -1; y < 2; ++y)
        {
          if (x != 0 || y != 0)
            Draw.SpriteBatch.DrawString(font, text, position.Floor() + new Vector2((float) x, (float) y), outlineColor, 0.0f, origin, scale, SpriteEffects.None, 0.0f);
        }
      }
      Draw.SpriteBatch.DrawString(font, text, position.Floor(), color, 0.0f, origin, scale, SpriteEffects.None, 0.0f);
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
      // TKTK
      // Rectangle clipRect = tex.ClipRect with
      // {
      //   Width = sliceSize
      // };
      Rectangle clipRect = new Rectangle(tex.ClipRect.X, tex.ClipRect.Y, sliceSize, tex.ClipRect.Height);
      int num = 0;
      for (; clipRect.X < tex.ClipRect.X + tex.ClipRect.Width; clipRect.Width = Math.Min(sliceSize, tex.ClipRect.X + tex.ClipRect.Width - clipRect.X))
      {
        Vector2 vector2 = new Vector2((float) (sliceSize * num), (float) Math.Round(Math.Sin((double) sineCounter + (double) sliceAdd * (double) num) * (double) amplitude));
        Draw.SpriteBatch.Draw(tex.Texture.Texture, position, new Rectangle?(clipRect), color, rotation, origin - vector2, scale, effects, 0.0f);
        ++num;
        clipRect.X += sliceSize;
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
      // TKTK
      // Rectangle clipRect = tex.ClipRect with
      // {
      //   Height = sliceSize
      // };
      Rectangle clipRect = new Rectangle(tex.ClipRect.X, tex.ClipRect.Y, sliceSize, tex.ClipRect.Height);
      int num = 0;
      for (; clipRect.Y < tex.ClipRect.Y + tex.ClipRect.Height; clipRect.Height = Math.Min(sliceSize, tex.ClipRect.Y + tex.ClipRect.Height - clipRect.Y))
      {
        Vector2 vector2 = new Vector2((float) Math.Round(Math.Sin((double) sineCounter + (double) sliceAdd * (double) num) * (double) amplitude), (float) (sliceSize * num));
        Draw.SpriteBatch.Draw(tex.Texture.Texture, position, new Rectangle?(clipRect), color, rotation, origin - vector2, scale, effects, 0.0f);
        ++num;
        clipRect.Y += sliceSize;
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
      // TKTK
      // Rectangle clipRect = tex.ClipRect with
      // {
      //   Height = sliceSize
      // };
      Rectangle clipRect = new Rectangle(tex.ClipRect.X, tex.ClipRect.Y, sliceSize, tex.ClipRect.Height);
      int num = 0;
      for (; clipRect.Y < tex.ClipRect.Y + tex.ClipRect.Height; clipRect.Y += clipRect.Height)
      {
        float amount = (float) (clipRect.Y - tex.ClipRect.Y) / (float) tex.ClipRect.Height;
        clipRect.Height = (int) MathHelper.Lerp((float) sliceSize, 1f, amount);
        clipRect.Height = Math.Min(sliceSize, tex.ClipRect.Y + tex.ClipRect.Height - clipRect.Y);
        Vector2 vector2 = new Vector2((float) Math.Round(Math.Sin((double) sineCounter + (double) sliceAdd * (double) num) * (double) amplitude * (double) amount), (float) (clipRect.Y - tex.ClipRect.Y));
        Draw.SpriteBatch.Draw(tex.Texture.Texture, position, new Rectangle?(clipRect), color, rotation, origin - vector2, scale, effects, 0.0f);
        ++num;
      }
    }
  }
}
