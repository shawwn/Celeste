// Decompiled with JetBrains decompiler
// Type: Celeste.OverworldLoader
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class OverworldLoader : Scene
  {
    public Overworld.StartMode StartMode;
    public HiresSnow Snow;
    private bool loaded;
    private bool fadeIn;
    private Overworld overworld;
    private Postcard postcard;
    private bool showVariantPostcard;
    private bool showUnlockCSidePostcard;

    public OverworldLoader(Overworld.StartMode startMode, HiresSnow snow = null)
    {
      this.StartMode = startMode;
      this.Snow = snow == null ? new HiresSnow(0.45f) : snow;
      this.fadeIn = snow == null;
    }

    public override void Begin()
    {
      this.Add((Monocle.Renderer) new HudRenderer());
      this.Add((Monocle.Renderer) this.Snow);
      if (this.fadeIn)
      {
        FadeWipe fadeWipe = new FadeWipe((Scene) this, true, (Action) null);
      }
      this.RendererList.UpdateLists();
      Session session = (Session) null;
      if (SaveData.Instance != null)
        session = SaveData.Instance.CurrentSession;
      this.Add(new Entity()
      {
        (Component) new Coroutine(this.Routine(session), true)
      });
      RunThread.Start(new Action(this.LoadThread), "OVERWORLD_LOADER", false);
    }

    private void LoadThread()
    {
      this.CheckVariantsPostcardAtLaunch();
      this.overworld = new Overworld(this);
      this.overworld.Entities.UpdateLists();
      this.loaded = true;
    }

    private IEnumerator Routine(Session session)
    {
      OverworldLoader overworldLoader = this;
      if ((overworldLoader.StartMode == Overworld.StartMode.AreaComplete || overworldLoader.StartMode == Overworld.StartMode.AreaQuit) && session != null)
      {
        if (session.UnlockedCSide)
          overworldLoader.showUnlockCSidePostcard = true;
        if (!Settings.Instance.VariantsUnlocked && SaveData.Instance != null && SaveData.Instance.TotalHeartGems >= 24)
          overworldLoader.showVariantPostcard = true;
      }
      if (overworldLoader.showUnlockCSidePostcard)
      {
        yield return (object) 3f;
        overworldLoader.Add((Entity) (overworldLoader.postcard = new Postcard(Dialog.Get("POSTCARD_CSIDES", (Language) null), -1)));
        yield return (object) overworldLoader.postcard.DisplayRoutine();
      }
      while (!overworldLoader.loaded)
        yield return (object) null;
      if (overworldLoader.showVariantPostcard)
        Settings.Instance.VariantsUnlocked = true;
      Engine.Scene = (Scene) overworldLoader.overworld;
    }

    public override void BeforeRender()
    {
      base.BeforeRender();
      if (this.postcard == null)
        return;
      this.postcard.BeforeRender();
    }

    private void CheckVariantsPostcardAtLaunch()
    {
      if (this.StartMode != Overworld.StartMode.Titlescreen || Settings.Instance.VariantsUnlocked || Settings.LastVersion != null && !(new Version(Settings.LastVersion) <= new Version(1, 2, 4, 2)) || !UserIO.Open(UserIO.Mode.Read))
        return;
      for (int slot = 0; slot < 3; ++slot)
      {
        if (UserIO.Exists(SaveData.GetFilename(slot)))
        {
          SaveData saveData = UserIO.Load<SaveData>(SaveData.GetFilename(slot), false);
          if (saveData != null)
          {
            saveData.AfterInitialize();
            if (saveData.TotalHeartGems >= 24)
            {
              this.showVariantPostcard = true;
              break;
            }
          }
        }
      }
      UserIO.Close();
      SaveData.Instance = (SaveData) null;
    }
  }
}
