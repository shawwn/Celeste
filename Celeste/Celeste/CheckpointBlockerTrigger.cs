﻿// Decompiled with JetBrains decompiler
// Type: Celeste.CheckpointBlockerTrigger
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  [Tracked(false)]
  public class CheckpointBlockerTrigger : Trigger
  {
    public CheckpointBlockerTrigger(EntityData data, Vector2 offset)
      : base(data, offset)
    {
    }
  }
}
