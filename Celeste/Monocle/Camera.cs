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
    private Matrix matrix = Matrix.get_Identity();
    private Matrix inverse = Matrix.get_Identity();
    private Vector2 position = Vector2.get_Zero();
    private Vector2 zoom = Vector2.get_One();
    private Vector2 origin = Vector2.get_Zero();
    private bool changed;
    private float angle;
    public Viewport Viewport;

    public Camera()
    {
      this.Viewport = (Viewport) null;
      ((Viewport) ref this.Viewport).set_Width(Engine.Width);
      ((Viewport) ref this.Viewport).set_Height(Engine.Height);
      this.UpdateMatrices();
    }

    public Camera(int width, int height)
    {
      this.Viewport = (Viewport) null;
      ((Viewport) ref this.Viewport).set_Width(width);
      ((Viewport) ref this.Viewport).set_Height(height);
      this.UpdateMatrices();
    }

    public override string ToString()
    {
      return "Camera:\n\tViewport: { " + (object) ((Viewport) ref this.Viewport).get_X() + ", " + (object) ((Viewport) ref this.Viewport).get_Y() + ", " + (object) ((Viewport) ref this.Viewport).get_Width() + ", " + (object) ((Viewport) ref this.Viewport).get_Height() + " }\n\tPosition: { " + (object) (float) this.position.X + ", " + (object) (float) this.position.Y + " }\n\tOrigin: { " + (object) (float) this.origin.X + ", " + (object) (float) this.origin.Y + " }\n\tZoom: { " + (object) (float) this.zoom.X + ", " + (object) (float) this.zoom.Y + " }\n\tAngle: " + (object) this.angle;
    }

    private void UpdateMatrices()
    {
      this.matrix = Matrix.op_Multiply(Matrix.op_Multiply(Matrix.op_Multiply(Matrix.op_Multiply(Matrix.get_Identity(), Matrix.CreateTranslation(new Vector3(Vector2.op_UnaryNegation(new Vector2((float) (int) Math.Floor((double) this.position.X), (float) (int) Math.Floor((double) this.position.Y))), 0.0f))), Matrix.CreateRotationZ(this.angle)), Matrix.CreateScale(new Vector3(this.zoom, 1f))), Matrix.CreateTranslation(new Vector3(new Vector2((float) (int) Math.Floor((double) this.origin.X), (float) (int) Math.Floor((double) this.origin.Y)), 0.0f)));
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
        return (float) this.position.X;
      }
      set
      {
        this.changed = true;
        this.position.X = (__Null) (double) value;
      }
    }

    public float Y
    {
      get
      {
        return (float) this.position.Y;
      }
      set
      {
        this.changed = true;
        this.position.Y = (__Null) (double) value;
      }
    }

    public float Zoom
    {
      get
      {
        return (float) this.zoom.X;
      }
      set
      {
        this.changed = true;
        this.zoom.X = (__Null) (double) (this.zoom.Y = (__Null) value);
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
        return (float) Vector2.Transform(Vector2.get_Zero(), this.Inverse).X;
      }
      set
      {
        if (this.changed)
          this.UpdateMatrices();
        this.X = (float) Vector2.Transform(Vector2.op_Multiply(Vector2.get_UnitX(), value), this.Matrix).X;
      }
    }

    public float Right
    {
      get
      {
        if (this.changed)
          this.UpdateMatrices();
        return (float) Vector2.Transform(Vector2.op_Multiply(Vector2.get_UnitX(), (float) ((Viewport) ref this.Viewport).get_Width()), this.Inverse).X;
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
        return (float) Vector2.Transform(Vector2.get_Zero(), this.Inverse).Y;
      }
      set
      {
        if (this.changed)
          this.UpdateMatrices();
        this.Y = (float) Vector2.Transform(Vector2.op_Multiply(Vector2.get_UnitY(), value), this.Matrix).Y;
      }
    }

    public float Bottom
    {
      get
      {
        if (this.changed)
          this.UpdateMatrices();
        return (float) Vector2.Transform(Vector2.op_Multiply(Vector2.get_UnitY(), (float) ((Viewport) ref this.Viewport).get_Height()), this.Inverse).Y;
      }
      set
      {
        throw new NotImplementedException();
      }
    }

    public void CenterOrigin()
    {
      this.origin = new Vector2((float) ((Viewport) ref this.Viewport).get_Width() / 2f, (float) ((Viewport) ref this.Viewport).get_Height() / 2f);
      this.changed = true;
    }

    public void RoundPosition()
    {
      this.position.X = (__Null) Math.Round((double) this.position.X);
      this.position.Y = (__Null) Math.Round((double) this.position.Y);
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
      this.Position = Vector2.op_Addition(this.Position, Vector2.op_Multiply(Vector2.op_Subtraction(position, this.Position), ease));
    }

    public void Approach(Vector2 position, float ease, float maxDistance)
    {
      Vector2 vector2 = Vector2.op_Multiply(Vector2.op_Subtraction(position, this.Position), ease);
      if ((double) ((Vector2) ref vector2).Length() > (double) maxDistance)
        this.Position = Vector2.op_Addition(this.Position, Vector2.op_Multiply(Vector2.Normalize(vector2), maxDistance));
      else
        this.Position = Vector2.op_Addition(this.Position, vector2);
    }
  }
}
