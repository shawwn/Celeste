// Decompiled with JetBrains decompiler
// Type: Celeste.FormationBackdrop
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  public class FormationBackdrop : Entity
  {
    public bool Display = false;
    public float Alpha = 1f;
    private float fade = 0.0f;
    private bool wasDisplayed;

    public FormationBackdrop()
    {
      this.Tag = (int) Tags.FrozenUpdate | (int) Tags.Global;
      this.Depth = -1999900;
    }

    public override void Update()
    {
      this.fade = Calc.Approach(this.fade, this.Display ? 1f : 0.0f, Engine.RawDeltaTime * 3f);
      if (this.Display)
        this.wasDisplayed = true;
      if (this.wasDisplayed)
      {
        Level scene = this.Scene as Level;
        Snow snow = scene.Foreground.Get<Snow>();
        if (snow != null)
          snow.Alpha = 1f - this.fade;
        WindSnowFG windSnowFg = scene.Foreground.Get<WindSnowFG>();
        if (windSnowFg != null)
          windSnowFg.Alpha = 1f - this.fade;
        if ((double) this.fade <= 0.0)
          this.wasDisplayed = false;
      }
      base.Update();
    }

    public override void Render()
    {
      Level scene = this.Scene as Level;
      if ((double) this.fade <= 0.0)
        return;
      Draw.Rect(scene.Camera.Left - 1f, scene.Camera.Top - 1f, 322f, 182f, Color.Black * this.fade * this.Alpha * 0.85f);
    }
  }
}

