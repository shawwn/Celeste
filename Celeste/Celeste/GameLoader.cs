// Decompiled with JetBrains decompiler
// Type: Celeste.GameLoader
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Celeste
{
  public class GameLoader : Scene
  {
    public static Stopwatch Stopwatch = new Stopwatch();
    public HiresSnow Snow;
    private bool loaded;
    private bool audioLoaded;
    private bool audioStarted;
    private bool dialogLoaded;
    private Entity handler;
    private Thread activeThread;
    private bool skipped;
    private bool ready;
    private List<MTexture> loadingTextures;
    private float loadingFrame;
    private float loadingAlpha;

    public GameLoader()
    {
      this.Snow = new HiresSnow(0.45f);
    }

    public override void Begin()
    {
      this.Add((Monocle.Renderer) new HudRenderer());
      this.Add((Monocle.Renderer) this.Snow);
      FadeWipe fadeWipe = new FadeWipe((Scene) this, true, (Action) null);
      this.RendererList.UpdateLists();
      this.Add(this.handler = new Entity());
      this.handler.Tag = (int) Tags.HUD;
      this.handler.Add((Component) new Coroutine(this.IntroRoutine(), true));
      this.activeThread = Thread.CurrentThread;
      RunThread.Start(new Action(this.LoadThread), "GAME_LOADER", true);
    }

    private void LoadThread()
    {
      Console.WriteLine("GAME DISPLAYED : " + (object) GameLoader.Stopwatch.ElapsedMilliseconds + "ms");
      MInput.Disabled = true;
      Audio.Init();
      Audio.Banks.Master = Audio.Banks.Load("Master Bank", true);
      Audio.Banks.Music = Audio.Banks.Load("music", false);
      Audio.Banks.Sfxs = Audio.Banks.Load("sfx", false);
      Audio.Banks.UI = Audio.Banks.Load("ui", false);
      Audio.Banks.NewContent = Audio.Banks.Load("new_content", false);
      Settings.Instance.ApplyVolumes();
      this.audioLoaded = true;
      Fonts.Load();
      Dialog.Load();
      this.dialogLoaded = true;
      MInput.Disabled = false;
      if (!GFX.LoadedMainContent)
      {
        GFX.LoadGame();
        GFX.LoadOther();
        GFX.LoadMountain();
        GFX.LoadPortraits();
      }
      GFX.LoadData();
      AreaData.Load();
      GFX.MountainTerrain = ObjModel.Create(Path.Combine(Engine.ContentDirectory, "Overworld", "mountain.obj"));
      GFX.MountainBuildings = ObjModel.Create(Path.Combine(Engine.ContentDirectory, "Overworld", "buildings.obj"));
      GFX.MountainCoreWall = ObjModel.Create(Path.Combine(Engine.ContentDirectory, "Overworld", "mountain_wall.obj"));
      Console.WriteLine("LOADED : " + (object) GameLoader.Stopwatch.ElapsedMilliseconds + "ms");
      GameLoader.Stopwatch.Stop();
      GameLoader.Stopwatch = (Stopwatch) null;
      this.loaded = true;
    }

    public IEnumerator IntroRoutine()
    {
      if (Celeste.PlayMode != Celeste.PlayModes.Debug)
      {
        for (float p = 0.0f; (double) p > 1.0 && !this.skipped; p += Engine.DeltaTime)
          yield return (object) null;
        if (!this.skipped)
        {
          Monocle.Image img = new Monocle.Image(GFX.Opening["presentedby"]);
          yield return (object) this.FadeInOut(img);
          img = (Monocle.Image) null;
        }
        if (!this.skipped)
        {
          Monocle.Image img = new Monocle.Image(GFX.Opening["gameby"]);
          yield return (object) this.FadeInOut(img);
          img = (Monocle.Image) null;
        }
        if (!this.skipped)
        {
          while (!this.dialogLoaded)
            yield return (object) null;
          AutoSavingNotice notice = new AutoSavingNotice();
          this.Add((Monocle.Renderer) notice);
          for (float p = 0.0f; (double) p < 1.0 && !this.skipped; p += Engine.DeltaTime)
            yield return (object) null;
          notice.Display = false;
          while (notice.StillVisible)
          {
            notice.ForceClose = this.skipped;
            yield return (object) null;
          }
          this.Remove((Monocle.Renderer) notice);
          notice = (AutoSavingNotice) null;
        }
      }
      this.ready = true;
      if (!this.loaded)
      {
        this.loadingTextures = GFX.Overworld.GetAtlasSubtextures("loading/");
        Monocle.Image img = new Monocle.Image(this.loadingTextures[0]);
        img.CenterOrigin();
        img.Scale = Vector2.One * 0.5f;
        this.handler.Add((Component) img);
        while (!this.loaded || (double) this.loadingAlpha > 0.0)
        {
          this.loadingFrame += Engine.DeltaTime * 10f;
          this.loadingAlpha = Calc.Approach(this.loadingAlpha, this.loaded ? 0.0f : 1f, Engine.DeltaTime * 4f);
          img.Texture = this.loadingTextures[(int) ((double) this.loadingFrame % (double) this.loadingTextures.Count)];
          img.Color = Color.White * Ease.CubeOut(this.loadingAlpha);
          img.Position = new Vector2(1792f, (float) (1080.0 - 128.0 * (double) Ease.CubeOut(this.loadingAlpha)));
          yield return (object) null;
        }
        img = (Monocle.Image) null;
      }
      MInput.Disabled = false;
      Engine.Scene = (Scene) new OverworldLoader(Overworld.StartMode.Titlescreen, this.Snow);
      GFX.Opening.Dispose();
      GFX.Opening = (Atlas) null;
    }

    private IEnumerator FadeInOut(Monocle.Image img)
    {
      float alpha = 0.0f;
      img.Color = Color.White * 0.0f;
      this.handler.Add((Component) img);
      for (float i = 0.0f; (double) i < 4.5 && !this.skipped; i += Engine.DeltaTime)
      {
        alpha = Ease.CubeOut(Math.Min(i, 1f));
        img.Color = Color.White * alpha;
        yield return (object) null;
      }
      while ((double) alpha > 0.0)
      {
        alpha -= Engine.DeltaTime * (this.skipped ? 8f : 1f);
        img.Color = Color.White * alpha;
        yield return (object) null;
      }
    }

    public override void Update()
    {
      if (this.audioLoaded && !this.audioStarted)
      {
        Audio.SetAmbience("event:/env/amb/worldmap", true);
        this.audioStarted = true;
      }
      if (!this.ready)
      {
        bool disabled = MInput.Disabled;
        MInput.Disabled = false;
        if (Input.MenuConfirm.Pressed)
          this.skipped = true;
        MInput.Disabled = disabled;
      }
      base.Update();
    }
  }
}

