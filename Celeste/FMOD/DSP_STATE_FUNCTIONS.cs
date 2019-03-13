// Decompiled with JetBrains decompiler
// Type: FMOD.DSP_STATE_FUNCTIONS
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;

namespace FMOD
{
  public struct DSP_STATE_FUNCTIONS
  {
    private DSP_ALLOC_FUNC alloc;
    private DSP_REALLOC_FUNC realloc;
    private DSP_FREE_FUNC free;
    private DSP_GETSAMPLERATE_FUNC getsamplerate;
    private DSP_GETBLOCKSIZE_FUNC getblocksize;
    private IntPtr dft;
    private IntPtr pan;
    private DSP_GETSPEAKERMODE_FUNC getspeakermode;
    private DSP_GETCLOCK_FUNC getclock;
    private DSP_GETLISTENERATTRIBUTES_FUNC getlistenerattributes;
    private DSP_LOG_FUNC log;
    private DSP_GETUSERDATA_FUNC getuserdata;
  }
}
