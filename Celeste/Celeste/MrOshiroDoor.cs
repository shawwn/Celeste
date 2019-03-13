// Decompiled with JetBrains decompiler
// Type: Celeste.MrOshiroDoor
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  public class MrOshiroDoor : Solid
  {
    private Sprite sprite;
    private Wiggler wiggler;

    public MrOshiroDoor(EntityData data, Vector2 offset)
      : base(data.Position + offset, 32f, 32f, false)
    {
      this.Add((Component) (this.sprite = GFX.SpriteBank.Create("ghost_door")));
      this.sprite.Position = new Vector2(this.Width, this.Height) / 2f;
      this.sprite.Play("idle", false, false);
      this.OnDashCollide = new DashCollision(this.OnDashed);
      this.Add((Component) (this.wiggler = Wiggler.Create(0.6f, 3f, (Action<float>) (f => this.sprite.Scale = Vector2.One * (float) (1.0 - (double) f * 0.200000002980232)), false, false)));
      this.SurfaceSoundIndex = 20;
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      this.Visible = this.Collidable = !this.SceneAs<Level>().Session.GetFlag("oshiro_resort_talked_1");
    }

    public void Open()
    {
      if (!this.Collidable)
        return;
      Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
      Audio.Play("event:/game/03_resort/forcefield_vanish", this.Position);
      this.sprite.Play("open", false, false);
      this.Collidable = false;
    }

    public void InstantOpen()
    {
      this.Collidable = this.Visible = false;
    }

    private DashCollisionResults OnDashed(Player player, Vector2 direction)
    {
      Audio.Play("event:/game/03_resort/forcefield_bump", this.Position);
      this.wiggler.Start();
      return DashCollisionResults.Bounce;
    }
  }
}

