// Decompiled with JetBrains decompiler
// Type: Celeste.FileErrorOverlay
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class FileErrorOverlay : Overlay
  {
    private FileErrorOverlay.Error mode;
    private TextMenu menu;

    public bool Open { get; private set; }

    public bool TryAgain { get; private set; }

    public bool Ignore { get; private set; }

    public FileErrorOverlay(FileErrorOverlay.Error mode)
    {
      this.Open = true;
      this.mode = mode;
      this.Add((Component) new Coroutine(this.Routine(), true));
      Engine.Scene.Add((Entity) this);
    }

    private IEnumerator Routine()
    {
      FileErrorOverlay fileErrorOverlay = this;
      yield return (object) fileErrorOverlay.FadeIn();
      bool waiting = true;
      int option = 0;
      Audio.Play("event:/ui/main/message_confirm");
      fileErrorOverlay.menu = new TextMenu();
      fileErrorOverlay.menu.Add((TextMenu.Item) new TextMenu.Header(Dialog.Clean("savefailed_title", (Language) null)));
      fileErrorOverlay.menu.Add(new TextMenu.Button(Dialog.Clean(fileErrorOverlay.mode == FileErrorOverlay.Error.Save ? "savefailed_retry" : "loadfailed_goback", (Language) null)).Pressed((Action) (() =>
      {
        option = 0;
        waiting = false;
      })));
      fileErrorOverlay.menu.Add(new TextMenu.Button(Dialog.Clean("savefailed_ignore", (Language) null)).Pressed((Action) (() =>
      {
        option = 1;
        waiting = false;
      })));
      while (waiting)
        yield return (object) null;
      fileErrorOverlay.menu = (TextMenu) null;
      fileErrorOverlay.Ignore = option == 1;
      fileErrorOverlay.TryAgain = option == 0;
      yield return (object) fileErrorOverlay.FadeOut();
      fileErrorOverlay.Open = false;
      fileErrorOverlay.RemoveSelf();
    }

    public override void Update()
    {
      base.Update();
      if (this.menu != null)
        this.menu.Update();
      if (SaveLoadIcon.Instance == null || SaveLoadIcon.Instance.Scene != this.Scene)
        return;
      SaveLoadIcon.Instance.Update();
    }

    public override void Render()
    {
      this.RenderFade();
      if (this.menu != null)
        this.menu.Render();
      base.Render();
    }

    public enum Error
    {
      Load,
      Save,
    }
  }
}
