// Decompiled with JetBrains decompiler
// Type: FMOD.DSP_METERING_INFO
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System.Runtime.InteropServices;

namespace FMOD
{
  [StructLayout(LayoutKind.Sequential)]
  public class DSP_METERING_INFO
  {
    public int numsamples;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
    public float[] peaklevel;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
    public float[] rmslevel;
    public short numchannels;
  }
}
