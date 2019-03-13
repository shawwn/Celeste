// Decompiled with JetBrains decompiler
// Type: Celeste.OuiMainMenu
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Celeste.Pico8;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Celeste
{
  public class OuiMainMenu : Oui
  {
    private static readonly Vector2 TargetPosition = new Vector2(160f, 160f);
    private static readonly Vector2 TweenFrom = new Vector2(-500f, 160f);
    private static readonly Color UnselectedColor = Color.White;
    private static readonly Color SelectedColorA = TextMenu.HighlightColorA;
    private static readonly Color SelectedColorB = TextMenu.HighlightColorB;
    private const float IconWidth = 64f;
    private const float IconSpacing = 20f;
    private float ease;
    private MainMenuClimb climbButton;
    private List<MenuButton> buttons;
    private bool startOnOptions;
    private bool mountainStartFront;

    public OuiMainMenu()
    {
      this.buttons = new List<MenuButton>();
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      this.Position = OuiMainMenu.TweenFrom;
      this.CreateButtons();
    }

    public void CreateButtons()
    {
      foreach (Entity button in this.buttons)
        button.RemoveSelf();
      this.buttons.Clear();
      Vector2 targetPosition1 = new Vector2(320f, 160f);
      Vector2 vector2_1 = new Vector2(-640f, 0.0f);
      this.climbButton = new MainMenuClimb((Oui) this, targetPosition1, targetPosition1 + vector2_1, new Action(this.OnBegin));
      if (!this.startOnOptions)
        this.climbButton.StartSelected();
      this.buttons.Add((MenuButton) this.climbButton);
      Vector2 targetPosition2 = targetPosition1 + Vector2.UnitY * this.climbButton.ButtonHeight;
      targetPosition2.X -= 140f;
      if (Celeste.PlayMode == Celeste.PlayModes.Debug)
      {
        MainMenuSmallButton mainMenuSmallButton = new MainMenuSmallButton("menu_debug", "menu/options", (Oui) this, targetPosition2, targetPosition2 + vector2_1, new Action(this.OnDebug));
        this.buttons.Add((MenuButton) mainMenuSmallButton);
        targetPosition2 += Vector2.UnitY * mainMenuSmallButton.ButtonHeight;
      }
      if (Settings.Instance.Pico8OnMainMenu || Celeste.PlayMode == Celeste.PlayModes.Debug || Celeste.PlayMode == Celeste.PlayModes.Event)
      {
        MainMenuSmallButton mainMenuSmallButton = new MainMenuSmallButton("menu_pico8", "menu/pico8", (Oui) this, targetPosition2, targetPosition2 + vector2_1, new Action(this.OnPico8));
        this.buttons.Add((MenuButton) mainMenuSmallButton);
        targetPosition2 += Vector2.UnitY * mainMenuSmallButton.ButtonHeight;
      }
      MainMenuSmallButton mainMenuSmallButton1 = new MainMenuSmallButton("menu_options", "menu/options", (Oui) this, targetPosition2, targetPosition2 + vector2_1, new Action(this.OnOptions));
      if (this.startOnOptions)
        mainMenuSmallButton1.StartSelected();
      this.buttons.Add((MenuButton) mainMenuSmallButton1);
      Vector2 targetPosition3 = targetPosition2 + Vector2.UnitY * mainMenuSmallButton1.ButtonHeight;
      MainMenuSmallButton mainMenuSmallButton2 = new MainMenuSmallButton("menu_credits", "menu/credits", (Oui) this, targetPosition3, targetPosition3 + vector2_1, new Action(this.OnCredits));
      this.buttons.Add((MenuButton) mainMenuSmallButton2);
      Vector2 targetPosition4 = targetPosition3 + Vector2.UnitY * mainMenuSmallButton2.ButtonHeight;
      MainMenuSmallButton mainMenuSmallButton3 = new MainMenuSmallButton("menu_exit", "menu/exit", (Oui) this, targetPosition4, targetPosition4 + vector2_1, new Action(this.OnExit));
      this.buttons.Add((MenuButton) mainMenuSmallButton3);
      Vector2 vector2_2 = targetPosition4 + Vector2.UnitY * mainMenuSmallButton3.ButtonHeight;
      for (int index = 0; index < this.buttons.Count; ++index)
      {
        if (index > 0)
          this.buttons[index].UpButton = this.buttons[index - 1];
        if (index < this.buttons.Count - 1)
          this.buttons[index].DownButton = this.buttons[index + 1];
        this.Scene.Add((Entity) this.buttons[index]);
      }
      if (!this.Visible || !this.Focused)
        return;
      foreach (MenuButton button in this.buttons)
        button.Position = button.TargetPosition;
    }

    public override void Removed(Scene scene)
    {
      foreach (MenuButton button in this.buttons)
        scene.Remove((Entity) button);
      base.Removed(scene);
    }

    public override bool IsStart(Overworld overworld, Overworld.StartMode start)
    {
      if (start == Overworld.StartMode.ReturnFromOptions)
      {
        this.startOnOptions = true;
        this.Add((Component) new Coroutine(this.Enter((Oui) null), true));
        return true;
      }
      if (start != Overworld.StartMode.MainMenu)
        return start == Overworld.StartMode.ReturnFromOptions || start == Overworld.StartMode.ReturnFromPico8;
      this.mountainStartFront = true;
      this.Add((Component) new Coroutine(this.Enter((Oui) null), true));
      return true;
    }

    public override IEnumerator Enter(Oui from)
    {
      if (from is OuiTitleScreen || from is OuiFileSelect)
      {
        Audio.Play("event:/ui/main/whoosh_list_in");
        yield return (object) 0.1f;
      }
      if (from is OuiTitleScreen)
      {
        MenuButton.ClearSelection(this.Scene);
        this.climbButton.StartSelected();
      }
      this.Visible = true;
      if (this.mountainStartFront)
        this.Overworld.Mountain.SnapCamera(-1, new MountainCamera(new Vector3(0.0f, 6f, 12f), MountainRenderer.RotateLookAt), false);
      this.Overworld.Mountain.GotoRotationMode();
      this.Overworld.Maddy.Hide(true);
      foreach (MenuButton button1 in this.buttons)
      {
        MenuButton button = button1;
        button.TweenIn(0.2f);
        button = (MenuButton) null;
      }
      yield return (object) 0.2f;
      this.Focused = true;
      this.mountainStartFront = false;
      yield return (object) null;
    }

    public override IEnumerator Leave(Oui next)
    {
      this.Focused = false;
      Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeInOut, 0.2f, true);
      tween.OnUpdate = (Action<Tween>) (t =>
      {
        this.ease = 1f - t.Eased;
        this.Position = Vector2.Lerp(OuiMainMenu.TargetPosition, OuiMainMenu.TweenFrom, t.Eased);
      });
      this.Add((Component) tween);
      bool keepClimb = this.climbButton.Selected && !(next is OuiTitleScreen);
      foreach (MenuButton button1 in this.buttons)
      {
        MenuButton button = button1;
        if (!(button == this.climbButton & keepClimb))
        {
          button.TweenOut(0.2f);
          button = (MenuButton) null;
        }
      }
      yield return (object) 0.2f;
      if (keepClimb)
        this.Add((Component) new Coroutine(this.SlideClimbOutLate(), true));
      else
        this.Visible = false;
    }

    private IEnumerator SlideClimbOutLate()
    {
      yield return (object) 0.2f;
      this.climbButton.TweenOut(0.2f);
      yield return (object) 0.2f;
      this.Visible = false;
    }

    public Color SelectionColor
    {
      get
      {
        return Settings.Instance.DisableFlashes || this.Scene.BetweenInterval(0.1f) ? OuiMainMenu.SelectedColorA : OuiMainMenu.SelectedColorB;
      }
    }

    public override void Update()
    {
      if (this.Selected && this.Focused && Input.MenuCancel.Pressed)
      {
        this.Focused = false;
        Audio.Play("event:/ui/main/whoosh_list_out");
        Audio.Play("event:/ui/main/button_back");
        this.Overworld.Goto<OuiTitleScreen>();
      }
      base.Update();
    }

    public override void Render()
    {
      foreach (MenuButton button in this.buttons)
      {
        if (button.Scene == this.Scene)
          button.Render();
      }
    }

    private void OnDebug()
    {
      Audio.Play("event:/ui/main/whoosh_list_out");
      Audio.Play("event:/ui/main/button_select");
      SaveData.InitializeDebugMode(true);
      this.Overworld.Goto<OuiChapterSelect>();
    }

    private void OnBegin()
    {
      Audio.Play("event:/ui/main/whoosh_list_out");
      Audio.Play("event:/ui/main/button_climb");
      if (Celeste.PlayMode == Celeste.PlayModes.Event)
      {
        SaveData.InitializeDebugMode(false);
        this.Overworld.Goto<OuiChapterSelect>();
      }
      else
        this.Overworld.Goto<OuiFileSelect>();
    }

    private void OnPico8()
    {
      Audio.Play("event:/ui/main/button_select");
      this.Focused = false;
      FadeWipe fadeWipe = new FadeWipe(this.Scene, false, (Action) (() =>
      {
        this.Focused = true;
        this.Overworld.EnteringPico8 = true;
        SaveData.Instance = (SaveData) null;
        SaveData.NoFileAssistChecks();
        Engine.Scene = (Scene) new Emulator((Scene) this.Overworld, 0, 0);
      }));
    }

    private void OnOptions()
    {
      Audio.Play("event:/ui/main/button_select");
      Audio.Play("event:/ui/main/whoosh_large_in");
      this.Overworld.Goto<OuiOptions>();
    }

    private void OnCredits()
    {
      Audio.Play("event:/ui/main/button_select");
      Audio.Play("event:/ui/main/whoosh_large_in");
      this.Overworld.Goto<OuiCredits>();
    }

    private void OnExit()
    {
      Audio.Play("event:/ui/main/button_select");
      this.Focused = false;
      FadeWipe fadeWipe = new FadeWipe(this.Scene, false, (Action) (() =>
      {
        Engine.Scene = new Scene();
        Engine.Instance.Exit();
      }));
    }
  }
}

