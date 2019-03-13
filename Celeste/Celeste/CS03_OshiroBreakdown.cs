// Decompiled with JetBrains decompiler
// Type: Celeste.CS03_OshiroBreakdown
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
  public class CS03_OshiroBreakdown : CutsceneEntity
  {
    private List<DustStaticSpinner> creatures = new List<DustStaticSpinner>();
    private List<Vector2> creatureHomes = new List<Vector2>();
    public const string Flag = "oshiro_breakdown";
    private const int PlayerWalkTo = 200;
    private NPC oshiro;
    private Player player;
    private Vector2 origin;
    private const int DustAmountA = 4;

    public CS03_OshiroBreakdown(Player player, NPC oshiro)
      : base(true, false)
    {
      this.oshiro = oshiro;
      this.player = player;
      this.origin = oshiro.Position;
    }

    public override void OnBegin(Level level)
    {
      this.Add((Component) new Coroutine(this.Cutscene(level), true));
    }

    private IEnumerator Cutscene(Level level)
    {
      this.player.StateMachine.State = Player.StDummy;
      this.player.StateMachine.Locked = true;
      this.Add((Component) new Coroutine(this.player.DummyWalkTo(this.player.X - 64f, false, 1f, false), true));
      List<DustStaticSpinner> dusts = level.Entities.FindAll<DustStaticSpinner>();
      dusts.Shuffle<DustStaticSpinner>();
      foreach (DustStaticSpinner dustStaticSpinner in dusts)
      {
        DustStaticSpinner dust = dustStaticSpinner;
        if ((double) (dust.Position - this.oshiro.Position).Length() < 128.0)
        {
          this.creatures.Add(dust);
          this.creatureHomes.Add(dust.Position);
          dust.Visible = false;
        }
        dust = (DustStaticSpinner) null;
      }
      yield return (object) this.PanCamera((float) level.Bounds.Left);
      yield return (object) 0.2f;
      yield return (object) this.Level.ZoomTo(new Vector2(100f, 120f), 2f, 0.5f);
      yield return (object) Textbox.Say("CH3_OSHIRO_BREAKDOWN", new Func<IEnumerator>(this.WalkLeft), new Func<IEnumerator>(this.WalkRight), new Func<IEnumerator>(this.CreateDustA), new Func<IEnumerator>(this.CreateDustB));
      this.Add((Component) new Coroutine(this.oshiro.MoveTo(new Vector2((float) (level.Bounds.Left - 64), this.oshiro.Y), false, new int?(), false), true));
      this.oshiro.Add((Component) new SoundSource("event:/char/oshiro/move_06_04d_exit"));
      yield return (object) 0.25f;
      yield return (object) this.PanCamera(this.player.CameraTarget.X);
      this.EndCutscene(level, true);
    }

    private IEnumerator PanCamera(float to)
    {
      float from = this.Level.Camera.X;
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime)
      {
        this.Level.Camera.X = from + (to - from) * Ease.CubeInOut(p);
        yield return (object) null;
      }
    }

    private IEnumerator WalkLeft()
    {
      (this.oshiro.Sprite as OshiroSprite).AllowSpriteChanges = false;
      yield return (object) this.oshiro.MoveTo(this.origin + new Vector2(-24f, 0.0f), false, new int?(), false);
      (this.oshiro.Sprite as OshiroSprite).AllowSpriteChanges = true;
    }

    private IEnumerator WalkRight()
    {
      (this.oshiro.Sprite as OshiroSprite).AllowSpriteChanges = false;
      yield return (object) this.oshiro.MoveTo(this.origin + new Vector2(0.0f, 0.0f), false, new int?(), false);
      (this.oshiro.Sprite as OshiroSprite).AllowSpriteChanges = true;
    }

    private IEnumerator CreateDustA()
    {
      this.Add((Component) new SoundSource(this.oshiro.Position, "event:/game/03_resort/sequence_oshirofluff_pt1"));
      (this.oshiro.Sprite as OshiroSprite).AllowSpriteChanges = false;
      this.oshiro.Sprite.Play("fall", false, false);
      Audio.Play("event:/char/oshiro/chat_collapse", this.oshiro.Position);
      Distort.AnxietyOrigin = new Vector2(0.5f, 0.5f);
      for (int i = 0; i < 4; ++i)
      {
        this.Add((Component) new Coroutine(this.MoveDust(this.creatures[i], this.creatureHomes[i]), true));
        Distort.Anxiety = 0.1f + Calc.Random.NextFloat(0.1f);
        if (i % 4 == 0)
        {
          Distort.Anxiety = 0.1f + Calc.Random.NextFloat(0.1f);
          this.Level.Shake(0.3f);
          Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
          yield return (object) 0.4f;
        }
        else
          yield return (object) 0.1f;
      }
      yield return (object) 0.5f;
    }

    private IEnumerator CreateDustB()
    {
      this.Add((Component) new SoundSource(this.oshiro.Position, "event:/game/03_resort/sequence_oshirofluff_pt2"));
      for (int i = 4; i < this.creatures.Count; ++i)
      {
        this.Add((Component) new Coroutine(this.MoveDust(this.creatures[i], this.creatureHomes[i]), true));
        Distort.Anxiety = 0.1f + Calc.Random.NextFloat(0.1f);
        this.Level.Shake(0.3f);
        Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
        if ((i - 4) % 4 == 0)
        {
          Distort.Anxiety = 0.1f + Calc.Random.NextFloat(0.1f);
          yield return (object) 0.4f;
        }
        else
          yield return (object) 0.1f;
      }
      yield return (object) 1f;
      while ((double) Distort.Anxiety > 0.0)
      {
        Distort.Anxiety -= Engine.DeltaTime;
        yield return (object) null;
      }
      yield return (object) this.Level.ZoomBack(0.5f);
      yield return (object) this.player.DummyWalkToExact(this.Level.Bounds.Left + 200, false, 1f);
      yield return (object) 1f;
      Audio.Play("event:/char/oshiro/chat_get_up", this.oshiro.Position);
      this.oshiro.Sprite.Play("recover", false, false);
      yield return (object) 0.7f;
      this.oshiro.Sprite.Scale.X = 1f;
      yield return (object) 0.5f;
    }

    private IEnumerator MoveDust(DustStaticSpinner creature, Vector2 to)
    {
      Vector2 from = this.oshiro.Position + new Vector2(0.0f, -12f);
      SimpleCurve curve = new SimpleCurve(from, to, (to + from) / 2f + Vector2.UnitY * (Calc.Random.NextFloat(60f) - 30f));
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime)
      {
        yield return (object) null;
        creature.Sprite.Scale = (float) (0.5 + (double) p * 0.5);
        creature.Position = curve.GetPoint(Ease.CubeOut(p));
        creature.Visible = true;
        if (this.Scene.OnInterval(0.02f))
          this.SceneAs<Level>().ParticlesBG.Emit(DustStaticSpinner.P_Move, 1, creature.Position, Vector2.One * 4f);
      }
    }

    public override void OnEnd(Level level)
    {
      this.player.StateMachine.Locked = false;
      this.player.StateMachine.State = Player.StNormal;
      if (this.WasSkipped)
      {
        this.player.X = (float) (level.Bounds.Left + 200);
        while (!this.player.OnGround(1))
          ++this.player.Y;
        for (int index = 0; index < this.creatures.Count; ++index)
        {
          this.creatures[index].ForceInstantiate();
          this.creatures[index].Visible = true;
          this.creatures[index].Position = this.creatureHomes[index];
        }
      }
      level.Camera.Position = this.player.CameraTarget;
      level.Remove((Entity) this.oshiro);
      level.Session.SetFlag("oshiro_breakdown", true);
    }
  }
}

