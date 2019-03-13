// Decompiled with JetBrains decompiler
// Type: Celeste.DisplacementRenderer
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
  public class DisplacementRenderer : Monocle.Renderer
  {
    public bool Enabled = true;
    private List<DisplacementRenderer.Burst> points = new List<DisplacementRenderer.Burst>();
    private float timer;

    public bool HasDisplacement(Scene scene)
    {
      if (this.points.Count <= 0 && scene.Tracker.GetComponent<DisplacementRenderHook>() == null)
        return (scene as Level).Foreground.Get<HeatWave>() != null;
      return true;
    }

    public DisplacementRenderer.Burst Add(DisplacementRenderer.Burst point)
    {
      this.points.Add(point);
      return point;
    }

    public DisplacementRenderer.Burst Remove(DisplacementRenderer.Burst point)
    {
      this.points.Remove(point);
      return point;
    }

    public DisplacementRenderer.Burst AddBurst(
      Vector2 position,
      float duration,
      float radiusFrom,
      float radiusTo,
      float alpha = 1f,
      Ease.Easer alphaEaser = null,
      Ease.Easer radiusEaser = null)
    {
      MTexture texture = GFX.Game["util/displacementcircle"];
      return this.Add(new DisplacementRenderer.Burst(texture, position, texture.Center, duration)
      {
        ScaleFrom = radiusFrom / (float) (texture.Width / 2),
        ScaleTo = radiusTo / (float) (texture.Width / 2),
        AlphaFrom = alpha,
        AlphaTo = 0.0f,
        AlphaEaser = alphaEaser
      });
    }

    public override void Update(Scene scene)
    {
      this.timer += Engine.DeltaTime;
      for (int index = this.points.Count - 1; index >= 0; --index)
      {
        if ((double) this.points[index].Percent >= 1.0)
          this.points.RemoveAt(index);
        else
          this.points[index].Update();
      }
    }

    public void Clear()
    {
      this.points.Clear();
    }

    public override void BeforeRender(Scene scene)
    {
      Distort.WaterSine = this.timer * 16f;
      Distort.WaterCameraY = (float) (int) Math.Floor((double) (scene as Level).Camera.Y);
      Camera camera = (scene as Level).Camera;
      Color color;
      ((Color) ref color).\u002Ector(0.5f, 0.5f, 0.0f, 1f);
      Engine.Graphics.get_GraphicsDevice().SetRenderTarget(GameplayBuffers.Displacement.Target);
      Engine.Graphics.get_GraphicsDevice().Clear(color);
      if (!this.Enabled)
        return;
      Draw.SpriteBatch.Begin((SpriteSortMode) 0, (BlendState) BlendState.AlphaBlend, (SamplerState) SamplerState.LinearClamp, (DepthStencilState) DepthStencilState.Default, (RasterizerState) RasterizerState.CullNone, (Effect) null, camera.Matrix);
      (scene as Level).Foreground.Get<HeatWave>()?.RenderDisplacement(scene as Level);
      foreach (DisplacementRenderHook component in scene.Tracker.GetComponents<DisplacementRenderHook>())
      {
        if (component.Visible && component.RenderDisplacement != null)
          component.RenderDisplacement();
      }
      foreach (DisplacementRenderer.Burst point in this.points)
        point.Render();
      foreach (Entity entity in scene.Tracker.GetEntities<FakeWall>())
        Draw.Rect(entity.X, entity.Y, entity.Width, entity.Height, color);
      Draw.SpriteBatch.End();
    }

    public class Burst
    {
      public float ScaleTo = 1f;
      public float AlphaFrom = 1f;
      public MTexture Texture;
      public Entity Follow;
      public Vector2 Position;
      public Vector2 Origin;
      public float Duration;
      public float Percent;
      public float ScaleFrom;
      public Ease.Easer ScaleEaser;
      public float AlphaTo;
      public Ease.Easer AlphaEaser;
      public Rectangle? WorldClipRect;
      public Collider WorldClipCollider;
      public int WorldClipPadding;

      public Burst(MTexture texture, Vector2 position, Vector2 origin, float duration)
      {
        this.Texture = texture;
        this.Position = position;
        this.Origin = origin;
        this.Duration = duration;
      }

      public void Update()
      {
        this.Percent += Engine.DeltaTime / this.Duration;
      }

      public void Render()
      {
        Vector2 position = this.Position;
        if (this.Follow != null)
          position = Vector2.op_Addition(position, this.Follow.Position);
        float num1 = this.AlphaEaser == null ? this.AlphaFrom + (this.AlphaTo - this.AlphaFrom) * this.Percent : this.AlphaFrom + (this.AlphaTo - this.AlphaFrom) * this.AlphaEaser(this.Percent);
        float num2 = this.ScaleEaser == null ? this.ScaleFrom + (this.ScaleTo - this.ScaleFrom) * this.Percent : this.ScaleFrom + (this.ScaleTo - this.ScaleFrom) * this.ScaleEaser(this.Percent);
        Vector2 origin = this.Origin;
        Rectangle clip;
        ((Rectangle) ref clip).\u002Ector(0, 0, this.Texture.Width, this.Texture.Height);
        if (this.WorldClipCollider != null)
          this.WorldClipRect = new Rectangle?(this.WorldClipCollider.Bounds);
        if (this.WorldClipRect.HasValue)
        {
          Rectangle rectangle = this.WorldClipRect.Value;
          ref __Null local1 = ref rectangle.X;
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          ^(int&) ref local1 = ^(int&) ref local1 - (1 + this.WorldClipPadding);
          ref __Null local2 = ref rectangle.Y;
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          ^(int&) ref local2 = ^(int&) ref local2 - (1 + this.WorldClipPadding);
          ref __Null local3 = ref rectangle.Width;
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          ^(int&) ref local3 = ^(int&) ref local3 + (1 + this.WorldClipPadding * 2);
          ref __Null local4 = ref rectangle.Height;
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          ^(int&) ref local4 = ^(int&) ref local4 + (1 + this.WorldClipPadding * 2);
          float num3 = (float) (position.X - origin.X * (double) num2);
          if ((double) num3 < (double) ((Rectangle) ref rectangle).get_Left())
          {
            int num4 = (int) (((double) ((Rectangle) ref rectangle).get_Left() - (double) num3) / (double) num2);
            ref __Null local5 = ref origin.X;
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            ^(float&) ref local5 = ^(float&) ref local5 - (float) num4;
            clip.X = (__Null) num4;
            ref __Null local6 = ref clip.Width;
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            ^(int&) ref local6 = ^(int&) ref local6 - num4;
          }
          float num5 = (float) (position.Y - origin.Y * (double) num2);
          if ((double) num5 < (double) ((Rectangle) ref rectangle).get_Top())
          {
            int num4 = (int) (((double) ((Rectangle) ref rectangle).get_Top() - (double) num5) / (double) num2);
            ref __Null local5 = ref origin.Y;
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            ^(float&) ref local5 = ^(float&) ref local5 - (float) num4;
            clip.Y = (__Null) num4;
            ref __Null local6 = ref clip.Height;
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            ^(int&) ref local6 = ^(int&) ref local6 - num4;
          }
          float num6 = (float) (position.X + ((double) this.Texture.Width - origin.X) * (double) num2);
          if ((double) num6 > (double) ((Rectangle) ref rectangle).get_Right())
          {
            int num4 = (int) (((double) num6 - (double) ((Rectangle) ref rectangle).get_Right()) / (double) num2);
            ref __Null local5 = ref clip.Width;
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            ^(int&) ref local5 = ^(int&) ref local5 - num4;
          }
          float num7 = (float) (position.Y + ((double) this.Texture.Height - origin.Y) * (double) num2);
          if ((double) num7 > (double) ((Rectangle) ref rectangle).get_Bottom())
          {
            int num4 = (int) (((double) num7 - (double) ((Rectangle) ref rectangle).get_Bottom()) / (double) num2);
            ref __Null local5 = ref clip.Height;
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            ^(int&) ref local5 = ^(int&) ref local5 - num4;
          }
        }
        this.Texture.Draw(position, origin, Color.op_Multiply(Color.get_White(), num1), Vector2.op_Multiply(Vector2.get_One(), num2), 0.0f, clip);
      }
    }
  }
}
