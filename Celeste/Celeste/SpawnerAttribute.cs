// Decompiled with JetBrains decompiler
// Type: Celeste.SpawnerAttribute
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;

namespace Celeste
{
  public class SpawnerAttribute : Attribute
  {
    public string Name;

    public SpawnerAttribute(string name = null)
    {
      this.Name = name;
    }
  }
}
