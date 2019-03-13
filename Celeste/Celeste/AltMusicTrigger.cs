// Decompiled with JetBrains decompiler
// Type: Celeste.AltMusicTrigger
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;

namespace Celeste
{
  public class AltMusicTrigger : Trigger
  {
    public string Track;
    public bool ResetOnLeave;

    public AltMusicTrigger(EntityData data, Vector2 offset)
      : base(data, offset)
    {
      this.Track = data.Attr("track", "");
      this.ResetOnLeave = data.Bool("resetOnLeave", true);
    }

    public override void OnEnter(Player player)
    {
      Audio.SetAltMusic(Sfxs.EventnameByHandle(this.Track));
    }

    public override void OnLeave(Player player)
    {
      if (!this.ResetOnLeave)
        return;
      Audio.SetAltMusic((string) null);
    }
  }
}
