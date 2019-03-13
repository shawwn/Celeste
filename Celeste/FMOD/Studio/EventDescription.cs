// Decompiled with JetBrains decompiler
// Type: FMOD.Studio.EventDescription
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FMOD.Studio
{
  public class EventDescription : HandleBase
  {
    public RESULT getID(out Guid id)
    {
      return EventDescription.FMOD_Studio_EventDescription_GetID(this.rawPtr, out id);
    }

    public RESULT getPath(out string path)
    {
      path = (string) null;
      byte[] numArray = new byte[256];
      int retrieved = 0;
      RESULT path1 = EventDescription.FMOD_Studio_EventDescription_GetPath(this.rawPtr, numArray, numArray.Length, out retrieved);
      if (path1 == RESULT.ERR_TRUNCATED)
      {
        numArray = new byte[retrieved];
        path1 = EventDescription.FMOD_Studio_EventDescription_GetPath(this.rawPtr, numArray, numArray.Length, out retrieved);
      }
      if (path1 == RESULT.OK)
        path = Encoding.UTF8.GetString(numArray, 0, retrieved - 1);
      return path1;
    }

    public RESULT getParameterCount(out int count)
    {
      return EventDescription.FMOD_Studio_EventDescription_GetParameterCount(this.rawPtr, out count);
    }

    public RESULT getParameterByIndex(int index, out PARAMETER_DESCRIPTION parameter)
    {
      parameter = new PARAMETER_DESCRIPTION();
      PARAMETER_DESCRIPTION_INTERNAL parameter1;
      RESULT parameterByIndex = EventDescription.FMOD_Studio_EventDescription_GetParameterByIndex(this.rawPtr, index, out parameter1);
      if (parameterByIndex != RESULT.OK)
        return parameterByIndex;
      parameter1.assign(out parameter);
      return parameterByIndex;
    }

    public RESULT getParameter(string name, out PARAMETER_DESCRIPTION parameter)
    {
      parameter = new PARAMETER_DESCRIPTION();
      PARAMETER_DESCRIPTION_INTERNAL parameter1;
      RESULT parameter2 = EventDescription.FMOD_Studio_EventDescription_GetParameter(this.rawPtr, Encoding.UTF8.GetBytes(name + "\0"), out parameter1);
      if (parameter2 != RESULT.OK)
        return parameter2;
      parameter1.assign(out parameter);
      return parameter2;
    }

    public RESULT getUserPropertyCount(out int count)
    {
      return EventDescription.FMOD_Studio_EventDescription_GetUserPropertyCount(this.rawPtr, out count);
    }

    public RESULT getUserPropertyByIndex(int index, out USER_PROPERTY property)
    {
      USER_PROPERTY_INTERNAL property1;
      RESULT userPropertyByIndex = EventDescription.FMOD_Studio_EventDescription_GetUserPropertyByIndex(this.rawPtr, index, out property1);
      if (userPropertyByIndex != RESULT.OK)
      {
        property = new USER_PROPERTY();
        return userPropertyByIndex;
      }
      property = property1.createPublic();
      return RESULT.OK;
    }

    public RESULT getUserProperty(string name, out USER_PROPERTY property)
    {
      USER_PROPERTY_INTERNAL property1;
      RESULT userProperty = EventDescription.FMOD_Studio_EventDescription_GetUserProperty(this.rawPtr, Encoding.UTF8.GetBytes(name + "\0"), out property1);
      if (userProperty != RESULT.OK)
      {
        property = new USER_PROPERTY();
        return userProperty;
      }
      property = property1.createPublic();
      return RESULT.OK;
    }

    public RESULT getLength(out int length)
    {
      return EventDescription.FMOD_Studio_EventDescription_GetLength(this.rawPtr, out length);
    }

    public RESULT getMinimumDistance(out float distance)
    {
      return EventDescription.FMOD_Studio_EventDescription_GetMinimumDistance(this.rawPtr, out distance);
    }

    public RESULT getMaximumDistance(out float distance)
    {
      return EventDescription.FMOD_Studio_EventDescription_GetMaximumDistance(this.rawPtr, out distance);
    }

    public RESULT getSoundSize(out float size)
    {
      return EventDescription.FMOD_Studio_EventDescription_GetSoundSize(this.rawPtr, out size);
    }

    public RESULT isSnapshot(out bool snapshot)
    {
      return EventDescription.FMOD_Studio_EventDescription_IsSnapshot(this.rawPtr, out snapshot);
    }

    public RESULT isOneshot(out bool oneshot)
    {
      return EventDescription.FMOD_Studio_EventDescription_IsOneshot(this.rawPtr, out oneshot);
    }

    public RESULT isStream(out bool isStream)
    {
      return EventDescription.FMOD_Studio_EventDescription_IsStream(this.rawPtr, out isStream);
    }

    public RESULT is3D(out bool is3D)
    {
      return EventDescription.FMOD_Studio_EventDescription_Is3D(this.rawPtr, out is3D);
    }

    public RESULT hasCue(out bool cue)
    {
      return EventDescription.FMOD_Studio_EventDescription_HasCue(this.rawPtr, out cue);
    }

    public RESULT createInstance(out EventInstance instance)
    {
      instance = (EventInstance) null;
      IntPtr instance1 = new IntPtr();
      RESULT instance2 = EventDescription.FMOD_Studio_EventDescription_CreateInstance(this.rawPtr, out instance1);
      if (instance2 != RESULT.OK)
        return instance2;
      instance = new EventInstance(instance1);
      return instance2;
    }

    public RESULT getInstanceCount(out int count)
    {
      return EventDescription.FMOD_Studio_EventDescription_GetInstanceCount(this.rawPtr, out count);
    }

    public RESULT getInstanceList(out EventInstance[] array)
    {
      array = (EventInstance[]) null;
      int count1;
      RESULT instanceCount = EventDescription.FMOD_Studio_EventDescription_GetInstanceCount(this.rawPtr, out count1);
      if (instanceCount != RESULT.OK)
        return instanceCount;
      if (count1 == 0)
      {
        array = new EventInstance[0];
        return instanceCount;
      }
      IntPtr[] array1 = new IntPtr[count1];
      int count2;
      RESULT instanceList = EventDescription.FMOD_Studio_EventDescription_GetInstanceList(this.rawPtr, array1, count1, out count2);
      if (instanceList != RESULT.OK)
        return instanceList;
      if (count2 > count1)
        count2 = count1;
      array = new EventInstance[count2];
      for (int index = 0; index < count2; ++index)
        array[index] = new EventInstance(array1[index]);
      return RESULT.OK;
    }

    public RESULT loadSampleData()
    {
      return EventDescription.FMOD_Studio_EventDescription_LoadSampleData(this.rawPtr);
    }

    public RESULT unloadSampleData()
    {
      return EventDescription.FMOD_Studio_EventDescription_UnloadSampleData(this.rawPtr);
    }

    public RESULT getSampleLoadingState(out LOADING_STATE state)
    {
      return EventDescription.FMOD_Studio_EventDescription_GetSampleLoadingState(this.rawPtr, out state);
    }

    public RESULT releaseAllInstances()
    {
      return EventDescription.FMOD_Studio_EventDescription_ReleaseAllInstances(this.rawPtr);
    }

    public RESULT setCallback(EVENT_CALLBACK callback, EVENT_CALLBACK_TYPE callbackmask = EVENT_CALLBACK_TYPE.ALL)
    {
      return EventDescription.FMOD_Studio_EventDescription_SetCallback(this.rawPtr, callback, callbackmask);
    }

    public RESULT getUserData(out IntPtr userdata)
    {
      return EventDescription.FMOD_Studio_EventDescription_GetUserData(this.rawPtr, out userdata);
    }

    public RESULT setUserData(IntPtr userdata)
    {
      return EventDescription.FMOD_Studio_EventDescription_SetUserData(this.rawPtr, userdata);
    }

    [DllImport("fmodstudio")]
    private static extern bool FMOD_Studio_EventDescription_IsValid(IntPtr eventdescription);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventDescription_GetID(
      IntPtr eventdescription,
      out Guid id);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventDescription_GetPath(
      IntPtr eventdescription,
      [Out] byte[] path,
      int size,
      out int retrieved);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventDescription_GetParameterCount(
      IntPtr eventdescription,
      out int count);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventDescription_GetParameterByIndex(
      IntPtr eventdescription,
      int index,
      out PARAMETER_DESCRIPTION_INTERNAL parameter);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventDescription_GetParameter(
      IntPtr eventdescription,
      byte[] name,
      out PARAMETER_DESCRIPTION_INTERNAL parameter);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventDescription_GetUserPropertyCount(
      IntPtr eventdescription,
      out int count);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventDescription_GetUserPropertyByIndex(
      IntPtr eventdescription,
      int index,
      out USER_PROPERTY_INTERNAL property);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventDescription_GetUserProperty(
      IntPtr eventdescription,
      byte[] name,
      out USER_PROPERTY_INTERNAL property);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventDescription_GetLength(
      IntPtr eventdescription,
      out int length);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventDescription_GetMinimumDistance(
      IntPtr eventdescription,
      out float distance);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventDescription_GetMaximumDistance(
      IntPtr eventdescription,
      out float distance);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventDescription_GetSoundSize(
      IntPtr eventdescription,
      out float size);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventDescription_IsSnapshot(
      IntPtr eventdescription,
      out bool snapshot);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventDescription_IsOneshot(
      IntPtr eventdescription,
      out bool oneshot);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventDescription_IsStream(
      IntPtr eventdescription,
      out bool isStream);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventDescription_Is3D(
      IntPtr eventdescription,
      out bool is3D);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventDescription_HasCue(
      IntPtr eventdescription,
      out bool cue);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventDescription_CreateInstance(
      IntPtr eventdescription,
      out IntPtr instance);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventDescription_GetInstanceCount(
      IntPtr eventdescription,
      out int count);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventDescription_GetInstanceList(
      IntPtr eventdescription,
      IntPtr[] array,
      int capacity,
      out int count);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventDescription_LoadSampleData(
      IntPtr eventdescription);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventDescription_UnloadSampleData(
      IntPtr eventdescription);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventDescription_GetSampleLoadingState(
      IntPtr eventdescription,
      out LOADING_STATE state);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventDescription_ReleaseAllInstances(
      IntPtr eventdescription);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventDescription_SetCallback(
      IntPtr eventdescription,
      EVENT_CALLBACK callback,
      EVENT_CALLBACK_TYPE callbackmask);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventDescription_GetUserData(
      IntPtr eventdescription,
      out IntPtr userdata);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_EventDescription_SetUserData(
      IntPtr eventdescription,
      IntPtr userdata);

    public EventDescription(IntPtr raw)
      : base(raw)
    {
    }

    protected override bool isValidInternal()
    {
      return EventDescription.FMOD_Studio_EventDescription_IsValid(this.rawPtr);
    }
  }
}
