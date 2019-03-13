// Decompiled with JetBrains decompiler
// Type: Celeste.NPC06_Badeline_Crying
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System.Collections;
using System.Collections.Generic;

namespace Celeste
{
  public class NPC06_Badeline_Crying : NPC
  {
    private List<NPC06_Badeline_Crying.Orb> orbs = new List<NPC06_Badeline_Crying.Orb>();
    private bool started;
    private Monocle.Image white;
    private BloomPoint bloom;
    private VertexLight light;
    public SoundSource LoopingSfx;

    public NPC06_Badeline_Crying(EntityData data, Vector2 offset)
      : base(data.Position + offset)
    {
      this.Add((Component) (this.Sprite = GFX.SpriteBank.Create("badeline_boss")));
      this.Sprite.Play("scaredIdle", false, false);
      this.Add((Component) (this.white = new Monocle.Image(GFX.Game["characters/badelineBoss/calm_white"])));
      this.white.Color = Color.White * 0.0f;
      this.white.Origin = this.Sprite.Origin;
      this.white.Position = this.Sprite.Position;
      this.Add((Component) (this.bloom = new BloomPoint(new Vector2(0.0f, -6f), 0.0f, 16f)));
      this.Add((Component) (this.light = new VertexLight(new Vector2(0.0f, -6f), Color.White, 1f, 24, 64)));
      this.Add((Component) (this.LoopingSfx = new SoundSource("event:/char/badeline/boss_idle_ground")));
    }

    public override void Awake(Scene scene)
    {
      base.Awake(scene);
      if (!this.Session.GetFlag("badeline_connection"))
        return;
      FinalBossStarfield finalBossStarfield = (scene as Level).Background.Get<FinalBossStarfield>();
      if (finalBossStarfield != null)
        finalBossStarfield.Alpha = 0.0f;
      foreach (Entity entity in this.Scene.Tracker.GetEntities<ReflectionTentacles>())
        entity.RemoveSelf();
      this.RemoveSelf();
    }

    public override void Update()
    {
      base.Update();
      Player entity = this.Scene.Tracker.GetEntity<Player>();
      if (this.started || entity == null || (double) entity.X <= (double) this.X - 32.0)
        return;
      this.Scene.Add((Entity) new CS06_BossEnd(entity, this));
      this.started = true;
    }

    public override void Removed(Scene scene)
    {
      base.Removed(scene);
      foreach (Entity orb in this.orbs)
        orb.RemoveSelf();
    }

    public IEnumerator TurnWhite(float duration)
    {
      float alpha = 0.0f;
      while ((double) alpha < 1.0)
      {
        alpha += Engine.DeltaTime / duration;
        this.white.Color = Color.White * alpha;
        this.bloom.Alpha = alpha;
        yield return (object) null;
      }
      this.Sprite.Visible = false;
    }

    public IEnumerator Disperse()
    {
      Input.Rumble(RumbleStrength.Light, RumbleLength.Long);
      float size = 1f;
      while (this.orbs.Count < 8)
      {
        float to = size - 0.125f;
        while ((double) size > (double) to)
        {
          this.white.Scale = Vector2.One * size;
          this.light.Alpha = size;
          this.bloom.Alpha = size;
          size -= Engine.DeltaTime;
          yield return (object) null;
        }
        NPC06_Badeline_Crying.Orb orb = new NPC06_Badeline_Crying.Orb(this.Position);
        orb.Target = this.Position + new Vector2(-16f, -40f);
        this.Scene.Add((Entity) orb);
        this.orbs.Add(orb);
        orb = (NPC06_Badeline_Crying.Orb) null;
      }
      yield return (object) 3.25f;
      int i = 0;
      foreach (NPC06_Badeline_Crying.Orb orb1 in this.orbs)
      {
        NPC06_Badeline_Crying.Orb orb = orb1;
        orb.Routine.Replace(orb.CircleRoutine((float) ((double) i / 8.0 * 6.28318548202515)));
        ++i;
        yield return (object) 0.2f;
        orb = (NPC06_Badeline_Crying.Orb) null;
      }
      yield return (object) 2f;
      foreach (NPC06_Badeline_Crying.Orb orb1 in this.orbs)
      {
        NPC06_Badeline_Crying.Orb orb = orb1;
        orb.Routine.Replace(orb.AbsorbRoutine());
        orb = (NPC06_Badeline_Crying.Orb) null;
      }
      yield return (object) 1f;
    }

    private class Orb : Entity
    {
      public Monocle.Image Sprite;
      public BloomPoint Bloom;
      private float ease;
      public Vector2 Target;
      public Coroutine Routine;

      public float Ease
      {
        get
        {
          return this.ease;
        }
        set
        {
          this.ease = value;
          this.Sprite.Scale = Vector2.One * this.ease;
          this.Bloom.Alpha = this.ease;
        }
      }

      public Orb(Vector2 position)
        : base(position)
      {
        this.Add((Component) (this.Sprite = new Monocle.Image(GFX.Game["characters/badeline/orb"])));
        this.Add((Component) (this.Bloom = new BloomPoint(0.0f, 32f)));
        this.Add((Component) (this.Routine = new Coroutine(this.FloatRoutine(), true)));
        this.Sprite.CenterOrigin();
        this.Depth = -10001;
      }

      public IEnumerator FloatRoutine()
      {
        Vector2 speed = Vector2.Zero;
        this.Ease = 0.2f;
        while (true)
        {
          Vector2 target = this.Target + Calc.AngleToVector(Calc.Random.NextFloat(6.283185f), 16f + Calc.Random.NextFloat(40f));
          float reset = 0.0f;
          while ((double) reset < 1.0 && (double) (target - this.Position).Length() > 8.0)
          {
            Vector2 dir = (target - this.Position).SafeNormalize();
            speed += dir * 420f * Engine.DeltaTime;
            if ((double) speed.Length() > 90.0)
              speed = speed.SafeNormalize(90f);
            this.Position = this.Position + speed * Engine.DeltaTime;
            reset += Engine.DeltaTime;
            this.Ease = Calc.Approach(this.Ease, 1f, Engine.DeltaTime * 4f);
            yield return (object) null;
            dir = new Vector2();
          }
          target = new Vector2();
        }
      }

      public IEnumerator CircleRoutine(float offset)
      {
        Vector2 from = this.Position;
        float ease = 0.0f;
        Player player = this.Scene.Tracker.GetEntity<Player>();
        while (player != null)
        {
          float rotation = this.Scene.TimeActive * 2f + offset;
          Vector2 target = player.Center + Calc.AngleToVector(rotation, 24f);
          ease = Calc.Approach(ease, 1f, Engine.DeltaTime * 2f);
          this.Position = from + (target - from) * Monocle.Ease.CubeInOut(ease);
          yield return (object) null;
          target = new Vector2();
        }
      }

      public IEnumerator AbsorbRoutine()
      {
        Player player = this.Scene.Tracker.GetEntity<Player>();
        if (player != null)
        {
          Vector2 from = this.Position;
          Vector2 to = player.Center;
          for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime)
          {
            float e = Monocle.Ease.BigBackIn(p);
            this.Position = from + (to - from) * e;
            this.Ease = (float) (0.200000002980232 + (1.0 - (double) e) * 0.800000011920929);
            yield return (object) null;
          }
        }
      }
    }
  }
}

