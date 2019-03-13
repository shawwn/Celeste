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
    private float timer = 0.0f;
    private List<DisplacementRenderer.Burst> points = new List<DisplacementRenderer.Burst>();

    public bool HasDisplacement(Scene scene)
    {
      return this.points.Count > 0 || scene.Tracker.GetComponent<DisplacementRenderHook>() != null || (scene as Level).Foreground.Get<HeatWave>() != null;
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
      Color color = new Color(0.5f, 0.5f, 0.0f, 1f);
      Engine.Graphics.GraphicsDevice.SetRenderTarget(GameplayBuffers.Displacement.Target);
      Engine.Graphics.GraphicsDevice.Clear(color);
      if (!this.Enabled)
        return;
      Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, (Effect) null, camera.Matrix);
      HeatWave heatWave = (scene as Level).Foreground.Get<HeatWave>();
      if (heatWave != null)
        heatWave.RenderDisplacement(scene as Level);
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
      public float Percent = 0.0f;
      public float ScaleFrom = 0.0f;
      public float ScaleTo = 1f;
      public Ease.Easer ScaleEaser = (Ease.Easer) null;
      public float AlphaFrom = 1f;
      public float AlphaTo = 0.0f;
      public Ease.Easer AlphaEaser = (Ease.Easer) null;
      public Rectangle? WorldClipRect = new Rectangle?();
      public Collider WorldClipCollider = (Collider) null;
      public int WorldClipPadding = 0;
      public MTexture Texture;
      public Entity Follow;
      public Vector2 Position;
      public Vector2 Origin;
      public float Duration;

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
          position += this.Follow.Position;
        float num1 = this.AlphaEaser == null ? this.AlphaFrom + (this.AlphaTo - this.AlphaFrom) * this.Percent : this.AlphaFrom + (this.AlphaTo - this.AlphaFrom) * this.AlphaEaser(this.Percent);
        float num2 = this.ScaleEaser == null ? this.ScaleFrom + (this.ScaleTo - this.ScaleFrom) * this.Percent : this.ScaleFrom + (this.ScaleTo - this.ScaleFrom) * this.ScaleEaser(this.Percent);
        Vector2 origin = this.Origin;
        Rectangle clip = new Rectangle(0, 0, this.Texture.Width, this.Texture.Height);
        if (this.WorldClipCollider != null)
          this.WorldClipRect = new Rectangle?(this.WorldClipCollider.Bounds);
        if (this.WorldClipRect.HasValue)
        {
          Rectangle rectangle = this.WorldClipRect.Value;
          rectangle.X -= 1 + this.WorldClipPadding;
          rectangle.Y -= 1 + this.WorldClipPadding;
          rectangle.Width += 1 + this.WorldClipPadding * 2;
          rectangle.Height += 1 + this.WorldClipPadding * 2;
          float num3 = position.X - origin.X * num2;
          if ((double) num3 < (double) rectangle.Left)
          {
            int num4 = (int) (((double) rectangle.Left - (double) num3) / (double) num2);
            origin.X -= (float) num4;
            clip.X = num4;
            clip.Width -= num4;
          }
          float num5 = position.Y - origin.Y * num2;
          if ((double) num5 < (double) rectangle.Top)
          {
            int num4 = (int) (((double) rectangle.Top - (double) num5) / (double) num2);
            origin.Y -= (float) num4;
            clip.Y = num4;
            clip.Height -= num4;
          }
          float num6 = position.X + ((float) this.Texture.Width - origin.X) * num2;
          if ((double) num6 > (double) rectangle.Right)
          {
            int num4 = (int) (((double) num6 - (double) rectangle.Right) / (double) num2);
            clip.Width -= num4;
          }
          float num7 = position.Y + ((float) this.Texture.Height - origin.Y) * num2;
          if ((double) num7 > (double) rectangle.Bottom)
          {
            int num4 = (int) (((double) num7 - (double) rectangle.Bottom) / (double) num2);
            clip.Height -= num4;
          }
        }
        this.Texture.Draw(position, origin, Color.White * num1, Vector2.One * num2, 0.0f, clip);
      }
    }
  }
}

