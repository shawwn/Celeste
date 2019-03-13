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
      CSGEN_StrawberrySeeds csgenStrawberrySeeds = this;
      csgenStrawberrySeeds.sfx = Audio.Play("event:/game/general/seed_complete_main", csgenStrawberrySeeds.Position);
      csgenStrawberrySeeds.snapshot = Audio.CreateSnapshot("snapshot:/music_mains_mute", true);
      Player entity = csgenStrawberrySeeds.Scene.Tracker.GetEntity<Player>();
      if (entity != null)
        csgenStrawberrySeeds.cameraStart = entity.CameraTarget;
      foreach (StrawberrySeed seed in csgenStrawberrySeeds.strawberry.Seeds)
        seed.OnAllCollected();
      csgenStrawberrySeeds.strawberry.Depth = -2000002;
      csgenStrawberrySeeds.strawberry.AddTag((int) Tags.FrozenUpdate);
      yield return (object) 0.35f;
      csgenStrawberrySeeds.Tag = (int) Tags.FrozenUpdate | (int) Tags.HUD;
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
      csgenStrawberrySeeds.system = new ParticleSystem(-2000002, 50);
      csgenStrawberrySeeds.system.Tag = (int) Tags.FrozenUpdate;
      level.Add((Entity) csgenStrawberrySeeds.system);
      float num1 = 6.283185f / (float) csgenStrawberrySeeds.strawberry.Seeds.Count;
      float angleOffset = 1.570796f;
      Vector2 vector2_1 = Vector2.get_Zero();
      foreach (StrawberrySeed seed in csgenStrawberrySeeds.strawberry.Seeds)
        vector2_1 = Vector2.op_Addition(vector2_1, seed.Position);
      Vector2 averagePos = Vector2.op_Division(vector2_1, (float) csgenStrawberrySeeds.strawberry.Seeds.Count);
      foreach (StrawberrySeed seed in csgenStrawberrySeeds.strawberry.Seeds)
      {
        seed.StartSpinAnimation(averagePos, csgenStrawberrySeeds.strawberry.Position, angleOffset, 4f);
        angleOffset -= num1;
      }
      Vector2 val = Vector2.op_Subtraction(csgenStrawberrySeeds.strawberry.Position, new Vector2(160f, 90f));
      Rectangle bounds1 = level.Bounds;
      double left = (double) ((Rectangle) ref bounds1).get_Left();
      Rectangle bounds2 = level.Bounds;
      double top = (double) ((Rectangle) ref bounds2).get_Top();
      Rectangle bounds3 = level.Bounds;
      double num2 = (double) (((Rectangle) ref bounds3).get_Right() - 320);
      Rectangle bounds4 = level.Bounds;
      double num3 = (double) (((Rectangle) ref bounds4).get_Bottom() - 180);
      Vector2 target = val.Clamp((float) left, (float) top, (float) num2, (float) num3);
      csgenStrawberrySeeds.Add((Component) new Coroutine(CutsceneEntity.CameraTo(target, 3.5f, Ease.CubeInOut, 0.0f), true));
      yield return (object) 4f;
      Input.Rumble(RumbleStrength.Light, RumbleLength.Long);
      Audio.Play("event:/game/general/seed_complete_berry", csgenStrawberrySeeds.strawberry.Position);
      foreach (StrawberrySeed seed in csgenStrawberrySeeds.strawberry.Seeds)
        seed.StartCombineAnimation(csgenStrawberrySeeds.strawberry.Position, 0.6f, csgenStrawberrySeeds.system);
      yield return (object) 0.6f;
      Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
      foreach (Entity seed in csgenStrawberrySeeds.strawberry.Seeds)
        seed.RemoveSelf();
      csgenStrawberrySeeds.strawberry.CollectedSeeds();
      yield return (object) 0.5f;
      Vector2 vector2_2 = Vector2.op_Subtraction(level.Camera.Position, csgenStrawberrySeeds.cameraStart);
      float dist = ((Vector2) ref vector2_2).Length();
      yield return (object) CutsceneEntity.CameraTo(csgenStrawberrySeeds.cameraStart, dist / 180f, (Ease.Easer) null, 0.0f);
      if ((double) dist > 80.0)
        yield return (object) 0.25f;
      level.EndCutscene();
      csgenStrawberrySeeds.OnEnd(level);
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
