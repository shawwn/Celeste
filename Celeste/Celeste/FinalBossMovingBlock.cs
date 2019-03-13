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
      FinalBossMovingBlock finalBossMovingBlock1 = this;
      while (true)
      {
        FinalBossMovingBlock finalBossMovingBlock = finalBossMovingBlock1;
        finalBossMovingBlock1.StartShaking(0.2f + finalBossMovingBlock1.startDelay);
        if (!finalBossMovingBlock1.isHighlighted)
        {
          for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime / (float) (0.200000002980232 + (double) finalBossMovingBlock1.startDelay + 0.200000002980232))
          {
            finalBossMovingBlock1.highlight.Alpha = Ease.CubeIn(p);
            finalBossMovingBlock1.sprite.Alpha = 1f - finalBossMovingBlock1.highlight.Alpha;
            yield return (object) null;
          }
          finalBossMovingBlock1.highlight.Alpha = 1f;
          finalBossMovingBlock1.sprite.Alpha = 0.0f;
          finalBossMovingBlock1.isHighlighted = true;
        }
        else
          yield return (object) (float) (0.200000002980232 + (double) finalBossMovingBlock1.startDelay + 0.200000002980232);
        finalBossMovingBlock1.startDelay = 0.0f;
        ++finalBossMovingBlock1.nodeIndex;
        finalBossMovingBlock1.nodeIndex %= finalBossMovingBlock1.nodes.Length;
        Vector2 from = finalBossMovingBlock1.Position;
        Vector2 to = finalBossMovingBlock1.nodes[finalBossMovingBlock1.nodeIndex];
        Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeIn, 0.8f, true);
        tween.OnUpdate = (Action<Tween>) (t => finalBossMovingBlock.MoveTo(Vector2.Lerp(from, to, t.Eased)));
        tween.OnComplete = (Action<Tween>) (t =>
        {
          if (finalBossMovingBlock.CollideCheck<SolidTiles>(Vector2.op_Addition(finalBossMovingBlock.Position, Vector2.op_Multiply(Vector2.op_Subtraction(to, from).SafeNormalize(), 2f))))
          {
            Audio.Play("event:/game/06_reflection/fallblock_boss_impact", finalBossMovingBlock.Center);
            finalBossMovingBlock.ImpactParticles(Vector2.op_Subtraction(to, from));
          }
          else
            finalBossMovingBlock.StopParticles(Vector2.op_Subtraction(to, from));
        });
        finalBossMovingBlock1.Add((Component) tween);
        yield return (object) 0.8f;
      }
    }

    private void StopParticles(Vector2 moved)
    {
      Level level = this.SceneAs<Level>();
      float direction = moved.Angle();
      if (moved.X > 0.0)
      {
        Vector2 vector2;
        ((Vector2) ref vector2).\u002Ector(this.Right - 1f, this.Top);
        for (int index = 0; (double) index < (double) this.Height; index += 4)
          level.Particles.Emit(FinalBossMovingBlock.P_Stop, Vector2.op_Addition(vector2, Vector2.op_Multiply(Vector2.get_UnitY(), (float) (2 + index + Calc.Random.Range(-1, 1)))), direction);
      }
      else if (moved.X < 0.0)
      {
        Vector2 vector2;
        ((Vector2) ref vector2).\u002Ector(this.Left, this.Top);
        for (int index = 0; (double) index < (double) this.Height; index += 4)
          level.Particles.Emit(FinalBossMovingBlock.P_Stop, Vector2.op_Addition(vector2, Vector2.op_Multiply(Vector2.get_UnitY(), (float) (2 + index + Calc.Random.Range(-1, 1)))), direction);
      }
      if (moved.Y > 0.0)
      {
        Vector2 vector2;
        ((Vector2) ref vector2).\u002Ector(this.Left, this.Bottom - 1f);
        for (int index = 0; (double) index < (double) this.Width; index += 4)
          level.Particles.Emit(FinalBossMovingBlock.P_Stop, Vector2.op_Addition(vector2, Vector2.op_Multiply(Vector2.get_UnitX(), (float) (2 + index + Calc.Random.Range(-1, 1)))), direction);
      }
      else
      {
        if (moved.Y >= 0.0)
          return;
        Vector2 vector2;
        ((Vector2) ref vector2).\u002Ector(this.Left, this.Top);
        for (int index = 0; (double) index < (double) this.Width; index += 4)
          level.Particles.Emit(FinalBossMovingBlock.P_Stop, Vector2.op_Addition(vector2, Vector2.op_Multiply(Vector2.get_UnitX(), (float) (2 + index + Calc.Random.Range(-1, 1)))), direction);
      }
    }

    private void BreakParticles()
    {
      Vector2 center = this.Center;
      for (int index1 = 0; (double) index1 < (double) this.Width; index1 += 4)
      {
        for (int index2 = 0; (double) index2 < (double) this.Height; index2 += 4)
        {
          Vector2 position = Vector2.op_Addition(this.Position, new Vector2((float) (2 + index1), (float) (2 + index2)));
          this.SceneAs<Level>().Particles.Emit(FinalBossMovingBlock.P_Break, 1, position, Vector2.op_Multiply(Vector2.get_One(), 2f), Vector2.op_Subtraction(position, center).Angle());
        }
      }
    }

    private void ImpactParticles(Vector2 moved)
    {
      if (moved.X < 0.0)
      {
        Vector2 vector2;
        ((Vector2) ref vector2).\u002Ector(0.0f, 2f);
        for (int index = 0; (double) index < (double) this.Height / 8.0; ++index)
        {
          Vector2 point;
          ((Vector2) ref point).\u002Ector(this.Left - 1f, this.Top + 4f + (float) (index * 8));
          if (!this.Scene.CollideCheck<Water>(point) && this.Scene.CollideCheck<Solid>(point))
          {
            this.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, Vector2.op_Addition(point, vector2), 0.0f);
            this.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, Vector2.op_Subtraction(point, vector2), 0.0f);
          }
        }
      }
      else if (moved.X > 0.0)
      {
        Vector2 vector2;
        ((Vector2) ref vector2).\u002Ector(0.0f, 2f);
        for (int index = 0; (double) index < (double) this.Height / 8.0; ++index)
        {
          Vector2 point;
          ((Vector2) ref point).\u002Ector(this.Right + 1f, this.Top + 4f + (float) (index * 8));
          if (!this.Scene.CollideCheck<Water>(point) && this.Scene.CollideCheck<Solid>(point))
          {
            this.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, Vector2.op_Addition(point, vector2), 3.141593f);
            this.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, Vector2.op_Subtraction(point, vector2), 3.141593f);
          }
        }
      }
      if (moved.Y < 0.0)
      {
        Vector2 vector2;
        ((Vector2) ref vector2).\u002Ector(2f, 0.0f);
        for (int index = 0; (double) index < (double) this.Width / 8.0; ++index)
        {
          Vector2 point;
          ((Vector2) ref point).\u002Ector(this.Left + 4f + (float) (index * 8), this.Top - 1f);
          if (!this.Scene.CollideCheck<Water>(point) && this.Scene.CollideCheck<Solid>(point))
          {
            this.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, Vector2.op_Addition(point, vector2), 1.570796f);
            this.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, Vector2.op_Subtraction(point, vector2), 1.570796f);
          }
        }
      }
      else
      {
        if (moved.Y <= 0.0)
          return;
        Vector2 vector2;
        ((Vector2) ref vector2).\u002Ector(2f, 0.0f);
        for (int index = 0; (double) index < (double) this.Width / 8.0; ++index)
        {
          Vector2 point;
          ((Vector2) ref point).\u002Ector(this.Left + 4f + (float) (index * 8), this.Bottom + 1f);
          if (!this.Scene.CollideCheck<Water>(point) && this.Scene.CollideCheck<Solid>(point))
          {
            this.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, Vector2.op_Addition(point, vector2), -1.570796f);
            this.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, Vector2.op_Subtraction(point, vector2), -1.570796f);
          }
        }
      }
    }

    public override void Render()
    {
      Vector2 position = this.Position;
      this.Position = Vector2.op_Addition(this.Position, this.Shake);
      base.Render();
      if ((double) this.highlight.Alpha > 0.0 && (double) this.highlight.Alpha < 1.0)
      {
        int num = (int) ((1.0 - (double) this.highlight.Alpha) * 16.0);
        Rectangle rect;
        ((Rectangle) ref rect).\u002Ector((int) this.X, (int) this.Y, (int) this.Width, (int) this.Height);
        ((Rectangle) ref rect).Inflate(num, num);
        Draw.HollowRect(rect, Color.Lerp(Color.get_Purple(), Color.get_Pink(), 0.7f));
      }
      this.Position = position;
    }

    private void Finish()
    {
      Vector2 from = Vector2.op_Addition(this.CenterRight, Vector2.op_Multiply(Vector2.get_UnitX(), 10f));
      for (int index1 = 0; (double) index1 < (double) this.Width / 8.0; ++index1)
      {
        for (int index2 = 0; (double) index2 < (double) this.Height / 8.0; ++index2)
          this.Scene.Add((Entity) Engine.Pooler.Create<Debris>().Init(Vector2.op_Addition(this.Position, new Vector2((float) (4 + index1 * 8), (float) (4 + index2 * 8))), 'f').BlastFrom(from));
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
