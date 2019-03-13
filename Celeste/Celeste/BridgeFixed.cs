// Decompiled with JetBrains decompiler
// Type: Celeste.BridgeFixed
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  public class BridgeFixed : Solid
  {
    public BridgeFixed(EntityData data, Vector2 offset)
      : base(data.Position + offset, (float) data.Width, 8f, true)
    {
      MTexture texture = GFX.Game["scenery/bridge_fixed"];
      for (int index = 0; (double) index < (double) this.Width; index += texture.Width)
      {
        Rectangle rectangle = new Rectangle(0, 0, texture.Width, texture.Height);
        if ((double) (index + rectangle.Width) > (double) this.Width)
          rectangle.Width = (int) this.Width - index;
        Monocle.Image image = new Monocle.Image(texture);
        image.Position = new Vector2((float) index, -8f);
        this.Add((Component) image);
      }
    }
  }
}

