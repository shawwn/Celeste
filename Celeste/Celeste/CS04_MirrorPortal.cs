// Decompiled with JetBrains decompiler
// Type: Celeste.CS04_MirrorPortal
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class CS04_MirrorPortal : CutsceneEntity
  {
    private Player player;
    private TempleMirrorPortal portal;
    private CS04_MirrorPortal.Fader fader;
    private SoundSource sfx;

    public CS04_MirrorPortal(Player player, TempleMirrorPortal portal)
      : base(true, false)
    {
      this.player = player;
      this.portal = portal;
    }

    public override void OnBegin(Level level)
    {
      this.Add((Component) new Coroutine(this.Cutscene(level), true));
      level.Add((Entity) (this.fader = new CS04_MirrorPortal.Fader()));
    }

    private IEnumerator Cutscene(Level level)
    {
      this.player.StateMachine.State = Player.StDummy;
      this.player.StateMachine.Locked = true;
      this.player.Dashes = 1;
      if (level.Session.Area.Mode == AreaMode.Normal)
        Audio.SetMusic((string) null, true, true);
      else
        this.Add((Component) new Coroutine(this.MusicFadeOutBSide(), true));
      this.Add((Component) (this.sfx = new SoundSource()));
      this.sfx.Position = this.portal.Center;
      this.sfx.Play("event:/music/lvl5/mirror_cutscene", (string) null, 0.0f);
      this.Add((Component) new Coroutine(this.CenterCamera(), true));
      yield return (object) this.player.DummyWalkToExact((int) this.portal.X, false, 1f);
      yield return (object) 0.25f;
      yield return (object) this.player.DummyWalkToExact((int) this.portal.X - 16, false, 1f);
      yield return (object) 0.5f;
      yield return (object) this.player.DummyWalkToExact((int) this.portal.X + 16, false, 1f);
      yield return (object) 0.25f;
      this.player.Facing = Facings.Left;
      yield return (object) 0.25f;
      yield return (object) this.player.DummyWalkToExact((int) this.portal.X, false, 1f);
      yield return (object) 0.1f;
      this.player.DummyAutoAnimate = false;
      this.player.Sprite.Play("lookUp", false, false);
      yield return (object) 1f;
      this.player.DummyAutoAnimate = true;
      this.portal.Activate();
      this.Add((Component) new Coroutine(level.ZoomTo(new Vector2(160f, 90f), 3f, 12f), true));
      yield return (object) 0.25f;
      this.player.ForceStrongWindHair.X = -1f;
      yield return (object) this.player.DummyWalkToExact((int) this.player.X + 12, true, 1f);
      yield return (object) 0.5f;
      this.player.Facing = Facings.Right;
      this.player.DummyAutoAnimate = false;
      this.player.DummyGravity = false;
      this.player.Sprite.Play("runWind", false, false);
      while ((double) this.player.Sprite.Rate > 0.0)
      {
        this.player.MoveH(this.player.Sprite.Rate * 10f * Engine.DeltaTime, (Collision) null, (Solid) null);
        this.player.MoveV((float) (-(1.0 - (double) this.player.Sprite.Rate) * 6.0) * Engine.DeltaTime, (Collision) null, (Solid) null);
        this.player.Sprite.Rate -= Engine.DeltaTime * 0.15f;
        yield return (object) null;
      }
      yield return (object) 0.5f;
      this.player.Sprite.Play("fallFast", false, false);
      this.player.Sprite.Rate = 1f;
      Vector2 target = this.portal.Center + new Vector2(0.0f, 8f);
      Vector2 from = this.player.Position;
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime * 2f)
      {
        this.player.Position = from + (target - from) * Ease.SineInOut(p);
        yield return (object) null;
      }
      this.player.ForceStrongWindHair.X = 0.0f;
      target = new Vector2();
      from = new Vector2();
      this.fader.Target = 1f;
      yield return (object) 2f;
      this.player.Sprite.Play("sleep", false, false);
      yield return (object) 1f;
      yield return (object) level.ZoomBack(1f);
      if (level.Session.Area.Mode == AreaMode.Normal)
      {
        level.Session.ColorGrade = "templevoid";
        for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime)
        {
          Glitch.Value = p * 0.05f;
          level.ScreenPadding = 32f * p;
          yield return (object) null;
        }
      }
      while ((double) (this.portal.DistortionFade -= Engine.DeltaTime * 2f) > 0.0)
        yield return (object) null;
      this.EndCutscene(level, true);
    }

    private IEnumerator CenterCamera()
    {
      Camera camera = this.Level.Camera;
      Vector2 target = this.portal.Center - new Vector2(160f, 90f);
      while ((double) (camera.Position - target).Length() > 1.0)
      {
        camera.Position += (target - camera.Position) * (1f - (float) Math.Pow(0.00999999977648258, (double) Engine.DeltaTime));
        yield return (object) null;
      }
    }

    private IEnumerator MusicFadeOutBSide()
    {
      for (float p = 1f; (double) p > 0.0; p -= Engine.DeltaTime)
      {
        Audio.SetMusicParam("fade", p);
        yield return (object) null;
      }
      Audio.SetMusicParam("fade", 0.0f);
    }

    public override void OnEnd(Level level)
    {
      level.OnEndOfFrame += (Action) (() =>
      {
        if (this.fader != null && !this.WasSkipped)
        {
          this.fader.Tag = (int) Tags.Global;
          this.fader.Target = 0.0f;
          this.fader.Ended = true;
        }
        Leader.StoreStrawberries(this.player.Leader);
        level.Remove((Entity) this.player);
        level.UnloadLevel();
        level.Session.Dreaming = true;
        level.Session.Keys.Clear();
        if (level.Session.Area.Mode == AreaMode.Normal)
        {
          level.Session.Level = "void";
          Session session = level.Session;
          Level level1 = level;
          Rectangle bounds = level.Bounds;
          double left = (double) bounds.Left;
          bounds = level.Bounds;
          double top = (double) bounds.Top;
          Vector2 from = new Vector2((float) left, (float) top);
          Vector2? nullable = new Vector2?(level1.GetSpawnPoint(from));
          session.RespawnPoint = nullable;
          level.LoadLevel(Player.IntroTypes.TempleMirrorVoid, false);
        }
        else
        {
          level.Session.Level = "c-00";
          Session session = level.Session;
          Level level1 = level;
          Rectangle bounds = level.Bounds;
          double left = (double) bounds.Left;
          bounds = level.Bounds;
          double top = (double) bounds.Top;
          Vector2 from = new Vector2((float) left, (float) top);
          Vector2? nullable = new Vector2?(level1.GetSpawnPoint(from));
          session.RespawnPoint = nullable;
          level.LoadLevel(Player.IntroTypes.WakeUp, false);
          Audio.SetMusicParam("fade", 1f);
        }
        Leader.RestoreStrawberries(level.Tracker.GetEntity<Player>().Leader);
        level.Camera.Y -= 8f;
        if (!this.WasSkipped && level.Wipe != null)
          level.Wipe.Cancel();
        if (this.fader == null)
          return;
        this.fader.RemoveTag((int) Tags.Global);
      });
    }

    private class Fader : Entity
    {
      public float Target = 0.0f;
      private float fade = 0.0f;
      public bool Ended;

      public Fader()
      {
        this.Depth = -1000000;
      }

      public override void Update()
      {
        this.fade = Calc.Approach(this.fade, this.Target, Engine.DeltaTime * 0.5f);
        if ((double) this.Target <= 0.0 && (double) this.fade <= 0.0 && this.Ended)
          this.RemoveSelf();
        base.Update();
      }

      public override void Render()
      {
        Camera camera = (this.Scene as Level).Camera;
        if ((double) this.fade > 0.0)
          Draw.Rect(camera.X - 10f, camera.Y - 10f, 340f, 200f, Color.Black * this.fade);
        Player entity = this.Scene.Tracker.GetEntity<Player>();
        if (entity == null || entity.OnGround(2))
          return;
        entity.Render();
      }
    }
  }
}

