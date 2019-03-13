// Decompiled with JetBrains decompiler
// Type: Celeste.RotateSpinner
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  public class RotateSpinner : Entity
  {
    public bool Moving = true;
    private const float RotationTime = 1.8f;
    private Vector2 center;
    private float rotationPercent;
    private float length;
    private bool fallOutOfScreen;

    public float Angle
    {
      get
      {
        return MathHelper.Lerp(4.712389f, -1.570796f, this.Easer(this.rotationPercent));
      }
    }

    public bool Clockwise { get; private set; }

    public RotateSpinner(EntityData data, Vector2 offset)
      : base(Vector2.op_Addition(data.Position, offset))
    {
      this.Depth = -50;
      this.center = Vector2.op_Addition(data.Nodes[0], offset);
      this.Clockwise = data.Bool("clockwise", false);
      this.Collider = (Collider) new Monocle.Circle(6f, 0.0f, 0.0f);
      this.Add((Component) new PlayerCollider(new Action<Player>(this.OnPlayer), (Collider) null, (Collider) null));
      this.Add((Component) new StaticMover()
      {
        SolidChecker = (Func<Solid, bool>) (s => s.CollidePoint(this.center)),
        JumpThruChecker = (Func<JumpThru, bool>) (jt => jt.CollidePoint(this.center)),
        OnMove = (Action<Vector2>) (v =>
        {
          this.center = Vector2.op_Addition(this.center, v);
          this.Position = Vector2.op_Addition(this.Position, v);
        }),
        OnDestroy = (Action) (() => this.fallOutOfScreen = true)
      });
      this.rotationPercent = this.EaserInverse(Calc.Percent(Calc.WrapAngle(Calc.Angle(this.center, this.Position)), -1.570796f, 4.712389f));
      Vector2 vector2 = Vector2.op_Subtraction(this.Position, this.center);
      this.length = ((Vector2) ref vector2).Length();
      this.Position = Vector2.op_Addition(this.center, Calc.AngleToVector(this.Angle, this.length));
    }

    private float Easer(float v)
    {
      return v;
    }

    private float EaserInverse(float v)
    {
      return v;
    }

    public override void Update()
    {
      base.Update();
      if (this.Moving)
      {
        if (this.Scene.OnInterval(0.02f))
          this.SceneAs<Level>().ParticlesBG.Emit(DustStaticSpinner.P_Move, 1, this.Position, Vector2.op_Multiply(Vector2.get_One(), 4f));
        if (this.Clockwise)
        {
          this.rotationPercent -= Engine.DeltaTime / 1.8f;
          ++this.rotationPercent;
        }
        else
          this.rotationPercent += Engine.DeltaTime / 1.8f;
        this.rotationPercent %= 1f;
        this.Position = Vector2.op_Addition(this.center, Calc.AngleToVector(this.Angle, this.length));
      }
      if (!this.fallOutOfScreen)
        return;
      ref __Null local = ref this.center.Y;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local = ^(float&) ref local + 160f * Engine.DeltaTime;
      double y = (double) this.Y;
      Rectangle bounds = (this.Scene as Level).Bounds;
      double num = (double) (((Rectangle) ref bounds).get_Bottom() + 32);
      if (y <= num)
        return;
      this.RemoveSelf();
    }

    public virtual void OnPlayer(Player player)
    {
      if (player.Die(Vector2.op_Subtraction(player.Position, this.Position).SafeNormalize(), false, true) == null)
        return;
      this.Moving = false;
    }
  }
}
