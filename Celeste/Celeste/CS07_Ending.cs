// Decompiled with JetBrains decompiler
// Type: Celeste.CS07_Ending
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class CS07_Ending : CutsceneEntity
  {
    private Player player;
    private BadelineDummy badeline;
    private Vector2 target;

    public CS07_Ending(Player player, Vector2 target)
      : base(false, true)
    {
      this.player = player;
      this.target = target;
    }

    public override void OnBegin(Level level)
    {
      level.RegisterAreaComplete();
      this.Add((Component) new Coroutine(this.Cutscene(level), true));
    }

    private IEnumerator Cutscene(Level level)
    {
      CS07_Ending cs07Ending = this;
      Audio.SetMusic((string) null, true, true);
      cs07Ending.player.StateMachine.State = 11;
      yield return (object) cs07Ending.player.DummyWalkTo((float) cs07Ending.target.X, false, 1f, false);
      yield return (object) 0.25f;
      cs07Ending.Add((Component) new Coroutine(CutsceneEntity.CameraTo(Vector2.op_Addition(cs07Ending.target, new Vector2(-160f, -130f)), 3f, Ease.CubeInOut, 0.0f), true));
      cs07Ending.player.Facing = Facings.Right;
      yield return (object) 1f;
      cs07Ending.player.Sprite.Play("idle", false, false);
      cs07Ending.player.DummyAutoAnimate = false;
      cs07Ending.player.Dashes = 1;
      level.Session.Inventory.Dashes = 1;
      level.Add((Entity) (cs07Ending.badeline = new BadelineDummy(cs07Ending.player.Center)));
      cs07Ending.player.CreateSplitParticles();
      Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
      cs07Ending.Level.Displacement.AddBurst(cs07Ending.player.Center, 0.4f, 8f, 32f, 0.5f, (Ease.Easer) null, (Ease.Easer) null);
      cs07Ending.badeline.Sprite.Scale.X = (__Null) 1.0;
      Audio.Play("event:/char/badeline/maddy_split", cs07Ending.player.Position);
      yield return (object) cs07Ending.badeline.FloatTo(Vector2.op_Addition(cs07Ending.target, new Vector2(-10f, -30f)), new int?(1), false, false);
      yield return (object) 0.5f;
      yield return (object) Textbox.Say("CH7_ENDING", new Func<IEnumerator>(cs07Ending.WaitABit), new Func<IEnumerator>(cs07Ending.SitDown), new Func<IEnumerator>(cs07Ending.BadelineApproaches));
      yield return (object) 1f;
      cs07Ending.EndCutscene(level, true);
    }

    private IEnumerator WaitABit()
    {
      yield return (object) 3f;
    }

    private IEnumerator SitDown()
    {
      yield return (object) 0.5f;
      this.player.DummyAutoAnimate = true;
      yield return (object) this.player.DummyWalkTo(this.player.X + 16f, false, 0.25f, false);
      yield return (object) 0.1f;
      this.player.DummyAutoAnimate = false;
      this.player.Sprite.Play("sitDown", false, false);
      yield return (object) 1f;
    }

    private IEnumerator BadelineApproaches()
    {
      CS07_Ending cs07Ending = this;
      yield return (object) 0.5f;
      cs07Ending.badeline.Sprite.Scale.X = (__Null) -1.0;
      yield return (object) 1f;
      cs07Ending.badeline.Sprite.Scale.X = (__Null) 1.0;
      yield return (object) 1f;
      cs07Ending.Add((Component) new Coroutine(CutsceneEntity.CameraTo(Vector2.op_Addition(cs07Ending.Level.Camera.Position, new Vector2(88f, 0.0f)), 6f, Ease.CubeInOut, 0.0f), true));
      cs07Ending.badeline.FloatSpeed = 40f;
      yield return (object) cs07Ending.badeline.FloatTo(new Vector2(cs07Ending.player.X - 10f, cs07Ending.player.Y - 4f), new int?(), true, false);
      yield return (object) 0.5f;
    }

    public override void OnEnd(Level level)
    {
      Audio.SetMusic((string) null, true, true);
      ScreenWipe screenWipe = level.CompleteArea(false, false);
      if (screenWipe == null)
        return;
      screenWipe.Duration = 2f;
      screenWipe.EndTimer = 1f;
    }
  }
}
