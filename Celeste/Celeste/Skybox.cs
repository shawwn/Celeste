// Decompiled with JetBrains decompiler
// Type: Celeste.Skybox
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System.Collections.Generic;

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
      Vector3 vector3_1;
      ((Vector3) ref vector3_1).\u002Ector(-size, size, -size);
      Vector3 vector3_2;
      ((Vector3) ref vector3_2).\u002Ector(size, size, -size);
      Vector3 vector3_3;
      ((Vector3) ref vector3_3).\u002Ector(size, size, size);
      Vector3 vector3_4;
      ((Vector3) ref vector3_4).\u002Ector(-size, size, size);
      Vector3 vector3_5;
      ((Vector3) ref vector3_5).\u002Ector(-size, -size, -size);
      Vector3 vector3_6;
      ((Vector3) ref vector3_6).\u002Ector(size, -size, -size);
      Vector3 vector3_7;
      ((Vector3) ref vector3_7).\u002Ector(size, -size, size);
      Vector3 vector3_8;
      ((Vector3) ref vector3_8).\u002Ector(-size, -size, size);
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
      Rectangle clipRect1 = tex.ClipRect;
      float num1 = (float) (((Rectangle) ref clipRect1).get_Left() + 1) / (float) tex.Texture.Width;
      Rectangle clipRect2 = tex.ClipRect;
      float num2 = (float) (((Rectangle) ref clipRect2).get_Top() + 1) / (float) tex.Texture.Height;
      Rectangle clipRect3 = tex.ClipRect;
      float num3 = (float) (((Rectangle) ref clipRect3).get_Right() - 1) / (float) tex.Texture.Width;
      Rectangle clipRect4 = tex.ClipRect;
      float num4 = (float) (((Rectangle) ref clipRect4).get_Bottom() - 1) / (float) tex.Texture.Height;
      int num5 = face * 6;
      VertexPositionColorTexture[] positionColorTextureArray1 = verts;
      int index1 = num5;
      int num6 = index1 + 1;
      VertexPositionColorTexture positionColorTexture1 = new VertexPositionColorTexture(a, Color.get_White(), new Vector2(num1, num2));
      positionColorTextureArray1[index1] = positionColorTexture1;
      VertexPositionColorTexture[] positionColorTextureArray2 = verts;
      int index2 = num6;
      int num7 = index2 + 1;
      VertexPositionColorTexture positionColorTexture2 = new VertexPositionColorTexture(b, Color.get_White(), new Vector2(num3, num2));
      positionColorTextureArray2[index2] = positionColorTexture2;
      VertexPositionColorTexture[] positionColorTextureArray3 = verts;
      int index3 = num7;
      int num8 = index3 + 1;
      VertexPositionColorTexture positionColorTexture3 = new VertexPositionColorTexture(c, Color.get_White(), new Vector2(num3, num4));
      positionColorTextureArray3[index3] = positionColorTexture3;
      VertexPositionColorTexture[] positionColorTextureArray4 = verts;
      int index4 = num8;
      int num9 = index4 + 1;
      VertexPositionColorTexture positionColorTexture4 = new VertexPositionColorTexture(a, Color.get_White(), new Vector2(num1, num2));
      positionColorTextureArray4[index4] = positionColorTexture4;
      VertexPositionColorTexture[] positionColorTextureArray5 = verts;
      int index5 = num9;
      int num10 = index5 + 1;
      VertexPositionColorTexture positionColorTexture5 = new VertexPositionColorTexture(c, Color.get_White(), new Vector2(num3, num4));
      positionColorTextureArray5[index5] = positionColorTexture5;
      VertexPositionColorTexture[] positionColorTextureArray6 = verts;
      int index6 = num10;
      int num11 = index6 + 1;
      VertexPositionColorTexture positionColorTexture6 = new VertexPositionColorTexture(d, Color.get_White(), new Vector2(num1, num4));
      positionColorTextureArray6[index6] = positionColorTexture6;
    }

    public void Draw(Matrix matrix, Color color)
    {
      Engine.Graphics.get_GraphicsDevice().set_RasterizerState(MountainModel.CullNoneRasterizer);
      Engine.Graphics.get_GraphicsDevice().get_SamplerStates().set_Item(0, (SamplerState) SamplerState.LinearClamp);
      Engine.Graphics.get_GraphicsDevice().set_BlendState((BlendState) BlendState.AlphaBlend);
      Engine.Graphics.get_GraphicsDevice().get_Textures().set_Item(0, (Microsoft.Xna.Framework.Graphics.Texture) this.Texture.Texture);
      for (int index = 0; index < this.Verts.Length; ++index)
        this.Verts[index].Color = (__Null) color;
      GFX.FxTexture.get_Parameters().get_Item("World").SetValue(matrix);
      using (List<EffectPass>.Enumerator enumerator = GFX.FxTexture.get_CurrentTechnique().get_Passes().GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          enumerator.Current.Apply();
          Engine.Graphics.get_GraphicsDevice().DrawUserPrimitives<VertexPositionColorTexture>((PrimitiveType) 0, (M0[]) this.Verts, 0, this.Verts.Length / 3);
        }
      }
    }
  }
}
