// Decompiled with JetBrains decompiler
// Type: Celeste.Solid
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste
{
  [Tracked(true)]
  public class Solid : Platform
  {
    public Vector2 Speed;
    public bool AllowStaticMovers = true;
    public bool EnableAssistModeChecks = true;
    public bool DisableLightsInside = true;
    public bool StopPlayerRunIntoAnimation = true;
    public bool SquishEvenInAssistMode;
    private static HashSet<Actor> riders = new HashSet<Actor>();

    public Solid(Vector2 position, float width, float height, bool safe)
      : base(position, safe)
    {
      this.Collider = (Collider) new Hitbox(width, height);
    }

    public override void Awake(Scene scene)
    {
      base.Awake(scene);
      if (!this.AllowStaticMovers)
        return;
      bool collidable = this.Collidable;
      this.Collidable = true;
      foreach (StaticMover component in scene.Tracker.GetComponents<StaticMover>())
      {
        if (component.IsRiding(this) && component.Platform == null)
        {
          this.staticMovers.Add(component);
          component.Platform = (Platform) this;
          if (component.OnAttach != null)
            component.OnAttach((Platform) this);
        }
      }
      this.Collidable = collidable;
    }

    public override void Update()
    {
      base.Update();
      this.MoveH(this.Speed.X * Engine.DeltaTime);
      this.MoveV(this.Speed.Y * Engine.DeltaTime);
      if (!this.EnableAssistModeChecks || SaveData.Instance == null || !SaveData.Instance.Assists.Invincible || this.Components.Get<SolidOnInvinciblePlayer>() != null || !this.Collidable)
        return;
      Player player = this.CollideFirst<Player>();
      Level scene = this.Scene as Level;
      if (player == null && (double) this.Bottom > (double) scene.Bounds.Bottom)
        player = this.CollideFirst<Player>(this.Position + Vector2.UnitY);
      if (player != null && player.StateMachine.State != 9 && player.StateMachine.State != 21)
      {
        this.Add((Component) new SolidOnInvinciblePlayer());
      }
      else
      {
        TheoCrystal theoCrystal = this.CollideFirst<TheoCrystal>();
        if (theoCrystal == null || theoCrystal.Hold.IsHeld)
          return;
        this.Add((Component) new SolidOnInvinciblePlayer());
      }
    }

    public bool HasRider()
    {
      foreach (Actor entity in this.Scene.Tracker.GetEntities<Actor>())
      {
        if (entity.IsRiding(this))
          return true;
      }
      return false;
    }

    public Player GetPlayerRider()
    {
      foreach (Player entity in this.Scene.Tracker.GetEntities<Player>())
      {
        if (entity.IsRiding(this))
          return entity;
      }
      return (Player) null;
    }

    public bool HasPlayerRider() => this.GetPlayerRider() != null;

    public bool HasPlayerOnTop() => this.GetPlayerOnTop() != null;

    public Player GetPlayerOnTop() => this.CollideFirst<Player>(this.Position - Vector2.UnitY);

    public bool HasPlayerClimbing() => this.GetPlayerClimbing() != null;

    public Player GetPlayerClimbing()
    {
      foreach (Player entity in this.Scene.Tracker.GetEntities<Player>())
      {
        if (entity.StateMachine.State == 1 && (entity.Facing == Facings.Left && this.CollideCheck((Entity) entity, this.Position + Vector2.UnitX) || entity.Facing == Facings.Right && this.CollideCheck((Entity) entity, this.Position - Vector2.UnitX)))
          return entity;
      }
      return (Player) null;
    }

    public void GetRiders()
    {
      foreach (Actor entity in this.Scene.Tracker.GetEntities<Actor>())
      {
        if (entity.IsRiding(this))
          Solid.riders.Add(entity);
      }
    }

    public override void MoveHExact(int move)
    {
      this.GetRiders();
      float right = this.Right;
      float left = this.Left;
      Player entity1 = this.Scene.Tracker.GetEntity<Player>();
      if (entity1 != null && Input.MoveX.Value == Math.Sign(move) && Math.Sign(entity1.Speed.X) == Math.Sign(move) && !Solid.riders.Contains((Actor) entity1) && this.CollideCheck((Entity) entity1, this.Position + Vector2.UnitX * (float) move - Vector2.UnitY))
        entity1.MoveV(1f);
      this.X += (float) move;
      this.MoveStaticMovers(Vector2.UnitX * (float) move);
      if (this.Collidable)
      {
        foreach (Actor entity2 in this.Scene.Tracker.GetEntities<Actor>())
        {
          if (entity2.AllowPushing)
          {
            bool collidable = entity2.Collidable;
            entity2.Collidable = true;
            if (!entity2.TreatNaive && this.CollideCheck((Entity) entity2, this.Position))
            {
              int moveH = move <= 0 ? move - (int) ((double) entity2.Right - (double) left) : move - (int) ((double) entity2.Left - (double) right);
              this.Collidable = false;
              entity2.MoveHExact(moveH, entity2.SquishCallback, this);
              entity2.LiftSpeed = this.LiftSpeed;
              this.Collidable = true;
            }
            else if (Solid.riders.Contains(entity2))
            {
              this.Collidable = false;
              if (entity2.TreatNaive)
                entity2.NaiveMove(Vector2.UnitX * (float) move);
              else
                entity2.MoveHExact(move);
              entity2.LiftSpeed = this.LiftSpeed;
              this.Collidable = true;
            }
            entity2.Collidable = collidable;
          }
        }
      }
      Solid.riders.Clear();
    }

    public override void MoveVExact(int move)
    {
      this.GetRiders();
      float bottom = this.Bottom;
      float top = this.Top;
      this.Y += (float) move;
      this.MoveStaticMovers(Vector2.UnitY * (float) move);
      if (this.Collidable)
      {
        foreach (Actor entity in this.Scene.Tracker.GetEntities<Actor>())
        {
          if (entity.AllowPushing)
          {
            bool collidable = entity.Collidable;
            entity.Collidable = true;
            if (!entity.TreatNaive && this.CollideCheck((Entity) entity, this.Position))
            {
              int moveV = move <= 0 ? move - (int) ((double) entity.Bottom - (double) top) : move - (int) ((double) entity.Top - (double) bottom);
              this.Collidable = false;
              entity.MoveVExact(moveV, entity.SquishCallback, this);
              entity.LiftSpeed = this.LiftSpeed;
              this.Collidable = true;
            }
            else if (Solid.riders.Contains(entity))
            {
              this.Collidable = false;
              if (entity.TreatNaive)
                entity.NaiveMove(Vector2.UnitY * (float) move);
              else
                entity.MoveVExact(move);
              entity.LiftSpeed = this.LiftSpeed;
              this.Collidable = true;
            }
            entity.Collidable = collidable;
          }
        }
      }
      Solid.riders.Clear();
    }
  }
}
