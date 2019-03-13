// Decompiled with JetBrains decompiler
// Type: Celeste.UnlockEverythingThingy
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  public class UnlockEverythingThingy : CheatListener
  {
    public UnlockEverythingThingy()
    {
      this.AddInput('u', (Func<bool>) (() => Input.MenuUp.Pressed && !Input.MenuUp.Repeating));
      this.AddInput('d', (Func<bool>) (() => Input.MenuDown.Pressed && !Input.MenuDown.Repeating));
      this.AddInput('r', (Func<bool>) (() => Input.MenuRight.Pressed && !Input.MenuRight.Repeating));
      this.AddInput('l', (Func<bool>) (() => Input.MenuLeft.Pressed && !Input.MenuLeft.Repeating));
      this.AddInput('A', (Func<bool>) (() => Input.MenuConfirm.Pressed));
      this.AddInput('L', (Func<bool>) (() => Input.MenuJournal.Pressed));
      this.AddInput('R', (Func<bool>) (() => Input.Grab.Pressed && !Input.MenuJournal.Pressed));
      this.AddCheat("lrLRuudlRA", new Action(this.EnteredCheat));
      this.Logging = true;
    }

    public void EnteredCheat()
    {
      Level level = this.SceneAs<Level>();
      level.PauseLock = true;
      level.Frozen = true;
      level.Flash(Color.White, false);
      Audio.Play("event:/game/06_reflection/feather_bubble_get", (this.Scene as Level).Camera.Position + new Vector2(160f, 90f));
      new FadeWipe(this.Scene, false, (Action) (() => this.UnlockEverything(level))).Duration = 2f;
      this.RemoveSelf();
    }

    public void UnlockEverything(Level level)
    {
      SaveData.Instance.UnlockedAreas = SaveData.Instance.MaxArea;
      SaveData.Instance.CheatMode = true;
      Settings.Instance.Pico8OnMainMenu = true;
      Settings.Instance.VariantsUnlocked = true;
      level.Session.InArea = false;
      Engine.Scene = (Scene) new LevelExit(LevelExit.Mode.GiveUp, level.Session, (HiresSnow) null);
    }
  }
}

