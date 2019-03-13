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
      CS03_OshiroBreakdown cs03OshiroBreakdown1 = this;
      cs03OshiroBreakdown1.player.StateMachine.State = 11;
      cs03OshiroBreakdown1.player.StateMachine.Locked = true;
      cs03OshiroBreakdown1.Add((Component) new Coroutine(cs03OshiroBreakdown1.player.DummyWalkTo(cs03OshiroBreakdown1.player.X - 64f, false, 1f, false), true));
      List<DustStaticSpinner> all = level.Entities.FindAll<DustStaticSpinner>();
      all.Shuffle<DustStaticSpinner>();
      foreach (DustStaticSpinner dustStaticSpinner in all)
      {
        Vector2 vector2 = Vector2.op_Subtraction(dustStaticSpinner.Position, cs03OshiroBreakdown1.oshiro.Position);
        if ((double) ((Vector2) ref vector2).Length() < 128.0)
        {
          cs03OshiroBreakdown1.creatures.Add(dustStaticSpinner);
          cs03OshiroBreakdown1.creatureHomes.Add(dustStaticSpinner.Position);
          dustStaticSpinner.Visible = false;
        }
      }
      CS03_OshiroBreakdown cs03OshiroBreakdown2 = cs03OshiroBreakdown1;
      Rectangle bounds1 = level.Bounds;
      double left = (double) ((Rectangle) ref bounds1).get_Left();
      yield return (object) cs03OshiroBreakdown2.PanCamera((float) left);
      yield return (object) 0.2f;
      yield return (object) cs03OshiroBreakdown1.Level.ZoomTo(new Vector2(100f, 120f), 2f, 0.5f);
      yield return (object) Textbox.Say("CH3_OSHIRO_BREAKDOWN", new Func<IEnumerator>(cs03OshiroBreakdown1.WalkLeft), new Func<IEnumerator>(cs03OshiroBreakdown1.WalkRight), new Func<IEnumerator>(cs03OshiroBreakdown1.CreateDustA), new Func<IEnumerator>(cs03OshiroBreakdown1.CreateDustB));
      CS03_OshiroBreakdown cs03OshiroBreakdown3 = cs03OshiroBreakdown1;
      NPC oshiro = cs03OshiroBreakdown1.oshiro;
      Rectangle bounds2 = level.Bounds;
      Vector2 target = new Vector2((float) (((Rectangle) ref bounds2).get_Left() - 64), cs03OshiroBreakdown1.oshiro.Y);
      int? turnAtEndTo = new int?();
      Coroutine coroutine = new Coroutine(oshiro.MoveTo(target, false, turnAtEndTo, false), true);
      cs03OshiroBreakdown3.Add((Component) coroutine);
      cs03OshiroBreakdown1.oshiro.Add((Component) new SoundSource("event:/char/oshiro/move_06_04d_exit"));
      yield return (object) 0.25f;
      yield return (object) cs03OshiroBreakdown1.PanCamera((float) cs03OshiroBreakdown1.player.CameraTarget.X);
      cs03OshiroBreakdown1.EndCutscene(level, true);
    }

    private IEnumerator PanCamera(float to)
    {
      CS03_OshiroBreakdown cs03OshiroBreakdown = this;
      float from = cs03OshiroBreakdown.Level.Camera.X;
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime)
      {
        cs03OshiroBreakdown.Level.Camera.X = from + (to - from) * Ease.CubeInOut(p);
        yield return (object) null;
      }
    }

    private IEnumerator WalkLeft()
    {
      (this.oshiro.Sprite as OshiroSprite).AllowSpriteChanges = false;
      yield return (object) this.oshiro.MoveTo(Vector2.op_Addition(this.origin, new Vector2(-24f, 0.0f)), false, new int?(), false);
      (this.oshiro.Sprite as OshiroSprite).AllowSpriteChanges = true;
    }

    private IEnumerator WalkRight()
    {
      (this.oshiro.Sprite as OshiroSprite).AllowSpriteChanges = false;
      yield return (object) this.oshiro.MoveTo(Vector2.op_Addition(this.origin, new Vector2(0.0f, 0.0f)), false, new int?(), false);
      (this.oshiro.Sprite as OshiroSprite).AllowSpriteChanges = true;
    }

    private IEnumerator CreateDustA()
    {
      CS03_OshiroBreakdown cs03OshiroBreakdown = this;
      cs03OshiroBreakdown.Add((Component) new SoundSource(cs03OshiroBreakdown.oshiro.Position, "event:/game/03_resort/sequence_oshirofluff_pt1"));
      (cs03OshiroBreakdown.oshiro.Sprite as OshiroSprite).AllowSpriteChanges = false;
      cs03OshiroBreakdown.oshiro.Sprite.Play("fall", false, false);
      Audio.Play("event:/char/oshiro/chat_collapse", cs03OshiroBreakdown.oshiro.Position);
      Distort.AnxietyOrigin = new Vector2(0.5f, 0.5f);
      for (int i = 0; i < 4; ++i)
      {
        cs03OshiroBreakdown.Add((Component) new Coroutine(cs03OshiroBreakdown.MoveDust(cs03OshiroBreakdown.creatures[i], cs03OshiroBreakdown.creatureHomes[i]), true));
        Distort.Anxiety = 0.1f + Calc.Random.NextFloat(0.1f);
        if (i % 4 == 0)
        {
          Distort.Anxiety = 0.1f + Calc.Random.NextFloat(0.1f);
          cs03OshiroBreakdown.Level.Shake(0.3f);
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
      CS03_OshiroBreakdown cs03OshiroBreakdown = this;
      cs03OshiroBreakdown.Add((Component) new SoundSource(cs03OshiroBreakdown.oshiro.Position, "event:/game/03_resort/sequence_oshirofluff_pt2"));
      for (int i = 4; i < cs03OshiroBreakdown.creatures.Count; ++i)
      {
        cs03OshiroBreakdown.Add((Component) new Coroutine(cs03OshiroBreakdown.MoveDust(cs03OshiroBreakdown.creatures[i], cs03OshiroBreakdown.creatureHomes[i]), true));
        Distort.Anxiety = 0.1f + Calc.Random.NextFloat(0.1f);
        cs03OshiroBreakdown.Level.Shake(0.3f);
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
      yield return (object) cs03OshiroBreakdown.Level.ZoomBack(0.5f);
      Player player = cs03OshiroBreakdown.player;
      Rectangle bounds = cs03OshiroBreakdown.Level.Bounds;
      int x = ((Rectangle) ref bounds).get_Left() + 200;
      yield return (object) player.DummyWalkToExact(x, false, 1f);
      yield return (object) 1f;
      Audio.Play("event:/char/oshiro/chat_get_up", cs03OshiroBreakdown.oshiro.Position);
      cs03OshiroBreakdown.oshiro.Sprite.Play("recover", false, false);
      yield return (object) 0.7f;
      cs03OshiroBreakdown.oshiro.Sprite.Scale.X = (__Null) 1.0;
      yield return (object) 0.5f;
    }

    private IEnumerator MoveDust(DustStaticSpinner creature, Vector2 to)
    {
      CS03_OshiroBreakdown cs03OshiroBreakdown = this;
      Vector2 begin = Vector2.op_Addition(cs03OshiroBreakdown.oshiro.Position, new Vector2(0.0f, -12f));
      SimpleCurve curve = new SimpleCurve(begin, to, Vector2.op_Addition(Vector2.op_Division(Vector2.op_Addition(to, begin), 2f), Vector2.op_Multiply(Vector2.get_UnitY(), Calc.Random.NextFloat(60f) - 30f)));
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime)
      {
        yield return (object) null;
        creature.Sprite.Scale = (float) (0.5 + (double) p * 0.5);
        creature.Position = curve.GetPoint(Ease.CubeOut(p));
        creature.Visible = true;
        if (cs03OshiroBreakdown.Scene.OnInterval(0.02f))
          cs03OshiroBreakdown.SceneAs<Level>().ParticlesBG.Emit(DustStaticSpinner.P_Move, 1, creature.Position, Vector2.op_Multiply(Vector2.get_One(), 4f));
      }
    }

    public override void OnEnd(Level level)
    {
      this.player.StateMachine.Locked = false;
      this.player.StateMachine.State = 0;
      if (this.WasSkipped)
      {
        Player player = this.player;
        Rectangle bounds = level.Bounds;
        double num = (double) (((Rectangle) ref bounds).get_Left() + 200);
        player.X = (float) num;
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
