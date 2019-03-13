// Decompiled with JetBrains decompiler
// Type: FMOD.TIMEUNIT
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;

namespace FMOD
{
  [Flags]
  public enum TIMEUNIT : uint
  {
    MS = 1,
    PCM = 2,
    PCMBYTES = 4,
    RAWBYTES = 8,
    PCMFRACTION = 16, // 0x00000010
    MODORDER = 256, // 0x00000100
    MODROW = 512, // 0x00000200
    MODPATTERN = 1024, // 0x00000400
  }
}
