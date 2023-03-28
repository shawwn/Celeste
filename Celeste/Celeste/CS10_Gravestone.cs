// Decompiled with JetBrains decompiler
// Type: Celeste.CS10_Gravestone
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class CS10_Gravestone : CutsceneEntity
  {
    private Player player;
    private NPC10_Gravestone gravestone;
    private BadelineDummy badeline;
    private BirdNPC bird;
    private Vector2 boostTarget;
    private bool addedBooster;

    public CS10_Gravestone(Player player, NPC10_Gravestone gravestone, Vector2 boostTarget)
      : base()
    {
      this.player = player;
      this.gravestone = gravestone;
      this.boostTarget = boostTarget;
    }

    public override void OnBegin(Level level) => this.Add((Component) new Coroutine(this.Cutscene()));

    private IEnumerator Cutscene()
    {
      CS10_Gravestone cs10Gravestone = this;
      cs10Gravestone.player.StateMachine.State = 11;
      cs10Gravestone.player.ForceCameraUpdate = true;
      cs10Gravestone.player.DummyGravity = false;
      cs10Gravestone.player.Speed.Y = 0.0f;
      yield return (object) 0.1f;
      yield return (object) cs10Gravestone.player.DummyWalkToExact((int) cs10Gravestone.gravestone.X - 30);
      yield return (object) 0.1f;
      cs10Gravestone.player.Facing = Facings.Right;
      yield return (object) 0.2f;
      yield return (object) cs10Gravestone.Level.ZoomTo(new Vector2(160f, 90f), 2f, 3f);
      cs10Gravestone.player.ForceCameraUpdate = false;
      yield return (object) 0.5f;
      yield return (object) Textbox.Say("CH9_GRAVESTONE", new Func<IEnumerator>(cs10Gravestone.StepForward), new Func<IEnumerator>(cs10Gravestone.BadelineAppears), new Func<IEnumerator>(cs10Gravestone.SitDown));
      yield return (object) 1f;
      yield return (object) cs10Gravestone.BirdStuff();
      yield return (object) cs10Gravestone.BadelineRejoin();
      yield return (object) 0.1f;
      yield return (object) cs10Gravestone.Level.ZoomBack(0.5f);
      yield return (object) 0.3f;
      cs10Gravestone.addedBooster = true;
      cs10Gravestone.Level.Displacement.AddBurst(cs10Gravestone.boostTarget, 0.5f, 8f, 32f, 0.5f);
      Audio.Play("event:/new_content/char/badeline/booster_first_appear", cs10Gravestone.boostTarget);
      cs10Gravestone.Level.Add((Entity) new BadelineBoost(new Vector2[1]
      {
        cs10Gravestone.boostTarget
      }, false));
      yield return (object) 0.2f;
      cs10Gravestone.EndCutscene(cs10Gravestone.Level);
    }

    private IEnumerator StepForward()
    {
      yield return (object) this.player.DummyWalkTo(this.player.X + 8f);
    }

    // private IEnumerator BadelineAppears()
    // {
    //   // ISSUE: reference to a compiler-generated field
    //   int num = this.\u003C\u003E1__state;
    //   CS10_Gravestone cs10Gravestone = this;
    //   if (num != 0)
    //   {
    //     if (num != 1)
    //       return false;
    //     // ISSUE: reference to a compiler-generated field
    //     this.\u003C\u003E1__state = -1;
    //     return false;
    //   }
    //   // ISSUE: reference to a compiler-generated field
    //   this.\u003C\u003E1__state = -1;
    //   cs10Gravestone.Level.Session.Inventory.Dashes = 1;
    //   cs10Gravestone.player.Dashes = 1;
    //   Vector2 position = cs10Gravestone.player.Position + new Vector2(-12f, -10f);
    //   cs10Gravestone.Level.Displacement.AddBurst(position, 0.5f, 8f, 32f, 0.5f);
    //   cs10Gravestone.Level.Add((Entity) (cs10Gravestone.badeline = new BadelineDummy(position)));
    //   Audio.Play("event:/char/badeline/maddy_split", position);
    //   cs10Gravestone.badeline.Sprite.Scale.X = 1f;
    //   // ISSUE: reference to a compiler-generated field
    //   this.\u003C\u003E2__current = (object) cs10Gravestone.badeline.FloatTo(position + new Vector2(0.0f, -6f), new int?(1), false);
    //   // ISSUE: reference to a compiler-generated field
    //   this.\u003C\u003E1__state = 1;
    //   return true;
    // }
    private IEnumerator BadelineAppears()
    {
      Level.Session.Inventory.Dashes = 1;
      player.Dashes = 1;
      Vector2 position = player.Position + new Vector2(-12f, -10f);
      Level.Displacement.AddBurst(position, 0.5f, 8f, 32f, 0.5f);
      Level.Add((Entity) (badeline = new BadelineDummy(position)));
      Audio.Play("event:/char/badeline/maddy_split", position);
      badeline.Sprite.Scale.X = 1f;
      yield return badeline.FloatTo(position + new Vector2(0.0f, -6f), new int?(1), false);
    }

    private IEnumerator SitDown()
    {
      yield return (object) 0.2f;
      this.player.DummyAutoAnimate = false;
      this.player.Sprite.Play("sitDown");
      yield return (object) 0.3f;
    }

    private IEnumerator BirdStuff()
    {
      CS10_Gravestone cs10Gravestone = this;
      cs10Gravestone.bird = new BirdNPC(cs10Gravestone.player.Position + new Vector2(88f, -200f), BirdNPC.Modes.None);
      cs10Gravestone.bird.DisableFlapSfx = true;
      cs10Gravestone.Scene.Add((Entity) cs10Gravestone.bird);
      EventInstance instance = Audio.Play("event:/game/general/bird_in", cs10Gravestone.bird.Position);
      cs10Gravestone.bird.Facing = Facings.Left;
      cs10Gravestone.bird.Sprite.Play("fall");
      Vector2 from = cs10Gravestone.bird.Position;
      Vector2 to = cs10Gravestone.gravestone.Position + new Vector2(1f, -16f);
      float percent = 0.0f;
      while ((double) percent < 1.0)
      {
        cs10Gravestone.bird.Position = from + (to - from) * Ease.QuadOut(percent);
        Audio.Position(instance, cs10Gravestone.bird.Position);
        if ((double) percent > 0.5)
          cs10Gravestone.bird.Sprite.Play("fly");
        percent += Engine.DeltaTime * 0.5f;
        yield return (object) null;
      }
      cs10Gravestone.bird.Position = to;
      cs10Gravestone.bird.Sprite.Play("idle");
      yield return (object) 0.5f;
      cs10Gravestone.bird.Sprite.Play("croak");
      yield return (object) 0.6f;
      Audio.Play("event:/game/general/bird_squawk", cs10Gravestone.bird.Position);
      yield return (object) 0.9f;
      Audio.Play("event:/char/madeline/stand", cs10Gravestone.player.Position);
      cs10Gravestone.player.Sprite.Play("idle");
      yield return (object) 1f;
      yield return (object) cs10Gravestone.bird.StartleAndFlyAway();
    }

    private IEnumerator BadelineRejoin()
    {
      CS10_Gravestone cs10Gravestone = this;
      Audio.Play("event:/new_content/char/badeline/maddy_join_quick", cs10Gravestone.badeline.Position);
      Vector2 from = cs10Gravestone.badeline.Position;
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime / 0.25f)
      {
        cs10Gravestone.badeline.Position = Vector2.Lerp(from, cs10Gravestone.player.Position, Ease.CubeIn(p));
        yield return (object) null;
      }
      cs10Gravestone.Level.Displacement.AddBurst(cs10Gravestone.player.Center, 0.5f, 8f, 32f, 0.5f);
      cs10Gravestone.Level.Session.Inventory.Dashes = 2;
      cs10Gravestone.player.Dashes = 2;
      cs10Gravestone.badeline.RemoveSelf();
    }

    public override void OnEnd(Level level)
    {
      this.player.Facing = Facings.Right;
      this.player.DummyAutoAnimate = true;
      this.player.DummyGravity = true;
      this.player.StateMachine.State = 0;
      this.Level.Session.Inventory.Dashes = 2;
      this.player.Dashes = 2;
      if (this.badeline != null)
        this.badeline.RemoveSelf();
      if (this.bird != null)
        this.bird.RemoveSelf();
      if (!this.addedBooster)
        level.Add((Entity) new BadelineBoost(new Vector2[1]
        {
          this.boostTarget
        }, false));
      level.ResetZoom();
    }
  }
}
