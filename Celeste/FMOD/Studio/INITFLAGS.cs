// Decompiled with JetBrains decompiler
// Type: FMOD.Studio.INITFLAGS
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;

namespace FMOD.Studio
{
  [Flags]
  public enum INITFLAGS : uint
  {
    NORMAL = 0,
    LIVEUPDATE = 1,
    ALLOW_MISSING_PLUGINS = 2,
    SYNCHRONOUS_UPDATE = 4,
    DEFERRED_CALLBACKS = 8,
    LOAD_FROM_UPDATE = 16, // 0x00000010
  }
}
