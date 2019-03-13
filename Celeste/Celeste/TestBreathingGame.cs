// Decompiled with JetBrains decompiler
// Type: Celeste.TestBreathingGame
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
  public class TestBreathingGame : Scene
  {
    private BreathingMinigame game;

    public TestBreathingGame()
    {
      this.game = new BreathingMinigame(true, (BreathingRumbler) null);
      this.Add((Entity) this.game);
    }

    public override void BeforeRender()
    {
      this.game.BeforeRender();
      base.BeforeRender();
    }

    public override void Render()
    {
      Draw.SpriteBatch.Begin((SpriteSortMode) 0, (BlendState) BlendState.AlphaBlend, (SamplerState) SamplerState.LinearClamp, (DepthStencilState) null, (RasterizerState) null, (Effect) null, Engine.ScreenMatrix);
      this.game.Render();
      Draw.SpriteBatch.End();
    }
  }
}
