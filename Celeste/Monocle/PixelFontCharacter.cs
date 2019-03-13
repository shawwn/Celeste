// Decompiled with JetBrains decompiler
// Type: Monocle.PixelFontCharacter
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System.Collections.Generic;
using System.Xml;

namespace Monocle
{
  public class PixelFontCharacter
  {
    public Dictionary<int, int> Kerning = new Dictionary<int, int>();
    public int Character;
    public MTexture Texture;
    public int XOffset;
    public int YOffset;
    public int XAdvance;

    public PixelFontCharacter(int character, MTexture texture, XmlElement xml)
    {
      this.Character = character;
      this.Texture = texture.GetSubtexture(xml.AttrInt("x"), xml.AttrInt("y"), xml.AttrInt("width"), xml.AttrInt("height"), (MTexture) null);
      this.XOffset = xml.AttrInt("xoffset");
      this.YOffset = xml.AttrInt("yoffset");
      this.XAdvance = xml.AttrInt("xadvance");
    }
  }
}
