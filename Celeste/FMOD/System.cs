// Decompiled with JetBrains decompiler
// Type: FMOD.System
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FMOD
{
  public class System : HandleBase
  {
    public RESULT release()
    {
      int num = (int) FMOD.System.FMOD_System_Release(this.rawPtr);
      if (num != 0)
        return (RESULT) num;
      this.rawPtr = IntPtr.Zero;
      return (RESULT) num;
    }

    public RESULT setOutput(OUTPUTTYPE output)
    {
      return FMOD.System.FMOD_System_SetOutput(this.rawPtr, output);
    }

    public RESULT getOutput(out OUTPUTTYPE output)
    {
      return FMOD.System.FMOD_System_GetOutput(this.rawPtr, out output);
    }

    public RESULT getNumDrivers(out int numdrivers)
    {
      return FMOD.System.FMOD_System_GetNumDrivers(this.rawPtr, out numdrivers);
    }

    public RESULT getDriverInfo(
      int id,
      StringBuilder name,
      int namelen,
      out Guid guid,
      out int systemrate,
      out SPEAKERMODE speakermode,
      out int speakermodechannels)
    {
      IntPtr num = Marshal.AllocHGlobal(name.Capacity);
      int driverInfo = (int) FMOD.System.FMOD_System_GetDriverInfo(this.rawPtr, id, num, namelen, out guid, out systemrate, out speakermode, out speakermodechannels);
      StringMarshalHelper.NativeToBuilder(name, num);
      Marshal.FreeHGlobal(num);
      return (RESULT) driverInfo;
    }

    public RESULT setDriver(int driver)
    {
      return FMOD.System.FMOD_System_SetDriver(this.rawPtr, driver);
    }

    public RESULT getDriver(out int driver)
    {
      return FMOD.System.FMOD_System_GetDriver(this.rawPtr, out driver);
    }

    public RESULT setSoftwareChannels(int numsoftwarechannels)
    {
      return FMOD.System.FMOD_System_SetSoftwareChannels(this.rawPtr, numsoftwarechannels);
    }

    public RESULT getSoftwareChannels(out int numsoftwarechannels)
    {
      return FMOD.System.FMOD_System_GetSoftwareChannels(this.rawPtr, out numsoftwarechannels);
    }

    public RESULT setSoftwareFormat(
      int samplerate,
      SPEAKERMODE speakermode,
      int numrawspeakers)
    {
      return FMOD.System.FMOD_System_SetSoftwareFormat(this.rawPtr, samplerate, speakermode, numrawspeakers);
    }

    public RESULT getSoftwareFormat(
      out int samplerate,
      out SPEAKERMODE speakermode,
      out int numrawspeakers)
    {
      return FMOD.System.FMOD_System_GetSoftwareFormat(this.rawPtr, out samplerate, out speakermode, out numrawspeakers);
    }

    public RESULT setDSPBufferSize(uint bufferlength, int numbuffers)
    {
      return FMOD.System.FMOD_System_SetDSPBufferSize(this.rawPtr, bufferlength, numbuffers);
    }

    public RESULT getDSPBufferSize(out uint bufferlength, out int numbuffers)
    {
      return FMOD.System.FMOD_System_GetDSPBufferSize(this.rawPtr, out bufferlength, out numbuffers);
    }

    public RESULT setFileSystem(
      FILE_OPENCALLBACK useropen,
      FILE_CLOSECALLBACK userclose,
      FILE_READCALLBACK userread,
      FILE_SEEKCALLBACK userseek,
      FILE_ASYNCREADCALLBACK userasyncread,
      FILE_ASYNCCANCELCALLBACK userasynccancel,
      int blockalign)
    {
      return FMOD.System.FMOD_System_SetFileSystem(this.rawPtr, useropen, userclose, userread, userseek, userasyncread, userasynccancel, blockalign);
    }

    public RESULT attachFileSystem(
      FILE_OPENCALLBACK useropen,
      FILE_CLOSECALLBACK userclose,
      FILE_READCALLBACK userread,
      FILE_SEEKCALLBACK userseek)
    {
      return FMOD.System.FMOD_System_AttachFileSystem(this.rawPtr, useropen, userclose, userread, userseek);
    }

    public RESULT setAdvancedSettings(ref ADVANCEDSETTINGS settings)
    {
      settings.cbSize = Marshal.SizeOf((object) settings);
      return FMOD.System.FMOD_System_SetAdvancedSettings(this.rawPtr, ref settings);
    }

    public RESULT getAdvancedSettings(ref ADVANCEDSETTINGS settings)
    {
      settings.cbSize = Marshal.SizeOf((object) settings);
      return FMOD.System.FMOD_System_GetAdvancedSettings(this.rawPtr, ref settings);
    }

    public RESULT setCallback(SYSTEM_CALLBACK callback, SYSTEM_CALLBACK_TYPE callbackmask)
    {
      return FMOD.System.FMOD_System_SetCallback(this.rawPtr, callback, callbackmask);
    }

    public RESULT setPluginPath(string path)
    {
      return FMOD.System.FMOD_System_SetPluginPath(this.rawPtr, Encoding.UTF8.GetBytes(path + "\0"));
    }

    public RESULT loadPlugin(string filename, out uint handle, uint priority)
    {
      return FMOD.System.FMOD_System_LoadPlugin(this.rawPtr, Encoding.UTF8.GetBytes(filename + "\0"), out handle, priority);
    }

    public RESULT loadPlugin(string filename, out uint handle)
    {
      return this.loadPlugin(filename, out handle, 0U);
    }

    public RESULT unloadPlugin(uint handle)
    {
      return FMOD.System.FMOD_System_UnloadPlugin(this.rawPtr, handle);
    }

    public RESULT getNumNestedPlugins(uint handle, out int count)
    {
      return FMOD.System.FMOD_System_GetNumNestedPlugins(this.rawPtr, handle, out count);
    }

    public RESULT getNestedPlugin(uint handle, int index, out uint nestedhandle)
    {
      return FMOD.System.FMOD_System_GetNestedPlugin(this.rawPtr, handle, index, out nestedhandle);
    }

    public RESULT getNumPlugins(PLUGINTYPE plugintype, out int numplugins)
    {
      return FMOD.System.FMOD_System_GetNumPlugins(this.rawPtr, plugintype, out numplugins);
    }

    public RESULT getPluginHandle(PLUGINTYPE plugintype, int index, out uint handle)
    {
      return FMOD.System.FMOD_System_GetPluginHandle(this.rawPtr, plugintype, index, out handle);
    }

    public RESULT getPluginInfo(
      uint handle,
      out PLUGINTYPE plugintype,
      StringBuilder name,
      int namelen,
      out uint version)
    {
      IntPtr num = Marshal.AllocHGlobal(name.Capacity);
      int pluginInfo = (int) FMOD.System.FMOD_System_GetPluginInfo(this.rawPtr, handle, out plugintype, num, namelen, out version);
      StringMarshalHelper.NativeToBuilder(name, num);
      Marshal.FreeHGlobal(num);
      return (RESULT) pluginInfo;
    }

    public RESULT setOutputByPlugin(uint handle)
    {
      return FMOD.System.FMOD_System_SetOutputByPlugin(this.rawPtr, handle);
    }

    public RESULT getOutputByPlugin(out uint handle)
    {
      return FMOD.System.FMOD_System_GetOutputByPlugin(this.rawPtr, out handle);
    }

    public RESULT createDSPByPlugin(uint handle, out DSP dsp)
    {
      dsp = (DSP) null;
      IntPtr dsp1;
      int dspByPlugin = (int) FMOD.System.FMOD_System_CreateDSPByPlugin(this.rawPtr, handle, out dsp1);
      dsp = new DSP(dsp1);
      return (RESULT) dspByPlugin;
    }

    public RESULT getDSPInfoByPlugin(uint handle, out IntPtr description)
    {
      return FMOD.System.FMOD_System_GetDSPInfoByPlugin(this.rawPtr, handle, out description);
    }

    public RESULT registerDSP(ref DSP_DESCRIPTION description, out uint handle)
    {
      return FMOD.System.FMOD_System_RegisterDSP(this.rawPtr, ref description, out handle);
    }

    public RESULT init(int maxchannels, INITFLAGS flags, IntPtr extradriverdata)
    {
      return FMOD.System.FMOD_System_Init(this.rawPtr, maxchannels, flags, extradriverdata);
    }

    public RESULT close()
    {
      return FMOD.System.FMOD_System_Close(this.rawPtr);
    }

    public RESULT update()
    {
      return FMOD.System.FMOD_System_Update(this.rawPtr);
    }

    public RESULT setSpeakerPosition(SPEAKER speaker, float x, float y, bool active)
    {
      return FMOD.System.FMOD_System_SetSpeakerPosition(this.rawPtr, speaker, x, y, active);
    }

    public RESULT getSpeakerPosition(
      SPEAKER speaker,
      out float x,
      out float y,
      out bool active)
    {
      return FMOD.System.FMOD_System_GetSpeakerPosition(this.rawPtr, speaker, out x, out y, out active);
    }

    public RESULT setStreamBufferSize(uint filebuffersize, TIMEUNIT filebuffersizetype)
    {
      return FMOD.System.FMOD_System_SetStreamBufferSize(this.rawPtr, filebuffersize, filebuffersizetype);
    }

    public RESULT getStreamBufferSize(
      out uint filebuffersize,
      out TIMEUNIT filebuffersizetype)
    {
      return FMOD.System.FMOD_System_GetStreamBufferSize(this.rawPtr, out filebuffersize, out filebuffersizetype);
    }

    public RESULT set3DSettings(float dopplerscale, float distancefactor, float rolloffscale)
    {
      return FMOD.System.FMOD_System_Set3DSettings(this.rawPtr, dopplerscale, distancefactor, rolloffscale);
    }

    public RESULT get3DSettings(
      out float dopplerscale,
      out float distancefactor,
      out float rolloffscale)
    {
      return FMOD.System.FMOD_System_Get3DSettings(this.rawPtr, out dopplerscale, out distancefactor, out rolloffscale);
    }

    public RESULT set3DNumListeners(int numlisteners)
    {
      return FMOD.System.FMOD_System_Set3DNumListeners(this.rawPtr, numlisteners);
    }

    public RESULT get3DNumListeners(out int numlisteners)
    {
      return FMOD.System.FMOD_System_Get3DNumListeners(this.rawPtr, out numlisteners);
    }

    public RESULT set3DListenerAttributes(
      int listener,
      ref VECTOR pos,
      ref VECTOR vel,
      ref VECTOR forward,
      ref VECTOR up)
    {
      return FMOD.System.FMOD_System_Set3DListenerAttributes(this.rawPtr, listener, ref pos, ref vel, ref forward, ref up);
    }

    public RESULT get3DListenerAttributes(
      int listener,
      out VECTOR pos,
      out VECTOR vel,
      out VECTOR forward,
      out VECTOR up)
    {
      return FMOD.System.FMOD_System_Get3DListenerAttributes(this.rawPtr, listener, out pos, out vel, out forward, out up);
    }

    public RESULT set3DRolloffCallback(CB_3D_ROLLOFFCALLBACK callback)
    {
      return FMOD.System.FMOD_System_Set3DRolloffCallback(this.rawPtr, callback);
    }

    public RESULT mixerSuspend()
    {
      return FMOD.System.FMOD_System_MixerSuspend(this.rawPtr);
    }

    public RESULT mixerResume()
    {
      return FMOD.System.FMOD_System_MixerResume(this.rawPtr);
    }

    public RESULT getDefaultMixMatrix(
      SPEAKERMODE sourcespeakermode,
      SPEAKERMODE targetspeakermode,
      float[] matrix,
      int matrixhop)
    {
      return FMOD.System.FMOD_System_GetDefaultMixMatrix(this.rawPtr, sourcespeakermode, targetspeakermode, matrix, matrixhop);
    }

    public RESULT getSpeakerModeChannels(SPEAKERMODE mode, out int channels)
    {
      return FMOD.System.FMOD_System_GetSpeakerModeChannels(this.rawPtr, mode, out channels);
    }

    public RESULT getVersion(out uint version)
    {
      return FMOD.System.FMOD_System_GetVersion(this.rawPtr, out version);
    }

    public RESULT getOutputHandle(out IntPtr handle)
    {
      return FMOD.System.FMOD_System_GetOutputHandle(this.rawPtr, out handle);
    }

    public RESULT getChannelsPlaying(out int channels, out int realchannels)
    {
      return FMOD.System.FMOD_System_GetChannelsPlaying(this.rawPtr, out channels, out realchannels);
    }

    public RESULT getCPUUsage(
      out float dsp,
      out float stream,
      out float geometry,
      out float update,
      out float total)
    {
      return FMOD.System.FMOD_System_GetCPUUsage(this.rawPtr, out dsp, out stream, out geometry, out update, out total);
    }

    public RESULT getFileUsage(
      out long sampleBytesRead,
      out long streamBytesRead,
      out long otherBytesRead)
    {
      return FMOD.System.FMOD_System_GetFileUsage(this.rawPtr, out sampleBytesRead, out streamBytesRead, out otherBytesRead);
    }

    public RESULT getSoundRAM(out int currentalloced, out int maxalloced, out int total)
    {
      return FMOD.System.FMOD_System_GetSoundRAM(this.rawPtr, out currentalloced, out maxalloced, out total);
    }

    public RESULT createSound(
      string name,
      MODE mode,
      ref CREATESOUNDEXINFO exinfo,
      out Sound sound)
    {
      sound = (Sound) null;
      byte[] bytes = Encoding.UTF8.GetBytes(name + "\0");
      exinfo.cbsize = Marshal.SizeOf((object) exinfo);
      IntPtr sound1;
      int sound2 = (int) FMOD.System.FMOD_System_CreateSound(this.rawPtr, bytes, mode, ref exinfo, out sound1);
      sound = new Sound(sound1);
      return (RESULT) sound2;
    }

    public RESULT createSound(
      byte[] data,
      MODE mode,
      ref CREATESOUNDEXINFO exinfo,
      out Sound sound)
    {
      sound = (Sound) null;
      exinfo.cbsize = Marshal.SizeOf((object) exinfo);
      IntPtr sound1;
      int sound2 = (int) FMOD.System.FMOD_System_CreateSound(this.rawPtr, data, mode, ref exinfo, out sound1);
      sound = new Sound(sound1);
      return (RESULT) sound2;
    }

    public RESULT createSound(string name, MODE mode, out Sound sound)
    {
      CREATESOUNDEXINFO exinfo = new CREATESOUNDEXINFO();
      exinfo.cbsize = Marshal.SizeOf((object) exinfo);
      return this.createSound(name, mode, ref exinfo, out sound);
    }

    public RESULT createStream(
      string name,
      MODE mode,
      ref CREATESOUNDEXINFO exinfo,
      out Sound sound)
    {
      sound = (Sound) null;
      byte[] bytes = Encoding.UTF8.GetBytes(name + "\0");
      exinfo.cbsize = Marshal.SizeOf((object) exinfo);
      IntPtr sound1;
      int stream = (int) FMOD.System.FMOD_System_CreateStream(this.rawPtr, bytes, mode, ref exinfo, out sound1);
      sound = new Sound(sound1);
      return (RESULT) stream;
    }

    public RESULT createStream(
      byte[] data,
      MODE mode,
      ref CREATESOUNDEXINFO exinfo,
      out Sound sound)
    {
      sound = (Sound) null;
      exinfo.cbsize = Marshal.SizeOf((object) exinfo);
      IntPtr sound1;
      int stream = (int) FMOD.System.FMOD_System_CreateStream(this.rawPtr, data, mode, ref exinfo, out sound1);
      sound = new Sound(sound1);
      return (RESULT) stream;
    }

    public RESULT createStream(string name, MODE mode, out Sound sound)
    {
      CREATESOUNDEXINFO exinfo = new CREATESOUNDEXINFO();
      exinfo.cbsize = Marshal.SizeOf((object) exinfo);
      return this.createStream(name, mode, ref exinfo, out sound);
    }

    public RESULT createDSP(ref DSP_DESCRIPTION description, out DSP dsp)
    {
      dsp = (DSP) null;
      IntPtr dsp1;
      int dsp2 = (int) FMOD.System.FMOD_System_CreateDSP(this.rawPtr, ref description, out dsp1);
      dsp = new DSP(dsp1);
      return (RESULT) dsp2;
    }

    public RESULT createDSPByType(DSP_TYPE type, out DSP dsp)
    {
      dsp = (DSP) null;
      IntPtr dsp1;
      int dspByType = (int) FMOD.System.FMOD_System_CreateDSPByType(this.rawPtr, type, out dsp1);
      dsp = new DSP(dsp1);
      return (RESULT) dspByType;
    }

    public RESULT createChannelGroup(string name, out ChannelGroup channelgroup)
    {
      channelgroup = (ChannelGroup) null;
      IntPtr channelgroup1;
      int channelGroup = (int) FMOD.System.FMOD_System_CreateChannelGroup(this.rawPtr, Encoding.UTF8.GetBytes(name + "\0"), out channelgroup1);
      channelgroup = new ChannelGroup(channelgroup1);
      return (RESULT) channelGroup;
    }

    public RESULT createSoundGroup(string name, out SoundGroup soundgroup)
    {
      soundgroup = (SoundGroup) null;
      IntPtr soundgroup1;
      int soundGroup = (int) FMOD.System.FMOD_System_CreateSoundGroup(this.rawPtr, Encoding.UTF8.GetBytes(name + "\0"), out soundgroup1);
      soundgroup = new SoundGroup(soundgroup1);
      return (RESULT) soundGroup;
    }

    public RESULT createReverb3D(out Reverb3D reverb)
    {
      IntPtr reverb1;
      int reverb3D = (int) FMOD.System.FMOD_System_CreateReverb3D(this.rawPtr, out reverb1);
      reverb = new Reverb3D(reverb1);
      return (RESULT) reverb3D;
    }

    public RESULT playSound(
      Sound sound,
      ChannelGroup channelGroup,
      bool paused,
      out Channel channel)
    {
      channel = (Channel) null;
      IntPtr channelGroup1 = (HandleBase) channelGroup != (HandleBase) null ? channelGroup.getRaw() : IntPtr.Zero;
      IntPtr channel1;
      int num = (int) FMOD.System.FMOD_System_PlaySound(this.rawPtr, sound.getRaw(), channelGroup1, paused, out channel1);
      channel = new Channel(channel1);
      return (RESULT) num;
    }

    public RESULT playDSP(
      DSP dsp,
      ChannelGroup channelGroup,
      bool paused,
      out Channel channel)
    {
      channel = (Channel) null;
      IntPtr channelGroup1 = (HandleBase) channelGroup != (HandleBase) null ? channelGroup.getRaw() : IntPtr.Zero;
      IntPtr channel1;
      int num = (int) FMOD.System.FMOD_System_PlayDSP(this.rawPtr, dsp.getRaw(), channelGroup1, paused, out channel1);
      channel = new Channel(channel1);
      return (RESULT) num;
    }

    public RESULT getChannel(int channelid, out Channel channel)
    {
      channel = (Channel) null;
      IntPtr channel1;
      int channel2 = (int) FMOD.System.FMOD_System_GetChannel(this.rawPtr, channelid, out channel1);
      channel = new Channel(channel1);
      return (RESULT) channel2;
    }

    public RESULT getMasterChannelGroup(out ChannelGroup channelgroup)
    {
      channelgroup = (ChannelGroup) null;
      IntPtr channelgroup1;
      int masterChannelGroup = (int) FMOD.System.FMOD_System_GetMasterChannelGroup(this.rawPtr, out channelgroup1);
      channelgroup = new ChannelGroup(channelgroup1);
      return (RESULT) masterChannelGroup;
    }

    public RESULT getMasterSoundGroup(out SoundGroup soundgroup)
    {
      soundgroup = (SoundGroup) null;
      IntPtr soundgroup1;
      int masterSoundGroup = (int) FMOD.System.FMOD_System_GetMasterSoundGroup(this.rawPtr, out soundgroup1);
      soundgroup = new SoundGroup(soundgroup1);
      return (RESULT) masterSoundGroup;
    }

    public RESULT attachChannelGroupToPort(
      uint portType,
      ulong portIndex,
      ChannelGroup channelgroup,
      bool passThru = false)
    {
      return FMOD.System.FMOD_System_AttachChannelGroupToPort(this.rawPtr, portType, portIndex, channelgroup.getRaw(), passThru);
    }

    public RESULT detachChannelGroupFromPort(ChannelGroup channelgroup)
    {
      return FMOD.System.FMOD_System_DetachChannelGroupFromPort(this.rawPtr, channelgroup.getRaw());
    }

    public RESULT setReverbProperties(int instance, ref REVERB_PROPERTIES prop)
    {
      return FMOD.System.FMOD_System_SetReverbProperties(this.rawPtr, instance, ref prop);
    }

    public RESULT getReverbProperties(int instance, out REVERB_PROPERTIES prop)
    {
      return FMOD.System.FMOD_System_GetReverbProperties(this.rawPtr, instance, out prop);
    }

    public RESULT lockDSP()
    {
      return FMOD.System.FMOD_System_LockDSP(this.rawPtr);
    }

    public RESULT unlockDSP()
    {
      return FMOD.System.FMOD_System_UnlockDSP(this.rawPtr);
    }

    public RESULT getRecordNumDrivers(out int numdrivers, out int numconnected)
    {
      return FMOD.System.FMOD_System_GetRecordNumDrivers(this.rawPtr, out numdrivers, out numconnected);
    }

    public RESULT getRecordDriverInfo(
      int id,
      StringBuilder name,
      int namelen,
      out Guid guid,
      out int systemrate,
      out SPEAKERMODE speakermode,
      out int speakermodechannels,
      out DRIVER_STATE state)
    {
      IntPtr num = Marshal.AllocHGlobal(name.Capacity);
      int recordDriverInfo = (int) FMOD.System.FMOD_System_GetRecordDriverInfo(this.rawPtr, id, num, namelen, out guid, out systemrate, out speakermode, out speakermodechannels, out state);
      StringMarshalHelper.NativeToBuilder(name, num);
      Marshal.FreeHGlobal(num);
      return (RESULT) recordDriverInfo;
    }

    public RESULT getRecordPosition(int id, out uint position)
    {
      return FMOD.System.FMOD_System_GetRecordPosition(this.rawPtr, id, out position);
    }

    public RESULT recordStart(int id, Sound sound, bool loop)
    {
      return FMOD.System.FMOD_System_RecordStart(this.rawPtr, id, sound.getRaw(), loop);
    }

    public RESULT recordStop(int id)
    {
      return FMOD.System.FMOD_System_RecordStop(this.rawPtr, id);
    }

    public RESULT isRecording(int id, out bool recording)
    {
      return FMOD.System.FMOD_System_IsRecording(this.rawPtr, id, out recording);
    }

    public RESULT createGeometry(int maxpolygons, int maxvertices, out Geometry geometry)
    {
      geometry = (Geometry) null;
      IntPtr geometry1;
      int geometry2 = (int) FMOD.System.FMOD_System_CreateGeometry(this.rawPtr, maxpolygons, maxvertices, out geometry1);
      geometry = new Geometry(geometry1);
      return (RESULT) geometry2;
    }

    public RESULT setGeometrySettings(float maxworldsize)
    {
      return FMOD.System.FMOD_System_SetGeometrySettings(this.rawPtr, maxworldsize);
    }

    public RESULT getGeometrySettings(out float maxworldsize)
    {
      return FMOD.System.FMOD_System_GetGeometrySettings(this.rawPtr, out maxworldsize);
    }

    public RESULT loadGeometry(IntPtr data, int datasize, out Geometry geometry)
    {
      geometry = (Geometry) null;
      IntPtr geometry1;
      int num = (int) FMOD.System.FMOD_System_LoadGeometry(this.rawPtr, data, datasize, out geometry1);
      geometry = new Geometry(geometry1);
      return (RESULT) num;
    }

    public RESULT getGeometryOcclusion(
      ref VECTOR listener,
      ref VECTOR source,
      out float direct,
      out float reverb)
    {
      return FMOD.System.FMOD_System_GetGeometryOcclusion(this.rawPtr, ref listener, ref source, out direct, out reverb);
    }

    public RESULT setNetworkProxy(string proxy)
    {
      return FMOD.System.FMOD_System_SetNetworkProxy(this.rawPtr, Encoding.UTF8.GetBytes(proxy + "\0"));
    }

    public RESULT getNetworkProxy(StringBuilder proxy, int proxylen)
    {
      IntPtr num = Marshal.AllocHGlobal(proxy.Capacity);
      int networkProxy = (int) FMOD.System.FMOD_System_GetNetworkProxy(this.rawPtr, num, proxylen);
      StringMarshalHelper.NativeToBuilder(proxy, num);
      Marshal.FreeHGlobal(num);
      return (RESULT) networkProxy;
    }

    public RESULT setNetworkTimeout(int timeout)
    {
      return FMOD.System.FMOD_System_SetNetworkTimeout(this.rawPtr, timeout);
    }

    public RESULT getNetworkTimeout(out int timeout)
    {
      return FMOD.System.FMOD_System_GetNetworkTimeout(this.rawPtr, out timeout);
    }

    public RESULT setUserData(IntPtr userdata)
    {
      return FMOD.System.FMOD_System_SetUserData(this.rawPtr, userdata);
    }

    public RESULT getUserData(out IntPtr userdata)
    {
      return FMOD.System.FMOD_System_GetUserData(this.rawPtr, out userdata);
    }

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_Release(IntPtr system);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_SetOutput(IntPtr system, OUTPUTTYPE output);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_GetOutput(IntPtr system, out OUTPUTTYPE output);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_GetNumDrivers(IntPtr system, out int numdrivers);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_GetDriverInfo(
      IntPtr system,
      int id,
      IntPtr name,
      int namelen,
      out Guid guid,
      out int systemrate,
      out SPEAKERMODE speakermode,
      out int speakermodechannels);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_SetDriver(IntPtr system, int driver);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_GetDriver(IntPtr system, out int driver);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_SetSoftwareChannels(
      IntPtr system,
      int numsoftwarechannels);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_GetSoftwareChannels(
      IntPtr system,
      out int numsoftwarechannels);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_SetSoftwareFormat(
      IntPtr system,
      int samplerate,
      SPEAKERMODE speakermode,
      int numrawspeakers);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_GetSoftwareFormat(
      IntPtr system,
      out int samplerate,
      out SPEAKERMODE speakermode,
      out int numrawspeakers);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_SetDSPBufferSize(
      IntPtr system,
      uint bufferlength,
      int numbuffers);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_GetDSPBufferSize(
      IntPtr system,
      out uint bufferlength,
      out int numbuffers);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_SetFileSystem(
      IntPtr system,
      FILE_OPENCALLBACK useropen,
      FILE_CLOSECALLBACK userclose,
      FILE_READCALLBACK userread,
      FILE_SEEKCALLBACK userseek,
      FILE_ASYNCREADCALLBACK userasyncread,
      FILE_ASYNCCANCELCALLBACK userasynccancel,
      int blockalign);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_AttachFileSystem(
      IntPtr system,
      FILE_OPENCALLBACK useropen,
      FILE_CLOSECALLBACK userclose,
      FILE_READCALLBACK userread,
      FILE_SEEKCALLBACK userseek);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_SetPluginPath(IntPtr system, byte[] path);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_LoadPlugin(
      IntPtr system,
      byte[] filename,
      out uint handle,
      uint priority);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_UnloadPlugin(IntPtr system, uint handle);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_GetNumNestedPlugins(
      IntPtr system,
      uint handle,
      out int count);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_GetNestedPlugin(
      IntPtr system,
      uint handle,
      int index,
      out uint nestedhandle);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_GetNumPlugins(
      IntPtr system,
      PLUGINTYPE plugintype,
      out int numplugins);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_GetPluginHandle(
      IntPtr system,
      PLUGINTYPE plugintype,
      int index,
      out uint handle);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_GetPluginInfo(
      IntPtr system,
      uint handle,
      out PLUGINTYPE plugintype,
      IntPtr name,
      int namelen,
      out uint version);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_CreateDSPByPlugin(
      IntPtr system,
      uint handle,
      out IntPtr dsp);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_SetOutputByPlugin(IntPtr system, uint handle);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_GetOutputByPlugin(IntPtr system, out uint handle);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_GetDSPInfoByPlugin(
      IntPtr system,
      uint handle,
      out IntPtr description);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_RegisterDSP(
      IntPtr system,
      ref DSP_DESCRIPTION description,
      out uint handle);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_Init(
      IntPtr system,
      int maxchannels,
      INITFLAGS flags,
      IntPtr extradriverdata);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_Close(IntPtr system);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_Update(IntPtr system);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_SetAdvancedSettings(
      IntPtr system,
      ref ADVANCEDSETTINGS settings);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_GetAdvancedSettings(
      IntPtr system,
      ref ADVANCEDSETTINGS settings);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_Set3DRolloffCallback(
      IntPtr system,
      CB_3D_ROLLOFFCALLBACK callback);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_MixerSuspend(IntPtr system);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_MixerResume(IntPtr system);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_GetDefaultMixMatrix(
      IntPtr system,
      SPEAKERMODE sourcespeakermode,
      SPEAKERMODE targetspeakermode,
      float[] matrix,
      int matrixhop);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_GetSpeakerModeChannels(
      IntPtr system,
      SPEAKERMODE mode,
      out int channels);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_SetCallback(
      IntPtr system,
      SYSTEM_CALLBACK callback,
      SYSTEM_CALLBACK_TYPE callbackmask);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_SetSpeakerPosition(
      IntPtr system,
      SPEAKER speaker,
      float x,
      float y,
      bool active);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_GetSpeakerPosition(
      IntPtr system,
      SPEAKER speaker,
      out float x,
      out float y,
      out bool active);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_Set3DSettings(
      IntPtr system,
      float dopplerscale,
      float distancefactor,
      float rolloffscale);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_Get3DSettings(
      IntPtr system,
      out float dopplerscale,
      out float distancefactor,
      out float rolloffscale);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_Set3DNumListeners(
      IntPtr system,
      int numlisteners);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_Get3DNumListeners(
      IntPtr system,
      out int numlisteners);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_Set3DListenerAttributes(
      IntPtr system,
      int listener,
      ref VECTOR pos,
      ref VECTOR vel,
      ref VECTOR forward,
      ref VECTOR up);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_Get3DListenerAttributes(
      IntPtr system,
      int listener,
      out VECTOR pos,
      out VECTOR vel,
      out VECTOR forward,
      out VECTOR up);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_SetStreamBufferSize(
      IntPtr system,
      uint filebuffersize,
      TIMEUNIT filebuffersizetype);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_GetStreamBufferSize(
      IntPtr system,
      out uint filebuffersize,
      out TIMEUNIT filebuffersizetype);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_GetVersion(IntPtr system, out uint version);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_GetOutputHandle(IntPtr system, out IntPtr handle);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_GetChannelsPlaying(
      IntPtr system,
      out int channels,
      out int realchannels);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_GetCPUUsage(
      IntPtr system,
      out float dsp,
      out float stream,
      out float geometry,
      out float update,
      out float total);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_GetFileUsage(
      IntPtr system,
      out long sampleBytesRead,
      out long streamBytesRead,
      out long otherBytesRead);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_GetSoundRAM(
      IntPtr system,
      out int currentalloced,
      out int maxalloced,
      out int total);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_CreateSound(
      IntPtr system,
      byte[] name_or_data,
      MODE mode,
      ref CREATESOUNDEXINFO exinfo,
      out IntPtr sound);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_CreateStream(
      IntPtr system,
      byte[] name_or_data,
      MODE mode,
      ref CREATESOUNDEXINFO exinfo,
      out IntPtr sound);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_CreateDSP(
      IntPtr system,
      ref DSP_DESCRIPTION description,
      out IntPtr dsp);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_CreateDSPByType(
      IntPtr system,
      DSP_TYPE type,
      out IntPtr dsp);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_CreateChannelGroup(
      IntPtr system,
      byte[] name,
      out IntPtr channelgroup);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_CreateSoundGroup(
      IntPtr system,
      byte[] name,
      out IntPtr soundgroup);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_CreateReverb3D(IntPtr system, out IntPtr reverb);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_PlaySound(
      IntPtr system,
      IntPtr sound,
      IntPtr channelGroup,
      bool paused,
      out IntPtr channel);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_PlayDSP(
      IntPtr system,
      IntPtr dsp,
      IntPtr channelGroup,
      bool paused,
      out IntPtr channel);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_GetChannel(
      IntPtr system,
      int channelid,
      out IntPtr channel);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_GetMasterChannelGroup(
      IntPtr system,
      out IntPtr channelgroup);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_GetMasterSoundGroup(
      IntPtr system,
      out IntPtr soundgroup);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_AttachChannelGroupToPort(
      IntPtr system,
      uint portType,
      ulong portIndex,
      IntPtr channelgroup,
      bool passThru);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_DetachChannelGroupFromPort(
      IntPtr system,
      IntPtr channelgroup);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_SetReverbProperties(
      IntPtr system,
      int instance,
      ref REVERB_PROPERTIES prop);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_GetReverbProperties(
      IntPtr system,
      int instance,
      out REVERB_PROPERTIES prop);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_LockDSP(IntPtr system);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_UnlockDSP(IntPtr system);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_GetRecordNumDrivers(
      IntPtr system,
      out int numdrivers,
      out int numconnected);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_GetRecordDriverInfo(
      IntPtr system,
      int id,
      IntPtr name,
      int namelen,
      out Guid guid,
      out int systemrate,
      out SPEAKERMODE speakermode,
      out int speakermodechannels,
      out DRIVER_STATE state);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_GetRecordPosition(
      IntPtr system,
      int id,
      out uint position);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_RecordStart(
      IntPtr system,
      int id,
      IntPtr sound,
      bool loop);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_RecordStop(IntPtr system, int id);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_IsRecording(
      IntPtr system,
      int id,
      out bool recording);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_CreateGeometry(
      IntPtr system,
      int maxpolygons,
      int maxvertices,
      out IntPtr geometry);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_SetGeometrySettings(
      IntPtr system,
      float maxworldsize);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_GetGeometrySettings(
      IntPtr system,
      out float maxworldsize);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_LoadGeometry(
      IntPtr system,
      IntPtr data,
      int datasize,
      out IntPtr geometry);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_GetGeometryOcclusion(
      IntPtr system,
      ref VECTOR listener,
      ref VECTOR source,
      out float direct,
      out float reverb);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_SetNetworkProxy(IntPtr system, byte[] proxy);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_GetNetworkProxy(
      IntPtr system,
      IntPtr proxy,
      int proxylen);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_SetNetworkTimeout(IntPtr system, int timeout);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_GetNetworkTimeout(IntPtr system, out int timeout);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_SetUserData(IntPtr system, IntPtr userdata);

    [DllImport("fmod")]
    private static extern RESULT FMOD_System_GetUserData(IntPtr system, out IntPtr userdata);

    public System(IntPtr raw)
      : base(raw)
    {
    }
  }
}
