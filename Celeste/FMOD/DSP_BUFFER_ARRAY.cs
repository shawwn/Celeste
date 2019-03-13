// Decompiled with JetBrains decompiler
// Type: FMOD.DSP_BUFFER_ARRAY
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;

namespace FMOD
{
  public struct DSP_BUFFER_ARRAY
  {
    public int numbuffers;
    public int[] buffernumchannels;
    public CHANNELMASK[] bufferchannelmask;
    public IntPtr[] buffers;
    public SPEAKERMODE speakermode;
  }
}
