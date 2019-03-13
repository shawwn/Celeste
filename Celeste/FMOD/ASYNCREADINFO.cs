// Decompiled with JetBrains decompiler
// Type: FMOD.ASYNCREADINFO
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;

namespace FMOD
{
  public struct ASYNCREADINFO
  {
    public IntPtr handle;
    public uint offset;
    public uint sizebytes;
    public int priority;
    public IntPtr userdata;
    public IntPtr buffer;
    public uint bytesread;
    public ASYNCREADINFO_DONE_CALLBACK done;
  }
}
