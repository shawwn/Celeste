// Decompiled with JetBrains decompiler
// Type: Celeste.CassetteBlockManager
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using FMOD.Studio;
using Monocle;
using System;

namespace Celeste
{
  [Tracked(false)]
  public class CassetteBlockManager : Entity
  {
    private int currentIndex;
    private float beatTimer;
    private int beatIndex;
    private float tempoMult;
    private int leadBeats;
    private int maxBeat;
    private bool isLevelMusic;
    private int beatIndexOffset;
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
      this.isLevelMusic = AreaData.Areas[this.SceneAs<Level>().Session.Area.ID].CassetteSong == null;
      if (this.isLevelMusic)
      {
        this.leadBeats = 0;
        this.beatIndexOffset = 5;
      }
      else
      {
        this.beatIndexOffset = 0;
        this.leadBeats = 16;
        this.snapshot = Audio.CreateSnapshot("snapshot:/music_mains_mute");
      }
      this.maxBeat = this.SceneAs<Level>().CassetteBlockBeats;
      this.tempoMult = this.SceneAs<Level>().CassetteBlockTempo;
    }

    public override void Removed(Scene scene)
    {
      base.Removed(scene);
      if (this.isLevelMusic)
        return;
      Audio.Stop(this.snapshot);
      Audio.Stop(this.sfx);
    }

    public override void SceneEnd(Scene scene)
    {
      base.SceneEnd(scene);
      if (this.isLevelMusic)
        return;
      Audio.Stop(this.snapshot);
      Audio.Stop(this.sfx);
    }

    public override void Update()
    {
      base.Update();
      if (this.isLevelMusic)
        this.sfx = Audio.CurrentMusicEventInstance;
      if ((HandleBase) this.sfx == (HandleBase) null && !this.isLevelMusic)
      {
        this.sfx = Audio.CreateInstance(AreaData.Areas[this.SceneAs<Level>().Session.Area.ID].CassetteSong);
        Audio.Play("event:/game/general/cassette_block_switch_2");
      }
      else
        this.AdvanceMusic(Engine.DeltaTime * this.tempoMult);
    }

    public void AdvanceMusic(float time)
    {
      double beatTimer = (double) this.beatTimer;
      this.beatTimer += time;
      if ((double) this.beatTimer < 0.1666666716337204)
        return;
      this.beatTimer -= 0.16666667f;
      ++this.beatIndex;
      this.beatIndex %= 256;
      if (this.beatIndex % 8 == 0)
      {
        ++this.currentIndex;
        this.currentIndex %= this.maxBeat;
        this.SetActiveIndex(this.currentIndex);
        if (!this.isLevelMusic)
          Audio.Play("event:/game/general/cassette_block_switch_2");
        Input.Rumble(RumbleStrength.Medium, RumbleLength.Short);
      }
      else if ((this.beatIndex + 1) % 8 == 0)
        this.SetWillActivate((this.currentIndex + 1) % this.maxBeat);
      else if ((this.beatIndex + 4) % 8 == 0 && !this.isLevelMusic)
        Audio.Play("event:/game/general/cassette_block_switch_1");
      if (this.leadBeats > 0)
      {
        --this.leadBeats;
        if (this.leadBeats == 0)
        {
          this.beatIndex = 0;
          if (!this.isLevelMusic)
          {
            int num = (int) this.sfx.start();
          }
        }
      }
      if (this.leadBeats > 0)
        return;
      // TKTK: Why is this.sfx null?
      if (sfx == null)
        return;
      int num1 = (int) this.sfx.setParameterValue("sixteenth_note", (float) this.GetSixteenthNote());
    }

    public int GetSixteenthNote() => (this.beatIndex + this.beatIndexOffset) % 256 + 1;

    public void StopBlocks()
    {
      foreach (CassetteBlock entity in this.Scene.Tracker.GetEntities<CassetteBlock>())
        entity.Finish();
      if (this.isLevelMusic)
        return;
      Audio.Stop(this.sfx);
    }

    public void Finish()
    {
      if (!this.isLevelMusic)
        Audio.Stop(this.snapshot);
      this.RemoveSelf();
    }

    public void OnLevelStart()
    {
      this.maxBeat = this.SceneAs<Level>().CassetteBlockBeats;
      this.tempoMult = this.SceneAs<Level>().CassetteBlockTempo;
      this.currentIndex = this.beatIndex % 8 < 5 ? this.maxBeat - 1 : this.maxBeat - 2;
      this.SilentUpdateBlocks();
    }

    private void SilentUpdateBlocks()
    {
      foreach (CassetteBlock entity in this.Scene.Tracker.GetEntities<CassetteBlock>())
      {
        if (entity.ID.Level == this.SceneAs<Level>().Session.Level)
          entity.SetActivatedSilently(entity.Index == this.currentIndex);
      }
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
