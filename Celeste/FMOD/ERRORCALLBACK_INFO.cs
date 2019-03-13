// Decompiled with JetBrains decompiler
// Type: FMOD.ERRORCALLBACK_INFO
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;
using System.Runtime.InteropServices;

namespace FMOD
{
  public struct ERRORCALLBACK_INFO
  {
    public RESULT result;
    public ERRORCALLBACK_INSTANCETYPE instancetype;
    public IntPtr instance;
    private IntPtr functionname_internal;
    private IntPtr functionparams_internal;

    public string functionname
    {
      get
      {
        return Marshal.PtrToStringAnsi(this.functionname_internal);
      }
    }

    public string functionparams
    {
      get
      {
        return Marshal.PtrToStringAnsi(this.functionparams_internal);
      }
    }
  }
}
