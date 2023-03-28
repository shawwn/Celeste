// Decompiled with JetBrains decompiler
// Type: Celeste.WaveDashPage00
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Globalization;

namespace Celeste
{
  public class WaveDashPage00 : WaveDashPage
  {
    private Color taskbarColor = Calc.HexToColor("d9d3b1");
    private string time;
    private Vector2 pptIcon;
    private Vector2 cursor;
    private bool selected;

    public WaveDashPage00()
    {
      this.AutoProgress = true;
      this.ClearColor = Calc.HexToColor("118475");
      this.time = DateTime.Now.ToString("h:mm tt", (IFormatProvider) CultureInfo.CreateSpecificCulture("en-US"));
      this.pptIcon = new Vector2(600f, 500f);
      this.cursor = new Vector2(1000f, 700f);
    }

    public override IEnumerator Routine()
    {
      WaveDashPage00 waveDashPage00 = this;
      yield return (object) 1f;
      yield return (object) waveDashPage00.MoveCursor(waveDashPage00.cursor + new Vector2(0.0f, -80f), 0.3f);
      yield return (object) 0.2f;
      yield return (object) waveDashPage00.MoveCursor(waveDashPage00.pptIcon, 0.8f);
      yield return (object) 0.7f;
      waveDashPage00.selected = true;
      Audio.Play("event:/new_content/game/10_farewell/ppt_doubleclick");
      yield return (object) 0.1f;
      waveDashPage00.selected = false;
      yield return (object) 0.1f;
      waveDashPage00.selected = true;
      yield return (object) 0.08f;
      waveDashPage00.selected = false;
      yield return (object) 0.5f;
      waveDashPage00.Presentation.ScaleInPoint = waveDashPage00.pptIcon;
    }

    private IEnumerator MoveCursor(Vector2 to, float time)
    {
      Vector2 from = this.cursor;
      for (float t = 0.0f; (double) t < 1.0; t += Engine.DeltaTime / time)
      {
        this.cursor = from + (to - from) * Ease.SineOut(t);
        yield return (object) null;
      }
    }

    public override void Update()
    {
    }

    public override void Render()
    {
      this.DrawIcon(new Vector2(160f, 120f), "desktop/mymountain_icon", Dialog.Clean("WAVEDASH_DESKTOP_MYPC"));
      this.DrawIcon(new Vector2(160f, 320f), "desktop/recyclebin_icon", Dialog.Clean("WAVEDASH_DESKTOP_RECYCLEBIN"));
      this.DrawIcon(this.pptIcon, "desktop/wavedashing_icon", Dialog.Clean("WAVEDASH_DESKTOP_POWERPOINT"));
      this.DrawTaskbar();
      this.Presentation.Gfx["desktop/cursor"].DrawCentered(this.cursor);
    }

    public void DrawTaskbar()
    {
      Draw.Rect(0.0f, (float) this.Height - 80f, (float) this.Width, 80f, this.taskbarColor);
      Draw.Rect(0.0f, (float) this.Height - 80f, (float) this.Width, 4f, Color.White * 0.5f);
      MTexture mtexture = this.Presentation.Gfx["desktop/startberry"];
      float height = 64f;
      float num1 = (float) ((double) height / (double) mtexture.Height * 0.699999988079071);
      string text = Dialog.Clean("WAVEDASH_DESKTOP_STARTBUTTON");
      float num2 = 0.6f;
      float width = (float) ((double) mtexture.Width * (double) num1 + (double) ActiveFont.Measure(text).X * (double) num2 + 32.0);
      Vector2 vector2 = new Vector2(8f, (float) ((double) this.Height - 80.0 + 8.0));
      Draw.Rect(vector2.X, vector2.Y, width, height, Color.White * 0.5f);
      mtexture.DrawJustified(vector2 + new Vector2(8f, height / 2f), new Vector2(0.0f, 0.5f), Color.White, Vector2.One * num1);
      ActiveFont.Draw(text, vector2 + new Vector2((float) ((double) mtexture.Width * (double) num1 + 16.0), height / 2f), new Vector2(0.0f, 0.5f), Vector2.One * num2, Color.Black * 0.8f);
      ActiveFont.Draw(this.time, new Vector2((float) this.Width - 24f, (float) this.Height - 40f), new Vector2(1f, 0.5f), Vector2.One * 0.6f, Color.Black * 0.8f);
    }

    private void DrawIcon(Vector2 position, string icon, string text)
    {
      bool flag = (double) this.cursor.X > (double) position.X - 64.0 && (double) this.cursor.Y > (double) position.Y - 64.0 && (double) this.cursor.X < (double) position.X + 64.0 && (double) this.cursor.Y < (double) position.Y + 80.0;
      if (this.selected & flag)
        Draw.Rect(position.X - 80f, position.Y - 80f, 160f, 200f, Color.White * 0.25f);
      if (flag)
        this.DrawDottedRect(position.X - 80f, position.Y - 80f, 160f, 200f);
      MTexture mtexture = this.Presentation.Gfx[icon];
      float scale = 128f / (float) mtexture.Height;
      mtexture.DrawCentered(position, Color.White, scale);
      ActiveFont.Draw(text, position + new Vector2(0.0f, 80f), new Vector2(0.5f, 0.0f), Vector2.One * 0.6f, this.selected & flag ? Color.Black : Color.White);
    }

    private void DrawDottedRect(float x, float y, float w, float h)
    {
      float num1 = 4f;
      Draw.Rect(x, y, w, num1, Color.White);
      Draw.Rect(x + w - num1, y, num1, h, Color.White);
      Draw.Rect(x, y, num1, h, Color.White);
      Draw.Rect(x, y + h - num1, w, num1, Color.White);
      if (this.selected)
        return;
      for (float num2 = 4f; (double) num2 < (double) w; num2 += num1 * 2f)
      {
        Draw.Rect(x + num2, y, num1, num1, this.ClearColor);
        Draw.Rect(x + w - num2, y + h - num1, num1, num1, this.ClearColor);
      }
      for (float num3 = 4f; (double) num3 < (double) h; num3 += num1 * 2f)
      {
        Draw.Rect(x, y + num3, num1, num1, this.ClearColor);
        Draw.Rect(x + w - num1, y + h - num3, num1, num1, this.ClearColor);
      }
    }
  }
}
