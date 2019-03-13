﻿// Decompiled with JetBrains decompiler
// Type: Monocle.TagExcludeRenderer
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monocle
{
  public class TagExcludeRenderer : Renderer
  {
    public BlendState BlendState;
    public SamplerState SamplerState;
    public Effect Effect;
    public Camera Camera;
    public int ExcludeTag;

    public TagExcludeRenderer(int excludeTag)
    {
      this.ExcludeTag = excludeTag;
      this.BlendState = (BlendState) BlendState.AlphaBlend;
      this.SamplerState = (SamplerState) SamplerState.LinearClamp;
      this.Camera = new Camera();
    }

    public override void BeforeRender(Scene scene)
    {
    }

    public override void Render(Scene scene)
    {
      Draw.SpriteBatch.Begin((SpriteSortMode) 0, this.BlendState, this.SamplerState, (DepthStencilState) DepthStencilState.None, (RasterizerState) RasterizerState.CullNone, this.Effect, Matrix.op_Multiply(this.Camera.Matrix, Engine.ScreenMatrix));
      foreach (Entity entity in scene.Entities)
      {
        if (entity.Visible && (entity.Tag & this.ExcludeTag) == 0)
          entity.Render();
      }
      if (Engine.Commands.Open)
      {
        foreach (Entity entity in scene.Entities)
        {
          if ((entity.Tag & this.ExcludeTag) == 0)
            entity.DebugRender(this.Camera);
        }
      }
      Draw.SpriteBatch.End();
    }

    public override void AfterRender(Scene scene)
    {
    }
  }
}
