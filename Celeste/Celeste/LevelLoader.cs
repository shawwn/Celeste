// Decompiled with JetBrains decompiler
// Type: Celeste.LevelLoader
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Celeste
{
  public class LevelLoader : Scene
  {
    private Session session;
    private Vector2? startPosition;
    private bool started;
    public Player.IntroTypes? PlayerIntroTypeOverride;

    public Level Level { get; private set; }

    public bool Loaded { get; private set; }

    public LevelLoader(Session session, Vector2? startPosition = null)
    {
      this.session = session;
      this.startPosition = startPosition.HasValue ? startPosition : session.RespawnPoint;
      this.Level = new Level();
      RunThread.Start(new Action(this.LoadingThread), "LEVEL_LOADER");
    }

    private void LoadingThread()
    {
      MapData mapData = this.session.MapData;
      AreaData areaData = AreaData.Get(this.session);
      if (this.session.Area.ID == 0)
        SaveData.Instance.Assists.DashMode = Assists.DashModes.Normal;
      this.Level.Add((Monocle.Renderer) (this.Level.GameplayRenderer = new GameplayRenderer()));
      this.Level.Add((Monocle.Renderer) (this.Level.Lighting = new LightingRenderer()));
      this.Level.Add((Monocle.Renderer) (this.Level.Bloom = new BloomRenderer()));
      this.Level.Add((Monocle.Renderer) (this.Level.Displacement = new DisplacementRenderer()));
      this.Level.Add((Monocle.Renderer) (this.Level.Background = new BackdropRenderer()));
      this.Level.Add((Monocle.Renderer) (this.Level.Foreground = new BackdropRenderer()));
      this.Level.Add((Entity) new DustEdges());
      this.Level.Add((Entity) new WaterSurface());
      this.Level.Add((Entity) new MirrorSurfaces());
      this.Level.Add((Entity) new GlassBlockBg());
      this.Level.Add((Entity) new LightningRenderer());
      this.Level.Add((Entity) new SeekerBarrierRenderer());
      this.Level.Add((Monocle.Renderer) (this.Level.HudRenderer = new HudRenderer()));
      if (this.session.Area.ID == 9)
        this.Level.Add((Entity) new IceTileOverlay());
      this.Level.BaseLightingAlpha = this.Level.Lighting.Alpha = areaData.DarknessAlpha;
      this.Level.Bloom.Base = areaData.BloomBase;
      this.Level.Bloom.Strength = areaData.BloomStrength;
      this.Level.BackgroundColor = mapData.BackgroundColor;
      this.Level.Background.Backdrops = mapData.CreateBackdrops(mapData.Background);
      foreach (Backdrop backdrop in this.Level.Background.Backdrops)
        backdrop.Renderer = this.Level.Background;
      this.Level.Foreground.Backdrops = mapData.CreateBackdrops(mapData.Foreground);
      foreach (Backdrop backdrop in this.Level.Foreground.Backdrops)
        backdrop.Renderer = this.Level.Foreground;
      this.Level.RendererList.UpdateLists();
      this.Level.Add((Entity) (this.Level.FormationBackdrop = new FormationBackdrop()));
      this.Level.Camera = this.Level.GameplayRenderer.Camera;
      Audio.SetCamera(this.Level.Camera);
      this.Level.Session = this.session;
      SaveData.Instance.StartSession(this.Level.Session);
      this.Level.Particles = new ParticleSystem(-8000, 400);
      this.Level.Particles.Tag = (int) Tags.Global;
      this.Level.Add((Entity) this.Level.Particles);
      this.Level.ParticlesBG = new ParticleSystem(8000, 400);
      this.Level.ParticlesBG.Tag = (int) Tags.Global;
      this.Level.Add((Entity) this.Level.ParticlesBG);
      this.Level.ParticlesFG = new ParticleSystem(-50000, 800);
      this.Level.ParticlesFG.Tag = (int) Tags.Global;
      this.Level.ParticlesFG.Add((Component) new MirrorReflection());
      this.Level.Add((Entity) this.Level.ParticlesFG);
      this.Level.Add((Entity) (this.Level.strawberriesDisplay = new TotalStrawberriesDisplay()));
      this.Level.Add((Entity) new SpeedrunTimerDisplay());
      this.Level.Add((Entity) new GameplayStats());
      this.Level.Add((Entity) new GrabbyIcon());
      Rectangle tileBounds1 = mapData.TileBounds;
      GFX.FGAutotiler.LevelBounds.Clear();
      VirtualMap<char> data1 = new VirtualMap<char>(tileBounds1.Width, tileBounds1.Height, '0');
      VirtualMap<char> data2 = new VirtualMap<char>(tileBounds1.Width, tileBounds1.Height, '0');
      VirtualMap<bool> virtualMap = new VirtualMap<bool>(tileBounds1.Width, tileBounds1.Height);
      Regex regex = new Regex("\\r\\n|\\n\\r|\\n|\\r");
      foreach (LevelData level in mapData.Levels)
      {
        Rectangle tileBounds2 = level.TileBounds;
        int left1 = tileBounds2.Left;
        tileBounds2 = level.TileBounds;
        int top1 = tileBounds2.Top;
        string[] strArray1 = regex.Split(level.Bg);
        for (int index1 = top1; index1 < top1 + strArray1.Length; ++index1)
        {
          for (int index2 = left1; index2 < left1 + strArray1[index1 - top1].Length; ++index2)
            data1[index2 - tileBounds1.X, index1 - tileBounds1.Y] = strArray1[index1 - top1][index2 - left1];
        }
        string[] strArray2 = regex.Split(level.Solids);
        for (int index3 = top1; index3 < top1 + strArray2.Length; ++index3)
        {
          for (int index4 = left1; index4 < left1 + strArray2[index3 - top1].Length; ++index4)
            data2[index4 - tileBounds1.X, index3 - tileBounds1.Y] = strArray2[index3 - top1][index4 - left1];
        }
        tileBounds2 = level.TileBounds;
        int left2 = tileBounds2.Left;
        while (true)
        {
          int num1 = left2;
          tileBounds2 = level.TileBounds;
          int right = tileBounds2.Right;
          if (num1 < right)
          {
            tileBounds2 = level.TileBounds;
            int top2 = tileBounds2.Top;
            while (true)
            {
              int num2 = top2;
              tileBounds2 = level.TileBounds;
              int bottom = tileBounds2.Bottom;
              if (num2 < bottom)
              {
                virtualMap[left2 - tileBounds1.Left, top2 - tileBounds1.Top] = true;
                ++top2;
              }
              else
                break;
            }
            ++left2;
          }
          else
            break;
        }
        GFX.FGAutotiler.LevelBounds.Add(new Rectangle(level.TileBounds.X - tileBounds1.X, level.TileBounds.Y - tileBounds1.Y, level.TileBounds.Width, level.TileBounds.Height));
      }
      foreach (Rectangle rectangle in mapData.Filler)
      {
        for (int left = rectangle.Left; left < rectangle.Right; ++left)
        {
          for (int top = rectangle.Top; top < rectangle.Bottom; ++top)
          {
            char ch1 = '0';
            if (rectangle.Top - tileBounds1.Y > 0)
            {
              char ch2 = data2[left - tileBounds1.X, rectangle.Top - tileBounds1.Y - 1];
              if (ch2 != '0')
                ch1 = ch2;
            }
            if (ch1 == '0' && rectangle.Left - tileBounds1.X > 0)
            {
              char ch3 = data2[rectangle.Left - tileBounds1.X - 1, top - tileBounds1.Y];
              if (ch3 != '0')
                ch1 = ch3;
            }
            if (ch1 == '0' && rectangle.Right - tileBounds1.X < tileBounds1.Width - 1)
            {
              char ch4 = data2[rectangle.Right - tileBounds1.X, top - tileBounds1.Y];
              if (ch4 != '0')
                ch1 = ch4;
            }
            if (ch1 == '0' && rectangle.Bottom - tileBounds1.Y < tileBounds1.Height - 1)
            {
              char ch5 = data2[left - tileBounds1.X, rectangle.Bottom - tileBounds1.Y];
              if (ch5 != '0')
                ch1 = ch5;
            }
            if (ch1 == '0')
              ch1 = '1';
            data2[left - tileBounds1.X, top - tileBounds1.Y] = ch1;
            virtualMap[left - tileBounds1.X, top - tileBounds1.Y] = true;
          }
        }
      }
      using (List<LevelData>.Enumerator enumerator = mapData.Levels.GetEnumerator())
      {
label_81:
        while (enumerator.MoveNext())
        {
          LevelData current = enumerator.Current;
          Rectangle tileBounds3 = current.TileBounds;
          int left3 = tileBounds3.Left;
          while (true)
          {
            int num3 = left3;
            tileBounds3 = current.TileBounds;
            int right = tileBounds3.Right;
            if (num3 < right)
            {
              tileBounds3 = current.TileBounds;
              int top = tileBounds3.Top;
              char ch6 = data1[left3 - tileBounds1.X, top - tileBounds1.Y];
              for (int index = 1; index < 4 && !virtualMap[left3 - tileBounds1.X, top - tileBounds1.Y - index]; ++index)
                data1[left3 - tileBounds1.X, top - tileBounds1.Y - index] = ch6;
              tileBounds3 = current.TileBounds;
              int num4 = tileBounds3.Bottom - 1;
              char ch7 = data1[left3 - tileBounds1.X, num4 - tileBounds1.Y];
              for (int index = 1; index < 4 && !virtualMap[left3 - tileBounds1.X, num4 - tileBounds1.Y + index]; ++index)
                data1[left3 - tileBounds1.X, num4 - tileBounds1.Y + index] = ch7;
              ++left3;
            }
            else
              break;
          }
          tileBounds3 = current.TileBounds;
          int num5 = tileBounds3.Top - 4;
          while (true)
          {
            int num6 = num5;
            tileBounds3 = current.TileBounds;
            int num7 = tileBounds3.Bottom + 4;
            if (num6 < num7)
            {
              tileBounds3 = current.TileBounds;
              int left4 = tileBounds3.Left;
              char ch8 = data1[left4 - tileBounds1.X, num5 - tileBounds1.Y];
              for (int index = 1; index < 4 && !virtualMap[left4 - tileBounds1.X - index, num5 - tileBounds1.Y]; ++index)
                data1[left4 - tileBounds1.X - index, num5 - tileBounds1.Y] = ch8;
              tileBounds3 = current.TileBounds;
              int num8 = tileBounds3.Right - 1;
              char ch9 = data1[num8 - tileBounds1.X, num5 - tileBounds1.Y];
              for (int index = 1; index < 4 && !virtualMap[num8 - tileBounds1.X + index, num5 - tileBounds1.Y]; ++index)
                data1[num8 - tileBounds1.X + index, num5 - tileBounds1.Y] = ch9;
              ++num5;
            }
            else
              goto label_81;
          }
        }
      }
      using (List<LevelData>.Enumerator enumerator = mapData.Levels.GetEnumerator())
      {
label_96:
        while (enumerator.MoveNext())
        {
          LevelData current = enumerator.Current;
          Rectangle tileBounds4 = current.TileBounds;
          int left = tileBounds4.Left;
          while (true)
          {
            int num9 = left;
            tileBounds4 = current.TileBounds;
            int right = tileBounds4.Right;
            if (num9 < right)
            {
              tileBounds4 = current.TileBounds;
              int top = tileBounds4.Top;
              if (data2[left - tileBounds1.X, top - tileBounds1.Y] == '0')
              {
                for (int index = 1; index < 8; ++index)
                  virtualMap[left - tileBounds1.X, top - tileBounds1.Y - index] = true;
              }
              tileBounds4 = current.TileBounds;
              int num10 = tileBounds4.Bottom - 1;
              if (data2[left - tileBounds1.X, num10 - tileBounds1.Y] == '0')
              {
                for (int index = 1; index < 8; ++index)
                  virtualMap[left - tileBounds1.X, num10 - tileBounds1.Y + index] = true;
              }
              ++left;
            }
            else
              goto label_96;
          }
        }
      }
      using (List<LevelData>.Enumerator enumerator = mapData.Levels.GetEnumerator())
      {
label_118:
        while (enumerator.MoveNext())
        {
          LevelData current = enumerator.Current;
          Rectangle tileBounds5 = current.TileBounds;
          int left5 = tileBounds5.Left;
          while (true)
          {
            int num11 = left5;
            tileBounds5 = current.TileBounds;
            int right = tileBounds5.Right;
            if (num11 < right)
            {
              tileBounds5 = current.TileBounds;
              int top = tileBounds5.Top;
              char ch10 = data2[left5 - tileBounds1.X, top - tileBounds1.Y];
              for (int index = 1; index < 4 && !virtualMap[left5 - tileBounds1.X, top - tileBounds1.Y - index]; ++index)
                data2[left5 - tileBounds1.X, top - tileBounds1.Y - index] = ch10;
              tileBounds5 = current.TileBounds;
              int num12 = tileBounds5.Bottom - 1;
              char ch11 = data2[left5 - tileBounds1.X, num12 - tileBounds1.Y];
              for (int index = 1; index < 4 && !virtualMap[left5 - tileBounds1.X, num12 - tileBounds1.Y + index]; ++index)
                data2[left5 - tileBounds1.X, num12 - tileBounds1.Y + index] = ch11;
              ++left5;
            }
            else
              break;
          }
          tileBounds5 = current.TileBounds;
          int num13 = tileBounds5.Top - 4;
          while (true)
          {
            int num14 = num13;
            tileBounds5 = current.TileBounds;
            int num15 = tileBounds5.Bottom + 4;
            if (num14 < num15)
            {
              tileBounds5 = current.TileBounds;
              int left6 = tileBounds5.Left;
              char ch12 = data2[left6 - tileBounds1.X, num13 - tileBounds1.Y];
              for (int index = 1; index < 4 && !virtualMap[left6 - tileBounds1.X - index, num13 - tileBounds1.Y]; ++index)
                data2[left6 - tileBounds1.X - index, num13 - tileBounds1.Y] = ch12;
              tileBounds5 = current.TileBounds;
              int num16 = tileBounds5.Right - 1;
              char ch13 = data2[num16 - tileBounds1.X, num13 - tileBounds1.Y];
              for (int index = 1; index < 4 && !virtualMap[num16 - tileBounds1.X + index, num13 - tileBounds1.Y]; ++index)
                data2[num16 - tileBounds1.X + index, num13 - tileBounds1.Y] = ch13;
              ++num13;
            }
            else
              goto label_118;
          }
        }
      }
      Vector2 position = new Vector2((float) tileBounds1.X, (float) tileBounds1.Y) * 8f;
      Calc.PushRandom(mapData.LoadSeed);
      Level level1 = this.Level;
      Level level2 = this.Level;
      BackgroundTiles backgroundTiles1;
      BackgroundTiles backgroundTiles2 = backgroundTiles1 = new BackgroundTiles(position, data1);
      BackgroundTiles backgroundTiles3 = backgroundTiles1;
      level2.BgTiles = backgroundTiles1;
      BackgroundTiles backgroundTiles4 = backgroundTiles3;
      level1.Add((Entity) backgroundTiles4);
      Level level3 = this.Level;
      Level level4 = this.Level;
      SolidTiles solidTiles1;
      SolidTiles solidTiles2 = solidTiles1 = new SolidTiles(position, data2);
      SolidTiles solidTiles3 = solidTiles1;
      level4.SolidTiles = solidTiles1;
      SolidTiles solidTiles4 = solidTiles3;
      level3.Add((Entity) solidTiles4);
      this.Level.BgData = data1;
      this.Level.SolidsData = data2;
      Calc.PopRandom();
      new Entity(position)
      {
        (Component) (this.Level.FgTilesLightMask = new TileGrid(8, 8, tileBounds1.Width, tileBounds1.Height))
      };
      this.Level.FgTilesLightMask.Color = Color.Black;
      foreach (LevelData level5 in mapData.Levels)
      {
        Rectangle tileBounds6 = level5.TileBounds;
        int left = tileBounds6.Left;
        tileBounds6 = level5.TileBounds;
        int top = tileBounds6.Top;
        int width = level5.TileBounds.Width;
        int height = level5.TileBounds.Height;
        if (!string.IsNullOrEmpty(level5.BgTiles))
        {
          int[,] tiles = Calc.ReadCSVIntGrid(level5.BgTiles, width, height);
          backgroundTiles2.Tiles.Overlay(GFX.SceneryTiles, tiles, left - tileBounds1.X, top - tileBounds1.Y);
        }
        if (!string.IsNullOrEmpty(level5.FgTiles))
        {
          int[,] tiles = Calc.ReadCSVIntGrid(level5.FgTiles, width, height);
          solidTiles2.Tiles.Overlay(GFX.SceneryTiles, tiles, left - tileBounds1.X, top - tileBounds1.Y);
          this.Level.FgTilesLightMask.Overlay(GFX.SceneryTiles, tiles, left - tileBounds1.X, top - tileBounds1.Y);
        }
      }
      if (areaData.OnLevelBegin != null)
        areaData.OnLevelBegin(this.Level);
      this.Level.StartPosition = this.startPosition;
      this.Level.Pathfinder = new Pathfinder(this.Level);
      this.Loaded = true;
    }

    private void StartLevel()
    {
      this.started = true;
      Session session = this.Level.Session;
      this.Level.LoadLevel(!this.PlayerIntroTypeOverride.HasValue ? (!session.FirstLevel || !session.StartedFromBeginning || !session.JustStarted ? Player.IntroTypes.Respawn : (session.Area.Mode != AreaMode.CSide ? AreaData.Get((Scene) this.Level).IntroType : Player.IntroTypes.WalkInRight)) : this.PlayerIntroTypeOverride.Value, true);
      this.Level.Session.JustStarted = false;
      if (Engine.Scene != this)
        return;
      Engine.Scene = (Scene) this.Level;
    }

    public override void Update()
    {
      base.Update();
      if (!this.Loaded || this.started)
        return;
      this.StartLevel();
    }
  }
}
