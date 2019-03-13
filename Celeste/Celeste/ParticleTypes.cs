// Decompiled with JetBrains decompiler
// Type: Celeste.ParticleTypes
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;

namespace Celeste
{
  public static class ParticleTypes
  {
    public static ParticleType Dust;
    public static ParticleType Chimney;
    public static ParticleType Steam;
    public static ParticleType VentDust;

    public static void Load()
    {
      Chooser<MTexture> chooser = new Chooser<MTexture>(new MTexture[4]
      {
        GFX.Game["particles/smoke0"],
        GFX.Game["particles/smoke1"],
        GFX.Game["particles/smoke2"],
        GFX.Game["particles/smoke3"]
      });
      ParticleTypes.Dust = new ParticleType()
      {
        SourceChooser = chooser,
        Color = Color.get_White(),
        Acceleration = new Vector2(0.0f, 4f),
        LifeMin = 0.3f,
        LifeMax = 0.5f,
        Size = 0.7f,
        SizeRange = 0.2f,
        Direction = 1.570796f,
        DirectionRange = 0.5f,
        SpeedMin = 5f,
        SpeedMax = 15f,
        RotationMode = ParticleType.RotationModes.Random,
        ScaleOut = true,
        UseActualDeltaTime = true
      };
      ParticleTypes.Chimney = new ParticleType()
      {
        SourceChooser = chooser,
        Color = Color.get_White(),
        Color2 = Color.get_LightGray(),
        ColorMode = ParticleType.ColorModes.Choose,
        Acceleration = new Vector2(-4f, 1f),
        LifeMin = 2f,
        LifeMax = 4f,
        Size = 1f,
        SizeRange = 0.25f,
        Direction = 1.570796f,
        DirectionRange = 0.5f,
        SpeedMin = 4f,
        SpeedMax = 12f,
        RotationMode = ParticleType.RotationModes.Random,
        ScaleOut = true
      };
      ParticleTypes.Steam = new ParticleType()
      {
        SourceChooser = chooser,
        Acceleration = new Vector2(0.0f, -4f),
        LifeMin = 2f,
        LifeMax = 4f,
        Size = 0.5f,
        SizeRange = 0.0f,
        Direction = -1.570796f,
        DirectionRange = 0.5f,
        SpeedMin = 4f,
        SpeedMax = 12f,
        RotationMode = ParticleType.RotationModes.Random,
        ScaleOut = true,
        Color = Color.op_Multiply(Color.get_White(), 0.2f),
        FadeMode = ParticleType.FadeModes.Late
      };
      ParticleTypes.VentDust = new ParticleType()
      {
        Color = Color.get_LightGray(),
        FadeMode = ParticleType.FadeModes.Linear,
        Size = 1f,
        SizeRange = 0.0f,
        SpeedMin = 20f,
        SpeedMax = 40f,
        Direction = 1.570796f,
        DirectionRange = 0.05f,
        Acceleration = Vector2.op_Multiply(Vector2.get_UnitY(), 20f),
        LifeMin = 0.4f,
        LifeMax = 0.8f
      };
      Player.P_DashA = new ParticleType()
      {
        Color = Calc.HexToColor("44B7FF"),
        Color2 = Calc.HexToColor("75c9ff"),
        ColorMode = ParticleType.ColorModes.Blink,
        FadeMode = ParticleType.FadeModes.Late,
        LifeMin = 1f,
        LifeMax = 1.8f,
        Size = 1f,
        SpeedMin = 10f,
        SpeedMax = 20f,
        Acceleration = new Vector2(0.0f, 8f),
        DirectionRange = 1.047198f
      };
      Player.P_DashB = new ParticleType(Player.P_DashA)
      {
        Color = Calc.HexToColor("AC3232"),
        Color2 = Calc.HexToColor("e05959")
      };
      Player.P_CassetteFly = new ParticleType()
      {
        Color = Color.op_Multiply(Color.get_White(), 0.6f),
        Source = GFX.Game["particles/bubble"],
        FadeMode = ParticleType.FadeModes.Late,
        LifeMin = 0.4f,
        LifeMax = 1f,
        Size = 1f,
        SizeRange = 0.5f,
        SpeedMin = 8f,
        SpeedMax = 16f,
        Acceleration = new Vector2(0.0f, -24f),
        DirectionRange = 6.283185f
      };
      Player.P_Split = new ParticleType()
      {
        Color = Player.TwoDashesHairColor,
        Color2 = Calc.HexToColor("e256d3"),
        ColorMode = ParticleType.ColorModes.Choose,
        Size = 1f,
        FadeMode = ParticleType.FadeModes.Late,
        LifeMin = 0.6f,
        LifeMax = 0.9f,
        SpeedMin = 60f,
        SpeedMax = 80f,
        SpeedMultiplier = 0.1f,
        DirectionRange = 6.283185f
      };
      Player.P_SummitLandA = new ParticleType(ParticleTypes.Dust)
      {
        DirectionRange = 1.396263f,
        SpeedMin = 5f,
        SpeedMax = 10f,
        LifeMin = 0.4f,
        LifeMax = 0.8f
      };
      Player.P_SummitLandB = new ParticleType()
      {
        SourceChooser = chooser,
        Color = Color.get_White(),
        FadeMode = ParticleType.FadeModes.Late,
        RotationMode = ParticleType.RotationModes.Random,
        Size = 0.8f,
        SizeRange = 0.4f,
        SpeedMin = 20f,
        SpeedMax = 60f,
        SpeedMultiplier = 0.1f,
        Acceleration = Vector2.op_Multiply(Vector2.get_UnitY(), -60f),
        LifeMin = 0.8f,
        LifeMax = 1.2f,
        DirectionRange = 0.5235988f
      };
      Player.P_SummitLandC = new ParticleType()
      {
        Size = 1f,
        Color = Player.TwoDashesHairColor,
        Color2 = Color.Lerp(Player.TwoDashesHairColor, Color.get_White(), 0.5f),
        ColorMode = ParticleType.ColorModes.Blink,
        FadeMode = ParticleType.FadeModes.Late,
        SpeedMin = 40f,
        SpeedMax = 140f,
        Direction = -1.570796f,
        DirectionRange = 1.745329f,
        SpeedMultiplier = 0.1f,
        Acceleration = new Vector2(0.0f, 20f),
        LifeMin = 0.8f,
        LifeMax = 1.6f
      };
      Torch.P_OnLight = new ParticleType()
      {
        Color = Calc.HexToColor("6385FF"),
        Color2 = Color.get_White(),
        ColorMode = ParticleType.ColorModes.Blink,
        FadeMode = ParticleType.FadeModes.Late,
        LifeMin = 0.6f,
        LifeMax = 0.8f,
        Size = 1f,
        SpeedMin = 80f,
        SpeedMax = 90f,
        SpeedMultiplier = 0.03f,
        DirectionRange = 6.283185f
      };
      Cloud.P_Cloud = new ParticleType()
      {
        Source = GFX.Game["particles/cloud"],
        Color = Calc.HexToColor("2c5fcc"),
        FadeMode = ParticleType.FadeModes.None,
        LifeMin = 0.25f,
        LifeMax = 0.3f,
        Size = 0.7f,
        SizeRange = 0.25f,
        ScaleOut = true,
        Direction = 4.712389f,
        DirectionRange = 0.1745329f,
        SpeedMin = 10f,
        SpeedMax = 20f,
        SpeedMultiplier = 0.01f,
        Acceleration = new Vector2(0.0f, 90f)
      };
      Cloud.P_FragileCloud = new ParticleType(Cloud.P_Cloud);
      Cloud.P_FragileCloud.Color = Calc.HexToColor("5e22ae");
      Booster.P_Burst = new ParticleType()
      {
        Source = GFX.Game["particles/blob"],
        Color = Calc.HexToColor("2c956e"),
        FadeMode = ParticleType.FadeModes.None,
        LifeMin = 0.5f,
        LifeMax = 0.8f,
        Size = 0.7f,
        SizeRange = 0.25f,
        ScaleOut = true,
        Direction = 4.712389f,
        DirectionRange = 0.1745329f,
        SpeedMin = 10f,
        SpeedMax = 20f,
        SpeedMultiplier = 0.01f,
        Acceleration = new Vector2(0.0f, 90f)
      };
      Booster.P_BurstRed = new ParticleType(Booster.P_Burst);
      Booster.P_BurstRed.Color = Calc.HexToColor("942c3e");
      TouchSwitch.P_Fire = new ParticleType()
      {
        Source = GFX.Game["particles/fire"],
        Color = Calc.HexToColor("f141df"),
        Color2 = Color.get_White(),
        ColorMode = ParticleType.ColorModes.Fade,
        FadeMode = ParticleType.FadeModes.Late,
        Acceleration = new Vector2(0.0f, -40f),
        LifeMin = 0.8f,
        LifeMax = 1.2f,
        Size = 0.5f,
        SizeRange = 0.4f,
        Direction = -1.570796f,
        DirectionRange = 0.5235988f,
        SpeedMin = 12f,
        SpeedMax = 10f,
        SpeedMultiplier = 0.2f,
        ScaleOut = true
      };
      TouchSwitch.P_FireWhite = new ParticleType(TouchSwitch.P_Fire)
      {
        Color = Color.get_White()
      };
      Water.P_Splash = new ParticleType()
      {
        Source = GFX.Game["particles/feather"],
        Color = Water.SurfaceColor,
        FadeMode = ParticleType.FadeModes.Late,
        Acceleration = new Vector2(0.0f, 20f),
        Size = 0.8333333f,
        SizeRange = 0.3333333f,
        ScaleOut = true,
        SpeedMin = 30f,
        SpeedMax = 24f,
        SpeedMultiplier = 0.98f,
        Direction = -1.570796f,
        DirectionRange = 0.6981317f,
        RotationMode = ParticleType.RotationModes.Random,
        LifeMin = 0.35f,
        LifeMax = 0.2f
      };
      Strawberry.P_Glow = new ParticleType()
      {
        Color = Calc.HexToColor("FF8563"),
        Color2 = Calc.HexToColor("FFF4A8"),
        ColorMode = ParticleType.ColorModes.Blink,
        FadeMode = ParticleType.FadeModes.Late,
        LifeMin = 1f,
        LifeMax = 1.5f,
        Size = 1f,
        SpeedMin = 2f,
        SpeedMax = 8f,
        DirectionRange = 6.283185f
      };
      Strawberry.P_GhostGlow = new ParticleType(Strawberry.P_Glow)
      {
        Color = Calc.HexToColor("6385FF"),
        Color2 = Calc.HexToColor("72F0FF")
      };
      Strawberry.P_GoldGlow = new ParticleType(Strawberry.P_Glow)
      {
        Color = Calc.HexToColor("ffdd62"),
        Color2 = Calc.HexToColor("fff7c4")
      };
      Strawberry.P_WingsBurst = new ParticleType()
      {
        Source = GFX.Game["particles/feather"],
        Color = Color.get_White(),
        FadeMode = ParticleType.FadeModes.Late,
        Acceleration = new Vector2(0.0f, 1.2f),
        Size = 0.5f,
        SizeRange = 0.3333333f,
        SpeedMin = 36f,
        SpeedMax = 12f,
        SpeedMultiplier = 0.98f,
        Direction = -1.570796f,
        DirectionRange = 2.792527f,
        RotationMode = ParticleType.RotationModes.Random,
        LifeMin = 1f,
        LifeMax = 0.3f
      };
      BirdNPC.P_Feather = new ParticleType(Strawberry.P_WingsBurst)
      {
        Color = Calc.HexToColor("639BFF"),
        LifeMin = 2f
      };
      Key.P_Shimmer = new ParticleType()
      {
        Color = Calc.HexToColor("e2d926"),
        Color2 = Calc.HexToColor("fffeef"),
        ColorMode = ParticleType.ColorModes.Blink,
        FadeMode = ParticleType.FadeModes.Late,
        LifeMin = 0.5f,
        LifeMax = 0.8f,
        Size = 1f,
        SpeedMin = 1f,
        SpeedMax = 2f,
        DirectionRange = 6.283185f
      };
      Key.P_Insert = new ParticleType(Key.P_Shimmer)
      {
        SpeedMin = 40f,
        SpeedMax = 60f,
        SpeedMultiplier = 0.05f
      };
      Key.P_Collect = new ParticleType(Key.P_Insert);
      Refill.P_Shatter = new ParticleType()
      {
        Source = GFX.Game["particles/triangle"],
        Color = Calc.HexToColor("d3ffd4"),
        Color2 = Calc.HexToColor("85fc87"),
        FadeMode = ParticleType.FadeModes.Late,
        LifeMin = 0.25f,
        LifeMax = 0.4f,
        Size = 1f,
        Direction = 4.712389f,
        DirectionRange = 0.8726646f,
        SpeedMin = 140f,
        SpeedMax = 210f,
        SpeedMultiplier = 0.005f,
        RotationMode = ParticleType.RotationModes.Random,
        SpinMin = 1.570796f,
        SpinMax = 4.712389f,
        SpinFlippedChance = true
      };
      Refill.P_Glow = new ParticleType()
      {
        LifeMin = 0.4f,
        LifeMax = 0.6f,
        Size = 1f,
        SizeRange = 0.0f,
        DirectionRange = 6.283185f,
        SpeedMin = 4f,
        SpeedMax = 8f,
        FadeMode = ParticleType.FadeModes.Late,
        Color = Calc.HexToColor("a5fff7"),
        Color2 = Calc.HexToColor("6de081"),
        ColorMode = ParticleType.ColorModes.Blink
      };
      Refill.P_Regen = new ParticleType(Refill.P_Glow)
      {
        SpeedMin = 30f,
        SpeedMax = 40f,
        SpeedMultiplier = 0.2f,
        DirectionRange = 6.283185f
      };
      DashSwitch.P_PressA = new ParticleType()
      {
        Color = Calc.HexToColor("99e550"),
        Color2 = Calc.HexToColor("d9ffb5"),
        ColorMode = ParticleType.ColorModes.Blink,
        Size = 1f,
        SizeRange = 0.0f,
        SpeedMin = 60f,
        SpeedMax = 80f,
        LifeMin = 0.8f,
        LifeMax = 1.2f,
        DirectionRange = 0.6981317f,
        SpeedMultiplier = 0.2f
      };
      DashSwitch.P_PressB = new ParticleType(DashSwitch.P_PressA)
      {
        SpeedMin = 100f,
        SpeedMax = 110f,
        DirectionRange = 0.3490658f
      };
      DashSwitch.P_PressAMirror = new ParticleType(DashSwitch.P_PressA)
      {
        Color = Calc.HexToColor("dce34f"),
        Color2 = Calc.HexToColor("fbffaf")
      };
      DashSwitch.P_PressBMirror = new ParticleType(DashSwitch.P_PressB)
      {
        Color = Calc.HexToColor("dce34f"),
        Color2 = Calc.HexToColor("fbffaf")
      };
      FallingBlock.P_FallDustA = ParticleTypes.Dust;
      FallingBlock.P_FallDustB = new ParticleType()
      {
        Color = Color.get_White(),
        FadeMode = ParticleType.FadeModes.Late,
        Size = 1f,
        Direction = 1.570796f,
        SpeedMin = 5f,
        SpeedMax = 25f,
        LifeMin = 0.8f,
        LifeMax = 1f,
        Acceleration = Vector2.op_Multiply(Vector2.get_UnitY(), 20f)
      };
      FallingBlock.P_LandDust = new ParticleType(ParticleTypes.Dust)
      {
        Color = Color.get_White(),
        DirectionRange = 0.1745329f,
        SpeedMin = 40f,
        SpeedMax = 50f,
        SpeedMultiplier = 0.6f,
        LifeMin = 0.6f,
        LifeMax = 0.8f,
        Acceleration = Vector2.op_Multiply(Vector2.get_UnitY(), -30f),
        ScaleOut = true
      };
      SwitchGate.P_Behind = new ParticleType()
      {
        Color = Calc.HexToColor("ffeb6b"),
        Color2 = Calc.HexToColor("d39332"),
        ColorMode = ParticleType.ColorModes.Blink,
        FadeMode = ParticleType.FadeModes.Late,
        LifeMin = 1f,
        LifeMax = 1.5f,
        Size = 1f,
        SpeedMin = 5f,
        SpeedMax = 10f,
        Acceleration = new Vector2(0.0f, 6f),
        DirectionRange = 6.283185f
      };
      SwitchGate.P_Dust = new ParticleType(ParticleTypes.Dust)
      {
        LifeMin = 0.5f,
        LifeMax = 1f,
        SpeedMin = 2f,
        SpeedMax = 4f
      };
      CrumblePlatform.P_Crumble = new ParticleType()
      {
        Color = Calc.HexToColor("847E87"),
        FadeMode = ParticleType.FadeModes.Late,
        Size = 1f,
        Direction = 1.570796f,
        SpeedMin = 5f,
        SpeedMax = 25f,
        LifeMin = 0.8f,
        LifeMax = 1f,
        Acceleration = Vector2.op_Multiply(Vector2.get_UnitY(), 20f)
      };
      ZipMover.P_Scrape = new ParticleType(ParticleTypes.Dust)
      {
        LifeMin = 0.3f,
        LifeMax = 0.1f
      };
      ZipMover.P_Sparks = new ParticleType()
      {
        Color = Calc.HexToColor("fff538"),
        Size = 1f,
        SizeRange = 0.0f,
        LifeMin = 0.15f,
        LifeMax = 0.25f,
        SpeedMin = 15f,
        SpeedMax = 20f,
        DirectionRange = 1.396263f,
        FadeMode = ParticleType.FadeModes.None,
        ColorMode = ParticleType.ColorModes.Static
      };
      NPC01_Theo.P_YOLO = new ParticleType()
      {
        Color = Calc.HexToColor("8EED80"),
        Color2 = Calc.HexToColor("37946E"),
        ColorMode = ParticleType.ColorModes.Blink,
        FadeMode = ParticleType.FadeModes.Late,
        Size = 1f,
        Direction = -1.570796f,
        DirectionRange = 0.7853982f,
        SpeedMin = 10f,
        SpeedMax = 20f,
        LifeMin = 0.3f,
        LifeMax = 0.6f
      };
      ClutterSwitch.P_Pressed = new ParticleType()
      {
        Color = Color.get_White(),
        Color2 = Color.get_Aqua(),
        ColorMode = ParticleType.ColorModes.Blink,
        FadeMode = ParticleType.FadeModes.Late,
        Size = 1f,
        Direction = -1.570796f,
        DirectionRange = 0.7853982f,
        SpeedMin = 30f,
        SpeedMax = 60f,
        LifeMin = 0.8f,
        LifeMax = 1.2f,
        SpeedMultiplier = 0.2f
      };
      ClutterSwitch.P_ClutterFly = new ParticleType()
      {
        Color = Color.get_White(),
        Color2 = Color.get_Aqua(),
        ColorMode = ParticleType.ColorModes.Blink,
        FadeMode = ParticleType.FadeModes.Late,
        Size = 1f,
        Direction = -1.570796f,
        DirectionRange = 0.7853982f,
        SpeedMin = 10f,
        SpeedMax = 20f,
        LifeMin = 0.8f,
        LifeMax = 1.2f,
        SpeedMultiplier = 0.1f
      };
      BadelineOldsite.P_Vanish = new ParticleType()
      {
        Color = Calc.HexToColor("6f2399"),
        Color2 = Calc.HexToColor("ec76f7"),
        ColorMode = ParticleType.ColorModes.Blink,
        FadeMode = ParticleType.FadeModes.Late,
        Size = 1f,
        Direction = 0.0f,
        DirectionRange = 6.283185f,
        SpeedMin = 5f,
        SpeedMax = 10f,
        LifeMin = 0.6f,
        LifeMax = 1.2f,
        SpeedMultiplier = 0.3f
      };
      DreamMirror.P_Shatter = new ParticleType()
      {
        Source = GFX.Game["particles/triangle"],
        Color = Calc.HexToColor("bdeff9"),
        FadeMode = ParticleType.FadeModes.Late,
        LifeMin = 0.4f,
        LifeMax = 0.8f,
        Size = 1f,
        Direction = 4.712389f,
        DirectionRange = 0.8726646f,
        SpeedMin = 110f,
        SpeedMax = 130f,
        SpeedMultiplier = 0.05f,
        Acceleration = new Vector2(0.0f, 40f),
        RotationMode = ParticleType.RotationModes.Random,
        SpinMin = 1.570796f,
        SpinMax = 4.712389f,
        SpinFlippedChance = true
      };
      NPC03_Oshiro_Lobby.P_AppearSpark = new ParticleType()
      {
        Color = Calc.HexToColor("5FCDE4"),
        Color2 = Color.get_White(),
        ColorMode = ParticleType.ColorModes.Blink,
        FadeMode = ParticleType.FadeModes.Late,
        Size = 1f,
        LifeMin = 0.4f,
        LifeMax = 0.8f,
        SpeedMin = 40f,
        SpeedMax = 60f,
        SpeedMultiplier = 0.1f,
        Acceleration = new Vector2(0.0f, 10f)
      };
      DustStaticSpinner.P_Move = new ParticleType()
      {
        SourceChooser = chooser,
        RotationMode = ParticleType.RotationModes.Random,
        SpinMin = 0.6981317f,
        SpinMax = 1.396263f,
        SpinFlippedChance = true,
        Color = Color.get_Black(),
        FadeMode = ParticleType.FadeModes.Late,
        LifeMin = 0.4f,
        LifeMax = 0.8f,
        Size = 0.5f,
        SizeRange = 0.2f,
        SpeedMin = 5f,
        SpeedMax = 10f,
        DirectionRange = 6.283185f
      };
      StrawberrySeed.P_Burst = new ParticleType()
      {
        Source = GFX.Game["particles/shatter"],
        Color = Color.get_White(),
        Color2 = Color.get_White(),
        ColorMode = ParticleType.ColorModes.Fade,
        LifeMin = 0.3f,
        LifeMax = 0.4f,
        Size = 0.8f,
        SizeRange = 0.3f,
        ScaleOut = true,
        Direction = 0.0f,
        DirectionRange = 0.0f,
        SpeedMin = 100f,
        SpeedMax = 140f,
        SpeedMultiplier = 1E-05f,
        RotationMode = ParticleType.RotationModes.SameAsDirection
      };
      SummitGem.P_Shatter = new ParticleType()
      {
        Source = GFX.Game["particles/triangle"],
        ColorMode = ParticleType.ColorModes.Static,
        FadeMode = ParticleType.FadeModes.Late,
        LifeMin = 0.25f,
        LifeMax = 0.4f,
        Size = 1f,
        Direction = 4.712389f,
        DirectionRange = 0.8726646f,
        SpeedMin = 140f,
        SpeedMax = 210f,
        SpeedMultiplier = 0.005f,
        RotationMode = ParticleType.RotationModes.Random,
        SpinMin = 1.570796f,
        SpinMax = 4.712389f,
        SpinFlippedChance = true
      };
      Payphone.P_Snow = new ParticleType(ParticleTypes.Dust)
      {
        LifeMin = 1f,
        LifeMax = 1.6f,
        Direction = -1.570796f,
        Acceleration = new Vector2(0.0f, 12f),
        SpeedMin = 20f,
        SpeedMax = 36f
      };
      Payphone.P_SnowB = new ParticleType(Payphone.P_Snow)
      {
        SpeedMin = 8f,
        SpeedMax = 18f
      };
      TempleMirrorPortal.P_CurtainDrop = new ParticleType()
      {
        Color = Color.get_Red(),
        FadeMode = ParticleType.FadeModes.Late,
        Size = 1f,
        LifeMin = 0.6f,
        LifeMax = 2f,
        SpeedMin = 5f,
        SpeedMax = 10f,
        Direction = -1.570796f,
        DirectionRange = 0.7853982f
      };
      LightBeam.P_Glow = new ParticleType()
      {
        Source = GFX.Game["particles/rect"],
        Color = Color.op_Multiply(Calc.HexToColor("fcf8de"), 0.4f),
        FadeMode = ParticleType.FadeModes.InAndOut,
        Size = 1f,
        SpeedMin = 16f,
        SpeedMax = 20f,
        LifeMin = 1.4f,
        LifeMax = 2.8f,
        RotationMode = ParticleType.RotationModes.SameAsDirection
      };
      BadelineBoost.P_Move = new ParticleType()
      {
        Source = GFX.Game["particles/shard"],
        Color = Color.get_White(),
        Color2 = Calc.HexToColor("e0a8d8"),
        ColorMode = ParticleType.ColorModes.Blink,
        FadeMode = ParticleType.FadeModes.Late,
        RotationMode = ParticleType.RotationModes.Random,
        Size = 0.8f,
        SizeRange = 0.4f,
        SpeedMin = 20f,
        SpeedMax = 40f,
        SpeedMultiplier = 0.2f,
        LifeMin = 0.4f,
        LifeMax = 0.6f,
        DirectionRange = 6.283185f
      };
      BadelineBoost.P_Ambience = new ParticleType()
      {
        Color = Calc.HexToColor("f78ae7"),
        Color2 = Calc.HexToColor("ffccf7"),
        ColorMode = ParticleType.ColorModes.Blink,
        FadeMode = ParticleType.FadeModes.Late,
        Size = 1f,
        DirectionRange = 6.283185f,
        SpeedMin = 20f,
        SpeedMax = 40f,
        SpeedMultiplier = 0.2f,
        LifeMin = 0.6f,
        LifeMax = 1f
      };
      FlyFeather.P_Collect = new ParticleType()
      {
        Source = GFX.Game["particles/feather"],
        Color = Player.FlyPowerHairColor,
        Color2 = Calc.HexToColor("fff20f"),
        ColorMode = ParticleType.ColorModes.Blink,
        FadeMode = ParticleType.FadeModes.Late,
        RotationMode = ParticleType.RotationModes.Random,
        Size = 1f,
        LifeMin = 0.6f,
        LifeMax = 0.9f,
        SpeedMin = 60f,
        SpeedMax = 70f,
        SpeedMultiplier = 0.1f,
        DirectionRange = 6.283185f,
        SpinFlippedChance = true,
        SpinMin = 0.5235988f,
        SpinMax = 1.396263f
      };
      FlyFeather.P_Boost = new ParticleType(FlyFeather.P_Collect)
      {
        DirectionRange = 1.396263f,
        SpeedMin = 20f,
        SpeedMax = 80f,
        SpeedMultiplier = 0.2f
      };
      FlyFeather.P_Flying = new ParticleType()
      {
        Color = Player.FlyPowerHairColor,
        Color2 = Calc.HexToColor("fff20f"),
        ColorMode = ParticleType.ColorModes.Blink,
        FadeMode = ParticleType.FadeModes.Late,
        Size = 1f,
        LifeMin = 0.4f,
        LifeMax = 0.8f,
        SpeedMin = 60f,
        SpeedMax = 70f,
        SpeedMultiplier = 0.25f,
        DirectionRange = 1.047198f
      };
      FlyFeather.P_Respawn = new ParticleType(Refill.P_Regen)
      {
        Color = Calc.HexToColor("ffdca4"),
        Color2 = Calc.HexToColor("ffe95e")
      };
      CrushBlock.P_Impact = new ParticleType(ParticleTypes.Dust)
      {
        LifeMin = 0.8f,
        LifeMax = 1.6f,
        SpeedMin = 8f,
        SpeedMax = 12f,
        Size = 1.2f,
        SizeRange = 0.5f
      };
      CrushBlock.P_Crushing = new ParticleType()
      {
        Source = GFX.Game["particles/rect"],
        Color = Calc.HexToColor("ff66e2"),
        Color2 = Calc.HexToColor("68fcff"),
        ColorMode = ParticleType.ColorModes.Blink,
        RotationMode = ParticleType.RotationModes.SameAsDirection,
        Size = 0.5f,
        SizeRange = 0.2f,
        DirectionRange = 0.5235988f,
        FadeMode = ParticleType.FadeModes.Late,
        LifeMin = 0.5f,
        LifeMax = 1.2f,
        SpeedMin = 30f,
        SpeedMax = 50f,
        SpeedMultiplier = 0.4f,
        Acceleration = new Vector2(0.0f, 10f)
      };
      CrushBlock.P_Activate = new ParticleType()
      {
        Source = GFX.Game["particles/rect"],
        Color = Calc.HexToColor("5fcde4"),
        Color2 = Color.get_White(),
        ColorMode = ParticleType.ColorModes.Blink,
        RotationMode = ParticleType.RotationModes.SameAsDirection,
        Size = 0.5f,
        SizeRange = 0.2f,
        DirectionRange = 0.5235988f,
        FadeMode = ParticleType.FadeModes.Late,
        LifeMin = 0.5f,
        LifeMax = 1.1f,
        SpeedMin = 60f,
        SpeedMax = 100f,
        SpeedMultiplier = 0.2f
      };
      Bumper.P_Ambience = new ParticleType()
      {
        Source = GFX.Game["particles/rect"],
        Color = Calc.HexToColor("47b5cc"),
        Color2 = Calc.HexToColor("c4f4ff"),
        ColorMode = ParticleType.ColorModes.Blink,
        FadeMode = ParticleType.FadeModes.InAndOut,
        Size = 0.5f,
        SizeRange = 0.2f,
        RotationMode = ParticleType.RotationModes.SameAsDirection,
        LifeMin = 0.2f,
        LifeMax = 0.4f,
        SpeedMin = 10f,
        SpeedMax = 20f,
        DirectionRange = 0.5235988f
      };
      Bumper.P_FireAmbience = new ParticleType(Bumper.P_Ambience)
      {
        Color = Calc.HexToColor("FFA808"),
        ColorMode = ParticleType.ColorModes.Static,
        DirectionRange = 0.8726646f,
        LifeMin = 0.3f,
        LifeMax = 0.6f
      };
      Bumper.P_Launch = new ParticleType()
      {
        Source = GFX.Game["particles/rect"],
        Color = Calc.HexToColor("47b5cc"),
        Color2 = Calc.HexToColor("c4f4ff"),
        ColorMode = ParticleType.ColorModes.Blink,
        FadeMode = ParticleType.FadeModes.Late,
        Size = 0.5f,
        SizeRange = 0.2f,
        RotationMode = ParticleType.RotationModes.Random,
        LifeMin = 0.6f,
        LifeMax = 1.2f,
        SpeedMin = 40f,
        SpeedMax = 140f,
        SpeedMultiplier = 0.1f,
        Acceleration = new Vector2(0.0f, 10f),
        DirectionRange = 0.6981317f
      };
      Bumper.P_FireHit = new ParticleType(Bumper.P_Launch)
      {
        Color = Calc.HexToColor("FFA808"),
        ColorMode = ParticleType.ColorModes.Static,
        Acceleration = new Vector2(0.0f, -40f),
        DirectionRange = 1.396263f,
        SpeedMin = 30f,
        SpeedMax = 100f,
        LifeMin = 0.8f,
        LifeMax = 1.6f
      };
      SwapBlock.P_Move = new ParticleType()
      {
        Size = 1f,
        Color = Calc.HexToColor("fbf236"),
        Color2 = Calc.HexToColor("6abe30"),
        ColorMode = ParticleType.ColorModes.Blink,
        DirectionRange = 0.6981317f,
        SpeedMin = 10f,
        SpeedMax = 20f,
        SpeedMultiplier = 0.3f,
        LifeMin = 0.3f,
        LifeMax = 0.5f
      };
      MoveBlock.P_Activate = new ParticleType()
      {
        Size = 1f,
        Color = Color.get_Black(),
        FadeMode = ParticleType.FadeModes.Late,
        DirectionRange = 0.3490658f,
        LifeMin = 0.4f,
        LifeMax = 0.6f,
        SpeedMin = 20f,
        SpeedMax = 40f,
        SpeedMultiplier = 0.25f
      };
      MoveBlock.P_Break = new ParticleType(MoveBlock.P_Activate);
      MoveBlock.P_Move = new ParticleType(MoveBlock.P_Activate);
      Seeker.P_BreakOut = new ParticleType()
      {
        Size = 1f,
        Color = Calc.HexToColor("643e73"),
        Color2 = Calc.HexToColor("3e2854"),
        ColorMode = ParticleType.ColorModes.Choose,
        FadeMode = ParticleType.FadeModes.Late,
        LifeMin = 0.3f,
        LifeMax = 0.6f,
        SpeedMin = 10f,
        SpeedMax = 30f,
        DirectionRange = 1.396263f
      };
      Seeker.P_Attack = new ParticleType()
      {
        Size = 1f,
        Color = Calc.HexToColor("99e550"),
        Color2 = Calc.HexToColor("ddffbc"),
        ColorMode = ParticleType.ColorModes.Blink,
        FadeMode = ParticleType.FadeModes.Late,
        LifeMin = 0.6f,
        LifeMax = 1.2f,
        SpeedMin = 20f,
        SpeedMax = 40f,
        SpeedMultiplier = 0.4f,
        DirectionRange = 1.745329f
      };
      Seeker.P_Stomp = new ParticleType(Seeker.P_Attack)
      {
        Direction = -1.570796f
      };
      Seeker.P_HitWall = new ParticleType()
      {
        Size = 1f,
        Color = Calc.HexToColor("99e550"),
        Color2 = Calc.HexToColor("ddffbc"),
        ColorMode = ParticleType.ColorModes.Blink,
        FadeMode = ParticleType.FadeModes.Late,
        LifeMin = 0.6f,
        LifeMax = 1.2f,
        SpeedMin = 30f,
        SpeedMax = 60f,
        SpeedMultiplier = 0.4f,
        DirectionRange = 1.745329f
      };
      Seeker.P_Regen = new ParticleType()
      {
        Color = Calc.HexToColor("cbdbfc"),
        Color2 = Calc.HexToColor("575fd9"),
        ColorMode = ParticleType.ColorModes.Blink,
        FadeMode = ParticleType.FadeModes.Late,
        Size = 1f,
        LifeMin = 0.4f,
        LifeMax = 1.2f,
        SpeedMin = 20f,
        SpeedMax = 100f,
        SpeedMultiplier = 0.4f,
        DirectionRange = 1.047198f
      };
      WaterInteraction.P_Drip = new ParticleType()
      {
        Color = Water.SurfaceColor,
        Color2 = Color.get_White(),
        ColorMode = ParticleType.ColorModes.Blink,
        FadeMode = ParticleType.FadeModes.Late,
        Size = 1f,
        LifeMin = 0.4f,
        LifeMax = 0.8f,
        SpeedMin = 10f,
        SpeedMax = 12f,
        SpeedMultiplier = 2f,
        Direction = 1.570796f,
        DirectionRange = 0.05f
      };
      FireBarrier.P_Deactivate = new ParticleType()
      {
        Color = RisingLava.Hot[0],
        Color2 = RisingLava.Hot[2],
        ColorMode = ParticleType.ColorModes.Blink,
        FadeMode = ParticleType.FadeModes.Late,
        Acceleration = new Vector2(0.0f, -20f),
        Size = 1f,
        LifeMin = 0.3f,
        LifeMax = 0.8f,
        SpeedMin = 5f,
        SpeedMax = 25f,
        DirectionRange = 0.5235988f
      };
      IceBlock.P_Deactivate = new ParticleType(FireBarrier.P_Deactivate)
      {
        Color = RisingLava.Cold[0],
        Color2 = RisingLava.Cold[2],
        Acceleration = new Vector2(0.0f, 15f)
      };
      BounceBlock.P_Reform = new ParticleType()
      {
        Color = Color.get_White(),
        FadeMode = ParticleType.FadeModes.Late,
        SpeedMin = 20f,
        SpeedMax = 50f,
        SpeedMultiplier = 0.1f,
        Size = 1f,
        LifeMin = 0.4f,
        LifeMax = 0.8f,
        DirectionRange = 0.5235988f
      };
      BounceBlock.P_FireBreak = new ParticleType(FireBarrier.P_Deactivate)
      {
        SpeedMin = 40f,
        SpeedMax = 80f,
        SpeedMultiplier = 0.1f
      };
      BounceBlock.P_IceBreak = new ParticleType(IceBlock.P_Deactivate);
      FireBall.P_IceBreak = new ParticleType(IceBlock.P_Deactivate)
      {
        SpeedMin = 20f,
        SpeedMax = 50f,
        SpeedMultiplier = 0.2f,
        DirectionRange = 6.283185f
      };
      FireBall.P_FireTrail = new ParticleType(FireBarrier.P_Deactivate)
      {
        Acceleration = new Vector2(0.0f, -5f),
        LifeMin = 0.6f,
        LifeMax = 1.2f,
        SpeedMin = 4f,
        SpeedMax = 8f,
        DirectionRange = 6.283185f
      };
      FireBall.P_IceTrail = new ParticleType(IceBlock.P_Deactivate)
      {
        Acceleration = new Vector2(0.0f, 5f),
        LifeMin = 0.6f,
        LifeMax = 1.2f,
        SpeedMin = 4f,
        SpeedMax = 8f,
        DirectionRange = 6.283185f
      };
      CrystalDebris.P_Dust = new ParticleType()
      {
        Color = Calc.HexToColor("FFFFFF"),
        Color2 = Calc.HexToColor("ff77a8"),
        ColorMode = ParticleType.ColorModes.Blink,
        FadeMode = ParticleType.FadeModes.Late,
        LifeMin = 0.25f,
        LifeMax = 0.6f,
        Size = 1f,
        SpeedMin = 2f,
        SpeedMax = 8f,
        DirectionRange = 6.283185f
      };
      FinalBossShot.P_Trail = new ParticleType()
      {
        Size = 1f,
        Color = Calc.HexToColor("ffced5"),
        Color2 = Calc.HexToColor("ff4f7d"),
        ColorMode = ParticleType.ColorModes.Blink,
        FadeMode = ParticleType.FadeModes.Late,
        SpeedMin = 10f,
        SpeedMax = 30f,
        DirectionRange = 0.6981317f,
        LifeMin = 0.3f,
        LifeMax = 0.6f
      };
      FinalBossBeam.P_Dissipate = new ParticleType()
      {
        Color = Calc.HexToColor("e60022"),
        Size = 1f,
        FadeMode = ParticleType.FadeModes.Late,
        SpeedMin = 15f,
        SpeedMax = 30f,
        DirectionRange = 1.047198f,
        LifeMin = 0.3f,
        LifeMax = 0.6f
      };
      FinalBossMovingBlock.P_Stop = new ParticleType()
      {
        Color = Calc.HexToColor("ffe0d3"),
        Size = 1f,
        FadeMode = ParticleType.FadeModes.Late,
        SpeedMin = 40f,
        SpeedMax = 100f,
        SpeedMultiplier = 0.1f,
        DirectionRange = 0.6981317f,
        LifeMin = 0.5f,
        LifeMax = 1.2f
      };
      FinalBossMovingBlock.P_Break = new ParticleType()
      {
        Color = Calc.HexToColor("ffe0d3"),
        Size = 1f,
        FadeMode = ParticleType.FadeModes.Late,
        SpeedMin = 20f,
        SpeedMax = 40f,
        SpeedMultiplier = 0.1f,
        DirectionRange = 0.6981317f,
        LifeMin = 0.4f,
        LifeMax = 0.8f
      };
      FinalBoss.P_Burst = new ParticleType()
      {
        Color = Calc.HexToColor("ff00b0"),
        Color2 = Calc.HexToColor("ff84d9"),
        ColorMode = ParticleType.ColorModes.Blink,
        FadeMode = ParticleType.FadeModes.Late,
        Size = 1f,
        DirectionRange = 1.047198f,
        SpeedMin = 40f,
        SpeedMax = 100f,
        SpeedMultiplier = 0.2f,
        LifeMin = 0.4f,
        LifeMax = 0.8f
      };
      Cassette.P_Shine = new ParticleType()
      {
        Size = 1f,
        Color = Calc.HexToColor("49aaf0"),
        Color2 = Calc.HexToColor("f049be"),
        ColorMode = ParticleType.ColorModes.Blink,
        FadeMode = ParticleType.FadeModes.InAndOut,
        DirectionRange = 6.283185f,
        SpeedMin = 5f,
        SpeedMax = 10f,
        LifeMin = 0.6f,
        LifeMax = 1f
      };
      TheoCrystal.P_Impact = new ParticleType()
      {
        Color = Calc.HexToColor("cbdbfc"),
        Size = 1f,
        FadeMode = ParticleType.FadeModes.Late,
        DirectionRange = 1.745329f,
        SpeedMin = 10f,
        SpeedMax = 20f,
        SpeedMultiplier = 0.1f,
        LifeMin = 0.3f,
        LifeMax = 0.8f
      };
      HeartGem.P_BlueShine = new ParticleType()
      {
        Size = 1f,
        Color = Calc.HexToColor("5caefa"),
        Color2 = Color.get_White(),
        ColorMode = ParticleType.ColorModes.Blink,
        FadeMode = ParticleType.FadeModes.InAndOut,
        Direction = -1.570796f,
        DirectionRange = 1.396263f,
        SpeedMin = 5f,
        SpeedMax = 10f,
        LifeMin = 0.6f,
        LifeMax = 1f
      };
      HeartGem.P_RedShine = new ParticleType(HeartGem.P_BlueShine)
      {
        Color = Calc.HexToColor("ff2457")
      };
      HeartGem.P_GoldShine = new ParticleType(HeartGem.P_BlueShine)
      {
        Color = Calc.HexToColor("fffc24")
      };
      ForsakenCitySatellite.Particles.Clear();
      using (Dictionary<string, Color>.Enumerator enumerator = ForsakenCitySatellite.Colors.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          KeyValuePair<string, Color> current = enumerator.Current;
          ForsakenCitySatellite.Particles.Add(current.Key, new ParticleType(Player.P_DashA)
          {
            Color = current.Value,
            Color2 = Color.Lerp(current.Value, Color.get_White(), 0.5f)
          });
        }
      }
      HeartGemDoor.P_Shimmer = new ParticleType()
      {
        Size = 1f,
        Color = Calc.HexToColor("baffff"),
        Color2 = Calc.HexToColor("5abce2"),
        ColorMode = ParticleType.ColorModes.Blink,
        FadeMode = ParticleType.FadeModes.Late,
        SpeedMin = 2f,
        SpeedMax = 5f,
        DirectionRange = 1.047198f,
        LifeMin = 1.4f,
        LifeMax = 2f
      };
      HeartGemDoor.P_Slice = new ParticleType()
      {
        Size = 1f,
        Color = Color.get_White(),
        Color2 = Color.op_Multiply(Color.get_White(), 0.65f),
        ColorMode = ParticleType.ColorModes.Choose,
        FadeMode = ParticleType.FadeModes.Late,
        SpeedMin = 0.0f,
        SpeedMax = 30f,
        Acceleration = Vector2.op_Multiply(Vector2.get_UnitY(), 20f),
        DirectionRange = 0.0f,
        Direction = -1.570796f,
        LifeMin = 0.4f,
        LifeMax = 1.8f
      };
    }
  }
}
