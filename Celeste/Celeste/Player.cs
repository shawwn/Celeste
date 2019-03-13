// Decompiled with JetBrains decompiler
// Type: Celeste.Player
// Assembly: Celeste, Version=1.0.0.0 Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

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
    private static readonly Vector2 CarryOffsetTarget = new Vector2(0.0f, -12f);
    private static Chooser<string> idleColdOptions = new Chooser<string>().Add("idleA", 5f).Add("idleB", 3f).Add("idleC", 1f);
    private static Chooser<string> idleNoBackpackOptions = new Chooser<string>().Add("idleA", 1f).Add("idleB", 3f).Add("idleC", 3f);
    private static Chooser<string> idleWarmOptions = new Chooser<string>().Add("idleA", 5f).Add("idleB", 3f);
    public static readonly Color NormalHairColor = Calc.HexToColor("AC3232");
    public static readonly Color FlyPowerHairColor = Calc.HexToColor("F2EB6D");
    public static readonly Color UsedHairColor = Calc.HexToColor("44B7FF");
    public static readonly Color FlashHairColor = Color.White;
    public static readonly Color TwoDashesHairColor = Calc.HexToColor("ff6def");
    public static readonly Color NormalBadelineHairColor = BadelineOldsite.HairColor;
    public static readonly Color UsedBadelineHairColor = Color.Lerp(BadelineOldsite.HairColor, Color.Black, 0.5f);
    public static readonly Color TwoDashesBadelineHairColor = Calc.HexToColor("ff6def");
    public float Stamina = 110f;
    public bool DummyAutoAnimate = true;
    public bool FlipInReflection = false;
    public int StrawberryCollectIndex = 0;
    public float StrawberryCollectResetTimer = 0.0f;
    private float wallSlideTimer = 1.2f;
    private List<Player.ChaserStateSound> activeSounds = new List<Player.ChaserStateSound>();
    private readonly Hitbox normalHitbox = new Hitbox(8f, 11f, -4f, -11f);
    private readonly Hitbox duckHitbox = new Hitbox(8f, 6f, -4f, -6f);
    private readonly Hitbox normalHurtbox = new Hitbox(8f, 9f, -4f, -11f);
    private readonly Hitbox duckHurtbox = new Hitbox(8f, 4f, -4f, -6f);
    private readonly Hitbox starFlyHitbox = new Hitbox(8f, 8f, -4f, -10f);
    private readonly Hitbox starFlyHurtbox = new Hitbox(6f, 6f, -3f, -9f);
    private Vector2 normalLightOffset = new Vector2(0.0f, -8f);
    private Vector2 duckingLightOffset = new Vector2(0.0f, -3f);
    private List<Entity> temp = new List<Entity>();
    private Color starFlyColor = Calc.HexToColor("ffd65c");
    public bool DummyMoving = false;
    public bool DummyGravity = true;
    public bool DummyFriction = true;
    public bool DummyMaxspeed = true;
    public static ParticleType P_DashA;
    public static ParticleType P_DashB;
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
    private const float ClimbUpCost = 45.45454f;
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
    public bool StrawberriesBlocked;
    public Vector2 PreviousPosition;
    public Vector2 ForceStrongWindHair;
    public Vector2? OverrideDashDirection;
    public bool JustRespawned;
    private Level level;
    private Collision onCollideH;
    private Collision onCollideV;
    private bool onGround;
    private bool wasOnGround;
    private int moveX;
    private bool flash;
    private bool wasDucking;
    private int climbTriggerDir;
    private float idleTimer;
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
    private float dashGliderBoostTimer;
    private List<Player.ChaserState> chaserStates;
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
    private Booster lastBooster;
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
    private EventInstance idleSfx;
    private float hairFlashTimer;
    public Color? OverrideHairColor;
    private Vector2 windDirection;
    private float windTimeout;
    private float windHairTimer;
    public Player.IntroTypes IntroType;
    private MirrorReflection reflection;
    public PlayerSpriteMode DefaultSpriteMode;
    private const float LaunchedBoostCheckSpeedSq = 10000f;
    private const float LaunchedJumpCheckSpeedSq = 48400f;
    private const float LaunchedMinSpeedSq = 19600f;
    private const float LaunchedDoubleSpeedSq = 22500f;
    private const float SideBounceSpeed = 240f;
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
    private const float StarFlyRotateSpeed = 5.585053f;
    private const float StarFlyEndX = 160f;
    private const float StarFlyEndXVarJumpTime = 0.1f;
    private const float StarFlyEndFlashDuration = 0.5f;
    private const float StarFlyEndNoBounceTime = 0.2f;
    private const float StarFlyWallBounce = -0.5f;
    private const float StarFlyMaxExitY = 0.0f;
    private const float StarFlyMaxExitX = 140f;
    private const float StarFlyExitUp = -100f;
    private BloomPoint starFlyBloom;
    private float starFlyTimer;
    private bool starFlyTransforming;
    private float starFlySpeedLerp;
    private Vector2 starFlyLastDir;
    private SoundSource starFlyLoopSfx;
    private SoundSource starFlyWarningSfx;
    private SimpleCurve cassetteFlyCurve;
    private float cassetteFlyLerp;
    private Vector2 attractTo;
    private Facings IntroWalkDirection;
    private Tween respawnTween;

    public bool Dead { get; private set; }

    public Player(Vector2 position, PlayerSpriteMode spriteMode)
      : base(new Vector2((float) (int) position.X, (float) (int) position.Y))
    {
      this.DefaultSpriteMode = spriteMode;
      this.Depth = 0;
      this.Tag = (int) Tags.Persistent;
      if (SaveData.Instance != null && SaveData.Instance.Assists.PlayAsBadeline)
        spriteMode = PlayerSpriteMode.Badeline;
      this.Sprite = new PlayerSprite(spriteMode);
      this.Add((Component) (this.Hair = new PlayerHair(this.Sprite)));
      this.Add((Component) this.Sprite);
      this.Hair.Color = spriteMode != PlayerSpriteMode.Badeline ? Player.NormalHairColor : Player.NormalBadelineHairColor;
      this.startHairCount = this.Sprite.HairCount;
      this.sweatSprite = GFX.SpriteBank.Create("player_sweat");
      this.Add((Component) this.sweatSprite);
      this.Collider = (Collider) this.normalHitbox;
      this.hurtbox = this.normalHurtbox;
      this.onCollideH = new Collision(this.OnCollideH);
      this.onCollideV = new Collision(this.OnCollideV);
      this.StateMachine = new StateMachine(23);
      this.StateMachine.SetCallbacks(0, new Func<int>(this.NormalUpdate), (Func<IEnumerator>) null, new Action(this.NormalBegin), new Action(this.NormalEnd));
      this.StateMachine.SetCallbacks(1, new Func<int>(this.ClimbUpdate), (Func<IEnumerator>) null, new Action(this.ClimbBegin), new Action(this.ClimbEnd));
      this.StateMachine.SetCallbacks(2, new Func<int>(this.DashUpdate), new Func<IEnumerator>(this.DashCoroutine), new Action(this.DashBegin), new Action(this.DashEnd));
      this.StateMachine.SetCallbacks(3, new Func<int>(this.SwimUpdate), (Func<IEnumerator>) null, new Action(this.SwimBegin), (Action) null);
      this.StateMachine.SetCallbacks(4, new Func<int>(this.BoostUpdate), new Func<IEnumerator>(this.BoostCoroutine), new Action(this.BoostBegin), new Action(this.BoostEnd));
      this.StateMachine.SetCallbacks(5, new Func<int>(this.RedDashUpdate), new Func<IEnumerator>(this.RedDashCoroutine), new Action(this.RedDashBegin), new Action(this.RedDashEnd));
      this.StateMachine.SetCallbacks(6, new Func<int>(this.HitSquashUpdate), (Func<IEnumerator>) null, new Action(this.HitSquashBegin), (Action) null);
      this.StateMachine.SetCallbacks(7, new Func<int>(this.LaunchUpdate), (Func<IEnumerator>) null, new Action(this.LaunchBegin), (Action) null);
      this.StateMachine.SetCallbacks(8, (Func<int>) null, new Func<IEnumerator>(this.PickupCoroutine), (Action) null, (Action) null);
      this.StateMachine.SetCallbacks(9, new Func<int>(this.DreamDashUpdate), (Func<IEnumerator>) null, new Action(this.DreamDashBegin), new Action(this.DreamDashEnd));
      this.StateMachine.SetCallbacks(10, new Func<int>(this.SummitLaunchUpdate), (Func<IEnumerator>) null, new Action(this.SummitLaunchBegin), (Action) null);
      this.StateMachine.SetCallbacks(11, new Func<int>(this.DummyUpdate), (Func<IEnumerator>) null, new Action(this.DummyBegin), (Action) null);
      this.StateMachine.SetCallbacks(12, (Func<int>) null, new Func<IEnumerator>(this.IntroWalkCoroutine), (Action) null, (Action) null);
      this.StateMachine.SetCallbacks(13, (Func<int>) null, new Func<IEnumerator>(this.IntroJumpCoroutine), (Action) null, (Action) null);
      this.StateMachine.SetCallbacks(14, (Func<int>) null, (Func<IEnumerator>) null, new Action(this.IntroRespawnBegin), new Action(this.IntroRespawnEnd));
      this.StateMachine.SetCallbacks(15, (Func<int>) null, new Func<IEnumerator>(this.IntroWakeUpCoroutine), (Action) null, (Action) null);
      this.StateMachine.SetCallbacks(20, new Func<int>(this.TempleFallUpdate), new Func<IEnumerator>(this.TempleFallCoroutine), (Action) null, (Action) null);
      this.StateMachine.SetCallbacks(18, new Func<int>(this.ReflectionFallUpdate), new Func<IEnumerator>(this.ReflectionFallCoroutine), new Action(this.ReflectionFallBegin), new Action(this.ReflectionFallEnd));
      this.StateMachine.SetCallbacks(16, new Func<int>(this.BirdDashTutorialUpdate), new Func<IEnumerator>(this.BirdDashTutorialCoroutine), new Action(this.BirdDashTutorialBegin), (Action) null);
      this.StateMachine.SetCallbacks(17, new Func<int>(this.FrozenUpdate), (Func<IEnumerator>) null, (Action) null, (Action) null);
      this.StateMachine.SetCallbacks(19, new Func<int>(this.StarFlyUpdate), new Func<IEnumerator>(this.StarFlyCoroutine), new Action(this.StarFlyBegin), new Action(this.StarFlyEnd));
      this.StateMachine.SetCallbacks(21, new Func<int>(this.CassetteFlyUpdate), new Func<IEnumerator>(this.CassetteFlyCoroutine), new Action(this.CassetteFlyBegin), new Action(this.CassetteFlyEnd));
      this.StateMachine.SetCallbacks(22, new Func<int>(this.AttractUpdate), (Func<IEnumerator>) null, new Action(this.AttractBegin), new Action(this.AttractEnd));
      this.Add((Component) this.StateMachine);
      this.Add((Component) (this.Leader = new Leader(new Vector2(0.0f, -8f))));
      this.lastAim = Vector2.UnitX;
      this.Facing = Facings.Right;
      this.chaserStates = new List<Player.ChaserState>();
      this.triggersInside = new HashSet<Trigger>();
      this.Add((Component) (this.Light = new VertexLight(this.normalLightOffset, Color.White, 1f, 32, 64)));
      this.Add((Component) new WaterInteraction((Func<bool>) (() => this.StateMachine.State == 2 || this.StateMachine.State == 18)));
      this.Add((Component) new WindMover(new Action<Vector2>(this.WindMove)));
      this.Add((Component) (this.wallSlideSfx = new SoundSource()));
      this.Add((Component) (this.swimSurfaceLoopSfx = new SoundSource()));
      this.Sprite.OnFrameChange = (Action<string>) (anim =>
      {
        if (this.Scene == null || this.Dead)
          return;
        int currentAnimationFrame = this.Sprite.CurrentAnimationFrame;
        if (anim.Equals("runSlow_carry") && (currentAnimationFrame == 0 || currentAnimationFrame == 6) || anim.Equals("runFast") && (currentAnimationFrame == 0 || currentAnimationFrame == 6) || (anim.Equals("runSlow") && (currentAnimationFrame == 0 || currentAnimationFrame == 6) || anim.Equals("walk") && (currentAnimationFrame == 0 || currentAnimationFrame == 6)) || (anim.Equals("runStumble") && currentAnimationFrame == 6 || anim.Equals("flip") && currentAnimationFrame == 4 || anim.Equals("runWind") && (currentAnimationFrame == 0 || currentAnimationFrame == 6) || anim.Equals("idleC") && this.Sprite.Mode == PlayerSpriteMode.MadelineNoBackpack && (currentAnimationFrame == 3 || currentAnimationFrame == 6 || (currentAnimationFrame == 8 || currentAnimationFrame == 11))) || anim.Equals("carryTheoWalk") && (currentAnimationFrame == 0 || currentAnimationFrame == 6) || anim.Equals("push") && (currentAnimationFrame == 8 || currentAnimationFrame == 15))
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
          this.Play("event:/char/madeline/campfire_stand", (string) null, 0.0f);
        else if (anim.Equals("sitDown") && currentAnimationFrame == 12)
          this.Play("event:/char/madeline/summit_sit", (string) null, 0.0f);
        if (anim.Equals("push") && (currentAnimationFrame == 8 || currentAnimationFrame == 15))
          Dust.BurstFG(this.Position + new Vector2((float) (-(int) this.Facing * 5), -1f), new Vector2((float) -(int) this.Facing, -0.5f).Angle(), 1, 0.0f);
      });
      this.Sprite.OnLastFrame = (Action<string>) (anim =>
      {
        if (this.Scene == null || this.Dead || (!(this.Sprite.CurrentAnimationID == "idle") || this.level.InCutscene) || (double) this.idleTimer <= 3.0 || !Calc.Random.Chance(0.2f))
          return;
        string id = this.Sprite.Mode != PlayerSpriteMode.Madeline ? Player.idleNoBackpackOptions.Choose() : (this.level.CoreMode == Session.CoreModes.Hot ? Player.idleWarmOptions : Player.idleColdOptions).Choose();
        if (!string.IsNullOrEmpty(id))
        {
          this.Sprite.Play(id, false, false);
          if (this.Sprite.Mode == PlayerSpriteMode.Madeline)
          {
            if (id == "idleB")
              this.idleSfx = this.Play("event:/char/madeline/idle_scratch", (string) null, 0.0f);
            else if (id == "idleC")
              this.idleSfx = this.Play("event:/char/madeline/idle_sneeze", (string) null, 0.0f);
          }
          else if (id == "idleA")
            this.idleSfx = this.Play("event:/char/madeline/idle_crackknuckles", (string) null, 0.0f);
        }
      });
      this.Sprite.OnChange = (Action<string, string>) ((last, next) =>
      {
        if (!(last == "idleB") && !(last == "idleC") || (next == null || next.StartsWith("idle")) || !((HandleBase) this.idleSfx != (HandleBase) null))
          return;
        Audio.Stop(this.idleSfx, true);
      });
      this.Add((Component) (this.reflection = new MirrorReflection()));
    }

    public void ResetSprite(PlayerSpriteMode mode)
    {
      string currentAnimationId = this.Sprite.CurrentAnimationID;
      int currentAnimationFrame = this.Sprite.CurrentAnimationFrame;
      this.Sprite.RemoveSelf();
      this.Add((Component) (this.Sprite = new PlayerSprite(mode)));
      this.Sprite.Play(currentAnimationId, false, false);
      this.Sprite.SetAnimationFrame(currentAnimationFrame);
      this.Hair.Sprite = this.Sprite;
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      this.level = this.SceneAs<Level>();
      this.lastDashes = this.Dashes = this.MaxDashes;
      if ((double) this.X > (double) this.level.Bounds.Center.X && this.IntroType != Player.IntroTypes.None)
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
          this.Sprite.Play("asleep", false, false);
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
      }
      this.IntroType = Player.IntroTypes.Transition;
      this.StartHair();
      this.PreviousPosition = this.Position;
    }

    public void StartTempleMirrorVoidSleep()
    {
      this.Sprite.Play("asleep", false, false);
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
      Audio.Stop(this.conveyorLoopSfx, true);
      foreach (Trigger trigger in this.triggersInside)
      {
        trigger.Triggered = false;
        trigger.OnLeave(this);
      }
    }

    public override void SceneEnd(Scene scene)
    {
      base.SceneEnd(scene);
      Audio.Stop(this.conveyorLoopSfx, true);
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
          this.Facing = ToFacing.Convert(-(int) this.Facing);
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
          this.Facing = ToFacing.Convert(-(int) this.Facing);
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
      if ((double) this.dashGliderBoostTimer > 0.0)
        this.dashGliderBoostTimer -= Engine.DeltaTime;
      if ((double) this.lowFrictionStopTimer > 0.0)
        this.lowFrictionStopTimer -= Engine.DeltaTime;
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
      if ((uint) this.wallSlideDir > 0U)
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
          this.sweatSprite.Play("idle", false, false);
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
        this.MoveHExact((int) vector2.X, (Collision) null, (Solid) null);
        this.MoveVExact((int) vector2.Y, (Collision) null, (Solid) null);
      }
      if ((double) this.noWindTimer > 0.0)
        this.noWindTimer -= Engine.DeltaTime;
      if (this.moveX != 0 && this.InControl && (this.StateMachine.State != 1 && this.StateMachine.State != 8) && this.StateMachine.State != 5 && this.StateMachine.State != 6)
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
      if ((uint) this.hopWaitX > 0U)
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
        this.MoveV(-40f * Engine.DeltaTime, (Collision) null, (Solid) null);
      if (!this.onGround && this.DashAttacking && (double) this.DashDir.Y == 0.0 && (this.CollideCheck<Solid>(this.Position + Vector2.UnitY * 3f) || this.CollideCheckOutside<JumpThru>(this.Position + Vector2.UnitY * 3f)))
        this.MoveVExact(3, (Collision) null, (Solid) null);
      if ((double) this.Speed.Y > 0.0 && this.CanUnDuck && (this.Collider != this.starFlyHitbox && !this.onGround) && (double) this.jumpGraceTimer <= 0.0)
        this.Ducking = false;
      if (this.StateMachine.State != 9 && this.StateMachine.State != 22)
        this.MoveH(this.Speed.X * Engine.DeltaTime, this.onCollideH, (Solid) null);
      if (this.StateMachine.State != 9 && this.StateMachine.State != 22)
        this.MoveV(this.Speed.Y * Engine.DeltaTime, this.onCollideV, (Solid) null);
      if (this.StateMachine.State == 3)
      {
        if ((double) this.Speed.Y < 0.0 && (double) this.Speed.Y >= -60.0)
        {
          while (!this.SwimCheck())
          {
            this.Speed.Y = 0.0f;
            if (this.MoveVExact(1, (Collision) null, (Solid) null))
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
          while (this.SwimCheck() && !this.MoveVExact(-1, (Collision) null, (Solid) null));
          if (this.SwimCheck())
            this.StateMachine.State = 3;
        }
        else
          this.StateMachine.State = 3;
      }
      if (this.Sprite.CurrentAnimationID != null && this.Sprite.CurrentAnimationID.Equals("wallslide") && (double) this.Speed.Y > 0.0)
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
          this.level.Camera.Position = position + (cameraTarget - position) * (1f - (float) Math.Pow(0.00999999977648258 / (double) num, (double) Engine.DeltaTime));
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
      if (this.InControl && !this.Dead && this.StateMachine.State != 9)
        this.level.EnforceBounds(this);
      this.UpdateChaserStates();
      this.UpdateHair(true);
      if (this.wasDucking != this.Ducking)
      {
        this.wasDucking = this.Ducking;
        if (this.wasDucking)
          this.Play("event:/char/madeline/duck", (string) null, 0.0f);
        else if (this.onGround)
          this.Play("event:/char/madeline/stand", (string) null, 0.0f);
      }
      if ((double) this.Speed.X != 0.0 && (this.StateMachine.State == 3 && !this.SwimUnderwaterCheck() || this.StateMachine.State == 0 && this.CollideCheck<Water>(this.Position)))
      {
        if (!this.swimSurfaceLoopSfx.Playing)
          this.swimSurfaceLoopSfx.Play("event:/char/madeline/water_move_shallow", (string) null, 0.0f);
      }
      else
        this.swimSurfaceLoopSfx.Stop(true);
      this.wasOnGround = this.onGround;
    }

    private void CreateTrail()
    {
      if (this.Sprite.Mode == PlayerSpriteMode.Badeline)
        TrailManager.Add((Entity) this, Player.NormalBadelineHairColor, 1f);
      else
        TrailManager.Add((Entity) this, this.wasDashB ? Player.NormalHairColor : Player.UsedHairColor, 1f);
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
      while (this.chaserStates.Count > 0 && (double) this.Scene.TimeActive - (double) this.chaserStates[0].TimeStamp > 4.0)
        this.chaserStates.RemoveAt(0);
      this.chaserStates.Add(new Player.ChaserState(this));
      this.activeSounds.Clear();
    }

    private void StartHair()
    {
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
        this.Hair.Color = this.Sprite.Mode != PlayerSpriteMode.Badeline ? Color.Lerp(this.Hair.Color, Player.UsedHairColor, 6f * Engine.DeltaTime) : Color.Lerp(this.Hair.Color, Player.UsedBadelineHairColor, 6f * Engine.DeltaTime);
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
          color = this.Sprite.Mode != PlayerSpriteMode.Badeline ? (this.Dashes != 2 ? Player.NormalHairColor : Player.TwoDashesHairColor) : (this.Dashes != 2 ? Player.NormalBadelineHairColor : Player.TwoDashesBadelineHairColor);
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
      if (this.InControl && this.Sprite.CurrentAnimationID != "throw" && (this.StateMachine.State != 20 && this.StateMachine.State != 18) && this.StateMachine.State != 19 && this.StateMachine.State != 21)
      {
        if (this.StateMachine.State == 22)
          this.Sprite.Play("fallFast", false, false);
        else if (this.StateMachine.State == 10)
          this.Sprite.Play("launch", false, false);
        else if (this.StateMachine.State == 8)
          this.Sprite.Play("pickup", false, false);
        else if (this.StateMachine.State == 3)
        {
          if (Input.MoveY.Value > 0)
            this.Sprite.Play("swimDown", false, false);
          else if (Input.MoveY.Value < 0)
            this.Sprite.Play("swimUp", false, false);
          else
            this.Sprite.Play("swimIdle", false, false);
        }
        else if (this.StateMachine.State == 9)
        {
          if (this.Sprite.CurrentAnimationID != "dreamDashIn" && this.Sprite.CurrentAnimationID != "dreamDashLoop")
            this.Sprite.Play("dreamDashIn", false, false);
        }
        else if (this.Sprite.DreamDashing && this.Sprite.LastAnimationID != "dreamDashOut")
          this.Sprite.Play("dreamDashOut", false, false);
        else if (this.Sprite.CurrentAnimationID != "dreamDashOut")
        {
          if (this.DashAttacking)
          {
            if (this.onGround && (double) this.DashDir.Y == 0.0 && (!this.Ducking && (double) this.Speed.X != 0.0) && this.moveX == -Math.Sign(this.Speed.X))
            {
              if (this.Scene.OnInterval(0.02f))
                Dust.Burst(this.Position, -1.570796f, 1);
              this.Sprite.Play("skid", false, false);
            }
            else
              this.Sprite.Play("dash", false, false);
          }
          else if (this.StateMachine.State == 1)
          {
            if (this.lastClimbMove < 0)
              this.Sprite.Play("climbUp", false, false);
            else if (this.lastClimbMove > 0)
              this.Sprite.Play("wallslide", false, false);
            else if (!this.CollideCheck<Solid>(this.Position + new Vector2((float) this.Facing, 6f)))
              this.Sprite.Play("dangling", false, false);
            else if ((int) Input.MoveX == -(int) this.Facing)
            {
              if (this.Sprite.CurrentAnimationID != "climbLookBack")
                this.Sprite.Play("climbLookBackStart", false, false);
            }
            else
              this.Sprite.Play("wallslide", false, false);
          }
          else if (this.Ducking && this.StateMachine.State == 0)
            this.Sprite.Play("duck", false, false);
          else if (this.onGround)
          {
            this.fastJump = false;
            if (this.Holding == null && this.moveX != 0 && this.CollideCheck<Solid>(this.Position + Vector2.UnitX * (float) this.moveX))
              this.Sprite.Play("push", false, false);
            else if ((double) Math.Abs(this.Speed.X) <= 25.0 && this.moveX == 0)
            {
              if (this.Holding != null)
                this.Sprite.Play("idle_carry", false, false);
              else if (!this.Scene.CollideCheck<Solid>(this.Position + new Vector2((float) this.Facing, 2f)) && !this.Scene.CollideCheck<Solid>(this.Position + new Vector2((float) ((int) this.Facing * 4), 2f)) && !this.CollideCheck<JumpThru>(this.Position + new Vector2((float) ((int) this.Facing * 4), 2f)))
                this.Sprite.Play("edge", false, false);
              else if (!this.Scene.CollideCheck<Solid>(this.Position + new Vector2((float) -(int) this.Facing, 2f)) && !this.Scene.CollideCheck<Solid>(this.Position + new Vector2((float) (-(int) this.Facing * 4), 2f)) && !this.CollideCheck<JumpThru>(this.Position + new Vector2((float) (-(int) this.Facing * 4), 2f)))
                this.Sprite.Play("edgeBack", false, false);
              else if (Input.MoveY.Value == -1)
              {
                if (this.Sprite.LastAnimationID != "lookUp")
                  this.Sprite.Play("lookUp", false, false);
              }
              else if (this.Sprite.CurrentAnimationID != null && !this.Sprite.CurrentAnimationID.Contains("idle"))
                this.Sprite.Play("idle", false, false);
            }
            else if (this.Holding != null)
              this.Sprite.Play("runSlow_carry", false, false);
            else if (Math.Sign(this.Speed.X) == -this.moveX && (uint) this.moveX > 0U)
            {
              if ((double) Math.Abs(this.Speed.X) > 90.0)
                this.Sprite.Play("skid", false, false);
              else if (this.Sprite.CurrentAnimationID != "skid")
                this.Sprite.Play("flip", false, false);
            }
            else if ((double) this.windDirection.X != 0.0 && (double) this.windTimeout > 0.0 && this.Facing == ToFacing.Convert(-Math.Sign(this.windDirection.X)))
              this.Sprite.Play("runWind", false, false);
            else if (!this.Sprite.Running || this.Sprite.CurrentAnimationID == "runWind")
            {
              if ((double) Math.Abs(this.Speed.X) < 45.0)
                this.Sprite.Play("runSlow", false, false);
              else
                this.Sprite.Play("runFast", false, false);
            }
          }
          else if (this.wallSlideDir != 0 && this.Holding == null)
            this.Sprite.Play("wallslide", false, false);
          else if ((double) this.Speed.Y < 0.0)
          {
            if (this.Holding != null)
              this.Sprite.Play("jumpSlow_carry", false, false);
            else if (this.fastJump || (double) Math.Abs(this.Speed.X) > 90.0)
            {
              this.fastJump = true;
              this.Sprite.Play("jumpFast", false, false);
            }
            else
              this.Sprite.Play("jumpSlow", false, false);
          }
          else if (this.Holding != null)
            this.Sprite.Play("fallSlow_carry", false, false);
          else if (this.fastJump || (double) this.Speed.Y >= 160.0 || this.level.InSpace)
          {
            this.fastJump = true;
            if (this.Sprite.LastAnimationID != "fallFast")
              this.Sprite.Play("fallFast", false, false);
          }
          else
            this.Sprite.Play("fallSlow", false, false);
        }
      }
      if (this.StateMachine.State == 11)
        return;
      if (this.level.InSpace)
        this.Sprite.Rate = 0.5f;
      else
        this.Sprite.Rate = 1f;
    }

    public void CreateSplitParticles()
    {
      this.level.Particles.Emit(Player.P_Split, 16, this.Center, Vector2.One * 6f);
    }

    public Vector2 CameraTarget
    {
      get
      {
        Vector2 vector2_1 = new Vector2();
        Vector2 vector2_2 = new Vector2(this.X - 160f, this.Y - 90f);
        if (this.StateMachine.State != 18)
          vector2_2 += new Vector2(this.level.CameraOffset.X, this.level.CameraOffset.Y);
        if (this.StateMachine.State == 19)
        {
          vector2_2.X += 0.2f * this.Speed.X;
          vector2_2.Y += 0.2f * this.Speed.Y;
        }
        else if (this.StateMachine.State == 5)
        {
          vector2_2.X += (float) (48 * Math.Sign(this.Speed.X));
          vector2_2.Y += (float) (48 * Math.Sign(this.Speed.Y));
        }
        else if (this.StateMachine.State == 10)
          vector2_2.Y -= 64f;
        else if (this.StateMachine.State == 18)
          vector2_2.Y += 32f;
        if ((double) this.CameraAnchorLerp.Length() > 0.0)
        {
          if (this.CameraAnchorIgnoreX && !this.CameraAnchorIgnoreY)
            vector2_2.Y = MathHelper.Lerp(vector2_2.Y, this.CameraAnchor.Y, this.CameraAnchorLerp.Y);
          else if (!this.CameraAnchorIgnoreX && this.CameraAnchorIgnoreY)
            vector2_2.X = MathHelper.Lerp(vector2_2.X, this.CameraAnchor.X, this.CameraAnchorLerp.X);
          else if ((double) this.CameraAnchorLerp.X == (double) this.CameraAnchorLerp.Y)
          {
            vector2_2 = Vector2.Lerp(vector2_2, this.CameraAnchor, this.CameraAnchorLerp.X);
          }
          else
          {
            vector2_2.X = MathHelper.Lerp(vector2_2.X, this.CameraAnchor.X, this.CameraAnchorLerp.X);
            vector2_2.Y = MathHelper.Lerp(vector2_2.Y, this.CameraAnchor.Y, this.CameraAnchorLerp.Y);
          }
        }
        vector2_1.X = MathHelper.Clamp(vector2_2.X, (float) this.level.Bounds.Left, (float) (this.level.Bounds.Right - 320));
        vector2_1.Y = MathHelper.Clamp(vector2_2.Y, (float) this.level.Bounds.Top, (float) (this.level.Bounds.Bottom - 180));
        if ((uint) this.level.CameraLockMode > 0U)
        {
          CameraLocker component = this.Scene.Tracker.GetComponent<CameraLocker>();
          if (this.level.CameraLockMode != Level.CameraLockModes.BoostSequence)
          {
            vector2_1.X = Math.Max(vector2_1.X, this.level.Camera.X);
            if (component != null)
              vector2_1.X = Math.Min(vector2_1.X, Math.Max((float) this.level.Bounds.Left, component.Entity.X - component.MaxXOffset));
          }
          if (this.level.CameraLockMode == Level.CameraLockModes.FinalBoss)
          {
            vector2_1.Y = Math.Max(vector2_1.Y, this.level.Camera.Y);
            if (component != null)
              vector2_1.Y = Math.Min(vector2_1.Y, Math.Max((float) this.level.Bounds.Top, component.Entity.Y - component.MaxYOffset));
          }
          else if (this.level.CameraLockMode == Level.CameraLockModes.BoostSequence)
          {
            this.level.CameraUpwardMaxY = Math.Min(this.level.Camera.Y + 180f, this.level.CameraUpwardMaxY);
            vector2_1.Y = Math.Min(vector2_1.Y, this.level.CameraUpwardMaxY);
            if (component != null)
              vector2_1.Y = Math.Max(vector2_1.Y, Math.Min((float) (this.level.Bounds.Bottom - 180), component.Entity.Y - component.MaxYOffset));
          }
        }
        foreach (Entity entity in this.Scene.Tracker.GetEntities<Killbox>())
        {
          if (entity.Collidable && ((double) this.Top < (double) entity.Bottom && (double) this.Right > (double) entity.Left && (double) this.Left < (double) entity.Right))
            vector2_1.Y = Math.Min(vector2_1.Y, entity.Top - 180f);
        }
        return vector2_1;
      }
    }

    public bool GetChasePosition(float sceneTime, float timeAgo, out Player.ChaserState chaseState)
    {
      if (!this.Dead)
      {
        bool flag = false;
        foreach (Player.ChaserState chaserState in this.chaserStates)
        {
          float num = sceneTime - chaserState.TimeStamp;
          if ((double) num <= (double) timeAgo)
          {
            if (flag || (double) timeAgo - (double) num < 0.0199999995529652)
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
        return (uint) (this.StateMachine.State - 11) > 6U;
      }
    }

    public PlayerInventory Inventory
    {
      get
      {
        if (this.level != null && this.level.Session != null)
          return this.level.Session.Inventory;
        return PlayerInventory.Default;
      }
    }

    public void OnTransition()
    {
      this.wallSlideTimer = 1.2f;
      this.jumpGraceTimer = 0.0f;
      this.forceMoveXTimer = 0.0f;
      this.chaserStates.Clear();
      this.RefillDash();
      this.RefillStamina();
      this.Leader.TransferFollowers();
    }

    public bool TransitionTo(Vector2 target, Vector2 direction)
    {
      this.MoveTowardsX(target.X, 60f * Engine.DeltaTime, (Collision) null);
      this.MoveTowardsY(target.Y, 60f * Engine.DeltaTime, (Collision) null);
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

    public bool LoseShards
    {
      get
      {
        return this.onGround;
      }
    }

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
          this.sweatSprite.Play("jump", true, false);
          break;
        case 4:
        case 7:
        case 22:
          this.sweatSprite.Play("jump", true, false);
          break;
        case 5:
        case 9:
          this.Speed = (double) this.Speed.X >= 0.0 && ((double) this.Speed.X != 0.0 || (double) this.Speed.Y >= 0.0) ? this.Speed.Rotate(-0.1745329f) : this.Speed.Rotate(0.1745329f);
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
        case 19:
          this.Speed = (double) this.Speed.X <= 0.0 ? this.Speed.Rotate(-0.6981317f) : this.Speed.Rotate(0.6981317f);
          break;
        case 21:
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
          this.sweatSprite.Play("jump", true, false);
          break;
      }
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Short);
      this.Play(this.Ducking ? "event:/new_content/madeline/hiccup_ducking" : "event:/new_content/madeline/hiccup_standing", (string) null, 0.0f);
    }

    public void Jump(bool particles = true, bool playSfx = true)
    {
      Input.Jump.ConsumeBuffer();
      this.jumpGraceTimer = 0.0f;
      this.varJumpTimer = 0.2f;
      this.AutoJump = false;
      this.dashAttackTimer = 0.0f;
      this.dashGliderBoostTimer = 0.0f;
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
          this.Play("event:/char/madeline/jump_assisted", (string) null, 0.0f);
        if (this.dreamJump)
          this.Play("event:/char/madeline/jump_dreamblock", (string) null, 0.0f);
        else
          this.Play("event:/char/madeline/jump", (string) null, 0.0f);
      }
      this.Sprite.Scale = new Vector2(0.6f, 1.4f);
      if (particles)
        Dust.Burst(this.BottomCenter, -1.570796f, 4);
      ++SaveData.Instance.TotalJumps;
    }

    private void SuperJump()
    {
      Input.Jump.ConsumeBuffer();
      this.jumpGraceTimer = 0.0f;
      this.varJumpTimer = 0.2f;
      this.AutoJump = false;
      this.dashAttackTimer = 0.0f;
      this.dashGliderBoostTimer = 0.0f;
      this.wallSlideTimer = 1.2f;
      this.wallBoostTimer = 0.0f;
      this.Speed.X = 260f * (float) this.Facing;
      this.Speed.Y = -105f;
      this.Speed += this.LiftBoost;
      this.Play("event:/char/madeline/jump", (string) null, 0.0f);
      if (this.Ducking)
      {
        this.Ducking = false;
        this.Speed.X *= 1.25f;
        this.Speed.Y *= 0.5f;
        this.Play("event:/char/madeline/jump_superslide", (string) null, 0.0f);
      }
      else
        this.Play("event:/char/madeline/jump_super", (string) null, 0.0f);
      this.varJumpSpeed = this.Speed.Y;
      this.launched = true;
      this.Sprite.Scale = new Vector2(0.6f, 1.4f);
      Dust.Burst(this.BottomCenter, -1.570796f, 4);
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
      return this.ClimbBoundsCheck(dir) && this.CollideCheck<Solid>(this.Position + Vector2.UnitX * (float) dir * (float) num);
    }

    private void WallJump(int dir)
    {
      this.Ducking = false;
      Input.Jump.ConsumeBuffer();
      this.jumpGraceTimer = 0.0f;
      this.varJumpTimer = 0.2f;
      this.AutoJump = false;
      this.dashAttackTimer = 0.0f;
      this.dashGliderBoostTimer = 0.0f;
      this.wallSlideTimer = 1.2f;
      this.wallBoostTimer = 0.0f;
      this.lowFrictionStopTimer = 0.15f;
      if ((uint) this.moveX > 0U)
      {
        this.forceMoveX = dir;
        this.forceMoveXTimer = 0.16f;
      }
      if (this.LiftSpeed == Vector2.Zero)
      {
        Solid solid = this.CollideFirst<Solid>(this.Position + Vector2.UnitX * 3f);
        if (solid != null)
          this.LiftSpeed = solid.LiftSpeed;
      }
      this.Speed.X = 130f * (float) dir;
      this.Speed.Y = -105f;
      this.Speed += this.LiftBoost;
      this.varJumpSpeed = this.Speed.Y;
      this.LaunchedBoostCheck();
      Platform platformByPriority = SurfaceIndex.GetPlatformByPriority(this.CollideAll<Platform>(this.Position - Vector2.UnitX * (float) dir * 4f, this.temp));
      if (platformByPriority != null)
        this.Play("event:/char/madeline/landing", "surface_index", (float) platformByPriority.GetWallSoundIndex(this, -dir));
      this.Play(dir < 0 ? "event:/char/madeline/jump_wall_right" : "event:/char/madeline/jump_wall_left", (string) null, 0.0f);
      this.Sprite.Scale = new Vector2(0.6f, 1.4f);
      if (dir == -1)
        Dust.Burst(this.Center + Vector2.UnitX * 2f, -2.356194f, 4);
      else
        Dust.Burst(this.Center + Vector2.UnitX * -2f, -0.7853982f, 4);
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
      this.dashGliderBoostTimer = 0.0f;
      this.wallSlideTimer = 1.2f;
      this.wallBoostTimer = 0.0f;
      this.Speed.X = 170f * (float) dir;
      this.Speed.Y = -160f;
      this.Speed += this.LiftBoost;
      this.varJumpSpeed = this.Speed.Y;
      this.launched = true;
      this.Play(dir < 0 ? "event:/char/madeline/jump_wall_right" : "event:/char/madeline/jump_wall_left", (string) null, 0.0f);
      this.Play("event:/char/madeline/jump_superwall", (string) null, 0.0f);
      this.Sprite.Scale = new Vector2(0.6f, 1.4f);
      if (dir == -1)
        Dust.Burst(this.Center + Vector2.UnitX * 2f, -2.356194f, 4);
      else
        Dust.Burst(this.Center + Vector2.UnitX * -2f, -0.7853982f, 4);
      ++SaveData.Instance.TotalWallJumps;
    }

    private void ClimbJump()
    {
      if (!this.onGround)
      {
        this.Stamina -= 27.5f;
        this.sweatSprite.Play("jump", true, false);
        Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
      }
      this.dreamJump = false;
      this.Jump(false, false);
      if (this.moveX == 0)
      {
        this.wallBoostDir = -(int) this.Facing;
        this.wallBoostTimer = 0.2f;
      }
      if (this.Facing == Facings.Right)
      {
        this.Play("event:/char/madeline/jump_climb_right", (string) null, 0.0f);
        Dust.Burst(this.Center + Vector2.UnitX * 2f, -2.356194f, 4);
      }
      else
      {
        this.Play("event:/char/madeline/jump_climb_left", (string) null, 0.0f);
        Dust.Burst(this.Center + Vector2.UnitX * -2f, -0.7853982f, 4);
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
      this.MoveVExact((int) ((double) fromY - (double) this.Bottom), (Collision) null, (Solid) null);
      if (!this.Inventory.NoRefills)
        this.RefillDash();
      this.RefillStamina();
      this.StateMachine.State = 0;
      this.jumpGraceTimer = 0.0f;
      this.varJumpTimer = 0.2f;
      this.AutoJump = true;
      this.AutoJumpTimer = 0.1f;
      this.dashAttackTimer = 0.0f;
      this.dashGliderBoostTimer = 0.0f;
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
      this.MoveV(fromY - this.Bottom, (Collision) null, (Solid) null);
      if (!this.Inventory.NoRefills)
        this.RefillDash();
      this.RefillStamina();
      this.StateMachine.State = 0;
      this.jumpGraceTimer = 0.0f;
      this.varJumpTimer = 0.2f;
      this.AutoJump = true;
      this.AutoJumpTimer = 0.0f;
      this.dashAttackTimer = 0.0f;
      this.dashGliderBoostTimer = 0.0f;
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

    public void SideBounce(int dir, float fromX, float fromY)
    {
      Collider collider = this.Collider;
      this.Collider = (Collider) this.normalHitbox;
      this.MoveV(Calc.Clamp(fromY - this.Bottom, -4f, 4f), (Collision) null, (Solid) null);
      if (dir > 0)
        this.MoveH(fromX - this.Left, (Collision) null, (Solid) null);
      else if (dir < 0)
        this.MoveH(fromX - this.Right, (Collision) null, (Solid) null);
      if (!this.Inventory.NoRefills)
        this.RefillDash();
      this.RefillStamina();
      this.StateMachine.State = 0;
      this.jumpGraceTimer = 0.0f;
      this.varJumpTimer = 0.2f;
      this.AutoJump = true;
      this.AutoJumpTimer = 0.0f;
      this.dashAttackTimer = 0.0f;
      this.dashGliderBoostTimer = 0.0f;
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
      this.dashGliderBoostTimer = 0.0f;
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
      this.dashGliderBoostTimer = 0.0f;
      this.wallSlideTimer = 1.2f;
      this.wallBoostTimer = 0.0f;
      this.launched = false;
      this.dashAttackTimer = 0.0f;
      this.dashGliderBoostTimer = 0.0f;
      this.forceMoveXTimer = 0.0f;
      this.StateMachine.State = 0;
    }

    public int MaxDashes
    {
      get
      {
        if (SaveData.Instance.Assists.DashMode != Assists.DashModes.Normal && !this.level.InCutscene)
          return 2;
        return this.Inventory.Dashes;
      }
    }

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

    public void RefillStamina()
    {
      this.Stamina = 110f;
    }

    public PlayerDeadBody Die(
      Vector2 direction,
      bool evenIfInvincible = false,
      bool registerDeathInStats = true)
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
      if (this.lastBooster != null)
        this.lastBooster.PlayerDied();
      this.level.InCutscene = false;
      this.level.Shake(0.3f);
      Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
      PlayerDeadBody playerDeadBody = new PlayerDeadBody(this, direction);
      if (goldenStrawb != null)
        playerDeadBody.DeathAction = (Action) (() => Engine.Scene = (Scene) new LevelExit(LevelExit.Mode.GoldenBerryRestart, session, (HiresSnow) null)
        {
          GoldenStrawberryEntryLevel = goldenStrawb.ID.Level
        });
      this.Scene.Add((Entity) playerDeadBody);
      this.Scene.Remove((Entity) this);
      Lookout entity = this.Scene.Tracker.GetEntity<Lookout>();
      if (entity != null)
        entity.StopInteracting();
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
      get
      {
        return this.Collider == this.duckHitbox || this.Collider == this.duckHurtbox;
      }
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
        bool flag = !this.CollideCheck<Solid>();
        this.Collider = collider;
        return flag;
      }
    }

    public bool CanUnDuckAt(Vector2 at)
    {
      Vector2 position = this.Position;
      this.Position = at;
      bool canUnDuck = this.CanUnDuck;
      this.Position = position;
      return canUnDuck;
    }

    public bool DuckFreeAt(Vector2 at)
    {
      Vector2 position = this.Position;
      Collider collider = this.Collider;
      this.Position = at;
      this.Collider = (Collider) this.duckHitbox;
      bool flag = !this.CollideCheck<Solid>();
      this.Position = position;
      this.Collider = collider;
      return flag;
    }

    private void Duck()
    {
      this.Collider = (Collider) this.duckHitbox;
    }

    private void UnDuck()
    {
      this.Collider = (Collider) this.normalHitbox;
    }

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

    private void Throw()
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
      }
      this.Play("event:/char/madeline/crystaltheo_throw", (string) null, 0.0f);
      this.Sprite.Play("throw", false, false);
      this.Holding = (Holdable) null;
    }

    private void Drop()
    {
      if (this.Holding == null)
        return;
      Input.Rumble(RumbleStrength.Light, RumbleLength.Short);
      this.Holding.Release(Vector2.Zero);
      this.Holding = (Holdable) null;
    }

    public void StartJumpGraceTime()
    {
      this.jumpGraceTimer = 0.1f;
    }

    public override bool IsRiding(Solid solid)
    {
      if (this.StateMachine.State == 9)
        return this.CollideCheck((Entity) solid);
      if (this.StateMachine.State == 1 || this.StateMachine.State == 6)
        return this.CollideCheck((Entity) solid, this.Position + Vector2.UnitX * (float) this.Facing);
      if ((uint) this.climbTriggerDir > 0U)
        return this.CollideCheck((Entity) solid, this.Position + Vector2.UnitX * (float) this.climbTriggerDir);
      return base.IsRiding(solid);
    }

    public override bool IsRiding(JumpThru jumpThru)
    {
      if (this.StateMachine.State == 9)
        return false;
      return this.StateMachine.State != 1 && (double) this.Speed.Y >= 0.0 && base.IsRiding(jumpThru);
    }

    public bool BounceCheck(float y)
    {
      return (double) this.Bottom <= (double) y + 3.0;
    }

    public void PointBounce(Vector2 from)
    {
      if (this.StateMachine.State == 4 && this.CurrentBooster != null)
        this.CurrentBooster.PlayerReleased();
      this.RefillDash();
      this.RefillStamina();
      Vector2 vector2 = (this.Center - from).SafeNormalize();
      if ((double) vector2.Y > -0.200000002980232 && (double) vector2.Y <= 0.400000005960464)
        vector2.Y = -0.2f;
      this.Speed = vector2 * 220f;
      this.Speed.X *= 1.5f;
      if ((double) Math.Abs(this.Speed.X) >= 100.0)
        return;
      this.Speed.X = (double) this.Speed.X != 0.0 ? (float) Math.Sign(this.Speed.X) * 100f : (float) -(int) this.Facing * 100f;
    }

    private void WindMove(Vector2 move)
    {
      if (this.JustRespawned || (double) this.noWindTimer > 0.0 || (!this.InControl || this.StateMachine.State == 4) || this.StateMachine.State == 2 || this.StateMachine.State == 10)
        return;
      Rectangle bounds;
      if ((double) move.X != 0.0 && this.StateMachine.State != 1)
      {
        this.windTimeout = 0.2f;
        this.windDirection.X = (float) Math.Sign(move.X);
        if (!this.CollideCheck<Solid>(this.Position + Vector2.UnitX * (float) -Math.Sign(move.X) * 3f))
        {
          if (this.Ducking && this.onGround)
            move.X *= 0.0f;
          if ((double) move.X < 0.0)
          {
            ref Vector2 local = ref move;
            double x = (double) move.X;
            bounds = this.level.Bounds;
            double num1 = (double) bounds.Left - ((double) this.ExactPosition.X + (double) this.Collider.Left);
            double num2 = (double) Math.Max((float) x, (float) num1);
            local.X = (float) num2;
          }
          else
          {
            ref Vector2 local = ref move;
            double x = (double) move.X;
            bounds = this.level.Bounds;
            double num1 = (double) bounds.Right - ((double) this.ExactPosition.X + (double) this.Collider.Right);
            double num2 = (double) Math.Min((float) x, (float) num1);
            local.X = (float) num2;
          }
          this.MoveH(move.X, (Collision) null, (Solid) null);
        }
      }
      if ((double) move.Y != 0.0)
      {
        this.windTimeout = 0.2f;
        this.windDirection.Y = (float) Math.Sign(move.Y);
        double bottom = (double) this.Bottom;
        bounds = this.level.Bounds;
        double top = (double) bounds.Top;
        if (bottom > top && ((double) this.Speed.Y < 0.0 || !this.OnGround(1)))
        {
          if (this.StateMachine.State == 1)
          {
            if ((double) move.Y <= 0.0 || (double) this.climbNoMoveTimer > 0.0)
              return;
            move.Y *= 0.4f;
          }
          this.MoveV(move.Y, (Collision) null, (Solid) null);
        }
      }
    }

    private void OnCollideH(CollisionData data)
    {
      this.canCurveDash = false;
      if (this.StateMachine.State == 19)
      {
        if ((double) this.starFlyTimer < 0.200000002980232)
        {
          this.Speed.X = 0.0f;
        }
        else
        {
          this.Play("event:/game/06_reflection/feather_state_bump", (string) null, 0.0f);
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
          if (this.StateMachine.State == 5)
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
                if (!this.CollideCheck<Solid>(this.Position + new Vector2((float) Math.Sign(this.Speed.X), (float) (index1 * index2))))
                {
                  this.MoveVExact(index1 * index2, (Collision) null, (Solid) null);
                  this.MoveHExact(Math.Sign(this.Speed.X), (Collision) null, (Solid) null);
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
          this.dashGliderBoostTimer = 0.0f;
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
          this.dashGliderBoostTimer = 0.0f;
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
        if ((double) this.starFlyTimer < 0.200000002980232)
        {
          this.Speed.Y = 0.0f;
        }
        else
        {
          this.Play("event:/game/06_reflection/feather_state_bump", (string) null, 0.0f);
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
                this.Rebound(0);
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
            if ((double) this.Speed.X <= 0.0)
            {
              for (int moveH = -1; moveH >= -4; --moveH)
              {
                if (!this.OnGround(this.Position + new Vector2((float) moveH, 0.0f), 1))
                {
                  this.MoveHExact(moveH, (Collision) null, (Solid) null);
                  this.MoveVExact(1, (Collision) null, (Solid) null);
                  return;
                }
              }
            }
            if ((double) this.Speed.X >= 0.0)
            {
              for (int moveH = 1; moveH <= 4; ++moveH)
              {
                if (!this.OnGround(this.Position + new Vector2((float) moveH, 0.0f), 1))
                {
                  this.MoveHExact(moveH, (Collision) null, (Solid) null);
                  this.MoveVExact(1, (Collision) null, (Solid) null);
                  return;
                }
              }
            }
          }
          if (this.DreamDashCheck(Vector2.UnitY * (float) Math.Sign(this.Speed.Y)))
          {
            this.StateMachine.State = 9;
            this.dashAttackTimer = 0.0f;
            this.dashGliderBoostTimer = 0.0f;
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
            if ((double) this.Speed.Y >= 80.0)
              Dust.Burst(this.Position, new Vector2(0.0f, -1f).Angle(), 8);
            if ((double) this.highestAirY < (double) this.Y - 50.0 && (double) this.Speed.Y >= 160.0 && (double) Math.Abs(this.Speed.X) >= 90.0)
              this.Sprite.Play("runStumble", false, false);
            Input.Rumble(RumbleStrength.Light, RumbleLength.Short);
            Platform platformByPriority = SurfaceIndex.GetPlatformByPriority(this.CollideAll<Platform>(this.Position + new Vector2(0.0f, 1f), this.temp));
            if (platformByPriority != null)
            {
              int landSoundIndex = platformByPriority.GetLandSoundIndex((Entity) this);
              if (landSoundIndex >= 0)
                this.Play((double) this.playFootstepOnLand > 0.0 ? "event:/char/madeline/footstep" : "event:/char/madeline/landing", "surface_index", (float) landSoundIndex);
              if (platformByPriority is DreamBlock)
                (platformByPriority as DreamBlock).FootstepRipple(this.Position);
            }
            this.playFootstepOnLand = 0.0f;
          }
        }
        else
        {
          if ((double) this.Speed.Y < 0.0)
          {
            int num = 4;
            if (this.DashAttacking && (double) this.Speed.X == 0.0)
              num = 5;
            if ((double) this.Speed.X <= 0.0)
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
            if ((double) this.Speed.X >= 0.0)
            {
              for (int index = 1; index <= num; ++index)
              {
                if (!this.CollideCheck<Solid>(this.Position + new Vector2((float) index, -1f)))
                {
                  this.Position = this.Position + new Vector2((float) index, -1f);
                  return;
                }
              }
            }
            if ((double) this.varJumpTimer < 0.150000005960464)
              this.varJumpTimer = 0.0f;
          }
          if (this.DreamDashCheck(Vector2.UnitY * (float) Math.Sign(this.Speed.Y)))
          {
            this.StateMachine.State = 9;
            this.dashAttackTimer = 0.0f;
            this.dashGliderBoostTimer = 0.0f;
            return;
          }
        }
        if (data.Hit != null && data.Hit.OnCollide != null)
          data.Hit.OnCollide(data.Direction);
        this.dashAttackTimer = 0.0f;
        this.dashGliderBoostTimer = 0.0f;
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
      if (!this.Ducking)
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
      if (!this.TrySquishWiggle(data))
      {
        this.Die(Vector2.Zero, false, true);
      }
      else
      {
        if (!flag || !this.CanUnDuck)
          return;
        this.Ducking = false;
      }
    }

    private void NormalBegin()
    {
      this.maxFall = 160f;
    }

    private void NormalEnd()
    {
      this.wallBoostTimer = 0.0f;
      this.wallSpeedRetentionTimer = 0.0f;
      this.hopWaitX = 0;
    }

    public bool ClimbBoundsCheck(int dir)
    {
      return (double) this.Left + (double) (dir * 2) >= (double) this.level.Bounds.Left && (double) this.Right + (double) (dir * 2) < (double) this.level.Bounds.Right;
    }

    public void ClimbTrigger(int dir)
    {
      this.climbTriggerDir = dir;
    }

    public bool ClimbCheck(int dir, int yAdd = 0)
    {
      return this.ClimbBoundsCheck(dir) && !ClimbBlocker.Check(this.Scene, (Entity) this, this.Position + Vector2.UnitY * (float) yAdd + Vector2.UnitX * 2f * (float) this.Facing) && this.CollideCheck<Solid>(this.Position + new Vector2((float) (dir * 2), (float) yAdd));
    }

    private int NormalUpdate()
    {
      if ((double) this.LiftBoost.Y < 0.0 && this.wasOnGround && !this.onGround && (double) this.Speed.Y >= 0.0)
        this.Speed.Y = this.LiftBoost.Y;
      if (this.Holding == null)
      {
        if (Input.Grab.Check && !this.IsTired && !this.Ducking)
        {
          foreach (Holdable component in this.Scene.Tracker.GetComponents<Holdable>())
          {
            if (component.Check(this) && this.Pickup(component))
              return 8;
          }
          if ((double) this.Speed.Y >= 0.0 && Math.Sign(this.Speed.X) != -(int) this.Facing)
          {
            if (this.ClimbCheck((int) this.Facing, 0))
            {
              this.Ducking = false;
              if (!SaveData.Instance.Assists.NoGrabbing)
                return 1;
              this.ClimbTrigger((int) this.Facing);
            }
            if (!SaveData.Instance.Assists.NoGrabbing && (int) Input.MoveY < 1 && (double) this.level.Wind.Y <= 0.0)
            {
              for (int index = 1; index <= 2; ++index)
              {
                if (!this.CollideCheck<Solid>(this.Position + Vector2.UnitY * (float) -index) && this.ClimbCheck((int) this.Facing, -index))
                {
                  this.MoveVExact(-index, (Collision) null, (Solid) null);
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
          if (this.onGround && (int) Input.MoveY != 1)
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
                  this.MoveH(50f * Engine.DeltaTime, (Collision) null, (Solid) null);
                  break;
                }
                if (this.CanUnDuckAt(this.Position - Vector2.UnitX * (float) index))
                {
                  this.MoveH(-50f * Engine.DeltaTime, (Collision) null, (Solid) null);
                  break;
                }
              }
            }
          }
        }
        else if (this.onGround && (int) Input.MoveY == 1 && (double) this.Speed.Y >= 0.0)
        {
          this.Ducking = true;
          this.Sprite.Scale = new Vector2(1.4f, 0.6f);
        }
      }
      else
      {
        if (!Input.Grab.Check && (double) this.minHoldTimer <= 0.0)
          this.Throw();
        if (!this.Ducking && this.onGround && (int) Input.MoveY == 1 && (double) this.Speed.Y >= 0.0)
        {
          this.Drop();
          this.Ducking = true;
          this.Sprite.Scale = new Vector2(1.4f, 0.6f);
        }
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
        float num2 = this.Holding == null || !this.Holding.SlowRun ? 90f : 70f;
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
      if ((int) Input.MoveY == 1 && (double) this.Speed.Y >= (double) target1)
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
        if (((Facings) this.moveX == this.Facing || this.moveX == 0 && Input.Grab.Check) && Input.MoveY.Value != 1)
        {
          if ((double) this.Speed.Y >= 0.0 && (double) this.wallSlideTimer > 0.0 && (this.Holding == null && this.ClimbBoundsCheck((int) this.Facing)) && this.CollideCheck<Solid>(this.Position + Vector2.UnitX * (float) this.Facing) && this.CanUnDuck)
          {
            this.Ducking = false;
            this.wallSlideDir = (int) this.Facing;
          }
          if ((uint) this.wallSlideDir > 0U)
          {
            if (Input.Grab.Check)
              this.ClimbTrigger(this.wallSlideDir);
            if ((double) this.wallSlideTimer > 0.600000023841858 && ClimbBlocker.Check((Scene) this.level, (Entity) this, this.Position + Vector2.UnitX * (float) this.wallSlideDir))
              this.wallSlideTimer = 0.6f;
            target3 = MathHelper.Lerp(160f, 20f, this.wallSlideTimer / 1.2f);
            if ((double) this.wallSlideTimer / 1.20000004768372 > 0.649999976158142)
              this.CreateWallSlideParticles(this.wallSlideDir);
          }
        }
        float num = (double) Math.Abs(this.Speed.Y) >= 40.0 || !Input.Jump.Check && !this.AutoJump ? 1f : 0.5f;
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
      if (Input.Jump.Pressed)
      {
        if ((double) this.jumpGraceTimer > 0.0)
          this.Jump(true, true);
        else if (this.CanUnDuck)
        {
          bool canUnDuck = this.CanUnDuck;
          if (canUnDuck && this.WallJumpCheck(1))
          {
            if (this.Facing == Facings.Right && Input.Grab.Check && (!SaveData.Instance.Assists.NoGrabbing && (double) this.Stamina > 0.0) && this.Holding == null && !ClimbBlocker.Check(this.Scene, (Entity) this, this.Position + Vector2.UnitX * 3f))
              this.ClimbJump();
            else if (this.DashAttacking && this.SuperWallJumpAngleCheck)
              this.SuperWallJump(-1);
            else
              this.WallJump(-1);
          }
          else if (canUnDuck && this.WallJumpCheck(-1))
          {
            if (this.Facing == Facings.Left && Input.Grab.Check && (!SaveData.Instance.Assists.NoGrabbing && (double) this.Stamina > 0.0) && this.Holding == null && !ClimbBlocker.Check(this.Scene, (Entity) this, this.Position + Vector2.UnitX * -3f))
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
              this.Jump(true, true);
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
      Vector2 center = this.Center;
      Dust.Burst(dir != 1 ? center + new Vector2(-5f, 4f) : center + new Vector2(5f, 4f), -1.570796f, 1);
    }

    private bool IsTired
    {
      get
      {
        return (double) this.CheckStamina < 20.0;
      }
    }

    private float CheckStamina
    {
      get
      {
        if ((double) this.wallBoostTimer > 0.0)
          return this.Stamina + 27.5f;
        return this.Stamina;
      }
    }

    private void PlaySweatEffectDangerOverride(string state)
    {
      if ((double) this.Stamina <= 20.0)
        this.sweatSprite.Play("danger", false, false);
      else
        this.sweatSprite.Play(state, false, false);
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
      this.sweatSprite.Play("idle", false, false);
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
      if (!Input.Grab.Check)
      {
        this.Speed += this.LiftBoost;
        this.Play("event:/char/madeline/grab_letgo", (string) null, 0.0f);
        return 0;
      }
      if (!this.CollideCheck<Solid>(this.Position + Vector2.UnitX * (float) this.Facing))
      {
        if ((double) this.Speed.Y < 0.0)
        {
          if (this.wallBoosting)
          {
            this.Speed += this.LiftBoost;
            this.Play("event:/char/madeline/grab_letgo", (string) null, 0.0f);
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
            else if (this.SlipCheck(0.0f))
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
        if (flag && this.SlipCheck(0.0f))
          target = 30f;
        this.Speed.Y = Calc.Approach(this.Speed.Y, target, 900f * Engine.DeltaTime);
      }
      if (Input.MoveY.Value != 1 && (double) this.Speed.Y > 0.0 && !this.CollideCheck<Solid>(this.Position + new Vector2((float) this.Facing, 1f)))
        this.Speed.Y = 0.0f;
      if ((double) this.climbNoMoveTimer <= 0.0)
      {
        if (this.lastClimbMove == -1)
        {
          this.Stamina -= 45.45454f * Engine.DeltaTime;
          if ((double) this.Stamina <= 20.0)
            this.sweatSprite.Play("danger", false, false);
          else if (this.sweatSprite.CurrentAnimationID != "climbLoop")
            this.sweatSprite.Play("climb", false, false);
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
      this.Play("event:/char/madeline/climb_ledge", (string) null, 0.0f);
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

    public int StartDash()
    {
      this.wasDashB = this.Dashes == 2;
      this.Dashes = Math.Max(0, this.Dashes - 1);
      Input.Dash.ConsumeBuffer();
      return 2;
    }

    public bool DashAttacking
    {
      get
      {
        return (double) this.dashAttackTimer > 0.0 || this.StateMachine.State == 5;
      }
    }

    public bool CanDash
    {
      get
      {
        return Input.Dash.Pressed && (double) this.dashCooldownTimer <= 0.0 && this.Dashes > 0 && (TalkComponent.PlayerOver == null || !Input.Talk.Pressed);
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
        Stats.Increment(Stat.DASHES, 1);
        bool flag = (double) this.DashDir.Y < 0.0 || (double) this.DashDir.Y == 0.0 && (double) this.DashDir.X > 0.0;
        if (this.DashDir == Vector2.Zero)
          flag = this.Facing == Facings.Right;
        if (flag)
        {
          if (this.wasDashB)
            this.Play("event:/char/madeline/dash_pink_right", (string) null, 0.0f);
          else
            this.Play("event:/char/madeline/dash_red_right", (string) null, 0.0f);
        }
        else if (this.wasDashB)
          this.Play("event:/char/madeline/dash_pink_left", (string) null, 0.0f);
        else
          this.Play("event:/char/madeline/dash_red_left", (string) null, 0.0f);
        if (this.SwimCheck())
          this.Play("event:/char/madeline/water_dash_gen", (string) null, 0.0f);
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
      this.level.Displacement.AddBurst(this.Center, 0.4f, 8f, 64f, 0.5f, Ease.QuadOut, Ease.QuadOut);
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
      this.dashAttackTimer = 0.3f;
      if (SaveData.Instance.Assists.SuperDashing)
        this.dashAttackTimer += 0.15f;
      this.beforeDashSpeed = this.Speed;
      this.Speed = Vector2.Zero;
      this.DashDir = Vector2.Zero;
      if (!this.onGround && this.Ducking && this.CanUnDuck)
      {
        this.Ducking = false;
      }
      else
      {
        if (this.Ducking || Input.MoveY.Value != 1)
          return;
        this.Ducking = true;
      }
    }

    private void DashEnd()
    {
      this.CallDashEvents();
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
        Vector2 aimVector = Input.GetAimVector(Facings.Right);
        if ((double) Vector2.Dot(aimVector, this.Speed.SafeNormalize()) >= -0.100000001490116)
        {
          this.Speed = this.Speed.RotateTowards(aimVector.Angle(), 4.18879f * Engine.DeltaTime);
          this.DashDir = this.Speed.SafeNormalize();
        }
      }
      if (SaveData.Instance.Assists.SuperDashing && this.CanDash)
      {
        this.StartDash();
        this.StateMachine.ForceState(2);
        return 2;
      }
      if (this.Holding == null && Input.Grab.Check && !this.IsTired && this.CanUnDuck)
      {
        foreach (Holdable component in this.Scene.Tracker.GetComponents<Holdable>())
        {
          if (component.Check(this) && this.Pickup(component))
            return 8;
        }
      }
      if ((double) this.DashDir.Y == 0.0)
      {
        foreach (JumpThru entity in this.Scene.Tracker.GetEntities<JumpThru>())
        {
          if (this.CollideCheck((Entity) entity) && (double) this.Bottom - (double) entity.Top <= 6.0)
            this.MoveVExact((int) ((double) entity.Top - (double) this.Bottom), (Collision) null, (Solid) null);
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
          if (this.Facing == Facings.Right && Input.Grab.Check && ((double) this.Stamina > 0.0 && this.Holding == null) && !ClimbBlocker.Check(this.Scene, (Entity) this, this.Position + Vector2.UnitX * 3f))
            this.ClimbJump();
          else
            this.WallJump(-1);
          return 0;
        }
        if (this.WallJumpCheck(-1))
        {
          if (this.Facing == Facings.Left && Input.Grab.Check && ((double) this.Stamina > 0.0 && this.Holding == null) && !ClimbBlocker.Check(this.Scene, (Entity) this, this.Position + Vector2.UnitX * -3f))
            this.ClimbJump();
          else
            this.WallJump(1);
          return 0;
        }
      }
      if (this.Speed != Vector2.Zero && this.level.OnInterval(0.02f))
        this.level.ParticlesFG.Emit(this.wasDashB ? Player.P_DashB : Player.P_DashA, this.Center + Calc.Random.Range(Vector2.One * -2f, Vector2.One * 2f), this.DashDir.Angle());
      return 2;
    }

    private bool SuperWallJumpAngleCheck
    {
      get
      {
        return (double) Math.Abs(this.DashDir.X) <= 0.200000002980232 && (double) this.DashDir.Y <= -0.75;
      }
    }

    private IEnumerator DashCoroutine()
    {
      yield return (object) null;
      Vector2 dir = this.lastAim;
      if (this.OverrideDashDirection.HasValue)
        dir = this.OverrideDashDirection.Value;
      Vector2 newSpeed = dir * 240f;
      if (Math.Sign(this.beforeDashSpeed.X) == Math.Sign(newSpeed.X) && (double) Math.Abs(this.beforeDashSpeed.X) > (double) Math.Abs(newSpeed.X))
        newSpeed.X = this.beforeDashSpeed.X;
      this.Speed = newSpeed;
      if (this.CollideCheck<Water>())
        this.Speed *= 0.75f;
      this.DashDir = dir;
      this.SceneAs<Level>().DirectionalShake(this.DashDir, 0.2f);
      if ((double) this.DashDir.X != 0.0)
        this.Facing = (Facings) Math.Sign(this.DashDir.X);
      this.CallDashEvents();
      if (this.StateMachine.PreviousState == 19)
        this.level.Particles.Emit(FlyFeather.P_Boost, 12, this.Center, Vector2.One * 4f, (-dir).Angle());
      if (this.onGround && (double) this.DashDir.X != 0.0 && ((double) this.DashDir.Y > 0.0 && (double) this.Speed.Y > 0.0) && (!this.Inventory.DreamDash || !this.CollideCheck<DreamBlock>(this.Position + Vector2.UnitY)))
      {
        this.DashDir.X = (float) Math.Sign(this.DashDir.X);
        this.DashDir.Y = 0.0f;
        this.Speed.Y = 0.0f;
        this.Speed.X *= 1.2f;
        this.Ducking = true;
      }
      SlashFx.Burst(this.Center, this.DashDir.Angle());
      this.CreateTrail();
      if (SaveData.Instance.Assists.SuperDashing)
      {
        this.dashTrailTimer = 0.1f;
        this.dashTrailCounter = 2;
      }
      else
      {
        this.dashTrailTimer = 0.08f;
        this.dashTrailCounter = 1;
      }
      if ((double) this.DashDir.X != 0.0 && Input.Grab.Check)
      {
        SwapBlock swapBlock = this.CollideFirst<SwapBlock>(this.Position + Vector2.UnitX * (float) Math.Sign(this.DashDir.X));
        if (swapBlock != null && (double) swapBlock.Direction.X == (double) Math.Sign(this.DashDir.X))
        {
          this.StateMachine.State = 1;
          this.Speed = Vector2.Zero;
          yield break;
        }
        else
          swapBlock = (SwapBlock) null;
      }
      Vector2 swapCancel = Vector2.One;
      foreach (SwapBlock entity in this.Scene.Tracker.GetEntities<SwapBlock>())
      {
        SwapBlock swapBlock = entity;
        if (this.CollideCheck((Entity) swapBlock, this.Position + Vector2.UnitY) && (swapBlock != null && swapBlock.Swapping))
        {
          if ((double) this.DashDir.X != 0.0 && (double) swapBlock.Direction.X == (double) Math.Sign(this.DashDir.X))
            this.Speed.X = swapCancel.X = 0.0f;
          if ((double) this.DashDir.Y != 0.0 && (double) swapBlock.Direction.Y == (double) Math.Sign(this.DashDir.Y))
            this.Speed.Y = swapCancel.Y = 0.0f;
        }
        swapBlock = (SwapBlock) null;
      }
      if (SaveData.Instance.Assists.SuperDashing)
        yield return (object) 0.3f;
      else
        yield return (object) 0.15f;
      this.CreateTrail();
      this.AutoJump = true;
      this.AutoJumpTimer = 0.0f;
      if ((double) this.DashDir.Y <= 0.0)
      {
        this.Speed = this.DashDir * 160f;
        this.Speed.X *= swapCancel.X;
        this.Speed.Y *= swapCancel.Y;
      }
      if ((double) this.Speed.Y < 0.0)
        this.Speed.Y *= 0.75f;
      this.StateMachine.State = 0;
    }

    private bool SwimCheck()
    {
      return this.CollideCheck<Water>(this.Position + Vector2.UnitY * -8f) && this.CollideCheck<Water>(this.Position);
    }

    private bool SwimUnderwaterCheck()
    {
      return this.CollideCheck<Water>(this.Position + Vector2.UnitY * -9f);
    }

    private bool SwimJumpCheck()
    {
      return !this.CollideCheck<Water>(this.Position + Vector2.UnitY * -14f);
    }

    private bool SwimRiseCheck()
    {
      return !this.CollideCheck<Water>(this.Position + Vector2.UnitY * -18f);
    }

    private bool UnderwaterMusicCheck()
    {
      return this.CollideCheck<Water>(this.Position) && this.CollideCheck<Water>(this.Position + Vector2.UnitY * -12f);
    }

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
        Input.Dash.ConsumeBuffer();
        return 2;
      }
      bool flag = this.SwimUnderwaterCheck();
      if (!flag && (double) this.Speed.Y >= 0.0 && (Input.Grab.Check && !this.IsTired) && this.CanUnDuck && (Math.Sign(this.Speed.X) != -(int) this.Facing && this.ClimbCheck((int) this.Facing, 0)))
      {
        if (SaveData.Instance.Assists.NoGrabbing)
          this.ClimbTrigger((int) this.Facing);
        else if (!this.MoveVExact(-1, (Collision) null, (Solid) null))
        {
          this.Ducking = false;
          return 1;
        }
      }
      Vector2 vector2 = Input.Aim.Value.SafeNormalize();
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
      this.Jump(true, true);
      return 0;
    }

    public void Boost(Booster booster)
    {
      this.StateMachine.State = 4;
      this.Speed = Vector2.Zero;
      this.boostTarget = booster.Center;
      this.boostRed = false;
      this.lastBooster = this.CurrentBooster = booster;
    }

    public void RedBoost(Booster booster)
    {
      this.StateMachine.State = 4;
      this.Speed = Vector2.Zero;
      this.boostTarget = booster.Center;
      this.boostRed = true;
      this.lastBooster = this.CurrentBooster = booster;
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
      this.MoveToX(vector2.X, (Collision) null);
      this.MoveToY(vector2.Y, (Collision) null);
    }

    private int BoostUpdate()
    {
      Vector2 vector2 = Calc.Approach(this.ExactPosition, this.boostTarget - this.Collider.Center + Input.Aim.Value * 3f, 80f * Engine.DeltaTime);
      this.MoveToX(vector2.X, (Collision) null);
      this.MoveToY(vector2.Y, (Collision) null);
      if (!Input.Dash.Pressed)
        return 4;
      Input.Dash.ConsumePress();
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
      this.Speed = Vector2.Zero;
      if (this.onGround || !this.Ducking || !this.CanUnDuck)
        return;
      this.Ducking = false;
    }

    private void RedDashEnd()
    {
      this.CallDashEvents();
    }

    private int RedDashUpdate()
    {
      this.StartedDashing = false;
      if (this.CanDash)
        return this.StartDash();
      if ((double) this.DashDir.Y == 0.0)
      {
        foreach (JumpThru entity in this.Scene.Tracker.GetEntities<JumpThru>())
        {
          if (this.CollideCheck((Entity) entity) && (double) this.Bottom - (double) entity.Top <= 6.0)
            this.MoveVExact((int) ((double) entity.Top - (double) this.Bottom), (Collision) null, (Solid) null);
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
          if (this.Facing == Facings.Right && Input.Grab.Check && ((double) this.Stamina > 0.0 && this.Holding == null) && !ClimbBlocker.Check(this.Scene, (Entity) this, this.Position + Vector2.UnitX * 3f))
            this.ClimbJump();
          else
            this.WallJump(-1);
          return 0;
        }
        if (this.WallJumpCheck(-1))
        {
          if (this.Facing == Facings.Left && Input.Grab.Check && ((double) this.Stamina > 0.0 && this.Holding == null) && !ClimbBlocker.Check(this.Scene, (Entity) this, this.Position + Vector2.UnitX * -3f))
            this.ClimbJump();
          else
            this.WallJump(1);
          return 0;
        }
      }
      return 5;
    }

    private IEnumerator RedDashCoroutine()
    {
      yield return (object) null;
      this.Speed = this.lastAim * 240f;
      this.DashDir = this.lastAim;
      this.SceneAs<Level>().DirectionalShake(this.DashDir, 0.2f);
      if ((double) this.DashDir.X != 0.0)
        this.Facing = (Facings) Math.Sign(this.DashDir.X);
      this.CallDashEvents();
    }

    private void HitSquashBegin()
    {
      this.hitSquashNoMoveTimer = 0.1f;
    }

    private int HitSquashUpdate()
    {
      this.Speed.X = Calc.Approach(this.Speed.X, 0.0f, 800f * Engine.DeltaTime);
      this.Speed.Y = Calc.Approach(this.Speed.Y, 0.0f, 800f * Engine.DeltaTime);
      if (Input.Jump.Pressed)
      {
        if (this.onGround)
          this.Jump(true, true);
        else if (this.WallJumpCheck(1))
        {
          if (this.Facing == Facings.Right && Input.Grab.Check && ((double) this.Stamina > 0.0 && this.Holding == null) && !ClimbBlocker.Check(this.Scene, (Entity) this, this.Position + Vector2.UnitX * 3f))
            this.ClimbJump();
          else
            this.WallJump(-1);
        }
        else if (this.WallJumpCheck(-1))
        {
          if (this.Facing == Facings.Left && Input.Grab.Check && ((double) this.Stamina > 0.0 && this.Holding == null) && !ClimbBlocker.Check(this.Scene, (Entity) this, this.Position + Vector2.UnitX * -3f))
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
      if (Input.Grab.Check && this.ClimbCheck((int) this.Facing, 0))
        return 1;
      if ((double) this.hitSquashNoMoveTimer <= 0.0)
        return 0;
      this.hitSquashNoMoveTimer -= Engine.DeltaTime;
      return 6;
    }

    public Vector2 ExplodeLaunch(Vector2 from, bool snapUp = true)
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
      else if ((double) num <= 0.550000011920929 && (double) num >= -0.550000011920929)
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
      if (Input.MoveX.Value == Math.Sign(this.Speed.X))
        this.Speed.X *= 1.2f;
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
      this.Speed.X = (float) (0.899999976158142 * (double) dir * 280.0);
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
      SlashFx.Burst(this.Center, this.Speed.Angle());
      this.RefillDash();
      this.RefillStamina();
      this.dashCooldownTimer = 0.2f;
      this.StateMachine.State = 7;
    }

    private void LaunchBegin()
    {
      this.launched = true;
    }

    private int LaunchUpdate()
    {
      if (this.launchApproachX.HasValue)
        this.MoveTowardsX(this.launchApproachX.Value, 60f * Engine.DeltaTime, (Collision) null);
      if (this.CanDash)
        return this.StartDash();
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
      this.Sprite.Play("launch", false, false);
      this.Speed = -Vector2.UnitY * 240f;
      this.summitLaunchParticleTimer = 0.4f;
    }

    private int SummitLaunchUpdate()
    {
      this.summitLaunchParticleTimer -= Engine.DeltaTime;
      if ((double) this.summitLaunchParticleTimer > 0.0 && this.Scene.OnInterval(0.03f))
        this.level.ParticlesFG.Emit(BadelineBoost.P_Move, 1, this.Center, Vector2.One * 4f);
      this.Facing = Facings.Right;
      this.MoveTowardsX(this.summitLaunchTargetX, 20f * Engine.DeltaTime, (Collision) null);
      this.Speed = -Vector2.UnitY * 240f;
      if (this.level.OnInterval(0.2f))
        this.level.Add((Entity) Engine.Pooler.Create<SpeedRing>().Init(this.Center, 1.570796f, Color.White));
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
      this.Play("event:/char/madeline/crystaltheo_lift", (string) null, 0.0f);
      Input.Rumble(RumbleStrength.Medium, RumbleLength.Short);
      Vector2 oldSpeed = this.Speed;
      float varJump = this.varJumpTimer;
      this.Speed = Vector2.Zero;
      Vector2 begin = this.Holding.Entity.Position - this.Position;
      Vector2 end = Player.CarryOffsetTarget;
      Vector2 control = new Vector2(begin.X + (float) (Math.Sign(begin.X) * 2), Player.CarryOffsetTarget.Y - 2f);
      SimpleCurve curve = new SimpleCurve(begin, end, control);
      this.carryOffset = begin;
      Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeInOut, 0.16f, true);
      tween.OnUpdate = (Action<Tween>) (t => this.carryOffset = curve.GetPoint(t.Eased));
      this.Add((Component) tween);
      yield return (object) tween.Wait();
      this.Speed = oldSpeed;
      this.Speed.Y = Math.Min(this.Speed.Y, 0.0f);
      this.varJumpTimer = varJump;
      this.StateMachine.State = 0;
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
      this.Play("event:/char/madeline/dreamblock_enter", (string) null, 0.0f);
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
      this.Play("event:/char/madeline/dreamblock_exit", (string) null, 0.0f);
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
            this.Play("event:/game/general/assist_dreamblockbounce", (string) null, 0.0f);
          }
          else
            this.Die(Vector2.Zero, false, true);
        }
        else if ((double) this.dreamDashCanEndTimer <= 0.0)
        {
          Celeste.Freeze(0.05f);
          if (Input.Jump.Pressed && (double) this.DashDir.X != 0.0)
          {
            this.dreamJump = true;
            this.Jump(true, true);
          }
          else
          {
            bool flag1 = this.ClimbCheck(-1, 0);
            bool flag2 = this.ClimbCheck(1, 0);
            if (Input.Grab.Check && ((double) this.DashDir.Y >= 0.0 || (double) this.DashDir.X != 0.0) && (this.moveX == 1 & flag2 || this.moveX == -1 & flag1))
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
          DisplacementRenderer.Burst burst = this.level.Displacement.AddBurst(this.Center, 0.3f, 0.0f, 40f, 1f, (Ease.Easer) null, (Ease.Easer) null);
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
      this.Sprite.Play("startStarFly", false, false);
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
      this.starFlyWarningSfx.Stop(true);
    }

    private void StarFlyEnd()
    {
      this.Play("event:/game/06_reflection/feather_state_end", (string) null, 0.0f);
      this.starFlyWarningSfx.Stop(true);
      this.starFlyLoopSfx.Stop(true);
      this.Hair.DrawPlayerSpriteOutline = false;
      this.Sprite.Color = Color.White;
      this.level.Displacement.AddBurst(this.Center, 0.25f, 8f, 32f, 1f, (Ease.Easer) null, (Ease.Easer) null);
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
      while (this.Sprite.CurrentAnimationID == "startStarFly")
        yield return (object) null;
      while (this.Speed != Vector2.Zero)
        yield return (object) null;
      yield return (object) 0.1f;
      this.Sprite.Color = this.starFlyColor;
      this.Sprite.HairCount = 7;
      this.Hair.DrawPlayerSpriteOutline = true;
      this.level.Displacement.AddBurst(this.Center, 0.25f, 8f, 32f, 1f, (Ease.Easer) null, (Ease.Easer) null);
      this.starFlyTransforming = false;
      this.starFlyTimer = 2f;
      this.RefillDash();
      this.RefillStamina();
      Vector2 dir = Input.Aim.Value;
      if (dir == Vector2.Zero)
        dir = Vector2.UnitX * (float) this.Facing;
      this.Speed = dir * 250f;
      this.starFlyLastDir = dir;
      this.level.Particles.Emit(FlyFeather.P_Boost, 12, this.Center, Vector2.One * 4f, (-dir).Angle());
      dir = new Vector2();
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
      this.level.DirectionalShake(this.starFlyLastDir, 0.3f);
      while ((double) this.starFlyTimer > 0.5)
        yield return (object) null;
      this.starFlyWarningSfx.Play("event:/game/06_reflection/feather_state_warning", (string) null, 0.0f);
    }

    private int StarFlyUpdate()
    {
      this.starFlyBloom.Alpha = Calc.Approach(this.starFlyBloom.Alpha, 0.7f, Engine.DeltaTime * 2f);
      if (this.starFlyTransforming)
      {
        this.Speed = Calc.Approach(this.Speed, Vector2.Zero, 1000f * Engine.DeltaTime);
      }
      else
      {
        Vector2 starFlyLastDir = Input.Aim.Value;
        bool flag1 = false;
        if (starFlyLastDir == Vector2.Zero)
        {
          flag1 = true;
          starFlyLastDir = this.starFlyLastDir;
        }
        Vector2 vec = this.Speed.SafeNormalize(Vector2.Zero);
        Vector2 vector2 = !(vec == Vector2.Zero) ? vec.RotateTowards(starFlyLastDir.Angle(), 5.585053f * Engine.DeltaTime) : starFlyLastDir;
        this.starFlyLastDir = vector2;
        float target;
        if (flag1)
        {
          this.starFlySpeedLerp = 0.0f;
          target = 91f;
        }
        else if (vector2 != Vector2.Zero && (double) Vector2.Dot(vector2, starFlyLastDir) >= 0.449999988079071)
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
            this.Jump(true, true);
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
        if (Input.Grab.Check)
        {
          bool flag2 = false;
          int dir = 0;
          if (Input.MoveX.Value != -1 && this.ClimbCheck(1, 0))
          {
            this.Facing = Facings.Right;
            dir = 1;
            flag2 = true;
          }
          else if (Input.MoveX.Value != 1 && this.ClimbCheck(-1, 0))
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
      this.Sprite.Play("bubble", false, false);
      this.Sprite.Y += 5f;
    }

    private void CassetteFlyEnd()
    {
    }

    private int CassetteFlyUpdate()
    {
      return 21;
    }

    private IEnumerator CassetteFlyCoroutine()
    {
      this.level.CanRetry = false;
      this.level.FormationBackdrop.Display = true;
      this.level.FormationBackdrop.Alpha = 0.5f;
      this.Sprite.Scale = Vector2.One * 1.25f;
      this.Depth = -2000000;
      yield return (object) 0.4f;
      while ((double) this.cassetteFlyLerp < 1.0)
      {
        if (this.level.OnInterval(0.03f))
          this.level.Particles.Emit(Player.P_CassetteFly, 2, this.Center, Vector2.One * 4f);
        this.cassetteFlyLerp = Calc.Approach(this.cassetteFlyLerp, 1f, 1.6f * Engine.DeltaTime);
        this.Position = this.cassetteFlyCurve.GetPoint(Ease.SineInOut(this.cassetteFlyLerp));
        this.level.Camera.Position = this.CameraTarget;
        yield return (object) null;
      }
      this.Position = this.cassetteFlyCurve.End;
      this.Sprite.Scale = Vector2.One * 1.25f;
      this.Sprite.Y -= 5f;
      this.Sprite.Play("fallFast", false, false);
      yield return (object) 0.2f;
      this.level.CanRetry = true;
      this.level.FormationBackdrop.Display = false;
      this.level.FormationBackdrop.Alpha = 0.5f;
      this.StateMachine.State = 0;
      this.Depth = 0;
    }

    public void StartAttract(Vector2 attractTo)
    {
      this.attractTo = attractTo.Round();
      this.StateMachine.State = 22;
    }

    private void AttractBegin()
    {
      this.Speed = Vector2.Zero;
    }

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
        this.MoveToX(vector2.X, (Collision) null);
        this.MoveToY(vector2.Y, (Collision) null);
      }
      return 22;
    }

    public bool AtAttractTarget
    {
      get
      {
        return this.StateMachine.State == 22 && this.ExactPosition == this.attractTo;
      }
    }

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
            this.Sprite.Play("idle", false, false);
          else
            this.Sprite.Play("walk", false, false);
        }
        else if ((double) this.Speed.Y < 0.0)
          this.Sprite.Play("jumpSlow", false, false);
        else
          this.Sprite.Play("fallSlow", false, false);
      }
      return 11;
    }

    public IEnumerator DummyWalkTo(
      float x,
      bool walkBackwards = false,
      float speedMultiplier = 1f,
      bool keepWalkingIntoWalls = false)
    {
      this.StateMachine.State = 11;
      if ((double) Math.Abs(this.X - x) > 4.0 && !this.Dead)
      {
        this.DummyMoving = true;
        if (walkBackwards)
        {
          this.Sprite.Rate = -1f;
          this.Facing = (Facings) Math.Sign(this.X - x);
        }
        else
          this.Facing = (Facings) Math.Sign(x - this.X);
        while ((double) Math.Abs(x - this.X) > 4.0 && this.Scene != null && (keepWalkingIntoWalls || !this.CollideCheck<Solid>(this.Position + Vector2.UnitX * (float) Math.Sign(x - this.X))))
        {
          this.Speed.X = Calc.Approach(this.Speed.X, (float) Math.Sign(x - this.X) * 64f * speedMultiplier, 1000f * Engine.DeltaTime);
          yield return (object) null;
        }
        this.Sprite.Rate = 1f;
        this.Sprite.Play("idle", false, false);
        this.DummyMoving = false;
      }
    }

    public IEnumerator DummyWalkToExact(
      int x,
      bool walkBackwards = false,
      float speedMultiplier = 1f)
    {
      this.StateMachine.State = 11;
      if ((double) this.X != (double) x)
      {
        this.DummyMoving = true;
        if (walkBackwards)
        {
          this.Sprite.Rate = -1f;
          this.Facing = (Facings) Math.Sign(this.X - (float) x);
        }
        else
          this.Facing = (Facings) Math.Sign((float) x - this.X);
        int last = Math.Sign(this.X - (float) x);
        while ((double) this.X != (double) x && !this.CollideCheck<Solid>(this.Position + new Vector2((float) this.Facing, 0.0f)))
        {
          this.Speed.X = Calc.Approach(this.Speed.X, (float) Math.Sign((float) x - this.X) * 64f * speedMultiplier, 1000f * Engine.DeltaTime);
          int next = Math.Sign(this.X - (float) x);
          if (next != last)
          {
            this.X = (float) x;
            break;
          }
          last = next;
          yield return (object) null;
        }
        this.Speed.X = 0.0f;
        this.Sprite.Rate = 1f;
        this.Sprite.Play("idle", false, false);
        this.DummyMoving = false;
      }
    }

    public IEnumerator DummyRunTo(float x, bool fastAnim = false)
    {
      this.StateMachine.State = 11;
      if ((double) Math.Abs(this.X - x) > 4.0)
      {
        this.DummyMoving = true;
        if (fastAnim)
          this.Sprite.Play("runFast", false, false);
        else if (!this.Sprite.LastAnimationID.StartsWith("run"))
          this.Sprite.Play("runSlow", false, false);
        this.Facing = (Facings) Math.Sign(x - this.X);
        while ((double) Math.Abs(this.X - x) > 4.0)
        {
          this.Speed.X = Calc.Approach(this.Speed.X, (float) Math.Sign(x - this.X) * 90f, 1000f * Engine.DeltaTime);
          yield return (object) null;
        }
        this.Sprite.Play("idle", false, false);
        this.DummyMoving = false;
      }
    }

    private int FrozenUpdate()
    {
      return 17;
    }

    private int TempleFallUpdate()
    {
      this.Facing = Facings.Right;
      if (!this.onGround)
      {
        int num = this.level.Bounds.Left + 160;
        this.Speed.X = Calc.Approach(this.Speed.X, 54f * ((double) Math.Abs((float) num - this.X) <= 4.0 ? 0.0f : (float) Math.Sign((float) num - this.X)), 325f * Engine.DeltaTime);
      }
      if (!this.onGround && this.DummyGravity)
        this.Speed.Y = Calc.Approach(this.Speed.Y, 320f, 225f * Engine.DeltaTime);
      return 20;
    }

    private IEnumerator TempleFallCoroutine()
    {
      this.Sprite.Play("fallFast", false, false);
      while (!this.onGround)
        yield return (object) null;
      this.Play("event:/char/madeline/mirrortemple_big_landing", (string) null, 0.0f);
      if (this.Dashes <= 1)
        this.Sprite.Play("fallPose", false, false);
      else
        this.Sprite.Play("idle", false, false);
      this.Sprite.Scale.Y = 0.7f;
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
      this.level.DirectionalShake(new Vector2(0.0f, 1f), 0.5f);
      this.Speed.X = 0.0f;
      this.level.Particles.Emit(Player.P_SummitLandA, 12, this.BottomCenter, Vector2.UnitX * 3f, -1.570796f);
      this.level.Particles.Emit(Player.P_SummitLandB, 8, this.BottomCenter - Vector2.UnitX * 2f, Vector2.UnitX * 2f, 3.403392f);
      this.level.Particles.Emit(Player.P_SummitLandB, 8, this.BottomCenter + Vector2.UnitX * 2f, Vector2.UnitX * 2f, -0.2617994f);
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime)
        yield return (object) null;
      this.StateMachine.State = 0;
    }

    private void ReflectionFallBegin()
    {
      this.IgnoreJumpThrus = true;
    }

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
        crystalStaticSpinner.Destroy(false);
        this.level.Shake(0.3f);
        Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
        Celeste.Freeze(0.01f);
      }
      return 18;
    }

    private IEnumerator ReflectionFallCoroutine()
    {
      this.Sprite.Play("bigFall", false, false);
      this.level.StartCutscene(new Action<Level>(this.OnReflectionFallSkip), true, false);
      for (float t = 0.0f; (double) t < 2.0; t += Engine.DeltaTime)
      {
        this.Speed.Y = 0.0f;
        yield return (object) null;
      }
      FallEffects.Show(true);
      this.Speed.Y = 320f;
      while (!this.CollideCheck<Water>())
        yield return (object) null;
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
      FallEffects.Show(false);
      this.Sprite.Play("bigFallRecover", false, false);
      this.level.Session.Audio.Music.Event = "event:/music/lvl6/main";
      this.level.Session.Audio.Apply();
      this.level.EndCutscene();
      yield return (object) 1.2f;
      this.StateMachine.State = 0;
    }

    private void OnReflectionFallSkip(Level level)
    {
      level.OnEndOfFrame += (Action) (() =>
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
        level.LoadLevel(Player.IntroTypes.None, false);
        FallEffects.Show(false);
        level.Session.Audio.Music.Event = "event:/music/lvl6/main";
        level.Session.Audio.Apply();
      });
    }

    public IEnumerator IntroWalkCoroutine()
    {
      Vector2 start = this.Position;
      if (this.IntroWalkDirection == Facings.Right)
      {
        this.X = (float) (this.level.Bounds.Left - 16);
        this.Facing = Facings.Right;
      }
      else
      {
        this.X = (float) (this.level.Bounds.Right + 16);
        this.Facing = Facings.Left;
      }
      yield return (object) 0.3f;
      this.Sprite.Play("runSlow", false, false);
      while ((double) Math.Abs(this.X - start.X) > 2.0 && !this.CollideCheck<Solid>(this.Position + new Vector2((float) this.Facing, 0.0f)))
      {
        this.MoveTowardsX(start.X, 64f * Engine.DeltaTime, (Collision) null);
        yield return (object) null;
      }
      this.Position = start;
      this.Sprite.Play("idle", false, false);
      yield return (object) 0.2f;
      this.StateMachine.State = 0;
    }

    private IEnumerator IntroJumpCoroutine()
    {
      Vector2 start = this.Position;
      bool wasSummitJump = this.StateMachine.PreviousState == 10;
      this.Depth = -1000000;
      this.Facing = Facings.Right;
      if (!wasSummitJump)
      {
        this.Y = (float) (this.level.Bounds.Bottom + 16);
        yield return (object) 0.5f;
      }
      else
      {
        start.Y = (float) (this.level.Bounds.Bottom - 24);
        this.MoveToX((float) ((int) Math.Round((double) this.X / 8.0) * 8), (Collision) null);
      }
      if (!wasSummitJump)
        this.Sprite.Play("jumpSlow", false, false);
      while ((double) this.Y > (double) start.Y - 8.0)
      {
        this.Y += -120f * Engine.DeltaTime;
        yield return (object) null;
      }
      this.Speed.Y = -100f;
      while ((double) this.Speed.Y < 0.0)
      {
        this.Speed.Y += Engine.DeltaTime * 800f;
        yield return (object) null;
      }
      this.Speed.Y = 0.0f;
      if (wasSummitJump)
      {
        yield return (object) 0.2f;
        this.Play("event:/char/madeline/summit_areastart", (string) null, 0.0f);
        this.Sprite.Play("launchRecover", false, false);
        yield return (object) 0.1f;
      }
      else
        yield return (object) 0.1f;
      if (!wasSummitJump)
        this.Sprite.Play("fallSlow", false, false);
      while (!this.onGround)
      {
        this.Speed.Y += Engine.DeltaTime * 800f;
        yield return (object) null;
      }
      if (this.StateMachine.PreviousState != 10)
        this.Position = start;
      this.Depth = 0;
      this.level.DirectionalShake(Vector2.UnitY, 0.3f);
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
      if (wasSummitJump)
      {
        this.level.Particles.Emit(Player.P_SummitLandA, 12, this.BottomCenter, Vector2.UnitX * 3f, -1.570796f);
        this.level.Particles.Emit(Player.P_SummitLandB, 8, this.BottomCenter - Vector2.UnitX * 2f, Vector2.UnitX * 2f, 3.403392f);
        this.level.Particles.Emit(Player.P_SummitLandB, 8, this.BottomCenter + Vector2.UnitX * 2f, Vector2.UnitX * 2f, -0.2617994f);
        this.level.ParticlesBG.Emit(Player.P_SummitLandC, 30, this.BottomCenter, Vector2.UnitX * 5f);
        yield return (object) 0.35f;
        for (int i = 0; i < this.Hair.Nodes.Count; ++i)
          this.Hair.Nodes[i] = new Vector2(0.0f, (float) (2 + i));
      }
      this.StateMachine.State = 0;
    }

    private IEnumerator IntroWakeUpCoroutine()
    {
      this.Sprite.Play("asleep", false, false);
      yield return (object) 0.5f;
      yield return (object) this.Sprite.PlayRoutine("wakeUp", false);
      yield return (object) 0.2f;
      this.StateMachine.State = 0;
    }

    private void IntroRespawnBegin()
    {
      this.Play("event:/char/madeline/revive", (string) null, 0.0f);
      this.Depth = -1000000;
      this.introEase = 1f;
      Vector2 from = this.Position;
      ref Vector2 local1 = ref from;
      double x = (double) from.X;
      Rectangle bounds1 = this.level.Bounds;
      double num1 = (double) bounds1.Left + 40.0;
      bounds1 = this.level.Bounds;
      double num2 = (double) bounds1.Right - 40.0;
      double num3 = (double) MathHelper.Clamp((float) x, (float) num1, (float) num2);
      local1.X = (float) num3;
      ref Vector2 local2 = ref from;
      double y = (double) from.Y;
      Rectangle bounds2 = this.level.Bounds;
      double num4 = (double) bounds2.Top + 40.0;
      bounds2 = this.level.Bounds;
      double num5 = (double) bounds2.Bottom - 40.0;
      double num6 = (double) MathHelper.Clamp((float) y, (float) num4, (float) num5);
      local2.Y = (float) num6;
      this.deadOffset = from;
      from -= this.Position;
      this.respawnTween = Tween.Create(Tween.TweenMode.Oneshot, (Ease.Easer) null, 0.6f, true);
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

    private void BirdDashTutorialBegin()
    {
      this.DashBegin();
      this.Play("event:/char/madeline/dash_red_right", (string) null, 0.0f);
      this.Sprite.Play("dash", false, false);
    }

    private int BirdDashTutorialUpdate()
    {
      return 16;
    }

    private IEnumerator BirdDashTutorialCoroutine()
    {
      yield return (object) null;
      this.CreateTrail();
      this.Add((Component) Alarm.Create(Alarm.AlarmMode.Oneshot, new Action(this.CreateTrail), 0.08f, true));
      this.Add((Component) Alarm.Create(Alarm.AlarmMode.Oneshot, new Action(this.CreateTrail), 0.15f, true));
      Vector2 aim = new Vector2(1f, -1f).SafeNormalize();
      this.Facing = Facings.Right;
      this.Speed = aim * 240f;
      this.DashDir = aim;
      this.SceneAs<Level>().DirectionalShake(this.DashDir, 0.2f);
      SlashFx.Burst(this.Center, this.DashDir.Angle());
      for (float time = 0.0f; (double) time < 0.150000005960464; time += Engine.DeltaTime)
      {
        if (this.Speed != Vector2.Zero && this.level.OnInterval(0.02f))
          this.level.ParticlesFG.Emit(Player.P_DashA, this.Center + Calc.Random.Range(Vector2.One * -2f, Vector2.One * 2f), this.DashDir.Angle());
        yield return (object) null;
      }
      this.AutoJump = true;
      this.AutoJumpTimer = 0.0f;
      if ((double) this.DashDir.Y <= 0.0)
        this.Speed = this.DashDir * 160f;
      if ((double) this.Speed.Y < 0.0)
        this.Speed.Y *= 0.75f;
      this.Sprite.Play("fallFast", false, false);
      bool climbing = false;
      while (!this.OnGround(1) && !climbing)
      {
        this.Speed.Y = Calc.Approach(this.Speed.Y, 160f, 900f * Engine.DeltaTime);
        if (this.CollideCheck<Solid>(this.Position + new Vector2(1f, 0.0f)))
          climbing = true;
        if ((double) this.Top > (double) this.level.Bounds.Bottom)
        {
          this.level.CancelCutscene();
          this.Die(Vector2.Zero, false, true);
        }
        yield return (object) null;
      }
      if (climbing)
      {
        this.Sprite.Play("wallslide", false, false);
        Dust.Burst(this.Position + new Vector2(4f, -6f), new Vector2(-4f, 0.0f).Angle(), 1);
        this.Speed.Y = 0.0f;
        yield return (object) 0.2f;
        this.Sprite.Play("climbUp", false, false);
        while (this.CollideCheck<Solid>(this.Position + new Vector2(1f, 0.0f)))
        {
          this.Y += -45f * Engine.DeltaTime;
          yield return (object) null;
        }
        this.Play("event:/char/madeline/climb_ledge", (string) null, 0.0f);
        this.Sprite.Play("jumpFast", false, false);
        this.Speed.Y = -105f;
        while (!this.OnGround(1))
        {
          this.Speed.Y = Calc.Approach(this.Speed.Y, 160f, 900f * Engine.DeltaTime);
          this.Speed.X = 20f;
          yield return (object) null;
        }
        this.Speed.X = 0.0f;
        this.Speed.Y = 0.0f;
        this.Sprite.Play("walk", false, false);
        for (float time = 0.0f; (double) time < 0.5; time += Engine.DeltaTime)
        {
          this.X += 32f * Engine.DeltaTime;
          yield return (object) null;
        }
        this.Sprite.Play("tired", false, false);
      }
      else
      {
        this.Sprite.Play("tired", false, false);
        this.Speed.Y = 0.0f;
        while ((double) this.Speed.X != 0.0)
        {
          this.Speed.X = Calc.Approach(this.Speed.X, 0.0f, 240f * Engine.DeltaTime);
          if (this.Scene.OnInterval(0.04f))
            Dust.Burst(this.BottomCenter + new Vector2(0.0f, -2f), -2.356194f, 1);
          yield return (object) null;
        }
      }
    }

    public EventInstance Play(string sound, string param = null, float value = 0.0f)
    {
      this.AddChaserStateSound(sound, param, value, Player.ChaserStateSound.Actions.Oneshot);
      return Audio.Play(sound, this.Center, param, value);
    }

    public void Loop(SoundSource sfx, string sound)
    {
      this.AddChaserStateSound(sound, (string) null, 0.0f, Player.ChaserStateSound.Actions.Loop);
      sfx.Play(sound, (string) null, 0.0f);
    }

    public void Stop(SoundSource sfx)
    {
      if (!sfx.Playing)
        return;
      this.AddChaserStateSound(sfx.EventName, (string) null, 0.0f, Player.ChaserStateSound.Actions.Stop);
      sfx.Stop(true);
    }

    private void AddChaserStateSound(string sound, Player.ChaserStateSound.Actions action)
    {
      this.AddChaserStateSound(sound, (string) null, 0.0f, action);
    }

    private void AddChaserStateSound(
      string sound,
      string param = null,
      float value = 0.0f,
      Player.ChaserStateSound.Actions action = Player.ChaserStateSound.Actions.Oneshot)
    {
      string str = (string) null;
      Sfxs.MadelineToBadelineSound.TryGetValue(sound, out str);
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

