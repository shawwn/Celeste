// Decompiled with JetBrains decompiler
// Type: Celeste.TriggerSpikes
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste
{
  public class TriggerSpikes : Entity
  {
    private const float RetractTime = 6f;
    private const float DelayTime = 0.4f;
    private TriggerSpikes.Directions direction;
    private Vector2 outwards;
    private Vector2 offset;
    private PlayerCollider pc;
    private Vector2 shakeOffset;
    private TriggerSpikes.SpikeInfo[] spikes;
    private List<MTexture> dustTextures;
    private List<MTexture> tentacleTextures;
    private Color[] tentacleColors;
    private int size;

    public TriggerSpikes(Vector2 position, int size, TriggerSpikes.Directions direction)
      : base(position)
    {
      this.size = size;
      this.direction = direction;
      switch (direction)
      {
        case TriggerSpikes.Directions.Up:
          this.tentacleTextures = GFX.Game.GetAtlasSubtextures("danger/triggertentacle/wiggle_v");
          this.outwards = new Vector2(0.0f, -1f);
          this.offset = new Vector2(0.0f, -1f);
          this.Collider = (Collider) new Hitbox((float) size, 4f, 0.0f, -4f);
          this.Add((Component) new SafeGroundBlocker((Collider) null));
          this.Add((Component) new LedgeBlocker(new Func<Player, bool>(this.UpSafeBlockCheck)));
          break;
        case TriggerSpikes.Directions.Down:
          this.tentacleTextures = GFX.Game.GetAtlasSubtextures("danger/triggertentacle/wiggle_v");
          this.outwards = new Vector2(0.0f, 1f);
          this.Collider = (Collider) new Hitbox((float) size, 4f, 0.0f, 0.0f);
          break;
        case TriggerSpikes.Directions.Left:
          this.tentacleTextures = GFX.Game.GetAtlasSubtextures("danger/triggertentacle/wiggle_h");
          this.outwards = new Vector2(-1f, 0.0f);
          this.Collider = (Collider) new Hitbox(4f, (float) size, -4f, 0.0f);
          this.Add((Component) new SafeGroundBlocker((Collider) null));
          this.Add((Component) new LedgeBlocker(new Func<Player, bool>(this.SideSafeBlockCheck)));
          break;
        case TriggerSpikes.Directions.Right:
          this.tentacleTextures = GFX.Game.GetAtlasSubtextures("danger/triggertentacle/wiggle_h");
          this.outwards = new Vector2(1f, 0.0f);
          this.offset = new Vector2(1f, 0.0f);
          this.Collider = (Collider) new Hitbox(4f, (float) size, 0.0f, 0.0f);
          this.Add((Component) new SafeGroundBlocker((Collider) null));
          this.Add((Component) new LedgeBlocker(new Func<Player, bool>(this.SideSafeBlockCheck)));
          break;
      }
      this.Add((Component) (this.pc = new PlayerCollider(new Action<Player>(this.OnCollide), (Collider) null, (Collider) null)));
      this.Add((Component) new StaticMover()
      {
        OnShake = new Action<Vector2>(this.OnShake),
        SolidChecker = new Func<Solid, bool>(this.IsRiding),
        JumpThruChecker = new Func<JumpThru, bool>(this.IsRiding)
      });
      this.Add((Component) new DustEdge(new Action(this.RenderSpikes)));
      this.Depth = -50;
    }

    public TriggerSpikes(EntityData data, Vector2 offset, TriggerSpikes.Directions dir)
      : this(data.Position + offset, TriggerSpikes.GetSize(data, dir), dir)
    {
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      Vector3[] edgeColors = DustStyles.Get(scene).EdgeColors;
      this.dustTextures = GFX.Game.GetAtlasSubtextures("danger/dustcreature/base");
      this.tentacleColors = new Color[edgeColors.Length];
      for (int index = 0; index < this.tentacleColors.Length; ++index)
        this.tentacleColors[index] = Color.Lerp(new Color(edgeColors[index]), Color.DarkSlateBlue, 0.4f);
      Vector2 vector2 = new Vector2(Math.Abs(this.outwards.Y), Math.Abs(this.outwards.X));
      this.spikes = new TriggerSpikes.SpikeInfo[this.size / 4];
      for (int index = 0; index < this.spikes.Length; ++index)
      {
        this.spikes[index].Parent = this;
        this.spikes[index].Index = index;
        this.spikes[index].WorldPosition = this.Position + vector2 * (float) (2 + index * 4);
        this.spikes[index].ParticleTimerOffset = Calc.Random.NextFloat(0.25f);
        this.spikes[index].TextureIndex = Calc.Random.Next(this.dustTextures.Count);
        this.spikes[index].DustOutDistance = Calc.Random.Choose<int>(3, 4, 6);
        this.spikes[index].TentacleColor = Calc.Random.Next(this.tentacleColors.Length);
        this.spikes[index].TentacleFrame = Calc.Random.NextFloat((float) this.tentacleTextures.Count);
      }
    }

    private void OnShake(Vector2 amount)
    {
      this.shakeOffset += amount;
    }

    private bool UpSafeBlockCheck(Player player)
    {
      int num1 = 8 * (int) player.Facing;
      int val1_1 = (int) (((double) player.Left + (double) num1 - (double) this.Left) / 4.0);
      int val1_2 = (int) (((double) player.Right + (double) num1 - (double) this.Left) / 4.0);
      if (val1_2 < 0 || val1_1 >= this.spikes.Length)
        return false;
      int num2 = Math.Max(val1_1, 0);
      int num3 = Math.Min(val1_2, this.spikes.Length - 1);
      for (int index = num2; index <= num3; ++index)
      {
        if ((double) this.spikes[index].Lerp >= 1.0)
          return true;
      }
      return false;
    }

    private bool SideSafeBlockCheck(Player player)
    {
      int val1_1 = (int) (((double) player.Top - (double) this.Top) / 4.0);
      int val1_2 = (int) (((double) player.Bottom - (double) this.Top) / 4.0);
      if (val1_2 < 0 || val1_1 >= this.spikes.Length)
        return false;
      int num1 = Math.Max(val1_1, 0);
      int num2 = Math.Min(val1_2, this.spikes.Length - 1);
      for (int index = num1; index <= num2; ++index)
      {
        if ((double) this.spikes[index].Lerp >= 1.0)
          return true;
      }
      return false;
    }

    private void OnCollide(Player player)
    {
      int minIndex;
      int maxIndex;
      this.GetPlayerCollideIndex(player, out minIndex, out maxIndex);
      if (maxIndex < 0 || minIndex >= this.spikes.Length)
        return;
      int num1 = Math.Max(minIndex, 0);
      int num2 = Math.Min(maxIndex, this.spikes.Length - 1);
      int index = num1;
      while (index <= num2 && !this.spikes[index].OnPlayer(player, this.outwards))
        ++index;
    }

    private void GetPlayerCollideIndex(Player player, out int minIndex, out int maxIndex)
    {
      minIndex = maxIndex = -1;
      switch (this.direction)
      {
        case TriggerSpikes.Directions.Up:
          if ((double) player.Speed.Y < 0.0)
            break;
          minIndex = (int) (((double) player.Left - (double) this.Left) / 4.0);
          maxIndex = (int) (((double) player.Right - (double) this.Left) / 4.0);
          break;
        case TriggerSpikes.Directions.Down:
          if ((double) player.Speed.Y > 0.0)
            break;
          minIndex = (int) (((double) player.Left - (double) this.Left) / 4.0);
          maxIndex = (int) (((double) player.Right - (double) this.Left) / 4.0);
          break;
        case TriggerSpikes.Directions.Left:
          if ((double) player.Speed.X < 0.0)
            break;
          minIndex = (int) (((double) player.Top - (double) this.Top) / 4.0);
          maxIndex = (int) (((double) player.Bottom - (double) this.Top) / 4.0);
          break;
        case TriggerSpikes.Directions.Right:
          if ((double) player.Speed.X > 0.0)
            break;
          minIndex = (int) (((double) player.Top - (double) this.Top) / 4.0);
          maxIndex = (int) (((double) player.Bottom - (double) this.Top) / 4.0);
          break;
      }
    }

    private bool PlayerCheck(int spikeIndex)
    {
      Player player = this.CollideFirst<Player>();
      if (player == null)
        return false;
      int minIndex;
      int maxIndex;
      this.GetPlayerCollideIndex(player, out minIndex, out maxIndex);
      return minIndex <= spikeIndex + 1 && maxIndex >= spikeIndex - 1;
    }

    private static int GetSize(EntityData data, TriggerSpikes.Directions dir)
    {
      switch (dir)
      {
        case TriggerSpikes.Directions.Up:
        case TriggerSpikes.Directions.Down:
          return data.Width;
        default:
          return data.Height;
      }
    }

    public override void Update()
    {
      base.Update();
      for (int index = 0; index < this.spikes.Length; ++index)
        this.spikes[index].Update();
    }

    public override void Render()
    {
      base.Render();
      Vector2 vector2 = new Vector2(Math.Abs(this.outwards.Y), Math.Abs(this.outwards.X));
      int count = this.tentacleTextures.Count;
      Vector2 one = Vector2.One;
      Vector2 justify = new Vector2(0.0f, 0.5f);
      if (this.direction == TriggerSpikes.Directions.Left)
        one.X = -1f;
      else if (this.direction == TriggerSpikes.Directions.Up)
        one.Y = -1f;
      if (this.direction == TriggerSpikes.Directions.Up || this.direction == TriggerSpikes.Directions.Down)
        justify = new Vector2(0.5f, 0.0f);
      for (int index = 0; index < this.spikes.Length; ++index)
      {
        if (!this.spikes[index].Triggered)
        {
          MTexture tentacleTexture = this.tentacleTextures[(int) ((double) this.spikes[index].TentacleFrame % (double) count)];
          Vector2 position = this.Position + vector2 * (float) (2 + index * 4);
          tentacleTexture.DrawJustified(position + vector2, justify, Color.Black, one, 0.0f);
          tentacleTexture.DrawJustified(position, justify, this.tentacleColors[this.spikes[index].TentacleColor], one, 0.0f);
        }
      }
      this.RenderSpikes();
    }

    private void RenderSpikes()
    {
      Vector2 vector2 = new Vector2(Math.Abs(this.outwards.Y), Math.Abs(this.outwards.X));
      for (int index = 0; index < this.spikes.Length; ++index)
      {
        if (this.spikes[index].Triggered)
          this.dustTextures[this.spikes[index].TextureIndex].DrawCentered(this.Position + this.outwards * (float) ((double) this.spikes[index].Lerp * (double) this.spikes[index].DustOutDistance - 4.0) + vector2 * (float) (2 + index * 4), Color.White, 0.5f * this.spikes[index].Lerp, this.spikes[index].TextureRotation);
      }
    }

    private bool IsRiding(Solid solid)
    {
      switch (this.direction)
      {
        case TriggerSpikes.Directions.Up:
          return this.CollideCheckOutside((Entity) solid, this.Position + Vector2.UnitY);
        case TriggerSpikes.Directions.Down:
          return this.CollideCheckOutside((Entity) solid, this.Position - Vector2.UnitY);
        case TriggerSpikes.Directions.Left:
          return this.CollideCheckOutside((Entity) solid, this.Position + Vector2.UnitX);
        case TriggerSpikes.Directions.Right:
          return this.CollideCheckOutside((Entity) solid, this.Position - Vector2.UnitX);
        default:
          return false;
      }
    }

    private bool IsRiding(JumpThru jumpThru)
    {
      if (this.direction != TriggerSpikes.Directions.Up)
        return false;
      return this.CollideCheck((Entity) jumpThru, this.Position + Vector2.UnitY);
    }

    public enum Directions
    {
      Up,
      Down,
      Left,
      Right,
    }

    private struct SpikeInfo
    {
      public TriggerSpikes Parent;
      public int Index;
      public Vector2 WorldPosition;
      public bool Triggered;
      public float RetractTimer;
      public float DelayTimer;
      public float Lerp;
      public float ParticleTimerOffset;
      public int TextureIndex;
      public float TextureRotation;
      public int DustOutDistance;
      public int TentacleColor;
      public float TentacleFrame;

      public void Update()
      {
        if (this.Triggered)
        {
          if ((double) this.DelayTimer > 0.0)
          {
            this.DelayTimer -= Engine.DeltaTime;
            if ((double) this.DelayTimer <= 0.0)
            {
              if (this.PlayerCheck())
                this.DelayTimer = 0.05f;
              else
                Audio.Play("event:/game/03_resort/fluff_tendril_emerge", this.WorldPosition);
            }
          }
          else
            this.Lerp = Calc.Approach(this.Lerp, 1f, 8f * Engine.DeltaTime);
          this.TextureRotation += Engine.DeltaTime * 1.2f;
        }
        else
        {
          this.Lerp = Calc.Approach(this.Lerp, 0.0f, 4f * Engine.DeltaTime);
          this.TentacleFrame += Engine.DeltaTime * 12f;
          if ((double) this.Lerp <= 0.0)
            this.Triggered = false;
        }
      }

      public bool PlayerCheck()
      {
        return this.Parent.PlayerCheck(this.Index);
      }

      public bool OnPlayer(Player player, Vector2 outwards)
      {
        if (!this.Triggered)
        {
          Audio.Play("event:/game/03_resort/fluff_tendril_touch", this.WorldPosition);
          this.Triggered = true;
          this.DelayTimer = 0.4f;
          this.RetractTimer = 6f;
        }
        else if ((double) this.Lerp >= 1.0)
        {
          player.Die(outwards, false, true);
          return true;
        }
        return false;
      }
    }
  }
}

