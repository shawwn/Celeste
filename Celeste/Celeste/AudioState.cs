// Decompiled with JetBrains decompiler
// Type: Celeste.AudioState
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using FMOD.Studio;
using System;

namespace Celeste
{
  [Serializable]
  public class AudioState
  {
    public static string[] LayerParameters = new string[10]
    {
      "layer0",
      "layer1",
      "layer2",
      "layer3",
      "layer4",
      "layer5",
      "layer6",
      "layer7",
      "layer8",
      "layer9"
    };
    public AudioTrackState Music = new AudioTrackState();
    public AudioTrackState Ambience = new AudioTrackState();

    public AudioState()
    {
    }

    public AudioState(AudioTrackState music, AudioTrackState ambience)
    {
      if (music != null)
        this.Music = music.Clone();
      if (ambience == null)
        return;
      this.Ambience = ambience.Clone();
    }

    public AudioState(string music, string ambience)
    {
      this.Music.Event = music;
      this.Ambience.Event = ambience;
    }

    public void Apply()
    {
      bool flag1 = Audio.SetMusic(this.Music.Event, false, true);
      if ((HandleBase) Audio.CurrentMusicEventInstance != (HandleBase) null)
      {
        foreach (MEP parameter in this.Music.Parameters)
          Audio.SetParameter(Audio.CurrentMusicEventInstance, parameter.Key, parameter.Value);
        if (flag1)
        {
          int num = (int) Audio.CurrentMusicEventInstance.start();
        }
      }
      bool flag2 = Audio.SetAmbience(this.Ambience.Event, false);
      if (!((HandleBase) Audio.CurrentAmbienceEventInstance != (HandleBase) null))
        return;
      foreach (MEP parameter in this.Ambience.Parameters)
        Audio.SetParameter(Audio.CurrentAmbienceEventInstance, parameter.Key, parameter.Value);
      if (!flag2)
        return;
      int num1 = (int) Audio.CurrentAmbienceEventInstance.start();
    }

    public void Stop(bool allowFadeOut = true)
    {
      Audio.SetMusic((string) null, false, allowFadeOut);
      Audio.SetAmbience((string) null, true);
    }

    public AudioState Clone()
    {
      return new AudioState()
      {
        Music = this.Music.Clone(),
        Ambience = this.Ambience.Clone()
      };
    }
  }
}
