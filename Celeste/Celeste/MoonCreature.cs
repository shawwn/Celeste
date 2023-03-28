// Decompiled with JetBrains decompiler
// Type: Celeste.MoonCreature
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  public class MoonCreature : Entity
  {
    private MoonCreature.TrailNode[] trail;
    private Vector2 start;
    private Vector2 target;
    private float targetTimer;
    private Vector2 speed;
    private Vector2 bump;
    private Player following;
    private Vector2 followingOffset;
    private float followingTime;
    private Color OrbColor;
    private Color CenterColor;
    private Sprite Sprite;
    private const float Acceleration = 90f;
    private const float FollowAcceleration = 120f;
    private const float MaxSpeed = 40f;
    private const float MaxFollowSpeed = 70f;
    private const float MaxFollowDistance = 200f;
    private readonly int spawn;
    private Rectangle originLevelBounds;

    public MoonCreature(Vector2 position)
    {
      this.Tag = (int) Tags.TransitionUpdate;
      this.Depth = -13010;
      this.Collider = (Collider) new Hitbox(20f, 20f, -10f, -10f);
      this.start = position;
      this.targetTimer = 0.0f;
      this.GetRandomTarget();
      this.Position = this.target;
      this.Add((Component) new PlayerCollider(new Action<Player>(this.OnPlayer)));
      this.OrbColor = Calc.HexToColor("b0e6ff");
      this.CenterColor = Calc.Random.Choose<Color>(Calc.HexToColor("c34fc7"), Calc.HexToColor("4f95c7"), Calc.HexToColor("53c74f"));
      Color color1 = Color.Lerp(this.CenterColor, Calc.HexToColor("bde4ee"), 0.5f);
      Color color2 = Color.Lerp(this.CenterColor, Calc.HexToColor("2f2941"), 0.5f);
      this.trail = new MoonCreature.TrailNode[10];
      for (int index = 0; index < 10; ++index)
        this.trail[index] = new MoonCreature.TrailNode()
        {
          Position = this.Position,
          Color = Color.Lerp(color1, color2, (float) index / 9f)
        };
      this.Add((Component) (this.Sprite = GFX.SpriteBank.Create("moonCreatureTiny")));
    }

    public MoonCreature(EntityData data, Vector2 offset)
      : this(data.Position + offset)
    {
      this.spawn = data.Int("number", 1) - 1;
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      for (int index = 0; index < this.spawn; ++index)
        scene.Add((Entity) new MoonCreature(this.Position + new Vector2((float) Calc.Random.Range(-4, 4), (float) Calc.Random.Range(-4, 4))));
      this.originLevelBounds = (scene as Level).Bounds;
    }

    private void OnPlayer(Player player)
    {
      Vector2 vector2 = (this.Position - player.Center).SafeNormalize(player.Speed.Length() * 0.3f);
      if ((double) vector2.LengthSquared() <= (double) this.bump.LengthSquared())
        return;
      this.bump = vector2;
      if ((double) (player.Center - this.start).Length() >= 200.0)
        return;
      this.following = player;
      this.followingTime = Calc.Random.Range(6f, 12f);
      this.GetFollowOffset();
    }

    private void GetFollowOffset() => this.followingOffset = new Vector2((float) (Calc.Random.Choose<int>(-1, 1) * Calc.Random.Range(8, 16)), Calc.Random.Range(-20f, 0.0f));

    private void GetRandomTarget()
    {
      Vector2 target = this.target;
      do
      {
        float length = Calc.Random.NextFloat(32f);
        this.target = this.start + Calc.AngleToVector(Calc.Random.NextFloat(6.2831855f), length);
      }
      while ((double) (target - this.target).Length() < 8.0);
    }

    public override void Update()
    {
      base.Update();
      if (this.following == null)
      {
        this.targetTimer -= Engine.DeltaTime;
        if ((double) this.targetTimer <= 0.0)
        {
          this.targetTimer = Calc.Random.Range(0.8f, 4f);
          this.GetRandomTarget();
        }
      }
      else
      {
        this.followingTime -= Engine.DeltaTime;
        this.targetTimer -= Engine.DeltaTime;
        if ((double) this.targetTimer <= 0.0)
        {
          this.targetTimer = Calc.Random.Range(0.8f, 2f);
          this.GetFollowOffset();
        }
        this.target = this.following.Center + this.followingOffset;
        if ((double) (this.Position - this.start).Length() > 200.0 || (double) this.followingTime <= 0.0)
        {
          this.following = (Player) null;
          this.targetTimer = 0.0f;
        }
      }
      this.speed += (this.target - this.Position).SafeNormalize() * (this.following == null ? 90f : 120f) * Engine.DeltaTime;
      this.speed = this.speed.SafeNormalize() * Math.Min(this.speed.Length(), this.following == null ? 40f : 70f);
      this.bump = this.bump.SafeNormalize() * Calc.Approach(this.bump.Length(), 0.0f, Engine.DeltaTime * 80f);
      this.Position = this.Position + (this.speed + this.bump) * Engine.DeltaTime;
      Vector2 position = this.Position;
      for (int index = 0; index < this.trail.Length; ++index)
      {
        Vector2 vector2 = (this.trail[index].Position - position).SafeNormalize();
        if (vector2 == Vector2.Zero)
          vector2 = new Vector2(0.0f, 1f);
        vector2.Y += 0.05f;
        Vector2 target = position + vector2 * 2f;
        this.trail[index].Position = Calc.Approach(this.trail[index].Position, target, 128f * Engine.DeltaTime);
        position = this.trail[index].Position;
      }
      this.X = Calc.Clamp(this.X, (float) (this.originLevelBounds.Left + 4), (float) (this.originLevelBounds.Right - 4));
      this.Y = Calc.Clamp(this.Y, (float) (this.originLevelBounds.Top + 4), (float) (this.originLevelBounds.Bottom - 4));
    }

    public override void Render()
    {
      Vector2 position1 = this.Position;
      this.Position = this.Position.Floor();
      for (int val = this.trail.Length - 1; val >= 0; --val)
      {
        Vector2 position2 = this.trail[val].Position;
        float num = Calc.ClampedMap((float) val, 0.0f, (float) (this.trail.Length - 1), 3f);
        Draw.Rect(position2.X - num / 2f, position2.Y - num / 2f, num, num, this.trail[val].Color);
      }
      base.Render();
      this.Position = position1;
    }

    private struct TrailNode
    {
      public Vector2 Position;
      public Color Color;
    }
  }
}
