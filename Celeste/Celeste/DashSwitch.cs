// Decompiled with JetBrains decompiler
// Type: Celeste.DashSwitch
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste
{
  public class DashSwitch : Solid
  {
    public static ParticleType P_PressA;
    public static ParticleType P_PressB;
    public static ParticleType P_PressAMirror;
    public static ParticleType P_PressBMirror;
    private DashSwitch.Sides side;
    private Vector2 pressedTarget;
    private bool pressed;
    private Vector2 pressDirection;
    private float speedY;
    private float startY;
    private bool persistent;
    private EntityID id;
    private bool mirrorMode;
    private bool playerWasOn;
    private Sprite sprite;

    public DashSwitch(
      Vector2 position,
      DashSwitch.Sides side,
      bool persistent,
      EntityID id,
      string spriteName)
      : base(position, 0.0f, 0.0f, true)
    {
      this.side = side;
      this.persistent = persistent;
      this.id = id;
      this.mirrorMode = spriteName != "default";
      this.Add((Component) (this.sprite = GFX.SpriteBank.Create("dashSwitch_" + spriteName)));
      this.sprite.Play("idle", false, false);
      if (side == DashSwitch.Sides.Up || side == DashSwitch.Sides.Down)
      {
        this.Collider.Width = 16f;
        this.Collider.Height = 8f;
      }
      else
      {
        this.Collider.Width = 8f;
        this.Collider.Height = 16f;
      }
      switch (side)
      {
        case DashSwitch.Sides.Up:
          this.sprite.Position = new Vector2(8f, 0.0f);
          this.sprite.Rotation = -1.570796f;
          this.pressedTarget = this.Position + Vector2.UnitY * -8f;
          this.pressDirection = -Vector2.UnitY;
          break;
        case DashSwitch.Sides.Down:
          this.sprite.Position = new Vector2(8f, 8f);
          this.sprite.Rotation = 1.570796f;
          this.pressedTarget = this.Position + Vector2.UnitY * 8f;
          this.pressDirection = Vector2.UnitY;
          this.startY = this.Y;
          break;
        case DashSwitch.Sides.Left:
          this.sprite.Position = new Vector2(0.0f, 8f);
          this.sprite.Rotation = 3.141593f;
          this.pressedTarget = this.Position + Vector2.UnitX * -8f;
          this.pressDirection = -Vector2.UnitX;
          break;
        case DashSwitch.Sides.Right:
          this.sprite.Position = new Vector2(8f, 8f);
          this.sprite.Rotation = 0.0f;
          this.pressedTarget = this.Position + Vector2.UnitX * 8f;
          this.pressDirection = Vector2.UnitX;
          break;
      }
      this.OnDashCollide = new DashCollision(this.OnDashed);
    }

    public static DashSwitch Create(EntityData data, Vector2 offset, EntityID id)
    {
      Vector2 position = data.Position + offset;
      bool persistent = data.Bool("persistent", false);
      string spriteName = data.Attr("sprite", "default");
      if (data.Name.Equals("dashSwitchH"))
      {
        if (data.Bool("leftSide", false))
          return new DashSwitch(position, DashSwitch.Sides.Left, persistent, id, spriteName);
        return new DashSwitch(position, DashSwitch.Sides.Right, persistent, id, spriteName);
      }
      if (data.Bool("ceiling", false))
        return new DashSwitch(position, DashSwitch.Sides.Up, persistent, id, spriteName);
      return new DashSwitch(position, DashSwitch.Sides.Down, persistent, id, spriteName);
    }

    public override void Awake(Scene scene)
    {
      base.Awake(scene);
      if (!this.persistent || !this.SceneAs<Level>().Session.GetFlag(this.FlagName))
        return;
      this.sprite.Play("pushed", false, false);
      this.Position = this.pressedTarget - this.pressDirection * 2f;
      this.pressed = true;
      this.Collidable = false;
      TempleGate gate = this.GetGate();
      if (gate != null)
        gate.StartOpen();
    }

    public override void Update()
    {
      base.Update();
      if (this.pressed || this.side != DashSwitch.Sides.Down)
        return;
      Player playerOnTop = this.GetPlayerOnTop();
      if (playerOnTop != null)
      {
        if (playerOnTop.Holding != null)
        {
          int num = (int) this.OnDashed(playerOnTop, Vector2.UnitY);
        }
        else
        {
          if ((double) this.speedY < 0.0)
            this.speedY = 0.0f;
          this.speedY = Calc.Approach(this.speedY, 70f, 200f * Engine.DeltaTime);
          this.MoveTowardsY(this.startY + 2f, this.speedY * Engine.DeltaTime);
          if (!this.playerWasOn)
            Audio.Play("event:/game/05_mirror_temple/button_depress", this.Position);
        }
      }
      else
      {
        if ((double) this.speedY > 0.0)
          this.speedY = 0.0f;
        this.speedY = Calc.Approach(this.speedY, -150f, 200f * Engine.DeltaTime);
        this.MoveTowardsY(this.startY, -this.speedY * Engine.DeltaTime);
        if (this.playerWasOn)
          Audio.Play("event:/game/05_mirror_temple/button_return", this.Position);
      }
      this.playerWasOn = playerOnTop != null;
    }

    public DashCollisionResults OnDashed(Player player, Vector2 direction)
    {
      if (!this.pressed && direction == this.pressDirection)
      {
        Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
        Audio.Play("event:/game/05_mirror_temple/button_activate", this.Position);
        this.sprite.Play("push", false, false);
        this.pressed = true;
        this.MoveTo(this.pressedTarget);
        this.Collidable = false;
        this.Position = this.Position - this.pressDirection * 2f;
        this.SceneAs<Level>().ParticlesFG.Emit(this.mirrorMode ? DashSwitch.P_PressAMirror : DashSwitch.P_PressA, 10, this.Position + this.sprite.Position, direction.Perpendicular() * 6f, this.sprite.Rotation - 3.141593f);
        this.SceneAs<Level>().ParticlesFG.Emit(this.mirrorMode ? DashSwitch.P_PressBMirror : DashSwitch.P_PressB, 4, this.Position + this.sprite.Position, direction.Perpendicular() * 6f, this.sprite.Rotation - 3.141593f);
        TempleGate gate = this.GetGate();
        if (gate != null)
          gate.SwitchOpen();
        TempleMirrorPortal first = this.Scene.Entities.FindFirst<TempleMirrorPortal>();
        if (first != null)
          first.OnSwitchHit(Math.Sign(this.X - (float) (this.Scene as Level).Bounds.Center.X));
        if (this.persistent)
          this.SceneAs<Level>().Session.SetFlag(this.FlagName, true);
      }
      return DashCollisionResults.NormalCollision;
    }

    private TempleGate GetGate()
    {
      List<Entity> entities = this.Scene.Tracker.GetEntities<TempleGate>();
      TempleGate templeGate1 = (TempleGate) null;
      float num1 = 0.0f;
      foreach (TempleGate templeGate2 in entities)
      {
        if (templeGate2.Type == TempleGate.Types.NearestSwitch && !templeGate2.ClaimedByASwitch && templeGate2.LevelID == this.id.Level)
        {
          float num2 = Vector2.DistanceSquared(this.Position, templeGate2.Position);
          if (templeGate1 == null || (double) num2 < (double) num1)
          {
            templeGate1 = templeGate2;
            num1 = num2;
          }
        }
      }
      if (templeGate1 != null)
        templeGate1.ClaimedByASwitch = true;
      return templeGate1;
    }

    private string FlagName
    {
      get
      {
        return DashSwitch.GetFlagName(this.id);
      }
    }

    public static string GetFlagName(EntityID id)
    {
      return "dashSwitch_" + id.Key;
    }

    public enum Sides
    {
      Up,
      Down,
      Left,
      Right,
    }
  }
}

