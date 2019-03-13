// Decompiled with JetBrains decompiler
// Type: Celeste.OuiCredits
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System.Collections;

namespace Celeste
{
  public class OuiCredits : Oui
  {
    private readonly Vector2 onScreen = new Vector2(960f, 0.0f);
    private readonly Vector2 offScreen = new Vector2(3840f, 0.0f);
    private Credits credits;
    private float vignetteAlpha;

    public override void Added(Scene scene)
    {
      base.Added(scene);
      this.Position = this.offScreen;
      this.Visible = false;
    }

    public override IEnumerator Enter(Oui from)
    {
      Audio.SetMusic("event:/music/menu/credits", true, true);
      this.Overworld.ShowConfirmUI = false;
      Credits.BorderColor = Color.Black;
      this.credits = new Credits(0.5f, 1f, true, false);
      this.credits.Enabled = false;
      this.Visible = true;
      this.vignetteAlpha = 0.0f;
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime * 4f)
      {
        this.Position = this.offScreen + (this.onScreen - this.offScreen) * Ease.CubeOut(p);
        yield return (object) null;
      }
    }

    public override IEnumerator Leave(Oui next)
    {
      Audio.Play("event:/ui/main/whoosh_large_out");
      this.Overworld.SetNormalMusic();
      this.Overworld.ShowConfirmUI = true;
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime * 4f)
      {
        this.Position = this.onScreen + (this.offScreen - this.onScreen) * Ease.CubeIn(p);
        yield return (object) null;
      }
      this.Visible = false;
    }

    public override void Update()
    {
      if (this.Focused && (Input.MenuCancel.Pressed || (double) this.credits.BottomTimer > 3.0))
        this.Overworld.Goto<OuiMainMenu>();
      if (this.credits != null)
      {
        this.credits.Update();
        this.credits.Enabled = this.Focused && this.Selected;
      }
      this.vignetteAlpha = Calc.Approach(this.vignetteAlpha, this.Selected ? 1f : 0.0f, Engine.DeltaTime * (this.Selected ? 1f : 4f));
      base.Update();
    }

    public override void Render()
    {
      if ((double) this.vignetteAlpha > 0.0)
      {
        Draw.Rect(-10f, -10f, 1940f, 1100f, Color.Black * this.vignetteAlpha * 0.4f);
        GFX.Overworld["vignette"].Draw(Vector2.Zero, Vector2.Zero, Color.White * Ease.CubeInOut(this.vignetteAlpha), 1f);
      }
      if (this.credits == null)
        return;
      this.credits.Render(this.Position);
    }
  }
}

