// Decompiled with JetBrains decompiler
// Type: FMOD.Studio.Union_IntBoolFloatString
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;
using System.Runtime.InteropServices;

namespace FMOD.Studio
{
  [StructLayout(LayoutKind.Explicit)]
  internal struct Union_IntBoolFloatString
  {
    [FieldOffset(0)]
    public int intvalue;
    [FieldOffset(0)]
    public bool boolvalue;
    [FieldOffset(0)]
    public float floatvalue;
    [FieldOffset(0)]
    public IntPtr stringvalue;
  }
}
