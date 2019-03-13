// Decompiled with JetBrains decompiler
// Type: Celeste.DreamStars
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  public class DreamStars : Backdrop
  {
    private DreamStars.Stars[] stars = new DreamStars.Stars[50];
    private Vector2 angle = Vector2.Normalize(new Vector2(-2f, -7f));
    private Vector2 lastCamera = Vector2.Zero;

    public DreamStars()
    {
      for (int index = 0; index < this.stars.Length; ++index)
      {
        this.stars[index].Position = new Vector2(Calc.Random.NextFloat(320f), Calc.Random.NextFloat(180f));
        this.stars[index].Speed = 24f + Calc.Random.NextFloat(24f);
        this.stars[index].Size = 2f + Calc.Random.NextFloat(6f);
      }
    }

    public override void Update(Scene scene)
    {
      base.Update(scene);
      Vector2 position = (scene as Level).Camera.Position;
      Vector2 vector2 = position - this.lastCamera;
      for (int index = 0; index < this.stars.Length; ++index)
        this.stars[index].Position += this.angle * this.stars[index].Speed * Engine.DeltaTime - vector2 * 0.5f;
      this.lastCamera = position;
    }

    public override void Render(Scene scene)
    {
      for (int index = 0; index < this.stars.Length; ++index)
        Draw.HollowRect(new Vector2(this.mod(this.stars[index].Position.X, 320f), this.mod(this.stars[index].Position.Y, 180f)), this.stars[index].Size, this.stars[index].Size, Color.Teal);
    }

    private float mod(float x, float m)
    {
      return (x % m + m) % m;
    }

    private struct Stars
    {
      public Vector2 Position;
      public float Speed;
      public float Size;
    }
  }
}

