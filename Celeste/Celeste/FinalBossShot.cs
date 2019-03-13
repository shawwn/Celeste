// Decompiled with JetBrains decompiler
// Type: Celeste.FinalBossShot
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  [Pooled]
  [Tracked(false)]
  public class FinalBossShot : Entity
  {
    public static ParticleType P_Trail;
    private const float MoveSpeed = 100f;
    private const float CantKillTime = 0.15f;
    private const float AppearTime = 0.1f;
    private FinalBoss boss;
    private Level level;
    private Vector2 speed;
    private float particleDir;
    private Vector2 anchor;
    private Vector2 perp;
    private Player target;
    private Vector2 targetPt;
    private float angleOffset;
    private bool dead;
    private float cantKillTimer;
    private float appearTimer;
    private bool hasBeenInCamera;
    private SineWave sine;
    private float sineMult;
    private Sprite sprite;

    public FinalBossShot()
      : base(Vector2.Zero)
    {
      this.Add((Component) (this.sprite = GFX.SpriteBank.Create("badeline_projectile")));
      this.Collider = (Collider) new Hitbox(4f, 4f, -2f, -2f);
      this.Add((Component) new PlayerCollider(new Action<Player>(this.OnPlayer), (Collider) null, (Collider) null));
      this.Depth = -1000000;
      this.Add((Component) (this.sine = new SineWave(1.4f)));
    }

    public FinalBossShot Init(FinalBoss boss, Player target, float angleOffset = 0.0f)
    {
      this.boss = boss;
      this.anchor = this.Position = boss.Center;
      this.target = target;
      this.angleOffset = angleOffset;
      this.dead = this.hasBeenInCamera = false;
      this.cantKillTimer = 0.15f;
      this.appearTimer = 0.1f;
      this.sine.Reset();
      this.sineMult = 0.0f;
      this.sprite.Play("charge", true, false);
      this.InitSpeed();
      return this;
    }

    public FinalBossShot Init(FinalBoss boss, Vector2 target)
    {
      this.boss = boss;
      this.anchor = this.Position = boss.Center;
      this.target = (Player) null;
      this.angleOffset = 0.0f;
      this.targetPt = target;
      this.dead = this.hasBeenInCamera = false;
      this.cantKillTimer = 0.15f;
      this.appearTimer = 0.1f;
      this.sine.Reset();
      this.sineMult = 0.0f;
      this.sprite.Play("charge", true, false);
      this.InitSpeed();
      return this;
    }

    private void InitSpeed()
    {
      this.speed = this.target == null ? (this.targetPt - this.Center).SafeNormalize(100f) : (this.target.Center - this.Center).SafeNormalize(100f);
      if ((double) this.angleOffset != 0.0)
        this.speed = this.speed.Rotate(this.angleOffset);
      this.perp = this.speed.Perpendicular().SafeNormalize();
      this.particleDir = (-this.speed).Angle();
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      this.level = this.SceneAs<Level>();
      if (!this.boss.Moving)
        return;
      this.RemoveSelf();
    }

    public override void Removed(Scene scene)
    {
      base.Removed(scene);
      this.level = (Level) null;
    }

    public override void Update()
    {
      base.Update();
      if ((double) this.appearTimer > 0.0)
      {
        this.Position = this.anchor = this.boss.ShotOrigin;
        this.appearTimer -= Engine.DeltaTime;
      }
      else
      {
        if ((double) this.cantKillTimer > 0.0)
          this.cantKillTimer -= Engine.DeltaTime;
        this.anchor += this.speed * Engine.DeltaTime;
        this.Position = this.anchor + this.perp * this.sineMult * this.sine.Value * 3f;
        this.sineMult = Calc.Approach(this.sineMult, 1f, 2f * Engine.DeltaTime);
        if (!this.dead)
        {
          bool flag = this.level.IsInCamera(this.Position, 8f);
          if (flag && !this.hasBeenInCamera)
            this.hasBeenInCamera = true;
          else if (!flag && this.hasBeenInCamera)
            this.Destroy();
          if (this.Scene.OnInterval(0.04f))
            this.level.ParticlesFG.Emit(FinalBossShot.P_Trail, 1, this.Center, Vector2.One * 2f, this.particleDir);
        }
      }
    }

    public override void Render()
    {
      Color color = this.sprite.Color;
      Vector2 position = this.sprite.Position;
      this.sprite.Color = Color.Black;
      this.sprite.Position = position + new Vector2(-1f, 0.0f);
      this.sprite.Render();
      this.sprite.Position = position + new Vector2(1f, 0.0f);
      this.sprite.Render();
      this.sprite.Position = position + new Vector2(0.0f, -1f);
      this.sprite.Render();
      this.sprite.Position = position + new Vector2(0.0f, 1f);
      this.sprite.Render();
      this.sprite.Color = color;
      this.sprite.Position = position;
      base.Render();
    }

    public void Destroy()
    {
      this.dead = true;
      this.RemoveSelf();
    }

    private void OnPlayer(Player player)
    {
      if (this.dead)
        return;
      if ((double) this.cantKillTimer > 0.0)
        this.Destroy();
      else
        player.Die((player.Center - this.Position).SafeNormalize(), false, true);
    }

    public enum ShotPatterns
    {
      Single,
      Double,
      Triple,
    }
  }
}

