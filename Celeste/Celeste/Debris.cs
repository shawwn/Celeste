// Decompiled with JetBrains decompiler
// Type: Celeste.Debris
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  [Pooled]
  public class Debris : Actor
  {
    private Monocle.Image image;
    private float lifeTimer;
    private float alpha;
    private Vector2 speed;
    private Collision collideH;
    private Collision collideV;
    private int rotateSign;
    private float fadeLerp;
    private bool dreaming;
    private SineWave dreamSine;
    private bool hasHitGround;
    private char tileset;

    public Debris()
      : base(Vector2.Zero)
    {
      this.Collider = (Collider) new Hitbox(4f, 4f, -2f, -2f);
      this.Tag = (int) Tags.Persistent;
      this.Depth = 2000;
      this.Add((Component) (this.image = new Monocle.Image((MTexture) null)));
      this.collideH = new Collision(this.OnCollideH);
      this.collideV = new Collision(this.OnCollideV);
      this.Add((Component) (this.dreamSine = new SineWave(0.6f)));
      this.dreamSine.Randomize();
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      this.dreaming = this.SceneAs<Level>().Session.Dreaming;
    }

    public Debris Init(Vector2 pos, char tileset)
    {
      this.Position = pos;
      this.tileset = tileset;
      this.lifeTimer = Calc.Random.Range(0.6f, 2.6f);
      this.alpha = 1f;
      this.hasHitGround = false;
      this.speed = Vector2.Zero;
      this.fadeLerp = 0.0f;
      this.rotateSign = Calc.Random.Choose<int>(1, -1);
      this.image.Texture = !GFX.Game.Has("debris/" + tileset.ToString()) ? GFX.Game["debris/1"] : GFX.Game["debris/" + tileset.ToString()];
      this.image.CenterOrigin();
      this.image.Color = Color.White * this.alpha;
      this.image.Rotation = Calc.Random.NextAngle();
      this.image.Scale.X = Calc.Random.Range(0.5f, 1f);
      this.image.Scale.Y = Calc.Random.Range(0.5f, 1f);
      this.image.FlipX = Calc.Random.Chance(0.5f);
      this.image.FlipY = Calc.Random.Chance(0.5f);
      return this;
    }

    public Debris BlastFrom(Vector2 from)
    {
      float length = (float) Calc.Random.Range(30, 40);
      this.speed = (this.Position - from).SafeNormalize(length);
      this.speed = this.speed.Rotate(Calc.Random.Range(-0.2617994f, 0.2617994f));
      return this;
    }

    private void OnCollideH(CollisionData data)
    {
      this.speed.X *= -0.8f;
    }

    private void OnCollideV(CollisionData data)
    {
      if ((double) this.speed.Y > 0.0)
        this.hasHitGround = true;
      this.speed.Y *= -0.6f;
      if ((double) this.speed.Y < 0.0 && (double) this.speed.Y > -50.0)
        this.speed.Y = 0.0f;
      if ((double) this.speed.Y == 0.0 && this.hasHitGround)
        return;
      this.ImpactSfx(Math.Abs(this.speed.Y));
    }

    private void ImpactSfx(float spd)
    {
      string path = "event:/game/general/debris_dirt";
      if (this.tileset == '4' || this.tileset == '5' || (this.tileset == '6' || this.tileset == '7') || (this.tileset == 'a' || this.tileset == 'c' || (this.tileset == 'd' || this.tileset == 'e')) || (this.tileset == 'f' || this.tileset == 'd') || this.tileset == 'g')
        path = "event:/game/general/debris_stone";
      else if (this.tileset == '9')
        path = "event:/game/general/debris_wood";
      Audio.Play(path, this.Position, "debris_velocity", Calc.ClampedMap(spd, 0.0f, 150f, 0.0f, 1f));
    }

    public override void Update()
    {
      base.Update();
      this.image.Rotation += Math.Abs(this.speed.X) * (float) this.rotateSign * Engine.DeltaTime;
      if ((double) this.fadeLerp < 1.0)
        this.fadeLerp = Calc.Approach(this.fadeLerp, 1f, 2f * Engine.DeltaTime);
      this.MoveH(this.speed.X * Engine.DeltaTime, this.collideH, (Solid) null);
      this.MoveV(this.speed.Y * Engine.DeltaTime, this.collideV, (Solid) null);
      if (this.dreaming)
      {
        this.speed.X = Calc.Approach(this.speed.X, 0.0f, 50f * Engine.DeltaTime);
        this.speed.Y = Calc.Approach(this.speed.Y, 6f * this.dreamSine.Value, 100f * Engine.DeltaTime);
      }
      else
      {
        bool flag = this.OnGround(1);
        this.speed.X = Calc.Approach(this.speed.X, 0.0f, (flag ? 50f : 20f) * Engine.DeltaTime);
        if (!flag)
          this.speed.Y = Calc.Approach(this.speed.Y, 100f, 400f * Engine.DeltaTime);
      }
      if ((double) this.lifeTimer > 0.0)
        this.lifeTimer -= Engine.DeltaTime;
      else if ((double) this.alpha > 0.0)
      {
        this.alpha -= 4f * Engine.DeltaTime;
        if ((double) this.alpha <= 0.0)
          this.RemoveSelf();
      }
      this.image.Color = Color.Lerp(Color.White, Color.Gray, this.fadeLerp) * this.alpha;
    }
  }
}

