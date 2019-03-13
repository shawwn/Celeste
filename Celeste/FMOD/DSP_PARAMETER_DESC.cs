// Decompiled with JetBrains decompiler
// Type: FMOD.DSP_PARAMETER_DESC
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System.Runtime.InteropServices;

namespace FMOD
{
  public struct DSP_PARAMETER_DESC
  {
    public DSP_PARAMETER_TYPE type;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
    public char[] name;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
    public char[] label;
    public string description;
    public DSP_PARAMETER_DESC_UNION desc;
  }
}
