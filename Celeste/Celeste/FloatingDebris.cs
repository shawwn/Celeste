// Decompiled with JetBrains decompiler
// Type: Celeste.FloatingDebris
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  [Tracked(false)]
  public class FloatingDebris : Entity
  {
    private Vector2 start;
    private Monocle.Image image;
    private SineWave sine;
    private float rotateSpeed;
    private Vector2 pushOut;
    private float accelMult = 1f;

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
      this.rotateSpeed = (float) (Calc.Random.Choose<int>(-2, -1, 0, 0, 0, 0, 0, 0, 0, 1, 2) * 40) * ((float) Math.PI / 180f);
      this.Add((Component) (this.sine = new SineWave(0.4f)));
      this.sine.Randomize();
      this.image.Y = this.sine.Value * 2f;
      this.Add((Component) new PlayerCollider(new Action<Player>(this.OnPlayer)));
    }

    public FloatingDebris(EntityData data, Vector2 offset)
      : this(data.Position + offset)
    {
    }

    public override void Update()
    {
      base.Update();
      if (this.pushOut != Vector2.Zero)
      {
        this.Position = this.Position + this.pushOut * Engine.DeltaTime;
        this.pushOut = Calc.Approach(this.pushOut, Vector2.Zero, 64f * this.accelMult * Engine.DeltaTime);
      }
      else
      {
        this.accelMult = 1f;
        this.Position = Calc.Approach(this.Position, this.start, 6f * Engine.DeltaTime);
      }
      this.image.Rotation += this.rotateSpeed * Engine.DeltaTime;
      this.image.Y = this.sine.Value * 2f;
    }

    private void OnPlayer(Player player)
    {
      Vector2 vector2 = (this.Position - player.Center).SafeNormalize(player.Speed.Length() * 0.2f);
      if ((double) vector2.LengthSquared() > (double) this.pushOut.LengthSquared())
        this.pushOut = vector2;
      this.accelMult = 1f;
    }

    public void OnExplode(Vector2 from)
    {
      this.pushOut = (this.Position - from).SafeNormalize(160f);
      this.accelMult = 4f;
    }
  }
}
