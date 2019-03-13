// Decompiled with JetBrains decompiler
// Type: Celeste.CoreModeToggle
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  public class CoreModeToggle : Entity
  {
    private const float Cooldown = 1f;
    private bool iceMode;
    private float cooldownTimer;
    private bool onlyFire;
    private bool onlyIce;
    private bool persistent;
    private bool playSounds;
    private Sprite sprite;

    public CoreModeToggle(Vector2 position, bool onlyFire, bool onlyIce, bool persistent)
      : base(position)
    {
      this.onlyFire = onlyFire;
      this.onlyIce = onlyIce;
      this.persistent = persistent;
      this.Collider = (Collider) new Hitbox(16f, 24f, -8f, -12f);
      this.Add((Component) new CoreModeListener(new Action<Session.CoreModes>(this.OnChangeMode)));
      this.Add((Component) new PlayerCollider(new Action<Player>(this.OnPlayer), (Collider) null, (Collider) null));
      this.Add((Component) (this.sprite = GFX.SpriteBank.Create("coreFlipSwitch")));
      this.Depth = 2000;
    }

    public CoreModeToggle(EntityData data, Vector2 offset)
      : this(data.Position + offset, data.Bool(nameof (onlyFire), false), data.Bool(nameof (onlyIce), false), data.Bool(nameof (persistent), false))
    {
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      this.iceMode = this.SceneAs<Level>().CoreMode == Session.CoreModes.Cold;
      this.SetSprite(false);
    }

    private void OnChangeMode(Session.CoreModes mode)
    {
      this.iceMode = mode == Session.CoreModes.Cold;
      this.SetSprite(true);
    }

    private void SetSprite(bool animate)
    {
      if (animate)
      {
        if (this.playSounds)
          Audio.Play(this.iceMode ? "event:/game/09_core/switch_to_cold" : "event:/game/09_core/switch_to_hot", this.Position);
        if (this.Usable)
        {
          this.sprite.Play(this.iceMode ? "ice" : "hot", false, false);
        }
        else
        {
          if (this.playSounds)
            Audio.Play("event:/game/09_core/switch_dies", this.Position);
          this.sprite.Play(this.iceMode ? "iceOff" : "hotOff", false, false);
        }
      }
      else if (this.Usable)
        this.sprite.Play(this.iceMode ? "iceLoop" : "hotLoop", false, false);
      else
        this.sprite.Play(this.iceMode ? "iceOffLoop" : "hotOffLoop", false, false);
      this.playSounds = false;
    }

    private void OnPlayer(Player player)
    {
      if (!this.Usable || (double) this.cooldownTimer > 0.0)
        return;
      this.playSounds = true;
      Level level = this.SceneAs<Level>();
      level.CoreMode = level.CoreMode != Session.CoreModes.Cold ? Session.CoreModes.Cold : Session.CoreModes.Hot;
      if (this.persistent)
        level.Session.CoreMode = level.CoreMode;
      Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
      level.Flash(Color.White * 0.15f, true);
      Celeste.Freeze(0.05f);
      this.cooldownTimer = 1f;
    }

    public override void Update()
    {
      base.Update();
      if ((double) this.cooldownTimer <= 0.0)
        return;
      this.cooldownTimer -= Engine.DeltaTime;
    }

    private bool Usable
    {
      get
      {
        return (!this.onlyFire || this.iceMode) && (!this.onlyIce || !this.iceMode);
      }
    }
  }
}

