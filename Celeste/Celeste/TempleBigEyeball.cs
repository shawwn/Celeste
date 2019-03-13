﻿// Decompiled with JetBrains decompiler
// Type: Celeste.TempleBigEyeball
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class TempleBigEyeball : Entity
  {
    private float pupilSpeed = 40f;
    private Sprite sprite;
    private Monocle.Image pupil;
    private bool triggered;
    private Vector2 pupilTarget;
    private float pupilDelay;
    private Wiggler bounceWiggler;
    private Wiggler pupilWiggler;
    private float shockwaveTimer;
    private bool shockwaveFlag;
    private bool bursting;

    public TempleBigEyeball(EntityData data, Vector2 offset)
      : base(Vector2.op_Addition(data.Position, offset))
    {
      this.Add((Component) (this.sprite = GFX.SpriteBank.Create("temple_eyeball")));
      this.Add((Component) (this.pupil = new Monocle.Image(GFX.Game["danger/templeeye/pupil"])));
      this.pupil.CenterOrigin();
      this.Collider = (Collider) new Hitbox(48f, 64f, -24f, -32f);
      this.Add((Component) new PlayerCollider(new Action<Player>(this.OnPlayer), (Collider) null, (Collider) null));
      this.Add((Component) new HoldableCollider(new Action<Holdable>(this.OnHoldable), (Collider) null));
      this.Add((Component) (this.bounceWiggler = Wiggler.Create(0.5f, 3f, (Action<float>) null, false, false)));
      this.Add((Component) (this.pupilWiggler = Wiggler.Create(0.5f, 3f, (Action<float>) null, false, false)));
      this.shockwaveTimer = 2f;
    }

    private void OnPlayer(Player player)
    {
      if (this.triggered)
        return;
      Audio.Play("event:/game/05_mirror_temple/eyewall_bounce", player.Position);
      player.ExplodeLaunch(Vector2.op_Addition(player.Center, Vector2.op_Multiply(Vector2.get_UnitX(), 20f)), true);
      this.bounceWiggler.Start();
    }

    private void OnHoldable(Holdable h)
    {
      if (!(h.Entity is TheoCrystal))
        return;
      TheoCrystal entity = h.Entity as TheoCrystal;
      if (this.triggered || entity.Speed.X <= 32.0 || entity.Hold.IsHeld)
        return;
      entity.Speed.X = (__Null) -50.0;
      entity.Speed.Y = (__Null) -10.0;
      this.triggered = true;
      this.bounceWiggler.Start();
      this.Collidable = false;
      Audio.SetAmbience((string) null, true);
      Audio.Play("event:/game/05_mirror_temple/eyewall_destroy", this.Position);
      Alarm.Set((Entity) this, 1.3f, (Action) (() => Audio.SetMusic((string) null, true, true)), Alarm.AlarmMode.Oneshot);
      this.Add((Component) new Coroutine(this.Burst(), true));
    }

    private IEnumerator Burst()
    {
      TempleBigEyeball templeBigEyeball = this;
      templeBigEyeball.bursting = true;
      Level level = templeBigEyeball.Scene as Level;
      level.StartCutscene(new Action<Level>(templeBigEyeball.OnSkip), false, true);
      level.RegisterAreaComplete();
      Celeste.Celeste.Freeze(0.1f);
      yield return (object) null;
      float start = Glitch.Value;
      Tween tween = Tween.Create(Tween.TweenMode.Oneshot, (Ease.Easer) null, 0.5f, true);
      tween.OnUpdate = (Action<Tween>) (t => Glitch.Value = MathHelper.Lerp(start, 0.0f, t.Eased));
      templeBigEyeball.Add((Component) tween);
      Player player = templeBigEyeball.Scene.Tracker.GetEntity<Player>();
      TheoCrystal entity = templeBigEyeball.Scene.Tracker.GetEntity<TheoCrystal>();
      if (player != null)
      {
        player.StateMachine.State = 11;
        player.StateMachine.Locked = true;
        if (player.OnGround(1))
        {
          player.DummyAutoAnimate = false;
          player.Sprite.Play("shaking", false, false);
        }
      }
      templeBigEyeball.Add((Component) new Coroutine(level.ZoomTo(Vector2.op_Subtraction(entity.TopCenter, level.Camera.Position), 2f, 0.5f), true));
      templeBigEyeball.Add((Component) new Coroutine(entity.Shatter(), true));
      foreach (TempleEye templeEye in templeBigEyeball.Scene.Entities.FindAll<TempleEye>())
        templeEye.Burst();
      templeBigEyeball.sprite.Play("burst", false, false);
      templeBigEyeball.pupil.Visible = false;
      level.Shake(0.4f);
      yield return (object) 2f;
      if (player != null && player.OnGround(1))
      {
        player.DummyAutoAnimate = false;
        player.Sprite.Play("shaking", false, false);
      }
      templeBigEyeball.Visible = false;
      TempleBigEyeball.Fader fade = new TempleBigEyeball.Fader();
      level.Add((Entity) fade);
      while ((double) (fade.Fade += Engine.DeltaTime) < 1.0)
        yield return (object) null;
      yield return (object) 1f;
      fade = (TempleBigEyeball.Fader) null;
      level.EndCutscene();
      level.CompleteArea(false, false);
    }

    private void OnSkip(Level level)
    {
      level.CompleteArea(false, false);
    }

    public override void Update()
    {
      base.Update();
      Player entity1 = this.Scene.Tracker.GetEntity<Player>();
      Rectangle bounds;
      if (entity1 != null)
      {
        double x1 = (double) entity1.X;
        bounds = (this.Scene as Level).Bounds;
        double left = (double) ((Rectangle) ref bounds).get_Left();
        double x2 = (double) this.X;
        Audio.SetMusicParam("eye_distance", Calc.ClampedMap((float) x1, (float) left, (float) x2, 0.0f, 1f));
      }
      if (entity1 != null && !this.bursting)
        Glitch.Value = Calc.ClampedMap(Math.Abs(this.X - entity1.X), 100f, 900f, 0.2f, 0.0f);
      if (!this.triggered && (double) this.shockwaveTimer > 0.0)
      {
        this.shockwaveTimer -= Engine.DeltaTime;
        if ((double) this.shockwaveTimer <= 0.0)
        {
          if (entity1 != null)
          {
            this.shockwaveTimer = Calc.ClampedMap(Math.Abs(this.X - entity1.X), 100f, 500f, 2f, 3f);
            this.shockwaveFlag = !this.shockwaveFlag;
            if (this.shockwaveFlag)
              --this.shockwaveTimer;
          }
          this.Scene.Add((Entity) Engine.Pooler.Create<TempleBigEyeballShockwave>().Init(Vector2.op_Addition(this.Center, new Vector2(50f, 0.0f))));
          this.pupilWiggler.Start();
          this.pupilTarget = new Vector2(-1f, 0.0f);
          this.pupilSpeed = 120f;
          this.pupilDelay = Math.Max(0.5f, this.pupilDelay);
        }
      }
      this.pupil.Position = Calc.Approach(this.pupil.Position, Vector2.op_Multiply(this.pupilTarget, 12f), Engine.DeltaTime * this.pupilSpeed);
      this.pupilSpeed = Calc.Approach(this.pupilSpeed, 40f, Engine.DeltaTime * 400f);
      TheoCrystal entity2 = this.Scene.Tracker.GetEntity<TheoCrystal>();
      if (entity2 != null && (double) Math.Abs(this.X - entity2.X) < 64.0 && (double) Math.Abs(this.Y - entity2.Y) < 64.0)
        this.pupilTarget = Vector2.op_Subtraction(entity2.Center, this.Position).SafeNormalize();
      else if ((double) this.pupilDelay < 0.0)
      {
        this.pupilTarget = Calc.AngleToVector(Calc.Random.NextFloat(6.283185f), 1f);
        this.pupilDelay = Calc.Random.Choose<float>(0.2f, 1f, 2f);
      }
      else
        this.pupilDelay -= Engine.DeltaTime;
      if (entity1 == null)
        return;
      Level scene = this.Scene as Level;
      double x = (double) entity1.X;
      bounds = scene.Bounds;
      double num1 = (double) (((Rectangle) ref bounds).get_Left() + 32);
      double num2 = (double) this.X - 32.0;
      Audio.SetMusicParam("eye_distance", Calc.ClampedMap((float) x, (float) num1, (float) num2, 1f, 0.0f));
    }

    public override void Render()
    {
      this.sprite.Scale.X = (__Null) (1.0 + 0.150000005960464 * (double) this.bounceWiggler.Value);
      this.pupil.Scale = Vector2.op_Multiply(Vector2.get_One(), (float) (1.0 + (double) this.pupilWiggler.Value * 0.150000005960464));
      base.Render();
    }

    private class Fader : Entity
    {
      public float Fade;

      public Fader()
      {
        this.Tag = (int) Tags.HUD;
      }

      public override void Render()
      {
        Draw.Rect(-10f, -10f, (float) (Engine.Width + 20), (float) (Engine.Height + 20), Color.op_Multiply(Color.get_White(), this.Fade));
      }
    }
  }
}
