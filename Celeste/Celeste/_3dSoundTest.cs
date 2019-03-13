// Decompiled with JetBrains decompiler
// Type: Celeste._3dSoundTest
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  public class _3dSoundTest : Entity
  {
    public SoundSource sfx;

    public _3dSoundTest(EntityData data, Vector2 offset)
      : base(data.Position + offset)
    {
      this.Add((Component) (this.sfx = new SoundSource()));
      this.sfx.Play("event:/3d_testing", (string) null, 0.0f);
    }

    public override void Render()
    {
      Draw.Rect(this.X - 8f, this.Y - 8f, 16f, 16f, Color.Yellow);
      Camera camera = (this.Scene as Level).Camera;
      Draw.HollowRect(this.X - 320f, camera.Y, 640f, 180f, Color.Red);
      Draw.HollowRect(this.X - 160f, camera.Y, 320f, 180f, Color.Yellow);
      Draw.HollowRect((float) ((double) this.X - 160.0 - 320.0), camera.Y, 960f, 180f, Color.Yellow);
    }
  }
}

