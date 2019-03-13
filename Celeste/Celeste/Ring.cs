// Decompiled with JetBrains decompiler
// Type: Celeste.Ring
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
  public class Ring
  {
    public VertexPositionColorTexture[] Verts = new VertexPositionColorTexture[144];
    public VirtualTexture Texture;
    public Color Color;

    public Ring(float top, float bottom, float distance, Color color, VirtualTexture texture)
    {
      this.Texture = texture;
      this.Color = color;
      for (int index1 = 0; index1 < 24; ++index1)
      {
        float x1 = (float) (index1 - 1) / 24f;
        float x2 = (float) index1 / 24f;
        Vector2 vector1 = Calc.AngleToVector(x1 * 6.283185f, distance);
        Vector2 vector2 = Calc.AngleToVector(x2 * 6.283185f, distance);
        int index2 = index1 * 6;
        this.Verts[index2].Color = color;
        this.Verts[index2].TextureCoordinate = new Vector2(x1, 0.01f);
        this.Verts[index2].Position = new Vector3(vector1.X, top, vector1.Y);
        this.Verts[index2 + 1].Color = color;
        this.Verts[index2 + 1].TextureCoordinate = new Vector2(x2, 0.01f);
        this.Verts[index2 + 1].Position = new Vector3(vector2.X, top, vector2.Y);
        this.Verts[index2 + 2].Color = color;
        this.Verts[index2 + 2].TextureCoordinate = new Vector2(x2, 1f);
        this.Verts[index2 + 2].Position = new Vector3(vector2.X, bottom, vector2.Y);
        this.Verts[index2 + 3].Color = color;
        this.Verts[index2 + 3].TextureCoordinate = new Vector2(x1, 0.01f);
        this.Verts[index2 + 3].Position = new Vector3(vector1.X, top, vector1.Y);
        this.Verts[index2 + 4].Color = color;
        this.Verts[index2 + 4].TextureCoordinate = new Vector2(x2, 1f);
        this.Verts[index2 + 4].Position = new Vector3(vector2.X, bottom, vector2.Y);
        this.Verts[index2 + 5].Color = color;
        this.Verts[index2 + 5].TextureCoordinate = new Vector2(x1, 1f);
        this.Verts[index2 + 5].Position = new Vector3(vector1.X, bottom, vector1.Y);
      }
    }

    public void Rotate(float amount)
    {
      for (int index = 0; index < this.Verts.Length; ++index)
        this.Verts[index].TextureCoordinate.X += amount;
    }

    public void Draw(Matrix matrix, RasterizerState rstate = null)
    {
      Engine.Graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
      Engine.Graphics.GraphicsDevice.RasterizerState = rstate == null ? MountainModel.CullCCRasterizer : rstate;
      Engine.Graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
      Engine.Graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
      Engine.Graphics.GraphicsDevice.Textures[0] = (Microsoft.Xna.Framework.Graphics.Texture) this.Texture.Texture;
      for (int index = 0; index < this.Verts.Length; ++index)
        this.Verts[index].Color = this.Color;
      GFX.FxTexture.Parameters["World"].SetValue(matrix);
      foreach (EffectPass pass in GFX.FxTexture.CurrentTechnique.Passes)
      {
        pass.Apply();
        Engine.Graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList, this.Verts, 0, this.Verts.Length / 3);
      }
    }
  }
}

