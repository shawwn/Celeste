// Decompiled with JetBrains decompiler
// Type: Celeste.NPC09_Granny_Outside
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class NPC09_Granny_Outside : NPC
  {
    public const string Flag = "granny_outside";
    public Hahaha Hahaha;
    public GrannyLaughSfx LaughSfx;
    private bool talking;
    private Player player;
    private bool leaving;

    public NPC09_Granny_Outside(EntityData data, Vector2 offset)
      : base(data.Position + offset)
    {
      this.Add((Component) (this.Sprite = GFX.SpriteBank.Create("granny")));
      this.Sprite.Play("idle", false, false);
      this.Add((Component) (this.LaughSfx = new GrannyLaughSfx(this.Sprite)));
      this.MoveAnim = "walk";
      this.IdleAnim = "idle";
      this.Maxspeed = 40f;
      this.SetupGrannySpriteSounds();
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      if ((scene as Level).Session.GetFlag("granny_outside"))
        this.RemoveSelf();
      scene.Add((Entity) (this.Hahaha = new Hahaha(this.Position + new Vector2(8f, -4f), "", false, new Vector2?())));
      this.Hahaha.Enabled = false;
    }

    public override void Update()
    {
      if (!this.talking)
      {
        this.player = this.Level.Tracker.GetEntity<Player>();
        if (this.player != null && (double) this.player.X > (double) this.X - 48.0)
        {
          (this.Scene as Level).StartCutscene(new Action<Level>(this.EndTalking), true, false);
          this.Add((Component) new Coroutine(this.TalkRoutine(this.player), true));
          this.talking = true;
        }
      }
      this.Hahaha.Enabled = this.Sprite.CurrentAnimationID == "laugh";
      base.Update();
    }

    private IEnumerator TalkRoutine(Player player)
    {
      player.StateMachine.State = Player.StDummy;
      while (!player.OnGround(1))
        yield return (object) null;
      this.Sprite.Scale.X = -1f;
      yield return (object) player.DummyWalkToExact((int) this.X - 16, false, 1f);
      yield return (object) 0.5f;
      yield return (object) this.Level.ZoomTo(new Vector2(200f, 110f), 2f, 0.5f);
      yield return (object) Textbox.Say("APP_OLDLADY_A", new Func<IEnumerator>(this.MoveRight), new Func<IEnumerator>(this.ExitRight));
      yield return (object) this.Level.ZoomBack(0.5f);
      this.Sprite.Scale.X = 1f;
      if (!this.leaving)
        yield return (object) this.ExitRight();
      while ((double) this.X < (double) (this.Level.Bounds.Right + 8))
        yield return (object) null;
      this.Level.EndCutscene();
      this.EndTalking(this.Level);
    }

    private IEnumerator MoveRight()
    {
      yield return (object) this.MoveTo(new Vector2(this.X + 8f, this.Y), false, new int?(), false);
    }

    private IEnumerator ExitRight()
    {
      this.leaving = true;
      this.Add((Component) new Coroutine(this.MoveTo(new Vector2((float) (this.Level.Bounds.Right + 16), this.Y), false, new int?(), false), true));
      yield return (object) null;
    }

    private void EndTalking(Level level)
    {
      if (this.player != null)
        this.player.StateMachine.State = Player.StNormal;
      this.Level.Session.SetFlag("granny_outside", true);
      this.RemoveSelf();
    }
  }
}

