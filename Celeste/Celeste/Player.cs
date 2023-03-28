// Decompiled with JetBrains decompiler
// Type: Celeste.Player
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using FMOD.Studio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Celeste
{
  [Tracked(false)]
  public class Player : Actor
  {
    public static ParticleType P_DashA;
    public static ParticleType P_DashB;
    public static ParticleType P_DashBadB;
    public static ParticleType P_CassetteFly;
    public static ParticleType P_Split;
    public static ParticleType P_SummitLandA;
    public static ParticleType P_SummitLandB;
    public static ParticleType P_SummitLandC;
    public const float MaxFall = 160f;
    private const float Gravity = 900f;
    private const float HalfGravThreshold = 40f;
    private const float FastMaxFall = 240f;
    private const float FastMaxAccel = 300f;
    public const float MaxRun = 90f;
    public const float RunAccel = 1000f;
    private const float RunReduce = 400f;
    private const float AirMult = 0.65f;
    private const float HoldingMaxRun = 70f;
    private const float HoldMinTime = 0.35f;
    private const float BounceAutoJumpTime = 0.1f;
    private const float DuckFriction = 500f;
    private const int DuckCorrectCheck = 4;
    private const float DuckCorrectSlide = 50f;
    private const float DodgeSlideSpeedMult = 1.2f;
    private const float DuckSuperJumpXMult = 1.25f;
    private const float DuckSuperJumpYMult = 0.5f;
    private const float JumpGraceTime = 0.1f;
    private const float JumpSpeed = -105f;
    private const float JumpHBoost = 40f;
    private const float VarJumpTime = 0.2f;
    private const float CeilingVarJumpGrace = 0.05f;
    private const int UpwardCornerCorrection = 4;
    private const int DashingUpwardCornerCorrection = 5;
    private const float WallSpeedRetentionTime = 0.06f;
    private const int WallJumpCheckDist = 3;
    private const int SuperWallJumpCheckDist = 5;
    private const float WallJumpForceTime = 0.16f;
    private const float WallJumpHSpeed = 130f;
    public const float WallSlideStartMax = 20f;
    private const float WallSlideTime = 1.2f;
    private const float BounceVarJumpTime = 0.2f;
    private const float BounceSpeed = -140f;
    private const float SuperBounceVarJumpTime = 0.2f;
    private const float SuperBounceSpeed = -185f;
    private const float SuperJumpSpeed = -105f;
    private const float SuperJumpH = 260f;
    private const float SuperWallJumpSpeed = -160f;
    private const float SuperWallJumpVarTime = 0.25f;
    private const float SuperWallJumpForceTime = 0.2f;
    private const float SuperWallJumpH = 170f;
    private const float DashSpeed = 240f;
    private const float EndDashSpeed = 160f;
    private const float EndDashUpMult = 0.75f;
    private const float DashTime = 0.15f;
    private const float SuperDashTime = 0.3f;
    private const float DashCooldown = 0.2f;
    private const float DashRefillCooldown = 0.1f;
    private const int DashHJumpThruNudge = 6;
    private const int DashCornerCorrection = 4;
    private const int DashVFloorSnapDist = 3;
    private const float DashAttackTime = 0.3f;
    private const float BoostMoveSpeed = 80f;
    public const float BoostTime = 0.25f;
    private const float DuckWindMult = 0.0f;
    private const int WindWallDistance = 3;
    private const float ReboundSpeedX = 120f;
    private const float ReboundSpeedY = -120f;
    private const float ReboundVarJumpTime = 0.15f;
    private const float ReflectBoundSpeed = 220f;
    private const float DreamDashSpeed = 240f;
    private const int DreamDashEndWiggle = 5;
    private const float DreamDashMinTime = 0.1f;
    public const float ClimbMaxStamina = 110f;
    private const float ClimbUpCost = 45.454544f;
    private const float ClimbStillCost = 10f;
    private const float ClimbJumpCost = 27.5f;
    private const int ClimbCheckDist = 2;
    private const int ClimbUpCheckDist = 2;
    private const float ClimbNoMoveTime = 0.1f;
    public const float ClimbTiredThreshold = 20f;
    private const float ClimbUpSpeed = -45f;
    private const float ClimbDownSpeed = 80f;
    private const float ClimbSlipSpeed = 30f;
    private const float ClimbAccel = 900f;
    private const float ClimbGrabYMult = 0.2f;
    private const float ClimbHopY = -120f;
    private const float ClimbHopX = 100f;
    private const float ClimbHopForceTime = 0.2f;
    private const float ClimbJumpBoostTime = 0.2f;
    private const float ClimbHopNoWindTime = 0.3f;
    private const float LaunchSpeed = 280f;
    private const float LaunchCancelThreshold = 220f;
    private const float LiftYCap = -130f;
    private const float LiftXCap = 250f;
    private const float JumpThruAssistSpeed = -40f;
    private const float FlyPowerFlashTime = 0.5f;
    private const float ThrowRecoil = 80f;
    private static readonly Vector2 CarryOffsetTarget = new Vector2(0.0f, -12f);
    private const float ChaserStateMaxTime = 4f;
    public const float WalkSpeed = 64f;
    private const float LowFrictionMult = 0.35f;
    private const float LowFrictionAirMult = 0.5f;
    private const float LowFrictionStopTime = 0.15f;
    private const float HiccupTimeMin = 1.2f;
    private const float HiccupTimeMax = 1.8f;
    private const float HiccupDuckMult = 0.5f;
    private const float HiccupAirBoost = -60f;
    private const float HiccupAirVarTime = 0.15f;
    private const float GliderMaxFall = 40f;
    private const float GliderWindMaxFall = 0.0f;
    private const float GliderWindUpFall = -32f;
    public const float GliderFastFall = 120f;
    private const float GliderSlowFall = 24f;
    private const float GliderGravMult = 0.5f;
    private const float GliderMaxRun = 108.00001f;
    private const float GliderRunMult = 0.5f;
    private const float GliderUpMinPickupSpeed = -105f;
    private const float GliderDashMinPickupSpeed = -240f;
    private const float GliderWallJumpForceTime = 0.26f;
    private const float DashGliderBoostTime = 0.55f;
    public const int StNormal = 0;
    public const int StClimb = 1;
    public const int StDash = 2;
    public const int StSwim = 3;
    public const int StBoost = 4;
    public const int StRedDash = 5;
    public const int StHitSquash = 6;
    public const int StLaunch = 7;
    public const int StPickup = 8;
    public const int StDreamDash = 9;
    public const int StSummitLaunch = 10;
    public const int StDummy = 11;
    public const int StIntroWalk = 12;
    public const int StIntroJump = 13;
    public const int StIntroRespawn = 14;
    public const int StIntroWakeUp = 15;
    public const int StBirdDashTutorial = 16;
    public const int StFrozen = 17;
    public const int StReflectionFall = 18;
    public const int StStarFly = 19;
    public const int StTempleFall = 20;
    public const int StCassetteFly = 21;
    public const int StAttract = 22;
    public const int StIntroMoonJump = 23;
    public const int StFlingBird = 24;
    public const int StIntroThinkForABit = 25;
    public const string TalkSfx = "player_talk";
    public Vector2 Speed;
    public Facings Facing;
    public PlayerSprite Sprite;
    public PlayerHair Hair;
    public StateMachine StateMachine;
    public Vector2 CameraAnchor;
    public bool CameraAnchorIgnoreX;
    public bool CameraAnchorIgnoreY;
    public Vector2 CameraAnchorLerp;
    public bool ForceCameraUpdate;
    public Leader Leader;
    public VertexLight Light;
    public int Dashes;
    public float Stamina = 110f;
    public bool StrawberriesBlocked;
    public Vector2 PreviousPosition;
    public bool DummyAutoAnimate = true;
    public Vector2 ForceStrongWindHair;
    public Vector2? OverrideDashDirection;
    public bool FlipInReflection;
    public bool JustRespawned;
    public bool EnforceLevelBounds = true;
    private Level level;
    private Collision onCollideH;
    private Collision onCollideV;
    private bool onGround;
    private bool wasOnGround;
    private int moveX;
    private bool flash;
    private bool wasDucking;
    private int climbTriggerDir;
    private bool holdCannotDuck;
    private bool windMovedUp;
    private float idleTimer;
    private static Chooser<string> idleColdOptions = new Chooser<string>().Add("idleA", 5f).Add("idleB", 3f).Add("idleC", 1f);
    private static Chooser<string> idleNoBackpackOptions = new Chooser<string>().Add("idleA", 1f).Add("idleB", 3f).Add("idleC", 3f);
    private static Chooser<string> idleWarmOptions = new Chooser<string>().Add("idleA", 5f).Add("idleB", 3f);
    public int StrawberryCollectIndex;
    public float StrawberryCollectResetTimer;
    private Hitbox hurtbox;
    private float jumpGraceTimer;
    public bool AutoJump;
    public float AutoJumpTimer;
    private float varJumpSpeed;
    private float varJumpTimer;
    private int forceMoveX;
    private float forceMoveXTimer;
    private int hopWaitX;
    private float hopWaitXSpeed;
    private Vector2 lastAim;
    private float dashCooldownTimer;
    private float dashRefillCooldownTimer;
    public Vector2 DashDir;
    private float wallSlideTimer = 1.2f;
    private int wallSlideDir;
    private float climbNoMoveTimer;
    private Vector2 carryOffset;
    private Vector2 deadOffset;
    private float introEase;
    private float wallSpeedRetentionTimer;
    private float wallSpeedRetained;
    private int wallBoostDir;
    private float wallBoostTimer;
    private float maxFall;
    private float dashAttackTimer;
    private float gliderBoostTimer;
    public List<Player.ChaserState> ChaserStates;
    private bool wasTired;
    private HashSet<Trigger> triggersInside;
    private float highestAirY;
    private bool dashStartedOnGround;
    private bool fastJump;
    private int lastClimbMove;
    private float noWindTimer;
    private float dreamDashCanEndTimer;
    private Solid climbHopSolid;
    private Vector2 climbHopSolidPosition;
    private SoundSource wallSlideSfx;
    private SoundSource swimSurfaceLoopSfx;
    private float playFootstepOnLand;
    private float minHoldTimer;
    public Booster CurrentBooster;
    public Booster LastBooster;
    private bool calledDashEvents;
    private int lastDashes;
    private Monocle.Sprite sweatSprite;
    private int startHairCount;
    private bool launched;
    private float launchedTimer;
    private float dashTrailTimer;
    private int dashTrailCounter;
    private bool canCurveDash;
    private float lowFrictionStopTimer;
    private float hiccupTimer;
    private List<Player.ChaserStateSound> activeSounds = new List<Player.ChaserStateSound>();
    private EventInstance idleSfx;
    public bool MuffleLanding;
    private Vector2 gliderBoostDir;
    private float explodeLaunchBoostTimer;
    private float explodeLaunchBoostSpeed;
    private bool demoDashed;
    private readonly Hitbox normalHitbox = new Hitbox(8f, 11f, -4f, -11f);
    private readonly Hitbox duckHitbox = new Hitbox(8f, 6f, -4f, -6f);
    private readonly Hitbox normalHurtbox = new Hitbox(8f, 9f, -4f, -11f);
    private readonly Hitbox duckHurtbox = new Hitbox(8f, 4f, -4f, -6f);
    private readonly Hitbox starFlyHitbox = new Hitbox(8f, 8f, -4f, -10f);
    private readonly Hitbox starFlyHurtbox = new Hitbox(6f, 6f, -3f, -9f);
    private Vector2 normalLightOffset = new Vector2(0.0f, -8f);
    private Vector2 duckingLightOffset = new Vector2(0.0f, -3f);
    private List<Entity> temp = new List<Entity>();
    public static readonly Color NormalHairColor = Calc.HexToColor("AC3232");
    public static readonly Color FlyPowerHairColor = Calc.HexToColor("F2EB6D");
    public static readonly Color UsedHairColor = Calc.HexToColor("44B7FF");
    public static readonly Color FlashHairColor = Color.White;
    public static readonly Color TwoDashesHairColor = Calc.HexToColor("ff6def");
    public static readonly Color NormalBadelineHairColor = BadelineOldsite.HairColor;
    public static readonly Color UsedBadelineHairColor = Player.UsedHairColor;
    public static readonly Color TwoDashesBadelineHairColor = Player.TwoDashesHairColor;
    private float hairFlashTimer;
    private bool startHairCalled;
    public Color? OverrideHairColor;
    private Vector2 windDirection;
    private float windTimeout;
    private float windHairTimer;
    public Player.IntroTypes IntroType;
    private MirrorReflection reflection;
    public PlayerSpriteMode DefaultSpriteMode;
    private PlayerSpriteMode? nextSpriteMode;
    private const float LaunchedBoostCheckSpeedSq = 10000f;
    private const float LaunchedJumpCheckSpeedSq = 48400f;
    private const float LaunchedMinSpeedSq = 19600f;
    private const float LaunchedDoubleSpeedSq = 22500f;
    private const float SideBounceSpeed = 240f;
    private const float SideBounceThreshold = 240f;
    private const float SideBounceForceMoveXTime = 0.3f;
    private const float SpacePhysicsMult = 0.6f;
    private EventInstance conveyorLoopSfx;
    private const float WallBoosterSpeed = -160f;
    private const float WallBoosterLiftSpeed = -80f;
    private const float WallBoosterAccel = 600f;
    private const float WallBoostingHopHSpeed = 100f;
    private const float WallBoosterOverTopSpeed = -180f;
    private const float IceBoosterSpeed = 40f;
    private const float IceBoosterAccel = 300f;
    private bool wallBoosting;
    private Vector2 beforeDashSpeed;
    private bool wasDashB;
    private const float SwimYSpeedMult = 0.5f;
    private const float SwimMaxRise = -60f;
    private const float SwimVDeccel = 600f;
    private const float SwimMax = 80f;
    private const float SwimUnderwaterMax = 60f;
    private const float SwimAccel = 600f;
    private const float SwimReduce = 400f;
    private const float SwimDashSpeedMult = 0.75f;
    private Vector2 boostTarget;
    private bool boostRed;
    private const float HitSquashNoMoveTime = 0.1f;
    private const float HitSquashFriction = 800f;
    private float hitSquashNoMoveTimer;
    private float? launchApproachX;
    private float summitLaunchTargetX;
    private float summitLaunchParticleTimer;
    private DreamBlock dreamBlock;
    private SoundSource dreamSfxLoop;
    private bool dreamJump;
    private const float StarFlyTransformDeccel = 1000f;
    private const float StarFlyTime = 2f;
    private const float StarFlyStartSpeed = 250f;
    private const float StarFlyTargetSpeed = 140f;
    private const float StarFlyMaxSpeed = 190f;
    private const float StarFlyMaxLerpTime = 1f;
    private const float StarFlySlowSpeed = 91f;
    private const float StarFlyAccel = 1000f;
    private const float StarFlyRotateSpeed = 5.5850534f;
    private const float StarFlyEndX = 160f;
    private const float StarFlyEndXVarJumpTime = 0.1f;
    private const float StarFlyEndFlashDuration = 0.5f;
    private const float StarFlyEndNoBounceTime = 0.2f;
    private const float StarFlyWallBounce = -0.5f;
    private const float StarFlyMaxExitY = 0.0f;
    private const float StarFlyMaxExitX = 140f;
    private const float StarFlyExitUp = -100f;
    private Color starFlyColor = Calc.HexToColor("ffd65c");
    private BloomPoint starFlyBloom;
    private float starFlyTimer;
    private bool starFlyTransforming;
    private float starFlySpeedLerp;
    private Vector2 starFlyLastDir;
    private SoundSource starFlyLoopSfx;
    private SoundSource starFlyWarningSfx;
    private FlingBird flingBird;
    private SimpleCurve cassetteFlyCurve;
    private float cassetteFlyLerp;
    private Vector2 attractTo;
    public bool DummyMoving;
    public bool DummyGravity = true;
    public bool DummyFriction = true;
    public bool DummyMaxspeed = true;
    private Facings IntroWalkDirection;
    private Tween respawnTween;

    public bool Dead { get; private set; }

    public Player(Vector2 position, PlayerSpriteMode spriteMode)
      : base(new Vector2((float) (int) position.X, (float) (int) position.Y))
    {
      Input.ResetGrab();
      this.DefaultSpriteMode = spriteMode;
      this.Depth = 0;
      this.Tag = (int) Tags.Persistent;
      if (SaveData.Instance != null && SaveData.Instance.Assists.PlayAsBadeline)
        spriteMode = PlayerSpriteMode.MadelineAsBadeline;
      this.Sprite = new PlayerSprite(spriteMode);
      this.Add((Component) (this.Hair = new PlayerHair(this.Sprite)));
      this.Add((Component) this.Sprite);
      this.Hair.Color = spriteMode != PlayerSpriteMode.MadelineAsBadeline ? Player.NormalHairColor : Player.NormalBadelineHairColor;
      this.startHairCount = this.Sprite.HairCount;
      this.sweatSprite = GFX.SpriteBank.Create("player_sweat");
      this.Add((Component) this.sweatSprite);
      this.Collider = (Collider) this.normalHitbox;
      this.hurtbox = this.normalHurtbox;
      this.onCollideH = new Collision(this.OnCollideH);
      this.onCollideV = new Collision(this.OnCollideV);
      this.StateMachine = new StateMachine(26);
      this.StateMachine.SetCallbacks(0, new Func<int>(this.NormalUpdate), begin: new Action(this.NormalBegin), end: new Action(this.NormalEnd));
      this.StateMachine.SetCallbacks(1, new Func<int>(this.ClimbUpdate), begin: new Action(this.ClimbBegin), end: new Action(this.ClimbEnd));
      this.StateMachine.SetCallbacks(2, new Func<int>(this.DashUpdate), new Func<IEnumerator>(this.DashCoroutine), new Action(this.DashBegin), new Action(this.DashEnd));
      this.StateMachine.SetCallbacks(3, new Func<int>(this.SwimUpdate), begin: new Action(this.SwimBegin));
      this.StateMachine.SetCallbacks(4, new Func<int>(this.BoostUpdate), new Func<IEnumerator>(this.BoostCoroutine), new Action(this.BoostBegin), new Action(this.BoostEnd));
      this.StateMachine.SetCallbacks(5, new Func<int>(this.RedDashUpdate), new Func<IEnumerator>(this.RedDashCoroutine), new Action(this.RedDashBegin), new Action(this.RedDashEnd));
      this.StateMachine.SetCallbacks(6, new Func<int>(this.HitSquashUpdate), begin: new Action(this.HitSquashBegin));
      this.StateMachine.SetCallbacks(7, new Func<int>(this.LaunchUpdate), begin: new Action(this.LaunchBegin));
      this.StateMachine.SetCallbacks(8, (Func<int>) null, new Func<IEnumerator>(this.PickupCoroutine));
      this.StateMachine.SetCallbacks(9, new Func<int>(this.DreamDashUpdate), begin: new Action(this.DreamDashBegin), end: new Action(this.DreamDashEnd));
      this.StateMachine.SetCallbacks(10, new Func<int>(this.SummitLaunchUpdate), begin: new Action(this.SummitLaunchBegin));
      this.StateMachine.SetCallbacks(11, new Func<int>(this.DummyUpdate), begin: new Action(this.DummyBegin));
      this.StateMachine.SetCallbacks(12, (Func<int>) null, new Func<IEnumerator>(this.IntroWalkCoroutine));
      this.StateMachine.SetCallbacks(13, (Func<int>) null, new Func<IEnumerator>(this.IntroJumpCoroutine));
      this.StateMachine.SetCallbacks(14, (Func<int>) null, begin: new Action(this.IntroRespawnBegin), end: new Action(this.IntroRespawnEnd));
      this.StateMachine.SetCallbacks(15, (Func<int>) null, new Func<IEnumerator>(this.IntroWakeUpCoroutine));
      this.StateMachine.SetCallbacks(20, new Func<int>(this.TempleFallUpdate), new Func<IEnumerator>(this.TempleFallCoroutine));
      this.StateMachine.SetCallbacks(18, new Func<int>(this.ReflectionFallUpdate), new Func<IEnumerator>(this.ReflectionFallCoroutine), new Action(this.ReflectionFallBegin), new Action(this.ReflectionFallEnd));
      this.StateMachine.SetCallbacks(16, new Func<int>(this.BirdDashTutorialUpdate), new Func<IEnumerator>(this.BirdDashTutorialCoroutine), new Action(this.BirdDashTutorialBegin));
      this.StateMachine.SetCallbacks(17, new Func<int>(this.FrozenUpdate));
      this.StateMachine.SetCallbacks(19, new Func<int>(this.StarFlyUpdate), new Func<IEnumerator>(this.StarFlyCoroutine), new Action(this.StarFlyBegin), new Action(this.StarFlyEnd));
      this.StateMachine.SetCallbacks(21, new Func<int>(this.CassetteFlyUpdate), new Func<IEnumerator>(this.CassetteFlyCoroutine), new Action(this.CassetteFlyBegin), new Action(this.CassetteFlyEnd));
      this.StateMachine.SetCallbacks(22, new Func<int>(this.AttractUpdate), begin: new Action(this.AttractBegin), end: new Action(this.AttractEnd));
      this.StateMachine.SetCallbacks(23, (Func<int>) null, new Func<IEnumerator>(this.IntroMoonJumpCoroutine));
      this.StateMachine.SetCallbacks(24, new Func<int>(this.FlingBirdUpdate), new Func<IEnumerator>(this.FlingBirdCoroutine), new Action(this.FlingBirdBegin), new Action(this.FlingBirdEnd));
      this.StateMachine.SetCallbacks(25, (Func<int>) null, new Func<IEnumerator>(this.IntroThinkForABitCoroutine));
      this.Add((Component) this.StateMachine);
      this.Add((Component) (this.Leader = new Leader(new Vector2(0.0f, -8f))));
      this.lastAim = Vector2.UnitX;
      this.Facing = Facings.Right;
      this.ChaserStates = new List<Player.ChaserState>();
      this.triggersInside = new HashSet<Trigger>();
      this.Add((Component) (this.Light = new VertexLight(this.normalLightOffset, Color.White, 1f, 32, 64)));
      this.Add((Component) new WaterInteraction((Func<bool>) (() => this.StateMachine.State == 2 || this.StateMachine.State == 18)));
      this.Add((Component) new WindMover(new Action<Vector2>(this.WindMove)));
      this.Add((Component) (this.wallSlideSfx = new SoundSource()));
      this.Add((Component) (this.swimSurfaceLoopSfx = new SoundSource()));
      this.Sprite.OnFrameChange = (Action<string>) (anim =>
      {
        if (this.Scene == null || this.Dead || !this.Sprite.Visible)
          return;
        int currentAnimationFrame = this.Sprite.CurrentAnimationFrame;
        if (anim.Equals("runSlow_carry") && (currentAnimationFrame == 0 || currentAnimationFrame == 6) || anim.Equals("runFast") && (currentAnimationFrame == 0 || currentAnimationFrame == 6) || anim.Equals("runSlow") && (currentAnimationFrame == 0 || currentAnimationFrame == 6) || anim.Equals("walk") && (currentAnimationFrame == 0 || currentAnimationFrame == 6) || anim.Equals("runStumble") && currentAnimationFrame == 6 || anim.Equals("flip") && currentAnimationFrame == 4 || anim.Equals("runWind") && (currentAnimationFrame == 0 || currentAnimationFrame == 6) || anim.Equals("idleC") && this.Sprite.Mode == PlayerSpriteMode.MadelineNoBackpack && (currentAnimationFrame == 3 || currentAnimationFrame == 6 || currentAnimationFrame == 8 || currentAnimationFrame == 11) || anim.Equals("carryTheoWalk") && (currentAnimationFrame == 0 || currentAnimationFrame == 6) || anim.Equals("push") && (currentAnimationFrame == 8 || currentAnimationFrame == 15))
        {
          Platform platformByPriority = SurfaceIndex.GetPlatformByPriority(this.CollideAll<Platform>(this.Position + Vector2.UnitY, this.temp));
          if (platformByPriority != null)
            this.Play("event:/char/madeline/footstep", "surface_index", (float) platformByPriority.GetStepSoundIndex((Entity) this));
        }
        else if (anim.Equals("climbUp") && currentAnimationFrame == 5 || anim.Equals("climbDown") && currentAnimationFrame == 5)
        {
          Platform platformByPriority = SurfaceIndex.GetPlatformByPriority(this.CollideAll<Solid>(this.Center + Vector2.UnitX * (float) this.Facing, this.temp));
          if (platformByPriority != null)
            this.Play("event:/char/madeline/handhold", "surface_index", (float) platformByPriority.GetWallSoundIndex(this, (int) this.Facing));
        }
        else if (anim.Equals("wakeUp") && currentAnimationFrame == 19)
          this.Play("event:/char/madeline/campfire_stand");
        else if (anim.Equals("sitDown") && currentAnimationFrame == 12)
          this.Play("event:/char/madeline/summit_sit");
        if (!anim.Equals("push") || currentAnimationFrame != 8 && currentAnimationFrame != 15)
          return;
        Dust.BurstFG(this.Position + new Vector2((float) (-(int) this.Facing * 5), -1f), new Vector2((float) -(int) this.Facing, -0.5f).Angle(), range: 0.0f);
      });
      this.Sprite.OnLastFrame = (Action<string>) (anim =>
      {
        if (this.Scene == null || this.Dead || !(this.Sprite.CurrentAnimationID == "idle") || this.level.InCutscene || (double) this.idleTimer <= 3.0 || !Calc.Random.Chance(0.2f))
          return;
        string id = this.Sprite.Mode != PlayerSpriteMode.Madeline ? Player.idleNoBackpackOptions.Choose() : (this.level.CoreMode == Session.CoreModes.Hot ? Player.idleWarmOptions : Player.idleColdOptions).Choose();
        if (string.IsNullOrEmpty(id) || !this.Sprite.Has(id))
          return;
        this.Sprite.Play(id);
        if (this.Sprite.Mode == PlayerSpriteMode.Madeline)
        {
          switch (id)
          {
            case "idleB":
              this.idleSfx = this.Play("event:/char/madeline/idle_scratch");
              break;
            case "idleC":
              this.idleSfx = this.Play("event:/char/madeline/idle_sneeze");
              break;
          }
        }
        else
        {
          if (!(id == "idleA"))
            return;
          this.idleSfx = this.Play("event:/char/madeline/idle_crackknuckles");
        }
      });
      this.Sprite.OnChange = (Action<string, string>) ((last, next) =>
      {
        if (!(last == "idleB") && !(last == "idleC") || next == null || next.StartsWith("idle") || !((HandleBase) this.idleSfx != (HandleBase) null))
          return;
        Audio.Stop(this.idleSfx);
      });
      this.Add((Component) (this.reflection = new MirrorReflection()));
    }

    public void ResetSpriteNextFrame(PlayerSpriteMode mode) => this.nextSpriteMode = new PlayerSpriteMode?(mode);

    public void ResetSprite(PlayerSpriteMode mode)
    {
      string currentAnimationId = this.Sprite.CurrentAnimationID;
      int currentAnimationFrame = this.Sprite.CurrentAnimationFrame;
      this.Sprite.RemoveSelf();
      this.Add((Component) (this.Sprite = new PlayerSprite(mode)));
      if (this.Sprite.Has(currentAnimationId))
      {
        this.Sprite.Play(currentAnimationId);
        if (currentAnimationFrame < this.Sprite.CurrentAnimationTotalFrames)
          this.Sprite.SetAnimationFrame(currentAnimationFrame);
      }
      this.Hair.Sprite = this.Sprite;
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      this.level = this.SceneAs<Level>();
      this.lastDashes = this.Dashes = this.MaxDashes;
      SpawnFacingTrigger spawnFacingTrigger = this.CollideFirst<SpawnFacingTrigger>();
      if (spawnFacingTrigger != null)
        this.Facing = spawnFacingTrigger.Facing;
      else if ((double) this.X > (double) this.level.Bounds.Center.X && this.IntroType != Player.IntroTypes.None)
        this.Facing = Facings.Left;
      switch (this.IntroType)
      {
        case Player.IntroTypes.Respawn:
          this.StateMachine.State = 14;
          this.JustRespawned = true;
          break;
        case Player.IntroTypes.WalkInRight:
          this.IntroWalkDirection = Facings.Right;
          this.StateMachine.State = 12;
          break;
        case Player.IntroTypes.WalkInLeft:
          this.IntroWalkDirection = Facings.Left;
          this.StateMachine.State = 12;
          break;
        case Player.IntroTypes.Jump:
          this.StateMachine.State = 13;
          break;
        case Player.IntroTypes.WakeUp:
          this.Sprite.Play("asleep");
          this.Facing = Facings.Right;
          this.StateMachine.State = 15;
          break;
        case Player.IntroTypes.Fall:
          this.StateMachine.State = 18;
          break;
        case Player.IntroTypes.TempleMirrorVoid:
          this.StartTempleMirrorVoidSleep();
          break;
        case Player.IntroTypes.None:
          this.StateMachine.State = 0;
          break;
        case Player.IntroTypes.ThinkForABit:
          this.StateMachine.State = 25;
          break;
      }
      this.IntroType = Player.IntroTypes.Transition;
      this.StartHair();
      this.PreviousPosition = this.Position;
    }

    public void StartTempleMirrorVoidSleep()
    {
      this.Sprite.Play("asleep");
      this.Facing = Facings.Right;
      this.StateMachine.State = 11;
      this.StateMachine.Locked = true;
      this.DummyAutoAnimate = false;
      this.DummyGravity = false;
    }

    public override void Removed(Scene scene)
    {
      base.Removed(scene);
      this.level = (Level) null;
      Audio.Stop(this.conveyorLoopSfx);
      foreach (Trigger trigger in this.triggersInside)
      {
        trigger.Triggered = false;
        trigger.OnLeave(this);
      }
      this.triggersInside.Clear();
    }

    public override void SceneEnd(Scene scene)
    {
      base.SceneEnd(scene);
      Audio.Stop(this.conveyorLoopSfx);
    }

    public override void Render()
    {
      if (SaveData.Instance.Assists.InvisibleMotion && this.InControl && (!this.onGround && this.StateMachine.State != 1 && this.StateMachine.State != 3 || (double) this.Speed.LengthSquared() > 800.0))
        return;
      Vector2 renderPosition = this.Sprite.RenderPosition;
      this.Sprite.RenderPosition = this.Sprite.RenderPosition.Floor();
      if (this.StateMachine.State == 14)
      {
        DeathEffect.Draw(this.Center + this.deadOffset, this.Hair.Color, this.introEase);
      }
      else
      {
        if (this.StateMachine.State != 19)
        {
          if (this.IsTired && this.flash)
            this.Sprite.Color = Color.Red;
          else
            this.Sprite.Color = Color.White;
        }
        if (this.reflection.IsRendering && this.FlipInReflection)
        {
          this.Facing = ToFacing.Convert(-(int)this.Facing);
          this.Hair.Facing = this.Facing;
        }
        this.Sprite.Scale.X *= (float) this.Facing;
        if (this.sweatSprite.LastAnimationID == "idle")
        {
          this.sweatSprite.Scale = this.Sprite.Scale;
        }
        else
        {
          this.sweatSprite.Scale.Y = this.Sprite.Scale.Y;
          this.sweatSprite.Scale.X = Math.Abs(this.Sprite.Scale.X) * (float) Math.Sign(this.sweatSprite.Scale.X);
        }
        base.Render();
        if (this.Sprite.CurrentAnimationID == "startStarFly")
        {
          float num = (float) this.Sprite.CurrentAnimationFrame / (float) this.Sprite.CurrentAnimationTotalFrames;
          GFX.Game.GetAtlasSubtexturesAt("characters/player/startStarFlyWhite", this.Sprite.CurrentAnimationFrame).Draw(this.Sprite.RenderPosition, this.Sprite.Origin, this.starFlyColor * num, this.Sprite.Scale, this.Sprite.Rotation, SpriteEffects.None);
        }
        this.Sprite.Scale.X *= (float) this.Facing;
        if (this.reflection.IsRendering && this.FlipInReflection)
        {
          this.Facing = ToFacing.Convert(-(int)this.Facing);
          this.Hair.Facing = this.Facing;
        }
      }
      this.Sprite.RenderPosition = renderPosition;
    }

    public override void DebugRender(Camera camera)
    {
      base.DebugRender(camera);
      Collider collider = this.Collider;
      this.Collider = (Collider) this.hurtbox;
      Draw.HollowRect(this.Collider, Color.Lime);
      this.Collider = collider;
    }

    public override void Update()
    {
      if (SaveData.Instance.Assists.InfiniteStamina)
        this.Stamina = 110f;
      this.PreviousPosition = this.Position;
      if (this.nextSpriteMode.HasValue)
      {
        this.ResetSprite(this.nextSpriteMode.Value);
        this.nextSpriteMode = new PlayerSpriteMode?();
      }
      this.climbTriggerDir = 0;
      if (SaveData.Instance.Assists.Hiccups)
      {
        if ((double) this.hiccupTimer <= 0.0)
          this.hiccupTimer = this.level.HiccupRandom.Range(1.2f, 1.8f);
        if (this.Ducking)
          this.hiccupTimer -= Engine.DeltaTime * 0.5f;
        else
          this.hiccupTimer -= Engine.DeltaTime;
        if ((double) this.hiccupTimer <= 0.0)
          this.HiccupJump();
      }
      if ((double) this.gliderBoostTimer > 0.0)
        this.gliderBoostTimer -= Engine.DeltaTime;
      if ((double) this.lowFrictionStopTimer > 0.0)
        this.lowFrictionStopTimer -= Engine.DeltaTime;
      if ((double) this.explodeLaunchBoostTimer > 0.0)
      {
        if (Input.MoveX.Value == Math.Sign(this.explodeLaunchBoostSpeed))
        {
          this.Speed.X = this.explodeLaunchBoostSpeed;
          this.explodeLaunchBoostTimer = 0.0f;
        }
        else
          this.explodeLaunchBoostTimer -= Engine.DeltaTime;
      }
      this.StrawberryCollectResetTimer -= Engine.DeltaTime;
      if ((double) this.StrawberryCollectResetTimer <= 0.0)
        this.StrawberryCollectIndex = 0;
      this.idleTimer += Engine.DeltaTime;
      if (this.level != null && this.level.InCutscene)
        this.idleTimer = -5f;
      else if ((double) this.Speed.X != 0.0 || (double) this.Speed.Y != 0.0)
        this.idleTimer = 0.0f;
      if (!this.Dead)
        Audio.MusicUnderwater = this.UnderwaterMusicCheck();
      if (this.JustRespawned && this.Speed != Vector2.Zero)
        this.JustRespawned = false;
      if (this.StateMachine.State == 9)
        this.onGround = this.OnSafeGround = false;
      else if ((double) this.Speed.Y >= 0.0)
      {
        Platform platform = (Platform) this.CollideFirst<Solid>(this.Position + Vector2.UnitY) ?? (Platform) this.CollideFirstOutside<JumpThru>(this.Position + Vector2.UnitY);
        if (platform != null)
        {
          this.onGround = true;
          this.OnSafeGround = platform.Safe;
        }
        else
          this.onGround = this.OnSafeGround = false;
      }
      else
        this.onGround = this.OnSafeGround = false;
      if (this.StateMachine.State == 3)
        this.OnSafeGround = true;
      if (this.OnSafeGround)
      {
        foreach (SafeGroundBlocker component in this.Scene.Tracker.GetComponents<SafeGroundBlocker>())
        {
          if (component.Check(this))
          {
            this.OnSafeGround = false;
            break;
          }
        }
      }
      this.playFootstepOnLand -= Engine.DeltaTime;
      this.highestAirY = !this.onGround ? Math.Min(this.Y, this.highestAirY) : this.Y;
      if (this.Scene.OnInterval(0.05f))
        this.flash = !this.flash;
      if (this.wallSlideDir != 0)
      {
        this.wallSlideTimer = Math.Max(this.wallSlideTimer - Engine.DeltaTime, 0.0f);
        this.wallSlideDir = 0;
      }
      if ((double) this.wallBoostTimer > 0.0)
      {
        this.wallBoostTimer -= Engine.DeltaTime;
        if (this.moveX == this.wallBoostDir)
        {
          this.Speed.X = 130f * (float) this.moveX;
          this.Stamina += 27.5f;
          this.wallBoostTimer = 0.0f;
          this.sweatSprite.Play("idle");
        }
      }
      if (this.onGround && this.StateMachine.State != 1)
      {
        this.AutoJump = false;
        this.Stamina = 110f;
        this.wallSlideTimer = 1.2f;
      }
      if ((double) this.dashAttackTimer > 0.0)
        this.dashAttackTimer -= Engine.DeltaTime;
      if (this.onGround)
      {
        this.dreamJump = false;
        this.jumpGraceTimer = 0.1f;
      }
      else if ((double) this.jumpGraceTimer > 0.0)
        this.jumpGraceTimer -= Engine.DeltaTime;
      if ((double) this.dashCooldownTimer > 0.0)
        this.dashCooldownTimer -= Engine.DeltaTime;
      if ((double) this.dashRefillCooldownTimer > 0.0)
        this.dashRefillCooldownTimer -= Engine.DeltaTime;
      else if (SaveData.Instance.Assists.DashMode == Assists.DashModes.Infinite && !this.level.InCutscene)
        this.RefillDash();
      else if (!this.Inventory.NoRefills)
      {
        if (this.StateMachine.State == 3)
          this.RefillDash();
        else if (this.onGround && (this.CollideCheck<Solid, NegaBlock>(this.Position + Vector2.UnitY) || this.CollideCheckOutside<JumpThru>(this.Position + Vector2.UnitY)) && (!this.CollideCheck<Spikes>(this.Position) || SaveData.Instance.Assists.Invincible))
          this.RefillDash();
      }
      if ((double) this.varJumpTimer > 0.0)
        this.varJumpTimer -= Engine.DeltaTime;
      if ((double) this.AutoJumpTimer > 0.0)
      {
        if (this.AutoJump)
        {
          this.AutoJumpTimer -= Engine.DeltaTime;
          if ((double) this.AutoJumpTimer <= 0.0)
            this.AutoJump = false;
        }
        else
          this.AutoJumpTimer = 0.0f;
      }
      if ((double) this.forceMoveXTimer > 0.0)
      {
        this.forceMoveXTimer -= Engine.DeltaTime;
        this.moveX = this.forceMoveX;
      }
      else
      {
        this.moveX = Input.MoveX.Value;
        this.climbHopSolid = (Solid) null;
      }
      if (this.climbHopSolid != null && !this.climbHopSolid.Collidable)
        this.climbHopSolid = (Solid) null;
      else if (this.climbHopSolid != null && this.climbHopSolid.Position != this.climbHopSolidPosition)
      {
        Vector2 vector2 = this.climbHopSolid.Position - this.climbHopSolidPosition;
        this.climbHopSolidPosition = this.climbHopSolid.Position;
        this.MoveHExact((int) vector2.X);
        this.MoveVExact((int) vector2.Y);
      }
      if ((double) this.noWindTimer > 0.0)
        this.noWindTimer -= Engine.DeltaTime;
      if (this.moveX != 0 && this.InControl && this.StateMachine.State != 1 && this.StateMachine.State != 8 && this.StateMachine.State != 5 && this.StateMachine.State != 6)
      {
        Facings moveX = (Facings) this.moveX;
        if (moveX != this.Facing && this.Ducking)
          this.Sprite.Scale = new Vector2(0.8f, 1.2f);
        this.Facing = moveX;
      }
      this.lastAim = Input.GetAimVector(this.Facing);
      if ((double) this.wallSpeedRetentionTimer > 0.0)
      {
        if (Math.Sign(this.Speed.X) == -Math.Sign(this.wallSpeedRetained))
          this.wallSpeedRetentionTimer = 0.0f;
        else if (!this.CollideCheck<Solid>(this.Position + Vector2.UnitX * (float) Math.Sign(this.wallSpeedRetained)))
        {
          this.Speed.X = this.wallSpeedRetained;
          this.wallSpeedRetentionTimer = 0.0f;
        }
        else
          this.wallSpeedRetentionTimer -= Engine.DeltaTime;
      }
      if (this.hopWaitX != 0)
      {
        if (Math.Sign(this.Speed.X) == -this.hopWaitX || (double) this.Speed.Y > 0.0)
          this.hopWaitX = 0;
        else if (!this.CollideCheck<Solid>(this.Position + Vector2.UnitX * (float) this.hopWaitX))
        {
          this.lowFrictionStopTimer = 0.15f;
          this.Speed.X = this.hopWaitXSpeed;
          this.hopWaitX = 0;
        }
      }
      if ((double) this.windTimeout > 0.0)
        this.windTimeout -= Engine.DeltaTime;
      Vector2 vector2_1 = this.windDirection;
      if ((double) this.ForceStrongWindHair.Length() > 0.0)
        vector2_1 = this.ForceStrongWindHair;
      if ((double) this.windTimeout > 0.0 && (double) vector2_1.X != 0.0)
      {
        this.windHairTimer += Engine.DeltaTime * 8f;
        this.Hair.StepPerSegment = new Vector2(vector2_1.X * 5f, (float) Math.Sin((double) this.windHairTimer));
        this.Hair.StepInFacingPerSegment = 0.0f;
        this.Hair.StepApproach = 128f;
        this.Hair.StepYSinePerSegment = 0.0f;
      }
      else if (this.Dashes > 1)
      {
        this.Hair.StepPerSegment = new Vector2((float) Math.Sin((double) this.Scene.TimeActive * 2.0) * 0.7f - (float) ((int) this.Facing * 3), (float) Math.Sin((double) this.Scene.TimeActive * 1.0));
        this.Hair.StepInFacingPerSegment = 0.0f;
        this.Hair.StepApproach = 90f;
        this.Hair.StepYSinePerSegment = 1f;
        this.Hair.StepPerSegment.Y += vector2_1.Y * 2f;
      }
      else
      {
        this.Hair.StepPerSegment = new Vector2(0.0f, 2f);
        this.Hair.StepInFacingPerSegment = 0.5f;
        this.Hair.StepApproach = 64f;
        this.Hair.StepYSinePerSegment = 0.0f;
        this.Hair.StepPerSegment.Y += vector2_1.Y * 0.5f;
      }
      if (this.StateMachine.State == 5)
        this.Sprite.HairCount = 1;
      else if (this.StateMachine.State != 19)
        this.Sprite.HairCount = this.Dashes > 1 ? 5 : this.startHairCount;
      if ((double) this.minHoldTimer > 0.0)
        this.minHoldTimer -= Engine.DeltaTime;
      if (this.launched)
      {
        if ((double) this.Speed.LengthSquared() < 19600.0)
        {
          this.launched = false;
        }
        else
        {
          float launchedTimer = this.launchedTimer;
          this.launchedTimer += Engine.DeltaTime;
          if ((double) this.launchedTimer >= 0.5)
          {
            this.launched = false;
            this.launchedTimer = 0.0f;
          }
          else if (Calc.OnInterval(this.launchedTimer, launchedTimer, 0.15f))
            this.level.Add((Entity) Engine.Pooler.Create<SpeedRing>().Init(this.Center, this.Speed.Angle(), Color.White));
        }
      }
      else
        this.launchedTimer = 0.0f;
      if (this.IsTired)
      {
        Input.Rumble(RumbleStrength.Light, RumbleLength.Short);
        if (!this.wasTired)
          this.wasTired = true;
      }
      else
        this.wasTired = false;
      base.Update();
      this.Light.Position = !this.Ducking ? this.normalLightOffset : this.duckingLightOffset;
      if (!this.onGround && (double) this.Speed.Y <= 0.0 && (this.StateMachine.State != 1 || this.lastClimbMove == -1) && this.CollideCheck<JumpThru>() && !this.JumpThruBoostBlockedCheck())
        this.MoveV(-40f * Engine.DeltaTime);
      if (!this.onGround && this.DashAttacking && (double) this.DashDir.Y == 0.0 && (this.CollideCheck<Solid>(this.Position + Vector2.UnitY * 3f) || this.CollideCheckOutside<JumpThru>(this.Position + Vector2.UnitY * 3f)) && !this.DashCorrectCheck(Vector2.UnitY * 3f))
        this.MoveVExact(3);
      if ((double) this.Speed.Y > 0.0 && this.CanUnDuck && this.Collider != this.starFlyHitbox && !this.onGround && (double) this.jumpGraceTimer <= 0.0)
        this.Ducking = false;
      if (this.StateMachine.State != 9 && this.StateMachine.State != 22)
        this.MoveH(this.Speed.X * Engine.DeltaTime, this.onCollideH);
      if (this.StateMachine.State != 9 && this.StateMachine.State != 22)
        this.MoveV(this.Speed.Y * Engine.DeltaTime, this.onCollideV);
      if (this.StateMachine.State == 3)
      {
        if ((double) this.Speed.Y < 0.0 && (double) this.Speed.Y >= -60.0)
        {
          while (!this.SwimCheck())
          {
            this.Speed.Y = 0.0f;
            if (this.MoveVExact(1))
              break;
          }
        }
      }
      else if (this.StateMachine.State == 0 && this.SwimCheck())
        this.StateMachine.State = 3;
      else if (this.StateMachine.State == 1 && this.SwimCheck())
      {
        Water water = this.CollideFirst<Water>(this.Position);
        if (water != null && (double) this.Center.Y < (double) water.Center.Y)
        {
          do
            ;
          while (this.SwimCheck() && !this.MoveVExact(-1));
          if (this.SwimCheck())
            this.StateMachine.State = 3;
        }
        else
          this.StateMachine.State = 3;
      }
      if ((this.Sprite.CurrentAnimationID == null || !this.Sprite.CurrentAnimationID.Equals("wallslide") ? 0 : ((double) this.Speed.Y > 0.0 ? 1 : 0)) != 0)
      {
        if (!this.wallSlideSfx.Playing)
          this.Loop(this.wallSlideSfx, "event:/char/madeline/wallslide");
        Platform platformByPriority = SurfaceIndex.GetPlatformByPriority(this.CollideAll<Solid>(this.Center + Vector2.UnitX * (float) this.Facing, this.temp));
        if (platformByPriority != null)
          this.wallSlideSfx.Param("surface_index", (float) platformByPriority.GetWallSoundIndex(this, (int) this.Facing));
      }
      else
        this.Stop(this.wallSlideSfx);
      this.UpdateSprite();
      this.UpdateCarry();
      if (this.StateMachine.State != 18)
      {
        foreach (Trigger entity in this.Scene.Tracker.GetEntities<Trigger>())
        {
          if (this.CollideCheck((Entity) entity))
          {
            if (!entity.Triggered)
            {
              entity.Triggered = true;
              this.triggersInside.Add(entity);
              entity.OnEnter(this);
            }
            entity.OnStay(this);
          }
          else if (entity.Triggered)
          {
            this.triggersInside.Remove(entity);
            entity.Triggered = false;
            entity.OnLeave(this);
          }
        }
      }
      this.StrawberriesBlocked = this.CollideCheck<BlockField>();
      if (this.InControl || this.ForceCameraUpdate)
      {
        if (this.StateMachine.State == 18)
        {
          this.level.Camera.Position = this.CameraTarget;
        }
        else
        {
          Vector2 position = this.level.Camera.Position;
          Vector2 cameraTarget = this.CameraTarget;
          float num = this.StateMachine.State == 20 ? 8f : 1f;
          this.level.Camera.Position = position + (cameraTarget - position) * (1f - (float) Math.Pow(0.009999999776482582 / (double) num, (double) Engine.DeltaTime));
        }
      }
      if (!this.Dead && this.StateMachine.State != 21)
      {
        Collider collider = this.Collider;
        this.Collider = (Collider) this.hurtbox;
        foreach (PlayerCollider component in this.Scene.Tracker.GetComponents<PlayerCollider>())
        {
          if (component.Check(this) && this.Dead)
          {
            this.Collider = collider;
            return;
          }
        }
        if (this.Collider == this.hurtbox)
          this.Collider = collider;
      }
      if (this.InControl && !this.Dead && this.StateMachine.State != 9 && this.EnforceLevelBounds)
        this.level.EnforceBounds(this);
      this.UpdateChaserStates();
      this.UpdateHair(true);
      if (this.wasDucking != this.Ducking)
      {
        this.wasDucking = this.Ducking;
        if (this.wasDucking)
          this.Play("event:/char/madeline/duck");
        else if (this.onGround)
          this.Play("event:/char/madeline/stand");
      }
      if ((double) this.Speed.X != 0.0 && (this.StateMachine.State == 3 && !this.SwimUnderwaterCheck() || this.StateMachine.State == 0 && this.CollideCheck<Water>(this.Position)))
      {
        if (!this.swimSurfaceLoopSfx.Playing)
          this.swimSurfaceLoopSfx.Play("event:/char/madeline/water_move_shallow");
      }
      else
        this.swimSurfaceLoopSfx.Stop();
      this.wasOnGround = this.onGround;
      this.windMovedUp = false;
    }

    private void CreateTrail()
    {
      Vector2 scale = new Vector2(Math.Abs(this.Sprite.Scale.X) * (float) this.Facing, this.Sprite.Scale.Y);
      if (this.Sprite.Mode == PlayerSpriteMode.MadelineAsBadeline)
        TrailManager.Add((Entity) this, scale, this.wasDashB ? Player.NormalBadelineHairColor : Player.UsedBadelineHairColor);
      else
        TrailManager.Add((Entity) this, scale, this.wasDashB ? Player.NormalHairColor : Player.UsedHairColor);
    }

    public void CleanUpTriggers()
    {
      if (this.triggersInside.Count <= 0)
        return;
      foreach (Trigger trigger in this.triggersInside)
      {
        trigger.OnLeave(this);
        trigger.Triggered = false;
      }
      this.triggersInside.Clear();
    }

    private void UpdateChaserStates()
    {
      while (this.ChaserStates.Count > 0 && (double) this.Scene.TimeActive - (double) this.ChaserStates[0].TimeStamp > 4.0)
        this.ChaserStates.RemoveAt(0);
      this.ChaserStates.Add(new Player.ChaserState(this));
      this.activeSounds.Clear();
    }

    private void StartHair()
    {
      if (this.startHairCalled)
        return;
      this.startHairCalled = true;
      this.Hair.Facing = this.Facing;
      this.Hair.Start();
      this.UpdateHair(true);
    }

    public void UpdateHair(bool applyGravity)
    {
      if (this.StateMachine.State == 19)
      {
        this.Hair.Color = this.Sprite.Color;
        applyGravity = false;
      }
      else if (this.Dashes == 0 && this.Dashes < this.MaxDashes)
      {
        this.Hair.Color = this.Sprite.Mode != PlayerSpriteMode.MadelineAsBadeline ? Color.Lerp(this.Hair.Color, Player.UsedHairColor, 6f * Engine.DeltaTime) : Color.Lerp(this.Hair.Color, Player.UsedBadelineHairColor, 6f * Engine.DeltaTime);
      }
      else
      {
        Color color;
        if (this.lastDashes != this.Dashes)
        {
          color = Player.FlashHairColor;
          this.hairFlashTimer = 0.12f;
        }
        else if ((double) this.hairFlashTimer > 0.0)
        {
          color = Player.FlashHairColor;
          this.hairFlashTimer -= Engine.DeltaTime;
        }
        else
          color = this.Sprite.Mode != PlayerSpriteMode.MadelineAsBadeline ? (this.Dashes != 2 ? Player.NormalHairColor : Player.TwoDashesHairColor) : (this.Dashes != 2 ? Player.NormalBadelineHairColor : Player.TwoDashesBadelineHairColor);
        this.Hair.Color = color;
      }
      if (this.OverrideHairColor.HasValue)
        this.Hair.Color = this.OverrideHairColor.Value;
      this.Hair.Facing = this.Facing;
      this.Hair.SimulateMotion = applyGravity;
      this.lastDashes = this.Dashes;
    }

    private void UpdateSprite()
    {
      this.Sprite.Scale.X = Calc.Approach(this.Sprite.Scale.X, 1f, 1.75f * Engine.DeltaTime);
      this.Sprite.Scale.Y = Calc.Approach(this.Sprite.Scale.Y, 1f, 1.75f * Engine.DeltaTime);
      if (this.InControl && this.Sprite.CurrentAnimationID != "throw" && this.StateMachine.State != 20 && this.StateMachine.State != 18 && this.StateMachine.State != 19 && this.StateMachine.State != 21)
      {
        if (this.StateMachine.State == 22)
          this.Sprite.Play("fallFast");
        else if (this.StateMachine.State == 10)
          this.Sprite.Play("launch");
        else if (this.StateMachine.State == 8)
          this.Sprite.Play("pickup");
        else if (this.StateMachine.State == 3)
        {
          if (Input.MoveY.Value > 0)
            this.Sprite.Play("swimDown");
          else if (Input.MoveY.Value < 0)
            this.Sprite.Play("swimUp");
          else
            this.Sprite.Play("swimIdle");
        }
        else if (this.StateMachine.State == 9)
        {
          if (this.Sprite.CurrentAnimationID != "dreamDashIn" && this.Sprite.CurrentAnimationID != "dreamDashLoop")
            this.Sprite.Play("dreamDashIn");
        }
        else if (this.Sprite.DreamDashing && this.Sprite.LastAnimationID != "dreamDashOut")
          this.Sprite.Play("dreamDashOut");
        else if (this.Sprite.CurrentAnimationID != "dreamDashOut")
        {
          if (this.DashAttacking)
          {
            if (this.onGround && (double) this.DashDir.Y == 0.0 && !this.Ducking && (double) this.Speed.X != 0.0 && this.moveX == -Math.Sign(this.Speed.X))
            {
              if (this.Scene.OnInterval(0.02f))
                Dust.Burst(this.Position, -1.5707964f);
              this.Sprite.Play("skid");
            }
            else if (this.Ducking)
              this.Sprite.Play("duck");
            else
              this.Sprite.Play("dash");
          }
          else if (this.StateMachine.State == 1)
          {
            if (this.lastClimbMove < 0)
              this.Sprite.Play("climbUp");
            else if (this.lastClimbMove > 0)
              this.Sprite.Play("wallslide");
            else if (!this.CollideCheck<Solid>(this.Position + new Vector2((float) this.Facing, 6f)))
              this.Sprite.Play("dangling");
            else if ((double) (float) Input.MoveX == (double) -(int) this.Facing)
            {
              if (this.Sprite.CurrentAnimationID != "climbLookBack")
                this.Sprite.Play("climbLookBackStart");
            }
            else
              this.Sprite.Play("wallslide");
          }
          else if (this.Ducking && this.StateMachine.State == 0)
            this.Sprite.Play("duck");
          else if (this.onGround)
          {
            this.fastJump = false;
            if (this.Holding == null && this.moveX != 0 && this.CollideCheck<Solid>(this.Position + Vector2.UnitX * (float) this.moveX) && !ClimbBlocker.EdgeCheck((Scene) this.level, (Entity) this, this.moveX))
              this.Sprite.Play("push");
            else if ((double) Math.Abs(this.Speed.X) <= 25.0 && this.moveX == 0)
            {
              if (this.Holding != null)
                this.Sprite.Play("idle_carry");
              else if (!this.Scene.CollideCheck<Solid>(this.Position + new Vector2((float) this.Facing, 2f)) && !this.Scene.CollideCheck<Solid>(this.Position + new Vector2((float) ((int) this.Facing * 4), 2f)) && !this.CollideCheck<JumpThru>(this.Position + new Vector2((float) ((int) this.Facing * 4), 2f)))
                this.Sprite.Play("edge");
              else if (!this.Scene.CollideCheck<Solid>(this.Position + new Vector2((float) -(int) this.Facing, 2f)) && !this.Scene.CollideCheck<Solid>(this.Position + new Vector2((float) (-(int) this.Facing * 4), 2f)) && !this.CollideCheck<JumpThru>(this.Position + new Vector2((float) (-(int) this.Facing * 4), 2f)))
                this.Sprite.Play("edgeBack");
              else if (Input.MoveY.Value == -1)
              {
                if (this.Sprite.LastAnimationID != "lookUp")
                  this.Sprite.Play("lookUp");
              }
              else if (this.Sprite.CurrentAnimationID != null && (!this.Sprite.CurrentAnimationID.Contains("idle") || this.Sprite.CurrentAnimationID == "idle_carry" && this.Holding == null))
                this.Sprite.Play("idle");
            }
            else if (this.Holding != null)
              this.Sprite.Play("runSlow_carry");
            else if (Math.Sign(this.Speed.X) == -this.moveX && this.moveX != 0)
            {
              if ((double) Math.Abs(this.Speed.X) > 90.0)
                this.Sprite.Play("skid");
              else if (this.Sprite.CurrentAnimationID != "skid")
                this.Sprite.Play("flip");
            }
            else if ((double) this.windDirection.X != 0.0 && (double) this.windTimeout > 0.0 && this.Facing == ToFacing.Convert(-Math.Sign(this.windDirection.X)))
              this.Sprite.Play("runWind");
            else if (!this.Sprite.Running || this.Sprite.CurrentAnimationID == "runWind" || this.Sprite.CurrentAnimationID == "runSlow_carry" && this.Holding == null)
            {
              if ((double) Math.Abs(this.Speed.X) < 45.0)
                this.Sprite.Play("runSlow");
              else
                this.Sprite.Play("runFast");
            }
          }
          else if (this.wallSlideDir != 0 && this.Holding == null)
            this.Sprite.Play("wallslide");
          else if ((double) this.Speed.Y < 0.0)
          {
            if (this.Holding != null)
              this.Sprite.Play("jumpSlow_carry");
            else if (this.fastJump || (double) Math.Abs(this.Speed.X) > 90.0)
            {
              this.fastJump = true;
              this.Sprite.Play("jumpFast");
            }
            else
              this.Sprite.Play("jumpSlow");
          }
          else if (this.Holding != null)
            this.Sprite.Play("fallSlow_carry");
          else if (this.fastJump || (double) this.Speed.Y >= 160.0 || this.level.InSpace)
          {
            this.fastJump = true;
            if (this.Sprite.LastAnimationID != "fallFast")
              this.Sprite.Play("fallFast");
          }
          else
            this.Sprite.Play("fallSlow");
        }
      }
      if (this.StateMachine.State == 11)
        return;
      if (this.level.InSpace)
        this.Sprite.Rate = 0.5f;
      else
        this.Sprite.Rate = 1f;
    }

    public void CreateSplitParticles() => this.level.Particles.Emit(Player.P_Split, 16, this.Center, Vector2.One * 6f);

    public Vector2 CameraTarget
    {
      get
      {
        Vector2 cameraTarget = new Vector2();
        Vector2 vector2 = new Vector2(this.X - 160f, this.Y - 90f);
        if (this.StateMachine.State != 18)
          vector2 += new Vector2(this.level.CameraOffset.X, this.level.CameraOffset.Y);
        if (this.StateMachine.State == 19)
        {
          vector2.X += 0.2f * this.Speed.X;
          vector2.Y += 0.2f * this.Speed.Y;
        }
        else if (this.StateMachine.State == 5)
        {
          vector2.X += (float) (48 * Math.Sign(this.Speed.X));
          vector2.Y += (float) (48 * Math.Sign(this.Speed.Y));
        }
        else if (this.StateMachine.State == 10)
          vector2.Y -= 64f;
        else if (this.StateMachine.State == 18)
          vector2.Y += 32f;
        if ((double) this.CameraAnchorLerp.Length() > 0.0)
        {
          if (this.CameraAnchorIgnoreX && !this.CameraAnchorIgnoreY)
            vector2.Y = MathHelper.Lerp(vector2.Y, this.CameraAnchor.Y, this.CameraAnchorLerp.Y);
          else if (!this.CameraAnchorIgnoreX && this.CameraAnchorIgnoreY)
            vector2.X = MathHelper.Lerp(vector2.X, this.CameraAnchor.X, this.CameraAnchorLerp.X);
          else if ((double) this.CameraAnchorLerp.X == (double) this.CameraAnchorLerp.Y)
          {
            vector2 = Vector2.Lerp(vector2, this.CameraAnchor, this.CameraAnchorLerp.X);
          }
          else
          {
            vector2.X = MathHelper.Lerp(vector2.X, this.CameraAnchor.X, this.CameraAnchorLerp.X);
            vector2.Y = MathHelper.Lerp(vector2.Y, this.CameraAnchor.Y, this.CameraAnchorLerp.Y);
          }
        }
        Rectangle bounds;
        if (this.EnforceLevelBounds)
        {
          cameraTarget.X = MathHelper.Clamp(vector2.X, (float) this.level.Bounds.Left, (float) (this.level.Bounds.Right - 320));
          ref Vector2 local = ref cameraTarget;
          double y = (double) vector2.Y;
          bounds = this.level.Bounds;
          double top = (double) bounds.Top;
          bounds = this.level.Bounds;
          double max = (double) (bounds.Bottom - 180);
          double num = (double) MathHelper.Clamp((float) y, (float) top, (float) max);
          local.Y = (float) num;
        }
        else
          cameraTarget = vector2;
        if (this.level.CameraLockMode != Level.CameraLockModes.None)
        {
          CameraLocker component = this.Scene.Tracker.GetComponent<CameraLocker>();
          if (this.level.CameraLockMode != Level.CameraLockModes.BoostSequence)
          {
            cameraTarget.X = Math.Max(cameraTarget.X, this.level.Camera.X);
            if (component != null)
            {
              ref Vector2 local = ref cameraTarget;
              double x = (double) cameraTarget.X;
              bounds = this.level.Bounds;
              double val2 = (double) Math.Max((float) bounds.Left, component.Entity.X - component.MaxXOffset);
              double num = (double) Math.Min((float) x, (float) val2);
              local.X = (float) num;
            }
          }
          if (this.level.CameraLockMode == Level.CameraLockModes.FinalBoss)
          {
            cameraTarget.Y = Math.Max(cameraTarget.Y, this.level.Camera.Y);
            if (component != null)
            {
              ref Vector2 local = ref cameraTarget;
              double y = (double) cameraTarget.Y;
              bounds = this.level.Bounds;
              double val2 = (double) Math.Max((float) bounds.Top, component.Entity.Y - component.MaxYOffset);
              double num = (double) Math.Min((float) y, (float) val2);
              local.Y = (float) num;
            }
          }
          else if (this.level.CameraLockMode == Level.CameraLockModes.BoostSequence)
          {
            this.level.CameraUpwardMaxY = Math.Min(this.level.Camera.Y + 180f, this.level.CameraUpwardMaxY);
            cameraTarget.Y = Math.Min(cameraTarget.Y, this.level.CameraUpwardMaxY);
            if (component != null)
            {
              ref Vector2 local = ref cameraTarget;
              double y = (double) cameraTarget.Y;
              bounds = this.level.Bounds;
              double val2 = (double) Math.Min((float) (bounds.Bottom - 180), component.Entity.Y - component.MaxYOffset);
              double num = (double) Math.Max((float) y, (float) val2);
              local.Y = (float) num;
            }
          }
        }
        foreach (Entity entity in this.Scene.Tracker.GetEntities<Killbox>())
        {
          if (entity.Collidable && (double) this.Top < (double) entity.Bottom && (double) this.Right > (double) entity.Left && (double) this.Left < (double) entity.Right)
            cameraTarget.Y = Math.Min(cameraTarget.Y, entity.Top - 180f);
        }
        return cameraTarget;
      }
    }

    public bool GetChasePosition(float sceneTime, float timeAgo, out Player.ChaserState chaseState)
    {
      if (!this.Dead)
      {
        bool flag = false;
        foreach (Player.ChaserState chaserState in this.ChaserStates)
        {
          float num = sceneTime - chaserState.TimeStamp;
          if ((double) num <= (double) timeAgo)
          {
            if (flag || (double) timeAgo - (double) num < 0.019999999552965164)
            {
              chaseState = chaserState;
              return true;
            }
            chaseState = new Player.ChaserState();
            return false;
          }
          flag = true;
        }
      }
      chaseState = new Player.ChaserState();
      return false;
    }

    public bool CanRetry
    {
      get
      {
        switch (this.StateMachine.State)
        {
          case 12:
          case 13:
          case 14:
          case 15:
          case 18:
          case 25:
            return false;
          default:
            return true;
        }
      }
    }

    public bool TimePaused
    {
      get
      {
        if (this.Dead)
          return true;
        switch (this.StateMachine.State)
        {
          case 10:
          case 12:
          case 13:
          case 14:
          case 15:
          case 25:
            return true;
          default:
            return false;
        }
      }
    }

    public bool InControl
    {
      get
      {
        switch (this.StateMachine.State)
        {
          case 11:
          case 12:
          case 13:
          case 14:
          case 15:
          case 16:
          case 17:
          case 23:
          case 25:
            return false;
          default:
            return true;
        }
      }
    }

    public PlayerInventory Inventory => this.level != null && this.level.Session != null ? this.level.Session.Inventory : PlayerInventory.Default;

    public void OnTransition()
    {
      this.wallSlideTimer = 1.2f;
      this.jumpGraceTimer = 0.0f;
      this.forceMoveXTimer = 0.0f;
      this.ChaserStates.Clear();
      this.RefillDash();
      this.RefillStamina();
      this.Leader.TransferFollowers();
    }

    public bool TransitionTo(Vector2 target, Vector2 direction)
    {
      this.MoveTowardsX(target.X, 60f * Engine.DeltaTime);
      this.MoveTowardsY(target.Y, 60f * Engine.DeltaTime);
      this.UpdateHair(false);
      this.UpdateCarry();
      if (!(this.Position == target))
        return false;
      this.ZeroRemainderX();
      this.ZeroRemainderY();
      this.Speed.X = (float) (int) Math.Round((double) this.Speed.X);
      this.Speed.Y = (float) (int) Math.Round((double) this.Speed.Y);
      return true;
    }

    public void BeforeSideTransition()
    {
    }

    public void BeforeDownTransition()
    {
      if (this.StateMachine.State != 5 && this.StateMachine.State != 18 && this.StateMachine.State != 19)
      {
        this.StateMachine.State = 0;
        this.Speed.Y = Math.Max(0.0f, this.Speed.Y);
        this.AutoJump = false;
        this.varJumpTimer = 0.0f;
      }
      foreach (Entity entity in this.Scene.Tracker.GetEntities<Platform>())
      {
        if (!(entity is SolidTiles) && this.CollideCheckOutside(entity, this.Position + Vector2.UnitY * this.Height))
          entity.Collidable = false;
      }
    }

    public void BeforeUpTransition()
    {
      this.Speed.X = 0.0f;
      if (this.StateMachine.State != 5 && this.StateMachine.State != 18 && this.StateMachine.State != 19)
      {
        this.varJumpSpeed = this.Speed.Y = -105f;
        this.StateMachine.State = this.StateMachine.State != 10 ? 0 : 13;
        this.AutoJump = true;
        this.AutoJumpTimer = 0.0f;
        this.varJumpTimer = 0.2f;
      }
      this.dashCooldownTimer = 0.2f;
    }

    public bool OnSafeGround { get; private set; }

    public bool LoseShards => this.onGround;

    private bool LaunchedBoostCheck()
    {
      if ((double) this.LiftBoost.LengthSquared() >= 10000.0 && (double) this.Speed.LengthSquared() >= 48400.0)
      {
        this.launched = true;
        return true;
      }
      this.launched = false;
      return false;
    }

    public void HiccupJump()
    {
      switch (this.StateMachine.State)
      {
        case 1:
          this.StateMachine.State = 0;
          this.varJumpSpeed = this.Speed.Y = -60f;
          this.varJumpTimer = 0.15f;
          this.Speed.X = 130f * (float) -(int) this.Facing;
          this.AutoJump = true;
          this.AutoJumpTimer = 0.0f;
          this.sweatSprite.Play("jump", true);
          break;
        case 4:
        case 7:
        case 22:
          this.sweatSprite.Play("jump", true);
          break;
        case 5:
        case 9:
          this.Speed = (double) this.Speed.X < 0.0 || (double) this.Speed.X == 0.0 && (double) this.Speed.Y < 0.0 ? this.Speed.Rotate(0.17453292f) : this.Speed.Rotate(-0.17453292f);
          break;
        case 10:
          return;
        case 11:
          return;
        case 12:
          return;
        case 13:
          return;
        case 14:
          return;
        case 15:
          return;
        case 16:
          return;
        case 17:
          return;
        case 18:
          return;
        case 19:
          this.Speed = (double) this.Speed.X <= 0.0 ? this.Speed.Rotate(-0.6981317f) : this.Speed.Rotate(0.6981317f);
          break;
        case 21:
          return;
        case 24:
          return;
        default:
          this.StateMachine.State = 0;
          this.Speed.X = Calc.Approach(this.Speed.X, 0.0f, 40f);
          if ((double) this.Speed.Y > -60.0)
          {
            this.varJumpSpeed = this.Speed.Y = -60f;
            this.varJumpTimer = 0.15f;
            this.AutoJump = true;
            this.AutoJumpTimer = 0.0f;
            if ((double) this.jumpGraceTimer > 0.0)
              this.jumpGraceTimer = 0.6f;
          }
          this.sweatSprite.Play("jump", true);
          break;
      }
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Short);
      this.Play(this.Ducking ? "event:/new_content/char/madeline/hiccup_ducking" : "event:/new_content/char/madeline/hiccup_standing");
    }

    public void Jump(bool particles = true, bool playSfx = true)
    {
      Input.Jump.ConsumeBuffer();
      this.jumpGraceTimer = 0.0f;
      this.varJumpTimer = 0.2f;
      this.AutoJump = false;
      this.dashAttackTimer = 0.0f;
      this.gliderBoostTimer = 0.0f;
      this.wallSlideTimer = 1.2f;
      this.wallBoostTimer = 0.0f;
      this.Speed.X += 40f * (float) this.moveX;
      this.Speed.Y = -105f;
      this.Speed += this.LiftBoost;
      this.varJumpSpeed = this.Speed.Y;
      this.LaunchedBoostCheck();
      if (playSfx)
      {
        if (this.launched)
          this.Play("event:/char/madeline/jump_assisted");
        if (this.dreamJump)
          this.Play("event:/char/madeline/jump_dreamblock");
        else
          this.Play("event:/char/madeline/jump");
      }
      this.Sprite.Scale = new Vector2(0.6f, 1.4f);
      if (particles)
      {
        int index = -1;
        Platform platformByPriority = SurfaceIndex.GetPlatformByPriority(this.CollideAll<Platform>(this.Position + Vector2.UnitY, this.temp));
        if (platformByPriority != null)
          index = platformByPriority.GetLandSoundIndex((Entity) this);
        Dust.Burst(this.BottomCenter, -1.5707964f, 4, this.DustParticleFromSurfaceIndex(index));
      }
      ++SaveData.Instance.TotalJumps;
    }

    private void SuperJump()
    {
      Input.Jump.ConsumeBuffer();
      this.jumpGraceTimer = 0.0f;
      this.varJumpTimer = 0.2f;
      this.AutoJump = false;
      this.dashAttackTimer = 0.0f;
      this.gliderBoostTimer = 0.0f;
      this.wallSlideTimer = 1.2f;
      this.wallBoostTimer = 0.0f;
      this.Speed.X = 260f * (float) this.Facing;
      this.Speed.Y = -105f;
      this.Speed += this.LiftBoost;
      this.gliderBoostTimer = 0.55f;
      this.Play("event:/char/madeline/jump");
      if (this.Ducking)
      {
        this.Ducking = false;
        this.Speed.X *= 1.25f;
        this.Speed.Y *= 0.5f;
        this.Play("event:/char/madeline/jump_superslide");
        this.gliderBoostDir = Calc.AngleToVector(-3f * (float) Math.PI / 16f, 1f);
      }
      else
      {
        this.gliderBoostDir = Calc.AngleToVector(-0.7853982f, 1f);
        this.Play("event:/char/madeline/jump_super");
      }
      this.varJumpSpeed = this.Speed.Y;
      this.launched = true;
      this.Sprite.Scale = new Vector2(0.6f, 1.4f);
      int index = -1;
      Platform platformByPriority = SurfaceIndex.GetPlatformByPriority(this.CollideAll<Platform>(this.Position + Vector2.UnitY, this.temp));
      if (platformByPriority != null)
        index = platformByPriority.GetLandSoundIndex((Entity) this);
      Dust.Burst(this.BottomCenter, -1.5707964f, 4, this.DustParticleFromSurfaceIndex(index));
      ++SaveData.Instance.TotalJumps;
    }

    private bool WallJumpCheck(int dir)
    {
      int num = 3;
      bool flag = this.DashAttacking && (double) this.DashDir.X == 0.0 && (double) this.DashDir.Y == -1.0;
      if (flag)
      {
        Spikes.Directions directions = dir <= 0 ? Spikes.Directions.Right : Spikes.Directions.Left;
        foreach (Spikes entity in this.level.Tracker.GetEntities<Spikes>())
        {
          if (entity.Direction == directions && this.CollideCheck((Entity) entity, this.Position + Vector2.UnitX * (float) dir * 5f))
          {
            flag = false;
            break;
          }
        }
      }
      if (flag)
        num = 5;
      return this.ClimbBoundsCheck(dir) && !ClimbBlocker.EdgeCheck((Scene) this.level, (Entity) this, dir * num) && this.CollideCheck<Solid>(this.Position + Vector2.UnitX * (float) dir * (float) num);
    }

    private void WallJump(int dir)
    {
      this.Ducking = false;
      Input.Jump.ConsumeBuffer();
      this.jumpGraceTimer = 0.0f;
      this.varJumpTimer = 0.2f;
      this.AutoJump = false;
      this.dashAttackTimer = 0.0f;
      this.gliderBoostTimer = 0.0f;
      this.wallSlideTimer = 1.2f;
      this.wallBoostTimer = 0.0f;
      this.lowFrictionStopTimer = 0.15f;
      if (this.Holding != null && this.Holding.SlowFall)
      {
        this.forceMoveX = dir;
        this.forceMoveXTimer = 0.26f;
      }
      else if (this.moveX != 0)
      {
        this.forceMoveX = dir;
        this.forceMoveXTimer = 0.16f;
      }
      if (this.LiftSpeed == Vector2.Zero)
      {
        Solid solid = this.CollideFirst<Solid>(this.Position + Vector2.UnitX * 3f * (float) -dir);
        if (solid != null)
          this.LiftSpeed = solid.LiftSpeed;
      }
      this.Speed.X = 130f * (float) dir;
      this.Speed.Y = -105f;
      this.Speed += this.LiftBoost;
      this.varJumpSpeed = this.Speed.Y;
      this.LaunchedBoostCheck();
      int index = -1;
      Platform platformByPriority = SurfaceIndex.GetPlatformByPriority(this.CollideAll<Platform>(this.Position - Vector2.UnitX * (float) dir * 4f, this.temp));
      if (platformByPriority != null)
      {
        index = platformByPriority.GetWallSoundIndex(this, -dir);
        this.Play("event:/char/madeline/landing", "surface_index", (float) index);
        if (platformByPriority is DreamBlock)
          (platformByPriority as DreamBlock).FootstepRipple(this.Position + new Vector2((float) (dir * 3), -4f));
      }
      this.Play(dir < 0 ? "event:/char/madeline/jump_wall_right" : "event:/char/madeline/jump_wall_left");
      this.Sprite.Scale = new Vector2(0.6f, 1.4f);
      if (dir == -1)
        Dust.Burst(this.Center + Vector2.UnitX * 2f, -2.3561945f, 4, this.DustParticleFromSurfaceIndex(index));
      else
        Dust.Burst(this.Center + Vector2.UnitX * -2f, -0.7853982f, 4, this.DustParticleFromSurfaceIndex(index));
      ++SaveData.Instance.TotalWallJumps;
    }

    private void SuperWallJump(int dir)
    {
      this.Ducking = false;
      Input.Jump.ConsumeBuffer();
      this.jumpGraceTimer = 0.0f;
      this.varJumpTimer = 0.25f;
      this.AutoJump = false;
      this.dashAttackTimer = 0.0f;
      this.gliderBoostTimer = 0.55f;
      this.gliderBoostDir = -Vector2.UnitY;
      this.wallSlideTimer = 1.2f;
      this.wallBoostTimer = 0.0f;
      this.Speed.X = 170f * (float) dir;
      this.Speed.Y = -160f;
      this.Speed += this.LiftBoost;
      this.varJumpSpeed = this.Speed.Y;
      this.launched = true;
      this.Play(dir < 0 ? "event:/char/madeline/jump_wall_right" : "event:/char/madeline/jump_wall_left");
      this.Play("event:/char/madeline/jump_superwall");
      this.Sprite.Scale = new Vector2(0.6f, 1.4f);
      int index = -1;
      Platform platformByPriority = SurfaceIndex.GetPlatformByPriority(this.CollideAll<Platform>(this.Position - Vector2.UnitX * (float) dir * 4f, this.temp));
      if (platformByPriority != null)
        index = platformByPriority.GetWallSoundIndex(this, dir);
      if (dir == -1)
        Dust.Burst(this.Center + Vector2.UnitX * 2f, -2.3561945f, 4, this.DustParticleFromSurfaceIndex(index));
      else
        Dust.Burst(this.Center + Vector2.UnitX * -2f, -0.7853982f, 4, this.DustParticleFromSurfaceIndex(index));
      ++SaveData.Instance.TotalWallJumps;
    }

    private void ClimbJump()
    {
      if (!this.onGround)
      {
        this.Stamina -= 27.5f;
        this.sweatSprite.Play("jump", true);
        Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
      }
      this.dreamJump = false;
      this.Jump(false, false);
      if (this.moveX == 0)
      {
        this.wallBoostDir = -(int) this.Facing;
        this.wallBoostTimer = 0.2f;
      }
      int index = -1;
      Platform platformByPriority = SurfaceIndex.GetPlatformByPriority(this.CollideAll<Platform>(this.Position - Vector2.UnitX * (float) this.Facing * 4f, this.temp));
      if (platformByPriority != null)
        index = platformByPriority.GetWallSoundIndex(this, (int) this.Facing);
      if (this.Facing == Facings.Right)
      {
        this.Play("event:/char/madeline/jump_climb_right");
        Dust.Burst(this.Center + Vector2.UnitX * 2f, -2.3561945f, 4, this.DustParticleFromSurfaceIndex(index));
      }
      else
      {
        this.Play("event:/char/madeline/jump_climb_left");
        Dust.Burst(this.Center + Vector2.UnitX * -2f, -0.7853982f, 4, this.DustParticleFromSurfaceIndex(index));
      }
    }

    public void Bounce(float fromY)
    {
      if (this.StateMachine.State == 4 && this.CurrentBooster != null)
      {
        this.CurrentBooster.PlayerReleased();
        this.CurrentBooster = (Booster) null;
      }
      Collider collider = this.Collider;
      this.Collider = (Collider) this.normalHitbox;
      this.MoveVExact((int) ((double) fromY - (double) this.Bottom));
      if (!this.Inventory.NoRefills)
        this.RefillDash();
      this.RefillStamina();
      this.StateMachine.State = 0;
      this.jumpGraceTimer = 0.0f;
      this.varJumpTimer = 0.2f;
      this.AutoJump = true;
      this.AutoJumpTimer = 0.1f;
      this.dashAttackTimer = 0.0f;
      this.gliderBoostTimer = 0.0f;
      this.wallSlideTimer = 1.2f;
      this.wallBoostTimer = 0.0f;
      this.varJumpSpeed = this.Speed.Y = -140f;
      this.launched = false;
      Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
      this.Sprite.Scale = new Vector2(0.6f, 1.4f);
      this.Collider = collider;
    }

    public void SuperBounce(float fromY)
    {
      if (this.StateMachine.State == 4 && this.CurrentBooster != null)
      {
        this.CurrentBooster.PlayerReleased();
        this.CurrentBooster = (Booster) null;
      }
      Collider collider = this.Collider;
      this.Collider = (Collider) this.normalHitbox;
      this.MoveV(fromY - this.Bottom);
      if (!this.Inventory.NoRefills)
        this.RefillDash();
      this.RefillStamina();
      this.StateMachine.State = 0;
      this.jumpGraceTimer = 0.0f;
      this.varJumpTimer = 0.2f;
      this.AutoJump = true;
      this.AutoJumpTimer = 0.0f;
      this.dashAttackTimer = 0.0f;
      this.gliderBoostTimer = 0.0f;
      this.wallSlideTimer = 1.2f;
      this.wallBoostTimer = 0.0f;
      this.Speed.X = 0.0f;
      this.varJumpSpeed = this.Speed.Y = -185f;
      this.launched = false;
      this.level.DirectionalShake(-Vector2.UnitY, 0.1f);
      Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
      this.Sprite.Scale = new Vector2(0.5f, 1.5f);
      this.Collider = collider;
    }

    public bool SideBounce(int dir, float fromX, float fromY)
    {
      if ((double) Math.Abs(this.Speed.X) > 240.0 && Math.Sign(this.Speed.X) == dir)
        return false;
      Collider collider = this.Collider;
      this.Collider = (Collider) this.normalHitbox;
      this.MoveV(Calc.Clamp(fromY - this.Bottom, -4f, 4f));
      if (dir > 0)
        this.MoveH(fromX - this.Left);
      else if (dir < 0)
        this.MoveH(fromX - this.Right);
      if (!this.Inventory.NoRefills)
        this.RefillDash();
      this.RefillStamina();
      this.StateMachine.State = 0;
      this.jumpGraceTimer = 0.0f;
      this.varJumpTimer = 0.2f;
      this.AutoJump = true;
      this.AutoJumpTimer = 0.0f;
      this.dashAttackTimer = 0.0f;
      this.gliderBoostTimer = 0.0f;
      this.wallSlideTimer = 1.2f;
      this.forceMoveX = dir;
      this.forceMoveXTimer = 0.3f;
      this.wallBoostTimer = 0.0f;
      this.launched = false;
      this.Speed.X = 240f * (float) dir;
      this.varJumpSpeed = this.Speed.Y = -140f;
      this.level.DirectionalShake(Vector2.UnitX * (float) dir, 0.1f);
      Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
      this.Sprite.Scale = new Vector2(1.5f, 0.5f);
      this.Collider = collider;
      return true;
    }

    public void Rebound(int direction = 0)
    {
      this.Speed.X = (float) direction * 120f;
      this.Speed.Y = -120f;
      this.varJumpSpeed = this.Speed.Y;
      this.varJumpTimer = 0.15f;
      this.AutoJump = true;
      this.AutoJumpTimer = 0.0f;
      this.dashAttackTimer = 0.0f;
      this.gliderBoostTimer = 0.0f;
      this.wallSlideTimer = 1.2f;
      this.wallBoostTimer = 0.0f;
      this.launched = false;
      this.lowFrictionStopTimer = 0.15f;
      this.forceMoveXTimer = 0.0f;
      this.StateMachine.State = 0;
    }

    public void ReflectBounce(Vector2 direction)
    {
      if ((double) direction.X != 0.0)
        this.Speed.X = direction.X * 220f;
      if ((double) direction.Y != 0.0)
        this.Speed.Y = direction.Y * 220f;
      this.AutoJumpTimer = 0.0f;
      this.dashAttackTimer = 0.0f;
      this.gliderBoostTimer = 0.0f;
      this.wallSlideTimer = 1.2f;
      this.wallBoostTimer = 0.0f;
      this.launched = false;
      this.dashAttackTimer = 0.0f;
      this.gliderBoostTimer = 0.0f;
      this.forceMoveXTimer = 0.0f;
      this.StateMachine.State = 0;
    }

    public int MaxDashes => SaveData.Instance.Assists.DashMode != Assists.DashModes.Normal && !this.level.InCutscene ? 2 : this.Inventory.Dashes;

    public bool RefillDash()
    {
      if (this.Dashes >= this.MaxDashes)
        return false;
      this.Dashes = this.MaxDashes;
      return true;
    }

    public bool UseRefill(bool twoDashes)
    {
      int num = this.MaxDashes;
      if (twoDashes)
        num = 2;
      if (this.Dashes >= num && (double) this.Stamina >= 20.0)
        return false;
      this.Dashes = num;
      this.RefillStamina();
      return true;
    }

    public void RefillStamina() => this.Stamina = 110f;

    public PlayerDeadBody Die(Vector2 direction, bool evenIfInvincible = false, bool registerDeathInStats = true)
    {
      Session session = this.level.Session;
      bool flag = !evenIfInvincible && SaveData.Instance.Assists.Invincible;
      if (this.Dead || flag || this.StateMachine.State == 18)
        return (PlayerDeadBody) null;
      this.Stop(this.wallSlideSfx);
      if (registerDeathInStats)
      {
        ++session.Deaths;
        ++session.DeathsInCurrentLevel;
        SaveData.Instance.AddDeath(session.Area);
      }
      Strawberry goldenStrawb = (Strawberry) null;
      foreach (Follower follower in this.Leader.Followers)
      {
        if (follower.Entity is Strawberry && (follower.Entity as Strawberry).Golden && !(follower.Entity as Strawberry).Winged)
          goldenStrawb = follower.Entity as Strawberry;
      }
      this.Dead = true;
      this.Leader.LoseFollowers();
      this.Depth = -1000000;
      this.Speed = Vector2.Zero;
      this.StateMachine.Locked = true;
      this.Collidable = false;
      this.Drop();
      if (this.LastBooster != null)
        this.LastBooster.PlayerDied();
      this.level.InCutscene = false;
      this.level.Shake();
      Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
      PlayerDeadBody playerDeadBody = new PlayerDeadBody(this, direction);
      if (goldenStrawb != null)
      {
        playerDeadBody.HasGolden = true;
        playerDeadBody.DeathAction = (Action) (() => Engine.Scene = (Scene) new LevelExit(LevelExit.Mode.GoldenBerryRestart, session)
        {
          GoldenStrawberryEntryLevel = goldenStrawb.ID.Level
        });
      }
      this.Scene.Add((Entity) playerDeadBody);
      this.Scene.Remove((Entity) this);
      this.Scene.Tracker.GetEntity<Lookout>()?.StopInteracting();
      return playerDeadBody;
    }

    private Vector2 LiftBoost
    {
      get
      {
        Vector2 liftSpeed = this.LiftSpeed;
        if ((double) Math.Abs(liftSpeed.X) > 250.0)
          liftSpeed.X = 250f * (float) Math.Sign(liftSpeed.X);
        if ((double) liftSpeed.Y > 0.0)
          liftSpeed.Y = 0.0f;
        else if ((double) liftSpeed.Y < -130.0)
          liftSpeed.Y = -130f;
        return liftSpeed;
      }
    }

    public bool Ducking
    {
      get => this.Collider == this.duckHitbox || this.Collider == this.duckHurtbox;
      set
      {
        if (value)
        {
          this.Collider = (Collider) this.duckHitbox;
          this.hurtbox = this.duckHurtbox;
        }
        else
        {
          this.Collider = (Collider) this.normalHitbox;
          this.hurtbox = this.normalHurtbox;
        }
      }
    }

    public bool CanUnDuck
    {
      get
      {
        if (!this.Ducking)
          return true;
        Collider collider = this.Collider;
        this.Collider = (Collider) this.normalHitbox;
        int num = !this.CollideCheck<Solid>() ? 1 : 0;
        this.Collider = collider;
        return num != 0;
      }
    }

    public bool CanUnDuckAt(Vector2 at)
    {
      Vector2 position = this.Position;
      this.Position = at;
      int num = this.CanUnDuck ? 1 : 0;
      this.Position = position;
      return num != 0;
    }

    public bool DuckFreeAt(Vector2 at)
    {
      Vector2 position = this.Position;
      Collider collider = this.Collider;
      this.Position = at;
      this.Collider = (Collider) this.duckHitbox;
      int num = !this.CollideCheck<Solid>() ? 1 : 0;
      this.Position = position;
      this.Collider = collider;
      return num != 0;
    }

    private void Duck() => this.Collider = (Collider) this.duckHitbox;

    private void UnDuck() => this.Collider = (Collider) this.normalHitbox;

    public Holdable Holding { get; set; }

    public void UpdateCarry()
    {
      if (this.Holding == null)
        return;
      if (this.Holding.Scene == null)
        this.Holding = (Holdable) null;
      else
        this.Holding.Carry(this.Position + this.carryOffset + Vector2.UnitY * this.Sprite.CarryYOffset);
    }

    public void Swat(int dir)
    {
      if (this.Holding == null)
        return;
      this.Holding.Release(new Vector2(0.8f * (float) dir, -0.25f));
      this.Holding = (Holdable) null;
    }

    private bool Pickup(Holdable pickup)
    {
      if (!pickup.Pickup(this))
        return false;
      this.Ducking = false;
      this.Holding = pickup;
      this.minHoldTimer = 0.35f;
      return true;
    }

    public void Throw()
    {
      if (this.Holding == null)
        return;
      if (Input.MoveY.Value == 1)
      {
        this.Drop();
      }
      else
      {
        Input.Rumble(RumbleStrength.Strong, RumbleLength.Short);
        this.Holding.Release(Vector2.UnitX * (float) this.Facing);
        this.Speed.X += 80f * (float) -(int) this.Facing;
        this.Play("event:/char/madeline/crystaltheo_throw");
        this.Sprite.Play("throw");
      }
      this.Holding = (Holdable) null;
    }

    public void Drop()
    {
      if (this.Holding == null)
        return;
      Input.Rumble(RumbleStrength.Light, RumbleLength.Short);
      this.Holding.Release(Vector2.Zero);
      this.Holding = (Holdable) null;
    }

    public void StartJumpGraceTime() => this.jumpGraceTimer = 0.1f;

    public override bool IsRiding(Solid solid)
    {
      if (this.StateMachine.State == 23)
        return false;
      if (this.StateMachine.State == 9)
        return this.CollideCheck((Entity) solid);
      if (this.StateMachine.State == 1 || this.StateMachine.State == 6)
        return this.CollideCheck((Entity) solid, this.Position + Vector2.UnitX * (float) this.Facing);
      return this.climbTriggerDir != 0 ? this.CollideCheck((Entity) solid, this.Position + Vector2.UnitX * (float) this.climbTriggerDir) : base.IsRiding(solid);
    }

    public override bool IsRiding(JumpThru jumpThru) => this.StateMachine.State != 9 && this.StateMachine.State != 1 && (double) this.Speed.Y >= 0.0 && base.IsRiding(jumpThru);

    public bool BounceCheck(float y) => (double) this.Bottom <= (double) y + 3.0;

    public void PointBounce(Vector2 from)
    {
      if (this.StateMachine.State == 2)
        this.StateMachine.State = 0;
      if (this.StateMachine.State == 4 && this.CurrentBooster != null)
        this.CurrentBooster.PlayerReleased();
      this.RefillDash();
      this.RefillStamina();
      Vector2 vector2 = (this.Center - from).SafeNormalize();
      if ((double) vector2.Y > -0.20000000298023224 && (double) vector2.Y <= 0.4000000059604645)
        vector2.Y = -0.2f;
      this.Speed = vector2 * 220f;
      this.Speed.X *= 1.5f;
      if ((double) Math.Abs(this.Speed.X) >= 100.0)
        return;
      if ((double) this.Speed.X == 0.0)
        this.Speed.X = (float) -(int) this.Facing * 100f;
      else
        this.Speed.X = (float) Math.Sign(this.Speed.X) * 100f;
    }

    private void WindMove(Vector2 move)
    {
      if (this.JustRespawned || (double) this.noWindTimer > 0.0 || !this.InControl || this.StateMachine.State == 4 || this.StateMachine.State == 2 || this.StateMachine.State == 10)
        return;
      if ((double) move.X != 0.0 && this.StateMachine.State != 1)
      {
        this.windTimeout = 0.2f;
        this.windDirection.X = (float) Math.Sign(move.X);
        if (!this.CollideCheck<Solid>(this.Position + Vector2.UnitX * (float) -Math.Sign(move.X) * 3f))
        {
          if (this.Ducking && this.onGround)
            move.X *= 0.0f;
          move.X = (double) move.X >= 0.0 ? Math.Min(move.X, (float) this.level.Bounds.Right - (this.ExactPosition.X + this.Collider.Right)) : Math.Max(move.X, (float) this.level.Bounds.Left - (this.ExactPosition.X + this.Collider.Left));
          this.MoveH(move.X);
        }
      }
      if ((double) move.Y == 0.0)
        return;
      this.windTimeout = 0.2f;
      this.windDirection.Y = (float) Math.Sign(move.Y);
      if ((double) this.Bottom <= (double) this.level.Bounds.Top || (double) this.Speed.Y >= 0.0 && this.OnGround())
        return;
      if (this.StateMachine.State == 1)
      {
        if ((double) move.Y <= 0.0 || (double) this.climbNoMoveTimer > 0.0)
          return;
        move.Y *= 0.4f;
      }
      if ((double) move.Y < 0.0)
        this.windMovedUp = true;
      this.MoveV(move.Y);
    }

    private void OnCollideH(CollisionData data)
    {
      this.canCurveDash = false;
      if (this.StateMachine.State == 19)
      {
        if ((double) this.starFlyTimer < 0.20000000298023224)
        {
          this.Speed.X = 0.0f;
        }
        else
        {
          this.Play("event:/game/06_reflection/feather_state_bump");
          Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
          this.Speed.X *= -0.5f;
        }
      }
      else
      {
        if (this.StateMachine.State == 9)
          return;
        if (this.DashAttacking && data.Hit != null && data.Hit.OnDashCollide != null && (double) data.Direction.X == (double) Math.Sign(this.DashDir.X))
        {
          DashCollisionResults collisionResults = data.Hit.OnDashCollide(this, data.Direction);
          if (collisionResults == DashCollisionResults.NormalOverride)
            collisionResults = DashCollisionResults.NormalCollision;
          else if (this.StateMachine.State == 5)
            collisionResults = DashCollisionResults.Ignore;
          switch (collisionResults)
          {
            case DashCollisionResults.Rebound:
              this.Rebound(-Math.Sign(this.Speed.X));
              return;
            case DashCollisionResults.Bounce:
              this.ReflectBounce(new Vector2((float) -Math.Sign(this.Speed.X), 0.0f));
              return;
            case DashCollisionResults.Ignore:
              return;
          }
        }
        if (this.StateMachine.State == 2 || this.StateMachine.State == 5)
        {
          if (this.onGround && this.DuckFreeAt(this.Position + Vector2.UnitX * (float) Math.Sign(this.Speed.X)))
          {
            this.Ducking = true;
            return;
          }
          if ((double) this.Speed.Y == 0.0 && (double) this.Speed.X != 0.0)
          {
            for (int index1 = 1; index1 <= 4; ++index1)
            {
              for (int index2 = 1; index2 >= -1; index2 -= 2)
              {
                Vector2 add = new Vector2((float) Math.Sign(this.Speed.X), (float) (index1 * index2));
                Vector2 at = this.Position + add;
                if (!this.CollideCheck<Solid>(at) && this.CollideCheck<Solid>(at - Vector2.UnitY * (float) index2) && !this.DashCorrectCheck(add))
                {
                  this.MoveVExact(index1 * index2);
                  this.MoveHExact(Math.Sign(this.Speed.X));
                  return;
                }
              }
            }
          }
        }
        if (this.DreamDashCheck(Vector2.UnitX * (float) Math.Sign(this.Speed.X)))
        {
          this.StateMachine.State = 9;
          this.dashAttackTimer = 0.0f;
          this.gliderBoostTimer = 0.0f;
        }
        else
        {
          if ((double) this.wallSpeedRetentionTimer <= 0.0)
          {
            this.wallSpeedRetained = this.Speed.X;
            this.wallSpeedRetentionTimer = 0.06f;
          }
          if (data.Hit != null && data.Hit.OnCollide != null)
            data.Hit.OnCollide(data.Direction);
          this.Speed.X = 0.0f;
          this.dashAttackTimer = 0.0f;
          this.gliderBoostTimer = 0.0f;
          if (this.StateMachine.State != 5)
            return;
          Input.Rumble(RumbleStrength.Medium, RumbleLength.Short);
          this.level.Displacement.AddBurst(this.Center, 0.5f, 8f, 48f, 0.4f, Ease.QuadOut, Ease.QuadOut);
          this.StateMachine.State = 6;
        }
      }
    }

    private void OnCollideV(CollisionData data)
    {
      this.canCurveDash = false;
      if (this.StateMachine.State == 19)
      {
        if ((double) this.starFlyTimer < 0.20000000298023224)
        {
          this.Speed.Y = 0.0f;
        }
        else
        {
          this.Play("event:/game/06_reflection/feather_state_bump");
          Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
          this.Speed.Y *= -0.5f;
        }
      }
      else if (this.StateMachine.State == 3)
      {
        this.Speed.Y = 0.0f;
      }
      else
      {
        if (this.StateMachine.State == 9)
          return;
        if (data.Hit != null && data.Hit.OnDashCollide != null)
        {
          if (this.DashAttacking && (double) data.Direction.Y == (double) Math.Sign(this.DashDir.Y))
          {
            DashCollisionResults collisionResults = data.Hit.OnDashCollide(this, data.Direction);
            if (this.StateMachine.State == 5)
              collisionResults = DashCollisionResults.Ignore;
            switch (collisionResults)
            {
              case DashCollisionResults.Rebound:
                this.Rebound();
                return;
              case DashCollisionResults.Bounce:
                this.ReflectBounce(new Vector2(0.0f, (float) -Math.Sign(this.Speed.Y)));
                return;
              case DashCollisionResults.Ignore:
                return;
            }
          }
          else if (this.StateMachine.State == 10)
          {
            int num = (int) data.Hit.OnDashCollide(this, data.Direction);
            return;
          }
        }
        if ((double) this.Speed.Y > 0.0)
        {
          if ((this.StateMachine.State == 2 || this.StateMachine.State == 5) && !this.dashStartedOnGround)
          {
            if ((double) this.Speed.X <= 0.009999999776482582)
            {
              for (int index = -1; index >= -4; --index)
              {
                if (!this.OnGround(this.Position + new Vector2((float) index, 0.0f)))
                {
                  this.MoveHExact(index);
                  this.MoveVExact(1);
                  return;
                }
              }
            }
            if ((double) this.Speed.X >= -0.009999999776482582)
            {
              for (int index = 1; index <= 4; ++index)
              {
                if (!this.OnGround(this.Position + new Vector2((float) index, 0.0f)))
                {
                  this.MoveHExact(index);
                  this.MoveVExact(1);
                  return;
                }
              }
            }
          }
          if (this.DreamDashCheck(Vector2.UnitY * (float) Math.Sign(this.Speed.Y)))
          {
            this.StateMachine.State = 9;
            this.dashAttackTimer = 0.0f;
            this.gliderBoostTimer = 0.0f;
            return;
          }
          if ((double) this.DashDir.X != 0.0 && (double) this.DashDir.Y > 0.0 && (double) this.Speed.Y > 0.0)
          {
            this.DashDir.X = (float) Math.Sign(this.DashDir.X);
            this.DashDir.Y = 0.0f;
            this.Speed.Y = 0.0f;
            this.Speed.X *= 1.2f;
            this.Ducking = true;
          }
          if (this.StateMachine.State != 1)
          {
            float amount = Math.Min(this.Speed.Y / 240f, 1f);
            this.Sprite.Scale.X = MathHelper.Lerp(1f, 1.6f, amount);
            this.Sprite.Scale.Y = MathHelper.Lerp(1f, 0.4f, amount);
            if ((double) this.highestAirY < (double) this.Y - 50.0 && (double) this.Speed.Y >= 160.0 && (double) Math.Abs(this.Speed.X) >= 90.0)
              this.Sprite.Play("runStumble");
            Input.Rumble(RumbleStrength.Light, RumbleLength.Short);
            Platform platformByPriority = SurfaceIndex.GetPlatformByPriority(this.CollideAll<Platform>(this.Position + new Vector2(0.0f, 1f), this.temp));
            int index = -1;
            if (platformByPriority != null)
            {
              index = platformByPriority.GetLandSoundIndex((Entity) this);
              if (index >= 0 && !this.MuffleLanding)
                this.Play((double) this.playFootstepOnLand > 0.0 ? "event:/char/madeline/footstep" : "event:/char/madeline/landing", "surface_index", (float) index);
              if (platformByPriority is DreamBlock)
                (platformByPriority as DreamBlock).FootstepRipple(this.Position);
              this.MuffleLanding = false;
            }
            if ((double) this.Speed.Y >= 80.0)
              Dust.Burst(this.Position, new Vector2(0.0f, -1f).Angle(), 8, this.DustParticleFromSurfaceIndex(index));
            this.playFootstepOnLand = 0.0f;
          }
        }
        else
        {
          if ((double) this.Speed.Y < 0.0)
          {
            int num = 4;
            if (this.DashAttacking && (double) Math.Abs(this.Speed.X) < 0.009999999776482582)
              num = 5;
            if ((double) this.Speed.X <= 0.009999999776482582)
            {
              for (int index = 1; index <= num; ++index)
              {
                if (!this.CollideCheck<Solid>(this.Position + new Vector2((float) -index, -1f)))
                {
                  this.Position = this.Position + new Vector2((float) -index, -1f);
                  return;
                }
              }
            }
            if ((double) this.Speed.X >= -0.009999999776482582)
            {
              for (int x = 1; x <= num; ++x)
              {
                if (!this.CollideCheck<Solid>(this.Position + new Vector2((float) x, -1f)))
                {
                  this.Position = this.Position + new Vector2((float) x, -1f);
                  return;
                }
              }
            }
            if ((double) this.varJumpTimer < 0.15000000596046448)
              this.varJumpTimer = 0.0f;
          }
          if (this.DreamDashCheck(Vector2.UnitY * (float) Math.Sign(this.Speed.Y)))
          {
            this.StateMachine.State = 9;
            this.dashAttackTimer = 0.0f;
            this.gliderBoostTimer = 0.0f;
            return;
          }
        }
        if (data.Hit != null && data.Hit.OnCollide != null)
          data.Hit.OnCollide(data.Direction);
        this.dashAttackTimer = 0.0f;
        this.gliderBoostTimer = 0.0f;
        this.Speed.Y = 0.0f;
        if (this.StateMachine.State != 5)
          return;
        Input.Rumble(RumbleStrength.Medium, RumbleLength.Short);
        this.level.Displacement.AddBurst(this.Center, 0.5f, 8f, 48f, 0.4f, Ease.QuadOut, Ease.QuadOut);
        this.StateMachine.State = 6;
      }
    }

    private bool DreamDashCheck(Vector2 dir)
    {
      if (this.Inventory.DreamDash && this.DashAttacking && ((double) dir.X == (double) Math.Sign(this.DashDir.X) || (double) dir.Y == (double) Math.Sign(this.DashDir.Y)))
      {
        DreamBlock dreamBlock = this.CollideFirst<DreamBlock>(this.Position + dir);
        if (dreamBlock != null)
        {
          if (this.CollideCheck<Solid, DreamBlock>(this.Position + dir))
          {
            Vector2 vector2 = new Vector2(Math.Abs(dir.Y), Math.Abs(dir.X));
            bool flag1;
            bool flag2;
            if ((double) dir.X != 0.0)
            {
              flag1 = (double) this.Speed.Y <= 0.0;
              flag2 = (double) this.Speed.Y >= 0.0;
            }
            else
            {
              flag1 = (double) this.Speed.X <= 0.0;
              flag2 = (double) this.Speed.X >= 0.0;
            }
            if (flag1)
            {
              for (int index = -1; index >= -4; --index)
              {
                if (!this.CollideCheck<Solid, DreamBlock>(this.Position + dir + vector2 * (float) index))
                {
                  this.Position = this.Position + vector2 * (float) index;
                  this.dreamBlock = dreamBlock;
                  return true;
                }
              }
            }
            if (flag2)
            {
              for (int index = 1; index <= 4; ++index)
              {
                if (!this.CollideCheck<Solid, DreamBlock>(this.Position + dir + vector2 * (float) index))
                {
                  this.Position = this.Position + vector2 * (float) index;
                  this.dreamBlock = dreamBlock;
                  return true;
                }
              }
            }
            return false;
          }
          this.dreamBlock = dreamBlock;
          return true;
        }
      }
      return false;
    }

    public void OnBoundsH()
    {
      this.Speed.X = 0.0f;
      if (this.StateMachine.State != 5)
        return;
      this.StateMachine.State = 0;
    }

    public void OnBoundsV()
    {
      this.Speed.Y = 0.0f;
      if (this.StateMachine.State != 5)
        return;
      this.StateMachine.State = 0;
    }

    protected override void OnSquish(CollisionData data)
    {
      bool flag = false;
      if (!this.Ducking && this.StateMachine.State != 1)
      {
        flag = true;
        this.Ducking = true;
        data.Pusher.Collidable = true;
        if (!this.CollideCheck<Solid>())
        {
          data.Pusher.Collidable = false;
          return;
        }
        Vector2 position = this.Position;
        this.Position = data.TargetPosition;
        if (!this.CollideCheck<Solid>())
        {
          data.Pusher.Collidable = false;
          return;
        }
        this.Position = position;
        data.Pusher.Collidable = false;
      }
      if (!this.TrySquishWiggle(data, wiggleY: 5))
      {
        bool evenIfInvincible = false;
        if (data.Pusher != null && data.Pusher.SquishEvenInAssistMode)
          evenIfInvincible = true;
        this.Die(Vector2.Zero, evenIfInvincible);
      }
      else
      {
        if (!flag || !this.CanUnDuck)
          return;
        this.Ducking = false;
      }
    }

    private void NormalBegin() => this.maxFall = 160f;

    private void NormalEnd()
    {
      this.wallBoostTimer = 0.0f;
      this.wallSpeedRetentionTimer = 0.0f;
      this.hopWaitX = 0;
    }

    public bool ClimbBoundsCheck(int dir) => (double) this.Left + (double) (dir * 2) >= (double) this.level.Bounds.Left && (double) this.Right + (double) (dir * 2) < (double) this.level.Bounds.Right;

    public void ClimbTrigger(int dir) => this.climbTriggerDir = dir;

    public bool ClimbCheck(int dir, int yAdd = 0) => this.ClimbBoundsCheck(dir) && !ClimbBlocker.Check(this.Scene, (Entity) this, this.Position + Vector2.UnitY * (float) yAdd + Vector2.UnitX * 2f * (float) this.Facing) && this.CollideCheck<Solid>(this.Position + new Vector2((float) (dir * 2), (float) yAdd));

    private int NormalUpdate()
    {
      if ((double) this.LiftBoost.Y < 0.0 && this.wasOnGround && !this.onGround && (double) this.Speed.Y >= 0.0)
        this.Speed.Y = this.LiftBoost.Y;
      if (this.Holding == null)
      {
        if (Input.GrabCheck && !this.IsTired && !this.Ducking)
        {
          foreach (Holdable component in this.Scene.Tracker.GetComponents<Holdable>())
          {
            if (component.Check(this) && this.Pickup(component))
              return 8;
          }
          if ((double) this.Speed.Y >= 0.0 && Math.Sign(this.Speed.X) != -(int) this.Facing)
          {
            if (this.ClimbCheck((int) this.Facing))
            {
              this.Ducking = false;
              if (!SaveData.Instance.Assists.NoGrabbing)
                return 1;
              this.ClimbTrigger((int) this.Facing);
            }
            if (!SaveData.Instance.Assists.NoGrabbing && (double) (float) Input.MoveY < 1.0 && (double) this.level.Wind.Y <= 0.0)
            {
              for (int index = 1; index <= 2; ++index)
              {
                if (!this.CollideCheck<Solid>(this.Position + Vector2.UnitY * (float) -index) && this.ClimbCheck((int) this.Facing, -index))
                {
                  this.MoveVExact(-index);
                  this.Ducking = false;
                  return 1;
                }
              }
            }
          }
        }
        if (this.CanDash)
        {
          this.Speed += this.LiftBoost;
          return this.StartDash();
        }
        if (this.Ducking)
        {
          if (this.onGround && (double) (float) Input.MoveY != 1.0)
          {
            if (this.CanUnDuck)
            {
              this.Ducking = false;
              this.Sprite.Scale = new Vector2(0.8f, 1.2f);
            }
            else if ((double) this.Speed.X == 0.0)
            {
              for (int index = 4; index > 0; --index)
              {
                if (this.CanUnDuckAt(this.Position + Vector2.UnitX * (float) index))
                {
                  this.MoveH(50f * Engine.DeltaTime);
                  break;
                }
                if (this.CanUnDuckAt(this.Position - Vector2.UnitX * (float) index))
                {
                  this.MoveH(-50f * Engine.DeltaTime);
                  break;
                }
              }
            }
          }
        }
        else if (this.onGround && (double) (float) Input.MoveY == 1.0 && (double) this.Speed.Y >= 0.0)
        {
          this.Ducking = true;
          this.Sprite.Scale = new Vector2(1.4f, 0.6f);
        }
      }
      else
      {
        if (!Input.GrabCheck && (double) this.minHoldTimer <= 0.0)
          this.Throw();
        if (!this.Ducking && this.onGround && (double) (float) Input.MoveY == 1.0 && (double) this.Speed.Y >= 0.0 && !this.holdCannotDuck)
        {
          this.Drop();
          this.Ducking = true;
          this.Sprite.Scale = new Vector2(1.4f, 0.6f);
        }
        else if (this.onGround && this.Ducking && (double) this.Speed.Y >= 0.0)
        {
          if (this.CanUnDuck)
            this.Ducking = false;
          else
            this.Drop();
        }
        else if (this.onGround && (double) (float) Input.MoveY != 1.0 && this.holdCannotDuck)
          this.holdCannotDuck = false;
      }
      if (this.Ducking && this.onGround)
      {
        this.Speed.X = Calc.Approach(this.Speed.X, 0.0f, 500f * Engine.DeltaTime);
      }
      else
      {
        float num1 = this.onGround ? 1f : 0.65f;
        if (this.onGround && this.level.CoreMode == Session.CoreModes.Cold)
          num1 *= 0.3f;
        if (SaveData.Instance.Assists.LowFriction && (double) this.lowFrictionStopTimer <= 0.0)
          num1 *= this.onGround ? 0.35f : 0.5f;
        float num2;
        if (this.Holding != null && this.Holding.SlowRun)
          num2 = 70f;
        else if (this.Holding != null && this.Holding.SlowFall && !this.onGround)
        {
          num2 = 108.00001f;
          num1 *= 0.5f;
        }
        else
          num2 = 90f;
        if (this.level.InSpace)
          num2 *= 0.6f;
        this.Speed.X = (double) Math.Abs(this.Speed.X) <= (double) num2 || Math.Sign(this.Speed.X) != this.moveX ? Calc.Approach(this.Speed.X, num2 * (float) this.moveX, 1000f * num1 * Engine.DeltaTime) : Calc.Approach(this.Speed.X, num2 * (float) this.moveX, 400f * num1 * Engine.DeltaTime);
      }
      float target1 = 160f;
      float target2 = 240f;
      if (this.level.InSpace)
      {
        target1 *= 0.6f;
        target2 *= 0.6f;
      }
      if (this.Holding != null && this.Holding.SlowFall && (double) this.forceMoveXTimer <= 0.0)
        this.maxFall = Calc.Approach(this.maxFall, (double) (float) Input.GliderMoveY != 1.0 ? (!this.windMovedUp || (double) (float) Input.GliderMoveY != -1.0 ? ((double) (float) Input.GliderMoveY != -1.0 ? (!this.windMovedUp ? 40f : 0.0f) : 24f) : -32f) : 120f, 300f * Engine.DeltaTime);
      else if ((double) (float) Input.MoveY == 1.0 && (double) this.Speed.Y >= (double) target1)
      {
        this.maxFall = Calc.Approach(this.maxFall, target2, 300f * Engine.DeltaTime);
        float num = target1 + (float) (((double) target2 - (double) target1) * 0.5);
        if ((double) this.Speed.Y >= (double) num)
        {
          float amount = Math.Min(1f, (float) (((double) this.Speed.Y - (double) num) / ((double) target2 - (double) num)));
          this.Sprite.Scale.X = MathHelper.Lerp(1f, 0.5f, amount);
          this.Sprite.Scale.Y = MathHelper.Lerp(1f, 1.5f, amount);
        }
      }
      else
        this.maxFall = Calc.Approach(this.maxFall, target1, 300f * Engine.DeltaTime);
      if (!this.onGround)
      {
        float target3 = this.maxFall;
        if (this.Holding != null && this.Holding.SlowFall)
          this.holdCannotDuck = (double) (float) Input.MoveY == 1.0;
        if (((Facings) this.moveX == this.Facing || this.moveX == 0 && Input.GrabCheck) && Input.MoveY.Value != 1)
        {
          if ((double) this.Speed.Y >= 0.0 && (double) this.wallSlideTimer > 0.0 && this.Holding == null && this.ClimbBoundsCheck((int) this.Facing) && this.CollideCheck<Solid>(this.Position + Vector2.UnitX * (float) this.Facing) && !ClimbBlocker.EdgeCheck((Scene) this.level, (Entity) this, (int) this.Facing) && this.CanUnDuck)
          {
            this.Ducking = false;
            this.wallSlideDir = (int) this.Facing;
          }
          if (this.wallSlideDir != 0)
          {
            if (Input.GrabCheck)
              this.ClimbTrigger(this.wallSlideDir);
            if ((double) this.wallSlideTimer > 0.6000000238418579 && ClimbBlocker.Check((Scene) this.level, (Entity) this, this.Position + Vector2.UnitX * (float) this.wallSlideDir))
              this.wallSlideTimer = 0.6f;
            target3 = MathHelper.Lerp(160f, 20f, this.wallSlideTimer / 1.2f);
            if ((double) this.wallSlideTimer / 1.2000000476837158 > 0.6499999761581421)
              this.CreateWallSlideParticles(this.wallSlideDir);
          }
        }
        float num = (double) Math.Abs(this.Speed.Y) >= 40.0 || !Input.Jump.Check && !this.AutoJump ? 1f : 0.5f;
        if (this.Holding != null && this.Holding.SlowFall && (double) this.forceMoveXTimer <= 0.0)
          num *= 0.5f;
        if (this.level.InSpace)
          num *= 0.6f;
        this.Speed.Y = Calc.Approach(this.Speed.Y, target3, 900f * num * Engine.DeltaTime);
      }
      if ((double) this.varJumpTimer > 0.0)
      {
        if (this.AutoJump || Input.Jump.Check)
          this.Speed.Y = Math.Min(this.Speed.Y, this.varJumpSpeed);
        else
          this.varJumpTimer = 0.0f;
      }
      if (Input.Jump.Pressed && (TalkComponent.PlayerOver == null || !Input.Talk.Pressed))
      {
        if ((double) this.jumpGraceTimer > 0.0)
          this.Jump();
        else if (this.CanUnDuck)
        {
          bool canUnDuck = this.CanUnDuck;
          if (canUnDuck && this.WallJumpCheck(1))
          {
            if (this.Facing == Facings.Right && Input.GrabCheck && !SaveData.Instance.Assists.NoGrabbing && (double) this.Stamina > 0.0 && this.Holding == null && !ClimbBlocker.Check(this.Scene, (Entity) this, this.Position + Vector2.UnitX * 3f))
              this.ClimbJump();
            else if (this.DashAttacking && this.SuperWallJumpAngleCheck)
              this.SuperWallJump(-1);
            else
              this.WallJump(-1);
          }
          else if (canUnDuck && this.WallJumpCheck(-1))
          {
            if (this.Facing == Facings.Left && Input.GrabCheck && !SaveData.Instance.Assists.NoGrabbing && (double) this.Stamina > 0.0 && this.Holding == null && !ClimbBlocker.Check(this.Scene, (Entity) this, this.Position + Vector2.UnitX * -3f))
              this.ClimbJump();
            else if (this.DashAttacking && this.SuperWallJumpAngleCheck)
              this.SuperWallJump(1);
            else
              this.WallJump(1);
          }
          else
          {
            Water water;
            if ((water = this.CollideFirst<Water>(this.Position + Vector2.UnitY * 2f)) != null)
            {
              this.Jump();
              water.TopSurface.DoRipple(this.Position, 1f);
            }
          }
        }
      }
      return 0;
    }

    public void CreateWallSlideParticles(int dir)
    {
      if (!this.Scene.OnInterval(0.01f))
        return;
      int index = -1;
      Platform platformByPriority = SurfaceIndex.GetPlatformByPriority(this.CollideAll<Platform>(this.Position + Vector2.UnitX * (float) dir * 4f, this.temp));
      if (platformByPriority != null)
        index = platformByPriority.GetWallSoundIndex(this, dir);
      ParticleType particleType = this.DustParticleFromSurfaceIndex(index);
      float x = particleType == ParticleTypes.Dust ? 5f : 2f;
      Vector2 center = this.Center;
      Dust.Burst(dir != 1 ? center + new Vector2(-x, 4f) : center + new Vector2(x, 4f), -1.5707964f, particleType: particleType);
    }

    private bool IsTired => (double) this.CheckStamina < 20.0;

    private float CheckStamina => (double) this.wallBoostTimer > 0.0 ? this.Stamina + 27.5f : this.Stamina;

    private void PlaySweatEffectDangerOverride(string state)
    {
      if ((double) this.Stamina <= 20.0)
        this.sweatSprite.Play("danger");
      else
        this.sweatSprite.Play(state);
    }

    private void ClimbBegin()
    {
      this.AutoJump = false;
      this.Speed.X = 0.0f;
      this.Speed.Y *= 0.2f;
      this.wallSlideTimer = 1.2f;
      this.climbNoMoveTimer = 0.1f;
      this.wallBoostTimer = 0.0f;
      this.lastClimbMove = 0;
      Input.Rumble(RumbleStrength.Medium, RumbleLength.Short);
      for (int index = 0; index < 2 && !this.CollideCheck<Solid>(this.Position + Vector2.UnitX * (float) this.Facing); ++index)
        this.Position = this.Position + Vector2.UnitX * (float) this.Facing;
      Platform platformByPriority = SurfaceIndex.GetPlatformByPriority(this.CollideAll<Solid>(this.Position + Vector2.UnitX * (float) this.Facing, this.temp));
      if (platformByPriority == null)
        return;
      this.Play("event:/char/madeline/grab", "surface_index", (float) platformByPriority.GetWallSoundIndex(this, (int) this.Facing));
      if (!(platformByPriority is DreamBlock))
        return;
      (platformByPriority as DreamBlock).FootstepRipple(this.Position + new Vector2((float) ((int) this.Facing * 3), -4f));
    }

    private void ClimbEnd()
    {
      if ((HandleBase) this.conveyorLoopSfx != (HandleBase) null)
      {
        int num1 = (int) this.conveyorLoopSfx.setParameterValue("end", 1f);
        int num2 = (int) this.conveyorLoopSfx.release();
        this.conveyorLoopSfx = (EventInstance) null;
      }
      this.wallSpeedRetentionTimer = 0.0f;
      if (this.sweatSprite == null || !(this.sweatSprite.CurrentAnimationID != "jump"))
        return;
      this.sweatSprite.Play("idle");
    }

    private int ClimbUpdate()
    {
      this.climbNoMoveTimer -= Engine.DeltaTime;
      if (this.onGround)
        this.Stamina = 110f;
      if (Input.Jump.Pressed && (!this.Ducking || this.CanUnDuck))
      {
        if (this.moveX == -(int) this.Facing)
          this.WallJump(-(int) this.Facing);
        else
          this.ClimbJump();
        return 0;
      }
      if (this.CanDash)
      {
        this.Speed += this.LiftBoost;
        return this.StartDash();
      }
      if (!Input.GrabCheck)
      {
        this.Speed += this.LiftBoost;
        this.Play("event:/char/madeline/grab_letgo");
        return 0;
      }
      if (!this.CollideCheck<Solid>(this.Position + Vector2.UnitX * (float) this.Facing))
      {
        if ((double) this.Speed.Y < 0.0)
        {
          if (this.wallBoosting)
          {
            this.Speed += this.LiftBoost;
            this.Play("event:/char/madeline/grab_letgo");
          }
          else
            this.ClimbHop();
        }
        return 0;
      }
      WallBooster wallBooster = this.WallBoosterCheck();
      if ((double) this.climbNoMoveTimer <= 0.0 && wallBooster != null)
      {
        this.wallBoosting = true;
        if ((HandleBase) this.conveyorLoopSfx == (HandleBase) null)
          this.conveyorLoopSfx = Audio.Play("event:/game/09_core/conveyor_activate", this.Position, "end", 0.0f);
        Audio.Position(this.conveyorLoopSfx, this.Position);
        this.Speed.Y = Calc.Approach(this.Speed.Y, -160f, 600f * Engine.DeltaTime);
        this.LiftSpeed = Vector2.UnitY * Math.Max(this.Speed.Y, -80f);
        Input.Rumble(RumbleStrength.Light, RumbleLength.Short);
      }
      else
      {
        this.wallBoosting = false;
        if ((HandleBase) this.conveyorLoopSfx != (HandleBase) null)
        {
          int num1 = (int) this.conveyorLoopSfx.setParameterValue("end", 1f);
          int num2 = (int) this.conveyorLoopSfx.release();
          this.conveyorLoopSfx = (EventInstance) null;
        }
        float target = 0.0f;
        bool flag = false;
        if ((double) this.climbNoMoveTimer <= 0.0)
        {
          if (ClimbBlocker.Check(this.Scene, (Entity) this, this.Position + Vector2.UnitX * (float) this.Facing))
            flag = true;
          else if (Input.MoveY.Value == -1)
          {
            target = -45f;
            if (this.CollideCheck<Solid>(this.Position - Vector2.UnitY) || this.ClimbHopBlockedCheck() && this.SlipCheck(-1f))
            {
              if ((double) this.Speed.Y < 0.0)
                this.Speed.Y = 0.0f;
              target = 0.0f;
              flag = true;
            }
            else if (this.SlipCheck())
            {
              this.ClimbHop();
              return 0;
            }
          }
          else if (Input.MoveY.Value == 1)
          {
            target = 80f;
            if (this.onGround)
            {
              if ((double) this.Speed.Y > 0.0)
                this.Speed.Y = 0.0f;
              target = 0.0f;
            }
            else
              this.CreateWallSlideParticles((int) this.Facing);
          }
          else
            flag = true;
        }
        else
          flag = true;
        this.lastClimbMove = Math.Sign(target);
        if (flag && this.SlipCheck())
          target = 30f;
        this.Speed.Y = Calc.Approach(this.Speed.Y, target, 900f * Engine.DeltaTime);
      }
      if (Input.MoveY.Value != 1 && (double) this.Speed.Y > 0.0 && !this.CollideCheck<Solid>(this.Position + new Vector2((float) this.Facing, 1f)))
        this.Speed.Y = 0.0f;
      if ((double) this.climbNoMoveTimer <= 0.0)
      {
        if (this.lastClimbMove == -1)
        {
          this.Stamina -= 45.454544f * Engine.DeltaTime;
          if ((double) this.Stamina <= 20.0)
            this.sweatSprite.Play("danger");
          else if (this.sweatSprite.CurrentAnimationID != "climbLoop")
            this.sweatSprite.Play("climb");
          if (this.Scene.OnInterval(0.2f))
            Input.Rumble(RumbleStrength.Climb, RumbleLength.Short);
        }
        else
        {
          if (this.lastClimbMove == 0)
            this.Stamina -= 10f * Engine.DeltaTime;
          if (!this.onGround)
          {
            this.PlaySweatEffectDangerOverride("still");
            if (this.Scene.OnInterval(0.8f))
              Input.Rumble(RumbleStrength.Climb, RumbleLength.Short);
          }
          else
            this.PlaySweatEffectDangerOverride("idle");
        }
      }
      else
        this.PlaySweatEffectDangerOverride("idle");
      if ((double) this.Stamina > 0.0)
        return 1;
      this.Speed += this.LiftBoost;
      return 0;
    }

    private WallBooster WallBoosterCheck()
    {
      if (ClimbBlocker.Check(this.Scene, (Entity) this, this.Position + Vector2.UnitX * (float) this.Facing))
        return (WallBooster) null;
      foreach (WallBooster entity in this.Scene.Tracker.GetEntities<WallBooster>())
      {
        if (entity.Facing == this.Facing && this.CollideCheck((Entity) entity))
          return entity;
      }
      return (WallBooster) null;
    }

    private void ClimbHop()
    {
      this.climbHopSolid = this.CollideFirst<Solid>(this.Position + Vector2.UnitX * (float) this.Facing);
      this.playFootstepOnLand = 0.5f;
      if (this.climbHopSolid != null)
      {
        this.climbHopSolidPosition = this.climbHopSolid.Position;
        this.hopWaitX = (int) this.Facing;
        this.hopWaitXSpeed = (float) this.Facing * 100f;
      }
      else
      {
        this.hopWaitX = 0;
        this.Speed.X = (float) this.Facing * 100f;
      }
      this.lowFrictionStopTimer = 0.15f;
      this.Speed.Y = Math.Min(this.Speed.Y, -120f);
      this.forceMoveX = 0;
      this.forceMoveXTimer = 0.2f;
      this.fastJump = false;
      this.noWindTimer = 0.3f;
      this.Play("event:/char/madeline/climb_ledge");
    }

    private bool SlipCheck(float addY = 0.0f)
    {
      Vector2 point = this.Facing != Facings.Right ? this.TopLeft - Vector2.UnitX + Vector2.UnitY * (4f + addY) : this.TopRight + Vector2.UnitY * (4f + addY);
      return !this.Scene.CollideCheck<Solid>(point) && !this.Scene.CollideCheck<Solid>(point + Vector2.UnitY * (addY - 4f));
    }

    private bool ClimbHopBlockedCheck()
    {
      foreach (Component follower in this.Leader.Followers)
      {
        if (follower.Entity is StrawberrySeed)
          return true;
      }
      foreach (LedgeBlocker component in this.Scene.Tracker.GetComponents<LedgeBlocker>())
      {
        if (component.HopBlockCheck(this))
          return true;
      }
      return this.CollideCheck<Solid>(this.Position - Vector2.UnitY * 6f);
    }

    private bool JumpThruBoostBlockedCheck()
    {
      foreach (LedgeBlocker component in this.Scene.Tracker.GetComponents<LedgeBlocker>())
      {
        if (component.JumpThruBoostCheck(this))
          return true;
      }
      return false;
    }

    private bool DashCorrectCheck(Vector2 add)
    {
      Vector2 position = this.Position;
      Collider collider = this.Collider;
      this.Position = this.Position + add;
      this.Collider = (Collider) this.hurtbox;
      foreach (LedgeBlocker component in this.Scene.Tracker.GetComponents<LedgeBlocker>())
      {
        if (component.DashCorrectCheck(this))
        {
          this.Position = position;
          this.Collider = collider;
          return true;
        }
      }
      this.Position = position;
      this.Collider = collider;
      return false;
    }

    public int StartDash()
    {
      this.wasDashB = this.Dashes == 2;
      this.Dashes = Math.Max(0, this.Dashes - 1);
      this.demoDashed = Input.CrouchDashPressed;
      Input.Dash.ConsumeBuffer();
      Input.CrouchDash.ConsumeBuffer();
      return 2;
    }

    public bool DashAttacking => (double) this.dashAttackTimer > 0.0 || this.StateMachine.State == 5;

    public bool CanDash
    {
      get
      {
        if (!Input.CrouchDashPressed && !Input.DashPressed || (double) this.dashCooldownTimer > 0.0 || this.Dashes <= 0 || TalkComponent.PlayerOver != null && Input.Talk.Pressed)
          return false;
        return this.LastBooster == null || !this.LastBooster.Ch9HubTransition || !this.LastBooster.BoostingPlayer;
      }
    }

    public bool StartedDashing { get; private set; }

    private void CallDashEvents()
    {
      if (this.calledDashEvents)
        return;
      this.calledDashEvents = true;
      if (this.CurrentBooster == null)
      {
        ++SaveData.Instance.TotalDashes;
        ++this.level.Session.Dashes;
        Stats.Increment(Stat.DASHES);
        bool flag = (double) this.DashDir.Y < 0.0 || (double) this.DashDir.Y == 0.0 && (double) this.DashDir.X > 0.0;
        if (this.DashDir == Vector2.Zero)
          flag = this.Facing == Facings.Right;
        if (flag)
        {
          if (this.wasDashB)
            this.Play("event:/char/madeline/dash_pink_right");
          else
            this.Play("event:/char/madeline/dash_red_right");
        }
        else if (this.wasDashB)
          this.Play("event:/char/madeline/dash_pink_left");
        else
          this.Play("event:/char/madeline/dash_red_left");
        if (this.SwimCheck())
          this.Play("event:/char/madeline/water_dash_gen");
        foreach (DashListener component in this.Scene.Tracker.GetComponents<DashListener>())
        {
          if (component.OnDash != null)
            component.OnDash(this.DashDir);
        }
      }
      else
      {
        this.CurrentBooster.PlayerBoosted(this, this.DashDir);
        this.CurrentBooster = (Booster) null;
      }
    }

    private void DashBegin()
    {
      this.calledDashEvents = false;
      this.dashStartedOnGround = this.onGround;
      this.launched = false;
      this.canCurveDash = true;
      if ((double) Engine.TimeRate > 0.25)
        Celeste.Freeze(0.05f);
      this.dashCooldownTimer = 0.2f;
      this.dashRefillCooldownTimer = 0.1f;
      this.StartedDashing = true;
      this.wallSlideTimer = 1.2f;
      this.dashTrailTimer = 0.0f;
      this.dashTrailCounter = 0;
      if (!SaveData.Instance.Assists.DashAssist)
        Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
      this.dashAttackTimer = 0.3f;
      this.gliderBoostTimer = 0.55f;
      if (SaveData.Instance.Assists.SuperDashing)
        this.dashAttackTimer += 0.15f;
      this.beforeDashSpeed = this.Speed;
      this.Speed = Vector2.Zero;
      this.DashDir = Vector2.Zero;
      if (!this.onGround && this.Ducking && this.CanUnDuck)
        this.Ducking = false;
      else if (!this.Ducking && (this.demoDashed || Input.MoveY.Value == 1))
        this.Ducking = true;
      this.DashAssistInit();
    }

    private void DashAssistInit()
    {
      if (!SaveData.Instance.Assists.DashAssist || this.demoDashed)
        return;
      Input.LastAim = Vector2.UnitX * (float) this.Facing;
      Engine.DashAssistFreeze = true;
      Engine.DashAssistFreezePress = false;
      PlayerDashAssist playerDashAssist = this.Scene.Tracker.GetEntity<PlayerDashAssist>();
      if (playerDashAssist == null)
        this.Scene.Add((Entity) (playerDashAssist = new PlayerDashAssist()));
      playerDashAssist.Direction = Input.GetAimVector(this.Facing).Angle();
      playerDashAssist.Scale = 0.0f;
      playerDashAssist.Offset = this.CurrentBooster != null || this.StateMachine.PreviousState == 5 ? new Vector2(0.0f, -4f) : Vector2.Zero;
    }

    private void DashEnd()
    {
      this.CallDashEvents();
      this.demoDashed = false;
    }

    private int DashUpdate()
    {
      this.StartedDashing = false;
      if ((double) this.dashTrailTimer > 0.0)
      {
        this.dashTrailTimer -= Engine.DeltaTime;
        if ((double) this.dashTrailTimer <= 0.0)
        {
          this.CreateTrail();
          --this.dashTrailCounter;
          if (this.dashTrailCounter > 0)
            this.dashTrailTimer = 0.1f;
        }
      }
      if (SaveData.Instance.Assists.SuperDashing && this.canCurveDash && Input.Aim.Value != Vector2.Zero && this.Speed != Vector2.Zero)
      {
        Vector2 vector = this.CorrectDashPrecision(Input.GetAimVector());
        float num = Vector2.Dot(vector, this.Speed.SafeNormalize());
        if ((double) num >= -0.10000000149011612 && (double) num < 0.9900000095367432)
        {
          this.Speed = this.Speed.RotateTowards(vector.Angle(), 4.1887903f * Engine.DeltaTime);
          this.DashDir = this.Speed.SafeNormalize();
          this.DashDir = this.CorrectDashPrecision(this.DashDir);
        }
      }
      if (SaveData.Instance.Assists.SuperDashing && this.CanDash)
      {
        this.StartDash();
        this.StateMachine.ForceState(2);
        return 2;
      }
      if (this.Holding == null && this.DashDir != Vector2.Zero && Input.GrabCheck && !this.IsTired && this.CanUnDuck)
      {
        foreach (Holdable component in this.Scene.Tracker.GetComponents<Holdable>())
        {
          if (component.Check(this) && this.Pickup(component))
            return 8;
        }
      }
      if ((double) Math.Abs(this.DashDir.Y) < 0.10000000149011612)
      {
        foreach (JumpThru entity in this.Scene.Tracker.GetEntities<JumpThru>())
        {
          if (this.CollideCheck((Entity) entity) && (double) this.Bottom - (double) entity.Top <= 6.0 && !this.DashCorrectCheck(Vector2.UnitY * (entity.Top - this.Bottom)))
            this.MoveVExact((int) ((double) entity.Top - (double) this.Bottom));
        }
        if (this.CanUnDuck && Input.Jump.Pressed && (double) this.jumpGraceTimer > 0.0)
        {
          this.SuperJump();
          return 0;
        }
      }
      if (this.SuperWallJumpAngleCheck)
      {
        if (Input.Jump.Pressed && this.CanUnDuck)
        {
          if (this.WallJumpCheck(1))
          {
            this.SuperWallJump(-1);
            return 0;
          }
          if (this.WallJumpCheck(-1))
          {
            this.SuperWallJump(1);
            return 0;
          }
        }
      }
      else if (Input.Jump.Pressed && this.CanUnDuck)
      {
        if (this.WallJumpCheck(1))
        {
          if (this.Facing == Facings.Right && Input.GrabCheck && (double) this.Stamina > 0.0 && this.Holding == null && !ClimbBlocker.Check(this.Scene, (Entity) this, this.Position + Vector2.UnitX * 3f))
            this.ClimbJump();
          else
            this.WallJump(-1);
          return 0;
        }
        if (this.WallJumpCheck(-1))
        {
          if (this.Facing == Facings.Left && Input.GrabCheck && (double) this.Stamina > 0.0 && this.Holding == null && !ClimbBlocker.Check(this.Scene, (Entity) this, this.Position + Vector2.UnitX * -3f))
            this.ClimbJump();
          else
            this.WallJump(1);
          return 0;
        }
      }
      if (this.Speed != Vector2.Zero && this.level.OnInterval(0.02f))
        this.level.ParticlesFG.Emit(this.wasDashB ? (this.Sprite.Mode != PlayerSpriteMode.MadelineAsBadeline ? Player.P_DashB : Player.P_DashBadB) : Player.P_DashA, this.Center + Calc.Random.Range(Vector2.One * -2f, Vector2.One * 2f), this.DashDir.Angle());
      return 2;
    }

    private bool SuperWallJumpAngleCheck => (double) Math.Abs(this.DashDir.X) <= 0.20000000298023224 && (double) this.DashDir.Y <= -0.75;

    private Vector2 CorrectDashPrecision(Vector2 dir)
    {
      if ((double) dir.X != 0.0 && (double) Math.Abs(dir.X) < 1.0 / 1000.0)
      {
        dir.X = 0.0f;
        dir.Y = (float) Math.Sign(dir.Y);
      }
      else if ((double) dir.Y != 0.0 && (double) Math.Abs(dir.Y) < 1.0 / 1000.0)
      {
        dir.Y = 0.0f;
        dir.X = (float) Math.Sign(dir.X);
      }
      return dir;
    }

    private IEnumerator DashCoroutine()
    {
      Player player = this;
      yield return (object) null;
      if (SaveData.Instance.Assists.DashAssist)
        Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
      player.level.Displacement.AddBurst(player.Center, 0.4f, 8f, 64f, 0.5f, Ease.QuadOut, Ease.QuadOut);
      Vector2 lastAim = player.lastAim;
      if (player.OverrideDashDirection.HasValue)
        lastAim = player.OverrideDashDirection.Value;
      Vector2 vector2_1 = player.CorrectDashPrecision(lastAim);
      Vector2 vector2_2 = vector2_1 * 240f;
      if (Math.Sign(player.beforeDashSpeed.X) == Math.Sign(vector2_2.X) && (double) Math.Abs(player.beforeDashSpeed.X) > (double) Math.Abs(vector2_2.X))
        vector2_2.X = player.beforeDashSpeed.X;
      player.Speed = vector2_2;
      if (player.CollideCheck<Water>())
        player.Speed *= 0.75f;
      player.gliderBoostDir = player.DashDir = vector2_1;
      player.SceneAs<Level>().DirectionalShake(player.DashDir, 0.2f);
      if ((double) player.DashDir.X != 0.0)
        player.Facing = (Facings) Math.Sign(player.DashDir.X);
      player.CallDashEvents();
      if (player.StateMachine.PreviousState == 19)
        player.level.Particles.Emit(FlyFeather.P_Boost, 12, player.Center, Vector2.One * 4f, (-vector2_1).Angle());
      if (player.onGround && (double) player.DashDir.X != 0.0 && (double) player.DashDir.Y > 0.0 && (double) player.Speed.Y > 0.0 && (!player.Inventory.DreamDash || !player.CollideCheck<DreamBlock>(player.Position + Vector2.UnitY)))
      {
        player.DashDir.X = (float) Math.Sign(player.DashDir.X);
        player.DashDir.Y = 0.0f;
        player.Speed.Y = 0.0f;
        player.Speed.X *= 1.2f;
        player.Ducking = true;
      }
      SlashFx.Burst(player.Center, player.DashDir.Angle());
      player.CreateTrail();
      if (SaveData.Instance.Assists.SuperDashing)
      {
        player.dashTrailTimer = 0.1f;
        player.dashTrailCounter = 2;
      }
      else
      {
        player.dashTrailTimer = 0.08f;
        player.dashTrailCounter = 1;
      }
      if ((double) player.DashDir.X != 0.0 && Input.GrabCheck)
      {
        SwapBlock swapBlock = player.CollideFirst<SwapBlock>(player.Position + Vector2.UnitX * (float) Math.Sign(player.DashDir.X));
        if (swapBlock != null && (double) swapBlock.Direction.X == (double) Math.Sign(player.DashDir.X))
        {
          player.StateMachine.State = 1;
          player.Speed = Vector2.Zero;
          yield break;
        }
      }
      Vector2 swapCancel = Vector2.One;
      foreach (SwapBlock entity in player.Scene.Tracker.GetEntities<SwapBlock>())
      {
        if (player.CollideCheck((Entity) entity, player.Position + Vector2.UnitY) && entity != null && entity.Swapping)
        {
          if ((double) player.DashDir.X != 0.0 && (double) entity.Direction.X == (double) Math.Sign(player.DashDir.X))
            player.Speed.X = swapCancel.X = 0.0f;
          if ((double) player.DashDir.Y != 0.0 && (double) entity.Direction.Y == (double) Math.Sign(player.DashDir.Y))
            player.Speed.Y = swapCancel.Y = 0.0f;
        }
      }
      if (SaveData.Instance.Assists.SuperDashing)
        yield return (object) 0.3f;
      else
        yield return (object) 0.15f;
      player.CreateTrail();
      player.AutoJump = true;
      player.AutoJumpTimer = 0.0f;
      if ((double) player.DashDir.Y <= 0.0)
      {
        player.Speed = player.DashDir * 160f;
        player.Speed.X *= swapCancel.X;
        player.Speed.Y *= swapCancel.Y;
      }
      if ((double) player.Speed.Y < 0.0)
        player.Speed.Y *= 0.75f;
      player.StateMachine.State = 0;
    }

    private bool SwimCheck() => this.CollideCheck<Water>(this.Position + Vector2.UnitY * -8f) && this.CollideCheck<Water>(this.Position);

    private bool SwimUnderwaterCheck() => this.CollideCheck<Water>(this.Position + Vector2.UnitY * -9f);

    private bool SwimJumpCheck() => !this.CollideCheck<Water>(this.Position + Vector2.UnitY * -14f);

    private bool SwimRiseCheck() => !this.CollideCheck<Water>(this.Position + Vector2.UnitY * -18f);

    private bool UnderwaterMusicCheck() => this.CollideCheck<Water>(this.Position) && this.CollideCheck<Water>(this.Position + Vector2.UnitY * -12f);

    private void SwimBegin()
    {
      if ((double) this.Speed.Y > 0.0)
        this.Speed.Y *= 0.5f;
      this.Stamina = 110f;
    }

    private int SwimUpdate()
    {
      if (!this.SwimCheck())
        return 0;
      if (this.CanUnDuck)
        this.Ducking = false;
      if (this.CanDash)
      {
        this.demoDashed = Input.CrouchDashPressed;
        Input.Dash.ConsumeBuffer();
        Input.CrouchDash.ConsumeBuffer();
        return 2;
      }
      bool flag = this.SwimUnderwaterCheck();
      if (!flag && (double) this.Speed.Y >= 0.0 && Input.GrabCheck && !this.IsTired && this.CanUnDuck && Math.Sign(this.Speed.X) != -(int) this.Facing && this.ClimbCheck((int) this.Facing))
      {
        if (SaveData.Instance.Assists.NoGrabbing)
          this.ClimbTrigger((int) this.Facing);
        else if (!this.MoveVExact(-1))
        {
          this.Ducking = false;
          return 1;
        }
      }
      Vector2 vector2 = Input.Feather.Value.SafeNormalize();
      float num1 = flag ? 60f : 80f;
      float num2 = 80f;
      this.Speed.X = (double) Math.Abs(this.Speed.X) <= 80.0 || Math.Sign(this.Speed.X) != Math.Sign(vector2.X) ? Calc.Approach(this.Speed.X, num1 * vector2.X, 600f * Engine.DeltaTime) : Calc.Approach(this.Speed.X, num1 * vector2.X, 400f * Engine.DeltaTime);
      if ((double) vector2.Y == 0.0 && this.SwimRiseCheck())
        this.Speed.Y = Calc.Approach(this.Speed.Y, -60f, 600f * Engine.DeltaTime);
      else if ((double) vector2.Y >= 0.0 || this.SwimUnderwaterCheck())
        this.Speed.Y = (double) Math.Abs(this.Speed.Y) <= 80.0 || Math.Sign(this.Speed.Y) != Math.Sign(vector2.Y) ? Calc.Approach(this.Speed.Y, num2 * vector2.Y, 600f * Engine.DeltaTime) : Calc.Approach(this.Speed.Y, num2 * vector2.Y, 400f * Engine.DeltaTime);
      if (!flag && this.moveX != 0 && this.CollideCheck<Solid>(this.Position + Vector2.UnitX * (float) this.moveX) && !this.CollideCheck<Solid>(this.Position + new Vector2((float) this.moveX, -3f)))
        this.ClimbHop();
      if (!Input.Jump.Pressed || !this.SwimJumpCheck())
        return 3;
      this.Jump();
      return 0;
    }

    public void Boost(Booster booster)
    {
      this.StateMachine.State = 4;
      this.Speed = Vector2.Zero;
      this.boostTarget = booster.Center;
      this.boostRed = false;
      this.LastBooster = this.CurrentBooster = booster;
    }

    public void RedBoost(Booster booster)
    {
      this.StateMachine.State = 4;
      this.Speed = Vector2.Zero;
      this.boostTarget = booster.Center;
      this.boostRed = true;
      this.LastBooster = this.CurrentBooster = booster;
    }

    private void BoostBegin()
    {
      this.RefillDash();
      this.RefillStamina();
      if (this.Holding == null)
        return;
      this.Drop();
    }

    private void BoostEnd()
    {
      Vector2 vector2 = (this.boostTarget - this.Collider.Center).Floor();
      this.MoveToX(vector2.X);
      this.MoveToY(vector2.Y);
    }

    private int BoostUpdate()
    {
      Vector2 vector2 = Calc.Approach(this.ExactPosition, this.boostTarget - this.Collider.Center + Input.Aim.Value * 3f, 80f * Engine.DeltaTime);
      this.MoveToX(vector2.X);
      this.MoveToY(vector2.Y);
      if (!Input.DashPressed && !Input.CrouchDashPressed)
        return 4;
      this.demoDashed = Input.CrouchDashPressed;
      Input.Dash.ConsumePress();
      Input.CrouchDash.ConsumeBuffer();
      return this.boostRed ? 5 : 2;
    }

    private IEnumerator BoostCoroutine()
    {
      yield return (object) 0.25f;
      this.StateMachine.State = !this.boostRed ? 2 : 5;
    }

    private void RedDashBegin()
    {
      this.calledDashEvents = false;
      this.dashStartedOnGround = false;
      Celeste.Freeze(0.05f);
      Dust.Burst(this.Position, (-this.DashDir).Angle(), 8);
      this.dashCooldownTimer = 0.2f;
      this.dashRefillCooldownTimer = 0.1f;
      this.StartedDashing = true;
      this.level.Displacement.AddBurst(this.Center, 0.5f, 0.0f, 80f, 0.666f, Ease.QuadOut, Ease.QuadOut);
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
      this.dashAttackTimer = 0.3f;
      this.gliderBoostTimer = 0.55f;
      this.DashDir = this.Speed = Vector2.Zero;
      if (!this.onGround && this.CanUnDuck)
        this.Ducking = false;
      this.DashAssistInit();
    }

    private void RedDashEnd() => this.CallDashEvents();

    private int RedDashUpdate()
    {
      this.StartedDashing = false;
      bool flag = this.LastBooster != null && this.LastBooster.Ch9HubTransition;
      this.gliderBoostTimer = 0.05f;
      if (this.CanDash)
        return this.StartDash();
      if ((double) this.DashDir.Y == 0.0)
      {
        foreach (JumpThru entity in this.Scene.Tracker.GetEntities<JumpThru>())
        {
          if (this.CollideCheck((Entity) entity) && (double) this.Bottom - (double) entity.Top <= 6.0)
            this.MoveVExact((int) ((double) entity.Top - (double) this.Bottom));
        }
        if (this.CanUnDuck && Input.Jump.Pressed && (double) this.jumpGraceTimer > 0.0 && !flag)
        {
          this.SuperJump();
          return 0;
        }
      }
      if (!flag)
      {
        if (this.SuperWallJumpAngleCheck)
        {
          if (Input.Jump.Pressed && this.CanUnDuck)
          {
            if (this.WallJumpCheck(1))
            {
              this.SuperWallJump(-1);
              return 0;
            }
            if (this.WallJumpCheck(-1))
            {
              this.SuperWallJump(1);
              return 0;
            }
          }
        }
        else if (Input.Jump.Pressed && this.CanUnDuck)
        {
          if (this.WallJumpCheck(1))
          {
            if (this.Facing == Facings.Right && Input.GrabCheck && (double) this.Stamina > 0.0 && this.Holding == null && !ClimbBlocker.Check(this.Scene, (Entity) this, this.Position + Vector2.UnitX * 3f))
              this.ClimbJump();
            else
              this.WallJump(-1);
            return 0;
          }
          if (this.WallJumpCheck(-1))
          {
            if (this.Facing == Facings.Left && Input.GrabCheck && (double) this.Stamina > 0.0 && this.Holding == null && !ClimbBlocker.Check(this.Scene, (Entity) this, this.Position + Vector2.UnitX * -3f))
              this.ClimbJump();
            else
              this.WallJump(1);
            return 0;
          }
        }
      }
      return 5;
    }

    private IEnumerator RedDashCoroutine()
    {
      Player player = this;
      yield return (object) null;
      player.Speed = player.CorrectDashPrecision(player.lastAim) * 240f;
      player.gliderBoostDir = player.DashDir = player.lastAim;
      player.SceneAs<Level>().DirectionalShake(player.DashDir, 0.2f);
      if ((double) player.DashDir.X != 0.0)
        player.Facing = (Facings) Math.Sign(player.DashDir.X);
      player.CallDashEvents();
    }

    private void HitSquashBegin() => this.hitSquashNoMoveTimer = 0.1f;

    private int HitSquashUpdate()
    {
      this.Speed.X = Calc.Approach(this.Speed.X, 0.0f, 800f * Engine.DeltaTime);
      this.Speed.Y = Calc.Approach(this.Speed.Y, 0.0f, 800f * Engine.DeltaTime);
      if (Input.Jump.Pressed)
      {
        if (this.onGround)
          this.Jump();
        else if (this.WallJumpCheck(1))
        {
          if (this.Facing == Facings.Right && Input.GrabCheck && (double) this.Stamina > 0.0 && this.Holding == null && !ClimbBlocker.Check(this.Scene, (Entity) this, this.Position + Vector2.UnitX * 3f))
            this.ClimbJump();
          else
            this.WallJump(-1);
        }
        else if (this.WallJumpCheck(-1))
        {
          if (this.Facing == Facings.Left && Input.GrabCheck && (double) this.Stamina > 0.0 && this.Holding == null && !ClimbBlocker.Check(this.Scene, (Entity) this, this.Position + Vector2.UnitX * -3f))
            this.ClimbJump();
          else
            this.WallJump(1);
        }
        else
          Input.Jump.ConsumeBuffer();
        return 0;
      }
      if (this.CanDash)
        return this.StartDash();
      if (Input.GrabCheck && this.ClimbCheck((int) this.Facing))
        return 1;
      if ((double) this.hitSquashNoMoveTimer <= 0.0)
        return 0;
      this.hitSquashNoMoveTimer -= Engine.DeltaTime;
      return 6;
    }

    public Vector2 ExplodeLaunch(Vector2 from, bool snapUp = true, bool sidesOnly = false)
    {
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
      Celeste.Freeze(0.1f);
      this.launchApproachX = new float?();
      Vector2 vector2 = (this.Center - from).SafeNormalize(-Vector2.UnitY);
      float num = Vector2.Dot(vector2, Vector2.UnitY);
      if (snapUp && (double) num <= -0.699999988079071)
      {
        vector2.X = 0.0f;
        vector2.Y = -1f;
      }
      else if ((double) num <= 0.6499999761581421 && (double) num >= -0.550000011920929)
      {
        vector2.Y = 0.0f;
        vector2.X = (float) Math.Sign(vector2.X);
      }
      if (sidesOnly && (double) vector2.X != 0.0)
      {
        vector2.Y = 0.0f;
        vector2.X = (float) Math.Sign(vector2.X);
      }
      this.Speed = 280f * vector2;
      if ((double) this.Speed.Y <= 50.0)
      {
        this.Speed.Y = Math.Min(-150f, this.Speed.Y);
        this.AutoJump = true;
      }
      if ((double) this.Speed.X != 0.0)
      {
        if (Input.MoveX.Value == Math.Sign(this.Speed.X))
        {
          this.explodeLaunchBoostTimer = 0.0f;
          this.Speed.X *= 1.2f;
        }
        else
        {
          this.explodeLaunchBoostTimer = 0.01f;
          this.explodeLaunchBoostSpeed = this.Speed.X * 1.2f;
        }
      }
      SlashFx.Burst(this.Center, this.Speed.Angle());
      if (!this.Inventory.NoRefills)
        this.RefillDash();
      this.RefillStamina();
      this.dashCooldownTimer = 0.2f;
      this.StateMachine.State = 7;
      return vector2;
    }

    public void FinalBossPushLaunch(int dir)
    {
      this.launchApproachX = new float?();
      this.Speed.X = (float) (0.8999999761581421 * (double) dir * 280.0);
      this.Speed.Y = -150f;
      this.AutoJump = true;
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
      SlashFx.Burst(this.Center, this.Speed.Angle());
      this.RefillDash();
      this.RefillStamina();
      this.dashCooldownTimer = 0.28f;
      this.StateMachine.State = 7;
    }

    public void BadelineBoostLaunch(float atX)
    {
      this.launchApproachX = new float?(atX);
      this.Speed.X = 0.0f;
      this.Speed.Y = -330f;
      this.AutoJump = true;
      if (this.Holding != null)
        this.Drop();
      SlashFx.Burst(this.Center, this.Speed.Angle());
      this.RefillDash();
      this.RefillStamina();
      this.dashCooldownTimer = 0.2f;
      this.StateMachine.State = 7;
    }

    private void LaunchBegin() => this.launched = true;

    private int LaunchUpdate()
    {
      if (this.launchApproachX.HasValue)
        this.MoveTowardsX(this.launchApproachX.Value, 60f * Engine.DeltaTime);
      if (this.CanDash)
        return this.StartDash();
      if (Input.GrabCheck && !this.IsTired && !this.Ducking)
      {
        foreach (Holdable component in this.Scene.Tracker.GetComponents<Holdable>())
        {
          if (component.Check(this) && this.Pickup(component))
            return 8;
        }
      }
      this.Speed.Y = (double) this.Speed.Y >= 0.0 ? Calc.Approach(this.Speed.Y, 160f, 225f * Engine.DeltaTime) : Calc.Approach(this.Speed.Y, 160f, 450f * Engine.DeltaTime);
      this.Speed.X = Calc.Approach(this.Speed.X, 0.0f, 200f * Engine.DeltaTime);
      return (double) this.Speed.Length() < 220.0 ? 0 : 7;
    }

    public void SummitLaunch(float targetX)
    {
      this.summitLaunchTargetX = targetX;
      this.StateMachine.State = 10;
    }

    private void SummitLaunchBegin()
    {
      this.wallBoostTimer = 0.0f;
      this.Sprite.Play("launch");
      this.Speed = -Vector2.UnitY * 240f;
      this.summitLaunchParticleTimer = 0.4f;
    }

    private int SummitLaunchUpdate()
    {
      this.summitLaunchParticleTimer -= Engine.DeltaTime;
      if ((double) this.summitLaunchParticleTimer > 0.0 && this.Scene.OnInterval(0.03f))
        this.level.ParticlesFG.Emit(BadelineBoost.P_Move, 1, this.Center, Vector2.One * 4f);
      this.Facing = Facings.Right;
      this.MoveTowardsX(this.summitLaunchTargetX, 20f * Engine.DeltaTime);
      this.Speed = -Vector2.UnitY * 240f;
      if (this.level.OnInterval(0.2f))
        this.level.Add((Entity) Engine.Pooler.Create<SpeedRing>().Init(this.Center, 1.5707964f, Color.White));
      CrystalStaticSpinner crystalStaticSpinner = this.Scene.CollideFirst<CrystalStaticSpinner>(new Rectangle((int) ((double) this.X - 4.0), (int) ((double) this.Y - 40.0), 8, 12));
      if (crystalStaticSpinner != null)
      {
        crystalStaticSpinner.Destroy();
        this.level.Shake();
        Input.Rumble(RumbleStrength.Medium, RumbleLength.Short);
        Celeste.Freeze(0.01f);
      }
      return 10;
    }

    public void StopSummitLaunch()
    {
      this.StateMachine.State = 0;
      this.Speed.Y = -140f;
      this.AutoJump = true;
      this.varJumpSpeed = this.Speed.Y;
    }

    private IEnumerator PickupCoroutine()
    {
      Player player = this;
      player.Play("event:/char/madeline/crystaltheo_lift");
      Input.Rumble(RumbleStrength.Medium, RumbleLength.Short);
      if (player.Holding != null && player.Holding.SlowFall && ((double) player.gliderBoostTimer - 0.1599999964237213 > 0.0 && (double) player.gliderBoostDir.Y < 0.0 || (double) player.Speed.Length() > 180.0 && (double) player.Speed.Y <= 0.0))
        Audio.Play("event:/new_content/game/10_farewell/glider_platform_dissipate", player.Position);
      Vector2 oldSpeed = player.Speed;
      float varJump = player.varJumpTimer;
      player.Speed = Vector2.Zero;
      Vector2 begin = player.Holding.Entity.Position - player.Position;
      Vector2 carryOffsetTarget = Player.CarryOffsetTarget;
      Vector2 control = new Vector2(begin.X + (float) (Math.Sign(begin.X) * 2), Player.CarryOffsetTarget.Y - 2f);
      SimpleCurve curve = new SimpleCurve(begin, carryOffsetTarget, control);
      player.carryOffset = begin;
      Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeInOut, 0.16f, true);
      tween.OnUpdate = (Action<Tween>) (t => this.carryOffset = curve.GetPoint(t.Eased));
      player.Add((Component) tween);
      yield return (object) tween.Wait();
      player.Speed = oldSpeed;
      player.Speed.Y = Math.Min(player.Speed.Y, 0.0f);
      player.varJumpTimer = varJump;
      player.StateMachine.State = 0;
      if (player.Holding != null && player.Holding.SlowFall)
      {
        if ((double) player.gliderBoostTimer > 0.0 && (double) player.gliderBoostDir.Y < 0.0)
        {
          Input.Rumble(RumbleStrength.Medium, RumbleLength.Short);
          player.gliderBoostTimer = 0.0f;
          player.Speed.Y = Math.Min(player.Speed.Y, -240f * Math.Abs(player.gliderBoostDir.Y));
        }
        else if ((double) player.Speed.Y < 0.0)
          player.Speed.Y = Math.Min(player.Speed.Y, -105f);
        if (player.onGround && (double) (float) Input.MoveY == 1.0)
          player.holdCannotDuck = true;
      }
    }

    private void DreamDashBegin()
    {
      if (this.dreamSfxLoop == null)
        this.Add((Component) (this.dreamSfxLoop = new SoundSource()));
      this.Speed = this.DashDir * 240f;
      this.TreatNaive = true;
      this.Depth = -12000;
      this.dreamDashCanEndTimer = 0.1f;
      this.Stamina = 110f;
      this.dreamJump = false;
      this.Play("event:/char/madeline/dreamblock_enter");
      this.Loop(this.dreamSfxLoop, "event:/char/madeline/dreamblock_travel");
    }

    private void DreamDashEnd()
    {
      this.Depth = 0;
      if (!this.dreamJump)
      {
        this.AutoJump = true;
        this.AutoJumpTimer = 0.0f;
      }
      if (!this.Inventory.NoRefills)
        this.RefillDash();
      this.RefillStamina();
      this.TreatNaive = false;
      if (this.dreamBlock != null)
      {
        if ((double) this.DashDir.X != 0.0)
        {
          this.jumpGraceTimer = 0.1f;
          this.dreamJump = true;
        }
        else
          this.jumpGraceTimer = 0.0f;
        this.dreamBlock.OnPlayerExit(this);
        this.dreamBlock = (DreamBlock) null;
      }
      this.Stop(this.dreamSfxLoop);
      this.Play("event:/char/madeline/dreamblock_exit");
      Input.Rumble(RumbleStrength.Medium, RumbleLength.Short);
    }

    private int DreamDashUpdate()
    {
      Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
      Vector2 position = this.Position;
      this.NaiveMove(this.Speed * Engine.DeltaTime);
      if ((double) this.dreamDashCanEndTimer > 0.0)
        this.dreamDashCanEndTimer -= Engine.DeltaTime;
      DreamBlock dreamBlock = this.CollideFirst<DreamBlock>();
      if (dreamBlock == null)
      {
        if (this.DreamDashedIntoSolid())
        {
          if (SaveData.Instance.Assists.Invincible)
          {
            this.Position = position;
            this.Speed *= -1f;
            this.Play("event:/game/general/assist_dreamblockbounce");
          }
          else
            this.Die(Vector2.Zero);
        }
        else if ((double) this.dreamDashCanEndTimer <= 0.0)
        {
          Celeste.Freeze(0.05f);
          if (Input.Jump.Pressed && (double) this.DashDir.X != 0.0)
          {
            this.dreamJump = true;
            this.Jump();
          }
          else if ((double) this.DashDir.Y >= 0.0 || (double) this.DashDir.X != 0.0)
          {
            if ((double) this.DashDir.X > 0.0 && this.CollideCheck<Solid>(this.Position - Vector2.UnitX * 5f))
              this.MoveHExact(-5);
            else if ((double) this.DashDir.X < 0.0 && this.CollideCheck<Solid>(this.Position + Vector2.UnitX * 5f))
              this.MoveHExact(5);
            bool flag1 = this.ClimbCheck(-1);
            bool flag2 = this.ClimbCheck(1);
            if (Input.GrabCheck && (this.moveX == 1 & flag2 || this.moveX == -1 & flag1))
            {
              this.Facing = (Facings) this.moveX;
              if (!SaveData.Instance.Assists.NoGrabbing)
                return 1;
              this.ClimbTrigger(this.moveX);
              this.Speed.X = 0.0f;
            }
          }
          return 0;
        }
      }
      else
      {
        this.dreamBlock = dreamBlock;
        if (this.Scene.OnInterval(0.1f))
          this.CreateTrail();
        if (this.level.OnInterval(0.04f))
        {
          DisplacementRenderer.Burst burst = this.level.Displacement.AddBurst(this.Center, 0.3f, 0.0f, 40f);
          burst.WorldClipCollider = this.dreamBlock.Collider;
          burst.WorldClipPadding = 2;
        }
      }
      return 9;
    }

    private bool DreamDashedIntoSolid()
    {
      if (!this.CollideCheck<Solid>())
        return false;
      for (int index1 = 1; index1 <= 5; ++index1)
      {
        for (int index2 = -1; index2 <= 1; index2 += 2)
        {
          for (int index3 = 1; index3 <= 5; ++index3)
          {
            for (int index4 = -1; index4 <= 1; index4 += 2)
            {
              Vector2 vector2 = new Vector2((float) (index1 * index2), (float) (index3 * index4));
              if (!this.CollideCheck<Solid>(this.Position + vector2))
              {
                this.Position = this.Position + vector2;
                return false;
              }
            }
          }
        }
      }
      return true;
    }

    public bool StartStarFly()
    {
      this.RefillStamina();
      if (this.StateMachine.State == 18)
        return false;
      if (this.StateMachine.State == 19)
      {
        this.starFlyTimer = 2f;
        this.Sprite.Color = this.starFlyColor;
        Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
      }
      else
        this.StateMachine.State = 19;
      return true;
    }

    private void StarFlyBegin()
    {
      this.Sprite.Play("startStarFly");
      this.starFlyTransforming = true;
      this.starFlyTimer = 2f;
      this.starFlySpeedLerp = 0.0f;
      this.jumpGraceTimer = 0.0f;
      if (this.starFlyBloom == null)
        this.Add((Component) (this.starFlyBloom = new BloomPoint(new Vector2(0.0f, -6f), 0.0f, 16f)));
      this.starFlyBloom.Visible = true;
      this.starFlyBloom.Alpha = 0.0f;
      this.Collider = (Collider) this.starFlyHitbox;
      this.hurtbox = this.starFlyHurtbox;
      if (this.starFlyLoopSfx == null)
      {
        this.Add((Component) (this.starFlyLoopSfx = new SoundSource()));
        this.starFlyLoopSfx.DisposeOnTransition = false;
        this.Add((Component) (this.starFlyWarningSfx = new SoundSource()));
        this.starFlyWarningSfx.DisposeOnTransition = false;
      }
      this.starFlyLoopSfx.Play("event:/game/06_reflection/feather_state_loop", "feather_speed", 1f);
      this.starFlyWarningSfx.Stop();
    }

    private void StarFlyEnd()
    {
      this.Play("event:/game/06_reflection/feather_state_end");
      this.starFlyWarningSfx.Stop();
      this.starFlyLoopSfx.Stop();
      this.Hair.DrawPlayerSpriteOutline = false;
      this.Sprite.Color = Color.White;
      this.level.Displacement.AddBurst(this.Center, 0.25f, 8f, 32f);
      this.starFlyBloom.Visible = false;
      this.Sprite.HairCount = this.startHairCount;
      this.StarFlyReturnToNormalHitbox();
      if (this.StateMachine.State == 2)
        return;
      this.level.Particles.Emit(FlyFeather.P_Boost, 12, this.Center, Vector2.One * 4f, (-this.Speed).Angle());
    }

    private void StarFlyReturnToNormalHitbox()
    {
      this.Collider = (Collider) this.normalHitbox;
      this.hurtbox = this.normalHurtbox;
      if (!this.CollideCheck<Solid>())
        return;
      Vector2 position = this.Position;
      this.Y -= this.normalHitbox.Bottom - this.starFlyHitbox.Bottom;
      if (!this.CollideCheck<Solid>())
        return;
      this.Position = position;
      this.Ducking = true;
      this.Y -= this.duckHitbox.Bottom - this.starFlyHitbox.Bottom;
      if (this.CollideCheck<Solid>())
      {
        this.Position = position;
        throw new Exception("Could not get out of solids when exiting Star Fly State!");
      }
    }

    private IEnumerator StarFlyCoroutine()
    {
      Player player = this;
      while (player.Sprite.CurrentAnimationID == "startStarFly")
        yield return (object) null;
      while (player.Speed != Vector2.Zero)
        yield return (object) null;
      yield return (object) 0.1f;
      player.Sprite.Color = player.starFlyColor;
      player.Sprite.HairCount = 7;
      player.Hair.DrawPlayerSpriteOutline = true;
      player.level.Displacement.AddBurst(player.Center, 0.25f, 8f, 32f);
      player.starFlyTransforming = false;
      player.starFlyTimer = 2f;
      player.RefillDash();
      player.RefillStamina();
      Vector2 vector2 = Input.Feather.Value;
      if (vector2 == Vector2.Zero)
        vector2 = Vector2.UnitX * (float) player.Facing;
      player.Speed = vector2 * 250f;
      player.starFlyLastDir = vector2;
      player.level.Particles.Emit(FlyFeather.P_Boost, 12, player.Center, Vector2.One * 4f, (-vector2).Angle());
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
      player.level.DirectionalShake(player.starFlyLastDir);
      while ((double) player.starFlyTimer > 0.5)
        yield return (object) null;
      player.starFlyWarningSfx.Play("event:/game/06_reflection/feather_state_warning");
    }

    private int StarFlyUpdate()
    {
      this.starFlyBloom.Alpha = Calc.Approach(this.starFlyBloom.Alpha, 0.7f, Engine.DeltaTime * 2f);
      Input.Rumble(RumbleStrength.Climb, RumbleLength.Short);
      if (this.starFlyTransforming)
      {
        this.Speed = Calc.Approach(this.Speed, Vector2.Zero, 1000f * Engine.DeltaTime);
      }
      else
      {
        Vector2 starFlyLastDir = Input.Feather.Value;
        bool flag1 = false;
        if (starFlyLastDir == Vector2.Zero)
        {
          flag1 = true;
          starFlyLastDir = this.starFlyLastDir;
        }
        Vector2 vec = this.Speed.SafeNormalize(Vector2.Zero);
        Vector2 vector2 = !(vec == Vector2.Zero) ? vec.RotateTowards(starFlyLastDir.Angle(), 5.5850534f * Engine.DeltaTime) : starFlyLastDir;
        this.starFlyLastDir = vector2;
        float target;
        if (flag1)
        {
          this.starFlySpeedLerp = 0.0f;
          target = 91f;
        }
        else if (vector2 != Vector2.Zero && (double) Vector2.Dot(vector2, starFlyLastDir) >= 0.44999998807907104)
        {
          this.starFlySpeedLerp = Calc.Approach(this.starFlySpeedLerp, 1f, Engine.DeltaTime / 1f);
          target = MathHelper.Lerp(140f, 190f, this.starFlySpeedLerp);
        }
        else
        {
          this.starFlySpeedLerp = 0.0f;
          target = 140f;
        }
        this.starFlyLoopSfx.Param("feather_speed", flag1 ? 0.0f : 1f);
        float num = Calc.Approach(this.Speed.Length(), target, 1000f * Engine.DeltaTime);
        this.Speed = vector2 * num;
        if (this.level.OnInterval(0.02f))
          this.level.Particles.Emit(FlyFeather.P_Flying, 1, this.Center, Vector2.One * 2f, (-this.Speed).Angle());
        if (Input.Jump.Pressed)
        {
          if (this.OnGround(3))
          {
            this.Jump();
            return 0;
          }
          if (this.WallJumpCheck(-1))
          {
            this.WallJump(1);
            return 0;
          }
          if (this.WallJumpCheck(1))
          {
            this.WallJump(-1);
            return 0;
          }
        }
        if (Input.GrabCheck)
        {
          bool flag2 = false;
          int dir = 0;
          if (Input.MoveX.Value != -1 && this.ClimbCheck(1))
          {
            this.Facing = Facings.Right;
            dir = 1;
            flag2 = true;
          }
          else if (Input.MoveX.Value != 1 && this.ClimbCheck(-1))
          {
            this.Facing = Facings.Left;
            dir = -1;
            flag2 = true;
          }
          if (flag2)
          {
            if (!SaveData.Instance.Assists.NoGrabbing)
              return 1;
            this.Speed = Vector2.Zero;
            this.ClimbTrigger(dir);
            return 0;
          }
        }
        if (this.CanDash)
          return this.StartDash();
        this.starFlyTimer -= Engine.DeltaTime;
        if ((double) this.starFlyTimer <= 0.0)
        {
          if (Input.MoveY.Value == -1)
            this.Speed.Y = -100f;
          if (Input.MoveY.Value < 1)
          {
            this.varJumpSpeed = this.Speed.Y;
            this.AutoJump = true;
            this.AutoJumpTimer = 0.0f;
            this.varJumpTimer = 0.2f;
          }
          if ((double) this.Speed.Y > 0.0)
            this.Speed.Y = 0.0f;
          if ((double) Math.Abs(this.Speed.X) > 140.0)
            this.Speed.X = 140f * (float) Math.Sign(this.Speed.X);
          Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
          return 0;
        }
        if ((double) this.starFlyTimer < 0.5 && this.Scene.OnInterval(0.05f))
        {
          if (this.Sprite.Color == this.starFlyColor)
            this.Sprite.Color = Player.NormalHairColor;
          else
            this.Sprite.Color = this.starFlyColor;
        }
      }
      return 19;
    }

    public bool DoFlingBird(FlingBird bird)
    {
      if (this.Dead || this.StateMachine.State == 24)
        return false;
      this.flingBird = bird;
      this.StateMachine.State = 24;
      if (this.Holding != null)
        this.Drop();
      return true;
    }

    public void FinishFlingBird()
    {
      this.StateMachine.State = 0;
      this.AutoJump = true;
      this.forceMoveX = 1;
      this.forceMoveXTimer = 0.2f;
      this.Speed = FlingBird.FlingSpeed;
      this.varJumpTimer = 0.2f;
      this.varJumpSpeed = this.Speed.Y;
      this.launched = true;
    }

    private void FlingBirdBegin()
    {
      this.RefillDash();
      this.RefillStamina();
    }

    private void FlingBirdEnd()
    {
    }

    private int FlingBirdUpdate()
    {
      this.MoveTowardsX(this.flingBird.X, 250f * Engine.DeltaTime);
      this.MoveTowardsY(this.flingBird.Y + 8f + this.Collider.Height, 250f * Engine.DeltaTime);
      return 24;
    }

    private IEnumerator FlingBirdCoroutine()
    {
      yield break;
    }

    public void StartCassetteFly(Vector2 targetPosition, Vector2 control)
    {
      this.StateMachine.State = 21;
      this.cassetteFlyCurve = new SimpleCurve(this.Position, targetPosition, control);
      this.cassetteFlyLerp = 0.0f;
      this.Speed = Vector2.Zero;
      if (this.Holding == null)
        return;
      this.Drop();
    }

    private void CassetteFlyBegin()
    {
      this.Sprite.Play("bubble");
      this.Sprite.Y += 5f;
    }

    private void CassetteFlyEnd()
    {
    }

    private int CassetteFlyUpdate() => 21;

    private IEnumerator CassetteFlyCoroutine()
    {
      Player player = this;
      player.level.CanRetry = false;
      player.level.FormationBackdrop.Display = true;
      player.level.FormationBackdrop.Alpha = 0.5f;
      player.Sprite.Scale = Vector2.One * 1.25f;
      player.Depth = -2000000;
      yield return (object) 0.4f;
      while ((double) player.cassetteFlyLerp < 1.0)
      {
        if (player.level.OnInterval(0.03f))
          player.level.Particles.Emit(Player.P_CassetteFly, 2, player.Center, Vector2.One * 4f);
        player.cassetteFlyLerp = Calc.Approach(player.cassetteFlyLerp, 1f, 1.6f * Engine.DeltaTime);
        player.Position = player.cassetteFlyCurve.GetPoint(Ease.SineInOut(player.cassetteFlyLerp));
        player.level.Camera.Position = player.CameraTarget;
        yield return (object) null;
      }
      player.Position = player.cassetteFlyCurve.End;
      player.Sprite.Scale = Vector2.One * 1.25f;
      player.Sprite.Y -= 5f;
      player.Sprite.Play("fallFast");
      yield return (object) 0.2f;
      player.level.CanRetry = true;
      player.level.FormationBackdrop.Display = false;
      player.level.FormationBackdrop.Alpha = 0.5f;
      player.StateMachine.State = 0;
      player.Depth = 0;
    }

    public void StartAttract(Vector2 attractTo)
    {
      this.attractTo = attractTo.Round();
      this.StateMachine.State = 22;
    }

    private void AttractBegin() => this.Speed = Vector2.Zero;

    private void AttractEnd()
    {
    }

    private int AttractUpdate()
    {
      if ((double) Vector2.Distance(this.attractTo, this.ExactPosition) <= 1.5)
      {
        this.Position = this.attractTo;
        this.ZeroRemainderX();
        this.ZeroRemainderY();
      }
      else
      {
        Vector2 vector2 = Calc.Approach(this.ExactPosition, this.attractTo, 200f * Engine.DeltaTime);
        this.MoveToX(vector2.X);
        this.MoveToY(vector2.Y);
      }
      return 22;
    }

    public bool AtAttractTarget => this.StateMachine.State == 22 && this.ExactPosition == this.attractTo;

    private void DummyBegin()
    {
      this.DummyMoving = false;
      this.DummyGravity = true;
      this.DummyAutoAnimate = true;
    }

    private int DummyUpdate()
    {
      if (this.CanUnDuck)
        this.Ducking = false;
      if (!this.onGround && this.DummyGravity)
      {
        float num = (double) Math.Abs(this.Speed.Y) >= 40.0 || !Input.Jump.Check && !this.AutoJump ? 1f : 0.5f;
        if (this.level.InSpace)
          num *= 0.6f;
        this.Speed.Y = Calc.Approach(this.Speed.Y, 160f, 900f * num * Engine.DeltaTime);
      }
      if ((double) this.varJumpTimer > 0.0)
      {
        if (this.AutoJump || Input.Jump.Check)
          this.Speed.Y = Math.Min(this.Speed.Y, this.varJumpSpeed);
        else
          this.varJumpTimer = 0.0f;
      }
      if (!this.DummyMoving)
      {
        if ((double) Math.Abs(this.Speed.X) > 90.0 && this.DummyMaxspeed)
          this.Speed.X = Calc.Approach(this.Speed.X, 90f * (float) Math.Sign(this.Speed.X), 2500f * Engine.DeltaTime);
        if (this.DummyFriction)
          this.Speed.X = Calc.Approach(this.Speed.X, 0.0f, 1000f * Engine.DeltaTime);
      }
      if (this.DummyAutoAnimate)
      {
        if (this.onGround)
        {
          if ((double) this.Speed.X == 0.0)
            this.Sprite.Play("idle");
          else
            this.Sprite.Play("walk");
        }
        else if ((double) this.Speed.Y < 0.0)
          this.Sprite.Play("jumpSlow");
        else
          this.Sprite.Play("fallSlow");
      }
      return 11;
    }

    public IEnumerator DummyWalkTo(
      float x,
      bool walkBackwards = false,
      float speedMultiplier = 1f,
      bool keepWalkingIntoWalls = false)
    {
      Player player = this;
      player.StateMachine.State = 11;
      if ((double) Math.Abs(player.X - x) > 4.0 && !player.Dead)
      {
        player.DummyMoving = true;
        if (walkBackwards)
        {
          player.Sprite.Rate = -1f;
          player.Facing = (Facings) Math.Sign(player.X - x);
        }
        else
          player.Facing = (Facings) Math.Sign(x - player.X);
        while ((double) Math.Abs(x - player.X) > 4.0 && player.Scene != null && (keepWalkingIntoWalls || !player.CollideCheck<Solid>(player.Position + Vector2.UnitX * (float) Math.Sign(x - player.X))))
        {
          player.Speed.X = Calc.Approach(player.Speed.X, (float) Math.Sign(x - player.X) * 64f * speedMultiplier, 1000f * Engine.DeltaTime);
          yield return (object) null;
        }
        player.Sprite.Rate = 1f;
        player.Sprite.Play("idle");
        player.DummyMoving = false;
      }
    }

    public IEnumerator DummyWalkToExact(
      int x,
      bool walkBackwards = false,
      float speedMultiplier = 1f,
      bool cancelOnFall = false)
    {
      Player player = this;
      player.StateMachine.State = 11;
      if ((double) player.X != (double) x)
      {
        player.DummyMoving = true;
        if (walkBackwards)
        {
          player.Sprite.Rate = -1f;
          player.Facing = (Facings) Math.Sign(player.X - (float) x);
        }
        else
          player.Facing = (Facings) Math.Sign((float) x - player.X);
        int last = Math.Sign(player.X - (float) x);
        while (!player.Dead && (double) player.X != (double) x && !player.CollideCheck<Solid>(player.Position + new Vector2((float) player.Facing, 0.0f)) && (!cancelOnFall || player.OnGround()))
        {
          player.Speed.X = Calc.Approach(player.Speed.X, (float) Math.Sign((float) x - player.X) * 64f * speedMultiplier, 1000f * Engine.DeltaTime);
          int num = Math.Sign(player.X - (float) x);
          if (num != last)
          {
            player.X = (float) x;
            break;
          }
          last = num;
          yield return (object) null;
        }
        player.Speed.X = 0.0f;
        player.Sprite.Rate = 1f;
        player.Sprite.Play("idle");
        player.DummyMoving = false;
      }
    }

    public IEnumerator DummyRunTo(float x, bool fastAnim = false)
    {
      Player player = this;
      player.StateMachine.State = 11;
      if ((double) Math.Abs(player.X - x) > 4.0)
      {
        player.DummyMoving = true;
        if (fastAnim)
          player.Sprite.Play("runFast");
        else if (!player.Sprite.LastAnimationID.StartsWith("run"))
          player.Sprite.Play("runSlow");
        player.Facing = (Facings) Math.Sign(x - player.X);
        while ((double) Math.Abs(player.X - x) > 4.0)
        {
          player.Speed.X = Calc.Approach(player.Speed.X, (float) Math.Sign(x - player.X) * 90f, 1000f * Engine.DeltaTime);
          yield return (object) null;
        }
        player.Sprite.Play("idle");
        player.DummyMoving = false;
      }
    }

    private int FrozenUpdate() => 17;

    private int TempleFallUpdate()
    {
      this.Facing = Facings.Right;
      if (!this.onGround)
      {
        int num = this.level.Bounds.Left + 160;
        this.Speed.X = Calc.Approach(this.Speed.X, 54.000004f * ((double) Math.Abs((float) num - this.X) <= 4.0 ? 0.0f : (float) Math.Sign((float) num - this.X)), 325f * Engine.DeltaTime);
      }
      if (!this.onGround && this.DummyGravity)
        this.Speed.Y = Calc.Approach(this.Speed.Y, 320f, 225f * Engine.DeltaTime);
      return 20;
    }

    private IEnumerator TempleFallCoroutine()
    {
      Player player = this;
      player.Sprite.Play("fallFast");
      while (!player.onGround)
        yield return (object) null;
      player.Play("event:/char/madeline/mirrortemple_big_landing");
      if (player.Dashes <= 1)
        player.Sprite.Play("fallPose");
      else
        player.Sprite.Play("idle");
      player.Sprite.Scale.Y = 0.7f;
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
      player.level.DirectionalShake(new Vector2(0.0f, 1f), 0.5f);
      player.Speed.X = 0.0f;
      player.level.Particles.Emit(Player.P_SummitLandA, 12, player.BottomCenter, Vector2.UnitX * 3f, -1.5707964f);
      player.level.Particles.Emit(Player.P_SummitLandB, 8, player.BottomCenter - Vector2.UnitX * 2f, Vector2.UnitX * 2f, 3.403392f);
      player.level.Particles.Emit(Player.P_SummitLandB, 8, player.BottomCenter + Vector2.UnitX * 2f, Vector2.UnitX * 2f, -0.2617994f);
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime)
        yield return (object) null;
      player.StateMachine.State = 0;
    }

    private void ReflectionFallBegin() => this.IgnoreJumpThrus = true;

    private void ReflectionFallEnd()
    {
      FallEffects.Show(false);
      this.IgnoreJumpThrus = false;
    }

    private int ReflectionFallUpdate()
    {
      this.Facing = Facings.Right;
      if (this.Scene.OnInterval(0.05f))
      {
        this.wasDashB = true;
        this.CreateTrail();
      }
      this.Speed.Y = !this.CollideCheck<Water>() ? Calc.Approach(this.Speed.Y, 320f, 225f * Engine.DeltaTime) : Calc.Approach(this.Speed.Y, -20f, 400f * Engine.DeltaTime);
      foreach (Entity entity in this.Scene.Tracker.GetEntities<FlyFeather>())
        entity.RemoveSelf();
      CrystalStaticSpinner crystalStaticSpinner = this.Scene.CollideFirst<CrystalStaticSpinner>(new Rectangle((int) ((double) this.X - 6.0), (int) ((double) this.Y - 6.0), 12, 12));
      if (crystalStaticSpinner != null)
      {
        crystalStaticSpinner.Destroy();
        this.level.Shake();
        Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
        Celeste.Freeze(0.01f);
      }
      return 18;
    }

    private IEnumerator ReflectionFallCoroutine()
    {
      Player player = this;
      player.Sprite.Play("bigFall");
      player.level.StartCutscene(new Action<Level>(player.OnReflectionFallSkip));
      for (float t = 0.0f; (double) t < 2.0; t += Engine.DeltaTime)
      {
        player.Speed.Y = 0.0f;
        yield return (object) null;
      }
      FallEffects.Show(true);
      player.Speed.Y = 320f;
      while (!player.CollideCheck<Water>())
        yield return (object) null;
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
      FallEffects.Show(false);
      player.Sprite.Play("bigFallRecover");
      player.level.Session.Audio.Music.Event = "event:/music/lvl6/main";
      player.level.Session.Audio.Apply();
      player.level.EndCutscene();
      yield return (object) 1.2f;
      player.StateMachine.State = 0;
    }

    private void OnReflectionFallSkip(Level level) => level.OnEndOfFrame += (Action) (() =>
    {
      level.Remove((Entity) this);
      level.UnloadLevel();
      level.Session.Level = "00";
      Session session = level.Session;
      Level level1 = level;
      Rectangle bounds = level.Bounds;
      double left = (double) bounds.Left;
      bounds = level.Bounds;
      double bottom = (double) bounds.Bottom;
      Vector2 from = new Vector2((float) left, (float) bottom);
      Vector2? nullable = new Vector2?(level1.GetSpawnPoint(from));
      session.RespawnPoint = nullable;
      level.LoadLevel(Player.IntroTypes.None);
      FallEffects.Show(false);
      level.Session.Audio.Music.Event = "event:/music/lvl6/main";
      level.Session.Audio.Apply();
    });

    public IEnumerator IntroWalkCoroutine()
    {
      Player player = this;
      Vector2 start = player.Position;
      if (player.IntroWalkDirection == Facings.Right)
      {
        player.X = (float) (player.level.Bounds.Left - 16);
        player.Facing = Facings.Right;
      }
      else
      {
        player.X = (float) (player.level.Bounds.Right + 16);
        player.Facing = Facings.Left;
      }
      yield return (object) 0.3f;
      player.Sprite.Play("runSlow");
      while ((double) Math.Abs(player.X - start.X) > 2.0 && !player.CollideCheck<Solid>(player.Position + new Vector2((float) player.Facing, 0.0f)))
      {
        player.MoveTowardsX(start.X, 64f * Engine.DeltaTime);
        yield return (object) null;
      }
      player.Position = start;
      player.Sprite.Play("idle");
      yield return (object) 0.2f;
      player.StateMachine.State = 0;
    }

    private IEnumerator IntroJumpCoroutine()
    {
      Player player = this;
      Vector2 start = player.Position;
      bool wasSummitJump = player.StateMachine.PreviousState == 10;
      player.Depth = -1000000;
      player.Facing = Facings.Right;
      if (!wasSummitJump)
      {
        player.Y = (float) (player.level.Bounds.Bottom + 16);
        yield return (object) 0.5f;
      }
      else
      {
        start.Y = (float) (player.level.Bounds.Bottom - 24);
        player.MoveToX((float) ((int) Math.Round((double) player.X / 8.0) * 8));
      }
      if (!wasSummitJump)
        player.Sprite.Play("jumpSlow");
      while ((double) player.Y > (double) start.Y - 8.0)
      {
        player.Y += -120f * Engine.DeltaTime;
        yield return (object) null;
      }
      player.Y = (float) Math.Round((double) player.Y);
      player.Speed.Y = -100f;
      while ((double) player.Speed.Y < 0.0)
      {
        player.Speed.Y += Engine.DeltaTime * 800f;
        yield return (object) null;
      }
      player.Speed.Y = 0.0f;
      if (wasSummitJump)
      {
        yield return (object) 0.2f;
        player.Play("event:/char/madeline/summit_areastart");
        player.Sprite.Play("launchRecover");
        yield return (object) 0.1f;
      }
      else
        yield return (object) 0.1f;
      if (!wasSummitJump)
        player.Sprite.Play("fallSlow");
      while (!player.onGround)
      {
        player.Speed.Y += Engine.DeltaTime * 800f;
        yield return (object) null;
      }
      if (player.StateMachine.PreviousState != 10)
        player.Position = start;
      player.Depth = 0;
      player.level.DirectionalShake(Vector2.UnitY);
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
      if (wasSummitJump)
      {
        player.level.Particles.Emit(Player.P_SummitLandA, 12, player.BottomCenter, Vector2.UnitX * 3f, -1.5707964f);
        player.level.Particles.Emit(Player.P_SummitLandB, 8, player.BottomCenter - Vector2.UnitX * 2f, Vector2.UnitX * 2f, 3.403392f);
        player.level.Particles.Emit(Player.P_SummitLandB, 8, player.BottomCenter + Vector2.UnitX * 2f, Vector2.UnitX * 2f, -0.2617994f);
        player.level.ParticlesBG.Emit(Player.P_SummitLandC, 30, player.BottomCenter, Vector2.UnitX * 5f);
        yield return (object) 0.35f;
        for (int index = 0; index < player.Hair.Nodes.Count; ++index)
          player.Hair.Nodes[index] = new Vector2(0.0f, (float) (2 + index));
      }
      player.StateMachine.State = 0;
    }

    private IEnumerator IntroMoonJumpCoroutine()
    {
      Player player = this;
      Vector2 start = player.Position;
      player.Facing = Facings.Right;
      player.Speed = Vector2.Zero;
      player.Visible = false;
      player.Y = (float) (player.level.Bounds.Bottom + 16);
      yield return (object) 0.5f;
      yield return (object) player.MoonLanding(start);
      player.StateMachine.State = 0;
    }

    public IEnumerator MoonLanding(Vector2 groundPosition)
    {
      Player player = this;
      player.Depth = -1000000;
      player.Speed = Vector2.Zero;
      player.Visible = true;
      player.Sprite.Play("jumpSlow");
      while ((double) player.Y > (double) groundPosition.Y - 8.0)
      {
        player.MoveV(-200f * Engine.DeltaTime);
        yield return (object) null;
      }
      player.Speed.Y = -200f;
      while ((double) player.Speed.Y < 0.0)
      {
        player.Speed.Y += Engine.DeltaTime * 400f;
        yield return (object) null;
      }
      player.Speed.Y = 0.0f;
      yield return (object) 0.2f;
      player.Sprite.Play("fallSlow");
      float s = 100f;
      while (!player.OnGround())
      {
        player.Speed.Y += Engine.DeltaTime * s;
        s = Calc.Approach(s, 2f, Engine.DeltaTime * 50f);
        yield return (object) null;
      }
      player.Depth = 0;
    }

    private IEnumerator IntroWakeUpCoroutine()
    {
      this.Sprite.Play("asleep");
      yield return (object) 0.5f;
      yield return (object) this.Sprite.PlayRoutine("wakeUp");
      yield return (object) 0.2f;
      this.StateMachine.State = 0;
    }

    private void IntroRespawnBegin()
    {
      this.Play("event:/char/madeline/revive");
      this.Depth = -1000000;
      this.introEase = 1f;
      Vector2 from = this.Position;
      ref Vector2 local1 = ref from;
      double x = (double) from.X;
      Rectangle bounds1 = this.level.Bounds;
      double min1 = (double) bounds1.Left + 40.0;
      bounds1 = this.level.Bounds;
      double max1 = (double) bounds1.Right - 40.0;
      double num1 = (double) MathHelper.Clamp((float) x, (float) min1, (float) max1);
      local1.X = (float) num1;
      ref Vector2 local2 = ref from;
      double y = (double) from.Y;
      Rectangle bounds2 = this.level.Bounds;
      double min2 = (double) bounds2.Top + 40.0;
      bounds2 = this.level.Bounds;
      double max2 = (double) bounds2.Bottom - 40.0;
      double num2 = (double) MathHelper.Clamp((float) y, (float) min2, (float) max2);
      local2.Y = (float) num2;
      this.deadOffset = from;
      from -= this.Position;
      this.respawnTween = Tween.Create(Tween.TweenMode.Oneshot, duration: 0.6f, start: true);
      this.respawnTween.OnUpdate = (Action<Tween>) (t =>
      {
        this.deadOffset = Vector2.Lerp(from, Vector2.Zero, t.Eased);
        this.introEase = 1f - t.Eased;
      });
      this.respawnTween.OnComplete = (Action<Tween>) (t =>
      {
        if (this.StateMachine.State != 14)
          return;
        this.StateMachine.State = 0;
        this.Sprite.Scale = new Vector2(1.5f, 0.5f);
      });
      this.Add((Component) this.respawnTween);
    }

    private void IntroRespawnEnd()
    {
      this.Depth = 0;
      this.deadOffset = Vector2.Zero;
      this.Remove((Component) this.respawnTween);
      this.respawnTween = (Tween) null;
    }

    public IEnumerator IntroThinkForABitCoroutine()
    {
      Player player = this;
      (player.Scene as Level).Camera.X += 8f;
      yield return (object) 0.1f;
      player.Sprite.Play("walk");
      float target = player.X + 8f;
      while ((double) player.X < (double) target)
      {
        player.MoveH(32f * Engine.DeltaTime);
        yield return (object) null;
      }
      player.Sprite.Play("idle");
      yield return (object) 0.3f;
      player.Facing = Facings.Left;
      yield return (object) 0.8f;
      player.Facing = Facings.Right;
      yield return (object) 0.1f;
      player.StateMachine.State = 0;
    }

    private void BirdDashTutorialBegin()
    {
      this.DashBegin();
      this.Play("event:/char/madeline/dash_red_right");
      this.Sprite.Play("dash");
    }

    private int BirdDashTutorialUpdate() => 16;

    private IEnumerator BirdDashTutorialCoroutine()
    {
      Player player = this;
      yield return (object) null;
      player.CreateTrail();
      player.Add((Component) Alarm.Create(Alarm.AlarmMode.Oneshot, new Action(player.CreateTrail), 0.08f, true));
      player.Add((Component) Alarm.Create(Alarm.AlarmMode.Oneshot, new Action(player.CreateTrail), 0.15f, true));
      Vector2 vector2 = new Vector2(1f, -1f).SafeNormalize();
      player.Facing = Facings.Right;
      player.Speed = vector2 * 240f;
      player.DashDir = vector2;
      player.SceneAs<Level>().DirectionalShake(player.DashDir, 0.2f);
      SlashFx.Burst(player.Center, player.DashDir.Angle());
      float time;
      for (time = 0.0f; (double) time < 0.15000000596046448; time += Engine.DeltaTime)
      {
        if (player.Speed != Vector2.Zero && player.level.OnInterval(0.02f))
          player.level.ParticlesFG.Emit(Player.P_DashA, player.Center + Calc.Random.Range(Vector2.One * -2f, Vector2.One * 2f), player.DashDir.Angle());
        yield return (object) null;
      }
      player.AutoJump = true;
      player.AutoJumpTimer = 0.0f;
      if ((double) player.DashDir.Y <= 0.0)
        player.Speed = player.DashDir * 160f;
      if ((double) player.Speed.Y < 0.0)
        player.Speed.Y *= 0.75f;
      player.Sprite.Play("fallFast");
      bool climbing = false;
      while (!player.OnGround() && !climbing)
      {
        player.Speed.Y = Calc.Approach(player.Speed.Y, 160f, 900f * Engine.DeltaTime);
        if (player.CollideCheck<Solid>(player.Position + new Vector2(1f, 0.0f)))
          climbing = true;
        if ((double) player.Top > (double) player.level.Bounds.Bottom)
        {
          player.level.CancelCutscene();
          player.Die(Vector2.Zero);
        }
        yield return (object) null;
      }
      if (climbing)
      {
        player.Sprite.Play("wallslide");
        Dust.Burst(player.Position + new Vector2(4f, -6f), new Vector2(-4f, 0.0f).Angle());
        player.Speed.Y = 0.0f;
        yield return (object) 0.2f;
        player.Sprite.Play("climbUp");
        while (player.CollideCheck<Solid>(player.Position + new Vector2(1f, 0.0f)))
        {
          player.Y += -45f * Engine.DeltaTime;
          yield return (object) null;
        }
        player.Y = (float) Math.Round((double) player.Y);
        player.Play("event:/char/madeline/climb_ledge");
        player.Sprite.Play("jumpFast");
        player.Speed.Y = -105f;
        while (!player.OnGround())
        {
          player.Speed.Y = Calc.Approach(player.Speed.Y, 160f, 900f * Engine.DeltaTime);
          player.Speed.X = 20f;
          yield return (object) null;
        }
        player.Speed.X = 0.0f;
        player.Speed.Y = 0.0f;
        player.Sprite.Play("walk");
        for (time = 0.0f; (double) time < 0.5; time += Engine.DeltaTime)
        {
          player.X += 32f * Engine.DeltaTime;
          yield return (object) null;
        }
        player.Sprite.Play("tired");
      }
      else
      {
        player.Sprite.Play("tired");
        player.Speed.Y = 0.0f;
        while ((double) player.Speed.X != 0.0)
        {
          player.Speed.X = Calc.Approach(player.Speed.X, 0.0f, 240f * Engine.DeltaTime);
          if (player.Scene.OnInterval(0.04f))
            Dust.Burst(player.BottomCenter + new Vector2(0.0f, -2f), -2.3561945f);
          yield return (object) null;
        }
      }
    }

    public EventInstance Play(string sound, string param = null, float value = 0.0f)
    {
      float num = 0.0f;
      if (this.Scene is Level scene && scene.Raining)
        num = 1f;
      this.AddChaserStateSound(sound, param, value);
      return Audio.Play(sound, this.Center, param, value, "raining", num);
    }

    public void Loop(SoundSource sfx, string sound)
    {
      this.AddChaserStateSound(sound, (string) null, 0.0f, Player.ChaserStateSound.Actions.Loop);
      sfx.Play(sound);
    }

    public void Stop(SoundSource sfx)
    {
      if (!sfx.Playing)
        return;
      this.AddChaserStateSound(sfx.EventName, (string) null, 0.0f, Player.ChaserStateSound.Actions.Stop);
      sfx.Stop();
    }

    private void AddChaserStateSound(string sound, Player.ChaserStateSound.Actions action) => this.AddChaserStateSound(sound, (string) null, 0.0f, action);

    private void AddChaserStateSound(
      string sound,
      string param = null,
      float value = 0.0f,
      Player.ChaserStateSound.Actions action = Player.ChaserStateSound.Actions.Oneshot)
    {
      string str = (string) null;
      SFX.MadelineToBadelineSound.TryGetValue(sound, out str);
      if (str == null)
        return;
      this.activeSounds.Add(new Player.ChaserStateSound()
      {
        Event = str,
        Parameter = param,
        ParameterValue = value,
        Action = action
      });
    }

    private ParticleType DustParticleFromSurfaceIndex(int index) => index == 40 ? ParticleTypes.SparkyDust : ParticleTypes.Dust;

    public enum IntroTypes
    {
      Transition,
      Respawn,
      WalkInRight,
      WalkInLeft,
      Jump,
      WakeUp,
      Fall,
      TempleMirrorVoid,
      None,
      ThinkForABit,
    }

    public struct ChaserStateSound
    {
      public string Event;
      public string Parameter;
      public float ParameterValue;
      public Player.ChaserStateSound.Actions Action;

      public enum Actions
      {
        Oneshot,
        Loop,
        Stop,
      }
    }

    public struct ChaserState
    {
      public Vector2 Position;
      public float TimeStamp;
      public string Animation;
      public Facings Facing;
      public bool OnGround;
      public Color HairColor;
      public int Depth;
      public Vector2 Scale;
      public Vector2 DashDirection;
      private Player.ChaserStateSound sound0;
      private Player.ChaserStateSound sound1;
      private Player.ChaserStateSound sound2;
      private Player.ChaserStateSound sound3;
      private Player.ChaserStateSound sound4;
      public int Sounds;

      public ChaserState(Player player)
      {
        this.Position = player.Position;
        this.TimeStamp = player.Scene.TimeActive;
        this.Animation = player.Sprite.CurrentAnimationID;
        this.Facing = player.Facing;
        this.OnGround = player.onGround;
        this.HairColor = player.Hair.Color;
        this.Depth = player.Depth;
        this.Scale = new Vector2(Math.Abs(player.Sprite.Scale.X) * (float) player.Facing, player.Sprite.Scale.Y);
        this.DashDirection = player.DashDir;
        List<Player.ChaserStateSound> activeSounds = player.activeSounds;
        this.Sounds = Math.Min(5, activeSounds.Count);
        this.sound0 = this.Sounds > 0 ? activeSounds[0] : new Player.ChaserStateSound();
        this.sound1 = this.Sounds > 1 ? activeSounds[1] : new Player.ChaserStateSound();
        this.sound2 = this.Sounds > 2 ? activeSounds[2] : new Player.ChaserStateSound();
        this.sound3 = this.Sounds > 3 ? activeSounds[3] : new Player.ChaserStateSound();
        this.sound4 = this.Sounds > 4 ? activeSounds[4] : new Player.ChaserStateSound();
      }

      public Player.ChaserStateSound this[int index]
      {
        get
        {
          switch (index)
          {
            case 0:
              return this.sound0;
            case 1:
              return this.sound1;
            case 2:
              return this.sound2;
            case 3:
              return this.sound3;
            case 4:
              return this.sound4;
            default:
              return new Player.ChaserStateSound();
          }
        }
      }
    }
  }
}
