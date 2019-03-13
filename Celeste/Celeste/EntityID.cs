// Decompiled with JetBrains decompiler
// Type: Celeste.EntityID
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;
using System.Xml.Serialization;

namespace Celeste
{
  [Serializable]
  public struct EntityID
  {
    public static readonly EntityID None = new EntityID("null", -1);
    [XmlIgnore]
    public string Level;
    [XmlIgnore]
    public int ID;

    [XmlAttribute]
    public string Key
    {
      get
      {
        return this.Level + ":" + (object) this.ID;
      }
      set
      {
        string[] strArray = value.Split(':');
        this.Level = strArray[0];
        this.ID = int.Parse(strArray[1]);
      }
    }

    public EntityID(string level, int entityID)
    {
      this.Level = level;
      this.ID = entityID;
    }

    public override string ToString()
    {
      return this.Key;
    }

    public override int GetHashCode()
    {
      return this.Level.GetHashCode() ^ this.ID;
    }
  }
}
