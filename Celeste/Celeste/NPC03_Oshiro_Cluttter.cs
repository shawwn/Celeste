// Decompiled with JetBrains decompiler
// Type: Celeste.NPC03_Oshiro_Cluttter
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
  public class NPC03_Oshiro_Cluttter : NPC
  {
    private int sectionsComplete = 0;
    private bool talked = false;
    private bool inRoutine = false;
    private List<Vector2> nodes = new List<Vector2>();
    public const string TalkFlagsA = "oshiro_clutter_";
    public const string TalkFlagsB = "oshiro_clutter_optional_";
    public const string ClearedFlags = "oshiro_clutter_cleared_";
    public const string FinishedFlag = "oshiro_clutter_finished";
    public const string DoorOpenFlag = "oshiro_clutter_door_open";
    public Vector2 HomePosition;
    private Coroutine paceRoutine;
    private Coroutine talkRoutine;
    private SoundSource paceSfx;
    private float paceTimer;

    public NPC03_Oshiro_Cluttter(EntityData data, Vector2 offset)
      : base(data.Position + offset)
    {
      this.Add((Component) (this.Sprite = (Sprite) new OshiroSprite(-1)));
      this.Add((Component) (this.Talker = new TalkComponent(new Rectangle(-24, -8, 48, 8), new Vector2(0.0f, -24f), new Action<Player>(this.OnTalk), (TalkComponent.HoverDisplay) null)));
      this.Add((Component) (this.Light = new VertexLight(-Vector2.UnitY * 16f, Color.White, 1f, 32, 64)));
      this.MoveAnim = "move";
      this.IdleAnim = "idle";
      foreach (Vector2 node in data.Nodes)
        this.nodes.Add(node + offset);
      this.Add((Component) (this.paceSfx = new SoundSource()));
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      if (this.Session.GetFlag("oshiro_clutter_finished"))
      {
        this.RemoveSelf();
      }
      else
      {
        if (this.Session.GetFlag("oshiro_clutter_cleared_0"))
          ++this.sectionsComplete;
        if (this.Session.GetFlag("oshiro_clutter_cleared_1"))
          ++this.sectionsComplete;
        if (this.Session.GetFlag("oshiro_clutter_cleared_2"))
          ++this.sectionsComplete;
        if (this.sectionsComplete == 0 || this.sectionsComplete == 3)
          this.Sprite.Scale.X = 1f;
        if (this.sectionsComplete > 0)
          this.Position = this.nodes[this.sectionsComplete - 1];
        else if (!this.Session.GetFlag("oshiro_clutter_0"))
          this.Add((Component) (this.paceRoutine = new Coroutine(this.Pace(), true)));
        if (this.sectionsComplete == 0 && this.Session.GetFlag("oshiro_clutter_0") && !this.Session.GetFlag("oshiro_clutter_optional_0"))
          this.Sprite.Play("idle_ground", false, false);
        if (this.sectionsComplete == 3 || this.Session.GetFlag("oshiro_clutter_optional_" + (object) this.sectionsComplete))
          this.Remove((Component) this.Talker);
      }
      this.HomePosition = this.Position;
    }

    public Vector2 ZoomPoint
    {
      get
      {
        if (this.sectionsComplete < 2)
          return this.Position + new Vector2(0.0f, -30f) - this.Level.Camera.Position;
        return this.Position + new Vector2(0.0f, -15f) - this.Level.Camera.Position;
      }
    }

    private void OnTalk(Player player)
    {
      this.talked = true;
      if (this.paceRoutine != null)
        this.paceRoutine.RemoveSelf();
      this.paceRoutine = (Coroutine) null;
      if (!this.Session.GetFlag("oshiro_clutter_" + (object) this.sectionsComplete))
      {
        this.Scene.Add((Entity) new CS03_OshiroClutter(player, this, this.sectionsComplete));
      }
      else
      {
        this.Level.StartCutscene(new Action<Level>(this.EndTalkRoutine), true, false);
        this.Session.SetFlag("oshiro_clutter_optional_" + (object) this.sectionsComplete, true);
        this.Add((Component) (this.talkRoutine = new Coroutine(this.TalkRoutine(player), true)));
        if (this.Talker != null)
          this.Talker.Enabled = false;
      }
    }

    private IEnumerator TalkRoutine(Player player)
    {
      yield return (object) this.PlayerApproach(player, true, new float?(24f), new int?(this.sectionsComplete == 1 || this.sectionsComplete == 2 ? -1 : 1));
      yield return (object) this.Level.ZoomTo(this.ZoomPoint, 2f, 0.5f);
      yield return (object) Textbox.Say("CH3_OSHIRO_CLUTTER" + (object) this.sectionsComplete + "_B", new Func<IEnumerator>(this.StandUp));
      yield return (object) this.Level.ZoomBack(0.5f);
      this.Level.EndCutscene();
      this.EndTalkRoutine(this.Level);
    }

    private void EndTalkRoutine(Level level)
    {
      if (this.talkRoutine != null)
        this.talkRoutine.RemoveSelf();
      this.talkRoutine = (Coroutine) null;
      (this.Sprite as OshiroSprite).Pop("idle", false);
      Player entity = this.Scene.Tracker.GetEntity<Player>();
      if (entity == null)
        return;
      entity.StateMachine.Locked = false;
      entity.StateMachine.State = Player.StNormal;
    }

    private IEnumerator StandUp()
    {
      Audio.Play("event:/char/oshiro/chat_get_up", this.Position);
      (this.Sprite as OshiroSprite).Pop("idle", false);
      yield return (object) 0.25f;
    }

    private IEnumerator Pace()
    {
      while (true)
      {
        (this.Sprite as OshiroSprite).Wiggle();
        yield return (object) this.PaceLeft();
        while ((double) this.paceTimer < 2.26600003242493)
          yield return (object) null;
        this.paceTimer = 0.0f;
        (this.Sprite as OshiroSprite).Wiggle();
        yield return (object) this.PaceRight();
        while ((double) this.paceTimer < 2.26600003242493)
          yield return (object) null;
        this.paceTimer = 0.0f;
      }
    }

    public IEnumerator PaceRight()
    {
      Vector2 target = this.HomePosition;
      if ((double) (this.Position - target).Length() > 8.0)
        this.paceSfx.Play("event:/char/oshiro/move_04_pace_right", (string) null, 0.0f);
      yield return (object) this.MoveTo(target, false, new int?(), false);
    }

    public IEnumerator PaceLeft()
    {
      Vector2 target = this.HomePosition + new Vector2(-20f, 0.0f);
      if ((double) (this.Position - target).Length() > 8.0)
        this.paceSfx.Play("event:/char/oshiro/move_04_pace_left", (string) null, 0.0f);
      yield return (object) this.MoveTo(target, false, new int?(), false);
    }

    public override void Update()
    {
      base.Update();
      this.paceTimer += Engine.DeltaTime;
      Player entity = this.Scene.Tracker.GetEntity<Player>();
      if (this.sectionsComplete == 3 && !this.inRoutine && (entity != null && (double) entity.X < (double) this.X + 32.0) && (double) entity.Y <= (double) this.Y)
      {
        this.OnTalk(entity);
        this.inRoutine = true;
      }
      if (this.sectionsComplete != 0 || this.talked)
        return;
      Level scene = this.Scene as Level;
      if (entity != null && !entity.Dead)
      {
        float num = Calc.ClampedMap(Vector2.Distance(this.Center, entity.Center), 40f, 128f, 0.0f, 1f);
        scene.Session.Audio.Music.Layer(1, num);
        scene.Session.Audio.Music.Layer(2, 1f - num);
        scene.Session.Audio.Apply();
      }
      else
      {
        scene.Session.Audio.Music.Layer(1, true);
        scene.Session.Audio.Music.Layer(2, false);
        scene.Session.Audio.Apply();
      }
    }
  }
}

