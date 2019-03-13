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
    {
      base.\u002Ector(effect);
      this.matrixParam = this.get_Parameters().get_Item("MatrixTransform");
    }

    protected virtual void OnApply()
    {
      Viewport viewport = ((GraphicsResource) this).get_GraphicsDevice().get_Viewport();
      this.matrixParam.SetValue(Matrix.op_Multiply(Matrix.CreateTranslation(-0.5f, -0.5f, 0.0f), Matrix.CreateOrthographicOffCenter(0.0f, (float) ((Viewport) ref viewport).get_Width(), (float) ((Viewport) ref viewport).get_Height(), 0.0f, 0.0f, 1f)));
      base.OnApply();
    }
  }
}
