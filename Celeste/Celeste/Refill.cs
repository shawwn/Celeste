﻿// Decompiled with JetBrains decompiler
// Type: Celeste.Refill
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class Refill : Entity
  {
    public static ParticleType P_Shatter;
    public static ParticleType P_Regen;
    public static ParticleType P_Glow;
    private Sprite sprite;
    private Sprite flash;
    private Monocle.Image outline;
    private Wiggler wiggler;
    private BloomPoint bloom;
    private VertexLight light;
    private Level level;
    private SineWave sine;
    private bool twoDashes;
    private bool oneUse;
    private float respawnTimer;

    public Refill(Vector2 position, bool twoDashes, bool oneUse)
      : base(position)
    {
      this.Collider = (Collider) new Hitbox(16f, 16f, -8f, -8f);
      this.Add((Component) new PlayerCollider(new Action<Player>(this.OnPlayer), (Collider) null, (Collider) null));
      this.twoDashes = twoDashes;
      this.oneUse = oneUse;
      string str = !twoDashes ? "objects/refill/" : "objects/refillTwo/";
      this.Add((Component) (this.outline = new Monocle.Image(GFX.Game[str + nameof (outline)])));
      this.outline.CenterOrigin();
      this.outline.Visible = false;
      this.Add((Component) (this.sprite = new Sprite(GFX.Game, str + "idle")));
      this.sprite.AddLoop("idle", "", 0.1f);
      this.sprite.Play("idle", false, false);
      this.sprite.CenterOrigin();
      this.Add((Component) (this.flash = new Sprite(GFX.Game, str + nameof (flash))));
      this.flash.Add(nameof (flash), "", 0.05f);
      this.flash.OnFinish = (Action<string>) (anim => this.flash.Visible = false);
      this.flash.CenterOrigin();
      this.Add((Component) (this.wiggler = Wiggler.Create(1f, 4f, (Action<float>) (v => this.sprite.Scale = this.flash.Scale = Vector2.One * (float) (1.0 + (double) v * 0.200000002980232)), false, false)));
      this.Add((Component) new MirrorReflection());
      this.Add((Component) (this.bloom = new BloomPoint(0.8f, 16f)));
      this.Add((Component) (this.light = new VertexLight(Color.White, 1f, 16, 48)));
      this.Add((Component) (this.sine = new SineWave(0.6f)));
      this.sine.Randomize();
      this.UpdateY();
      this.Depth = -100;
    }

    public Refill(EntityData data, Vector2 offset)
      : this(data.Position + offset, data.Bool("twoDash", false), data.Bool(nameof (oneUse), false))
    {
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      this.level = this.SceneAs<Level>();
    }

    public override void Update()
    {
      base.Update();
      if ((double) this.respawnTimer > 0.0)
      {
        this.respawnTimer -= Engine.DeltaTime;
        if ((double) this.respawnTimer <= 0.0)
          this.Respawn();
      }
      else if (this.Scene.OnInterval(0.1f))
        this.level.ParticlesFG.Emit(Refill.P_Glow, 1, this.Position, Vector2.One * 5f);
      this.UpdateY();
      this.light.Alpha = Calc.Approach(this.light.Alpha, this.sprite.Visible ? 1f : 0.0f, 4f * Engine.DeltaTime);
      this.bloom.Alpha = this.light.Alpha * 0.8f;
      if (!this.Scene.OnInterval(2f) || !this.sprite.Visible)
        return;
      this.flash.Play("flash", true, false);
      this.flash.Visible = true;
    }

    private void Respawn()
    {
      if (this.Collidable)
        return;
      this.Collidable = true;
      this.sprite.Visible = true;
      this.outline.Visible = false;
      this.Depth = -100;
      this.wiggler.Start();
      Audio.Play("event:/game/general/diamond_return", this.Position);
      this.level.ParticlesFG.Emit(Refill.P_Regen, 16, this.Position, Vector2.One * 2f);
    }

    private void UpdateY()
    {
      this.flash.Y = this.sprite.Y = this.bloom.Y = this.sine.Value * 2f;
    }

    public override void Render()
    {
      if (this.sprite.Visible)
        this.sprite.DrawOutline(1);
      base.Render();
    }

    private void OnPlayer(Player player)
    {
      if (!player.UseRefill(this.twoDashes))
        return;
      Audio.Play("event:/game/general/diamond_touch", this.Position);
      Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
      this.Collidable = false;
      this.Add((Component) new Coroutine(this.RefillRoutine(player), true));
      this.respawnTimer = 2.5f;
    }

    private IEnumerator RefillRoutine(Player player)
    {
      Celeste.Celeste.Freeze(0.05f);
      yield return (object) null;
      this.level.Shake(0.3f);
      this.sprite.Visible = this.flash.Visible = false;
      if (!this.oneUse)
        this.outline.Visible = true;
      this.Depth = 8999;
      yield return (object) 0.05f;
      float angle = player.Speed.Angle();
      this.level.ParticlesFG.Emit(Refill.P_Shatter, 5, this.Position, Vector2.One * 4f, angle - 1.570796f);
      this.level.ParticlesFG.Emit(Refill.P_Shatter, 5, this.Position, Vector2.One * 4f, angle + 1.570796f);
      SlashFx.Burst(this.Position, angle);
      if (this.oneUse)
        this.RemoveSelf();
    }
  }
}

