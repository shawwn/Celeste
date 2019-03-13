// Decompiled with JetBrains decompiler
// Type: FMOD.INITFLAGS
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;

namespace FMOD
{
  [Flags]
  public enum INITFLAGS : uint
  {
    NORMAL = 0,
    STREAM_FROM_UPDATE = 1,
    MIX_FROM_UPDATE = 2,
    _3D_RIGHTHANDED = 4,
    CHANNEL_LOWPASS = 256, // 0x00000100
    CHANNEL_DISTANCEFILTER = 512, // 0x00000200
    PROFILE_ENABLE = 65536, // 0x00010000
    VOL0_BECOMES_VIRTUAL = 131072, // 0x00020000
    GEOMETRY_USECLOSEST = 262144, // 0x00040000
    PREFER_DOLBY_DOWNMIX = 524288, // 0x00080000
    THREAD_UNSAFE = 1048576, // 0x00100000
    PROFILE_METER_ALL = 2097152, // 0x00200000
    DISABLE_SRS_HIGHPASSFILTER = 4194304, // 0x00400000
  }
}
