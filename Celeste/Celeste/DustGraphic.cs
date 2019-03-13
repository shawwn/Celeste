// Decompiled with JetBrains decompiler
// Type: Celeste.DustGraphic
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
  [Tracked(false)]
  public class DustGraphic : Component
  {
    public float Scale = 1f;
    private List<DustGraphic.Node> nodes = new List<DustGraphic.Node>();
    public List<DustGraphic.Node> LeftNodes = new List<DustGraphic.Node>();
    public List<DustGraphic.Node> RightNodes = new List<DustGraphic.Node>();
    public List<DustGraphic.Node> TopNodes = new List<DustGraphic.Node>();
    public List<DustGraphic.Node> BottomNodes = new List<DustGraphic.Node>();
    public int EyeFlip = 1;
    private bool leftEyeVisible = true;
    private bool rightEyeVisible = true;
    public Vector2 Position;
    private MTexture center;
    public Action OnEstablish;
    public Vector2 EyeTargetDirection;
    public Vector2 EyeDirection;
    private bool eyesExist;
    private int eyeTextureIndex;
    private MTexture eyeTexture;
    private Vector2 eyeLookRange;
    private bool eyesMoveByRotation;
    private bool autoControlEyes;
    private bool eyesFollowPlayer;
    private Coroutine blink;
    private DustGraphic.Eyeballs eyes;
    private float timer;
    private float offset;
    private bool ignoreSolids;
    private bool autoExpandDust;
    private float shakeTimer;
    private Vector2 shakeValue;
    private int randomSeed;

    public bool Estableshed { get; private set; }

    public Vector2 RenderPosition
    {
      get
      {
        return this.Entity.Position + this.Position + this.shakeValue;
      }
    }

    private bool InView
    {
      get
      {
        Camera camera = (this.Scene as Level).Camera;
        Vector2 position = this.Entity.Position;
        return (double) position.X + 16.0 >= (double) camera.Left && (double) position.Y + 16.0 >= (double) camera.Top && (double) position.X - 16.0 <= (double) camera.Right && (double) position.Y - 16.0 <= (double) camera.Bottom;
      }
    }

    public DustGraphic(bool ignoreSolids, bool autoControlEyes = false, bool autoExpandDust = false)
      : base(true, true)
    {
      this.ignoreSolids = ignoreSolids;
      this.autoControlEyes = autoControlEyes;
      this.autoExpandDust = autoExpandDust;
      this.center = Calc.Random.Choose<MTexture>(GFX.Game.GetAtlasSubtextures("danger/dustcreature/center"));
      this.offset = Calc.Random.NextFloat() * 4f;
      this.timer = Calc.Random.NextFloat();
      this.EyeTargetDirection = this.EyeDirection = Calc.AngleToVector(Calc.Random.NextFloat(6.283185f), 1f);
      this.eyeTextureIndex = Calc.Random.Next(128);
      this.eyesExist = true;
      if (autoControlEyes)
      {
        this.eyesExist = Calc.Random.Chance(0.5f);
        this.eyesFollowPlayer = Calc.Random.Chance(0.3f);
      }
      this.randomSeed = Calc.Random.Next();
    }

    public override void Added(Entity entity)
    {
      base.Added(entity);
      entity.Add((Component) new TransitionListener()
      {
        OnIn = (Action<float>) (f => this.AddDustNodesIfInCamera())
      });
      entity.Add((Component) new DustEdge(new Action(((Component) this).Render)));
    }

    public override void Update()
    {
      this.timer += Engine.DeltaTime * 0.6f;
      bool inView = this.InView;
      if ((double) this.shakeTimer > 0.0)
      {
        this.shakeTimer -= Engine.DeltaTime;
        if ((double) this.shakeTimer <= 0.0)
          this.shakeValue = Vector2.Zero;
        else if (this.Scene.OnInterval(0.05f))
          this.shakeValue = Calc.Random.ShakeVector();
      }
      if (this.eyesExist)
      {
        if (this.EyeDirection != this.EyeTargetDirection & inView)
        {
          if (!this.eyesMoveByRotation)
          {
            this.EyeDirection = Calc.Approach(this.EyeDirection, this.EyeTargetDirection, 12f * Engine.DeltaTime);
          }
          else
          {
            float val = this.EyeDirection.Angle();
            float target = this.EyeTargetDirection.Angle();
            float angleRadians = Calc.AngleApproach(val, target, 8f * Engine.DeltaTime);
            this.EyeDirection = (double) angleRadians != (double) target ? Calc.AngleToVector(angleRadians, 1f) : this.EyeTargetDirection;
          }
        }
        if (this.eyesFollowPlayer & inView)
        {
          Player entity = this.Entity.Scene.Tracker.GetEntity<Player>();
          if (entity != null)
          {
            Vector2 vector = (entity.Position - this.Entity.Position).SafeNormalize();
            this.EyeTargetDirection = !this.eyesMoveByRotation ? vector : Calc.AngleToVector(Calc.AngleApproach(this.eyeLookRange.Angle(), vector.Angle(), 0.7853982f), 1f);
          }
        }
        if (this.blink != null)
          this.blink.Update();
      }
      if (this.nodes.Count <= 0 && this.Entity.Scene != null && !this.Estableshed)
      {
        this.AddDustNodesIfInCamera();
      }
      else
      {
        foreach (DustGraphic.Node node in this.nodes)
          node.Rotation += Engine.DeltaTime * 0.5f;
      }
    }

    public void OnHitPlayer()
    {
      if (SaveData.Instance.Assists.Invincible)
        return;
      this.shakeTimer = 0.6f;
      if (!this.eyesExist)
        return;
      this.blink = (Coroutine) null;
      this.leftEyeVisible = true;
      this.rightEyeVisible = true;
      this.eyeTexture = GFX.Game["danger/dustcreature/deadEyes"];
    }

    public void AddDustNodesIfInCamera()
    {
      if (this.nodes.Count > 0 || !this.InView || DustEdges.DustGraphicEstabledCounter > 25 || this.Estableshed)
        return;
      Calc.PushRandom(this.randomSeed);
      int x = (int) this.Entity.X;
      int y = (int) this.Entity.Y;
      Vector2 vector2 = new Vector2(1f, 1f).SafeNormalize();
      this.AddNode(new Vector2(-vector2.X, -vector2.Y), this.ignoreSolids || !this.Entity.Scene.CollideCheck<Solid>(new Rectangle(x - 8, y - 8, 8, 8)));
      this.AddNode(new Vector2(vector2.X, -vector2.Y), this.ignoreSolids || !this.Entity.Scene.CollideCheck<Solid>(new Rectangle(x, y - 8, 8, 8)));
      this.AddNode(new Vector2(-vector2.X, vector2.Y), this.ignoreSolids || !this.Entity.Scene.CollideCheck<Solid>(new Rectangle(x - 8, y, 8, 8)));
      this.AddNode(new Vector2(vector2.X, vector2.Y), this.ignoreSolids || !this.Entity.Scene.CollideCheck<Solid>(new Rectangle(x, y, 8, 8)));
      if (this.nodes[0].Enabled || this.nodes[2].Enabled)
        --this.Position.X;
      if (this.nodes[1].Enabled || this.nodes[3].Enabled)
        ++this.Position.X;
      if (this.nodes[0].Enabled || this.nodes[1].Enabled)
        --this.Position.Y;
      if (this.nodes[2].Enabled || this.nodes[3].Enabled)
        ++this.Position.Y;
      int num = 0;
      foreach (DustGraphic.Node node in this.nodes)
      {
        if (node.Enabled)
          ++num;
      }
      this.eyesMoveByRotation = num < 4;
      if (this.autoControlEyes && this.eyesExist && this.eyesMoveByRotation)
      {
        this.eyeLookRange = Vector2.Zero;
        if (this.nodes[0].Enabled)
          this.eyeLookRange += new Vector2(-1f, -1f).SafeNormalize();
        if (this.nodes[1].Enabled)
          this.eyeLookRange += new Vector2(1f, -1f).SafeNormalize();
        if (this.nodes[2].Enabled)
          this.eyeLookRange += new Vector2(-1f, 1f).SafeNormalize();
        if (this.nodes[3].Enabled)
          this.eyeLookRange += new Vector2(1f, 1f).SafeNormalize();
        if (num > 0 && (double) this.eyeLookRange.Length() > 0.0)
        {
          this.eyeLookRange /= (float) num;
          this.eyeLookRange = this.eyeLookRange.SafeNormalize();
        }
        this.EyeTargetDirection = this.EyeDirection = this.eyeLookRange;
      }
      if (this.eyesExist)
      {
        this.blink = new Coroutine(this.BlinkRoutine(), true);
        List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures(DustStyles.Get(this.Scene).EyeTextures);
        this.eyeTexture = atlasSubtextures[this.eyeTextureIndex % atlasSubtextures.Count];
        this.Entity.Scene.Add((Entity) (this.eyes = new DustGraphic.Eyeballs(this)));
      }
      ++DustEdges.DustGraphicEstabledCounter;
      this.Estableshed = true;
      if (this.OnEstablish != null)
        this.OnEstablish();
      Calc.PopRandom();
    }

    private void AddNode(Vector2 angle, bool enabled)
    {
      Vector2 vector2 = new Vector2(1f, 1f);
      if (this.autoExpandDust)
      {
        int num1 = Math.Sign(angle.X);
        int num2 = Math.Sign(angle.Y);
        this.Entity.Collidable = false;
        if (this.Scene.CollideCheck<Solid>(new Rectangle((int) ((double) this.Entity.X - 4.0 + (double) (num1 * 16)), (int) ((double) this.Entity.Y - 4.0 + (double) (num2 * 4)), 8, 8)) || this.Scene.CollideCheck<DustStaticSpinner>(new Rectangle((int) ((double) this.Entity.X - 4.0 + (double) (num1 * 16)), (int) ((double) this.Entity.Y - 4.0 + (double) (num2 * 4)), 8, 8)))
          vector2.X = 5f;
        if (this.Scene.CollideCheck<Solid>(new Rectangle((int) ((double) this.Entity.X - 4.0 + (double) (num1 * 4)), (int) ((double) this.Entity.Y - 4.0 + (double) (num2 * 16)), 8, 8)) || this.Scene.CollideCheck<DustStaticSpinner>(new Rectangle((int) ((double) this.Entity.X - 4.0 + (double) (num1 * 4)), (int) ((double) this.Entity.Y - 4.0 + (double) (num2 * 16)), 8, 8)))
          vector2.Y = 5f;
        this.Entity.Collidable = true;
      }
      DustGraphic.Node node = new DustGraphic.Node();
      node.Base = Calc.Random.Choose<MTexture>(GFX.Game.GetAtlasSubtextures("danger/dustcreature/base"));
      node.Overlay = Calc.Random.Choose<MTexture>(GFX.Game.GetAtlasSubtextures("danger/dustcreature/overlay"));
      node.Rotation = Calc.Random.NextFloat(6.283185f);
      node.Angle = angle * vector2;
      node.Enabled = enabled;
      this.nodes.Add(node);
      if ((double) angle.X < 0.0)
        this.LeftNodes.Add(node);
      else
        this.RightNodes.Add(node);
      if ((double) angle.Y < 0.0)
        this.TopNodes.Add(node);
      else
        this.BottomNodes.Add(node);
    }

    private IEnumerator BlinkRoutine()
    {
      while (true)
      {
        yield return (object) (float) (2.0 + (double) Calc.Random.NextFloat(1.5f));
        this.leftEyeVisible = false;
        yield return (object) (float) (0.0199999995529652 + (double) Calc.Random.NextFloat(0.05f));
        this.rightEyeVisible = false;
        yield return (object) 0.25f;
        this.leftEyeVisible = this.rightEyeVisible = true;
      }
    }

    public override void Render()
    {
      if (!this.InView)
        return;
      Vector2 renderPosition = this.RenderPosition;
      foreach (DustGraphic.Node node in this.nodes)
      {
        if (node.Enabled)
        {
          node.Base.DrawCentered(renderPosition + node.Angle * this.Scale, Color.White, this.Scale, node.Rotation);
          node.Overlay.DrawCentered(renderPosition + node.Angle * this.Scale, Color.White, this.Scale, -node.Rotation);
        }
      }
      this.center.DrawCentered(renderPosition, Color.White, this.Scale, this.timer);
    }

    public class Node
    {
      public MTexture Base;
      public MTexture Overlay;
      public float Rotation;
      public Vector2 Angle;
      public bool Enabled;
    }

    private class Eyeballs : Entity
    {
      public DustGraphic Dust;
      public Color Color;

      public Eyeballs(DustGraphic dust)
      {
        this.Dust = dust;
        this.Depth = this.Dust.Entity.Depth - 1;
      }

      public override void Added(Scene scene)
      {
        base.Added(scene);
        this.Color = DustStyles.Get(scene).EyeColor;
      }

      public override void Update()
      {
        base.Update();
        if (this.Dust.Entity != null && this.Dust.Scene != null)
          return;
        this.RemoveSelf();
      }

      public override void Render()
      {
        if (!this.Dust.Visible || !this.Dust.Entity.Visible)
          return;
        Vector2 vector2 = new Vector2(-this.Dust.EyeDirection.Y, this.Dust.EyeDirection.X).SafeNormalize();
        if (this.Dust.leftEyeVisible)
          this.Dust.eyeTexture.DrawCentered(this.Dust.RenderPosition + (this.Dust.EyeDirection * 5f + vector2 * 3f) * this.Dust.Scale, this.Color, this.Dust.Scale);
        if (this.Dust.rightEyeVisible)
          this.Dust.eyeTexture.DrawCentered(this.Dust.RenderPosition + (this.Dust.EyeDirection * 5f - vector2 * 3f) * this.Dust.Scale, this.Color, this.Dust.Scale);
      }
    }
  }
}

