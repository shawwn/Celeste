// Decompiled with JetBrains decompiler
// Type: Monocle.Atlas
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-43code\bin\Celeste\Celeste.exe


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
    private Dictionary<string, MTexture> textures = new Dictionary<string, MTexture>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private Dictionary<string, List<MTexture>> orderedTexturesCache = new Dictionary<string, List<MTexture>>();
    public List<VirtualTexture> Sources;

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
              string atlasPath = current.Attr("name");
              Rectangle clipRect = current.Rect();
              atlas.textures[atlasPath] = !current.HasAttr("frameX") ? new MTexture(parent1, atlasPath, clipRect) : new MTexture(parent1, atlasPath, clipRect, new Vector2((float) -current.AttrInt("frameX"), (float) -current.AttrInt("frameY")), current.AttrInt("frameWidth"), current.AttrInt("frameHeight"));
            }
            break;
          }
          finally
          {
            (enumerator1 as IDisposable)?.Dispose();
          }
        case Atlas.AtlasDataFormat.CrunchXml:
          IEnumerator enumerator2 = Calc.LoadContentXML(path)[nameof (atlas)].GetEnumerator();
          try
          {
            while (enumerator2.MoveNext())
            {
              XmlElement current = (XmlElement) enumerator2.Current;
              string str = current.Attr("n", "");
              VirtualTexture texture2 = VirtualContent.CreateTexture(Path.Combine(Path.GetDirectoryName(path), str + ".png"));
              MTexture parent2 = new MTexture(texture2);
              atlas.Sources.Add(texture2);
              foreach (XmlElement xml2 in (XmlNode) current)
              {
                string atlasPath = xml2.Attr("n");
                Rectangle clipRect = new Rectangle(xml2.AttrInt("x"), xml2.AttrInt("y"), xml2.AttrInt("w"), xml2.AttrInt("h"));
                atlas.textures[atlasPath] = !xml2.HasAttr("fx") ? new MTexture(parent2, atlasPath, clipRect) : new MTexture(parent2, atlasPath, clipRect, new Vector2((float) -xml2.AttrInt("fx"), (float) -xml2.AttrInt("fy")), xml2.AttrInt("fw"), xml2.AttrInt("fh"));
              }
            }
            break;
          }
          finally
          {
            (enumerator2 as IDisposable)?.Dispose();
          }
        case Atlas.AtlasDataFormat.CrunchBinary:
          using (FileStream fileStream = File.OpenRead(Path.Combine(Engine.ContentDirectory, path)))
          {
            BinaryReader stream = new BinaryReader((Stream) fileStream);
            short num1 = stream.ReadInt16();
            for (int index1 = 0; index1 < (int) num1; ++index1)
            {
              string str = stream.ReadNullTerminatedString();
              VirtualTexture texture2 = VirtualContent.CreateTexture(Path.Combine(Path.GetDirectoryName(path), str + ".png"));
              atlas.Sources.Add(texture2);
              MTexture parent2 = new MTexture(texture2);
              short num2 = stream.ReadInt16();
              for (int index2 = 0; index2 < (int) num2; ++index2)
              {
                string atlasPath = stream.ReadNullTerminatedString();
                short num3 = stream.ReadInt16();
                short num4 = stream.ReadInt16();
                short num5 = stream.ReadInt16();
                short num6 = stream.ReadInt16();
                short num7 = stream.ReadInt16();
                short num8 = stream.ReadInt16();
                short num9 = stream.ReadInt16();
                short num10 = stream.ReadInt16();
                atlas.textures[atlasPath] = new MTexture(parent2, atlasPath, new Rectangle((int) num3, (int) num4, (int) num5, (int) num6), new Vector2((float) -num7, (float) -num8), (int) num9, (int) num10);
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
          using (FileStream fileStream = File.OpenRead(Path.Combine(Engine.ContentDirectory, path + ".bin")))
          {
            BinaryReader stream = new BinaryReader((Stream) fileStream);
            short num1 = stream.ReadInt16();
            for (int index1 = 0; index1 < (int) num1; ++index1)
            {
              string path2_2 = stream.ReadNullTerminatedString();
              string path1 = Path.Combine(Path.GetDirectoryName(path), path2_2);
              short num2 = stream.ReadInt16();
              for (int index2 = 0; index2 < (int) num2; ++index2)
              {
                string index3 = stream.ReadNullTerminatedString();
                stream.ReadInt16();
                stream.ReadInt16();
                stream.ReadInt16();
                stream.ReadInt16();
                short num3 = stream.ReadInt16();
                short num4 = stream.ReadInt16();
                short num5 = stream.ReadInt16();
                short num6 = stream.ReadInt16();
                VirtualTexture texture2 = VirtualContent.CreateTexture(Path.Combine(path1, index3 + ".png"));
                atlas.Sources.Add(texture2);
                atlas.textures[index3] = new MTexture(texture2, new Vector2((float) -num3, (float) -num4), (int) num5, (int) num6);
              }
            }
            break;
          }
        case Atlas.AtlasDataFormat.Packer:
          using (FileStream fileStream = File.OpenRead(Path.Combine(Engine.ContentDirectory, path + ".meta")))
          {
            BinaryReader binaryReader = new BinaryReader((Stream) fileStream);
            binaryReader.ReadInt32();
            binaryReader.ReadString();
            binaryReader.ReadInt32();
            short num1 = binaryReader.ReadInt16();
            for (int index1 = 0; index1 < (int) num1; ++index1)
            {
              string str = binaryReader.ReadString();
              VirtualTexture texture2 = VirtualContent.CreateTexture(Path.Combine(Path.GetDirectoryName(path), str + ".data"));
              atlas.Sources.Add(texture2);
              MTexture parent2 = new MTexture(texture2);
              short num2 = binaryReader.ReadInt16();
              for (int index2 = 0; index2 < (int) num2; ++index2)
              {
                string atlasPath = binaryReader.ReadString().Replace('\\', '/');
                short num3 = binaryReader.ReadInt16();
                short num4 = binaryReader.ReadInt16();
                short num5 = binaryReader.ReadInt16();
                short num6 = binaryReader.ReadInt16();
                short num7 = binaryReader.ReadInt16();
                short num8 = binaryReader.ReadInt16();
                short num9 = binaryReader.ReadInt16();
                short num10 = binaryReader.ReadInt16();
                atlas.textures[atlasPath] = new MTexture(parent2, atlasPath, new Rectangle((int) num3, (int) num4, (int) num5, (int) num6), new Vector2((float) -num7, (float) -num8), (int) num9, (int) num10);
              }
            }
            break;
          }
        case Atlas.AtlasDataFormat.PackerNoAtlas:
          using (FileStream fileStream = File.OpenRead(Path.Combine(Engine.ContentDirectory, path + ".meta")))
          {
            BinaryReader binaryReader = new BinaryReader((Stream) fileStream);
            binaryReader.ReadInt32();
            binaryReader.ReadString();
            binaryReader.ReadInt32();
            short num1 = binaryReader.ReadInt16();
            for (int index1 = 0; index1 < (int) num1; ++index1)
            {
              string path2_2 = binaryReader.ReadString();
              string path1 = Path.Combine(Path.GetDirectoryName(path), path2_2);
              short num2 = binaryReader.ReadInt16();
              for (int index2 = 0; index2 < (int) num2; ++index2)
              {
                string index3 = binaryReader.ReadString().Replace('\\', '/');
                binaryReader.ReadInt16();
                binaryReader.ReadInt16();
                binaryReader.ReadInt16();
                binaryReader.ReadInt16();
                short num3 = binaryReader.ReadInt16();
                short num4 = binaryReader.ReadInt16();
                short num5 = binaryReader.ReadInt16();
                short num6 = binaryReader.ReadInt16();
                VirtualTexture texture2 = VirtualContent.CreateTexture(Path.Combine(path1, index3 + ".data"));
                atlas.Sources.Add(texture2);
                atlas.textures[index3] = new MTexture(texture2, new Vector2((float) -num3, (float) -num4), (int) num5, (int) num6);
              }
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
      get
      {
        return this.textures[id];
      }
      set
      {
        this.textures[id] = value;
      }
    }

    public bool Has(string id)
    {
      return this.textures.ContainsKey(id);
    }

    public MTexture GetOrDefault(string id, MTexture defaultTexture)
    {
      if (string.IsNullOrEmpty(id) || !this.Has(id))
        return defaultTexture;
      return this.textures[id];
    }

    public List<MTexture> GetAtlasSubtextures(string key)
    {
      List<MTexture> mtextureList;
      if (!this.orderedTexturesCache.TryGetValue(key, out mtextureList))
      {
        mtextureList = new List<MTexture>();
        int index = 0;
        while (true)
        {
          MTexture subtextureFromAtlasAt = this.GetAtlasSubtextureFromAtlasAt(key, index);
          if (subtextureFromAtlasAt != null)
          {
            mtextureList.Add(subtextureFromAtlasAt);
            ++index;
          }
          else
            break;
        }
        this.orderedTexturesCache.Add(key, mtextureList);
      }
      return mtextureList;
    }

    private MTexture GetAtlasSubtextureFromCacheAt(string key, int index)
    {
      return this.orderedTexturesCache[key][index];
    }

    private MTexture GetAtlasSubtextureFromAtlasAt(string key, int index)
    {
      if (index == 0 && this.textures.ContainsKey(key))
        return this.textures[key];
      string str = index.ToString();
      for (int length = str.Length; str.Length < length + 6; str = "0" + str)
      {
        MTexture mtexture;
        if (this.textures.TryGetValue(key + str, out mtexture))
          return mtexture;
      }
      return (MTexture) null;
    }

    public MTexture GetAtlasSubtexturesAt(string key, int index)
    {
      List<MTexture> mtextureList;
      if (this.orderedTexturesCache.TryGetValue(key, out mtextureList))
        return mtextureList[index];
      return this.GetAtlasSubtextureFromAtlasAt(key, index);
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

