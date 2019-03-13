// Decompiled with JetBrains decompiler
// Type: Celeste.IntroCarBarrier
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  public class IntroCarBarrier : Entity
  {
    public IntroCarBarrier(Vector2 position, int depth, Color color)
    {
      this.Position = position;
      this.Depth = depth;
      Monocle.Image image = new Monocle.Image(GFX.Game["scenery/car/barrier"]);
      image.Origin = new Vector2(0.0f, image.Height);
      image.Color = color;
      this.Add((Component) image);
    }
  }
}
