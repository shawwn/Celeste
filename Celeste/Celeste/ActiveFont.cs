// Decompiled with JetBrains decompiler
// Type: Celeste.ActiveFont
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  public static class ActiveFont
  {
    public static PixelFont Font
    {
      get
      {
        return Fonts.Faces[Dialog.Language.FontFace];
      }
    }

    public static PixelFontSize FontSize
    {
      get
      {
        return ActiveFont.Font.Get(ActiveFont.BaseSize);
      }
    }

    public static float BaseSize
    {
      get
      {
        return Dialog.Language.FontFaceSize;
      }
    }

    public static float LineHeight
    {
      get
      {
        return (float) ActiveFont.Font.Get(ActiveFont.BaseSize).LineHeight;
      }
    }

    public static Vector2 Measure(char text)
    {
      return ActiveFont.Font.Get(ActiveFont.BaseSize).Measure(text);
    }

    public static Vector2 Measure(string text)
    {
      return ActiveFont.Font.Get(ActiveFont.BaseSize).Measure(text);
    }

    public static float WidthToNextLine(string text, int start)
    {
      return ActiveFont.Font.Get(ActiveFont.BaseSize).WidthToNextLine(text, start);
    }

    public static float HeightOf(string text)
    {
      return ActiveFont.Font.Get(ActiveFont.BaseSize).HeightOf(text);
    }

    public static void Draw(
      char character,
      Vector2 position,
      Vector2 justify,
      Vector2 scale,
      Color color)
    {
      ActiveFont.Font.Draw(ActiveFont.BaseSize, character, position, justify, scale, color);
    }

    private static void Draw(
      string text,
      Vector2 position,
      Vector2 justify,
      Vector2 scale,
      Color color,
      float edgeDepth,
      Color edgeColor,
      float stroke,
      Color strokeColor)
    {
      ActiveFont.Font.Draw(ActiveFont.BaseSize, text, position, justify, scale, color, edgeDepth, edgeColor, stroke, strokeColor);
    }

    public static void Draw(string text, Vector2 position, Color color)
    {
      ActiveFont.Draw(text, position, Vector2.Zero, Vector2.One, color, 0.0f, Color.Transparent, 0.0f, Color.Transparent);
    }

    public static void Draw(
      string text,
      Vector2 position,
      Vector2 justify,
      Vector2 scale,
      Color color)
    {
      ActiveFont.Draw(text, position, justify, scale, color, 0.0f, Color.Transparent, 0.0f, Color.Transparent);
    }

    public static void DrawOutline(
      string text,
      Vector2 position,
      Vector2 justify,
      Vector2 scale,
      Color color,
      float stroke,
      Color strokeColor)
    {
      ActiveFont.Draw(text, position, justify, scale, color, 0.0f, Color.Transparent, stroke, strokeColor);
    }

    public static void DrawEdgeOutline(
      string text,
      Vector2 position,
      Vector2 justify,
      Vector2 scale,
      Color color,
      float edgeDepth,
      Color edgeColor,
      float stroke = 0.0f,
      Color strokeColor = default (Color))
    {
      ActiveFont.Draw(text, position, justify, scale, color, edgeDepth, edgeColor, stroke, strokeColor);
    }
  }
}

