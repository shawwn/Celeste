// Decompiled with JetBrains decompiler
// Type: Monocle.PixelFontSize
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Monocle
{
  public class PixelFontSize
  {
    private StringBuilder temp = new StringBuilder();
    public List<MTexture> Textures;
    public Dictionary<int, PixelFontCharacter> Characters;
    public int LineHeight;
    public float Size;
    public bool Outline;

    public string AutoNewline(string text, int width)
    {
      if (string.IsNullOrEmpty(text))
        return text;
      this.temp.Clear();
      string[] strArray = Regex.Split(text, "(\\s)");
      float num1 = 0.0f;
      foreach (string text1 in strArray)
      {
        float x = (float) this.Measure(text1).X;
        if ((double) x + (double) num1 > (double) width)
        {
          this.temp.Append('\n');
          num1 = 0.0f;
          if (text1.Equals(" "))
            continue;
        }
        if ((double) x > (double) width)
        {
          int num2 = 1;
          int startIndex = 0;
          for (; num2 < text1.Length; ++num2)
          {
            if (num2 - startIndex > 1 && this.Measure(text1.Substring(startIndex, num2 - startIndex - 1)).X > (double) width)
            {
              this.temp.Append(text1.Substring(startIndex, num2 - startIndex - 1));
              this.temp.Append('\n');
              startIndex = num2 - 1;
            }
          }
          string text2 = text1.Substring(startIndex, text1.Length - startIndex);
          this.temp.Append(text2);
          num1 += (float) this.Measure(text2).X;
        }
        else
        {
          num1 += x;
          this.temp.Append(text1);
        }
      }
      return this.temp.ToString();
    }

    public PixelFontCharacter Get(int id)
    {
      PixelFontCharacter pixelFontCharacter = (PixelFontCharacter) null;
      if (this.Characters.TryGetValue(id, out pixelFontCharacter))
        return pixelFontCharacter;
      return (PixelFontCharacter) null;
    }

    public Vector2 Measure(char text)
    {
      PixelFontCharacter pixelFontCharacter = (PixelFontCharacter) null;
      if (this.Characters.TryGetValue((int) text, out pixelFontCharacter))
        return new Vector2((float) pixelFontCharacter.XAdvance, (float) this.LineHeight);
      return Vector2.get_Zero();
    }

    public Vector2 Measure(string text)
    {
      if (string.IsNullOrEmpty(text))
        return Vector2.get_Zero();
      Vector2 vector2;
      ((Vector2) ref vector2).\u002Ector(0.0f, (float) this.LineHeight);
      float num1 = 0.0f;
      for (int index = 0; index < text.Length; ++index)
      {
        if (text[index] == '\n')
        {
          ref __Null local = ref vector2.Y;
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          ^(float&) ref local = ^(float&) ref local + (float) this.LineHeight;
          if ((double) num1 > vector2.X)
            vector2.X = (__Null) (double) num1;
          num1 = 0.0f;
        }
        else
        {
          PixelFontCharacter pixelFontCharacter = (PixelFontCharacter) null;
          if (this.Characters.TryGetValue((int) text[index], out pixelFontCharacter))
          {
            num1 += (float) pixelFontCharacter.XAdvance;
            int num2;
            if (index < text.Length - 1 && pixelFontCharacter.Kerning.TryGetValue((int) text[index + 1], out num2))
              num1 += (float) num2;
          }
        }
      }
      if ((double) num1 > vector2.X)
        vector2.X = (__Null) (double) num1;
      return vector2;
    }

    public float WidthToNextLine(string text, int start)
    {
      if (string.IsNullOrEmpty(text))
        return 0.0f;
      float num1 = 0.0f;
      int index = start;
      for (int length = text.Length; index < length && text[index] != '\n'; ++index)
      {
        PixelFontCharacter pixelFontCharacter = (PixelFontCharacter) null;
        if (this.Characters.TryGetValue((int) text[index], out pixelFontCharacter))
        {
          num1 += (float) pixelFontCharacter.XAdvance;
          int num2;
          if (index < length - 1 && pixelFontCharacter.Kerning.TryGetValue((int) text[index + 1], out num2))
            num1 += (float) num2;
        }
      }
      return num1;
    }

    public float HeightOf(string text)
    {
      if (string.IsNullOrEmpty(text))
        return 0.0f;
      int num = 1;
      if (text.IndexOf('\n') >= 0)
      {
        for (int index = 0; index < text.Length; ++index)
        {
          if (text[index] == '\n')
            ++num;
        }
      }
      return (float) (num * this.LineHeight);
    }

    public void Draw(
      char character,
      Vector2 position,
      Vector2 justify,
      Vector2 scale,
      Color color)
    {
      if (char.IsWhiteSpace(character))
        return;
      PixelFontCharacter pixelFontCharacter = (PixelFontCharacter) null;
      if (!this.Characters.TryGetValue((int) character, out pixelFontCharacter))
        return;
      Vector2 vector2_1 = this.Measure(character);
      Vector2 vector2_2;
      ((Vector2) ref vector2_2).\u002Ector((float) (vector2_1.X * justify.X), (float) (vector2_1.Y * justify.Y));
      Vector2 val = Vector2.op_Addition(position, Vector2.op_Multiply(Vector2.op_Subtraction(new Vector2((float) pixelFontCharacter.XOffset, (float) pixelFontCharacter.YOffset), vector2_2), scale));
      pixelFontCharacter.Texture.Draw(val.Floor(), Vector2.get_Zero(), color, scale);
    }

    public void Draw(
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
      if (string.IsNullOrEmpty(text))
        return;
      Vector2 zero = Vector2.get_Zero();
      float num1 = justify.X != 0.0 ? this.WidthToNextLine(text, 0) : 0.0f;
      Vector2 vector2;
      ((Vector2) ref vector2).\u002Ector(num1 * (float) justify.X, this.HeightOf(text) * (float) justify.Y);
      for (int index = 0; index < text.Length; ++index)
      {
        if (text[index] == '\n')
        {
          zero.X = (__Null) 0.0;
          ref __Null local = ref zero.Y;
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          ^(float&) ref local = ^(float&) ref local + (float) this.LineHeight;
          if (justify.X != 0.0)
            vector2.X = (__Null) ((double) this.WidthToNextLine(text, index + 1) * justify.X);
        }
        else
        {
          PixelFontCharacter pixelFontCharacter = (PixelFontCharacter) null;
          if (this.Characters.TryGetValue((int) text[index], out pixelFontCharacter))
          {
            Vector2 position1 = Vector2.op_Addition(position, Vector2.op_Multiply(Vector2.op_Subtraction(Vector2.op_Addition(zero, new Vector2((float) pixelFontCharacter.XOffset, (float) pixelFontCharacter.YOffset)), vector2), scale));
            if ((double) stroke > 0.0 && !this.Outline)
            {
              if ((double) edgeDepth > 0.0)
              {
                pixelFontCharacter.Texture.Draw(Vector2.op_Addition(position1, new Vector2(0.0f, -stroke)), Vector2.get_Zero(), strokeColor, scale);
                for (float num2 = -stroke; (double) num2 < (double) edgeDepth + (double) stroke; num2 += stroke)
                {
                  pixelFontCharacter.Texture.Draw(Vector2.op_Addition(position1, new Vector2(-stroke, num2)), Vector2.get_Zero(), strokeColor, scale);
                  pixelFontCharacter.Texture.Draw(Vector2.op_Addition(position1, new Vector2(stroke, num2)), Vector2.get_Zero(), strokeColor, scale);
                }
                pixelFontCharacter.Texture.Draw(Vector2.op_Addition(position1, new Vector2(-stroke, edgeDepth + stroke)), Vector2.get_Zero(), strokeColor, scale);
                pixelFontCharacter.Texture.Draw(Vector2.op_Addition(position1, new Vector2(0.0f, edgeDepth + stroke)), Vector2.get_Zero(), strokeColor, scale);
                pixelFontCharacter.Texture.Draw(Vector2.op_Addition(position1, new Vector2(stroke, edgeDepth + stroke)), Vector2.get_Zero(), strokeColor, scale);
              }
              else
              {
                pixelFontCharacter.Texture.Draw(Vector2.op_Addition(position1, Vector2.op_Multiply(new Vector2(-1f, -1f), stroke)), Vector2.get_Zero(), strokeColor, scale);
                pixelFontCharacter.Texture.Draw(Vector2.op_Addition(position1, Vector2.op_Multiply(new Vector2(0.0f, -1f), stroke)), Vector2.get_Zero(), strokeColor, scale);
                pixelFontCharacter.Texture.Draw(Vector2.op_Addition(position1, Vector2.op_Multiply(new Vector2(1f, -1f), stroke)), Vector2.get_Zero(), strokeColor, scale);
                pixelFontCharacter.Texture.Draw(Vector2.op_Addition(position1, Vector2.op_Multiply(new Vector2(-1f, 0.0f), stroke)), Vector2.get_Zero(), strokeColor, scale);
                pixelFontCharacter.Texture.Draw(Vector2.op_Addition(position1, Vector2.op_Multiply(new Vector2(1f, 0.0f), stroke)), Vector2.get_Zero(), strokeColor, scale);
                pixelFontCharacter.Texture.Draw(Vector2.op_Addition(position1, Vector2.op_Multiply(new Vector2(-1f, 1f), stroke)), Vector2.get_Zero(), strokeColor, scale);
                pixelFontCharacter.Texture.Draw(Vector2.op_Addition(position1, Vector2.op_Multiply(new Vector2(0.0f, 1f), stroke)), Vector2.get_Zero(), strokeColor, scale);
                pixelFontCharacter.Texture.Draw(Vector2.op_Addition(position1, Vector2.op_Multiply(new Vector2(1f, 1f), stroke)), Vector2.get_Zero(), strokeColor, scale);
              }
            }
            if ((double) edgeDepth > 0.0)
              pixelFontCharacter.Texture.Draw(Vector2.op_Addition(position1, Vector2.op_Multiply(Vector2.get_UnitY(), edgeDepth)), Vector2.get_Zero(), edgeColor, scale);
            pixelFontCharacter.Texture.Draw(position1, Vector2.get_Zero(), color, scale);
            ref __Null local1 = ref zero.X;
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            ^(float&) ref local1 = ^(float&) ref local1 + (float) pixelFontCharacter.XAdvance;
            int num3;
            if (index < text.Length - 1 && pixelFontCharacter.Kerning.TryGetValue((int) text[index + 1], out num3))
            {
              ref __Null local2 = ref zero.X;
              // ISSUE: cast to a reference type
              // ISSUE: explicit reference operation
              // ISSUE: cast to a reference type
              // ISSUE: explicit reference operation
              ^(float&) ref local2 = ^(float&) ref local2 + (float) num3;
            }
          }
        }
      }
    }

    public void Draw(string text, Vector2 position, Color color)
    {
      this.Draw(text, position, Vector2.get_Zero(), Vector2.get_One(), color, 0.0f, Color.get_Transparent(), 0.0f, Color.get_Transparent());
    }

    public void Draw(string text, Vector2 position, Vector2 justify, Vector2 scale, Color color)
    {
      this.Draw(text, position, justify, scale, color, 0.0f, Color.get_Transparent(), 0.0f, Color.get_Transparent());
    }

    public void DrawOutline(
      string text,
      Vector2 position,
      Vector2 justify,
      Vector2 scale,
      Color color,
      float stroke,
      Color strokeColor)
    {
      this.Draw(text, position, justify, scale, color, 0.0f, Color.get_Transparent(), stroke, strokeColor);
    }

    public void DrawEdgeOutline(
      string text,
      Vector2 position,
      Vector2 justify,
      Vector2 scale,
      Color color,
      float edgeDepth,
      Color edgeColor,
      float stroke = 0.0f,
      Color strokeColor = null)
    {
      this.Draw(text, position, justify, scale, color, edgeDepth, edgeColor, stroke, strokeColor);
    }
  }
}
