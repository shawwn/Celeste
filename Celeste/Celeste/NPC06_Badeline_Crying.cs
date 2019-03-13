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
      : base(Vector2.op_Addition(data.Position, offset))
    {
      this.Add((Component) (this.Sprite = GFX.SpriteBank.Create("badeline_boss")));
      this.Sprite.Play("scaredIdle", false, false);
      this.Add((Component) (this.white = new Monocle.Image(GFX.Game["characters/badelineBoss/calm_white"])));
      this.white.Color = Color.op_Multiply(Color.get_White(), 0.0f);
      this.white.Origin = this.Sprite.Origin;
      this.white.Position = this.Sprite.Position;
      this.Add((Component) (this.bloom = new BloomPoint(new Vector2(0.0f, -6f), 0.0f, 16f)));
      this.Add((Component) (this.light = new VertexLight(new Vector2(0.0f, -6f), Color.get_White(), 1f, 24, 64)));
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
      NPC06_Badeline_Crying c06BadelineCrying = this;
      float alpha = 0.0f;
      while ((double) alpha < 1.0)
      {
        alpha += Engine.DeltaTime / duration;
        c06BadelineCrying.white.Color = Color.op_Multiply(Color.get_White(), alpha);
        c06BadelineCrying.bloom.Alpha = alpha;
        yield return (object) null;
      }
      c06BadelineCrying.Sprite.Visible = false;
    }

    public IEnumerator Disperse()
    {
      NPC06_Badeline_Crying c06BadelineCrying = this;
      Input.Rumble(RumbleStrength.Light, RumbleLength.Long);
      float size = 1f;
      while (c06BadelineCrying.orbs.Count < 8)
      {
        float to = size - 0.125f;
        while ((double) size > (double) to)
        {
          c06BadelineCrying.white.Scale = Vector2.op_Multiply(Vector2.get_One(), size);
          c06BadelineCrying.light.Alpha = size;
          c06BadelineCrying.bloom.Alpha = size;
          size -= Engine.DeltaTime;
          yield return (object) null;
        }
        NPC06_Badeline_Crying.Orb orb = new NPC06_Badeline_Crying.Orb(c06BadelineCrying.Position);
        orb.Target = Vector2.op_Addition(c06BadelineCrying.Position, new Vector2(-16f, -40f));
        c06BadelineCrying.Scene.Add((Entity) orb);
        c06BadelineCrying.orbs.Add(orb);
      }
      yield return (object) 3.25f;
      int i = 0;
      foreach (NPC06_Badeline_Crying.Orb orb in c06BadelineCrying.orbs)
      {
        orb.Routine.Replace(orb.CircleRoutine((float) ((double) i / 8.0 * 6.28318548202515)));
        ++i;
        yield return (object) 0.2f;
      }
      yield return (object) 2f;
      foreach (NPC06_Badeline_Crying.Orb orb in c06BadelineCrying.orbs)
        orb.Routine.Replace(orb.AbsorbRoutine());
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
          this.Sprite.Scale = Vector2.op_Multiply(Vector2.get_One(), this.ease);
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
        NPC06_Badeline_Crying.Orb orb = this;
        Vector2 speed = Vector2.get_Zero();
        orb.Ease = 0.2f;
        while (true)
        {
          Vector2 target = Vector2.op_Addition(orb.Target, Calc.AngleToVector(Calc.Random.NextFloat(6.283185f), 16f + Calc.Random.NextFloat(40f)));
          float reset = 0.0f;
          while ((double) reset < 1.0)
          {
            Vector2 vector2 = Vector2.op_Subtraction(target, orb.Position);
            if ((double) ((Vector2) ref vector2).Length() > 8.0)
            {
              speed = Vector2.op_Addition(speed, Vector2.op_Multiply(Vector2.op_Multiply(Vector2.op_Subtraction(target, orb.Position).SafeNormalize(), 420f), Engine.DeltaTime));
              if ((double) ((Vector2) ref speed).Length() > 90.0)
                speed = speed.SafeNormalize(90f);
              orb.Position = Vector2.op_Addition(orb.Position, Vector2.op_Multiply(speed, Engine.DeltaTime));
              reset += Engine.DeltaTime;
              orb.Ease = Calc.Approach(orb.Ease, 1f, Engine.DeltaTime * 4f);
              yield return (object) null;
            }
            else
              break;
          }
          target = (Vector2) null;
        }
      }

      public IEnumerator CircleRoutine(float offset)
      {
        NPC06_Badeline_Crying.Orb orb = this;
        Vector2 from = orb.Position;
        float ease = 0.0f;
        Player player = orb.Scene.Tracker.GetEntity<Player>();
        while (player != null)
        {
          Vector2 vector2 = Vector2.op_Addition(player.Center, Calc.AngleToVector(orb.Scene.TimeActive * 2f + offset, 24f));
          ease = Calc.Approach(ease, 1f, Engine.DeltaTime * 2f);
          orb.Position = Vector2.op_Addition(from, Vector2.op_Multiply(Vector2.op_Subtraction(vector2, from), Ease.CubeInOut(ease)));
          yield return (object) null;
        }
      }

      public IEnumerator AbsorbRoutine()
      {
        NPC06_Badeline_Crying.Orb orb = this;
        Player entity = orb.Scene.Tracker.GetEntity<Player>();
        if (entity != null)
        {
          Vector2 from = orb.Position;
          Vector2 to = entity.Center;
          for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime)
          {
            float num = Ease.BigBackIn(p);
            orb.Position = Vector2.op_Addition(from, Vector2.op_Multiply(Vector2.op_Subtraction(to, from), num));
            orb.Ease = (float) (0.200000002980232 + (1.0 - (double) num) * 0.800000011920929);
            yield return (object) null;
          }
        }
      }
    }
  }
}
