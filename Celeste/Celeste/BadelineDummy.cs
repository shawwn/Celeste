// Decompiled with JetBrains decompiler
// Type: Celeste.BadelineDummy
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class BadelineDummy : Entity
  {
    public PlayerSprite Sprite;
    public PlayerHair Hair;
    public BadelineAutoAnimator AutoAnimator;
    public SineWave Wave;
    public VertexLight Light;
    public float FloatSpeed = 120f;
    public float FloatAccel = 240f;
    public float Floatness = 2f;
    public Vector2 floatNormal = new Vector2(0.0f, 1f);

    public BadelineDummy(Vector2 position)
      : base(position)
    {
      this.Collider = (Collider) new Hitbox(6f, 6f, -3f, -7f);
      this.Sprite = new PlayerSprite(PlayerSpriteMode.Badeline);
      this.Sprite.Play("fallSlow");
      this.Sprite.Scale.X = -1f;
      this.Hair = new PlayerHair(this.Sprite);
      this.Hair.Color = BadelineOldsite.HairColor;
      this.Hair.Border = Color.Black;
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
      this.Wave.OnUpdate = (Action<float>) (f => this.Sprite.Position = this.floatNormal * f * this.Floatness);
      this.Add((Component) (this.Light = new VertexLight(new Vector2(0.0f, -8f), Color.PaleVioletRed, 1f, 20, 60)));
    }

    public void Appear(Level level, bool silent = false)
    {
      if (!silent)
      {
        Audio.Play("event:/char/badeline/appear", this.Position);
        Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
      }
      level.Displacement.AddBurst(this.Center, 0.5f, 24f, 96f, 0.4f);
      level.Particles.Emit(BadelineOldsite.P_Vanish, 12, this.Center, Vector2.One * 6f);
    }

    public void Vanish()
    {
      Audio.Play("event:/char/badeline/disappear", this.Position);
      this.Shockwave();
      this.SceneAs<Level>().Particles.Emit(BadelineOldsite.P_Vanish, 12, this.Center, Vector2.One * 6f);
      this.RemoveSelf();
    }

    private void Shockwave() => this.SceneAs<Level>().Displacement.AddBurst(this.Center, 0.5f, 24f, 96f, 0.4f);

    public IEnumerator FloatTo(
      Vector2 target,
      int? turnAtEndTo = null,
      bool faceDirection = true,
      bool fadeLight = false,
      bool quickEnd = false)
    {
      BadelineDummy badelineDummy = this;
      badelineDummy.Sprite.Play("fallSlow");
      if (faceDirection && Math.Sign(target.X - badelineDummy.X) != 0)
        badelineDummy.Sprite.Scale.X = (float) Math.Sign(target.X - badelineDummy.X);
      Vector2 vector2 = (target - badelineDummy.Position).SafeNormalize();
      Vector2 perp = new Vector2(-vector2.Y, vector2.X);
      float speed = 0.0f;
      while (badelineDummy.Position != target)
      {
        speed = Calc.Approach(speed, badelineDummy.FloatSpeed, badelineDummy.FloatAccel * Engine.DeltaTime);
        badelineDummy.Position = Calc.Approach(badelineDummy.Position, target, speed * Engine.DeltaTime);
        badelineDummy.Floatness = Calc.Approach(badelineDummy.Floatness, 4f, 8f * Engine.DeltaTime);
        badelineDummy.floatNormal = Calc.Approach(badelineDummy.floatNormal, perp, Engine.DeltaTime * 12f);
        if (fadeLight)
          badelineDummy.Light.Alpha = Calc.Approach(badelineDummy.Light.Alpha, 0.0f, Engine.DeltaTime * 2f);
        yield return (object) null;
      }
      if (quickEnd)
      {
        badelineDummy.Floatness = 2f;
      }
      else
      {
        while ((double) badelineDummy.Floatness != 2.0)
        {
          badelineDummy.Floatness = Calc.Approach(badelineDummy.Floatness, 2f, 8f * Engine.DeltaTime);
          yield return (object) null;
        }
      }
      if (turnAtEndTo.HasValue)
        badelineDummy.Sprite.Scale.X = (float) turnAtEndTo.Value;
    }

    public IEnumerator WalkTo(float x, float speed = 64f)
    {
      BadelineDummy badelineDummy = this;
      badelineDummy.Floatness = 0.0f;
      badelineDummy.Sprite.Play("walk");
      if (Math.Sign(x - badelineDummy.X) != 0)
        badelineDummy.Sprite.Scale.X = (float) Math.Sign(x - badelineDummy.X);
      while ((double) badelineDummy.X != (double) x)
      {
        badelineDummy.X = Calc.Approach(badelineDummy.X, x, Engine.DeltaTime * speed);
        yield return (object) null;
      }
      badelineDummy.Sprite.Play("idle");
    }

    public IEnumerator SmashBlock(Vector2 target)
    {
      BadelineDummy badelineDummy = this;
      badelineDummy.SceneAs<Level>().Displacement.AddBurst(badelineDummy.Position, 0.5f, 24f, 96f);
      badelineDummy.Sprite.Play("dreamDashLoop");
      Vector2 from = badelineDummy.Position;
      float p;
      for (p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime * 6f)
      {
        badelineDummy.Position = from + (target - from) * Ease.CubeOut(p);
        yield return (object) null;
      }
      badelineDummy.Scene.Entities.FindFirst<DashBlock>().Break(badelineDummy.Position, new Vector2(0.0f, -1f), false);
      badelineDummy.Sprite.Play("idle");
      for (p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime * 4f)
      {
        badelineDummy.Position = target + (from - target) * Ease.CubeOut(p);
        yield return (object) null;
      }
      badelineDummy.Sprite.Play("fallSlow");
    }

    public override void Update()
    {
      if ((double) this.Sprite.Scale.X != 0.0)
        this.Hair.Facing = (Facings) Math.Sign(this.Sprite.Scale.X);
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
