// Decompiled with JetBrains decompiler
// Type: Celeste.FinalBossMovingBlock
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  [Tracked(false)]
  public class FinalBossMovingBlock : Solid
  {
    public static ParticleType P_Stop;
    public static ParticleType P_Break;
    public int BossNodeIndex;
    private float startDelay;
    private int nodeIndex;
    private Vector2[] nodes;
    private TileGrid sprite;
    private TileGrid highlight;
    private Coroutine moveCoroutine;
    private bool isHighlighted;

    public FinalBossMovingBlock(Vector2[] nodes, float width, float height, int bossNodeIndex)
      : base(nodes[0], width, height, false)
    {
      this.BossNodeIndex = bossNodeIndex;
      this.nodes = nodes;
      int newSeed = Calc.Random.Next();
      Calc.PushRandom(newSeed);
      this.sprite = GFX.FGAutotiler.GenerateBox('g', (int) this.Width / 8, (int) this.Height / 8).TileGrid;
      this.Add((Component) this.sprite);
      Calc.PopRandom();
      Calc.PushRandom(newSeed);
      this.highlight = GFX.FGAutotiler.GenerateBox('G', (int) ((double) this.Width / 8.0), (int) this.Height / 8).TileGrid;
      this.highlight.Alpha = 0.0f;
      this.Add((Component) this.highlight);
      Calc.PopRandom();
      this.Add((Component) new TileInterceptor(this.sprite, false));
      this.Add((Component) new LightOcclude(1f));
    }

    public FinalBossMovingBlock(EntityData data, Vector2 offset)
      : this(data.NodesWithPosition(offset), (float) data.Width, (float) data.Height, data.Int(nameof (nodeIndex), 0))
    {
    }

    public override void OnShake(Vector2 amount)
    {
      base.OnShake(amount);
      this.sprite.Position = amount;
    }

    public void StartMoving(float delay)
    {
      this.startDelay = delay;
      this.Add((Component) (this.moveCoroutine = new Coroutine(this.MoveSequence(), true)));
    }

    private IEnumerator MoveSequence()
    {
      while (true)
      {
        this.StartShaking(0.2f + this.startDelay);
        if (!this.isHighlighted)
        {
          for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime / (float) (0.200000002980232 + (double) this.startDelay + 0.200000002980232))
          {
            this.highlight.Alpha = Ease.CubeIn(p);
            this.sprite.Alpha = 1f - this.highlight.Alpha;
            yield return (object) null;
          }
          this.highlight.Alpha = 1f;
          this.sprite.Alpha = 0.0f;
          this.isHighlighted = true;
        }
        else
          yield return (object) (float) (0.200000002980232 + (double) this.startDelay + 0.200000002980232);
        this.startDelay = 0.0f;
        ++this.nodeIndex;
        this.nodeIndex %= this.nodes.Length;
        Vector2 from = this.Position;
        Vector2 to = this.nodes[this.nodeIndex];
        Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeIn, 0.8f, true);
        tween.OnUpdate = (Action<Tween>) (t => this.MoveTo(Vector2.Lerp(from, to, t.Eased)));
        tween.OnComplete = (Action<Tween>) (t =>
        {
          if (this.CollideCheck<SolidTiles>(this.Position + (to - from).SafeNormalize() * 2f))
          {
            Audio.Play("event:/game/06_reflection/fallblock_boss_impact", this.Center);
            this.ImpactParticles(to - from);
          }
          else
            this.StopParticles(to - from);
        });
        this.Add((Component) tween);
        yield return (object) 0.8f;
        tween = (Tween) null;
      }
    }

    private void StopParticles(Vector2 moved)
    {
      Level level = this.SceneAs<Level>();
      float direction = moved.Angle();
      if ((double) moved.X > 0.0)
      {
        Vector2 vector2 = new Vector2(this.Right - 1f, this.Top);
        for (int index = 0; (double) index < (double) this.Height; index += 4)
          level.Particles.Emit(FinalBossMovingBlock.P_Stop, vector2 + Vector2.UnitY * (float) (2 + index + Calc.Random.Range(-1, 1)), direction);
      }
      else if ((double) moved.X < 0.0)
      {
        Vector2 vector2 = new Vector2(this.Left, this.Top);
        for (int index = 0; (double) index < (double) this.Height; index += 4)
          level.Particles.Emit(FinalBossMovingBlock.P_Stop, vector2 + Vector2.UnitY * (float) (2 + index + Calc.Random.Range(-1, 1)), direction);
      }
      if ((double) moved.Y > 0.0)
      {
        Vector2 vector2 = new Vector2(this.Left, this.Bottom - 1f);
        for (int index = 0; (double) index < (double) this.Width; index += 4)
          level.Particles.Emit(FinalBossMovingBlock.P_Stop, vector2 + Vector2.UnitX * (float) (2 + index + Calc.Random.Range(-1, 1)), direction);
      }
      else
      {
        if ((double) moved.Y >= 0.0)
          return;
        Vector2 vector2 = new Vector2(this.Left, this.Top);
        for (int index = 0; (double) index < (double) this.Width; index += 4)
          level.Particles.Emit(FinalBossMovingBlock.P_Stop, vector2 + Vector2.UnitX * (float) (2 + index + Calc.Random.Range(-1, 1)), direction);
      }
    }

    private void BreakParticles()
    {
      Vector2 center = this.Center;
      for (int index1 = 0; (double) index1 < (double) this.Width; index1 += 4)
      {
        for (int index2 = 0; (double) index2 < (double) this.Height; index2 += 4)
        {
          Vector2 position = this.Position + new Vector2((float) (2 + index1), (float) (2 + index2));
          this.SceneAs<Level>().Particles.Emit(FinalBossMovingBlock.P_Break, 1, position, Vector2.One * 2f, (position - center).Angle());
        }
      }
    }

    private void ImpactParticles(Vector2 moved)
    {
      if ((double) moved.X < 0.0)
      {
        Vector2 vector2 = new Vector2(0.0f, 2f);
        for (int index = 0; (double) index < (double) this.Height / 8.0; ++index)
        {
          Vector2 point = new Vector2(this.Left - 1f, this.Top + 4f + (float) (index * 8));
          if (!this.Scene.CollideCheck<Water>(point) && this.Scene.CollideCheck<Solid>(point))
          {
            this.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, point + vector2, 0.0f);
            this.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, point - vector2, 0.0f);
          }
        }
      }
      else if ((double) moved.X > 0.0)
      {
        Vector2 vector2 = new Vector2(0.0f, 2f);
        for (int index = 0; (double) index < (double) this.Height / 8.0; ++index)
        {
          Vector2 point = new Vector2(this.Right + 1f, this.Top + 4f + (float) (index * 8));
          if (!this.Scene.CollideCheck<Water>(point) && this.Scene.CollideCheck<Solid>(point))
          {
            this.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, point + vector2, 3.141593f);
            this.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, point - vector2, 3.141593f);
          }
        }
      }
      if ((double) moved.Y < 0.0)
      {
        Vector2 vector2 = new Vector2(2f, 0.0f);
        for (int index = 0; (double) index < (double) this.Width / 8.0; ++index)
        {
          Vector2 point = new Vector2(this.Left + 4f + (float) (index * 8), this.Top - 1f);
          if (!this.Scene.CollideCheck<Water>(point) && this.Scene.CollideCheck<Solid>(point))
          {
            this.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, point + vector2, 1.570796f);
            this.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, point - vector2, 1.570796f);
          }
        }
      }
      else
      {
        if ((double) moved.Y <= 0.0)
          return;
        Vector2 vector2 = new Vector2(2f, 0.0f);
        for (int index = 0; (double) index < (double) this.Width / 8.0; ++index)
        {
          Vector2 point = new Vector2(this.Left + 4f + (float) (index * 8), this.Bottom + 1f);
          if (!this.Scene.CollideCheck<Water>(point) && this.Scene.CollideCheck<Solid>(point))
          {
            this.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, point + vector2, -1.570796f);
            this.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, point - vector2, -1.570796f);
          }
        }
      }
    }

    public override void Render()
    {
      Vector2 position = this.Position;
      this.Position = this.Position + this.Shake;
      base.Render();
      if ((double) this.highlight.Alpha > 0.0 && (double) this.highlight.Alpha < 1.0)
      {
        int num = (int) ((1.0 - (double) this.highlight.Alpha) * 16.0);
        Rectangle rect = new Rectangle((int) this.X, (int) this.Y, (int) this.Width, (int) this.Height);
        rect.Inflate(num, num);
        Draw.HollowRect(rect, Color.Lerp(Color.Purple, Color.Pink, 0.7f));
      }
      this.Position = position;
    }

    private void Finish()
    {
      Vector2 from = this.CenterRight + Vector2.UnitX * 10f;
      for (int index1 = 0; (double) index1 < (double) this.Width / 8.0; ++index1)
      {
        for (int index2 = 0; (double) index2 < (double) this.Height / 8.0; ++index2)
          this.Scene.Add((Entity) Engine.Pooler.Create<Debris>().Init(this.Position + new Vector2((float) (4 + index1 * 8), (float) (4 + index2 * 8)), 'f').BlastFrom(from));
      }
      this.BreakParticles();
      this.DestroyStaticMovers();
      this.RemoveSelf();
    }

    public void Destroy(float delay)
    {
      if (this.Scene == null)
        return;
      if (this.moveCoroutine != null)
        this.Remove((Component) this.moveCoroutine);
      if ((double) delay <= 0.0)
      {
        this.Finish();
      }
      else
      {
        this.StartShaking(delay);
        Alarm.Set((Entity) this, delay, new Action(this.Finish), Alarm.AlarmMode.Oneshot);
      }
    }
  }
}

