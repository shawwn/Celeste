// Decompiled with JetBrains decompiler
// Type: Celeste.IntroPavement
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  public class IntroPavement : Solid
  {
    private int columns;

    public IntroPavement(Vector2 position, int width)
      : base(position, (float) width, 8f, true)
    {
      this.columns = width / 8;
      this.Depth = -10;
      this.SurfaceSoundIndex = 1;
      this.SurfaceSoundPriority = 10;
    }

    public override void Awake(Scene scene)
    {
      for (int index = 0; index < this.columns; ++index)
      {
        int num = index >= this.columns - 2 ? (index != this.columns - 2 ? 3 : 2) : Calc.Random.Next(0, 2);
        Monocle.Image image = new Monocle.Image(GFX.Game["scenery/car/pavement"].GetSubtexture(num * 8, 0, 8, 8, (MTexture) null));
        image.Position = new Vector2((float) (index * 8), 0.0f);
        this.Add((Component) image);
      }
    }
  }
}
