// Decompiled with JetBrains decompiler
// Type: FMOD.MEMORY_REALLOC_CALLBACK
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;

namespace FMOD
{
  public delegate IntPtr MEMORY_REALLOC_CALLBACK(
    IntPtr ptr,
    uint size,
    MEMORY_TYPE type,
    StringWrapper sourcestr);
}
