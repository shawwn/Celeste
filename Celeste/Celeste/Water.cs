// Decompiled with JetBrains decompiler
// Type: Celeste.Water
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste
{
  [Tracked(false)]
  public class Water : Entity
  {
    public static readonly Color FillColor = Color.LightSkyBlue * 0.3f;
    public static readonly Color SurfaceColor = Color.LightSkyBlue * 0.8f;
    public static readonly Color RayTopColor = Color.LightSkyBlue * 0.6f;
    public static readonly Vector2 RayAngle = new Vector2(-4f, 8f).SafeNormalize();
    public List<Water.Surface> Surfaces = new List<Water.Surface>();
    private HashSet<WaterInteraction> contains = new HashSet<WaterInteraction>();
    public static ParticleType P_Splash;
    public Water.Surface TopSurface;
    public Water.Surface BottomSurface;
    private Rectangle fill;
    private bool[,] grid;
    private Water.Tension playerBottomTension;

    public Water(EntityData data, Vector2 offset)
      : this(data.Position + offset, true, data.Bool("hasBottom", false), (float) data.Width, (float) data.Height)
    {
    }

    public Water(
      Vector2 position,
      bool topSurface,
      bool bottomSurface,
      float width,
      float height)
    {
      this.Position = position;
      this.Tag = (int) Tags.TransitionUpdate;
      this.Depth = -9999;
      this.Collider = (Collider) new Hitbox(width, height, 0.0f, 0.0f);
      this.grid = new bool[(int) ((double) width / 8.0), (int) ((double) height / 8.0)];
      this.fill = new Rectangle(0, 0, (int) width, (int) height);
      int num = 8;
      if (topSurface)
      {
        this.TopSurface = new Water.Surface(this.Position + new Vector2(width / 2f, (float) num), new Vector2(0.0f, -1f), width, height);
        this.Surfaces.Add(this.TopSurface);
        this.fill.Y += num;
        this.fill.Height -= num;
      }
      if (bottomSurface)
      {
        this.BottomSurface = new Water.Surface(this.Position + new Vector2(width / 2f, height - (float) num), new Vector2(0.0f, 1f), width, height);
        this.Surfaces.Add(this.BottomSurface);
        this.fill.Height -= num;
      }
      this.Add((Component) new DisplacementRenderHook(new Action(this.RenderDisplacement)));
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      int index1 = 0;
      for (int length1 = this.grid.GetLength(0); index1 < length1; ++index1)
      {
        int index2 = 0;
        for (int length2 = this.grid.GetLength(1); index2 < length2; ++index2)
          this.grid[index1, index2] = !this.Scene.CollideCheck<Solid>(new Rectangle((int) this.X + index1 * 8, (int) this.Y + index2 * 8, 8, 8));
      }
    }

    public override void Update()
    {
      base.Update();
      foreach (Water.Surface surface in this.Surfaces)
        surface.Update();
      foreach (WaterInteraction component in this.Scene.Tracker.GetComponents<WaterInteraction>())
      {
        Entity entity = component.Entity;
        bool flag1 = this.contains.Contains(component);
        bool flag2 = this.CollideCheck(entity);
        if (flag1 != flag2)
        {
          if ((double) entity.Center.Y <= (double) this.Center.Y && this.TopSurface != null)
            this.TopSurface.DoRipple(entity.Center, 1f);
          else if ((double) entity.Center.Y > (double) this.Center.Y && this.BottomSurface != null)
            this.BottomSurface.DoRipple(entity.Center, 1f);
          bool flag3 = component.IsDashing();
          int num = (double) entity.Center.Y >= (double) this.Center.Y || this.Scene.CollideCheck<Solid>(new Rectangle((int) entity.Center.X - 4, (int) entity.Center.Y, 8, 16)) ? 0 : 1;
          if (flag1)
          {
            if (flag3)
              Audio.Play("event:/char/madeline/water_dash_out", entity.Center, "deep", (float) num);
            else
              Audio.Play("event:/char/madeline/water_out", entity.Center, "deep", (float) num);
            component.DrippingTimer = 2f;
          }
          else
          {
            if (flag3 && num == 1)
              Audio.Play("event:/char/madeline/water_dash_in", entity.Center, "deep", (float) num);
            else
              Audio.Play("event:/char/madeline/water_in", entity.Center, "deep", (float) num);
            component.DrippingTimer = 0.0f;
          }
          if (flag1)
            this.contains.Remove(component);
          else
            this.contains.Add(component);
        }
        if (this.BottomSurface != null && entity is Player)
        {
          if (flag2 && (double) entity.Y > (double) this.Bottom - 8.0)
          {
            if (this.playerBottomTension == null)
              this.playerBottomTension = this.BottomSurface.SetTension(entity.Position, 0.0f);
            this.playerBottomTension.Position = this.BottomSurface.GetPointAlong(entity.Position);
            this.playerBottomTension.Strength = Calc.ClampedMap(entity.Y, this.Bottom - 8f, this.Bottom + 4f, 0.0f, 1f);
          }
          else if (this.playerBottomTension != null)
          {
            this.BottomSurface.RemoveTension(this.playerBottomTension);
            this.playerBottomTension = (Water.Tension) null;
          }
        }
      }
    }

    public void RenderDisplacement()
    {
      Color color = new Color(0.5f, 0.5f, 0.25f, 1f);
      int index1 = 0;
      int length1 = this.grid.GetLength(0);
      int length2 = this.grid.GetLength(1);
      for (; index1 < length1; ++index1)
      {
        if (length2 > 0 && this.grid[index1, 0])
          Draw.Rect(this.X + (float) (index1 * 8), this.Y + 3f, 8f, 5f, color);
        for (int index2 = 1; index2 < length2; ++index2)
        {
          if (this.grid[index1, index2])
          {
            int num = 1;
            while (index2 + num < length2 && this.grid[index1, index2 + num])
              ++num;
            Draw.Rect(this.X + (float) (index1 * 8), this.Y + (float) (index2 * 8), 8f, (float) (num * 8), color);
            index2 += num - 1;
          }
        }
      }
    }

    public override void Render()
    {
      Draw.Rect(this.X + (float) this.fill.X, this.Y + (float) this.fill.Y, (float) this.fill.Width, (float) this.fill.Height, Water.FillColor);
      GameplayRenderer.End();
      foreach (Water.Surface surface in this.Surfaces)
        surface.Render((this.Scene as Level).Camera);
      GameplayRenderer.Begin();
    }

    public class Ripple
    {
      public float Position;
      public float Speed;
      public float Height;
      public float Percent;
      public float Duration;
    }

    public class Tension
    {
      public float Position;
      public float Strength;
    }

    public class Ray
    {
      public float Position;
      public float Percent;
      public float Duration;
      public float Width;
      public float Length;
      private float MaxWidth;

      public Ray(float maxWidth)
      {
        this.MaxWidth = maxWidth;
        this.Reset(Calc.Random.NextFloat());
      }

      public void Reset(float percent)
      {
        this.Position = Calc.Random.NextFloat() * this.MaxWidth;
        this.Percent = percent;
        this.Duration = Calc.Random.Range(2f, 8f);
        this.Width = (float) Calc.Random.Range(2, 16);
        this.Length = Calc.Random.Range(8f, 128f);
      }
    }

    public class Surface
    {
      public List<Water.Ripple> Ripples = new List<Water.Ripple>();
      public List<Water.Ray> Rays = new List<Water.Ray>();
      public List<Water.Tension> Tensions = new List<Water.Tension>();
      public const int Resolution = 4;
      public const float RaysPerPixel = 0.2f;
      public const float BaseHeight = 6f;
      public readonly Vector2 Outwards;
      public readonly int Width;
      public readonly int BodyHeight;
      public Vector2 Position;
      private float timer;
      private VertexPositionColor[] mesh;
      private int fillStartIndex;
      private int rayStartIndex;
      private int surfaceStartIndex;

      public Surface(Vector2 position, Vector2 outwards, float width, float bodyHeight)
      {
        this.Position = position;
        this.Outwards = outwards;
        this.Width = (int) width;
        this.BodyHeight = (int) bodyHeight;
        int num1 = (int) ((double) width / 4.0);
        int num2 = (int) ((double) width * 0.200000002980232);
        this.Rays = new List<Water.Ray>();
        for (int index = 0; index < num2; ++index)
          this.Rays.Add(new Water.Ray(width));
        this.fillStartIndex = 0;
        this.rayStartIndex = num1 * 6;
        this.surfaceStartIndex = (num1 + num2) * 6;
        this.mesh = new VertexPositionColor[(num1 * 2 + num2) * 6];
        for (int fillStartIndex = this.fillStartIndex; fillStartIndex < this.fillStartIndex + num1 * 6; ++fillStartIndex)
          this.mesh[fillStartIndex].Color = Water.FillColor;
        for (int rayStartIndex = this.rayStartIndex; rayStartIndex < this.rayStartIndex + num2 * 6; ++rayStartIndex)
          this.mesh[rayStartIndex].Color = Color.Transparent;
        for (int surfaceStartIndex = this.surfaceStartIndex; surfaceStartIndex < this.surfaceStartIndex + num1 * 6; ++surfaceStartIndex)
          this.mesh[surfaceStartIndex].Color = Water.SurfaceColor;
      }

      public float GetPointAlong(Vector2 position)
      {
        Vector2 vector2_1 = this.Outwards.Perpendicular();
        Vector2 lineA = this.Position + vector2_1 * (float) (-this.Width / 2);
        Vector2 lineB = this.Position + vector2_1 * (float) (this.Width / 2);
        Vector2 vector2_2 = Calc.ClosestPointOnLine(lineA, lineB, position);
        return (lineA - vector2_2).Length();
      }

      public Water.Tension SetTension(Vector2 position, float strength)
      {
        Water.Tension tension = new Water.Tension()
        {
          Position = this.GetPointAlong(position),
          Strength = strength
        };
        this.Tensions.Add(tension);
        return tension;
      }

      public void RemoveTension(Water.Tension tension)
      {
        this.Tensions.Remove(tension);
      }

      public void DoRipple(Vector2 position, float multiplier)
      {
        float num1 = 80f;
        float num2 = 3f;
        float pointAlong = this.GetPointAlong(position);
        int num3 = 2;
        if (this.Width < 200)
        {
          num2 *= Calc.ClampedMap((float) this.Width, 0.0f, 200f, 0.25f, 1f);
          multiplier *= Calc.ClampedMap((float) this.Width, 0.0f, 200f, 0.5f, 1f);
        }
        this.Ripples.Add(new Water.Ripple()
        {
          Position = pointAlong,
          Speed = -num1,
          Height = (float) num3 * multiplier,
          Percent = 0.0f,
          Duration = num2
        });
        this.Ripples.Add(new Water.Ripple()
        {
          Position = pointAlong,
          Speed = num1,
          Height = (float) num3 * multiplier,
          Percent = 0.0f,
          Duration = num2
        });
      }

      public void Update()
      {
        this.timer += Engine.DeltaTime;
        Vector2 vector2_1 = this.Outwards.Perpendicular();
        for (int index = this.Ripples.Count - 1; index >= 0; --index)
        {
          Water.Ripple ripple = this.Ripples[index];
          if ((double) ripple.Percent > 1.0)
          {
            this.Ripples.RemoveAt(index);
          }
          else
          {
            ripple.Position += ripple.Speed * Engine.DeltaTime;
            if ((double) ripple.Position < 0.0 || (double) ripple.Position > (double) this.Width)
            {
              ripple.Speed = -ripple.Speed;
              ripple.Position = Calc.Clamp(ripple.Position, 0.0f, (float) this.Width);
            }
            ripple.Percent += Engine.DeltaTime / ripple.Duration;
          }
        }
        int num1 = 0;
        int fillStartIndex = this.fillStartIndex;
        int surfaceStartIndex = this.surfaceStartIndex;
        while (num1 < this.Width)
        {
          int num2 = num1;
          float surfaceHeight1 = this.GetSurfaceHeight((float) num2);
          int num3 = Math.Min(num1 + 4, this.Width);
          float surfaceHeight2 = this.GetSurfaceHeight((float) num3);
          this.mesh[fillStartIndex].Position = new Vector3(this.Position + vector2_1 * (float) (-this.Width / 2 + num2) + this.Outwards * surfaceHeight1, 0.0f);
          this.mesh[fillStartIndex + 1].Position = new Vector3(this.Position + vector2_1 * (float) (-this.Width / 2 + num3) + this.Outwards * surfaceHeight2, 0.0f);
          this.mesh[fillStartIndex + 2].Position = new Vector3(this.Position + vector2_1 * (float) (-this.Width / 2 + num2), 0.0f);
          this.mesh[fillStartIndex + 3].Position = new Vector3(this.Position + vector2_1 * (float) (-this.Width / 2 + num3) + this.Outwards * surfaceHeight2, 0.0f);
          this.mesh[fillStartIndex + 4].Position = new Vector3(this.Position + vector2_1 * (float) (-this.Width / 2 + num3), 0.0f);
          this.mesh[fillStartIndex + 5].Position = new Vector3(this.Position + vector2_1 * (float) (-this.Width / 2 + num2), 0.0f);
          this.mesh[surfaceStartIndex].Position = new Vector3(this.Position + vector2_1 * (float) (-this.Width / 2 + num2) + this.Outwards * (surfaceHeight1 + 1f), 0.0f);
          this.mesh[surfaceStartIndex + 1].Position = new Vector3(this.Position + vector2_1 * (float) (-this.Width / 2 + num3) + this.Outwards * (surfaceHeight2 + 1f), 0.0f);
          this.mesh[surfaceStartIndex + 2].Position = new Vector3(this.Position + vector2_1 * (float) (-this.Width / 2 + num2) + this.Outwards * surfaceHeight1, 0.0f);
          this.mesh[surfaceStartIndex + 3].Position = new Vector3(this.Position + vector2_1 * (float) (-this.Width / 2 + num3) + this.Outwards * (surfaceHeight2 + 1f), 0.0f);
          this.mesh[surfaceStartIndex + 4].Position = new Vector3(this.Position + vector2_1 * (float) (-this.Width / 2 + num3) + this.Outwards * surfaceHeight2, 0.0f);
          this.mesh[surfaceStartIndex + 5].Position = new Vector3(this.Position + vector2_1 * (float) (-this.Width / 2 + num2) + this.Outwards * surfaceHeight1, 0.0f);
          num1 += 4;
          fillStartIndex += 6;
          surfaceStartIndex += 6;
        }
        Vector2 vector2_2 = this.Position + vector2_1 * ((float) -this.Width / 2f);
        int rayStartIndex = this.rayStartIndex;
        foreach (Water.Ray ray in this.Rays)
        {
          if ((double) ray.Percent > 1.0)
            ray.Reset(0.0f);
          ray.Percent += Engine.DeltaTime / ray.Duration;
          float num2 = 1f;
          if ((double) ray.Percent < 0.100000001490116)
            num2 = Calc.ClampedMap(ray.Percent, 0.0f, 0.1f, 0.0f, 1f);
          else if ((double) ray.Percent > 0.899999976158142)
            num2 = Calc.ClampedMap(ray.Percent, 0.9f, 1f, 1f, 0.0f);
          float position1 = Math.Max(0.0f, ray.Position - ray.Width / 2f);
          float position2 = Math.Min((float) this.Width, ray.Position + ray.Width / 2f);
          float num3 = Math.Min((float) this.BodyHeight, 0.7f * ray.Length);
          float num4 = 0.3f * ray.Length;
          Vector2 vector2_3 = vector2_2 + vector2_1 * position1 + this.Outwards * this.GetSurfaceHeight(position1);
          Vector2 vector2_4 = vector2_2 + vector2_1 * position2 + this.Outwards * this.GetSurfaceHeight(position2);
          Vector2 vector2_5 = vector2_2 + vector2_1 * (position2 - num4) - this.Outwards * num3;
          Vector2 vector2_6 = vector2_2 + vector2_1 * (position1 - num4) - this.Outwards * num3;
          this.mesh[rayStartIndex].Position = new Vector3(vector2_3, 0.0f);
          this.mesh[rayStartIndex].Color = Water.RayTopColor * num2;
          this.mesh[rayStartIndex + 1].Position = new Vector3(vector2_4, 0.0f);
          this.mesh[rayStartIndex + 1].Color = Water.RayTopColor * num2;
          this.mesh[rayStartIndex + 2].Position = new Vector3(vector2_6, 0.0f);
          this.mesh[rayStartIndex + 3].Position = new Vector3(vector2_4, 0.0f);
          this.mesh[rayStartIndex + 3].Color = Water.RayTopColor * num2;
          this.mesh[rayStartIndex + 4].Position = new Vector3(vector2_5, 0.0f);
          this.mesh[rayStartIndex + 5].Position = new Vector3(vector2_6, 0.0f);
          rayStartIndex += 6;
        }
      }

      public float GetSurfaceHeight(Vector2 position)
      {
        return this.GetSurfaceHeight(this.GetPointAlong(position));
      }

      public float GetSurfaceHeight(float position)
      {
        if ((double) position < 0.0 || (double) position > (double) this.Width)
          return 0.0f;
        float num1 = 0.0f;
        foreach (Water.Ripple ripple in this.Ripples)
        {
          float val = Math.Abs(ripple.Position - position);
          float num2 = (double) val >= 12.0 ? Calc.ClampedMap(val, 16f, 32f, -0.75f, 0.0f) : Calc.ClampedMap(val, 0.0f, 16f, 1f, -0.75f);
          num1 += num2 * ripple.Height * Ease.CubeIn(1f - ripple.Percent);
        }
        float num3 = Calc.Clamp(num1, -4f, 4f);
        foreach (Water.Tension tension in this.Tensions)
        {
          float t = Calc.ClampedMap(Math.Abs(tension.Position - position), 0.0f, 24f, 1f, 0.0f);
          num3 += (float) ((double) Ease.CubeOut(t) * (double) tension.Strength * 12.0);
        }
        float val1 = position / (float) this.Width;
        return num3 * Calc.ClampedMap(val1, 0.0f, 0.1f, 0.5f, 1f) * Calc.ClampedMap(val1, 0.9f, 1f, 1f, 0.5f) + (float) Math.Sin((double) this.timer + (double) position * 0.100000001490116) + 6f;
      }

      public void Render(Camera camera)
      {
        GFX.DrawVertices<VertexPositionColor>(camera.Matrix, this.mesh, this.mesh.Length, (Effect) null, (BlendState) null);
      }
    }
  }
}

