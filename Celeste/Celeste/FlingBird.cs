// Decompiled with JetBrains decompiler
// Type: Celeste.FlingBird
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Celeste
{
  public class FlingBird : Entity
  {
    public static ParticleType P_Feather;
    public const float SkipDist = 100f;
    public static readonly Vector2 FlingSpeed = new Vector2(380f, -100f);
    private Vector2 spriteOffset = new Vector2(0.0f, 8f);
    private Sprite sprite;
    private FlingBird.States state;
    private Vector2 flingSpeed;
    private Vector2 flingTargetSpeed;
    private float flingAccel;
    private Color trailColor = Calc.HexToColor("639bff");
    private EntityData entityData;
    private SoundSource moveSfx;
    private int segmentIndex;
    public List<Vector2[]> NodeSegments;
    public List<bool> SegmentsWaiting;
    public bool LightningRemoved;

    public FlingBird(Vector2[] nodes, bool skippable)
      : base(nodes[0])
    {
      this.Depth = -1;
      this.Add((Component) (this.sprite = GFX.SpriteBank.Create("bird")));
      this.sprite.Play("hover");
      this.sprite.Scale.X = -1f;
      this.sprite.Position = this.spriteOffset;
      this.sprite.OnFrameChange = (Action<string>) (spr => BirdNPC.FlapSfxCheck(this.sprite));
      this.Collider = (Collider) new Monocle.Circle(16f);
      this.Add((Component) new PlayerCollider(new Action<Player>(this.OnPlayer)));
      this.Add((Component) (this.moveSfx = new SoundSource()));
      this.NodeSegments = new List<Vector2[]>();
      this.NodeSegments.Add(nodes);
      this.SegmentsWaiting = new List<bool>();
      this.SegmentsWaiting.Add(skippable);
      this.Add((Component) new TransitionListener()
      {
        OnOut = (Action<float>) (t => this.sprite.Color = Color.White * (1f - Calc.Map(t, 0.0f, 0.4f)))
      });
    }

    public FlingBird(EntityData data, Vector2 levelOffset)
      : this(data.NodesWithPosition(levelOffset), data.Bool("waiting"))
    {
      this.entityData = data;
    }

    public override void Awake(Scene scene)
    {
      base.Awake(scene);
      List<FlingBird> all = this.Scene.Entities.FindAll<FlingBird>();
      for (int index = all.Count - 1; index >= 0; --index)
      {
        if (all[index].entityData.Level.Name != this.entityData.Level.Name)
          all.RemoveAt(index);
      }
      all.Sort((Comparison<FlingBird>) ((a, b) => Math.Sign(a.X - b.X)));
      if (all[0] == this)
      {
        for (int index = 1; index < all.Count; ++index)
        {
          this.NodeSegments.Add(all[index].NodeSegments[0]);
          this.SegmentsWaiting.Add(all[index].SegmentsWaiting[0]);
          all[index].RemoveSelf();
        }
      }
      if (this.SegmentsWaiting[0])
      {
        this.sprite.Play("hoverStressed");
        this.sprite.Scale.X = 1f;
      }
      Player entity = scene.Tracker.GetEntity<Player>();
      if (entity == null || (double) entity.X <= (double) this.X)
        return;
      this.RemoveSelf();
    }

    private void Skip()
    {
      this.state = FlingBird.States.Move;
      this.Add((Component) new Coroutine(this.MoveRoutine()));
    }

    private void OnPlayer(Player player)
    {
      if (this.state != FlingBird.States.Wait || !player.DoFlingBird(this))
        return;
      this.flingSpeed = player.Speed * 0.4f;
      this.flingSpeed.Y = 120f;
      this.flingTargetSpeed = Vector2.Zero;
      this.flingAccel = 1000f;
      player.Speed = Vector2.Zero;
      this.state = FlingBird.States.Fling;
      this.Add((Component) new Coroutine(this.DoFlingRoutine(player)));
      Audio.Play("event:/new_content/game/10_farewell/bird_throw", this.Center);
    }

    public override void Update()
    {
      base.Update();
      if (this.state != FlingBird.States.Wait)
        this.sprite.Position = Calc.Approach(this.sprite.Position, this.spriteOffset, 32f * Engine.DeltaTime);
      switch (this.state)
      {
        case FlingBird.States.Wait:
          Player entity = this.Scene.Tracker.GetEntity<Player>();
          if (entity != null && (double) entity.X - (double) this.X >= 100.0)
          {
            this.Skip();
            break;
          }
          if (this.SegmentsWaiting[this.segmentIndex] && this.LightningRemoved)
          {
            this.Skip();
            break;
          }
          if (entity == null)
            break;
          float num = Calc.ClampedMap((entity.Center - this.Position).Length(), 16f, 64f, 12f, 0.0f);
          this.sprite.Position = Calc.Approach(this.sprite.Position, this.spriteOffset + (entity.Center - this.Position).SafeNormalize() * num, 32f * Engine.DeltaTime);
          break;
        case FlingBird.States.Fling:
          if ((double) this.flingAccel > 0.0)
            this.flingSpeed = Calc.Approach(this.flingSpeed, this.flingTargetSpeed, this.flingAccel * Engine.DeltaTime);
          this.Position = this.Position + this.flingSpeed * Engine.DeltaTime;
          break;
        case FlingBird.States.WaitForLightningClear:
          if (this.Scene.Entities.FindFirst<Lightning>() != null && (double) this.X <= (double) (this.Scene as Level).Bounds.Right)
            break;
          this.sprite.Scale.X = 1f;
          this.state = FlingBird.States.Leaving;
          this.Add((Component) new Coroutine(this.LeaveRoutine()));
          break;
      }
    }

    private IEnumerator DoFlingRoutine(Player player)
    {
      FlingBird flingBird = this;
      Level level = flingBird.Scene as Level;
      Vector2 screenSpaceFocusPoint = player.Position - level.Camera.Position;
      screenSpaceFocusPoint.X = Calc.Clamp(screenSpaceFocusPoint.X, 145f, 215f);
      screenSpaceFocusPoint.Y = Calc.Clamp(screenSpaceFocusPoint.Y, 85f, 95f);
      flingBird.Add((Component) new Coroutine(level.ZoomTo(screenSpaceFocusPoint, 1.1f, 0.2f)));
      Engine.TimeRate = 0.8f;
      Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
      while (flingBird.flingSpeed != Vector2.Zero)
        yield return (object) null;
      flingBird.sprite.Play("throw");
      flingBird.sprite.Scale.X = 1f;
      flingBird.flingSpeed = new Vector2(-140f, 140f);
      flingBird.flingTargetSpeed = Vector2.Zero;
      flingBird.flingAccel = 1400f;
      yield return (object) 0.1f;
      Celeste.Freeze(0.05f);
      flingBird.flingTargetSpeed = FlingBird.FlingSpeed;
      flingBird.flingAccel = 6000f;
      yield return (object) 0.1f;
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
      Engine.TimeRate = 1f;
      level.Shake();
      flingBird.Add((Component) new Coroutine(level.ZoomBack(0.1f)));
      player.FinishFlingBird();
      flingBird.flingTargetSpeed = Vector2.Zero;
      flingBird.flingAccel = 4000f;
      yield return (object) 0.3f;
      flingBird.Add((Component) new Coroutine(flingBird.MoveRoutine()));
    }

    private IEnumerator MoveRoutine()
    {
      FlingBird flingBird = this;
      flingBird.state = FlingBird.States.Move;
      flingBird.sprite.Play("fly");
      flingBird.sprite.Scale.X = 1f;
      flingBird.moveSfx.Play("event:/new_content/game/10_farewell/bird_relocate");
      for (int nodeIndex = 1; nodeIndex < flingBird.NodeSegments[flingBird.segmentIndex].Length - 1; nodeIndex += 2)
      {
        Vector2 position = flingBird.Position;
        Vector2 anchor = flingBird.NodeSegments[flingBird.segmentIndex][nodeIndex];
        Vector2 to = flingBird.NodeSegments[flingBird.segmentIndex][nodeIndex + 1];
        yield return (object) flingBird.MoveOnCurve(position, anchor, to);
      }
      ++flingBird.segmentIndex;
      bool atEnding = flingBird.segmentIndex >= flingBird.NodeSegments.Count;
      if (!atEnding)
      {
        Vector2 position = flingBird.Position;
        Vector2 anchor = flingBird.NodeSegments[flingBird.segmentIndex - 1][flingBird.NodeSegments[flingBird.segmentIndex - 1].Length - 1];
        Vector2 to = flingBird.NodeSegments[flingBird.segmentIndex][0];
        yield return (object) flingBird.MoveOnCurve(position, anchor, to);
      }
      flingBird.sprite.Rotation = 0.0f;
      flingBird.sprite.Scale = Vector2.One;
      if (atEnding)
      {
        flingBird.sprite.Play("hoverStressed");
        flingBird.sprite.Scale.X = 1f;
        flingBird.state = FlingBird.States.WaitForLightningClear;
      }
      else
      {
        if (flingBird.SegmentsWaiting[flingBird.segmentIndex])
          flingBird.sprite.Play("hoverStressed");
        else
          flingBird.sprite.Play("hover");
        flingBird.sprite.Scale.X = -1f;
        flingBird.state = FlingBird.States.Wait;
      }
    }

    // private IEnumerator LeaveRoutine()
    // {
    //   // ISSUE: reference to a compiler-generated field
    //   int num = this.\u003C\u003E1__state;
    //   FlingBird flingBird = this;
    //   if (num != 0)
    //   {
    //     if (num != 1)
    //       return false;
    //     // ISSUE: reference to a compiler-generated field
    //     this.\u003C\u003E1__state = -1;
    //     flingBird.RemoveSelf();
    //     return false;
    //   }
    //   // ISSUE: reference to a compiler-generated field
    //   this.\u003C\u003E1__state = -1;
    //   flingBird.sprite.Scale.X = 1f;
    //   flingBird.sprite.Play("fly");
    //   Vector2 to = new Vector2((float) ((flingBird.Scene as Level).Bounds.Right + 32), flingBird.Y);
    //   // ISSUE: reference to a compiler-generated field
    //   this.\u003C\u003E2__current = (object) flingBird.MoveOnCurve(flingBird.Position, (flingBird.Position + to) * 0.5f - Vector2.UnitY * 12f, to);
    //   // ISSUE: reference to a compiler-generated field
    //   this.\u003C\u003E1__state = 1;
    //   return true;
    // }
    
    private IEnumerator LeaveRoutine()
    {
      FlingBird flingBird = this;
      flingBird.sprite.Scale.X = 1f;
      flingBird.sprite.Play("fly");
      Vector2 to = new Vector2((float) ((flingBird.Scene as Level).Bounds.Right + 32), flingBird.Y);
      yield return flingBird.MoveOnCurve(flingBird.Position, (flingBird.Position + to) * 0.5f - Vector2.UnitY * 12f, to);
    }

    private IEnumerator MoveOnCurve(Vector2 from, Vector2 anchor, Vector2 to)
    {
      FlingBird flingBird = this;
      SimpleCurve curve = new SimpleCurve(from, to, anchor);
      float duration = curve.GetLengthParametric(32) / 500f;
      Vector2 was = from;
      for (float t = 0.016f; (double) t <= 1.0; t += Engine.DeltaTime / duration)
      {
        flingBird.Position = curve.GetPoint(t).Floor();
        flingBird.sprite.Rotation = Calc.Angle(curve.GetPoint(Math.Max(0.0f, t - 0.05f)), curve.GetPoint(Math.Min(1f, t + 0.05f)));
        flingBird.sprite.Scale.X = 1.25f;
        flingBird.sprite.Scale.Y = 0.7f;
        if ((double) (was - flingBird.Position).Length() > 32.0)
        {
          TrailManager.Add((Entity) flingBird, flingBird.trailColor);
          was = flingBird.Position;
        }
        yield return (object) null;
      }
      flingBird.Position = to;
    }

    public override void Render() => base.Render();

    private void DrawLine(Vector2 a, Vector2 anchor, Vector2 b) => new SimpleCurve(a, b, anchor).Render(Color.Red, 32);

    private enum States
    {
      Wait,
      Fling,
      Move,
      WaitForLightningClear,
      Leaving,
    }
  }
}
