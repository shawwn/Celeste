// Decompiled with JetBrains decompiler
// Type: Celeste.CrystalDebris
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  [Pooled]
  public class CrystalDebris : Actor
  {
    public static ParticleType P_Dust;
    private Monocle.Image image;
    private float percent;
    private float duration;
    private Vector2 speed;
    private Collision collideH;
    private Collision collideV;
    private Color color;
    private bool bossShatter;

    public CrystalDebris()
      : base(Vector2.get_Zero())
    {
      this.Depth = -9990;
      this.Collider = (Collider) new Hitbox(2f, 2f, -1f, -1f);
      this.collideH = new Collision(this.OnCollideH);
      this.collideV = new Collision(this.OnCollideV);
      this.image = new Monocle.Image(GFX.Game["particles/shard"]);
      this.image.CenterOrigin();
      this.Add((Component) this.image);
    }

    private void Init(Vector2 position, Color color, bool boss)
    {
      this.Position = position;
      this.image.Color = this.color = color;
      this.image.Scale = Vector2.get_One();
      this.percent = 0.0f;
      this.duration = boss ? Calc.Random.Range(0.25f, 1f) : Calc.Random.Range(1f, 2f);
      this.speed = Calc.AngleToVector(Calc.Random.NextAngle(), boss ? (float) Calc.Random.Range(200, 240) : (float) Calc.Random.Range(60, 160));
      this.bossShatter = boss;
    }

    public override void Update()
    {
      base.Update();
      if ((double) this.percent > 1.0)
      {
        this.RemoveSelf();
      }
      else
      {
        this.percent += Engine.DeltaTime / this.duration;
        if (!this.bossShatter)
        {
          this.speed.X = (__Null) (double) Calc.Approach((float) this.speed.X, 0.0f, Engine.DeltaTime * 20f);
          ref __Null local = ref this.speed.Y;
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          ^(float&) ref local = ^(float&) ref local + 200f * Engine.DeltaTime;
        }
        else
          this.speed = Vector2.op_Multiply(this.speed.SafeNormalize(), Calc.Approach(((Vector2) ref this.speed).Length(), 0.0f, 300f * Engine.DeltaTime));
        if ((double) ((Vector2) ref this.speed).Length() > 0.0)
          this.image.Rotation = this.speed.Angle();
        this.image.Scale = Vector2.op_Multiply(Vector2.get_One(), Calc.ClampedMap(this.percent, 0.8f, 1f, 1f, 0.0f));
        ref __Null local1 = ref this.image.Scale.X;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(float&) ref local1 = ^(float&) ref local1 * Calc.ClampedMap(((Vector2) ref this.speed).Length(), 0.0f, 400f, 1f, 2f);
        ref __Null local2 = ref this.image.Scale.Y;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(float&) ref local2 = ^(float&) ref local2 * Calc.ClampedMap(((Vector2) ref this.speed).Length(), 0.0f, 400f, 1f, 0.2f);
        this.MoveH((float) this.speed.X * Engine.DeltaTime, this.collideH, (Solid) null);
        this.MoveV((float) this.speed.Y * Engine.DeltaTime, this.collideV, (Solid) null);
        if (!this.Scene.OnInterval(0.05f))
          return;
        (this.Scene as Level).ParticlesFG.Emit(CrystalDebris.P_Dust, this.Position);
      }
    }

    public override void Render()
    {
      Color color = this.image.Color;
      this.image.Color = Color.get_Black();
      this.image.Position = new Vector2(-1f, 0.0f);
      this.image.Render();
      this.image.Position = new Vector2(0.0f, -1f);
      this.image.Render();
      this.image.Position = new Vector2(1f, 0.0f);
      this.image.Render();
      this.image.Position = new Vector2(0.0f, 1f);
      this.image.Render();
      this.image.Position = Vector2.get_Zero();
      this.image.Color = color;
      base.Render();
    }

    private void OnCollideH(CollisionData hit)
    {
      ref __Null local = ref this.speed.X;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local = ^(float&) ref local * -0.8f;
    }

    private void OnCollideV(CollisionData hit)
    {
      if (this.bossShatter)
      {
        this.RemoveSelf();
      }
      else
      {
        if (Math.Sign((float) this.speed.X) != 0)
        {
          ref __Null local = ref this.speed.X;
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          ^(float&) ref local = ^(float&) ref local + (float) (Math.Sign((float) this.speed.X) * 5);
        }
        else
        {
          ref __Null local = ref this.speed.X;
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          ^(float&) ref local = ^(float&) ref local + (float) (Calc.Random.Choose<int>(-1, 1) * 5);
        }
        ref __Null local1 = ref this.speed.Y;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(float&) ref local1 = ^(float&) ref local1 * -1.2f;
      }
    }

    public static void Burst(Vector2 position, Color color, bool boss, int count = 1)
    {
      for (int index = 0; index < count; ++index)
      {
        CrystalDebris crystalDebris = Engine.Pooler.Create<CrystalDebris>();
        Vector2 position1 = Vector2.op_Addition(position, new Vector2((float) Calc.Random.Range(-4, 4), (float) Calc.Random.Range(-4, 4)));
        crystalDebris.Init(position1, color, boss);
        Engine.Scene.Add((Entity) crystalDebris);
      }
    }
  }
}
