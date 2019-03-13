// Decompiled with JetBrains decompiler
// Type: Celeste.CustomSpriteEffect
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste
{
  public class CustomSpriteEffect : Effect
  {
    private EffectParameter matrixParam;

    public CustomSpriteEffect(Effect effect)
      : base(effect)
    {
      this.matrixParam = this.Parameters["MatrixTransform"];
    }

    protected override void OnApply()
    {
      Viewport viewport = this.GraphicsDevice.Viewport;
      this.matrixParam.SetValue(Matrix.CreateOrthographicOffCenter(0.0f, (float) viewport.Width, (float) viewport.Height, 0.0f, 0.0f, 1f));
      base.OnApply();
    }
  }
}

