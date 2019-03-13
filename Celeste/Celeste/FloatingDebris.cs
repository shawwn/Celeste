// Decompiled with JetBrains decompiler
// Type: Celeste.FloatingDebris
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  public class FloatingDebris : Entity
  {
    private Vector2 start;
    private Monocle.Image image;
    private SineWave sine;
    private float rotateSpeed;
    private Vector2 pushOut;

    public FloatingDebris(Vector2 position)
      : base(position)
    {
      this.start = this.Position;
      this.Collider = (Collider) new Hitbox(12f, 12f, -6f, -6f);
      this.Depth = -5;
      MTexture parent = GFX.Game["scenery/debris"];
      this.image = new Monocle.Image(new MTexture(parent, Calc.Random.Next(parent.Width / 8) * 8, 0, 8, 8));
      this.image.CenterOrigin();
      this.Add((Component) this.image);
      this.rotateSpeed = (float) (Calc.Random.Choose<int>(new int[11]
      {
        -2,
        -1,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        1,
        2
      }) * 40) * ((float) Math.PI / 180f);
      this.Add((Component) (this.sine = new SineWave(0.4f)));
      this.sine.Randomize();
      this.image.Y = this.sine.Value * 2f;
      this.Add((Component) new PlayerCollider(new Action<Player>(this.OnPlayer), (Collider) null, (Collider) null));
    }

    public FloatingDebris(EntityData data, Vector2 offset)
      : this(Vector2.op_Addition(data.Position, offset))
    {
    }

    public override void Update()
    {
      base.Update();
      if (Vector2.op_Inequality(this.pushOut, Vector2.get_Zero()))
      {
        this.Position = Vector2.op_Addition(this.Position, Vector2.op_Multiply(this.pushOut, Engine.DeltaTime));
        this.pushOut = Calc.Approach(this.pushOut, Vector2.get_Zero(), 64f * Engine.DeltaTime);
      }
      else
        this.Position = Calc.Approach(this.Position, this.start, 6f * Engine.DeltaTime);
      this.image.Rotation += this.rotateSpeed * Engine.DeltaTime;
      this.image.Y = this.sine.Value * 2f;
    }

    private void OnPlayer(Player player)
    {
      Vector2 vector2 = Vector2.op_Subtraction(this.Position, player.Center).SafeNormalize(((Vector2) ref player.Speed).Length() * 0.2f);
      if ((double) ((Vector2) ref vector2).LengthSquared() <= (double) ((Vector2) ref this.pushOut).LengthSquared())
        return;
      this.pushOut = vector2;
    }
  }
}
