// Decompiled with JetBrains decompiler
// Type: FMOD.Studio.COMMANDREPLAY_FLAGS
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;

namespace FMOD.Studio
{
  [Flags]
  public enum COMMANDREPLAY_FLAGS : uint
  {
    NORMAL = 0,
    SKIP_CLEANUP = 1,
    FAST_FORWARD = 2,
  }
}
