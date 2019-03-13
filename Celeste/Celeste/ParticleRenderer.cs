// Decompiled with JetBrains decompiler
// Type: Celeste.ParticleRenderer
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System.Collections.Generic;

namespace Celeste
{
  public class ParticleRenderer : Monocle.Renderer
  {
    public List<ParticleSystem> Systems;

    public ParticleRenderer(params ParticleSystem[] system)
    {
      this.Systems = new List<ParticleSystem>();
      this.Systems.AddRange((IEnumerable<ParticleSystem>) system);
    }

    public override void Update(Scene scene)
    {
      foreach (Entity system in this.Systems)
        system.Update();
      base.Update(scene);
    }

    public override void Render(Scene scene)
    {
      Draw.SpriteBatch.Begin((SpriteSortMode) 0, (BlendState) BlendState.AlphaBlend, (SamplerState) SamplerState.LinearClamp, (DepthStencilState) DepthStencilState.None, (RasterizerState) RasterizerState.CullNone, (Effect) null, Engine.ScreenMatrix);
      foreach (Entity system in this.Systems)
        system.Render();
      Draw.SpriteBatch.End();
    }
  }
}
