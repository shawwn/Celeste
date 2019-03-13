﻿// Decompiled with JetBrains decompiler
// Type: Celeste.Slider
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  public class Slider : Entity
  {
    private const float MaxSpeed = 80f;
    private const float Accel = 400f;
    private Vector2 dir;
    private Vector2 surface;
    private bool foundSurfaceAfterCorner;
    private bool gotOutOfWall;
    private float speed;
    private bool moving;

    public Slider(Vector2 position, bool clockwise, Slider.Surfaces surface)
      : base(position)
    {
      this.Collider = (Collider) new Monocle.Circle(10f, 0.0f, 0.0f);
      this.Add((Component) new StaticMover());
      switch (surface)
      {
        case Slider.Surfaces.Ceiling:
          this.dir = Vector2.op_UnaryNegation(Vector2.get_UnitX());
          this.surface = Vector2.op_UnaryNegation(Vector2.get_UnitY());
          break;
        case Slider.Surfaces.LeftWall:
          this.dir = Vector2.op_UnaryNegation(Vector2.get_UnitY());
          this.surface = Vector2.op_UnaryNegation(Vector2.get_UnitX());
          break;
        case Slider.Surfaces.RightWall:
          this.dir = Vector2.get_UnitY();
          this.surface = Vector2.get_UnitX();
          break;
        default:
          this.dir = Vector2.get_UnitX();
          this.surface = Vector2.get_UnitY();
          break;
      }
      if (!clockwise)
        this.dir = Vector2.op_Multiply(this.dir, -1f);
      this.moving = true;
      this.foundSurfaceAfterCorner = this.gotOutOfWall = true;
      this.speed = 80f;
      this.Add((Component) new PlayerCollider(new Action<Player>(this.OnPlayer), (Collider) null, (Collider) null));
    }

    public Slider(EntityData e, Vector2 offset)
      : this(Vector2.op_Addition(e.Position, offset), e.Bool("clockwise", true), e.Enum<Slider.Surfaces>(nameof (surface), Slider.Surfaces.Floor))
    {
    }

    public override void Awake(Scene scene)
    {
      base.Awake(scene);
      int num = 0;
      while (!this.Scene.CollideCheck<Solid>(this.Position))
      {
        this.Position = Vector2.op_Addition(this.Position, this.surface);
        if (num >= 100)
          throw new Exception("Couldn't find surface");
      }
    }

    private void OnPlayer(Player Player)
    {
      Player.Die(Vector2.op_Subtraction(Player.Center, this.Center).SafeNormalize(Vector2.op_UnaryNegation(Vector2.get_UnitY())), false, true);
      this.moving = false;
    }

    public override void Update()
    {
      base.Update();
      if (!this.moving)
        return;
      this.speed = Calc.Approach(this.speed, 80f, 400f * Engine.DeltaTime);
      this.Position = Vector2.op_Addition(this.Position, Vector2.op_Multiply(Vector2.op_Multiply(this.dir, this.speed), Engine.DeltaTime));
      if (!this.OnSurfaceCheck())
      {
        if (!this.foundSurfaceAfterCorner)
          return;
        this.Position = this.Position.Round();
        int num = 0;
        while (!this.OnSurfaceCheck())
        {
          this.Position = Vector2.op_Subtraction(this.Position, this.dir);
          ++num;
          if (num >= 100)
            throw new Exception("Couldn't get back onto corner!");
        }
        this.foundSurfaceAfterCorner = false;
        Vector2 dir = this.dir;
        this.dir = this.surface;
        this.surface = Vector2.op_UnaryNegation(dir);
      }
      else
      {
        this.foundSurfaceAfterCorner = true;
        if (this.InWallCheck())
        {
          if (!this.gotOutOfWall)
            return;
          this.Position = this.Position.Round();
          int num = 0;
          while (this.InWallCheck())
          {
            this.Position = Vector2.op_Subtraction(this.Position, this.dir);
            ++num;
            if (num >= 100)
              throw new Exception("Couldn't get out of wall!");
          }
          this.Position = Vector2.op_Addition(this.Position, Vector2.op_Subtraction(this.dir, this.surface));
          this.gotOutOfWall = false;
          Vector2 surface = this.surface;
          this.surface = this.dir;
          this.dir = Vector2.op_UnaryNegation(surface);
        }
        else
          this.gotOutOfWall = true;
      }
    }

    private bool OnSurfaceCheck()
    {
      return this.Scene.CollideCheck<Solid>(Vector2.op_Addition(this.Position.Round(), this.surface));
    }

    private bool InWallCheck()
    {
      return this.Scene.CollideCheck<Solid>(Vector2.op_Subtraction(this.Position.Round(), this.surface));
    }

    public override void Render()
    {
      Draw.Circle(this.Position, 12f, Color.get_Red(), 8);
    }

    public enum Surfaces
    {
      Floor,
      Ceiling,
      LeftWall,
      RightWall,
    }
  }
}
