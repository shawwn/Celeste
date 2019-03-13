// Decompiled with JetBrains decompiler
// Type: FMOD.SoundGroup
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FMOD
{
  public class SoundGroup : HandleBase
  {
    public RESULT release()
    {
      int num = (int) SoundGroup.FMOD_SoundGroup_Release(this.getRaw());
      if (num != 0)
        return (RESULT) num;
      this.rawPtr = IntPtr.Zero;
      return (RESULT) num;
    }

    public RESULT getSystemObject(out FMOD.System system)
    {
      system = (FMOD.System) null;
      IntPtr system1;
      int systemObject = (int) SoundGroup.FMOD_SoundGroup_GetSystemObject(this.rawPtr, out system1);
      system = new FMOD.System(system1);
      return (RESULT) systemObject;
    }

    public RESULT setMaxAudible(int maxaudible)
    {
      return SoundGroup.FMOD_SoundGroup_SetMaxAudible(this.rawPtr, maxaudible);
    }

    public RESULT getMaxAudible(out int maxaudible)
    {
      return SoundGroup.FMOD_SoundGroup_GetMaxAudible(this.rawPtr, out maxaudible);
    }

    public RESULT setMaxAudibleBehavior(SOUNDGROUP_BEHAVIOR behavior)
    {
      return SoundGroup.FMOD_SoundGroup_SetMaxAudibleBehavior(this.rawPtr, behavior);
    }

    public RESULT getMaxAudibleBehavior(out SOUNDGROUP_BEHAVIOR behavior)
    {
      return SoundGroup.FMOD_SoundGroup_GetMaxAudibleBehavior(this.rawPtr, out behavior);
    }

    public RESULT setMuteFadeSpeed(float speed)
    {
      return SoundGroup.FMOD_SoundGroup_SetMuteFadeSpeed(this.rawPtr, speed);
    }

    public RESULT getMuteFadeSpeed(out float speed)
    {
      return SoundGroup.FMOD_SoundGroup_GetMuteFadeSpeed(this.rawPtr, out speed);
    }

    public RESULT setVolume(float volume)
    {
      return SoundGroup.FMOD_SoundGroup_SetVolume(this.rawPtr, volume);
    }

    public RESULT getVolume(out float volume)
    {
      return SoundGroup.FMOD_SoundGroup_GetVolume(this.rawPtr, out volume);
    }

    public RESULT stop()
    {
      return SoundGroup.FMOD_SoundGroup_Stop(this.rawPtr);
    }

    public RESULT getName(StringBuilder name, int namelen)
    {
      IntPtr num = Marshal.AllocHGlobal(name.Capacity);
      int name1 = (int) SoundGroup.FMOD_SoundGroup_GetName(this.rawPtr, num, namelen);
      StringMarshalHelper.NativeToBuilder(name, num);
      Marshal.FreeHGlobal(num);
      return (RESULT) name1;
    }

    public RESULT getNumSounds(out int numsounds)
    {
      return SoundGroup.FMOD_SoundGroup_GetNumSounds(this.rawPtr, out numsounds);
    }

    public RESULT getSound(int index, out Sound sound)
    {
      sound = (Sound) null;
      IntPtr sound1;
      int sound2 = (int) SoundGroup.FMOD_SoundGroup_GetSound(this.rawPtr, index, out sound1);
      sound = new Sound(sound1);
      return (RESULT) sound2;
    }

    public RESULT getNumPlaying(out int numplaying)
    {
      return SoundGroup.FMOD_SoundGroup_GetNumPlaying(this.rawPtr, out numplaying);
    }

    public RESULT setUserData(IntPtr userdata)
    {
      return SoundGroup.FMOD_SoundGroup_SetUserData(this.rawPtr, userdata);
    }

    public RESULT getUserData(out IntPtr userdata)
    {
      return SoundGroup.FMOD_SoundGroup_GetUserData(this.rawPtr, out userdata);
    }

    [DllImport("fmod")]
    private static extern RESULT FMOD_SoundGroup_Release(IntPtr soundgroup);

    [DllImport("fmod")]
    private static extern RESULT FMOD_SoundGroup_GetSystemObject(
      IntPtr soundgroup,
      out IntPtr system);

    [DllImport("fmod")]
    private static extern RESULT FMOD_SoundGroup_SetMaxAudible(
      IntPtr soundgroup,
      int maxaudible);

    [DllImport("fmod")]
    private static extern RESULT FMOD_SoundGroup_GetMaxAudible(
      IntPtr soundgroup,
      out int maxaudible);

    [DllImport("fmod")]
    private static extern RESULT FMOD_SoundGroup_SetMaxAudibleBehavior(
      IntPtr soundgroup,
      SOUNDGROUP_BEHAVIOR behavior);

    [DllImport("fmod")]
    private static extern RESULT FMOD_SoundGroup_GetMaxAudibleBehavior(
      IntPtr soundgroup,
      out SOUNDGROUP_BEHAVIOR behavior);

    [DllImport("fmod")]
    private static extern RESULT FMOD_SoundGroup_SetMuteFadeSpeed(
      IntPtr soundgroup,
      float speed);

    [DllImport("fmod")]
    private static extern RESULT FMOD_SoundGroup_GetMuteFadeSpeed(
      IntPtr soundgroup,
      out float speed);

    [DllImport("fmod")]
    private static extern RESULT FMOD_SoundGroup_SetVolume(IntPtr soundgroup, float volume);

    [DllImport("fmod")]
    private static extern RESULT FMOD_SoundGroup_GetVolume(
      IntPtr soundgroup,
      out float volume);

    [DllImport("fmod")]
    private static extern RESULT FMOD_SoundGroup_Stop(IntPtr soundgroup);

    [DllImport("fmod")]
    private static extern RESULT FMOD_SoundGroup_GetName(
      IntPtr soundgroup,
      IntPtr name,
      int namelen);

    [DllImport("fmod")]
    private static extern RESULT FMOD_SoundGroup_GetNumSounds(
      IntPtr soundgroup,
      out int numsounds);

    [DllImport("fmod")]
    private static extern RESULT FMOD_SoundGroup_GetSound(
      IntPtr soundgroup,
      int index,
      out IntPtr sound);

    [DllImport("fmod")]
    private static extern RESULT FMOD_SoundGroup_GetNumPlaying(
      IntPtr soundgroup,
      out int numplaying);

    [DllImport("fmod")]
    private static extern RESULT FMOD_SoundGroup_SetUserData(
      IntPtr soundgroup,
      IntPtr userdata);

    [DllImport("fmod")]
    private static extern RESULT FMOD_SoundGroup_GetUserData(
      IntPtr soundgroup,
      out IntPtr userdata);

    public SoundGroup(IntPtr raw)
      : base(raw)
    {
    }
  }
}
