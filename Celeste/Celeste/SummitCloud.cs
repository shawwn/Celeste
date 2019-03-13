// Decompiled with JetBrains decompiler
// Type: Celeste.SummitCloud
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste
{
  public class SummitCloud : Entity
  {
    private Monocle.Image image;
    private float diff;

    public SummitCloud(EntityData data, Vector2 offset)
      : base(data.Position + offset)
    {
      this.Depth = -10550;
      this.diff = Calc.Random.Range(0.1f, 0.2f);
      List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures("scenery/summitclouds/cloud");
      this.image = new Monocle.Image(Calc.Random.Choose<MTexture>(atlasSubtextures));
      this.image.CenterOrigin();
      this.image.Scale.X = (float) Calc.Random.Choose<int>(-1, 1);
      this.Add((Component) this.image);
      SineWave sineWave = new SineWave(Calc.Random.Range(0.05f, 0.15f));
      sineWave.Randomize();
      sineWave.OnUpdate = (Action<float>) (f => this.image.Y = f * 8f);
      this.Add((Component) sineWave);
    }

    private Vector2 RenderPosition
    {
      get
      {
        return this.Position + (this.Position + new Vector2(128f, 64f) / 2f - ((this.Scene as Level).Camera.Position + new Vector2(160f, 90f))) * (0.1f + this.diff);
      }
    }

    public override void Render()
    {
      Vector2 position = this.Position;
      this.Position = this.RenderPosition;
      base.Render();
      this.Position = position;
    }
  }
}

