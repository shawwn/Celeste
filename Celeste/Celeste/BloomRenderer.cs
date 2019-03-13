// Decompiled with JetBrains decompiler
// Type: Celeste.BloomRenderer
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System.Collections.Generic;

namespace Celeste
{
  public class BloomRenderer : Monocle.Renderer
  {
    public static readonly BlendState BlurredScreenToMask = new BlendState()
    {
      ColorSourceBlend = Blend.One,
      ColorDestinationBlend = Blend.Zero,
      ColorBlendFunction = BlendFunction.Add,
      AlphaSourceBlend = Blend.Zero,
      AlphaDestinationBlend = Blend.One,
      AlphaBlendFunction = BlendFunction.Add
    };
    public static readonly BlendState AdditiveMaskToScreen = new BlendState()
    {
      ColorSourceBlend = Blend.SourceAlpha,
      ColorDestinationBlend = Blend.One,
      ColorBlendFunction = BlendFunction.Add,
      AlphaSourceBlend = Blend.Zero,
      AlphaDestinationBlend = Blend.One,
      AlphaBlendFunction = BlendFunction.Add
    };
    public static readonly BlendState CutoutBlendstate = new BlendState()
    {
      ColorSourceBlend = Blend.One,
      ColorDestinationBlend = Blend.One,
      AlphaSourceBlend = Blend.One,
      AlphaDestinationBlend = Blend.One,
      ColorBlendFunction = BlendFunction.Min,
      AlphaBlendFunction = BlendFunction.Min
    };
    public float Strength = 1f;
    public float Base = 0.0f;
    private MTexture gradient;

    public BloomRenderer()
    {
      this.gradient = GFX.Game["util/bloomgradient"];
    }

    public void Apply(VirtualRenderTarget target, Scene scene)
    {
      if ((double) this.Strength <= 0.0)
        return;
      VirtualRenderTarget tempA = GameplayBuffers.TempA;
      Texture2D texture = GaussianBlur.Blur((Texture2D) (RenderTarget2D) target, GameplayBuffers.TempA, GameplayBuffers.TempB, 0.0f, true, GaussianBlur.Samples.Nine, 1f, GaussianBlur.Direction.Both, 1f);
      List<Component> components1 = scene.Tracker.GetComponents<BloomPoint>();
      List<Component> components2 = scene.Tracker.GetComponents<EffectCutout>();
      Engine.Instance.GraphicsDevice.SetRenderTarget((RenderTarget2D) tempA);
      Engine.Instance.GraphicsDevice.Clear(Color.Transparent);
      if ((double) this.Base < 1.0)
      {
        Camera camera = (scene as Level).Camera;
        Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, (Effect) null, camera.Matrix);
        float num = 1f / (float) this.gradient.Width;
        foreach (Component component in components1)
        {
          BloomPoint bloomPoint = component as BloomPoint;
          if (bloomPoint.Visible && (double) bloomPoint.Radius > 0.0 && (double) bloomPoint.Alpha > 0.0)
            this.gradient.DrawCentered(bloomPoint.Entity.Position + bloomPoint.Position, Color.White * bloomPoint.Alpha, bloomPoint.Radius * 2f * num);
        }
        foreach (CustomBloom component in scene.Tracker.GetComponents<CustomBloom>())
        {
          if (component.Visible && component.OnRenderBloom != null)
            component.OnRenderBloom();
        }
        foreach (Entity entity in scene.Tracker.GetEntities<SeekerBarrier>())
          Draw.Rect(entity.Collider, Color.White);
        Draw.SpriteBatch.End();
        if (components2.Count > 0)
        {
          Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BloomRenderer.CutoutBlendstate, SamplerState.PointClamp, (DepthStencilState) null, (RasterizerState) null, (Effect) null, camera.Matrix);
          foreach (Component component in components2)
          {
            EffectCutout effectCutout = component as EffectCutout;
            if (effectCutout.Visible)
              Draw.Rect((float) effectCutout.Left, (float) effectCutout.Top, (float) (effectCutout.Right - effectCutout.Left), (float) (effectCutout.Bottom - effectCutout.Top), Color.White * (1f - effectCutout.Alpha));
          }
          Draw.SpriteBatch.End();
        }
      }
      Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
      Draw.Rect(-10f, -10f, 340f, 200f, Color.White * this.Base);
      Draw.SpriteBatch.End();
      Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BloomRenderer.BlurredScreenToMask);
      Draw.SpriteBatch.Draw(texture, Vector2.Zero, Color.White);
      Draw.SpriteBatch.End();
      Engine.Instance.GraphicsDevice.SetRenderTarget((RenderTarget2D) target);
      Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BloomRenderer.AdditiveMaskToScreen);
      for (int index = 0; (double) index < (double) this.Strength; ++index)
      {
        float num = (double) index < (double) this.Strength - 1.0 ? 1f : this.Strength - (float) index;
        Draw.SpriteBatch.Draw((Texture2D) (RenderTarget2D) tempA, Vector2.Zero, Color.White * num);
      }
      Draw.SpriteBatch.End();
    }
  }
}

