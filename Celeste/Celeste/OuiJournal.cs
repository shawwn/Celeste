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
    private Color backColor = Color.Lerp(Color.get_White(), Color.get_Black(), 0.2f);
    private MTexture arrow = GFX.Journal["pageArrow"];
    private const float onScreenX = 0.0f;
    private const float offScreenX = -1920f;
    public bool PageTurningLocked;
    public int PageIndex;
    public VirtualRenderTarget CurrentPageBuffer;
    public VirtualRenderTarget NextPageBuffer;
    private bool turningPage;
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
      OuiJournal journal = this;
      Stats.MakeRequest();
      journal.Overworld.ShowConfirmUI = false;
      journal.fromAreaInspect = from is OuiChapterPanel;
      journal.PageIndex = 0;
      journal.Visible = true;
      journal.X = -1920f;
      journal.turningPage = false;
      journal.turningScale = 1f;
      journal.rotation = 0.0f;
      journal.dot = 0.0f;
      journal.dotTarget = 0.0f;
      journal.dotEase = 0.0f;
      journal.leftArrowEase = 0.0f;
      journal.rightArrowEase = 0.0f;
      journal.NextPageBuffer = VirtualContent.CreateRenderTarget("journal-a", 1610, 1000, false, true, 0);
      journal.CurrentPageBuffer = VirtualContent.CreateRenderTarget("journal-b", 1610, 1000, false, true, 0);
      journal.Pages.Add((OuiJournalPage) new OuiJournalCover(journal));
      journal.Pages.Add((OuiJournalPage) new OuiJournalProgress(journal));
      journal.Pages.Add((OuiJournalPage) new OuiJournalSpeedrun(journal));
      journal.Pages.Add((OuiJournalPage) new OuiJournalDeaths(journal));
      journal.Pages.Add((OuiJournalPage) new OuiJournalPoem(journal));
      if (Stats.Has())
        journal.Pages.Add((OuiJournalPage) new OuiJournalGlobal(journal));
      int num1 = 0;
      foreach (OuiJournalPage page in journal.Pages)
        page.PageIndex = num1++;
      journal.Pages[0].Redraw(journal.CurrentPageBuffer);
      journal.cameraStart = journal.Overworld.Mountain.UntiltedCamera;
      journal.cameraEnd = journal.cameraStart;
      ref Vector3 local = ref journal.cameraEnd.Position;
      local = Vector3.op_Addition(local, Vector3.op_Multiply(Vector3.op_UnaryNegation(journal.cameraStart.Rotation.Forward()), 1f));
      double num2 = (double) journal.Overworld.Mountain.EaseCamera(journal.Overworld.Mountain.Area, journal.cameraEnd, new float?(2f), true, false);
      journal.Overworld.Mountain.AllowUserRotation = false;
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime / 0.4f)
      {
        journal.rotation = -0.025f * Ease.BackOut(p);
        journal.X = (float) (1920.0 * (double) Ease.CubeInOut(p) - 1920.0);
        journal.dotEase = p;
        yield return (object) null;
      }
      journal.dotEase = 1f;
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
      OuiJournal ouiJournal = this;
      Audio.Play("event:/ui/world_map/journal/back");
      double num = (double) ouiJournal.Overworld.Mountain.EaseCamera(ouiJournal.Overworld.Mountain.Area, ouiJournal.cameraStart, new float?(0.4f), true, false);
      UserIO.SaveHandler(false, true);
      yield return (object) ouiJournal.EaseOut(0.4f);
      while (UserIO.Saving)
        yield return (object) null;
      ouiJournal.CurrentPageBuffer.Dispose();
      ouiJournal.NextPageBuffer.Dispose();
      ouiJournal.Overworld.ShowConfirmUI = true;
      ouiJournal.Pages.Clear();
      ouiJournal.Visible = false;
      ouiJournal.Overworld.Mountain.AllowUserRotation = true;
    }

    private IEnumerator EaseOut(float duration)
    {
      OuiJournal ouiJournal = this;
      float rotFrom = ouiJournal.rotation;
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime / duration)
      {
        ouiJournal.rotation = rotFrom * (1f - Ease.BackOut(p));
        ouiJournal.X = (float) (0.0 + -1920.0 * (double) Ease.CubeInOut(p));
        ouiJournal.dotEase = 1f - p;
        yield return (object) null;
      }
      ouiJournal.dotEase = 0.0f;
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
      if (this.PageTurningLocked || !Input.MenuJournal.Pressed && !Input.MenuCancel.Pressed)
        return;
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
      Vector2 position = Vector2.op_Addition(this.Position, new Vector2(128f, 120f));
      float num1 = Ease.CubeInOut(Math.Max(0.0f, this.turningScale));
      float num2 = Ease.CubeInOut(Math.Abs(Math.Min(0.0f, this.turningScale)));
      MTexture mtexture1 = GFX.Journal["edge"];
      mtexture1.Draw(Vector2.op_Addition(position, new Vector2((float) -mtexture1.Width, 0.0f)), Vector2.get_Zero(), Color.get_White(), 1f, this.rotation);
      if (this.PageIndex > 0)
        GFX.Journal[this.PrevPage.PageTexture].Draw(position, Vector2.get_Zero(), this.backColor, new Vector2(-1f, 1f), this.rotation);
      if (this.turningPage)
      {
        GFX.Journal[this.NextPage.PageTexture].Draw(position, Vector2.get_Zero(), Color.get_White(), 1f, this.rotation);
        Draw.SpriteBatch.Draw((Texture2D) (RenderTarget2D) this.NextPageBuffer, position, new Rectangle?(this.NextPageBuffer.Bounds), Color.get_White(), this.rotation, Vector2.get_Zero(), Vector2.get_One(), (SpriteEffects) 0, 0.0f);
      }
      if (this.turningPage && (double) num2 > 0.0)
        GFX.Journal[this.Page.PageTexture].Draw(position, Vector2.get_Zero(), this.backColor, new Vector2(-1f * num2, 1f), this.rotation);
      if ((double) num1 > 0.0)
      {
        GFX.Journal[this.Page.PageTexture].Draw(position, Vector2.get_Zero(), Color.get_White(), new Vector2(num1, 1f), this.rotation);
        Draw.SpriteBatch.Draw((Texture2D) (RenderTarget2D) this.CurrentPageBuffer, position, new Rectangle?(this.CurrentPageBuffer.Bounds), Color.get_White(), this.rotation, Vector2.get_Zero(), new Vector2(num1, 1f), (SpriteEffects) 0, 0.0f);
      }
      if (this.Pages.Count <= 0)
        return;
      int count = this.Pages.Count;
      MTexture mtexture2 = GFX.Gui["dot_outline"];
      int num3 = mtexture2.Width * count;
      Vector2 vector2;
      ((Vector2) ref vector2).\u002Ector(960f, (float) (1040.0 - 40.0 * (double) Ease.CubeOut(this.dotEase)));
      for (int index = 0; index < count; ++index)
        mtexture2.DrawCentered(Vector2.op_Addition(vector2, new Vector2((float) (-num3 / 2) + (float) mtexture2.Width * ((float) index + 0.5f), 0.0f)), Color.op_Multiply(Color.get_White(), 0.25f));
      float num4 = (float) (1.0 + (double) Calc.YoYo(this.dot % 1f) * 4.0);
      mtexture2.DrawCentered(Vector2.op_Addition(vector2, new Vector2((float) (-num3 / 2) + (float) mtexture2.Width * (this.dot + 0.5f), 0.0f)), Color.get_White(), new Vector2(num4, 1f));
      GFX.Gui["dotarrow_outline"].DrawCentered(Vector2.op_Addition(vector2, new Vector2((float) (-num3 / 2 - 50), (float) (32.0 * (1.0 - (double) Ease.CubeOut(this.leftArrowEase))))), Color.op_Multiply(Color.get_White(), this.leftArrowEase), new Vector2(-1f, 1f));
      GFX.Gui["dotarrow_outline"].DrawCentered(Vector2.op_Addition(vector2, new Vector2((float) (num3 / 2 + 50), (float) (32.0 * (1.0 - (double) Ease.CubeOut(this.rightArrowEase))))), Color.op_Multiply(Color.get_White(), this.rightArrowEase));
    }
  }
}
