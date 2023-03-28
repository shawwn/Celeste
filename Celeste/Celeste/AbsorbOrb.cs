// Decompiled with JetBrains decompiler
// Type: Celeste.AbsorbOrb
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  public class AbsorbOrb : Entity
  {
    public Entity AbsorbInto;
    public Vector2? AbsorbTarget;
    private SimpleCurve curve;
    private float duration;
    private float percent;
    private float consumeDelay;
    private float burstSpeed;
    private Vector2 burstDirection;
    private Vector2 burstScale;
    private float alpha = 1f;
    private Monocle.Image sprite;
    private BloomPoint bloom;

    public AbsorbOrb(Vector2 position, Entity into = null, Vector2? absorbTarget = null)
    {
      this.AbsorbInto = into;
      this.AbsorbTarget = absorbTarget;
      this.Position = position;
      this.Tag = (int) Tags.FrozenUpdate;
      this.Depth = -2000000;
      this.consumeDelay = (float) (0.699999988079071 + (double) Calc.Random.NextFloat() * 0.30000001192092896);
      this.burstSpeed = (float) (80.0 + (double) Calc.Random.NextFloat() * 40.0);
      this.burstDirection = Calc.AngleToVector(Calc.Random.NextFloat() * 6.2831855f, 1f);
      this.Add((Component) (this.sprite = new Monocle.Image(GFX.Game["collectables/heartGem/orb"])));
      this.sprite.CenterOrigin();
      this.Add((Component) (this.bloom = new BloomPoint(1f, 16f)));
    }

    public override void Update()
    {
      base.Update();
      Vector2 vector2_1 = Vector2.Zero;
      bool flag = false;
      if (this.AbsorbInto != null)
      {
        vector2_1 = this.AbsorbInto.Center;
        flag = this.AbsorbInto.Scene == null || this.AbsorbInto is Player && (this.AbsorbInto as Player).Dead;
      }
      else if (this.AbsorbTarget.HasValue)
      {
        vector2_1 = this.AbsorbTarget.Value;
      }
      else
      {
        Player entity = this.Scene.Tracker.GetEntity<Player>();
        if (entity != null)
          vector2_1 = entity.Center;
        flag = entity == null || entity.Scene == null || entity.Dead;
      }
      if (flag)
      {
        this.Position = this.Position + this.burstDirection * this.burstSpeed * Engine.RawDeltaTime;
        this.burstSpeed = Calc.Approach(this.burstSpeed, 800f, Engine.RawDeltaTime * 200f);
        this.sprite.Rotation = this.burstDirection.Angle();
        this.sprite.Scale = new Vector2(Math.Min(2f, (float) (0.5 + (double) this.burstSpeed * 0.019999999552965164)), Math.Max(0.05f, (float) (0.5 - (double) this.burstSpeed * 0.004000000189989805)));
        this.sprite.Color = Color.White * (this.alpha = Calc.Approach(this.alpha, 0.0f, Engine.DeltaTime));
      }
      else if ((double) this.consumeDelay > 0.0)
      {
        this.Position = this.Position + this.burstDirection * this.burstSpeed * Engine.RawDeltaTime;
        this.burstSpeed = Calc.Approach(this.burstSpeed, 0.0f, Engine.RawDeltaTime * 120f);
        this.sprite.Rotation = this.burstDirection.Angle();
        this.sprite.Scale = new Vector2(Math.Min(2f, (float) (0.5 + (double) this.burstSpeed * 0.019999999552965164)), Math.Max(0.05f, (float) (0.5 - (double) this.burstSpeed * 0.004000000189989805)));
        this.consumeDelay -= Engine.RawDeltaTime;
        if ((double) this.consumeDelay > 0.0)
          return;
        Vector2 position = this.Position;
        Vector2 end = vector2_1;
        Vector2 vector2_2 = (position + end) / 2f;
        Vector2 vector2_3 = (end - position).SafeNormalize().Perpendicular() * (position - end).Length() * (float) (0.05000000074505806 + (double) Calc.Random.NextFloat() * 0.44999998807907104);
        float num1 = end.X - position.X;
        float num2 = end.Y - position.Y;
        if ((double) Math.Abs(num1) > (double) Math.Abs(num2) && Math.Sign(vector2_3.X) != Math.Sign(num1) || (double) Math.Abs(num2) > (double) Math.Abs(num2) && Math.Sign(vector2_3.Y) != Math.Sign(num2))
          vector2_3 *= -1f;
        this.curve = new SimpleCurve(position, end, vector2_2 + vector2_3);
        this.duration = 0.3f + Calc.Random.NextFloat(0.25f);
        this.burstScale = this.sprite.Scale;
      }
      else
      {
        this.curve.End = vector2_1;
        if ((double) this.percent >= 1.0)
          this.RemoveSelf();
        this.percent = Calc.Approach(this.percent, 1f, Engine.RawDeltaTime / this.duration);
        float percent = Ease.CubeIn(this.percent);
        this.Position = this.curve.GetPoint(percent);
        float num = Calc.YoYo(percent) * this.curve.GetLengthParametric(10);
        this.sprite.Scale = new Vector2(Math.Min(2f, (float) (0.5 + (double) num * 0.019999999552965164)), Math.Max(0.05f, (float) (0.5 - (double) num * 0.004000000189989805)));
        this.sprite.Color = Color.White * (1f - percent);
        this.sprite.Rotation = Calc.Angle(this.Position, this.curve.GetPoint(Ease.CubeIn(this.percent + 0.01f)));
      }
    }
  }
}
