// Decompiled with JetBrains decompiler
// Type: Celeste.CSGEN_StrawberrySeeds
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class CSGEN_StrawberrySeeds : CutsceneEntity
  {
    private Strawberry strawberry;
    private Vector2 cameraStart;
    private ParticleSystem system;
    private EventInstance snapshot;
    private EventInstance sfx;

    public CSGEN_StrawberrySeeds(Strawberry strawberry)
      : base(true, false)
    {
      this.strawberry = strawberry;
    }

    public override void OnBegin(Level level)
    {
      this.cameraStart = level.Camera.Position;
      this.Add((Component) new Coroutine(this.Cutscene(level), true));
    }

    private IEnumerator Cutscene(Level level)
    {
      this.sfx = Audio.Play("event:/game/general/seed_complete_main", this.Position);
      this.snapshot = Audio.CreateSnapshot("snapshot:/music_mains_mute", true);
      Player player = this.Scene.Tracker.GetEntity<Player>();
      if (player != null)
        this.cameraStart = player.CameraTarget;
      foreach (StrawberrySeed seed1 in this.strawberry.Seeds)
      {
        StrawberrySeed seed = seed1;
        seed.OnAllCollected();
        seed = (StrawberrySeed) null;
      }
      this.strawberry.Depth = -2000002;
      this.strawberry.AddTag((int) Tags.FrozenUpdate);
      yield return (object) 0.35f;
      this.Tag = (int) Tags.FrozenUpdate | (int) Tags.HUD;
      level.Frozen = true;
      level.FormationBackdrop.Display = true;
      level.FormationBackdrop.Alpha = 0.5f;
      level.Displacement.Clear();
      level.Displacement.Enabled = false;
      Audio.BusPaused("bus:/gameplay_sfx/ambience", new bool?(true));
      Audio.BusPaused("bus:/gameplay_sfx/char", new bool?(true));
      Audio.BusPaused("bus:/gameplay_sfx/game/general/yes_pause", new bool?(true));
      Audio.BusPaused("bus:/gameplay_sfx/game/chapters", new bool?(true));
      yield return (object) 0.1f;
      this.system = new ParticleSystem(-2000002, 50);
      this.system.Tag = (int) Tags.FrozenUpdate;
      level.Add((Entity) this.system);
      float angleSep = 6.283185f / (float) this.strawberry.Seeds.Count;
      float angle = 1.570796f;
      Vector2 avg = Vector2.Zero;
      foreach (StrawberrySeed seed1 in this.strawberry.Seeds)
      {
        StrawberrySeed seed = seed1;
        avg += seed.Position;
        seed = (StrawberrySeed) null;
      }
      avg /= (float) this.strawberry.Seeds.Count;
      foreach (StrawberrySeed seed1 in this.strawberry.Seeds)
      {
        StrawberrySeed seed = seed1;
        seed.StartSpinAnimation(avg, this.strawberry.Position, angle, 4f);
        angle -= angleSep;
        seed = (StrawberrySeed) null;
      }
      avg = new Vector2();
      Vector2 target = this.strawberry.Position - new Vector2(160f, 90f);
      target = target.Clamp((float) level.Bounds.Left, (float) level.Bounds.Top, (float) (level.Bounds.Right - 320), (float) (level.Bounds.Bottom - 180));
      this.Add((Component) new Coroutine(CutsceneEntity.CameraTo(target, 3.5f, Ease.CubeInOut, 0.0f), true));
      target = new Vector2();
      yield return (object) 4f;
      Input.Rumble(RumbleStrength.Light, RumbleLength.Long);
      Audio.Play("event:/game/general/seed_complete_berry", this.strawberry.Position);
      foreach (StrawberrySeed seed1 in this.strawberry.Seeds)
      {
        StrawberrySeed seed = seed1;
        seed.StartCombineAnimation(this.strawberry.Position, 0.6f, this.system);
        seed = (StrawberrySeed) null;
      }
      yield return (object) 0.6f;
      Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
      foreach (StrawberrySeed seed1 in this.strawberry.Seeds)
      {
        StrawberrySeed seed = seed1;
        seed.RemoveSelf();
        seed = (StrawberrySeed) null;
      }
      this.strawberry.CollectedSeeds();
      yield return (object) 0.5f;
      float dist = (level.Camera.Position - this.cameraStart).Length();
      yield return (object) CutsceneEntity.CameraTo(this.cameraStart, dist / 180f, (Ease.Easer) null, 0.0f);
      if ((double) dist > 80.0)
        yield return (object) 0.25f;
      level.EndCutscene();
      this.OnEnd(level);
    }

    public override void OnEnd(Level level)
    {
      if (this.WasSkipped)
        Audio.Stop(this.sfx, true);
      level.OnEndOfFrame += (Action) (() =>
      {
        if (this.WasSkipped)
        {
          foreach (Entity seed in this.strawberry.Seeds)
            seed.RemoveSelf();
          this.strawberry.CollectedSeeds();
          level.Camera.Position = this.cameraStart;
        }
        this.strawberry.Depth = -100;
        this.strawberry.RemoveTag((int) Tags.FrozenUpdate);
        level.Frozen = false;
        level.FormationBackdrop.Display = false;
        level.Displacement.Enabled = true;
      });
      this.RemoveSelf();
    }

    private void EndSfx()
    {
      Audio.BusPaused("bus:/gameplay_sfx/ambience", new bool?(false));
      Audio.BusPaused("bus:/gameplay_sfx/char", new bool?(false));
      Audio.BusPaused("bus:/gameplay_sfx/game/general/yes_pause", new bool?(false));
      Audio.BusPaused("bus:/gameplay_sfx/game/chapters", new bool?(false));
      Audio.EndSnapshot(this.snapshot);
    }

    public override void Removed(Scene scene)
    {
      this.EndSfx();
      base.Removed(scene);
    }

    public override void SceneEnd(Scene scene)
    {
      this.EndSfx();
      base.SceneEnd(scene);
    }
  }
}

