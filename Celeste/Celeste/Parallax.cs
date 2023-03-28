// Decompiled with JetBrains decompiler
// Type: Celeste.Parallax
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
  public class Parallax : Backdrop
  {
    public Vector2 CameraOffset = Vector2.Zero;
    public BlendState BlendState = BlendState.AlphaBlend;
    public MTexture Texture;
    public bool DoFadeIn;
    public float Alpha = 1f;
    private float fadeIn = 1f;

    public Parallax(MTexture texture)
    {
      this.Name = texture.AtlasPath;
      this.Texture = texture;
    }

    public override void Update(Scene scene)
    {
      base.Update(scene);
      this.Position = this.Position + this.Speed * Engine.DeltaTime;
      this.Position = this.Position + this.WindMultiplier * (scene as Level).Wind * Engine.DeltaTime;
      if (this.DoFadeIn)
        this.fadeIn = Calc.Approach(this.fadeIn, this.Visible ? 1f : 0.0f, Engine.DeltaTime);
      else
        this.fadeIn = this.Visible ? 1f : 0.0f;
    }

    public override void Render(Scene scene)
    {
      Vector2 vector2_1 = ((scene as Level).Camera.Position + this.CameraOffset).Floor();
      Vector2 vector2_2 = (this.Position - vector2_1 * this.Scroll).Floor();
      float num = this.fadeIn * this.Alpha * this.FadeAlphaMultiplier;
      if (this.FadeX != null)
        num *= this.FadeX.Value(vector2_1.X + 160f);
      if (this.FadeY != null)
        num *= this.FadeY.Value(vector2_1.Y + 90f);
      Color color = this.Color;
      if ((double) num < 1.0)
        color *= num;
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
