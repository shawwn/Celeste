// Decompiled with JetBrains decompiler
// Type: Celeste.WaveDashPage03
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System.Collections;

namespace Celeste
{
  public class WaveDashPage03 : WaveDashPage
  {
    private string title;
    private string titleDisplayed;
    private MTexture clipArt;
    private float clipArtEase;
    private FancyText.Text infoText;
    private AreaCompleteTitle easyText;

    public WaveDashPage03()
    {
      this.Transition = WaveDashPage.Transitions.Blocky;
      this.ClearColor = Calc.HexToColor("d9ead3");
      this.title = Dialog.Clean("WAVEDASH_PAGE3_TITLE");
      this.titleDisplayed = "";
    }

    public override void Added(WaveDashPresentation presentation)
    {
      base.Added(presentation);
      this.clipArt = presentation.Gfx["moveset"];
    }

    public override IEnumerator Routine()
    {
      WaveDashPage03 waveDashPage03 = this;
      while (waveDashPage03.titleDisplayed.Length < waveDashPage03.title.Length)
      {
        waveDashPage03.titleDisplayed += waveDashPage03.title[waveDashPage03.titleDisplayed.Length].ToString();
        yield return (object) 0.05f;
      }
      yield return (object) waveDashPage03.PressButton();
      Audio.Play("event:/new_content/game/10_farewell/ppt_wavedash_whoosh");
      while ((double) waveDashPage03.clipArtEase < 1.0)
      {
        waveDashPage03.clipArtEase = Calc.Approach(waveDashPage03.clipArtEase, 1f, Engine.DeltaTime);
        yield return (object) null;
      }
      yield return (object) 0.25f;
      waveDashPage03.infoText = FancyText.Parse(Dialog.Get("WAVEDASH_PAGE3_INFO"), waveDashPage03.Width - 240, 32, defaultColor: new Color?(Color.Black * 0.7f));
      yield return (object) waveDashPage03.PressButton();
      Audio.Play("event:/new_content/game/10_farewell/ppt_its_easy");
      waveDashPage03.easyText = new AreaCompleteTitle(new Vector2((float) waveDashPage03.Width / 2f, (float) (waveDashPage03.Height - 150)), Dialog.Clean("WAVEDASH_PAGE3_EASY"), 2f, true);
      yield return (object) 1f;
    }

    public override void Update()
    {
      if (this.easyText == null)
        return;
      this.easyText.Update();
    }

    public override void Render()
    {
      ActiveFont.DrawOutline(this.titleDisplayed, new Vector2(128f, 100f), Vector2.Zero, Vector2.One * 1.5f, Color.White, 2f, Color.Black);
      if ((double) this.clipArtEase > 0.0)
        this.clipArt.DrawCentered(new Vector2((float) this.Width / 2f, (float) ((double) this.Height / 2.0 - 90.0)), Color.White * this.clipArtEase, Vector2.One * (float) (1.0 + (1.0 - (double) this.clipArtEase) * 3.0) * 0.8f, (float) ((1.0 - (double) this.clipArtEase) * 8.0));
      if (this.infoText != null)
        this.infoText.Draw(new Vector2((float) this.Width / 2f, (float) (this.Height - 350)), new Vector2(0.5f, 0.0f), Vector2.One, 1f);
      if (this.easyText == null)
        return;
      this.easyText.Render();
    }
  }
}
