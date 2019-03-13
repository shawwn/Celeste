// Decompiled with JetBrains decompiler
// Type: FMOD.Studio.SOUND_INFO_INTERNAL
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;
using System.Runtime.InteropServices;

namespace FMOD.Studio
{
  public struct SOUND_INFO_INTERNAL
  {
    private IntPtr name_or_data;
    private MODE mode;
    private CREATESOUNDEXINFO exinfo;
    private int subsoundindex;

    public void assign(out SOUND_INFO publicInfo)
    {
      publicInfo = new SOUND_INFO();
      publicInfo.mode = this.mode;
      publicInfo.exinfo = this.exinfo;
      publicInfo.exinfo.inclusionlist = Marshal.AllocHGlobal(Marshal.SizeOf(typeof (int)));
      Marshal.WriteInt32(publicInfo.exinfo.inclusionlist, this.subsoundindex);
      publicInfo.exinfo.inclusionlistnum = 1;
      publicInfo.subsoundindex = this.subsoundindex;
      if (this.name_or_data != IntPtr.Zero)
      {
        int num;
        int length;
        if ((this.mode & (MODE.OPENMEMORY | MODE.OPENMEMORY_POINT)) != MODE.DEFAULT)
        {
          publicInfo.mode = publicInfo.mode & ~MODE.OPENMEMORY_POINT | MODE.OPENMEMORY;
          num = (int) this.exinfo.fileoffset;
          publicInfo.exinfo.fileoffset = 0U;
          length = (int) this.exinfo.length;
        }
        else
        {
          num = 0;
          length = MarshallingHelper.stringLengthUtf8(this.name_or_data) + 1;
        }
        publicInfo.name_or_data = new byte[length];
        Marshal.Copy(new IntPtr(this.name_or_data.ToInt64() + (long) num), publicInfo.name_or_data, 0, length);
      }
      else
        publicInfo.name_or_data = (byte[]) null;
    }
  }
}
