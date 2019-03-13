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
      : base(Vector2.op_Addition(data.Position, offset))
    {
      this.Add((Component) (this.sprite = GFX.SpriteBank.Create("heartgem0")));
      this.sprite.Color = Color.op_Multiply(Color.get_White(), 0.25f);
      this.sprite.Play("spin", false, false);
      this.Add((Component) new BloomPoint(0.5f, 16f));
      this.Add((Component) new VertexLight(Color.get_Aqua(), 1f, 32, 64));
    }

    public override void Render()
    {
      for (int index = 0; (double) index < (double) this.sprite.Height; ++index)
        this.sprite.DrawSubrect(new Vector2((float) Math.Sin((double) this.Scene.TimeActive * 2.0 + (double) index * 0.400000005960464) * 2f, (float) index), new Rectangle(0, index, (int) this.sprite.Width, 1));
    }
  }
}
