// Decompiled with JetBrains decompiler
// Type: FMOD.Studio.Bus
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FMOD.Studio
{
  public class Bus : HandleBase
  {
    public RESULT getID(out Guid id)
    {
      return Bus.FMOD_Studio_Bus_GetID(this.rawPtr, out id);
    }

    public RESULT getPath(out string path)
    {
      path = (string) null;
      byte[] numArray = new byte[256];
      int retrieved = 0;
      RESULT path1 = Bus.FMOD_Studio_Bus_GetPath(this.rawPtr, numArray, numArray.Length, out retrieved);
      if (path1 == RESULT.ERR_TRUNCATED)
      {
        numArray = new byte[retrieved];
        path1 = Bus.FMOD_Studio_Bus_GetPath(this.rawPtr, numArray, numArray.Length, out retrieved);
      }
      if (path1 == RESULT.OK)
        path = Encoding.UTF8.GetString(numArray, 0, retrieved - 1);
      return path1;
    }

    public RESULT getVolume(out float volume, out float finalvolume)
    {
      return Bus.FMOD_Studio_Bus_GetVolume(this.rawPtr, out volume, out finalvolume);
    }

    public RESULT setVolume(float volume)
    {
      return Bus.FMOD_Studio_Bus_SetVolume(this.rawPtr, volume);
    }

    public RESULT getPaused(out bool paused)
    {
      return Bus.FMOD_Studio_Bus_GetPaused(this.rawPtr, out paused);
    }

    public RESULT setPaused(bool paused)
    {
      return Bus.FMOD_Studio_Bus_SetPaused(this.rawPtr, paused);
    }

    public RESULT getMute(out bool mute)
    {
      return Bus.FMOD_Studio_Bus_GetMute(this.rawPtr, out mute);
    }

    public RESULT setMute(bool mute)
    {
      return Bus.FMOD_Studio_Bus_SetMute(this.rawPtr, mute);
    }

    public RESULT stopAllEvents(STOP_MODE mode)
    {
      return Bus.FMOD_Studio_Bus_StopAllEvents(this.rawPtr, mode);
    }

    public RESULT lockChannelGroup()
    {
      return Bus.FMOD_Studio_Bus_LockChannelGroup(this.rawPtr);
    }

    public RESULT unlockChannelGroup()
    {
      return Bus.FMOD_Studio_Bus_UnlockChannelGroup(this.rawPtr);
    }

    public RESULT getChannelGroup(out ChannelGroup group)
    {
      group = (ChannelGroup) null;
      IntPtr group1 = new IntPtr();
      RESULT channelGroup = Bus.FMOD_Studio_Bus_GetChannelGroup(this.rawPtr, out group1);
      if (channelGroup != RESULT.OK)
        return channelGroup;
      group = new ChannelGroup(group1);
      return channelGroup;
    }

    [DllImport("fmodstudio")]
    private static extern bool FMOD_Studio_Bus_IsValid(IntPtr bus);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_Bus_GetID(IntPtr bus, out Guid id);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_Bus_GetPath(
      IntPtr bus,
      [Out] byte[] path,
      int size,
      out int retrieved);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_Bus_GetVolume(
      IntPtr bus,
      out float volume,
      out float finalvolume);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_Bus_SetVolume(IntPtr bus, float volume);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_Bus_GetPaused(IntPtr bus, out bool paused);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_Bus_SetPaused(IntPtr bus, bool paused);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_Bus_GetMute(IntPtr bus, out bool mute);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_Bus_SetMute(IntPtr bus, bool mute);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_Bus_StopAllEvents(IntPtr bus, STOP_MODE mode);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_Bus_LockChannelGroup(IntPtr bus);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_Bus_UnlockChannelGroup(IntPtr bus);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_Bus_GetChannelGroup(IntPtr bus, out IntPtr group);

    public Bus(IntPtr raw)
      : base(raw)
    {
    }

    protected override bool isValidInternal()
    {
      return Bus.FMOD_Studio_Bus_IsValid(this.rawPtr);
    }
  }
}
