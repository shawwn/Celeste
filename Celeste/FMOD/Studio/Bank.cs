// Decompiled with JetBrains decompiler
// Type: FMOD.Studio.Bank
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FMOD.Studio
{
  public class Bank : HandleBase
  {
    public RESULT getID(out Guid id)
    {
      return Bank.FMOD_Studio_Bank_GetID(this.rawPtr, out id);
    }

    public RESULT getPath(out string path)
    {
      path = (string) null;
      byte[] numArray = new byte[256];
      int retrieved = 0;
      RESULT path1 = Bank.FMOD_Studio_Bank_GetPath(this.rawPtr, numArray, numArray.Length, out retrieved);
      if (path1 == RESULT.ERR_TRUNCATED)
      {
        numArray = new byte[retrieved];
        path1 = Bank.FMOD_Studio_Bank_GetPath(this.rawPtr, numArray, numArray.Length, out retrieved);
      }
      if (path1 == RESULT.OK)
        path = Encoding.UTF8.GetString(numArray, 0, retrieved - 1);
      return path1;
    }

    public RESULT unload()
    {
      RESULT result = Bank.FMOD_Studio_Bank_Unload(this.rawPtr);
      if (result != RESULT.OK)
        return result;
      this.rawPtr = IntPtr.Zero;
      return RESULT.OK;
    }

    public RESULT loadSampleData()
    {
      return Bank.FMOD_Studio_Bank_LoadSampleData(this.rawPtr);
    }

    public RESULT unloadSampleData()
    {
      return Bank.FMOD_Studio_Bank_UnloadSampleData(this.rawPtr);
    }

    public RESULT getLoadingState(out LOADING_STATE state)
    {
      return Bank.FMOD_Studio_Bank_GetLoadingState(this.rawPtr, out state);
    }

    public RESULT getSampleLoadingState(out LOADING_STATE state)
    {
      return Bank.FMOD_Studio_Bank_GetSampleLoadingState(this.rawPtr, out state);
    }

    public RESULT getStringCount(out int count)
    {
      return Bank.FMOD_Studio_Bank_GetStringCount(this.rawPtr, out count);
    }

    public RESULT getStringInfo(int index, out Guid id, out string path)
    {
      path = (string) null;
      byte[] numArray = new byte[256];
      int retrieved = 0;
      RESULT stringInfo = Bank.FMOD_Studio_Bank_GetStringInfo(this.rawPtr, index, out id, numArray, numArray.Length, out retrieved);
      if (stringInfo == RESULT.ERR_TRUNCATED)
      {
        numArray = new byte[retrieved];
        stringInfo = Bank.FMOD_Studio_Bank_GetStringInfo(this.rawPtr, index, out id, numArray, numArray.Length, out retrieved);
      }
      if (stringInfo == RESULT.OK)
        path = Encoding.UTF8.GetString(numArray, 0, retrieved - 1);
      return RESULT.OK;
    }

    public RESULT getEventCount(out int count)
    {
      return Bank.FMOD_Studio_Bank_GetEventCount(this.rawPtr, out count);
    }

    public RESULT getEventList(out EventDescription[] array)
    {
      array = (EventDescription[]) null;
      int count1;
      RESULT eventCount = Bank.FMOD_Studio_Bank_GetEventCount(this.rawPtr, out count1);
      if (eventCount != RESULT.OK)
        return eventCount;
      if (count1 == 0)
      {
        array = new EventDescription[0];
        return eventCount;
      }
      IntPtr[] array1 = new IntPtr[count1];
      int count2;
      RESULT eventList = Bank.FMOD_Studio_Bank_GetEventList(this.rawPtr, array1, count1, out count2);
      if (eventList != RESULT.OK)
        return eventList;
      if (count2 > count1)
        count2 = count1;
      array = new EventDescription[count2];
      for (int index = 0; index < count2; ++index)
        array[index] = new EventDescription(array1[index]);
      return RESULT.OK;
    }

    public RESULT getBusCount(out int count)
    {
      return Bank.FMOD_Studio_Bank_GetBusCount(this.rawPtr, out count);
    }

    public RESULT getBusList(out Bus[] array)
    {
      array = (Bus[]) null;
      int count1;
      RESULT busCount = Bank.FMOD_Studio_Bank_GetBusCount(this.rawPtr, out count1);
      if (busCount != RESULT.OK)
        return busCount;
      if (count1 == 0)
      {
        array = new Bus[0];
        return busCount;
      }
      IntPtr[] array1 = new IntPtr[count1];
      int count2;
      RESULT busList = Bank.FMOD_Studio_Bank_GetBusList(this.rawPtr, array1, count1, out count2);
      if (busList != RESULT.OK)
        return busList;
      if (count2 > count1)
        count2 = count1;
      array = new Bus[count2];
      for (int index = 0; index < count2; ++index)
        array[index] = new Bus(array1[index]);
      return RESULT.OK;
    }

    public RESULT getVCACount(out int count)
    {
      return Bank.FMOD_Studio_Bank_GetVCACount(this.rawPtr, out count);
    }

    public RESULT getVCAList(out VCA[] array)
    {
      array = (VCA[]) null;
      int count1;
      RESULT vcaCount = Bank.FMOD_Studio_Bank_GetVCACount(this.rawPtr, out count1);
      if (vcaCount != RESULT.OK)
        return vcaCount;
      if (count1 == 0)
      {
        array = new VCA[0];
        return vcaCount;
      }
      IntPtr[] array1 = new IntPtr[count1];
      int count2;
      RESULT vcaList = Bank.FMOD_Studio_Bank_GetVCAList(this.rawPtr, array1, count1, out count2);
      if (vcaList != RESULT.OK)
        return vcaList;
      if (count2 > count1)
        count2 = count1;
      array = new VCA[count2];
      for (int index = 0; index < count2; ++index)
        array[index] = new VCA(array1[index]);
      return RESULT.OK;
    }

    public RESULT getUserData(out IntPtr userdata)
    {
      return Bank.FMOD_Studio_Bank_GetUserData(this.rawPtr, out userdata);
    }

    public RESULT setUserData(IntPtr userdata)
    {
      return Bank.FMOD_Studio_Bank_SetUserData(this.rawPtr, userdata);
    }

    [DllImport("fmodstudio")]
    private static extern bool FMOD_Studio_Bank_IsValid(IntPtr bank);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_Bank_GetID(IntPtr bank, out Guid id);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_Bank_GetPath(
      IntPtr bank,
      [Out] byte[] path,
      int size,
      out int retrieved);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_Bank_Unload(IntPtr bank);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_Bank_LoadSampleData(IntPtr bank);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_Bank_UnloadSampleData(IntPtr bank);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_Bank_GetLoadingState(
      IntPtr bank,
      out LOADING_STATE state);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_Bank_GetSampleLoadingState(
      IntPtr bank,
      out LOADING_STATE state);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_Bank_GetStringCount(IntPtr bank, out int count);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_Bank_GetStringInfo(
      IntPtr bank,
      int index,
      out Guid id,
      [Out] byte[] path,
      int size,
      out int retrieved);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_Bank_GetEventCount(IntPtr bank, out int count);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_Bank_GetEventList(
      IntPtr bank,
      IntPtr[] array,
      int capacity,
      out int count);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_Bank_GetBusCount(IntPtr bank, out int count);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_Bank_GetBusList(
      IntPtr bank,
      IntPtr[] array,
      int capacity,
      out int count);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_Bank_GetVCACount(IntPtr bank, out int count);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_Bank_GetVCAList(
      IntPtr bank,
      IntPtr[] array,
      int capacity,
      out int count);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_Bank_GetUserData(
      IntPtr studiosystem,
      out IntPtr userdata);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_Bank_SetUserData(
      IntPtr studiosystem,
      IntPtr userdata);

    public Bank(IntPtr raw)
      : base(raw)
    {
    }

    protected override bool isValidInternal()
    {
      return Bank.FMOD_Studio_Bank_IsValid(this.rawPtr);
    }
  }
}
