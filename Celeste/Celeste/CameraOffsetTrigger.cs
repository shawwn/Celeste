// Decompiled with JetBrains decompiler
// Type: Celeste.CameraOffsetTrigger
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;

namespace Celeste
{
  public class CameraOffsetTrigger : Trigger
  {
    public Vector2 CameraOffset;

    public CameraOffsetTrigger(EntityData data, Vector2 offset)
      : base(data, offset)
    {
      this.CameraOffset = new Vector2(data.Float("cameraX", 0.0f), data.Float("cameraY", 0.0f));
      this.CameraOffset.X *= 48f;
      this.CameraOffset.Y *= 32f;
    }

    public override void OnEnter(Player player)
    {
      this.SceneAs<Level>().CameraOffset = this.CameraOffset;
    }
  }
}

