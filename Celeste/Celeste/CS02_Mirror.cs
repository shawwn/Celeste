// Decompiled with JetBrains decompiler
// Type: Celeste.CS02_Mirror
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
  public class CS02_Mirror : CutsceneEntity
  {
    private int direction = 1;
    private Player player;
    private DreamMirror mirror;
    private float playerEndX;
    private SoundSource sfx;

    public CS02_Mirror(Player player, DreamMirror mirror)
      : base(true, false)
    {
      this.player = player;
      this.mirror = mirror;
    }

    public override void OnBegin(Level level)
    {
      this.Add((Component) new Coroutine(this.Cutscene(level), true));
    }

    private IEnumerator Cutscene(Level level)
    {
      this.Add((Component) (this.sfx = new SoundSource()));
      this.sfx.Position = this.mirror.Center;
      this.sfx.Play("event:/music/lvl2/dreamblock_sting_pt1", (string) null, 0.0f);
      this.direction = Math.Sign(this.player.X - this.mirror.X);
      this.player.StateMachine.State = Player.StDummy;
      this.playerEndX = (float) (8 * this.direction);
      yield return (object) 1f;
      this.player.Facing = ToFacing.Convert(-this.direction);
      yield return (object) 0.4f;
      yield return (object) this.player.DummyRunTo(this.mirror.X + this.playerEndX, false);
      yield return (object) 0.5f;
      yield return (object) level.ZoomTo(this.mirror.Position - level.Camera.Position - Vector2.UnitY * 24f, 2f, 1f);
      yield return (object) 0.5f;
      yield return (object) this.mirror.BreakRoutine(this.direction);
      this.player.DummyAutoAnimate = false;
      this.player.Sprite.Play("lookUp", false, false);
      Vector2 from = level.Camera.Position;
      Vector2 to = level.Camera.Position + new Vector2(0.0f, -80f);
      for (float ease = 0.0f; (double) ease < 1.0; ease += Engine.DeltaTime * 1.2f)
      {
        level.Camera.Position = from + (to - from) * Ease.CubeInOut(ease);
        yield return (object) null;
      }
      this.Add((Component) new Coroutine(this.ZoomBack(), true));
      List<Entity>.Enumerator enumerator = this.Scene.Tracker.GetEntities<DreamBlock>().GetEnumerator();
      try
      {
        if (enumerator.MoveNext())
        {
          DreamBlock block = (DreamBlock) enumerator.Current;
          yield return (object) block.Activate();
        }
      }
      finally
      {
        enumerator.Dispose();
      }
      enumerator = new List<Entity>.Enumerator();
      from = new Vector2();
      to = new Vector2();
      yield return (object) 0.5f;
      this.EndCutscene(level, true);
    }

    private IEnumerator ZoomBack()
    {
      yield return (object) 1.2f;
      yield return (object) this.Level.ZoomBack(3f);
    }

    public override void OnEnd(Level level)
    {
      this.mirror.Broken(this.WasSkipped);
      Player entity1 = this.Scene.Tracker.GetEntity<Player>();
      if (entity1 != null)
      {
        entity1.StateMachine.State = Player.StNormal;
        entity1.DummyAutoAnimate = true;
        entity1.Speed = Vector2.Zero;
        entity1.X = this.mirror.X + this.playerEndX;
        entity1.Facing = (uint) this.direction <= 0U ? Facings.Right : ToFacing.Convert(-this.direction);
      }
      foreach (DreamBlock entity2 in this.Scene.Tracker.GetEntities<DreamBlock>())
        entity2.ActivateNoRoutine();
      level.ResetZoom();
      level.Session.Inventory.DreamDash = true;
      level.Session.Audio.Music.Event = "event:/music/lvl2/mirror";
      level.Session.Audio.Apply();
    }
  }
}

