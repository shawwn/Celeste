// Decompiled with JetBrains decompiler
// Type: Celeste.AudioTrackState
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Celeste
{
  [Serializable]
  public class AudioTrackState
  {
    public List<MEP> Parameters = new List<MEP>();
    [XmlIgnore]
    private string ev;

    [XmlAttribute]
    public string Event
    {
      get
      {
        return this.ev;
      }
      set
      {
        if (!(this.ev != value))
          return;
        this.ev = value;
        this.Parameters.Clear();
      }
    }

    [XmlIgnore]
    public int Progress
    {
      get
      {
        return (int) this.GetParam("progress");
      }
      set
      {
        this.Param("progress", (float) value);
      }
    }

    public AudioTrackState()
    {
    }

    public AudioTrackState(string ev)
    {
      this.Event = ev;
    }

    public AudioTrackState Layer(int index, float value)
    {
      return this.Param(AudioState.LayerParameters[index], value);
    }

    public AudioTrackState Layer(int index, bool value)
    {
      return this.Param(AudioState.LayerParameters[index], value);
    }

    public AudioTrackState SetProgress(int value)
    {
      this.Progress = value;
      return this;
    }

    public AudioTrackState Param(string key, float value)
    {
      foreach (MEP parameter in this.Parameters)
      {
        if (parameter.Key != null && parameter.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase))
        {
          parameter.Value = value;
          return this;
        }
      }
      this.Parameters.Add(new MEP(key, value));
      return this;
    }

    public AudioTrackState Param(string key, bool value)
    {
      return this.Param(key, value ? 1f : 0.0f);
    }

    public float GetParam(string key)
    {
      foreach (MEP parameter in this.Parameters)
      {
        if (parameter.Key != null && parameter.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase))
          return parameter.Value;
      }
      return 0.0f;
    }

    public AudioTrackState Clone()
    {
      AudioTrackState audioTrackState = new AudioTrackState();
      audioTrackState.Event = this.Event;
      foreach (MEP parameter in this.Parameters)
        audioTrackState.Parameters.Add(new MEP(parameter.Key, parameter.Value));
      return audioTrackState;
    }
  }
}
