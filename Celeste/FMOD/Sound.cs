// Decompiled with JetBrains decompiler
// Type: FMOD.Sound
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FMOD
{
  public class Sound : HandleBase
  {
    public RESULT release()
    {
      int num = (int) Sound.FMOD_Sound_Release(this.rawPtr);
      if (num != 0)
        return (RESULT) num;
      this.rawPtr = IntPtr.Zero;
      return (RESULT) num;
    }

    public RESULT getSystemObject(out FMOD.System system)
    {
      system = (FMOD.System) null;
      IntPtr system1;
      int systemObject = (int) Sound.FMOD_Sound_GetSystemObject(this.rawPtr, out system1);
      system = new FMOD.System(system1);
      return (RESULT) systemObject;
    }

    public RESULT @lock(
      uint offset,
      uint length,
      out IntPtr ptr1,
      out IntPtr ptr2,
      out uint len1,
      out uint len2)
    {
      return Sound.FMOD_Sound_Lock(this.rawPtr, offset, length, out ptr1, out ptr2, out len1, out len2);
    }

    public RESULT unlock(IntPtr ptr1, IntPtr ptr2, uint len1, uint len2)
    {
      return Sound.FMOD_Sound_Unlock(this.rawPtr, ptr1, ptr2, len1, len2);
    }

    public RESULT setDefaults(float frequency, int priority)
    {
      return Sound.FMOD_Sound_SetDefaults(this.rawPtr, frequency, priority);
    }

    public RESULT getDefaults(out float frequency, out int priority)
    {
      return Sound.FMOD_Sound_GetDefaults(this.rawPtr, out frequency, out priority);
    }

    public RESULT set3DMinMaxDistance(float min, float max)
    {
      return Sound.FMOD_Sound_Set3DMinMaxDistance(this.rawPtr, min, max);
    }

    public RESULT get3DMinMaxDistance(out float min, out float max)
    {
      return Sound.FMOD_Sound_Get3DMinMaxDistance(this.rawPtr, out min, out max);
    }

    public RESULT set3DConeSettings(
      float insideconeangle,
      float outsideconeangle,
      float outsidevolume)
    {
      return Sound.FMOD_Sound_Set3DConeSettings(this.rawPtr, insideconeangle, outsideconeangle, outsidevolume);
    }

    public RESULT get3DConeSettings(
      out float insideconeangle,
      out float outsideconeangle,
      out float outsidevolume)
    {
      return Sound.FMOD_Sound_Get3DConeSettings(this.rawPtr, out insideconeangle, out outsideconeangle, out outsidevolume);
    }

    public RESULT set3DCustomRolloff(ref VECTOR points, int numpoints)
    {
      return Sound.FMOD_Sound_Set3DCustomRolloff(this.rawPtr, ref points, numpoints);
    }

    public RESULT get3DCustomRolloff(out IntPtr points, out int numpoints)
    {
      return Sound.FMOD_Sound_Get3DCustomRolloff(this.rawPtr, out points, out numpoints);
    }

    public RESULT getSubSound(int index, out Sound subsound)
    {
      subsound = (Sound) null;
      IntPtr subsound1;
      int subSound = (int) Sound.FMOD_Sound_GetSubSound(this.rawPtr, index, out subsound1);
      subsound = new Sound(subsound1);
      return (RESULT) subSound;
    }

    public RESULT getSubSoundParent(out Sound parentsound)
    {
      parentsound = (Sound) null;
      IntPtr parentsound1;
      int subSoundParent = (int) Sound.FMOD_Sound_GetSubSoundParent(this.rawPtr, out parentsound1);
      parentsound = new Sound(parentsound1);
      return (RESULT) subSoundParent;
    }

    public RESULT getName(StringBuilder name, int namelen)
    {
      IntPtr num = Marshal.AllocHGlobal(name.Capacity);
      int name1 = (int) Sound.FMOD_Sound_GetName(this.rawPtr, num, namelen);
      StringMarshalHelper.NativeToBuilder(name, num);
      Marshal.FreeHGlobal(num);
      return (RESULT) name1;
    }

    public RESULT getLength(out uint length, TIMEUNIT lengthtype)
    {
      return Sound.FMOD_Sound_GetLength(this.rawPtr, out length, lengthtype);
    }

    public RESULT getFormat(
      out SOUND_TYPE type,
      out SOUND_FORMAT format,
      out int channels,
      out int bits)
    {
      return Sound.FMOD_Sound_GetFormat(this.rawPtr, out type, out format, out channels, out bits);
    }

    public RESULT getNumSubSounds(out int numsubsounds)
    {
      return Sound.FMOD_Sound_GetNumSubSounds(this.rawPtr, out numsubsounds);
    }

    public RESULT getNumTags(out int numtags, out int numtagsupdated)
    {
      return Sound.FMOD_Sound_GetNumTags(this.rawPtr, out numtags, out numtagsupdated);
    }

    public RESULT getTag(string name, int index, out TAG tag)
    {
      return Sound.FMOD_Sound_GetTag(this.rawPtr, name, index, out tag);
    }

    public RESULT getOpenState(
      out OPENSTATE openstate,
      out uint percentbuffered,
      out bool starving,
      out bool diskbusy)
    {
      return Sound.FMOD_Sound_GetOpenState(this.rawPtr, out openstate, out percentbuffered, out starving, out diskbusy);
    }

    public RESULT readData(IntPtr buffer, uint length, out uint read)
    {
      return Sound.FMOD_Sound_ReadData(this.rawPtr, buffer, length, out read);
    }

    public RESULT seekData(uint pcm)
    {
      return Sound.FMOD_Sound_SeekData(this.rawPtr, pcm);
    }

    public RESULT setSoundGroup(SoundGroup soundgroup)
    {
      return Sound.FMOD_Sound_SetSoundGroup(this.rawPtr, soundgroup.getRaw());
    }

    public RESULT getSoundGroup(out SoundGroup soundgroup)
    {
      soundgroup = (SoundGroup) null;
      IntPtr soundgroup1;
      int soundGroup = (int) Sound.FMOD_Sound_GetSoundGroup(this.rawPtr, out soundgroup1);
      soundgroup = new SoundGroup(soundgroup1);
      return (RESULT) soundGroup;
    }

    public RESULT getNumSyncPoints(out int numsyncpoints)
    {
      return Sound.FMOD_Sound_GetNumSyncPoints(this.rawPtr, out numsyncpoints);
    }

    public RESULT getSyncPoint(int index, out IntPtr point)
    {
      return Sound.FMOD_Sound_GetSyncPoint(this.rawPtr, index, out point);
    }

    public RESULT getSyncPointInfo(
      IntPtr point,
      StringBuilder name,
      int namelen,
      out uint offset,
      TIMEUNIT offsettype)
    {
      IntPtr num = Marshal.AllocHGlobal(name.Capacity);
      int syncPointInfo = (int) Sound.FMOD_Sound_GetSyncPointInfo(this.rawPtr, point, num, namelen, out offset, offsettype);
      StringMarshalHelper.NativeToBuilder(name, num);
      Marshal.FreeHGlobal(num);
      return (RESULT) syncPointInfo;
    }

    public RESULT addSyncPoint(
      uint offset,
      TIMEUNIT offsettype,
      string name,
      out IntPtr point)
    {
      return Sound.FMOD_Sound_AddSyncPoint(this.rawPtr, offset, offsettype, name, out point);
    }

    public RESULT deleteSyncPoint(IntPtr point)
    {
      return Sound.FMOD_Sound_DeleteSyncPoint(this.rawPtr, point);
    }

    public RESULT setMode(MODE mode)
    {
      return Sound.FMOD_Sound_SetMode(this.rawPtr, mode);
    }

    public RESULT getMode(out MODE mode)
    {
      return Sound.FMOD_Sound_GetMode(this.rawPtr, out mode);
    }

    public RESULT setLoopCount(int loopcount)
    {
      return Sound.FMOD_Sound_SetLoopCount(this.rawPtr, loopcount);
    }

    public RESULT getLoopCount(out int loopcount)
    {
      return Sound.FMOD_Sound_GetLoopCount(this.rawPtr, out loopcount);
    }

    public RESULT setLoopPoints(
      uint loopstart,
      TIMEUNIT loopstarttype,
      uint loopend,
      TIMEUNIT loopendtype)
    {
      return Sound.FMOD_Sound_SetLoopPoints(this.rawPtr, loopstart, loopstarttype, loopend, loopendtype);
    }

    public RESULT getLoopPoints(
      out uint loopstart,
      TIMEUNIT loopstarttype,
      out uint loopend,
      TIMEUNIT loopendtype)
    {
      return Sound.FMOD_Sound_GetLoopPoints(this.rawPtr, out loopstart, loopstarttype, out loopend, loopendtype);
    }

    public RESULT getMusicNumChannels(out int numchannels)
    {
      return Sound.FMOD_Sound_GetMusicNumChannels(this.rawPtr, out numchannels);
    }

    public RESULT setMusicChannelVolume(int channel, float volume)
    {
      return Sound.FMOD_Sound_SetMusicChannelVolume(this.rawPtr, channel, volume);
    }

    public RESULT getMusicChannelVolume(int channel, out float volume)
    {
      return Sound.FMOD_Sound_GetMusicChannelVolume(this.rawPtr, channel, out volume);
    }

    public RESULT setMusicSpeed(float speed)
    {
      return Sound.FMOD_Sound_SetMusicSpeed(this.rawPtr, speed);
    }

    public RESULT getMusicSpeed(out float speed)
    {
      return Sound.FMOD_Sound_GetMusicSpeed(this.rawPtr, out speed);
    }

    public RESULT setUserData(IntPtr userdata)
    {
      return Sound.FMOD_Sound_SetUserData(this.rawPtr, userdata);
    }

    public RESULT getUserData(out IntPtr userdata)
    {
      return Sound.FMOD_Sound_GetUserData(this.rawPtr, out userdata);
    }

    [DllImport("fmod")]
    private static extern RESULT FMOD_Sound_Release(IntPtr sound);

    [DllImport("fmod")]
    private static extern RESULT FMOD_Sound_GetSystemObject(IntPtr sound, out IntPtr system);

    [DllImport("fmod")]
    private static extern RESULT FMOD_Sound_Lock(
      IntPtr sound,
      uint offset,
      uint length,
      out IntPtr ptr1,
      out IntPtr ptr2,
      out uint len1,
      out uint len2);

    [DllImport("fmod")]
    private static extern RESULT FMOD_Sound_Unlock(
      IntPtr sound,
      IntPtr ptr1,
      IntPtr ptr2,
      uint len1,
      uint len2);

    [DllImport("fmod")]
    private static extern RESULT FMOD_Sound_SetDefaults(
      IntPtr sound,
      float frequency,
      int priority);

    [DllImport("fmod")]
    private static extern RESULT FMOD_Sound_GetDefaults(
      IntPtr sound,
      out float frequency,
      out int priority);

    [DllImport("fmod")]
    private static extern RESULT FMOD_Sound_Set3DMinMaxDistance(
      IntPtr sound,
      float min,
      float max);

    [DllImport("fmod")]
    private static extern RESULT FMOD_Sound_Get3DMinMaxDistance(
      IntPtr sound,
      out float min,
      out float max);

    [DllImport("fmod")]
    private static extern RESULT FMOD_Sound_Set3DConeSettings(
      IntPtr sound,
      float insideconeangle,
      float outsideconeangle,
      float outsidevolume);

    [DllImport("fmod")]
    private static extern RESULT FMOD_Sound_Get3DConeSettings(
      IntPtr sound,
      out float insideconeangle,
      out float outsideconeangle,
      out float outsidevolume);

    [DllImport("fmod")]
    private static extern RESULT FMOD_Sound_Set3DCustomRolloff(
      IntPtr sound,
      ref VECTOR points,
      int numpoints);

    [DllImport("fmod")]
    private static extern RESULT FMOD_Sound_Get3DCustomRolloff(
      IntPtr sound,
      out IntPtr points,
      out int numpoints);

    [DllImport("fmod")]
    private static extern RESULT FMOD_Sound_GetSubSound(
      IntPtr sound,
      int index,
      out IntPtr subsound);

    [DllImport("fmod")]
    private static extern RESULT FMOD_Sound_GetSubSoundParent(
      IntPtr sound,
      out IntPtr parentsound);

    [DllImport("fmod")]
    private static extern RESULT FMOD_Sound_GetName(IntPtr sound, IntPtr name, int namelen);

    [DllImport("fmod")]
    private static extern RESULT FMOD_Sound_GetLength(
      IntPtr sound,
      out uint length,
      TIMEUNIT lengthtype);

    [DllImport("fmod")]
    private static extern RESULT FMOD_Sound_GetFormat(
      IntPtr sound,
      out SOUND_TYPE type,
      out SOUND_FORMAT format,
      out int channels,
      out int bits);

    [DllImport("fmod")]
    private static extern RESULT FMOD_Sound_GetNumSubSounds(
      IntPtr sound,
      out int numsubsounds);

    [DllImport("fmod")]
    private static extern RESULT FMOD_Sound_GetNumTags(
      IntPtr sound,
      out int numtags,
      out int numtagsupdated);

    [DllImport("fmod")]
    private static extern RESULT FMOD_Sound_GetTag(
      IntPtr sound,
      string name,
      int index,
      out TAG tag);

    [DllImport("fmod")]
    private static extern RESULT FMOD_Sound_GetOpenState(
      IntPtr sound,
      out OPENSTATE openstate,
      out uint percentbuffered,
      out bool starving,
      out bool diskbusy);

    [DllImport("fmod")]
    private static extern RESULT FMOD_Sound_ReadData(
      IntPtr sound,
      IntPtr buffer,
      uint length,
      out uint read);

    [DllImport("fmod")]
    private static extern RESULT FMOD_Sound_SeekData(IntPtr sound, uint pcm);

    [DllImport("fmod")]
    private static extern RESULT FMOD_Sound_SetSoundGroup(IntPtr sound, IntPtr soundgroup);

    [DllImport("fmod")]
    private static extern RESULT FMOD_Sound_GetSoundGroup(IntPtr sound, out IntPtr soundgroup);

    [DllImport("fmod")]
    private static extern RESULT FMOD_Sound_GetNumSyncPoints(
      IntPtr sound,
      out int numsyncpoints);

    [DllImport("fmod")]
    private static extern RESULT FMOD_Sound_GetSyncPoint(
      IntPtr sound,
      int index,
      out IntPtr point);

    [DllImport("fmod")]
    private static extern RESULT FMOD_Sound_GetSyncPointInfo(
      IntPtr sound,
      IntPtr point,
      IntPtr name,
      int namelen,
      out uint offset,
      TIMEUNIT offsettype);

    [DllImport("fmod")]
    private static extern RESULT FMOD_Sound_AddSyncPoint(
      IntPtr sound,
      uint offset,
      TIMEUNIT offsettype,
      string name,
      out IntPtr point);

    [DllImport("fmod")]
    private static extern RESULT FMOD_Sound_DeleteSyncPoint(IntPtr sound, IntPtr point);

    [DllImport("fmod")]
    private static extern RESULT FMOD_Sound_SetMode(IntPtr sound, MODE mode);

    [DllImport("fmod")]
    private static extern RESULT FMOD_Sound_GetMode(IntPtr sound, out MODE mode);

    [DllImport("fmod")]
    private static extern RESULT FMOD_Sound_SetLoopCount(IntPtr sound, int loopcount);

    [DllImport("fmod")]
    private static extern RESULT FMOD_Sound_GetLoopCount(IntPtr sound, out int loopcount);

    [DllImport("fmod")]
    private static extern RESULT FMOD_Sound_SetLoopPoints(
      IntPtr sound,
      uint loopstart,
      TIMEUNIT loopstarttype,
      uint loopend,
      TIMEUNIT loopendtype);

    [DllImport("fmod")]
    private static extern RESULT FMOD_Sound_GetLoopPoints(
      IntPtr sound,
      out uint loopstart,
      TIMEUNIT loopstarttype,
      out uint loopend,
      TIMEUNIT loopendtype);

    [DllImport("fmod")]
    private static extern RESULT FMOD_Sound_GetMusicNumChannels(
      IntPtr sound,
      out int numchannels);

    [DllImport("fmod")]
    private static extern RESULT FMOD_Sound_SetMusicChannelVolume(
      IntPtr sound,
      int channel,
      float volume);

    [DllImport("fmod")]
    private static extern RESULT FMOD_Sound_GetMusicChannelVolume(
      IntPtr sound,
      int channel,
      out float volume);

    [DllImport("fmod")]
    private static extern RESULT FMOD_Sound_SetMusicSpeed(IntPtr sound, float speed);

    [DllImport("fmod")]
    private static extern RESULT FMOD_Sound_GetMusicSpeed(IntPtr sound, out float speed);

    [DllImport("fmod")]
    private static extern RESULT FMOD_Sound_SetUserData(IntPtr sound, IntPtr userdata);

    [DllImport("fmod")]
    private static extern RESULT FMOD_Sound_GetUserData(IntPtr sound, out IntPtr userdata);

    public Sound(IntPtr raw)
      : base(raw)
    {
    }
  }
}
