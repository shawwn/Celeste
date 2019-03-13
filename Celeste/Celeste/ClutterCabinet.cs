// Decompiled with JetBrains decompiler
// Type: Celeste.ClutterCabinet
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  [Tracked(false)]
  public class ClutterCabinet : Entity
  {
    private Sprite sprite;

    public bool Opened { get; private set; }

    public ClutterCabinet(Vector2 position)
      : base(position)
    {
      this.Add((Component) (this.sprite = GFX.SpriteBank.Create("clutterCabinet")));
      this.sprite.Position = new Vector2(8f);
      this.sprite.Play("idle", false, false);
      this.Depth = -10001;
    }

    public ClutterCabinet(EntityData data, Vector2 offset)
      : this(data.Position + offset)
    {
    }

    public void Open()
    {
      this.sprite.Play("open", false, false);
      this.Opened = true;
    }

    public void Close()
    {
      this.sprite.Play("close", false, false);
      this.Opened = false;
    }
  }
}

