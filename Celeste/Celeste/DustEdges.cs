// Decompiled with JetBrains decompiler
// Type: Celeste.DustEdges
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
  [Tracked(false)]
  public class DustEdges : Entity
  {
    public static int DustGraphicEstabledCounter = 0;
    private Vector2 noiseFromPos = new Vector2();
    private Vector2 noiseToPos = new Vector2();
    private bool hasDust;
    private float noiseEase;
    private VirtualTexture DustNoiseFrom;
    private VirtualTexture DustNoiseTo;

    public DustEdges()
    {
      this.AddTag((int) Tags.Global | (int) Tags.TransitionUpdate);
      this.Depth = -48;
      this.Add((Component) new BeforeRenderHook(new Action(this.BeforeRender)));
    }

    private void CreateTextures()
    {
      this.DustNoiseFrom = VirtualContent.CreateTexture("dust-noise-a", 128, 72, Color.White);
      this.DustNoiseTo = VirtualContent.CreateTexture("dust-noise-b", 128, 72, Color.White);
      Color[] data = new Color[this.DustNoiseFrom.Width * this.DustNoiseTo.Height];
      for (int index = 0; index < data.Length; ++index)
        data[index] = new Color(Calc.Random.NextFloat(), 0.0f, 0.0f, 0.0f);
      this.DustNoiseFrom.Texture.SetData<Color>(data);
      for (int index = 0; index < data.Length; ++index)
        data[index] = new Color(Calc.Random.NextFloat(), 0.0f, 0.0f, 0.0f);
      this.DustNoiseTo.Texture.SetData<Color>(data);
    }

    public override void Update()
    {
      this.noiseEase = Calc.Approach(this.noiseEase, 1f, Engine.DeltaTime);
      if ((double) this.noiseEase == 1.0)
      {
        VirtualTexture dustNoiseFrom = this.DustNoiseFrom;
        this.DustNoiseFrom = this.DustNoiseTo;
        this.DustNoiseTo = dustNoiseFrom;
        this.noiseFromPos = this.noiseToPos;
        this.noiseToPos = new Vector2(Calc.Random.NextFloat(), Calc.Random.NextFloat());
        this.noiseEase = 0.0f;
      }
      DustEdges.DustGraphicEstabledCounter = 0;
    }

    public override void Removed(Scene scene)
    {
      base.Removed(scene);
      this.Dispose();
    }

    public override void SceneEnd(Scene scene)
    {
      base.SceneEnd(scene);
      this.Dispose();
    }

    public override void HandleGraphicsReset()
    {
      base.HandleGraphicsReset();
      this.Dispose();
    }

    private void Dispose()
    {
      if (this.DustNoiseFrom != null)
        this.DustNoiseFrom.Dispose();
      if (this.DustNoiseTo == null)
        return;
      this.DustNoiseTo.Dispose();
    }

    public void BeforeRender()
    {
      List<Component> components = this.Scene.Tracker.GetComponents<DustEdge>();
      this.hasDust = components.Count > 0;
      if (!this.hasDust)
        return;
      Engine.Graphics.GraphicsDevice.SetRenderTarget((RenderTarget2D) GameplayBuffers.TempA);
      Engine.Graphics.GraphicsDevice.Clear(Color.Transparent);
      Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, (Effect) null, (this.Scene as Level).Camera.Matrix);
      foreach (Component component in components)
      {
        DustEdge dustEdge = component as DustEdge;
        if (dustEdge.Visible && dustEdge.Entity.Visible)
          dustEdge.RenderDust();
      }
      Draw.SpriteBatch.End();
      if (this.DustNoiseFrom == null || this.DustNoiseFrom.IsDisposed)
        this.CreateTextures();
      Vector2 vector2 = this.FlooredCamera();
      Engine.Graphics.GraphicsDevice.SetRenderTarget((RenderTarget2D) GameplayBuffers.ResortDust);
      Engine.Graphics.GraphicsDevice.Clear(Color.Transparent);
      Engine.Graphics.GraphicsDevice.Textures[1] = (Texture) this.DustNoiseFrom.Texture;
      Engine.Graphics.GraphicsDevice.Textures[2] = (Texture) this.DustNoiseTo.Texture;
      GFX.FxDust.Parameters["colors"].SetValue(DustStyles.Get(this.Scene).EdgeColors);
      GFX.FxDust.Parameters["noiseEase"].SetValue(this.noiseEase);
      GFX.FxDust.Parameters["noiseFromPos"].SetValue(this.noiseFromPos + new Vector2(vector2.X / 320f, vector2.Y / 180f));
      GFX.FxDust.Parameters["noiseToPos"].SetValue(this.noiseToPos + new Vector2(vector2.X / 320f, vector2.Y / 180f));
      GFX.FxDust.Parameters["pixel"].SetValue(new Vector2(1f / 320f, 0.005555556f));
      Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, GFX.FxDust, Matrix.Identity);
      Draw.SpriteBatch.Draw((Texture2D) (RenderTarget2D) GameplayBuffers.TempA, Vector2.Zero, Color.White);
      Draw.SpriteBatch.End();
    }

    public override void Render()
    {
      if (!this.hasDust)
        return;
      Vector2 position = this.FlooredCamera();
      Draw.SpriteBatch.Draw((Texture2D) (RenderTarget2D) GameplayBuffers.ResortDust, position, Color.White);
    }

    private Vector2 FlooredCamera()
    {
      Vector2 position = (this.Scene as Level).Camera.Position;
      position.X = (float) (int) Math.Floor((double) position.X);
      position.Y = (float) (int) Math.Floor((double) position.Y);
      return position;
    }
  }
}

