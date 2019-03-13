// Decompiled with JetBrains decompiler
// Type: Celeste.TheoPhone
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  public class TheoPhone : Entity
  {
    private VertexLight light;

    public TheoPhone(Vector2 position)
      : base(position)
    {
      this.Add((Component) (this.light = new VertexLight(Color.LawnGreen, 1f, 8, 16)));
      this.Add((Component) new Monocle.Image(GFX.Game["characters/theo/phone"]).JustifyOrigin(0.5f, 1f));
    }

    public override void Update()
    {
      if (this.Scene.OnInterval(0.5f))
        this.light.Visible = !this.light.Visible;
      base.Update();
    }
  }
}

