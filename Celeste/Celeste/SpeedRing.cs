// Decompiled with JetBrains decompiler
// Type: Celeste.SpeedRing
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
  [Pooled]
  [Tracked(false)]
  public class SpeedRing : Entity
  {
    private int index;
    private float angle;
    private float lerp;
    private Color color;
    private Vector2 normal;

    public SpeedRing Init(Vector2 position, float angle, Color color)
    {
      this.Position = position;
      this.angle = angle;
      this.color = color;
      this.lerp = 0.0f;
      this.normal = Calc.AngleToVector(angle, 1f);
      return this;
    }

    public override void Update()
    {
      this.lerp += 3f * Engine.DeltaTime;
      this.Position = Vector2.op_Addition(this.Position, Vector2.op_Multiply(Vector2.op_Multiply(this.normal, 10f), Engine.DeltaTime));
      if ((double) this.lerp < 1.0)
        return;
      this.RemoveSelf();
    }

    public override void Render()
    {
      Color color = Color.op_Multiply(this.color, MathHelper.Lerp(0.6f, 0.0f, this.lerp));
      if (((Color) ref color).get_A() <= (byte) 0)
        return;
      Draw.SpriteBatch.Draw((Texture2D) (RenderTarget2D) GameplayBuffers.SpeedRings, Vector2.op_Addition(this.Position, new Vector2(-32f, -32f)), new Rectangle?(new Rectangle(this.index % 4 * 64, this.index / 4 * 64, 64, 64)), color);
    }

    private void DrawRing(Vector2 position)
    {
      float maxRadius = MathHelper.Lerp(4f, 14f, this.lerp);
      Vector2 vector2 = this.GetVectorAtAngle(0.0f, maxRadius);
      for (int index = 1; index <= 8; ++index)
      {
        Vector2 vectorAtAngle = this.GetVectorAtAngle((float) index * 0.3926991f, maxRadius);
        Draw.Line(Vector2.op_Addition(position, vector2), Vector2.op_Addition(position, vectorAtAngle), Color.get_White());
        Draw.Line(Vector2.op_Subtraction(position, vector2), Vector2.op_Subtraction(position, vectorAtAngle), Color.get_White());
        vector2 = vectorAtAngle;
      }
    }

    private Vector2 GetVectorAtAngle(float radians, float maxRadius)
    {
      Vector2 vector = Calc.AngleToVector(radians, 1f);
      float num = MathHelper.Lerp(maxRadius, maxRadius * 0.5f, Math.Abs(Vector2.Dot(vector, this.normal)));
      return Vector2.op_Multiply(vector, num);
    }

    public static void DrawToBuffer(Level level)
    {
      List<Entity> entities = level.Tracker.GetEntities<SpeedRing>();
      int num = 0;
      if (entities.Count <= 0)
        return;
      Engine.Graphics.get_GraphicsDevice().SetRenderTarget((RenderTarget2D) GameplayBuffers.SpeedRings);
      Engine.Graphics.get_GraphicsDevice().Clear(Color.get_Transparent());
      Draw.SpriteBatch.Begin((SpriteSortMode) 0, (BlendState) BlendState.AlphaBlend, (SamplerState) SamplerState.PointClamp, (DepthStencilState) DepthStencilState.None, (RasterizerState) RasterizerState.CullNone);
      foreach (SpeedRing speedRing in entities)
      {
        speedRing.index = num;
        speedRing.DrawRing(new Vector2((float) (num % 4 * 64 + 32), (float) (num / 4 * 64 + 32)));
        ++num;
      }
      Draw.SpriteBatch.End();
    }
  }
}
