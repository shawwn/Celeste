// Decompiled with JetBrains decompiler
// Type: Celeste.AbsorbOrb
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  public class AbsorbOrb : Entity
  {
    private float alpha = 1f;
    public Entity AbsorbInto;
    private SimpleCurve curve;
    private float duration;
    private float percent;
    private float consumeDelay;
    private float burstSpeed;
    private Vector2 burstDirection;
    private Vector2 burstScale;
    private Monocle.Image sprite;
    private BloomPoint bloom;

    public AbsorbOrb(Vector2 position, Entity into = null)
    {
      this.AbsorbInto = into;
      this.Position = position;
      this.Tag = (int) Tags.FrozenUpdate;
      this.Depth = -2000000;
      this.consumeDelay = (float) (0.699999988079071 + (double) Calc.Random.NextFloat() * 0.300000011920929);
      this.burstSpeed = (float) (80.0 + (double) Calc.Random.NextFloat() * 40.0);
      this.burstDirection = Calc.AngleToVector(Calc.Random.NextFloat() * 6.283185f, 1f);
      this.Add((Component) (this.sprite = new Monocle.Image(GFX.Game["collectables/heartGem/orb"])));
      this.sprite.CenterOrigin();
      this.Add((Component) (this.bloom = new BloomPoint(1f, 16f)));
    }

    public override void Update()
    {
      base.Update();
      Entity entity = this.AbsorbInto != null ? this.AbsorbInto : (Entity) this.Scene.Tracker.GetEntity<Player>();
      if ((entity == null || entity.Scene == null ? 1 : (!(entity is Player) ? 0 : ((entity as Player).Dead ? 1 : 0))) != 0)
      {
        this.Position = Vector2.op_Addition(this.Position, Vector2.op_Multiply(Vector2.op_Multiply(this.burstDirection, this.burstSpeed), Engine.RawDeltaTime));
        this.burstSpeed = Calc.Approach(this.burstSpeed, 800f, Engine.RawDeltaTime * 200f);
        this.sprite.Rotation = this.burstDirection.Angle();
        this.sprite.Scale = new Vector2(Math.Min(2f, (float) (0.5 + (double) this.burstSpeed * 0.0199999995529652)), Math.Max(0.05f, (float) (0.5 - (double) this.burstSpeed * 0.00400000018998981)));
        this.sprite.Color = Color.op_Multiply(Color.get_White(), this.alpha = Calc.Approach(this.alpha, 0.0f, Engine.DeltaTime));
      }
      else if ((double) this.consumeDelay > 0.0)
      {
        this.Position = Vector2.op_Addition(this.Position, Vector2.op_Multiply(Vector2.op_Multiply(this.burstDirection, this.burstSpeed), Engine.RawDeltaTime));
        this.burstSpeed = Calc.Approach(this.burstSpeed, 0.0f, Engine.RawDeltaTime * 120f);
        this.sprite.Rotation = this.burstDirection.Angle();
        this.sprite.Scale = new Vector2(Math.Min(2f, (float) (0.5 + (double) this.burstSpeed * 0.0199999995529652)), Math.Max(0.05f, (float) (0.5 - (double) this.burstSpeed * 0.00400000018998981)));
        this.consumeDelay -= Engine.RawDeltaTime;
        if ((double) this.consumeDelay > 0.0)
          return;
        Vector2 position = this.Position;
        Vector2 center = entity.Center;
        Vector2 vector2_1 = Vector2.op_Division(Vector2.op_Addition(position, center), 2f);
        Vector2 vector2_2 = Vector2.op_Subtraction(center, position).SafeNormalize().Perpendicular();
        Vector2 vector2_3 = Vector2.op_Subtraction(position, center);
        double num1 = (double) ((Vector2) ref vector2_3).Length();
        Vector2 vector2_4 = Vector2.op_Multiply(Vector2.op_Multiply(vector2_2, (float) num1), (float) (0.0500000007450581 + (double) Calc.Random.NextFloat() * 0.449999988079071));
        float num2 = (float) (center.X - position.X);
        float num3 = (float) (center.Y - position.Y);
        if ((double) Math.Abs(num2) > (double) Math.Abs(num3) && Math.Sign((float) vector2_4.X) != Math.Sign(num2) || (double) Math.Abs(num3) > (double) Math.Abs(num3) && Math.Sign((float) vector2_4.Y) != Math.Sign(num3))
          vector2_4 = Vector2.op_Multiply(vector2_4, -1f);
        this.curve = new SimpleCurve(position, center, Vector2.op_Addition(vector2_1, vector2_4));
        this.duration = 0.3f + Calc.Random.NextFloat(0.25f);
        this.burstScale = this.sprite.Scale;
      }
      else
      {
        this.curve.End = entity.Center;
        if ((double) this.percent >= 1.0)
          this.RemoveSelf();
        this.percent = Calc.Approach(this.percent, 1f, Engine.RawDeltaTime / this.duration);
        float percent = Ease.CubeIn(this.percent);
        this.Position = this.curve.GetPoint(percent);
        float num = Calc.YoYo(percent) * this.curve.GetLengthParametric(10);
        this.sprite.Scale = new Vector2(Math.Min(2f, (float) (0.5 + (double) num * 0.0199999995529652)), Math.Max(0.05f, (float) (0.5 - (double) num * 0.00400000018998981)));
        this.sprite.Color = Color.op_Multiply(Color.get_White(), 1f - percent);
        this.sprite.Rotation = Calc.Angle(this.Position, this.curve.GetPoint(Ease.CubeIn(this.percent + 0.01f)));
      }
    }
  }
}
