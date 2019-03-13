// Decompiled with JetBrains decompiler
// Type: Celeste.Trigger
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  [Tracked(true)]
  public abstract class Trigger : Entity
  {
    public bool Triggered;

    public bool PlayerIsInside { get; private set; }

    public Trigger(EntityData data, Vector2 offset)
      : base(data.Position + offset)
    {
      this.Collider = (Collider) new Hitbox((float) data.Width, (float) data.Height, 0.0f, 0.0f);
      this.Visible = false;
    }

    public virtual void OnEnter(Player player)
    {
      this.PlayerIsInside = true;
    }

    public virtual void OnStay(Player player)
    {
    }

    public virtual void OnLeave(Player player)
    {
      this.PlayerIsInside = false;
    }

    protected float GetPositionLerp(Player player, Trigger.PositionModes mode)
    {
      switch (mode)
      {
        case Trigger.PositionModes.HorizontalCenter:
          return Math.Min(Calc.ClampedMap(player.CenterX, this.Left, this.CenterX, 0.0f, 1f), Calc.ClampedMap(player.CenterX, this.Right, this.CenterX, 0.0f, 1f));
        case Trigger.PositionModes.VerticalCenter:
          return Math.Min(Calc.ClampedMap(player.CenterY, this.Top, this.CenterY, 0.0f, 1f), Calc.ClampedMap(player.CenterY, this.Bottom, this.CenterY, 0.0f, 1f));
        case Trigger.PositionModes.TopToBottom:
          return Calc.ClampedMap(player.CenterY, this.Top, this.Bottom, 0.0f, 1f);
        case Trigger.PositionModes.BottomToTop:
          return Calc.ClampedMap(player.CenterY, this.Bottom, this.Top, 0.0f, 1f);
        case Trigger.PositionModes.LeftToRight:
          return Calc.ClampedMap(player.CenterX, this.Left, this.Right, 0.0f, 1f);
        case Trigger.PositionModes.RightToLeft:
          return Calc.ClampedMap(player.CenterX, this.Right, this.Left, 0.0f, 1f);
        default:
          return 1f;
      }
    }

    public enum PositionModes
    {
      NoEffect,
      HorizontalCenter,
      VerticalCenter,
      TopToBottom,
      BottomToTop,
      LeftToRight,
      RightToLeft,
    }
  }
}

