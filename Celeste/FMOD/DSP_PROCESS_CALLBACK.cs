// Decompiled with JetBrains decompiler
// Type: FMOD.DSP_PROCESS_CALLBACK
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

namespace FMOD
{
  public delegate RESULT DSP_PROCESS_CALLBACK(
    ref DSP_STATE dsp_state,
    uint length,
    ref DSP_BUFFER_ARRAY inbufferarray,
    ref DSP_BUFFER_ARRAY outbufferarray,
    bool inputsidle,
    DSP_PROCESS_OPERATION op);
}
