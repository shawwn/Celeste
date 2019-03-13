// Decompiled with JetBrains decompiler
// Type: FMOD.Studio.CommandReplay
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FMOD.Studio
{
  public class CommandReplay : HandleBase
  {
    public RESULT getSystem(out FMOD.Studio.System system)
    {
      system = (FMOD.Studio.System) null;
      IntPtr system1 = new IntPtr();
      int system2 = (int) CommandReplay.FMOD_Studio_CommandReplay_GetSystem(this.rawPtr, out system1);
      if (system2 != 0)
        return (RESULT) system2;
      system = new FMOD.Studio.System(system1);
      return (RESULT) system2;
    }

    public RESULT getLength(out float totalTime)
    {
      return CommandReplay.FMOD_Studio_CommandReplay_GetLength(this.rawPtr, out totalTime);
    }

    public RESULT getCommandCount(out int count)
    {
      return CommandReplay.FMOD_Studio_CommandReplay_GetCommandCount(this.rawPtr, out count);
    }

    public RESULT getCommandInfo(int commandIndex, out COMMAND_INFO info)
    {
      COMMAND_INFO_INTERNAL info1 = new COMMAND_INFO_INTERNAL();
      RESULT commandInfo = CommandReplay.FMOD_Studio_CommandReplay_GetCommandInfo(this.rawPtr, commandIndex, out info1);
      if (commandInfo != RESULT.OK)
      {
        info = new COMMAND_INFO();
        return commandInfo;
      }
      info = info1.createPublic();
      return commandInfo;
    }

    public RESULT getCommandString(int commandIndex, out string description)
    {
      description = (string) null;
      byte[] numArray = new byte[8];
      RESULT commandString;
      while (true)
      {
        commandString = CommandReplay.FMOD_Studio_CommandReplay_GetCommandString(this.rawPtr, commandIndex, numArray, numArray.Length);
        switch (commandString)
        {
          case RESULT.OK:
            goto label_3;
          case RESULT.ERR_TRUNCATED:
            numArray = new byte[2 * numArray.Length];
            continue;
          default:
            goto label_7;
        }
      }
label_3:
      int count = 0;
      while (numArray[count] != (byte) 0)
        ++count;
      description = Encoding.UTF8.GetString(numArray, 0, count);
label_7:
      return commandString;
    }

    public RESULT getCommandAtTime(float time, out int commandIndex)
    {
      return CommandReplay.FMOD_Studio_CommandReplay_GetCommandAtTime(this.rawPtr, time, out commandIndex);
    }

    public RESULT setBankPath(string bankPath)
    {
      return CommandReplay.FMOD_Studio_CommandReplay_SetBankPath(this.rawPtr, Encoding.UTF8.GetBytes(bankPath + "\0"));
    }

    public RESULT start()
    {
      return CommandReplay.FMOD_Studio_CommandReplay_Start(this.rawPtr);
    }

    public RESULT stop()
    {
      return CommandReplay.FMOD_Studio_CommandReplay_Stop(this.rawPtr);
    }

    public RESULT seekToTime(float time)
    {
      return CommandReplay.FMOD_Studio_CommandReplay_SeekToTime(this.rawPtr, time);
    }

    public RESULT seekToCommand(int commandIndex)
    {
      return CommandReplay.FMOD_Studio_CommandReplay_SeekToCommand(this.rawPtr, commandIndex);
    }

    public RESULT getPaused(out bool paused)
    {
      return CommandReplay.FMOD_Studio_CommandReplay_GetPaused(this.rawPtr, out paused);
    }

    public RESULT setPaused(bool paused)
    {
      return CommandReplay.FMOD_Studio_CommandReplay_SetPaused(this.rawPtr, paused);
    }

    public RESULT getPlaybackState(out PLAYBACK_STATE state)
    {
      return CommandReplay.FMOD_Studio_CommandReplay_GetPlaybackState(this.rawPtr, out state);
    }

    public RESULT getCurrentCommand(out int commandIndex, out float currentTime)
    {
      return CommandReplay.FMOD_Studio_CommandReplay_GetCurrentCommand(this.rawPtr, out commandIndex, out currentTime);
    }

    public RESULT release()
    {
      return CommandReplay.FMOD_Studio_CommandReplay_Release(this.rawPtr);
    }

    public RESULT setFrameCallback(COMMANDREPLAY_FRAME_CALLBACK callback)
    {
      return CommandReplay.FMOD_Studio_CommandReplay_SetFrameCallback(this.rawPtr, callback);
    }

    public RESULT setLoadBankCallback(COMMANDREPLAY_LOAD_BANK_CALLBACK callback)
    {
      return CommandReplay.FMOD_Studio_CommandReplay_SetLoadBankCallback(this.rawPtr, callback);
    }

    public RESULT setCreateInstanceCallback(COMMANDREPLAY_CREATE_INSTANCE_CALLBACK callback)
    {
      return CommandReplay.FMOD_Studio_CommandReplay_SetCreateInstanceCallback(this.rawPtr, callback);
    }

    public RESULT getUserData(out IntPtr userdata)
    {
      return CommandReplay.FMOD_Studio_CommandReplay_GetUserData(this.rawPtr, out userdata);
    }

    public RESULT setUserData(IntPtr userdata)
    {
      return CommandReplay.FMOD_Studio_CommandReplay_SetUserData(this.rawPtr, userdata);
    }

    [DllImport("fmodstudio")]
    private static extern bool FMOD_Studio_CommandReplay_IsValid(IntPtr replay);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_CommandReplay_GetSystem(
      IntPtr replay,
      out IntPtr system);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_CommandReplay_GetLength(
      IntPtr replay,
      out float totalTime);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_CommandReplay_GetCommandCount(
      IntPtr replay,
      out int count);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_CommandReplay_GetCommandInfo(
      IntPtr replay,
      int commandIndex,
      out COMMAND_INFO_INTERNAL info);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_CommandReplay_GetCommandString(
      IntPtr replay,
      int commandIndex,
      [Out] byte[] description,
      int capacity);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_CommandReplay_GetCommandAtTime(
      IntPtr replay,
      float time,
      out int commandIndex);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_CommandReplay_SetBankPath(
      IntPtr replay,
      byte[] bankPath);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_CommandReplay_Start(IntPtr replay);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_CommandReplay_Stop(IntPtr replay);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_CommandReplay_SeekToTime(
      IntPtr replay,
      float time);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_CommandReplay_SeekToCommand(
      IntPtr replay,
      int commandIndex);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_CommandReplay_GetPaused(
      IntPtr replay,
      out bool paused);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_CommandReplay_SetPaused(
      IntPtr replay,
      bool paused);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_CommandReplay_GetPlaybackState(
      IntPtr replay,
      out PLAYBACK_STATE state);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_CommandReplay_GetCurrentCommand(
      IntPtr replay,
      out int commandIndex,
      out float currentTime);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_CommandReplay_Release(IntPtr replay);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_CommandReplay_SetFrameCallback(
      IntPtr replay,
      COMMANDREPLAY_FRAME_CALLBACK callback);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_CommandReplay_SetLoadBankCallback(
      IntPtr replay,
      COMMANDREPLAY_LOAD_BANK_CALLBACK callback);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_CommandReplay_SetCreateInstanceCallback(
      IntPtr replay,
      COMMANDREPLAY_CREATE_INSTANCE_CALLBACK callback);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_CommandReplay_GetUserData(
      IntPtr replay,
      out IntPtr userdata);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_CommandReplay_SetUserData(
      IntPtr replay,
      IntPtr userdata);

    public CommandReplay(IntPtr raw)
      : base(raw)
    {
    }

    protected override bool isValidInternal()
    {
      return CommandReplay.FMOD_Studio_CommandReplay_IsValid(this.rawPtr);
    }
  }
}
