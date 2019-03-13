// Decompiled with JetBrains decompiler
// Type: Celeste.MusicFadeTrigger
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  public class MusicFadeTrigger : Trigger
  {
    public bool LeftToRight;
    public float FadeA;
    public float FadeB;
    public string Parameter;

    public MusicFadeTrigger(EntityData data, Vector2 offset)
      : base(data, offset)
    {
      this.LeftToRight = data.Attr("direction", "leftToRight") == "leftToRight";
      this.FadeA = data.Float("fadeA", 0.0f);
      this.FadeB = data.Float("fadeB", 1f);
      this.Parameter = data.Attr("parameter", "");
    }

    public override void OnStay(Player player)
    {
      float num = !this.LeftToRight ? Calc.ClampedMap((float) player.Center.Y, this.Top, this.Bottom, this.FadeA, this.FadeB) : Calc.ClampedMap((float) player.Center.X, this.Left, this.Right, this.FadeA, this.FadeB);
      if (string.IsNullOrEmpty(this.Parameter))
        Audio.SetMusicParam("fade", num);
      else
        Audio.SetMusicParam("escape", num);
    }
  }
}
