// Decompiled with JetBrains decompiler
// Type: Celeste.SoundSourceEntity
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  public class SoundSourceEntity : Entity
  {
    private string eventName;
    private SoundSource sfx;

    public SoundSourceEntity(EntityData data, Vector2 offset)
      : base(data.Position + offset)
    {
      this.Tag = (int) Tags.TransitionUpdate;
      this.Add((Component) (this.sfx = new SoundSource()));
      this.eventName = Sfxs.EventnameByHandle(data.Attr("sound", ""));
    }

    public override void Awake(Scene scene)
    {
      base.Awake(scene);
      this.sfx.Play(this.eventName, (string) null, 0.0f);
    }
  }
}

