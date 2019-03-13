// Decompiled with JetBrains decompiler
// Type: Celeste.NPC
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Celeste
{
  public class NPC : Entity
  {
    public float Maxspeed = 80f;
    public string MoveAnim = "";
    public string IdleAnim = "";
    public bool MoveY = true;
    public bool UpdateLight = true;
    private List<Entity> temp = new List<Entity>();
    public const string MetTheo = "MetTheo";
    public const string TheoKnowsName = "TheoKnowsName";
    public const float TheoMaxSpeed = 48f;
    public Sprite Sprite;
    public TalkComponent Talker;
    public VertexLight Light;
    public Level Level;
    public SoundSource PhoneTapSfx;

    public Session Session
    {
      get
      {
        return this.Level.Session;
      }
    }

    public NPC(Vector2 position)
    {
      this.Position = position;
      this.Depth = 1000;
      this.Collider = (Collider) new Hitbox(8f, 8f, -4f, -8f);
      this.Add((Component) new MirrorReflection());
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      this.Level = scene as Level;
    }

    public override void Update()
    {
      base.Update();
      if (this.UpdateLight && this.Light != null)
      {
        Rectangle bounds = this.Level.Bounds;
        this.Light.Alpha = Calc.Approach(this.Light.Alpha, (double) this.X <= (double) (((Rectangle) ref bounds).get_Left() - 16) || (double) this.Y <= (double) (((Rectangle) ref bounds).get_Top() - 16) || (double) this.X >= (double) (((Rectangle) ref bounds).get_Right() + 16) || (double) this.Y >= (double) (((Rectangle) ref bounds).get_Bottom() + 16) || this.Level.Transitioning ? 0.0f : 1f, Engine.DeltaTime * 2f);
      }
      if (this.Sprite != null && this.Sprite.CurrentAnimationID == "usePhone")
      {
        if (this.PhoneTapSfx == null)
          this.Add((Component) (this.PhoneTapSfx = new SoundSource()));
        if (this.PhoneTapSfx.Playing)
          return;
        this.PhoneTapSfx.Play("event:/char/theo/phone_taps_loop", (string) null, 0.0f);
      }
      else
      {
        if (this.PhoneTapSfx == null || !this.PhoneTapSfx.Playing)
          return;
        this.PhoneTapSfx.Stop(true);
      }
    }

    public void SetupTheoSpriteSounds()
    {
      this.Sprite.OnFrameChange = (Action<string>) (anim =>
      {
        int currentAnimationFrame = this.Sprite.CurrentAnimationFrame;
        if (anim == "walk" && (currentAnimationFrame == 0 || currentAnimationFrame == 6) || anim == "run" && (currentAnimationFrame == 0 || currentAnimationFrame == 4))
        {
          Platform platformByPriority = SurfaceIndex.GetPlatformByPriority(this.CollideAll<Platform>(Vector2.op_Addition(this.Position, Vector2.get_UnitY()), this.temp));
          if (platformByPriority == null)
            return;
          Audio.Play("event:/char/madeline/footstep", this.Center, "surface_index", (float) platformByPriority.GetStepSoundIndex((Entity) this));
        }
        else if (anim == "crawl" && currentAnimationFrame == 0)
        {
          if (this.Level.Transitioning)
            return;
          Audio.Play("event:/char/theo/resort_crawl", this.Position);
        }
        else
        {
          if (!(anim == "pullVent") || currentAnimationFrame != 0)
            return;
          Audio.Play("event:/char/theo/resort_vent_tug", this.Position);
        }
      });
    }

    public void SetupGrannySpriteSounds()
    {
      this.Sprite.OnFrameChange = (Action<string>) (anim =>
      {
        int currentAnimationFrame = this.Sprite.CurrentAnimationFrame;
        if (anim == "walk" && (currentAnimationFrame == 0 || currentAnimationFrame == 4))
        {
          Platform platformByPriority = SurfaceIndex.GetPlatformByPriority(this.CollideAll<Platform>(Vector2.op_Addition(this.Position, Vector2.get_UnitY()), this.temp));
          if (platformByPriority == null)
            return;
          Audio.Play("event:/char/madeline/footstep", this.Center, "surface_index", (float) platformByPriority.GetStepSoundIndex((Entity) this));
        }
        else
        {
          if (!(anim == "walk") || currentAnimationFrame != 2)
            return;
          Audio.Play("event:/char/granny/cane_tap", this.Position);
        }
      });
    }

    public IEnumerator PlayerApproachRightSide(
      Player player,
      bool turnToFace = true,
      float? spacing = null)
    {
      yield return (object) this.PlayerApproach(player, turnToFace, spacing, new int?(1));
    }

    public IEnumerator PlayerApproachLeftSide(
      Player player,
      bool turnToFace = true,
      float? spacing = null)
    {
      yield return (object) this.PlayerApproach(player, turnToFace, spacing, new int?(-1));
    }

    public IEnumerator PlayerApproach(
      Player player,
      bool turnToFace = true,
      float? spacing = null,
      int? side = null)
    {
      NPC npc = this;
      if (!side.HasValue)
        side = new int?(Math.Sign(player.X - npc.X));
      int? nullable1 = side;
      int num = 0;
      if (nullable1.GetValueOrDefault() == num & nullable1.HasValue)
        side = new int?(1);
      player.StateMachine.State = 11;
      player.StateMachine.Locked = true;
      if (spacing.HasValue)
      {
        Player player1 = player;
        float x1 = npc.X;
        nullable1 = side;
        float? nullable2 = nullable1.HasValue ? new float?((float) nullable1.GetValueOrDefault()) : new float?();
        float? nullable3 = spacing;
        float? nullable4 = nullable2.HasValue & nullable3.HasValue ? new float?(nullable2.GetValueOrDefault() * nullable3.GetValueOrDefault()) : new float?();
        float? nullable5;
        if (!nullable4.HasValue)
        {
          nullable3 = new float?();
          nullable5 = nullable3;
        }
        else
          nullable5 = new float?(x1 + nullable4.GetValueOrDefault());
        nullable3 = nullable5;
        int x2 = (int) nullable3.Value;
        yield return (object) player1.DummyWalkToExact(x2, false, 1f);
      }
      else if ((double) Math.Abs(npc.X - player.X) < 12.0 || Math.Sign(player.X - npc.X) != side.Value)
      {
        Player player1 = player;
        float x1 = npc.X;
        nullable1 = side;
        float? nullable2 = nullable1.HasValue ? new float?((float) (nullable1.GetValueOrDefault() * 12)) : new float?();
        int x2 = (int) (nullable2.HasValue ? new float?(x1 + nullable2.GetValueOrDefault()) : new float?()).Value;
        yield return (object) player1.DummyWalkToExact(x2, false, 1f);
      }
      player.Facing = (Facings) -side.Value;
      if (turnToFace && npc.Sprite != null)
        npc.Sprite.Scale.X = (__Null) (double) side.Value;
      yield return (object) null;
    }

    public IEnumerator PlayerApproach48px()
    {
      // ISSUE: reference to a compiler-generated field
      int num = this.\u003C\u003E1__state;
      NPC npc = this;
      if (num != 0)
      {
        if (num != 1)
          return false;
        // ISSUE: reference to a compiler-generated field
        this.\u003C\u003E1__state = -1;
        return false;
      }
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      Player entity = npc.Scene.Tracker.GetEntity<Player>();
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E2__current = (object) npc.PlayerApproach(entity, true, new float?(48f), new int?());
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = 1;
      return true;
    }

    public IEnumerator PlayerLeave(Player player, float? to = null)
    {
      if (to.HasValue)
        yield return (object) player.DummyWalkToExact((int) to.Value, false, 1f);
      player.StateMachine.Locked = false;
      player.StateMachine.State = 0;
      yield return (object) null;
    }

    public IEnumerator MoveTo(
      Vector2 target,
      bool fadeIn = false,
      int? turnAtEndTo = null,
      bool removeAtEnd = false)
    {
      NPC npc = this;
      if (removeAtEnd)
        npc.Tag |= (int) Tags.TransitionUpdate;
      if (Math.Sign((float) target.X - npc.X) != 0 && npc.Sprite != null)
        npc.Sprite.Scale.X = (__Null) (double) Math.Sign((float) target.X - npc.X);
      Vector2.op_Subtraction(target, npc.Position).SafeNormalize();
      float alpha = fadeIn ? 0.0f : 1f;
      if (npc.Sprite != null && npc.Sprite.Has(npc.MoveAnim))
        npc.Sprite.Play(npc.MoveAnim, false, false);
      float speed = 0.0f;
      while (npc.MoveY && Vector2.op_Inequality(npc.Position, target) || !npc.MoveY && (double) npc.X != target.X)
      {
        speed = Calc.Approach(speed, npc.Maxspeed, 160f * Engine.DeltaTime);
        if (npc.MoveY)
          npc.Position = Calc.Approach(npc.Position, target, speed * Engine.DeltaTime);
        else
          npc.X = Calc.Approach(npc.X, (float) target.X, speed * Engine.DeltaTime);
        if (npc.Sprite != null)
          npc.Sprite.Color = Color.op_Multiply(Color.get_White(), alpha);
        alpha = Calc.Approach(alpha, 1f, Engine.DeltaTime);
        yield return (object) null;
      }
      if (npc.Sprite != null && npc.Sprite.Has(npc.IdleAnim))
        npc.Sprite.Play(npc.IdleAnim, false, false);
      while ((double) alpha < 1.0)
      {
        if (npc.Sprite != null)
          npc.Sprite.Color = Color.op_Multiply(Color.get_White(), alpha);
        alpha = Calc.Approach(alpha, 1f, Engine.DeltaTime);
        yield return (object) null;
      }
      if (turnAtEndTo.HasValue && npc.Sprite != null)
        npc.Sprite.Scale.X = (__Null) (double) turnAtEndTo.Value;
      if (removeAtEnd)
        npc.Scene.Remove((Entity) npc);
      yield return (object) null;
    }

    public void MoveToAndRemove(Vector2 target)
    {
      this.Add((Component) new Coroutine(this.MoveTo(target, false, new int?(), true), true));
    }
  }
}
