// Decompiled with JetBrains decompiler
// Type: FMOD.TAG
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;
using System.Runtime.InteropServices;

namespace FMOD
{
  public struct TAG
  {
    public TAGTYPE type;
    public TAGDATATYPE datatype;
    private IntPtr name_internal;
    public IntPtr data;
    public uint datalen;
    public bool updated;

    public string name
    {
      get
      {
        return Marshal.PtrToStringAnsi(this.name_internal);
      }
    }
  }
}
