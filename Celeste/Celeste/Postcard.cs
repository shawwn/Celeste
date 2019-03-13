// Decompiled with JetBrains decompiler
// Type: Celeste.Postcard
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System.Collections;

namespace Celeste
{
  public class Postcard : Entity
  {
    private float alpha = 1f;
    private float scale = 1f;
    private float rotation = 0.0f;
    private float buttonEase = 0.0f;
    private const float TextScale = 0.7f;
    private MTexture postcard;
    private VirtualRenderTarget target;
    private FancyText.Text text;
    private int area;
    private Coroutine easeButtonIn;

    public Postcard(string msg, int area = -1)
    {
      this.Visible = false;
      this.Tag = (int) Tags.HUD;
      this.area = area;
      this.postcard = GFX.Gui[nameof (postcard)];
      this.text = FancyText.Parse(msg, (int) ((double) (this.postcard.Width - 120) / 0.699999988079071), -1, 1f, new Color?(Color.Black * 0.6f), (Language) null);
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
      if (this.area >= 0)
        Audio.Play("event:/ui/main/postcard_ch" + (object) this.area + "_in");
      else
        Audio.Play("event:/ui/main/postcard_csides_in");
      Vector2 center = new Vector2((float) Engine.Width, (float) Engine.Height) / 2f;
      Vector2 from = center + new Vector2(0.0f, 200f);
      Vector2 to = center;
      float rFrom = -0.1f;
      float rTo = 0.05f;
      this.Visible = true;
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime * 0.8f)
      {
        this.Position = from + (to - from) * Ease.CubeOut(p);
        this.alpha = Ease.CubeOut(p);
        this.rotation = rFrom + (rTo - rFrom) * Ease.CubeOut(p);
        yield return (object) null;
      }
      this.Add((Component) (this.easeButtonIn = new Coroutine(this.EaseButtinIn(), true)));
    }

    private IEnumerator EaseButtinIn()
    {
      yield return (object) 0.75f;
      while ((double) (this.buttonEase += Engine.DeltaTime * 2f) < 1.0)
        yield return (object) null;
    }

    public IEnumerator EaseOut()
    {
      if (this.area >= 0)
        Audio.Play("event:/ui/main/postcard_ch" + (object) this.area + "_out");
      else
        Audio.Play("event:/ui/main/postcard_csides_out");
      if (this.easeButtonIn != null)
        this.easeButtonIn.RemoveSelf();
      Vector2 from = this.Position;
      Vector2 to = new Vector2((float) Engine.Width, (float) Engine.Height) / 2f + new Vector2(0.0f, -200f);
      float rFrom = this.rotation;
      float rTo = this.rotation + 0.1f;
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime)
      {
        this.Position = from + (to - from) * Ease.CubeIn(p);
        this.alpha = 1f - Ease.CubeIn(p);
        this.rotation = rFrom + (rTo - rFrom) * Ease.CubeIn(p);
        this.buttonEase = Calc.Approach(this.buttonEase, 0.0f, Engine.DeltaTime * 8f);
        yield return (object) null;
      }
      this.alpha = 0.0f;
      this.RemoveSelf();
    }

    public void BeforeRender()
    {
      if (this.target == null)
        this.target = VirtualContent.CreateRenderTarget("postcard", this.postcard.Width, this.postcard.Height, false, true, 0);
      Engine.Graphics.GraphicsDevice.SetRenderTarget((RenderTarget2D) this.target);
      Engine.Graphics.GraphicsDevice.Clear(Color.Transparent);
      Draw.SpriteBatch.Begin();
      this.postcard.Draw(Vector2.Zero);
      ActiveFont.Draw(SaveData.Instance != null ? SaveData.Instance.Name : Dialog.Clean("FILE_DEFAULT", (Language) null), new Vector2(115f, 30f), Vector2.Zero, Vector2.One * 0.9f, Color.Black * 0.7f);
      this.text.DrawJustifyPerLine(new Vector2((float) this.postcard.Width, (float) this.postcard.Height) / 2f + new Vector2(0.0f, 40f), new Vector2(0.5f, 0.5f), Vector2.One * 0.7f, 1f, 0, int.MaxValue);
      Draw.SpriteBatch.End();
    }

    public override void Render()
    {
      if (this.target != null)
        Draw.SpriteBatch.Draw((Texture2D) (RenderTarget2D) this.target, this.Position, new Rectangle?(this.target.Bounds), Color.White * this.alpha, this.rotation, new Vector2((float) this.target.Width, (float) this.target.Height) / 2f, this.scale, SpriteEffects.None, 0.0f);
      if ((double) this.buttonEase <= 0.0)
        return;
      Input.GuiButton(Input.MenuConfirm, "controls/keyboard/oemquestion").DrawCentered(new Vector2((float) (Engine.Width - 120), (float) (Engine.Height - 100) - 20f * Ease.CubeOut(this.buttonEase)), Color.White * Ease.CubeOut(this.buttonEase));
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

