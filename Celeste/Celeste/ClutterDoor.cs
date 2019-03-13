﻿// Decompiled with JetBrains decompiler
// Type: Celeste.ClutterDoor
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class ClutterDoor : Solid
  {
    public ClutterBlock.Colors Color;
    private Sprite sprite;
    private Wiggler wiggler;

    public ClutterDoor(EntityData data, Vector2 offset, Session session)
      : base(Vector2.op_Addition(data.Position, offset), (float) data.Width, (float) data.Height, false)
    {
      this.Color = data.Enum<ClutterBlock.Colors>("type", ClutterBlock.Colors.Green);
      this.SurfaceSoundIndex = 20;
      this.Tag = (int) Tags.TransitionUpdate;
      this.Add((Component) (this.sprite = GFX.SpriteBank.Create("ghost_door")));
      this.sprite.Position = Vector2.op_Division(new Vector2(this.Width, this.Height), 2f);
      this.sprite.Play("idle", false, false);
      this.OnDashCollide = new DashCollision(this.OnDashed);
      this.Add((Component) (this.wiggler = Wiggler.Create(0.6f, 3f, (Action<float>) (f => this.sprite.Scale = Vector2.op_Multiply(Vector2.get_One(), (float) (1.0 - (double) f * 0.200000002980232))), false, false)));
      if (this.IsLocked(session))
        return;
      this.InstantUnlock();
    }

    public override void Update()
    {
      Level scene = this.Scene as Level;
      if (scene.Transitioning && this.CollideCheck<Player>())
      {
        this.Visible = false;
        this.Collidable = false;
      }
      else if (!this.Collidable && this.IsLocked(scene.Session) && !this.CollideCheck<Player>())
      {
        this.Visible = true;
        this.Collidable = true;
        this.wiggler.Start();
        Audio.Play("event:/game/03_resort/forcefield_bump", this.Position);
      }
      base.Update();
    }

    public bool IsLocked(Session session)
    {
      if (session.GetFlag("oshiro_clutter_door_open"))
        return this.IsComplete(session);
      return true;
    }

    public bool IsComplete(Session session)
    {
      return session.GetFlag("oshiro_clutter_cleared_" + (object) (int) this.Color);
    }

    public IEnumerator UnlockRoutine()
    {
      ClutterDoor clutterDoor = this;
      Camera camera = clutterDoor.SceneAs<Level>().Camera;
      Vector2 from = camera.Position;
      Vector2 to = clutterDoor.CameraTarget();
      Vector2 vector2 = Vector2.op_Subtraction(from, to);
      float p;
      if ((double) ((Vector2) ref vector2).Length() > 8.0)
      {
        for (p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime)
        {
          camera.Position = Vector2.op_Addition(from, Vector2.op_Multiply(Vector2.op_Subtraction(to, from), Ease.CubeInOut(p)));
          yield return (object) null;
        }
      }
      else
        yield return (object) 0.2f;
      Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
      Audio.Play("event:/game/03_resort/forcefield_vanish", clutterDoor.Position);
      clutterDoor.sprite.Play("open", false, false);
      clutterDoor.Collidable = false;
      for (p = 0.0f; (double) p < 0.400000005960464; p += Engine.DeltaTime)
      {
        camera.Position = clutterDoor.CameraTarget();
        yield return (object) null;
      }
    }

    public void InstantUnlock()
    {
      this.Visible = this.Collidable = false;
    }

    private Vector2 CameraTarget()
    {
      Level level = this.SceneAs<Level>();
      Vector2 vector2 = Vector2.op_Subtraction(this.Position, Vector2.op_Division(new Vector2(320f, 180f), 2f));
      ref Vector2 local1 = ref vector2;
      // ISSUE: variable of the null type
      __Null x = vector2.X;
      Rectangle bounds1 = level.Bounds;
      double left = (double) ((Rectangle) ref bounds1).get_Left();
      bounds1 = level.Bounds;
      double num1 = (double) (((Rectangle) ref bounds1).get_Right() - 320);
      double num2 = (double) MathHelper.Clamp((float) x, (float) left, (float) num1);
      local1.X = (__Null) num2;
      ref Vector2 local2 = ref vector2;
      // ISSUE: variable of the null type
      __Null y = vector2.Y;
      Rectangle bounds2 = level.Bounds;
      double top = (double) ((Rectangle) ref bounds2).get_Top();
      bounds2 = level.Bounds;
      double num3 = (double) (((Rectangle) ref bounds2).get_Bottom() - 180);
      double num4 = (double) MathHelper.Clamp((float) y, (float) top, (float) num3);
      local2.Y = (__Null) num4;
      return vector2;
    }

    private DashCollisionResults OnDashed(Player player, Vector2 direction)
    {
      this.wiggler.Start();
      Audio.Play("event:/game/03_resort/forcefield_bump", this.Position);
      return DashCollisionResults.Bounce;
    }
  }
}
