// Decompiled with JetBrains decompiler
// Type: Celeste.Snowball
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  public class Snowball : Entity
  {
    private const float ResetTime = 0.8f;
    private Sprite sprite;
    private float resetTimer;
    private Level level;
    private SineWave sine;
    private float atY;
    private SoundSource spawnSfx;
    private Collider bounceCollider;

    public Snowball()
    {
      this.Depth = -12500;
      this.Collider = (Collider) new Hitbox(12f, 9f, -5f, -2f);
      this.bounceCollider = (Collider) new Hitbox(16f, 6f, -6f, -8f);
      this.Add((Component) new PlayerCollider(new Action<Player>(this.OnPlayer), (Collider) null, (Collider) null));
      this.Add((Component) new PlayerCollider(new Action<Player>(this.OnPlayerBounce), this.bounceCollider, (Collider) null));
      this.Add((Component) (this.sine = new SineWave(0.5f)));
      this.Add((Component) (this.sprite = GFX.SpriteBank.Create("snowball")));
      this.sprite.Play("spin", false, false);
      this.Add((Component) (this.spawnSfx = new SoundSource()));
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      this.level = this.SceneAs<Level>();
      this.ResetPosition();
    }

    private void ResetPosition()
    {
      Player entity = this.level.Tracker.GetEntity<Player>();
      if (entity != null && (double) entity.Right < (double) (this.level.Bounds.Right - 64))
      {
        this.spawnSfx.Play("event:/game/04_cliffside/snowball_spawn", (string) null, 0.0f);
        this.Collidable = this.Visible = true;
        this.resetTimer = 0.0f;
        this.X = this.level.Camera.Right + 10f;
        this.atY = this.Y = entity.CenterY;
        this.sine.Reset();
        this.sprite.Play("spin", false, false);
      }
      else
        this.resetTimer = 0.05f;
    }

    private void Destroy()
    {
      this.Collidable = false;
      this.sprite.Play("break", false, false);
    }

    private void OnPlayer(Player player)
    {
      player.Die(new Vector2(-1f, 0.0f), false, true);
      this.Destroy();
      Audio.Play("event:/game/04_cliffside/snowball_impact", this.Position);
    }

    private void OnPlayerBounce(Player player)
    {
      if (this.CollideCheck((Entity) player))
        return;
      Celeste.Freeze(0.1f);
      player.Bounce(this.Top - 2f);
      this.Destroy();
      Audio.Play("event:/game/general/thing_booped", this.Position);
    }

    public override void Update()
    {
      base.Update();
      this.X -= 200f * Engine.DeltaTime;
      this.Y = this.atY + 4f * this.sine.Value;
      if ((double) this.X >= (double) this.level.Camera.Left - 60.0)
        return;
      this.resetTimer += Engine.DeltaTime;
      if ((double) this.resetTimer >= 0.800000011920929)
        this.ResetPosition();
    }

    public override void Render()
    {
      this.sprite.DrawOutline(1);
      base.Render();
    }
  }
}

