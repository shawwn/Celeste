// Decompiled with JetBrains decompiler
// Type: Celeste.HeartGemDisplay
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class HeartGemDisplay : Component
  {
    public Vector2 Position;
    public Sprite[] Sprites;
    public Vector2 TargetPosition;
    private Monocle.Image bg;
    private Wiggler rotateWiggler;
    private Coroutine routine;
    private Vector2 bounce;
    private Tween tween;

    public HeartGemDisplay(int heartgem, bool hasGem)
      : base(true, true)
    {
      this.Sprites = new Sprite[3];
      for (int index = 0; index < this.Sprites.Length; ++index)
      {
        this.Sprites[index] = GFX.GuiSpriteBank.Create(nameof (heartgem) + (object) index);
        this.Sprites[index].Visible = heartgem == index & hasGem;
        this.Sprites[index].Play("spin", false, false);
      }
      this.bg = new Monocle.Image(GFX.Gui["collectables/heartgem/0/spin00"]);
      this.bg.Color = Color.Black;
      this.bg.CenterOrigin();
      this.rotateWiggler = Wiggler.Create(0.4f, 6f, (Action<float>) null, false, false);
      this.rotateWiggler.UseRawDeltaTime = true;
      SimpleCurve curve = new SimpleCurve(Vector2.UnitY * 80f, Vector2.Zero, Vector2.UnitY * -160f);
      this.tween = Tween.Create(Tween.TweenMode.Oneshot, (Ease.Easer) null, 0.4f, false);
      this.tween.OnStart = (Action<Tween>) (t => this.SpriteColor = Color.Transparent);
      this.tween.OnUpdate = (Action<Tween>) (t =>
      {
        this.bounce = curve.GetPoint(t.Eased);
        this.SpriteColor = Color.White * Calc.LerpClamp(0.0f, 1f, t.Percent * 1.5f);
      });
    }

    private Color SpriteColor
    {
      get
      {
        return this.Sprites[0].Color;
      }
      set
      {
        for (int index = 0; index < this.Sprites.Length; ++index)
          this.Sprites[index].Color = value;
      }
    }

    public void Wiggle()
    {
      this.rotateWiggler.Start();
      for (int index = 0; index < this.Sprites.Length; ++index)
      {
        if (this.Sprites[index].Visible)
        {
          this.Sprites[index].Play("spin", true, false);
          this.Sprites[index].SetAnimationFrame(19);
        }
      }
    }

    public void Appear(AreaMode mode)
    {
      this.tween.Start();
      this.routine = new Coroutine(this.AppearSequence(this.Sprites[(int) mode]), true);
      this.routine.UseRawDeltaTime = true;
    }

    public void SetCurrentMode(AreaMode mode, bool has)
    {
      for (int index = 0; index < this.Sprites.Length; ++index)
        this.Sprites[index].Visible = (AreaMode) index == mode & has;
      if (has)
        return;
      this.routine = (Coroutine) null;
    }

    public override void Update()
    {
      base.Update();
      if (this.routine != null && this.routine.Active)
        this.routine.Update();
      if (this.rotateWiggler.Active)
        this.rotateWiggler.Update();
      for (int index = 0; index < this.Sprites.Length; ++index)
      {
        if (this.Sprites[index].Active)
          this.Sprites[index].Update();
      }
      if (this.tween != null && this.tween.Active)
        this.tween.Update();
      this.Position = Calc.Approach(this.Position, this.TargetPosition, 200f * Engine.DeltaTime);
      for (int index = 0; index < this.Sprites.Length; ++index)
      {
        this.Sprites[index].Scale.X = Calc.Approach(this.Sprites[index].Scale.X, 1f, 2f * Engine.DeltaTime);
        this.Sprites[index].Scale.Y = Calc.Approach(this.Sprites[index].Scale.Y, 1f, 2f * Engine.DeltaTime);
      }
    }

    public override void Render()
    {
      base.Render();
      this.bg.Position = this.Entity.Position + this.Position;
      for (int index = 0; index < this.Sprites.Length; ++index)
      {
        if (this.Sprites[index].Visible)
        {
          this.Sprites[index].Rotation = (float) ((double) this.rotateWiggler.Value * 30.0 * (Math.PI / 180.0));
          this.Sprites[index].Position = this.Entity.Position + this.Position + this.bounce;
          this.Sprites[index].Render();
        }
      }
    }

    private IEnumerator AppearSequence(Sprite sprite)
    {
      sprite.Play("idle", false, false);
      sprite.Visible = true;
      sprite.Scale = new Vector2(0.8f, 1.4f);
      yield return (object) this.tween.Wait();
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
      sprite.Scale = new Vector2(1.4f, 0.8f);
      yield return (object) 0.4f;
      sprite.CenterOrigin();
      this.rotateWiggler.Start();
      Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
      sprite.Play("spin", false, false);
      this.routine = (Coroutine) null;
    }
  }
}

