// Decompiled with JetBrains decompiler
// Type: Celeste.StarfieldWipe
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;

namespace Celeste
{
  public class StarfieldWipe : ScreenWipe
  {
    public static readonly BlendState SubtractBlendmode = new BlendState()
    {
      ColorSourceBlend = Blend.One,
      ColorDestinationBlend = Blend.One,
      ColorBlendFunction = BlendFunction.ReverseSubtract,
      AlphaSourceBlend = Blend.One,
      AlphaDestinationBlend = Blend.One,
      AlphaBlendFunction = BlendFunction.Add
    };
    private StarfieldWipe.Star[] stars = new StarfieldWipe.Star[64];
    private VertexPositionColor[] verts = new VertexPositionColor[1536];
    private Vector2[] starShape = new Vector2[5];
    private bool hasDrawn;

    public StarfieldWipe(Scene scene, bool wipeIn, Action onComplete = null)
      : base(scene, wipeIn, onComplete)
    {
      for (int index = 0; index < 5; ++index)
        this.starShape[index] = Calc.AngleToVector((float) ((double) index / 5.0 * 6.2831854820251465), 1f);
      for (int index = 0; index < this.stars.Length; ++index)
        this.stars[index] = new StarfieldWipe.Star((float) Math.Pow((double) index / (double) this.stars.Length, 5.0));
      for (int index = 0; index < this.verts.Length; ++index)
        this.verts[index].Color = this.WipeIn ? Color.Black : Color.White;
    }

    public override void Update(Scene scene)
    {
      base.Update(scene);
      for (int index = 0; index < this.stars.Length; ++index)
        this.stars[index].Update();
    }

    public override void BeforeRender(Scene scene)
    {
      this.hasDrawn = true;
      Engine.Graphics.GraphicsDevice.SetRenderTarget((RenderTarget2D) Celeste.WipeTarget);
      Engine.Graphics.GraphicsDevice.Clear(this.WipeIn ? Color.White : Color.Black);
      if ((double) this.Percent > 0.800000011920929)
      {
        float height = Calc.Map(this.Percent, 0.8f, 1f) * 1082f;
        Draw.SpriteBatch.Begin();
        Draw.Rect(-1f, (float) ((1080.0 - (double) height) * 0.5), 1922f, height, !this.WipeIn ? Color.White : Color.Black);
        Draw.SpriteBatch.End();
      }
      int index1 = 0;
      for (int index2 = 0; index2 < this.stars.Length; ++index2)
      {
        float xPosition = (float) ((double) this.stars[index2].X % 2920.0 - 500.0);
        float yPosition = this.stars[index2].Y + (float) Math.Sin((double) this.stars[index2].Sine) * this.stars[index2].SineDistance;
        float scale = (float) ((0.10000000149011612 + (double) this.stars[index2].Scale * 0.8999999761581421) * 1080.0 * 0.800000011920929) * Ease.CubeIn(this.Percent);
        this.DrawStar(ref index1, Matrix.CreateRotationZ(this.stars[index2].Rotation) * Matrix.CreateScale(scale) * Matrix.CreateTranslation(xPosition, yPosition, 0.0f));
      }
      GFX.DrawVertices<VertexPositionColor>(Matrix.Identity, this.verts, this.verts.Length);
    }

    private void DrawStar(ref int index, Matrix matrix)
    {
      int num = index;
      for (int index1 = 1; index1 < this.starShape.Length - 1; ++index1)
      {
        this.verts[index++].Position = new Vector3(this.starShape[0], 0.0f);
        this.verts[index++].Position = new Vector3(this.starShape[index1], 0.0f);
        this.verts[index++].Position = new Vector3(this.starShape[index1 + 1], 0.0f);
      }
      for (int index2 = 0; index2 < 5; ++index2)
      {
        Vector2 vector2_1 = this.starShape[index2];
        Vector2 vector2_2 = this.starShape[(index2 + 1) % 5];
        Vector2 vector2_3 = (vector2_1 + vector2_2) * 0.5f + (vector2_1 - vector2_2).SafeNormalize().TurnRight();
        this.verts[index++].Position = new Vector3(vector2_1, 0.0f);
        this.verts[index++].Position = new Vector3(vector2_3, 0.0f);
        this.verts[index++].Position = new Vector3(vector2_2, 0.0f);
      }
      for (int index3 = num; index3 < num + 24; ++index3)
        this.verts[index3].Position = Vector3.Transform(this.verts[index3].Position, matrix);
    }

    public override void Render(Scene scene)
    {
      Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, StarfieldWipe.SubtractBlendmode, SamplerState.LinearClamp, (DepthStencilState) null, (RasterizerState) null, (Effect) null, Engine.ScreenMatrix);
      if (this.WipeIn && (double) this.Percent <= 0.009999999776482582 || !this.WipeIn && (double) this.Percent >= 0.9900000095367432)
        Draw.Rect(-1f, -1f, 1922f, 1082f, Color.White);
      else if (this.hasDrawn)
        Draw.SpriteBatch.Draw((Texture2D) (RenderTarget2D) Celeste.WipeTarget, new Vector2(-1f, -1f), Color.White);
      Draw.SpriteBatch.End();
    }

    private struct Star
    {
      public float X;
      public float Y;
      public float Sine;
      public float SineDistance;
      public float Speed;
      public float Scale;
      public float Rotation;

      public Star(float scale)
      {
        this.Scale = scale;
        float num = 1f - scale;
        this.X = (float) Calc.Random.Range(0, 2920);
        this.Y = (float) (1080.0 * (0.5 + (double) Calc.Random.Choose<int>(-1, 1) * (double) num * (double) Calc.Random.Range(0.25f, 0.5f)));
        this.Sine = Calc.Random.NextFloat(6.2831855f);
        this.SineDistance = (float) ((double) scale * 1080.0 * 0.05000000074505806);
        this.Speed = (float) ((0.5 + (1.0 - (double) this.Scale) * 0.5) * 1920.0 * 0.05000000074505806);
        this.Rotation = Calc.Random.NextFloat(6.2831855f);
      }

      public void Update()
      {
        this.X += this.Speed * Engine.DeltaTime;
        this.Sine += (float) ((1.0 - (double) this.Scale) * 8.0) * Engine.DeltaTime;
        this.Rotation += (1f - this.Scale) * Engine.DeltaTime;
      }
    }
  }
}
