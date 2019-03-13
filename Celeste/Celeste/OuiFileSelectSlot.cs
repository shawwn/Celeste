// Decompiled with JetBrains decompiler
// Type: Celeste.OuiFileSelectSlot
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
  public class OuiFileSelectSlot : Entity
  {
    public List<bool> Cassettes = new List<bool>();
    public List<bool[]> HeartGems = new List<bool[]>();
    private float timeScale = 1f;
    private List<OuiFileSelectSlot.Button> buttons = new List<OuiFileSelectSlot.Button>();
    public SaveData SaveData;
    public int FileSlot;
    public string Name;
    public bool AssistModeEnabled;
    public bool VariantModeEnabled;
    public bool Exists;
    public bool Corrupted;
    public string Time;
    public int FurthestArea;
    public Sprite Portrait;
    public bool HasBlackgems;
    public StrawberriesCounter Strawberries;
    public DeathsCounter Deaths;
    private const int height = 300;
    private const int spacing = 10;
    private const float portraitSize = 200f;
    public bool StartingGame;
    public bool Renaming;
    public bool Assisting;
    private OuiFileSelect fileSelect;
    private bool deleting;
    private float highlightEase;
    private float highlightEaseDelay;
    private float selectedEase;
    private float deletingEase;
    private Tween tween;
    private int buttonIndex;
    private int deleteIndex;
    private Wiggler wiggler;
    private float failedToDeleteEase;
    private float failedToDeleteTimer;
    private float screenFlash;
    private float inputDelay;
    private float newgameFade;
    private OuiFileSelectSlot.Button assistButton;
    private OuiFileSelectSlot.Button variantButton;

    public Vector2 IdlePosition
    {
      get
      {
        return new Vector2(960f, (float) (540 + 310 * (this.FileSlot - 1)));
      }
    }

    public Vector2 SelectedPosition
    {
      get
      {
        return new Vector2(960f, 440f);
      }
    }

    private bool highlighted
    {
      get
      {
        return this.fileSelect.SlotIndex == this.FileSlot;
      }
    }

    private bool selected
    {
      get
      {
        if (this.fileSelect.SlotSelected)
          return this.highlighted;
        return false;
      }
    }

    private OuiFileSelectSlot(int index, OuiFileSelect fileSelect)
    {
      this.FileSlot = index;
      this.fileSelect = fileSelect;
      this.Tag |= (int) Tags.HUD | (int) Tags.PauseUpdate;
      this.Visible = false;
      this.Add((Component) (this.wiggler = Wiggler.Create(0.4f, 4f, (Action<float>) null, false, false)));
    }

    public OuiFileSelectSlot(int index, OuiFileSelect fileSelect, bool corrupted)
      : this(index, fileSelect)
    {
      this.Corrupted = corrupted;
      this.Exists = corrupted;
      this.Setup();
    }

    public OuiFileSelectSlot(int index, OuiFileSelect fileSelect, SaveData data)
      : this(index, fileSelect)
    {
      this.Exists = true;
      this.SaveData = data;
      this.Name = data.Name;
      this.AssistModeEnabled = data.AssistMode;
      this.VariantModeEnabled = data.VariantMode;
      this.Add((Component) (this.Deaths = new DeathsCounter(AreaMode.Normal, false, data.TotalDeaths, 0)));
      this.Add((Component) (this.Strawberries = new StrawberriesCounter(true, data.TotalStrawberries, 0, false)));
      this.Time = Dialog.FileTime(data.Time);
      if (TimeSpan.FromTicks(data.Time).TotalHours > 0.0)
        this.timeScale = 0.725f;
      this.FurthestArea = data.UnlockedAreas;
      foreach (AreaStats area in data.Areas)
      {
        if (area.ID <= data.UnlockedAreas)
        {
          if (!AreaData.Areas[area.ID].Interlude)
          {
            bool[] flagArray = new bool[3];
            for (int index1 = 0; index1 < flagArray.Length; ++index1)
              flagArray[index1] = area.Modes[index1].HeartGem;
            this.Cassettes.Add(area.Cassette);
            this.HeartGems.Add(flagArray);
          }
        }
        else
          break;
      }
      this.Setup();
    }

    private void Setup()
    {
      string id1 = "portrait_madeline";
      string id2 = "idle_normal";
      this.Portrait = GFX.PortraitsSpriteBank.Create(id1);
      this.Portrait.Play(id2, false, false);
      this.Portrait.Scale = Vector2.op_Multiply(Vector2.get_One(), 200f / (float) GFX.PortraitsSpriteBank.SpriteData[id1].Sources[0].XML.AttrInt("size", 160));
      this.Add((Component) this.Portrait);
    }

    public void CreateButtons()
    {
      this.buttons.Clear();
      if (this.Exists)
      {
        if (!this.Corrupted)
        {
          this.buttons.Add(new OuiFileSelectSlot.Button()
          {
            Label = Dialog.Clean("file_continue", (Language) null),
            Action = new Action(this.OnContinueSelected)
          });
          if (this.SaveData != null)
          {
            List<OuiFileSelectSlot.Button> buttons1 = this.buttons;
            OuiFileSelectSlot.Button button1 = new OuiFileSelectSlot.Button();
            button1.Label = Dialog.Clean("FILE_ASSIST_" + (this.AssistModeEnabled ? "ON" : "OFF"), (Language) null);
            button1.Action = new Action(this.OnAssistSelected);
            button1.Scale = 0.7f;
            OuiFileSelectSlot.Button button2 = button1;
            this.assistButton = button1;
            OuiFileSelectSlot.Button button3 = button2;
            buttons1.Add(button3);
            if (Settings.Instance.VariantsUnlocked || this.SaveData.CheatMode)
            {
              List<OuiFileSelectSlot.Button> buttons2 = this.buttons;
              OuiFileSelectSlot.Button button4 = new OuiFileSelectSlot.Button();
              button4.Label = "[BETA] " + Dialog.Clean("FILE_VARIANT_" + (this.VariantModeEnabled ? "ON" : "OFF"), (Language) null);
              button4.Action = new Action(this.OnVariantSelected);
              button4.Scale = 0.7f;
              OuiFileSelectSlot.Button button5 = button4;
              this.variantButton = button4;
              OuiFileSelectSlot.Button button6 = button5;
              buttons2.Add(button6);
            }
          }
        }
        this.buttons.Add(new OuiFileSelectSlot.Button()
        {
          Label = Dialog.Clean("file_delete", (Language) null),
          Action = new Action(this.OnDeleteSelected),
          Scale = 0.7f
        });
      }
      else
      {
        this.buttons.Add(new OuiFileSelectSlot.Button()
        {
          Label = Dialog.Clean("file_begin", (Language) null),
          Action = new Action(this.OnNewGameSelected)
        });
        this.buttons.Add(new OuiFileSelectSlot.Button()
        {
          Label = Dialog.Clean("file_rename", (Language) null),
          Action = new Action(this.OnRenameSelected),
          Scale = 0.7f
        });
        List<OuiFileSelectSlot.Button> buttons1 = this.buttons;
        OuiFileSelectSlot.Button button1 = new OuiFileSelectSlot.Button();
        button1.Label = Dialog.Clean("FILE_ASSIST_" + (this.AssistModeEnabled ? "ON" : "OFF"), (Language) null);
        button1.Action = new Action(this.OnAssistSelected);
        button1.Scale = 0.7f;
        OuiFileSelectSlot.Button button2 = button1;
        this.assistButton = button1;
        OuiFileSelectSlot.Button button3 = button2;
        buttons1.Add(button3);
        if (!Settings.Instance.VariantsUnlocked)
          return;
        List<OuiFileSelectSlot.Button> buttons2 = this.buttons;
        OuiFileSelectSlot.Button button4 = new OuiFileSelectSlot.Button();
        button4.Label = "[BETA] " + Dialog.Clean("FILE_VARIANT_" + (this.VariantModeEnabled ? "ON" : "OFF"), (Language) null);
        button4.Action = new Action(this.OnVariantSelected);
        button4.Scale = 0.7f;
        OuiFileSelectSlot.Button button5 = button4;
        this.variantButton = button4;
        OuiFileSelectSlot.Button button6 = button5;
        buttons2.Add(button6);
      }
    }

    private void OnContinueSelected()
    {
      this.StartingGame = true;
      Audio.Play("event:/ui/main/savefile_begin");
      SaveData.Start(this.SaveData, this.FileSlot);
      SaveData.Instance.AssistMode = this.AssistModeEnabled;
      SaveData.Instance.VariantMode = this.VariantModeEnabled;
      SaveData.Instance.AssistModeChecks();
      if (SaveData.Instance.CurrentSession != null && SaveData.Instance.CurrentSession.InArea)
      {
        Audio.SetMusic((string) null, true, true);
        Audio.SetAmbience((string) null, true);
        this.fileSelect.Overworld.ShowInputUI = false;
        FadeWipe fadeWipe = new FadeWipe(this.Scene, false, (Action) (() => LevelEnter.Go(SaveData.Instance.CurrentSession, true)));
      }
      else if (SaveData.Instance.Areas[0].Modes[0].Completed || SaveData.Instance.CheatMode)
      {
        if (SaveData.Instance.CurrentSession != null && SaveData.Instance.CurrentSession.ShouldAdvance)
          SaveData.Instance.LastArea.ID = SaveData.Instance.UnlockedAreas;
        SaveData.Instance.CurrentSession = (Session) null;
        (this.Scene as Overworld).Goto<OuiChapterSelect>();
      }
      else
      {
        Audio.SetMusic((string) null, true, true);
        Audio.SetAmbience((string) null, true);
        this.EnterFirstArea();
      }
    }

    private void OnDeleteSelected()
    {
      this.deleting = true;
      this.wiggler.Start();
      Audio.Play("event:/ui/main/message_confirm");
    }

    private void OnNewGameSelected()
    {
      Audio.SetMusic((string) null, true, true);
      Audio.SetAmbience((string) null, true);
      Audio.Play("event:/ui/main/savefile_begin");
      SaveData.Start(new SaveData()
      {
        Name = this.Name,
        AssistMode = this.AssistModeEnabled,
        VariantMode = this.VariantModeEnabled
      }, this.FileSlot);
      this.StartingGame = true;
      this.EnterFirstArea();
    }

    private void EnterFirstArea()
    {
      this.fileSelect.Overworld.Maddy.Disabled = true;
      this.fileSelect.Overworld.ShowInputUI = false;
      this.Add((Component) new Coroutine(this.EnterFirstAreaRoutine(), true));
    }

    private IEnumerator EnterFirstAreaRoutine()
    {
      Overworld overworld = this.fileSelect.Overworld;
      yield return (object) this.fileSelect.Leave((Oui) null);
      yield return (object) overworld.Mountain.EaseCamera(0, AreaData.Areas[0].MountainIdle, new float?(), true, false);
      yield return (object) 0.3f;
      double num = (double) overworld.Mountain.EaseCamera(0, AreaData.Areas[0].MountainZoom, new float?(1f), true, false);
      yield return (object) 0.4f;
      AreaData.Areas[0].Wipe((Scene) overworld, false, (Action) null);
      overworld.RendererList.UpdateLists();
      overworld.RendererList.MoveToFront((Monocle.Renderer) overworld.Snow);
      yield return (object) 0.5f;
      LevelEnter.Go(new Session(new AreaKey(0, AreaMode.Normal), (string) null, (AreaStats) null), false);
    }

    private void OnRenameSelected()
    {
      this.Renaming = true;
      OuiFileNaming ouiFileNaming = this.fileSelect.Overworld.Goto<OuiFileNaming>();
      ouiFileNaming.FileSlot = this;
      ouiFileNaming.StartingName = this.Name;
      Audio.Play("event:/ui/main/savefile_rename_start");
    }

    private void OnAssistSelected()
    {
      this.Assisting = true;
      this.fileSelect.Overworld.Goto<OuiAssistMode>().FileSlot = this;
      Audio.Play("event:/ui/main/assist_button_info");
    }

    private void OnVariantSelected()
    {
      if (!Settings.Instance.VariantsUnlocked && (this.SaveData == null || !this.SaveData.CheatMode))
        return;
      this.VariantModeEnabled = !this.VariantModeEnabled;
      if (this.VariantModeEnabled)
        this.AssistModeEnabled = false;
      this.assistButton.Label = Dialog.Clean("FILE_ASSIST_" + (this.AssistModeEnabled ? "ON" : "OFF"), (Language) null);
      this.variantButton.Label = "[BETA] " + Dialog.Clean("FILE_VARIANT_" + (this.VariantModeEnabled ? "ON" : "OFF"), (Language) null);
    }

    public Vector2 HiddenPosition(int x, int y)
    {
      if (!this.selected)
        return Vector2.op_Addition(new Vector2(960f, this.Y), Vector2.op_Multiply(new Vector2((float) x, (float) y), new Vector2(1920f, 1080f)));
      return Vector2.op_Addition(Vector2.op_Division(new Vector2(1920f, 1080f), 2f), Vector2.op_Multiply(new Vector2((float) x, (float) y), new Vector2(1920f, 1080f)));
    }

    public void Show()
    {
      this.Visible = true;
      this.deleting = false;
      this.StartingGame = false;
      this.Renaming = false;
      this.Assisting = false;
      this.selectedEase = 0.0f;
      this.highlightEase = 0.0f;
      this.highlightEaseDelay = 0.35f;
      Vector2 from = this.Position;
      this.StartTween(0.25f, (Action<Tween>) (f => this.Position = Vector2.Lerp(from, this.IdlePosition, f.Eased)), false);
    }

    public void Select(bool resetButtonIndex)
    {
      this.Visible = true;
      this.deleting = false;
      this.StartingGame = false;
      this.Renaming = false;
      this.Assisting = false;
      this.CreateButtons();
      Vector2 from = this.Position;
      this.wiggler.Start();
      if (resetButtonIndex)
        this.buttonIndex = 0;
      this.deleteIndex = 1;
      this.inputDelay = 0.1f;
      this.StartTween(0.25f, (Action<Tween>) (f =>
      {
        this.Position = Vector2.Lerp(from, this.SelectedPosition, this.selectedEase = f.Eased);
        this.newgameFade = Math.Max(this.newgameFade, f.Eased);
      }), false);
    }

    public void Unselect()
    {
      Vector2 from = this.Position;
      this.buttonIndex = 0;
      this.StartTween(0.25f, (Action<Tween>) (f =>
      {
        this.selectedEase = 1f - f.Eased;
        this.newgameFade = 1f - f.Eased;
        this.Position = Vector2.Lerp(from, this.IdlePosition, f.Eased);
      }), false);
    }

    public void MoveTo(float x, float y)
    {
      Vector2 from = this.Position;
      Vector2 to = new Vector2(x, y);
      this.StartTween(0.25f, (Action<Tween>) (f => this.Position = Vector2.Lerp(from, to, f.Eased)), false);
    }

    public void Hide(int x, int y)
    {
      Vector2 from = this.Position;
      Vector2 to = this.HiddenPosition(x, y);
      this.StartTween(0.25f, (Action<Tween>) (f => this.Position = Vector2.Lerp(from, to, f.Eased)), true);
    }

    private void StartTween(float duration, Action<Tween> callback, bool hide = false)
    {
      if (this.tween != null && this.tween.Entity == this)
        this.tween.RemoveSelf();
      this.Add((Component) (this.tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeInOut, duration, false)));
      this.tween.OnUpdate = callback;
      this.tween.OnComplete = (Action<Tween>) (t =>
      {
        if (hide)
          this.Visible = false;
        this.tween = (Tween) null;
      });
      this.tween.Start();
    }

    public override void Update()
    {
      this.inputDelay -= Engine.DeltaTime;
      if (this.selected && this.fileSelect.Selected && (this.fileSelect.Focused && !this.StartingGame) && (this.tween == null && (double) this.inputDelay <= 0.0 && !this.StartingGame))
      {
        if (this.deleting)
        {
          if (Input.MenuCancel.Pressed)
          {
            this.deleting = false;
            this.wiggler.Start();
            Audio.Play("event:/ui/main/button_back");
          }
          else if (Input.MenuUp.Pressed && this.deleteIndex > 0)
          {
            this.deleteIndex = 0;
            this.wiggler.Start();
            Audio.Play("event:/ui/main/rollover_up");
          }
          else if (Input.MenuDown.Pressed && this.deleteIndex < 1)
          {
            this.deleteIndex = 1;
            this.wiggler.Start();
            Audio.Play("event:/ui/main/rollover_down");
          }
          else if (Input.MenuConfirm.Pressed)
          {
            if (this.deleteIndex == 1)
            {
              this.deleting = false;
              this.wiggler.Start();
              Audio.Play("event:/ui/main/button_back");
            }
            else if (SaveData.TryDelete(this.FileSlot))
            {
              this.Exists = false;
              this.Corrupted = false;
              this.deleting = false;
              this.deletingEase = 0.0f;
              this.fileSelect.UnselectHighlighted();
              Audio.Play("event:/ui/main/savefile_delete");
              if (!Settings.Instance.DisableFlashes)
                this.screenFlash = 1f;
              this.CreateButtons();
            }
            else
            {
              this.failedToDeleteEase = 0.0f;
              this.failedToDeleteTimer = 3f;
              Audio.Play("event:/ui/main/button_invalid");
            }
          }
        }
        else if (Input.MenuCancel.Pressed)
        {
          if (this.fileSelect.HasSlots)
          {
            this.fileSelect.UnselectHighlighted();
            Audio.Play("event:/ui/main/whoosh_savefile_in");
            Audio.Play("event:/ui/main/button_back");
          }
        }
        else if (Input.MenuUp.Pressed && this.buttonIndex > 0)
        {
          --this.buttonIndex;
          this.wiggler.Start();
          Audio.Play("event:/ui/main/rollover_up");
        }
        else if (Input.MenuDown.Pressed && this.buttonIndex < this.buttons.Count - 1)
        {
          ++this.buttonIndex;
          this.wiggler.Start();
          Audio.Play("event:/ui/main/rollover_down");
        }
        else if (Input.MenuConfirm.Pressed)
          this.buttons[this.buttonIndex].Action();
      }
      if ((double) this.highlightEaseDelay <= 0.0)
        this.highlightEase = Calc.Approach(this.highlightEase, !this.highlighted || !this.Exists && this.selected ? 0.0f : 1f, Engine.DeltaTime * 4f);
      else
        this.highlightEaseDelay -= Engine.DeltaTime;
      this.Depth = this.highlighted ? -10 : 0;
      if (this.Renaming || this.Assisting)
        this.selectedEase = Calc.Approach(this.selectedEase, 0.0f, Engine.DeltaTime * 4f);
      this.deletingEase = Calc.Approach(this.deletingEase, this.deleting ? 1f : 0.0f, Engine.DeltaTime * 4f);
      this.failedToDeleteEase = Calc.Approach(this.failedToDeleteEase, (double) this.failedToDeleteTimer > 0.0 ? 1f : 0.0f, Engine.DeltaTime * 4f);
      this.failedToDeleteTimer -= Engine.DeltaTime;
      this.screenFlash = Calc.Approach(this.screenFlash, 0.0f, Engine.DeltaTime * 4f);
      base.Update();
    }

    public override void Render()
    {
      MTexture mtexture1 = GFX.Gui["fileselect/card"];
      MTexture mtexture2 = GFX.Gui["fileselect/ticket"];
      float num1 = Ease.CubeInOut(this.highlightEase);
      float num2 = this.wiggler.Value * 8f;
      if ((double) this.selectedEase > 0.0)
      {
        Vector2 vector2_1 = Vector2.op_Addition(this.Position, new Vector2(0.0f, (float) (350.0 * (double) this.selectedEase - 150.0)));
        float lineHeight = ActiveFont.LineHeight;
        for (int index = 0; index < this.buttons.Count; ++index)
        {
          OuiFileSelectSlot.Button button = this.buttons[index];
          Vector2 vector2_2 = Vector2.op_Multiply(Vector2.get_UnitX(), this.buttonIndex != index || this.deleting ? 0.0f : num2);
          Color color = this.SelectionColor(this.buttonIndex == index && !this.deleting);
          ActiveFont.DrawOutline(button.Label, Vector2.op_Addition(vector2_1, vector2_2), new Vector2(0.5f, 0.0f), Vector2.op_Multiply(Vector2.get_One(), button.Scale), color, 2f, Color.get_Black());
          ref __Null local = ref vector2_1.Y;
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          ^(float&) ref local = ^(float&) ref local + (float) ((double) lineHeight * (double) button.Scale + 15.0);
        }
      }
      Vector2 position1 = Vector2.op_Addition(this.Position, Vector2.op_Multiply(Vector2.op_Multiply(Vector2.get_UnitX(), num1), 360f));
      mtexture2.DrawCentered(position1);
      if ((double) this.highlightEase > 0.0 && this.Exists && !this.Corrupted)
      {
        int num3 = -280;
        int num4 = 600;
        for (int index1 = 0; index1 < this.Cassettes.Count; ++index1)
        {
          GFX.Gui[this.Cassettes[index1] ? "fileselect/cassette" : "fileselect/dot"].DrawCentered(Vector2.op_Addition(position1, new Vector2((float) num3 + (float) (((double) index1 + 0.5) * 75.0), -75f)));
          bool[] heartGem = this.HeartGems[index1];
          int num5 = 0;
          for (int index2 = 0; index2 < heartGem.Length; ++index2)
          {
            if (heartGem[index2])
              ++num5;
          }
          Vector2 position2 = Vector2.op_Addition(position1, new Vector2((float) num3 + (float) (((double) index1 + 0.5) * 75.0), -12f));
          if (num5 == 0)
          {
            GFX.Gui["fileselect/dot"].DrawCentered(position2);
          }
          else
          {
            ref __Null local = ref position2.Y;
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            ^(float&) ref local = ^(float&) ref local - (float) ((double) (num5 - 1) * 0.5 * 14.0);
            int index2 = 0;
            int num6 = 0;
            for (; index2 < heartGem.Length; ++index2)
            {
              if (heartGem[index2])
              {
                GFX.Gui["fileselect/heartgem" + (object) index2].DrawCentered(Vector2.op_Addition(position2, new Vector2(0.0f, (float) (num6 * 14))));
                ++num6;
              }
            }
          }
        }
        this.Deaths.Position = Vector2.op_Subtraction(Vector2.op_Addition(position1, new Vector2((float) num3, 68f)), this.Position);
        this.Deaths.Render();
        ActiveFont.Draw(this.Time, Vector2.op_Addition(position1, new Vector2((float) (num3 + num4), 68f)), new Vector2(1f, 0.5f), Vector2.op_Multiply(Vector2.get_One(), this.timeScale), Color.op_Multiply(Color.get_Black(), 0.6f));
      }
      else if (this.Corrupted)
        ActiveFont.Draw(Dialog.Clean("file_corrupted", (Language) null), position1, new Vector2(0.5f, 0.5f), Vector2.get_One(), Color.op_Multiply(Color.get_Black(), 0.8f));
      else if (!this.Exists)
        ActiveFont.Draw(Dialog.Clean("file_newgame", (Language) null), position1, new Vector2(0.5f, 0.5f), Vector2.get_One(), Color.op_Multiply(Color.get_Black(), 0.8f));
      Vector2 position3 = Vector2.op_Subtraction(this.Position, Vector2.op_Multiply(Vector2.op_Multiply(Vector2.get_UnitX(), num1), 360f));
      mtexture1.DrawCentered(position3);
      if (!this.Corrupted)
      {
        int num3 = 64;
        int num4 = 16;
        float num5 = (float) (mtexture1.Width - num3 * 2) - 200f - (float) num4;
        float num6 = (float) ((double) (-mtexture1.Width / 2 + num3) + 200.0 + (double) num4 + (double) num5 / 2.0);
        float num7 = this.Exists ? 1f : this.newgameFade;
        if ((double) this.newgameFade > 0.0 || this.Exists)
        {
          if (this.AssistModeEnabled)
            GFX.Gui["fileselect/assist"].DrawCentered(position3, Color.op_Multiply(Color.get_White(), num7));
          else if (this.VariantModeEnabled)
            GFX.Gui["fileselect/variants"].DrawCentered(position3, Color.op_Multiply(Color.get_White(), num7));
        }
        if (this.Exists)
        {
          if (this.SaveData.TotalStrawberries >= 175)
            GFX.Gui["fileselect/strawberry"].DrawCentered(position3, Color.op_Multiply(Color.get_White(), num7));
          if (this.SaveData.Areas.Count > 7 && this.SaveData.Areas[7].Modes[0].Completed)
            GFX.Gui["fileselect/flag"].DrawCentered(position3, Color.op_Multiply(Color.get_White(), num7));
          if (this.SaveData.TotalHeartGems >= 16)
            GFX.Gui["fileselect/heart"].DrawCentered(position3, Color.op_Multiply(Color.get_White(), num7));
          if (this.SaveData.CheatMode)
            GFX.Gui["fileselect/cheatmode"].DrawCentered(position3, Color.op_Multiply(Color.get_White(), num7));
          if (this.SaveData.TotalGoldenStrawberries >= 25)
            GFX.Gui["fileselect/goldberry"].DrawCentered(position3, Color.op_Multiply(Color.get_White(), num7));
          if (this.SaveData.TotalHeartGems >= 24)
            GFX.Gui["fileselect/goldheart"].DrawCentered(position3, Color.op_Multiply(Color.get_White(), num7));
        }
        if (this.Exists || this.Renaming || (double) this.newgameFade > 0.0)
        {
          this.Portrait.RenderPosition = Vector2.op_Addition(position3, new Vector2((float) (-mtexture1.Width / 2 + num3) + 100f, 0.0f));
          this.Portrait.Color = Color.op_Multiply(Color.get_White(), num7);
          this.Portrait.Render();
          GFX.Gui["fileselect/portraitOverlay"].DrawCentered(this.Portrait.RenderPosition, Color.op_Multiply(Color.get_White(), num7));
          Vector2 position2 = Vector2.op_Addition(position3, new Vector2(num6, (float) ((this.Exists ? 0 : 64) - 32)));
          float num8 = Math.Min(1f, (float) (440.0 / ActiveFont.Measure(this.Name).X));
          ActiveFont.Draw(this.Name, position2, new Vector2(0.5f, 1f), Vector2.op_Multiply(Vector2.get_One(), num8), Color.op_Multiply(Color.op_Multiply(Color.get_Black(), 0.8f), num7));
          if (this.Renaming && this.Scene.BetweenInterval(0.3f))
            ActiveFont.Draw("|", new Vector2((float) (position2.X + ActiveFont.Measure(this.Name).X * (double) num8 * 0.5), (float) position2.Y), new Vector2(0.0f, 1f), Vector2.op_Multiply(Vector2.get_One(), num8), Color.op_Multiply(Color.op_Multiply(Color.get_Black(), 0.8f), num7));
        }
        if (this.Exists)
        {
          if (this.FurthestArea < AreaData.Areas.Count)
            ActiveFont.Draw(Dialog.Clean(AreaData.Areas[this.FurthestArea].Name, (Language) null), Vector2.op_Addition(position3, new Vector2(num6, -10f)), new Vector2(0.5f, 0.5f), Vector2.op_Multiply(Vector2.get_One(), 0.8f), Color.op_Multiply(Color.get_Black(), 0.6f));
          this.Strawberries.Position = Vector2.op_Subtraction(Vector2.op_Addition(position3, new Vector2(num6, 55f)), this.Position);
          this.Strawberries.Render();
        }
      }
      else
        ActiveFont.Draw(Dialog.Clean("file_failedtoload", (Language) null), position3, new Vector2(0.5f, 0.5f), Vector2.get_One(), Color.op_Multiply(Color.get_Black(), 0.8f));
      if ((double) this.deletingEase > 0.0)
      {
        float num3 = Ease.CubeOut(this.deletingEase);
        Vector2 vector2;
        ((Vector2) ref vector2).\u002Ector(960f, 540f);
        float lineHeight = ActiveFont.LineHeight;
        Draw.Rect(-10f, -10f, 1940f, 1100f, Color.op_Multiply(Color.op_Multiply(Color.get_Black(), num3), 0.9f));
        ActiveFont.Draw(Dialog.Clean("file_delete_really", (Language) null), Vector2.op_Addition(vector2, new Vector2(0.0f, (float) (-16.0 - 64.0 * (1.0 - (double) num3)))), new Vector2(0.5f, 1f), Vector2.get_One(), Color.op_Multiply(Color.get_White(), num3));
        ActiveFont.DrawOutline(Dialog.Clean("file_delete_yes", (Language) null), Vector2.op_Addition(vector2, new Vector2((float) ((!this.deleting || this.deleteIndex != 0 ? 0.0 : (double) num2) * 1.20000004768372) * num3, (float) (16.0 + 64.0 * (1.0 - (double) num3)))), new Vector2(0.5f, 0.0f), Vector2.op_Multiply(Vector2.get_One(), 0.8f), this.deleting ? this.SelectionColor(this.deleteIndex == 0) : Color.get_Gray(), 2f, Color.op_Multiply(Color.get_Black(), num3));
        ActiveFont.DrawOutline(Dialog.Clean("file_delete_no", (Language) null), Vector2.op_Addition(vector2, new Vector2((float) ((!this.deleting || this.deleteIndex != 1 ? 0.0 : (double) num2) * 1.20000004768372) * num3, (float) (16.0 + (double) lineHeight + 64.0 * (1.0 - (double) num3)))), new Vector2(0.5f, 0.0f), Vector2.op_Multiply(Vector2.get_One(), 0.8f), this.deleting ? this.SelectionColor(this.deleteIndex == 1) : Color.get_Gray(), 2f, Color.op_Multiply(Color.get_Black(), num3));
        if ((double) this.failedToDeleteEase > 0.0)
        {
          Vector2 position2;
          ((Vector2) ref position2).\u002Ector(960f, (float) (980.0 - 100.0 * (double) this.deletingEase));
          Vector2 scale = Vector2.op_Multiply(Vector2.get_One(), 0.8f);
          if ((double) this.failedToDeleteEase < 1.0 && (double) this.failedToDeleteTimer > 0.0)
          {
            position2 = Vector2.op_Addition(position2, new Vector2((float) (Calc.Random.Next(10) - 5), (float) (Calc.Random.Next(10) - 5)));
            scale = Vector2.op_Multiply(Vector2.get_One(), (float) (0.800000011920929 + 0.200000002980232 * (1.0 - (double) this.failedToDeleteEase)));
          }
          ActiveFont.DrawOutline(Dialog.Clean("file_delete_failed", (Language) null), position2, new Vector2(0.5f, 0.0f), scale, Color.op_Multiply(Color.get_PaleVioletRed(), this.deletingEase), 2f, Color.op_Multiply(Color.get_Black(), this.deletingEase));
        }
      }
      if ((double) this.screenFlash <= 0.0)
        return;
      Draw.Rect(-10f, -10f, 1940f, 1100f, Color.op_Multiply(Color.get_White(), Ease.CubeOut(this.screenFlash)));
    }

    public Color SelectionColor(bool selected)
    {
      if (!selected)
        return Color.get_White();
      if (!Settings.Instance.DisableFlashes && !this.Scene.BetweenInterval(0.1f))
        return TextMenu.HighlightColorB;
      return TextMenu.HighlightColorA;
    }

    private class Button
    {
      public float Scale = 1f;
      public string Label;
      public Action Action;
    }
  }
}
