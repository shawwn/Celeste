// Decompiled with JetBrains decompiler
// Type: Celeste.Skybox
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
  public class Skybox
  {
    public VertexPositionColorTexture[] Verts;
    public VirtualTexture Texture;

    public Skybox(VirtualTexture texture, float size = 25f, bool bottom = false)
    {
      this.Texture = texture;
      this.Verts = new VertexPositionColorTexture[bottom ? 36 : 30];
      Vector3 vector3_1 = new Vector3(-size, size, -size);
      Vector3 vector3_2 = new Vector3(size, size, -size);
      Vector3 vector3_3 = new Vector3(size, size, size);
      Vector3 vector3_4 = new Vector3(-size, size, size);
      Vector3 vector3_5 = new Vector3(-size, -size, -size);
      Vector3 vector3_6 = new Vector3(size, -size, -size);
      Vector3 vector3_7 = new Vector3(size, -size, size);
      Vector3 vector3_8 = new Vector3(-size, -size, size);
      MTexture mtexture = new MTexture(texture);
      MTexture subtexture1 = mtexture.GetSubtexture(1024, 1024, 1024, 1024, (MTexture) null);
      MTexture subtexture2 = mtexture.GetSubtexture(1024, 0, 1024, 1024, (MTexture) null);
      MTexture subtexture3 = mtexture.GetSubtexture(0, 0, 1024, 1024, (MTexture) null);
      MTexture subtexture4 = mtexture.GetSubtexture(2048, 0, 1024, 1024, (MTexture) null);
      MTexture subtexture5 = mtexture.GetSubtexture(2048, 1024, 1024, 1024, (MTexture) null);
      this.AddFace(this.Verts, 0, subtexture1, vector3_1, vector3_2, vector3_3, vector3_4);
      this.AddFace(this.Verts, 1, subtexture2, vector3_2, vector3_1, vector3_5, vector3_6);
      this.AddFace(this.Verts, 2, subtexture5, vector3_4, vector3_3, vector3_7, vector3_8);
      this.AddFace(this.Verts, 3, subtexture4, vector3_3, vector3_2, vector3_6, vector3_7);
      this.AddFace(this.Verts, 4, subtexture3, vector3_1, vector3_4, vector3_8, vector3_5);
      if (!bottom)
        return;
      this.AddFace(this.Verts, 5, mtexture.GetSubtexture(0, 1024, 1024, 1024, (MTexture) null), vector3_8, vector3_7, vector3_6, vector3_5);
    }

    private void AddFace(
      VertexPositionColorTexture[] verts,
      int face,
      MTexture tex,
      Vector3 a,
      Vector3 b,
      Vector3 c,
      Vector3 d)
    {
      float x1 = (float) (tex.ClipRect.Left + 1) / (float) tex.Texture.Width;
      float y1 = (float) (tex.ClipRect.Top + 1) / (float) tex.Texture.Height;
      float x2 = (float) (tex.ClipRect.Right - 1) / (float) tex.Texture.Width;
      float y2 = (float) (tex.ClipRect.Bottom - 1) / (float) tex.Texture.Height;
      int num1 = face * 6;
      VertexPositionColorTexture[] positionColorTextureArray1 = verts;
      int index1 = num1;
      int num2 = index1 + 1;
      VertexPositionColorTexture positionColorTexture1 = new VertexPositionColorTexture(a, Color.White, new Vector2(x1, y1));
      positionColorTextureArray1[index1] = positionColorTexture1;
      VertexPositionColorTexture[] positionColorTextureArray2 = verts;
      int index2 = num2;
      int num3 = index2 + 1;
      VertexPositionColorTexture positionColorTexture2 = new VertexPositionColorTexture(b, Color.White, new Vector2(x2, y1));
      positionColorTextureArray2[index2] = positionColorTexture2;
      VertexPositionColorTexture[] positionColorTextureArray3 = verts;
      int index3 = num3;
      int num4 = index3 + 1;
      VertexPositionColorTexture positionColorTexture3 = new VertexPositionColorTexture(c, Color.White, new Vector2(x2, y2));
      positionColorTextureArray3[index3] = positionColorTexture3;
      VertexPositionColorTexture[] positionColorTextureArray4 = verts;
      int index4 = num4;
      int num5 = index4 + 1;
      VertexPositionColorTexture positionColorTexture4 = new VertexPositionColorTexture(a, Color.White, new Vector2(x1, y1));
      positionColorTextureArray4[index4] = positionColorTexture4;
      VertexPositionColorTexture[] positionColorTextureArray5 = verts;
      int index5 = num5;
      int num6 = index5 + 1;
      VertexPositionColorTexture positionColorTexture5 = new VertexPositionColorTexture(c, Color.White, new Vector2(x2, y2));
      positionColorTextureArray5[index5] = positionColorTexture5;
      VertexPositionColorTexture[] positionColorTextureArray6 = verts;
      int index6 = num6;
      int num7 = index6 + 1;
      VertexPositionColorTexture positionColorTexture6 = new VertexPositionColorTexture(d, Color.White, new Vector2(x1, y2));
      positionColorTextureArray6[index6] = positionColorTexture6;
    }

    public void Draw(Matrix matrix, Color color)
    {
      Engine.Graphics.GraphicsDevice.RasterizerState = MountainModel.CullNoneRasterizer;
      Engine.Graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
      Engine.Graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
      Engine.Graphics.GraphicsDevice.Textures[0] = (Microsoft.Xna.Framework.Graphics.Texture) this.Texture.Texture;
      for (int index = 0; index < this.Verts.Length; ++index)
        this.Verts[index].Color = color;
      GFX.FxTexture.Parameters["World"].SetValue(matrix);
      foreach (EffectPass pass in GFX.FxTexture.CurrentTechnique.Passes)
      {
        pass.Apply();
        Engine.Graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList, this.Verts, 0, this.Verts.Length / 3);
      }
    }
  }
}

