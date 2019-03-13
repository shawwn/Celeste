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
      GameLoader gameLoader = this;
      if (Celeste.Celeste.PlayMode != Celeste.Celeste.PlayModes.Debug)
      {
        float p;
        for (p = 0.0f; (double) p > 1.0 && !gameLoader.skipped; p += Engine.DeltaTime)
          yield return (object) null;
        if (!gameLoader.skipped)
        {
          Monocle.Image img = new Monocle.Image(GFX.Opening["presentedby"]);
          yield return (object) gameLoader.FadeInOut(img);
        }
        if (!gameLoader.skipped)
        {
          Monocle.Image img = new Monocle.Image(GFX.Opening["gameby"]);
          yield return (object) gameLoader.FadeInOut(img);
        }
        if (!gameLoader.skipped)
        {
          while (!gameLoader.dialogLoaded)
            yield return (object) null;
          AutoSavingNotice notice = new AutoSavingNotice();
          gameLoader.Add((Monocle.Renderer) notice);
          for (p = 0.0f; (double) p < 1.0 && !gameLoader.skipped; p += Engine.DeltaTime)
            yield return (object) null;
          notice.Display = false;
          while (notice.StillVisible)
          {
            notice.ForceClose = gameLoader.skipped;
            yield return (object) null;
          }
          gameLoader.Remove((Monocle.Renderer) notice);
          notice = (AutoSavingNotice) null;
        }
      }
      gameLoader.ready = true;
      if (!gameLoader.loaded)
      {
        gameLoader.loadingTextures = GFX.Overworld.GetAtlasSubtextures("loading/");
        Monocle.Image img = new Monocle.Image(gameLoader.loadingTextures[0]);
        img.CenterOrigin();
        img.Scale = Vector2.op_Multiply(Vector2.get_One(), 0.5f);
        gameLoader.handler.Add((Component) img);
        while (!gameLoader.loaded || (double) gameLoader.loadingAlpha > 0.0)
        {
          gameLoader.loadingFrame += Engine.DeltaTime * 10f;
          gameLoader.loadingAlpha = Calc.Approach(gameLoader.loadingAlpha, gameLoader.loaded ? 0.0f : 1f, Engine.DeltaTime * 4f);
          img.Texture = gameLoader.loadingTextures[(int) ((double) gameLoader.loadingFrame % (double) gameLoader.loadingTextures.Count)];
          img.Color = Color.op_Multiply(Color.get_White(), Ease.CubeOut(gameLoader.loadingAlpha));
          img.Position = new Vector2(1792f, (float) (1080.0 - 128.0 * (double) Ease.CubeOut(gameLoader.loadingAlpha)));
          yield return (object) null;
        }
        img = (Monocle.Image) null;
      }
      MInput.Disabled = false;
      Engine.Scene = (Scene) new OverworldLoader(Overworld.StartMode.Titlescreen, gameLoader.Snow);
      GFX.Opening.Dispose();
      GFX.Opening = (Atlas) null;
    }

    private IEnumerator FadeInOut(Monocle.Image img)
    {
      float alpha = 0.0f;
      img.Color = Color.op_Multiply(Color.get_White(), 0.0f);
      this.handler.Add((Component) img);
      for (float i = 0.0f; (double) i < 4.5 && !this.skipped; i += Engine.DeltaTime)
      {
        alpha = Ease.CubeOut(Math.Min(i, 1f));
        img.Color = Color.op_Multiply(Color.get_White(), alpha);
        yield return (object) null;
      }
      while ((double) alpha > 0.0)
      {
        alpha -= Engine.DeltaTime * (this.skipped ? 8f : 1f);
        img.Color = Color.op_Multiply(Color.get_White(), alpha);
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
        int num = MInput.Disabled ? 1 : 0;
        MInput.Disabled = false;
        if (Input.MenuConfirm.Pressed)
          this.skipped = true;
        MInput.Disabled = num != 0;
      }
      base.Update();
    }
  }
}
