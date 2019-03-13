// Decompiled with JetBrains decompiler
// Type: Celeste.RidgeGate
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class RidgeGate : Solid
  {
    private Vector2? node;

    public RidgeGate(EntityData data, Vector2 offset)
      : this(data.Position + offset, (float) data.Width, (float) data.Height, data.FirstNodeNullable(new Vector2?(offset)))
    {
    }

    public RidgeGate(Vector2 position, float width, float height, Vector2? node)
      : base(position, width, height, true)
    {
      this.node = node;
      this.Add((Component) new Monocle.Image(GFX.Game["objects/ridgeGate"]));
    }

    public override void Awake(Scene scene)
    {
      base.Awake(scene);
      if (!this.node.HasValue || !this.CollideCheck<Player>())
        return;
      this.Visible = this.Collidable = false;
      Vector2 position = this.Position;
      this.Position = this.node.Value;
      this.Add((Component) new Coroutine(this.EnterSequence(position), true));
    }

    private IEnumerator EnterSequence(Vector2 moveTo)
    {
      this.Visible = this.Collidable = true;
      yield return (object) 0.25f;
      Audio.Play("event:/game/04_cliffside/stone_blockade", this.Position);
      yield return (object) 0.25f;
      Vector2 start = this.Position;
      Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeOut, 1f, true);
      tween.OnUpdate = (Action<Tween>) (t => this.MoveTo(Vector2.Lerp(start, moveTo, t.Eased)));
      this.Add((Component) tween);
    }
  }
}

