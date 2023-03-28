// Decompiled with JetBrains decompiler
// Type: Celeste.FlingBirdIntro
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class FlingBirdIntro : Entity
  {
    public Vector2 BirdEndPosition;
    public Sprite Sprite;
    public SoundEmitter CrashSfxEmitter;
    private Vector2[] nodes;
    private bool startedRoutine;
    private Vector2 start;
    private InvisibleBarrier fakeRightWall;
    private bool crashes;
    private Coroutine flyToRoutine;
    private bool emitParticles;
    private bool inCutscene;

    public FlingBirdIntro(Vector2 position, Vector2[] nodes, bool crashes)
      : base(position)
    {
      this.crashes = crashes;
      this.Add((Component) (this.Sprite = GFX.SpriteBank.Create("bird")));
      this.Sprite.Play(crashes ? "hoverStressed" : "hover");
      this.Sprite.Scale.X = crashes ? -1f : 1f;
      this.Sprite.OnFrameChange = (Action<string>) (anim =>
      {
        if (this.inCutscene)
          return;
        BirdNPC.FlapSfxCheck(this.Sprite);
      });
      this.Collider = (Collider) new Monocle.Circle(16f, y: -8f);
      this.Add((Component) new PlayerCollider(new Action<Player>(this.OnPlayer)));
      this.nodes = nodes;
      this.start = position;
      this.BirdEndPosition = nodes[nodes.Length - 1];
    }

    public FlingBirdIntro(EntityData data, Vector2 levelOffset)
      : this(data.Position + levelOffset, data.NodesOffset(levelOffset), data.Bool(nameof (crashes)))
    {
    }

    public override void Awake(Scene scene)
    {
      base.Awake(scene);
      if (!this.crashes && (scene as Level).Session.GetFlag("MissTheBird"))
      {
        this.RemoveSelf();
      }
      else
      {
        Player entity1 = this.Scene.Tracker.GetEntity<Player>();
        if (entity1 != null && (double) entity1.X > (double) this.X)
        {
          if (this.crashes)
            CS10_CatchTheBird.HandlePostCutsceneSpawn(this, scene as Level);
          CassetteBlockManager entity2 = this.Scene.Tracker.GetEntity<CassetteBlockManager>();
          if (entity2 != null)
          {
            entity2.StopBlocks();
            entity2.Finish();
          }
          this.RemoveSelf();
        }
        else
          scene.Add((Entity) (this.fakeRightWall = new InvisibleBarrier(new Vector2(this.X + 160f, this.Y - 200f), 8f, 400f)));
        if (this.crashes)
          return;
        Vector2 position = this.Position;
        this.Position = new Vector2(this.X - 150f, (float) ((scene as Level).Bounds.Top - 8));
        this.Add((Component) (this.flyToRoutine = new Coroutine(this.FlyTo(position))));
      }
    }

    private IEnumerator FlyTo(Vector2 to)
    {
      FlingBirdIntro flingBirdIntro = this;
      flingBirdIntro.Add((Component) new SoundSource().Play("event:/new_content/game/10_farewell/bird_flappyscene_entry"));
      flingBirdIntro.Sprite.Play("fly");
      Vector2 from = flingBirdIntro.Position;
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime * 0.3f)
      {
        flingBirdIntro.Position = from + (to - from) * Ease.SineOut(p);
        yield return (object) null;
      }
      flingBirdIntro.Sprite.Play("hover");
      float sine = 0.0f;
      while (true)
      {
        flingBirdIntro.Position = to + Vector2.UnitY * (float) Math.Sin((double) sine) * 8f;
        sine += Engine.DeltaTime * 2f;
        yield return (object) null;
      }
    }

    public override void Removed(Scene scene)
    {
      if (this.fakeRightWall != null)
        this.fakeRightWall.RemoveSelf();
      this.fakeRightWall = (InvisibleBarrier) null;
      base.Removed(scene);
    }

    private void OnPlayer(Player player)
    {
      if (player.Dead || this.startedRoutine)
        return;
      if (this.flyToRoutine != null)
        this.flyToRoutine.RemoveSelf();
      this.startedRoutine = true;
      player.Speed = Vector2.Zero;
      this.Depth = player.Depth - 5;
      this.Sprite.Play("hoverStressed");
      this.Sprite.Scale.X = 1f;
      this.fakeRightWall.RemoveSelf();
      this.fakeRightWall = (InvisibleBarrier) null;
      if (!this.crashes)
      {
        this.Scene.Add((Entity) new CS10_MissTheBird(player, this));
      }
      else
      {
        CassetteBlockManager entity = this.Scene.Tracker.GetEntity<CassetteBlockManager>();
        if (entity != null)
        {
          entity.StopBlocks();
          entity.Finish();
        }
        this.Scene.Add((Entity) new CS10_CatchTheBird(player, this));
      }
    }

    public override void Update()
    {
      if (!this.startedRoutine && this.fakeRightWall != null)
      {
        Level scene = this.Scene as Level;
        if ((double) scene.Camera.X > (double) this.fakeRightWall.X - 320.0 - 16.0)
          scene.Camera.X = (float) ((double) this.fakeRightWall.X - 320.0 - 16.0);
      }
      if (this.emitParticles && this.Scene.OnInterval(0.1f))
        this.SceneAs<Level>().ParticlesBG.Emit(FlingBird.P_Feather, 1, this.Position + new Vector2(0.0f, -8f), new Vector2(6f, 4f));
      base.Update();
    }

    public IEnumerator DoGrabbingRoutine(Player player)
    {
      FlingBirdIntro follow = this;
      Level level = follow.Scene as Level;
      follow.inCutscene = true;
      follow.CrashSfxEmitter = follow.crashes ? SoundEmitter.Play("event:/new_content/game/10_farewell/bird_crashscene_start", (Entity) follow) : SoundEmitter.Play("event:/new_content/game/10_farewell/bird_flappyscene", (Entity) follow);
      player.StateMachine.State = 11;
      player.DummyGravity = false;
      player.DummyAutoAnimate = false;
      player.ForceCameraUpdate = true;
      player.Sprite.Play("jumpSlow_carry");
      player.Speed = Vector2.Zero;
      player.Facing = Facings.Right;
      Celeste.Freeze(0.1f);
      level.Shake();
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Short);
      follow.emitParticles = true;
      follow.Add((Component) new Coroutine(level.ZoomTo(new Vector2(140f, 120f), 1.5f, 4f)));
      float sin = 0.0f;
      int index = 0;
      while (index < follow.nodes.Length - 1)
      {
        Vector2 position = follow.Position;
        Vector2 node = follow.nodes[index];
        SimpleCurve curve = new SimpleCurve(position, node, position + (node - position) * 0.5f + new Vector2(0.0f, -24f));
        float duration = curve.GetLengthParametric(32) / 100f;
        if ((double) node.Y < (double) position.Y)
        {
          duration *= 1.1f;
          follow.Sprite.Rate = 2f;
        }
        else
        {
          duration *= 0.8f;
          follow.Sprite.Rate = 1f;
        }
        if (!follow.crashes)
        {
          if (index == 0)
            duration = 0.7f;
          if (index == 1)
            duration += 0.191f;
          if (index == 2)
            duration += 0.191f;
        }
        for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime / duration)
        {
          sin += Engine.DeltaTime * 10f;
          follow.Position = (curve.GetPoint(p) + Vector2.UnitY * (float) Math.Sin((double) sin) * 8f).Floor();
          player.Position = follow.Position + new Vector2(2f, 10f);
          switch (follow.Sprite.CurrentAnimationFrame)
          {
            case 1:
              Player player1 = player;
              player1.Position = player1.Position + new Vector2(1f, -1f);
              break;
            case 2:
              Player player2 = player;
              player2.Position = player2.Position + new Vector2(-1f, 0.0f);
              break;
            case 3:
              Player player3 = player;
              player3.Position = player3.Position + new Vector2(-1f, 1f);
              break;
            case 4:
              Player player4 = player;
              player4.Position = player4.Position + new Vector2(1f, 3f);
              break;
            case 5:
              Player player5 = player;
              player5.Position = player5.Position + new Vector2(2f, 5f);
              break;
          }
          yield return (object) null;
        }
        level.Shake();
        Input.Rumble(RumbleStrength.Medium, RumbleLength.Short);
        ++index;
        curve = new SimpleCurve();
      }
      follow.Sprite.Rate = 1f;
      Celeste.Freeze(0.05f);
      yield return (object) null;
      level.Shake();
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Long);
      level.Flash(Color.White);
      follow.emitParticles = false;
      follow.inCutscene = false;
    }
  }
}
