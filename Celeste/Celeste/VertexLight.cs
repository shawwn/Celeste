// Decompiled with JetBrains decompiler
// Type: Celeste.VertexLight
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  [Tracked(false)]
  public class VertexLight : Component
  {
    public int Index = -1;
    public bool Dirty = true;
    public float InSolidAlphaMultiplier = 1f;
    public Color Color = Color.White;
    public float Alpha = 1f;
    private float startRadius = 16f;
    private float endRadius = 32f;
    public bool InSolid;
    public Vector2 LastNonSolidPosition;
    public Vector2 LastEntityPosition;
    public Vector2 LastPosition;
    public bool Started;
    public bool Spotlight;
    public float SpotlightDirection;
    public float SpotlightPush;
    private Vector2 position;

    public Vector2 Center
    {
      get
      {
        return this.Entity.Position + this.position;
      }
    }

    public float X
    {
      get
      {
        return this.position.X;
      }
      set
      {
        this.Position = new Vector2(value, this.position.Y);
      }
    }

    public float Y
    {
      get
      {
        return this.position.Y;
      }
      set
      {
        this.Position = new Vector2(this.position.X, value);
      }
    }

    public Vector2 Position
    {
      get
      {
        return this.position;
      }
      set
      {
        if (!(this.position != value))
          return;
        this.Dirty = true;
        this.position = value;
      }
    }

    public float StartRadius
    {
      get
      {
        return this.startRadius;
      }
      set
      {
        if ((double) this.startRadius == (double) value)
          return;
        this.Dirty = true;
        this.startRadius = value;
      }
    }

    public float EndRadius
    {
      get
      {
        return this.endRadius;
      }
      set
      {
        if ((double) this.endRadius == (double) value)
          return;
        this.Dirty = true;
        this.endRadius = value;
      }
    }

    public VertexLight()
      : base(true, true)
    {
    }

    public VertexLight(Color color, float alpha, int startFade, int endFade)
      : this(Vector2.Zero, color, alpha, startFade, endFade)
    {
    }

    public VertexLight(Vector2 position, Color color, float alpha, int startFade, int endFade)
      : base(true, true)
    {
      this.Position = position;
      this.Color = color;
      this.Alpha = alpha;
      this.StartRadius = (float) startFade;
      this.EndRadius = (float) endFade;
    }

    public override void Added(Entity entity)
    {
      base.Added(entity);
      this.LastNonSolidPosition = this.Center;
      this.LastEntityPosition = this.Entity.Position;
      this.LastPosition = this.Position;
    }

    public override void Update()
    {
      this.InSolidAlphaMultiplier = Calc.Approach(this.InSolidAlphaMultiplier, this.InSolid ? 0.0f : 1f, Engine.DeltaTime * 4f);
      base.Update();
    }

    public override void HandleGraphicsReset()
    {
      this.Dirty = true;
      base.HandleGraphicsReset();
    }

    public Tween CreatePulseTween()
    {
      float startA = this.StartRadius;
      float startB = startA + 6f;
      float endA = this.EndRadius;
      float endB = endA + 12f;
      Tween tween = Tween.Create(Tween.TweenMode.Persist, (Ease.Easer) null, 0.5f, false);
      tween.OnUpdate = (Action<Tween>) (t =>
      {
        this.StartRadius = (float) (int) MathHelper.Lerp(startB, startA, t.Eased);
        this.EndRadius = (float) (int) MathHelper.Lerp(endB, endA, t.Eased);
      });
      return tween;
    }

    public Tween CreateFadeInTween(float time)
    {
      float from = 0.0f;
      float to = this.Alpha;
      this.Alpha = 0.0f;
      Tween tween = Tween.Create(Tween.TweenMode.Persist, Ease.CubeOut, time, false);
      tween.OnUpdate = (Action<Tween>) (t => this.Alpha = MathHelper.Lerp(from, to, t.Eased));
      return tween;
    }

    public Tween CreateBurstTween(float time)
    {
      time += 0.8f;
      float delay = (time - 0.8f) / time;
      float startA = this.StartRadius;
      float startB = startA + 6f;
      float endA = this.EndRadius;
      float endB = endA + 12f;
      Tween tween = Tween.Create(Tween.TweenMode.Persist, (Ease.Easer) null, time, false);
      tween.OnUpdate = (Action<Tween>) (t =>
      {
        float amount;
        if ((double) t.Percent >= (double) delay)
        {
          float t1 = MathHelper.Clamp((float) (((double) t.Percent - (double) delay) / (1.0 - (double) delay)), 0.0f, 1f);
          amount = Ease.CubeIn(t1);
        }
        else
          amount = 0.0f;
        this.StartRadius = (float) (int) MathHelper.Lerp(startB, startA, amount);
        this.EndRadius = (float) (int) MathHelper.Lerp(endB, endA, amount);
      });
      return tween;
    }
  }
}

