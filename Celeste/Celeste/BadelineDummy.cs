// Decompiled with JetBrains decompiler
// Type: Celeste.BadelineDummy
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class BadelineDummy : Entity
  {
    public float FloatSpeed = 120f;
    public float FloatAccel = 240f;
    public float Floatness = 2f;
    public Vector2 floatNormal = new Vector2(0.0f, 1f);
    public PlayerSprite Sprite;
    public PlayerHair Hair;
    public BadelineAutoAnimator AutoAnimator;
    public SineWave Wave;
    public VertexLight Light;

    public BadelineDummy(Vector2 position)
      : base(position)
    {
      this.Collider = (Collider) new Hitbox(6f, 6f, -3f, -7f);
      this.Sprite = new PlayerSprite(PlayerSpriteMode.Badeline);
      this.Sprite.Play("fallSlow", false, false);
      this.Sprite.Scale.X = (__Null) -1.0;
      this.Hair = new PlayerHair(this.Sprite);
      this.Hair.Color = BadelineOldsite.HairColor;
      this.Hair.Border = Color.get_Black();
      this.Hair.Facing = Facings.Left;
      this.Add((Component) this.Hair);
      this.Add((Component) this.Sprite);
      this.Add((Component) (this.AutoAnimator = new BadelineAutoAnimator()));
      this.Sprite.OnFrameChange = (Action<string>) (anim =>
      {
        int currentAnimationFrame = this.Sprite.CurrentAnimationFrame;
        if ((!(anim == "walk") || currentAnimationFrame != 0 && currentAnimationFrame != 6) && (!(anim == "runSlow") || currentAnimationFrame != 0 && currentAnimationFrame != 6) && (!(anim == "runFast") || currentAnimationFrame != 0 && currentAnimationFrame != 6))
          return;
        Audio.Play("event:/char/badeline/footstep", this.Position);
      });
      this.Add((Component) (this.Wave = new SineWave(0.25f)));
      this.Wave.OnUpdate = (Action<float>) (f => this.Sprite.Position = Vector2.op_Multiply(Vector2.op_Multiply(this.floatNormal, f), this.Floatness));
      this.Add((Component) (this.Light = new VertexLight(new Vector2(0.0f, -8f), Color.get_PaleVioletRed(), 1f, 20, 60)));
    }

    public void Appear(Level level, bool silent = false)
    {
      if (!silent)
      {
        Audio.Play("event:/char/badeline/appear", this.Position);
        Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
      }
      level.Displacement.AddBurst(this.Center, 0.5f, 24f, 96f, 0.4f, (Ease.Easer) null, (Ease.Easer) null);
      level.Particles.Emit(BadelineOldsite.P_Vanish, 12, this.Center, Vector2.op_Multiply(Vector2.get_One(), 6f));
    }

    public void Vanish()
    {
      Audio.Play("event:/char/badeline/disappear", this.Position);
      this.Shockwave();
      this.SceneAs<Level>().Particles.Emit(BadelineOldsite.P_Vanish, 12, this.Center, Vector2.op_Multiply(Vector2.get_One(), 6f));
      this.RemoveSelf();
    }

    private void Shockwave()
    {
      this.SceneAs<Level>().Displacement.AddBurst(this.Center, 0.5f, 24f, 96f, 0.4f, (Ease.Easer) null, (Ease.Easer) null);
    }

    public IEnumerator FloatTo(
      Vector2 target,
      int? turnAtEndTo = null,
      bool faceDirection = true,
      bool fadeLight = false)
    {
      BadelineDummy badelineDummy = this;
      badelineDummy.Sprite.Play("fallSlow", false, false);
      if (faceDirection && Math.Sign((float) target.X - badelineDummy.X) != 0)
        badelineDummy.Sprite.Scale.X = (__Null) (double) Math.Sign((float) target.X - badelineDummy.X);
      Vector2 vector2 = Vector2.op_Subtraction(target, badelineDummy.Position).SafeNormalize();
      Vector2 perp = new Vector2((float) -vector2.Y, (float) vector2.X);
      float speed = 0.0f;
      while (Vector2.op_Inequality(badelineDummy.Position, target))
      {
        speed = Calc.Approach(speed, badelineDummy.FloatSpeed, badelineDummy.FloatAccel * Engine.DeltaTime);
        badelineDummy.Position = Calc.Approach(badelineDummy.Position, target, speed * Engine.DeltaTime);
        badelineDummy.Floatness = Calc.Approach(badelineDummy.Floatness, 4f, 8f * Engine.DeltaTime);
        badelineDummy.floatNormal = Calc.Approach(badelineDummy.floatNormal, perp, Engine.DeltaTime * 12f);
        if (fadeLight)
          badelineDummy.Light.Alpha = Calc.Approach(badelineDummy.Light.Alpha, 0.0f, Engine.DeltaTime * 2f);
        yield return (object) null;
      }
      while ((double) badelineDummy.Floatness != 2.0)
      {
        badelineDummy.Floatness = Calc.Approach(badelineDummy.Floatness, 2f, 8f * Engine.DeltaTime);
        yield return (object) null;
      }
      if (turnAtEndTo.HasValue)
        badelineDummy.Sprite.Scale.X = (__Null) (double) turnAtEndTo.Value;
    }

    public IEnumerator WalkTo(float x)
    {
      BadelineDummy badelineDummy = this;
      badelineDummy.Floatness = 0.0f;
      badelineDummy.Sprite.Play("walk", false, false);
      if (Math.Sign(x - badelineDummy.X) != 0)
        badelineDummy.Sprite.Scale.X = (__Null) (double) Math.Sign(x - badelineDummy.X);
      while ((double) badelineDummy.X != (double) x)
      {
        badelineDummy.X = Calc.Approach(badelineDummy.X, x, Engine.DeltaTime * 64f);
        yield return (object) null;
      }
      badelineDummy.Sprite.Play("idle", false, false);
    }

    public IEnumerator SmashBlock(Vector2 target)
    {
      BadelineDummy badelineDummy = this;
      badelineDummy.SceneAs<Level>().Displacement.AddBurst(badelineDummy.Position, 0.5f, 24f, 96f, 1f, (Ease.Easer) null, (Ease.Easer) null);
      badelineDummy.Sprite.Play("dreamDashLoop", false, false);
      Vector2 from = badelineDummy.Position;
      float p;
      for (p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime * 6f)
      {
        badelineDummy.Position = Vector2.op_Addition(from, Vector2.op_Multiply(Vector2.op_Subtraction(target, from), Ease.CubeOut(p)));
        yield return (object) null;
      }
      badelineDummy.Scene.Entities.FindFirst<DashBlock>().Break(badelineDummy.Position, new Vector2(0.0f, -1f), false);
      badelineDummy.Sprite.Play("idle", false, false);
      for (p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime * 4f)
      {
        badelineDummy.Position = Vector2.op_Addition(target, Vector2.op_Multiply(Vector2.op_Subtraction(from, target), Ease.CubeOut(p)));
        yield return (object) null;
      }
      badelineDummy.Sprite.Play("fallSlow", false, false);
    }

    public override void Update()
    {
      if (this.Sprite.Scale.X != 0.0)
        this.Hair.Facing = (Facings) Math.Sign((float) this.Sprite.Scale.X);
      base.Update();
    }

    public override void Render()
    {
      Vector2 renderPosition = this.Sprite.RenderPosition;
      this.Sprite.RenderPosition = this.Sprite.RenderPosition.Floor();
      base.Render();
      this.Sprite.RenderPosition = renderPosition;
    }
  }
}
