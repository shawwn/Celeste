// Decompiled with JetBrains decompiler
// Type: Celeste.Ring
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;

namespace Celeste
{
  public class Ring
  {
    public VertexPositionColorTexture[] Verts;
    public VirtualTexture Texture;
    public Color TopColor;
    public Color BotColor;

    public Ring(
      float top,
      float bottom,
      float distance,
      float wavy,
      int steps,
      Color color,
      VirtualTexture texture,
      float texScale = 1f)
      : this(top, bottom, distance, wavy, steps, color, color, texture, texScale)
    {
    }

    public Ring(
      float top,
      float bottom,
      float distance,
      float wavy,
      int steps,
      Color topColor,
      Color botColor,
      VirtualTexture texture,
      float texScale = 1f)
    {
      this.Texture = texture;
      this.TopColor = topColor;
      this.BotColor = botColor;
      this.Verts = new VertexPositionColorTexture[steps * 24];
      float y1 = (float) ((1.0 - (double) texScale) * 0.5 + 0.009999999776482582);
      float y2 = (float) (1.0 - (1.0 - (double) texScale) * 0.5);
      for (int index1 = 0; index1 < steps; ++index1)
      {
        float num1 = (float) (index1 - 1) / (float) steps;
        float num2 = (float) index1 / (float) steps;
        Vector2 vector1 = Calc.AngleToVector(num1 * 6.2831855f, distance);
        Vector2 vector2 = Calc.AngleToVector(num2 * 6.2831855f, distance);
        float num3 = 0.0f;
        float num4 = 0.0f;
        if ((double) wavy > 0.0)
        {
          num3 = (float) (Math.Sin((double) num1 * 6.2831854820251465 * 3.0 + (double) wavy) * (double) Math.Abs(top - bottom) * 0.4000000059604645);
          num4 = (float) (Math.Sin((double) num2 * 6.2831854820251465 * 3.0 + (double) wavy) * (double) Math.Abs(top - bottom) * 0.4000000059604645);
        }
        int index2 = index1 * 6;
        this.Verts[index2].Color = topColor;
        this.Verts[index2].TextureCoordinate = new Vector2(num1 * texScale, y1);
        this.Verts[index2].Position = new Vector3(vector1.X, top + num3, vector1.Y);
        this.Verts[index2 + 1].Color = topColor;
        this.Verts[index2 + 1].TextureCoordinate = new Vector2(num2 * texScale, y1);
        this.Verts[index2 + 1].Position = new Vector3(vector2.X, top + num4, vector2.Y);
        this.Verts[index2 + 2].Color = botColor;
        this.Verts[index2 + 2].TextureCoordinate = new Vector2(num2 * texScale, y2);
        this.Verts[index2 + 2].Position = new Vector3(vector2.X, bottom + num4, vector2.Y);
        this.Verts[index2 + 3].Color = topColor;
        this.Verts[index2 + 3].TextureCoordinate = new Vector2(num1 * texScale, y1);
        this.Verts[index2 + 3].Position = new Vector3(vector1.X, top + num3, vector1.Y);
        this.Verts[index2 + 4].Color = botColor;
        this.Verts[index2 + 4].TextureCoordinate = new Vector2(num2 * texScale, y2);
        this.Verts[index2 + 4].Position = new Vector3(vector2.X, bottom + num4, vector2.Y);
        this.Verts[index2 + 5].Color = botColor;
        this.Verts[index2 + 5].TextureCoordinate = new Vector2(num1 * texScale, y2);
        this.Verts[index2 + 5].Position = new Vector3(vector1.X, bottom + num3, vector1.Y);
      }
    }

    public void Rotate(float amount)
    {
      for (int index = 0; index < this.Verts.Length; ++index)
        this.Verts[index].TextureCoordinate.X += amount;
    }

    public void Draw(Matrix matrix, RasterizerState rstate = null, float alpha = 1f)
    {
      Engine.Graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
      Engine.Graphics.GraphicsDevice.RasterizerState = rstate == null ? MountainModel.CullCCRasterizer : rstate;
      Engine.Graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
      Engine.Graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
      Engine.Graphics.GraphicsDevice.Textures[0] = (Microsoft.Xna.Framework.Graphics.Texture) this.Texture.Texture;
      Color color1 = this.TopColor * alpha;
      Color color2 = this.BotColor * alpha;
      for (int index = 0; index < this.Verts.Length; index += 6)
      {
        this.Verts[index].Color = color1;
        this.Verts[index + 1].Color = color1;
        this.Verts[index + 2].Color = color2;
        this.Verts[index + 3].Color = color1;
        this.Verts[index + 4].Color = color2;
        this.Verts[index + 5].Color = color2;
      }
      GFX.FxTexture.Parameters["World"].SetValue(matrix);
      foreach (EffectPass pass in GFX.FxTexture.CurrentTechnique.Passes)
      {
        pass.Apply();
        Engine.Graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList, this.Verts, 0, this.Verts.Length / 3);
      }
    }
  }
}
