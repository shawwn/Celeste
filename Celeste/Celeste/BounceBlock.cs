// Decompiled with JetBrains decompiler
// Type: Celeste.BounceBlock
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste
{
  public class BounceBlock : Solid
  {
    private bool reformed = true;
    public static ParticleType P_Reform;
    public static ParticleType P_FireBreak;
    public static ParticleType P_IceBreak;
    private const float WindUpDelay = 0.0f;
    private const float WindUpDist = 10f;
    private const float IceWindUpDist = 16f;
    private const float BounceDist = 24f;
    private const float LiftSpeedXMult = 0.75f;
    private const float RespawnTime = 1.6f;
    private const float WallPushTime = 0.1f;
    private const float BounceEndTime = 0.05f;
    private Vector2 bounceDir;
    private BounceBlock.States state;
    private Vector2 startPos;
    private float moveSpeed;
    private float windUpStartTimer;
    private float windUpProgress;
    private bool iceMode;
    private bool iceModeNext;
    private float respawnTimer;
    private float bounceEndTimer;
    private Vector2 bounceLift;
    private float reappearFlash;
    private Vector2 debrisDirection;
    private List<Monocle.Image> hotImages;
    private List<Monocle.Image> coldImages;
    private Sprite hotCenterSprite;
    private Sprite coldCenterSprite;

    public BounceBlock(Vector2 position, float width, float height)
      : base(position, width, height, false)
    {
      this.state = BounceBlock.States.Waiting;
      this.startPos = this.Position;
      this.hotImages = this.BuildSprite(GFX.Game["objects/bumpblocknew/fire00"]);
      this.hotCenterSprite = GFX.SpriteBank.Create("bumpBlockCenterFire");
      this.hotCenterSprite.Position = new Vector2(this.Width, this.Height) / 2f;
      this.hotCenterSprite.Visible = false;
      this.Add((Component) this.hotCenterSprite);
      this.coldImages = this.BuildSprite(GFX.Game["objects/bumpblocknew/ice00"]);
      this.coldCenterSprite = GFX.SpriteBank.Create("bumpBlockCenterIce");
      this.coldCenterSprite.Position = new Vector2(this.Width, this.Height) / 2f;
      this.coldCenterSprite.Visible = false;
      this.Add((Component) this.coldCenterSprite);
      this.Add((Component) new CoreModeListener(new Action<Session.CoreModes>(this.OnChangeMode)));
    }

    public BounceBlock(EntityData data, Vector2 offset)
      : this(data.Position + offset, (float) data.Width, (float) data.Height)
    {
    }

    private List<Monocle.Image> BuildSprite(MTexture source)
    {
      List<Monocle.Image> imageList = new List<Monocle.Image>();
      int num1 = source.Width / 8;
      int num2 = source.Height / 8;
      for (int index1 = 0; (double) index1 < (double) this.Width; index1 += 8)
      {
        for (int index2 = 0; (double) index2 < (double) this.Height; index2 += 8)
        {
          int num3 = index1 != 0 ? ((double) index1 < (double) this.Width - 8.0 ? Calc.Random.Next(1, num1 - 1) : num1 - 1) : 0;
          int num4 = index2 != 0 ? ((double) index2 < (double) this.Height - 8.0 ? Calc.Random.Next(1, num2 - 1) : num2 - 1) : 0;
          Monocle.Image image = new Monocle.Image(source.GetSubtexture(num3 * 8, num4 * 8, 8, 8, (MTexture) null));
          image.Position = new Vector2((float) index1, (float) index2);
          imageList.Add(image);
          this.Add((Component) image);
        }
      }
      return imageList;
    }

    private void ToggleSprite()
    {
      this.hotCenterSprite.Visible = !this.iceMode;
      this.coldCenterSprite.Visible = this.iceMode;
      foreach (Component hotImage in this.hotImages)
        hotImage.Visible = !this.iceMode;
      foreach (Component coldImage in this.coldImages)
        coldImage.Visible = this.iceMode;
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      this.iceModeNext = this.iceMode = this.SceneAs<Level>().CoreMode == Session.CoreModes.Cold;
      this.ToggleSprite();
    }

    private void OnChangeMode(Session.CoreModes coreMode)
    {
      this.iceModeNext = coreMode == Session.CoreModes.Cold;
    }

    private void CheckModeChange()
    {
      if (this.iceModeNext == this.iceMode)
        return;
      this.iceMode = this.iceModeNext;
      this.ToggleSprite();
    }

    public override void Render()
    {
      Vector2 position = this.Position;
      this.Position = this.Position + this.Shake;
      if (this.state != BounceBlock.States.Broken && this.reformed)
        base.Render();
      if ((double) this.reappearFlash > 0.0)
      {
        float num1 = Ease.CubeOut(this.reappearFlash);
        float num2 = num1 * 2f;
        Draw.Rect(this.X - num2, this.Y - num2, this.Width + num2 * 2f, this.Height + num2 * 2f, Color.White * num1);
      }
      this.Position = position;
    }

    public override void Update()
    {
      base.Update();
      this.reappearFlash = Calc.Approach(this.reappearFlash, 0.0f, Engine.DeltaTime * 8f);
      if (this.state == BounceBlock.States.Waiting)
      {
        this.CheckModeChange();
        this.moveSpeed = Calc.Approach(this.moveSpeed, 100f, 400f * Engine.DeltaTime);
        Vector2 position = Calc.Approach(this.ExactPosition, this.startPos, this.moveSpeed * Engine.DeltaTime);
        Vector2 liftSpeed = (position - this.ExactPosition).SafeNormalize(this.moveSpeed);
        liftSpeed.X *= 0.75f;
        this.MoveTo(position, liftSpeed);
        this.windUpProgress = Calc.Approach(this.windUpProgress, 0.0f, 1f * Engine.DeltaTime);
        Player player = this.WindUpPlayerCheck();
        if (player == null)
          return;
        this.moveSpeed = 80f;
        this.windUpStartTimer = 0.0f;
        this.bounceDir = !this.iceMode ? (player.Center - this.Center).SafeNormalize() : -Vector2.UnitY;
        this.state = BounceBlock.States.WindingUp;
        Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
        if (this.iceMode)
        {
          this.StartShaking(0.2f);
          Audio.Play("event:/game/09_core/iceblock_touch", this.Center);
        }
        else
          Audio.Play("event:/game/09_core/bounceblock_touch", this.Center);
      }
      else if (this.state == BounceBlock.States.WindingUp)
      {
        Player player = this.WindUpPlayerCheck();
        if (player != null)
          this.bounceDir = !this.iceMode ? (player.Center - this.Center).SafeNormalize() : -Vector2.UnitY;
        if ((double) this.windUpStartTimer > 0.0)
        {
          this.windUpStartTimer -= Engine.DeltaTime;
          this.windUpProgress = Calc.Approach(this.windUpProgress, 0.0f, 1f * Engine.DeltaTime);
        }
        else
        {
          this.moveSpeed = Calc.Approach(this.moveSpeed, this.iceMode ? 35f : 40f, 600f * Engine.DeltaTime);
          float num = this.iceMode ? 0.333f : 1f;
          Vector2 target = this.startPos - this.bounceDir * (this.iceMode ? 16f : 10f);
          Vector2 position = Calc.Approach(this.ExactPosition, target, this.moveSpeed * num * Engine.DeltaTime);
          Vector2 liftSpeed = (position - this.ExactPosition).SafeNormalize(this.moveSpeed * num);
          liftSpeed.X *= 0.75f;
          this.MoveTo(position, liftSpeed);
          this.windUpProgress = Calc.ClampedMap(Vector2.Distance(this.ExactPosition, target), 16f, 2f, 0.0f, 1f);
          if (this.iceMode && (double) Vector2.DistanceSquared(this.ExactPosition, target) <= 12.0)
            this.StartShaking(0.1f);
          else if (!this.iceMode && (double) this.windUpProgress >= 0.5)
            this.StartShaking(0.1f);
          if ((double) Vector2.DistanceSquared(this.ExactPosition, target) <= 2.0)
          {
            if (this.iceMode)
              this.Break();
            else
              this.state = BounceBlock.States.Bouncing;
            this.moveSpeed = 0.0f;
          }
        }
      }
      else if (this.state == BounceBlock.States.Bouncing)
      {
        this.moveSpeed = Calc.Approach(this.moveSpeed, 140f, 800f * Engine.DeltaTime);
        Vector2 target = this.startPos + this.bounceDir * 24f;
        Vector2 position = Calc.Approach(this.ExactPosition, target, this.moveSpeed * Engine.DeltaTime);
        this.bounceLift = (position - this.ExactPosition).SafeNormalize(Math.Min(this.moveSpeed * 3f, 200f));
        this.bounceLift.X *= 0.75f;
        this.MoveTo(position, this.bounceLift);
        this.windUpProgress = 1f;
        if (!(this.ExactPosition == target) && (this.iceMode || this.WindUpPlayerCheck() != null))
          return;
        this.debrisDirection = (target - this.startPos).SafeNormalize();
        this.state = BounceBlock.States.BounceEnd;
        Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
        this.moveSpeed = 0.0f;
        this.bounceEndTimer = 0.05f;
        this.ShakeOffPlayer(this.bounceLift);
      }
      else if (this.state == BounceBlock.States.BounceEnd)
      {
        this.bounceEndTimer -= Engine.DeltaTime;
        if ((double) this.bounceEndTimer > 0.0)
          return;
        this.Break();
      }
      else
      {
        if (this.state != BounceBlock.States.Broken)
          return;
        this.Depth = 8990;
        this.reformed = false;
        if ((double) this.respawnTimer > 0.0)
        {
          this.respawnTimer -= Engine.DeltaTime;
        }
        else
        {
          Vector2 position = this.Position;
          this.Position = this.startPos;
          if (!this.CollideCheck<Actor>() && !this.CollideCheck<Solid>())
          {
            this.CheckModeChange();
            Audio.Play(this.iceMode ? "event:/game/09_core/iceblock_reappear" : "event:/game/09_core/bounceblock_reappear", this.Center);
            float duration = 0.35f;
            for (int index1 = 0; (double) index1 < (double) this.Width; index1 += 8)
            {
              for (int index2 = 0; (double) index2 < (double) this.Height; index2 += 8)
              {
                Vector2 to = new Vector2((float) ((double) this.X + (double) index1 + 4.0), (float) ((double) this.Y + (double) index2 + 4.0));
                this.Scene.Add((Entity) Engine.Pooler.Create<BounceBlock.RespawnDebris>().Init(to + (to - this.Center).SafeNormalize() * 12f, to, this.iceMode, duration));
              }
            }
            Alarm.Set((Entity) this, duration, (Action) (() =>
            {
              this.reformed = true;
              this.reappearFlash = 0.6f;
              this.EnableStaticMovers();
              this.ReformParticles();
            }), Alarm.AlarmMode.Oneshot);
            this.Depth = -9000;
            this.MoveStaticMovers(this.Position - position);
            this.Collidable = true;
            this.state = BounceBlock.States.Waiting;
          }
          else
            this.Position = position;
        }
      }
    }

    private void ReformParticles()
    {
      Level level = this.SceneAs<Level>();
      for (int index = 0; (double) index < (double) this.Width; index += 4)
      {
        level.Particles.Emit(BounceBlock.P_Reform, new Vector2(this.X + 2f + (float) index + (float) Calc.Random.Range(-1, 1), this.Y), -1.570796f);
        level.Particles.Emit(BounceBlock.P_Reform, new Vector2(this.X + 2f + (float) index + (float) Calc.Random.Range(-1, 1), this.Bottom - 1f), 1.570796f);
      }
      for (int index = 0; (double) index < (double) this.Height; index += 4)
      {
        level.Particles.Emit(BounceBlock.P_Reform, new Vector2(this.X, this.Y + 2f + (float) index + (float) Calc.Random.Range(-1, 1)), 3.141593f);
        level.Particles.Emit(BounceBlock.P_Reform, new Vector2(this.Right - 1f, this.Y + 2f + (float) index + (float) Calc.Random.Range(-1, 1)), 0.0f);
      }
    }

    private Player WindUpPlayerCheck()
    {
      Player player = this.CollideFirst<Player>(this.Position - Vector2.UnitY);
      if (player != null && (double) player.Speed.Y < 0.0)
        player = (Player) null;
      if (player == null)
      {
        player = this.CollideFirst<Player>(this.Position + Vector2.UnitX);
        if (player == null || player.StateMachine.State != 1 || player.Facing != Facings.Left)
        {
          player = this.CollideFirst<Player>(this.Position - Vector2.UnitX);
          if (player == null || player.StateMachine.State != 1 || player.Facing != Facings.Right)
            player = (Player) null;
        }
      }
      return player;
    }

    private void ShakeOffPlayer(Vector2 liftSpeed)
    {
      Player player = this.WindUpPlayerCheck();
      if (player == null)
        return;
      player.StateMachine.State = Player.StNormal;
      player.Speed = liftSpeed;
      player.StartJumpGraceTime();
    }

    private void Break()
    {
      if (!this.iceMode)
        Audio.Play("event:/game/09_core/bounceblock_break", this.Center);
      Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
      this.state = BounceBlock.States.Broken;
      this.Collidable = false;
      this.DisableStaticMovers();
      this.respawnTimer = 1.6f;
      Vector2 direction1 = new Vector2(0.0f, 1f);
      if (!this.iceMode)
        direction1 = this.debrisDirection;
      Vector2 center = this.Center;
      for (int index1 = 0; (double) index1 < (double) this.Width; index1 += 8)
      {
        for (int index2 = 0; (double) index2 < (double) this.Height; index2 += 8)
        {
          if (this.iceMode)
            direction1 = (new Vector2((float) ((double) this.X + (double) index1 + 4.0), (float) ((double) this.Y + (double) index2 + 4.0)) - center).SafeNormalize();
          this.Scene.Add((Entity) Engine.Pooler.Create<BounceBlock.BreakDebris>().Init(new Vector2((float) ((double) this.X + (double) index1 + 4.0), (float) ((double) this.Y + (double) index2 + 4.0)), direction1, this.iceMode));
        }
      }
      float num = this.debrisDirection.Angle();
      Level level = this.SceneAs<Level>();
      for (int index1 = 0; (double) index1 < (double) this.Width; index1 += 4)
      {
        for (int index2 = 0; (double) index2 < (double) this.Height; index2 += 4)
        {
          Vector2 position = this.Position + new Vector2((float) (2 + index1), (float) (2 + index2)) + Calc.Random.Range(-Vector2.One, Vector2.One);
          float direction2 = this.iceMode ? (position - center).Angle() : num;
          level.Particles.Emit(this.iceMode ? BounceBlock.P_IceBreak : BounceBlock.P_FireBreak, position, direction2);
        }
      }
    }

    private enum States
    {
      Waiting,
      WindingUp,
      Bouncing,
      BounceEnd,
      Broken,
    }

    [Pooled]
    private class RespawnDebris : Entity
    {
      private Monocle.Image sprite;
      private Vector2 from;
      private Vector2 to;
      private float percent;
      private float duration;

      public BounceBlock.RespawnDebris Init(
        Vector2 from,
        Vector2 to,
        bool ice,
        float duration)
      {
        List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures(ice ? "objects/bumpblocknew/ice_rubble" : "objects/bumpblocknew/fire_rubble");
        MTexture texture = Calc.Random.Choose<MTexture>(atlasSubtextures);
        if (this.sprite == null)
        {
          this.Add((Component) (this.sprite = new Monocle.Image(texture)));
          this.sprite.CenterOrigin();
        }
        else
          this.sprite.Texture = texture;
        this.Position = this.from = from;
        this.percent = 0.0f;
        this.to = to;
        this.duration = duration;
        return this;
      }

      public override void Update()
      {
        if ((double) this.percent > 1.0)
        {
          this.RemoveSelf();
        }
        else
        {
          this.percent += Engine.DeltaTime / this.duration;
          this.Position = Vector2.Lerp(this.from, this.to, Ease.CubeIn(this.percent));
          this.sprite.Color = Color.White * this.percent;
        }
      }

      public override void Render()
      {
        this.sprite.DrawOutline(Color.Black, 1);
        base.Render();
      }
    }

    [Pooled]
    private class BreakDebris : Entity
    {
      private Monocle.Image sprite;
      private Vector2 speed;
      private float percent;
      private float duration;

      public BounceBlock.BreakDebris Init(Vector2 position, Vector2 direction, bool ice)
      {
        List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures(ice ? "objects/bumpblocknew/ice_rubble" : "objects/bumpblocknew/fire_rubble");
        MTexture texture = Calc.Random.Choose<MTexture>(atlasSubtextures);
        if (this.sprite == null)
        {
          this.Add((Component) (this.sprite = new Monocle.Image(texture)));
          this.sprite.CenterOrigin();
        }
        else
          this.sprite.Texture = texture;
        this.Position = position;
        direction = Calc.AngleToVector(direction.Angle() + Calc.Random.Range(-0.1f, 0.1f), 1f);
        this.speed = direction * (ice ? (float) Calc.Random.Range(20, 40) : (float) Calc.Random.Range(120, 200));
        this.percent = 0.0f;
        this.duration = (float) Calc.Random.Range(2, 3);
        return this;
      }

      public override void Update()
      {
        base.Update();
        if ((double) this.percent >= 1.0)
        {
          this.RemoveSelf();
        }
        else
        {
          this.Position = this.Position + this.speed * Engine.DeltaTime;
          this.speed.X = Calc.Approach(this.speed.X, 0.0f, 180f * Engine.DeltaTime);
          this.speed.Y += 200f * Engine.DeltaTime;
          this.percent += Engine.DeltaTime / this.duration;
          this.sprite.Color = Color.White * (1f - this.percent);
        }
      }

      public override void Render()
      {
        this.sprite.DrawOutline(Color.Black, 1);
        base.Render();
      }
    }
  }
}

