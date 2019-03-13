// Decompiled with JetBrains decompiler
// Type: Celeste.IceTileOverlay
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste
{
  public class IceTileOverlay : Entity
  {
    private float alpha = 0.0f;
    private List<MTexture> surfaces;

    public IceTileOverlay()
    {
      this.Depth = -10010;
      this.Tag = (int) Tags.Global;
      this.Visible = false;
      this.surfaces = GFX.Game.GetAtlasSubtextures("scenery/iceSurface");
    }

    public override void Update()
    {
      base.Update();
      this.alpha = Calc.Approach(this.alpha, (this.Scene as Level).CoreMode == Session.CoreModes.Cold ? 1f : 0.0f, Engine.DeltaTime * 4f);
      this.Visible = (double) this.alpha > 0.0;
    }

    public override void Render()
    {
      Level scene = this.Scene as Level;
      Camera camera = scene.Camera;
      Color color = Color.White * this.alpha;
      int num1 = (int) (Math.Floor(((double) camera.Left - (double) scene.SolidTiles.X) / 8.0) - 1.0);
      int num2 = (int) (Math.Floor(((double) camera.Top - (double) scene.SolidTiles.Y) / 8.0) - 1.0);
      int num3 = (int) (Math.Ceiling(((double) camera.Right - (double) scene.SolidTiles.X) / 8.0) + 1.0);
      int num4 = (int) (Math.Ceiling(((double) camera.Bottom - (double) scene.SolidTiles.Y) / 8.0) + 1.0);
      for (int x = num1; x < num3; ++x)
      {
        for (int y = num2; y < num4; ++y)
        {
          if (scene.SolidsData.SafeCheck(x, y) != '0' && scene.SolidsData.SafeCheck(x, y - 1) == '0')
          {
            Vector2 position = scene.SolidTiles.Position + new Vector2((float) x, (float) y) * 8f;
            this.surfaces[(x * 5 + y * 17) % this.surfaces.Count].Draw(position, Vector2.Zero, color);
          }
        }
      }
    }
  }
}

