// Decompiled with JetBrains decompiler
// Type: FMOD.Studio.SOUND_INFO
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FMOD.Studio
{
  public class SOUND_INFO
  {
    public byte[] name_or_data;
    public MODE mode;
    public CREATESOUNDEXINFO exinfo;
    public int subsoundindex;

    public string name
    {
      get
      {
        if ((this.mode & (MODE.OPENMEMORY | MODE.OPENMEMORY_POINT)) != MODE.DEFAULT || this.name_or_data == null)
          return (string) null;
        int count = Array.IndexOf<byte>(this.name_or_data, (byte) 0);
        if (count > 0)
          return Encoding.UTF8.GetString(this.name_or_data, 0, count);
        return (string) null;
      }
    }

    ~SOUND_INFO()
    {
      if (!(this.exinfo.inclusionlist != IntPtr.Zero))
        return;
      Marshal.FreeHGlobal(this.exinfo.inclusionlist);
    }
  }
}
