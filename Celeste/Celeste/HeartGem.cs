// Decompiled with JetBrains decompiler
// Type: Celeste.HeartGem
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Celeste
{
  [Tracked(false)]
  public class HeartGem : Entity
  {
    private bool autoPulse = true;
    private List<InvisibleBarrier> walls = new List<InvisibleBarrier>();
    public static ParticleType P_BlueShine;
    public static ParticleType P_RedShine;
    public static ParticleType P_GoldShine;
    public bool IsGhost;
    public const float GhostAlpha = 0.8f;
    private Sprite sprite;
    private Sprite white;
    private ParticleType shineParticle;
    public Wiggler ScaleWiggler;
    private Wiggler moveWiggler;
    private Vector2 moveWiggleDir;
    private BloomPoint bloom;
    private VertexLight light;
    private Poem poem;
    private float timer;
    private bool collected;
    private float bounceSfxDelay;
    private bool removeCameraTriggers;
    private SoundEmitter sfx;
    private HoldableCollider holdableCollider;

    public HeartGem(Vector2 position)
      : base(position)
    {
      this.Add((Component) (this.holdableCollider = new HoldableCollider(new Action<Holdable>(this.OnHoldable), (Collider) null)));
      this.Add((Component) new MirrorReflection());
    }

    public HeartGem(EntityData data, Vector2 offset)
      : this(Vector2.op_Addition(data.Position, offset))
    {
      this.removeCameraTriggers = data.Bool(nameof (removeCameraTriggers), false);
    }

    public override void Awake(Scene scene)
    {
      base.Awake(scene);
      AreaKey area = (this.Scene as Level).Session.Area;
      this.IsGhost = SaveData.Instance.Areas[area.ID].Modes[(int) area.Mode].HeartGem;
      string id = "heartgem" + (object) (int) area.Mode;
      if (this.IsGhost)
        id = "heartGemGhost";
      this.Add((Component) (this.sprite = GFX.SpriteBank.Create(id)));
      this.sprite.Play("spin", false, false);
      this.sprite.OnLoop = (Action<string>) (anim =>
      {
        if (!this.Visible || !(anim == "spin") || !this.autoPulse)
          return;
        Audio.Play("event:/game/general/crystalheart_pulse", this.Position);
        this.ScaleWiggler.Start();
        (this.Scene as Level).Displacement.AddBurst(this.Position, 0.35f, 8f, 48f, 0.25f, (Ease.Easer) null, (Ease.Easer) null);
      });
      if (this.IsGhost)
        this.sprite.Color = Color.op_Multiply(Color.get_White(), 0.8f);
      this.Collider = (Collider) new Hitbox(16f, 16f, -8f, -8f);
      this.Add((Component) new PlayerCollider(new Action<Player>(this.OnPlayer), (Collider) null, (Collider) null));
      this.Add((Component) (this.ScaleWiggler = Wiggler.Create(0.5f, 4f, (Action<float>) (f => this.sprite.Scale = Vector2.op_Multiply(Vector2.get_One(), (float) (1.0 + (double) f * 0.25))), false, false)));
      this.Add((Component) (this.bloom = new BloomPoint(0.75f, 16f)));
      Color color;
      if (area.Mode == AreaMode.Normal)
      {
        color = Color.get_Aqua();
        this.shineParticle = HeartGem.P_BlueShine;
      }
      else if (area.Mode == AreaMode.BSide)
      {
        color = Color.get_Red();
        this.shineParticle = HeartGem.P_RedShine;
      }
      else
      {
        color = Color.get_Gold();
        this.shineParticle = HeartGem.P_GoldShine;
      }
      this.Add((Component) (this.light = new VertexLight(Color.Lerp(color, Color.get_White(), 0.5f), 1f, 32, 64)));
      this.moveWiggler = Wiggler.Create(0.8f, 2f, (Action<float>) null, false, false);
      this.moveWiggler.StartZero = true;
      this.Add((Component) this.moveWiggler);
    }

    public override void Update()
    {
      this.bounceSfxDelay -= Engine.DeltaTime;
      this.timer += Engine.DeltaTime;
      this.sprite.Position = Vector2.op_Addition(Vector2.op_Multiply(Vector2.op_Multiply(Vector2.get_UnitY(), (float) Math.Sin((double) this.timer * 2.0)), 2f), Vector2.op_Multiply(Vector2.op_Multiply(this.moveWiggleDir, this.moveWiggler.Value), -8f));
      if (this.white != null)
      {
        this.white.Position = this.sprite.Position;
        this.white.Scale = this.sprite.Scale;
        if (this.white.CurrentAnimationID != this.sprite.CurrentAnimationID)
          this.white.Play(this.sprite.CurrentAnimationID, false, false);
        this.white.SetAnimationFrame(this.sprite.CurrentAnimationFrame);
      }
      if (this.collected)
      {
        Player entity = this.Scene.Tracker.GetEntity<Player>();
        if (entity == null || entity.Dead)
          this.EndCutscene();
      }
      base.Update();
      if (this.collected || !this.Scene.OnInterval(0.1f))
        return;
      this.SceneAs<Level>().Particles.Emit(this.shineParticle, 1, this.Center, Vector2.op_Multiply(Vector2.get_One(), 8f));
    }

    public void OnHoldable(Holdable h)
    {
      Player entity = this.Scene.Tracker.GetEntity<Player>();
      if (this.collected || entity == null || !h.Dangerous(this.holdableCollider))
        return;
      this.Collect(entity);
    }

    public void OnPlayer(Player player)
    {
      if (this.collected || (this.Scene as Level).Frozen)
        return;
      if (player.DashAttacking)
      {
        this.Collect(player);
      }
      else
      {
        if ((double) this.bounceSfxDelay <= 0.0)
        {
          Audio.Play("event:/game/general/crystalheart_bounce", this.Position);
          this.bounceSfxDelay = 0.1f;
        }
        player.PointBounce(this.Center);
        this.moveWiggler.Start();
        this.ScaleWiggler.Start();
        this.moveWiggleDir = Vector2.op_Subtraction(this.Center, player.Center).SafeNormalize(Vector2.get_UnitY());
        Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
      }
    }

    private void Collect(Player player)
    {
      this.Scene.Tracker.GetEntity<AngryOshiro>()?.StopControllingTime();
      this.Add((Component) new Coroutine(this.CollectRoutine(player), true)
      {
        UseRawDeltaTime = true
      });
      this.collected = true;
      if (!this.removeCameraTriggers)
        return;
      foreach (Entity entity in this.Scene.Entities.FindAll<CameraOffsetTrigger>())
        entity.RemoveSelf();
    }

    private IEnumerator CollectRoutine(Player player)
    {
      HeartGem heartGem = this;
      Level level = heartGem.Scene as Level;
      AreaKey area = level.Session.Area;
      string poemID = AreaData.Get((Scene) level).Mode[(int) area.Mode].PoemID;
      bool completeArea = area.Mode != AreaMode.Normal || area.ID == 9;
      level.CanRetry = false;
      if (completeArea)
      {
        Audio.SetMusic((string) null, true, true);
        Audio.SetAmbience((string) null, true);
        List<Strawberry> strawberryList = new List<Strawberry>();
        foreach (Follower follower in player.Leader.Followers)
        {
          if (follower.Entity is Strawberry)
            strawberryList.Add(follower.Entity as Strawberry);
        }
        foreach (Strawberry strawberry in strawberryList)
          strawberry.OnCollect();
      }
      string sfx = "event:/game/general/crystalheart_blue_get";
      if (area.Mode == AreaMode.BSide)
        sfx = "event:/game/general/crystalheart_red_get";
      else if (area.Mode == AreaMode.CSide)
        sfx = "event:/game/general/crystalheart_gold_get";
      heartGem.sfx = SoundEmitter.Play(sfx, (Entity) heartGem, new Vector2?());
      // ISSUE: reference to a compiler-generated method
      heartGem.Add((Component) new LevelEndingHook(new Action(heartGem.\u003CCollectRoutine\u003Eb__29_0)));
      List<InvisibleBarrier> walls1 = heartGem.walls;
      Rectangle bounds1 = level.Bounds;
      double right = (double) ((Rectangle) ref bounds1).get_Right();
      Rectangle bounds2 = level.Bounds;
      double top1 = (double) ((Rectangle) ref bounds2).get_Top();
      InvisibleBarrier invisibleBarrier1 = new InvisibleBarrier(new Vector2((float) right, (float) top1), 8f, (float) level.Bounds.Height);
      walls1.Add(invisibleBarrier1);
      List<InvisibleBarrier> walls2 = heartGem.walls;
      Rectangle bounds3 = level.Bounds;
      double num1 = (double) (((Rectangle) ref bounds3).get_Left() - 8);
      Rectangle bounds4 = level.Bounds;
      double top2 = (double) ((Rectangle) ref bounds4).get_Top();
      InvisibleBarrier invisibleBarrier2 = new InvisibleBarrier(new Vector2((float) num1, (float) top2), 8f, (float) level.Bounds.Height);
      walls2.Add(invisibleBarrier2);
      List<InvisibleBarrier> walls3 = heartGem.walls;
      bounds4 = level.Bounds;
      double left = (double) ((Rectangle) ref bounds4).get_Left();
      bounds4 = level.Bounds;
      double num2 = (double) (((Rectangle) ref bounds4).get_Top() - 8);
      InvisibleBarrier invisibleBarrier3 = new InvisibleBarrier(new Vector2((float) left, (float) num2), (float) level.Bounds.Width, 8f);
      walls3.Add(invisibleBarrier3);
      foreach (InvisibleBarrier wall in heartGem.walls)
        heartGem.Scene.Add((Entity) wall);
      heartGem.Add((Component) (heartGem.white = GFX.SpriteBank.Create("heartGemWhite")));
      heartGem.Depth = -2000000;
      yield return (object) null;
      Celeste.Celeste.Freeze(0.2f);
      yield return (object) null;
      Engine.TimeRate = 0.5f;
      player.Depth = -2000000;
      for (int index = 0; index < 10; ++index)
        heartGem.Scene.Add((Entity) new AbsorbOrb(heartGem.Position, (Entity) null));
      level.Shake(0.3f);
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
      level.Flash(Color.get_White(), false);
      level.FormationBackdrop.Display = true;
      level.FormationBackdrop.Alpha = 1f;
      heartGem.light.Alpha = heartGem.bloom.Alpha = 0.0f;
      heartGem.Visible = false;
      float t;
      for (t = 0.0f; (double) t < 2.0; t += Engine.RawDeltaTime)
      {
        Engine.TimeRate = Calc.Approach(Engine.TimeRate, 0.0f, Engine.RawDeltaTime * 0.25f);
        yield return (object) null;
      }
      Engine.TimeRate = 1f;
      heartGem.Tag = (int) Tags.FrozenUpdate;
      level.Frozen = true;
      heartGem.RegisterAsCollected(level, poemID);
      if (completeArea)
      {
        level.TimerStopped = true;
        level.RegisterAreaComplete();
      }
      string text = (string) null;
      if (!string.IsNullOrEmpty(poemID))
        text = Dialog.Clean("poem_" + poemID, (Language) null);
      heartGem.poem = new Poem(text, area.Mode);
      heartGem.poem.Alpha = 0.0f;
      heartGem.Scene.Add((Entity) heartGem.poem);
      for (t = 0.0f; (double) t < 1.0; t += Engine.RawDeltaTime)
      {
        heartGem.poem.Alpha = Ease.CubeOut(t);
        yield return (object) null;
      }
      while (!Input.MenuConfirm.Pressed && !Input.MenuCancel.Pressed)
        yield return (object) null;
      heartGem.sfx.Source.Param("end", 1f);
      if (!completeArea)
      {
        level.FormationBackdrop.Display = false;
        for (t = 0.0f; (double) t < 1.0; t += Engine.RawDeltaTime * 2f)
        {
          heartGem.poem.Alpha = Ease.CubeIn(1f - t);
          yield return (object) null;
        }
        player.Depth = 0;
        heartGem.EndCutscene();
      }
      else
      {
        FadeWipe fadeWipe = new FadeWipe((Scene) level, false, (Action) null);
        fadeWipe.Duration = 3.25f;
        yield return (object) fadeWipe.Duration;
        level.CompleteArea(false, true);
      }
    }

    private void EndCutscene()
    {
      Level scene = this.Scene as Level;
      scene.Frozen = false;
      scene.CanRetry = true;
      scene.FormationBackdrop.Display = false;
      Engine.TimeRate = 1f;
      if (this.poem != null)
        this.poem.RemoveSelf();
      foreach (Entity wall in this.walls)
        wall.RemoveSelf();
      this.RemoveSelf();
    }

    private void RegisterAsCollected(Level level, string poemID)
    {
      level.Session.HeartGem = true;
      level.Session.UpdateLevelStartDashes();
      int unlockedModes = SaveData.Instance.UnlockedModes;
      SaveData.Instance.RegisterHeartGem(level.Session.Area);
      if (!string.IsNullOrEmpty(poemID))
        SaveData.Instance.RegisterPoemEntry(poemID);
      if (unlockedModes < 3 && SaveData.Instance.UnlockedModes >= 3)
        level.Session.UnlockedCSide = true;
      if (SaveData.Instance.TotalHeartGems < 24)
        return;
      Achievements.Register(Achievement.CSIDES);
    }
  }
}
