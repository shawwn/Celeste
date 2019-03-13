// Decompiled with JetBrains decompiler
// Type: Celeste.LevelUpEffect
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  public class LevelUpEffect : Entity
  {
    private Sprite sprite;

    public LevelUpEffect(Vector2 position)
      : base(position)
    {
      this.Depth = -1000000;
      Audio.Play("event:/game/06_reflection/hug_levelup_text_in", this.Position);
      this.Add((Component) (this.sprite = GFX.SpriteBank.Create("player_level_up")));
      this.sprite.OnLastFrame = (Action<string>) (anim => this.RemoveSelf());
      this.sprite.OnFrameChange = (Action<string>) (anim =>
      {
        if (this.sprite.CurrentAnimationFrame != 20)
          return;
        Audio.Play("event:/game/06_reflection/hug_levelup_text_out");
      });
      this.sprite.Play("levelUp", false, false);
    }
  }
}
