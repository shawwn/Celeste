// Decompiled with JetBrains decompiler
// Type: Celeste.DreamHeartGem
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  public class DreamHeartGem : Entity
  {
    private Sprite sprite;

    public DreamHeartGem(EntityData data, Vector2 offset)
      : base(data.Position + offset)
    {
      this.Add((Component) (this.sprite = GFX.SpriteBank.Create("heartgem0")));
      this.sprite.Color = Color.White * 0.25f;
      this.sprite.Play("spin", false, false);
      this.Add((Component) new BloomPoint(0.5f, 16f));
      this.Add((Component) new VertexLight(Color.Aqua, 1f, 32, 64));
    }

    public override void Render()
    {
      for (int y = 0; (double) y < (double) this.sprite.Height; ++y)
        this.sprite.DrawSubrect(new Vector2((float) Math.Sin((double) this.Scene.TimeActive * 2.0 + (double) y * 0.400000005960464) * 2f, (float) y), new Rectangle(0, y, (int) this.sprite.Width, 1));
    }
  }
}

