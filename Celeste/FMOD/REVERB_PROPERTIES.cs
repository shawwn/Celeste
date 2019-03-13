// Decompiled with JetBrains decompiler
// Type: FMOD.REVERB_PROPERTIES
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

namespace FMOD
{
  public struct REVERB_PROPERTIES
  {
    public float DecayTime;
    public float EarlyDelay;
    public float LateDelay;
    public float HFReference;
    public float HFDecayRatio;
    public float Diffusion;
    public float Density;
    public float LowShelfFrequency;
    public float LowShelfGain;
    public float HighCut;
    public float EarlyLateMix;
    public float WetLevel;

    public REVERB_PROPERTIES(
      float decayTime,
      float earlyDelay,
      float lateDelay,
      float hfReference,
      float hfDecayRatio,
      float diffusion,
      float density,
      float lowShelfFrequency,
      float lowShelfGain,
      float highCut,
      float earlyLateMix,
      float wetLevel)
    {
      this.DecayTime = decayTime;
      this.EarlyDelay = earlyDelay;
      this.LateDelay = lateDelay;
      this.HFReference = hfReference;
      this.HFDecayRatio = hfDecayRatio;
      this.Diffusion = diffusion;
      this.Density = density;
      this.LowShelfFrequency = lowShelfFrequency;
      this.LowShelfGain = lowShelfGain;
      this.HighCut = highCut;
      this.EarlyLateMix = earlyLateMix;
      this.WetLevel = wetLevel;
    }
  }
}
