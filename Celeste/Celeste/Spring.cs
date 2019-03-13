// Decompiled with JetBrains decompiler
// Type: Celeste.Spring
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  public class Spring : Entity
  {
    private Sprite sprite;
    private Wiggler wiggler;
    private StaticMover staticMover;
    public Spring.Orientations Orientation;
    private bool playerCanUse;

    public Spring(Vector2 position, Spring.Orientations orientation, bool playerCanUse)
      : base(position)
    {
      this.Orientation = orientation;
      this.playerCanUse = playerCanUse;
      this.Add((Component) new PlayerCollider(new Action<Player>(this.OnCollide), (Collider) null, (Collider) null));
      this.Add((Component) new HoldableCollider(new Action<Holdable>(this.OnHoldable), (Collider) null));
      this.Add((Component) (this.sprite = new Sprite(GFX.Game, "objects/spring/")));
      this.sprite.Add("idle", "", 0.0f, new int[1]);
      this.sprite.Add("bounce", "", 0.07f, "idle", 0, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 3, 4, 5);
      this.sprite.Play("idle", false, false);
      this.sprite.Origin.X = (__Null) ((double) this.sprite.Width / 2.0);
      this.sprite.Origin.Y = (__Null) (double) this.sprite.Height;
      this.Depth = -8501;
      this.staticMover = new StaticMover();
      this.staticMover.OnAttach = (Action<Platform>) (p => this.Depth = p.Depth + 1);
      switch (orientation)
      {
        case Spring.Orientations.Floor:
          this.staticMover.SolidChecker = (Func<Solid, bool>) (s => this.CollideCheck((Entity) s, Vector2.op_Addition(this.Position, Vector2.get_UnitY())));
          this.staticMover.JumpThruChecker = (Func<JumpThru, bool>) (jt => this.CollideCheck((Entity) jt, Vector2.op_Addition(this.Position, Vector2.get_UnitY())));
          this.Add((Component) this.staticMover);
          break;
        case Spring.Orientations.WallLeft:
          this.staticMover.SolidChecker = (Func<Solid, bool>) (s => this.CollideCheck((Entity) s, Vector2.op_Subtraction(this.Position, Vector2.get_UnitX())));
          this.staticMover.JumpThruChecker = (Func<JumpThru, bool>) (jt => this.CollideCheck((Entity) jt, Vector2.op_Subtraction(this.Position, Vector2.get_UnitX())));
          this.Add((Component) this.staticMover);
          break;
        case Spring.Orientations.WallRight:
          this.staticMover.SolidChecker = (Func<Solid, bool>) (s => this.CollideCheck((Entity) s, Vector2.op_Addition(this.Position, Vector2.get_UnitX())));
          this.staticMover.JumpThruChecker = (Func<JumpThru, bool>) (jt => this.CollideCheck((Entity) jt, Vector2.op_Addition(this.Position, Vector2.get_UnitX())));
          this.Add((Component) this.staticMover);
          break;
      }
      this.Add((Component) (this.wiggler = Wiggler.Create(1f, 4f, (Action<float>) (v => this.sprite.Scale.Y = (__Null) (1.0 + (double) v * 0.200000002980232)), false, false)));
      if (orientation == Spring.Orientations.Floor)
        this.Collider = (Collider) new Hitbox(16f, 6f, -8f, -6f);
      else if (orientation == Spring.Orientations.WallLeft)
      {
        this.Collider = (Collider) new Hitbox(6f, 16f, 0.0f, -8f);
        this.sprite.Rotation = 1.570796f;
      }
      else
      {
        if (orientation != Spring.Orientations.WallRight)
          throw new Exception("Orientation not supported!");
        this.Collider = (Collider) new Hitbox(6f, 16f, -6f, -8f);
        this.sprite.Rotation = -1.570796f;
      }
      this.staticMover.OnEnable = new Action(this.OnEnable);
      this.staticMover.OnDisable = new Action(this.OnDisable);
    }

    public Spring(EntityData data, Vector2 offset, Spring.Orientations orientation)
      : this(Vector2.op_Addition(data.Position, offset), orientation, data.Bool(nameof (playerCanUse), true))
    {
    }

    private void OnEnable()
    {
      this.Visible = this.Collidable = true;
      this.sprite.Play("idle", false, false);
    }

    private void OnDisable()
    {
      this.Visible = this.Collidable = false;
    }

    private void OnCollide(Player player)
    {
      if (player.StateMachine.State == 9 || !this.playerCanUse)
        return;
      if (this.Orientation == Spring.Orientations.Floor)
      {
        if (player.Speed.Y < 0.0)
          return;
        this.BounceAnimate();
        player.SuperBounce(this.Top);
      }
      else if (this.Orientation == Spring.Orientations.WallLeft)
      {
        this.BounceAnimate();
        player.SideBounce(1, this.Right, this.CenterY);
      }
      else
      {
        if (this.Orientation != Spring.Orientations.WallRight)
          throw new Exception("Orientation not supported!");
        this.BounceAnimate();
        player.SideBounce(-1, this.Left, this.CenterY);
      }
    }

    private void BounceAnimate()
    {
      Audio.Play("event:/game/general/spring", this.BottomCenter);
      this.staticMover.TriggerPlatform();
      this.sprite.Play("bounce", true, false);
      this.wiggler.Start();
    }

    private void OnHoldable(Holdable h)
    {
      if (!h.HitSpring(this))
        return;
      this.BounceAnimate();
    }

    private void OnSeeker(Seeker seeker)
    {
      if (seeker.Speed.Y < -120.0)
        return;
      this.BounceAnimate();
      seeker.HitSpring();
    }

    public override void Render()
    {
      this.sprite.DrawOutline(1);
      base.Render();
    }

    public enum Orientations
    {
      Floor,
      WallLeft,
      WallRight,
    }
  }
}
