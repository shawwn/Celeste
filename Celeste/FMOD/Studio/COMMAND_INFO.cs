// Decompiled with JetBrains decompiler
// Type: FMOD.Studio.COMMAND_INFO
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

namespace FMOD.Studio
{
  public struct COMMAND_INFO
  {
    public string commandname;
    public int parentcommandindex;
    public int framenumber;
    public float frametime;
    public INSTANCETYPE instancetype;
    public INSTANCETYPE outputtype;
    public uint instancehandle;
    public uint outputhandle;
  }
}
