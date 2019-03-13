// Decompiled with JetBrains decompiler
// Type: Celeste.OuiJournal
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Celeste
{
  public class OuiJournal : Oui
  {
    public List<OuiJournalPage> Pages = new List<OuiJournalPage>();
    public int PageIndex = 0;
    private bool turningPage = false;
    private Color backColor = Color.Lerp(Color.White, Color.Black, 0.2f);
    private MTexture arrow = GFX.Journal["pageArrow"];
    private const float onScreenX = 0.0f;
    private const float offScreenX = -1920f;
    public bool PageTurningLocked;
    public VirtualRenderTarget CurrentPageBuffer;
    public VirtualRenderTarget NextPageBuffer;
    private float turningScale;
    private bool fromAreaInspect;
    private float rotation;
    private MountainCamera cameraStart;
    private MountainCamera cameraEnd;
    private float dot;
    private float dotTarget;
    private float dotEase;
    private float leftArrowEase;
    private float rightArrowEase;

    public OuiJournalPage Page
    {
      get
      {
        return this.Pages[this.PageIndex];
      }
    }

    public OuiJournalPage NextPage
    {
      get
      {
        return this.Pages[this.PageIndex + 1];
      }
    }

    public OuiJournalPage PrevPage
    {
      get
      {
        return this.Pages[this.PageIndex - 1];
      }
    }

    public override IEnumerator Enter(Oui from)
    {
      Stats.MakeRequest();
      this.Overworld.ShowConfirmUI = false;
      this.fromAreaInspect = from is OuiChapterPanel;
      this.PageIndex = 0;
      this.Visible = true;
      this.X = -1920f;
      this.turningPage = false;
      this.turningScale = 1f;
      this.rotation = 0.0f;
      this.dot = 0.0f;
      this.dotTarget = 0.0f;
      this.dotEase = 0.0f;
      this.leftArrowEase = 0.0f;
      this.rightArrowEase = 0.0f;
      this.NextPageBuffer = VirtualContent.CreateRenderTarget("journal-a", 1610, 1000, false, true, 0);
      this.CurrentPageBuffer = VirtualContent.CreateRenderTarget("journal-b", 1610, 1000, false, true, 0);
      this.Pages.Add((OuiJournalPage) new OuiJournalCover(this));
      this.Pages.Add((OuiJournalPage) new OuiJournalProgress(this));
      this.Pages.Add((OuiJournalPage) new OuiJournalSpeedrun(this));
      this.Pages.Add((OuiJournalPage) new OuiJournalDeaths(this));
      this.Pages.Add((OuiJournalPage) new OuiJournalPoem(this));
      if (Stats.Has())
        this.Pages.Add((OuiJournalPage) new OuiJournalGlobal(this));
      int i = 0;
      foreach (OuiJournalPage page1 in this.Pages)
      {
        OuiJournalPage page = page1;
        page.PageIndex = i++;
        page = (OuiJournalPage) null;
      }
      this.Pages[0].Redraw(this.CurrentPageBuffer);
      this.cameraStart = this.Overworld.Mountain.UntiltedCamera;
      this.cameraEnd = this.cameraStart;
      this.cameraEnd.Position += -this.cameraStart.Rotation.Forward() * 1f;
      double num = (double) this.Overworld.Mountain.EaseCamera(this.Overworld.Mountain.Area, this.cameraEnd, new float?(2f), true, false);
      this.Overworld.Mountain.AllowUserRotation = false;
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime / 0.4f)
      {
        this.rotation = -0.025f * Ease.BackOut(p);
        this.X = (float) (1920.0 * (double) Ease.CubeInOut(p) - 1920.0);
        this.dotEase = p;
        yield return (object) null;
      }
      this.dotEase = 1f;
    }

    public override void HandleGraphicsReset()
    {
      base.HandleGraphicsReset();
      if (this.Pages.Count <= 0)
        return;
      this.Page.Redraw(this.CurrentPageBuffer);
    }

    public IEnumerator TurnPage(int direction)
    {
      this.turningPage = true;
      if (direction < 0)
      {
        --this.PageIndex;
        this.turningScale = -1f;
        --this.dotTarget;
        this.Page.Redraw(this.CurrentPageBuffer);
        this.NextPage.Redraw(this.NextPageBuffer);
        while ((double) (this.turningScale = Calc.Approach(this.turningScale, 1f, Engine.DeltaTime * 8f)) < 1.0)
          yield return (object) null;
      }
      else
      {
        this.NextPage.Redraw(this.NextPageBuffer);
        this.turningScale = 1f;
        ++this.dotTarget;
        while ((double) (this.turningScale = Calc.Approach(this.turningScale, -1f, Engine.DeltaTime * 8f)) > -1.0)
          yield return (object) null;
        ++this.PageIndex;
        this.Page.Redraw(this.CurrentPageBuffer);
      }
      this.turningScale = 1f;
      this.turningPage = false;
    }

    public override IEnumerator Leave(Oui next)
    {
      Audio.Play("event:/ui/world_map/journal/back");
      double num = (double) this.Overworld.Mountain.EaseCamera(this.Overworld.Mountain.Area, this.cameraStart, new float?(0.4f), true, false);
      UserIO.SaveHandler(false, true);
      yield return (object) this.EaseOut(0.4f);
      while (UserIO.Saving)
        yield return (object) null;
      this.CurrentPageBuffer.Dispose();
      this.NextPageBuffer.Dispose();
      this.Overworld.ShowConfirmUI = true;
      this.Pages.Clear();
      this.Visible = false;
      this.Overworld.Mountain.AllowUserRotation = true;
    }

    private IEnumerator EaseOut(float duration)
    {
      float rotFrom = this.rotation;
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime / duration)
      {
        this.rotation = rotFrom * (1f - Ease.BackOut(p));
        this.X = (float) (0.0 + -1920.0 * (double) Ease.CubeInOut(p));
        this.dotEase = 1f - p;
        yield return (object) null;
      }
      this.dotEase = 0.0f;
    }

    public override void Update()
    {
      base.Update();
      this.dot = Calc.Approach(this.dot, this.dotTarget, Engine.DeltaTime * 8f);
      this.leftArrowEase = Calc.Approach(this.leftArrowEase, (double) this.dotTarget > 0.0 ? 1f : 0.0f, Engine.DeltaTime * 5f) * this.dotEase;
      this.rightArrowEase = Calc.Approach(this.rightArrowEase, (double) this.dotTarget < (double) (this.Pages.Count - 1) ? 1f : 0.0f, Engine.DeltaTime * 5f) * this.dotEase;
      if (!this.Focused || this.turningPage)
        return;
      this.Page.Update();
      if (!this.PageTurningLocked)
      {
        if (Input.MenuLeft.Pressed && this.PageIndex > 0)
        {
          if (this.PageIndex == 1)
            Audio.Play("event:/ui/world_map/journal/page_cover_back");
          else
            Audio.Play("event:/ui/world_map/journal/page_main_back");
          this.Add((Component) new Coroutine(this.TurnPage(-1), true));
        }
        else if (Input.MenuRight.Pressed && this.PageIndex < this.Pages.Count - 1)
        {
          if (this.PageIndex == 0)
            Audio.Play("event:/ui/world_map/journal/page_cover_forward");
          else
            Audio.Play("event:/ui/world_map/journal/page_main_forward");
          this.Add((Component) new Coroutine(this.TurnPage(1), true));
        }
      }
      if (!this.PageTurningLocked && (Input.MenuJournal.Pressed || Input.MenuCancel.Pressed))
        this.Close();
    }

    private void Close()
    {
      if (this.fromAreaInspect)
        this.Overworld.Goto<OuiChapterPanel>();
      else
        this.Overworld.Goto<OuiChapterSelect>();
    }

    public override void Render()
    {
      Vector2 position = this.Position + new Vector2(128f, 120f);
      float x1 = Ease.CubeInOut(Math.Max(0.0f, this.turningScale));
      float num1 = Ease.CubeInOut(Math.Abs(Math.Min(0.0f, this.turningScale)));
      MTexture mtexture1 = GFX.Journal["edge"];
      mtexture1.Draw(position + new Vector2((float) -mtexture1.Width, 0.0f), Vector2.Zero, Color.White, 1f, this.rotation);
      if (this.PageIndex > 0)
        GFX.Journal[this.PrevPage.PageTexture].Draw(position, Vector2.Zero, this.backColor, new Vector2(-1f, 1f), this.rotation);
      if (this.turningPage)
      {
        GFX.Journal[this.NextPage.PageTexture].Draw(position, Vector2.Zero, Color.White, 1f, this.rotation);
        Draw.SpriteBatch.Draw((Texture2D) (RenderTarget2D) this.NextPageBuffer, position, new Rectangle?(this.NextPageBuffer.Bounds), Color.White, this.rotation, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.0f);
      }
      if (this.turningPage && (double) num1 > 0.0)
        GFX.Journal[this.Page.PageTexture].Draw(position, Vector2.Zero, this.backColor, new Vector2(-1f * num1, 1f), this.rotation);
      if ((double) x1 > 0.0)
      {
        GFX.Journal[this.Page.PageTexture].Draw(position, Vector2.Zero, Color.White, new Vector2(x1, 1f), this.rotation);
        Draw.SpriteBatch.Draw((Texture2D) (RenderTarget2D) this.CurrentPageBuffer, position, new Rectangle?(this.CurrentPageBuffer.Bounds), Color.White, this.rotation, Vector2.Zero, new Vector2(x1, 1f), SpriteEffects.None, 0.0f);
      }
      if (this.Pages.Count <= 0)
        return;
      int count = this.Pages.Count;
      MTexture mtexture2 = GFX.Gui["dot_outline"];
      int num2 = mtexture2.Width * count;
      Vector2 vector2 = new Vector2(960f, (float) (1040.0 - 40.0 * (double) Ease.CubeOut(this.dotEase)));
      for (int index = 0; index < count; ++index)
        mtexture2.DrawCentered(vector2 + new Vector2((float) (-num2 / 2) + (float) mtexture2.Width * ((float) index + 0.5f), 0.0f), Color.White * 0.25f);
      float x2 = (float) (1.0 + (double) Calc.YoYo(this.dot % 1f) * 4.0);
      mtexture2.DrawCentered(vector2 + new Vector2((float) (-num2 / 2) + (float) mtexture2.Width * (this.dot + 0.5f), 0.0f), Color.White, new Vector2(x2, 1f));
      GFX.Gui["dotarrow_outline"].DrawCentered(vector2 + new Vector2((float) (-num2 / 2 - 50), (float) (32.0 * (1.0 - (double) Ease.CubeOut(this.leftArrowEase)))), Color.White * this.leftArrowEase, new Vector2(-1f, 1f));
      GFX.Gui["dotarrow_outline"].DrawCentered(vector2 + new Vector2((float) (num2 / 2 + 50), (float) (32.0 * (1.0 - (double) Ease.CubeOut(this.rightArrowEase)))), Color.White * this.rightArrowEase);
    }
  }
}

