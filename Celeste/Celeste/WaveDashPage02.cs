// Decompiled with JetBrains decompiler
// Type: Celeste.WaveDashPage02
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System.Collections;
using System.Collections.Generic;

namespace Celeste
{
  public class WaveDashPage02 : WaveDashPage
  {
    private List<WaveDashPage02.TitleText> title = new List<WaveDashPage02.TitleText>();
    private FancyText.Text list;
    private int listIndex;
    private float impossibleEase;

    public WaveDashPage02()
    {
      this.Transition = WaveDashPage.Transitions.Rotate3D;
      this.ClearColor = Calc.HexToColor("fff2cc");
    }

    public override void Added(WaveDashPresentation presentation) => base.Added(presentation);

    public override IEnumerator Routine()
    {
      WaveDashPage02 waveDashPage02 = this;
      string[] text = Dialog.Clean("WAVEDASH_PAGE2_TITLE").Split('|');
      Vector2 pos = new Vector2(128f, 128f);
      for (int i = 0; i < text.Length; ++i)
      {
        WaveDashPage02.TitleText item = new WaveDashPage02.TitleText(pos, text[i]);
        waveDashPage02.title.Add(item);
        yield return (object) item.Stamp();
        pos.X += item.Width + ActiveFont.Measure(' ').X * 1.5f;
        item = (WaveDashPage02.TitleText) null;
      }
      text = (string[]) null;
      pos = new Vector2();
      yield return (object) waveDashPage02.PressButton();
      waveDashPage02.list = FancyText.Parse(Dialog.Get("WAVEDASH_PAGE2_LIST"), waveDashPage02.Width, 32, defaultColor: new Color?(Color.Black * 0.7f));
      float delay = 0.0f;
      for (; waveDashPage02.listIndex < waveDashPage02.list.Nodes.Count; ++waveDashPage02.listIndex)
      {
        if (waveDashPage02.list.Nodes[waveDashPage02.listIndex] is FancyText.NewLine)
        {
          yield return (object) waveDashPage02.PressButton();
        }
        else
        {
          delay += 0.008f;
          if ((double) delay >= 0.01600000075995922)
          {
            delay -= 0.016f;
            yield return (object) 0.016f;
          }
        }
      }
      yield return (object) waveDashPage02.PressButton();
      Audio.Play("event:/new_content/game/10_farewell/ppt_impossible");
      while ((double) waveDashPage02.impossibleEase < 1.0)
      {
        waveDashPage02.impossibleEase = Calc.Approach(waveDashPage02.impossibleEase, 1f, Engine.DeltaTime);
        yield return (object) null;
      }
    }

    public override void Update()
    {
    }

    public override void Render()
    {
      foreach (WaveDashPage02.TitleText titleText in this.title)
        titleText.Render();
      if (this.list != null)
        this.list.Draw(new Vector2(160f, 260f), new Vector2(0.0f, 0.0f), Vector2.One, 1f, end: this.listIndex);
      if ((double) this.impossibleEase <= 0.0)
        return;
      MTexture mtexture = this.Presentation.Gfx["Guy Clip Art"];
      float scale = 0.75f;
      mtexture.Draw(new Vector2((float) this.Width - (float) mtexture.Width * scale, (float) this.Height - 640f * this.impossibleEase), Vector2.Zero, Color.White, scale);
      Matrix transformationMatrix = Matrix.CreateRotationZ((float) ((double) Ease.CubeIn(1f - this.impossibleEase) * 8.0 - 0.5)) * Matrix.CreateTranslation((float) (this.Width - 500), (float) (this.Height - 600), 0.0f);
      Draw.SpriteBatch.End();
      Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, (DepthStencilState) null, (RasterizerState) null, (Effect) null, transformationMatrix);
      ActiveFont.Draw(Dialog.Clean("WAVEDASH_PAGE2_IMPOSSIBLE"), Vector2.Zero, new Vector2(0.5f, 0.5f), Vector2.One * (float) (2.0 + (1.0 - (double) this.impossibleEase) * 0.5), Color.Black * this.impossibleEase);
      Draw.SpriteBatch.End();
      Draw.SpriteBatch.Begin();
    }

    private class TitleText
    {
      public const float Scale = 1.5f;
      public string Text;
      public Vector2 Position;
      public float Width;
      private float ease;

      public TitleText(Vector2 pos, string text)
      {
        this.Position = pos;
        this.Text = text;
        this.Width = ActiveFont.Measure(text).X * 1.5f;
      }

      public IEnumerator Stamp()
      {
        while ((double) this.ease < 1.0)
        {
          this.ease = Calc.Approach(this.ease, 1f, Engine.DeltaTime * 4f);
          yield return (object) null;
        }
        yield return (object) 0.2f;
      }

      public void Render()
      {
        if ((double) this.ease <= 0.0)
          return;
        ActiveFont.DrawOutline(this.Text, this.Position + new Vector2(this.Width / 2f, (float) ((double) ActiveFont.LineHeight * 0.5 * 1.5)), new Vector2(0.5f, 0.5f), Vector2.One * (float) (1.0 + (1.0 - (double) Ease.CubeOut(this.ease))) * 1.5f, Color.White, 2f, Color.Black);
      }
    }
  }
}
