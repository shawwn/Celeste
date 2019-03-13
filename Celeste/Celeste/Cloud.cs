// Decompiled with JetBrains decompiler
// Type: Celeste.Cloud
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  public class Cloud : JumpThru
  {
    private bool waiting = true;
    public static ParticleType P_Cloud;
    public static ParticleType P_FragileCloud;
    private Sprite sprite;
    private Wiggler wiggler;
    private ParticleType particleType;
    private SoundSource sfx;
    private float speed;
    private float startY;
    private float respawnTimer;
    private bool returning;
    private bool fragile;
    private float timer;
    private Vector2 scale;
    private bool canRumble;

    public Cloud(Vector2 position, bool fragile)
      : base(position, 32, false)
    {
      this.fragile = fragile;
      this.startY = this.Y;
      this.Collider.Position.X = -16f;
      this.timer = Calc.Random.NextFloat() * 4f;
      this.Add((Component) (this.wiggler = Wiggler.Create(0.3f, 4f, (Action<float>) null, false, false)));
      this.particleType = fragile ? Cloud.P_FragileCloud : Cloud.P_Cloud;
      this.SurfaceSoundIndex = 4;
      this.Add((Component) new LightOcclude(0.2f));
      this.scale = Vector2.One;
      this.Add((Component) (this.sfx = new SoundSource()));
    }

    public Cloud(EntityData data, Vector2 offset)
      : this(data.Position + offset, data.Bool(nameof (fragile), false))
    {
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      string id = this.fragile ? "cloudFragile" : "cloud";
      if ((uint) this.SceneAs<Level>().Session.Area.Mode > 0U)
      {
        this.Collider.Position.X += 2f;
        this.Collider.Width -= 6f;
        id += "Remix";
      }
      this.Add((Component) (this.sprite = GFX.SpriteBank.Create(id)));
      this.sprite.Origin = new Vector2(this.sprite.Width / 2f, 8f);
      this.sprite.OnFrameChange = (Action<string>) (s =>
      {
        if (!(s == "spawn") || this.sprite.CurrentAnimationFrame != 6)
          return;
        this.wiggler.Start();
      });
    }

    public override void Update()
    {
      base.Update();
      this.scale.X = Calc.Approach(this.scale.X, 1f, 1f * Engine.DeltaTime);
      this.scale.Y = Calc.Approach(this.scale.Y, 1f, 1f * Engine.DeltaTime);
      this.timer += Engine.DeltaTime;
      if (this.GetPlayerRider() != null)
        this.sprite.Position = Vector2.Zero;
      else
        this.sprite.Position = Calc.Approach(this.sprite.Position, new Vector2(0.0f, (float) Math.Sin((double) this.timer * 2.0)), Engine.DeltaTime * 4f);
      if ((double) this.respawnTimer > 0.0)
      {
        this.respawnTimer -= Engine.DeltaTime;
        if ((double) this.respawnTimer > 0.0)
          return;
        this.waiting = true;
        this.Y = this.startY;
        this.speed = 0.0f;
        this.scale = Vector2.One;
        this.Collidable = true;
        this.sprite.Play("spawn", false, false);
        this.sfx.Play("event:/game/04_cliffside/cloud_pink_reappear", (string) null, 0.0f);
      }
      else if (this.waiting)
      {
        Player playerRider = this.GetPlayerRider();
        if (playerRider == null || (double) playerRider.Speed.Y < 0.0)
          return;
        this.canRumble = true;
        this.speed = 180f;
        this.scale = new Vector2(1.3f, 0.7f);
        this.waiting = false;
        if (this.fragile)
          Audio.Play("event:/game/04_cliffside/cloud_pink_boost", this.Position);
        else
          Audio.Play("event:/game/04_cliffside/cloud_blue_boost", this.Position);
      }
      else if (this.returning)
      {
        this.speed = Calc.Approach(this.speed, 180f, 600f * Engine.DeltaTime);
        this.MoveTowardsY(this.startY, this.speed * Engine.DeltaTime);
        if ((double) this.ExactPosition.Y != (double) this.startY)
          return;
        this.returning = false;
        this.waiting = true;
        this.speed = 0.0f;
      }
      else
      {
        if (this.fragile && this.Collidable && !this.HasPlayerRider())
        {
          this.Collidable = false;
          this.sprite.Play("fade", false, false);
        }
        if ((double) this.speed < 0.0 && this.canRumble)
        {
          this.canRumble = false;
          if (this.HasPlayerRider())
            Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
        }
        if ((double) this.speed < 0.0 && this.Scene.OnInterval(0.02f))
          (this.Scene as Level).ParticlesBG.Emit(this.particleType, 1, this.Position + new Vector2(0.0f, 2f), new Vector2(this.Collider.Width / 2f, 1f), 1.570796f);
        if (this.fragile && (double) this.speed < 0.0)
          this.sprite.Scale.Y = Calc.Approach(this.sprite.Scale.Y, 0.0f, Engine.DeltaTime * 4f);
        if ((double) this.Y >= (double) this.startY)
        {
          this.speed -= 1200f * Engine.DeltaTime;
        }
        else
        {
          this.speed += 1200f * Engine.DeltaTime;
          if ((double) this.speed >= -100.0)
          {
            Player playerRider = this.GetPlayerRider();
            if (playerRider != null && (double) playerRider.Speed.Y >= 0.0)
              playerRider.Speed.Y = -200f;
            if (this.fragile)
            {
              this.Collidable = false;
              this.sprite.Play("fade", false, false);
              this.respawnTimer = 2.5f;
            }
            else
            {
              this.scale = new Vector2(0.7f, 1.3f);
              this.returning = true;
            }
          }
        }
        float liftSpeedV = this.speed;
        if ((double) liftSpeedV < 0.0)
          liftSpeedV = -220f;
        this.MoveV(this.speed * Engine.DeltaTime, liftSpeedV);
      }
    }

    public override void Render()
    {
      this.sprite.Scale = this.scale * (float) (1.0 + 0.100000001490116 * (double) this.wiggler.Value);
      base.Render();
    }
  }
}

