// Decompiled with JetBrains decompiler
// Type: Celeste.ClutterBlock
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste
{
  public class ClutterBlock : Entity
  {
    public HashSet<ClutterBlock> HasBelow = new HashSet<ClutterBlock>();
    public List<ClutterBlock> Below = new List<ClutterBlock>();
    public List<ClutterBlock> Above = new List<ClutterBlock>();
    private float floatTarget = 0.0f;
    private float floatDelay = 0.0f;
    private float floatTimer = 0.0f;
    public ClutterBlock.Colors BlockColor;
    public Monocle.Image Image;
    public bool OnTheGround;
    public bool TopSideOpen;
    public bool LeftSideOpen;
    public bool RightSideOpen;

    public ClutterBlock(Vector2 position, MTexture texture, ClutterBlock.Colors color)
      : base(position)
    {
      this.BlockColor = color;
      this.Add((Component) (this.Image = new Monocle.Image(texture)));
      this.Collider = (Collider) new Hitbox((float) texture.Width, (float) texture.Height, 0.0f, 0.0f);
      this.Depth = -9998;
    }

    public void WeightDown()
    {
      foreach (ClutterBlock clutterBlock in this.Below)
        clutterBlock.WeightDown();
      this.floatTarget = 0.0f;
      this.floatDelay = 0.1f;
    }

    public override void Update()
    {
      base.Update();
      if (this.OnTheGround)
        return;
      if ((double) this.floatDelay <= 0.0)
      {
        Player entity = this.Scene.Tracker.GetEntity<Player>();
        if (entity != null && ((((!this.TopSideOpen ? (false ? 1 : 0) : ((double) entity.Right <= (double) this.Left || (double) entity.Left >= (double) this.Right || (double) entity.Bottom < (double) this.Top - 1.0 ? (false ? 1 : 0) : ((double) entity.Bottom <= (double) this.Top + 4.0 ? 1 : 0))) | (entity.StateMachine.State != 1 || !this.LeftSideOpen || ((double) entity.Right < (double) this.Left - 1.0 || (double) entity.Right >= (double) this.Left + 4.0) || (double) entity.Bottom <= (double) this.Top ? 0 : ((double) entity.Top < (double) this.Bottom ? 1 : 0))) != 0 ? 1 : 0) | (entity.StateMachine.State != 1 || !this.RightSideOpen || ((double) entity.Left > (double) this.Right + 1.0 || (double) entity.Left <= (double) this.Right - 4.0) || (double) entity.Bottom <= (double) this.Top ? 0 : ((double) entity.Top < (double) this.Bottom ? 1 : 0))) != 0)
          this.WeightDown();
      }
      this.floatTimer += Engine.DeltaTime;
      this.floatDelay -= Engine.DeltaTime;
      if ((double) this.floatDelay <= 0.0)
        this.floatTarget = Calc.Approach(this.floatTarget, this.WaveTarget, Engine.DeltaTime * 4f);
      this.Image.Y = this.floatTarget;
    }

    private float WaveTarget
    {
      get
      {
        return (float) (-((Math.Sin((double) ((int) this.Position.X / 16) * 0.25 + (double) this.floatTimer * 2.0) + 1.0) / 2.0) - 1.0);
      }
    }

    public void Absorb(ClutterAbsorbEffect effect)
    {
      effect.FlyClutter(this.Position + new Vector2(this.Image.Width * 0.5f, this.Image.Height * 0.5f + this.floatTarget), this.Image.Texture, true, Calc.Random.NextFloat(0.5f));
      this.Scene.Remove((Entity) this);
    }

    public enum Colors
    {
      Red,
      Green,
      Yellow,
      Lightning,
    }
  }
}

