// Decompiled with JetBrains decompiler
// Type: Celeste.MEP
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;
using System.Xml.Serialization;

namespace Celeste
{
  [Serializable]
  public class MEP
  {
    [XmlAttribute]
    public string Key;
    [XmlAttribute]
    public float Value;

    public MEP()
    {
    }

    public MEP(string key, float value)
    {
      this.Key = key;
      this.Value = value;
    }
  }
}
