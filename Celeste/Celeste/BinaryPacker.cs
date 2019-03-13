// Decompiled with JetBrains decompiler
// Type: Celeste.BinaryPacker
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Celeste
{
  public static class BinaryPacker
  {
    public static readonly HashSet<string> IgnoreAttributes = new HashSet<string>()
    {
      "_eid"
    };
    public static string InnerTextAttributeName = "innerText";
    public static string OutputFileExtension = ".bin";
    private static Dictionary<string, short> stringValue = new Dictionary<string, short>();
    private static string[] stringLookup;
    private static short stringCounter;

    public static void ToBinary(string filename, string outdir = null)
    {
      string extension = Path.GetExtension(filename);
      if (outdir != null)
        Path.Combine(outdir + Path.GetFileName(filename));
      filename.Replace(extension, BinaryPacker.OutputFileExtension);
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.Load(filename);
      XmlElement rootElement = (XmlElement) null;
      foreach (object childNode in xmlDocument.ChildNodes)
      {
        if (childNode is XmlElement)
        {
          rootElement = childNode as XmlElement;
          break;
        }
      }
      BinaryPacker.ToBinary(rootElement, outdir);
    }

    public static void ToBinary(XmlElement rootElement, string outfilename)
    {
      BinaryPacker.stringValue.Clear();
      BinaryPacker.stringCounter = (short) 0;
      BinaryPacker.CreateLookupTable(rootElement);
      BinaryPacker.AddLookupValue(BinaryPacker.InnerTextAttributeName);
      using (FileStream fileStream = new FileStream(outfilename, FileMode.Create))
      {
        BinaryWriter writer = new BinaryWriter((Stream) fileStream);
        writer.Write("CELESTE MAP");
        writer.Write(Path.GetFileNameWithoutExtension(outfilename));
        writer.Write((short) BinaryPacker.stringValue.Count);
        foreach (KeyValuePair<string, short> keyValuePair in BinaryPacker.stringValue)
          writer.Write(keyValuePair.Key);
        BinaryPacker.WriteElement(writer, rootElement);
        writer.Flush();
      }
    }

    private static void CreateLookupTable(XmlElement element)
    {
      BinaryPacker.AddLookupValue(element.Name);
      foreach (XmlAttribute attribute in (XmlNamedNodeMap) element.Attributes)
      {
        if (!BinaryPacker.IgnoreAttributes.Contains(attribute.Name))
        {
          BinaryPacker.AddLookupValue(attribute.Name);
          byte type;
          object result;
          if (BinaryPacker.ParseValue(attribute.Value, out type, out result) && type == (byte) 5)
            BinaryPacker.AddLookupValue(attribute.Value);
        }
      }
      foreach (object childNode in element.ChildNodes)
      {
        if (childNode is XmlElement)
          BinaryPacker.CreateLookupTable(childNode as XmlElement);
      }
    }

    private static void AddLookupValue(string name)
    {
      if (BinaryPacker.stringValue.ContainsKey(name))
        return;
      BinaryPacker.stringValue.Add(name, BinaryPacker.stringCounter);
      ++BinaryPacker.stringCounter;
    }

    private static void WriteElement(BinaryWriter writer, XmlElement element)
    {
      int num1 = 0;
      foreach (object childNode in element.ChildNodes)
      {
        if (childNode is XmlElement)
          ++num1;
      }
      int num2 = 0;
      foreach (XmlAttribute attribute in (XmlNamedNodeMap) element.Attributes)
      {
        if (!BinaryPacker.IgnoreAttributes.Contains(attribute.Name))
          ++num2;
      }
      if (element.InnerText.Length > 0 && num1 == 0)
        ++num2;
      writer.Write(BinaryPacker.stringValue[element.Name]);
      writer.Write((byte) num2);
      foreach (XmlAttribute attribute in (XmlNamedNodeMap) element.Attributes)
      {
        if (!BinaryPacker.IgnoreAttributes.Contains(attribute.Name))
        {
          byte type;
          object result;
          BinaryPacker.ParseValue(attribute.Value, out type, out result);
          writer.Write(BinaryPacker.stringValue[attribute.Name]);
          writer.Write(type);
          switch (type)
          {
            case 0:
              writer.Write((bool) result);
              continue;
            case 1:
              writer.Write((byte) result);
              continue;
            case 2:
              writer.Write((short) result);
              continue;
            case 3:
              writer.Write((int) result);
              continue;
            case 4:
              writer.Write((float) result);
              continue;
            case 5:
              writer.Write(BinaryPacker.stringValue[(string) result]);
              continue;
            default:
              continue;
          }
        }
      }
      if (element.InnerText.Length > 0 && num1 == 0)
      {
        writer.Write(BinaryPacker.stringValue[BinaryPacker.InnerTextAttributeName]);
        if (element.Name == "solids" || element.Name == "bg")
        {
          byte[] buffer = RunLengthEncoding.Encode(element.InnerText);
          writer.Write((byte) 7);
          writer.Write((short) buffer.Length);
          writer.Write(buffer);
        }
        else
        {
          writer.Write((byte) 6);
          writer.Write(element.InnerText);
        }
      }
      writer.Write((short) num1);
      foreach (object childNode in element.ChildNodes)
      {
        if (childNode is XmlElement)
          BinaryPacker.WriteElement(writer, childNode as XmlElement);
      }
    }

    private static bool ParseValue(string value, out byte type, out object result)
    {
      bool result1;
      if (bool.TryParse(value, out result1))
      {
        type = (byte) 0;
        result = (object) result1;
      }
      else
      {
        byte result2;
        if (byte.TryParse(value, out result2))
        {
          type = (byte) 1;
          result = (object) result2;
        }
        else
        {
          short result3;
          if (short.TryParse(value, out result3))
          {
            type = (byte) 2;
            result = (object) result3;
          }
          else
          {
            int result4;
            if (int.TryParse(value, out result4))
            {
              type = (byte) 3;
              result = (object) result4;
            }
            else
            {
              float result5;
              if (float.TryParse(value, NumberStyles.Integer | NumberStyles.AllowDecimalPoint, (IFormatProvider) CultureInfo.InvariantCulture, out result5))
              {
                type = (byte) 4;
                result = (object) result5;
              }
              else
              {
                type = (byte) 5;
                result = (object) value;
              }
            }
          }
        }
      }
      return true;
    }

    public static BinaryPacker.Element FromBinary(string filename)
    {
      BinaryPacker.Element element;
      using (FileStream fileStream = File.OpenRead(filename))
      {
        BinaryReader reader = new BinaryReader((Stream) fileStream);
        reader.ReadString();
        string str = reader.ReadString();
        short num = reader.ReadInt16();
        BinaryPacker.stringLookup = new string[(int) num];
        for (int index = 0; index < (int) num; ++index)
          BinaryPacker.stringLookup[index] = reader.ReadString();
        element = BinaryPacker.ReadElement(reader);
        element.Package = str;
      }
      return element;
    }

    private static BinaryPacker.Element ReadElement(BinaryReader reader)
    {
      BinaryPacker.Element element = new BinaryPacker.Element();
      element.Name = BinaryPacker.stringLookup[(int) reader.ReadInt16()];
      byte num1 = reader.ReadByte();
      if (num1 > (byte) 0)
        element.Attributes = new Dictionary<string, object>();
      for (int index = 0; index < (int) num1; ++index)
      {
        string key = BinaryPacker.stringLookup[(int) reader.ReadInt16()];
        byte num2 = reader.ReadByte();
        object obj = (object) null;
        switch (num2)
        {
          case 0:
            obj = (object) reader.ReadBoolean();
            break;
          case 1:
            obj = (object) Convert.ToInt32(reader.ReadByte());
            break;
          case 2:
            obj = (object) Convert.ToInt32(reader.ReadInt16());
            break;
          case 3:
            obj = (object) reader.ReadInt32();
            break;
          case 4:
            obj = (object) reader.ReadSingle();
            break;
          case 5:
            obj = (object) BinaryPacker.stringLookup[(int) reader.ReadInt16()];
            break;
          case 6:
            obj = (object) reader.ReadString();
            break;
          case 7:
            short num3 = reader.ReadInt16();
            obj = (object) RunLengthEncoding.Decode(reader.ReadBytes((int) num3));
            break;
        }
        element.Attributes.Add(key, obj);
      }
      short num4 = reader.ReadInt16();
      if (num4 > (short) 0)
        element.Children = new List<BinaryPacker.Element>();
      for (int index = 0; index < (int) num4; ++index)
        element.Children.Add(BinaryPacker.ReadElement(reader));
      return element;
    }

    public class Element
    {
      public string Package;
      public string Name;
      public Dictionary<string, object> Attributes;
      public List<BinaryPacker.Element> Children;

      public bool HasAttr(string name)
      {
        if (this.Attributes != null)
          return this.Attributes.ContainsKey(name);
        return false;
      }

      public string Attr(string name, string defaultValue = "")
      {
        object obj;
        if (this.Attributes == null || !this.Attributes.TryGetValue(name, out obj))
          obj = (object) defaultValue;
        return obj.ToString();
      }

      public bool AttrBool(string name, bool defaultValue = false)
      {
        object obj;
        if (this.Attributes == null || !this.Attributes.TryGetValue(name, out obj))
          obj = (object) defaultValue;
        if (obj is bool)
          return (bool) obj;
        return bool.Parse(obj.ToString());
      }

      public float AttrFloat(string name, float defaultValue = 0.0f)
      {
        object obj;
        if (this.Attributes == null || !this.Attributes.TryGetValue(name, out obj))
          obj = (object) defaultValue;
        if (obj is float)
          return (float) obj;
        return float.Parse(obj.ToString(), (IFormatProvider) CultureInfo.InvariantCulture);
      }
    }
  }
}
