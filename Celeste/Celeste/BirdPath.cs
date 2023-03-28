// Decompiled with JetBrains decompiler
// Type: Celeste.BirdPath
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class BirdPath : Entity
  {
    private Vector2 start;
    private Sprite sprite;
    private Vector2[] nodes;
    private Color trailColor = Calc.HexToColor("639bff");
    private Vector2 target;
    private Vector2 speed;
    private float maxspeed;
    private Vector2 lastTrail;
    private float speedMult;
    private EntityID ID;
    private bool onlyOnce;
    private bool onlyIfLeft;

    public BirdPath(EntityID id, EntityData data, Vector2 offset)
      : this(id, data.Position + offset, data.NodesOffset(offset), data.Bool("only_once"), data.Bool(nameof (onlyIfLeft)), data.Float(nameof (speedMult), 1f))
    {
    }

    public BirdPath(
      EntityID id,
      Vector2 position,
      Vector2[] nodes,
      bool onlyOnce,
      bool onlyIfLeft,
      float speedMult)
    {
      this.Tag = (int) Tags.TransitionUpdate;
      this.ID = id;
      this.Position = position;
      this.start = position;
      this.nodes = nodes;
      this.onlyOnce = onlyOnce;
      this.onlyIfLeft = onlyIfLeft;
      this.speedMult = speedMult;
      this.maxspeed = 150f * speedMult;
      this.Add((Component) (this.sprite = GFX.SpriteBank.Create("bird")));
      this.sprite.Play("flyupRoll");
      this.sprite.JustifyOrigin(0.5f, 0.75f);
      this.Add((Component) new SoundSource("event:/new_content/game/10_farewell/bird_flyuproll")
      {
        RemoveOnOneshotEnd = true
      });
      this.Add((Component) new Coroutine(this.Routine()));
    }

    public void WaitForTrigger() => this.Visible = this.Active = false;

    public void Trigger() => this.Visible = this.Active = true;

    public override void Added(Scene scene)
    {
      base.Added(scene);
      if (!this.onlyOnce)
        return;
      (this.Scene as Level).Session.DoNotLoad.Add(this.ID);
    }

    public override void Awake(Scene scene)
    {
      base.Awake(scene);
      if (!this.onlyIfLeft)
        return;
      Player entity = this.Scene.Tracker.GetEntity<Player>();
      if (entity == null || (double) entity.X <= (double) this.X)
        return;
      this.RemoveSelf();
    }

    private IEnumerator Routine()
    {
      BirdPath birdPath = this;
      Vector2 begin = birdPath.start;
      for (int i = 0; i <= birdPath.nodes.Length - 1; i += 2)
      {
        Vector2 node = birdPath.nodes[i];
        Vector2 next = birdPath.nodes[i + 1];
        SimpleCurve curve = new SimpleCurve(begin, next, node);
        float duration = curve.GetLengthParametric(32) / birdPath.maxspeed;
        bool playedSfx = false;
        Vector2 position = birdPath.Position;
        for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime * birdPath.speedMult / duration)
        {
          birdPath.target = curve.GetPoint(p);
          if ((double) p > 0.8999999761581421)
          {
            if (!playedSfx && birdPath.sprite.CurrentAnimationID != "flyupRoll")
            {
              birdPath.Add((Component) new SoundSource("event:/new_content/game/10_farewell/bird_flyuproll")
              {
                RemoveOnOneshotEnd = true
              });
              playedSfx = true;
            }
            birdPath.sprite.Play("flyupRoll");
          }
          yield return (object) null;
        }
        begin = next;
        next = new Vector2();
        curve = new SimpleCurve();
      }
      birdPath.RemoveSelf();
    }

    public override void Update()
    {
      if ((this.Scene as Level).Transitioning)
      {
        foreach (Component component in (Entity) this)
        {
          if (component is SoundSource soundSource)
            soundSource.UpdateSfxPosition();
        }
      }
      else
      {
        base.Update();
        int num1 = Math.Sign(this.X - this.target.X);
        this.speed += (this.target - this.Position).SafeNormalize() * 800f * Engine.DeltaTime;
        if ((double) this.speed.Length() > (double) this.maxspeed)
          this.speed = this.speed.SafeNormalize(this.maxspeed);
        this.Position = this.Position + this.speed * Engine.DeltaTime;
        int num2 = Math.Sign(this.X - this.target.X);
        if (num1 != num2)
          this.speed.X *= 0.75f;
        this.sprite.Rotation = 1.5707964f + Calc.AngleLerp(this.speed.Angle(), Calc.Angle(this.Position, this.target), 0.5f);
        if ((double) (this.lastTrail - this.Position).Length() <= 32.0)
          return;
        TrailManager.Add((Entity) this, this.trailColor);
        this.lastTrail = this.Position;
      }
    }

    public override void Render() => base.Render();

    public override void DebugRender(Camera camera)
    {
      Vector2 begin = this.start;
      for (int index = 0; index < this.nodes.Length - 1; index += 2)
      {
        Vector2 node = this.nodes[index + 1];
        new SimpleCurve(begin, node, this.nodes[index]).Render(Color.Red * 0.25f, 32);
        begin = node;
      }
      Draw.Line(this.Position, this.Position + (this.target - this.Position).SafeNormalize() * ((this.target - this.Position).Length() - 3f), Color.Yellow);
      Draw.Circle(this.target, 3f, Color.Yellow, 16);
    }
  }
}
