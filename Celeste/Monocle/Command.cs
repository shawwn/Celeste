// Decompiled with JetBrains decompiler
// Type: Monocle.Command
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;

namespace Monocle
{
  public class Command : Attribute
  {
    public string Name;
    public string Help;

    public Command(string name, string help)
    {
      this.Name = name;
      this.Help = help;
    }
  }
}
