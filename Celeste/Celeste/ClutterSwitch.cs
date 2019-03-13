// Decompiled with JetBrains decompiler
// Type: Celeste.ClutterSwitch
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Celeste
{
  public class ClutterSwitch : Solid
  {
    private float targetXScale = 1f;
    public const float LightingAlphaAdd = 0.05f;
    public static ParticleType P_Pressed;
    public static ParticleType P_ClutterFly;
    private const int PressedAdd = 10;
    private const int PressedSpriteAdd = 2;
    private const int UnpressedLightRadius = 32;
    private const int PressedLightRadius = 24;
    private const int BrightLightRadius = 64;
    private ClutterBlock.Colors color;
    private float startY;
    private float atY;
    private float speedY;
    private bool pressed;
    private Sprite sprite;
    private Monocle.Image icon;
    private VertexLight vertexLight;
    private bool playerWasOnTop;
    private SoundSource cutsceneSfx;

    public ClutterSwitch(Vector2 position, ClutterBlock.Colors color)
      : base(position, 32f, 16f, true)
    {
      this.color = color;
      this.startY = this.atY = this.Y;
      this.OnDashCollide = new DashCollision(this.OnDashed);
      this.SurfaceSoundIndex = 21;
      this.Add((Component) (this.sprite = GFX.SpriteBank.Create("clutterSwitch")));
      this.sprite.Position = new Vector2(16f, 16f);
      this.sprite.Play("idle", false, false);
      this.Add((Component) (this.icon = new Monocle.Image(GFX.Game["objects/resortclutter/icon_" + color.ToString()])));
      this.icon.CenterOrigin();
      this.icon.Position = new Vector2(16f, 8f);
      this.Add((Component) (this.vertexLight = new VertexLight(new Vector2(this.CenterX - this.X, -1f), Color.get_Aqua(), 1f, 32, 64)));
    }

    public ClutterSwitch(EntityData data, Vector2 offset)
      : this(Vector2.op_Addition(data.Position, offset), data.Enum<ClutterBlock.Colors>("type", ClutterBlock.Colors.Green))
    {
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      if (this.color == ClutterBlock.Colors.Lightning && this.SceneAs<Level>().Session.GetFlag("disable_lightning"))
      {
        this.BePressed();
      }
      else
      {
        if (!this.SceneAs<Level>().Session.GetFlag("oshiro_clutter_cleared_" + (object) (int) this.color))
          return;
        this.BePressed();
      }
    }

    private void BePressed()
    {
      this.pressed = true;
      this.atY += 10f;
      this.Y += 10f;
      this.sprite.Y += 2f;
      this.sprite.Play("active", false, false);
      this.Remove((Component) this.icon);
      this.vertexLight.StartRadius = 24f;
      this.vertexLight.EndRadius = 48f;
    }

    public override void Update()
    {
      base.Update();
      if (this.HasPlayerOnTop())
      {
        if ((double) this.speedY < 0.0)
          this.speedY = 0.0f;
        this.speedY = Calc.Approach(this.speedY, 70f, 200f * Engine.DeltaTime);
        this.MoveTowardsY(this.atY + (this.pressed ? 2f : 4f), this.speedY * Engine.DeltaTime);
        this.targetXScale = 1.2f;
        if (!this.playerWasOnTop)
          Audio.Play("event:/game/03_resort/clutterswitch_squish", this.Position);
        this.playerWasOnTop = true;
      }
      else
      {
        if ((double) this.speedY > 0.0)
          this.speedY = 0.0f;
        this.speedY = Calc.Approach(this.speedY, -150f, 200f * Engine.DeltaTime);
        this.MoveTowardsY(this.atY, -this.speedY * Engine.DeltaTime);
        this.targetXScale = 1f;
        if (this.playerWasOnTop)
          Audio.Play("event:/game/03_resort/clutterswitch_return", this.Position);
        this.playerWasOnTop = false;
      }
      this.sprite.Scale.X = (__Null) (double) Calc.Approach((float) this.sprite.Scale.X, this.targetXScale, 0.8f * Engine.DeltaTime);
    }

    private DashCollisionResults OnDashed(Player player, Vector2 direction)
    {
      if (!this.pressed && Vector2.op_Equality(direction, Vector2.get_UnitY()))
      {
        Celeste.Celeste.Freeze(0.2f);
        Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
        Level scene = this.Scene as Level;
        scene.Session.SetFlag("oshiro_clutter_cleared_" + (object) (int) this.color, true);
        scene.Session.SetFlag("oshiro_clutter_door_open", false);
        this.vertexLight.StartRadius = 64f;
        this.vertexLight.EndRadius = 128f;
        scene.DirectionalShake(Vector2.get_UnitY(), 0.6f);
        scene.Particles.Emit(ClutterSwitch.P_Pressed, 20, Vector2.op_Subtraction(this.TopCenter, Vector2.op_Multiply(Vector2.get_UnitY(), 10f)), new Vector2(16f, 8f));
        this.BePressed();
        this.sprite.Scale.X = (__Null) 1.5;
        if (this.color == ClutterBlock.Colors.Lightning)
          this.Add((Component) new Coroutine(this.LightningRoutine(player), true));
        else
          this.Add((Component) new Coroutine(this.AbsorbRoutine(player), true));
      }
      return DashCollisionResults.NormalCollision;
    }

    private IEnumerator LightningRoutine(Player player)
    {
      // ISSUE: reference to a compiler-generated field
      int num = this.\u003C\u003E1__state;
      ClutterSwitch clutterSwitch = this;
      if (num != 0)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      clutterSwitch.SceneAs<Level>().Session.SetFlag("disable_lightning", true);
      foreach (Entity entity in clutterSwitch.Scene.Entities.FindAll<Lightning>())
        entity.RemoveSelf();
      return false;
    }

    private IEnumerator AbsorbRoutine(Player player)
    {
      ClutterSwitch clutterSwitch = this;
      clutterSwitch.Add((Component) (clutterSwitch.cutsceneSfx = new SoundSource()));
      float duration = 0.0f;
      if (clutterSwitch.color == ClutterBlock.Colors.Green)
      {
        clutterSwitch.cutsceneSfx.Play("event:/game/03_resort/clutterswitch_books", (string) null, 0.0f);
        duration = 6.366f;
      }
      else if (clutterSwitch.color == ClutterBlock.Colors.Red)
      {
        clutterSwitch.cutsceneSfx.Play("event:/game/03_resort/clutterswitch_linens", (string) null, 0.0f);
        duration = 6.15f;
      }
      else if (clutterSwitch.color == ClutterBlock.Colors.Yellow)
      {
        clutterSwitch.cutsceneSfx.Play("event:/game/03_resort/clutterswitch_boxes", (string) null, 0.0f);
        duration = 6.066f;
      }
      clutterSwitch.Add((Component) Alarm.Create(Alarm.AlarmMode.Oneshot, (Action) (() => Audio.Play("event:/game/03_resort/clutterswitch_finish", this.Position)), duration, true));
      player.StateMachine.State = 11;
      Vector2 target = Vector2.op_Addition(clutterSwitch.Position, new Vector2(clutterSwitch.Width / 2f, 0.0f));
      ClutterAbsorbEffect effect = new ClutterAbsorbEffect();
      clutterSwitch.Scene.Add((Entity) effect);
      clutterSwitch.sprite.Play("break", false, false);
      Level level = clutterSwitch.SceneAs<Level>();
      ++level.Session.Audio.Music.Progress;
      level.Session.Audio.Apply();
      level.Session.LightingAlphaAdd -= 0.05f;
      float start1 = level.Lighting.Alpha;
      Tween tween1 = Tween.Create(Tween.TweenMode.Oneshot, Ease.SineInOut, 2f, true);
      tween1.OnUpdate = (Action<Tween>) (t => level.Lighting.Alpha = MathHelper.Lerp(start1, 0.05f, t.Eased));
      clutterSwitch.Add((Component) tween1);
      Alarm.Set((Entity) clutterSwitch, 3f, (Action) (() =>
      {
        float start2 = this.vertexLight.StartRadius;
        float end = this.vertexLight.EndRadius;
        Tween tween2 = Tween.Create(Tween.TweenMode.Oneshot, Ease.SineInOut, 2f, true);
        tween2.OnUpdate = (Action<Tween>) (t =>
        {
          level.Lighting.Alpha = MathHelper.Lerp(0.05f, level.BaseLightingAlpha + level.Session.LightingAlphaAdd, t.Eased);
          this.vertexLight.StartRadius = (float) (int) Math.Round((double) MathHelper.Lerp(start2, 24f, t.Eased));
          this.vertexLight.EndRadius = (float) (int) Math.Round((double) MathHelper.Lerp(end, 48f, t.Eased));
        });
        this.Add((Component) tween2);
      }), Alarm.AlarmMode.Oneshot);
      Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
      foreach (ClutterBlock clutterBlock in clutterSwitch.Scene.Entities.FindAll<ClutterBlock>())
      {
        if (clutterBlock.BlockColor == clutterSwitch.color)
          clutterBlock.Absorb(effect);
      }
      foreach (ClutterBlockBase clutterBlockBase in clutterSwitch.Scene.Entities.FindAll<ClutterBlockBase>())
      {
        if (clutterBlockBase.BlockColor == clutterSwitch.color)
          clutterBlockBase.Deactivate();
      }
      yield return (object) 1.5f;
      player.StateMachine.State = 0;
      List<MTexture> images = GFX.Game.GetAtlasSubtextures("objects/resortclutter/" + clutterSwitch.color.ToString() + "_");
      for (int i = 0; i < 25; ++i)
      {
        for (int index = 0; index < 5; ++index)
          effect.FlyClutter(Vector2.op_Addition(target, Calc.AngleToVector(Calc.Random.NextFloat(6.283185f), 320f)), Calc.Random.Choose<MTexture>(images), false, 0.0f);
        level.Shake(0.3f);
        Input.Rumble(RumbleStrength.Light, RumbleLength.Long);
        yield return (object) 0.05f;
      }
      yield return (object) 1.5f;
      effect.CloseCabinets();
      yield return (object) 0.2f;
      Input.Rumble(RumbleStrength.Medium, RumbleLength.FullSecond);
      yield return (object) 0.3f;
    }

    public override void Removed(Scene scene)
    {
      Level level = scene as Level;
      level.Lighting.Alpha = level.BaseLightingAlpha + level.Session.LightingAlphaAdd;
      base.Removed(scene);
    }
  }
}
