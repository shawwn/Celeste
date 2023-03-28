// Decompiled with JetBrains decompiler
// Type: Celeste.WaveDashPage01
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System.Collections;

namespace Celeste
{
  public class WaveDashPage01 : WaveDashPage
  {
    private AreaCompleteTitle title;
    private float subtitleEase;

    public WaveDashPage01()
    {
      this.Transition = WaveDashPage.Transitions.ScaleIn;
      this.ClearColor = Calc.HexToColor("9fc5e8");
    }

    public override void Added(WaveDashPresentation presentation) => base.Added(presentation);

    public override IEnumerator Routine()
    {
      WaveDashPage01 waveDashPage01 = this;
      Audio.SetAltMusic("event:/new_content/music/lvl10/intermission_powerpoint");
      yield return (object) 1f;
      waveDashPage01.title = new AreaCompleteTitle(new Vector2((float) waveDashPage01.Width / 2f, (float) ((double) waveDashPage01.Height / 2.0 - 100.0)), Dialog.Clean("WAVEDASH_PAGE1_TITLE"), 2f, true);
      yield return (object) 1f;
      while ((double) waveDashPage01.subtitleEase < 1.0)
      {
        waveDashPage01.subtitleEase = Calc.Approach(waveDashPage01.subtitleEase, 1f, Engine.DeltaTime);
        yield return (object) null;
      }
      yield return (object) 0.1f;
    }

    public override void Update()
    {
      if (this.title == null)
        return;
      this.title.Update();
    }

    public override void Render()
    {
      if (this.title != null)
        this.title.Render();
      if ((double) this.subtitleEase <= 0.0)
        return;
      Vector2 position = new Vector2((float) this.Width / 2f, (float) ((double) this.Height / 2.0 + 80.0));
      float x = (float) (1.0 + (double) Ease.BigBackIn(1f - this.subtitleEase) * 2.0);
      float y = (float) (0.25 + (double) Ease.BigBackIn(this.subtitleEase) * 0.75);
      ActiveFont.Draw(Dialog.Clean("WAVEDASH_PAGE1_SUBTITLE"), position, new Vector2(0.5f, 0.5f), new Vector2(x, y), Color.Black * 0.8f);
    }
  }
}
