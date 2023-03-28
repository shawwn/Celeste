// Decompiled with JetBrains decompiler
// Type: Celeste.Level
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

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
    public bool Completed;
    public bool NewLevel;
    public bool TimerStarted;
    public bool TimerStopped;
    public bool TimerHidden;
    public Session Session;
    public Vector2? StartPosition;
    public bool DarkRoom;
    public Player.IntroTypes LastIntroType;
    public bool InCredits;
    public bool AllowHudHide = true;
    public VirtualMap<char> SolidsData;
    public VirtualMap<char> BgData;
    public float NextTransitionDuration = 0.65f;
    public const float DefaultTransitionDuration = 0.65f;
    public ScreenWipe Wipe;
    private Coroutine transition;
    private Coroutine saving;
    public FormationBackdrop FormationBackdrop;
    public SolidTiles SolidTiles;
    public BackgroundTiles BgTiles;
    public Color BackgroundColor = Color.Black;
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
    private Vector2 shakeDirection;
    private int lastDirectionalShake;
    private float shakeTimer;
    private Vector2 cameraPreShake;
    public float ScreenPadding;
    private float flash;
    private Color flashColor = Color.White;
    private bool doFlash;
    private bool flashDrawPlayer;
    private float glitchTimer;
    private float glitchSeed;
    public float Zoom = 1f;
    public float ZoomTarget = 1f;
    public Vector2 ZoomFocusPoint;
    private string lastColorGrade;
    private float colorGradeEase;
    private float colorGradeEaseSpeed = 1f;
    public Vector2 Wind;
    public float WindSine;
    public float WindSineTimer;
    public bool Frozen;
    public bool PauseLock;
    public bool CanRetry = true;
    public bool PauseMainMenuOpen;
    private bool wasPaused;
    private float wasPausedTimer;
    private float unpauseTimer;
    public bool SaveQuitDisabled;
    public bool InCutscene;
    public bool SkippingCutscene;
    private Coroutine skipCoroutine;
    private Action<Level> onCutsceneSkip;
    private bool onCutsceneSkipFadeIn;
    private bool onCutsceneSkipResetZoom;
    private bool endingChapterAfterCutscene;
    public static EventInstance DialogSnapshot;
    private static EventInstance PauseSnapshot;
    private static EventInstance AssistSpeedSnapshot;
    private static int AssistSpeedSnapshotValue = -1;
    public Pathfinder Pathfinder;
    public PlayerDeadBody RetryPlayerCorpse;
    public float BaseLightingAlpha;
    private bool updateHair = true;
    public bool InSpace;
    public bool HasCassetteBlocks;
    public float CassetteBlockTempo;
    public int CassetteBlockBeats;
    public Random HiccupRandom;
    public bool Raining;
    private Session.CoreModes coreMode;

    public Vector2 LevelOffset
    {
      get
      {
        Rectangle bounds = this.Bounds;
        double left = (double) bounds.Left;
        bounds = this.Bounds;
        double top = (double) bounds.Top;
        return new Vector2((float) left, (float) top);
      }
    }

    public Point LevelSolidOffset
    {
      get
      {
        Rectangle bounds = this.Bounds;
        int x = bounds.Left / 8 - this.TileBounds.X;
        bounds = this.Bounds;
        int y = bounds.Top / 8 - this.TileBounds.Y;
        return new Point(x, y);
      }
    }

    public Rectangle TileBounds => this.Session.MapData.TileBounds;

    public bool Transitioning => this.transition != null;

    public Vector2 ShakeVector { get; private set; }

    public float VisualWind => this.Wind.X + this.WindSine;

    public bool FrozenOrPaused => this.Frozen || this.Paused;

    public bool CanPause
    {
      get
      {
        Player entity = this.Tracker.GetEntity<Player>();
        if (entity == null || entity.Dead || this.wasPaused || this.Paused || this.PauseLock || this.SkippingCutscene || this.Transitioning || this.Wipe != null || UserIO.Saving)
          return false;
        return entity.LastBooster == null || !entity.LastBooster.Ch9HubTransition || !entity.LastBooster.BoostingPlayer;
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
        return this.Tracker.GetEntity<Textbox>() == null && this.Tracker.GetEntity<MiniTextbox>() == null && !this.Frozen && !this.InCutscene;
      }
    }

    public override void Begin()
    {
      ScreenWipe.WipeColor = Color.Black;
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
      Audio.BusStopAll("bus:/gameplay_sfx");
      Audio.MusicUnderwater = false;
      Audio.SetAmbience((string) null);
      Audio.SetAltMusic((string) null);
      Audio.EndSnapshot(Level.DialogSnapshot);
      Audio.ReleaseSnapshot(Level.AssistSpeedSnapshot);
      Level.AssistSpeedSnapshot = (EventInstance) null;
      Level.AssistSpeedSnapshotValue = -1;
      GameplayBuffers.Unload();
      ClutterBlockGenerator.Dispose();
      Engine.TimeRateB = 1f;
    }

    public void LoadLevel(Player.IntroTypes playerIntro, bool isFromLoader = false)
    {
      this.TimerHidden = false;
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
      this.colorGradeEaseSpeed = 1f;
      this.HasCassetteBlocks = false;
      this.CassetteBlockTempo = 1f;
      this.CassetteBlockBeats = 2;
      this.Raining = false;
      bool flag1 = false;
      bool flag2 = false;
      if (this.HiccupRandom == null)
        this.HiccupRandom = new Random(this.Session.Area.ID * 77 + (int) this.Session.Area.Mode * 999);
      this.Entities.FindFirst<LightningRenderer>()?.Reset();
      Calc.PushRandom(this.Session.LevelData.LoadSeed);
      MapData mapData = this.Session.MapData;
      LevelData levelData = this.Session.LevelData;
      Vector2 vector2_1 = new Vector2((float) levelData.Bounds.Left, (float) levelData.Bounds.Top);
      bool flag3 = playerIntro != Player.IntroTypes.Fall || levelData.Name == "0";
      this.DarkRoom = levelData.Dark && !this.Session.GetFlag("ignore_darkness_" + levelData.Name);
      this.Zoom = 1f;
      if (this.Session.Audio == null)
        this.Session.Audio = AreaData.Get(this.Session).Mode[(int) this.Session.Area.Mode].AudioState.Clone();
      if (!levelData.DelayAltMusic)
        Audio.SetAltMusic(SFX.EventnameByHandle(levelData.AltMusic));
      if (levelData.Music.Length > 0)
        this.Session.Audio.Music.Event = SFX.EventnameByHandle(levelData.Music);
      if (!AreaData.GetMode(this.Session.Area).IgnoreLevelAudioLayerData)
      {
        for (int index = 0; index < 4; ++index)
          this.Session.Audio.Music.Layer(index + 1, levelData.MusicLayers[index]);
      }
      if (levelData.MusicProgress >= 0)
        this.Session.Audio.Music.Progress = levelData.MusicProgress;
      this.Session.Audio.Music.Layer(6, levelData.MusicWhispers);
      if (levelData.Ambience.Length > 0)
        this.Session.Audio.Ambience.Event = SFX.EventnameByHandle(levelData.Ambience);
      if (levelData.AmbienceProgress >= 0)
        this.Session.Audio.Ambience.Progress = levelData.AmbienceProgress;
      this.Session.Audio.Apply(isFromLoader);
      this.CoreMode = this.Session.CoreMode;
      this.NewLevel = !this.Session.LevelFlags.Contains(levelData.Name);
      if (flag3)
      {
        if (!this.Session.LevelFlags.Contains(levelData.Name))
          this.Session.FurthestSeenLevel = levelData.Name;
        this.Session.LevelFlags.Add(levelData.Name);
        this.Session.UpdateLevelStartDashes();
      }
      Vector2? nullable = new Vector2?();
      this.CameraOffset = new Vector2(48f, 32f) * levelData.CameraOffset;
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
      List<EntityID> entityIdList = new List<EntityID>();
      Player entity1 = this.Tracker.GetEntity<Player>();
      if (entity1 != null)
      {
        foreach (Follower follower in entity1.Leader.Followers)
          entityIdList.Add(follower.ParentEntityID);
      }
      foreach (EntityData entity2 in levelData.Entities)
      {
        int id = entity2.ID;
        EntityID entityId = new EntityID(levelData.Name, id);
        if (!this.Session.DoNotLoad.Contains(entityId) && !entityIdList.Contains(entityId))
        {
          switch (entity2.Name)
          {
            case "SoundTest3d":
              this.Add((Entity) new _3dSoundTest(entity2, vector2_1));
              continue;
            case "SummitBackgroundManager":
              this.Add((Entity) new AscendManager(entity2, vector2_1));
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
            case "birdPath":
              this.Add((Entity) new BirdPath(entityId, entity2, vector2_1));
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
              CassetteBlock cassetteBlock = new CassetteBlock(entity2, vector2_1, entityId);
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
              if (flag3)
              {
                Checkpoint checkpoint = new Checkpoint(entity2, vector2_1);
                this.Add((Entity) checkpoint);
                nullable = new Vector2?(entity2.Position + vector2_1 + checkpoint.SpawnOffset);
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
              Level.ConditionBlockModes conditionBlockModes = entity2.Enum<Level.ConditionBlockModes>("condition");
              EntityID none = EntityID.None;
              string[] strArray = entity2.Attr("conditionID").Split(':');
              none.Level = strArray[0];
              none.ID = Convert.ToInt32(strArray[1]);
              bool flag4;
              switch (conditionBlockModes)
              {
                case Level.ConditionBlockModes.Key:
                  flag4 = this.Session.DoNotLoad.Contains(none);
                  break;
                case Level.ConditionBlockModes.Button:
                  flag4 = this.Session.GetFlag(DashSwitch.GetFlagName(none));
                  break;
                case Level.ConditionBlockModes.Strawberry:
                  flag4 = this.Session.Strawberries.Contains(none);
                  break;
                default:
                  throw new Exception("Condition type not supported!");
              }
              if (flag4)
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
            case "crumbleWallOnRumble":
              this.Add((Entity) new CrumbleWallOnRumble(entity2, vector2_1, entityId));
              continue;
            case "crushBlock":
              this.Add((Entity) new CrushBlock(entity2, vector2_1));
              continue;
            case "cutsceneNode":
              this.Add((Entity) new CutsceneNode(entity2, vector2_1));
              continue;
            case "darkChaser":
              this.Add((Entity) new BadelineOldsite(entity2, vector2_1, index1));
              ++index1;
              continue;
            case "dashBlock":
              this.Add((Entity) new DashBlock(entity2, vector2_1, entityId));
              continue;
            case "dashSwitchH":
            case "dashSwitchV":
              this.Add((Entity) DashSwitch.Create(entity2, vector2_1, entityId));
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
              this.Add((Entity) new DreamMirror(vector2_1 + entity2.Position));
              continue;
            case "exitBlock":
              this.Add((Entity) new ExitBlock(entity2, vector2_1));
              continue;
            case "eyebomb":
              this.Add((Entity) new Puffer(entity2, vector2_1));
              continue;
            case "fakeBlock":
              this.Add((Entity) new FakeWall(entityId, entity2, vector2_1, FakeWall.Modes.Block));
              continue;
            case "fakeHeart":
              this.Add((Entity) new FakeHeart(entity2, vector2_1));
              continue;
            case "fakeWall":
              this.Add((Entity) new FakeWall(entityId, entity2, vector2_1, FakeWall.Modes.Wall));
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
            case "flingBird":
              this.Add((Entity) new FlingBird(entity2, vector2_1));
              continue;
            case "flingBirdIntro":
              this.Add((Entity) new FlingBirdIntro(entity2, vector2_1));
              continue;
            case "floatingDebris":
              this.Add((Entity) new FloatingDebris(entity2, vector2_1));
              continue;
            case "floatySpaceBlock":
              this.Add((Entity) new FloatySpaceBlock(entity2, vector2_1));
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
            case "glider":
              this.Add((Entity) new Glider(entity2, vector2_1));
              continue;
            case "goldenBerry":
              int num1 = SaveData.Instance.CheatMode ? 1 : 0;
              bool flag5 = this.Session.FurthestSeenLevel == this.Session.Level || this.Session.Deaths == 0;
              bool flag6 = SaveData.Instance.UnlockedModes >= 3 || SaveData.Instance.DebugMode;
              bool completed = SaveData.Instance.Areas[this.Session.Area.ID].Modes[(int) this.Session.Area.Mode].Completed;
              if (((num1 != 0 ? 1 : (flag6 & completed ? 1 : 0)) & (flag5 ? 1 : 0)) != 0)
              {
                this.Add((Entity) new Strawberry(entity2, vector2_1, entityId));
                continue;
              }
              continue;
            case "goldenBlock":
              this.Add((Entity) new GoldenBlock(entity2, vector2_1));
              continue;
            case "gondola":
              this.Add((Entity) new Gondola(entity2, vector2_1));
              continue;
            case "greenBlocks":
              ClutterBlockGenerator.Init(this);
              ClutterBlockGenerator.Add((int) ((double) entity2.Position.X / 8.0), (int) ((double) entity2.Position.Y / 8.0), entity2.Width / 8, entity2.Height / 8, ClutterBlock.Colors.Green);
              continue;
            case "hahaha":
              this.Add((Entity) new Hahaha(entity2, vector2_1));
              continue;
            case "hanginglamp":
              this.Add((Entity) new HangingLamp(entity2, vector2_1 + entity2.Position));
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
            case "kevins_pc":
              this.Add((Entity) new KevinsPC(entity2, vector2_1));
              continue;
            case "key":
              this.Add((Entity) new Key(entity2, vector2_1, entityId));
              continue;
            case "killbox":
              this.Add((Entity) new Killbox(entity2, vector2_1));
              continue;
            case "lamp":
              this.Add((Entity) new Lamp(vector2_1 + entity2.Position, entity2.Bool("broken")));
              continue;
            case "light":
              this.Add((Entity) new PropLight(entity2, vector2_1));
              continue;
            case "lightbeam":
              this.Add((Entity) new LightBeam(entity2, vector2_1));
              continue;
            case "lightning":
              if (entity2.Bool("perLevel") || !this.Session.GetFlag("disable_lightning"))
              {
                this.Add((Entity) new Lightning(entity2, vector2_1));
                flag2 = true;
                continue;
              }
              continue;
            case "lightningBlock":
              this.Add((Entity) new LightningBreakerBox(entity2, vector2_1));
              continue;
            case "lockBlock":
              this.Add((Entity) new LockBlock(entity2, vector2_1, entityId));
              continue;
            case "memorial":
              this.Add((Entity) new Memorial(entity2, vector2_1));
              continue;
            case "memorialTextController":
              if (this.Session.Dashes == 0 && this.Session.StartedFromBeginning)
              {
                this.Add((Entity) new Strawberry(entity2, vector2_1, entityId));
                continue;
              }
              continue;
            case "moonCreature":
              this.Add((Entity) new MoonCreature(entity2, vector2_1));
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
              string lower = entity2.Attr("npc").ToLower();
              Vector2 position = entity2.Position + vector2_1;
              switch (lower)
              {
                case "granny_00_house":
                  this.Add((Entity) new NPC00_Granny(position));
                  continue;
                case "theo_01_campfire":
                  this.Add((Entity) new NPC01_Theo(position));
                  continue;
                case "theo_02_campfire":
                  this.Add((Entity) new NPC02_Theo(position));
                  continue;
                case "theo_03_escaping":
                  if (!this.Session.GetFlag("resort_theo"))
                  {
                    this.Add((Entity) new NPC03_Theo_Escaping(position));
                    continue;
                  }
                  continue;
                case "theo_03_vents":
                  this.Add((Entity) new NPC03_Theo_Vents(position));
                  continue;
                case "oshiro_03_lobby":
                  this.Add((Entity) new NPC03_Oshiro_Lobby(position));
                  continue;
                case "oshiro_03_hallway":
                  this.Add((Entity) new NPC03_Oshiro_Hallway1(position));
                  continue;
                case "oshiro_03_hallway2":
                  this.Add((Entity) new NPC03_Oshiro_Hallway2(position));
                  continue;
                case "oshiro_03_bigroom":
                  this.Add((Entity) new NPC03_Oshiro_Cluttter(entity2, vector2_1));
                  continue;
                case "oshiro_03_breakdown":
                  this.Add((Entity) new NPC03_Oshiro_Breakdown(position));
                  continue;
                case "oshiro_03_suite":
                  this.Add((Entity) new NPC03_Oshiro_Suite(position));
                  continue;
                case "oshiro_03_rooftop":
                  this.Add((Entity) new NPC03_Oshiro_Rooftop(position));
                  continue;
                case "granny_04_cliffside":
                  this.Add((Entity) new NPC04_Granny(position));
                  continue;
                case "theo_04_cliffside":
                  this.Add((Entity) new NPC04_Theo(position));
                  continue;
                case "theo_05_entrance":
                  this.Add((Entity) new NPC05_Theo_Entrance(position));
                  continue;
                case "theo_05_inmirror":
                  this.Add((Entity) new NPC05_Theo_Mirror(position));
                  continue;
                case "evil_05":
                  this.Add((Entity) new NPC05_Badeline(entity2, vector2_1));
                  continue;
                case "theo_06_plateau":
                  this.Add((Entity) new NPC06_Theo_Plateau(entity2, vector2_1));
                  continue;
                case "granny_06_intro":
                  this.Add((Entity) new NPC06_Granny(entity2, vector2_1));
                  continue;
                case "badeline_06_crying":
                  this.Add((Entity) new NPC06_Badeline_Crying(entity2, vector2_1));
                  continue;
                case "granny_06_ending":
                  this.Add((Entity) new NPC06_Granny_Ending(entity2, vector2_1));
                  continue;
                case "theo_06_ending":
                  this.Add((Entity) new NPC06_Theo_Ending(entity2, vector2_1));
                  continue;
                case "granny_07x":
                  this.Add((Entity) new NPC07X_Granny_Ending(entity2, vector2_1));
                  continue;
                case "theo_08_inside":
                  this.Add((Entity) new NPC08_Theo(entity2, vector2_1));
                  continue;
                case "granny_08_inside":
                  this.Add((Entity) new NPC08_Granny(entity2, vector2_1));
                  continue;
                case "granny_09_outside":
                  this.Add((Entity) new NPC09_Granny_Outside(entity2, vector2_1));
                  continue;
                case "granny_09_inside":
                  this.Add((Entity) new NPC09_Granny_Inside(entity2, vector2_1));
                  continue;
                case "gravestone_10":
                  this.Add((Entity) new NPC10_Gravestone(entity2, vector2_1));
                  continue;
                case "granny_10_never":
                  this.Add((Entity) new NPC07X_Granny_Ending(entity2, vector2_1, true));
                  continue;
                default:
                  continue;
              }
            case "oshirodoor":
              this.Add((Entity) new MrOshiroDoor(entity2, vector2_1));
              continue;
            case "payphone":
              this.Add((Entity) new Payphone(vector2_1 + entity2.Position));
              continue;
            case "picoconsole":
              this.Add((Entity) new PicoConsole(entity2, vector2_1));
              continue;
            case "plateau":
              this.Add((Entity) new Plateau(entity2, vector2_1));
              continue;
            case "playbackBillboard":
              this.Add((Entity) new PlaybackBillboard(entity2, vector2_1));
              continue;
            case "playbackTutorial":
              this.Add((Entity) new PlayerPlayback(entity2, vector2_1));
              continue;
            case "playerSeeker":
              this.Add((Entity) new PlayerSeeker(entity2, vector2_1));
              continue;
            case "powerSourceNumber":
              this.Add((Entity) new PowerSourceNumber(entity2.Position + vector2_1, entity2.Int("number", 1), this.GotCollectables(entity2)));
              continue;
            case "redBlocks":
              ClutterBlockGenerator.Init(this);
              ClutterBlockGenerator.Add((int) ((double) entity2.Position.X / 8.0), (int) ((double) entity2.Position.Y / 8.0), entity2.Width / 8, entity2.Height / 8, ClutterBlock.Colors.Red);
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
              if (this.GotCollectables(entity2))
              {
                this.Add((Entity) new RidgeGate(entity2, vector2_1));
                continue;
              }
              continue;
            case "risingLava":
              this.Add((Entity) new RisingLava(entity2, vector2_1));
              continue;
            case "rotateSpinner":
              if (this.Session.Area.ID == 10)
              {
                this.Add((Entity) new StarRotateSpinner(entity2, vector2_1));
                continue;
              }
              if (this.Session.Area.ID == 3 || this.Session.Area.ID == 7 && this.Session.Level.StartsWith("d-"))
              {
                this.Add((Entity) new DustRotateSpinner(entity2, vector2_1));
                continue;
              }
              this.Add((Entity) new BladeRotateSpinner(entity2, vector2_1));
              continue;
            case "rotatingPlatforms":
              Vector2 vector2_2 = entity2.Position + vector2_1;
              Vector2 center = entity2.Nodes[0] + vector2_1;
              int width = entity2.Width;
              int num2 = entity2.Int("platforms");
              bool clockwise = entity2.Bool("clockwise");
              float length = (vector2_2 - center).Length();
              float num3 = (vector2_2 - center).Angle();
              float num4 = 6.2831855f / (float) num2;
              for (int index2 = 0; index2 < num2; ++index2)
              {
                float angleRadians = Calc.WrapAngle(num3 + num4 * (float) index2);
                this.Add((Entity) new RotatingPlatform(center + Calc.AngleToVector(angleRadians, length), width, center, clockwise));
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
              else if (this.Session.Area.ID == 10)
                color = CrystalColor.Rainbow;
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
              this.Add((Entity) new Strawberry(entity2, vector2_1, entityId));
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
              this.Add((Entity) new SummitGem(entity2, vector2_1, entityId));
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
              this.Add((Entity) new TempleCrackedBlock(entityId, entity2, vector2_1));
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
              this.Add((Entity) new Torch(entity2, vector2_1, entityId));
              continue;
            case "touchSwitch":
              this.Add((Entity) new TouchSwitch(entity2, vector2_1));
              continue;
            case "towerviewer":
              this.Add((Entity) new Lookout(entity2, vector2_1));
              continue;
            case "trackSpinner":
              if (this.Session.Area.ID == 10)
              {
                this.Add((Entity) new StarTrackSpinner(entity2, vector2_1));
                continue;
              }
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
            case "wavedashmachine":
              this.Add((Entity) new WaveDashTutorialMachine(entity2, vector2_1));
              continue;
            case "whiteblock":
              this.Add((Entity) new WhiteBlock(entity2, vector2_1));
              continue;
            case "wire":
              this.Add((Entity) new Wire(entity2, vector2_1));
              continue;
            case "yellowBlocks":
              ClutterBlockGenerator.Init(this);
              ClutterBlockGenerator.Add((int) ((double) entity2.Position.X / 8.0), (int) ((double) entity2.Position.Y / 8.0), entity2.Width / 8, entity2.Height / 8, ClutterBlock.Colors.Yellow);
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
            case "birdPathTrigger":
              this.Add((Entity) new BirdPathTrigger(trigger, vector2_1));
              continue;
            case "blackholeStrength":
              this.Add((Entity) new BlackholeStrengthTrigger(trigger, vector2_1));
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
              string flag7 = trigger.Attr("deleteFlag");
              if (string.IsNullOrEmpty(flag7) || !this.Session.GetFlag(flag7))
              {
                this.Add((Entity) new CameraTargetTrigger(trigger, vector2_1));
                continue;
              }
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
            case "detachFollowersTrigger":
              this.Add((Entity) new DetachStrawberryTrigger(trigger, vector2_1));
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
            case "moonGlitchBackgroundTrigger":
              this.Add((Entity) new MoonGlitchBackgroundTrigger(trigger, vector2_1));
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
            case "rumbleTrigger":
              this.Add((Entity) new RumbleTrigger(trigger, vector2_1, id));
              continue;
            case "spawnFacingTrigger":
              this.Add((Entity) new SpawnFacingTrigger(trigger, vector2_1));
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
        this.Add((Entity) new Decal(fgDecal.Texture, vector2_1 + fgDecal.Position, fgDecal.Scale, -10500));
      foreach (DecalData bgDecal in levelData.BgDecals)
        this.Add((Entity) new Decal(bgDecal.Texture, vector2_1 + bgDecal.Position, bgDecal.Scale, 9000));
      if (playerIntro != Player.IntroTypes.Transition)
      {
        if (this.Session.JustStarted && !this.Session.StartedFromBeginning && nullable.HasValue && !this.StartPosition.HasValue)
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
        SpotlightWipe.FocusPoint = this.Session.RespawnPoint.Value - this.Camera.Position;
        if (playerIntro != Player.IntroTypes.Respawn && playerIntro != Player.IntroTypes.Fall)
        {
          SpotlightWipe spotlightWipe = new SpotlightWipe((Scene) this, true);
        }
        else
          this.DoScreenWipe(true);
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
        for (int index3 = 0; index3 < numArray.GetLength(0); ++index3)
        {
          for (int index4 = 0; index4 < numArray.GetLength(1); ++index4)
          {
            if (numArray[index3, index4] != -1)
              TileInterceptor.TileCheck((Scene) this, tileset[numArray[index3, index4]], new Vector2((float) (index3 * 8), (float) (index4 * 8)) + this.LevelOffset);
          }
        }
      }
      LightningRenderer entity3 = this.Tracker.GetEntity<LightningRenderer>();
      if (entity3 != null)
      {
        if (flag2)
          entity3.StartAmbience();
        else
          entity3.StopAmbience();
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
      if (this.Session.FirstLevel && this.Session.Strawberries.Count <= 0 && !this.Session.Cassette && !this.Session.HeartGem && !this.Session.HitCheckpoint)
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
      this.LoadLevel(Player.IntroTypes.Respawn);
      this.strawberriesDisplay.DrawLerp = 0.0f;
      WindController first = this.Entities.FindFirst<WindController>();
      if (first != null)
        first.SnapWind();
      else
        this.Wind = Vector2.Zero;
    }

    private bool ShouldCreateCassetteManager => this.Session.Area.Mode != AreaMode.Normal || !this.Session.Cassette;

    private bool GotCollectables(EntityData e)
    {
      bool flag1 = true;
      bool flag2 = true;
      List<EntityID> entityIdList1 = new List<EntityID>();
      if (e.Attr("strawberries").Length > 0)
      {
        string str1 = e.Attr("strawberries");
        char[] chArray1 = new char[1]{ ',' };
        foreach (string str2 in str1.Split(chArray1))
        {
          EntityID none = EntityID.None;
          char[] chArray2 = new char[1]{ ':' };
          string[] strArray = str2.Split(chArray2);
          none.Level = strArray[0];
          none.ID = Convert.ToInt32(strArray[1]);
          entityIdList1.Add(none);
        }
      }
      foreach (EntityID entityId in entityIdList1)
      {
        if (!this.Session.Strawberries.Contains(entityId))
        {
          flag1 = false;
          break;
        }
      }
      List<EntityID> entityIdList2 = new List<EntityID>();
      if (e.Attr("keys").Length > 0)
      {
        string str3 = e.Attr("keys");
        char[] chArray3 = new char[1]{ ',' };
        foreach (string str4 in str3.Split(chArray3))
        {
          EntityID none = EntityID.None;
          char[] chArray4 = new char[1]{ ':' };
          string[] strArray = str4.Split(chArray4);
          none.Level = strArray[0];
          none.ID = Convert.ToInt32(strArray[1]);
          entityIdList2.Add(none);
        }
      }
      foreach (EntityID entityId in entityIdList2)
      {
        if (!this.Session.DoNotLoad.Contains(entityId))
        {
          flag2 = false;
          break;
        }
      }
      return flag2 & flag1;
    }

    public void TransitionTo(LevelData next, Vector2 direction)
    {
      this.Session.CoreMode = this.CoreMode;
      this.transition = new Coroutine(this.TransitionRoutine(next, direction));
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
          component.Stop();
      }
      level.PreviousBounds = new Rectangle?(level.Bounds);
      level.Session.Level = next.Name;
      level.Session.FirstLevel = false;
      level.Session.DeathsInCurrentLevel = 0;
      level.LoadLevel(Player.IntroTypes.Transition);
      Audio.SetParameter(Audio.CurrentAmbienceEventInstance, "has_conveyors", level.Tracker.GetEntities<WallBooster>().Count > 0 ? 1f : 0.0f);
      List<Component> transitionIn = level.Tracker.GetComponentsCopy<TransitionListener>();
      transitionIn.RemoveAll((Predicate<Component>) (c => transitionOut.Contains(c)));
      GC.Collect();
      float cameraAt = 0.0f;
      Vector2 cameraFrom = level.Camera.Position;
      Vector2 dirPad = direction * 4f;
      if (direction == Vector2.UnitY)
        dirPad = direction * 12f;
      Vector2 playerTo = player.Position;
      while ((double) direction.X != 0.0 && (double) playerTo.Y >= (double) level.Bounds.Bottom)
        --playerTo.Y;
      while (!level.IsInBounds(playerTo, dirPad))
        playerTo += direction;
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
          level.Camera.Position = (double) cameraAt <= 0.8999999761581421 ? Vector2.Lerp(cameraFrom, cameraTo, Ease.CubeOut(cameraAt)) : cameraTo;
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
      Rectangle bounds = level.Bounds;
      bounds.Inflate(16, 16);
      level.Particles.ClearRect(bounds, false);
      level.ParticlesBG.ClearRect(bounds, false);
      level.ParticlesFG.ClearRect(bounds, false);
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
        Audio.SetAltMusic(SFX.EventnameByHandle(level.Session.LevelData.AltMusic));
      cameraFrom = new Vector2();
      playerTo = new Vector2();
      cameraTo = new Vector2();
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
        double left = (double) bounds.Left;
        bounds = this.Bounds;
        double bottom = (double) bounds.Bottom;
        return this.GetSpawnPoint(new Vector2((float) left, (float) bottom));
      }
    }

    public Vector2 GetSpawnPoint(Vector2 from) => this.Session.GetSpawnPoint(from);

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

    public Rectangle Bounds => this.Session.LevelData.Bounds;

    public Rectangle? PreviousBounds { get; private set; }

    public void TeleportTo(
      Player player,
      string nextLevel,
      Player.IntroTypes introType,
      Vector2? nearestSpawn = null)
    {
      Leader.StoreStrawberries(player.Leader);
      Vector2 position = player.Position;
      this.Remove((Entity) player);
      this.UnloadLevel();
      this.Session.Level = nextLevel;
      Session session = this.Session;
      Rectangle bounds = this.Bounds;
      double left = (double) bounds.Left;
      bounds = this.Bounds;
      double top = (double) bounds.Top;
      Vector2? nullable = new Vector2?(this.GetSpawnPoint(new Vector2((float) left, (float) top) + (nearestSpawn.HasValue ? nearestSpawn.Value : Vector2.Zero)));
      session.RespawnPoint = nullable;
      if (introType == Player.IntroTypes.Transition)
      {
        player.Position = this.Session.RespawnPoint.Value;
        player.Hair.MoveHairBy(player.Position - position);
        player.MuffleLanding = true;
        this.Add((Entity) player);
        this.LoadLevel(Player.IntroTypes.Transition);
        this.Entities.UpdateLists();
      }
      else
      {
        this.LoadLevel(introType);
        this.Entities.UpdateLists();
        player = this.Tracker.GetEntity<Player>();
      }
      this.Camera.Position = player.CameraTarget;
      this.Update();
      Leader.RestoreStrawberries(player.Leader);
    }

    public void AutoSave()
    {
      if (this.saving != null)
        return;
      this.saving = new Coroutine(this.SavingRoutine());
    }

    public bool IsAutoSaving() => this.saving != null;

    private IEnumerator SavingRoutine()
    {
      UserIO.SaveHandler(true, false);
      while (UserIO.Saving)
        yield return (object) null;
      this.saving = (Coroutine) null;
    }

    public void UpdateTime()
    {
      if (this.InCredits || this.Session.Area.ID == 8 || this.TimerStopped)
        return;
      long ticks = TimeSpan.FromSeconds((double) Engine.RawDeltaTime).Ticks;
      SaveData.Instance.AddTime(this.Session.Area, ticks);
      if (!this.TimerStarted && !this.InCutscene)
      {
        Player entity = this.Tracker.GetEntity<Player>();
        if (entity != null && !entity.TimePaused)
          this.TimerStarted = true;
      }
      if (this.Completed || !this.TimerStarted)
        return;
      this.Session.Time += ticks;
    }

    public override void Update()
    {
      if ((double) this.unpauseTimer > 0.0)
      {
        this.unpauseTimer -= Engine.RawDeltaTime;
        this.UpdateTime();
      }
      else if (this.Overlay != null)
      {
        this.Overlay.Update();
        this.Entities.UpdateLists();
      }
      else
      {
        int num1 = 10;
        if (!this.InCutscene && this.Tracker.GetEntity<Player>() != null && this.Wipe == null && !this.Frozen)
          num1 = SaveData.Instance.Assists.GameSpeed;
        Engine.TimeRateB = (float) num1 / 10f;
        if (num1 != 10)
        {
          if ((HandleBase) Level.AssistSpeedSnapshot == (HandleBase) null || Level.AssistSpeedSnapshotValue != num1)
          {
            Audio.ReleaseSnapshot(Level.AssistSpeedSnapshot);
            Level.AssistSpeedSnapshot = (EventInstance) null;
            Level.AssistSpeedSnapshotValue = num1;
            if (Level.AssistSpeedSnapshotValue < 10)
              Level.AssistSpeedSnapshot = Audio.CreateSnapshot("snapshot:/assist_game_speed/assist_speed_" + (object) (Level.AssistSpeedSnapshotValue * 10));
            else if (Level.AssistSpeedSnapshotValue <= 16)
              Level.AssistSpeedSnapshot = Audio.CreateSnapshot("snapshot:/variant_speed/variant_speed_" + (object) (Level.AssistSpeedSnapshotValue * 10));
          }
        }
        else if ((HandleBase) Level.AssistSpeedSnapshot != (HandleBase) null)
        {
          Audio.ReleaseSnapshot(Level.AssistSpeedSnapshot);
          Level.AssistSpeedSnapshot = (EventInstance) null;
          Level.AssistSpeedSnapshotValue = -1;
        }
        if (this.wasPaused && !this.Paused)
          this.EndPauseEffects();
        if (this.CanPause && Input.QuickRestart.Pressed)
        {
          Input.QuickRestart.ConsumeBuffer();
          this.Pause(quickReset: true);
        }
        else if (this.CanPause && (Input.Pause.Pressed || Input.ESC.Pressed))
        {
          Input.Pause.ConsumeBuffer();
          Input.ESC.ConsumeBuffer();
          this.Pause();
        }
        if (this.wasPaused && !this.Paused)
          this.wasPaused = false;
        if (this.Paused)
          this.wasPausedTimer = 0.0f;
        else
          this.wasPausedTimer += Engine.DeltaTime;
        this.UpdateTime();
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
        this.HudRenderer.BackgroundFade = Calc.Approach(this.HudRenderer.BackgroundFade, this.Paused ? 1f : 0.0f, 8f * Engine.RawDeltaTime);
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
            if (this.shakeDirection == Vector2.Zero)
            {
              this.ShakeVector = new Vector2((float) (-num2 + Calc.Random.Next(num2 * 2 + 1)), (float) (-num2 + Calc.Random.Next(num2 * 2 + 1)));
            }
            else
            {
              if (this.lastDirectionalShake == 0)
                this.lastDirectionalShake = 1;
              else
                this.lastDirectionalShake *= -1;
              this.ShakeVector = -this.shakeDirection * (float) this.lastDirectionalShake * (float) num2;
            }
            if (Settings.Instance.ScreenShake == ScreenshakeAmount.Half)
              this.ShakeVector = new Vector2((float) Math.Sign(this.ShakeVector.X), (float) Math.Sign(this.ShakeVector.Y));
          }
          this.shakeTimer -= Engine.RawDeltaTime * (Settings.Instance.ScreenShake == ScreenshakeAmount.Half ? 1.5f : 1f);
        }
        else
          this.ShakeVector = Vector2.Zero;
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
            this.colorGradeEase = Calc.Approach(this.colorGradeEase, 1f, Engine.DeltaTime * this.colorGradeEaseSpeed);
        }
        if (Celeste.PlayMode != Celeste.PlayModes.Debug)
          return;
        if (MInput.Keyboard.Pressed(Keys.Tab) && Engine.Scene.Tracker.GetEntity<KeyboardConfigUI>() == null && Engine.Scene.Tracker.GetEntity<ButtonConfigUI>() == null)
          Engine.Scene = (Scene) new MapEditor(this.Session.Area);
        if (MInput.Keyboard.Pressed(Keys.F1))
        {
          Celeste.ReloadAssets(true, false, false, new AreaKey?(this.Session.Area));
          Engine.Scene = (Scene) new LevelLoader(this.Session);
        }
        else if (MInput.Keyboard.Pressed(Keys.F2))
        {
          Celeste.ReloadAssets(true, true, false, new AreaKey?(this.Session.Area));
          Engine.Scene = (Scene) new LevelLoader(this.Session);
        }
        else
        {
          if (!MInput.Keyboard.Pressed(Keys.F3))
            return;
          Celeste.ReloadAssets(true, true, true, new AreaKey?(this.Session.Area));
          Engine.Scene = (Scene) new LevelLoader(this.Session);
        }
      }
    }

    public override void BeforeRender()
    {
      this.cameraPreShake = this.Camera.Position;
      this.Camera.Position += this.ShakeVector;
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
      Engine.Instance.GraphicsDevice.SetRenderTarget((RenderTarget2D) GameplayBuffers.Gameplay);
      Engine.Instance.GraphicsDevice.Clear(Color.Transparent);
      this.GameplayRenderer.Render((Scene) this);
      this.Lighting.Render((Scene) this);
      Engine.Instance.GraphicsDevice.SetRenderTarget((RenderTarget2D) GameplayBuffers.Level);
      Engine.Instance.GraphicsDevice.Clear(this.BackgroundColor);
      this.Background.Render((Scene) this);
      Distort.Render((Texture2D) (RenderTarget2D) GameplayBuffers.Gameplay, (Texture2D) (RenderTarget2D) GameplayBuffers.Displacement, this.Displacement.HasDisplacement((Scene) this));
      this.Bloom.Apply(GameplayBuffers.Level, (Scene) this);
      this.Foreground.Render((Scene) this);
      Glitch.Apply(GameplayBuffers.Level, this.glitchTimer * 2f, this.glitchSeed, 6.2831855f);
      if (Engine.DashAssistFreeze)
      {
        PlayerDashAssist entity = this.Tracker.GetEntity<PlayerDashAssist>();
        if (entity != null)
        {
          Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, (Effect) null, this.Camera.Matrix);
          entity.Render();
          Draw.SpriteBatch.End();
        }
      }
      if ((double) this.flash > 0.0)
      {
        Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, (Effect) null);
        Draw.Rect(-1f, -1f, 322f, 182f, this.flashColor * this.flash);
        Draw.SpriteBatch.End();
        if (this.flashDrawPlayer)
        {
          Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, (Effect) null, this.Camera.Matrix);
          Player entity = this.Tracker.GetEntity<Player>();
          if (entity != null && entity.Visible)
            entity.Render();
          Draw.SpriteBatch.End();
        }
      }
      Engine.Instance.GraphicsDevice.SetRenderTarget((RenderTarget2D) null);
      Engine.Instance.GraphicsDevice.Clear(Color.Black);
      Engine.Instance.GraphicsDevice.Viewport = Engine.Viewport;
      Matrix transformationMatrix = Matrix.CreateScale(6f) * Engine.ScreenMatrix;
      Vector2 vector2_1 = new Vector2(320f, 180f);
      Vector2 vector2_2 = vector2_1 / this.ZoomTarget;
      Vector2 origin = (double) this.ZoomTarget != 1.0 ? (this.ZoomFocusPoint - vector2_2 / 2f) / (vector2_1 - vector2_2) * vector2_1 : Vector2.Zero;
      MTexture orDefault1 = GFX.ColorGrades.GetOrDefault(this.lastColorGrade, GFX.ColorGrades["none"]);
      MTexture orDefault2 = GFX.ColorGrades.GetOrDefault(this.Session.ColorGrade, GFX.ColorGrades["none"]);
      if ((double) this.colorGradeEase > 0.0 && orDefault1 != orDefault2)
        ColorGrade.Set(orDefault1, orDefault2, this.colorGradeEase);
      else
        ColorGrade.Set(orDefault2);
      float scale = this.Zoom * (float) ((320.0 - (double) this.ScreenPadding * 2.0) / 320.0);
      Vector2 vector2_3 = new Vector2(this.ScreenPadding, this.ScreenPadding * (9f / 16f));
      if (SaveData.Instance.Assists.MirrorMode)
      {
        vector2_3.X = -vector2_3.X;
        origin.X = (float) (160.0 - ((double) origin.X - 160.0));
      }
      Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, ColorGrade.Effect, transformationMatrix);
      Draw.SpriteBatch.Draw((Texture2D) (RenderTarget2D) GameplayBuffers.Level, origin + vector2_3, new Rectangle?(GameplayBuffers.Level.Bounds), Color.White, 0.0f, origin, scale, SaveData.Instance.Assists.MirrorMode ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0.0f);
      Draw.SpriteBatch.End();
      if (this.Pathfinder != null && this.Pathfinder.DebugRenderEnabled)
      {
        Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, (Effect) null, this.Camera.Matrix * transformationMatrix);
        this.Pathfinder.Render();
        Draw.SpriteBatch.End();
      }
      if ((!this.Paused || !this.PauseMainMenuOpen) && (double) this.wasPausedTimer >= 1.0 || !Input.MenuJournal.Check || !this.AllowHudHide)
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
      Level.PauseSnapshot = Audio.CreateSnapshot("snapshot:/pause_menu");
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
        Audio.Play("event:/ui/main/message_confirm");
        this.PauseMainMenuOpen = false;
        this.GiveUp(0, true, minimal, false);
      }
      else
      {
        this.PauseMainMenuOpen = true;
        TextMenu menu = new TextMenu();
        if (!minimal)
          menu.Add((TextMenu.Item) new TextMenu.Header(Dialog.Clean("menu_pause_title")));
        menu.Add(new TextMenu.Button(Dialog.Clean("menu_pause_resume")).Pressed((Action) (() => menu.OnCancel())));
        if (this.InCutscene && !this.SkippingCutscene)
          menu.Add(new TextMenu.Button(Dialog.Clean("menu_pause_skip_cutscene")).Pressed((Action) (() =>
          {
            this.SkipCutscene();
            this.Paused = false;
            this.PauseMainMenuOpen = false;
            menu.RemoveSelf();
          })));
        if (!minimal && !this.InCutscene && !this.SkippingCutscene)
        {
          TextMenu.Item obj;
          menu.Add(obj = new TextMenu.Button(Dialog.Clean("menu_pause_retry")).Pressed((Action) (() =>
          {
            if (player != null && !player.Dead)
            {
              Engine.TimeRate = 1f;
              Distort.GameRate = 1f;
              Distort.Anxiety = 0.0f;
              this.InCutscene = this.SkippingCutscene = false;
              this.RetryPlayerCorpse = player.Die(Vector2.Zero, true);
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
          obj.Disabled = !this.CanRetry || player != null && !player.CanRetry || this.Frozen || this.Completed;
        }
        if (!minimal && SaveData.Instance.AssistMode)
        {
          TextMenu.Item item = (TextMenu.Item) null;
          menu.Add(item = new TextMenu.Button(Dialog.Clean("menu_pause_assist")).Pressed((Action) (() =>
          {
            menu.RemoveSelf();
            this.PauseMainMenuOpen = false;
            this.AssistMode(menu.IndexOf(item), minimal);
          })));
        }
        if (!minimal && SaveData.Instance.VariantMode)
        {
          TextMenu.Item item = (TextMenu.Item) null;
          menu.Add(item = new TextMenu.Button(Dialog.Clean("menu_pause_variant")).Pressed((Action) (() =>
          {
            menu.RemoveSelf();
            this.PauseMainMenuOpen = false;
            this.VariantMode(menu.IndexOf(item), minimal);
          })));
        }
        TextMenu.Item item1 = (TextMenu.Item) null;
        menu.Add(item1 = new TextMenu.Button(Dialog.Clean("menu_pause_options")).Pressed((Action) (() =>
        {
          menu.RemoveSelf();
          this.PauseMainMenuOpen = false;
          this.Options(menu.IndexOf(item1), minimal);
        })));
        if (!minimal && Celeste.PlayMode != Celeste.PlayModes.Event)
        {
          TextMenu.Item obj;
          menu.Add(obj = new TextMenu.Button(Dialog.Clean("menu_pause_savequit")).Pressed((Action) (() =>
          {
            menu.Focused = false;
            Engine.TimeRate = 1f;
            Audio.SetMusic((string) null);
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
          menu.Add(item2 = new TextMenu.Button(Dialog.Clean("menu_pause_restartarea")).Pressed((Action) (() =>
          {
            this.PauseMainMenuOpen = false;
            menu.RemoveSelf();
            this.GiveUp(menu.IndexOf(item2), true, minimal, true);
          })));
          (item2 as TextMenu.Button).ConfirmSfx = "event:/ui/main/message_confirm";
          if (SaveData.Instance.Areas[0].Modes[0].Completed || SaveData.Instance.DebugMode || SaveData.Instance.CheatMode)
          {
            TextMenu.Item item3 = (TextMenu.Item) null;
            menu.Add(item3 = new TextMenu.Button(Dialog.Clean("menu_pause_return")).Pressed((Action) (() =>
            {
              this.PauseMainMenuOpen = false;
              menu.RemoveSelf();
              this.GiveUp(menu.IndexOf(item3), false, minimal, false);
            })));
            (item3 as TextMenu.Button).ConfirmSfx = "event:/ui/main/message_confirm";
          }
          if (Celeste.PlayMode == Celeste.PlayModes.Event)
            menu.Add(new TextMenu.Button(Dialog.Clean("menu_pause_restartdemo")).Pressed((Action) (() =>
            {
              this.EndPauseEffects();
              Audio.SetMusic((string) null);
              menu.Focused = false;
              this.DoScreenWipe(false, (Action) (() => LevelEnter.Go(new Session(new AreaKey(0)), false)));
            })));
        }
        menu.OnESC = menu.OnCancel = menu.OnPause = (Action) (() =>
        {
          this.PauseMainMenuOpen = false;
          menu.RemoveSelf();
          this.Paused = false;
          Audio.Play("event:/ui/game/unpause");
          this.unpauseTimer = 0.15f;
        });
        if (startIndex > 0)
          menu.Selection = startIndex;
        this.Add((Entity) menu);
      }
    }

    private void GiveUp(int returnIndex, bool restartArea, bool minimal, bool showHint)
    {
      this.Paused = true;
      QuickResetHint quickHint = (QuickResetHint) null;
      ReturnMapHint returnHint = (ReturnMapHint) null;
      if (!restartArea)
        this.Add((Entity) (returnHint = new ReturnMapHint()));
      TextMenu menu = new TextMenu();
      menu.AutoScroll = false;
      menu.Position = new Vector2((float) Engine.Width / 2f, (float) ((double) Engine.Height / 2.0 - 100.0));
      menu.Add((TextMenu.Item) new TextMenu.Header(Dialog.Clean(restartArea ? "menu_restart_title" : "menu_return_title")));
      menu.Add(new TextMenu.Button(Dialog.Clean(restartArea ? "menu_restart_continue" : "menu_return_continue")).Pressed((Action) (() =>
      {
        Engine.TimeRate = 1f;
        menu.Focused = false;
        this.Session.InArea = false;
        Audio.SetMusic((string) null);
        Audio.BusStopAll("bus:/gameplay_sfx", true);
        if (restartArea)
          this.DoScreenWipe(false, (Action) (() => Engine.Scene = (Scene) new LevelExit(LevelExit.Mode.Restart, this.Session)));
        else
          this.DoScreenWipe(false, (Action) (() => Engine.Scene = (Scene) new LevelExit(LevelExit.Mode.GiveUp, this.Session, this.HiresSnow)), true);
        foreach (LevelEndingHook component in this.Tracker.GetComponents<LevelEndingHook>())
        {
          if (component.OnEnd != null)
            component.OnEnd();
        }
      })));
      menu.Add(new TextMenu.Button(Dialog.Clean(restartArea ? "menu_restart_cancel" : "menu_return_cancel")).Pressed((Action) (() => menu.OnCancel())));
      menu.OnPause = menu.OnESC = (Action) (() =>
      {
        menu.RemoveSelf();
        quickHint?.RemoveSelf();
        returnHint?.RemoveSelf();
        this.Paused = false;
        this.unpauseTimer = 0.15f;
        Audio.Play("event:/ui/game/unpause");
      });
      menu.OnCancel = (Action) (() =>
      {
        Audio.Play("event:/ui/main/button_back");
        menu.RemoveSelf();
        quickHint?.RemoveSelf();
        returnHint?.RemoveSelf();
        this.Pause(returnIndex, minimal);
      });
      this.Add((Entity) menu);
    }

    private void Options(int returnIndex, bool minimal)
    {
      this.Paused = true;
      bool oldAllowHudHide = this.AllowHudHide;
      this.AllowHudHide = false;
      TextMenu options = MenuOptions.Create(true, Level.PauseSnapshot);
      options.OnESC = options.OnCancel = (Action) (() =>
      {
        Audio.Play("event:/ui/main/button_back");
        this.AllowHudHide = oldAllowHudHide;
        options.CloseAndRun(this.SaveFromOptions(), (Action) (() => this.Pause(returnIndex, minimal)));
      });
      options.OnPause = (Action) (() =>
      {
        Audio.Play("event:/ui/main/button_back");
        options.CloseAndRun(this.SaveFromOptions(), (Action) (() =>
        {
          this.AllowHudHide = oldAllowHudHide;
          this.Paused = false;
          this.unpauseTimer = 0.15f;
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
      menu.Add((TextMenu.Item) new TextMenu.Header(Dialog.Clean("MENU_ASSIST_TITLE")));
      menu.Add((TextMenu.Item) new TextMenu.Slider(Dialog.Clean("MENU_ASSIST_GAMESPEED"), (Func<int, string>) (i => (i * 10).ToString() + "%"), 5, 10, SaveData.Instance.Assists.GameSpeed).Change((Action<int>) (i =>
      {
        SaveData.Instance.Assists.GameSpeed = i;
        Engine.TimeRateB = (float) SaveData.Instance.Assists.GameSpeed / 10f;
      })));
      menu.Add((TextMenu.Item) new TextMenu.OnOff(Dialog.Clean("MENU_ASSIST_INFINITE_STAMINA"), SaveData.Instance.Assists.InfiniteStamina).Change((Action<bool>) (on => SaveData.Instance.Assists.InfiniteStamina = on)));
      TextMenu textMenu = menu;
      string label = Dialog.Clean("MENU_ASSIST_AIR_DASHES");
      int dashMode = (int) SaveData.Instance.Assists.DashMode;
      TextMenu.Option<int> option1;
      TextMenu.Option<int> option2 = option1 = new TextMenu.Slider(label, (Func<int, string>) (i =>
      {
        if (i == 0)
          return Dialog.Clean("MENU_ASSIST_AIR_DASHES_NORMAL");
        return i == 1 ? Dialog.Clean("MENU_ASSIST_AIR_DASHES_TWO") : Dialog.Clean("MENU_ASSIST_AIR_DASHES_INFINITE");
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
      menu.Add((TextMenu.Item) new TextMenu.OnOff(Dialog.Clean("MENU_ASSIST_DASH_ASSIST"), SaveData.Instance.Assists.DashAssist).Change((Action<bool>) (on => SaveData.Instance.Assists.DashAssist = on)));
      menu.Add((TextMenu.Item) new TextMenu.OnOff(Dialog.Clean("MENU_ASSIST_INVINCIBLE"), SaveData.Instance.Assists.Invincible).Change((Action<bool>) (on => SaveData.Instance.Assists.Invincible = on)));
      menu.OnESC = menu.OnCancel = (Action) (() =>
      {
        Audio.Play("event:/ui/main/button_back");
        this.Pause(returnIndex, minimal);
        menu.Close();
      });
      menu.OnPause = (Action) (() =>
      {
        Audio.Play("event:/ui/main/button_back");
        this.Paused = false;
        this.unpauseTimer = 0.15f;
        menu.Close();
      });
      this.Add((Entity) menu);
    }

    private void VariantMode(int returnIndex, bool minimal)
    {
      this.Paused = true;
      TextMenu menu = new TextMenu();
      menu.Add((TextMenu.Item) new TextMenu.Header(Dialog.Clean("MENU_VARIANT_TITLE")));
      menu.Add((TextMenu.Item) new TextMenu.SubHeader(Dialog.Clean("MENU_VARIANT_SUBTITLE")));
      TextMenu textMenu1 = menu;
      string label1 = Dialog.Clean("MENU_ASSIST_GAMESPEED");
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
      menu.Add((TextMenu.Item) new TextMenu.OnOff(Dialog.Clean("MENU_VARIANT_MIRROR"), SaveData.Instance.Assists.MirrorMode).Change((Action<bool>) (on =>
      {
        SaveData.Instance.Assists.MirrorMode = on;
        Input.MoveX.Inverted = Input.Aim.InvertedX = Input.Feather.InvertedX = on;
      })));
      menu.Add((TextMenu.Item) new TextMenu.OnOff(Dialog.Clean("MENU_VARIANT_360DASHING"), SaveData.Instance.Assists.ThreeSixtyDashing).Change((Action<bool>) (on => SaveData.Instance.Assists.ThreeSixtyDashing = on)));
      menu.Add((TextMenu.Item) new TextMenu.OnOff(Dialog.Clean("MENU_VARIANT_INVISMOTION"), SaveData.Instance.Assists.InvisibleMotion).Change((Action<bool>) (on => SaveData.Instance.Assists.InvisibleMotion = on)));
      menu.Add((TextMenu.Item) new TextMenu.OnOff(Dialog.Clean("MENU_VARIANT_NOGRABBING"), SaveData.Instance.Assists.NoGrabbing).Change((Action<bool>) (on => SaveData.Instance.Assists.NoGrabbing = on)));
      menu.Add((TextMenu.Item) new TextMenu.OnOff(Dialog.Clean("MENU_VARIANT_LOWFRICTION"), SaveData.Instance.Assists.LowFriction).Change((Action<bool>) (on => SaveData.Instance.Assists.LowFriction = on)));
      menu.Add((TextMenu.Item) new TextMenu.OnOff(Dialog.Clean("MENU_VARIANT_SUPERDASHING"), SaveData.Instance.Assists.SuperDashing).Change((Action<bool>) (on => SaveData.Instance.Assists.SuperDashing = on)));
      menu.Add((TextMenu.Item) new TextMenu.OnOff(Dialog.Clean("MENU_VARIANT_HICCUPS"), SaveData.Instance.Assists.Hiccups).Change((Action<bool>) (on => SaveData.Instance.Assists.Hiccups = on)));
      menu.Add((TextMenu.Item) new TextMenu.OnOff(Dialog.Clean("MENU_VARIANT_PLAYASBADELINE"), SaveData.Instance.Assists.PlayAsBadeline).Change((Action<bool>) (on =>
      {
        SaveData.Instance.Assists.PlayAsBadeline = on;
        Player entity = this.Tracker.GetEntity<Player>();
        if (entity == null)
          return;
        PlayerSpriteMode mode = SaveData.Instance.Assists.PlayAsBadeline ? PlayerSpriteMode.MadelineAsBadeline : entity.DefaultSpriteMode;
        if (entity.Active)
          entity.ResetSpriteNextFrame(mode);
        else
          entity.ResetSprite(mode);
      })));
      menu.Add((TextMenu.Item) new TextMenu.SubHeader(Dialog.Clean("MENU_ASSIST_SUBTITLE")));
      menu.Add((TextMenu.Item) new TextMenu.OnOff(Dialog.Clean("MENU_ASSIST_INFINITE_STAMINA"), SaveData.Instance.Assists.InfiniteStamina).Change((Action<bool>) (on => SaveData.Instance.Assists.InfiniteStamina = on)));
      TextMenu textMenu2 = menu;
      string label2 = Dialog.Clean("MENU_ASSIST_AIR_DASHES");
      int dashMode = (int) SaveData.Instance.Assists.DashMode;
      TextMenu.Option<int> option1;
      TextMenu.Option<int> option2 = option1 = new TextMenu.Slider(label2, (Func<int, string>) (i =>
      {
        if (i == 0)
          return Dialog.Clean("MENU_ASSIST_AIR_DASHES_NORMAL");
        return i == 1 ? Dialog.Clean("MENU_ASSIST_AIR_DASHES_TWO") : Dialog.Clean("MENU_ASSIST_AIR_DASHES_INFINITE");
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
      menu.Add((TextMenu.Item) new TextMenu.OnOff(Dialog.Clean("MENU_ASSIST_DASH_ASSIST"), SaveData.Instance.Assists.DashAssist).Change((Action<bool>) (on => SaveData.Instance.Assists.DashAssist = on)));
      menu.Add((TextMenu.Item) new TextMenu.OnOff(Dialog.Clean("MENU_ASSIST_INVINCIBLE"), SaveData.Instance.Assists.Invincible).Change((Action<bool>) (on => SaveData.Instance.Assists.Invincible = on)));
      menu.OnESC = menu.OnCancel = (Action) (() =>
      {
        Audio.Play("event:/ui/main/button_back");
        this.Pause(returnIndex, minimal);
        menu.Close();
      });
      menu.OnPause = (Action) (() =>
      {
        Audio.Play("event:/ui/main/button_back");
        this.Paused = false;
        this.unpauseTimer = 0.15f;
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
      this.colorGradeEaseSpeed = 1f;
      this.Session.ColorGrade = next;
    }

    public void NextColorGrade(string next, float speed = 1f)
    {
      if (!(this.Session.ColorGrade != next))
        return;
      this.colorGradeEase = 0.0f;
      this.colorGradeEaseSpeed = speed;
      this.Session.ColorGrade = next;
    }

    public void Shake(float time = 0.3f)
    {
      if (Settings.Instance.ScreenShake == ScreenshakeAmount.Off)
        return;
      this.shakeDirection = Vector2.Zero;
      this.shakeTimer = Math.Max(this.shakeTimer, time);
    }

    public void StopShake() => this.shakeTimer = 0.0f;

    public void DirectionalShake(Vector2 dir, float time = 0.3f)
    {
      if (Settings.Instance.ScreenShake == ScreenshakeAmount.Off)
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

    public IEnumerator ZoomAcross(Vector2 screenSpaceFocusPoint, float zoom, float duration)
    {
      float fromZoom = this.Zoom;
      Vector2 fromFocus = this.ZoomFocusPoint;
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime / duration)
      {
        float amount = Ease.SineInOut(MathHelper.Clamp(p, 0.0f, 1f));
        this.Zoom = this.ZoomTarget = MathHelper.Lerp(fromZoom, zoom, amount);
        this.ZoomFocusPoint = Vector2.Lerp(fromFocus, screenSpaceFocusPoint, amount);
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
      this.ZoomFocusPoint = new Vector2(320f, 180f) / 2f;
    }

    public void DoScreenWipe(bool wipeIn, Action onComplete = null, bool hiresSnow = false)
    {
      AreaData.Get(this.Session).DoScreenWipe((Scene) this, wipeIn, onComplete);
      if (!hiresSnow)
        return;
      this.Add((Monocle.Renderer) (this.HiresSnow = new HiresSnow()));
      this.HiresSnow.Alpha = 0.0f;
      this.HiresSnow.AttachAlphaTo = this.Wipe;
    }

    public Session.CoreModes CoreMode
    {
      get => this.coreMode;
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

    public bool InsideCamera(Vector2 position, float expand = 0.0f) => (double) position.X >= (double) this.Camera.Left - (double) expand && (double) position.X < (double) this.Camera.Right + (double) expand && (double) position.Y >= (double) this.Camera.Top - (double) expand && (double) position.Y < (double) this.Camera.Bottom + (double) expand;

    public void EnforceBounds(Player player)
    {
      Rectangle bounds = this.Bounds;
      Rectangle rectangle = new Rectangle((int) this.Camera.Left, (int) this.Camera.Top, 320, 180);
      if (this.transition != null)
        return;
      if (this.CameraLockMode == Level.CameraLockModes.FinalBoss && (double) player.Left < (double) rectangle.Left)
      {
        player.Left = (float) rectangle.Left;
        player.OnBoundsH();
      }
      else if ((double) player.Left < (double) bounds.Left)
      {
        if ((double) player.Top >= (double) bounds.Top && (double) player.Bottom < (double) bounds.Bottom && this.Session.MapData.CanTransitionTo(this, player.Center + Vector2.UnitX * -8f))
        {
          player.BeforeSideTransition();
          this.NextLevel(player.Center + Vector2.UnitX * -8f, -Vector2.UnitX);
          return;
        }
        player.Left = (float) bounds.Left;
        player.OnBoundsH();
      }
      TheoCrystal entity = this.Tracker.GetEntity<TheoCrystal>();
      if (this.CameraLockMode == Level.CameraLockModes.FinalBoss && (double) player.Right > (double) rectangle.Right && rectangle.Right < bounds.Right - 4)
      {
        player.Right = (float) rectangle.Right;
        player.OnBoundsH();
      }
      else if (entity != null && (player.Holding == null || !player.Holding.IsHeld) && (double) player.Right > (double) (bounds.Right - 1))
        player.Right = (float) (bounds.Right - 1);
      else if ((double) player.Right > (double) bounds.Right)
      {
        if ((double) player.Top >= (double) bounds.Top && (double) player.Bottom < (double) bounds.Bottom && this.Session.MapData.CanTransitionTo(this, player.Center + Vector2.UnitX * 8f))
        {
          player.BeforeSideTransition();
          this.NextLevel(player.Center + Vector2.UnitX * 8f, Vector2.UnitX);
          return;
        }
        player.Right = (float) bounds.Right;
        player.OnBoundsH();
      }
      if (this.CameraLockMode != Level.CameraLockModes.None && (double) player.Top < (double) rectangle.Top)
      {
        player.Top = (float) rectangle.Top;
        player.OnBoundsV();
      }
      else if ((double) player.CenterY < (double) bounds.Top)
      {
        if (this.Session.MapData.CanTransitionTo(this, player.Center - Vector2.UnitY * 12f))
        {
          player.BeforeUpTransition();
          this.NextLevel(player.Center - Vector2.UnitY * 12f, -Vector2.UnitY);
          return;
        }
        if ((double) player.Top < (double) (bounds.Top - 24))
        {
          player.Top = (float) (bounds.Top - 24);
          player.OnBoundsV();
        }
      }
      if (this.CameraLockMode != Level.CameraLockModes.None && rectangle.Bottom < bounds.Bottom - 4 && (double) player.Top > (double) rectangle.Bottom)
      {
        if (SaveData.Instance.Assists.Invincible)
        {
          player.Play("event:/game/general/assist_screenbottom");
          player.Bounce((float) rectangle.Bottom);
        }
        else
          player.Die(Vector2.Zero);
      }
      else if ((double) player.Bottom > (double) bounds.Bottom && this.Session.MapData.CanTransitionTo(this, player.Center + Vector2.UnitY * 12f) && !this.Session.LevelData.DisableDownTransition)
      {
        if (player.CollideCheck<Solid>(player.Position + Vector2.UnitY * 4f))
          return;
        player.BeforeDownTransition();
        this.NextLevel(player.Center + Vector2.UnitY * 12f, Vector2.UnitY);
      }
      else if ((double) player.Top > (double) bounds.Bottom && SaveData.Instance.Assists.Invincible)
      {
        player.Play("event:/game/general/assist_screenbottom");
        player.Bounce((float) bounds.Bottom);
      }
      else
      {
        if ((double) player.Top <= (double) (bounds.Bottom + 4))
          return;
        player.Die(Vector2.Zero);
      }
    }

    public bool IsInBounds(Entity entity)
    {
      Rectangle bounds = this.Bounds;
      return (double) entity.Right > (double) bounds.Left && (double) entity.Bottom > (double) bounds.Top && (double) entity.Left < (double) bounds.Right && (double) entity.Top < (double) bounds.Bottom;
    }

    public bool IsInBounds(Vector2 position)
    {
      Rectangle bounds = this.Bounds;
      return (double) position.X >= (double) bounds.Left && (double) position.Y >= (double) bounds.Top && (double) position.X < (double) bounds.Right && (double) position.Y < (double) bounds.Bottom;
    }

    public bool IsInBounds(Vector2 position, float pad)
    {
      Rectangle bounds = this.Bounds;
      return (double) position.X >= (double) bounds.Left - (double) pad && (double) position.Y >= (double) bounds.Top - (double) pad && (double) position.X < (double) bounds.Right + (double) pad && (double) position.Y < (double) bounds.Bottom + (double) pad;
    }

    public bool IsInBounds(Vector2 position, Vector2 dirPad)
    {
      float num1 = Math.Max(dirPad.X, 0.0f);
      float num2 = Math.Max(-dirPad.X, 0.0f);
      float num3 = Math.Max(dirPad.Y, 0.0f);
      float num4 = Math.Max(-dirPad.Y, 0.0f);
      Rectangle bounds = this.Bounds;
      return (double) position.X >= (double) bounds.Left + (double) num1 && (double) position.Y >= (double) bounds.Top + (double) num3 && (double) position.X < (double) bounds.Right - (double) num2 && (double) position.Y < (double) bounds.Bottom - (double) num4;
    }

    public bool IsInCamera(Vector2 position, float pad)
    {
      Rectangle rectangle = new Rectangle((int) this.Camera.X, (int) this.Camera.Y, 320, 180);
      return (double) position.X >= (double) rectangle.Left - (double) pad && (double) position.Y >= (double) rectangle.Top - (double) pad && (double) position.X < (double) rectangle.Right + (double) pad && (double) position.Y < (double) rectangle.Bottom + (double) pad;
    }

    public void StartCutscene(
      Action<Level> onSkip,
      bool fadeInOnSkip = true,
      bool endingChapterAfterCutscene = false,
      bool resetZoomOnSkip = true)
    {
      this.endingChapterAfterCutscene = endingChapterAfterCutscene;
      this.InCutscene = true;
      this.onCutsceneSkip = onSkip;
      this.onCutsceneSkipFadeIn = fadeInOnSkip;
      this.onCutsceneSkipResetZoom = resetZoomOnSkip;
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
      this.skipCoroutine = new Coroutine(this.SkipCutsceneRoutine());
    }

    private IEnumerator SkipCutsceneRoutine()
    {
      Level level = this;
      FadeWipe fadeWipe1 = new FadeWipe((Scene) level, false);
      fadeWipe1.Duration = 0.25f;
      yield return (object) fadeWipe1.Wait();
      level.onCutsceneSkip(level);
      level.strawberriesDisplay.DrawLerp = 0.0f;
      if (level.onCutsceneSkipResetZoom)
        level.ResetZoom();
      GameplayStats first = level.Entities.FindFirst<GameplayStats>();
      if (first != null)
        first.DrawLerp = 0.0f;
      if (level.onCutsceneSkipFadeIn)
      {
        FadeWipe fadeWipe2 = new FadeWipe((Scene) level, true);
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

    private void NextLevel(Vector2 at, Vector2 dir) => this.OnEndOfFrame += (Action) (() =>
    {
      Engine.TimeRate = 1f;
      Distort.Anxiety = 0.0f;
      Distort.GameRate = 1f;
      this.TransitionTo(this.Session.MapData.GetAt(at), dir);
    });

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

    public ScreenWipe CompleteArea(
      bool spotlightWipe = true,
      bool skipScreenWipe = false,
      bool skipCompleteScreen = false)
    {
      this.RegisterAreaComplete();
      this.PauseLock = true;
      Action onComplete = !(AreaData.Get(this.Session).Interlude | skipCompleteScreen) ? (Action) (() => Engine.Scene = (Scene) new LevelExit(LevelExit.Mode.Completed, this.Session)) : (Action) (() => Engine.Scene = (Scene) new LevelExit(LevelExit.Mode.CompletedInterlude, this.Session, this.HiresSnow));
      if (!this.SkippingCutscene && !skipScreenWipe)
      {
        if (!spotlightWipe)
          return (ScreenWipe) new FadeWipe((Scene) this, false, onComplete);
        Player entity = this.Tracker.GetEntity<Player>();
        if (entity != null)
          SpotlightWipe.FocusPoint = entity.Position - this.Camera.Position - new Vector2(0.0f, 8f);
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
