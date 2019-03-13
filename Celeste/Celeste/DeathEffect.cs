// Decompiled with JetBrains decompiler
// Type: Celeste.DeathEffect
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  public class DeathEffect : Component
  {
    public float Duration = 0.834f;
    public Vector2 Position;
    public Color Color;
    public float Percent;
    public Action<float> OnUpdate;
    public Action OnEnd;

    public DeathEffect(Color color, Vector2? offset = null)
      : base(true, true)
    {
      this.Color = color;
      this.Position = offset.HasValue ? offset.Value : Vector2.Zero;
      this.Percent = 0.0f;
    }

    public override void Update()
    {
      base.Update();
      if ((double) this.Percent > 1.0)
      {
        this.RemoveSelf();
        if (this.OnEnd != null)
          this.OnEnd();
      }
      this.Percent = Calc.Approach(this.Percent, 1f, Engine.DeltaTime / this.Duration);
      if (this.OnUpdate == null)
        return;
      this.OnUpdate(this.Percent);
    }

    public override void Render()
    {
      DeathEffect.Draw(this.Entity.Position + this.Position, this.Color, this.Percent);
    }

    public static void Draw(Vector2 position, Color color, float ease)
    {
      Color color1 = Math.Floor((double) ease * 10.0) % 2.0 == 0.0 ? color : Color.White;
      MTexture mtexture = GFX.Game["characters/player/hair00"];
      float num = (double) ease < 0.5 ? 0.5f + ease : Ease.CubeOut((float) (1.0 - ((double) ease - 0.5) * 2.0));
      for (int index = 0; index < 8; ++index)
      {
        Vector2 vector = Calc.AngleToVector((float) (((double) index / 8.0 + (double) ease * 0.25) * 6.28318548202515), Ease.CubeOut(ease) * 24f);
        mtexture.DrawCentered(position + vector + new Vector2(-1f, 0.0f), Color.Black, new Vector2(num, num));
        mtexture.DrawCentered(position + vector + new Vector2(1f, 0.0f), Color.Black, new Vector2(num, num));
        mtexture.DrawCentered(position + vector + new Vector2(0.0f, -1f), Color.Black, new Vector2(num, num));
        mtexture.DrawCentered(position + vector + new Vector2(0.0f, 1f), Color.Black, new Vector2(num, num));
      }
      for (int index = 0; index < 8; ++index)
      {
        Vector2 vector = Calc.AngleToVector((float) (((double) index / 8.0 + (double) ease * 0.25) * 6.28318548202515), Ease.CubeOut(ease) * 24f);
        mtexture.DrawCentered(position + vector, color1, new Vector2(num, num));
      }
    }
  }
}

