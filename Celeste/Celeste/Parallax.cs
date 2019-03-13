// Decompiled with JetBrains decompiler
// Type: Celeste.Parallax
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
  public class Parallax : Backdrop
  {
    public Vector2 CameraOffset = Vector2.Zero;
    public BlendState BlendState = BlendState.AlphaBlend;
    private float fadeIn = 1f;
    public MTexture Texture;
    public bool DoFadeIn;

    public Parallax(MTexture texture)
    {
      this.Name = texture.AtlasPath;
      this.Texture = texture;
    }

    public override void Update(Scene scene)
    {
      base.Update(scene);
      this.Position = this.Position + this.Speed * Engine.DeltaTime;
      if (this.DoFadeIn)
        this.fadeIn = Calc.Approach(this.fadeIn, this.Visible ? 1f : 0.0f, Engine.DeltaTime);
      else
        this.fadeIn = this.Visible ? 1f : 0.0f;
    }

    public override void Render(Scene scene)
    {
      Vector2 vector2_1 = ((scene as Level).Camera.Position + this.CameraOffset).Floor();
      Vector2 vector2_2 = (this.Position - vector2_1 * this.Scroll).Floor();
      float fadeIn = this.fadeIn;
      if (this.FadeX != null)
        fadeIn *= this.FadeX.Value(vector2_1.X + 160f);
      if (this.FadeY != null)
        fadeIn *= this.FadeY.Value(vector2_1.Y + 90f);
      Color color = this.Color;
      if ((double) fadeIn < 1.0)
        color *= fadeIn;
      if (color.A <= (byte) 1)
        return;
      if (this.LoopX)
      {
        while ((double) vector2_2.X < 0.0)
          vector2_2.X += (float) this.Texture.Width;
        while ((double) vector2_2.X > 0.0)
          vector2_2.X -= (float) this.Texture.Width;
      }
      if (this.LoopY)
      {
        while ((double) vector2_2.Y < 0.0)
          vector2_2.Y += (float) this.Texture.Height;
        while ((double) vector2_2.Y > 0.0)
          vector2_2.Y -= (float) this.Texture.Height;
      }
      SpriteEffects flip = SpriteEffects.None;
      if (this.FlipX && this.FlipY)
        flip = SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically;
      else if (this.FlipX)
        flip = SpriteEffects.FlipHorizontally;
      else if (this.FlipY)
        flip = SpriteEffects.FlipVertically;
      for (float x = vector2_2.X; (double) x < 320.0; x += (float) this.Texture.Width)
      {
        for (float y = vector2_2.Y; (double) y < 180.0; y += (float) this.Texture.Height)
        {
          this.Texture.Draw(new Vector2(x, y), Vector2.Zero, color, 1f, 0.0f, flip);
          if (!this.LoopY)
            break;
        }
        if (!this.LoopX)
          break;
      }
    }
  }
}

