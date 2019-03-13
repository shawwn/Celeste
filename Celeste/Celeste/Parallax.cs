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
    public Vector2 CameraOffset = Vector2.get_Zero();
    public BlendState BlendState = (BlendState) BlendState.AlphaBlend;
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
      this.Position = Vector2.op_Addition(this.Position, Vector2.op_Multiply(this.Speed, Engine.DeltaTime));
      if (this.DoFadeIn)
        this.fadeIn = Calc.Approach(this.fadeIn, this.Visible ? 1f : 0.0f, Engine.DeltaTime);
      else
        this.fadeIn = this.Visible ? 1f : 0.0f;
    }

    public override void Render(Scene scene)
    {
      Vector2 vector2_1 = Vector2.op_Addition((scene as Level).Camera.Position, this.CameraOffset).Floor();
      Vector2 vector2_2 = Vector2.op_Subtraction(this.Position, Vector2.op_Multiply(vector2_1, this.Scroll)).Floor();
      float fadeIn = this.fadeIn;
      if (this.FadeX != null)
        fadeIn *= this.FadeX.Value((float) (vector2_1.X + 160.0));
      if (this.FadeY != null)
        fadeIn *= this.FadeY.Value((float) (vector2_1.Y + 90.0));
      Color color = this.Color;
      if ((double) fadeIn < 1.0)
        color = Color.op_Multiply(color, fadeIn);
      if (((Color) ref color).get_A() <= (byte) 1)
        return;
      if (this.LoopX)
      {
        while (vector2_2.X < 0.0)
        {
          ref __Null local = ref vector2_2.X;
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          ^(float&) ref local = ^(float&) ref local + (float) this.Texture.Width;
        }
        while (vector2_2.X > 0.0)
        {
          ref __Null local = ref vector2_2.X;
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          ^(float&) ref local = ^(float&) ref local - (float) this.Texture.Width;
        }
      }
      if (this.LoopY)
      {
        while (vector2_2.Y < 0.0)
        {
          ref __Null local = ref vector2_2.Y;
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          ^(float&) ref local = ^(float&) ref local + (float) this.Texture.Height;
        }
        while (vector2_2.Y > 0.0)
        {
          ref __Null local = ref vector2_2.Y;
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          ^(float&) ref local = ^(float&) ref local - (float) this.Texture.Height;
        }
      }
      SpriteEffects flip = (SpriteEffects) 0;
      if (this.FlipX && this.FlipY)
        flip = (SpriteEffects) 3;
      else if (this.FlipX)
        flip = (SpriteEffects) 1;
      else if (this.FlipY)
        flip = (SpriteEffects) 2;
      for (float x = (float) vector2_2.X; (double) x < 320.0; x += (float) this.Texture.Width)
      {
        for (float y = (float) vector2_2.Y; (double) y < 180.0; y += (float) this.Texture.Height)
        {
          this.Texture.Draw(new Vector2(x, y), Vector2.get_Zero(), color, 1f, 0.0f, flip);
          if (!this.LoopY)
            break;
        }
        if (!this.LoopX)
          break;
      }
    }
  }
}
