// Decompiled with JetBrains decompiler
// Type: FMOD.Studio.SYSTEM_CALLBACK_TYPE
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;

namespace FMOD.Studio
{
  [Flags]
  public enum SYSTEM_CALLBACK_TYPE : uint
  {
    PREUPDATE = 1,
    POSTUPDATE = 2,
    BANK_UNLOAD = 4,
    ALL = 4294967295, // 0xFFFFFFFF
  }
}
