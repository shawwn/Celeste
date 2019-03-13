﻿// Decompiled with JetBrains decompiler
// Type: Celeste.MirrorSurfaces
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
  public class MirrorSurfaces : Entity
  {
    public const int MaxMirrorOffset = 32;
    private bool hasReflections;
    private VirtualRenderTarget target;

    public MirrorSurfaces()
    {
      this.Depth = 9490;
      this.Tag = (int) Tags.Global;
      this.Add((Component) new BeforeRenderHook(new Action(this.BeforeRender)));
    }

    public void BeforeRender()
    {
      Level scene = this.Scene as Level;
      List<Component> components1 = this.Scene.Tracker.GetComponents<MirrorReflection>();
      List<Component> components2 = this.Scene.Tracker.GetComponents<MirrorSurface>();
      if (!(this.hasReflections = components2.Count > 0 && components1.Count > 0))
        return;
      if (this.target == null)
        this.target = VirtualContent.CreateRenderTarget("mirror-surfaces", 320, 180, false, true, 0);
      Matrix matrix = Matrix.op_Multiply(Matrix.CreateTranslation(32f, 32f, 0.0f), scene.Camera.Matrix);
      components1.Sort((Comparison<Component>) ((a, b) => b.Entity.Depth - a.Entity.Depth));
      Engine.Graphics.get_GraphicsDevice().SetRenderTarget((RenderTarget2D) GameplayBuffers.MirrorSources);
      Engine.Graphics.get_GraphicsDevice().Clear(Color.get_Transparent());
      Draw.SpriteBatch.Begin((SpriteSortMode) 0, (BlendState) BlendState.AlphaBlend, (SamplerState) SamplerState.PointClamp, (DepthStencilState) DepthStencilState.None, (RasterizerState) RasterizerState.CullNone, (Effect) null, matrix);
      foreach (MirrorReflection mirrorReflection in components1)
      {
        if ((mirrorReflection.Entity.Visible || mirrorReflection.IgnoreEntityVisible) && mirrorReflection.Visible)
        {
          mirrorReflection.IsRendering = true;
          mirrorReflection.Entity.Render();
          mirrorReflection.IsRendering = false;
        }
      }
      Draw.SpriteBatch.End();
      Engine.Graphics.get_GraphicsDevice().SetRenderTarget((RenderTarget2D) GameplayBuffers.MirrorMasks);
      Engine.Graphics.get_GraphicsDevice().Clear(Color.get_Transparent());
      Draw.SpriteBatch.Begin((SpriteSortMode) 0, (BlendState) BlendState.AlphaBlend, (SamplerState) SamplerState.PointClamp, (DepthStencilState) null, (RasterizerState) RasterizerState.CullNone, (Effect) null, matrix);
      foreach (MirrorSurface mirrorSurface in components2)
      {
        if (mirrorSurface.Visible && mirrorSurface.OnRender != null)
          mirrorSurface.OnRender();
      }
      Draw.SpriteBatch.End();
      Engine.Graphics.get_GraphicsDevice().SetRenderTarget((RenderTarget2D) this.target);
      Engine.Graphics.get_GraphicsDevice().Clear(Color.get_Transparent());
      Engine.Graphics.get_GraphicsDevice().get_Textures().set_Item(1, (Texture) (RenderTarget2D) GameplayBuffers.MirrorSources);
      GFX.FxMirrors.get_Parameters().get_Item("pixel").SetValue(new Vector2(1f / (float) GameplayBuffers.MirrorMasks.Width, 1f / (float) GameplayBuffers.MirrorMasks.Height));
      Draw.SpriteBatch.Begin((SpriteSortMode) 0, (BlendState) BlendState.AlphaBlend, (SamplerState) SamplerState.PointClamp, (DepthStencilState) null, (RasterizerState) null, GFX.FxMirrors, Matrix.get_Identity());
      Draw.SpriteBatch.Draw((Texture2D) (RenderTarget2D) GameplayBuffers.MirrorMasks, new Vector2(-32f, -32f), Color.get_White());
      Draw.SpriteBatch.End();
    }

    public override void Render()
    {
      if (!this.hasReflections)
        return;
      Draw.SpriteBatch.Draw((Texture2D) (RenderTarget2D) this.target, this.FlooredCamera(), Color.op_Multiply(Color.get_White(), 0.5f));
    }

    private Vector2 FlooredCamera()
    {
      Vector2 position = (this.Scene as Level).Camera.Position;
      position.X = (__Null) (double) (int) Math.Floor((double) position.X);
      position.Y = (__Null) (double) (int) Math.Floor((double) position.Y);
      return position;
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

    public void Dispose()
    {
      if (this.target != null && !this.target.IsDisposed)
        this.target.Dispose();
      this.target = (VirtualRenderTarget) null;
    }
  }
}
