// Decompiled with JetBrains decompiler
// Type: Celeste.NPC03_Theo_Vents
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class NPC03_Theo_Vents : NPC
  {
    private const string AppeardFlag = "theoVentsAppeared";
    private const string TalkedFlag = "theoVentsTalked";
    private const int SpriteAppearY = -8;
    private float particleDelay;
    private bool appeared;
    private NPC03_Theo_Vents.Grate grate;

    public NPC03_Theo_Vents(Vector2 position)
      : base(position)
    {
      this.Tag = (int) Tags.TransitionUpdate;
      this.Add((Component) (this.Sprite = GFX.SpriteBank.Create("theo")));
      this.Sprite.Scale.Y = (__Null) -1.0;
      this.Sprite.Scale.X = (__Null) -1.0;
      this.Visible = false;
      this.Maxspeed = 48f;
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      if (this.Session.GetFlag("theoVentsTalked"))
        this.RemoveSelf();
      else
        this.Add((Component) new Coroutine(this.Appear(), true));
    }

    public override void Update()
    {
      base.Update();
      if (this.appeared)
        return;
      this.particleDelay -= Engine.DeltaTime;
      if ((double) this.particleDelay > 0.0)
        return;
      this.Level.ParticlesFG.Emit(ParticleTypes.VentDust, 8, this.Position, new Vector2(6f, 0.0f));
      if (this.grate != null)
        this.grate.Shake();
      this.particleDelay = Calc.Random.Choose<float>(1f, 2f, 3f);
    }

    private IEnumerator Appear()
    {
      NPC03_Theo_Vents npC03TheoVents = this;
      if (!npC03TheoVents.Session.GetFlag("theoVentsAppeared"))
      {
        npC03TheoVents.grate = new NPC03_Theo_Vents.Grate(npC03TheoVents.Position);
        npC03TheoVents.Scene.Add((Entity) npC03TheoVents.grate);
        Player entity;
        do
        {
          yield return (object) null;
          entity = npC03TheoVents.Scene.Tracker.GetEntity<Player>();
        }
        while (entity == null || (double) entity.X <= (double) npC03TheoVents.X - 32.0);
        Audio.Play("event:/char/theo/resort_ceilingvent_hey", npC03TheoVents.Position);
        npC03TheoVents.Level.ParticlesFG.Emit(ParticleTypes.VentDust, 24, npC03TheoVents.Position, new Vector2(6f, 0.0f));
        npC03TheoVents.grate.Fall();
        int from = -24;
        for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime * 2f)
        {
          yield return (object) null;
          npC03TheoVents.Visible = true;
          npC03TheoVents.Sprite.Y = (float) from + (float) (-8 - from) * Ease.CubeOut(p);
        }
        npC03TheoVents.Session.SetFlag("theoVentsAppeared", true);
      }
      npC03TheoVents.appeared = true;
      npC03TheoVents.Sprite.Y = -8f;
      npC03TheoVents.Visible = true;
      npC03TheoVents.Add((Component) (npC03TheoVents.Talker = new TalkComponent(new Rectangle(-16, 0, 32, 100), new Vector2(0.0f, -8f), new Action<Player>(npC03TheoVents.OnTalk), (TalkComponent.HoverDisplay) null)));
    }

    private void OnTalk(Player player)
    {
      this.Level.StartCutscene(new Action<Level>(this.OnTalkEnd), true, false);
      this.Add((Component) new Coroutine(this.Talk(player), true));
    }

    private IEnumerator Talk(Player player)
    {
      NPC03_Theo_Vents npC03TheoVents = this;
      yield return (object) npC03TheoVents.PlayerApproach(player, true, new float?(10f), new int?(-1));
      player.DummyAutoAnimate = false;
      player.Sprite.Play("lookUp", false, false);
      Rectangle bounds1 = npC03TheoVents.Level.Bounds;
      double num = (double) (((Rectangle) ref bounds1).get_Right() - 320);
      Rectangle bounds2 = npC03TheoVents.Level.Bounds;
      double top = (double) ((Rectangle) ref bounds2).get_Top();
      yield return (object) CutsceneEntity.CameraTo(new Vector2((float) num, (float) top), 0.5f, (Ease.Easer) null, 0.0f);
      yield return (object) npC03TheoVents.Level.ZoomTo(new Vector2(240f, 70f), 2f, 0.5f);
      yield return (object) Textbox.Say("CH3_THEO_VENTS");
      yield return (object) npC03TheoVents.Disappear();
      yield return (object) 0.25f;
      yield return (object) npC03TheoVents.Level.ZoomBack(0.5f);
      npC03TheoVents.Level.EndCutscene();
      npC03TheoVents.OnTalkEnd(npC03TheoVents.Level);
    }

    private void OnTalkEnd(Level level)
    {
      Player entity = this.Scene.Tracker.GetEntity<Player>();
      if (entity != null)
      {
        entity.DummyAutoAnimate = true;
        entity.StateMachine.Locked = false;
        entity.StateMachine.State = 0;
      }
      this.Session.SetFlag("theoVentsTalked", true);
      this.RemoveSelf();
    }

    private IEnumerator Disappear()
    {
      NPC03_Theo_Vents npC03TheoVents = this;
      Audio.Play("event:/char/theo/resort_ceilingvent_seeya", npC03TheoVents.Position);
      int to = -24;
      float from = npC03TheoVents.Sprite.Y;
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime * 2f)
      {
        yield return (object) null;
        npC03TheoVents.Level.ParticlesFG.Emit(ParticleTypes.VentDust, 1, npC03TheoVents.Position, new Vector2(6f, 0.0f));
        npC03TheoVents.Sprite.Y = from + ((float) to - from) * Ease.BackIn(p);
      }
    }

    private class Grate : Entity
    {
      private float alpha = 1f;
      private Monocle.Image sprite;
      private float shake;
      private Vector2 speed;
      private bool falling;

      public Grate(Vector2 position)
        : base(position)
      {
        this.Add((Component) (this.sprite = new Monocle.Image(GFX.Game["scenery/grate"])));
        this.sprite.JustifyOrigin(0.5f, 0.0f);
      }

      public void Shake()
      {
        if (this.falling)
          return;
        Audio.Play("event:/char/theo/resort_ceilingvent_shake", this.Position);
        this.shake = 0.5f;
      }

      public void Fall()
      {
        Audio.Play("event:/char/theo/resort_ceilingvent_popoff", this.Position);
        this.falling = true;
        this.speed = new Vector2(40f, 200f);
        this.Collider = (Collider) new Hitbox(2f, 2f, -1f, 0.0f);
      }

      public override void Update()
      {
        if ((double) this.shake > 0.0)
        {
          this.shake -= Engine.DeltaTime;
          if (this.Scene.OnInterval(0.05f))
            this.sprite.X = 1f - this.sprite.X;
        }
        if (this.falling)
        {
          this.speed.X = (__Null) (double) Calc.Approach((float) this.speed.X, 0.0f, Engine.DeltaTime * 80f);
          ref __Null local = ref this.speed.Y;
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          ^(float&) ref local = ^(float&) ref local + 200f * Engine.DeltaTime;
          this.Position = Vector2.op_Addition(this.Position, Vector2.op_Multiply(this.speed, Engine.DeltaTime));
          if (this.CollideCheck<Solid>(Vector2.op_Addition(this.Position, new Vector2(0.0f, 2f))) && this.speed.Y > 0.0)
            this.speed.Y = (__Null) (-this.speed.Y * 0.25);
          this.alpha -= Engine.DeltaTime;
          this.sprite.Rotation += Engine.DeltaTime;
          this.sprite.Color = Color.op_Multiply(Color.get_White(), this.alpha);
          if ((double) this.alpha <= 0.0)
            this.RemoveSelf();
        }
        base.Update();
      }
    }
  }
}
