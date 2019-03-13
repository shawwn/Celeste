// Decompiled with JetBrains decompiler
// Type: Celeste.CoreModeListener
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Monocle;
using System;

namespace Celeste
{
  [Tracked(false)]
  public class CoreModeListener : Component
  {
    public Action<Session.CoreModes> OnChange;

    public CoreModeListener(Action<Session.CoreModes> onChange)
      : base(false, false)
    {
      this.OnChange = onChange;
    }
  }
}
