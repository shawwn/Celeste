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
    private const int scarfSegmentSize = 2;
    private MTexture[] scarfSegments;
    private float ease;
    private float journalEase;
    private bool journalEnabled;
    private bool disableInput;
    private bool display;
    private float inputDelay;

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
      OuiChapterSelect ouiChapterSelect = this;
      ouiChapterSelect.Visible = true;
      ouiChapterSelect.EaseCamera();
      ouiChapterSelect.display = true;
      ouiChapterSelect.journalEnabled = Celeste.Celeste.PlayMode == Celeste.Celeste.PlayModes.Debug || SaveData.Instance.CheatMode;
      for (int id = 0; id <= SaveData.Instance.UnlockedAreas && !ouiChapterSelect.journalEnabled; ++id)
      {
        if (SaveData.Instance.Areas[id].Modes[0].TimePlayed > 0L && !AreaData.Get(id).Interlude)
          ouiChapterSelect.journalEnabled = true;
      }
      OuiChapterSelectIcon unselected = (OuiChapterSelectIcon) null;
      if (from is OuiChapterPanel)
        (unselected = ouiChapterSelect.icons[ouiChapterSelect.area]).Unselect();
      foreach (OuiChapterSelectIcon icon in ouiChapterSelect.icons)
      {
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
      OuiChapterSelect ouiChapterSelect = this;
      OuiChapterSelectIcon selected = (OuiChapterSelectIcon) null;
      if (next is OuiChapterPanel)
        (selected = ouiChapterSelect.icons[ouiChapterSelect.area]).Select();
      foreach (OuiChapterSelectIcon icon in ouiChapterSelect.icons)
      {
        if (selected != icon)
          icon.Hide();
        yield return (object) 0.01f;
      }
      ouiChapterSelect.Visible = false;
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
      OuiChapterSelect ouiChapterSelect = this;
      if (ouiChapterSelect.area < SaveData.Instance.MaxArea)
      {
        int nextArea = ouiChapterSelect.area + 1;
        if (nextArea == 9)
          ouiChapterSelect.icons[nextArea].HideIcon = true;
        while (!ouiChapterSelect.Selected)
          yield return (object) null;
        yield return (object) 1f;
        Audio.Play("event:/ui/postgame/unlock_newchapter");
        Audio.Play("event:/ui/world_map/icon/roll_right");
        ouiChapterSelect.area = nextArea;
        ouiChapterSelect.EaseCamera();
        ouiChapterSelect.Overworld.Maddy.Hide(true);
        if (ouiChapterSelect.area == 9)
        {
          bool ready = false;
          ouiChapterSelect.icons[ouiChapterSelect.area].CoreUnlock((Action) (() => ready = true));
          while (!ready)
            yield return (object) null;
        }
        yield return (object) 0.25f;
      }
      ouiChapterSelect.disableInput = false;
      ouiChapterSelect.Focused = true;
      ouiChapterSelect.Overworld.ShowInputUI = true;
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
      Vector2 vector2;
      ((Vector2) ref vector2).\u002Ector(960f, (float) -this.scarf.Height * Ease.CubeInOut(1f - this.ease));
      for (int index = 0; index < this.scarfSegments.Length; ++index)
      {
        float num1 = Ease.CubeIn((float) index / (float) this.scarfSegments.Length);
        float num2 = (float) ((double) num1 * Math.Sin((double) this.Scene.RawTimeActive * 4.0 + (double) index * 0.0500000007450581) * 4.0 - (double) num1 * 16.0);
        this.scarfSegments[index].DrawJustified(Vector2.op_Addition(vector2, new Vector2(num2, (float) (index * 2))), new Vector2(0.5f, 0.0f));
      }
      if ((double) this.journalEase <= 0.0)
        return;
      Vector2 position;
      ((Vector2) ref position).\u002Ector(128f * Ease.CubeOut(this.journalEase), 952f);
      GFX.Gui["menu/journal"].DrawCentered(position, Color.op_Multiply(Color.get_White(), Ease.CubeOut(this.journalEase)));
      Input.GuiButton(Input.MenuJournal, "controls/keyboard/oemquestion").Draw(position, Vector2.get_Zero(), Color.op_Multiply(Color.get_White(), Ease.CubeOut(this.journalEase)));
    }

    private void EaseCamera()
    {
      AreaData area = AreaData.Areas[this.area];
      double num = (double) this.Overworld.Mountain.EaseCamera(this.area, area.MountainIdle, new float?(), true, this.area == 10);
      this.Overworld.Mountain.Model.EaseState(area.MountainState);
    }
  }
}
