// Decompiled with JetBrains decompiler
// Type: Celeste.LevelExit
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.IO;
using System.Xml;

namespace Celeste
{
  public class LevelExit : Scene
  {
    private LevelExit.Mode mode;
    private Session session;
    private float timer;
    private XmlElement completeXml;
    private Atlas completeAtlas;
    private bool completeLoaded;
    private HiresSnow snow;
    private OverworldLoader overworldLoader;
    public string GoldenStrawberryEntryLevel;
    private const float MinTimeForCompleteScreen = 3.3f;

    public LevelExit(LevelExit.Mode mode, Session session, HiresSnow snow = null)
    {
      this.Add((Monocle.Renderer) new HudRenderer());
      this.session = session;
      this.mode = mode;
      this.snow = snow;
    }

    public override void Begin()
    {
      base.Begin();
      if (this.mode != LevelExit.Mode.GoldenBerryRestart)
        SaveLoadIcon.Show((Scene) this);
      bool flag = this.snow == null;
      if (flag)
        this.snow = new HiresSnow(0.45f);
      if (this.mode == LevelExit.Mode.Completed)
      {
        this.snow.Direction = new Vector2(0.0f, 16f);
        if (flag)
          this.snow.Reset();
        RunThread.Start(new Action(this.LoadCompleteThread), "COMPLETE_LEVEL", false);
        if (this.session.Area.Mode != AreaMode.Normal)
          Audio.SetMusic("event:/music/menu/complete_bside", true, true);
        else if (this.session.Area.ID == 7)
          Audio.SetMusic("event:/music/menu/complete_summit", true, true);
        else
          Audio.SetMusic("event:/music/menu/complete_area", true, true);
        Audio.SetAmbience((string) null, true);
      }
      if (this.mode == LevelExit.Mode.GiveUp)
        this.overworldLoader = new OverworldLoader(Overworld.StartMode.AreaQuit, this.snow);
      else if (this.mode == LevelExit.Mode.SaveAndQuit)
        this.overworldLoader = new OverworldLoader(Overworld.StartMode.MainMenu, this.snow);
      else if (this.mode == LevelExit.Mode.CompletedInterlude)
        this.overworldLoader = new OverworldLoader(Overworld.StartMode.AreaComplete, this.snow);
      Entity entity;
      this.Add(entity = new Entity());
      entity.Add((Component) new Coroutine(this.Routine(), true));
      if (this.mode != LevelExit.Mode.Restart && this.mode != LevelExit.Mode.GoldenBerryRestart)
      {
        this.Add((Monocle.Renderer) this.snow);
        if (flag)
        {
          FadeWipe fadeWipe = new FadeWipe((Scene) this, true, (Action) null);
        }
      }
      Stats.Store();
      this.RendererList.UpdateLists();
    }

    private void LoadCompleteThread()
    {
      this.completeXml = AreaData.Get(this.session).CompleteScreenXml;
      if (this.completeXml != null && this.completeXml.HasAttr("atlas"))
        this.completeAtlas = Atlas.FromAtlas(Path.Combine("Graphics", "Atlases", this.completeXml.Attr("atlas")), Atlas.AtlasDataFormat.PackerNoAtlas);
      this.completeLoaded = true;
    }

    private IEnumerator Routine()
    {
      if (this.mode != LevelExit.Mode.GoldenBerryRestart)
      {
        UserIO.SaveHandler(true, true);
        while (UserIO.Saving)
          yield return (object) null;
        if (this.mode == LevelExit.Mode.Completed)
        {
          while (!this.completeLoaded)
            yield return (object) null;
        }
        while (SaveLoadIcon.OnScreen)
          yield return (object) null;
      }
      if (this.mode == LevelExit.Mode.Completed)
      {
        while ((double) this.timer < 3.29999995231628)
          yield return (object) null;
        Audio.SetMusicParam("end", 1f);
        Engine.Scene = (Scene) new AreaComplete(this.session, this.completeXml, this.completeAtlas, this.snow);
      }
      else if (this.mode == LevelExit.Mode.GiveUp || this.mode == LevelExit.Mode.SaveAndQuit || this.mode == LevelExit.Mode.CompletedInterlude)
        Engine.Scene = (Scene) this.overworldLoader;
      else if (this.mode == LevelExit.Mode.Restart || this.mode == LevelExit.Mode.GoldenBerryRestart)
      {
        Session session;
        if (this.mode == LevelExit.Mode.GoldenBerryRestart)
        {
          if ((this.session.Audio.Music.Event == "event:/music/lvl7/main" || this.session.Audio.Music.Event == "event:/music/lvl7/final_ascent") && this.session.Audio.Music.Progress > 0)
            Audio.SetMusic((string) null, true, true);
          session = this.session.Restart(this.GoldenStrawberryEntryLevel);
        }
        else
          session = this.session.Restart((string) null);
        LevelLoader levelLoader = new LevelLoader(session, new Vector2?());
        if (this.mode == LevelExit.Mode.GoldenBerryRestart)
          levelLoader.PlayerIntroTypeOverride = new Player.IntroTypes?(Player.IntroTypes.Respawn);
        Engine.Scene = (Scene) levelLoader;
      }
    }

    public override void Update()
    {
      this.timer += Engine.DeltaTime;
      base.Update();
    }

    public enum Mode
    {
      SaveAndQuit,
      GiveUp,
      Restart,
      GoldenBerryRestart,
      Completed,
      CompletedInterlude,
    }
  }
}
