// Decompiled with JetBrains decompiler
// Type: Celeste.Ring
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System.Collections.Generic;

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
        float num1 = (float) (index1 - 1) / 24f;
        float num2 = (float) index1 / 24f;
        Vector2 vector1 = Calc.AngleToVector(num1 * 6.283185f, distance);
        Vector2 vector2 = Calc.AngleToVector(num2 * 6.283185f, distance);
        int index2 = index1 * 6;
        this.Verts[index2].Color = (__Null) color;
        this.Verts[index2].TextureCoordinate = (__Null) new Vector2(num1, 0.01f);
        this.Verts[index2].Position = (__Null) new Vector3((float) vector1.X, top, (float) vector1.Y);
        this.Verts[index2 + 1].Color = (__Null) color;
        this.Verts[index2 + 1].TextureCoordinate = (__Null) new Vector2(num2, 0.01f);
        this.Verts[index2 + 1].Position = (__Null) new Vector3((float) vector2.X, top, (float) vector2.Y);
        this.Verts[index2 + 2].Color = (__Null) color;
        this.Verts[index2 + 2].TextureCoordinate = (__Null) new Vector2(num2, 1f);
        this.Verts[index2 + 2].Position = (__Null) new Vector3((float) vector2.X, bottom, (float) vector2.Y);
        this.Verts[index2 + 3].Color = (__Null) color;
        this.Verts[index2 + 3].TextureCoordinate = (__Null) new Vector2(num1, 0.01f);
        this.Verts[index2 + 3].Position = (__Null) new Vector3((float) vector1.X, top, (float) vector1.Y);
        this.Verts[index2 + 4].Color = (__Null) color;
        this.Verts[index2 + 4].TextureCoordinate = (__Null) new Vector2(num2, 1f);
        this.Verts[index2 + 4].Position = (__Null) new Vector3((float) vector2.X, bottom, (float) vector2.Y);
        this.Verts[index2 + 5].Color = (__Null) color;
        this.Verts[index2 + 5].TextureCoordinate = (__Null) new Vector2(num1, 1f);
        this.Verts[index2 + 5].Position = (__Null) new Vector3((float) vector1.X, bottom, (float) vector1.Y);
      }
    }

    public void Rotate(float amount)
    {
      for (int index = 0; index < this.Verts.Length; ++index)
      {
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ref __Null local = ref (^(Vector2&) ref this.Verts[index].TextureCoordinate).X;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(float&) ref local = ^(float&) ref local + amount;
      }
    }

    public void Draw(Matrix matrix, RasterizerState rstate = null)
    {
      Engine.Graphics.get_GraphicsDevice().set_DepthStencilState((DepthStencilState) DepthStencilState.Default);
      Engine.Graphics.get_GraphicsDevice().set_RasterizerState(rstate == null ? MountainModel.CullCCRasterizer : rstate);
      Engine.Graphics.get_GraphicsDevice().get_SamplerStates().set_Item(0, (SamplerState) SamplerState.LinearWrap);
      Engine.Graphics.get_GraphicsDevice().set_BlendState((BlendState) BlendState.AlphaBlend);
      Engine.Graphics.get_GraphicsDevice().get_Textures().set_Item(0, (Microsoft.Xna.Framework.Graphics.Texture) this.Texture.Texture);
      for (int index = 0; index < this.Verts.Length; ++index)
        this.Verts[index].Color = (__Null) this.Color;
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
