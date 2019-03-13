// Decompiled with JetBrains decompiler
// Type: Celeste.MenuOptions
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using FMOD.Studio;
using Monocle;
using System;

namespace Celeste
{
  public static class MenuOptions
  {
    private static TextMenu menu;
    private static bool inGame;
    private static TextMenu.Item window;
    private static TextMenu.Item viewport;
    private static EventInstance snapshot;

    public static TextMenu Create(bool inGame = false, EventInstance snapshot = null)
    {
      MenuOptions.inGame = inGame;
      MenuOptions.snapshot = snapshot;
      MenuOptions.menu = new TextMenu();
      MenuOptions.menu.Add((TextMenu.Item) new TextMenu.Header(Dialog.Clean("options_title", (Language) null)));
      if (!inGame && Dialog.Languages.Count > 1)
      {
        MenuOptions.menu.Add((TextMenu.Item) new TextMenu.SubHeader(""));
        TextMenu.LanguageButton languageButton = new TextMenu.LanguageButton(Dialog.Clean("options_language", (Language) null), Dialog.Language);
        languageButton.Pressed(new Action(MenuOptions.SelectLanguage));
        MenuOptions.menu.Add((TextMenu.Item) languageButton);
      }
      MenuOptions.menu.Add((TextMenu.Item) new TextMenu.SubHeader(Dialog.Clean("options_controls", (Language) null)));
      MenuOptions.CreateRumble(MenuOptions.menu);
      MenuOptions.menu.Add(new TextMenu.Button(Dialog.Clean("options_keyconfig", (Language) null)).Pressed(new Action(MenuOptions.OpenKeyboardConfig)));
      MenuOptions.menu.Add(new TextMenu.Button(Dialog.Clean("options_btnconfig", (Language) null)).Pressed(new Action(MenuOptions.OpenButtonConfig)));
      MenuOptions.menu.Add((TextMenu.Item) new TextMenu.SubHeader(Dialog.Clean("options_video", (Language) null)));
      MenuOptions.menu.Add((TextMenu.Item) new TextMenu.OnOff(Dialog.Clean("options_fullscreen", (Language) null), Settings.Instance.Fullscreen).Change(new Action<bool>(MenuOptions.SetFullscreen)));
      TextMenu menu = MenuOptions.menu;
      string label = Dialog.Clean("options_window", (Language) null);
      int windowScale = Settings.Instance.WindowScale;
      TextMenu.Option<int> option;
      MenuOptions.window = (TextMenu.Item) (option = new TextMenu.Slider(label, (Func<int, string>) (i => i.ToString() + "x"), 3, 8, windowScale).Change(new Action<int>(MenuOptions.SetWindow)));
      menu.Add((TextMenu.Item) option);
      MenuOptions.menu.Add((TextMenu.Item) new TextMenu.OnOff(Dialog.Clean("options_vsync", (Language) null), Settings.Instance.VSync).Change(new Action<bool>(MenuOptions.SetVSync)));
      MenuOptions.menu.Add((TextMenu.Item) new TextMenu.OnOff(Dialog.Clean("OPTIONS_DISABLE_FLASH", (Language) null), Settings.Instance.DisableFlashes).Change((Action<bool>) (b => Settings.Instance.DisableFlashes = b)));
      MenuOptions.menu.Add((TextMenu.Item) new TextMenu.OnOff(Dialog.Clean("OPTIONS_DISABLE_SHAKE", (Language) null), !Settings.Instance.DisableScreenShake).Change((Action<bool>) (b => Settings.Instance.DisableScreenShake = !b)));
      MenuOptions.menu.Add(MenuOptions.viewport = new TextMenu.Button(Dialog.Clean("OPTIONS_VIEWPORT_PC", (Language) null)).Pressed(new Action(MenuOptions.OpenViewportAdjustment)));
      MenuOptions.menu.Add((TextMenu.Item) new TextMenu.SubHeader(Dialog.Clean("options_audio", (Language) null)));
      MenuOptions.menu.Add(new TextMenu.Slider(Dialog.Clean("options_music", (Language) null), (Func<int, string>) (i => i.ToString()), 0, 10, Settings.Instance.MusicVolume).Change(new Action<int>(MenuOptions.SetMusic)).Enter(new Action(MenuOptions.EnterSound)).Leave(new Action(MenuOptions.LeaveSound)));
      MenuOptions.menu.Add(new TextMenu.Slider(Dialog.Clean("options_sounds", (Language) null), (Func<int, string>) (i => i.ToString()), 0, 10, Settings.Instance.SFXVolume).Change(new Action<int>(MenuOptions.SetSfx)).Enter(new Action(MenuOptions.EnterSound)).Leave(new Action(MenuOptions.LeaveSound)));
      MenuOptions.menu.Add((TextMenu.Item) new TextMenu.SubHeader(Dialog.Clean("options_gameplay", (Language) null)));
      MenuOptions.menu.Add((TextMenu.Item) new TextMenu.Slider(Dialog.Clean("options_speedrun", (Language) null), (Func<int, string>) (i =>
      {
        if (i == 0)
          return Dialog.Get("OPTIONS_OFF", (Language) null);
        if (i == 1)
          return Dialog.Get("OPTIONS_SPEEDRUN_CHAPTER", (Language) null);
        return Dialog.Get("OPTIONS_SPEEDRUN_FILE", (Language) null);
      }), 0, 2, (int) Settings.Instance.SpeedrunClock).Change(new Action<int>(MenuOptions.SetSpeedrunClock)));
      MenuOptions.viewport.Visible = Settings.Instance.Fullscreen;
      if (MenuOptions.window != null)
        MenuOptions.window.Visible = !Settings.Instance.Fullscreen;
      if ((double) MenuOptions.menu.Height > (double) MenuOptions.menu.ScrollableMinSize)
        MenuOptions.menu.Position.Y = MenuOptions.menu.ScrollTargetY;
      return MenuOptions.menu;
    }

    private static void CreateRumble(TextMenu menu)
    {
      menu.Add((TextMenu.Item) new TextMenu.Slider(Dialog.Clean("options_rumble_PC", (Language) null), (Func<int, string>) (i =>
      {
        if (i == 2)
          return Dialog.Clean("OPTIONS_RUMBLE_ON", (Language) null);
        if (i == 1)
          return Dialog.Clean("OPTIONS_RUMBLE_HALF", (Language) null);
        return Dialog.Clean("OPTIONS_RUMBLE_OFF", (Language) null);
      }), 0, 2, (int) Settings.Instance.Rumble).Change((Action<int>) (i =>
      {
        Settings.Instance.Rumble = (RumbleAmount) i;
        Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
      })));
    }

    private static void SetFullscreen(bool on)
    {
      Settings.Instance.Fullscreen = on;
      Settings.Instance.ApplyScreen();
      if (MenuOptions.window != null)
        MenuOptions.window.Visible = !on;
      if (MenuOptions.viewport == null)
        return;
      MenuOptions.viewport.Visible = on;
    }

    private static void SetVSync(bool on)
    {
      Settings.Instance.VSync = on;
      Engine.Graphics.SynchronizeWithVerticalRetrace = Settings.Instance.VSync;
      Engine.Graphics.ApplyChanges();
    }

    private static void SetWindow(int scale)
    {
      Settings.Instance.WindowScale = scale;
      Settings.Instance.ApplyScreen();
    }

    private static void SetMusic(int volume)
    {
      Settings.Instance.MusicVolume = volume;
      Settings.Instance.ApplyMusicVolume();
    }

    private static void SetSfx(int volume)
    {
      Settings.Instance.SFXVolume = volume;
      Settings.Instance.ApplySFXVolume();
    }

    private static void SetSpeedrunClock(int val)
    {
      Settings.Instance.SpeedrunClock = (SpeedrunType) val;
    }

    private static void OpenViewportAdjustment()
    {
      if (Engine.Scene is Overworld)
        (Engine.Scene as Overworld).ShowInputUI = false;
      MenuOptions.menu.Visible = false;
      MenuOptions.menu.Focused = false;
      Engine.Scene.Add((Entity) new ViewportAdjustmentUI()
      {
        OnClose = (Action) (() =>
        {
          MenuOptions.menu.Visible = true;
          MenuOptions.menu.Focused = true;
          if (!(Engine.Scene is Overworld))
            return;
          (Engine.Scene as Overworld).ShowInputUI = true;
        })
      });
      Engine.Scene.OnEndOfFrame += (Action) (() => Engine.Scene.Entities.UpdateLists());
    }

    private static void SelectLanguage()
    {
      MenuOptions.menu.Focused = false;
      LanguageSelectUI languageSelectUi = new LanguageSelectUI();
      languageSelectUi.OnChange = (Action<string>) (id =>
      {
        Settings.Instance.Language = id;
        Settings.Instance.ApplyLanguage();
      });
      languageSelectUi.OnClose = (Action) (() => MenuOptions.menu.Focused = true);
      Engine.Scene.Add((Entity) languageSelectUi);
      Engine.Scene.OnEndOfFrame += (Action) (() => Engine.Scene.Entities.UpdateLists());
    }

    private static void OpenKeyboardConfig()
    {
      MenuOptions.menu.Focused = false;
      KeyboardConfigUI keyboardConfigUi = new KeyboardConfigUI();
      keyboardConfigUi.OnClose = (Action) (() => MenuOptions.menu.Focused = true);
      Engine.Scene.Add((Entity) keyboardConfigUi);
      Engine.Scene.OnEndOfFrame += (Action) (() => Engine.Scene.Entities.UpdateLists());
    }

    private static void OpenButtonConfig()
    {
      MenuOptions.menu.Focused = false;
      ButtonConfigUI buttonConfigUi = new ButtonConfigUI();
      buttonConfigUi.OnClose = (Action) (() => MenuOptions.menu.Focused = true);
      Engine.Scene.Add((Entity) buttonConfigUi);
      Engine.Scene.OnEndOfFrame += (Action) (() => Engine.Scene.Entities.UpdateLists());
    }

    private static void EnterSound()
    {
      if (!MenuOptions.inGame || !((HandleBase) MenuOptions.snapshot != (HandleBase) null))
        return;
      Audio.EndSnapshot(MenuOptions.snapshot);
    }

    private static void LeaveSound()
    {
      if (!MenuOptions.inGame || !((HandleBase) MenuOptions.snapshot != (HandleBase) null))
        return;
      Audio.ResumeSnapshot(MenuOptions.snapshot);
    }
  }
}

