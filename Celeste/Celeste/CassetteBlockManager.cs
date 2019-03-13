// Decompiled with JetBrains decompiler
// Type: Celeste.CassetteBlockManager
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  [Tracked(false)]
  public class CassetteBlockManager : Entity
  {
    private int leadBeats = 16;
    private int currentIndex;
    private float beatTimer;
    private int beatIndex;
    private float tempoMult;
    private int maxBeat;
    private EventInstance sfx;
    private EventInstance snapshot;

    public CassetteBlockManager()
    {
      this.Tag = (int) Tags.Global;
      this.Add((Component) new TransitionListener()
      {
        OnOutBegin = (Action) (() =>
        {
          if (!this.SceneAs<Level>().HasCassetteBlocks)
          {
            this.RemoveSelf();
          }
          else
          {
            this.maxBeat = this.SceneAs<Level>().CassetteBlockBeats;
            this.tempoMult = this.SceneAs<Level>().CassetteBlockTempo;
          }
        })
      });
    }

    public override void Awake(Scene scene)
    {
      base.Awake(scene);
      this.snapshot = Audio.CreateSnapshot("snapshot:/music_mains_mute", true);
      this.maxBeat = this.SceneAs<Level>().CassetteBlockBeats;
      this.tempoMult = this.SceneAs<Level>().CassetteBlockTempo;
    }

    public override void Removed(Scene scene)
    {
      base.Removed(scene);
      Audio.Stop(this.snapshot, true);
      Audio.Stop(this.sfx, true);
    }

    public override void SceneEnd(Scene scene)
    {
      base.SceneEnd(scene);
      Audio.Stop(this.snapshot, true);
      Audio.Stop(this.sfx, true);
    }

    public override void Update()
    {
      base.Update();
      if ((HandleBase) this.sfx == (HandleBase) null)
      {
        this.sfx = Audio.CreateInstance(AreaData.Areas[this.SceneAs<Level>().Session.Area.ID].CassetteSong, new Vector2?());
        Audio.Play("event:/game/general/cassette_block_switch_2");
      }
      else
        this.AdvanceMusic(Engine.DeltaTime * this.tempoMult);
    }

    public void AdvanceMusic(float time)
    {
      double beatTimer = (double) this.beatTimer;
      this.beatTimer += time;
      if ((double) this.beatTimer < 0.16666667163372)
        return;
      this.beatTimer -= 0.1666667f;
      ++this.beatIndex;
      this.beatIndex %= 256;
      if (this.beatIndex % 8 == 0)
      {
        ++this.currentIndex;
        this.currentIndex %= this.maxBeat;
        this.SetActiveIndex(this.currentIndex);
        Audio.Play("event:/game/general/cassette_block_switch_2");
        Input.Rumble(RumbleStrength.Medium, RumbleLength.Short);
      }
      else if ((this.beatIndex + 1) % 8 == 0)
        this.SetWillActivate((this.currentIndex + 1) % this.maxBeat);
      else if ((this.beatIndex + 4) % 8 == 0)
        Audio.Play("event:/game/general/cassette_block_switch_1");
      if (this.leadBeats > 0)
      {
        --this.leadBeats;
        if (this.leadBeats == 0)
        {
          this.beatIndex = 0;
          int num = (int) this.sfx.start();
        }
      }
      if (this.leadBeats > 0)
        return;
      int num1 = (int) this.sfx.setParameterValue("sixteenth_note", (float) (this.beatIndex + 1));
    }

    public void StopBlocks()
    {
      foreach (CassetteBlock entity in this.Scene.Tracker.GetEntities<CassetteBlock>())
        entity.Finish();
      Audio.Stop(this.sfx, true);
    }

    public void Finish()
    {
      Audio.Stop(this.snapshot, true);
      this.RemoveSelf();
    }

    public void OnLevelStart()
    {
      this.currentIndex = this.beatIndex % 8 < 5 ? this.maxBeat - 1 : this.maxBeat - 2;
      this.SilentUpdateBlocks();
    }

    private void SilentUpdateBlocks()
    {
      foreach (CassetteBlock entity in this.Scene.Tracker.GetEntities<CassetteBlock>())
        entity.SetActivatedSilently(entity.Index == this.currentIndex);
    }

    public void SetActiveIndex(int index)
    {
      foreach (CassetteBlock entity in this.Scene.Tracker.GetEntities<CassetteBlock>())
        entity.Activated = entity.Index == index;
    }

    public void SetWillActivate(int index)
    {
      foreach (CassetteBlock entity in this.Scene.Tracker.GetEntities<CassetteBlock>())
      {
        if (entity.Index == index || entity.Activated)
          entity.WillToggle();
      }
    }
  }
}
