// Decompiled with JetBrains decompiler
// Type: Monocle.Atlas
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Monocle
{
  public class Atlas
  {
    public List<VirtualTexture> Sources;
    private Dictionary<string, MTexture> textures = new Dictionary<string, MTexture>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private Dictionary<string, List<MTexture>> orderedTexturesCache = new Dictionary<string, List<MTexture>>();
    private Dictionary<string, string> links = new Dictionary<string, string>();

    public static Atlas FromAtlas(string path, Atlas.AtlasDataFormat format)
    {
      Atlas atlas = new Atlas();
      atlas.Sources = new List<VirtualTexture>();
      Atlas.ReadAtlasData(atlas, path, format);
      return atlas;
    }

    private static void ReadAtlasData(Atlas atlas, string path, Atlas.AtlasDataFormat format)
    {
      switch (format)
      {
        case Atlas.AtlasDataFormat.TexturePacker_Sparrow:
          XmlElement xml1 = Calc.LoadContentXML(path)["TextureAtlas"];
          string path2_1 = xml1.Attr("imagePath", "");
          VirtualTexture texture1 = VirtualContent.CreateTexture(Path.Combine(Path.GetDirectoryName(path), path2_1));
          MTexture parent1 = new MTexture(texture1);
          atlas.Sources.Add(texture1);
          IEnumerator enumerator1 = xml1.GetElementsByTagName("SubTexture").GetEnumerator();
          try
          {
            while (enumerator1.MoveNext())
            {
              XmlElement current = (XmlElement) enumerator1.Current;
              string str = current.Attr("name");
              Rectangle clipRect = current.Rect();
              atlas.textures[str] = !current.HasAttr("frameX") ? new MTexture(parent1, str, clipRect) : new MTexture(parent1, str, clipRect, new Vector2((float) -current.AttrInt("frameX"), (float) -current.AttrInt("frameY")), current.AttrInt("frameWidth"), current.AttrInt("frameHeight"));
            }
            break;
          }
          finally
          {
            if (enumerator1 is IDisposable disposable)
              disposable.Dispose();
          }
        case Atlas.AtlasDataFormat.CrunchXml:
          IEnumerator enumerator2 = Calc.LoadContentXML(path)[nameof (atlas)].GetEnumerator();
          try
          {
            while (enumerator2.MoveNext())
            {
              XmlElement current = (XmlElement) enumerator2.Current;
              string str1 = current.Attr("n", "");
              VirtualTexture texture2 = VirtualContent.CreateTexture(Path.Combine(Path.GetDirectoryName(path), str1 + ".png"));
              MTexture parent2 = new MTexture(texture2);
              atlas.Sources.Add(texture2);
              foreach (XmlElement xml2 in (XmlNode) current)
              {
                string str2 = xml2.Attr("n");
                Rectangle clipRect = new Rectangle(xml2.AttrInt("x"), xml2.AttrInt("y"), xml2.AttrInt("w"), xml2.AttrInt("h"));
                atlas.textures[str2] = !xml2.HasAttr("fx") ? new MTexture(parent2, str2, clipRect) : new MTexture(parent2, str2, clipRect, new Vector2((float) -xml2.AttrInt("fx"), (float) -xml2.AttrInt("fy")), xml2.AttrInt("fw"), xml2.AttrInt("fh"));
              }
            }
            break;
          }
          finally
          {
            if (enumerator2 is IDisposable disposable)
              disposable.Dispose();
          }
        case Atlas.AtlasDataFormat.CrunchBinary:
          using (FileStream input = File.OpenRead(Path.Combine(Engine.ContentDirectory, path)))
          {
            BinaryReader stream = new BinaryReader((Stream) input);
            short num1 = stream.ReadInt16();
            for (int index1 = 0; index1 < (int) num1; ++index1)
            {
              string str3 = stream.ReadNullTerminatedString();
              VirtualTexture texture3 = VirtualContent.CreateTexture(Path.Combine(Path.GetDirectoryName(path), str3 + ".png"));
              atlas.Sources.Add(texture3);
              MTexture parent3 = new MTexture(texture3);
              short num2 = stream.ReadInt16();
              for (int index2 = 0; index2 < (int) num2; ++index2)
              {
                string str4 = stream.ReadNullTerminatedString();
                short x = stream.ReadInt16();
                short y = stream.ReadInt16();
                short width1 = stream.ReadInt16();
                short height1 = stream.ReadInt16();
                short num3 = stream.ReadInt16();
                short num4 = stream.ReadInt16();
                short width2 = stream.ReadInt16();
                short height2 = stream.ReadInt16();
                atlas.textures[str4] = new MTexture(parent3, str4, new Rectangle((int) x, (int) y, (int) width1, (int) height1), new Vector2((float) -num3, (float) -num4), (int) width2, (int) height2);
              }
            }
            break;
          }
        case Atlas.AtlasDataFormat.CrunchXmlOrBinary:
          if (File.Exists(Path.Combine(Engine.ContentDirectory, path + ".bin")))
          {
            Atlas.ReadAtlasData(atlas, path + ".bin", Atlas.AtlasDataFormat.CrunchBinary);
            break;
          }
          Atlas.ReadAtlasData(atlas, path + ".xml", Atlas.AtlasDataFormat.CrunchXml);
          break;
        case Atlas.AtlasDataFormat.CrunchBinaryNoAtlas:
          using (FileStream input = File.OpenRead(Path.Combine(Engine.ContentDirectory, path + ".bin")))
          {
            BinaryReader stream = new BinaryReader((Stream) input);
            short num5 = stream.ReadInt16();
            for (int index3 = 0; index3 < (int) num5; ++index3)
            {
              string path2_2 = stream.ReadNullTerminatedString();
              string path1 = Path.Combine(Path.GetDirectoryName(path), path2_2);
              short num6 = stream.ReadInt16();
              for (int index4 = 0; index4 < (int) num6; ++index4)
              {
                string key = stream.ReadNullTerminatedString();
                int num7 = (int) stream.ReadInt16();
                int num8 = (int) stream.ReadInt16();
                int num9 = (int) stream.ReadInt16();
                int num10 = (int) stream.ReadInt16();
                short num11 = stream.ReadInt16();
                short num12 = stream.ReadInt16();
                short frameWidth = stream.ReadInt16();
                short frameHeight = stream.ReadInt16();
                VirtualTexture texture4 = VirtualContent.CreateTexture(Path.Combine(path1, key + ".png"));
                atlas.Sources.Add(texture4);
                atlas.textures[key] = new MTexture(texture4, new Vector2((float) -num11, (float) -num12), (int) frameWidth, (int) frameHeight);
              }
            }
            break;
          }
        case Atlas.AtlasDataFormat.Packer:
          using (FileStream input = File.OpenRead(Path.Combine(Engine.ContentDirectory, path + ".meta")))
          {
            BinaryReader binaryReader = new BinaryReader((Stream) input);
            binaryReader.ReadInt32();
            binaryReader.ReadString();
            binaryReader.ReadInt32();
            short num13 = binaryReader.ReadInt16();
            for (int index5 = 0; index5 < (int) num13; ++index5)
            {
              string str5 = binaryReader.ReadString();
              VirtualTexture texture5 = VirtualContent.CreateTexture(Path.Combine(Path.GetDirectoryName(path), str5 + ".data"));
              atlas.Sources.Add(texture5);
              MTexture parent4 = new MTexture(texture5);
              short num14 = binaryReader.ReadInt16();
              for (int index6 = 0; index6 < (int) num14; ++index6)
              {
                string str6 = binaryReader.ReadString().Replace('\\', '/');
                short x = binaryReader.ReadInt16();
                short y = binaryReader.ReadInt16();
                short width3 = binaryReader.ReadInt16();
                short height3 = binaryReader.ReadInt16();
                short num15 = binaryReader.ReadInt16();
                short num16 = binaryReader.ReadInt16();
                short width4 = binaryReader.ReadInt16();
                short height4 = binaryReader.ReadInt16();
                atlas.textures[str6] = new MTexture(parent4, str6, new Rectangle((int) x, (int) y, (int) width3, (int) height3), new Vector2((float) -num15, (float) -num16), (int) width4, (int) height4);
              }
            }
            if (input.Position >= input.Length || !(binaryReader.ReadString() == "LINKS"))
              break;
            short num17 = binaryReader.ReadInt16();
            for (int index = 0; index < (int) num17; ++index)
            {
              string key = binaryReader.ReadString();
              string str = binaryReader.ReadString();
              atlas.links.Add(key, str);
            }
            break;
          }
        case Atlas.AtlasDataFormat.PackerNoAtlas:
          using (FileStream input = File.OpenRead(Path.Combine(Engine.ContentDirectory, path + ".meta")))
          {
            BinaryReader binaryReader = new BinaryReader((Stream) input);
            binaryReader.ReadInt32();
            binaryReader.ReadString();
            binaryReader.ReadInt32();
            short num18 = binaryReader.ReadInt16();
            for (int index7 = 0; index7 < (int) num18; ++index7)
            {
              string path2_3 = binaryReader.ReadString();
              string path1 = Path.Combine(Path.GetDirectoryName(path), path2_3);
              short num19 = binaryReader.ReadInt16();
              for (int index8 = 0; index8 < (int) num19; ++index8)
              {
                string key = binaryReader.ReadString().Replace('\\', '/');
                int num20 = (int) binaryReader.ReadInt16();
                int num21 = (int) binaryReader.ReadInt16();
                int num22 = (int) binaryReader.ReadInt16();
                int num23 = (int) binaryReader.ReadInt16();
                short num24 = binaryReader.ReadInt16();
                short num25 = binaryReader.ReadInt16();
                short frameWidth = binaryReader.ReadInt16();
                short frameHeight = binaryReader.ReadInt16();
                VirtualTexture texture6 = VirtualContent.CreateTexture(Path.Combine(path1, key + ".data"));
                atlas.Sources.Add(texture6);
                atlas.textures[key] = new MTexture(texture6, new Vector2((float) -num24, (float) -num25), (int) frameWidth, (int) frameHeight);
                atlas.textures[key].AtlasPath = key;
              }
            }
            if (input.Position >= input.Length || !(binaryReader.ReadString() == "LINKS"))
              break;
            short num26 = binaryReader.ReadInt16();
            for (int index = 0; index < (int) num26; ++index)
            {
              string key = binaryReader.ReadString();
              string str = binaryReader.ReadString();
              atlas.links.Add(key, str);
            }
            break;
          }
        default:
          throw new NotImplementedException();
      }
    }

    public static Atlas FromMultiAtlas(
      string rootPath,
      string[] dataPath,
      Atlas.AtlasDataFormat format)
    {
      Atlas atlas = new Atlas();
      atlas.Sources = new List<VirtualTexture>();
      for (int index = 0; index < dataPath.Length; ++index)
        Atlas.ReadAtlasData(atlas, Path.Combine(rootPath, dataPath[index]), format);
      return atlas;
    }

    public static Atlas FromMultiAtlas(
      string rootPath,
      string filename,
      Atlas.AtlasDataFormat format)
    {
      Atlas atlas = new Atlas();
      atlas.Sources = new List<VirtualTexture>();
      int num = 0;
      while (true)
      {
        string str = Path.Combine(rootPath, filename + num.ToString() + ".xml");
        if (File.Exists(Path.Combine(Engine.ContentDirectory, str)))
        {
          Atlas.ReadAtlasData(atlas, str, format);
          ++num;
        }
        else
          break;
      }
      return atlas;
    }

    public static Atlas FromDirectory(string path)
    {
      Atlas atlas = new Atlas();
      atlas.Sources = new List<VirtualTexture>();
      string contentDirectory = Engine.ContentDirectory;
      int length1 = contentDirectory.Length;
      string path1 = Path.Combine(contentDirectory, path);
      int length2 = path1.Length;
      foreach (string file in Directory.GetFiles(path1, "*", SearchOption.AllDirectories))
      {
        string extension = Path.GetExtension(file);
        if (!(extension != ".png") || !(extension != ".xnb"))
        {
          VirtualTexture texture = VirtualContent.CreateTexture(file.Substring(length1 + 1));
          atlas.Sources.Add(texture);
          string str = file.Substring(length2 + 1);
          string key = str.Substring(0, str.Length - 4).Replace('\\', '/');
          atlas.textures.Add(key, new MTexture(texture));
        }
      }
      return atlas;
    }

    public MTexture this[string id]
    {
      get => this.textures[id];
      set => this.textures[id] = value;
    }

    public bool Has(string id) => this.textures.ContainsKey(id);

    public MTexture GetOrDefault(string id, MTexture defaultTexture) => string.IsNullOrEmpty(id) || !this.Has(id) ? defaultTexture : this.textures[id];

    public List<MTexture> GetAtlasSubtextures(string key)
    {
      List<MTexture> atlasSubtextures;
      if (!this.orderedTexturesCache.TryGetValue(key, out atlasSubtextures))
      {
        atlasSubtextures = new List<MTexture>();
        int index = 0;
        while (true)
        {
          MTexture subtextureFromAtlasAt = this.GetAtlasSubtextureFromAtlasAt(key, index);
          if (subtextureFromAtlasAt != null)
          {
            atlasSubtextures.Add(subtextureFromAtlasAt);
            ++index;
          }
          else
            break;
        }
        this.orderedTexturesCache.Add(key, atlasSubtextures);
      }
      return atlasSubtextures;
    }

    private MTexture GetAtlasSubtextureFromCacheAt(string key, int index) => this.orderedTexturesCache[key][index];

    private MTexture GetAtlasSubtextureFromAtlasAt(string key, int index)
    {
      if (index == 0 && this.textures.ContainsKey(key))
        return this.textures[key];
      string str = index.ToString();
      for (int length = str.Length; str.Length < length + 6; str = "0" + str)
      {
        MTexture subtextureFromAtlasAt;
        if (this.textures.TryGetValue(key + str, out subtextureFromAtlasAt))
          return subtextureFromAtlasAt;
      }
      return (MTexture) null;
    }

    public MTexture GetAtlasSubtexturesAt(string key, int index)
    {
      List<MTexture> mtextureList;
      return this.orderedTexturesCache.TryGetValue(key, out mtextureList) ? mtextureList[index] : this.GetAtlasSubtextureFromAtlasAt(key, index);
    }

    public MTexture GetLinkedTexture(string key)
    {
      string key1;
      MTexture mtexture;
      return key != null && this.links.TryGetValue(key, out key1) && this.textures.TryGetValue(key1, out mtexture) ? mtexture : (MTexture) null;
    }

    public void Dispose()
    {
      foreach (VirtualAsset source in this.Sources)
        source.Dispose();
      this.Sources.Clear();
      this.textures.Clear();
    }

    public enum AtlasDataFormat
    {
      TexturePacker_Sparrow,
      CrunchXml,
      CrunchBinary,
      CrunchXmlOrBinary,
      CrunchBinaryNoAtlas,
      Packer,
      PackerNoAtlas,
    }
  }
}
