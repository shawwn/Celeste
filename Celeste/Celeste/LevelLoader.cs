// Decompiled with JetBrains decompiler
// Type: Celeste.LevelLoader
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Celeste
{
  public class LevelLoader : Scene
  {
    private bool started = false;
    private Session session;
    private Vector2? startPosition;
    public Player.IntroTypes? PlayerIntroTypeOverride;

    public Level Level { get; private set; }

    public bool Loaded { get; private set; }

    public LevelLoader(Session session, Vector2? startPosition = null)
    {
      this.session = session;
      this.startPosition = startPosition.HasValue ? startPosition : session.RespawnPoint;
      this.Level = new Level();
      RunThread.Start(new Action(this.LoadingThread), "LEVEL_LOADER", false);
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
      this.Level.Add((Monocle.Renderer) (this.Level.HudRenderer = new HudRenderer()));
      if (this.session.Area.ID == 9)
        this.Level.Add((Entity) new IceTileOverlay());
      this.Level.BaseLightingAlpha = this.Level.Lighting.Alpha = areaData.DarknessAlpha;
      this.Level.Bloom.Base = areaData.BloomBase;
      this.Level.Bloom.Strength = areaData.BloomStrength;
      this.Level.BackgroundColor = mapData.BackgroundColor;
      this.Level.Background.Backdrops = mapData.CreateBackdrops(mapData.Background);
      this.Level.Foreground.Backdrops = mapData.CreateBackdrops(mapData.Foreground);
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
      this.Level.ParticlesFG = new ParticleSystem(-50000, 400);
      this.Level.ParticlesFG.Tag = (int) Tags.Global;
      this.Level.ParticlesFG.Add((Component) new MirrorReflection());
      this.Level.Add((Entity) this.Level.ParticlesFG);
      this.Level.Add((Entity) (this.Level.strawberriesDisplay = new TotalStrawberriesDisplay()));
      this.Level.Add((Entity) new SpeedrunTimerDisplay());
      this.Level.Add((Entity) new GameplayStats());
      Rectangle tileBounds1 = mapData.TileBounds;
      GFX.FGAutotiler.LevelBounds.Clear();
      VirtualMap<char> data1 = new VirtualMap<char>(tileBounds1.Width, tileBounds1.Height, '0');
      VirtualMap<char> data2 = new VirtualMap<char>(tileBounds1.Width, tileBounds1.Height, '0');
      VirtualMap<bool> virtualMap = new VirtualMap<bool>(tileBounds1.Width, tileBounds1.Height, false);
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
        for (int index1 = top1; index1 < top1 + strArray2.Length; ++index1)
        {
          for (int index2 = left1; index2 < left1 + strArray2[index1 - top1].Length; ++index2)
            data2[index2 - tileBounds1.X, index1 - tileBounds1.Y] = strArray2[index1 - top1][index2 - left1];
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
              char ch2 = data2[rectangle.Left - tileBounds1.X - 1, top - tileBounds1.Y];
              if (ch2 != '0')
                ch1 = ch2;
            }
            if (ch1 == '0' && rectangle.Right - tileBounds1.X < tileBounds1.Width - 1)
            {
              char ch2 = data2[rectangle.Right - tileBounds1.X, top - tileBounds1.Y];
              if (ch2 != '0')
                ch1 = ch2;
            }
            if (ch1 == '0' && rectangle.Bottom - tileBounds1.Y < tileBounds1.Height - 1)
            {
              char ch2 = data2[left - tileBounds1.X, rectangle.Bottom - tileBounds1.Y];
              if (ch2 != '0')
                ch1 = ch2;
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
label_75:
        while (enumerator.MoveNext())
        {
          LevelData current = enumerator.Current;
          Rectangle tileBounds2 = current.TileBounds;
          int left1 = tileBounds2.Left;
          while (true)
          {
            int num1 = left1;
            tileBounds2 = current.TileBounds;
            int right = tileBounds2.Right;
            if (num1 < right)
            {
              tileBounds2 = current.TileBounds;
              int top = tileBounds2.Top;
              char ch1 = data1[left1 - tileBounds1.X, top - tileBounds1.Y];
              for (int index = 1; index < 4 && !virtualMap[left1 - tileBounds1.X, top - tileBounds1.Y - index]; ++index)
                data1[left1 - tileBounds1.X, top - tileBounds1.Y - index] = ch1;
              tileBounds2 = current.TileBounds;
              int num2 = tileBounds2.Bottom - 1;
              char ch2 = data1[left1 - tileBounds1.X, num2 - tileBounds1.Y];
              for (int index = 1; index < 4 && !virtualMap[left1 - tileBounds1.X, num2 - tileBounds1.Y + index]; ++index)
                data1[left1 - tileBounds1.X, num2 - tileBounds1.Y + index] = ch2;
              ++left1;
            }
            else
              break;
          }
          tileBounds2 = current.TileBounds;
          int num3 = tileBounds2.Top - 4;
          while (true)
          {
            int num1 = num3;
            tileBounds2 = current.TileBounds;
            int num2 = tileBounds2.Bottom + 4;
            if (num1 < num2)
            {
              tileBounds2 = current.TileBounds;
              int left2 = tileBounds2.Left;
              char ch1 = data1[left2 - tileBounds1.X, num3 - tileBounds1.Y];
              for (int index = 1; index < 4 && !virtualMap[left2 - tileBounds1.X - index, num3 - tileBounds1.Y]; ++index)
                data1[left2 - tileBounds1.X - index, num3 - tileBounds1.Y] = ch1;
              tileBounds2 = current.TileBounds;
              int num4 = tileBounds2.Right - 1;
              char ch2 = data1[num4 - tileBounds1.X, num3 - tileBounds1.Y];
              for (int index = 1; index < 4 && !virtualMap[num4 - tileBounds1.X + index, num3 - tileBounds1.Y]; ++index)
                data1[num4 - tileBounds1.X + index, num3 - tileBounds1.Y] = ch2;
              ++num3;
            }
            else
              goto label_75;
          }
        }
      }
      using (List<LevelData>.Enumerator enumerator = mapData.Levels.GetEnumerator())
      {
label_90:
        while (enumerator.MoveNext())
        {
          LevelData current = enumerator.Current;
          Rectangle tileBounds2 = current.TileBounds;
          int left = tileBounds2.Left;
          while (true)
          {
            int num1 = left;
            tileBounds2 = current.TileBounds;
            int right = tileBounds2.Right;
            if (num1 < right)
            {
              tileBounds2 = current.TileBounds;
              int top = tileBounds2.Top;
              if (data2[left - tileBounds1.X, top - tileBounds1.Y] == '0')
              {
                for (int index = 1; index < 8; ++index)
                  virtualMap[left - tileBounds1.X, top - tileBounds1.Y - index] = true;
              }
              tileBounds2 = current.TileBounds;
              int num2 = tileBounds2.Bottom - 1;
              if (data2[left - tileBounds1.X, num2 - tileBounds1.Y] == '0')
              {
                for (int index = 1; index < 8; ++index)
                  virtualMap[left - tileBounds1.X, num2 - tileBounds1.Y + index] = true;
              }
              ++left;
            }
            else
              goto label_90;
          }
        }
      }
      using (List<LevelData>.Enumerator enumerator = mapData.Levels.GetEnumerator())
      {
label_112:
        while (enumerator.MoveNext())
        {
          LevelData current = enumerator.Current;
          Rectangle tileBounds2 = current.TileBounds;
          int left1 = tileBounds2.Left;
          while (true)
          {
            int num1 = left1;
            tileBounds2 = current.TileBounds;
            int right = tileBounds2.Right;
            if (num1 < right)
            {
              tileBounds2 = current.TileBounds;
              int top = tileBounds2.Top;
              char ch1 = data2[left1 - tileBounds1.X, top - tileBounds1.Y];
              for (int index = 1; index < 4 && !virtualMap[left1 - tileBounds1.X, top - tileBounds1.Y - index]; ++index)
                data2[left1 - tileBounds1.X, top - tileBounds1.Y - index] = ch1;
              tileBounds2 = current.TileBounds;
              int num2 = tileBounds2.Bottom - 1;
              char ch2 = data2[left1 - tileBounds1.X, num2 - tileBounds1.Y];
              for (int index = 1; index < 4 && !virtualMap[left1 - tileBounds1.X, num2 - tileBounds1.Y + index]; ++index)
                data2[left1 - tileBounds1.X, num2 - tileBounds1.Y + index] = ch2;
              ++left1;
            }
            else
              break;
          }
          tileBounds2 = current.TileBounds;
          int num3 = tileBounds2.Top - 4;
          while (true)
          {
            int num1 = num3;
            tileBounds2 = current.TileBounds;
            int num2 = tileBounds2.Bottom + 4;
            if (num1 < num2)
            {
              tileBounds2 = current.TileBounds;
              int left2 = tileBounds2.Left;
              char ch1 = data2[left2 - tileBounds1.X, num3 - tileBounds1.Y];
              for (int index = 1; index < 4 && !virtualMap[left2 - tileBounds1.X - index, num3 - tileBounds1.Y]; ++index)
                data2[left2 - tileBounds1.X - index, num3 - tileBounds1.Y] = ch1;
              tileBounds2 = current.TileBounds;
              int num4 = tileBounds2.Right - 1;
              char ch2 = data2[num4 - tileBounds1.X, num3 - tileBounds1.Y];
              for (int index = 1; index < 4 && !virtualMap[num4 - tileBounds1.X + index, num3 - tileBounds1.Y]; ++index)
                data2[num4 - tileBounds1.X + index, num3 - tileBounds1.Y] = ch2;
              ++num3;
            }
            else
              goto label_112;
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
        int left = level5.TileBounds.Left;
        int top = level5.TileBounds.Top;
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

