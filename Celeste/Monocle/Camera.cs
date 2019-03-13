// Decompiled with JetBrains decompiler
// Type: Monocle.Camera
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Monocle
{
  public class Camera
  {
    private Matrix matrix = Matrix.Identity;
    private Matrix inverse = Matrix.Identity;
    private Vector2 position = Vector2.Zero;
    private Vector2 zoom = Vector2.One;
    private Vector2 origin = Vector2.Zero;
    private float angle = 0.0f;
    private bool changed;
    public Viewport Viewport;

    public Camera()
    {
      this.Viewport = new Viewport();
      this.Viewport.Width = Engine.Width;
      this.Viewport.Height = Engine.Height;
      this.UpdateMatrices();
    }

    public Camera(int width, int height)
    {
      this.Viewport = new Viewport();
      this.Viewport.Width = width;
      this.Viewport.Height = height;
      this.UpdateMatrices();
    }

    public override string ToString()
    {
      return "Camera:\n\tViewport: { " + (object) this.Viewport.X + ", " + (object) this.Viewport.Y + ", " + (object) this.Viewport.Width + ", " + (object) this.Viewport.Height + " }\n\tPosition: { " + (object) this.position.X + ", " + (object) this.position.Y + " }\n\tOrigin: { " + (object) this.origin.X + ", " + (object) this.origin.Y + " }\n\tZoom: { " + (object) this.zoom.X + ", " + (object) this.zoom.Y + " }\n\tAngle: " + (object) this.angle;
    }

    private void UpdateMatrices()
    {
      this.matrix = Matrix.Identity * Matrix.CreateTranslation(new Vector3(-new Vector2((float) (int) Math.Floor((double) this.position.X), (float) (int) Math.Floor((double) this.position.Y)), 0.0f)) * Matrix.CreateRotationZ(this.angle) * Matrix.CreateScale(new Vector3(this.zoom, 1f)) * Matrix.CreateTranslation(new Vector3(new Vector2((float) (int) Math.Floor((double) this.origin.X), (float) (int) Math.Floor((double) this.origin.Y)), 0.0f));
      this.inverse = Matrix.Invert(this.matrix);
      this.changed = false;
    }

    public void CopyFrom(Camera other)
    {
      this.position = other.position;
      this.origin = other.origin;
      this.angle = other.angle;
      this.zoom = other.zoom;
      this.changed = true;
    }

    public Matrix Matrix
    {
      get
      {
        if (this.changed)
          this.UpdateMatrices();
        return this.matrix;
      }
    }

    public Matrix Inverse
    {
      get
      {
        if (this.changed)
          this.UpdateMatrices();
        return this.inverse;
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
        this.changed = true;
        this.position = value;
      }
    }

    public Vector2 Origin
    {
      get
      {
        return this.origin;
      }
      set
      {
        this.changed = true;
        this.origin = value;
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
        this.changed = true;
        this.position.X = value;
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
        this.changed = true;
        this.position.Y = value;
      }
    }

    public float Zoom
    {
      get
      {
        return this.zoom.X;
      }
      set
      {
        this.changed = true;
        this.zoom.X = this.zoom.Y = value;
      }
    }

    public float Angle
    {
      get
      {
        return this.angle;
      }
      set
      {
        this.changed = true;
        this.angle = value;
      }
    }

    public float Left
    {
      get
      {
        if (this.changed)
          this.UpdateMatrices();
        return Vector2.Transform(Vector2.Zero, this.Inverse).X;
      }
      set
      {
        if (this.changed)
          this.UpdateMatrices();
        this.X = Vector2.Transform(Vector2.UnitX * value, this.Matrix).X;
      }
    }

    public float Right
    {
      get
      {
        if (this.changed)
          this.UpdateMatrices();
        return Vector2.Transform(Vector2.UnitX * (float) this.Viewport.Width, this.Inverse).X;
      }
      set
      {
        throw new NotImplementedException();
      }
    }

    public float Top
    {
      get
      {
        if (this.changed)
          this.UpdateMatrices();
        return Vector2.Transform(Vector2.Zero, this.Inverse).Y;
      }
      set
      {
        if (this.changed)
          this.UpdateMatrices();
        this.Y = Vector2.Transform(Vector2.UnitY * value, this.Matrix).Y;
      }
    }

    public float Bottom
    {
      get
      {
        if (this.changed)
          this.UpdateMatrices();
        return Vector2.Transform(Vector2.UnitY * (float) this.Viewport.Height, this.Inverse).Y;
      }
      set
      {
        throw new NotImplementedException();
      }
    }

    public void CenterOrigin()
    {
      this.origin = new Vector2((float) this.Viewport.Width / 2f, (float) this.Viewport.Height / 2f);
      this.changed = true;
    }

    public void RoundPosition()
    {
      this.position.X = (float) Math.Round((double) this.position.X);
      this.position.Y = (float) Math.Round((double) this.position.Y);
      this.changed = true;
    }

    public Vector2 ScreenToCamera(Vector2 position)
    {
      return Vector2.Transform(position, this.Inverse);
    }

    public Vector2 CameraToScreen(Vector2 position)
    {
      return Vector2.Transform(position, this.Matrix);
    }

    public void Approach(Vector2 position, float ease)
    {
      this.Position += (position - this.Position) * ease;
    }

    public void Approach(Vector2 position, float ease, float maxDistance)
    {
      Vector2 vector2 = (position - this.Position) * ease;
      if ((double) vector2.Length() > (double) maxDistance)
        this.Position += Vector2.Normalize(vector2) * maxDistance;
      else
        this.Position += vector2;
    }
  }
}

