// Decompiled with JetBrains decompiler
// Type: Celeste.AreaKey
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;
using System.Xml.Serialization;

namespace Celeste
{
  [Serializable]
  public struct AreaKey
  {
    public static readonly AreaKey None = new AreaKey(-1, AreaMode.Normal);
    public static readonly AreaKey Default = new AreaKey(0, AreaMode.Normal);
    [XmlAttribute]
    public int ID;
    [XmlAttribute]
    public AreaMode Mode;

    public AreaKey(int id, AreaMode mode = AreaMode.Normal)
    {
      this.ID = id;
      this.Mode = mode;
    }

    public int ChapterIndex
    {
      get
      {
        if (AreaData.Areas[this.ID].Interlude)
          return -1;
        int num = 0;
        for (int index = 0; index <= this.ID; ++index)
        {
          if (!AreaData.Areas[index].Interlude)
            ++num;
        }
        return num;
      }
    }

    public static bool operator ==(AreaKey a, AreaKey b)
    {
      if (a.ID == b.ID)
        return a.Mode == b.Mode;
      return false;
    }

    public static bool operator !=(AreaKey a, AreaKey b)
    {
      if (a.ID == b.ID)
        return a.Mode != b.Mode;
      return true;
    }

    public override bool Equals(object obj)
    {
      return false;
    }

    public override int GetHashCode()
    {
      return (int) (this.ID * 3 + this.Mode);
    }

    public override string ToString()
    {
      string str = this.ID.ToString();
      if (this.Mode == AreaMode.BSide)
        str += "H";
      else if (this.Mode == AreaMode.CSide)
        str += "HH";
      return str;
    }
  }
}
