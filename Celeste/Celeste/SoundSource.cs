﻿// Decompiled with JetBrains decompiler
// Type: Celeste.SoundSource
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  [Tracked(false)]
  public class SoundSource : Component
  {
    public Vector2 Position = Vector2.Zero;
    public bool DisposeOnTransition = true;
    public string EventName;
    private EventInstance instance;
    private bool is3D;
    private bool isOneshot;

    public bool Playing { get; private set; }

    public bool Is3D
    {
      get
      {
        return this.is3D;
      }
    }

    public bool IsOneshot
    {
      get
      {
        return this.isOneshot;
      }
    }

    public bool InstancePlaying
    {
      get
      {
        if ((HandleBase) this.instance != (HandleBase) null)
        {
          PLAYBACK_STATE state;
          int playbackState = (int) this.instance.getPlaybackState(out state);
          if (state == PLAYBACK_STATE.PLAYING || state == PLAYBACK_STATE.STARTING || state == PLAYBACK_STATE.SUSTAINING)
            return true;
        }
        return false;
      }
    }

    public SoundSource()
      : base(true, false)
    {
    }

    public SoundSource(string path)
      : this()
    {
      this.Play(path, (string) null, 0.0f);
    }

    public SoundSource(Vector2 offset, string path)
      : this()
    {
      this.Position = offset;
      this.Play(path, (string) null, 0.0f);
    }

    public SoundSource Play(string path, string param = null, float value = 0.0f)
    {
      this.Stop(true);
      this.EventName = path;
      EventDescription eventDescription = Audio.GetEventDescription(path);
      if ((HandleBase) eventDescription != (HandleBase) null)
      {
        int instance = (int) eventDescription.createInstance(out this.instance);
        int num1 = (int) eventDescription.is3D(out this.is3D);
        int num2 = (int) eventDescription.isOneshot(out this.isOneshot);
      }
      if ((HandleBase) this.instance != (HandleBase) null)
      {
        if (this.is3D)
        {
          Vector2 position = this.Position;
          if (this.Entity != null)
            position += this.Entity.Position;
          Audio.Position(this.instance, position);
        }
        if (param != null)
        {
          int num1 = (int) this.instance.setParameterValue(param, value);
        }
        int num2 = (int) this.instance.start();
        this.Playing = true;
      }
      return this;
    }

    public SoundSource Param(string param, float value)
    {
      if ((HandleBase) this.instance != (HandleBase) null)
      {
        int num = (int) this.instance.setParameterValue(param, value);
      }
      return this;
    }

    public SoundSource Pause()
    {
      if ((HandleBase) this.instance != (HandleBase) null)
      {
        int num = (int) this.instance.setPaused(true);
      }
      this.Playing = false;
      return this;
    }

    public SoundSource Resume()
    {
      if ((HandleBase) this.instance != (HandleBase) null)
      {
        bool paused1;
        int paused2 = (int) this.instance.getPaused(out paused1);
        if (paused1)
        {
          int num = (int) this.instance.setPaused(false);
          this.Playing = true;
        }
      }
      return this;
    }

    public SoundSource Stop(bool allowFadeout = true)
    {
      Audio.Stop(this.instance, allowFadeout);
      this.instance = (EventInstance) null;
      this.Playing = false;
      return this;
    }

    public override void Update()
    {
      if (this.is3D && (HandleBase) this.instance != (HandleBase) null)
      {
        Vector2 position = this.Position;
        if (this.Entity != null)
          position += this.Entity.Position;
        Audio.Position(this.instance, position);
      }
      if (!this.isOneshot || !((HandleBase) this.instance != (HandleBase) null))
        return;
      PLAYBACK_STATE state;
      int playbackState = (int) this.instance.getPlaybackState(out state);
      if (state == PLAYBACK_STATE.STOPPED)
      {
        int num = (int) this.instance.release();
        this.instance = (EventInstance) null;
        this.Playing = false;
      }
    }

    public override void EntityRemoved(Scene scene)
    {
      base.EntityRemoved(scene);
      this.Stop(true);
    }

    public override void Removed(Entity entity)
    {
      base.Removed(entity);
      this.Stop(true);
    }

    public override void SceneEnd(Scene scene)
    {
      base.SceneEnd(scene);
      this.Stop(false);
    }

    public override void DebugRender(Camera camera)
    {
      Vector2 position = this.Position;
      if (this.Entity != null)
        position += this.Entity.Position;
      Draw.HollowRect(position.X - 2f, position.Y - 2f, 4f, 4f, Color.BlueViolet);
    }
  }
}

