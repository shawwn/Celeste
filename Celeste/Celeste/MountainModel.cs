// Decompiled with JetBrains decompiler
// Type: Celeste.MountainModel
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste
{
  public class MountainModel : IDisposable
  {
    private float easeState = 1f;
    private MountainState[] mountainStates = new MountainState[4];
    public Vector3 CoreWallPosition = Vector3.get_Zero();
    private VertexPositionColorTexture[] billboardInfo = new VertexPositionColorTexture[2048];
    private Texture2D[] billboardTextures = new Texture2D[512];
    public List<VertexPositionColor> DebugPoints = new List<VertexPositionColor>();
    public MountainCamera Camera;
    public Vector3 Forward;
    public float SkyboxOffset;
    public bool LockBufferResizing;
    private VirtualRenderTarget buffer;
    private VirtualRenderTarget blurA;
    private VirtualRenderTarget blurB;
    public static RasterizerState MountainRasterizer;
    public static RasterizerState CullNoneRasterizer;
    public static RasterizerState CullCCRasterizer;
    public static RasterizerState CullCRasterizer;
    private int currState;
    private int nextState;
    private int targetState;
    private VertexBuffer billboardVertices;
    private IndexBuffer billboardIndices;
    private Ring fog;
    private Ring fog2;
    public float NearFogAlpha;
    public bool DrawDebugPoints;

    public MountainModel()
    {
      this.mountainStates[0] = new MountainState(GFX.MountainTerrainTextures[0], GFX.MountainBuildingTextures[0], GFX.MountainSkyboxTextures[0], Calc.HexToColor("010817"));
      this.mountainStates[1] = new MountainState(GFX.MountainTerrainTextures[1], GFX.MountainBuildingTextures[1], GFX.MountainSkyboxTextures[1], Calc.HexToColor("13203E"));
      this.mountainStates[2] = new MountainState(GFX.MountainTerrainTextures[2], GFX.MountainBuildingTextures[2], GFX.MountainSkyboxTextures[2], Calc.HexToColor("281A35"));
      this.mountainStates[3] = new MountainState(GFX.MountainTerrainTextures[0], GFX.MountainBuildingTextures[0], GFX.MountainSkyboxTextures[0], Calc.HexToColor("010817"));
      this.fog = new Ring(6f, -1f, 20f, Color.get_White(), GFX.MountainFogTexture);
      this.fog2 = new Ring(6f, -4f, 10f, Color.get_White(), GFX.MountainFogTexture);
      this.ResetRenderTargets();
      this.ResetBillboardBuffers();
    }

    public void SnapState(int state)
    {
      this.currState = this.nextState = this.targetState = state % this.mountainStates.Length;
      this.easeState = 1f;
    }

    public void EaseState(int state)
    {
      this.targetState = state % this.mountainStates.Length;
    }

    public void Update()
    {
      if (this.currState != this.nextState)
      {
        this.easeState = Calc.Approach(this.easeState, 1f, (this.nextState == this.targetState ? 1f : 4f) * Engine.DeltaTime);
        if ((double) this.easeState >= 1.0)
          this.currState = this.nextState;
      }
      else if (this.nextState != this.targetState)
      {
        this.nextState = this.targetState;
        this.easeState = 0.0f;
      }
      this.fog.Rotate((float) (-(double) Engine.DeltaTime * 0.00999999977648258));
      this.fog.Color = Color.Lerp(this.mountainStates[this.currState].FogColor, this.mountainStates[this.nextState].FogColor, this.easeState);
      this.fog2.Rotate((float) (-(double) Engine.DeltaTime * 0.00999999977648258));
      this.fog2.Color = Color.op_Multiply(Color.op_Multiply(Color.get_White(), 0.3f), this.NearFogAlpha);
    }

    public void ResetRenderTargets()
    {
      int width = Math.Min(1920, Engine.ViewWidth);
      int height = Math.Min(1080, Engine.ViewHeight);
      if (this.buffer != null && !this.buffer.IsDisposed && (this.buffer.Width == width || this.LockBufferResizing))
        return;
      this.DisposeTargets();
      this.buffer = VirtualContent.CreateRenderTarget("mountain-a", width, height, true, false, 0);
      this.blurA = VirtualContent.CreateRenderTarget("mountain-blur-a", width / 2, height / 2, false, true, 0);
      this.blurB = VirtualContent.CreateRenderTarget("mountain-blur-b", width / 2, height / 2, false, true, 0);
    }

    public void ResetBillboardBuffers()
    {
      if (this.billboardVertices != null && !((GraphicsResource) this.billboardIndices).get_IsDisposed() && (!((GraphicsResource) this.billboardIndices).get_GraphicsDevice().get_IsDisposed() && !((GraphicsResource) this.billboardVertices).get_IsDisposed()) && (!((GraphicsResource) this.billboardVertices).get_GraphicsDevice().get_IsDisposed() && this.billboardInfo.Length <= this.billboardVertices.get_VertexCount()))
        return;
      this.DisposeBillboardBuffers();
      this.billboardVertices = new VertexBuffer(Engine.Graphics.get_GraphicsDevice(), typeof (VertexPositionColorTexture), this.billboardInfo.Length, (BufferUsage) 0);
      this.billboardIndices = new IndexBuffer(Engine.Graphics.get_GraphicsDevice(), typeof (short), this.billboardInfo.Length / 4 * 6, (BufferUsage) 0);
      short[] numArray = new short[this.billboardIndices.get_IndexCount()];
      int index = 0;
      int num = 0;
      while (index < numArray.Length)
      {
        numArray[index] = (short) num;
        numArray[index + 1] = (short) (num + 1);
        numArray[index + 2] = (short) (num + 2);
        numArray[index + 3] = (short) num;
        numArray[index + 4] = (short) (num + 2);
        numArray[index + 5] = (short) (num + 3);
        index += 6;
        num += 4;
      }
      this.billboardIndices.SetData<short>((M0[]) numArray);
    }

    public void Dispose()
    {
      this.DisposeTargets();
      this.DisposeBillboardBuffers();
    }

    public void DisposeTargets()
    {
      if (this.buffer == null || this.buffer.IsDisposed)
        return;
      this.buffer.Dispose();
      this.blurA.Dispose();
      this.blurB.Dispose();
    }

    public void DisposeBillboardBuffers()
    {
      if (this.billboardVertices != null && !((GraphicsResource) this.billboardVertices).get_IsDisposed())
        ((GraphicsResource) this.billboardVertices).Dispose();
      if (this.billboardIndices == null || ((GraphicsResource) this.billboardIndices).get_IsDisposed())
        return;
      ((GraphicsResource) this.billboardIndices).Dispose();
    }

    public void BeforeRender(Scene scene)
    {
      this.ResetRenderTargets();
      Quaternion rotation = this.Camera.Rotation;
      Matrix perspectiveFieldOfView = Matrix.CreatePerspectiveFieldOfView(0.7853982f, (float) Engine.Width / (float) Engine.Height, 0.25f, 50f);
      Matrix matrix1 = Matrix.op_Multiply(Matrix.CreateTranslation(Vector3.op_UnaryNegation(this.Camera.Position)), Matrix.CreateFromQuaternion(rotation));
      Matrix matrix2 = Matrix.op_Multiply(matrix1, perspectiveFieldOfView);
      this.Forward = Vector3.Transform(Vector3.get_Forward(), this.Camera.Rotation.Conjugated());
      Engine.Graphics.get_GraphicsDevice().SetRenderTarget((RenderTarget2D) this.buffer);
      Matrix matrix3 = Matrix.op_Multiply(Matrix.op_Multiply(Matrix.CreateTranslation(0.0f, (float) (5.0 - this.Camera.Position.Y * 1.10000002384186), 0.0f), Matrix.CreateFromQuaternion(rotation)), perspectiveFieldOfView);
      if (this.currState == this.nextState)
      {
        this.mountainStates[this.currState].Skybox.Draw(matrix3, Color.get_White());
      }
      else
      {
        this.mountainStates[this.currState].Skybox.Draw(matrix3, Color.get_White());
        this.mountainStates[this.nextState].Skybox.Draw(matrix3, Color.op_Multiply(Color.get_White(), this.easeState));
      }
      if (this.currState != this.nextState)
      {
        GFX.FxMountain.get_Parameters().get_Item("ease").SetValue(this.easeState);
        GFX.FxMountain.set_CurrentTechnique(GFX.FxMountain.get_Techniques().get_Item("Easing"));
      }
      else
        GFX.FxMountain.set_CurrentTechnique(GFX.FxMountain.get_Techniques().get_Item("Single"));
      Engine.Graphics.get_GraphicsDevice().set_DepthStencilState((DepthStencilState) DepthStencilState.Default);
      Engine.Graphics.get_GraphicsDevice().set_BlendState((BlendState) BlendState.AlphaBlend);
      Engine.Graphics.get_GraphicsDevice().set_RasterizerState(MountainModel.MountainRasterizer);
      GFX.FxMountain.get_Parameters().get_Item("WorldViewProj").SetValue(matrix2);
      GFX.FxMountain.get_Parameters().get_Item("fog").SetValue(((Color) ref this.fog.Color).ToVector3());
      Engine.Graphics.get_GraphicsDevice().get_Textures().set_Item(0, (Texture) this.mountainStates[this.currState].TerrainTexture.Texture);
      Engine.Graphics.get_GraphicsDevice().get_SamplerStates().set_Item(0, (SamplerState) SamplerState.LinearClamp);
      if (this.currState != this.nextState)
      {
        Engine.Graphics.get_GraphicsDevice().get_Textures().set_Item(1, (Texture) this.mountainStates[this.nextState].TerrainTexture.Texture);
        Engine.Graphics.get_GraphicsDevice().get_SamplerStates().set_Item(1, (SamplerState) SamplerState.LinearClamp);
      }
      GFX.MountainTerrain.Draw(GFX.FxMountain);
      GFX.FxMountain.get_Parameters().get_Item("WorldViewProj").SetValue(Matrix.op_Multiply(Matrix.CreateTranslation(this.CoreWallPosition), matrix2));
      GFX.MountainCoreWall.Draw(GFX.FxMountain);
      GFX.FxMountain.get_Parameters().get_Item("WorldViewProj").SetValue(matrix2);
      Engine.Graphics.get_GraphicsDevice().get_Textures().set_Item(0, (Texture) this.mountainStates[this.currState].BuildingsTexture.Texture);
      Engine.Graphics.get_GraphicsDevice().get_SamplerStates().set_Item(0, (SamplerState) SamplerState.LinearClamp);
      if (this.currState != this.nextState)
      {
        Engine.Graphics.get_GraphicsDevice().get_Textures().set_Item(1, (Texture) this.mountainStates[this.nextState].BuildingsTexture.Texture);
        Engine.Graphics.get_GraphicsDevice().get_SamplerStates().set_Item(1, (SamplerState) SamplerState.LinearClamp);
      }
      GFX.MountainBuildings.Draw(GFX.FxMountain);
      this.fog.Draw(matrix2, (RasterizerState) null);
      this.DrawBillboards(matrix2, scene.Tracker.GetComponents<Billboard>());
      this.fog2.Draw(matrix2, MountainModel.CullCRasterizer);
      if (this.DrawDebugPoints && this.DebugPoints.Count > 0)
      {
        GFX.FxDebug.set_World(Matrix.get_Identity());
        GFX.FxDebug.set_View(matrix1);
        GFX.FxDebug.set_Projection(perspectiveFieldOfView);
        GFX.FxDebug.set_TextureEnabled(false);
        GFX.FxDebug.set_VertexColorEnabled(true);
        VertexPositionColor[] array = this.DebugPoints.ToArray();
        using (List<EffectPass>.Enumerator enumerator = ((Effect) GFX.FxDebug).get_CurrentTechnique().get_Passes().GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            enumerator.Current.Apply();
            Engine.Graphics.get_GraphicsDevice().DrawUserPrimitives<VertexPositionColor>((PrimitiveType) 0, (M0[]) array, 0, array.Length / 3);
          }
        }
      }
      GaussianBlur.Blur((Texture2D) (RenderTarget2D) this.buffer, this.blurA, this.blurB, 0.75f, true, GaussianBlur.Samples.Five, 1f, GaussianBlur.Direction.Both, 1f);
    }

    private void DrawBillboards(Matrix matrix, List<Component> billboards)
    {
      int val1 = 0;
      int num1 = this.billboardInfo.Length / 4;
      Vector3 vector3_1 = Vector3.Transform(Vector3.get_Left(), this.Camera.Rotation.LookAt(Vector3.get_Zero(), this.Forward, Vector3.get_Up()).Conjugated());
      Vector3 vector3_2 = Vector3.Transform(Vector3.get_Up(), this.Camera.Rotation.LookAt(Vector3.get_Zero(), this.Forward, Vector3.get_Up()).Conjugated());
      foreach (Billboard billboard in billboards)
      {
        if (billboard.Entity.Visible && billboard.Visible)
        {
          if (billboard.BeforeRender != null)
            billboard.BeforeRender();
          if (((Color) ref billboard.Color).get_A() >= (byte) 0 && billboard.Size.X != 0.0 && (billboard.Size.Y != 0.0 && billboard.Scale.X != 0.0) && (billboard.Scale.Y != 0.0 && billboard.Texture != null))
          {
            if (val1 < num1)
            {
              Vector3 position = billboard.Position;
              Vector3 vector3_3 = Vector3.op_Multiply(Vector3.op_Multiply(vector3_1, (float) billboard.Size.X), (float) billboard.Scale.X);
              Vector3 vector3_4 = Vector3.op_Multiply(Vector3.op_Multiply(vector3_2, (float) billboard.Size.Y), (float) billboard.Scale.Y);
              Vector3 vector3_5 = Vector3.op_UnaryNegation(vector3_3);
              Vector3 vector3_6 = Vector3.op_UnaryNegation(vector3_4);
              int index1 = val1 * 4;
              int index2 = val1 * 4 + 1;
              int index3 = val1 * 4 + 2;
              int index4 = val1 * 4 + 3;
              this.billboardInfo[index1].Color = (__Null) billboard.Color;
              // ISSUE: cast to a reference type
              // ISSUE: explicit reference operation
              (^(Vector2&) ref this.billboardInfo[index1].TextureCoordinate).X = (__Null) (double) billboard.Texture.LeftUV;
              // ISSUE: cast to a reference type
              // ISSUE: explicit reference operation
              (^(Vector2&) ref this.billboardInfo[index1].TextureCoordinate).Y = (__Null) (double) billboard.Texture.BottomUV;
              this.billboardInfo[index1].Position = (__Null) Vector3.op_Addition(Vector3.op_Addition(position, vector3_3), vector3_6);
              this.billboardInfo[index2].Color = (__Null) billboard.Color;
              // ISSUE: cast to a reference type
              // ISSUE: explicit reference operation
              (^(Vector2&) ref this.billboardInfo[index2].TextureCoordinate).X = (__Null) (double) billboard.Texture.LeftUV;
              // ISSUE: cast to a reference type
              // ISSUE: explicit reference operation
              (^(Vector2&) ref this.billboardInfo[index2].TextureCoordinate).Y = (__Null) (double) billboard.Texture.TopUV;
              this.billboardInfo[index2].Position = (__Null) Vector3.op_Addition(Vector3.op_Addition(position, vector3_3), vector3_4);
              this.billboardInfo[index3].Color = (__Null) billboard.Color;
              // ISSUE: cast to a reference type
              // ISSUE: explicit reference operation
              (^(Vector2&) ref this.billboardInfo[index3].TextureCoordinate).X = (__Null) (double) billboard.Texture.RightUV;
              // ISSUE: cast to a reference type
              // ISSUE: explicit reference operation
              (^(Vector2&) ref this.billboardInfo[index3].TextureCoordinate).Y = (__Null) (double) billboard.Texture.TopUV;
              this.billboardInfo[index3].Position = (__Null) Vector3.op_Addition(Vector3.op_Addition(position, vector3_5), vector3_4);
              this.billboardInfo[index4].Color = (__Null) billboard.Color;
              // ISSUE: cast to a reference type
              // ISSUE: explicit reference operation
              (^(Vector2&) ref this.billboardInfo[index4].TextureCoordinate).X = (__Null) (double) billboard.Texture.RightUV;
              // ISSUE: cast to a reference type
              // ISSUE: explicit reference operation
              (^(Vector2&) ref this.billboardInfo[index4].TextureCoordinate).Y = (__Null) (double) billboard.Texture.BottomUV;
              this.billboardInfo[index4].Position = (__Null) Vector3.op_Addition(Vector3.op_Addition(position, vector3_5), vector3_6);
              this.billboardTextures[val1] = billboard.Texture.Texture.Texture;
            }
            ++val1;
          }
        }
      }
      this.ResetBillboardBuffers();
      if (val1 <= 0)
        return;
      this.billboardVertices.SetData<VertexPositionColorTexture>((M0[]) this.billboardInfo);
      Engine.Graphics.get_GraphicsDevice().SetVertexBuffer(this.billboardVertices);
      Engine.Graphics.get_GraphicsDevice().set_Indices(this.billboardIndices);
      Engine.Graphics.get_GraphicsDevice().set_RasterizerState(MountainModel.CullNoneRasterizer);
      Engine.Graphics.get_GraphicsDevice().get_SamplerStates().set_Item(0, (SamplerState) SamplerState.LinearClamp);
      Engine.Graphics.get_GraphicsDevice().set_BlendState((BlendState) BlendState.AlphaBlend);
      Engine.Graphics.get_GraphicsDevice().set_DepthStencilState((DepthStencilState) DepthStencilState.DepthRead);
      GFX.FxTexture.get_Parameters().get_Item("World").SetValue(matrix);
      int num2 = Math.Min(val1, this.billboardInfo.Length / 4);
      Texture2D billboardTexture = this.billboardTextures[0];
      int offset = 0;
      for (int index = 1; index < num2; ++index)
      {
        if (this.billboardTextures[index] != billboardTexture)
        {
          this.DrawBillboardBatch(billboardTexture, offset, index - offset);
          billboardTexture = this.billboardTextures[index];
          offset = index;
        }
      }
      this.DrawBillboardBatch(billboardTexture, offset, num2 - offset);
      if (val1 * 4 <= this.billboardInfo.Length)
        return;
      this.billboardInfo = new VertexPositionColorTexture[this.billboardInfo.Length * 2];
      this.billboardTextures = new Texture2D[this.billboardInfo.Length / 4];
    }

    private void DrawBillboardBatch(Texture2D texture, int offset, int sprites)
    {
      Engine.Graphics.get_GraphicsDevice().get_Textures().set_Item(0, (Texture) texture);
      using (List<EffectPass>.Enumerator enumerator = GFX.FxTexture.get_CurrentTechnique().get_Passes().GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          enumerator.Current.Apply();
          Engine.Graphics.get_GraphicsDevice().DrawIndexedPrimitives((PrimitiveType) 0, offset * 4, 0, sprites * 4, 0, sprites * 2);
        }
      }
    }

    public void Render()
    {
      float num = (float) Engine.ViewWidth / (float) this.buffer.Width;
      Draw.SpriteBatch.Begin((SpriteSortMode) 0, (BlendState) BlendState.AlphaBlend, (SamplerState) SamplerState.LinearClamp, (DepthStencilState) null, (RasterizerState) null);
      Draw.SpriteBatch.Draw((Texture2D) (RenderTarget2D) this.buffer, Vector2.get_Zero(), new Rectangle?(this.buffer.Bounds), Color.op_Multiply(Color.get_White(), 1f), 0.0f, Vector2.get_Zero(), num, (SpriteEffects) 0, 0.0f);
      Draw.SpriteBatch.Draw((Texture2D) (RenderTarget2D) this.blurB, Vector2.get_Zero(), new Rectangle?(this.blurB.Bounds), Color.get_White(), 0.0f, Vector2.get_Zero(), num * 2f, (SpriteEffects) 0, 0.0f);
      Draw.SpriteBatch.End();
    }

    static MountainModel()
    {
      RasterizerState rasterizerState1 = new RasterizerState();
      rasterizerState1.set_CullMode((CullMode) 1);
      rasterizerState1.set_MultiSampleAntiAlias(true);
      MountainModel.MountainRasterizer = rasterizerState1;
      RasterizerState rasterizerState2 = new RasterizerState();
      rasterizerState2.set_CullMode((CullMode) 0);
      rasterizerState2.set_MultiSampleAntiAlias(false);
      MountainModel.CullNoneRasterizer = rasterizerState2;
      RasterizerState rasterizerState3 = new RasterizerState();
      rasterizerState3.set_CullMode((CullMode) 2);
      rasterizerState3.set_MultiSampleAntiAlias(false);
      MountainModel.CullCCRasterizer = rasterizerState3;
      RasterizerState rasterizerState4 = new RasterizerState();
      rasterizerState4.set_CullMode((CullMode) 1);
      rasterizerState4.set_MultiSampleAntiAlias(false);
      MountainModel.CullCRasterizer = rasterizerState4;
    }
  }
}
