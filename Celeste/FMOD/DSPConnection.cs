// Decompiled with JetBrains decompiler
// Type: FMOD.DSPConnection
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;
using System.Runtime.InteropServices;

namespace FMOD
{
  public class DSPConnection : HandleBase
  {
    public RESULT getInput(out DSP input)
    {
      input = (DSP) null;
      IntPtr input1;
      int input2 = (int) DSPConnection.FMOD_DSPConnection_GetInput(this.rawPtr, out input1);
      input = new DSP(input1);
      return (RESULT) input2;
    }

    public RESULT getOutput(out DSP output)
    {
      output = (DSP) null;
      IntPtr output1;
      int output2 = (int) DSPConnection.FMOD_DSPConnection_GetOutput(this.rawPtr, out output1);
      output = new DSP(output1);
      return (RESULT) output2;
    }

    public RESULT setMix(float volume)
    {
      return DSPConnection.FMOD_DSPConnection_SetMix(this.rawPtr, volume);
    }

    public RESULT getMix(out float volume)
    {
      return DSPConnection.FMOD_DSPConnection_GetMix(this.rawPtr, out volume);
    }

    public RESULT setMixMatrix(
      float[] matrix,
      int outchannels,
      int inchannels,
      int inchannel_hop)
    {
      return DSPConnection.FMOD_DSPConnection_SetMixMatrix(this.rawPtr, matrix, outchannels, inchannels, inchannel_hop);
    }

    public RESULT getMixMatrix(
      float[] matrix,
      out int outchannels,
      out int inchannels,
      int inchannel_hop)
    {
      return DSPConnection.FMOD_DSPConnection_GetMixMatrix(this.rawPtr, matrix, out outchannels, out inchannels, inchannel_hop);
    }

    public RESULT getType(out DSPCONNECTION_TYPE type)
    {
      return DSPConnection.FMOD_DSPConnection_GetType(this.rawPtr, out type);
    }

    public RESULT setUserData(IntPtr userdata)
    {
      return DSPConnection.FMOD_DSPConnection_SetUserData(this.rawPtr, userdata);
    }

    public RESULT getUserData(out IntPtr userdata)
    {
      return DSPConnection.FMOD_DSPConnection_GetUserData(this.rawPtr, out userdata);
    }

    [DllImport("fmod")]
    private static extern RESULT FMOD_DSPConnection_GetInput(
      IntPtr dspconnection,
      out IntPtr input);

    [DllImport("fmod")]
    private static extern RESULT FMOD_DSPConnection_GetOutput(
      IntPtr dspconnection,
      out IntPtr output);

    [DllImport("fmod")]
    private static extern RESULT FMOD_DSPConnection_SetMix(IntPtr dspconnection, float volume);

    [DllImport("fmod")]
    private static extern RESULT FMOD_DSPConnection_GetMix(
      IntPtr dspconnection,
      out float volume);

    [DllImport("fmod")]
    private static extern RESULT FMOD_DSPConnection_SetMixMatrix(
      IntPtr dspconnection,
      float[] matrix,
      int outchannels,
      int inchannels,
      int inchannel_hop);

    [DllImport("fmod")]
    private static extern RESULT FMOD_DSPConnection_GetMixMatrix(
      IntPtr dspconnection,
      float[] matrix,
      out int outchannels,
      out int inchannels,
      int inchannel_hop);

    [DllImport("fmod")]
    private static extern RESULT FMOD_DSPConnection_GetType(
      IntPtr dspconnection,
      out DSPCONNECTION_TYPE type);

    [DllImport("fmod")]
    private static extern RESULT FMOD_DSPConnection_SetUserData(
      IntPtr dspconnection,
      IntPtr userdata);

    [DllImport("fmod")]
    private static extern RESULT FMOD_DSPConnection_GetUserData(
      IntPtr dspconnection,
      out IntPtr userdata);

    public DSPConnection(IntPtr raw)
      : base(raw)
    {
    }
  }
}
