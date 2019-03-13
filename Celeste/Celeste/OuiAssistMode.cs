// Decompiled with JetBrains decompiler
// Type: Celeste.OuiAssistMode
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Celeste
{
  public class OuiAssistMode : Oui
  {
    private List<OuiAssistMode.Page> pages = new List<OuiAssistMode.Page>();
    private int pageIndex = 0;
    private int questionIndex = 1;
    private float questionEase = 0.0f;
    private float dot = 0.0f;
    private Color iconColor = Calc.HexToColor("44adf7");
    public OuiFileSelectSlot FileSlot;
    private float fade;
    private Wiggler wiggler;
    private FancyText.Text questionText;
    private float leftArrowEase;
    private float rightArrowEase;
    private EventInstance mainSfx;
    private const float textScale = 0.8f;

    public OuiAssistMode()
    {
      this.Visible = false;
      this.Add((Component) (this.wiggler = Wiggler.Create(0.4f, 4f, (Action<float>) null, false, false)));
    }

    public override IEnumerator Enter(Oui from)
    {
      this.Focused = false;
      this.Visible = true;
      this.pageIndex = 0;
      this.questionIndex = 1;
      this.questionEase = 0.0f;
      this.dot = 0.0f;
      this.questionText = FancyText.Parse(Dialog.Get("ASSIST_ASK", (Language) null), 1600, -1, 1f, new Color?(Color.White), (Language) null);
      if (!this.FileSlot.AssistModeEnabled)
      {
        int i = 0;
        while (Dialog.Has("ASSIST_MODE_" + (object) i, (Language) null))
        {
          OuiAssistMode.Page page = new OuiAssistMode.Page();
          page.Text = FancyText.Parse(Dialog.Get("ASSIST_MODE_" + (object) i, (Language) null), 2000, -1, 1f, new Color?(Color.White * 0.9f), (Language) null);
          page.Ease = 0.0f;
          this.pages.Add(page);
          ++i;
          page = (OuiAssistMode.Page) null;
        }
        this.pages[0].Ease = 1f;
        this.mainSfx = Audio.Play("event:/ui/main/assist_info_whistle");
      }
      else
        this.questionEase = 1f;
      while ((double) this.fade < 1.0)
      {
        this.fade += Engine.DeltaTime * 4f;
        yield return (object) null;
      }
      this.Focused = true;
      this.Add((Component) new Coroutine(this.InputRoutine(), true));
    }

    public override IEnumerator Leave(Oui next)
    {
      this.Focused = false;
      while ((double) this.fade > 0.0)
      {
        this.fade -= Engine.DeltaTime * 4f;
        yield return (object) null;
      }
      if ((HandleBase) this.mainSfx != (HandleBase) null)
      {
        int num = (int) this.mainSfx.release();
      }
      this.pages.Clear();
      this.Visible = false;
    }

    private IEnumerator InputRoutine()
    {
      while (true)
      {
        if (!Input.MenuCancel.Pressed)
        {
          int was = this.pageIndex;
          if ((Input.MenuConfirm.Pressed || Input.MenuRight.Pressed) && this.pageIndex < this.pages.Count)
          {
            ++this.pageIndex;
            Audio.Play("event:/ui/main/rollover_down");
            Audio.SetParameter(this.mainSfx, "assist_progress", (float) this.pageIndex);
          }
          else if (Input.MenuLeft.Pressed && this.pageIndex > 0)
          {
            Audio.Play("event:/ui/main/rollover_up");
            --this.pageIndex;
          }
          if (was != this.pageIndex)
          {
            if (was < this.pages.Count)
            {
              this.pages[was].Direction = (float) Math.Sign(was - this.pageIndex);
              while ((double) (this.pages[was].Ease = Calc.Approach(this.pages[was].Ease, 0.0f, Engine.DeltaTime * 8f)) != 0.0)
                yield return (object) null;
            }
            else
            {
              while ((double) (this.questionEase = Calc.Approach(this.questionEase, 0.0f, Engine.DeltaTime * 8f)) != 0.0)
                yield return (object) null;
            }
            if (this.pageIndex < this.pages.Count)
            {
              this.pages[this.pageIndex].Direction = (float) Math.Sign(this.pageIndex - was);
              while ((double) (this.pages[this.pageIndex].Ease = Calc.Approach(this.pages[this.pageIndex].Ease, 1f, Engine.DeltaTime * 8f)) != 1.0)
                yield return (object) null;
            }
            else
            {
              while ((double) (this.questionEase = Calc.Approach(this.questionEase, 1f, Engine.DeltaTime * 8f)) != 1.0)
                yield return (object) null;
            }
          }
          if (this.pageIndex >= this.pages.Count)
          {
            if (!Input.MenuConfirm.Pressed)
            {
              if (Input.MenuUp.Pressed && this.questionIndex > 0)
              {
                Audio.Play("event:/ui/main/rollover_up");
                --this.questionIndex;
                this.wiggler.Start();
              }
              else if (Input.MenuDown.Pressed && this.questionIndex < 1)
              {
                Audio.Play("event:/ui/main/rollover_down");
                ++this.questionIndex;
                this.wiggler.Start();
              }
            }
            else
              goto label_22;
          }
          yield return (object) null;
        }
        else
          break;
      }
      this.Focused = false;
      this.Overworld.Goto<OuiFileSelect>();
      Audio.Play("event:/ui/main/button_back");
      Audio.SetParameter(this.mainSfx, "assist_progress", 6f);
      yield break;
label_22:
      this.FileSlot.AssistModeEnabled = this.questionIndex == 0;
      if (this.FileSlot.AssistModeEnabled)
        this.FileSlot.VariantModeEnabled = false;
      this.FileSlot.CreateButtons();
      this.Focused = false;
      this.Overworld.Goto<OuiFileSelect>();
      Audio.Play(this.questionIndex == 0 ? "event:/ui/main/assist_button_yes" : "event:/ui/main/assist_button_no");
      Audio.SetParameter(this.mainSfx, "assist_progress", this.questionIndex == 0 ? 4f : 5f);
    }

    public override void Update()
    {
      this.dot = Calc.Approach(this.dot, (float) this.pageIndex, Engine.DeltaTime * 8f);
      this.leftArrowEase = Calc.Approach(this.leftArrowEase, this.pageIndex > 0 ? 1f : 0.0f, Engine.DeltaTime * 4f);
      this.rightArrowEase = Calc.Approach(this.rightArrowEase, this.pageIndex < this.pages.Count ? 1f : 0.0f, Engine.DeltaTime * 4f);
      base.Update();
    }

    public override void Render()
    {
      if (!this.Visible)
        return;
      Draw.Rect(-10f, -10f, 1940f, 1100f, Color.Black * this.fade * 0.9f);
      for (int index = 0; index < this.pages.Count; ++index)
      {
        OuiAssistMode.Page page = this.pages[index];
        float num = Ease.CubeOut(page.Ease);
        if ((double) num > 0.0)
        {
          Vector2 position = new Vector2(960f, 620f);
          position.X += (float) ((double) page.Direction * (1.0 - (double) num) * 256.0);
          page.Text.DrawJustifyPerLine(position, new Vector2(0.5f, 0.0f), Vector2.One * 0.8f, num * this.fade, 0, int.MaxValue);
        }
      }
      if ((double) this.questionEase > 0.0)
      {
        float num1 = Ease.CubeOut(this.questionEase);
        float num2 = this.wiggler.Value * 8f;
        Vector2 position = new Vector2((float) (960.0 + (1.0 - (double) num1) * 256.0), 620f);
        float lineHeight = ActiveFont.LineHeight;
        this.questionText.DrawJustifyPerLine(position, new Vector2(0.5f, 0.0f), Vector2.One, num1 * this.fade, 0, int.MaxValue);
        ActiveFont.DrawOutline(Dialog.Clean("ASSIST_YES", (Language) null), position + new Vector2((float) ((this.questionIndex == 0 ? (double) num2 : 0.0) * 1.20000004768372) * num1, (float) ((double) lineHeight * 1.39999997615814 + 10.0)), new Vector2(0.5f, 0.0f), Vector2.One * 0.8f, this.SelectionColor(this.questionIndex == 0), 2f, Color.Black * num1 * this.fade);
        ActiveFont.DrawOutline(Dialog.Clean("ASSIST_NO", (Language) null), position + new Vector2((float) ((this.questionIndex == 1 ? (double) num2 : 0.0) * 1.20000004768372) * num1, (float) ((double) lineHeight * 2.20000004768372 + 20.0)), new Vector2(0.5f, 0.0f), Vector2.One * 0.8f, this.SelectionColor(this.questionIndex == 1), 2f, Color.Black * num1 * this.fade);
      }
      if (this.pages.Count > 0)
      {
        int num1 = this.pages.Count + 1;
        MTexture mtexture = GFX.Gui["dot"];
        int num2 = mtexture.Width * num1;
        Vector2 vector2 = new Vector2(960f, (float) (960.0 - 40.0 * (double) Ease.CubeOut(this.fade)));
        for (int index = 0; index < num1; ++index)
          mtexture.DrawCentered(vector2 + new Vector2((float) (-num2 / 2) + (float) mtexture.Width * ((float) index + 0.5f), 0.0f), Color.White * 0.25f);
        float x = (float) (1.0 + (double) Calc.YoYo(this.dot % 1f) * 4.0);
        mtexture.DrawCentered(vector2 + new Vector2((float) (-num2 / 2) + (float) mtexture.Width * (this.dot + 0.5f), 0.0f), this.iconColor, new Vector2(x, 1f));
        GFX.Gui["dotarrow"].DrawCentered(vector2 + new Vector2((float) (-num2 / 2 - 50), (float) (32.0 * (1.0 - (double) Ease.CubeOut(this.leftArrowEase)))), this.iconColor * this.leftArrowEase, new Vector2(-1f, 1f));
        GFX.Gui["dotarrow"].DrawCentered(vector2 + new Vector2((float) (num2 / 2 + 50), (float) (32.0 * (1.0 - (double) Ease.CubeOut(this.rightArrowEase)))), this.iconColor * this.rightArrowEase);
      }
      GFX.Gui["assistmode"].DrawJustified(new Vector2(960f, (float) (540.0 + 64.0 * (double) Ease.CubeOut(this.fade))), new Vector2(0.5f, 1f), this.iconColor * this.fade);
    }

    private Color SelectionColor(bool selected)
    {
      if (selected)
        return (Settings.Instance.DisableFlashes || this.Scene.BetweenInterval(0.1f) ? TextMenu.HighlightColorA : TextMenu.HighlightColorB) * this.fade;
      return Color.White * this.fade;
    }

    private class Page
    {
      public float Ease = 0.0f;
      public FancyText.Text Text;
      public float Direction;
    }
  }
}

