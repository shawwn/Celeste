// Decompiled with JetBrains decompiler
// Type: Celeste.OuiFileSelect
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class OuiFileSelect : Oui
  {
    public OuiFileSelectSlot[] Slots = new OuiFileSelectSlot[3];
    public int SlotIndex;
    public bool SlotSelected;
    public static bool Loaded;
    private bool loadedSuccess;
    public bool HasSlots;

    public OuiFileSelect()
    {
      OuiFileSelect.Loaded = false;
    }

    public override IEnumerator Enter(Oui from)
    {
      OuiFileSelect ouiFileSelect = this;
      ouiFileSelect.SlotSelected = false;
      if (!OuiFileSelect.Loaded)
      {
        RunThread.Start(new Action(ouiFileSelect.LoadThread), "FILE_LOADING", false);
        float elapsed = 0.0f;
        while (!OuiFileSelect.Loaded || (double) elapsed < 0.5)
        {
          elapsed += Engine.DeltaTime;
          yield return (object) null;
        }
        if (!ouiFileSelect.loadedSuccess)
        {
          FileErrorOverlay error = new FileErrorOverlay(FileErrorOverlay.Error.Load);
          while (error.Open)
            yield return (object) null;
          if (!error.Ignore)
          {
            ouiFileSelect.Overworld.Goto<OuiMainMenu>();
            yield break;
          }
          else
            error = (FileErrorOverlay) null;
        }
      }
      else if (!(from is OuiFileNaming) && !(from is OuiAssistMode))
        yield return (object) 0.2f;
      ouiFileSelect.HasSlots = false;
      for (int index = 0; index < ouiFileSelect.Slots.Length; ++index)
      {
        if (ouiFileSelect.Slots[index].Exists)
          ouiFileSelect.HasSlots = true;
      }
      Audio.Play("event:/ui/main/whoosh_savefile_in");
      if (from is OuiFileNaming || from is OuiAssistMode)
      {
        if (!ouiFileSelect.SlotSelected)
          ouiFileSelect.SelectSlot(false);
      }
      else if (!ouiFileSelect.HasSlots)
      {
        ouiFileSelect.SlotIndex = 0;
        ouiFileSelect.Slots[ouiFileSelect.SlotIndex].Position = new Vector2((float) ouiFileSelect.Slots[ouiFileSelect.SlotIndex].HiddenPosition(1, 0).X, (float) ouiFileSelect.Slots[ouiFileSelect.SlotIndex].SelectedPosition.Y);
        ouiFileSelect.SelectSlot(true);
      }
      else if (!ouiFileSelect.SlotSelected)
      {
        Alarm.Set((Entity) ouiFileSelect, 0.4f, (Action) (() => Audio.Play("event:/ui/main/savefile_rollover_first")), Alarm.AlarmMode.Oneshot);
        for (int i = 0; i < ouiFileSelect.Slots.Length; ++i)
        {
          ouiFileSelect.Slots[i].Position = new Vector2((float) ouiFileSelect.Slots[i].HiddenPosition(1, 0).X, (float) ouiFileSelect.Slots[i].IdlePosition.Y);
          ouiFileSelect.Slots[i].Show();
          yield return (object) 0.02f;
        }
      }
    }

    private void LoadThread()
    {
      if (UserIO.Open(UserIO.Mode.Read))
      {
        for (int index = 0; index < this.Slots.Length; ++index)
        {
          if (this.Slots[index] != null)
            this.Scene.Remove((Entity) this.Slots[index]);
          OuiFileSelectSlot ouiFileSelectSlot;
          if (!UserIO.Exists(SaveData.GetFilename(index)))
          {
            ouiFileSelectSlot = new OuiFileSelectSlot(index, this, false);
          }
          else
          {
            SaveData data = UserIO.Load<SaveData>(SaveData.GetFilename(index), false);
            if (data != null)
            {
              data.AfterInitialize();
              ouiFileSelectSlot = new OuiFileSelectSlot(index, this, data);
            }
            else
              ouiFileSelectSlot = new OuiFileSelectSlot(index, this, true);
          }
          this.Slots[index] = ouiFileSelectSlot;
          this.Scene.Add((Entity) ouiFileSelectSlot);
        }
        UserIO.Close();
        this.loadedSuccess = true;
      }
      OuiFileSelect.Loaded = true;
    }

    public override IEnumerator Leave(Oui next)
    {
      Audio.Play("event:/ui/main/whoosh_savefile_out");
      int slideTo = 1;
      if (next == null || next is OuiChapterSelect || (next is OuiFileNaming || next is OuiAssistMode))
        slideTo = -1;
      for (int i = 0; i < this.Slots.Length; ++i)
      {
        if (next is OuiFileNaming && this.SlotIndex == i)
          this.Slots[i].MoveTo((float) this.Slots[i].IdlePosition.X, (float) this.Slots[0].IdlePosition.Y);
        else if (next is OuiAssistMode && this.SlotIndex == i)
          this.Slots[i].MoveTo((float) this.Slots[i].IdlePosition.X, -400f);
        else
          this.Slots[i].Hide(slideTo, 0);
        yield return (object) 0.02f;
      }
    }

    public void UnselectHighlighted()
    {
      this.SlotSelected = false;
      this.Slots[this.SlotIndex].Unselect();
      for (int index = 0; index < this.Slots.Length; ++index)
      {
        if (this.SlotIndex != index)
          this.Slots[index].Show();
      }
    }

    public void SelectSlot(bool reset)
    {
      if (!this.Slots[this.SlotIndex].Exists & reset)
      {
        this.Slots[this.SlotIndex].Name = Dialog.Clean("FILE_DEFAULT", (Language) null);
        this.Slots[this.SlotIndex].AssistModeEnabled = false;
        this.Slots[this.SlotIndex].VariantModeEnabled = false;
      }
      this.SlotSelected = true;
      this.Slots[this.SlotIndex].Select(reset);
      for (int index = 0; index < this.Slots.Length; ++index)
      {
        if (this.SlotIndex != index)
          this.Slots[index].Hide(0, index < this.SlotIndex ? -1 : 1);
      }
    }

    public override void Update()
    {
      base.Update();
      if (!this.Focused)
        return;
      if (!this.SlotSelected)
      {
        if (Input.MenuUp.Pressed && this.SlotIndex > 0)
        {
          Audio.Play("event:/ui/main/savefile_rollover_up");
          --this.SlotIndex;
        }
        else if (Input.MenuDown.Pressed && this.SlotIndex < this.Slots.Length - 1)
        {
          Audio.Play("event:/ui/main/savefile_rollover_down");
          ++this.SlotIndex;
        }
        else if (Input.MenuConfirm.Pressed)
        {
          Audio.Play("event:/ui/main/button_select");
          Audio.Play("event:/ui/main/whoosh_savefile_out");
          this.SelectSlot(true);
        }
        else
        {
          if (!Input.MenuCancel.Pressed)
            return;
          Audio.Play("event:/ui/main/button_back");
          this.Overworld.Goto<OuiMainMenu>();
        }
      }
      else
      {
        if (!Input.MenuCancel.Pressed || this.HasSlots || this.Slots[this.SlotIndex].StartingGame)
          return;
        Audio.Play("event:/ui/main/button_back");
        this.Overworld.Goto<OuiMainMenu>();
      }
    }
  }
}
