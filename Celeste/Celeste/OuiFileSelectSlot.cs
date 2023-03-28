// Decompiled with JetBrains decompiler
// Type: Celeste.OuiFileSelectSlot
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Celeste
{
  public class OuiFileSelectSlot : Entity
  {
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
    public List<bool> Cassettes = new List<bool>();
    public List<bool[]> HeartGems = new List<bool[]>();
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
    private float timeScale = 1f;
    private OuiFileSelectSlot.Button assistButton;
    private OuiFileSelectSlot.Button variantButton;
    private Sprite normalCard;
    private Sprite goldCard;
    private Sprite normalTicket;
    private Sprite goldTicket;
    private List<OuiFileSelectSlot.Button> buttons = new List<OuiFileSelectSlot.Button>();

    public Vector2 IdlePosition => new Vector2(960f, (float) (540 + 310 * (this.FileSlot - 1)));

    public Vector2 SelectedPosition => new Vector2(960f, 440f);

    private bool highlighted => this.fileSelect.SlotIndex == this.FileSlot;

    private bool selected => this.fileSelect.SlotSelected && this.highlighted;

    private bool Golden => !this.Corrupted && this.Exists && this.SaveData.TotalStrawberries >= 202;

    private Sprite Card => !this.Golden ? this.normalCard : this.goldCard;

    private Sprite Ticket => !this.Golden ? this.normalTicket : this.goldTicket;

    private OuiFileSelectSlot(int index, OuiFileSelect fileSelect)
    {
      this.FileSlot = index;
      this.fileSelect = fileSelect;
      this.Tag |= (int) Tags.HUD | (int) Tags.PauseUpdate;
      this.Visible = false;
      this.Add((Component) (this.wiggler = Wiggler.Create(0.4f, 4f)));
      this.normalTicket = new Sprite(MTN.FileSelect, "ticket");
      this.normalTicket.AddLoop("idle", "", 0.1f);
      this.normalTicket.Add("shine", "", 0.1f, "idle");
      this.normalTicket.CenterOrigin();
      this.normalTicket.Play("idle");
      this.normalCard = new Sprite(MTN.FileSelect, "card");
      this.normalCard.AddLoop("idle", "", 0.1f);
      this.normalCard.Add("shine", "", 0.1f, "idle");
      this.normalCard.CenterOrigin();
      this.normalCard.Play("idle");
      this.goldTicket = new Sprite(MTN.FileSelect, "ticketShine");
      this.goldTicket.AddLoop("idle", "", 0.1f, new int[1]);
      this.goldTicket.Add("shine", "", 0.05f, "idle", 0, 0, 0, 0, 0, 1, 2, 3, 4, 5);
      this.goldTicket.CenterOrigin();
      this.goldTicket.Play("idle");
      this.goldCard = new Sprite(MTN.FileSelect, "cardShine");
      this.goldCard.AddLoop("idle", "", 0.1f, 5);
      this.goldCard.Add("shine", "", 0.05f, "idle");
      this.goldCard.CenterOrigin();
      this.goldCard.Play("idle");
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
      if (!Dialog.Language.CanDisplay(this.Name))
        this.Name = Dialog.Clean("FILE_DEFAULT");
      if (!Settings.Instance.VariantsUnlocked && data.TotalHeartGems >= 24)
        Settings.Instance.VariantsUnlocked = true;
      this.AssistModeEnabled = data.AssistMode;
      this.VariantModeEnabled = data.VariantMode;
      this.Add((Component) (this.Deaths = new DeathsCounter(AreaMode.Normal, false, data.TotalDeaths)));
      this.Add((Component) (this.Strawberries = new StrawberriesCounter(true, data.TotalStrawberries)));
      this.Time = Dialog.FileTime(data.Time);
      if (TimeSpan.FromTicks(data.Time).TotalHours > 0.0)
        this.timeScale = 0.725f;
      this.FurthestArea = data.UnlockedAreas;
      foreach (AreaStats area in data.Areas)
      {
        if (area.ID <= data.UnlockedAreas)
        {
          if (!AreaData.Areas[area.ID].Interlude && AreaData.Areas[area.ID].CanFullClear)
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
      string str = "portrait_madeline";
      string id = "idle_normal";
      this.Portrait = GFX.PortraitsSpriteBank.Create(str);
      this.Portrait.Play(id);
      this.Portrait.Scale = Vector2.One * (200f / (float) GFX.PortraitsSpriteBank.SpriteData[str].Sources[0].XML.AttrInt("size", 160));
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
            Label = Dialog.Clean("file_continue"),
            Action = new Action(this.OnContinueSelected)
          });
          if (this.SaveData != null)
          {
            List<OuiFileSelectSlot.Button> buttons1 = this.buttons;
            OuiFileSelectSlot.Button button1 = new OuiFileSelectSlot.Button();
            button1.Label = Dialog.Clean("FILE_ASSIST_" + (this.AssistModeEnabled ? "ON" : "OFF"));
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
              button4.Label = Dialog.Clean("FILE_VARIANT_" + (this.VariantModeEnabled ? "ON" : "OFF"));
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
          Label = Dialog.Clean("file_delete"),
          Action = new Action(this.OnDeleteSelected),
          Scale = 0.7f
        });
      }
      else
      {
        this.buttons.Add(new OuiFileSelectSlot.Button()
        {
          Label = Dialog.Clean("file_begin"),
          Action = new Action(this.OnNewGameSelected)
        });
        this.buttons.Add(new OuiFileSelectSlot.Button()
        {
          Label = Dialog.Clean("file_rename"),
          Action = new Action(this.OnRenameSelected),
          Scale = 0.7f
        });
        List<OuiFileSelectSlot.Button> buttons3 = this.buttons;
        OuiFileSelectSlot.Button button7 = new OuiFileSelectSlot.Button();
        button7.Label = Dialog.Clean("FILE_ASSIST_" + (this.AssistModeEnabled ? "ON" : "OFF"));
        button7.Action = new Action(this.OnAssistSelected);
        button7.Scale = 0.7f;
        OuiFileSelectSlot.Button button8 = button7;
        this.assistButton = button7;
        OuiFileSelectSlot.Button button9 = button8;
        buttons3.Add(button9);
        if (!Settings.Instance.VariantsUnlocked)
          return;
        List<OuiFileSelectSlot.Button> buttons4 = this.buttons;
        OuiFileSelectSlot.Button button10 = new OuiFileSelectSlot.Button();
        button10.Label = Dialog.Clean("FILE_VARIANT_" + (this.VariantModeEnabled ? "ON" : "OFF"));
        button10.Action = new Action(this.OnVariantSelected);
        button10.Scale = 0.7f;
        OuiFileSelectSlot.Button button11 = button10;
        this.variantButton = button10;
        OuiFileSelectSlot.Button button12 = button11;
        buttons4.Add(button12);
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
        Audio.SetMusic((string) null);
        Audio.SetAmbience((string) null);
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
        Audio.SetMusic((string) null);
        Audio.SetAmbience((string) null);
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
      Audio.SetMusic((string) null);
      Audio.SetAmbience((string) null);
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
      this.Add((Component) new Coroutine(this.EnterFirstAreaRoutine()));
    }

    private IEnumerator EnterFirstAreaRoutine()
    {
      Overworld overworld = this.fileSelect.Overworld;
      yield return (object) this.fileSelect.Leave((Oui) null);
      yield return (object) overworld.Mountain.EaseCamera(0, AreaData.Areas[0].MountainIdle);
      yield return (object) 0.3f;
      double num = (double) overworld.Mountain.EaseCamera(0, AreaData.Areas[0].MountainZoom, new float?(1f));
      yield return (object) 0.4f;
      AreaData.Areas[0].Wipe((Scene) overworld, false, (Action) null);
      overworld.RendererList.UpdateLists();
      overworld.RendererList.MoveToFront((Monocle.Renderer) overworld.Snow);
      yield return (object) 0.5f;
      LevelEnter.Go(new Session(new AreaKey(0)), false);
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
      {
        this.AssistModeEnabled = false;
        Audio.Play("event:/ui/main/button_toggle_on");
      }
      else
        Audio.Play("event:/ui/main/button_toggle_off");
      this.assistButton.Label = Dialog.Clean("FILE_ASSIST_" + (this.AssistModeEnabled ? "ON" : "OFF"));
      this.variantButton.Label = Dialog.Clean("FILE_VARIANT_" + (this.VariantModeEnabled ? "ON" : "OFF"));
    }

    public Vector2 HiddenPosition(int x, int y) => !this.selected ? new Vector2(960f, this.Y) + new Vector2((float) x, (float) y) * new Vector2(1920f, 1080f) : new Vector2(1920f, 1080f) / 2f + new Vector2((float) x, (float) y) * new Vector2(1920f, 1080f);

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
      this.StartTween(0.25f, (Action<Tween>) (f => this.Position = Vector2.Lerp(from, this.IdlePosition, f.Eased)));
    }

    public void Select(bool resetButtonIndex)
    {
      this.Visible = true;
      this.deleting = false;
      this.StartingGame = false;
      this.Renaming = false;
      this.Assisting = false;
      this.CreateButtons();
      this.Card.Play("shine");
      this.Ticket.Play("shine");
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
      }));
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
      }));
    }

    public void MoveTo(float x, float y)
    {
      Vector2 from = this.Position;
      Vector2 to = new Vector2(x, y);
      this.StartTween(0.25f, (Action<Tween>) (f => this.Position = Vector2.Lerp(from, to, f.Eased)));
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
      this.Add((Component) (this.tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeInOut, duration)));
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
      this.Ticket.Update();
      this.Card.Update();
      if (this.selected && this.fileSelect.Selected && this.fileSelect.Focused && !this.StartingGame && this.tween == null && (double) this.inputDelay <= 0.0 && !this.StartingGame)
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
      float num1 = Ease.CubeInOut(this.highlightEase);
      float num2 = this.wiggler.Value * 8f;
      if ((double) this.selectedEase > 0.0)
      {
        Vector2 vector2_1 = this.Position + new Vector2(0.0f, (float) (350.0 * (double) this.selectedEase - 150.0));
        float lineHeight = ActiveFont.LineHeight;
        for (int index = 0; index < this.buttons.Count; ++index)
        {
          OuiFileSelectSlot.Button button = this.buttons[index];
          Vector2 vector2_2 = Vector2.UnitX * (this.buttonIndex != index || this.deleting ? 0.0f : num2);
          Color color = this.SelectionColor(this.buttonIndex == index && !this.deleting);
          ActiveFont.DrawOutline(button.Label, vector2_1 + vector2_2, new Vector2(0.5f, 0.0f), Vector2.One * button.Scale, color, 2f, Color.Black);
          vector2_1.Y += (float) ((double) lineHeight * (double) button.Scale + 15.0);
        }
      }
      Vector2 position1 = this.Position + Vector2.UnitX * num1 * 360f;
      this.Ticket.RenderPosition = position1;
      this.Ticket.Render();
      if ((double) this.highlightEase > 0.0 && this.Exists && !this.Corrupted)
      {
        int x = -280;
        int num3 = 600;
        for (int index1 = 0; index1 < this.Cassettes.Count; ++index1)
        {
          MTN.FileSelect[this.Cassettes[index1] ? "cassette" : "dot"].DrawCentered(position1 + new Vector2((float) x + (float) (((double) index1 + 0.5) * 75.0), -75f));
          bool[] heartGem = this.HeartGems[index1];
          int num4 = 0;
          for (int index2 = 0; index2 < heartGem.Length; ++index2)
          {
            if (heartGem[index2])
              ++num4;
          }
          Vector2 position2 = position1 + new Vector2((float) x + (float) (((double) index1 + 0.5) * 75.0), -12f);
          if (num4 == 0)
          {
            MTN.FileSelect["dot"].DrawCentered(position2);
          }
          else
          {
            position2.Y -= (float) ((double) (num4 - 1) * 0.5 * 14.0);
            int index3 = 0;
            int num5 = 0;
            for (; index3 < heartGem.Length; ++index3)
            {
              if (heartGem[index3])
              {
                MTN.FileSelect["heartgem" + (object) index3].DrawCentered(position2 + new Vector2(0.0f, (float) (num5 * 14)));
                ++num5;
              }
            }
          }
        }
        this.Deaths.Position = position1 + new Vector2((float) x, 68f) - this.Position;
        this.Deaths.Render();
        ActiveFont.Draw(this.Time, position1 + new Vector2((float) (x + num3), 68f), new Vector2(1f, 0.5f), Vector2.One * this.timeScale, Color.Black * 0.6f);
      }
      else if (this.Corrupted)
        ActiveFont.Draw(Dialog.Clean("file_corrupted"), position1, new Vector2(0.5f, 0.5f), Vector2.One, Color.Black * 0.8f);
      else if (!this.Exists)
        ActiveFont.Draw(Dialog.Clean("file_newgame"), position1, new Vector2(0.5f, 0.5f), Vector2.One, Color.Black * 0.8f);
      Vector2 position3 = this.Position - Vector2.UnitX * num1 * 360f;
      int num6 = 64;
      int num7 = 16;
      float num8 = (float) ((double) this.Card.Width - (double) (num6 * 2) - 200.0) - (float) num7;
      float x1 = (float) (-(double) this.Card.Width / 2.0 + (double) num6 + 200.0 + (double) num7 + (double) num8 / 2.0);
      float num9 = this.Exists ? 1f : this.newgameFade;
      if (!this.Corrupted)
      {
        if ((double) this.newgameFade > 0.0 || this.Exists)
        {
          if (this.AssistModeEnabled)
            MTN.FileSelect["assist"].DrawCentered(position3, Color.White * num9);
          else if (this.VariantModeEnabled)
            MTN.FileSelect["variants"].DrawCentered(position3, Color.White * num9);
        }
        if (this.Exists && this.SaveData.CheatMode)
          MTN.FileSelect["cheatmode"].DrawCentered(position3, Color.White * num9);
      }
      this.Card.RenderPosition = position3;
      this.Card.Render();
      if (!this.Corrupted)
      {
        if (this.Exists)
        {
          if (this.SaveData.TotalStrawberries >= 175)
            MTN.FileSelect["strawberry"].DrawCentered(position3, Color.White * num9);
          if (this.SaveData.Areas.Count > 7 && this.SaveData.Areas[7].Modes[0].Completed)
            MTN.FileSelect["flag"].DrawCentered(position3, Color.White * num9);
          if (this.SaveData.TotalCassettes >= 8)
            MTN.FileSelect["cassettes"].DrawCentered(position3, Color.White * num9);
          if (this.SaveData.TotalHeartGems >= 16)
            MTN.FileSelect["heart"].DrawCentered(position3, Color.White * num9);
          if (this.SaveData.TotalGoldenStrawberries >= 25)
            MTN.FileSelect["goldberry"].DrawCentered(position3, Color.White * num9);
          if (this.SaveData.TotalHeartGems >= 24)
            MTN.FileSelect["goldheart"].DrawCentered(position3, Color.White * num9);
          if (this.SaveData.Areas.Count > 10 && this.SaveData.Areas[10].Modes[0].Completed)
            MTN.FileSelect["farewell"].DrawCentered(position3, Color.White * num9);
        }
        if (this.Exists || this.Renaming || (double) this.newgameFade > 0.0)
        {
          this.Portrait.RenderPosition = position3 + new Vector2((float) (-(double) this.Card.Width / 2.0 + (double) num6 + 100.0), 0.0f);
          this.Portrait.Color = Color.White * num9;
          this.Portrait.Render();
          MTN.FileSelect[!this.Golden ? "portraitOverlay" : "portraitOverlayGold"].DrawCentered(this.Portrait.RenderPosition, Color.White * num9);
          string name = this.Name;
          Vector2 position4 = position3 + new Vector2(x1, (float) ((this.Exists ? 0 : 64) - 32));
          float num10 = Math.Min(1f, 440f / ActiveFont.Measure(name).X);
          ActiveFont.Draw(name, position4, new Vector2(0.5f, 1f), Vector2.One * num10, Color.Black * 0.8f * num9);
          if (this.Renaming && this.Scene.BetweenInterval(0.3f))
            ActiveFont.Draw("|", new Vector2(position4.X + (float) ((double) ActiveFont.Measure(name).X * (double) num10 * 0.5), position4.Y), new Vector2(0.0f, 1f), Vector2.One * num10, Color.Black * 0.8f * num9);
        }
        if (this.Exists)
        {
          if (this.FurthestArea < AreaData.Areas.Count)
            ActiveFont.Draw(Dialog.Clean(AreaData.Areas[this.FurthestArea].Name), position3 + new Vector2(x1, -10f), new Vector2(0.5f, 0.5f), Vector2.One * 0.8f, Color.Black * 0.6f);
          this.Strawberries.Position = position3 + new Vector2(x1, 55f) - this.Position;
          this.Strawberries.Render();
        }
      }
      else
        ActiveFont.Draw(Dialog.Clean("file_failedtoload"), position3, new Vector2(0.5f, 0.5f), Vector2.One, Color.Black * 0.8f);
      if ((double) this.deletingEase > 0.0)
      {
        float num11 = Ease.CubeOut(this.deletingEase);
        Vector2 vector2 = new Vector2(960f, 540f);
        float lineHeight = ActiveFont.LineHeight;
        Draw.Rect(-10f, -10f, 1940f, 1100f, Color.Black * num11 * 0.9f);
        ActiveFont.Draw(Dialog.Clean("file_delete_really"), vector2 + new Vector2(0.0f, (float) (-16.0 - 64.0 * (1.0 - (double) num11))), new Vector2(0.5f, 1f), Vector2.One, Color.White * num11);
        ActiveFont.DrawOutline(Dialog.Clean("file_delete_yes"), vector2 + new Vector2((float) ((!this.deleting || this.deleteIndex != 0 ? 0.0 : (double) num2) * 1.2000000476837158) * num11, (float) (16.0 + 64.0 * (1.0 - (double) num11))), new Vector2(0.5f, 0.0f), Vector2.One * 0.8f, this.deleting ? this.SelectionColor(this.deleteIndex == 0) : Color.Gray, 2f, Color.Black * num11);
        ActiveFont.DrawOutline(Dialog.Clean("file_delete_no"), vector2 + new Vector2((float) ((!this.deleting || this.deleteIndex != 1 ? 0.0 : (double) num2) * 1.2000000476837158) * num11, (float) (16.0 + (double) lineHeight + 64.0 * (1.0 - (double) num11))), new Vector2(0.5f, 0.0f), Vector2.One * 0.8f, this.deleting ? this.SelectionColor(this.deleteIndex == 1) : Color.Gray, 2f, Color.Black * num11);
        if ((double) this.failedToDeleteEase > 0.0)
        {
          Vector2 position5 = new Vector2(960f, (float) (980.0 - 100.0 * (double) this.deletingEase));
          Vector2 scale = Vector2.One * 0.8f;
          if ((double) this.failedToDeleteEase < 1.0 && (double) this.failedToDeleteTimer > 0.0)
          {
            position5 += new Vector2((float) (Calc.Random.Next(10) - 5), (float) (Calc.Random.Next(10) - 5));
            scale = Vector2.One * (float) (0.800000011920929 + 0.20000000298023224 * (1.0 - (double) this.failedToDeleteEase));
          }
          ActiveFont.DrawOutline(Dialog.Clean("file_delete_failed"), position5, new Vector2(0.5f, 0.0f), scale, Color.PaleVioletRed * this.deletingEase, 2f, Color.Black * this.deletingEase);
        }
      }
      if ((double) this.screenFlash <= 0.0)
        return;
      Draw.Rect(-10f, -10f, 1940f, 1100f, Color.White * Ease.CubeOut(this.screenFlash));
    }

    public Color SelectionColor(bool selected)
    {
      if (!selected)
        return Color.White;
      return !Settings.Instance.DisableFlashes && !this.Scene.BetweenInterval(0.1f) ? TextMenu.HighlightColorB : TextMenu.HighlightColorA;
    }

    private class Button
    {
      public string Label;
      public Action Action;
      public float Scale = 1f;
    }
  }
}
