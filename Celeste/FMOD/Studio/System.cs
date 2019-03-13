// Decompiled with JetBrains decompiler
// Type: FMOD.Studio.System
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FMOD.Studio
{
  public class System : HandleBase
  {
    public static RESULT create(out FMOD.Studio.System studiosystem)
    {
      studiosystem = (FMOD.Studio.System) null;
      IntPtr studiosystem1;
      RESULT result = FMOD.Studio.System.FMOD_Studio_System_Create(out studiosystem1, 69636U);
      if (result != RESULT.OK)
        return result;
      studiosystem = new FMOD.Studio.System(studiosystem1);
      return result;
    }

    public RESULT setAdvancedSettings(ADVANCEDSETTINGS settings)
    {
      settings.cbsize = Marshal.SizeOf(typeof (ADVANCEDSETTINGS));
      return FMOD.Studio.System.FMOD_Studio_System_SetAdvancedSettings(this.rawPtr, ref settings);
    }

    public RESULT getAdvancedSettings(out ADVANCEDSETTINGS settings)
    {
      settings.cbsize = Marshal.SizeOf(typeof (ADVANCEDSETTINGS));
      return FMOD.Studio.System.FMOD_Studio_System_GetAdvancedSettings(this.rawPtr, out settings);
    }

    public RESULT initialize(
      int maxchannels,
      INITFLAGS studioFlags,
      FMOD.INITFLAGS flags,
      IntPtr extradriverdata)
    {
      return FMOD.Studio.System.FMOD_Studio_System_Initialize(this.rawPtr, maxchannels, studioFlags, flags, extradriverdata);
    }

    public RESULT release()
    {
      return FMOD.Studio.System.FMOD_Studio_System_Release(this.rawPtr);
    }

    public RESULT update()
    {
      return FMOD.Studio.System.FMOD_Studio_System_Update(this.rawPtr);
    }

    public RESULT getLowLevelSystem(out FMOD.System system)
    {
      system = (FMOD.System) null;
      IntPtr system1 = new IntPtr();
      RESULT lowLevelSystem = FMOD.Studio.System.FMOD_Studio_System_GetLowLevelSystem(this.rawPtr, out system1);
      if (lowLevelSystem != RESULT.OK)
        return lowLevelSystem;
      system = new FMOD.System(system1);
      return lowLevelSystem;
    }

    public RESULT getEvent(string path, out EventDescription _event)
    {
      _event = (EventDescription) null;
      IntPtr description = new IntPtr();
      RESULT result = FMOD.Studio.System.FMOD_Studio_System_GetEvent(this.rawPtr, Encoding.UTF8.GetBytes(path + "\0"), out description);
      if (result != RESULT.OK)
        return result;
      _event = new EventDescription(description);
      return result;
    }

    public RESULT getBus(string path, out Bus bus)
    {
      bus = (Bus) null;
      IntPtr bus1 = new IntPtr();
      RESULT bus2 = FMOD.Studio.System.FMOD_Studio_System_GetBus(this.rawPtr, Encoding.UTF8.GetBytes(path + "\0"), out bus1);
      if (bus2 != RESULT.OK)
        return bus2;
      bus = new Bus(bus1);
      return bus2;
    }

    public RESULT getVCA(string path, out VCA vca)
    {
      vca = (VCA) null;
      IntPtr vca1 = new IntPtr();
      RESULT vca2 = FMOD.Studio.System.FMOD_Studio_System_GetVCA(this.rawPtr, Encoding.UTF8.GetBytes(path + "\0"), out vca1);
      if (vca2 != RESULT.OK)
        return vca2;
      vca = new VCA(vca1);
      return vca2;
    }

    public RESULT getBank(string path, out Bank bank)
    {
      bank = (Bank) null;
      IntPtr bank1 = new IntPtr();
      RESULT bank2 = FMOD.Studio.System.FMOD_Studio_System_GetBank(this.rawPtr, Encoding.UTF8.GetBytes(path + "\0"), out bank1);
      if (bank2 != RESULT.OK)
        return bank2;
      bank = new Bank(bank1);
      return bank2;
    }

    public RESULT getEventByID(Guid guid, out EventDescription _event)
    {
      _event = (EventDescription) null;
      IntPtr description = new IntPtr();
      RESULT eventById = FMOD.Studio.System.FMOD_Studio_System_GetEventByID(this.rawPtr, ref guid, out description);
      if (eventById != RESULT.OK)
        return eventById;
      _event = new EventDescription(description);
      return eventById;
    }

    public RESULT getBusByID(Guid guid, out Bus bus)
    {
      bus = (Bus) null;
      IntPtr bus1 = new IntPtr();
      RESULT busById = FMOD.Studio.System.FMOD_Studio_System_GetBusByID(this.rawPtr, ref guid, out bus1);
      if (busById != RESULT.OK)
        return busById;
      bus = new Bus(bus1);
      return busById;
    }

    public RESULT getVCAByID(Guid guid, out VCA vca)
    {
      vca = (VCA) null;
      IntPtr vca1 = new IntPtr();
      RESULT vcaById = FMOD.Studio.System.FMOD_Studio_System_GetVCAByID(this.rawPtr, ref guid, out vca1);
      if (vcaById != RESULT.OK)
        return vcaById;
      vca = new VCA(vca1);
      return vcaById;
    }

    public RESULT getBankByID(Guid guid, out Bank bank)
    {
      bank = (Bank) null;
      IntPtr bank1 = new IntPtr();
      RESULT bankById = FMOD.Studio.System.FMOD_Studio_System_GetBankByID(this.rawPtr, ref guid, out bank1);
      if (bankById != RESULT.OK)
        return bankById;
      bank = new Bank(bank1);
      return bankById;
    }

    public RESULT getSoundInfo(string key, out SOUND_INFO info)
    {
      SOUND_INFO_INTERNAL info1;
      RESULT soundInfo = FMOD.Studio.System.FMOD_Studio_System_GetSoundInfo(this.rawPtr, Encoding.UTF8.GetBytes(key + "\0"), out info1);
      if (soundInfo != RESULT.OK)
      {
        info = new SOUND_INFO();
        return soundInfo;
      }
      info1.assign(out info);
      return soundInfo;
    }

    public RESULT lookupID(string path, out Guid guid)
    {
      return FMOD.Studio.System.FMOD_Studio_System_LookupID(this.rawPtr, Encoding.UTF8.GetBytes(path + "\0"), out guid);
    }

    public RESULT lookupPath(Guid guid, out string path)
    {
      path = (string) null;
      byte[] numArray = new byte[256];
      int retrieved = 0;
      RESULT result = FMOD.Studio.System.FMOD_Studio_System_LookupPath(this.rawPtr, ref guid, numArray, numArray.Length, out retrieved);
      if (result == RESULT.ERR_TRUNCATED)
      {
        numArray = new byte[retrieved];
        result = FMOD.Studio.System.FMOD_Studio_System_LookupPath(this.rawPtr, ref guid, numArray, numArray.Length, out retrieved);
      }
      if (result == RESULT.OK)
        path = Encoding.UTF8.GetString(numArray, 0, retrieved - 1);
      return result;
    }

    public RESULT getNumListeners(out int numlisteners)
    {
      return FMOD.Studio.System.FMOD_Studio_System_GetNumListeners(this.rawPtr, out numlisteners);
    }

    public RESULT setNumListeners(int numlisteners)
    {
      return FMOD.Studio.System.FMOD_Studio_System_SetNumListeners(this.rawPtr, numlisteners);
    }

    public RESULT getListenerAttributes(int listener, out _3D_ATTRIBUTES attributes)
    {
      return FMOD.Studio.System.FMOD_Studio_System_GetListenerAttributes(this.rawPtr, listener, out attributes);
    }

    public RESULT setListenerAttributes(int listener, _3D_ATTRIBUTES attributes)
    {
      return FMOD.Studio.System.FMOD_Studio_System_SetListenerAttributes(this.rawPtr, listener, ref attributes);
    }

    public RESULT getListenerWeight(int listener, out float weight)
    {
      return FMOD.Studio.System.FMOD_Studio_System_GetListenerWeight(this.rawPtr, listener, out weight);
    }

    public RESULT setListenerWeight(int listener, float weight)
    {
      return FMOD.Studio.System.FMOD_Studio_System_SetListenerWeight(this.rawPtr, listener, weight);
    }

    public RESULT loadBankFile(string name, LOAD_BANK_FLAGS flags, out Bank bank)
    {
      bank = (Bank) null;
      IntPtr bank1 = new IntPtr();
      RESULT result = FMOD.Studio.System.FMOD_Studio_System_LoadBankFile(this.rawPtr, Encoding.UTF8.GetBytes(name + "\0"), flags, out bank1);
      if (result != RESULT.OK)
        return result;
      bank = new Bank(bank1);
      return result;
    }

    public RESULT loadBankMemory(byte[] buffer, LOAD_BANK_FLAGS flags, out Bank bank)
    {
      bank = (Bank) null;
      IntPtr bank1 = new IntPtr();
      RESULT result = FMOD.Studio.System.FMOD_Studio_System_LoadBankMemory(this.rawPtr, buffer, buffer.Length, LOAD_MEMORY_MODE.LOAD_MEMORY, flags, out bank1);
      if (result != RESULT.OK)
        return result;
      bank = new Bank(bank1);
      return result;
    }

    public RESULT loadBankCustom(BANK_INFO info, LOAD_BANK_FLAGS flags, out Bank bank)
    {
      bank = (Bank) null;
      info.size = Marshal.SizeOf((object) info);
      IntPtr bank1 = new IntPtr();
      RESULT result = FMOD.Studio.System.FMOD_Studio_System_LoadBankCustom(this.rawPtr, ref info, flags, out bank1);
      if (result != RESULT.OK)
        return result;
      bank = new Bank(bank1);
      return result;
    }

    public RESULT unloadAll()
    {
      return FMOD.Studio.System.FMOD_Studio_System_UnloadAll(this.rawPtr);
    }

    public RESULT flushCommands()
    {
      return FMOD.Studio.System.FMOD_Studio_System_FlushCommands(this.rawPtr);
    }

    public RESULT flushSampleLoading()
    {
      return FMOD.Studio.System.FMOD_Studio_System_FlushSampleLoading(this.rawPtr);
    }

    public RESULT startCommandCapture(string path, COMMANDCAPTURE_FLAGS flags)
    {
      return FMOD.Studio.System.FMOD_Studio_System_StartCommandCapture(this.rawPtr, Encoding.UTF8.GetBytes(path + "\0"), flags);
    }

    public RESULT stopCommandCapture()
    {
      return FMOD.Studio.System.FMOD_Studio_System_StopCommandCapture(this.rawPtr);
    }

    public RESULT loadCommandReplay(
      string path,
      COMMANDREPLAY_FLAGS flags,
      out CommandReplay replay)
    {
      replay = (CommandReplay) null;
      IntPtr commandReplay = new IntPtr();
      int num = (int) FMOD.Studio.System.FMOD_Studio_System_LoadCommandReplay(this.rawPtr, Encoding.UTF8.GetBytes(path + "\0"), flags, out commandReplay);
      if (num != 0)
        return (RESULT) num;
      replay = new CommandReplay(commandReplay);
      return (RESULT) num;
    }

    public RESULT getBankCount(out int count)
    {
      return FMOD.Studio.System.FMOD_Studio_System_GetBankCount(this.rawPtr, out count);
    }

    public RESULT getBankList(out Bank[] array)
    {
      array = (Bank[]) null;
      int count1;
      RESULT bankCount = FMOD.Studio.System.FMOD_Studio_System_GetBankCount(this.rawPtr, out count1);
      if (bankCount != RESULT.OK)
        return bankCount;
      if (count1 == 0)
      {
        array = new Bank[0];
        return bankCount;
      }
      IntPtr[] array1 = new IntPtr[count1];
      int count2;
      RESULT bankList = FMOD.Studio.System.FMOD_Studio_System_GetBankList(this.rawPtr, array1, count1, out count2);
      if (bankList != RESULT.OK)
        return bankList;
      if (count2 > count1)
        count2 = count1;
      array = new Bank[count2];
      for (int index = 0; index < count2; ++index)
        array[index] = new Bank(array1[index]);
      return RESULT.OK;
    }

    public RESULT getCPUUsage(out CPU_USAGE usage)
    {
      return FMOD.Studio.System.FMOD_Studio_System_GetCPUUsage(this.rawPtr, out usage);
    }

    public RESULT getBufferUsage(out BUFFER_USAGE usage)
    {
      return FMOD.Studio.System.FMOD_Studio_System_GetBufferUsage(this.rawPtr, out usage);
    }

    public RESULT resetBufferUsage()
    {
      return FMOD.Studio.System.FMOD_Studio_System_ResetBufferUsage(this.rawPtr);
    }

    public RESULT setCallback(SYSTEM_CALLBACK callback, SYSTEM_CALLBACK_TYPE callbackmask = SYSTEM_CALLBACK_TYPE.ALL)
    {
      return FMOD.Studio.System.FMOD_Studio_System_SetCallback(this.rawPtr, callback, callbackmask);
    }

    public RESULT getUserData(out IntPtr userdata)
    {
      return FMOD.Studio.System.FMOD_Studio_System_GetUserData(this.rawPtr, out userdata);
    }

    public RESULT setUserData(IntPtr userdata)
    {
      return FMOD.Studio.System.FMOD_Studio_System_SetUserData(this.rawPtr, userdata);
    }

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_System_Create(
      out IntPtr studiosystem,
      uint headerversion);

    [DllImport("fmodstudio")]
    private static extern bool FMOD_Studio_System_IsValid(IntPtr studiosystem);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_System_SetAdvancedSettings(
      IntPtr studiosystem,
      ref ADVANCEDSETTINGS settings);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_System_GetAdvancedSettings(
      IntPtr studiosystem,
      out ADVANCEDSETTINGS settings);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_System_Initialize(
      IntPtr studiosystem,
      int maxchannels,
      INITFLAGS studioFlags,
      FMOD.INITFLAGS flags,
      IntPtr extradriverdata);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_System_Release(IntPtr studiosystem);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_System_Update(IntPtr studiosystem);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_System_GetLowLevelSystem(
      IntPtr studiosystem,
      out IntPtr system);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_System_GetEvent(
      IntPtr studiosystem,
      byte[] path,
      out IntPtr description);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_System_GetBus(
      IntPtr studiosystem,
      byte[] path,
      out IntPtr bus);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_System_GetVCA(
      IntPtr studiosystem,
      byte[] path,
      out IntPtr vca);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_System_GetBank(
      IntPtr studiosystem,
      byte[] path,
      out IntPtr bank);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_System_GetEventByID(
      IntPtr studiosystem,
      ref Guid guid,
      out IntPtr description);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_System_GetBusByID(
      IntPtr studiosystem,
      ref Guid guid,
      out IntPtr bus);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_System_GetVCAByID(
      IntPtr studiosystem,
      ref Guid guid,
      out IntPtr vca);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_System_GetBankByID(
      IntPtr studiosystem,
      ref Guid guid,
      out IntPtr bank);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_System_GetSoundInfo(
      IntPtr studiosystem,
      byte[] key,
      out SOUND_INFO_INTERNAL info);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_System_LookupID(
      IntPtr studiosystem,
      byte[] path,
      out Guid guid);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_System_LookupPath(
      IntPtr studiosystem,
      ref Guid guid,
      [Out] byte[] path,
      int size,
      out int retrieved);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_System_GetNumListeners(
      IntPtr studiosystem,
      out int numlisteners);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_System_SetNumListeners(
      IntPtr studiosystem,
      int numlisteners);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_System_GetListenerAttributes(
      IntPtr studiosystem,
      int listener,
      out _3D_ATTRIBUTES attributes);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_System_SetListenerAttributes(
      IntPtr studiosystem,
      int listener,
      ref _3D_ATTRIBUTES attributes);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_System_GetListenerWeight(
      IntPtr studiosystem,
      int listener,
      out float weight);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_System_SetListenerWeight(
      IntPtr studiosystem,
      int listener,
      float weight);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_System_LoadBankFile(
      IntPtr studiosystem,
      byte[] filename,
      LOAD_BANK_FLAGS flags,
      out IntPtr bank);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_System_LoadBankMemory(
      IntPtr studiosystem,
      byte[] buffer,
      int length,
      LOAD_MEMORY_MODE mode,
      LOAD_BANK_FLAGS flags,
      out IntPtr bank);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_System_LoadBankCustom(
      IntPtr studiosystem,
      ref BANK_INFO info,
      LOAD_BANK_FLAGS flags,
      out IntPtr bank);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_System_UnloadAll(IntPtr studiosystem);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_System_FlushCommands(IntPtr studiosystem);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_System_FlushSampleLoading(IntPtr studiosystem);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_System_StartCommandCapture(
      IntPtr studiosystem,
      byte[] path,
      COMMANDCAPTURE_FLAGS flags);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_System_StopCommandCapture(IntPtr studiosystem);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_System_LoadCommandReplay(
      IntPtr studiosystem,
      byte[] path,
      COMMANDREPLAY_FLAGS flags,
      out IntPtr commandReplay);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_System_GetBankCount(
      IntPtr studiosystem,
      out int count);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_System_GetBankList(
      IntPtr studiosystem,
      IntPtr[] array,
      int capacity,
      out int count);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_System_GetCPUUsage(
      IntPtr studiosystem,
      out CPU_USAGE usage);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_System_GetBufferUsage(
      IntPtr studiosystem,
      out BUFFER_USAGE usage);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_System_ResetBufferUsage(IntPtr studiosystem);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_System_SetCallback(
      IntPtr studiosystem,
      SYSTEM_CALLBACK callback,
      SYSTEM_CALLBACK_TYPE callbackmask);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_System_GetUserData(
      IntPtr studiosystem,
      out IntPtr userdata);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_System_SetUserData(
      IntPtr studiosystem,
      IntPtr userdata);

    public System(IntPtr raw)
      : base(raw)
    {
    }

    protected override bool isValidInternal()
    {
      return FMOD.Studio.System.FMOD_Studio_System_IsValid(this.rawPtr);
    }
  }
}
