// Decompiled with JetBrains decompiler
// Type: Celeste.ForegroundDebris
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste
{
  public class ForegroundDebris : Entity
  {
    private float parallax = 0.0f;
    private Vector2 start;

    public ForegroundDebris(Vector2 position)
      : base(position)
    {
      this.start = this.Position;
      this.Depth = -999900;
      string key = "scenery/fgdebris/" + Calc.Random.Choose<string>("rock_a", "rock_b");
      List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures(key);
      atlasSubtextures.Reverse();
      foreach (MTexture texture in atlasSubtextures)
      {
        Monocle.Image img = new Monocle.Image(texture);
        img.CenterOrigin();
        this.Add((Component) img);
        SineWave sine = new SineWave(0.4f);
        sine.Randomize();
        sine.OnUpdate = (Action<float>) (f => img.Y = sine.Value * 2f);
        this.Add((Component) sine);
      }
      this.parallax = 0.05f + Calc.Random.NextFloat(0.08f);
    }

    public ForegroundDebris(EntityData data, Vector2 offset)
      : this(data.Position + offset)
    {
    }

    public override void Render()
    {
      Vector2 vector2 = this.SceneAs<Level>().Camera.Position + new Vector2(320f, 180f) / 2f - this.start;
      Vector2 position = this.Position;
      this.Position = this.Position - vector2 * this.parallax;
      base.Render();
      this.Position = position;
    }
  }
}

