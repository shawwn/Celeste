// Decompiled with JetBrains decompiler
// Type: Monocle.SpriteBank
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;
using System.Collections.Generic;
using System.Xml;

namespace Monocle
{
  public class SpriteBank
  {
    public Atlas Atlas;
    public XmlDocument XML;
    public Dictionary<string, Monocle.SpriteData> SpriteData;

    public SpriteBank(Atlas atlas, XmlDocument xml)
    {
      this.Atlas = atlas;
      this.XML = xml;
      this.SpriteData = new Dictionary<string, Monocle.SpriteData>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      Dictionary<string, XmlElement> dictionary = new Dictionary<string, XmlElement>();
      foreach (object childNode in this.XML["Sprites"].ChildNodes)
      {
        if (childNode is XmlElement)
        {
          XmlElement xml1 = childNode as XmlElement;
          dictionary.Add(xml1.Name, xml1);
          if (this.SpriteData.ContainsKey(xml1.Name))
            throw new Exception("Duplicate sprite name in SpriteData: '" + xml1.Name + "'!");
          Monocle.SpriteData spriteData = this.SpriteData[xml1.Name] = new Monocle.SpriteData(this.Atlas);
          if (xml1.HasAttr("copy"))
            spriteData.Add(dictionary[xml1.Attr("copy")], xml1.Attr("path"));
          spriteData.Add(xml1, (string) null);
        }
      }
    }

    public SpriteBank(Atlas atlas, string xmlPath)
      : this(atlas, Calc.LoadContentXML(xmlPath))
    {
    }

    public bool Has(string id)
    {
      return this.SpriteData.ContainsKey(id);
    }

    public Sprite Create(string id)
    {
      if (this.SpriteData.ContainsKey(id))
        return this.SpriteData[id].Create();
      throw new Exception("Missing animation name in SpriteData: '" + id + "'!");
    }

    public Sprite CreateOn(Sprite sprite, string id)
    {
      if (this.SpriteData.ContainsKey(id))
        return this.SpriteData[id].CreateOn(sprite);
      throw new Exception("Missing animation name in SpriteData: '" + id + "'!");
    }
  }
}
