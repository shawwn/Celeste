// Decompiled with JetBrains decompiler
// Type: Celeste.GFX
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace Celeste
{
  public static class GFX
  {
    public static Atlas Game;
    public static Atlas Gui;
    public static Atlas Opening;
    public static Atlas Misc;
    public static Atlas Portraits;
    public static Atlas ColorGrades;
    public static VirtualTexture SplashScreen;
    public static VirtualTexture MagicGlowNoise;
    public static Effect FxMountain;
    public static Effect FxDistort;
    public static Effect FxGlitch;
    public static Effect FxGaussianBlur;
    public static Effect FxPrimitive;
    public static Effect FxDust;
    public static Effect FxDither;
    public static Effect FxMagicGlow;
    public static Effect FxMirrors;
    public static Effect FxColorGrading;
    public static BasicEffect FxDebug;
    public static Effect FxTexture;
    public static Effect FxLighting;
    public static SpriteBank SpriteBank;
    public static SpriteBank GuiSpriteBank;
    public static SpriteBank PortraitsSpriteBank;
    public static XmlDocument CompleteScreensXml;
    public static AnimatedTilesBank AnimatedTilesBank;
    public static Tileset SceneryTiles;
    public static Autotiler BGAutotiler;
    public static Autotiler FGAutotiler;
    public const float PortraitSize = 240f;
    public static readonly BlendState Subtract = new BlendState()
    {
      ColorSourceBlend = Blend.One,
      ColorDestinationBlend = Blend.One,
      ColorBlendFunction = BlendFunction.ReverseSubtract,
      AlphaSourceBlend = Blend.One,
      AlphaDestinationBlend = Blend.One,
      AlphaBlendFunction = BlendFunction.Add
    };
    public static readonly BlendState DestinationTransparencySubtract = new BlendState()
    {
      ColorSourceBlend = Blend.One,
      ColorDestinationBlend = Blend.One,
      ColorBlendFunction = BlendFunction.ReverseSubtract,
      AlphaSourceBlend = Blend.Zero,
      AlphaDestinationBlend = Blend.One,
      AlphaBlendFunction = BlendFunction.Add
    };

    public static bool Loaded { get; private set; }

    public static bool DataLoaded { get; private set; }

    public static void Load()
    {
      if (!GFX.Loaded)
      {
        Stopwatch stopwatch = Stopwatch.StartNew();
        GFX.Game = Atlas.FromAtlas(Path.Combine("Graphics", "Atlases", "Gameplay"), Atlas.AtlasDataFormat.Packer);
        GFX.Opening = Atlas.FromAtlas(Path.Combine("Graphics", "Atlases", "Opening"), Atlas.AtlasDataFormat.PackerNoAtlas);
        GFX.Gui = Atlas.FromAtlas(Path.Combine("Graphics", "Atlases", "Gui"), Atlas.AtlasDataFormat.Packer);
        GFX.GuiSpriteBank = new SpriteBank(GFX.Gui, Path.Combine("Graphics", "SpritesGui.xml"));
        GFX.Misc = Atlas.FromAtlas(Path.Combine("Graphics", "Atlases", "Misc"), Atlas.AtlasDataFormat.PackerNoAtlas);
        GFX.Portraits = Atlas.FromAtlas(Path.Combine("Graphics", "Atlases", "Portraits"), Atlas.AtlasDataFormat.PackerNoAtlas);
        Draw.Particle = GFX.Game["util/particle"];
        Draw.Pixel = new MTexture(GFX.Game["util/pixel"], 1, 1, 1, 1);
        ParticleTypes.Load();
        GFX.ColorGrades = Atlas.FromDirectory(Path.Combine("Graphics", "ColorGrading"));
        GFX.MagicGlowNoise = VirtualContent.CreateTexture("glow-noise", 128, 128, Color.White);
        Color[] data = new Color[GFX.MagicGlowNoise.Width * GFX.MagicGlowNoise.Height];
        for (int index = 0; index < data.Length; ++index)
          data[index] = new Color(Calc.Random.NextFloat(), Calc.Random.NextFloat(), Calc.Random.NextFloat(), 0.0f);
        GFX.MagicGlowNoise.Texture.SetData<Color>(data);
        Console.WriteLine(" - GFX LOAD: " + (object) stopwatch.ElapsedMilliseconds + "ms");
      }
      GFX.Loaded = true;
    }

    public static void LoadData()
    {
      if (!GFX.DataLoaded)
      {
        Stopwatch stopwatch = Stopwatch.StartNew();
        GFX.PortraitsSpriteBank = new SpriteBank(GFX.Portraits, Path.Combine("Graphics", "Portraits.xml"));
        GFX.SpriteBank = new SpriteBank(GFX.Game, Path.Combine("Graphics", "Sprites.xml"));
        GFX.BGAutotiler = new Autotiler(Path.Combine("Graphics", "BackgroundTiles.xml"));
        GFX.FGAutotiler = new Autotiler(Path.Combine("Graphics", "ForegroundTiles.xml"));
        GFX.SceneryTiles = new Tileset(GFX.Game["tilesets/scenery"], 8, 8);
        PlayerSprite.ClearFramesMetadata();
        PlayerSprite.CreateFramesMetadata("player");
        PlayerSprite.CreateFramesMetadata("player_no_backpack");
        PlayerSprite.CreateFramesMetadata("badeline");
        PlayerSprite.CreateFramesMetadata("player_badeline");
        PlayerSprite.CreateFramesMetadata("player_playback");
        GFX.CompleteScreensXml = Calc.LoadContentXML(Path.Combine("Graphics", "CompleteScreens.xml"));
        GFX.AnimatedTilesBank = new AnimatedTilesBank();
        foreach (XmlElement xml in (XmlNode) Calc.LoadContentXML(Path.Combine("Graphics", "AnimatedTiles.xml"))["Data"])
        {
          if (xml != null)
            GFX.AnimatedTilesBank.Add(xml.Attr("name"), xml.AttrFloat("delay", 0.0f), xml.AttrVector2("posX", "posY", Vector2.Zero), xml.AttrVector2("origX", "origY", Vector2.Zero), GFX.Game.GetAtlasSubtextures(xml.Attr("path")));
        }
        Console.WriteLine(" - GFX DATA LOAD: " + (object) stopwatch.ElapsedMilliseconds + "ms");
      }
      GFX.DataLoaded = true;
    }

    public static void Unload()
    {
      if (GFX.Loaded)
      {
        GFX.Game.Dispose();
        GFX.Game = (Atlas) null;
        GFX.Gui.Dispose();
        GFX.Gui = (Atlas) null;
        GFX.Opening.Dispose();
        GFX.Opening = (Atlas) null;
        GFX.Misc.Dispose();
        GFX.Misc = (Atlas) null;
        GFX.ColorGrades.Dispose();
        GFX.ColorGrades = (Atlas) null;
        GFX.MagicGlowNoise.Dispose();
        GFX.MagicGlowNoise = (VirtualTexture) null;
        GFX.Portraits.Dispose();
        GFX.Portraits = (Atlas) null;
      }
      GFX.Loaded = false;
    }

    public static void UnloadData()
    {
      if (GFX.DataLoaded)
      {
        GFX.GuiSpriteBank = (SpriteBank) null;
        GFX.PortraitsSpriteBank = (SpriteBank) null;
        GFX.SpriteBank = (SpriteBank) null;
        GFX.CompleteScreensXml = (XmlDocument) null;
        GFX.SceneryTiles = (Tileset) null;
        GFX.BGAutotiler = (Autotiler) null;
        GFX.FGAutotiler = (Autotiler) null;
      }
      GFX.DataLoaded = false;
    }

    public static void LoadEffects()
    {
      GFX.FxMountain = GFX.LoadFx("MountainRender");
      GFX.FxGaussianBlur = GFX.LoadFx("GaussianBlur");
      GFX.FxDistort = GFX.LoadFx("Distort");
      GFX.FxDust = GFX.LoadFx("Dust");
      GFX.FxPrimitive = GFX.LoadFx("Primitive");
      GFX.FxDither = GFX.LoadFx("Dither");
      GFX.FxMagicGlow = GFX.LoadFx("MagicGlow");
      GFX.FxMirrors = GFX.LoadFx("Mirrors");
      GFX.FxColorGrading = GFX.LoadFx("ColorGrade");
      GFX.FxGlitch = GFX.LoadFx("Glitch");
      GFX.FxTexture = GFX.LoadFx("Texture");
      GFX.FxLighting = GFX.LoadFx("Lighting");
      GFX.FxDebug = new BasicEffect(Engine.Graphics.GraphicsDevice);
    }

    public static Effect LoadFx(string name) => Engine.Instance.Content.Load<Effect>(Path.Combine("Effects", name));

    public static void DrawVertices<T>(
      Matrix matrix,
      T[] vertices,
      int vertexCount,
      Effect effect = null,
      BlendState blendState = null)
      where T : struct, IVertexType
    {
      Effect effect1 = effect != null ? effect : GFX.FxPrimitive;
      BlendState blendState1 = blendState != null ? blendState : BlendState.AlphaBlend;
      Vector2 local;
      Viewport viewport = Engine.Graphics.GraphicsDevice.Viewport;
      double width = (double) viewport.Width;
      viewport = Engine.Graphics.GraphicsDevice.Viewport;
      double height = (double) viewport.Height;
      local = new Vector2((float) width, (float) height);
      matrix *= Matrix.CreateScale((float) (1.0 / (double) local.X * 2.0), (float) (-(1.0 / (double) local.Y) * 2.0), 1f);
      matrix *= Matrix.CreateTranslation(-1f, 1f, 0.0f);
      Engine.Instance.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
      Engine.Instance.GraphicsDevice.BlendState = blendState1;
      effect1.Parameters["World"].SetValue(matrix);
      foreach (EffectPass pass in effect1.CurrentTechnique.Passes)
      {
        pass.Apply();
        Engine.Instance.GraphicsDevice.DrawUserPrimitives<T>(PrimitiveType.TriangleList, vertices, 0, vertexCount / 3);
      }
    }

    public static void DrawIndexedVertices<T>(
      Matrix matrix,
      T[] vertices,
      int vertexCount,
      int[] indices,
      int primitiveCount,
      Effect effect = null,
      BlendState blendState = null)
      where T : struct, IVertexType
    {
      Effect effect1 = effect != null ? effect : GFX.FxPrimitive;
      BlendState blendState1 = blendState != null ? blendState : BlendState.AlphaBlend;
      Vector2 local;
      Viewport viewport = Engine.Graphics.GraphicsDevice.Viewport;
      double width = (double) viewport.Width;
      viewport = Engine.Graphics.GraphicsDevice.Viewport;
      double height = (double) viewport.Height;
      local = new Vector2((float) width, (float) height);
      matrix *= Matrix.CreateScale((float) (1.0 / (double) local.X * 2.0), (float) (-(1.0 / (double) local.Y) * 2.0), 1f);
      matrix *= Matrix.CreateTranslation(-1f, 1f, 0.0f);
      Engine.Instance.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
      Engine.Instance.GraphicsDevice.BlendState = blendState1;
      effect1.Parameters["World"].SetValue(matrix);
      foreach (EffectPass pass in effect1.CurrentTechnique.Passes)
      {
        pass.Apply();
        Engine.Instance.GraphicsDevice.DrawUserIndexedPrimitives<T>(PrimitiveType.TriangleList, vertices, 0, vertexCount, indices, 0, primitiveCount);
      }
    }
  }
}
