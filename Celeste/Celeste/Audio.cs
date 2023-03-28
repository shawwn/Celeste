// Decompiled with JetBrains decompiler
// Type: Celeste.Audio
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using FMOD;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.IO;

namespace Celeste
{
  public static class Audio
  {
    private static FMOD.Studio._3D_ATTRIBUTES attributes3d = new FMOD.Studio._3D_ATTRIBUTES();
    public static Dictionary<string, EventDescription> cachedEventDescriptions = new Dictionary<string, EventDescription>();
    private static EventInstance currentMusicEvent = (EventInstance) null;
    private static EventInstance currentAltMusicEvent = (EventInstance) null;
    private static EventInstance currentAmbientEvent = (EventInstance) null;
    private static EventInstance mainDownSnapshot = (EventInstance) null;
    public static string CurrentMusic = "";
    private static FMOD.Studio.System system;
    private static Camera currentCamera;
    private static bool ready;
    private static bool musicUnderwater;
    private static EventInstance musicUnderwaterSnapshot;

    public static void Init()
    {
      FMOD.Studio.INITFLAGS studioFlags = FMOD.Studio.INITFLAGS.NORMAL;
      if (Settings.Instance.LaunchWithFMODLiveUpdate)
        studioFlags = FMOD.Studio.INITFLAGS.LIVEUPDATE;
      Audio.CheckFmod(FMOD.Studio.System.create(out Audio.system));
      Audio.CheckFmod(Audio.system.initialize(1024, studioFlags, FMOD.INITFLAGS.NORMAL, IntPtr.Zero));
      ref FMOD.Studio._3D_ATTRIBUTES local1 = ref Audio.attributes3d;
      VECTOR vector1 = new VECTOR();
      vector1.x = 0.0f;
      vector1.y = 0.0f;
      vector1.z = 1f;
      VECTOR vector2 = vector1;
      local1.forward = vector2;
      ref FMOD.Studio._3D_ATTRIBUTES local2 = ref Audio.attributes3d;
      vector1 = new VECTOR();
      vector1.x = 0.0f;
      vector1.y = 1f;
      vector1.z = 0.0f;
      VECTOR vector3 = vector1;
      local2.up = vector3;
      Audio.SetListenerPosition(new Vector3(0.0f, 0.0f, 1f), new Vector3(0.0f, 1f, 0.0f), new Vector3(0.0f, 0.0f, -345f));
      Audio.ready = true;
    }

    public static void Update()
    {
      if (!((FMOD.Studio.HandleBase) Audio.system != (FMOD.Studio.HandleBase) null) || !Audio.ready)
        return;
      Audio.CheckFmod(Audio.system.update());
    }

    public static void Unload()
    {
      if (!((FMOD.Studio.HandleBase) Audio.system != (FMOD.Studio.HandleBase) null))
        return;
      Audio.CheckFmod(Audio.system.unloadAll());
    }

    public static void SetListenerPosition(Vector3 forward, Vector3 up, Vector3 position)
    {
      FMOD.Studio._3D_ATTRIBUTES attributes = new FMOD.Studio._3D_ATTRIBUTES()
      {
        forward = {
          x = forward.X,
          z = forward.Y
        }
      };
      attributes.forward.z = forward.Z;
      attributes.up.x = up.X;
      attributes.up.y = up.Y;
      attributes.up.z = up.Z;
      attributes.position.x = position.X;
      attributes.position.y = position.Y;
      attributes.position.z = position.Z;
      int num = (int) Audio.system.setListenerAttributes(0, attributes);
    }

    public static void SetCamera(Camera camera)
    {
      Audio.currentCamera = camera;
    }

    internal static void CheckFmod(RESULT result)
    {
      if ((uint) result > 0U)
        throw new Exception("FMOD Failed: " + (object) result);
    }

    public static EventInstance Play(string path)
    {
      EventInstance instance = Audio.CreateInstance(path, new Vector2?());
      if ((FMOD.Studio.HandleBase) instance != (FMOD.Studio.HandleBase) null)
      {
        int num1 = (int) instance.start();
        int num2 = (int) instance.release();
      }
      return instance;
    }

    public static EventInstance Play(string path, string param, float value)
    {
      EventInstance instance = Audio.CreateInstance(path, new Vector2?());
      if ((FMOD.Studio.HandleBase) instance != (FMOD.Studio.HandleBase) null)
      {
        Audio.SetParameter(instance, param, value);
        int num1 = (int) instance.start();
        int num2 = (int) instance.release();
      }
      return instance;
    }

    public static EventInstance Play(string path, Vector2 position)
    {
      EventInstance instance = Audio.CreateInstance(path, new Vector2?(position));
      if ((FMOD.Studio.HandleBase) instance != (FMOD.Studio.HandleBase) null)
      {
        int num1 = (int) instance.start();
        int num2 = (int) instance.release();
      }
      return instance;
    }

    public static EventInstance Play(
      string path,
      Vector2 position,
      string param,
      float value)
    {
      EventInstance instance = Audio.CreateInstance(path, new Vector2?(position));
      if ((FMOD.Studio.HandleBase) instance != (FMOD.Studio.HandleBase) null)
      {
        if (param != null)
        {
          int num1 = (int) instance.setParameterValue(param, value);
        }
        int num2 = (int) instance.start();
        int num3 = (int) instance.release();
      }
      return instance;
    }

    public static EventInstance Play(
      string path,
      Vector2 position,
      string param,
      float value,
      string param2,
      float value2)
    {
      EventInstance instance = Audio.CreateInstance(path, new Vector2?(position));
      if ((FMOD.Studio.HandleBase) instance != (FMOD.Studio.HandleBase) null)
      {
        if (param != null)
        {
          int num1 = (int) instance.setParameterValue(param, value);
        }
        if (param2 != null)
        {
          int num2 = (int) instance.setParameterValue(param2, value2);
        }
        int num3 = (int) instance.start();
        int num4 = (int) instance.release();
      }
      return instance;
    }

    public static EventInstance Loop(string path)
    {
      EventInstance instance = Audio.CreateInstance(path, new Vector2?());
      if ((FMOD.Studio.HandleBase) instance != (FMOD.Studio.HandleBase) null)
      {
        int num = (int) instance.start();
      }
      return instance;
    }

    public static EventInstance Loop(string path, string param, float value)
    {
      EventInstance instance = Audio.CreateInstance(path, new Vector2?());
      if ((FMOD.Studio.HandleBase) instance != (FMOD.Studio.HandleBase) null)
      {
        int num1 = (int) instance.setParameterValue(param, value);
        int num2 = (int) instance.start();
      }
      return instance;
    }

    public static EventInstance Loop(string path, Vector2 position)
    {
      EventInstance instance = Audio.CreateInstance(path, new Vector2?(position));
      if ((FMOD.Studio.HandleBase) instance != (FMOD.Studio.HandleBase) null)
      {
        int num = (int) instance.start();
      }
      return instance;
    }

    public static EventInstance Loop(
      string path,
      Vector2 position,
      string param,
      float value)
    {
      EventInstance instance = Audio.CreateInstance(path, new Vector2?(position));
      if ((FMOD.Studio.HandleBase) instance != (FMOD.Studio.HandleBase) null)
      {
        int num1 = (int) instance.setParameterValue(param, value);
        int num2 = (int) instance.start();
      }
      return instance;
    }

    public static void Pause(EventInstance instance)
    {
      if (!((FMOD.Studio.HandleBase) instance != (FMOD.Studio.HandleBase) null))
        return;
      int num = (int) instance.setPaused(true);
    }

    public static void Resume(EventInstance instance)
    {
      if (!((FMOD.Studio.HandleBase) instance != (FMOD.Studio.HandleBase) null))
        return;
      int num = (int) instance.setPaused(false);
    }

    public static void Position(EventInstance instance, Vector2 position)
    {
      if (!((FMOD.Studio.HandleBase) instance != (FMOD.Studio.HandleBase) null))
        return;
      Vector2 vector2 = Vector2.Zero;
      if (Audio.currentCamera != null)
        vector2 = Audio.currentCamera.Position + new Vector2(320f, 180f) / 2f;
      float num1 = position.X - vector2.X;
      if (SaveData.Instance != null && SaveData.Instance.Assists.MirrorMode)
        num1 = -num1;
      Audio.attributes3d.position.x = num1;
      Audio.attributes3d.position.y = position.Y - vector2.Y;
      Audio.attributes3d.position.z = 0.0f;
      int num2 = (int) instance.set3DAttributes(Audio.attributes3d);
    }

    public static void SetParameter(EventInstance instance, string param, float value)
    {
      if (!((FMOD.Studio.HandleBase) instance != (FMOD.Studio.HandleBase) null))
        return;
      int num = (int) instance.setParameterValue(param, value);
    }

    public static void Stop(EventInstance instance, bool allowFadeOut = true)
    {
      if (!((FMOD.Studio.HandleBase) instance != (FMOD.Studio.HandleBase) null))
        return;
      int num1 = (int) instance.stop(allowFadeOut ? STOP_MODE.ALLOWFADEOUT : STOP_MODE.IMMEDIATE);
      int num2 = (int) instance.release();
    }

    public static EventInstance CreateInstance(string path, Vector2? position = null)
    {
      EventDescription eventDescription = Audio.GetEventDescription(path);
      if (!((FMOD.Studio.HandleBase) eventDescription != (FMOD.Studio.HandleBase) null))
        return (EventInstance) null;
      EventInstance instance1;
      int instance2 = (int) eventDescription.createInstance(out instance1);
      bool is3D;
      int num = (int) eventDescription.is3D(out is3D);
      if (is3D && position.HasValue)
        Audio.Position(instance1, position.Value);
      return instance1;
    }

    public static EventDescription GetEventDescription(string path)
    {
      EventDescription _event = (EventDescription) null;
      if (path != null && !Audio.cachedEventDescriptions.TryGetValue(path, out _event))
      {
        RESULT result = Audio.system.getEvent(path, out _event);
        switch (result)
        {
          case RESULT.OK:
            int num = (int) _event.loadSampleData();
            Audio.cachedEventDescriptions.Add(path, _event);
            goto case RESULT.ERR_EVENT_NOTFOUND;
          case RESULT.ERR_EVENT_NOTFOUND:
            break;
          default:
            throw new Exception("FMOD getEvent failed: " + (object) result);
        }
      }
      return _event;
    }

    public static string GetEventName(EventInstance instance)
    {
      if ((FMOD.Studio.HandleBase) instance != (FMOD.Studio.HandleBase) null)
      {
        EventDescription description1;
        int description2 = (int) instance.getDescription(out description1);
        if ((FMOD.Studio.HandleBase) description1 != (FMOD.Studio.HandleBase) null)
        {
          string path1 = "";
          int path2 = (int) description1.getPath(out path1);
          return path1;
        }
      }
      return "";
    }

    public static bool IsPlaying(EventInstance instance)
    {
      if ((FMOD.Studio.HandleBase) instance != (FMOD.Studio.HandleBase) null)
      {
        PLAYBACK_STATE state;
        int playbackState = (int) instance.getPlaybackState(out state);
        if (state == PLAYBACK_STATE.PLAYING || state == PLAYBACK_STATE.STARTING)
          return true;
      }
      return false;
    }

    public static bool BusPaused(string path, bool? pause = null)
    {
      bool paused1 = false;
      Bus bus;
      if ((FMOD.Studio.HandleBase) Audio.system != (FMOD.Studio.HandleBase) null && Audio.system.getBus(path, out bus) == RESULT.OK)
      {
        if (pause.HasValue)
        {
          int num = (int) bus.setPaused(pause.Value);
        }
        int paused2 = (int) bus.getPaused(out paused1);
      }
      return paused1;
    }

    public static bool BusMuted(string path, bool? mute)
    {
      bool paused1 = false;
      Bus bus;
      if (Audio.system.getBus(path, out bus) == RESULT.OK)
      {
        if (mute.HasValue)
        {
          int num = (int) bus.setMute(mute.Value);
        }
        int paused2 = (int) bus.getPaused(out paused1);
      }
      return paused1;
    }

    public static void BusStopAll(string path, bool immediate = false)
    {
      Bus bus;
      if (!((FMOD.Studio.HandleBase) Audio.system != (FMOD.Studio.HandleBase) null) || Audio.system.getBus(path, out bus) != RESULT.OK)
        return;
      int num = (int) bus.stopAllEvents(immediate ? STOP_MODE.IMMEDIATE : STOP_MODE.ALLOWFADEOUT);
    }

    public static float VCAVolume(string path, float? volume = null)
    {
      VCA vca1;
      RESULT vca2 = Audio.system.getVCA(path, out vca1);
      float volume1 = 1f;
      float finalvolume = 1f;
      if (vca2 == RESULT.OK)
      {
        if (volume.HasValue)
        {
          int num = (int) vca1.setVolume(volume.Value);
        }
        int volume2 = (int) vca1.getVolume(out volume1, out finalvolume);
      }
      return volume1;
    }

    public static EventInstance CreateSnapshot(string name, bool start = true)
    {
      EventDescription _event;
      int num1 = (int) Audio.system.getEvent(name, out _event);
      if ((FMOD.Studio.HandleBase) _event == (FMOD.Studio.HandleBase) null)
        throw new Exception("Snapshot " + name + " doesn't exist");
      EventInstance instance1;
      int instance2 = (int) _event.createInstance(out instance1);
      if (start)
      {
        int num2 = (int) instance1.start();
      }
      return instance1;
    }

    public static void ResumeSnapshot(EventInstance snapshot)
    {
      if (!((FMOD.Studio.HandleBase) snapshot != (FMOD.Studio.HandleBase) null))
        return;
      int num = (int) snapshot.start();
    }

    public static bool IsSnapshotRunning(EventInstance snapshot)
    {
      if (!((FMOD.Studio.HandleBase) snapshot != (FMOD.Studio.HandleBase) null))
        return false;
      PLAYBACK_STATE state;
      int playbackState = (int) snapshot.getPlaybackState(out state);
      return state == PLAYBACK_STATE.PLAYING || state == PLAYBACK_STATE.STARTING || state == PLAYBACK_STATE.SUSTAINING;
    }

    public static void EndSnapshot(EventInstance snapshot)
    {
      if (!((FMOD.Studio.HandleBase) snapshot != (FMOD.Studio.HandleBase) null))
        return;
      int num = (int) snapshot.stop(STOP_MODE.ALLOWFADEOUT);
    }

    public static void ReleaseSnapshot(EventInstance snapshot)
    {
      if (!((FMOD.Studio.HandleBase) snapshot != (FMOD.Studio.HandleBase) null))
        return;
      int num1 = (int) snapshot.stop(STOP_MODE.ALLOWFADEOUT);
      int num2 = (int) snapshot.release();
    }

    public static EventInstance CurrentMusicEventInstance
    {
      get
      {
        return Audio.currentMusicEvent;
      }
    }

    public static EventInstance CurrentAmbienceEventInstance
    {
      get
      {
        return Audio.currentAmbientEvent;
      }
    }

    public static bool SetMusic(string path, bool startPlaying = true, bool allowFadeOut = true)
    {
      if (string.IsNullOrEmpty(path) || path == "null")
      {
        Audio.Stop(Audio.currentMusicEvent, allowFadeOut);
        Audio.currentMusicEvent = (EventInstance) null;
        Audio.CurrentMusic = "";
      }
      else if (!Audio.CurrentMusic.Equals(path, StringComparison.OrdinalIgnoreCase))
      {
        Audio.Stop(Audio.currentMusicEvent, allowFadeOut);
        EventInstance instance = Audio.CreateInstance(path, new Vector2?());
        if ((FMOD.Studio.HandleBase) instance != (FMOD.Studio.HandleBase) null & startPlaying)
        {
          int num = (int) instance.start();
        }
        Audio.currentMusicEvent = instance;
        Audio.CurrentMusic = Audio.GetEventName(instance);
        return true;
      }
      return false;
    }

    public static bool SetAmbience(string path, bool startPlaying = true)
    {
      if (string.IsNullOrEmpty(path) || path == "null")
      {
        Audio.Stop(Audio.currentAmbientEvent, true);
        Audio.currentAmbientEvent = (EventInstance) null;
      }
      else if (!Audio.GetEventName(Audio.currentAmbientEvent).Equals(path, StringComparison.OrdinalIgnoreCase))
      {
        Audio.Stop(Audio.currentAmbientEvent, true);
        EventInstance instance = Audio.CreateInstance(path, new Vector2?());
        if ((FMOD.Studio.HandleBase) instance != (FMOD.Studio.HandleBase) null & startPlaying)
        {
          int num = (int) instance.start();
        }
        Audio.currentAmbientEvent = instance;
        return true;
      }
      return false;
    }

    public static void SetMusicParam(string path, float value)
    {
      if (!((FMOD.Studio.HandleBase) Audio.currentMusicEvent != (FMOD.Studio.HandleBase) null))
        return;
      int num = (int) Audio.currentMusicEvent.setParameterValue(path, value);
    }

    public static void SetAltMusic(string path)
    {
      if (string.IsNullOrEmpty(path))
      {
        Audio.EndSnapshot(Audio.mainDownSnapshot);
        Audio.Stop(Audio.currentAltMusicEvent, true);
        Audio.currentAltMusicEvent = (EventInstance) null;
      }
      else
      {
        if (Audio.GetEventName(Audio.currentAltMusicEvent).Equals(path, StringComparison.OrdinalIgnoreCase))
          return;
        Audio.StartMainDownSnapshot();
        Audio.Stop(Audio.currentAltMusicEvent, true);
        Audio.currentAltMusicEvent = Audio.Loop(path);
      }
    }

    private static void StartMainDownSnapshot()
    {
      if ((FMOD.Studio.HandleBase) Audio.mainDownSnapshot == (FMOD.Studio.HandleBase) null)
        Audio.mainDownSnapshot = Audio.CreateSnapshot("snapshot:/music_mains_mute", true);
      else
        Audio.ResumeSnapshot(Audio.mainDownSnapshot);
    }

    private static void EndMainDownSnapshot()
    {
      Audio.EndSnapshot(Audio.mainDownSnapshot);
    }

    public static float MusicVolume
    {
      get
      {
        return Audio.VCAVolume("vca:/music", new float?());
      }
      set
      {
        double num = (double) Audio.VCAVolume("vca:/music", new float?(value));
      }
    }

    public static float SfxVolume
    {
      get
      {
        return Audio.VCAVolume("vca:/gameplay_sfx", new float?());
      }
      set
      {
        double num1 = (double) Audio.VCAVolume("vca:/gameplay_sfx", new float?(value));
        double num2 = (double) Audio.VCAVolume("vca:/ui_sfx", new float?(value));
      }
    }

    public static bool PauseMusic
    {
      get
      {
        return Audio.BusPaused("bus:/music", new bool?());
      }
      set
      {
        Audio.BusPaused("bus:/music", new bool?(value));
      }
    }

    public static bool PauseGameplaySfx
    {
      get
      {
        return Audio.BusPaused("bus:/gameplay_sfx", new bool?());
      }
      set
      {
        Audio.BusPaused("bus:/gameplay_sfx", new bool?(value));
        Audio.BusPaused("bus:/music/stings", new bool?(value));
      }
    }

    public static bool PauseUISfx
    {
      get
      {
        return Audio.BusPaused("bus:/ui_sfx", new bool?());
      }
      set
      {
        Audio.BusPaused("bus:/ui_sfx", new bool?(value));
      }
    }

    public static bool MusicUnderwater
    {
      get
      {
        return Audio.musicUnderwater;
      }
      set
      {
        if (Audio.musicUnderwater == value)
          return;
        Audio.musicUnderwater = value;
        if (Audio.musicUnderwater)
        {
          if ((FMOD.Studio.HandleBase) Audio.musicUnderwaterSnapshot == (FMOD.Studio.HandleBase) null)
            Audio.musicUnderwaterSnapshot = Audio.CreateSnapshot("snapshot:/underwater", true);
          else
            Audio.ResumeSnapshot(Audio.musicUnderwaterSnapshot);
        }
        else
          Audio.EndSnapshot(Audio.musicUnderwaterSnapshot);
      }
    }

    public static class Banks
    {
      public static Bank Master;
      public static Bank Music;
      public static Bank Sfxs;
      public static Bank UI;
      public static Bank DlcMusic;
      public static Bank DlcSfxs;

      public static Bank Load(string name, bool loadStrings)
      {
        string str = Path.Combine(Engine.ContentDirectory, "FMOD", "Desktop", name);
        Bank bank1;
        Audio.CheckFmod(Audio.system.loadBankFile(str + ".bank", LOAD_BANK_FLAGS.NORMAL, out bank1));
        if (loadStrings)
        {
          Bank bank2;
          Audio.CheckFmod(Audio.system.loadBankFile(str + ".strings.bank", LOAD_BANK_FLAGS.NORMAL, out bank2));
        }
        return bank1;
      }
    }
  }
}

