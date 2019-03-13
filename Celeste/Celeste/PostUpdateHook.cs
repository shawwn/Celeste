// Decompiled with JetBrains decompiler
// Type: Celeste.PostUpdateHook
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Monocle;
using System;

namespace Celeste
{
  [Tracked(false)]
  public class PostUpdateHook : Component
  {
    public Action OnPostUpdate;

    public PostUpdateHook(Action onPostUpdate)
      : base(false, false)
    {
      this.OnPostUpdate = onPostUpdate;
    }
  }
}
