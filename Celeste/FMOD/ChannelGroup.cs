// Decompiled with JetBrains decompiler
// Type: FMOD.ChannelGroup
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FMOD
{
  public class ChannelGroup : ChannelControl
  {
    public RESULT release()
    {
      int num = (int) ChannelGroup.FMOD_ChannelGroup_Release(this.getRaw());
      if (num != 0)
        return (RESULT) num;
      this.rawPtr = IntPtr.Zero;
      return (RESULT) num;
    }

    public RESULT addGroup(
      ChannelGroup group,
      bool propagatedspclock,
      out DSPConnection connection)
    {
      connection = (DSPConnection) null;
      IntPtr connection1;
      int num = (int) ChannelGroup.FMOD_ChannelGroup_AddGroup(this.getRaw(), group.getRaw(), propagatedspclock, out connection1);
      connection = new DSPConnection(connection1);
      return (RESULT) num;
    }

    public RESULT getNumGroups(out int numgroups)
    {
      return ChannelGroup.FMOD_ChannelGroup_GetNumGroups(this.getRaw(), out numgroups);
    }

    public RESULT getGroup(int index, out ChannelGroup group)
    {
      group = (ChannelGroup) null;
      IntPtr group1;
      int group2 = (int) ChannelGroup.FMOD_ChannelGroup_GetGroup(this.getRaw(), index, out group1);
      group = new ChannelGroup(group1);
      return (RESULT) group2;
    }

    public RESULT getParentGroup(out ChannelGroup group)
    {
      group = (ChannelGroup) null;
      IntPtr group1;
      int parentGroup = (int) ChannelGroup.FMOD_ChannelGroup_GetParentGroup(this.getRaw(), out group1);
      group = new ChannelGroup(group1);
      return (RESULT) parentGroup;
    }

    public RESULT getName(StringBuilder name, int namelen)
    {
      IntPtr num = Marshal.AllocHGlobal(name.Capacity);
      int name1 = (int) ChannelGroup.FMOD_ChannelGroup_GetName(this.getRaw(), num, namelen);
      StringMarshalHelper.NativeToBuilder(name, num);
      Marshal.FreeHGlobal(num);
      return (RESULT) name1;
    }

    public RESULT getNumChannels(out int numchannels)
    {
      return ChannelGroup.FMOD_ChannelGroup_GetNumChannels(this.getRaw(), out numchannels);
    }

    public RESULT getChannel(int index, out Channel channel)
    {
      channel = (Channel) null;
      IntPtr channel1;
      int channel2 = (int) ChannelGroup.FMOD_ChannelGroup_GetChannel(this.getRaw(), index, out channel1);
      channel = new Channel(channel1);
      return (RESULT) channel2;
    }

    [DllImport("fmod")]
    private static extern RESULT FMOD_ChannelGroup_Release(IntPtr channelgroup);

    [DllImport("fmod")]
    private static extern RESULT FMOD_ChannelGroup_AddGroup(
      IntPtr channelgroup,
      IntPtr group,
      bool propagatedspclock,
      out IntPtr connection);

    [DllImport("fmod")]
    private static extern RESULT FMOD_ChannelGroup_GetNumGroups(
      IntPtr channelgroup,
      out int numgroups);

    [DllImport("fmod")]
    private static extern RESULT FMOD_ChannelGroup_GetGroup(
      IntPtr channelgroup,
      int index,
      out IntPtr group);

    [DllImport("fmod")]
    private static extern RESULT FMOD_ChannelGroup_GetParentGroup(
      IntPtr channelgroup,
      out IntPtr group);

    [DllImport("fmod")]
    private static extern RESULT FMOD_ChannelGroup_GetName(
      IntPtr channelgroup,
      IntPtr name,
      int namelen);

    [DllImport("fmod")]
    private static extern RESULT FMOD_ChannelGroup_GetNumChannels(
      IntPtr channelgroup,
      out int numchannels);

    [DllImport("fmod")]
    private static extern RESULT FMOD_ChannelGroup_GetChannel(
      IntPtr channelgroup,
      int index,
      out IntPtr channel);

    public ChannelGroup(IntPtr raw)
      : base(raw)
    {
    }
  }
}
