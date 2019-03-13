// Decompiled with JetBrains decompiler
// Type: Monocle.SaveLoad
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace Monocle
{
  public static class SaveLoad
  {
    public static void SerializeToFile<T>(T obj, string filepath, SaveLoad.SerializeModes mode)
    {
      using (FileStream fileStream = new FileStream(filepath, FileMode.Create))
      {
        if (mode == SaveLoad.SerializeModes.Binary)
        {
          new BinaryFormatter().Serialize((Stream) fileStream, (object) obj);
        }
        else
        {
          if (mode != SaveLoad.SerializeModes.XML)
            return;
          new XmlSerializer(typeof (T)).Serialize((Stream) fileStream, (object) obj);
        }
      }
    }

    public static bool SafeSerializeToFile<T>(T obj, string filepath, SaveLoad.SerializeModes mode)
    {
      try
      {
        SaveLoad.SerializeToFile<T>(obj, filepath, mode);
        return true;
      }
      catch
      {
        return false;
      }
    }

    public static T DeserializeFromFile<T>(string filepath, SaveLoad.SerializeModes mode)
    {
      using (FileStream fileStream = File.OpenRead(filepath))
      {
        if (mode == SaveLoad.SerializeModes.Binary)
          return (T) new BinaryFormatter().Deserialize((Stream) fileStream);
        return (T) new XmlSerializer(typeof (T)).Deserialize((Stream) fileStream);
      }
    }

    public static T SafeDeserializeFromFile<T>(
      string filepath,
      SaveLoad.SerializeModes mode,
      bool debugUnsafe = false)
    {
      if (!File.Exists(filepath))
        return default (T);
      if (debugUnsafe)
        return SaveLoad.DeserializeFromFile<T>(filepath, mode);
      try
      {
        return SaveLoad.DeserializeFromFile<T>(filepath, mode);
      }
      catch
      {
        return default (T);
      }
    }

    public static T SafeDeserializeFromFile<T>(
      string filepath,
      SaveLoad.SerializeModes mode,
      out bool loadError,
      bool debugUnsafe = false)
    {
      if (File.Exists(filepath))
      {
        if (debugUnsafe)
        {
          loadError = false;
          return SaveLoad.DeserializeFromFile<T>(filepath, mode);
        }
        try
        {
          loadError = false;
          return SaveLoad.DeserializeFromFile<T>(filepath, mode);
        }
        catch
        {
          loadError = true;
          return default (T);
        }
      }
      else
      {
        loadError = false;
        return default (T);
      }
    }

    public enum SerializeModes
    {
      Binary,
      XML,
    }
  }
}
