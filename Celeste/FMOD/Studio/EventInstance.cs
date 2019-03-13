// Decompiled with JetBrains decompiler
// Type: FMOD.Studio.EventInstance
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FMOD.Studio
{
  public class EventInstance : HandleBase
  {
    public RESULT getDescription(out EventDescription description)
    {
      description = (EventDescription) null;
      IntPtr description1;
      RESULT description2 = EventInstance.FMOD_Studio_EventInstance_GetDescription(this.rawPtr, out description1);
      if (description2 != RESULT.OK)
        return description2;
      description = new EventDescription(description1);
      return description2;
    }

    public RESULT getVolume(out float volume, out float finalvolume)
    {
      return EventInstance.FMOD_Studio_EventInstance_GetVolume(this.rawPtr, out volume, out finalvolume);
    }

    public RESULT setVolume(float volume)
    {
      return EventInstance.FMOD_Studio_EventInstance_SetVolume(this.rawPtr, volume);
    }

    public RESULT getPitch(out float pitch, out float finalpitch)
    {
      return EventInstance.FMOD_Studio_EventInstance_GetPitch(this.rawPtr, out pitch, out finalpitch);
    }

    public RESULT setPitch(float pitch)
    {
      return EventInstance.FMOD_Studio_EventInstance_SetPitch(this.rawPtr, pitch);
    }

    public RESULT get3DAttributes(out _3D_ATTRIBUTES attributes)
    {
      return EventInstance.FMOD_Studio_EventInstance_Get3DAttributes(this.rawPtr, out attributes);
    }

    public RESULT set3DAttributes(_3D_ATTRIBUTES attributes)
    {
      return EventInstance.FMOD_Studio_EventInstance_Set3DAttributes(this.rawPtr, ref attributes);
    }

    public RESULT getListenerMask(out uint mask)
    {
      return EventInstance.FMOD_Studio_EventInstance_GetListenerMask(this.rawPtr, out mask);
    }

    public RESULT setListenerMask(uint mask)
    {
      return EventInstance.FMOD_Studio_EventInstance_SetListenerMask(this.rawPtr, mask);
    }

    public RESULT getProperty(EVENT_PROPERTY index, out float value)
    {
      return EventInstance.FMOD_Studio_EventInstance_GetProperty(this.rawPtr, index, out value);
    }

    public RESULT setProperty(EVENT_PROPERTY index, float value)
    {
      return EventInstance.FMOD_Studio_EventInstance_SetProperty(this.rawPtr, index, value);
    }

    public RESULT getReverbLevel(int index, out float level)
    {
      return EventInstance.FMOD_Studio_EventInstance_GetReverbLevel(this.rawPtr, index, out level);
    }

    public RESULT setReverbLevel(int index, float level)
    {
      return EventInstance.FMOD_Studio_EventInstance_SetReverbLevel(this.rawPtr, index, level);
    }

    public RESULT getPaused(out bool paused)
    {
      return EventInstance.FMOD_Studio_EventInstance_GetPaused(this.rawPtr, out paused);
    }

    public RESULT setPaused(bool paused)
    {
      return EventInstance.FMOD_Studio_EventInstance_SetPaused(this.rawPtr, paused);
    }

    public RESULT start()
    {
      return EventInstance.FMOD_Studio_EventInstance_Start(this.rawPtr);
    }

    public RESULT stop(STOP_MODE mode)
    {
      return EventInstance.FMOD_Studio_EventInstance_Stop(this.rawPtr, mode);
    }

    public RESULT getTimelinePosition(out int position)
    {
      return EventInstance.FMOD_Studio_EventInstance_GetTimelinePosition(this.rawPtr, out position);
    }

    public RESULT setTimelinePosition(int position)
    {
      return EventInstance.FMOD_Studio_EventInstance_SetTimelinePosition(this.rawPtr, position);
    }

    public RESULT getPlaybackState(out PLAYBACK_STATE state)
    {
      return EventInstance.FMOD_Studio_EventInstance_GetPlaybackState(this.rawPtr, out state);
    }

    public RESULT getChannelGroup(out ChannelGroup group)
    {
      group = (ChannelGroup) null;
      IntPtr group1 = new IntPtr();
      RESULT channelGroup = EventInstance.FMOD_Studio_EventInstance_GetChannelGroup(this.rawPtr, out group1);
      if (channelGroup != RESULT.OK)
        return channelGroup;
      group = new ChannelGroup(group1);
      return channelGroup;
    }

    public RESULT release()
    {
      return EventInstance.FMOD_Studio_EventInstance_Release(this.rawPtr);
    }

    public RESULT isVirtual(out bool virtualState)
    {
      return EventInstance.FMOD_Studio_EventInstance_IsVirtual(this.rawPtr, out virtualState);
    }

    public RESULT getParameter(string name, out ParameterInstance instance)
    {
      instance = (ParameterInstance) null;
      IntPtr parameter1 = new IntPtr();
      RESULT parameter2 = EventInstance.FMOD_Studio_EventInstance_GetParameter(this.rawPtr, Encoding.UTF8.GetBytes(name + "\0"), out parameter1);
      if (parameter2 != RESULT.OK)
        return parameter2;
      instance = new ParameterInstance(parameter1);
      return parameter2;
    }

    public RESULT getParameterCount(out int count)
    {
      return EventInstance.FMOD_Studio_EventInstance_GetParameterCount(this.rawPtr, out count);
    }

    public RESULT getParameterByIndex(int index, out ParameterInstance instance)
    {
      instance = (ParameterInstance) null;
      IntPtr parameter = new IntPtr();
      RESULT parameterByIndex = EventInstance.FMOD_Studio_EventInstance_GetParameterByIndex(this.rawPtr, index, out parameter);
      if (parameterByIndex != RESULT.OK)
        return parameterByIndex;
      instance = new ParameterInstance(parameter);
      return parameterByIndex;
    }

    public RESULT getParameterValue(string name, out float value, out float finalvalue)
    {
      return EventInstance.FMOD_Studio_EventInstance_GetParameterValue(this.rawPtr, Encoding.UTF8.GetBytes(name + "\0"), out value, out finalvalue);
    }

    public RESULT setParameterValue(string name, float value)
    {
      return EventInstance.FMOD_Studio_EventInstance_SetParameterValue(this.rawPtr, Encoding.UTF8.GetBytes(name + "\0"), value);
    }

    public RESULT getParameterValueByIndex(int index, out float value, out float finalvalue)
    {
      return EventInstance.FMOD_Studio_EventInstance_GetParameterValueByIndex(this.rawPtr, index, out value, out finalvalue);
    }

    public RESULT setParameterValueByIndex(int index, float value)
    {
      return EventInstance.FMOD_Studio_EventInstance_SetParameterValueByIndex(this.rawPtr, index, value);
    }

    public RESULT setParameterValuesByIndices(int[] indices, float[] values, int count)
    {
      return EventInstance.FMOD_Studio_EventInstance_SetParameterValuesByIndices(this.rawPtr, indices, values, count);
    }

    public RESULT triggerCue()
    {
      return EventInstance.FMOD_Studio_EventInstance_TriggerCue(this.rawPtr);
    }

    public RESULT setCallback(EVENT_CALLBACK callback, EVENT_CALLBACK_TYPE callbackmask = EVENT_CALLBACK_TYPE.ALL)
    {
      return EventInstance.FMOD_Studio_EventInstance_SetCallback(this.rawPtr, callback, callbackmask);
    }

    public RESULT getUserData(out IntPtr userdata)
    {
      return EventInstance.FMOD_Studio_EventInstance_GetUserData(this.rawPtr, out userdata);
    }

    public RESULT setUserData(IntPtr userdata)
    {
      return EventInstance.FMOD_Studio_EventInstance_SetUserData(this.rawPtr, userdata);
    }

    [DllImport("fmodstudio")]
    private static extern bool FMOD_Studio_EventInstance_IsValid(IntPtr _event);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventInstance_GetDescription(
      IntPtr _event,
      out IntPtr description);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventInstance_GetVolume(
      IntPtr _event,
      out float volume,
      out float finalvolume);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventInstance_SetVolume(
      IntPtr _event,
      float volume);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventInstance_GetPitch(
      IntPtr _event,
      out float pitch,
      out float finalpitch);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventInstance_SetPitch(
      IntPtr _event,
      float pitch);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventInstance_Get3DAttributes(
      IntPtr _event,
      out _3D_ATTRIBUTES attributes);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventInstance_Set3DAttributes(
      IntPtr _event,
      ref _3D_ATTRIBUTES attributes);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventInstance_GetListenerMask(
      IntPtr _event,
      out uint mask);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventInstance_SetListenerMask(
      IntPtr _event,
      uint mask);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventInstance_GetProperty(
      IntPtr _event,
      EVENT_PROPERTY index,
      out float value);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventInstance_SetProperty(
      IntPtr _event,
      EVENT_PROPERTY index,
      float value);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventInstance_GetReverbLevel(
      IntPtr _event,
      int index,
      out float level);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventInstance_SetReverbLevel(
      IntPtr _event,
      int index,
      float level);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventInstance_GetPaused(
      IntPtr _event,
      out bool paused);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventInstance_SetPaused(
      IntPtr _event,
      bool paused);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventInstance_Start(IntPtr _event);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventInstance_Stop(IntPtr _event, STOP_MODE mode);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventInstance_GetTimelinePosition(
      IntPtr _event,
      out int position);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventInstance_SetTimelinePosition(
      IntPtr _event,
      int position);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventInstance_GetPlaybackState(
      IntPtr _event,
      out PLAYBACK_STATE state);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventInstance_GetChannelGroup(
      IntPtr _event,
      out IntPtr group);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventInstance_Release(IntPtr _event);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventInstance_IsVirtual(
      IntPtr _event,
      out bool virtualState);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventInstance_GetParameter(
      IntPtr _event,
      byte[] name,
      out IntPtr parameter);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventInstance_GetParameterByIndex(
      IntPtr _event,
      int index,
      out IntPtr parameter);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventInstance_GetParameterCount(
      IntPtr _event,
      out int count);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventInstance_GetParameterValue(
      IntPtr _event,
      byte[] name,
      out float value,
      out float finalvalue);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventInstance_SetParameterValue(
      IntPtr _event,
      byte[] name,
      float value);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventInstance_GetParameterValueByIndex(
      IntPtr _event,
      int index,
      out float value,
      out float finalvalue);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventInstance_SetParameterValueByIndex(
      IntPtr _event,
      int index,
      float value);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventInstance_SetParameterValuesByIndices(
      IntPtr _event,
      int[] indices,
      float[] values,
      int count);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventInstance_TriggerCue(IntPtr _event);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventInstance_SetCallback(
      IntPtr _event,
      EVENT_CALLBACK callback,
      EVENT_CALLBACK_TYPE callbackmask);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventInstance_GetUserData(
      IntPtr _event,
      out IntPtr userdata);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventInstance_SetUserData(
      IntPtr _event,
      IntPtr userdata);

    public EventInstance(IntPtr raw)
      : base(raw)
    {
    }

    protected override bool isValidInternal()
    {
      return EventInstance.FMOD_Studio_EventInstance_IsValid(this.rawPtr);
    }
  }
}
