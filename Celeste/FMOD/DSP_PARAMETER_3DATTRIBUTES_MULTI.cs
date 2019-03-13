// Decompiled with JetBrains decompiler
// Type: FMOD.DSP_PARAMETER_3DATTRIBUTES_MULTI
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System.Runtime.InteropServices;

namespace FMOD
{
  public struct DSP_PARAMETER_3DATTRIBUTES_MULTI
  {
    public int numlisteners;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
    public _3D_ATTRIBUTES[] relative;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
    public float[] weight;
    public _3D_ATTRIBUTES absolute;
  }
}
