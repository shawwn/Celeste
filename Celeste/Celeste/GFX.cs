// Decompiled with JetBrains decompiler
// Type: Celeste.GFX
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Celeste
{
  public static class GFX
  {
    public static Atlas Game;
    public static Atlas Gui;
    public static Atlas Opening;
    public static Atlas Journal;
    public static Atlas Misc;
    public static Atlas Portraits;
    public static Atlas ColorGrades;
    public static Atlas Overworld;
    public static Atlas Mountain;
    public static Atlas Checkpoints;
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
    public static ObjModel MountainTerrain;
    public static ObjModel MountainBuildings;
    public static ObjModel MountainCoreWall;
    public static VirtualTexture[] MountainTerrainTextures;
    public static VirtualTexture[] MountainBuildingTextures;
    public static VirtualTexture[] MountainSkyboxTextures;
    public static VirtualTexture MountainFogTexture;
    public const float PortraitSize = 240f;
    public static bool LoadedMainContent;
    public static readonly BlendState Subtract;
    public static readonly BlendState DestinationTransparencySubtract;

    public static void LoadGame()
    {
      GFX.LoadedMainContent = true;
      GFX.Game = Atlas.FromAtlas(Path.Combine("Graphics", "Atlases", "Gameplay"), Atlas.AtlasDataFormat.Packer);
      Draw.Particle = GFX.Game["util/particle"];
      Draw.Pixel = new MTexture(GFX.Game["util/pixel"], 1, 1, 1, 1);
      ParticleTypes.Load();
    }

    public static void UnloadGame()
    {
      GFX.Game.Dispose();
      GFX.Game = (Atlas) null;
    }

    public static void LoadGui()
    {
      GFX.Opening = Atlas.FromAtlas(Path.Combine("Graphics", "Atlases", "Opening"), Atlas.AtlasDataFormat.PackerNoAtlas);
      GFX.Gui = Atlas.FromAtlas(Path.Combine("Graphics", "Atlases", "Gui"), Atlas.AtlasDataFormat.Packer);
      GFX.GuiSpriteBank = new SpriteBank(GFX.Gui, Path.Combine("Graphics", "SpritesGui.xml"));
      GFX.Journal = Atlas.FromAtlas(Path.Combine("Graphics", "Atlases", "Journal"), Atlas.AtlasDataFormat.Packer);
      GFX.Misc = Atlas.FromAtlas(Path.Combine("Graphics", "Atlases", "Misc"), Atlas.AtlasDataFormat.PackerNoAtlas);
    }

    public static void UnloadGui()
    {
      GFX.Gui.Dispose();
      GFX.Gui = (Atlas) null;
      if (GFX.Opening != null)
        GFX.Opening.Dispose();
      GFX.Opening = (Atlas) null;
      GFX.Journal.Dispose();
      GFX.Journal = (Atlas) null;
      GFX.Misc.Dispose();
      GFX.Misc = (Atlas) null;
      GFX.GuiSpriteBank = (SpriteBank) null;
    }

    public static void LoadOverworld()
    {
      GFX.Overworld = Atlas.FromAtlas(Path.Combine("Graphics", "Atlases", "Overworld"), Atlas.AtlasDataFormat.PackerNoAtlas);
    }

    public static void UnloadOverworld()
    {
      GFX.Overworld.Dispose();
      GFX.Overworld = (Atlas) null;
    }

    public static void LoadMountain()
    {
      GFX.Mountain = Atlas.FromAtlas(Path.Combine("Graphics", "Atlases", "Mountain"), Atlas.AtlasDataFormat.PackerNoAtlas);
      GFX.Checkpoints = Atlas.FromAtlas(Path.Combine("Graphics", "Atlases", "Checkpoints"), Atlas.AtlasDataFormat.Packer);
      GFX.MountainTerrainTextures = new VirtualTexture[3];
      GFX.MountainBuildingTextures = new VirtualTexture[3];
      GFX.MountainSkyboxTextures = new VirtualTexture[3];
      for (int index = 0; index < 3; ++index)
      {
        GFX.MountainSkyboxTextures[index] = GFX.Mountain["skybox_" + (object) index].Texture;
        GFX.MountainTerrainTextures[index] = GFX.Mountain["mountain_" + (object) index].Texture;
        GFX.MountainBuildingTextures[index] = GFX.Mountain["buildings_" + (object) index].Texture;
      }
      GFX.MountainFogTexture = GFX.Mountain["fog"].Texture;
    }

    public static void UnloadMountain()
    {
      GFX.Mountain.Dispose();
      GFX.Mountain = (Atlas) null;
      GFX.Checkpoints.Dispose();
      GFX.Checkpoints = (Atlas) null;
    }

    public static void LoadOther()
    {
      GFX.ColorGrades = Atlas.FromDirectory(Path.Combine("Graphics", "ColorGrading"));
      GFX.MagicGlowNoise = VirtualContent.CreateTexture("glow-noise", 128, 128, Color.get_White());
      Color[] colorArray = new Color[GFX.MagicGlowNoise.Width * GFX.MagicGlowNoise.Height];
      for (int index = 0; index < colorArray.Length; ++index)
        colorArray[index] = new Color(Calc.Random.NextFloat(), Calc.Random.NextFloat(), Calc.Random.NextFloat(), 0.0f);
      GFX.MagicGlowNoise.Texture.SetData<Color>((M0[]) colorArray);
    }

    public static void UnloadOther()
    {
      GFX.ColorGrades.Dispose();
      GFX.ColorGrades = (Atlas) null;
      GFX.MagicGlowNoise.Dispose();
      GFX.MagicGlowNoise = (VirtualTexture) null;
    }

    public static void LoadPortraits()
    {
      GFX.Portraits = Atlas.FromAtlas(Path.Combine("Graphics", "Atlases", "Portraits"), Atlas.AtlasDataFormat.PackerNoAtlas);
      GFX.PortraitsSpriteBank = new SpriteBank(GFX.Portraits, Path.Combine("Graphics", "Portraits.xml"));
    }

    public static void UnloadPortraits()
    {
      GFX.Portraits.Dispose();
      GFX.Portraits = (Atlas) null;
      GFX.PortraitsSpriteBank = (SpriteBank) null;
    }

    public static void LoadData()
    {
      GFX.SpriteBank = new SpriteBank(GFX.Game, Path.Combine("Graphics", "Sprites.xml"));
      GFX.BGAutotiler = new Autotiler(Path.Combine("Graphics", "BackgroundTiles.xml"));
      GFX.FGAutotiler = new Autotiler(Path.Combine("Graphics", "ForegroundTiles.xml"));
      GFX.SceneryTiles = new Tileset(GFX.Game["tilesets/scenery"], 8, 8);
      PlayerSprite.ClearFramesMetadata();
      PlayerSprite.CreateFramesMetadata("player");
      PlayerSprite.CreateFramesMetadata("player_no_backpack");
      PlayerSprite.CreateFramesMetadata("badeline");
      GFX.CompleteScreensXml = Calc.LoadContentXML(Path.Combine("Graphics", "CompleteScreens.xml"));
      GFX.AnimatedTilesBank = new AnimatedTilesBank();
      foreach (XmlElement xml in (XmlNode) Calc.LoadContentXML(Path.Combine("Graphics", "AnimatedTiles.xml"))["Data"])
      {
        if (xml != null)
          GFX.AnimatedTilesBank.Add(xml.Attr("name"), xml.AttrFloat("delay", 0.0f), xml.AttrVector2("posX", "posY", Vector2.get_Zero()), xml.AttrVector2("origX", "origY", Vector2.get_Zero()), GFX.Game.GetAtlasSubtextures(xml.Attr("path")));
      }
    }

    public static void UnloadData()
    {
      GFX.SpriteBank = (SpriteBank) null;
      GFX.CompleteScreensXml = (XmlDocument) null;
      GFX.SceneryTiles = (Tileset) null;
      GFX.BGAutotiler = (Autotiler) null;
      GFX.FGAutotiler = (Autotiler) null;
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
      GFX.FxDebug = new BasicEffect(Engine.Graphics.get_GraphicsDevice());
    }

    public static Effect LoadFx(string name)
    {
      return (Effect) Engine.Instance.get_Content().Load<Effect>(Path.Combine("Effects", name));
    }

    public static void DrawVertices<T>(
      Matrix matrix,
      T[] vertices,
      int vertexCount,
      Effect effect = null,
      BlendState blendState = null)
      where T : struct, IVertexType
    {
      Effect effect1 = effect != null ? effect : GFX.FxPrimitive;
      BlendState blendState1 = blendState != null ? blendState : (BlendState) BlendState.AlphaBlend;
      Vector2 vector2;
      ref Vector2 local = ref vector2;
      Viewport viewport = Engine.Graphics.get_GraphicsDevice().get_Viewport();
      double width = (double) ((Viewport) ref viewport).get_Width();
      viewport = Engine.Graphics.get_GraphicsDevice().get_Viewport();
      double height = (double) ((Viewport) ref viewport).get_Height();
      ((Vector2) ref local).\u002Ector((float) width, (float) height);
      matrix = Matrix.op_Multiply(matrix, Matrix.CreateScale((float) (1.0 / vector2.X * 2.0), (float) (-(1.0 / vector2.Y) * 2.0), 1f));
      matrix = Matrix.op_Multiply(matrix, Matrix.CreateTranslation(-1f, 1f, 0.0f));
      Engine.Instance.get_GraphicsDevice().set_RasterizerState((RasterizerState) RasterizerState.CullNone);
      Engine.Instance.get_GraphicsDevice().set_BlendState(blendState1);
      effect1.get_Parameters().get_Item("World").SetValue(matrix);
      using (List<EffectPass>.Enumerator enumerator = effect1.get_CurrentTechnique().get_Passes().GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          enumerator.Current.Apply();
          Engine.Instance.get_GraphicsDevice().DrawUserPrimitives<T>((PrimitiveType) 0, vertices, 0, vertexCount / 3);
        }
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
      BlendState blendState1 = blendState != null ? blendState : (BlendState) BlendState.AlphaBlend;
      Vector2 vector2;
      ref Vector2 local = ref vector2;
      Viewport viewport = Engine.Graphics.get_GraphicsDevice().get_Viewport();
      double width = (double) ((Viewport) ref viewport).get_Width();
      viewport = Engine.Graphics.get_GraphicsDevice().get_Viewport();
      double height = (double) ((Viewport) ref viewport).get_Height();
      ((Vector2) ref local).\u002Ector((float) width, (float) height);
      matrix = Matrix.op_Multiply(matrix, Matrix.CreateScale((float) (1.0 / vector2.X * 2.0), (float) (-(1.0 / vector2.Y) * 2.0), 1f));
      matrix = Matrix.op_Multiply(matrix, Matrix.CreateTranslation(-1f, 1f, 0.0f));
      Engine.Instance.get_GraphicsDevice().set_RasterizerState((RasterizerState) RasterizerState.CullNone);
      Engine.Instance.get_GraphicsDevice().set_BlendState(blendState1);
      effect1.get_Parameters().get_Item("World").SetValue(matrix);
      using (List<EffectPass>.Enumerator enumerator = effect1.get_CurrentTechnique().get_Passes().GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          enumerator.Current.Apply();
          Engine.Instance.get_GraphicsDevice().DrawUserIndexedPrimitives<T>((PrimitiveType) 0, vertices, 0, vertexCount, indices, 0, primitiveCount);
        }
      }
    }

    static GFX()
    {
      BlendState blendState1 = new BlendState();
      blendState1.set_ColorSourceBlend((Blend) 0);
      blendState1.set_ColorDestinationBlend((Blend) 0);
      blendState1.set_ColorBlendFunction((BlendFunction) 2);
      blendState1.set_AlphaSourceBlend((Blend) 0);
      blendState1.set_AlphaDestinationBlend((Blend) 0);
      blendState1.set_AlphaBlendFunction((BlendFunction) 0);
      GFX.Subtract = blendState1;
      BlendState blendState2 = new BlendState();
      blendState2.set_ColorSourceBlend((Blend) 0);
      blendState2.set_ColorDestinationBlend((Blend) 0);
      blendState2.set_ColorBlendFunction((BlendFunction) 2);
      blendState2.set_AlphaSourceBlend((Blend) 1);
      blendState2.set_AlphaDestinationBlend((Blend) 0);
      blendState2.set_AlphaBlendFunction((BlendFunction) 0);
      GFX.DestinationTransparencySubtract = blendState2;
    }
  }
}
