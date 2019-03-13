// Decompiled with JetBrains decompiler
// Type: FMOD.DSP_PARAMETER_FFT
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;
using System.Runtime.InteropServices;

namespace FMOD
{
  public struct DSP_PARAMETER_FFT
  {
    public int length;
    public int numchannels;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
    private IntPtr[] spectrum_internal;

    public float[][] spectrum
    {
      get
      {
        float[][] numArray = new float[this.numchannels][];
        for (int index = 0; index < this.numchannels; ++index)
        {
          numArray[index] = new float[this.length];
          Marshal.Copy(this.spectrum_internal[index], numArray[index], 0, this.length);
        }
        return numArray;
      }
    }
  }
}
