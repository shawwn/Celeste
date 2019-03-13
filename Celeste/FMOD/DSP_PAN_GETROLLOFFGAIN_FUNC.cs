// Decompiled with JetBrains decompiler
// Type: FMOD.DSP_PAN_GETROLLOFFGAIN_FUNC
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

namespace FMOD
{
  public delegate RESULT DSP_PAN_GETROLLOFFGAIN_FUNC(
    ref DSP_STATE dsp_state,
    DSP_PAN_3D_ROLLOFF_TYPE rolloff,
    float distance,
    float mindistance,
    float maxdistance,
    out float gain);
}
