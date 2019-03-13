// Decompiled with JetBrains decompiler
// Type: Celeste.Lightning
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  public class Lightning : Entity
  {
    public const string Flag = "disable_lightning";

    public Lightning(Vector2 position, int width, int height)
      : base(position)
    {
      this.Collider = (Collider) new Hitbox((float) width, (float) height, 0.0f, 0.0f);
      this.Add((Component) new PlayerCollider(new Action<Player>(this.OnPlayer), (Collider) null, (Collider) null));
    }

    public Lightning(EntityData data, Vector2 levelOffset)
      : this(Vector2.op_Addition(data.Position, levelOffset), data.Width, data.Height)
    {
    }

    public override void Render()
    {
      base.Render();
      Draw.Rect(this.Collider, Color.get_Yellow());
    }

    private void OnPlayer(Player player)
    {
      if (SaveData.Instance.Assists.Invincible)
        player.ReflectBounce(Vector2.op_Multiply(Vector2.get_UnitX(), (float) Math.Sign(player.X - this.X)));
      else
        player.Die(Vector2.op_Multiply(Vector2.get_UnitX(), (float) Math.Sign(player.X - this.X)), false, true);
    }
  }
}
