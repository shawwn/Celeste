﻿// Decompiled with JetBrains decompiler
// Type: FMOD.DSP_GETPARAM_DATA_CALLBACK
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;

namespace FMOD
{
  public delegate RESULT DSP_GETPARAM_DATA_CALLBACK(
    ref DSP_STATE dsp_state,
    int index,
    ref IntPtr data,
    ref uint length,
    IntPtr valuestr);
}
