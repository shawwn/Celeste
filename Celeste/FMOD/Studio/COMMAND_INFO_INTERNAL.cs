// Decompiled with JetBrains decompiler
// Type: FMOD.Studio.COMMAND_INFO_INTERNAL
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;

namespace FMOD.Studio
{
  internal struct COMMAND_INFO_INTERNAL
  {
    public IntPtr commandname;
    public int parentcommandindex;
    public int framenumber;
    public float frametime;
    public INSTANCETYPE instancetype;
    public INSTANCETYPE outputtype;
    public uint instancehandle;
    public uint outputhandle;

    public COMMAND_INFO createPublic()
    {
      return new COMMAND_INFO()
      {
        commandname = MarshallingHelper.stringFromNativeUtf8(this.commandname),
        parentcommandindex = this.parentcommandindex,
        framenumber = this.framenumber,
        frametime = this.frametime,
        instancetype = this.instancetype,
        outputtype = this.outputtype,
        instancehandle = this.instancehandle,
        outputhandle = this.outputhandle
      };
    }
  }
}
