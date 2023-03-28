// Decompiled with JetBrains decompiler
// Type: Celeste.Postcard
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System.Collections;

namespace Celeste
{
  public class Postcard : Entity
  {
    private const float TextScale = 0.7f;
    private MTexture postcard;
    private VirtualRenderTarget target;
    private FancyText.Text text;
    private float alpha = 1f;
    private float scale = 1f;
    private float rotation;
    private float buttonEase;
    private string sfxEventIn;
    private string sfxEventOut;
    private Coroutine easeButtonIn;

    public Postcard(string msg, int area)
      : this(msg, "event:/ui/main/postcard_ch" + (object) area + "_in", "event:/ui/main/postcard_ch" + (object) area + "_out")
    {
    }

    public Postcard(string msg, string sfxEventIn, string sfxEventOut)
    {
      this.Visible = false;
      this.Tag = (int) Tags.HUD;
      this.sfxEventIn = sfxEventIn;
      this.sfxEventOut = sfxEventOut;
      this.postcard = GFX.Gui[nameof (postcard)];
      this.text = FancyText.Parse(msg, (int) ((double) (this.postcard.Width - 120) / 0.699999988079071), -1, defaultColor: new Color?(Color.Black * 0.6f));
    }

    public IEnumerator DisplayRoutine()
    {
      yield return (object) this.EaseIn();
      yield return (object) 0.75f;
      while (!Input.MenuConfirm.Pressed)
        yield return (object) null;
      Audio.Play("event:/ui/main/button_lowkey");
      yield return (object) this.EaseOut();
      yield return (object) 1.2f;
    }

    public IEnumerator EaseIn()
    {
      Postcard postcard = this;
      Audio.Play(postcard.sfxEventIn);
      Vector2 vector2 = new Vector2((float) Engine.Width, (float) Engine.Height) / 2f;
      Vector2 from = vector2 + new Vector2(0.0f, 200f);
      Vector2 to = vector2;
      float rFrom = -0.1f;
      float rTo = 0.05f;
      postcard.Visible = true;
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime * 0.8f)
      {
        postcard.Position = from + (to - from) * Ease.CubeOut(p);
        postcard.alpha = Ease.CubeOut(p);
        postcard.rotation = rFrom + (rTo - rFrom) * Ease.CubeOut(p);
        yield return (object) null;
      }
      postcard.Add((Component) (postcard.easeButtonIn = new Coroutine(postcard.EaseButtinIn())));
    }

    private IEnumerator EaseButtinIn()
    {
      yield return (object) 0.75f;
      while ((double) (this.buttonEase += Engine.DeltaTime * 2f) < 1.0)
        yield return (object) null;
    }

    public IEnumerator EaseOut()
    {
      Postcard postcard = this;
      Audio.Play(postcard.sfxEventOut);
      if (postcard.easeButtonIn != null)
        postcard.easeButtonIn.RemoveSelf();
      Vector2 from = postcard.Position;
      Vector2 to = new Vector2((float) Engine.Width, (float) Engine.Height) / 2f + new Vector2(0.0f, -200f);
      float rFrom = postcard.rotation;
      float rTo = postcard.rotation + 0.1f;
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime)
      {
        postcard.Position = from + (to - from) * Ease.CubeIn(p);
        postcard.alpha = 1f - Ease.CubeIn(p);
        postcard.rotation = rFrom + (rTo - rFrom) * Ease.CubeIn(p);
        postcard.buttonEase = Calc.Approach(postcard.buttonEase, 0.0f, Engine.DeltaTime * 8f);
        yield return (object) null;
      }
      postcard.alpha = 0.0f;
      postcard.RemoveSelf();
    }

    public void BeforeRender()
    {
      if (this.target == null)
        this.target = VirtualContent.CreateRenderTarget("postcard", this.postcard.Width, this.postcard.Height);
      Engine.Graphics.GraphicsDevice.SetRenderTarget((RenderTarget2D) this.target);
      Engine.Graphics.GraphicsDevice.Clear(Color.Transparent);
      Draw.SpriteBatch.Begin();
      string text = Dialog.Clean("FILE_DEFAULT");
      if (SaveData.Instance != null && Dialog.Language.CanDisplay(SaveData.Instance.Name))
        text = SaveData.Instance.Name;
      this.postcard.Draw(Vector2.Zero);
      ActiveFont.Draw(text, new Vector2(115f, 30f), Vector2.Zero, Vector2.One * 0.9f, Color.Black * 0.7f);
      this.text.DrawJustifyPerLine(new Vector2((float) this.postcard.Width, (float) this.postcard.Height) / 2f + new Vector2(0.0f, 40f), new Vector2(0.5f, 0.5f), Vector2.One * 0.7f, 1f);
      Draw.SpriteBatch.End();
    }

    public override void Render()
    {
      if (this.target != null)
        Draw.SpriteBatch.Draw((Texture2D) (RenderTarget2D) this.target, this.Position, new Rectangle?(this.target.Bounds), Color.White * this.alpha, this.rotation, new Vector2((float) this.target.Width, (float) this.target.Height) / 2f, this.scale, SpriteEffects.None, 0.0f);
      if ((double) this.buttonEase <= 0.0)
        return;
      Input.GuiButton(Input.MenuConfirm).DrawCentered(new Vector2((float) (Engine.Width - 120), (float) (Engine.Height - 100) - 20f * Ease.CubeOut(this.buttonEase)), Color.White * Ease.CubeOut(this.buttonEase));
    }

    public override void Removed(Scene scene)
    {
      this.Dispose();
      base.Removed(scene);
    }

    public override void SceneEnd(Scene scene)
    {
      this.Dispose();
      base.SceneEnd(scene);
    }

    private void Dispose()
    {
      if (this.target != null)
        this.target.Dispose();
      this.target = (VirtualRenderTarget) null;
    }
  }
}
