// Decompiled with JetBrains decompiler
// Type: Monocle.Particle
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Monocle
{
  public struct Particle
  {
    public Entity Track;
    public ParticleType Type;
    public MTexture Source;
    public bool Active;
    public Color Color;
    public Color StartColor;
    public Vector2 Position;
    public Vector2 Speed;
    public float Size;
    public float StartSize;
    public float Life;
    public float StartLife;
    public float ColorSwitch;
    public float Rotation;
    public float Spin;

    public bool SimulateFor(float duration)
    {
      if ((double) duration > (double) this.Life)
      {
        this.Life = 0.0f;
        this.Active = false;
        return false;
      }
      float num1 = Engine.TimeRate * ((float) Engine.Instance.TargetElapsedTime.Milliseconds / 1000f);
      if ((double) num1 > 0.0)
      {
        for (float num2 = 0.0f; (double) num2 < (double) duration; num2 += num1)
          this.Update(new float?(num1));
      }
      return true;
    }

    public void Update(float? delta = null)
    {
      float num1 = !delta.HasValue ? (this.Type.UseActualDeltaTime ? Engine.RawDeltaTime : Engine.DeltaTime) : delta.Value;
      float num2 = this.Life / this.StartLife;
      this.Life -= num1;
      if ((double) this.Life <= 0.0)
      {
        this.Active = false;
      }
      else
      {
        if (this.Type.RotationMode == ParticleType.RotationModes.SameAsDirection)
        {
          if (this.Speed != Vector2.Zero)
            this.Rotation = this.Speed.Angle();
        }
        else
          this.Rotation += this.Spin * num1;
        float num3 = this.Type.FadeMode != ParticleType.FadeModes.Linear ? (this.Type.FadeMode != ParticleType.FadeModes.Late ? (this.Type.FadeMode != ParticleType.FadeModes.InAndOut ? 1f : ((double) num2 <= 0.75 ? ((double) num2 >= 0.25 ? 1f : num2 / 0.25f) : (float) (1.0 - ((double) num2 - 0.75) / 0.25))) : Math.Min(1f, num2 / 0.25f)) : num2;
        if ((double) num3 == 0.0)
        {
          this.Color = Color.Transparent;
        }
        else
        {
          if (this.Type.ColorMode == ParticleType.ColorModes.Static)
            this.Color = this.StartColor;
          else if (this.Type.ColorMode == ParticleType.ColorModes.Fade)
            this.Color = Color.Lerp(this.Type.Color2, this.StartColor, num2);
          else if (this.Type.ColorMode == ParticleType.ColorModes.Blink)
            this.Color = Calc.BetweenInterval(this.Life, 0.1f) ? this.StartColor : this.Type.Color2;
          else if (this.Type.ColorMode == ParticleType.ColorModes.Choose)
            this.Color = this.StartColor;
          if ((double) num3 < 1.0)
            this.Color *= num3;
        }
        this.Position += this.Speed * num1;
        this.Speed += this.Type.Acceleration * num1;
        this.Speed = Calc.Approach(this.Speed, Vector2.Zero, this.Type.Friction * num1);
        if ((double) this.Type.SpeedMultiplier != 1.0)
          this.Speed *= (float) Math.Pow((double) this.Type.SpeedMultiplier, (double) num1);
        if (!this.Type.ScaleOut)
          return;
        this.Size = this.StartSize * Ease.CubeOut(num2);
      }
    }

    public void Render()
    {
      Vector2 position = new Vector2((float) (int) this.Position.X, (float) (int) this.Position.Y);
      if (this.Track != null)
        position += this.Track.Position;
      Draw.SpriteBatch.Draw(this.Source.Texture.Texture, position, new Rectangle?(this.Source.ClipRect), this.Color, this.Rotation, this.Source.Center, this.Size, SpriteEffects.None, 0.0f);
    }

    public void Render(float alpha)
    {
      Vector2 position = new Vector2((float) (int) this.Position.X, (float) (int) this.Position.Y);
      if (this.Track != null)
        position += this.Track.Position;
      Draw.SpriteBatch.Draw(this.Source.Texture.Texture, position, new Rectangle?(this.Source.ClipRect), this.Color * alpha, this.Rotation, this.Source.Center, this.Size, SpriteEffects.None, 0.0f);
    }
  }
}

