// Decompiled with JetBrains decompiler
// Type: Celeste.TileInterceptor
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste
{
  [Tracked(false)]
  public class TileInterceptor : Component
  {
    public Action<MTexture, Vector2, Point> Intercepter;
    public bool HighPriority;

    public TileInterceptor(Action<MTexture, Vector2, Point> intercepter, bool highPriority)
      : base(false, false)
    {
      this.Intercepter = intercepter;
      this.HighPriority = highPriority;
    }

    public TileInterceptor(TileGrid applyToGrid, bool highPriority)
      : base(false, false)
    {
      this.Intercepter = (Action<MTexture, Vector2, Point>) ((t, v, p) => applyToGrid.Tiles[p.X, p.Y] = t);
      this.HighPriority = highPriority;
    }

    public static bool TileCheck(Scene scene, MTexture tile, Vector2 at)
    {
      at += Vector2.One * 4f;
      TileInterceptor tileInterceptor1 = (TileInterceptor) null;
      List<Component> components = scene.Tracker.GetComponents<TileInterceptor>();
      for (int index = components.Count - 1; index >= 0; --index)
      {
        TileInterceptor tileInterceptor2 = (TileInterceptor) components[index];
        if ((tileInterceptor1 == null || tileInterceptor2.HighPriority) && tileInterceptor2.Entity.CollidePoint(at))
        {
          tileInterceptor1 = tileInterceptor2;
          if (tileInterceptor2.HighPriority)
            break;
        }
      }
      if (tileInterceptor1 == null)
        return false;
      Point point = new Point((int) (((double) at.X - (double) tileInterceptor1.Entity.X) / 8.0), (int) (((double) at.Y - (double) tileInterceptor1.Entity.Y) / 8.0));
      tileInterceptor1.Intercepter(tile, at, point);
      return true;
    }
  }
}

