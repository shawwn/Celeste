// Decompiled with JetBrains decompiler
// Type: Celeste.Spring
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

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
    public Color DisabledColor = Color.White;
    public bool VisibleWhenDisabled;

    public Spring(Vector2 position, Spring.Orientations orientation, bool playerCanUse)
      : base(position)
    {
      this.Orientation = orientation;
      this.playerCanUse = playerCanUse;
      this.Add((Component) new PlayerCollider(new Action<Player>(this.OnCollide)));
      this.Add((Component) new HoldableCollider(new Action<Holdable>(this.OnHoldable)));
      PufferCollider pufferCollider = new PufferCollider(new Action<Puffer>(this.OnPuffer));
      this.Add((Component) pufferCollider);
      this.Add((Component) (this.sprite = new Sprite(GFX.Game, "objects/spring/")));
      this.sprite.Add("idle", "", 0.0f, new int[1]);
      this.sprite.Add("bounce", "", 0.07f, "idle", 0, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 3, 4, 5);
      this.sprite.Add("disabled", "white", 0.07f);
      this.sprite.Play("idle");
      this.sprite.Origin.X = this.sprite.Width / 2f;
      this.sprite.Origin.Y = this.sprite.Height;
      this.Depth = -8501;
      this.staticMover = new StaticMover();
      this.staticMover.OnAttach = (Action<Platform>) (p => this.Depth = p.Depth + 1);
      switch (orientation)
      {
        case Spring.Orientations.Floor:
          this.staticMover.SolidChecker = (Func<Solid, bool>) (s => this.CollideCheck((Entity) s, this.Position + Vector2.UnitY));
          this.staticMover.JumpThruChecker = (Func<JumpThru, bool>) (jt => this.CollideCheck((Entity) jt, this.Position + Vector2.UnitY));
          this.Add((Component) this.staticMover);
          break;
        case Spring.Orientations.WallLeft:
          this.staticMover.SolidChecker = (Func<Solid, bool>) (s => this.CollideCheck((Entity) s, this.Position - Vector2.UnitX));
          this.staticMover.JumpThruChecker = (Func<JumpThru, bool>) (jt => this.CollideCheck((Entity) jt, this.Position - Vector2.UnitX));
          this.Add((Component) this.staticMover);
          break;
        case Spring.Orientations.WallRight:
          this.staticMover.SolidChecker = (Func<Solid, bool>) (s => this.CollideCheck((Entity) s, this.Position + Vector2.UnitX));
          this.staticMover.JumpThruChecker = (Func<JumpThru, bool>) (jt => this.CollideCheck((Entity) jt, this.Position + Vector2.UnitX));
          this.Add((Component) this.staticMover);
          break;
      }
      this.Add((Component) (this.wiggler = Wiggler.Create(1f, 4f, (Action<float>) (v => this.sprite.Scale.Y = (float) (1.0 + (double) v * 0.20000000298023224)))));
      if (orientation == Spring.Orientations.Floor)
      {
        this.Collider = (Collider) new Hitbox(16f, 6f, -8f, -6f);
        pufferCollider.Collider = (Collider) new Hitbox(16f, 10f, -8f, -10f);
      }
      else if (orientation == Spring.Orientations.WallLeft)
      {
        this.Collider = (Collider) new Hitbox(6f, 16f, y: -8f);
        pufferCollider.Collider = (Collider) new Hitbox(12f, 16f, y: -8f);
        this.sprite.Rotation = 1.5707964f;
      }
      else
      {
        if (orientation != Spring.Orientations.WallRight)
          throw new Exception("Orientation not supported!");
        this.Collider = (Collider) new Hitbox(6f, 16f, -6f, -8f);
        pufferCollider.Collider = (Collider) new Hitbox(12f, 16f, -12f, -8f);
        this.sprite.Rotation = -1.5707964f;
      }
      this.staticMover.OnEnable = new Action(this.OnEnable);
      this.staticMover.OnDisable = new Action(this.OnDisable);
    }

    public Spring(EntityData data, Vector2 offset, Spring.Orientations orientation)
      : this(data.Position + offset, orientation, data.Bool(nameof (playerCanUse), true))
    {
    }

    private void OnEnable()
    {
      this.Visible = this.Collidable = true;
      this.sprite.Color = Color.White;
      this.sprite.Play("idle");
    }

    private void OnDisable()
    {
      this.Collidable = false;
      if (this.VisibleWhenDisabled)
      {
        this.sprite.Play("disabled");
        this.sprite.Color = this.DisabledColor;
      }
      else
        this.Visible = false;
    }

    private void OnCollide(Player player)
    {
      if (player.StateMachine.State == 9 || !this.playerCanUse)
        return;
      if (this.Orientation == Spring.Orientations.Floor)
      {
        if ((double) player.Speed.Y < 0.0)
          return;
        this.BounceAnimate();
        player.SuperBounce(this.Top);
      }
      else if (this.Orientation == Spring.Orientations.WallLeft)
      {
        if (!player.SideBounce(1, this.Right, this.CenterY))
          return;
        this.BounceAnimate();
      }
      else
      {
        if (this.Orientation != Spring.Orientations.WallRight)
          throw new Exception("Orientation not supported!");
        if (!player.SideBounce(-1, this.Left, this.CenterY))
          return;
        this.BounceAnimate();
      }
    }

    private void BounceAnimate()
    {
      Audio.Play("event:/game/general/spring", this.BottomCenter);
      this.staticMover.TriggerPlatform();
      this.sprite.Play("bounce", true);
      this.wiggler.Start();
    }

    private void OnHoldable(Holdable h)
    {
      if (!h.HitSpring(this))
        return;
      this.BounceAnimate();
    }

    private void OnPuffer(Puffer p)
    {
      if (!p.HitSpring(this))
        return;
      this.BounceAnimate();
    }

    private void OnSeeker(Seeker seeker)
    {
      if ((double) seeker.Speed.Y < -120.0)
        return;
      this.BounceAnimate();
      seeker.HitSpring();
    }

    public override void Render()
    {
      if (this.Collidable)
        this.sprite.DrawOutline();
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
