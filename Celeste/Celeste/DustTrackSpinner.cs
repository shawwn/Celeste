// Decompiled with JetBrains decompiler
// Type: Celeste.DustTrackSpinner
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste
{
  public class DustTrackSpinner : TrackSpinner
  {
    private DustGraphic dusty;
    private Vector2 outwards;

    public DustTrackSpinner(EntityData data, Vector2 offset)
      : base(data, offset)
    {
      this.Add((Component) (this.dusty = new DustGraphic(true, false, false)));
      this.dusty.EyeDirection = this.dusty.EyeTargetDirection = Vector2.op_Subtraction(this.End, this.Start).SafeNormalize();
      this.dusty.OnEstablish = new Action(this.Establish);
      this.Depth = -50;
    }

    private void Establish()
    {
      Vector2 vector2_1 = Vector2.op_Subtraction(this.End, this.Start).SafeNormalize();
      Vector2 vector2_2;
      ((Vector2) ref vector2_2).\u002Ector((float) -vector2_1.Y, (float) vector2_1.X);
      bool flag = this.Scene.CollideCheck<Solid>(new Rectangle((int) ((double) this.X + vector2_2.X * 4.0) - 2, (int) ((double) this.Y + vector2_2.Y * 4.0) - 2, 4, 4));
      if (!flag)
      {
        vector2_2 = Vector2.op_UnaryNegation(vector2_2);
        flag = this.Scene.CollideCheck<Solid>(new Rectangle((int) ((double) this.X + vector2_2.X * 4.0) - 2, (int) ((double) this.Y + vector2_2.Y * 4.0) - 2, 4, 4));
      }
      if (!flag)
        return;
      Vector2 vector2_3 = Vector2.op_Subtraction(this.End, this.Start);
      float num1 = ((Vector2) ref vector2_3).Length();
      for (int index = 8; (double) index < (double) num1 & flag; index += 8)
        flag = flag && this.Scene.CollideCheck<Solid>(new Rectangle((int) ((double) this.X + vector2_2.X * 4.0 + vector2_1.X * (double) index) - 2, (int) ((double) this.Y + vector2_2.Y * 4.0 + vector2_1.Y * (double) index) - 2, 4, 4));
      if (!flag)
        return;
      List<DustGraphic.Node> nodeList = (List<DustGraphic.Node>) null;
      if (vector2_2.X < 0.0)
        nodeList = this.dusty.LeftNodes;
      else if (vector2_2.X > 0.0)
        nodeList = this.dusty.RightNodes;
      else if (vector2_2.Y < 0.0)
        nodeList = this.dusty.TopNodes;
      else if (vector2_2.Y > 0.0)
        nodeList = this.dusty.BottomNodes;
      if (nodeList != null)
      {
        foreach (DustGraphic.Node node in nodeList)
          node.Enabled = false;
      }
      this.outwards = Vector2.op_UnaryNegation(vector2_2);
      DustGraphic dusty1 = this.dusty;
      dusty1.Position = Vector2.op_Subtraction(dusty1.Position, vector2_2);
      DustGraphic dusty2 = this.dusty;
      DustGraphic dusty3 = this.dusty;
      double num2 = (double) this.outwards.Angle();
      double num3 = this.Up ? (double) this.Angle + 3.14159274101257 : (double) this.Angle;
      Vector2 vector;
      vector2_3 = vector = Calc.AngleToVector(Calc.AngleLerp((float) num2, (float) num3, 0.3f), 1f);
      dusty3.EyeTargetDirection = vector;
      Vector2 vector2_4 = vector2_3;
      dusty2.EyeDirection = vector2_4;
    }

    public override void Update()
    {
      base.Update();
      if (!this.Moving || (double) this.PauseTimer >= 0.0 || !this.Scene.OnInterval(0.02f))
        return;
      this.SceneAs<Level>().ParticlesBG.Emit(DustStaticSpinner.P_Move, 1, this.Position, Vector2.op_Multiply(Vector2.get_One(), 4f));
    }

    public override void OnPlayer(Player player)
    {
      base.OnPlayer(player);
      this.dusty.OnHitPlayer();
    }

    public override void OnTrackEnd()
    {
      if (Vector2.op_Inequality(this.outwards, Vector2.get_Zero()))
      {
        this.dusty.EyeTargetDirection = Calc.AngleToVector(Calc.AngleLerp(this.outwards.Angle(), this.Up ? this.Angle + 3.141593f : this.Angle, 0.3f), 1f);
      }
      else
      {
        this.dusty.EyeTargetDirection = Calc.AngleToVector(this.Up ? this.Angle + 3.141593f : this.Angle, 1f);
        this.dusty.EyeFlip = -this.dusty.EyeFlip;
      }
    }
  }
}
