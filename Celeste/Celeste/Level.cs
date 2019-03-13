// Decompiled with JetBrains decompiler
// Type: Celeste.Level
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Celeste.Editor;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Celeste
{
  public class Level : Scene, IOverlayHandler
  {
    private static int AssistSpeedSnapshotValue = -1;
    public float NextTransitionDuration = 0.65f;
    public Color BackgroundColor = Color.get_Black();
    private Color flashColor = Color.get_White();
    public float Zoom = 1f;
    public float ZoomTarget = 1f;
    public bool CanRetry = true;
    private bool updateHair = true;
    public bool Completed;
    public bool NewLevel;
    public bool TimerStarted;
    public bool TimerStopped;
    public Session Session;
    public Vector2? StartPosition;
    public bool DarkRoom;
    public Player.IntroTypes LastIntroType;
    public bool InCredits;
    public VirtualMap<char> SolidsData;
    public VirtualMap<char> BgData;
    public const float DefaultTransitionDuration = 0.65f;
    public ScreenWipe Wipe;
    private Coroutine transition;
    private Coroutine saving;
    public FormationBackdrop FormationBackdrop;
    public SolidTiles SolidTiles;
    public BackgroundTiles BgTiles;
    public BackdropRenderer Background;
    public BackdropRenderer Foreground;
    public GameplayRenderer GameplayRenderer;
    public HudRenderer HudRenderer;
    public LightingRenderer Lighting;
    public DisplacementRenderer Displacement;
    public BloomRenderer Bloom;
    public TileGrid FgTilesLightMask;
    public ParticleSystem Particles;
    public ParticleSystem ParticlesBG;
    public ParticleSystem ParticlesFG;
    public HiresSnow HiresSnow;
    public TotalStrawberriesDisplay strawberriesDisplay;
    private WindController windController;
    public const float CameraOffsetXInterval = 48f;
    public const float CameraOffsetYInterval = 32f;
    public Camera Camera;
    public Level.CameraLockModes CameraLockMode;
    public Vector2 CameraOffset;
    public float CameraUpwardMaxY;
    private Vector2 shakeVector;
    private Vector2 shakeDirection;
    private int lastDirectionalShake;
    private float shakeTimer;
    private Vector2 cameraPreShake;
    public float ScreenPadding;
    private float flash;
    private bool doFlash;
    private bool flashDrawPlayer;
    private float glitchTimer;
    private float glitchSeed;
    public Vector2 ZoomFocusPoint;
    private string lastColorGrade;
    private float colorGradeEase;
    public Vector2 Wind;
    public float WindSine;
    public float WindSineTimer;
    public bool Frozen;
    public bool PauseLock;
    public bool PauseMainMenuOpen;
    private bool wasPaused;
    public bool SaveQuitDisabled;
    public bool InCutscene;
    public bool SkippingCutscene;
    private Coroutine skipCoroutine;
    private Action<Level> onCutsceneSkip;
    private bool onCutsceneSkipFadeIn;
    private bool endingChapterAfterCutscene;
    public static EventInstance DialogSnapshot;
    private static EventInstance PauseSnapshot;
    private static EventInstance AssistSpeedSnapshot;
    public Pathfinder Pathfinder;
    public PlayerDeadBody RetryPlayerCorpse;
    public float BaseLightingAlpha;
    public bool InSpace;
    public bool HasCassetteBlocks;
    public float CassetteBlockTempo;
    public int CassetteBlockBeats;
    public Random HiccupRandom;
    private Session.CoreModes coreMode;

    public Vector2 LevelOffset
    {
      get
      {
        Rectangle bounds = this.Bounds;
        double left = (double) ((Rectangle) ref bounds).get_Left();
        bounds = this.Bounds;
        double top = (double) ((Rectangle) ref bounds).get_Top();
        return new Vector2((float) left, (float) top);
      }
    }

    public Point LevelSolidOffset
    {
      get
      {
        Rectangle bounds = this.Bounds;
        int num1 = ((Rectangle) ref bounds).get_Left() / 8 - this.TileBounds.X;
        bounds = this.Bounds;
        int num2 = ((Rectangle) ref bounds).get_Top() / 8 - this.TileBounds.Y;
        return new Point(num1, num2);
      }
    }

    public Rectangle TileBounds
    {
      get
      {
        return this.Session.MapData.TileBounds;
      }
    }

    public bool Transitioning
    {
      get
      {
        return this.transition != null;
      }
    }

    public Vector2 ShakeVector
    {
      get
      {
        return this.shakeVector;
      }
    }

    public float VisualWind
    {
      get
      {
        return (float) this.Wind.X + this.WindSine;
      }
    }

    public bool FrozenOrPaused
    {
      get
      {
        if (!this.Frozen)
          return this.Paused;
        return true;
      }
    }

    public bool CanPause
    {
      get
      {
        Player entity = this.Tracker.GetEntity<Player>();
        if (entity != null && !entity.Dead && (!this.wasPaused && !this.Paused) && (!this.PauseLock && !this.SkippingCutscene && (!this.Transitioning && this.Wipe == null)))
          return !UserIO.Saving;
        return false;
      }
    }

    public Overlay Overlay { get; set; }

    public bool ShowHud
    {
      get
      {
        if (this.Completed)
          return false;
        if (this.Paused)
          return true;
        if (this.Tracker.GetEntity<Textbox>() == null && this.Tracker.GetEntity<MiniTextbox>() == null && !this.Frozen)
          return !this.InCutscene;
        return false;
      }
    }

    public override void Begin()
    {
      GameplayBuffers.Create();
      Distort.WaterAlpha = 1f;
      Distort.WaterSineDirection = 1f;
      Audio.MusicUnderwater = false;
      Audio.EndSnapshot(Level.DialogSnapshot);
      base.Begin();
    }

    public override void End()
    {
      base.End();
      this.Foreground.Ended((Scene) this);
      this.Background.Ended((Scene) this);
      this.EndPauseEffects();
      Audio.BusStopAll("bus:/gameplay_sfx", false);
      Audio.MusicUnderwater = false;
      Audio.SetAmbience((string) null, true);
      Audio.SetAltMusic((string) null);
      Audio.EndSnapshot(Level.DialogSnapshot);
      Audio.EndSnapshot(Level.AssistSpeedSnapshot);
      Level.AssistSpeedSnapshot = (EventInstance) null;
      Level.AssistSpeedSnapshotValue = -1;
      GameplayBuffers.Unload();
      ClutterBlockGenerator.Dispose();
      Engine.TimeRateB = 1f;
    }

    public void LoadLevel(Player.IntroTypes playerIntro, bool isFromLoader = false)
    {
      this.TimerStopped = false;
      this.LastIntroType = playerIntro;
      this.Background.Fade = 0.0f;
      this.CanRetry = true;
      this.ScreenPadding = 0.0f;
      this.Displacement.Enabled = true;
      this.PauseLock = false;
      this.Frozen = false;
      this.CameraLockMode = Level.CameraLockModes.None;
      this.RetryPlayerCorpse = (PlayerDeadBody) null;
      this.FormationBackdrop.Display = false;
      this.SaveQuitDisabled = false;
      this.lastColorGrade = this.Session.ColorGrade;
      this.colorGradeEase = 0.0f;
      this.HasCassetteBlocks = false;
      this.CassetteBlockTempo = 1f;
      this.CassetteBlockBeats = 2;
      bool flag1 = false;
      if (this.HiccupRandom == null)
        this.HiccupRandom = new Random(this.Session.Area.ID * 77 + (int) this.Session.Area.Mode * 999);
      Calc.PushRandom(this.Session.LevelData.LoadSeed);
      MapData mapData = this.Session.MapData;
      LevelData levelData = this.Session.LevelData;
      Vector2 vector2_1;
      ((Vector2) ref vector2_1).\u002Ector((float) ((Rectangle) ref levelData.Bounds).get_Left(), (float) ((Rectangle) ref levelData.Bounds).get_Top());
      bool flag2 = playerIntro != Player.IntroTypes.Fall || levelData.Name == "0";
      this.DarkRoom = levelData.Dark && !this.Session.GetFlag("ignore_darkness_" + levelData.Name);
      this.Zoom = 1f;
      if (this.Session.Audio == null)
        this.Session.Audio = AreaData.Get(this.Session).Mode[(int) this.Session.Area.Mode].AudioState.Clone();
      if (!levelData.DelayAltMusic)
        Audio.SetAltMusic(Sfxs.EventnameByHandle(levelData.AltMusic));
      if (levelData.Music.Length > 0)
        this.Session.Audio.Music.Event = Sfxs.EventnameByHandle(levelData.Music);
      if (!AreaData.GetMode(this.Session.Area).IgnoreLevelAudioLayerData)
      {
        for (int index = 0; index < 4; ++index)
          this.Session.Audio.Music.Layer(index + 1, levelData.MusicLayers[index]);
      }
      if (levelData.MusicProgress >= 0)
        this.Session.Audio.Music.Progress = levelData.MusicProgress;
      this.Session.Audio.Music.Layer(6, levelData.MusicWhispers);
      if (levelData.Ambience.Length > 0)
        this.Session.Audio.Ambience.Event = Sfxs.EventnameByHandle(levelData.Ambience);
      if (levelData.AmbienceProgress >= 0)
        this.Session.Audio.Ambience.Progress = levelData.AmbienceProgress;
      this.Session.Audio.Apply();
      this.CoreMode = this.Session.CoreMode;
      this.NewLevel = !this.Session.LevelFlags.Contains(levelData.Name);
      if (flag2)
      {
        if (!this.Session.LevelFlags.Contains(levelData.Name))
          this.Session.FurthestSeenLevel = levelData.Name;
        this.Session.LevelFlags.Add(levelData.Name);
        this.Session.UpdateLevelStartDashes();
      }
      Vector2? nullable = new Vector2?();
      this.CameraOffset = Vector2.op_Multiply(new Vector2(48f, 32f), levelData.CameraOffset);
      this.Entities.FindFirst<WindController>()?.RemoveSelf();
      this.Add((Entity) (this.windController = new WindController(levelData.WindPattern)));
      if (playerIntro != Player.IntroTypes.Transition)
        this.windController.SetStartPattern();
      if (levelData.Underwater)
        this.Add((Entity) new Water(vector2_1, false, false, (float) levelData.Bounds.Width, (float) levelData.Bounds.Height));
      this.InSpace = levelData.Space;
      if (this.InSpace)
        this.Add((Entity) new SpaceController());
      if (levelData.Name == "-1" && this.Session.Area.ID == 0 && !SaveData.Instance.CheatMode)
        this.Add((Entity) new UnlockEverythingThingy());
      int index1 = 0;
      List<EntityID> entityIdList1 = new List<EntityID>();
      Player entity1 = this.Tracker.GetEntity<Player>();
      if (entity1 != null)
      {
        foreach (Follower follower in entity1.Leader.Followers)
          entityIdList1.Add(follower.ParentEntityID);
      }
      foreach (EntityData entity2 in levelData.Entities)
      {
        int id = entity2.ID;
        EntityID entityId1 = new EntityID(levelData.Name, id);
        if (!this.Session.DoNotLoad.Contains(entityId1) && !entityIdList1.Contains(entityId1))
        {
          switch (entity2.Name)
          {
            case "SoundTest3d":
              this.Add((Entity) new _3dSoundTest(entity2, vector2_1));
              continue;
            case "SummitBackgroundManager":
              this.Add((Entity) new SummitBackgroundManager(entity2, vector2_1));
              continue;
            case "badelineBoost":
              this.Add((Entity) new BadelineBoost(entity2, vector2_1));
              continue;
            case "bigSpinner":
              this.Add((Entity) new Bumper(entity2, vector2_1));
              continue;
            case "bigWaterfall":
              this.Add((Entity) new BigWaterfall(entity2, vector2_1));
              continue;
            case "bird":
              this.Add((Entity) new BirdNPC(entity2, vector2_1));
              continue;
            case "birdForsakenCityGem":
              this.Add((Entity) new ForsakenCitySatellite(entity2, vector2_1));
              continue;
            case "blackGem":
              if (!this.Session.HeartGem || this.Session.Area.Mode != AreaMode.Normal)
              {
                this.Add((Entity) new HeartGem(entity2, vector2_1));
                continue;
              }
              continue;
            case "blockField":
              this.Add((Entity) new BlockField(entity2, vector2_1));
              continue;
            case "bonfire":
              this.Add((Entity) new Bonfire(entity2, vector2_1));
              continue;
            case "booster":
              this.Add((Entity) new Booster(entity2, vector2_1));
              continue;
            case "bounceBlock":
              this.Add((Entity) new BounceBlock(entity2, vector2_1));
              continue;
            case "bridge":
              this.Add((Entity) new Bridge(entity2, vector2_1));
              continue;
            case "bridgeFixed":
              this.Add((Entity) new BridgeFixed(entity2, vector2_1));
              continue;
            case "cassette":
              if (!this.Session.Cassette)
              {
                this.Add((Entity) new Cassette(entity2, vector2_1));
                continue;
              }
              continue;
            case "cassetteBlock":
              CassetteBlock cassetteBlock = new CassetteBlock(entity2, vector2_1);
              this.Add((Entity) cassetteBlock);
              this.HasCassetteBlocks = true;
              if ((double) this.CassetteBlockTempo == 1.0)
                this.CassetteBlockTempo = cassetteBlock.Tempo;
              this.CassetteBlockBeats = Math.Max(cassetteBlock.Index + 1, this.CassetteBlockBeats);
              if (!flag1)
              {
                flag1 = true;
                if (this.Tracker.GetEntity<CassetteBlockManager>() == null && this.ShouldCreateCassetteManager)
                {
                  this.Add((Entity) new CassetteBlockManager());
                  continue;
                }
                continue;
              }
              continue;
            case "chaserBarrier":
              this.Add((Entity) new ChaserBarrier(entity2, vector2_1));
              continue;
            case "checkpoint":
              if (flag2)
              {
                this.Add((Entity) new Checkpoint(entity2, vector2_1));
                nullable = new Vector2?(Vector2.op_Addition(entity2.Position, vector2_1));
                continue;
              }
              continue;
            case "cliffflag":
              this.Add((Entity) new CliffFlags(entity2, vector2_1));
              continue;
            case "cliffside_flag":
              this.Add((Entity) new CliffsideWindFlag(entity2, vector2_1));
              continue;
            case "clothesline":
              this.Add((Entity) new Clothesline(entity2, vector2_1));
              continue;
            case "cloud":
              this.Add((Entity) new Cloud(entity2, vector2_1));
              continue;
            case "clutterCabinet":
              this.Add((Entity) new ClutterCabinet(entity2, vector2_1));
              continue;
            case "clutterDoor":
              this.Add((Entity) new ClutterDoor(entity2, vector2_1, this.Session));
              continue;
            case "cobweb":
              this.Add((Entity) new Cobweb(entity2, vector2_1));
              continue;
            case "colorSwitch":
              this.Add((Entity) new ClutterSwitch(entity2, vector2_1));
              continue;
            case "conditionBlock":
              Level.ConditionBlockModes conditionBlockModes = entity2.Enum<Level.ConditionBlockModes>("condition", Level.ConditionBlockModes.Key);
              EntityID none1 = EntityID.None;
              string[] strArray1 = entity2.Attr("conditionID", "").Split(':');
              none1.Level = strArray1[0];
              none1.ID = Convert.ToInt32(strArray1[1]);
              bool flag3;
              switch (conditionBlockModes)
              {
                case Level.ConditionBlockModes.Key:
                  flag3 = this.Session.DoNotLoad.Contains(none1);
                  break;
                case Level.ConditionBlockModes.Button:
                  flag3 = this.Session.GetFlag(DashSwitch.GetFlagName(none1));
                  break;
                case Level.ConditionBlockModes.Strawberry:
                  flag3 = this.Session.Strawberries.Contains(none1);
                  break;
                default:
                  throw new Exception("Condition type not supported!");
              }
              if (flag3)
              {
                this.Add((Entity) new ExitBlock(entity2, vector2_1));
                continue;
              }
              continue;
            case "coreMessage":
              this.Add((Entity) new CoreMessage(entity2, vector2_1));
              continue;
            case "coreModeToggle":
              this.Add((Entity) new CoreModeToggle(entity2, vector2_1));
              continue;
            case "coverupWall":
              this.Add((Entity) new CoverupWall(entity2, vector2_1));
              continue;
            case "crumbleBlock":
              this.Add((Entity) new CrumblePlatform(entity2, vector2_1));
              continue;
            case "crushBlock":
              this.Add((Entity) new CrushBlock(entity2, vector2_1));
              continue;
            case "darkChaser":
              this.Add((Entity) new BadelineOldsite(entity2, vector2_1, index1));
              ++index1;
              continue;
            case "dashBlock":
              this.Add((Entity) new DashBlock(entity2, vector2_1, entityId1));
              continue;
            case "dashSwitchH":
            case "dashSwitchV":
              this.Add((Entity) DashSwitch.Create(entity2, vector2_1, entityId1));
              continue;
            case "door":
              this.Add((Entity) new Door(entity2, vector2_1));
              continue;
            case "dreamBlock":
              this.Add((Entity) new DreamBlock(entity2, vector2_1));
              continue;
            case "dreamHeartGem":
              if (!this.Session.HeartGem)
              {
                this.Add((Entity) new DreamHeartGem(entity2, vector2_1));
                continue;
              }
              continue;
            case "dreammirror":
              this.Add((Entity) new DreamMirror(Vector2.op_Addition(vector2_1, entity2.Position)));
              continue;
            case "exitBlock":
              this.Add((Entity) new ExitBlock(entity2, vector2_1));
              continue;
            case "fakeBlock":
              this.Add((Entity) new FakeWall(entityId1, entity2, vector2_1, FakeWall.Modes.Block));
              continue;
            case "fakeHeart":
              this.Add((Entity) new FakeHeart(entity2, vector2_1));
              continue;
            case "fakeWall":
              this.Add((Entity) new FakeWall(entityId1, entity2, vector2_1, FakeWall.Modes.Wall));
              continue;
            case "fallingBlock":
              this.Add((Entity) new FallingBlock(entity2, vector2_1));
              continue;
            case "finalBoss":
              this.Add((Entity) new FinalBoss(entity2, vector2_1));
              continue;
            case "finalBossFallingBlock":
              this.Add((Entity) FallingBlock.CreateFinalBossBlock(entity2, vector2_1));
              continue;
            case "finalBossMovingBlock":
              this.Add((Entity) new FinalBossMovingBlock(entity2, vector2_1));
              continue;
            case "fireBall":
              this.Add((Entity) new FireBall(entity2, vector2_1));
              continue;
            case "fireBarrier":
              this.Add((Entity) new FireBarrier(entity2, vector2_1));
              continue;
            case "floatingDebris":
              this.Add((Entity) new FloatingDebris(entity2, vector2_1));
              continue;
            case "flutterbird":
              this.Add((Entity) new FlutterBird(entity2, vector2_1));
              continue;
            case "foregroundDebris":
              this.Add((Entity) new ForegroundDebris(entity2, vector2_1));
              continue;
            case "friendlyGhost":
              this.Add((Entity) new AngryOshiro(entity2, vector2_1));
              continue;
            case "glassBlock":
              this.Add((Entity) new GlassBlock(entity2, vector2_1));
              continue;
            case "goldenBerry":
              if ((SaveData.Instance.UnlockedModes >= 3 || SaveData.Instance.CheatMode || SaveData.Instance.DebugMode) && ((this.Session.Area.Mode != AreaMode.CSide || SaveData.Instance.Areas[this.Session.Area.ID].Modes[(int) this.Session.Area.Mode].Completed) && (this.Session.FurthestSeenLevel == this.Session.Level || this.Session.Deaths == 0)))
              {
                this.Add((Entity) new Strawberry(entity2, vector2_1, entityId1));
                continue;
              }
              continue;
            case "gondola":
              this.Add((Entity) new Gondola(entity2, vector2_1));
              continue;
            case "greenBlocks":
              ClutterBlockGenerator.Init(this);
              ClutterBlockGenerator.Add((int) (entity2.Position.X / 8.0), (int) (entity2.Position.Y / 8.0), entity2.Width / 8, entity2.Height / 8, ClutterBlock.Colors.Green);
              continue;
            case "hahaha":
              this.Add((Entity) new Hahaha(entity2, vector2_1));
              continue;
            case "hanginglamp":
              this.Add((Entity) new HangingLamp(entity2, Vector2.op_Addition(vector2_1, entity2.Position)));
              continue;
            case "heartGemDoor":
              this.Add((Entity) new HeartGemDoor(entity2, vector2_1));
              continue;
            case "iceBlock":
              this.Add((Entity) new IceBlock(entity2, vector2_1));
              continue;
            case "infiniteStar":
              this.Add((Entity) new FlyFeather(entity2, vector2_1));
              continue;
            case "introCar":
              this.Add((Entity) new IntroCar(entity2, vector2_1));
              continue;
            case "introCrusher":
              this.Add((Entity) new IntroCrusher(entity2, vector2_1));
              continue;
            case "invisibleBarrier":
              this.Add((Entity) new InvisibleBarrier(entity2, vector2_1));
              continue;
            case "jumpThru":
              this.Add((Entity) new JumpthruPlatform(entity2, vector2_1));
              continue;
            case "key":
              this.Add((Entity) new Key(entity2, vector2_1, entityId1));
              continue;
            case "killbox":
              this.Add((Entity) new Killbox(entity2, vector2_1));
              continue;
            case "lamp":
              this.Add((Entity) new Lamp(Vector2.op_Addition(vector2_1, entity2.Position), entity2.Bool("broken", false)));
              continue;
            case "light":
              this.Add((Entity) new PropLight(entity2, vector2_1));
              continue;
            case "lightbeam":
              this.Add((Entity) new LightBeam(entity2, vector2_1));
              continue;
            case "lightning":
              if (!this.Session.GetFlag("disable_lightning"))
              {
                this.Add((Entity) new Lightning(entity2, vector2_1));
                continue;
              }
              continue;
            case "lockBlock":
              this.Add((Entity) new LockBlock(entity2, vector2_1, entityId1));
              continue;
            case "memorial":
              this.Add((Entity) new Memorial(entity2, vector2_1));
              continue;
            case "memorialTextController":
              if (this.Session.Dashes == 0 && this.Session.StartedFromBeginning)
              {
                this.Add((Entity) new Strawberry(entity2, vector2_1, entityId1));
                continue;
              }
              continue;
            case "moveBlock":
              this.Add((Entity) new MoveBlock(entity2, vector2_1));
              continue;
            case "movingPlatform":
              this.Add((Entity) new MovingPlatform(entity2, vector2_1));
              continue;
            case "negaBlock":
              this.Add((Entity) new NegaBlock(entity2, vector2_1));
              continue;
            case "npc":
              string lower = entity2.Attr("npc", "").ToLower();
              Vector2 position = Vector2.op_Addition(entity2.Position, vector2_1);
              if (lower == "granny_00_house")
              {
                this.Add((Entity) new NPC00_Granny(position));
                continue;
              }
              if (lower == "theo_01_campfire")
              {
                this.Add((Entity) new NPC01_Theo(position));
                continue;
              }
              if (lower == "theo_02_campfire")
              {
                this.Add((Entity) new NPC02_Theo(position));
                continue;
              }
              if (lower == "theo_03_escaping")
              {
                if (!this.Session.GetFlag("resort_theo"))
                {
                  this.Add((Entity) new NPC03_Theo_Escaping(position));
                  continue;
                }
                continue;
              }
              if (lower == "theo_03_vents")
              {
                this.Add((Entity) new NPC03_Theo_Vents(position));
                continue;
              }
              if (lower == "oshiro_03_lobby")
              {
                this.Add((Entity) new NPC03_Oshiro_Lobby(position));
                continue;
              }
              if (lower == "oshiro_03_hallway")
              {
                this.Add((Entity) new NPC03_Oshiro_Hallway1(position));
                continue;
              }
              if (lower == "oshiro_03_hallway2")
              {
                this.Add((Entity) new NPC03_Oshiro_Hallway2(position));
                continue;
              }
              if (lower == "oshiro_03_bigroom")
              {
                this.Add((Entity) new NPC03_Oshiro_Cluttter(entity2, vector2_1));
                continue;
              }
              if (lower == "oshiro_03_breakdown")
              {
                this.Add((Entity) new NPC03_Oshiro_Breakdown(position));
                continue;
              }
              if (lower == "oshiro_03_suite")
              {
                this.Add((Entity) new NPC03_Oshiro_Suite(position));
                continue;
              }
              if (lower == "oshiro_03_rooftop")
              {
                this.Add((Entity) new NPC03_Oshiro_Rooftop(position));
                continue;
              }
              if (lower == "granny_04_cliffside")
              {
                this.Add((Entity) new NPC04_Granny(position));
                continue;
              }
              if (lower == "theo_04_cliffside")
              {
                this.Add((Entity) new NPC04_Theo(position));
                continue;
              }
              if (lower == "theo_05_entrance")
              {
                this.Add((Entity) new NPC05_Theo_Entrance(position));
                continue;
              }
              if (lower == "theo_05_inmirror")
              {
                this.Add((Entity) new NPC05_Theo_Mirror(position));
                continue;
              }
              if (lower == "evil_05")
              {
                this.Add((Entity) new NPC05_Badeline(entity2, vector2_1));
                continue;
              }
              if (lower == "theo_06_plateau")
              {
                this.Add((Entity) new NPC06_Theo_Plateau(entity2, vector2_1));
                continue;
              }
              if (lower == "granny_06_intro")
              {
                this.Add((Entity) new NPC06_Granny(entity2, vector2_1));
                continue;
              }
              if (lower == "badeline_06_crying")
              {
                this.Add((Entity) new NPC06_Badeline_Crying(entity2, vector2_1));
                continue;
              }
              if (lower == "granny_06_ending")
              {
                this.Add((Entity) new NPC06_Granny_Ending(entity2, vector2_1));
                continue;
              }
              if (lower == "theo_06_ending")
              {
                this.Add((Entity) new NPC06_Theo_Ending(entity2, vector2_1));
                continue;
              }
              if (lower == "granny_07x")
              {
                this.Add((Entity) new NPC07X_Granny_Ending(entity2, vector2_1));
                continue;
              }
              if (lower == "theo_08_inside")
              {
                this.Add((Entity) new NPC08_Theo(entity2, vector2_1));
                continue;
              }
              if (lower == "granny_08_inside")
              {
                this.Add((Entity) new NPC08_Granny(entity2, vector2_1));
                continue;
              }
              if (lower == "granny_09_outside")
              {
                this.Add((Entity) new NPC09_Granny_Outside(entity2, vector2_1));
                continue;
              }
              if (lower == "granny_09_inside")
              {
                this.Add((Entity) new NPC09_Granny_Inside(entity2, vector2_1));
                continue;
              }
              continue;
            case "oshirodoor":
              this.Add((Entity) new MrOshiroDoor(entity2, vector2_1));
              continue;
            case "payphone":
              this.Add((Entity) new Payphone(Vector2.op_Addition(vector2_1, entity2.Position)));
              continue;
            case "picoconsole":
              this.Add((Entity) new PicoConsole(entity2, vector2_1));
              continue;
            case "plateau":
              this.Add((Entity) new Plateau(entity2, vector2_1));
              continue;
            case "playerSeeker":
              this.Add((Entity) new PlayerSeeker(entity2, vector2_1));
              continue;
            case "redBlocks":
              ClutterBlockGenerator.Init(this);
              ClutterBlockGenerator.Add((int) (entity2.Position.X / 8.0), (int) (entity2.Position.Y / 8.0), entity2.Width / 8, entity2.Height / 8, ClutterBlock.Colors.Red);
              continue;
            case "refill":
              this.Add((Entity) new Refill(entity2, vector2_1));
              continue;
            case "reflectionHeartStatue":
              this.Add((Entity) new ReflectionHeartStatue(entity2, vector2_1));
              continue;
            case "resortLantern":
              this.Add((Entity) new ResortLantern(entity2, vector2_1));
              continue;
            case "resortRoofEnding":
              this.Add((Entity) new ResortRoofEnding(entity2, vector2_1));
              continue;
            case "resortmirror":
              this.Add((Entity) new ResortMirror(entity2, vector2_1));
              continue;
            case "ridgeGate":
              List<EntityID> entityIdList2 = new List<EntityID>();
              string str1 = entity2.Attr("strawberries", "");
              char[] chArray1 = new char[1]{ ',' };
              foreach (string str2 in str1.Split(chArray1))
              {
                EntityID none2 = EntityID.None;
                char[] chArray2 = new char[1]{ ':' };
                string[] strArray2 = str2.Split(chArray2);
                none2.Level = strArray2[0];
                none2.ID = Convert.ToInt32(strArray2[1]);
                entityIdList2.Add(none2);
              }
              bool flag4 = true;
              foreach (EntityID entityId2 in entityIdList2)
              {
                if (!this.Session.Strawberries.Contains(entityId2))
                {
                  flag4 = false;
                  break;
                }
              }
              if (flag4)
              {
                this.Add((Entity) new RidgeGate(entity2, vector2_1));
                continue;
              }
              continue;
            case "risingLava":
              this.Add((Entity) new RisingLava(entity2, vector2_1));
              continue;
            case "rotateSpinner":
              if (this.Session.Area.ID == 3 || this.Session.Area.ID == 7 && this.Session.Level.StartsWith("d-"))
              {
                this.Add((Entity) new DustRotateSpinner(entity2, vector2_1));
                continue;
              }
              this.Add((Entity) new BladeRotateSpinner(entity2, vector2_1));
              continue;
            case "rotatingPlatforms":
              Vector2 vector2_2 = Vector2.op_Addition(entity2.Position, vector2_1);
              Vector2 center = Vector2.op_Addition(entity2.Nodes[0], vector2_1);
              int width = entity2.Width;
              int num1 = entity2.Int("platforms", 0);
              bool clockwise = entity2.Bool("clockwise", false);
              Vector2 vector2_3 = Vector2.op_Subtraction(vector2_2, center);
              float length = ((Vector2) ref vector2_3).Length();
              float num2 = Vector2.op_Subtraction(vector2_2, center).Angle();
              float num3 = 6.283185f / (float) num1;
              for (int index2 = 0; index2 < num1; ++index2)
              {
                float angleRadians = Calc.WrapAngle(num2 + num3 * (float) index2);
                this.Add((Entity) new RotatingPlatform(Vector2.op_Addition(center, Calc.AngleToVector(angleRadians, length)), width, center, clockwise));
              }
              continue;
            case "sandwichLava":
              this.Add((Entity) new SandwichLava(entity2, vector2_1));
              continue;
            case "seeker":
              this.Add((Entity) new Seeker(entity2, vector2_1));
              continue;
            case "seekerBarrier":
              this.Add((Entity) new SeekerBarrier(entity2, vector2_1));
              continue;
            case "seekerStatue":
              this.Add((Entity) new SeekerStatue(entity2, vector2_1));
              continue;
            case "sinkingPlatform":
              this.Add((Entity) new SinkingPlatform(entity2, vector2_1));
              continue;
            case "slider":
              this.Add((Entity) new Slider(entity2, vector2_1));
              continue;
            case "soundSource":
              this.Add((Entity) new SoundSourceEntity(entity2, vector2_1));
              continue;
            case "spikesDown":
              this.Add((Entity) new Spikes(entity2, vector2_1, Spikes.Directions.Down));
              continue;
            case "spikesLeft":
              this.Add((Entity) new Spikes(entity2, vector2_1, Spikes.Directions.Left));
              continue;
            case "spikesRight":
              this.Add((Entity) new Spikes(entity2, vector2_1, Spikes.Directions.Right));
              continue;
            case "spikesUp":
              this.Add((Entity) new Spikes(entity2, vector2_1, Spikes.Directions.Up));
              continue;
            case "spinner":
              if (this.Session.Area.ID == 3 || this.Session.Area.ID == 7 && this.Session.Level.StartsWith("d-"))
              {
                this.Add((Entity) new DustStaticSpinner(entity2, vector2_1));
                continue;
              }
              CrystalColor color = CrystalColor.Blue;
              if (this.Session.Area.ID == 5)
                color = CrystalColor.Red;
              else if (this.Session.Area.ID == 6)
                color = CrystalColor.Purple;
              this.Add((Entity) new CrystalStaticSpinner(entity2, vector2_1, color));
              continue;
            case "spring":
              this.Add((Entity) new Spring(entity2, vector2_1, Spring.Orientations.Floor));
              continue;
            case "starClimbController":
              this.Add((Entity) new StarJumpController());
              continue;
            case "starJumpBlock":
              this.Add((Entity) new StarJumpBlock(entity2, vector2_1));
              continue;
            case "strawberry":
              this.Add((Entity) new Strawberry(entity2, vector2_1, entityId1));
              continue;
            case "summitGemManager":
              this.Add((Entity) new SummitGemManager(entity2, vector2_1));
              continue;
            case "summitcheckpoint":
              this.Add((Entity) new SummitCheckpoint(entity2, vector2_1));
              continue;
            case "summitcloud":
              this.Add((Entity) new SummitCloud(entity2, vector2_1));
              continue;
            case "summitgem":
              this.Add((Entity) new SummitGem(entity2, vector2_1, entityId1));
              continue;
            case "swapBlock":
            case "switchBlock":
              this.Add((Entity) new SwapBlock(entity2, vector2_1));
              continue;
            case "switchGate":
              this.Add((Entity) new SwitchGate(entity2, vector2_1));
              continue;
            case "templeBigEyeball":
              this.Add((Entity) new TempleBigEyeball(entity2, vector2_1));
              continue;
            case "templeCrackedBlock":
              this.Add((Entity) new TempleCrackedBlock(entityId1, entity2, vector2_1));
              continue;
            case "templeEye":
              this.Add((Entity) new TempleEye(entity2, vector2_1));
              continue;
            case "templeGate":
              this.Add((Entity) new TempleGate(entity2, vector2_1, levelData.Name));
              continue;
            case "templeMirror":
              this.Add((Entity) new TempleMirror(entity2, vector2_1));
              continue;
            case "templeMirrorPortal":
              this.Add((Entity) new TempleMirrorPortal(entity2, vector2_1));
              continue;
            case "tentacles":
              this.Add((Entity) new ReflectionTentacles(entity2, vector2_1));
              continue;
            case "theoCrystal":
              this.Add((Entity) new TheoCrystal(entity2, vector2_1));
              continue;
            case "theoCrystalPedestal":
              this.Add((Entity) new TheoCrystalPedestal(entity2, vector2_1));
              continue;
            case "torch":
              this.Add((Entity) new Torch(entity2, vector2_1, entityId1));
              continue;
            case "touchSwitch":
              this.Add((Entity) new TouchSwitch(entity2, vector2_1));
              continue;
            case "towerviewer":
              this.Add((Entity) new Lookout(entity2, vector2_1));
              continue;
            case "trackSpinner":
              if (this.Session.Area.ID == 3 || this.Session.Area.ID == 7 && this.Session.Level.StartsWith("d-"))
              {
                this.Add((Entity) new DustTrackSpinner(entity2, vector2_1));
                continue;
              }
              this.Add((Entity) new BladeTrackSpinner(entity2, vector2_1));
              continue;
            case "trapdoor":
              this.Add((Entity) new Trapdoor(entity2, vector2_1));
              continue;
            case "triggerSpikesDown":
              this.Add((Entity) new TriggerSpikes(entity2, vector2_1, TriggerSpikes.Directions.Down));
              continue;
            case "triggerSpikesLeft":
              this.Add((Entity) new TriggerSpikes(entity2, vector2_1, TriggerSpikes.Directions.Left));
              continue;
            case "triggerSpikesRight":
              this.Add((Entity) new TriggerSpikes(entity2, vector2_1, TriggerSpikes.Directions.Right));
              continue;
            case "triggerSpikesUp":
              this.Add((Entity) new TriggerSpikes(entity2, vector2_1, TriggerSpikes.Directions.Up));
              continue;
            case "wallBooster":
              this.Add((Entity) new WallBooster(entity2, vector2_1));
              continue;
            case "wallSpringLeft":
              this.Add((Entity) new Spring(entity2, vector2_1, Spring.Orientations.WallLeft));
              continue;
            case "wallSpringRight":
              this.Add((Entity) new Spring(entity2, vector2_1, Spring.Orientations.WallRight));
              continue;
            case "water":
              this.Add((Entity) new Water(entity2, vector2_1));
              continue;
            case "waterfall":
              this.Add((Entity) new WaterFall(entity2, vector2_1));
              continue;
            case "whiteblock":
              this.Add((Entity) new WhiteBlock(entity2, vector2_1));
              continue;
            case "wire":
              this.Add((Entity) new Wire(entity2, vector2_1));
              continue;
            case "yellowBlocks":
              ClutterBlockGenerator.Init(this);
              ClutterBlockGenerator.Add((int) (entity2.Position.X / 8.0), (int) (entity2.Position.Y / 8.0), entity2.Width / 8, entity2.Height / 8, ClutterBlock.Colors.Yellow);
              continue;
            case "zipMover":
              this.Add((Entity) new ZipMover(entity2, vector2_1));
              continue;
            default:
              continue;
          }
        }
      }
      ClutterBlockGenerator.Generate();
      foreach (EntityData trigger in levelData.Triggers)
      {
        int entityID = trigger.ID + 10000000;
        EntityID id = new EntityID(levelData.Name, entityID);
        if (!this.Session.DoNotLoad.Contains(id))
        {
          switch (trigger.Name)
          {
            case "altMusicTrigger":
              this.Add((Entity) new AltMusicTrigger(trigger, vector2_1));
              continue;
            case "ambienceParamTrigger":
              this.Add((Entity) new AmbienceParamTrigger(trigger, vector2_1));
              continue;
            case "bloomFadeTrigger":
              this.Add((Entity) new BloomFadeTrigger(trigger, vector2_1));
              continue;
            case "cameraAdvanceTargetTrigger":
              this.Add((Entity) new CameraAdvanceTargetTrigger(trigger, vector2_1));
              continue;
            case "cameraOffsetTrigger":
              this.Add((Entity) new CameraOffsetTrigger(trigger, vector2_1));
              continue;
            case "cameraTargetTrigger":
              this.Add((Entity) new CameraTargetTrigger(trigger, vector2_1));
              continue;
            case "changeRespawnTrigger":
              this.Add((Entity) new ChangeRespawnTrigger(trigger, vector2_1));
              continue;
            case "checkpointBlockerTrigger":
              this.Add((Entity) new CheckpointBlockerTrigger(trigger, vector2_1));
              continue;
            case "creditsTrigger":
              this.Add((Entity) new CreditsTrigger(trigger, vector2_1));
              continue;
            case "eventTrigger":
              this.Add((Entity) new EventTrigger(trigger, vector2_1));
              continue;
            case "goldenBerryCollectTrigger":
              this.Add((Entity) new GoldBerryCollectTrigger(trigger, vector2_1));
              continue;
            case "interactTrigger":
              this.Add((Entity) new InteractTrigger(trigger, vector2_1));
              continue;
            case "lightFadeTrigger":
              this.Add((Entity) new LightFadeTrigger(trigger, vector2_1));
              continue;
            case "lookoutBlocker":
              this.Add((Entity) new LookoutBlocker(trigger, vector2_1));
              continue;
            case "minitextboxTrigger":
              this.Add((Entity) new MiniTextboxTrigger(trigger, vector2_1, id));
              continue;
            case "musicFadeTrigger":
              this.Add((Entity) new MusicFadeTrigger(trigger, vector2_1));
              continue;
            case "musicTrigger":
              this.Add((Entity) new MusicTrigger(trigger, vector2_1));
              continue;
            case "noRefillTrigger":
              this.Add((Entity) new NoRefillTrigger(trigger, vector2_1));
              continue;
            case "oshiroTrigger":
              this.Add((Entity) new OshiroTrigger(trigger, vector2_1));
              continue;
            case "respawnTargetTrigger":
              this.Add((Entity) new RespawnTargetTrigger(trigger, vector2_1));
              continue;
            case "stopBoostTrigger":
              this.Add((Entity) new StopBoostTrigger(trigger, vector2_1));
              continue;
            case "windAttackTrigger":
              this.Add((Entity) new WindAttackTrigger(trigger, vector2_1));
              continue;
            case "windTrigger":
              this.Add((Entity) new WindTrigger(trigger, vector2_1));
              continue;
            default:
              continue;
          }
        }
      }
      foreach (DecalData fgDecal in levelData.FgDecals)
        this.Add((Entity) new Decal(fgDecal.Texture, Vector2.op_Addition(vector2_1, fgDecal.Position), fgDecal.Scale, -10500));
      foreach (DecalData bgDecal in levelData.BgDecals)
        this.Add((Entity) new Decal(bgDecal.Texture, Vector2.op_Addition(vector2_1, bgDecal.Position), bgDecal.Scale, 9000));
      if (playerIntro != Player.IntroTypes.Transition)
      {
        if (this.Session.JustStarted && !this.Session.StartedFromBeginning && nullable.HasValue)
          this.StartPosition = nullable;
        if (!this.Session.RespawnPoint.HasValue)
          this.Session.RespawnPoint = !this.StartPosition.HasValue ? new Vector2?(this.DefaultSpawnPoint) : new Vector2?(this.GetSpawnPoint(this.StartPosition.Value));
        Player player = new Player(this.Session.RespawnPoint.Value, this.Session.Inventory.Backpack ? PlayerSpriteMode.Madeline : PlayerSpriteMode.MadelineNoBackpack);
        player.IntroType = playerIntro;
        this.Add((Entity) player);
        this.Entities.UpdateLists();
        Level.CameraLockModes cameraLockMode = this.CameraLockMode;
        this.CameraLockMode = Level.CameraLockModes.None;
        this.Camera.Position = this.GetFullCameraTargetAt(player, player.Position);
        this.CameraLockMode = cameraLockMode;
        this.CameraUpwardMaxY = this.Camera.Y + 180f;
        foreach (EntityID key in this.Session.Keys)
          this.Add((Entity) new Key(player, key));
        SpotlightWipe.FocusPoint = Vector2.op_Subtraction(this.Session.RespawnPoint.Value, this.Camera.Position);
        if (playerIntro != Player.IntroTypes.Respawn && playerIntro != Player.IntroTypes.Fall)
        {
          SpotlightWipe spotlightWipe = new SpotlightWipe((Scene) this, true, (Action) null);
        }
        else
          this.DoScreenWipe(true, (Action) null, false);
        if (isFromLoader)
          this.RendererList.UpdateLists();
        this.Lighting.Alpha = !this.DarkRoom ? this.BaseLightingAlpha + this.Session.LightingAlphaAdd : this.Session.DarkRoomAlpha;
        this.Bloom.Base = AreaData.Get(this.Session).BloomBase + this.Session.BloomBaseAdd;
      }
      else
        this.Entities.UpdateLists();
      if (this.HasCassetteBlocks && this.ShouldCreateCassetteManager)
        this.Tracker.GetEntity<CassetteBlockManager>()?.OnLevelStart();
      if (!string.IsNullOrEmpty(levelData.ObjTiles))
      {
        Tileset tileset = new Tileset(GFX.Game["tilesets/scenery"], 8, 8);
        int[,] numArray = Calc.ReadCSVIntGrid(levelData.ObjTiles, this.Bounds.Width / 8, this.Bounds.Height / 8);
        for (int index2 = 0; index2 < numArray.GetLength(0); ++index2)
        {
          for (int index3 = 0; index3 < numArray.GetLength(1); ++index3)
          {
            if (numArray[index2, index3] != -1)
              TileInterceptor.TileCheck((Scene) this, tileset[numArray[index2, index3]], Vector2.op_Addition(new Vector2((float) (index2 * 8), (float) (index3 * 8)), this.LevelOffset));
          }
        }
      }
      Calc.PopRandom();
    }

    public void UnloadLevel()
    {
      List<Entity> excludingTagMask = this.GetEntitiesExcludingTagMask((int) Tags.Global);
      foreach (Entity entity in this.Tracker.GetEntities<Textbox>())
        excludingTagMask.Add(entity);
      this.UnloadEntities(excludingTagMask);
      this.Entities.UpdateLists();
    }

    public void Reload()
    {
      if (this.Completed)
        return;
      if (this.Session.FirstLevel && this.Session.Strawberries.Count <= 0 && (!this.Session.Cassette && !this.Session.HeartGem) && !this.Session.HitCheckpoint)
      {
        this.Session.Time = 0L;
        this.Session.Deaths = 0;
        this.TimerStarted = false;
      }
      this.Session.Dashes = this.Session.DashesAtLevelStart;
      Glitch.Value = 0.0f;
      Engine.TimeRate = 1f;
      Distort.Anxiety = 0.0f;
      Distort.GameRate = 1f;
      Audio.SetMusicParam("fade", 1f);
      this.ParticlesBG.Clear();
      this.Particles.Clear();
      this.ParticlesFG.Clear();
      TrailManager.Clear();
      this.UnloadLevel();
      GC.Collect();
      GC.WaitForPendingFinalizers();
      this.LoadLevel(Player.IntroTypes.Respawn, false);
      this.strawberriesDisplay.DrawLerp = 0.0f;
      WindController first = this.Entities.FindFirst<WindController>();
      if (first != null)
        first.SnapWind();
      else
        this.Wind = Vector2.get_Zero();
    }

    private bool ShouldCreateCassetteManager
    {
      get
      {
        if (this.Session.Area.Mode == AreaMode.Normal)
          return !this.Session.Cassette;
        return true;
      }
    }

    public void TransitionTo(LevelData next, Vector2 direction)
    {
      this.Session.CoreMode = this.CoreMode;
      this.transition = new Coroutine(this.TransitionRoutine(next, direction), true);
    }

    private IEnumerator TransitionRoutine(LevelData next, Vector2 direction)
    {
      Level level = this;
      Player player = level.Tracker.GetEntity<Player>();
      List<Entity> toRemove = level.GetEntitiesExcludingTagMask((int) Tags.Persistent | (int) Tags.Global);
      List<Component> transitionOut = level.Tracker.GetComponentsCopy<TransitionListener>();
      player.CleanUpTriggers();
      foreach (SoundSource component in level.Tracker.GetComponents<SoundSource>())
      {
        if (component.DisposeOnTransition)
          component.Stop(true);
      }
      level.PreviousBounds = new Rectangle?(level.Bounds);
      level.Session.Level = next.Name;
      level.Session.FirstLevel = false;
      level.Session.DeathsInCurrentLevel = 0;
      level.LoadLevel(Player.IntroTypes.Transition, false);
      Audio.SetParameter(Audio.CurrentAmbienceEventInstance, "has_conveyors", level.Tracker.GetEntities<WallBooster>().Count > 0 ? 1f : 0.0f);
      List<Component> transitionIn = level.Tracker.GetComponentsCopy<TransitionListener>();
      transitionIn.RemoveAll((Predicate<Component>) (c => transitionOut.Contains(c)));
      float cameraAt = 0.0f;
      Vector2 cameraFrom = level.Camera.Position;
      Vector2 dirPad = Vector2.op_Multiply(direction, 4f);
      if (Vector2.op_Equality(direction, Vector2.get_UnitY()))
        dirPad = Vector2.op_Multiply(direction, 12f);
      Vector2 playerTo = player.Position;
      while (direction.X != 0.0)
      {
        // ISSUE: variable of the null type
        __Null y = playerTo.Y;
        Rectangle bounds = level.Bounds;
        double bottom = (double) ((Rectangle) ref bounds).get_Bottom();
        if (y >= bottom)
        {
          ref __Null local = ref playerTo.Y;
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          ^(float&) ref local = ^(float&) ref local - 1f;
        }
        else
          break;
      }
      while (!level.IsInBounds(playerTo, dirPad))
        playerTo = Vector2.op_Addition(playerTo, direction);
      Vector2 cameraTo = level.GetFullCameraTargetAt(player, playerTo);
      Vector2 position = player.Position;
      player.Position = playerTo;
      foreach (Entity entity in player.CollideAll<WindTrigger>())
      {
        if (!toRemove.Contains(entity))
        {
          level.windController.SetPattern((entity as WindTrigger).Pattern);
          break;
        }
      }
      level.windController.SetStartPattern();
      player.Position = position;
      foreach (TransitionListener transitionListener in transitionOut)
      {
        if (transitionListener.OnOutBegin != null)
          transitionListener.OnOutBegin();
      }
      foreach (TransitionListener transitionListener in transitionIn)
      {
        if (transitionListener.OnInBegin != null)
          transitionListener.OnInBegin();
      }
      float lightingStart = level.Lighting.Alpha;
      float lightingEnd = level.DarkRoom ? level.Session.DarkRoomAlpha : level.BaseLightingAlpha + level.Session.LightingAlphaAdd;
      bool lightingWait = (double) lightingStart >= (double) level.Session.DarkRoomAlpha || (double) lightingEnd >= (double) level.Session.DarkRoomAlpha;
      if ((double) lightingEnd > (double) lightingStart & lightingWait)
      {
        Audio.Play("event:/game/05_mirror_temple/room_lightlevel_down");
        for (; (double) level.Lighting.Alpha != (double) lightingEnd; level.Lighting.Alpha = Calc.Approach(level.Lighting.Alpha, lightingEnd, 2f * Engine.DeltaTime))
          yield return (object) null;
      }
      bool cameraFinished = false;
      while (!player.TransitionTo(playerTo, direction) || (double) cameraAt < 1.0)
      {
        yield return (object) null;
        if (!cameraFinished)
        {
          cameraAt = Calc.Approach(cameraAt, 1f, Engine.DeltaTime / level.NextTransitionDuration);
          level.Camera.Position = (double) cameraAt <= 0.899999976158142 ? Vector2.Lerp(cameraFrom, cameraTo, Ease.CubeOut(cameraAt)) : cameraTo;
          if (!lightingWait && (double) lightingStart < (double) lightingEnd)
            level.Lighting.Alpha = lightingStart + (lightingEnd - lightingStart) * cameraAt;
          foreach (TransitionListener transitionListener in transitionOut)
          {
            if (transitionListener.OnOut != null)
              transitionListener.OnOut(cameraAt);
          }
          foreach (TransitionListener transitionListener in transitionIn)
          {
            if (transitionListener.OnIn != null)
              transitionListener.OnIn(cameraAt);
          }
          if ((double) cameraAt >= 1.0)
            cameraFinished = true;
        }
      }
      if ((double) lightingEnd < (double) lightingStart & lightingWait)
      {
        Audio.Play("event:/game/05_mirror_temple/room_lightlevel_up");
        for (; (double) level.Lighting.Alpha != (double) lightingEnd; level.Lighting.Alpha = Calc.Approach(level.Lighting.Alpha, lightingEnd, 2f * Engine.DeltaTime))
          yield return (object) null;
      }
      level.UnloadEntities(toRemove);
      level.Entities.UpdateLists();
      Rectangle bounds1 = level.Bounds;
      ((Rectangle) ref bounds1).Inflate(16, 16);
      level.Particles.ClearRect(bounds1, false);
      level.ParticlesBG.ClearRect(bounds1, false);
      level.ParticlesFG.ClearRect(bounds1, false);
      RespawnTargetTrigger respawnTargetTrigger = player.CollideFirst<RespawnTargetTrigger>();
      Vector2 to = respawnTargetTrigger != null ? respawnTargetTrigger.Target : player.Position;
      level.Session.RespawnPoint = new Vector2?(level.Session.LevelData.Spawns.ClosestTo(to));
      player.OnTransition();
      foreach (TransitionListener transitionListener in transitionIn)
      {
        if (transitionListener.OnInEnd != null)
          transitionListener.OnInEnd();
      }
      if (level.Session.LevelData.DelayAltMusic)
        Audio.SetAltMusic(Sfxs.EventnameByHandle(level.Session.LevelData.AltMusic));
      cameraFrom = (Vector2) null;
      playerTo = (Vector2) null;
      cameraTo = (Vector2) null;
      level.NextTransitionDuration = 0.65f;
      level.transition = (Coroutine) null;
    }

    public void UnloadEntities(List<Entity> entities)
    {
      foreach (Entity entity in entities)
        this.Remove(entity);
    }

    public Vector2 DefaultSpawnPoint
    {
      get
      {
        Rectangle bounds = this.Bounds;
        double left = (double) ((Rectangle) ref bounds).get_Left();
        bounds = this.Bounds;
        double bottom = (double) ((Rectangle) ref bounds).get_Bottom();
        return this.GetSpawnPoint(new Vector2((float) left, (float) bottom));
      }
    }

    public Vector2 GetSpawnPoint(Vector2 from)
    {
      return this.Session.GetSpawnPoint(from);
    }

    public Vector2 GetFullCameraTargetAt(Player player, Vector2 at)
    {
      Vector2 position = player.Position;
      player.Position = at;
      foreach (Entity entity in this.Tracker.GetEntities<Trigger>())
      {
        if (entity is CameraTargetTrigger && player.CollideCheck(entity))
          (entity as CameraTargetTrigger).OnStay(player);
        else if (entity is CameraOffsetTrigger && player.CollideCheck(entity))
          (entity as CameraOffsetTrigger).OnEnter(player);
      }
      Vector2 cameraTarget = player.CameraTarget;
      player.Position = position;
      return cameraTarget;
    }

    public Rectangle Bounds
    {
      get
      {
        return this.Session.LevelData.Bounds;
      }
    }

    public Rectangle? PreviousBounds { get; private set; }

    public void AutoSave()
    {
      if (this.saving != null)
        return;
      this.saving = new Coroutine(this.SavingRoutine(), true);
    }

    private IEnumerator SavingRoutine()
    {
      UserIO.SaveHandler(true, false);
      while (UserIO.Saving)
        yield return (object) null;
      this.saving = (Coroutine) null;
    }

    public override void Update()
    {
      if (this.Overlay != null)
      {
        this.Overlay.Update();
        this.Entities.UpdateLists();
      }
      else
      {
        int num1 = 10;
        if (!this.InCutscene && this.Tracker.GetEntity<Player>() != null && this.Wipe == null)
          num1 = SaveData.Instance.Assists.GameSpeed;
        Engine.TimeRateB = (float) num1 / 10f;
        if (num1 != 10)
        {
          if ((HandleBase) Level.AssistSpeedSnapshot == (HandleBase) null || Level.AssistSpeedSnapshotValue != num1)
          {
            Audio.EndSnapshot(Level.AssistSpeedSnapshot);
            Level.AssistSpeedSnapshotValue = num1;
            Level.AssistSpeedSnapshot = Level.AssistSpeedSnapshotValue >= 10 ? Audio.CreateSnapshot("snapshot:/variant_speed/variant_speed_" + (object) (Level.AssistSpeedSnapshotValue * 10), true) : Audio.CreateSnapshot("snapshot:/assist_game_speed/assist_speed_" + (object) (Level.AssistSpeedSnapshotValue * 10), true);
          }
        }
        else if ((HandleBase) Level.AssistSpeedSnapshot != (HandleBase) null)
        {
          Audio.EndSnapshot(Level.AssistSpeedSnapshot);
          Level.AssistSpeedSnapshot = (EventInstance) null;
          Level.AssistSpeedSnapshotValue = -1;
        }
        if (this.wasPaused && !this.Paused)
          this.EndPauseEffects();
        if (this.CanPause && Celeste.Input.QuickResetPressed())
          this.Pause(0, false, true);
        else if (this.CanPause && (Celeste.Input.Pause.Pressed || Celeste.Input.ESC.Pressed))
          this.Pause(0, false, false);
        if (this.wasPaused && !this.Paused)
          this.wasPaused = false;
        if (!this.InCredits && this.Session.Area.ID != 8 && !this.TimerStopped)
        {
          long ticks = TimeSpan.FromSeconds((double) Engine.RawDeltaTime).Ticks;
          SaveData.Instance.AddTime(this.Session.Area, ticks);
          if (!this.TimerStarted && !this.InCutscene)
          {
            Player entity = this.Tracker.GetEntity<Player>();
            if (entity != null && !entity.TimePaused)
              this.TimerStarted = true;
          }
          if (!this.Completed && this.TimerStarted)
            this.Session.Time += ticks;
        }
        if (this.saving != null)
          this.saving.Update();
        if (!this.Paused)
        {
          this.glitchTimer += Engine.DeltaTime;
          this.glitchSeed = Calc.Random.NextFloat();
        }
        if (this.SkippingCutscene)
        {
          if (this.skipCoroutine != null)
            this.skipCoroutine.Update();
          this.RendererList.Update();
        }
        else if (this.FrozenOrPaused)
        {
          bool disabled = MInput.Disabled;
          MInput.Disabled = false;
          if (!this.Paused)
          {
            foreach (Entity entity in this[Tags.FrozenUpdate])
            {
              if (entity.Active)
                entity.Update();
            }
          }
          foreach (Entity entity in this[Tags.PauseUpdate])
          {
            if (entity.Active)
              entity.Update();
          }
          MInput.Disabled = disabled;
          if (this.Wipe != null)
            this.Wipe.Update((Scene) this);
          if (this.HiresSnow != null)
            this.HiresSnow.Update((Scene) this);
          this.Entities.UpdateLists();
        }
        else if (!this.Transitioning)
        {
          if (this.RetryPlayerCorpse == null)
          {
            base.Update();
          }
          else
          {
            this.RetryPlayerCorpse.Update();
            this.RendererList.Update();
            foreach (Entity entity in this[Tags.PauseUpdate])
            {
              if (entity.Active)
                entity.Update();
            }
          }
        }
        else
        {
          foreach (Entity entity in this[Tags.TransitionUpdate])
            entity.Update();
          this.transition.Update();
          this.RendererList.Update();
        }
        this.HudRenderer.BackgroundFade = Calc.Approach(this.HudRenderer.BackgroundFade, this.Paused ? 1f : 0.0f, 8f * Engine.DeltaTime);
        if (!this.FrozenOrPaused)
        {
          this.WindSineTimer += Engine.DeltaTime;
          this.WindSine = (float) (Math.Sin((double) this.WindSineTimer) + 1.0) / 2f;
        }
        foreach (PostUpdateHook component in this.Tracker.GetComponents<PostUpdateHook>())
        {
          if (component.Entity.Active)
            component.OnPostUpdate();
        }
        if (this.updateHair)
        {
          foreach (Component component in this.Tracker.GetComponents<PlayerHair>())
          {
            if (component.Active && component.Entity.Active)
              (component as PlayerHair).AfterUpdate();
          }
          if (this.FrozenOrPaused)
            this.updateHair = false;
        }
        else if (!this.FrozenOrPaused)
          this.updateHair = true;
        if ((double) this.shakeTimer > 0.0)
        {
          if (this.OnRawInterval(0.04f))
          {
            int num2 = (int) Math.Ceiling((double) this.shakeTimer * 10.0);
            if (Vector2.op_Equality(this.shakeDirection, Vector2.get_Zero()))
            {
              this.shakeVector.X = (__Null) (double) (-num2 + Calc.Random.Next(num2 * 2 + 1));
              this.shakeVector.Y = (__Null) (double) (-num2 + Calc.Random.Next(num2 * 2 + 1));
            }
            else
            {
              if (this.lastDirectionalShake == 0)
                this.lastDirectionalShake = 1;
              else
                this.lastDirectionalShake *= -1;
              this.shakeVector = Vector2.op_Multiply(Vector2.op_Multiply(Vector2.op_UnaryNegation(this.shakeDirection), (float) this.lastDirectionalShake), (float) num2);
            }
          }
          this.shakeTimer -= Engine.RawDeltaTime;
        }
        else
          this.shakeVector = Vector2.get_Zero();
        if (this.doFlash)
        {
          this.flash = Calc.Approach(this.flash, 1f, Engine.DeltaTime * 10f);
          if ((double) this.flash >= 1.0)
            this.doFlash = false;
        }
        else if ((double) this.flash > 0.0)
          this.flash = Calc.Approach(this.flash, 0.0f, Engine.DeltaTime * 3f);
        if (this.lastColorGrade != this.Session.ColorGrade)
        {
          if ((double) this.colorGradeEase >= 1.0)
          {
            this.colorGradeEase = 0.0f;
            this.lastColorGrade = this.Session.ColorGrade;
          }
          else
            this.colorGradeEase = Calc.Approach(this.colorGradeEase, 1f, Engine.DeltaTime);
        }
        if (Celeste.Celeste.PlayMode != Celeste.Celeste.PlayModes.Debug)
          return;
        if (MInput.Keyboard.Pressed((Keys) 9))
          Engine.Scene = (Scene) new MapEditor(this.Session.Area, true);
        if (MInput.Keyboard.Pressed((Keys) 112))
        {
          Celeste.Celeste.ReloadAssets(true, false, false, new AreaKey?(this.Session.Area));
          Engine.Scene = (Scene) new LevelLoader(this.Session, new Vector2?());
        }
        else if (MInput.Keyboard.Pressed((Keys) 113))
        {
          Celeste.Celeste.ReloadAssets(true, true, false, new AreaKey?(this.Session.Area));
          Engine.Scene = (Scene) new LevelLoader(this.Session, new Vector2?());
        }
        else
        {
          if (!MInput.Keyboard.Pressed((Keys) 114))
            return;
          Celeste.Celeste.ReloadAssets(true, true, true, new AreaKey?(this.Session.Area));
          Engine.Scene = (Scene) new LevelLoader(this.Session, new Vector2?());
        }
      }
    }

    public override void BeforeRender()
    {
      this.cameraPreShake = this.Camera.Position;
      Camera camera = this.Camera;
      camera.Position = Vector2.op_Addition(camera.Position, this.shakeVector);
      this.Camera.Position = this.Camera.Position.Floor();
      foreach (BeforeRenderHook component in this.Tracker.GetComponents<BeforeRenderHook>())
      {
        if (component.Visible)
          component.Callback();
      }
      SpeedRing.DrawToBuffer(this);
      base.BeforeRender();
    }

    public override void Render()
    {
      Engine.Instance.get_GraphicsDevice().SetRenderTarget((RenderTarget2D) GameplayBuffers.Gameplay);
      Engine.Instance.get_GraphicsDevice().Clear(Color.get_Transparent());
      this.GameplayRenderer.Render((Scene) this);
      this.Lighting.Render((Scene) this);
      Engine.Instance.get_GraphicsDevice().SetRenderTarget((RenderTarget2D) GameplayBuffers.Level);
      Engine.Instance.get_GraphicsDevice().Clear(this.BackgroundColor);
      this.Background.Render((Scene) this);
      Distort.Render((Texture2D) (RenderTarget2D) GameplayBuffers.Gameplay, (Texture2D) (RenderTarget2D) GameplayBuffers.Displacement, this.Displacement.HasDisplacement((Scene) this));
      this.Bloom.Apply(GameplayBuffers.Level, (Scene) this);
      this.Foreground.Render((Scene) this);
      Glitch.Apply(GameplayBuffers.Level, this.glitchTimer * 2f, this.glitchSeed, 6.283185f);
      if ((double) this.flash > 0.0)
      {
        Draw.SpriteBatch.Begin((SpriteSortMode) 0, (BlendState) BlendState.AlphaBlend, (SamplerState) SamplerState.PointClamp, (DepthStencilState) DepthStencilState.Default, (RasterizerState) RasterizerState.CullNone, (Effect) null);
        Draw.Rect(-1f, -1f, 322f, 182f, Color.op_Multiply(this.flashColor, this.flash));
        Draw.SpriteBatch.End();
        if (this.flashDrawPlayer)
        {
          Draw.SpriteBatch.Begin((SpriteSortMode) 0, (BlendState) BlendState.AlphaBlend, (SamplerState) SamplerState.PointClamp, (DepthStencilState) DepthStencilState.Default, (RasterizerState) RasterizerState.CullNone, (Effect) null, this.Camera.Matrix);
          Player entity = this.Tracker.GetEntity<Player>();
          if (entity != null && entity.Visible)
            entity.Render();
          Draw.SpriteBatch.End();
        }
      }
      Engine.Instance.get_GraphicsDevice().SetRenderTarget((RenderTarget2D) null);
      Engine.Instance.get_GraphicsDevice().Clear(Color.get_Black());
      Engine.Instance.get_GraphicsDevice().set_Viewport(Engine.Viewport);
      Matrix matrix = Matrix.op_Multiply(Matrix.CreateScale(6f), Engine.ScreenMatrix);
      Vector2 vector2_1;
      ((Vector2) ref vector2_1).\u002Ector(320f, 180f);
      Vector2 vector2_2 = Vector2.op_Division(vector2_1, this.ZoomTarget);
      Vector2 vector2_3 = (double) this.ZoomTarget != 1.0 ? Vector2.op_Multiply(Vector2.op_Division(Vector2.op_Subtraction(this.ZoomFocusPoint, Vector2.op_Division(vector2_2, 2f)), Vector2.op_Subtraction(vector2_1, vector2_2)), vector2_1) : Vector2.get_Zero();
      MTexture orDefault1 = GFX.ColorGrades.GetOrDefault(this.lastColorGrade, GFX.ColorGrades["none"]);
      MTexture orDefault2 = GFX.ColorGrades.GetOrDefault(this.Session.ColorGrade, GFX.ColorGrades["none"]);
      if ((double) this.colorGradeEase > 0.0 && orDefault1 != orDefault2)
        ColorGrade.Set(orDefault1, orDefault2, this.colorGradeEase);
      else
        ColorGrade.Set(orDefault2);
      float num = this.Zoom * (float) ((320.0 - (double) this.ScreenPadding * 2.0) / 320.0);
      Vector2 vector2_4;
      ((Vector2) ref vector2_4).\u002Ector(this.ScreenPadding, this.ScreenPadding * (9f / 16f));
      if (SaveData.Instance.Assists.MirrorMode)
      {
        vector2_4.X = -vector2_4.X;
        vector2_3.X = (__Null) (160.0 - (vector2_3.X - 160.0));
      }
      Draw.SpriteBatch.Begin((SpriteSortMode) 0, (BlendState) BlendState.AlphaBlend, (SamplerState) SamplerState.PointClamp, (DepthStencilState) DepthStencilState.Default, (RasterizerState) RasterizerState.CullNone, ColorGrade.Effect, matrix);
      Draw.SpriteBatch.Draw((Texture2D) (RenderTarget2D) GameplayBuffers.Level, Vector2.op_Addition(vector2_3, vector2_4), new Rectangle?(GameplayBuffers.Level.Bounds), Color.get_White(), 0.0f, vector2_3, num, SaveData.Instance.Assists.MirrorMode ? (SpriteEffects) 1 : (SpriteEffects) 0, 0.0f);
      Draw.SpriteBatch.End();
      if (this.Pathfinder != null && this.Pathfinder.DebugRenderEnabled)
      {
        Draw.SpriteBatch.Begin((SpriteSortMode) 0, (BlendState) BlendState.AlphaBlend, (SamplerState) SamplerState.PointClamp, (DepthStencilState) DepthStencilState.Default, (RasterizerState) RasterizerState.CullNone, (Effect) null, Matrix.op_Multiply(this.Camera.Matrix, matrix));
        this.Pathfinder.Render();
        Draw.SpriteBatch.End();
      }
      if (!this.Paused || !this.PauseMainMenuOpen || !Celeste.Input.MenuJournal.Check)
        this.HudRenderer.Render((Scene) this);
      if (this.Wipe != null)
        this.Wipe.Render((Scene) this);
      if (this.HiresSnow == null)
        return;
      this.HiresSnow.Render((Scene) this);
    }

    public override void AfterRender()
    {
      base.AfterRender();
      this.Camera.Position = this.cameraPreShake;
    }

    private void StartPauseEffects()
    {
      if (Audio.CurrentMusic == "event:/music/lvl0/bridge")
        Audio.PauseMusic = true;
      Audio.PauseGameplaySfx = true;
      Audio.Play("event:/ui/game/pause");
      if (!((HandleBase) Level.PauseSnapshot == (HandleBase) null))
        return;
      Level.PauseSnapshot = Audio.CreateSnapshot("snapshot:/pause_menu", true);
    }

    private void EndPauseEffects()
    {
      Audio.PauseMusic = false;
      Audio.PauseGameplaySfx = false;
      Audio.ReleaseSnapshot(Level.PauseSnapshot);
      Level.PauseSnapshot = (EventInstance) null;
    }

    public void Pause(int startIndex = 0, bool minimal = false, bool quickReset = false)
    {
      this.wasPaused = true;
      Player player = this.Tracker.GetEntity<Player>();
      if (!this.Paused)
        this.StartPauseEffects();
      this.Paused = true;
      if (quickReset)
      {
        this.PauseMainMenuOpen = false;
        this.GiveUp(0, true, minimal, false);
      }
      else
      {
        this.PauseMainMenuOpen = true;
        TextMenu menu = new TextMenu();
        if (!minimal)
          menu.Add((TextMenu.Item) new TextMenu.Header(Dialog.Clean("menu_pause_title", (Language) null)));
        menu.Add(new TextMenu.Button(Dialog.Clean("menu_pause_resume", (Language) null)).Pressed((Action) (() => menu.OnCancel())));
        if (this.InCutscene && !this.SkippingCutscene)
          menu.Add(new TextMenu.Button(Dialog.Clean("menu_pause_skip_cutscene", (Language) null)).Pressed((Action) (() =>
          {
            this.SkipCutscene();
            this.Paused = false;
            this.PauseMainMenuOpen = false;
            menu.RemoveSelf();
          })));
        if (!minimal && !this.InCutscene && !this.SkippingCutscene)
        {
          TextMenu.Item obj;
          menu.Add(obj = new TextMenu.Button(Dialog.Clean("menu_pause_retry", (Language) null)).Pressed((Action) (() =>
          {
            if (player != null && !player.Dead)
            {
              Engine.TimeRate = 1f;
              Distort.GameRate = 1f;
              Distort.Anxiety = 0.0f;
              this.InCutscene = this.SkippingCutscene = false;
              this.RetryPlayerCorpse = player.Die(Vector2.get_Zero(), true, true);
              foreach (LevelEndingHook component in this.Tracker.GetComponents<LevelEndingHook>())
              {
                if (component.OnEnd != null)
                  component.OnEnd();
              }
            }
            this.Paused = false;
            this.PauseMainMenuOpen = false;
            this.EndPauseEffects();
            menu.RemoveSelf();
          })));
          obj.Disabled = !this.CanRetry || player != null && !player.CanRetry || this.Frozen;
        }
        if (!minimal && SaveData.Instance.AssistMode)
        {
          TextMenu.Item item = (TextMenu.Item) null;
          menu.Add(item = new TextMenu.Button(Dialog.Clean("menu_pause_assist", (Language) null)).Pressed((Action) (() =>
          {
            menu.RemoveSelf();
            this.PauseMainMenuOpen = false;
            this.AssistMode(menu.IndexOf(item), minimal);
          })));
        }
        if (!minimal && SaveData.Instance.VariantMode)
        {
          TextMenu.Item item = (TextMenu.Item) null;
          menu.Add(item = new TextMenu.Button(Dialog.Clean("menu_pause_variant", (Language) null)).Pressed((Action) (() =>
          {
            menu.RemoveSelf();
            this.PauseMainMenuOpen = false;
            this.VariantMode(menu.IndexOf(item), minimal);
          })));
        }
        TextMenu.Item item1 = (TextMenu.Item) null;
        menu.Add(item1 = new TextMenu.Button(Dialog.Clean("menu_pause_options", (Language) null)).Pressed((Action) (() =>
        {
          menu.RemoveSelf();
          this.PauseMainMenuOpen = false;
          this.Options(menu.IndexOf(item1), minimal);
        })));
        if (!minimal && Celeste.Celeste.PlayMode != Celeste.Celeste.PlayModes.Event)
        {
          TextMenu.Item obj;
          menu.Add(obj = new TextMenu.Button(Dialog.Clean("menu_pause_savequit", (Language) null)).Pressed((Action) (() =>
          {
            menu.Focused = false;
            Engine.TimeRate = 1f;
            Audio.SetMusic((string) null, true, true);
            Audio.BusStopAll("bus:/gameplay_sfx", true);
            this.Session.InArea = true;
            ++this.Session.Deaths;
            ++this.Session.DeathsInCurrentLevel;
            SaveData.Instance.AddDeath(this.Session.Area);
            this.DoScreenWipe(false, (Action) (() => Engine.Scene = (Scene) new LevelExit(LevelExit.Mode.SaveAndQuit, this.Session, this.HiresSnow)), true);
            foreach (LevelEndingHook component in this.Tracker.GetComponents<LevelEndingHook>())
            {
              if (component.OnEnd != null)
                component.OnEnd();
            }
          })));
          if (this.SaveQuitDisabled || player != null && player.StateMachine.State == 18)
            obj.Disabled = true;
        }
        if (!minimal)
        {
          menu.Add((TextMenu.Item) new TextMenu.SubHeader(""));
          TextMenu.Item item2 = (TextMenu.Item) null;
          menu.Add(item2 = new TextMenu.Button(Dialog.Clean("menu_pause_restartarea", (Language) null)).Pressed((Action) (() =>
          {
            this.PauseMainMenuOpen = false;
            menu.RemoveSelf();
            this.GiveUp(menu.IndexOf(item2), true, minimal, true);
          })));
          (item2 as TextMenu.Button).ConfirmSfx = "event:/ui/main/message_confirm";
          if (SaveData.Instance.Areas[0].Modes[0].Completed || SaveData.Instance.DebugMode || SaveData.Instance.CheatMode)
          {
            TextMenu.Item item3 = (TextMenu.Item) null;
            menu.Add(item3 = new TextMenu.Button(Dialog.Clean("menu_pause_return", (Language) null)).Pressed((Action) (() =>
            {
              this.PauseMainMenuOpen = false;
              menu.RemoveSelf();
              this.GiveUp(menu.IndexOf(item3), false, minimal, false);
            })));
            (item3 as TextMenu.Button).ConfirmSfx = "event:/ui/main/message_confirm";
          }
          if (Celeste.Celeste.PlayMode == Celeste.Celeste.PlayModes.Event)
            menu.Add(new TextMenu.Button(Dialog.Clean("menu_pause_restartdemo", (Language) null)).Pressed((Action) (() =>
            {
              this.EndPauseEffects();
              Audio.SetMusic((string) null, true, true);
              menu.Focused = false;
              this.DoScreenWipe(false, (Action) (() => LevelEnter.Go(new Session(new AreaKey(0, AreaMode.Normal), (string) null, (AreaStats) null), false)), false);
            })));
        }
        menu.OnESC = menu.OnCancel = menu.OnPause = (Action) (() =>
        {
          this.PauseMainMenuOpen = false;
          menu.RemoveSelf();
          this.Paused = false;
          Audio.Play("event:/ui/game/unpause");
          Engine.FreezeTimer = 0.15f;
        });
        if (startIndex > 0)
          menu.Selection = startIndex;
        this.Add((Entity) menu);
      }
    }

    private void GiveUp(int returnIndex, bool restartArea, bool minimal, bool showHint)
    {
      this.Paused = true;
      QuickResetHint hint = (QuickResetHint) null;
      if (restartArea & showHint)
        this.Add((Entity) (hint = new QuickResetHint()));
      TextMenu menu = new TextMenu()
      {
        (TextMenu.Item) new TextMenu.Header(Dialog.Clean("menu_giveup_title", (Language) null))
      };
      menu.Add(new TextMenu.Button(Dialog.Clean(restartArea ? "menu_giveup_restart" : "menu_giveup_return", (Language) null)).Pressed((Action) (() =>
      {
        Engine.TimeRate = 1f;
        menu.Focused = false;
        this.Session.InArea = false;
        Audio.SetMusic((string) null, true, true);
        Audio.BusStopAll("bus:/gameplay_sfx", true);
        if (restartArea)
          this.DoScreenWipe(false, (Action) (() => Engine.Scene = (Scene) new LevelExit(LevelExit.Mode.Restart, this.Session, (HiresSnow) null)), false);
        else
          this.DoScreenWipe(false, (Action) (() => Engine.Scene = (Scene) new LevelExit(LevelExit.Mode.GiveUp, this.Session, this.HiresSnow)), true);
        foreach (LevelEndingHook component in this.Tracker.GetComponents<LevelEndingHook>())
        {
          if (component.OnEnd != null)
            component.OnEnd();
        }
      })));
      menu.Add(new TextMenu.Button(Dialog.Clean("menu_giveup_cancel", (Language) null)).Pressed((Action) (() => menu.OnCancel())));
      menu.OnPause = menu.OnESC = (Action) (() =>
      {
        menu.RemoveSelf();
        hint?.RemoveSelf();
        this.Paused = false;
        Engine.FreezeTimer = 0.15f;
        Audio.Play("event:/ui/game/unpause");
      });
      menu.OnCancel = (Action) (() =>
      {
        Audio.Play("event:/ui/main/button_back");
        menu.RemoveSelf();
        hint?.RemoveSelf();
        this.Pause(returnIndex, minimal, false);
      });
      this.Add((Entity) menu);
    }

    private void Options(int returnIndex, bool minimal)
    {
      this.Paused = true;
      TextMenu options = MenuOptions.Create(true, Level.PauseSnapshot);
      options.OnESC = options.OnCancel = (Action) (() =>
      {
        Audio.Play("event:/ui/main/button_back");
        options.CloseAndRun(this.SaveFromOptions(), (Action) (() => this.Pause(returnIndex, minimal, false)));
      });
      options.OnPause = (Action) (() =>
      {
        Audio.Play("event:/ui/main/button_back");
        options.CloseAndRun(this.SaveFromOptions(), (Action) (() =>
        {
          this.Paused = false;
          Engine.FreezeTimer = 0.15f;
        }));
      });
      this.Add((Entity) options);
    }

    private IEnumerator SaveFromOptions()
    {
      UserIO.SaveHandler(false, true);
      while (UserIO.Saving)
        yield return (object) null;
    }

    private void AssistMode(int returnIndex, bool minimal)
    {
      this.Paused = true;
      TextMenu menu = new TextMenu();
      menu.Add((TextMenu.Item) new TextMenu.Header(Dialog.Clean("MENU_ASSIST_TITLE", (Language) null)));
      menu.Add((TextMenu.Item) new TextMenu.Slider(Dialog.Clean("MENU_ASSIST_GAMESPEED", (Language) null), (Func<int, string>) (i => (i * 10).ToString() + "%"), 5, 10, SaveData.Instance.Assists.GameSpeed).Change((Action<int>) (i =>
      {
        SaveData.Instance.Assists.GameSpeed = i;
        Engine.TimeRateB = (float) SaveData.Instance.Assists.GameSpeed / 10f;
      })));
      menu.Add((TextMenu.Item) new TextMenu.OnOff(Dialog.Clean("MENU_ASSIST_INFINITE_STAMINA", (Language) null), SaveData.Instance.Assists.InfiniteStamina).Change((Action<bool>) (on => SaveData.Instance.Assists.InfiniteStamina = on)));
      TextMenu textMenu = menu;
      string label = Dialog.Clean("MENU_ASSIST_AIR_DASHES", (Language) null);
      int dashMode = (int) SaveData.Instance.Assists.DashMode;
      TextMenu.Option<int> option1;
      TextMenu.Option<int> option2 = option1 = new TextMenu.Slider(label, (Func<int, string>) (i =>
      {
        if (i == 0)
          return Dialog.Clean("MENU_ASSIST_AIR_DASHES_NORMAL", (Language) null);
        if (i == 1)
          return Dialog.Clean("MENU_ASSIST_AIR_DASHES_TWO", (Language) null);
        return Dialog.Clean("MENU_ASSIST_AIR_DASHES_INFINITE", (Language) null);
      }), 0, 2, dashMode).Change((Action<int>) (on =>
      {
        SaveData.Instance.Assists.DashMode = (Assists.DashModes) on;
        Player entity = this.Tracker.GetEntity<Player>();
        if (entity == null)
          return;
        entity.Dashes = Math.Min(entity.Dashes, entity.MaxDashes);
      }));
      textMenu.Add((TextMenu.Item) option1);
      if (this.Session.Area.ID == 0)
        option2.Disabled = true;
      menu.Add((TextMenu.Item) new TextMenu.OnOff(Dialog.Clean("MENU_ASSIST_INVINCIBLE", (Language) null), SaveData.Instance.Assists.Invincible).Change((Action<bool>) (on => SaveData.Instance.Assists.Invincible = on)));
      menu.OnESC = menu.OnCancel = (Action) (() =>
      {
        Audio.Play("event:/ui/main/button_back");
        this.Pause(returnIndex, minimal, false);
        menu.Close();
      });
      menu.OnPause = (Action) (() =>
      {
        Audio.Play("event:/ui/main/button_back");
        this.Paused = false;
        Engine.FreezeTimer = 0.15f;
        menu.Close();
      });
      this.Add((Entity) menu);
    }

    private void VariantMode(int returnIndex, bool minimal)
    {
      this.Paused = true;
      TextMenu menu = new TextMenu();
      menu.Add((TextMenu.Item) new TextMenu.Header(Dialog.Clean("MENU_VARIANT_TITLE", (Language) null)));
      menu.Add((TextMenu.Item) new TextMenu.SubHeader(Dialog.Clean("MENU_VARIANT_SUBTITLE", (Language) null)));
      TextMenu textMenu1 = menu;
      string label1 = Dialog.Clean("MENU_ASSIST_GAMESPEED", (Language) null);
      int gameSpeed = SaveData.Instance.Assists.GameSpeed;
      TextMenu.Slider speed;
      TextMenu.Slider slider = speed = new TextMenu.Slider(label1, (Func<int, string>) (i => (i * 10).ToString() + "%"), 5, 16, gameSpeed);
      textMenu1.Add((TextMenu.Item) slider);
      speed.Change((Action<int>) (i =>
      {
        if (i > 10)
        {
          if (speed.Values[speed.PreviousIndex].Item2 > i)
            --i;
          else
            ++i;
        }
        speed.Index = i - 5;
        SaveData.Instance.Assists.GameSpeed = i;
        Engine.TimeRateB = (float) SaveData.Instance.Assists.GameSpeed / 10f;
      }));
      menu.Add((TextMenu.Item) new TextMenu.OnOff(Dialog.Clean("MENU_VARIANT_MIRROR", (Language) null), SaveData.Instance.Assists.MirrorMode).Change((Action<bool>) (on =>
      {
        SaveData.Instance.Assists.MirrorMode = on;
        Celeste.Input.MoveX.Inverted = Celeste.Input.Aim.InvertedX = on;
      })));
      menu.Add((TextMenu.Item) new TextMenu.OnOff(Dialog.Clean("MENU_VARIANT_360DASHING", (Language) null), SaveData.Instance.Assists.ThreeSixtyDashing).Change((Action<bool>) (on => SaveData.Instance.Assists.ThreeSixtyDashing = on)));
      menu.Add((TextMenu.Item) new TextMenu.OnOff(Dialog.Clean("MENU_VARIANT_INVISMOTION", (Language) null), SaveData.Instance.Assists.InvisibleMotion).Change((Action<bool>) (on => SaveData.Instance.Assists.InvisibleMotion = on)));
      menu.Add((TextMenu.Item) new TextMenu.OnOff(Dialog.Clean("MENU_VARIANT_NOGRABBING", (Language) null), SaveData.Instance.Assists.NoGrabbing).Change((Action<bool>) (on => SaveData.Instance.Assists.NoGrabbing = on)));
      menu.Add((TextMenu.Item) new TextMenu.OnOff(Dialog.Clean("MENU_VARIANT_LOWFRICTION", (Language) null), SaveData.Instance.Assists.LowFriction).Change((Action<bool>) (on => SaveData.Instance.Assists.LowFriction = on)));
      menu.Add((TextMenu.Item) new TextMenu.OnOff(Dialog.Clean("MENU_VARIANT_SUPERDASHING", (Language) null), SaveData.Instance.Assists.SuperDashing).Change((Action<bool>) (on => SaveData.Instance.Assists.SuperDashing = on)));
      menu.Add((TextMenu.Item) new TextMenu.OnOff(Dialog.Clean("MENU_VARIANT_HICCUPS", (Language) null), SaveData.Instance.Assists.Hiccups).Change((Action<bool>) (on => SaveData.Instance.Assists.Hiccups = on)));
      menu.Add((TextMenu.Item) new TextMenu.SubHeader(Dialog.Clean("MENU_ASSIST_SUBTITLE", (Language) null)));
      menu.Add((TextMenu.Item) new TextMenu.OnOff(Dialog.Clean("MENU_ASSIST_INFINITE_STAMINA", (Language) null), SaveData.Instance.Assists.InfiniteStamina).Change((Action<bool>) (on => SaveData.Instance.Assists.InfiniteStamina = on)));
      TextMenu textMenu2 = menu;
      string label2 = Dialog.Clean("MENU_ASSIST_AIR_DASHES", (Language) null);
      int dashMode = (int) SaveData.Instance.Assists.DashMode;
      TextMenu.Option<int> option1;
      TextMenu.Option<int> option2 = option1 = new TextMenu.Slider(label2, (Func<int, string>) (i =>
      {
        if (i == 0)
          return Dialog.Clean("MENU_ASSIST_AIR_DASHES_NORMAL", (Language) null);
        if (i == 1)
          return Dialog.Clean("MENU_ASSIST_AIR_DASHES_TWO", (Language) null);
        return Dialog.Clean("MENU_ASSIST_AIR_DASHES_INFINITE", (Language) null);
      }), 0, 2, dashMode).Change((Action<int>) (on =>
      {
        SaveData.Instance.Assists.DashMode = (Assists.DashModes) on;
        Player entity = this.Tracker.GetEntity<Player>();
        if (entity == null)
          return;
        entity.Dashes = Math.Min(entity.Dashes, entity.MaxDashes);
      }));
      textMenu2.Add((TextMenu.Item) option1);
      if (this.Session.Area.ID == 0)
        option2.Disabled = true;
      menu.Add((TextMenu.Item) new TextMenu.OnOff(Dialog.Clean("MENU_ASSIST_INVINCIBLE", (Language) null), SaveData.Instance.Assists.Invincible).Change((Action<bool>) (on => SaveData.Instance.Assists.Invincible = on)));
      menu.OnESC = menu.OnCancel = (Action) (() =>
      {
        Audio.Play("event:/ui/main/button_back");
        this.Pause(returnIndex, minimal, false);
        menu.Close();
      });
      menu.OnPause = (Action) (() =>
      {
        Audio.Play("event:/ui/main/button_back");
        this.Paused = false;
        Engine.FreezeTimer = 0.15f;
        menu.Close();
      });
      this.Add((Entity) menu);
    }

    public void SnapColorGrade(string next)
    {
      if (!(this.Session.ColorGrade != next))
        return;
      this.lastColorGrade = next;
      this.colorGradeEase = 0.0f;
      this.Session.ColorGrade = next;
    }

    public void NextColorGrade(string next)
    {
      if (!(this.Session.ColorGrade != next))
        return;
      this.colorGradeEase = 0.0f;
      this.Session.ColorGrade = next;
    }

    public void Shake(float time = 0.3f)
    {
      if (Settings.Instance.DisableScreenShake)
        return;
      this.shakeDirection = Vector2.get_Zero();
      this.shakeTimer = Math.Max(this.shakeTimer, time);
    }

    public void StopShake()
    {
      this.shakeTimer = 0.0f;
    }

    public void DirectionalShake(Vector2 dir, float time = 0.3f)
    {
      if (Settings.Instance.DisableScreenShake)
        return;
      this.shakeDirection = dir.SafeNormalize();
      this.lastDirectionalShake = 0;
      this.shakeTimer = Math.Max(this.shakeTimer, time);
    }

    public void Flash(Color color, bool drawPlayerOver = false)
    {
      if (Settings.Instance.DisableFlashes)
        return;
      this.doFlash = true;
      this.flashDrawPlayer = drawPlayerOver;
      this.flash = 1f;
      this.flashColor = color;
    }

    public void ZoomSnap(Vector2 screenSpaceFocusPoint, float zoom)
    {
      this.ZoomFocusPoint = screenSpaceFocusPoint;
      this.ZoomTarget = this.Zoom = zoom;
    }

    public IEnumerator ZoomTo(Vector2 screenSpaceFocusPoint, float zoom, float duration)
    {
      this.ZoomFocusPoint = screenSpaceFocusPoint;
      this.ZoomTarget = zoom;
      float from = this.Zoom;
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime / duration)
      {
        this.Zoom = MathHelper.Lerp(from, this.ZoomTarget, Ease.SineInOut(MathHelper.Clamp(p, 0.0f, 1f)));
        yield return (object) null;
      }
      this.Zoom = this.ZoomTarget;
    }

    public IEnumerator ZoomAcross(
      Vector2 screenSpaceFocusPoint,
      float zoom,
      float duration)
    {
      float fromZoom = this.Zoom;
      Vector2 fromFocus = this.ZoomFocusPoint;
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime / duration)
      {
        float num = Ease.SineInOut(MathHelper.Clamp(p, 0.0f, 1f));
        this.Zoom = this.ZoomTarget = MathHelper.Lerp(fromZoom, zoom, num);
        this.ZoomFocusPoint = Vector2.Lerp(fromFocus, screenSpaceFocusPoint, num);
        yield return (object) null;
      }
      this.Zoom = this.ZoomTarget;
      this.ZoomFocusPoint = screenSpaceFocusPoint;
    }

    public IEnumerator ZoomBack(float duration)
    {
      float from = this.Zoom;
      float to = 1f;
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime / duration)
      {
        this.Zoom = MathHelper.Lerp(from, to, Ease.SineInOut(MathHelper.Clamp(p, 0.0f, 1f)));
        yield return (object) null;
      }
      this.ResetZoom();
    }

    public void ResetZoom()
    {
      this.Zoom = 1f;
      this.ZoomTarget = 1f;
      this.ZoomFocusPoint = Vector2.op_Division(new Vector2(320f, 180f), 2f);
    }

    public void DoScreenWipe(bool wipeIn, Action onComplete = null, bool hiresSnow = false)
    {
      AreaData.Get(this.Session).DoScreenWipe((Scene) this, wipeIn, onComplete);
      if (!hiresSnow)
        return;
      this.Add((Monocle.Renderer) (this.HiresSnow = new HiresSnow(0.45f)));
      this.HiresSnow.Alpha = 0.0f;
      this.HiresSnow.AttachAlphaTo = this.Wipe;
    }

    public Session.CoreModes CoreMode
    {
      get
      {
        return this.coreMode;
      }
      set
      {
        if (this.coreMode == value)
          return;
        this.coreMode = value;
        this.Session.SetFlag("cold", this.coreMode == Session.CoreModes.Cold);
        Audio.SetParameter(Audio.CurrentAmbienceEventInstance, "room_state", this.coreMode == Session.CoreModes.Hot ? 0.0f : 1f);
        if (Audio.CurrentMusic == "event:/music/lvl9/main")
        {
          this.Session.Audio.Music.Layer(1, this.coreMode == Session.CoreModes.Hot);
          this.Session.Audio.Music.Layer(2, this.coreMode == Session.CoreModes.Cold);
          this.Session.Audio.Apply();
        }
        foreach (CoreModeListener component in this.Tracker.GetComponents<CoreModeListener>())
        {
          if (component.OnChange != null)
            component.OnChange(value);
        }
      }
    }

    public bool InsideCamera(Vector2 position, float expand = 0.0f)
    {
      if (position.X >= (double) this.Camera.Left - (double) expand && position.X < (double) this.Camera.Right + (double) expand && position.Y >= (double) this.Camera.Top - (double) expand)
        return position.Y < (double) this.Camera.Bottom + (double) expand;
      return false;
    }

    public void EnforceBounds(Player player)
    {
      Rectangle bounds = this.Bounds;
      Rectangle rectangle;
      ((Rectangle) ref rectangle).\u002Ector((int) this.Camera.Left, (int) this.Camera.Top, 320, 180);
      if (this.transition != null)
        return;
      if (this.CameraLockMode == Level.CameraLockModes.FinalBoss && (double) player.Left < (double) ((Rectangle) ref rectangle).get_Left())
      {
        player.Left = (float) ((Rectangle) ref rectangle).get_Left();
        player.OnBoundsH();
      }
      else if ((double) player.Left < (double) ((Rectangle) ref bounds).get_Left())
      {
        if ((double) player.Top >= (double) ((Rectangle) ref bounds).get_Top() && (double) player.Bottom < (double) ((Rectangle) ref bounds).get_Bottom() && this.Session.MapData.CanTransitionTo(this, Vector2.op_Addition(player.Center, Vector2.op_Multiply(Vector2.get_UnitX(), -8f))))
        {
          player.BeforeSideTransition();
          this.NextLevel(Vector2.op_Addition(player.Center, Vector2.op_Multiply(Vector2.get_UnitX(), -8f)), Vector2.op_UnaryNegation(Vector2.get_UnitX()));
          return;
        }
        player.Left = (float) ((Rectangle) ref bounds).get_Left();
        player.OnBoundsH();
      }
      TheoCrystal entity = this.Tracker.GetEntity<TheoCrystal>();
      if (this.CameraLockMode == Level.CameraLockModes.FinalBoss && (double) player.Right > (double) ((Rectangle) ref rectangle).get_Right() && ((Rectangle) ref rectangle).get_Right() < ((Rectangle) ref bounds).get_Right() - 4)
      {
        player.Right = (float) ((Rectangle) ref rectangle).get_Right();
        player.OnBoundsH();
      }
      else if (entity != null && (player.Holding == null || !player.Holding.IsHeld) && (double) player.Right > (double) (((Rectangle) ref bounds).get_Right() - 1))
        player.Right = (float) (((Rectangle) ref bounds).get_Right() - 1);
      else if ((double) player.Right > (double) ((Rectangle) ref bounds).get_Right())
      {
        if ((double) player.Top >= (double) ((Rectangle) ref bounds).get_Top() && (double) player.Bottom < (double) ((Rectangle) ref bounds).get_Bottom() && this.Session.MapData.CanTransitionTo(this, Vector2.op_Addition(player.Center, Vector2.op_Multiply(Vector2.get_UnitX(), 8f))))
        {
          player.BeforeSideTransition();
          this.NextLevel(Vector2.op_Addition(player.Center, Vector2.op_Multiply(Vector2.get_UnitX(), 8f)), Vector2.get_UnitX());
          return;
        }
        player.Right = (float) ((Rectangle) ref bounds).get_Right();
        player.OnBoundsH();
      }
      if (this.CameraLockMode != Level.CameraLockModes.None && (double) player.Top < (double) ((Rectangle) ref rectangle).get_Top())
      {
        player.Top = (float) ((Rectangle) ref rectangle).get_Top();
        player.OnBoundsV();
      }
      else if ((double) player.CenterY < (double) ((Rectangle) ref bounds).get_Top())
      {
        if (this.Session.MapData.CanTransitionTo(this, Vector2.op_Subtraction(player.Center, Vector2.op_Multiply(Vector2.get_UnitY(), 12f))))
        {
          player.BeforeUpTransition();
          this.NextLevel(Vector2.op_Subtraction(player.Center, Vector2.op_Multiply(Vector2.get_UnitY(), 12f)), Vector2.op_UnaryNegation(Vector2.get_UnitY()));
          return;
        }
        if ((double) player.Top < (double) (((Rectangle) ref bounds).get_Top() - 24))
        {
          player.Top = (float) (((Rectangle) ref bounds).get_Top() - 24);
          player.OnBoundsV();
        }
      }
      if (this.CameraLockMode != Level.CameraLockModes.None && ((Rectangle) ref rectangle).get_Bottom() < ((Rectangle) ref bounds).get_Bottom() - 4 && (double) player.Top > (double) ((Rectangle) ref rectangle).get_Bottom())
      {
        if (SaveData.Instance.Assists.Invincible)
        {
          player.Play("event:/game/general/assist_screenbottom", (string) null, 0.0f);
          player.Bounce((float) ((Rectangle) ref rectangle).get_Bottom());
        }
        else
          player.Die(Vector2.get_Zero(), false, true);
      }
      else if ((double) player.Bottom > (double) ((Rectangle) ref bounds).get_Bottom() && this.Session.MapData.CanTransitionTo(this, Vector2.op_Addition(player.Center, Vector2.op_Multiply(Vector2.get_UnitY(), 12f))) && !this.Session.LevelData.DisableDownTransition)
      {
        if (player.CollideCheck<Solid>(Vector2.op_Addition(player.Position, Vector2.op_Multiply(Vector2.get_UnitY(), 4f))))
          return;
        player.BeforeDownTransition();
        this.NextLevel(Vector2.op_Addition(player.Center, Vector2.op_Multiply(Vector2.get_UnitY(), 12f)), Vector2.get_UnitY());
      }
      else
      {
        if ((double) player.Top <= (double) ((Rectangle) ref bounds).get_Bottom())
          return;
        if (SaveData.Instance.Assists.Invincible)
        {
          player.Play("event:/game/general/assist_screenbottom", (string) null, 0.0f);
          player.Bounce((float) ((Rectangle) ref bounds).get_Bottom());
        }
        else
          player.Die(Vector2.get_Zero(), false, true);
      }
    }

    public bool IsInBounds(Entity entity)
    {
      Rectangle bounds = this.Bounds;
      if ((double) entity.Right > (double) ((Rectangle) ref bounds).get_Left() && (double) entity.Bottom > (double) ((Rectangle) ref bounds).get_Top() && (double) entity.Left < (double) ((Rectangle) ref bounds).get_Right())
        return (double) entity.Top < (double) ((Rectangle) ref bounds).get_Bottom();
      return false;
    }

    public bool IsInBounds(Vector2 position)
    {
      Rectangle bounds = this.Bounds;
      if (position.X >= (double) ((Rectangle) ref bounds).get_Left() && position.Y >= (double) ((Rectangle) ref bounds).get_Top() && position.X < (double) ((Rectangle) ref bounds).get_Right())
        return position.Y < (double) ((Rectangle) ref bounds).get_Bottom();
      return false;
    }

    public bool IsInBounds(Vector2 position, float pad)
    {
      Rectangle bounds = this.Bounds;
      if (position.X >= (double) ((Rectangle) ref bounds).get_Left() - (double) pad && position.Y >= (double) ((Rectangle) ref bounds).get_Top() - (double) pad && position.X < (double) ((Rectangle) ref bounds).get_Right() + (double) pad)
        return position.Y < (double) ((Rectangle) ref bounds).get_Bottom() + (double) pad;
      return false;
    }

    public bool IsInBounds(Vector2 position, Vector2 dirPad)
    {
      float num1 = Math.Max((float) dirPad.X, 0.0f);
      float num2 = Math.Max((float) -dirPad.X, 0.0f);
      float num3 = Math.Max((float) dirPad.Y, 0.0f);
      float num4 = Math.Max((float) -dirPad.Y, 0.0f);
      Rectangle bounds = this.Bounds;
      if (position.X >= (double) ((Rectangle) ref bounds).get_Left() + (double) num1 && position.Y >= (double) ((Rectangle) ref bounds).get_Top() + (double) num3 && position.X < (double) ((Rectangle) ref bounds).get_Right() - (double) num2)
        return position.Y < (double) ((Rectangle) ref bounds).get_Bottom() - (double) num4;
      return false;
    }

    public bool IsInCamera(Vector2 position, float pad)
    {
      Rectangle rectangle;
      ((Rectangle) ref rectangle).\u002Ector((int) this.Camera.X, (int) this.Camera.Y, 320, 180);
      if (position.X >= (double) ((Rectangle) ref rectangle).get_Left() - (double) pad && position.Y >= (double) ((Rectangle) ref rectangle).get_Top() - (double) pad && position.X < (double) ((Rectangle) ref rectangle).get_Right() + (double) pad)
        return position.Y < (double) ((Rectangle) ref rectangle).get_Bottom() + (double) pad;
      return false;
    }

    public void StartCutscene(
      Action<Level> onSkip,
      bool fadeInOnSkip = true,
      bool endingChapterAfterCutscene = false)
    {
      this.endingChapterAfterCutscene = endingChapterAfterCutscene;
      this.InCutscene = true;
      this.onCutsceneSkip = onSkip;
      this.onCutsceneSkipFadeIn = fadeInOnSkip;
    }

    public void CancelCutscene()
    {
      this.InCutscene = false;
      this.SkippingCutscene = false;
    }

    public void SkipCutscene()
    {
      this.SkippingCutscene = true;
      Engine.TimeRate = 1f;
      Distort.Anxiety = 0.0f;
      Distort.GameRate = 1f;
      if (this.endingChapterAfterCutscene)
        Audio.BusStopAll("bus:/gameplay_sfx", true);
      List<Entity> entityList = new List<Entity>();
      foreach (Entity entity in this.Tracker.GetEntities<Textbox>())
        entityList.Add(entity);
      foreach (Entity entity in entityList)
        entity.RemoveSelf();
      this.skipCoroutine = new Coroutine(this.SkipCutsceneRoutine(), true);
    }

    private IEnumerator SkipCutsceneRoutine()
    {
      Level level = this;
      FadeWipe fadeWipe1 = new FadeWipe((Scene) level, false, (Action) null);
      fadeWipe1.Duration = 0.25f;
      yield return (object) fadeWipe1.Wait();
      level.onCutsceneSkip(level);
      level.strawberriesDisplay.DrawLerp = 0.0f;
      level.ResetZoom();
      GameplayStats first = level.Entities.FindFirst<GameplayStats>();
      if (first != null)
        first.DrawLerp = 0.0f;
      if (level.onCutsceneSkipFadeIn)
      {
        FadeWipe fadeWipe2 = new FadeWipe((Scene) level, true, (Action) null);
        fadeWipe2.Duration = 0.25f;
        level.RendererList.UpdateLists();
        yield return (object) fadeWipe2.Wait();
      }
      level.SkippingCutscene = false;
      level.EndCutscene();
    }

    public void EndCutscene()
    {
      if (this.SkippingCutscene)
        return;
      this.InCutscene = false;
    }

    private void NextLevel(Vector2 at, Vector2 dir)
    {
      this.OnEndOfFrame += (Action) (() =>
      {
        Engine.TimeRate = 1f;
        Distort.Anxiety = 0.0f;
        Distort.GameRate = 1f;
        this.TransitionTo(this.Session.MapData.GetAt(at), dir);
      });
    }

    public void RegisterAreaComplete()
    {
      if (this.Completed)
        return;
      Player entity = this.Tracker.GetEntity<Player>();
      if (entity != null)
      {
        List<Strawberry> strawberryList = new List<Strawberry>();
        foreach (Follower follower in entity.Leader.Followers)
        {
          if (follower.Entity is Strawberry)
            strawberryList.Add(follower.Entity as Strawberry);
        }
        foreach (Strawberry strawberry in strawberryList)
          strawberry.OnCollect();
      }
      this.Completed = true;
      SaveData.Instance.RegisterCompletion(this.Session);
    }

    public ScreenWipe CompleteArea(bool spotlightWipe = true, bool skipScreenWipe = false)
    {
      this.RegisterAreaComplete();
      this.PauseLock = true;
      Action onComplete = !AreaData.Get(this.Session).Interlude ? (Action) (() => Engine.Scene = (Scene) new LevelExit(LevelExit.Mode.Completed, this.Session, (HiresSnow) null)) : (Action) (() => Engine.Scene = (Scene) new LevelExit(LevelExit.Mode.CompletedInterlude, this.Session, this.HiresSnow));
      if (!this.SkippingCutscene && !skipScreenWipe)
      {
        if (!spotlightWipe)
          return (ScreenWipe) new FadeWipe((Scene) this, false, onComplete);
        Player entity = this.Tracker.GetEntity<Player>();
        if (entity != null)
          SpotlightWipe.FocusPoint = Vector2.op_Subtraction(Vector2.op_Subtraction(entity.Position, this.Camera.Position), new Vector2(0.0f, 8f));
        return (ScreenWipe) new SpotlightWipe((Scene) this, false, onComplete);
      }
      Audio.BusStopAll("bus:/gameplay_sfx", true);
      onComplete();
      return (ScreenWipe) null;
    }

    public enum CameraLockModes
    {
      None,
      BoostSequence,
      FinalBoss,
      FinalBossNoY,
      Lava,
    }

    private enum ConditionBlockModes
    {
      Key,
      Button,
      Strawberry,
    }
  }
}
