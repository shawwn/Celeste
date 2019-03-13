// Decompiled with JetBrains decompiler
// Type: Monocle.PixelFont
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Monocle
{
  public class PixelFont
  {
    public List<PixelFontSize> Sizes = new List<PixelFontSize>();
    private List<VirtualTexture> managedTextures = new List<VirtualTexture>();
    public string Face;

    public PixelFont(string face)
    {
      this.Face = face;
    }

    public PixelFontSize AddFontSize(string path, Atlas atlas = null, bool outline = false)
    {
      XmlElement data = Calc.LoadXML(path)["font"];
      return this.AddFontSize(path, data, atlas, outline);
    }

    public PixelFontSize AddFontSize(
      string path,
      XmlElement data,
      Atlas atlas = null,
      bool outline = false)
    {
      float num1 = data["info"].AttrFloat("size");
      foreach (PixelFontSize size in this.Sizes)
      {
        if ((double) size.Size == (double) num1)
          return size;
      }
      List<MTexture> mtextureList = new List<MTexture>();
      foreach (XmlElement xml in (XmlNode) data["pages"])
      {
        string str = xml.Attr("file");
        string withoutExtension = Path.GetFileNameWithoutExtension(str);
        if (atlas != null && atlas.Has(withoutExtension))
        {
          mtextureList.Add(atlas[withoutExtension]);
        }
        else
        {
          VirtualTexture texture = VirtualContent.CreateTexture(Path.Combine(Path.GetDirectoryName(path).Substring(Engine.ContentDirectory.Length + 1), str));
          mtextureList.Add(new MTexture(texture));
          this.managedTextures.Add(texture);
        }
      }
      PixelFontSize pixelFontSize = new PixelFontSize()
      {
        Textures = mtextureList,
        Characters = new Dictionary<int, PixelFontCharacter>(),
        LineHeight = data["common"].AttrInt("lineHeight"),
        Size = num1,
        Outline = outline
      };
      foreach (XmlElement xml in (XmlNode) data["chars"])
      {
        int num2 = xml.AttrInt("id");
        int index = xml.AttrInt("page", 0);
        pixelFontSize.Characters.Add(num2, new PixelFontCharacter(num2, mtextureList[index], xml));
      }
      if (data["kernings"] != null)
      {
        foreach (XmlElement xml in (XmlNode) data["kernings"])
        {
          int key1 = xml.AttrInt("first");
          int key2 = xml.AttrInt("second");
          int num2 = xml.AttrInt("amount");
          PixelFontCharacter pixelFontCharacter = (PixelFontCharacter) null;
          if (pixelFontSize.Characters.TryGetValue(key1, out pixelFontCharacter))
            pixelFontCharacter.Kerning.Add(key2, num2);
        }
      }
      this.Sizes.Add(pixelFontSize);
      this.Sizes.Sort((Comparison<PixelFontSize>) ((a, b) => Math.Sign(a.Size - b.Size)));
      return pixelFontSize;
    }

    public PixelFontSize Get(float size)
    {
      int index1 = 0;
      for (int index2 = this.Sizes.Count - 1; index1 < index2; ++index1)
      {
        if ((double) this.Sizes[index1].Size >= (double) size)
          return this.Sizes[index1];
      }
      return this.Sizes[this.Sizes.Count - 1];
    }

    public void Draw(
      float baseSize,
      char character,
      Vector2 position,
      Vector2 justify,
      Vector2 scale,
      Color color)
    {
      PixelFontSize pixelFontSize = this.Get(baseSize * Math.Max(scale.X, scale.Y));
      scale *= baseSize / pixelFontSize.Size;
      pixelFontSize.Draw(character, position, justify, scale, color);
    }

    public void Draw(
      float baseSize,
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
      PixelFontSize pixelFontSize = this.Get(baseSize * Math.Max(scale.X, scale.Y));
      scale *= baseSize / pixelFontSize.Size;
      pixelFontSize.Draw(text, position, justify, scale, color, edgeDepth, edgeColor, stroke, strokeColor);
    }

    public void Draw(float baseSize, string text, Vector2 position, Color color)
    {
      Vector2 one = Vector2.One;
      PixelFontSize pixelFontSize = this.Get(baseSize * Math.Max(one.X, one.Y));
      Vector2 vector2 = one * (baseSize / pixelFontSize.Size);
      pixelFontSize.Draw(text, position, Vector2.Zero, Vector2.One, color, 0.0f, Color.Transparent, 0.0f, Color.Transparent);
    }

    public void Draw(
      float baseSize,
      string text,
      Vector2 position,
      Vector2 justify,
      Vector2 scale,
      Color color)
    {
      PixelFontSize pixelFontSize = this.Get(baseSize * Math.Max(scale.X, scale.Y));
      scale *= baseSize / pixelFontSize.Size;
      pixelFontSize.Draw(text, position, justify, scale, color, 0.0f, Color.Transparent, 0.0f, Color.Transparent);
    }

    public void DrawOutline(
      float baseSize,
      string text,
      Vector2 position,
      Vector2 justify,
      Vector2 scale,
      Color color,
      float stroke,
      Color strokeColor)
    {
      PixelFontSize pixelFontSize = this.Get(baseSize * Math.Max(scale.X, scale.Y));
      scale *= baseSize / pixelFontSize.Size;
      pixelFontSize.Draw(text, position, justify, scale, color, 0.0f, Color.Transparent, stroke, strokeColor);
    }

    public void DrawEdgeOutline(
      float baseSize,
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
      PixelFontSize pixelFontSize = this.Get(baseSize * Math.Max(scale.X, scale.Y));
      scale *= baseSize / pixelFontSize.Size;
      pixelFontSize.Draw(text, position, justify, scale, color, edgeDepth, edgeColor, stroke, strokeColor);
    }

    public void Dispose()
    {
      foreach (VirtualAsset managedTexture in this.managedTextures)
        managedTexture.Dispose();
      this.Sizes.Clear();
    }
  }
}

