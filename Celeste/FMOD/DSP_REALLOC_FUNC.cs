﻿// Decompiled with JetBrains decompiler
// Type: FMOD.DSP_REALLOC_FUNC
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;

namespace FMOD
{
  public delegate IntPtr DSP_REALLOC_FUNC(
    IntPtr ptr,
    uint size,
    MEMORY_TYPE type,
    StringWrapper sourcestr);
}
