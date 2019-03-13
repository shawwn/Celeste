// Decompiled with JetBrains decompiler
// Type: Celeste.OuiChapterSelect
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Celeste
{
  public class OuiChapterSelect : Oui
  {
    private List<OuiChapterSelectIcon> icons = new List<OuiChapterSelectIcon>();
    private int indexToSnap = -1;
    private MTexture scarf = GFX.Gui["areas/hover"];
    private float inputDelay = 0.0f;
    private const int scarfSegmentSize = 2;
    private MTexture[] scarfSegments;
    private float ease;
    private float journalEase;
    private bool journalEnabled;
    private bool disableInput;
    private bool display;

    private int area
    {
      get
      {
        return SaveData.Instance.LastArea.ID;
      }
      set
      {
        SaveData.Instance.LastArea.ID = value;
      }
    }

    public override bool IsStart(Overworld overworld, Overworld.StartMode start)
    {
      if (start == Overworld.StartMode.AreaComplete || start == Overworld.StartMode.AreaQuit)
        this.indexToSnap = this.area;
      return false;
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      int count = AreaData.Areas.Count;
      for (int area = 0; area < count; ++area)
      {
        MTexture front = GFX.Gui[AreaData.Areas[area].Icon];
        MTexture back = GFX.Gui.Has(AreaData.Areas[area].Icon + "_back") ? GFX.Gui[AreaData.Areas[area].Icon + "_back"] : front;
        this.icons.Add(new OuiChapterSelectIcon(area, front, back));
        this.Scene.Add((Entity) this.icons[area]);
      }
      this.scarfSegments = new MTexture[this.scarf.Height / 2];
      for (int index = 0; index < this.scarfSegments.Length; ++index)
        this.scarfSegments[index] = this.scarf.GetSubtexture(0, index * 2, this.scarf.Width, 2, (MTexture) null);
      if (this.indexToSnap >= 0)
      {
        this.area = this.indexToSnap;
        this.icons[this.indexToSnap].SnapToSelected();
      }
      this.Depth = -20;
    }

    public override IEnumerator Enter(Oui from)
    {
      this.Visible = true;
      this.EaseCamera();
      this.display = true;
      this.journalEnabled = Celeste.PlayMode == Celeste.PlayModes.Debug || SaveData.Instance.CheatMode;
      for (int i = 0; i <= SaveData.Instance.UnlockedAreas && !this.journalEnabled; ++i)
      {
        if (SaveData.Instance.Areas[i].Modes[0].TimePlayed > 0L && !AreaData.Get(i).Interlude)
          this.journalEnabled = true;
      }
      OuiChapterSelectIcon unselected = (OuiChapterSelectIcon) null;
      if (from is OuiChapterPanel)
        (unselected = this.icons[this.area]).Unselect();
      foreach (OuiChapterSelectIcon icon1 in this.icons)
      {
        OuiChapterSelectIcon icon = icon1;
        if (icon.Area <= SaveData.Instance.UnlockedAreas && icon != unselected)
        {
          icon.Position = icon.HiddenPosition;
          icon.Show();
          icon.AssistModeUnlockable = false;
        }
        else if (SaveData.Instance.AssistMode && icon.Area == SaveData.Instance.UnlockedAreas + 1)
        {
          icon.Position = icon.HiddenPosition;
          icon.Show();
          icon.AssistModeUnlockable = true;
        }
        yield return (object) 0.01f;
        icon = (OuiChapterSelectIcon) null;
      }
      if (from is OuiChapterPanel)
        yield return (object) 0.25f;
    }

    public override IEnumerator Leave(Oui next)
    {
      this.display = false;
      if (next is OuiMainMenu)
      {
        UserIO.SaveHandler(true, false);
        yield return (object) this.EaseOut(next);
        while (UserIO.Saving)
          yield return (object) null;
      }
      else
        yield return (object) this.EaseOut(next);
    }

    private IEnumerator EaseOut(Oui next)
    {
      OuiChapterSelectIcon selected = (OuiChapterSelectIcon) null;
      if (next is OuiChapterPanel)
        (selected = this.icons[this.area]).Select();
      foreach (OuiChapterSelectIcon icon1 in this.icons)
      {
        OuiChapterSelectIcon icon = icon1;
        if (selected != icon)
          icon.Hide();
        yield return (object) 0.01f;
        icon = (OuiChapterSelectIcon) null;
      }
      this.Visible = false;
    }

    public void AdvanceToNext()
    {
      this.Overworld.ShowInputUI = false;
      this.Focused = false;
      this.disableInput = true;
      this.Add((Component) new Coroutine(this.AutoAdvanceRoutine(), true));
    }

    private IEnumerator AutoAdvanceRoutine()
    {
      if (this.area < SaveData.Instance.MaxArea)
      {
        int nextArea = this.area + 1;
        if (nextArea == 9)
          this.icons[nextArea].HideIcon = true;
        while (!this.Selected)
          yield return (object) null;
        yield return (object) 1f;
        Audio.Play("event:/ui/postgame/unlock_newchapter");
        Audio.Play("event:/ui/world_map/icon/roll_right");
        this.area = nextArea;
        this.EaseCamera();
        this.Overworld.Maddy.Hide(true);
        if (this.area == 9)
        {
          bool ready = false;
          this.icons[this.area].CoreUnlock((Action) (() => ready = true));
          while (!ready)
            yield return (object) null;
        }
        yield return (object) 0.25f;
      }
      this.disableInput = false;
      this.Focused = true;
      this.Overworld.ShowInputUI = true;
    }

    public override void Update()
    {
      if (this.Focused && !this.disableInput)
      {
        this.inputDelay -= Engine.DeltaTime;
        if (this.area >= 0 && this.area < AreaData.Areas.Count)
          Input.SetLightbarColor(AreaData.Get(this.area).TitleBaseColor);
        if (Input.MenuCancel.Pressed)
        {
          Audio.Play("event:/ui/main/button_back");
          this.Overworld.Goto<OuiMainMenu>();
          this.Overworld.Maddy.Hide(true);
        }
        else if (Input.MenuJournal.Pressed && this.journalEnabled)
        {
          Audio.Play("event:/ui/world_map/journal/select");
          this.Overworld.Goto<OuiJournal>();
        }
        else if ((double) this.inputDelay <= 0.0)
        {
          if (this.area > 0 && Input.MenuLeft.Pressed)
          {
            Audio.Play("event:/ui/world_map/icon/roll_left");
            this.inputDelay = 0.2f;
            --this.area;
            this.icons[this.area].Hovered(-1);
            this.EaseCamera();
            this.Overworld.Maddy.Hide(true);
          }
          else if (Input.MenuRight.Pressed)
          {
            bool flag = SaveData.Instance.AssistMode && this.area == SaveData.Instance.UnlockedAreas && this.area < SaveData.Instance.MaxArea;
            if (this.area < SaveData.Instance.UnlockedAreas | flag)
            {
              Audio.Play("event:/ui/world_map/icon/roll_right");
              this.inputDelay = 0.2f;
              ++this.area;
              this.icons[this.area].Hovered(1);
              this.EaseCamera();
              this.Overworld.Maddy.Hide(true);
            }
          }
          else if (Input.MenuConfirm.Pressed)
          {
            if (this.icons[this.area].AssistModeUnlockable)
            {
              Audio.Play("event:/ui/world_map/icon/assist_skip");
              this.Focused = false;
              this.Overworld.ShowInputUI = false;
              this.icons[this.area].AssistModeUnlock((Action) (() =>
              {
                this.Focused = true;
                this.Overworld.ShowInputUI = true;
                if (this.area >= SaveData.Instance.MaxArea)
                  return;
                OuiChapterSelectIcon icon = this.icons[this.area + 1];
                icon.AssistModeUnlockable = true;
                icon.Position = icon.HiddenPosition;
                icon.Show();
              }));
            }
            else
            {
              Audio.Play("event:/ui/world_map/icon/select");
              SaveData.Instance.LastArea.Mode = AreaMode.Normal;
              this.Overworld.Goto<OuiChapterPanel>();
            }
          }
        }
      }
      this.ease = Calc.Approach(this.ease, this.display ? 1f : 0.0f, Engine.DeltaTime * 3f);
      this.journalEase = Calc.Approach(this.journalEase, !this.display || this.disableInput || (!this.Focused || !this.journalEnabled) ? 0.0f : 1f, Engine.DeltaTime * 4f);
      base.Update();
    }

    public override void Render()
    {
      Vector2 vector2 = new Vector2(960f, (float) -this.scarf.Height * Ease.CubeInOut(1f - this.ease));
      for (int index = 0; index < this.scarfSegments.Length; ++index)
      {
        float num = Ease.CubeIn((float) index / (float) this.scarfSegments.Length);
        float x = (float) ((double) num * Math.Sin((double) this.Scene.RawTimeActive * 4.0 + (double) index * 0.0500000007450581) * 4.0 - (double) num * 16.0);
        this.scarfSegments[index].DrawJustified(vector2 + new Vector2(x, (float) (index * 2)), new Vector2(0.5f, 0.0f));
      }
      if ((double) this.journalEase <= 0.0)
        return;
      Vector2 position = new Vector2(128f * Ease.CubeOut(this.journalEase), 952f);
      GFX.Gui["menu/journal"].DrawCentered(position, Color.White * Ease.CubeOut(this.journalEase));
      Input.GuiButton(Input.MenuJournal, "controls/keyboard/oemquestion").Draw(position, Vector2.Zero, Color.White * Ease.CubeOut(this.journalEase));
    }

    private void EaseCamera()
    {
      AreaData area = AreaData.Areas[this.area];
      double num = (double) this.Overworld.Mountain.EaseCamera(this.area, area.MountainIdle, new float?(), true, this.area == 10);
      this.Overworld.Mountain.Model.EaseState(area.MountainState);
    }
  }
}

