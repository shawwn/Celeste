﻿// Decompiled with JetBrains decompiler
// Type: FMOD.DSP_STATE
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;

namespace FMOD
{
  public struct DSP_STATE
  {
    public IntPtr instance;
    public IntPtr plugindata;
    public uint channelmask;
    public int source_speakermode;
    public IntPtr sidechaindata;
    public int sidechainchannels;
    public IntPtr functions;
    public int systemobject;
  }
}
