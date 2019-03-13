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
    private float particleDelay = 0.0f;
    private bool appeared = false;
    private const string AppeardFlag = "theoVentsAppeared";
    private const string TalkedFlag = "theoVentsTalked";
    private const int SpriteAppearY = -8;
    private NPC03_Theo_Vents.Grate grate;

    public NPC03_Theo_Vents(Vector2 position)
      : base(position)
    {
      this.Tag = (int) Tags.TransitionUpdate;
      this.Add((Component) (this.Sprite = GFX.SpriteBank.Create("theo")));
      this.Sprite.Scale.Y = -1f;
      this.Sprite.Scale.X = -1f;
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
      if ((double) this.particleDelay <= 0.0)
      {
        this.Level.ParticlesFG.Emit(ParticleTypes.VentDust, 8, this.Position, new Vector2(6f, 0.0f));
        if (this.grate != null)
          this.grate.Shake();
        this.particleDelay = Calc.Random.Choose<float>(1f, 2f, 3f);
      }
    }

    private IEnumerator Appear()
    {
      if (!this.Session.GetFlag("theoVentsAppeared"))
      {
        this.grate = new NPC03_Theo_Vents.Grate(this.Position);
        this.Scene.Add((Entity) this.grate);
        while (true)
        {
          yield return (object) null;
          Player player = this.Scene.Tracker.GetEntity<Player>();
          if (player == null || (double) player.X <= (double) this.X - 32.0)
            player = (Player) null;
          else
            break;
        }
        Audio.Play("event:/char/theo/resort_ceilingvent_hey", this.Position);
        this.Level.ParticlesFG.Emit(ParticleTypes.VentDust, 24, this.Position, new Vector2(6f, 0.0f));
        this.grate.Fall();
        int from = -24;
        for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime * 2f)
        {
          yield return (object) null;
          this.Visible = true;
          this.Sprite.Y = (float) from + (float) (-8 - from) * Ease.CubeOut(p);
        }
        this.Session.SetFlag("theoVentsAppeared", true);
      }
      this.appeared = true;
      this.Sprite.Y = -8f;
      this.Visible = true;
      this.Add((Component) (this.Talker = new TalkComponent(new Rectangle(-16, 0, 32, 100), new Vector2(0.0f, -8f), new Action<Player>(this.OnTalk), (TalkComponent.HoverDisplay) null)));
    }

    private void OnTalk(Player player)
    {
      this.Level.StartCutscene(new Action<Level>(this.OnTalkEnd), true, false);
      this.Add((Component) new Coroutine(this.Talk(player), true));
    }

    private IEnumerator Talk(Player player)
    {
      yield return (object) this.PlayerApproach(player, true, new float?(10f), new int?(-1));
      player.DummyAutoAnimate = false;
      player.Sprite.Play("lookUp", false, false);
      Rectangle bounds = this.Level.Bounds;
      double num = (double) (bounds.Right - 320);
      bounds = this.Level.Bounds;
      double top = (double) bounds.Top;
      yield return (object) CutsceneEntity.CameraTo(new Vector2((float) num, (float) top), 0.5f, (Ease.Easer) null, 0.0f);
      yield return (object) this.Level.ZoomTo(new Vector2(240f, 70f), 2f, 0.5f);
      yield return (object) Textbox.Say("CH3_THEO_VENTS");
      yield return (object) this.Disappear();
      yield return (object) 0.25f;
      yield return (object) this.Level.ZoomBack(0.5f);
      this.Level.EndCutscene();
      this.OnTalkEnd(this.Level);
    }

    private void OnTalkEnd(Level level)
    {
      Player entity = this.Scene.Tracker.GetEntity<Player>();
      if (entity != null)
      {
        entity.DummyAutoAnimate = true;
        entity.StateMachine.Locked = false;
        entity.StateMachine.State = Player.StNormal;
      }
      this.Session.SetFlag("theoVentsTalked", true);
      this.RemoveSelf();
    }

    private IEnumerator Disappear()
    {
      Audio.Play("event:/char/theo/resort_ceilingvent_seeya", this.Position);
      int to = -24;
      float from = this.Sprite.Y;
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime * 2f)
      {
        yield return (object) null;
        this.Level.ParticlesFG.Emit(ParticleTypes.VentDust, 1, this.Position, new Vector2(6f, 0.0f));
        this.Sprite.Y = from + ((float) to - from) * Ease.BackIn(p);
      }
    }

    private class Grate : Entity
    {
      private float shake = 0.0f;
      private float alpha = 1f;
      private Monocle.Image sprite;
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
          this.speed.X = Calc.Approach(this.speed.X, 0.0f, Engine.DeltaTime * 80f);
          this.speed.Y += 200f * Engine.DeltaTime;
          this.Position = this.Position + this.speed * Engine.DeltaTime;
          if (this.CollideCheck<Solid>(this.Position + new Vector2(0.0f, 2f)) && (double) this.speed.Y > 0.0)
            this.speed.Y = (float) (-(double) this.speed.Y * 0.25);
          this.alpha -= Engine.DeltaTime;
          this.sprite.Rotation += Engine.DeltaTime;
          this.sprite.Color = Color.White * this.alpha;
          if ((double) this.alpha <= 0.0)
            this.RemoveSelf();
        }
        base.Update();
      }
    }
  }
}

