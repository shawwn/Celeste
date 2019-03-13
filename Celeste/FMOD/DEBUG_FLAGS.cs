// Decompiled with JetBrains decompiler
// Type: FMOD.DEBUG_FLAGS
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;

namespace FMOD
{
  [Flags]
  public enum DEBUG_FLAGS : uint
  {
    NONE = 0,
    ERROR = 1,
    WARNING = 2,
    LOG = 4,
    TYPE_MEMORY = 256, // 0x00000100
    TYPE_FILE = 512, // 0x00000200
    TYPE_CODEC = 1024, // 0x00000400
    TYPE_TRACE = 2048, // 0x00000800
    DISPLAY_TIMESTAMPS = 65536, // 0x00010000
    DISPLAY_LINENUMBERS = 131072, // 0x00020000
    DISPLAY_THREAD = 262144, // 0x00040000
  }
}
