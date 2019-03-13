﻿// Decompiled with JetBrains decompiler
// Type: Celeste.FinalBossBeam
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;

namespace Celeste
{
  [Pooled]
  [Tracked(false)]
  public class FinalBossBeam : Entity
  {
    private VertexPositionColor[] fade = new VertexPositionColor[24];
    public static ParticleType P_Dissipate;
    public const float ChargeTime = 1.4f;
    public const float FollowTime = 0.9f;
    public const float ActiveTime = 0.12f;
    private const float AngleStartOffset = 100f;
    private const float RotationSpeed = 200f;
    private const float CollideCheckSep = 2f;
    private const float BeamLength = 2000f;
    private const float BeamStartDist = 12f;
    private const int BeamsDrawn = 15;
    private const float SideDarknessAlpha = 0.35f;
    private FinalBoss boss;
    private Player player;
    private Sprite beamSprite;
    private Sprite beamStartSprite;
    private float chargeTimer;
    private float followTimer;
    private float activeTimer;
    private float angle;
    private float beamAlpha;
    private float sideFadeAlpha;

    public FinalBossBeam()
    {
      this.Add((Component) (this.beamSprite = GFX.SpriteBank.Create("badeline_beam")));
      this.beamSprite.OnLastFrame = (Action<string>) (anim =>
      {
        if (!(anim == "shoot"))
          return;
        this.Destroy();
      });
      this.Add((Component) (this.beamStartSprite = GFX.SpriteBank.Create("badeline_beam_start")));
      this.beamSprite.Visible = false;
      this.Depth = -1000000;
    }

    public FinalBossBeam Init(FinalBoss boss, Player target)
    {
      this.boss = boss;
      this.chargeTimer = 1.4f;
      this.followTimer = 0.9f;
      this.activeTimer = 0.12f;
      this.beamSprite.Play("charge", false, false);
      this.sideFadeAlpha = 0.0f;
      this.beamAlpha = 0.0f;
      int num = (double) target.Y > (double) boss.Y + 16.0 ? -1 : 1;
      if ((double) target.X >= (double) boss.X)
        num *= -1;
      this.angle = Calc.Angle(boss.BeamOrigin, target.Center);
      Vector2 to = Vector2.op_Addition(Calc.ClosestPointOnLine(boss.BeamOrigin, Vector2.op_Addition(boss.BeamOrigin, Calc.AngleToVector(this.angle, 2000f)), target.Center), Vector2.op_Multiply(Vector2.op_Subtraction(target.Center, boss.BeamOrigin).Perpendicular().SafeNormalize(100f), (float) num));
      this.angle = Calc.Angle(boss.BeamOrigin, to);
      return this;
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      if (!this.boss.Moving)
        return;
      this.RemoveSelf();
    }

    public override void Update()
    {
      base.Update();
      this.player = this.Scene.Tracker.GetEntity<Player>();
      this.beamAlpha = Calc.Approach(this.beamAlpha, 1f, 2f * Engine.DeltaTime);
      if ((double) this.chargeTimer > 0.0)
      {
        this.sideFadeAlpha = Calc.Approach(this.sideFadeAlpha, 1f, Engine.DeltaTime);
        if (this.player == null || this.player.Dead)
          return;
        this.followTimer -= Engine.DeltaTime;
        this.chargeTimer -= Engine.DeltaTime;
        if ((double) this.followTimer > 0.0 && Vector2.op_Inequality(this.player.Center, this.boss.BeamOrigin))
          this.angle = Calc.Angle(this.boss.BeamOrigin, Calc.Approach(Calc.ClosestPointOnLine(this.boss.BeamOrigin, Vector2.op_Addition(this.boss.BeamOrigin, Calc.AngleToVector(this.angle, 2000f)), this.player.Center), this.player.Center, 200f * Engine.DeltaTime));
        else if (this.beamSprite.CurrentAnimationID == "charge")
          this.beamSprite.Play("lock", false, false);
        if ((double) this.chargeTimer > 0.0)
          return;
        this.SceneAs<Level>().DirectionalShake(Calc.AngleToVector(this.angle, 1f), 0.15f);
        Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
        this.DissipateParticles();
      }
      else
      {
        if ((double) this.activeTimer <= 0.0)
          return;
        this.sideFadeAlpha = Calc.Approach(this.sideFadeAlpha, 0.0f, Engine.DeltaTime * 8f);
        if (this.beamSprite.CurrentAnimationID != "shoot")
        {
          this.beamSprite.Play("shoot", false, false);
          this.beamStartSprite.Play("shoot", true, false);
        }
        this.activeTimer -= Engine.DeltaTime;
        if ((double) this.activeTimer <= 0.0)
          return;
        this.PlayerCollideCheck();
      }
    }

    private void DissipateParticles()
    {
      Level level = this.SceneAs<Level>();
      Vector2 closestTo = Vector2.op_Addition(level.Camera.Position, new Vector2(160f, 90f));
      Vector2 lineA = Vector2.op_Addition(this.boss.BeamOrigin, Calc.AngleToVector(this.angle, 12f));
      Vector2 lineB = Vector2.op_Addition(this.boss.BeamOrigin, Calc.AngleToVector(this.angle, 2000f));
      Vector2 vector = Vector2.op_Subtraction(lineB, lineA).Perpendicular().SafeNormalize();
      Vector2 vector2_1 = Vector2.op_Subtraction(lineB, lineA).SafeNormalize();
      Vector2 min = Vector2.op_Multiply(Vector2.op_UnaryNegation(vector), 1f);
      Vector2 max = Vector2.op_Multiply(vector, 1f);
      float direction1 = vector.Angle();
      float direction2 = Vector2.op_UnaryNegation(vector).Angle();
      float num = Vector2.Distance(closestTo, lineA) - 12f;
      Vector2 vector2_2 = Calc.ClosestPointOnLine(lineA, lineB, closestTo);
      for (int index1 = 0; index1 < 200; index1 += 12)
      {
        for (int index2 = -1; index2 <= 1; index2 += 2)
        {
          level.ParticlesFG.Emit(FinalBossBeam.P_Dissipate, Vector2.op_Addition(Vector2.op_Addition(Vector2.op_Addition(vector2_2, Vector2.op_Multiply(vector2_1, (float) index1)), Vector2.op_Multiply(Vector2.op_Multiply(vector, 2f), (float) index2)), Calc.Random.Range(min, max)), direction1);
          level.ParticlesFG.Emit(FinalBossBeam.P_Dissipate, Vector2.op_Addition(Vector2.op_Subtraction(Vector2.op_Addition(vector2_2, Vector2.op_Multiply(vector2_1, (float) index1)), Vector2.op_Multiply(Vector2.op_Multiply(vector, 2f), (float) index2)), Calc.Random.Range(min, max)), direction2);
          if (index1 != 0 && (double) index1 < (double) num)
          {
            level.ParticlesFG.Emit(FinalBossBeam.P_Dissipate, Vector2.op_Addition(Vector2.op_Addition(Vector2.op_Subtraction(vector2_2, Vector2.op_Multiply(vector2_1, (float) index1)), Vector2.op_Multiply(Vector2.op_Multiply(vector, 2f), (float) index2)), Calc.Random.Range(min, max)), direction1);
            level.ParticlesFG.Emit(FinalBossBeam.P_Dissipate, Vector2.op_Addition(Vector2.op_Subtraction(Vector2.op_Subtraction(vector2_2, Vector2.op_Multiply(vector2_1, (float) index1)), Vector2.op_Multiply(Vector2.op_Multiply(vector, 2f), (float) index2)), Calc.Random.Range(min, max)), direction2);
          }
        }
      }
    }

    private void PlayerCollideCheck()
    {
      Vector2 from = Vector2.op_Addition(this.boss.BeamOrigin, Calc.AngleToVector(this.angle, 12f));
      Vector2 to = Vector2.op_Addition(this.boss.BeamOrigin, Calc.AngleToVector(this.angle, 2000f));
      Vector2 vector2 = Vector2.op_Subtraction(to, from).Perpendicular().SafeNormalize(2f);
      Player player = (this.Scene.CollideFirst<Player>(Vector2.op_Addition(from, vector2), Vector2.op_Addition(to, vector2)) ?? this.Scene.CollideFirst<Player>(Vector2.op_Subtraction(from, vector2), Vector2.op_Subtraction(to, vector2))) ?? this.Scene.CollideFirst<Player>(from, to);
      player?.Die(Vector2.op_Subtraction(player.Center, this.boss.BeamOrigin).SafeNormalize(), false, true);
    }

    public override void Render()
    {
      Vector2 offset = this.boss.BeamOrigin;
      Vector2 vector1 = Calc.AngleToVector(this.angle, this.beamSprite.Width);
      this.beamSprite.Rotation = this.angle;
      this.beamSprite.Color = Color.op_Multiply(Color.get_White(), this.beamAlpha);
      this.beamStartSprite.Rotation = this.angle;
      this.beamStartSprite.Color = Color.op_Multiply(Color.get_White(), this.beamAlpha);
      if (this.beamSprite.CurrentAnimationID == "shoot")
        offset = Vector2.op_Addition(offset, Calc.AngleToVector(this.angle, 8f));
      for (int index = 0; index < 15; ++index)
      {
        this.beamSprite.RenderPosition = offset;
        this.beamSprite.Render();
        offset = Vector2.op_Addition(offset, vector1);
      }
      if (this.beamSprite.CurrentAnimationID == "shoot")
      {
        this.beamStartSprite.RenderPosition = this.boss.BeamOrigin;
        this.beamStartSprite.Render();
      }
      GameplayRenderer.End();
      Vector2 vector2 = vector1.SafeNormalize();
      Vector2 vector2_1 = vector2.Perpendicular();
      Color color = Color.op_Multiply(Color.op_Multiply(Color.get_Black(), this.sideFadeAlpha), 0.35f);
      Color transparent = Color.get_Transparent();
      Vector2 vector2_2 = Vector2.op_Multiply(vector2, 4000f);
      Vector2 vector2_3 = Vector2.op_Multiply(vector2_1, 120f);
      int v = 0;
      this.Quad(ref v, offset, Vector2.op_Addition(Vector2.op_UnaryNegation(vector2_2), Vector2.op_Multiply(vector2_3, 2f)), Vector2.op_Addition(vector2_2, Vector2.op_Multiply(vector2_3, 2f)), Vector2.op_Addition(vector2_2, vector2_3), Vector2.op_Addition(Vector2.op_UnaryNegation(vector2_2), vector2_3), color, color);
      this.Quad(ref v, offset, Vector2.op_Addition(Vector2.op_UnaryNegation(vector2_2), vector2_3), Vector2.op_Addition(vector2_2, vector2_3), vector2_2, Vector2.op_UnaryNegation(vector2_2), color, transparent);
      this.Quad(ref v, offset, Vector2.op_UnaryNegation(vector2_2), vector2_2, Vector2.op_Subtraction(vector2_2, vector2_3), Vector2.op_Subtraction(Vector2.op_UnaryNegation(vector2_2), vector2_3), transparent, color);
      this.Quad(ref v, offset, Vector2.op_Subtraction(Vector2.op_UnaryNegation(vector2_2), vector2_3), Vector2.op_Subtraction(vector2_2, vector2_3), Vector2.op_Subtraction(vector2_2, Vector2.op_Multiply(vector2_3, 2f)), Vector2.op_Subtraction(Vector2.op_UnaryNegation(vector2_2), Vector2.op_Multiply(vector2_3, 2f)), color, color);
      GFX.DrawVertices<VertexPositionColor>((this.Scene as Level).Camera.Matrix, this.fade, this.fade.Length, (Effect) null, (BlendState) null);
      GameplayRenderer.Begin();
    }

    private void Quad(
      ref int v,
      Vector2 offset,
      Vector2 a,
      Vector2 b,
      Vector2 c,
      Vector2 d,
      Color ab,
      Color cd)
    {
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      (^(Vector3&) ref this.fade[v].Position).X = offset.X + a.X;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      (^(Vector3&) ref this.fade[v].Position).Y = offset.Y + a.Y;
      this.fade[v++].Color = (__Null) ab;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      (^(Vector3&) ref this.fade[v].Position).X = offset.X + b.X;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      (^(Vector3&) ref this.fade[v].Position).Y = offset.Y + b.Y;
      this.fade[v++].Color = (__Null) ab;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      (^(Vector3&) ref this.fade[v].Position).X = offset.X + c.X;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      (^(Vector3&) ref this.fade[v].Position).Y = offset.Y + c.Y;
      this.fade[v++].Color = (__Null) cd;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      (^(Vector3&) ref this.fade[v].Position).X = offset.X + a.X;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      (^(Vector3&) ref this.fade[v].Position).Y = offset.Y + a.Y;
      this.fade[v++].Color = (__Null) ab;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      (^(Vector3&) ref this.fade[v].Position).X = offset.X + c.X;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      (^(Vector3&) ref this.fade[v].Position).Y = offset.Y + c.Y;
      this.fade[v++].Color = (__Null) cd;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      (^(Vector3&) ref this.fade[v].Position).X = offset.X + d.X;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      (^(Vector3&) ref this.fade[v].Position).Y = offset.Y + d.Y;
      this.fade[v++].Color = (__Null) cd;
    }

    public void Destroy()
    {
      this.RemoveSelf();
    }
  }
}
