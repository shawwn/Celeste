// Decompiled with JetBrains decompiler
// Type: Celeste.GameLoader
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

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
    public HiresSnow Snow;
    private Atlas opening;
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
      Console.WriteLine("GAME DISPLAYED (in " + (object) Celeste.LoadTimer.ElapsedMilliseconds + "ms)");
      this.Snow = new HiresSnow();
      this.opening = Atlas.FromAtlas(Path.Combine("Graphics", "Atlases", "Opening"), Atlas.AtlasDataFormat.PackerNoAtlas);
    }

    public override void Begin()
    {
      this.Add((Monocle.Renderer) new HudRenderer());
      this.Add((Monocle.Renderer) this.Snow);
      FadeWipe fadeWipe = new FadeWipe((Scene) this, true);
      this.RendererList.UpdateLists();
      this.Add(this.handler = new Entity());
      this.handler.Tag = (int) Tags.HUD;
      this.handler.Add((Component) new Coroutine(this.IntroRoutine()));
      this.activeThread = Thread.CurrentThread;
      this.activeThread.Priority = ThreadPriority.Lowest;
      RunThread.Start(new Action(this.LoadThread), "GAME_LOADER", true);
    }

    private void LoadThread()
    {
      MInput.Disabled = true;
      Stopwatch stopwatch1 = Stopwatch.StartNew();
      Audio.Init();
      Audio.Banks.Master = Audio.Banks.Load("Master Bank", true);
      Audio.Banks.Music = Audio.Banks.Load("music", false);
      Audio.Banks.Sfxs = Audio.Banks.Load("sfx", false);
      Audio.Banks.UI = Audio.Banks.Load("ui", false);
      Audio.Banks.DlcMusic = Audio.Banks.Load("dlc_music", false);
      Audio.Banks.DlcSfxs = Audio.Banks.Load("dlc_sfx", false);
      Settings.Instance.ApplyVolumes();
      this.audioLoaded = true;
      Console.WriteLine(" - AUDIO LOAD: " + (object) stopwatch1.ElapsedMilliseconds + "ms");
      GFX.Load();
      MTN.Load();
      GFX.LoadData();
      MTN.LoadData();
      Stopwatch stopwatch2 = Stopwatch.StartNew();
      Fonts.Prepare();
      Dialog.Load();
      Fonts.Load(Dialog.Languages["english"].FontFace);
      Fonts.Load(Dialog.Languages[Settings.Instance.Language].FontFace);
      this.dialogLoaded = true;
      Console.WriteLine(" - DIA/FONT LOAD: " + (object) stopwatch2.ElapsedMilliseconds + "ms");
      MInput.Disabled = false;
      Stopwatch stopwatch3 = Stopwatch.StartNew();
      AreaData.Load();
      Console.WriteLine(" - LEVELS LOAD: " + (object) stopwatch3.ElapsedMilliseconds + "ms");
      Console.WriteLine("DONE LOADING (in " + (object) Celeste.LoadTimer.ElapsedMilliseconds + "ms)");
      Celeste.LoadTimer.Stop();
      Celeste.LoadTimer = (Stopwatch) null;
      this.loaded = true;
    }

    public IEnumerator IntroRoutine()
    {
      GameLoader gameLoader = this;
      if (Celeste.PlayMode != Celeste.PlayModes.Debug)
      {
        float p;
        for (p = 0.0f; (double) p > 1.0 && !gameLoader.skipped; p += Engine.DeltaTime)
          yield return (object) null;
        if (!gameLoader.skipped)
        {
          Monocle.Image img = new Monocle.Image(gameLoader.opening["presentedby"]);
          yield return (object) gameLoader.FadeInOut(img);
        }
        if (!gameLoader.skipped)
        {
          Monocle.Image img = new Monocle.Image(gameLoader.opening["gameby"]);
          yield return (object) gameLoader.FadeInOut(img);
        }
        bool flag = !Celeste.IsGGP;
        if (!gameLoader.skipped & flag)
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
        gameLoader.loadingTextures = OVR.Atlas.GetAtlasSubtextures("loading/");
        Monocle.Image img = new Monocle.Image(gameLoader.loadingTextures[0]);
        img.CenterOrigin();
        img.Scale = Vector2.One * 0.5f;
        gameLoader.handler.Add((Component) img);
        while (!gameLoader.loaded || (double) gameLoader.loadingAlpha > 0.0)
        {
          gameLoader.loadingFrame += Engine.DeltaTime * 10f;
          gameLoader.loadingAlpha = Calc.Approach(gameLoader.loadingAlpha, gameLoader.loaded ? 0.0f : 1f, Engine.DeltaTime * 4f);
          img.Texture = gameLoader.loadingTextures[(int) ((double) gameLoader.loadingFrame % (double) gameLoader.loadingTextures.Count)];
          img.Color = Color.White * Ease.CubeOut(gameLoader.loadingAlpha);
          img.Position = new Vector2(1792f, (float) (1080.0 - 128.0 * (double) Ease.CubeOut(gameLoader.loadingAlpha)));
          yield return (object) null;
        }
        img = (Monocle.Image) null;
      }
      gameLoader.opening.Dispose();
      gameLoader.activeThread.Priority = ThreadPriority.Normal;
      MInput.Disabled = false;
      Engine.Scene = (Scene) new OverworldLoader(Overworld.StartMode.Titlescreen, gameLoader.Snow);
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
        Audio.SetAmbience("event:/env/amb/worldmap");
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
