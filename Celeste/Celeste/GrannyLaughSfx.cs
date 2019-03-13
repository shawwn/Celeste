// Decompiled with JetBrains decompiler
// Type: Celeste.GrannyLaughSfx
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Monocle;

namespace Celeste
{
  public class GrannyLaughSfx : Component
  {
    private bool ready = true;
    public bool FirstPlay;
    private Sprite sprite;

    public GrannyLaughSfx(Sprite sprite)
      : base(true, false)
    {
      this.sprite = sprite;
    }

    public override void Update()
    {
      if (this.sprite.CurrentAnimationID == "laugh" && this.sprite.CurrentAnimationFrame == 0 && this.ready)
      {
        if (this.FirstPlay)
          Audio.Play("event:/char/granny/laugh_firstphrase", this.Entity.Position);
        else
          Audio.Play("event:/char/granny/laugh_oneha", this.Entity.Position);
        this.ready = false;
      }
      if (this.FirstPlay || !(this.sprite.CurrentAnimationID != "laugh") && this.sprite.CurrentAnimationFrame <= 0)
        return;
      this.ready = true;
    }
  }
}
