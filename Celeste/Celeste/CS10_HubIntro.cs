// Decompiled with JetBrains decompiler
// Type: Celeste.CS10_HubIntro
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Celeste
{
  public class CS10_HubIntro : CutsceneEntity
  {
    public const string Flag = "hub_intro";
    public const float BirdOffset = 190f;
    private Player player;
    private List<LockBlock> locks;
    private Booster booster;
    private CS10_HubIntro.Bird bird;
    private Vector2 spawn;
    private List<EventInstance> sfxs = new List<EventInstance>();

    public CS10_HubIntro(Scene scene, Player player)
      : base()
    {
      this.player = player;
      this.spawn = (scene as Level).GetSpawnPoint(player.Position);
      this.locks = scene.Entities.FindAll<LockBlock>();
      this.locks.Sort((Comparison<LockBlock>) ((a, b) => (int) ((double) a.Y - (double) b.Y)));
      foreach (Entity entity in this.locks)
        entity.Visible = false;
      this.booster = scene.Entities.FindFirst<Booster>();
      if (this.booster == null)
        return;
      this.booster.Visible = false;
    }

    public override void OnBegin(Level level) => this.Add((Component) new Coroutine(this.Cutscene(level)));

    private IEnumerator Cutscene(Level level)
    {
      CS10_HubIntro cs10HubIntro1 = this;
      if (cs10HubIntro1.player.Holding != null)
        cs10HubIntro1.player.Throw();
      cs10HubIntro1.player.StateMachine.State = 11;
      cs10HubIntro1.player.ForceCameraUpdate = true;
      while (!cs10HubIntro1.player.OnGround())
        yield return (object) null;
      cs10HubIntro1.player.ForceCameraUpdate = false;
      yield return (object) 0.1f;
      cs10HubIntro1.player.DummyAutoAnimate = false;
      cs10HubIntro1.player.Sprite.Play("lookUp");
      yield return (object) 0.25f;
      Level level1 = level;
      CS10_HubIntro cs10HubIntro2 = cs10HubIntro1;
      double x = (double) cs10HubIntro1.spawn.X;
      double y = (double) level.Bounds.Top + 190.0;
      CS10_HubIntro.Bird bird1;
      CS10_HubIntro.Bird bird2 = bird1 = new CS10_HubIntro.Bird(new Vector2((float) x, (float) y));
      cs10HubIntro2.bird = bird1;
      CS10_HubIntro.Bird bird3 = bird2;
      level1.Add((Entity) bird3);
      Audio.Play("event:/new_content/game/10_farewell/bird_camera_pan_up");
      yield return (object) CutsceneEntity.CameraTo(new Vector2(cs10HubIntro1.spawn.X - 160f, (float) ((double) level.Bounds.Top + 190.0 - 90.0)), 2f, Ease.CubeInOut);
      yield return (object) cs10HubIntro1.bird.IdleRoutine();
      cs10HubIntro1.Add((Component) new Coroutine(CutsceneEntity.CameraTo(new Vector2(level.Camera.X, (float) level.Bounds.Top), 0.8f, Ease.CubeInOut, 0.1f)));
      Input.Rumble(RumbleStrength.Light, RumbleLength.Long);
      yield return (object) cs10HubIntro1.bird.FlyAwayRoutine();
      cs10HubIntro1.bird.RemoveSelf();
      cs10HubIntro1.bird = (CS10_HubIntro.Bird) null;
      yield return (object) 0.5f;
      float duration = 6f;
      string sfx = "event:/new_content/game/10_farewell/locked_door_appear_1".Substring(0, "event:/new_content/game/10_farewell/locked_door_appear_1".Length - 1);
      int doorIndex = 1;
      cs10HubIntro1.Add((Component) new Coroutine(CutsceneEntity.CameraTo(new Vector2(level.Camera.X, (float) (level.Bounds.Bottom - 180)), duration, Ease.SineInOut)));
      cs10HubIntro1.Add((Component) new Coroutine(cs10HubIntro1.Level.ZoomTo(new Vector2(160f, 90f), 1.5f, duration)));
      for (float t = 0.0f; (double) t < (double) duration; t += Engine.DeltaTime)
      {
        foreach (LockBlock lockBlock in cs10HubIntro1.locks)
        {
          if (!lockBlock.Visible && (double) level.Camera.Y + 90.0 > (double) lockBlock.Y - 20.0)
          {
            cs10HubIntro1.sfxs.Add(Audio.Play(sfx + (object) doorIndex, lockBlock.Center));
            lockBlock.Appear();
            Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
            ++doorIndex;
          }
        }
        yield return (object) null;
      }
      sfx = (string) null;
      yield return (object) 0.5f;
      if (cs10HubIntro1.booster != null)
      {
        Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
        cs10HubIntro1.booster.Appear();
      }
      yield return (object) 0.3f;
      yield return (object) cs10HubIntro1.Level.ZoomBack(0.3f);
      cs10HubIntro1.EndCutscene(level);
    }

    public override void OnEnd(Level level)
    {
      if (this.WasSkipped)
      {
        foreach (EventInstance instance in this.sfxs)
          Audio.Stop(instance);
        if (this.bird != null)
          Audio.Stop(this.bird.sfx);
      }
      foreach (Entity entity in this.locks)
        entity.Visible = true;
      if (this.booster != null)
        this.booster.Visible = true;
      if (this.bird != null)
        this.bird.RemoveSelf();
      if (this.WasSkipped)
        this.player.Position = this.spawn;
      this.player.Speed = Vector2.Zero;
      this.player.DummyAutoAnimate = true;
      this.player.ForceCameraUpdate = false;
      this.player.StateMachine.State = 0;
      level.Camera.Y = (float) (level.Bounds.Bottom - 180);
      level.Session.SetFlag("hub_intro");
      level.ResetZoom();
    }

    private class Bird : Entity
    {
      private Sprite sprite;
      public EventInstance sfx;

      public Bird(Vector2 position)
      {
        this.Position = position;
        this.Depth = -8500;
        this.Add((Component) (this.sprite = GFX.SpriteBank.Create("bird")));
        this.sprite.Play("hover");
        this.sprite.OnFrameChange = (Action<string>) (spr => BirdNPC.FlapSfxCheck(this.sprite));
      }

      public IEnumerator IdleRoutine()
      {
        yield return (object) 0.5f;
      }

      public IEnumerator FlyAwayRoutine()
      {
        CS10_HubIntro.Bird bird = this;
        Level level = bird.Scene as Level;
        bird.sfx = Audio.Play("event:/new_content/game/10_farewell/bird_fly_uptonext", bird.Position);
        bird.sprite.Play("flyup");
        float spd = -32f;
        while ((double) bird.Y > (double) (level.Bounds.Top - 32))
        {
          spd -= 400f * Engine.DeltaTime;
          bird.Y += spd * Engine.DeltaTime;
          yield return (object) null;
        }
      }
    }
  }
}
