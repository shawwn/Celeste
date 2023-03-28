// Decompiled with JetBrains decompiler
// Type: Celeste.MountainModel
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste
{
  public class MountainModel : IDisposable
  {
    public MountainCamera Camera;
    public Vector3 Forward;
    public float SkyboxOffset;
    public bool LockBufferResizing;
    private VirtualRenderTarget buffer;
    private VirtualRenderTarget blurA;
    private VirtualRenderTarget blurB;
    public static RasterizerState MountainRasterizer = new RasterizerState()
    {
      CullMode = CullMode.CullClockwiseFace,
      MultiSampleAntiAlias = true
    };
    public static RasterizerState CullNoneRasterizer = new RasterizerState()
    {
      CullMode = CullMode.None,
      MultiSampleAntiAlias = false
    };
    public static RasterizerState CullCCRasterizer = new RasterizerState()
    {
      CullMode = CullMode.CullCounterClockwiseFace,
      MultiSampleAntiAlias = false
    };
    public static RasterizerState CullCRasterizer = new RasterizerState()
    {
      CullMode = CullMode.CullClockwiseFace,
      MultiSampleAntiAlias = false
    };
    private int currState;
    private int nextState;
    private int targetState;
    private float easeState = 1f;
    private MountainState[] mountainStates = new MountainState[4];
    public Vector3 CoreWallPosition = Vector3.Zero;
    private VertexBuffer billboardVertices;
    private IndexBuffer billboardIndices;
    private VertexPositionColorTexture[] billboardInfo = new VertexPositionColorTexture[2048];
    private Texture2D[] billboardTextures = new Texture2D[512];
    private Ring fog;
    private Ring fog2;
    public float NearFogAlpha;
    public float StarEase;
    public float SnowStretch;
    public float SnowSpeedAddition = 1f;
    public float SnowForceFloat;
    private Ring starsky;
    private Ring starfog;
    private Ring stardots0;
    private Ring starstream0;
    private Ring starstream1;
    private Ring starstream2;
    private bool ignoreCameraRotation;
    private Quaternion lastCameraRotation;
    private Vector3 starCenter = new Vector3(0.0f, 32f, 0.0f);
    private float birdTimer;
    public List<VertexPositionColor> DebugPoints = new List<VertexPositionColor>();
    public bool DrawDebugPoints;

    public MountainModel()
    {
      this.mountainStates[0] = new MountainState(MTN.MountainTerrainTextures[0], MTN.MountainBuildingTextures[0], MTN.MountainSkyboxTextures[0], Calc.HexToColor("010817"));
      this.mountainStates[1] = new MountainState(MTN.MountainTerrainTextures[1], MTN.MountainBuildingTextures[1], MTN.MountainSkyboxTextures[1], Calc.HexToColor("13203E"));
      this.mountainStates[2] = new MountainState(MTN.MountainTerrainTextures[2], MTN.MountainBuildingTextures[2], MTN.MountainSkyboxTextures[2], Calc.HexToColor("281A35"));
      this.mountainStates[3] = new MountainState(MTN.MountainTerrainTextures[0], MTN.MountainBuildingTextures[0], MTN.MountainSkyboxTextures[0], Calc.HexToColor("010817"));
      this.fog = new Ring(6f, -1f, 20f, 0.0f, 24, Color.White, MTN.MountainFogTexture);
      this.fog2 = new Ring(6f, -4f, 10f, 0.0f, 24, Color.White, MTN.MountainFogTexture);
      this.starsky = new Ring(18f, -18f, 20f, 0.0f, 24, Color.White, Color.Transparent, MTN.MountainStarSky);
      this.starfog = new Ring(10f, -18f, 19.5f, 0.0f, 24, Calc.HexToColor("020915"), Color.Transparent, MTN.MountainFogTexture);
      this.stardots0 = new Ring(16f, -18f, 19f, 0.0f, 24, Color.White, Color.Transparent, MTN.MountainStars, 4f);
      this.starstream0 = new Ring(5f, -8f, 18.5f, 0.2f, 80, Color.Black, MTN.MountainStarStream);
      this.starstream1 = new Ring(4f, -6f, 18f, 1f, 80, Calc.HexToColor("9228e2") * 0.5f, MTN.MountainStarStream);
      this.starstream2 = new Ring(3f, -4f, 17.9f, 1.4f, 80, Calc.HexToColor("30ffff") * 0.5f, MTN.MountainStarStream);
      this.ResetRenderTargets();
      this.ResetBillboardBuffers();
    }

    public void SnapState(int state)
    {
      this.currState = this.nextState = this.targetState = state % this.mountainStates.Length;
      this.easeState = 1f;
      if (state != 3)
        return;
      this.StarEase = 1f;
    }

    public void EaseState(int state)
    {
      this.targetState = state % this.mountainStates.Length;
      this.lastCameraRotation = this.Camera.Rotation;
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
      this.StarEase = Calc.Approach(this.StarEase, this.nextState == 3 ? 1f : 0.0f, (this.nextState == 3 ? 1.5f : 1f) * Engine.DeltaTime);
      this.SnowForceFloat = Calc.ClampedMap(this.StarEase, 0.95f, 1f);
      this.ignoreCameraRotation = this.nextState == 3 && this.currState != 3 && (double) this.StarEase < 0.5 || this.nextState != 3 && this.currState == 3 && (double) this.StarEase > 0.5;
      if (this.nextState == 3)
      {
        this.SnowStretch = Calc.ClampedMap(this.StarEase, 0.0f, 0.25f) * 50f;
        this.SnowSpeedAddition = this.SnowStretch * 4f;
      }
      else
      {
        this.SnowStretch = Calc.ClampedMap(this.StarEase, 0.25f, 1f) * 50f;
        this.SnowSpeedAddition = (float) (-(double) this.SnowStretch * 4.0);
      }
      this.starfog.Rotate((float) (-(double) Engine.DeltaTime * 0.009999999776482582));
      this.fog.Rotate((float) (-(double) Engine.DeltaTime * 0.009999999776482582));
      this.fog.TopColor = this.fog.BotColor = Color.Lerp(this.mountainStates[this.currState].FogColor, this.mountainStates[this.nextState].FogColor, this.easeState);
      this.fog2.Rotate((float) (-(double) Engine.DeltaTime * 0.009999999776482582));
      this.fog2.TopColor = this.fog2.BotColor = Color.White * 0.3f * this.NearFogAlpha;
      this.starstream1.Rotate(Engine.DeltaTime * 0.01f);
      this.starstream2.Rotate(Engine.DeltaTime * 0.02f);
      this.birdTimer += Engine.DeltaTime;
    }

    public void ResetRenderTargets()
    {
      int width = Math.Min(1920, Engine.ViewWidth);
      int height = Math.Min(1080, Engine.ViewHeight);
      if (this.buffer != null && !this.buffer.IsDisposed && (this.buffer.Width == width || this.LockBufferResizing))
        return;
      this.DisposeTargets();
      this.buffer = VirtualContent.CreateRenderTarget("mountain-a", width, height, true, false);
      this.blurA = VirtualContent.CreateRenderTarget("mountain-blur-a", width / 2, height / 2);
      this.blurB = VirtualContent.CreateRenderTarget("mountain-blur-b", width / 2, height / 2);
    }

    public void ResetBillboardBuffers()
    {
      if (this.billboardVertices != null && !this.billboardIndices.IsDisposed && !this.billboardIndices.GraphicsDevice.IsDisposed && !this.billboardVertices.IsDisposed && !this.billboardVertices.GraphicsDevice.IsDisposed && this.billboardInfo.Length <= this.billboardVertices.VertexCount)
        return;
      this.DisposeBillboardBuffers();
      this.billboardVertices = new VertexBuffer(Engine.Graphics.GraphicsDevice, typeof (VertexPositionColorTexture), this.billboardInfo.Length, BufferUsage.None);
      this.billboardIndices = new IndexBuffer(Engine.Graphics.GraphicsDevice, typeof (short), this.billboardInfo.Length / 4 * 6, BufferUsage.None);
      short[] data = new short[this.billboardIndices.IndexCount];
      int index = 0;
      int num = 0;
      while (index < data.Length)
      {
        data[index] = (short) num;
        data[index + 1] = (short) (num + 1);
        data[index + 2] = (short) (num + 2);
        data[index + 3] = (short) num;
        data[index + 4] = (short) (num + 2);
        data[index + 5] = (short) (num + 3);
        index += 6;
        num += 4;
      }
      this.billboardIndices.SetData<short>(data);
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
      if (this.billboardVertices != null && !this.billboardVertices.IsDisposed)
        this.billboardVertices.Dispose();
      if (this.billboardIndices == null || this.billboardIndices.IsDisposed)
        return;
      this.billboardIndices.Dispose();
    }

    public void BeforeRender(Scene scene)
    {
      this.ResetRenderTargets();
      Quaternion quaternion = this.Camera.Rotation;
      if (this.ignoreCameraRotation)
        quaternion = this.lastCameraRotation;
      Matrix perspectiveFieldOfView = Matrix.CreatePerspectiveFieldOfView(0.7853982f, (float) Engine.Width / (float) Engine.Height, 0.25f, 50f);
      Matrix matrix1 = Matrix.CreateTranslation(-this.Camera.Position) * Matrix.CreateFromQuaternion(quaternion);
      Matrix matrix2 = matrix1 * perspectiveFieldOfView;
      this.Forward = Vector3.Transform(Vector3.Forward, this.Camera.Rotation.Conjugated());
      Engine.Graphics.GraphicsDevice.SetRenderTarget((RenderTarget2D) this.buffer);
      if ((double) this.StarEase < 1.0)
      {
        Matrix matrix3 = Matrix.CreateTranslation(0.0f, (float) (5.0 - (double) this.Camera.Position.Y * 1.100000023841858), 0.0f) * Matrix.CreateFromQuaternion(quaternion) * perspectiveFieldOfView;
        if (this.currState == this.nextState)
        {
          this.mountainStates[this.currState].Skybox.Draw(matrix3, Color.White);
        }
        else
        {
          this.mountainStates[this.currState].Skybox.Draw(matrix3, Color.White);
          this.mountainStates[this.nextState].Skybox.Draw(matrix3, Color.White * this.easeState);
        }
        if (this.currState != this.nextState)
        {
          GFX.FxMountain.Parameters["ease"].SetValue(this.easeState);
          GFX.FxMountain.CurrentTechnique = GFX.FxMountain.Techniques["Easing"];
        }
        else
          GFX.FxMountain.CurrentTechnique = GFX.FxMountain.Techniques["Single"];
        Engine.Graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        Engine.Graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
        Engine.Graphics.GraphicsDevice.RasterizerState = MountainModel.MountainRasterizer;
        GFX.FxMountain.Parameters["WorldViewProj"].SetValue(matrix2);
        GFX.FxMountain.Parameters["fog"].SetValue(this.fog.TopColor.ToVector3());
        Engine.Graphics.GraphicsDevice.Textures[0] = (Texture) this.mountainStates[this.currState].TerrainTexture.Texture;
        Engine.Graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
        if (this.currState != this.nextState)
        {
          Engine.Graphics.GraphicsDevice.Textures[1] = (Texture) this.mountainStates[this.nextState].TerrainTexture.Texture;
          Engine.Graphics.GraphicsDevice.SamplerStates[1] = SamplerState.LinearClamp;
        }
        MTN.MountainTerrain.Draw(GFX.FxMountain);
        GFX.FxMountain.Parameters["WorldViewProj"].SetValue(Matrix.CreateTranslation(this.CoreWallPosition) * matrix2);
        MTN.MountainCoreWall.Draw(GFX.FxMountain);
        GFX.FxMountain.Parameters["WorldViewProj"].SetValue(matrix2);
        Engine.Graphics.GraphicsDevice.Textures[0] = (Texture) this.mountainStates[this.currState].BuildingsTexture.Texture;
        Engine.Graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
        if (this.currState != this.nextState)
        {
          Engine.Graphics.GraphicsDevice.Textures[1] = (Texture) this.mountainStates[this.nextState].BuildingsTexture.Texture;
          Engine.Graphics.GraphicsDevice.SamplerStates[1] = SamplerState.LinearClamp;
        }
        MTN.MountainBuildings.Draw(GFX.FxMountain);
        this.fog.Draw(matrix2);
      }
      if ((double) this.StarEase > 0.0)
      {
        Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, (DepthStencilState) null, (RasterizerState) null);
        Draw.Rect(0.0f, 0.0f, (float) this.buffer.Width, (float) this.buffer.Height, Color.Black * Ease.CubeInOut(Calc.ClampedMap(this.StarEase, 0.0f, 0.6f)));
        Draw.SpriteBatch.End();
        Matrix matrix4 = Matrix.CreateTranslation(this.starCenter - this.Camera.Position) * Matrix.CreateFromQuaternion(quaternion) * perspectiveFieldOfView;
        float alpha = Calc.ClampedMap(this.StarEase, 0.8f, 1f);
        this.starsky.Draw(matrix4, MountainModel.CullCCRasterizer, alpha);
        this.starfog.Draw(matrix4, MountainModel.CullCCRasterizer, alpha);
        this.stardots0.Draw(matrix4, MountainModel.CullCCRasterizer, alpha);
        this.starstream0.Draw(matrix4, MountainModel.CullCCRasterizer, alpha);
        this.starstream1.Draw(matrix4, MountainModel.CullCCRasterizer, alpha);
        this.starstream2.Draw(matrix4, MountainModel.CullCCRasterizer, alpha);
        Engine.Graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        Engine.Graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
        Engine.Graphics.GraphicsDevice.RasterizerState = MountainModel.CullCRasterizer;
        Engine.Graphics.GraphicsDevice.Textures[0] = (Texture) MTN.MountainMoonTexture.Texture;
        Engine.Graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
        GFX.FxMountain.CurrentTechnique = GFX.FxMountain.Techniques["Single"];
        GFX.FxMountain.Parameters["WorldViewProj"].SetValue(matrix2);
        GFX.FxMountain.Parameters["fog"].SetValue(this.fog.TopColor.ToVector3());
        MTN.MountainMoon.Draw(GFX.FxMountain);
        float num = this.birdTimer * 0.2f;
        Matrix matrix5 = Matrix.CreateScale(0.25f) * Matrix.CreateRotationZ((float) Math.Cos((double) num * 2.0) * 0.5f) * Matrix.CreateRotationX((float) (0.4000000059604645 + Math.Sin((double) num) * 0.05000000074505806)) * Matrix.CreateRotationY((float) (-(double) num - 1.5707963705062866)) * Matrix.CreateTranslation((float) Math.Cos((double) num) * 2.2f, (float) (31.0 + Math.Sin((double) num * 2.0) * 0.800000011920929), (float) Math.Sin((double) num) * 2.2f);
        GFX.FxMountain.Parameters["WorldViewProj"].SetValue(matrix5 * matrix2);
        GFX.FxMountain.Parameters["fog"].SetValue(this.fog.TopColor.ToVector3());
        MTN.MountainBird.Draw(GFX.FxMountain);
      }
      this.DrawBillboards(matrix2, scene.Tracker.GetComponents<Billboard>());
      if ((double) this.StarEase < 1.0)
        this.fog2.Draw(matrix2, MountainModel.CullCRasterizer);
      if (this.DrawDebugPoints && this.DebugPoints.Count > 0)
      {
        GFX.FxDebug.World = Matrix.Identity;
        GFX.FxDebug.View = matrix1;
        GFX.FxDebug.Projection = perspectiveFieldOfView;
        GFX.FxDebug.TextureEnabled = false;
        GFX.FxDebug.VertexColorEnabled = true;
        VertexPositionColor[] array = this.DebugPoints.ToArray();
        foreach (EffectPass pass in GFX.FxDebug.CurrentTechnique.Passes)
        {
          pass.Apply();
          Engine.Graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, array, 0, array.Length / 3);
        }
      }
      GaussianBlur.Blur((Texture2D) (RenderTarget2D) this.buffer, this.blurA, this.blurB, 0.75f, samples: GaussianBlur.Samples.Five);
    }

    private void DrawBillboards(Matrix matrix, List<Component> billboards)
    {
      int val1 = 0;
      int num1 = this.billboardInfo.Length / 4;
      Vector3 vector3_1 = Vector3.Transform(Vector3.Left, this.Camera.Rotation.LookAt(Vector3.Zero, this.Forward, Vector3.Up).Conjugated());
      Vector3 vector3_2 = Vector3.Transform(Vector3.Up, this.Camera.Rotation.LookAt(Vector3.Zero, this.Forward, Vector3.Up).Conjugated());
      foreach (Billboard billboard in billboards)
      {
        if (billboard.Entity.Visible && billboard.Visible)
        {
          if (billboard.BeforeRender != null)
            billboard.BeforeRender();
          if (billboard.Color.A >= (byte) 0 && (double) billboard.Size.X != 0.0 && (double) billboard.Size.Y != 0.0 && (double) billboard.Scale.X != 0.0 && (double) billboard.Scale.Y != 0.0 && billboard.Texture != null)
          {
            if (val1 < num1)
            {
              Vector3 position = billboard.Position;
              Vector3 vector3_3 = vector3_1 * billboard.Size.X * billboard.Scale.X;
              Vector3 vector3_4 = vector3_2 * billboard.Size.Y * billboard.Scale.Y;
              Vector3 vector3_5 = -vector3_3;
              Vector3 vector3_6 = -vector3_4;
              int index1 = val1 * 4;
              int index2 = val1 * 4 + 1;
              int index3 = val1 * 4 + 2;
              int index4 = val1 * 4 + 3;
              this.billboardInfo[index1].Color = billboard.Color;
              this.billboardInfo[index1].TextureCoordinate.X = billboard.Texture.LeftUV;
              this.billboardInfo[index1].TextureCoordinate.Y = billboard.Texture.BottomUV;
              this.billboardInfo[index1].Position = position + vector3_3 + vector3_6;
              this.billboardInfo[index2].Color = billboard.Color;
              this.billboardInfo[index2].TextureCoordinate.X = billboard.Texture.LeftUV;
              this.billboardInfo[index2].TextureCoordinate.Y = billboard.Texture.TopUV;
              this.billboardInfo[index2].Position = position + vector3_3 + vector3_4;
              this.billboardInfo[index3].Color = billboard.Color;
              this.billboardInfo[index3].TextureCoordinate.X = billboard.Texture.RightUV;
              this.billboardInfo[index3].TextureCoordinate.Y = billboard.Texture.TopUV;
              this.billboardInfo[index3].Position = position + vector3_5 + vector3_4;
              this.billboardInfo[index4].Color = billboard.Color;
              this.billboardInfo[index4].TextureCoordinate.X = billboard.Texture.RightUV;
              this.billboardInfo[index4].TextureCoordinate.Y = billboard.Texture.BottomUV;
              this.billboardInfo[index4].Position = position + vector3_5 + vector3_6;
              this.billboardTextures[val1] = billboard.Texture.Texture.Texture;
            }
            ++val1;
          }
        }
      }
      this.ResetBillboardBuffers();
      if (val1 <= 0)
        return;
      this.billboardVertices.SetData<VertexPositionColorTexture>(this.billboardInfo);
      Engine.Graphics.GraphicsDevice.SetVertexBuffer(this.billboardVertices);
      Engine.Graphics.GraphicsDevice.Indices = this.billboardIndices;
      Engine.Graphics.GraphicsDevice.RasterizerState = MountainModel.CullNoneRasterizer;
      Engine.Graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
      Engine.Graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
      Engine.Graphics.GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
      GFX.FxTexture.Parameters["World"].SetValue(matrix);
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
      Engine.Graphics.GraphicsDevice.Textures[0] = (Texture) texture;
      foreach (EffectPass pass in GFX.FxTexture.CurrentTechnique.Passes)
      {
        pass.Apply();
        Engine.Graphics.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, offset * 4, 0, sprites * 4, 0, sprites * 2);
      }
    }

    public void Render()
    {
      float scale = (float) Engine.ViewWidth / (float) this.buffer.Width;
      Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, (DepthStencilState) null, (RasterizerState) null);
      Draw.SpriteBatch.Draw((Texture2D) (RenderTarget2D) this.buffer, Vector2.Zero, new Rectangle?(this.buffer.Bounds), Color.White * 1f, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
      Draw.SpriteBatch.Draw((Texture2D) (RenderTarget2D) this.blurB, Vector2.Zero, new Rectangle?(this.blurB.Bounds), Color.White, 0.0f, Vector2.Zero, scale * 2f, SpriteEffects.None, 0.0f);
      Draw.SpriteBatch.End();
    }
  }
}
