// Decompiled with JetBrains decompiler
// Type: Celeste.LightFadeTrigger
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;

namespace Celeste
{
  public class LightFadeTrigger : Trigger
  {
    public float LightAddFrom;
    public float LightAddTo;
    public Trigger.PositionModes PositionMode;

    public LightFadeTrigger(EntityData data, Vector2 offset)
      : base(data, offset)
    {
      this.AddTag((int) Tags.TransitionUpdate);
      this.LightAddFrom = data.Float("lightAddFrom", 0.0f);
      this.LightAddTo = data.Float("lightAddTo", 0.0f);
      this.PositionMode = data.Enum<Trigger.PositionModes>("positionMode", Trigger.PositionModes.NoEffect);
    }

    public override void OnStay(Player player)
    {
      Level scene = this.Scene as Level;
      Session session = scene.Session;
      float num1 = this.LightAddFrom + (this.LightAddTo - this.LightAddFrom) * MathHelper.Clamp(this.GetPositionLerp(player, this.PositionMode), 0.0f, 1f);
      double num2 = (double) num1;
      session.LightingAlphaAdd = (float) num2;
      scene.Lighting.Alpha = scene.BaseLightingAlpha + num1;
    }
  }
}
