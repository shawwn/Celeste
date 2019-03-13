// Decompiled with JetBrains decompiler
// Type: Celeste.LightBeam
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  public class LightBeam : Entity
  {
    private MTexture texture = GFX.Game["util/lightbeam"];
    private Color color = new Color(0.8f, 1f, 1f);
    private float alpha = 0.0f;
    private float timer = Calc.Random.NextFloat(1000f);
    public static ParticleType P_Glow;
    public int LightWidth;
    public int LightLength;
    public float Rotation;
    public string Flag;

    public LightBeam(EntityData data, Vector2 offset)
      : base(data.Position + offset)
    {
      this.Tag = (int) Tags.TransitionUpdate;
      this.Depth = -9998;
      this.LightWidth = data.Width;
      this.LightLength = data.Height;
      this.Flag = data.Attr("flag", "");
      this.Rotation = data.Float("rotation", 0.0f) * ((float) Math.PI / 180f);
    }

    public override void Update()
    {
      this.timer += Engine.DeltaTime;
      Level scene = this.Scene as Level;
      Player entity = this.Scene.Tracker.GetEntity<Player>();
      if (entity != null && (string.IsNullOrEmpty(this.Flag) || scene.Session.GetFlag(this.Flag)))
      {
        Vector2 vector2_1 = Calc.ClosestPointOnLine(this.Position, this.Position + Calc.AngleToVector(this.Rotation + 1.570796f, 1f) * 10000f, entity.Center);
        Vector2 vector2_2 = vector2_1 - this.Position;
        float target = Math.Min(1f, (float) ((double) Math.Max(0.0f, (float) ((double) vector2_2.Length() - 8.0)) / (double) this.LightLength));
        vector2_2 = vector2_1 - entity.Center;
        if ((double) vector2_2.Length() > (double) this.LightWidth / 2.0)
          target = 1f;
        if (scene.Transitioning)
          target = 0.0f;
        this.alpha = Calc.Approach(this.alpha, target, Engine.DeltaTime * 4f);
      }
      if ((double) this.alpha >= 0.5 && scene.OnInterval(0.8f))
      {
        Vector2 vector = Calc.AngleToVector(this.Rotation + 1.570796f, 1f);
        Vector2 position = this.Position - vector * 4f + (float) (Calc.Random.Next(this.LightWidth - 4) + 2 - this.LightWidth / 2) * vector.Perpendicular();
        scene.Particles.Emit(LightBeam.P_Glow, position, this.Rotation + 1.570796f);
      }
      base.Update();
    }

    public override void Render()
    {
      if ((double) this.alpha <= 0.0)
        return;
      this.DrawTexture(0.0f, (float) this.LightWidth, (float) (this.LightLength - 4) + (float) Math.Sin((double) this.timer * 2.0) * 4f, 0.4f);
      for (int index = 0; index < this.LightWidth; index += 4)
      {
        float num = this.timer + (float) index * 0.6f;
        float width = (float) (4.0 + Math.Sin((double) num * 0.5 + 1.20000004768372) * 4.0);
        float offset = (float) Math.Sin(((double) num + (double) (index * 32)) * 0.100000001490116 + Math.Sin((double) num * 0.0500000007450581 + (double) index * 0.100000001490116) * 0.25) * (float) ((double) this.LightWidth / 2.0 - (double) width / 2.0);
        float length = (float) this.LightLength + (float) Math.Sin((double) num * 0.25) * 8f;
        float a = (float) (0.600000023841858 + Math.Sin((double) num + 0.800000011920929) * 0.300000011920929);
        this.DrawTexture(offset, width, length, a);
      }
    }

    private void DrawTexture(float offset, float width, float length, float a)
    {
      float rotation = this.Rotation + 1.570796f;
      if ((double) width < 1.0)
        return;
      this.texture.Draw(this.Position + Calc.AngleToVector(this.Rotation, 1f) * offset, new Vector2(0.0f, 0.5f), this.color * a * this.alpha, new Vector2(1f / (float) this.texture.Width * length, width), rotation);
    }
  }
}

