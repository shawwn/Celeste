// Decompiled with JetBrains decompiler
// Type: Celeste.LightingRenderer
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System.Collections.Generic;

namespace Celeste
{
  public class LightingRenderer : Monocle.Renderer
  {
    public Color BaseColor = Color.get_Black();
    public float Alpha = 0.1f;
    private VertexPositionColor[] verts = new VertexPositionColor[8192];
    private LightingRenderer.VertexPositionColorMaskTexture[] resultVerts = new LightingRenderer.VertexPositionColorMaskTexture[384];
    private int[] indices = new int[8192];
    private float nonSpotlightAlphaMultiplier = 1f;
    private Vector3[] angles = new Vector3[20];
    public static BlendState GradientBlendState;
    public static BlendState OccludeBlendState;
    public const int TextureSize = 1024;
    public const int TextureSplit = 4;
    public const int Channels = 4;
    public const int Padding = 8;
    public const int CircleSegments = 20;
    private const int Cells = 16;
    private const int MaxLights = 64;
    private const int Radius = 128;
    private const int LightRadius = 120;
    private int vertexCount;
    private int indexCount;
    private VertexLight[] lights;
    private VertexLight spotlight;
    private bool inSpotlight;

    public LightingRenderer()
    {
      this.lights = new VertexLight[64];
      for (int index = 0; index < 20; ++index)
        this.angles[index] = new Vector3(Calc.AngleToVector((float) ((double) index / 20.0 * 6.28318548202515), 1f), 0.0f);
    }

    public VertexLight SetSpotlight(VertexLight light)
    {
      this.spotlight = light;
      this.inSpotlight = true;
      return light;
    }

    public void UnsetSpotlight()
    {
      this.inSpotlight = false;
    }

    public override void Update(Scene scene)
    {
      this.nonSpotlightAlphaMultiplier = Calc.Approach(this.nonSpotlightAlphaMultiplier, this.inSpotlight ? 0.0f : 1f, Engine.DeltaTime * 2f);
      base.Update(scene);
    }

    public override void BeforeRender(Scene scene)
    {
      Level level = scene as Level;
      Camera camera = level.Camera;
      for (int index = 0; index < 64; ++index)
      {
        if (this.lights[index] != null && this.lights[index].Entity.Scene != scene)
        {
          this.lights[index].Index = -1;
          this.lights[index] = (VertexLight) null;
        }
      }
      foreach (VertexLight component in scene.Tracker.GetComponents<VertexLight>())
      {
        if ((component.Entity == null || !component.Entity.Visible || (!component.Visible || (double) component.Alpha <= 0.0) || (((Color) ref component.Color).get_A() <= (byte) 0 || component.Center.X + (double) component.EndRadius <= (double) camera.X || (component.Center.Y + (double) component.EndRadius <= (double) camera.Y || component.Center.X - (double) component.EndRadius >= (double) camera.X + 320.0)) ? 0 : (component.Center.Y - (double) component.EndRadius < (double) camera.Y + 180.0 ? 1 : 0)) != 0)
        {
          if (component.Index < 0)
          {
            component.Dirty = true;
            for (int index = 0; index < 64; ++index)
            {
              if (this.lights[index] == null)
              {
                this.lights[index] = component;
                component.Index = index;
                break;
              }
            }
          }
          if (Vector2.op_Inequality(component.LastPosition, component.Position) || Vector2.op_Inequality(component.LastEntityPosition, component.Entity.Position) || component.Dirty)
          {
            component.LastPosition = component.Position;
            component.InSolid = false;
            foreach (Solid solid in scene.CollideAll<Solid>(component.Center))
            {
              if (solid.DisableLightsInside)
              {
                component.InSolid = true;
                break;
              }
            }
            if (!component.InSolid)
              component.LastNonSolidPosition = component.Center;
            if (component.InSolid && !component.Started)
              component.InSolidAlphaMultiplier = 0.0f;
          }
          if (Vector2.op_Inequality(component.Entity.Position, component.LastEntityPosition))
          {
            component.Dirty = true;
            component.LastEntityPosition = component.Entity.Position;
          }
          component.Started = true;
        }
        else if (component.Index >= 0)
        {
          this.lights[component.Index] = (VertexLight) null;
          component.Index = -1;
          component.Started = false;
        }
      }
      Engine.Graphics.get_GraphicsDevice().SetRenderTarget((RenderTarget2D) GameplayBuffers.LightBuffer);
      Engine.Instance.get_GraphicsDevice().set_RasterizerState((RasterizerState) RasterizerState.CullNone);
      Matrix matrix = Matrix.op_Multiply(Matrix.op_Multiply(Matrix.CreateScale(0.0009765625f), Matrix.CreateScale(2f, -2f, 1f)), Matrix.CreateTranslation(-1f, 1f, 0.0f));
      this.ClearDirtyLights(matrix);
      this.DrawLightGradients(matrix);
      this.DrawLightOccluders(matrix, level);
      Engine.Graphics.get_GraphicsDevice().SetRenderTarget((RenderTarget2D) GameplayBuffers.Light);
      Engine.Graphics.get_GraphicsDevice().Clear(this.BaseColor);
      Engine.Graphics.get_GraphicsDevice().get_Textures().set_Item(0, (Texture) (RenderTarget2D) GameplayBuffers.LightBuffer);
      this.StartDrawingPrimitives();
      for (int index = 0; index < 64; ++index)
      {
        VertexLight light = this.lights[index];
        if (light != null)
        {
          light.Dirty = false;
          float num1 = light.Alpha * light.InSolidAlphaMultiplier;
          if ((double) this.nonSpotlightAlphaMultiplier < 1.0 && light != this.spotlight)
            num1 *= this.nonSpotlightAlphaMultiplier;
          if ((double) num1 > 0.0 && ((Color) ref light.Color).get_A() > (byte) 0 && (double) light.EndRadius >= 2.0)
          {
            int num2 = 128;
            while ((double) light.EndRadius <= (double) (num2 / 2))
              num2 /= 2;
            this.DrawLight(index, light.InSolid ? light.LastNonSolidPosition : light.Center, Color.op_Multiply(light.Color, num1), (float) num2);
          }
        }
      }
      if (this.vertexCount > 0)
        GFX.DrawIndexedVertices<LightingRenderer.VertexPositionColorMaskTexture>(camera.Matrix, this.resultVerts, this.vertexCount, this.indices, this.indexCount / 3, GFX.FxLighting, (BlendState) BlendState.Additive);
      GaussianBlur.Blur((Texture2D) (RenderTarget2D) GameplayBuffers.Light, GameplayBuffers.TempA, GameplayBuffers.Light, 0.0f, true, GaussianBlur.Samples.Nine, 1f, GaussianBlur.Direction.Both, 1f);
    }

    private void ClearDirtyLights(Matrix matrix)
    {
      this.StartDrawingPrimitives();
      for (int index = 0; index < 64; ++index)
      {
        VertexLight light = this.lights[index];
        if (light != null && light.Dirty)
          this.SetClear(index);
      }
      if (this.vertexCount <= 0)
        return;
      Engine.Instance.get_GraphicsDevice().set_BlendState(LightingRenderer.OccludeBlendState);
      GFX.FxPrimitive.get_Parameters().get_Item("World").SetValue(matrix);
      using (List<EffectPass>.Enumerator enumerator = GFX.FxPrimitive.get_CurrentTechnique().get_Passes().GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          enumerator.Current.Apply();
          Engine.Instance.get_GraphicsDevice().DrawUserIndexedPrimitives<VertexPositionColor>((PrimitiveType) 0, (M0[]) this.verts, 0, this.vertexCount, this.indices, 0, this.indexCount / 3);
        }
      }
    }

    private void DrawLightGradients(Matrix matrix)
    {
      this.StartDrawingPrimitives();
      for (int index = 0; index < 64; ++index)
      {
        VertexLight light = this.lights[index];
        if (light != null && light.Dirty)
          this.SetGradient(index, Calc.Clamp(light.StartRadius, 0.0f, 120f), Calc.Clamp(light.EndRadius, 0.0f, 120f));
      }
      if (this.vertexCount <= 0)
        return;
      Engine.Instance.get_GraphicsDevice().set_BlendState(LightingRenderer.GradientBlendState);
      GFX.FxPrimitive.get_Parameters().get_Item("World").SetValue(matrix);
      using (List<EffectPass>.Enumerator enumerator = GFX.FxPrimitive.get_CurrentTechnique().get_Passes().GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          enumerator.Current.Apply();
          Engine.Instance.get_GraphicsDevice().DrawUserIndexedPrimitives<VertexPositionColor>((PrimitiveType) 0, (M0[]) this.verts, 0, this.vertexCount, this.indices, 0, this.indexCount / 3);
        }
      }
    }

    private void DrawLightOccluders(Matrix matrix, Level level)
    {
      this.StartDrawingPrimitives();
      Rectangle tileBounds = level.Session.MapData.TileBounds;
      List<Component> components1 = level.Tracker.GetComponents<LightOcclude>();
      List<Component> components2 = level.Tracker.GetComponents<EffectCutout>();
      foreach (LightOcclude lightOcclude in components1)
      {
        if (lightOcclude.Visible && lightOcclude.Entity.Visible)
          lightOcclude.RenderBounds = new Rectangle(lightOcclude.Left, lightOcclude.Top, lightOcclude.Width, lightOcclude.Height);
      }
      for (int index = 0; index < 64; ++index)
      {
        VertexLight light1 = this.lights[index];
        if (light1 != null && light1.Dirty)
        {
          Vector2 light2 = light1.InSolid ? light1.LastNonSolidPosition : light1.Center;
          Rectangle clamp;
          ((Rectangle) ref clamp).\u002Ector((int) (light2.X - (double) light1.EndRadius), (int) (light2.Y - (double) light1.EndRadius), (int) light1.EndRadius * 2, (int) light1.EndRadius * 2);
          Vector3 center = this.GetCenter(index);
          Color mask1 = this.GetMask(index, 0.0f, 1f);
          foreach (LightOcclude lightOcclude in components1)
          {
            if (lightOcclude.Visible && lightOcclude.Entity.Visible && (double) lightOcclude.Alpha > 0.0)
            {
              Rectangle renderBounds = lightOcclude.RenderBounds;
              if (((Rectangle) ref renderBounds).Intersects(clamp))
              {
                Rectangle rectangle = renderBounds.ClampTo(clamp);
                Color mask2 = this.GetMask(index, 1f - lightOcclude.Alpha, 1f);
                if (((Rectangle) ref rectangle).get_Bottom() > ((Rectangle) ref clamp).get_Top() && ((Rectangle) ref rectangle).get_Bottom() < ((Rectangle) ref clamp).get_Center().Y)
                  this.SetOccluder(center, mask2, light2, new Vector2((float) ((Rectangle) ref rectangle).get_Left(), (float) ((Rectangle) ref rectangle).get_Bottom()), new Vector2((float) ((Rectangle) ref rectangle).get_Right(), (float) ((Rectangle) ref rectangle).get_Bottom()));
                if (((Rectangle) ref rectangle).get_Top() < ((Rectangle) ref clamp).get_Bottom() && ((Rectangle) ref rectangle).get_Top() > ((Rectangle) ref clamp).get_Center().Y)
                  this.SetOccluder(center, mask2, light2, new Vector2((float) ((Rectangle) ref rectangle).get_Left(), (float) ((Rectangle) ref rectangle).get_Top()), new Vector2((float) ((Rectangle) ref rectangle).get_Right(), (float) ((Rectangle) ref rectangle).get_Top()));
                if (((Rectangle) ref rectangle).get_Right() > ((Rectangle) ref clamp).get_Left() && ((Rectangle) ref rectangle).get_Right() < ((Rectangle) ref clamp).get_Center().X)
                  this.SetOccluder(center, mask2, light2, new Vector2((float) ((Rectangle) ref rectangle).get_Right(), (float) ((Rectangle) ref rectangle).get_Top()), new Vector2((float) ((Rectangle) ref rectangle).get_Right(), (float) ((Rectangle) ref rectangle).get_Bottom()));
                if (((Rectangle) ref rectangle).get_Left() < ((Rectangle) ref clamp).get_Right() && ((Rectangle) ref rectangle).get_Left() > ((Rectangle) ref clamp).get_Center().X)
                  this.SetOccluder(center, mask2, light2, new Vector2((float) ((Rectangle) ref rectangle).get_Left(), (float) ((Rectangle) ref rectangle).get_Top()), new Vector2((float) ((Rectangle) ref rectangle).get_Left(), (float) ((Rectangle) ref rectangle).get_Bottom()));
              }
            }
          }
          int num1 = ((Rectangle) ref clamp).get_Left() / 8 - ((Rectangle) ref tileBounds).get_Left();
          int num2 = ((Rectangle) ref clamp).get_Top() / 8 - ((Rectangle) ref tileBounds).get_Top();
          int num3 = clamp.Height / 8;
          int num4 = clamp.Width / 8;
          int num5 = num1 + num4;
          int num6 = num2 + num3;
          for (int y = num2; y < num2 + num3 / 2; ++y)
          {
            for (int x = num1; x < num5; ++x)
            {
              if (level.SolidsData.SafeCheck(x, y) != '0' && level.SolidsData.SafeCheck(x, y + 1) == '0')
              {
                int num7 = x;
                do
                {
                  ++x;
                }
                while (x < num5 && level.SolidsData.SafeCheck(x, y) != '0' && level.SolidsData.SafeCheck(x, y + 1) == '0');
                this.SetOccluder(center, mask1, light2, Vector2.op_Multiply(new Vector2((float) (tileBounds.X + num7), (float) (tileBounds.Y + y + 1)), 8f), Vector2.op_Multiply(new Vector2((float) (tileBounds.X + x), (float) (tileBounds.Y + y + 1)), 8f));
              }
            }
          }
          for (int x = num1; x < num1 + num4 / 2; ++x)
          {
            for (int y = num2; y < num6; ++y)
            {
              if (level.SolidsData.SafeCheck(x, y) != '0' && level.SolidsData.SafeCheck(x + 1, y) == '0')
              {
                int num7 = y;
                do
                {
                  ++y;
                }
                while (y < num6 && level.SolidsData.SafeCheck(x, y) != '0' && level.SolidsData.SafeCheck(x + 1, y) == '0');
                this.SetOccluder(center, mask1, light2, Vector2.op_Multiply(new Vector2((float) (tileBounds.X + x + 1), (float) (tileBounds.Y + num7)), 8f), Vector2.op_Multiply(new Vector2((float) (tileBounds.X + x + 1), (float) (tileBounds.Y + y)), 8f));
              }
            }
          }
          for (int y = num2 + num3 / 2; y < num6; ++y)
          {
            for (int x = num1; x < num5; ++x)
            {
              if (level.SolidsData.SafeCheck(x, y) != '0' && level.SolidsData.SafeCheck(x, y - 1) == '0')
              {
                int num7 = x;
                do
                {
                  ++x;
                }
                while (x < num5 && level.SolidsData.SafeCheck(x, y) != '0' && level.SolidsData.SafeCheck(x, y - 1) == '0');
                this.SetOccluder(center, mask1, light2, Vector2.op_Multiply(new Vector2((float) (tileBounds.X + num7), (float) (tileBounds.Y + y)), 8f), Vector2.op_Multiply(new Vector2((float) (tileBounds.X + x), (float) (tileBounds.Y + y)), 8f));
              }
            }
          }
          for (int x = num1 + num4 / 2; x < num5; ++x)
          {
            for (int y = num2; y < num6; ++y)
            {
              if (level.SolidsData.SafeCheck(x, y) != '0' && level.SolidsData.SafeCheck(x - 1, y) == '0')
              {
                int num7 = y;
                do
                {
                  ++y;
                }
                while (y < num6 && level.SolidsData.SafeCheck(x, y) != '0' && level.SolidsData.SafeCheck(x - 1, y) == '0');
                this.SetOccluder(center, mask1, light2, Vector2.op_Multiply(new Vector2((float) (tileBounds.X + x), (float) (tileBounds.Y + num7)), 8f), Vector2.op_Multiply(new Vector2((float) (tileBounds.X + x), (float) (tileBounds.Y + y)), 8f));
              }
            }
          }
          foreach (EffectCutout effectCutout in components2)
          {
            if (effectCutout.Visible && effectCutout.Entity.Visible && (double) effectCutout.Alpha > 0.0)
            {
              Rectangle bounds = effectCutout.Bounds;
              if (((Rectangle) ref bounds).Intersects(clamp))
              {
                Rectangle rectangle = bounds.ClampTo(clamp);
                Color mask2 = this.GetMask(index, 1f - effectCutout.Alpha, 1f);
                this.SetCutout(center, mask2, light2, (float) rectangle.X, (float) rectangle.Y, (float) rectangle.Width, (float) rectangle.Height);
              }
            }
          }
          for (int x = num1; x < num5; ++x)
          {
            for (int y = num2; y < num6; ++y)
            {
              if (level.FgTilesLightMask.Tiles.SafeCheck(x, y) != null)
                this.SetCutout(center, mask1, light2, (float) ((tileBounds.X + x) * 8), (float) ((tileBounds.Y + y) * 8), 8f, 8f);
            }
          }
        }
      }
      if (this.vertexCount <= 0)
        return;
      Engine.Instance.get_GraphicsDevice().set_BlendState(LightingRenderer.OccludeBlendState);
      GFX.FxPrimitive.get_Parameters().get_Item("World").SetValue(matrix);
      using (List<EffectPass>.Enumerator enumerator = GFX.FxPrimitive.get_CurrentTechnique().get_Passes().GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          enumerator.Current.Apply();
          Engine.Instance.get_GraphicsDevice().DrawUserIndexedPrimitives<VertexPositionColor>((PrimitiveType) 0, (M0[]) this.verts, 0, this.vertexCount, this.indices, 0, this.indexCount / 3);
        }
      }
    }

    private Color GetMask(int index, float maskOn, float maskOff)
    {
      int num = index / 16;
      return new Color(num == 0 ? maskOn : maskOff, num == 1 ? maskOn : maskOff, num == 2 ? maskOn : maskOff, num == 3 ? maskOn : maskOff);
    }

    private Vector3 GetCenter(int index)
    {
      int num = index % 16;
      return new Vector3((float) (128.0 * ((double) (num % 4) + 0.5) * 2.0), (float) (128.0 * ((double) (num / 4) + 0.5) * 2.0), 0.0f);
    }

    private void StartDrawingPrimitives()
    {
      this.vertexCount = 0;
      this.indexCount = 0;
    }

    private void SetClear(int index)
    {
      Vector3 center = this.GetCenter(index);
      Color mask = this.GetMask(index, 0.0f, 1f);
      this.indices[this.indexCount++] = this.vertexCount;
      this.indices[this.indexCount++] = this.vertexCount + 1;
      this.indices[this.indexCount++] = this.vertexCount + 2;
      this.indices[this.indexCount++] = this.vertexCount;
      this.indices[this.indexCount++] = this.vertexCount + 2;
      this.indices[this.indexCount++] = this.vertexCount + 3;
      this.verts[this.vertexCount].Position = (__Null) Vector3.op_Addition(center, new Vector3((float) sbyte.MinValue, (float) sbyte.MinValue, 0.0f));
      this.verts[this.vertexCount++].Color = (__Null) mask;
      this.verts[this.vertexCount].Position = (__Null) Vector3.op_Addition(center, new Vector3(128f, (float) sbyte.MinValue, 0.0f));
      this.verts[this.vertexCount++].Color = (__Null) mask;
      this.verts[this.vertexCount].Position = (__Null) Vector3.op_Addition(center, new Vector3(128f, 128f, 0.0f));
      this.verts[this.vertexCount++].Color = (__Null) mask;
      this.verts[this.vertexCount].Position = (__Null) Vector3.op_Addition(center, new Vector3((float) sbyte.MinValue, 128f, 0.0f));
      this.verts[this.vertexCount++].Color = (__Null) mask;
    }

    private void SetGradient(int index, float startFade, float endFade)
    {
      Vector3 center = this.GetCenter(index);
      Color mask = this.GetMask(index, 1f, 0.0f);
      int vertexCount = this.vertexCount;
      this.verts[this.vertexCount].Position = (__Null) center;
      this.verts[this.vertexCount].Color = (__Null) mask;
      ++this.vertexCount;
      for (int index1 = 0; index1 < 20; ++index1)
      {
        this.verts[this.vertexCount].Position = (__Null) Vector3.op_Addition(center, Vector3.op_Multiply(this.angles[index1], startFade));
        this.verts[this.vertexCount].Color = (__Null) mask;
        ++this.vertexCount;
        this.verts[this.vertexCount].Position = (__Null) Vector3.op_Addition(center, Vector3.op_Multiply(this.angles[index1], endFade));
        this.verts[this.vertexCount].Color = (__Null) Color.get_Transparent();
        ++this.vertexCount;
        int num1 = index1;
        int num2 = (index1 + 1) % 20;
        this.indices[this.indexCount++] = vertexCount;
        this.indices[this.indexCount++] = vertexCount + 1 + num1 * 2;
        this.indices[this.indexCount++] = vertexCount + 1 + num2 * 2;
        this.indices[this.indexCount++] = vertexCount + 1 + num1 * 2;
        this.indices[this.indexCount++] = vertexCount + 2 + num1 * 2;
        this.indices[this.indexCount++] = vertexCount + 2 + num2 * 2;
        this.indices[this.indexCount++] = vertexCount + 1 + num1 * 2;
        this.indices[this.indexCount++] = vertexCount + 2 + num2 * 2;
        this.indices[this.indexCount++] = vertexCount + 1 + num2 * 2;
      }
    }

    private void SetOccluder(
      Vector3 center,
      Color mask,
      Vector2 light,
      Vector2 edgeA,
      Vector2 edgeB)
    {
      Vector2 vector1 = Vector2.op_Subtraction(edgeA, light).Floor();
      Vector2 vector2 = Vector2.op_Subtraction(edgeB, light).Floor();
      float num = vector1.Angle();
      float target = vector2.Angle();
      int vertexCount = this.vertexCount;
      this.verts[this.vertexCount].Position = (__Null) Vector3.op_Addition(center, new Vector3(vector1, 0.0f));
      this.verts[this.vertexCount++].Color = (__Null) mask;
      this.verts[this.vertexCount].Position = (__Null) Vector3.op_Addition(center, new Vector3(vector2, 0.0f));
      this.verts[this.vertexCount++].Color = (__Null) mask;
      for (; (double) num != (double) target; num = Calc.AngleApproach(num, target, 0.7853982f))
      {
        this.verts[this.vertexCount].Position = (__Null) Vector3.op_Addition(center, new Vector3(Calc.AngleToVector(num, 128f), 0.0f));
        this.verts[this.vertexCount].Color = (__Null) mask;
        this.indices[this.indexCount++] = vertexCount;
        this.indices[this.indexCount++] = this.vertexCount;
        this.indices[this.indexCount++] = this.vertexCount + 1;
        ++this.vertexCount;
      }
      this.verts[this.vertexCount].Position = (__Null) Vector3.op_Addition(center, new Vector3(Calc.AngleToVector(num, 128f), 0.0f));
      this.verts[this.vertexCount].Color = (__Null) mask;
      this.indices[this.indexCount++] = vertexCount;
      this.indices[this.indexCount++] = this.vertexCount;
      this.indices[this.indexCount++] = vertexCount + 1;
      ++this.vertexCount;
    }

    private void SetCutout(
      Vector3 center,
      Color mask,
      Vector2 light,
      float x,
      float y,
      float width,
      float height)
    {
      this.indices[this.indexCount++] = this.vertexCount;
      this.indices[this.indexCount++] = this.vertexCount + 1;
      this.indices[this.indexCount++] = this.vertexCount + 2;
      this.indices[this.indexCount++] = this.vertexCount;
      this.indices[this.indexCount++] = this.vertexCount + 2;
      this.indices[this.indexCount++] = this.vertexCount + 3;
      this.verts[this.vertexCount].Position = (__Null) Vector3.op_Addition(center, new Vector3(x - (float) light.X, y - (float) light.Y, 0.0f));
      this.verts[this.vertexCount++].Color = (__Null) mask;
      this.verts[this.vertexCount].Position = (__Null) Vector3.op_Addition(center, new Vector3((float) ((double) x + (double) width - light.X), y - (float) light.Y, 0.0f));
      this.verts[this.vertexCount++].Color = (__Null) mask;
      this.verts[this.vertexCount].Position = (__Null) Vector3.op_Addition(center, new Vector3((float) ((double) x + (double) width - light.X), (float) ((double) y + (double) height - light.Y), 0.0f));
      this.verts[this.vertexCount++].Color = (__Null) mask;
      this.verts[this.vertexCount].Position = (__Null) Vector3.op_Addition(center, new Vector3(x - (float) light.X, (float) ((double) y + (double) height - light.Y), 0.0f));
      this.verts[this.vertexCount++].Color = (__Null) mask;
    }

    private void DrawLight(int index, Vector2 position, Color color, float radius)
    {
      Vector3 center = this.GetCenter(index);
      Color mask = this.GetMask(index, 1f, 0.0f);
      this.indices[this.indexCount++] = this.vertexCount;
      this.indices[this.indexCount++] = this.vertexCount + 1;
      this.indices[this.indexCount++] = this.vertexCount + 2;
      this.indices[this.indexCount++] = this.vertexCount;
      this.indices[this.indexCount++] = this.vertexCount + 2;
      this.indices[this.indexCount++] = this.vertexCount + 3;
      this.resultVerts[this.vertexCount].Position = new Vector3(Vector2.op_Addition(position, new Vector2(-radius, -radius)), 0.0f);
      this.resultVerts[this.vertexCount].Color = color;
      this.resultVerts[this.vertexCount].Mask = mask;
      this.resultVerts[this.vertexCount++].Texcoord = Vector2.op_Division(new Vector2((float) center.X - radius, (float) center.Y - radius), 1024f);
      this.resultVerts[this.vertexCount].Position = new Vector3(Vector2.op_Addition(position, new Vector2(radius, -radius)), 0.0f);
      this.resultVerts[this.vertexCount].Color = color;
      this.resultVerts[this.vertexCount].Mask = mask;
      this.resultVerts[this.vertexCount++].Texcoord = Vector2.op_Division(new Vector2((float) center.X + radius, (float) center.Y - radius), 1024f);
      this.resultVerts[this.vertexCount].Position = new Vector3(Vector2.op_Addition(position, new Vector2(radius, radius)), 0.0f);
      this.resultVerts[this.vertexCount].Color = color;
      this.resultVerts[this.vertexCount].Mask = mask;
      this.resultVerts[this.vertexCount++].Texcoord = Vector2.op_Division(new Vector2((float) center.X + radius, (float) center.Y + radius), 1024f);
      this.resultVerts[this.vertexCount].Position = new Vector3(Vector2.op_Addition(position, new Vector2(-radius, radius)), 0.0f);
      this.resultVerts[this.vertexCount].Color = color;
      this.resultVerts[this.vertexCount].Mask = mask;
      this.resultVerts[this.vertexCount++].Texcoord = Vector2.op_Division(new Vector2((float) center.X - radius, (float) center.Y + radius), 1024f);
    }

    public override void Render(Scene scene)
    {
      GFX.FxDither.set_CurrentTechnique(GFX.FxDither.get_Techniques().get_Item("InvertDither"));
      GFX.FxDither.get_Parameters().get_Item("size").SetValue(new Vector2((float) GameplayBuffers.Light.Width, (float) GameplayBuffers.Light.Height));
      Draw.SpriteBatch.Begin((SpriteSortMode) 0, GFX.DestinationTransparencySubtract, (SamplerState) SamplerState.PointClamp, (DepthStencilState) DepthStencilState.None, (RasterizerState) RasterizerState.CullNone, GFX.FxDither, Matrix.get_Identity());
      Draw.SpriteBatch.Draw((Texture2D) (RenderTarget2D) GameplayBuffers.Light, Vector2.get_Zero(), Color.op_Multiply(Color.get_White(), MathHelper.Clamp(this.Alpha, 0.0f, 1f)));
      Draw.SpriteBatch.End();
    }

    static LightingRenderer()
    {
      BlendState blendState1 = new BlendState();
      blendState1.set_AlphaBlendFunction((BlendFunction) 4);
      blendState1.set_ColorBlendFunction((BlendFunction) 4);
      blendState1.set_ColorSourceBlend((Blend) 0);
      blendState1.set_ColorDestinationBlend((Blend) 0);
      blendState1.set_AlphaSourceBlend((Blend) 0);
      blendState1.set_AlphaDestinationBlend((Blend) 0);
      LightingRenderer.GradientBlendState = blendState1;
      BlendState blendState2 = new BlendState();
      blendState2.set_AlphaBlendFunction((BlendFunction) 3);
      blendState2.set_ColorBlendFunction((BlendFunction) 3);
      blendState2.set_ColorSourceBlend((Blend) 0);
      blendState2.set_ColorDestinationBlend((Blend) 0);
      blendState2.set_AlphaSourceBlend((Blend) 0);
      blendState2.set_AlphaDestinationBlend((Blend) 0);
      LightingRenderer.OccludeBlendState = blendState2;
    }

    private struct VertexPositionColorMaskTexture : IVertexType
    {
      public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(new VertexElement[4]
      {
        new VertexElement(0, (VertexElementFormat) 2, (VertexElementUsage) 0, 0),
        new VertexElement(12, (VertexElementFormat) 4, (VertexElementUsage) 1, 0),
        new VertexElement(16, (VertexElementFormat) 4, (VertexElementUsage) 1, 1),
        new VertexElement(20, (VertexElementFormat) 1, (VertexElementUsage) 2, 0)
      });
      public Vector3 Position;
      public Color Color;
      public Color Mask;
      public Vector2 Texcoord;

      VertexDeclaration IVertexType.VertexDeclaration
      {
        get
        {
          return LightingRenderer.VertexPositionColorMaskTexture.VertexDeclaration;
        }
      }
    }
  }
}
