// Decompiled with JetBrains decompiler
// Type: Celeste.FadeWipe
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;

namespace Celeste
{
  public class FadeWipe : ScreenWipe
  {
    private VertexPositionColor[] vertexBuffer = new VertexPositionColor[6];
    public Action<float> OnUpdate;

    public FadeWipe(Scene scene, bool wipeIn, Action onComplete = null)
      : base(scene, wipeIn, onComplete)
    {
    }

    public override void Update(Scene scene)
    {
      base.Update(scene);
      if (this.OnUpdate == null)
        return;
      this.OnUpdate(this.Percent);
    }

    public override void Render(Scene scene)
    {
      Color color = ScreenWipe.WipeColor * (this.WipeIn ? 1f - Ease.CubeIn(this.Percent) : Ease.CubeOut(this.Percent));
      this.vertexBuffer[0].Color = color;
      this.vertexBuffer[0].Position = new Vector3(-10f, -10f, 0.0f);
      this.vertexBuffer[1].Color = color;
      this.vertexBuffer[1].Position = new Vector3((float) this.Right, -10f, 0.0f);
      this.vertexBuffer[2].Color = color;
      this.vertexBuffer[2].Position = new Vector3(-10f, (float) this.Bottom, 0.0f);
      this.vertexBuffer[3].Color = color;
      this.vertexBuffer[3].Position = new Vector3((float) this.Right, -10f, 0.0f);
      this.vertexBuffer[4].Color = color;
      this.vertexBuffer[4].Position = new Vector3((float) this.Right, (float) this.Bottom, 0.0f);
      this.vertexBuffer[5].Color = color;
      this.vertexBuffer[5].Position = new Vector3(-10f, (float) this.Bottom, 0.0f);
      ScreenWipe.DrawPrimitives(this.vertexBuffer);
    }
  }
}

