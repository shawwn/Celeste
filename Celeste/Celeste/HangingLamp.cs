// Decompiled with JetBrains decompiler
// Type: Celeste.HangingLamp
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste
{
  public class HangingLamp : Entity
  {
    private List<Monocle.Image> images = new List<Monocle.Image>();
    private float speed = 0.0f;
    private float rotation = 0.0f;
    private float soundDelay = 0.0f;
    public readonly int Length;
    private BloomPoint bloom;
    private VertexLight light;
    private SoundSource sfx;

    public HangingLamp(Vector2 position, int length)
    {
      this.Position = position + Vector2.UnitX * 4f;
      this.Length = Math.Max(16, length);
      this.Depth = 2000;
      MTexture mtexture = GFX.Game["objects/hanginglamp"];
      for (int index = 0; index < this.Length - 8; index += 8)
      {
        Monocle.Image image;
        this.Add((Component) (image = new Monocle.Image(mtexture.GetSubtexture(0, 8, 8, 8, (MTexture) null))));
        image.Origin.X = 4f;
        image.Origin.Y = (float) -index;
        this.images.Add(image);
      }
      Monocle.Image image1;
      this.Add((Component) (image1 = new Monocle.Image(mtexture.GetSubtexture(0, 0, 8, 8, (MTexture) null))));
      image1.Origin.X = 4f;
      Monocle.Image image2;
      this.Add((Component) (image2 = new Monocle.Image(mtexture.GetSubtexture(0, 16, 8, 8, (MTexture) null))));
      image2.Origin.X = 4f;
      image2.Origin.Y = (float) -(this.Length - 8);
      this.images.Add(image2);
      this.Add((Component) (this.bloom = new BloomPoint(Vector2.UnitY * (float) (this.Length - 4), 1f, 48f)));
      this.Add((Component) (this.light = new VertexLight(Vector2.UnitY * (float) (this.Length - 4), Color.White, 1f, 24, 48)));
      this.Add((Component) (this.sfx = new SoundSource()));
      this.Collider = (Collider) new Hitbox(8f, (float) this.Length, -4f, 0.0f);
    }

    public HangingLamp(EntityData e, Vector2 position)
      : this(position, Math.Max(16, e.Height))
    {
    }

    public override void Update()
    {
      base.Update();
      this.soundDelay -= Engine.DeltaTime;
      Player entity = this.Scene.Tracker.GetEntity<Player>();
      if (entity != null && this.Collider.Collide((Entity) entity))
      {
        this.speed = (float) (-(double) entity.Speed.X * 0.00499999988824129 * (((double) entity.Y - (double) this.Y) / (double) this.Length));
        if ((double) Math.Abs(this.speed) < 0.100000001490116)
          this.speed = 0.0f;
        else if ((double) this.soundDelay <= 0.0)
        {
          this.sfx.Play("event:/game/02_old_site/lantern_hit", (string) null, 0.0f);
          this.soundDelay = 0.25f;
        }
      }
      float num = Math.Sign(this.rotation) == Math.Sign(this.speed) ? 8f : 6f;
      if ((double) Math.Abs(this.rotation) < 0.5)
        num *= 0.5f;
      if ((double) Math.Abs(this.rotation) < 0.25)
        num *= 0.5f;
      float rotation = this.rotation;
      this.speed += (float) -Math.Sign(this.rotation) * num * Engine.DeltaTime;
      this.rotation += this.speed * Engine.DeltaTime;
      this.rotation = Calc.Clamp(this.rotation, -0.4f, 0.4f);
      if ((double) Math.Abs(this.rotation) < 0.0199999995529652 && (double) Math.Abs(this.speed) < 0.200000002980232)
        this.rotation = this.speed = 0.0f;
      else if (Math.Sign(this.rotation) != Math.Sign(rotation) && (double) this.soundDelay <= 0.0 && (double) Math.Abs(this.speed) > 0.5)
      {
        this.sfx.Play("event:/game/02_old_site/lantern_hit", (string) null, 0.0f);
        this.soundDelay = 0.25f;
      }
      foreach (GraphicsComponent image in this.images)
        image.Rotation = this.rotation;
      Vector2 vector = Calc.AngleToVector(this.rotation + 1.570796f, (float) this.Length - 4f);
      this.bloom.Position = this.light.Position = vector;
      this.sfx.Position = vector;
    }

    public override void Render()
    {
      foreach (Component component in this.Components)
      {
        Monocle.Image image = component as Monocle.Image;
        if (image != null)
          image.DrawOutline(1);
      }
      base.Render();
    }
  }
}

