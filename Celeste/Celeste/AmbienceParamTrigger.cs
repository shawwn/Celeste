// Decompiled with JetBrains decompiler
// Type: Celeste.AmbienceParamTrigger
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  public class AmbienceParamTrigger : Trigger
  {
    public string Parameter;
    public float From;
    public float To;
    public Trigger.PositionModes PositionMode;

    public AmbienceParamTrigger(EntityData data, Vector2 offset)
      : base(data, offset)
    {
      this.Parameter = data.Attr("parameter", "");
      this.From = data.Float("from", 0.0f);
      this.To = data.Float("to", 0.0f);
      this.PositionMode = data.Enum<Trigger.PositionModes>("direction", Trigger.PositionModes.NoEffect);
    }

    public override void OnStay(Player player)
    {
      float num = Calc.ClampedMap(this.GetPositionLerp(player, this.PositionMode), 0.0f, 1f, this.From, this.To);
      Level scene = this.Scene as Level;
      scene.Session.Audio.Ambience.Param(this.Parameter, num);
      scene.Session.Audio.Apply();
    }
  }
}
