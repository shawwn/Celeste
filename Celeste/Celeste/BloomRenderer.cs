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
    public float Strength = 1f;
    public float Base;
    private MTexture gradient;
    public static readonly BlendState BlurredScreenToMask;
    public static readonly BlendState AdditiveMaskToScreen;
    public static readonly BlendState CutoutBlendstate;

    public BloomRenderer()
    {
      this.gradient = GFX.Game["util/bloomgradient"];
    }

    public void Apply(VirtualRenderTarget target, Scene scene)
    {
      if ((double) this.Strength <= 0.0)
        return;
      VirtualRenderTarget tempA = GameplayBuffers.TempA;
      Texture2D texture2D = GaussianBlur.Blur((Texture2D) (RenderTarget2D) target, GameplayBuffers.TempA, GameplayBuffers.TempB, 0.0f, true, GaussianBlur.Samples.Nine, 1f, GaussianBlur.Direction.Both, 1f);
      List<Component> components1 = scene.Tracker.GetComponents<BloomPoint>();
      List<Component> components2 = scene.Tracker.GetComponents<EffectCutout>();
      Engine.Instance.get_GraphicsDevice().SetRenderTarget((RenderTarget2D) tempA);
      Engine.Instance.get_GraphicsDevice().Clear(Color.get_Transparent());
      if ((double) this.Base < 1.0)
      {
        Camera camera = (scene as Level).Camera;
        Draw.SpriteBatch.Begin((SpriteSortMode) 0, (BlendState) BlendState.AlphaBlend, (SamplerState) SamplerState.PointClamp, (DepthStencilState) DepthStencilState.None, (RasterizerState) RasterizerState.CullNone, (Effect) null, camera.Matrix);
        float num = 1f / (float) this.gradient.Width;
        foreach (Component component in components1)
        {
          BloomPoint bloomPoint = component as BloomPoint;
          if (bloomPoint.Visible && (double) bloomPoint.Radius > 0.0 && (double) bloomPoint.Alpha > 0.0)
            this.gradient.DrawCentered(Vector2.op_Addition(bloomPoint.Entity.Position, bloomPoint.Position), Color.op_Multiply(Color.get_White(), bloomPoint.Alpha), bloomPoint.Radius * 2f * num);
        }
        foreach (CustomBloom component in scene.Tracker.GetComponents<CustomBloom>())
        {
          if (component.Visible && component.OnRenderBloom != null)
            component.OnRenderBloom();
        }
        foreach (Entity entity in scene.Tracker.GetEntities<SeekerBarrier>())
          Draw.Rect(entity.Collider, Color.get_White());
        Draw.SpriteBatch.End();
        if (components2.Count > 0)
        {
          Draw.SpriteBatch.Begin((SpriteSortMode) 0, BloomRenderer.CutoutBlendstate, (SamplerState) SamplerState.PointClamp, (DepthStencilState) null, (RasterizerState) null, (Effect) null, camera.Matrix);
          foreach (Component component in components2)
          {
            EffectCutout effectCutout = component as EffectCutout;
            if (effectCutout.Visible)
              Draw.Rect((float) effectCutout.Left, (float) effectCutout.Top, (float) (effectCutout.Right - effectCutout.Left), (float) (effectCutout.Bottom - effectCutout.Top), Color.op_Multiply(Color.get_White(), 1f - effectCutout.Alpha));
          }
          Draw.SpriteBatch.End();
        }
      }
      Draw.SpriteBatch.Begin((SpriteSortMode) 0, (BlendState) BlendState.AlphaBlend);
      Draw.Rect(-10f, -10f, 340f, 200f, Color.op_Multiply(Color.get_White(), this.Base));
      Draw.SpriteBatch.End();
      Draw.SpriteBatch.Begin((SpriteSortMode) 0, BloomRenderer.BlurredScreenToMask);
      Draw.SpriteBatch.Draw(texture2D, Vector2.get_Zero(), Color.get_White());
      Draw.SpriteBatch.End();
      Engine.Instance.get_GraphicsDevice().SetRenderTarget((RenderTarget2D) target);
      Draw.SpriteBatch.Begin((SpriteSortMode) 0, BloomRenderer.AdditiveMaskToScreen);
      for (int index = 0; (double) index < (double) this.Strength; ++index)
      {
        float num = (double) index < (double) this.Strength - 1.0 ? 1f : this.Strength - (float) index;
        Draw.SpriteBatch.Draw((Texture2D) (RenderTarget2D) tempA, Vector2.get_Zero(), Color.op_Multiply(Color.get_White(), num));
      }
      Draw.SpriteBatch.End();
    }

    static BloomRenderer()
    {
      BlendState blendState1 = new BlendState();
      blendState1.set_ColorSourceBlend((Blend) 0);
      blendState1.set_ColorDestinationBlend((Blend) 1);
      blendState1.set_ColorBlendFunction((BlendFunction) 0);
      blendState1.set_AlphaSourceBlend((Blend) 1);
      blendState1.set_AlphaDestinationBlend((Blend) 0);
      blendState1.set_AlphaBlendFunction((BlendFunction) 0);
      BloomRenderer.BlurredScreenToMask = blendState1;
      BlendState blendState2 = new BlendState();
      blendState2.set_ColorSourceBlend((Blend) 4);
      blendState2.set_ColorDestinationBlend((Blend) 0);
      blendState2.set_ColorBlendFunction((BlendFunction) 0);
      blendState2.set_AlphaSourceBlend((Blend) 1);
      blendState2.set_AlphaDestinationBlend((Blend) 0);
      blendState2.set_AlphaBlendFunction((BlendFunction) 0);
      BloomRenderer.AdditiveMaskToScreen = blendState2;
      BlendState blendState3 = new BlendState();
      blendState3.set_ColorSourceBlend((Blend) 0);
      blendState3.set_ColorDestinationBlend((Blend) 0);
      blendState3.set_AlphaSourceBlend((Blend) 0);
      blendState3.set_AlphaDestinationBlend((Blend) 0);
      blendState3.set_ColorBlendFunction((BlendFunction) 3);
      blendState3.set_AlphaBlendFunction((BlendFunction) 3);
      BloomRenderer.CutoutBlendstate = blendState3;
    }
  }
}
