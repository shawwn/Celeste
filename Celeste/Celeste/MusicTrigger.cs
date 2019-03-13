// Decompiled with JetBrains decompiler
// Type: Celeste.MusicTrigger
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;

namespace Celeste
{
  public class MusicTrigger : Trigger
  {
    public string Track;
    public bool SetInSession;
    public bool ResetOnLeave;
    private string oldTrack;

    public MusicTrigger(EntityData data, Vector2 offset)
      : base(data, offset)
    {
      this.Track = data.Attr("track", "");
      this.ResetOnLeave = data.Bool("resetOnLeave", true);
    }

    public override void OnEnter(Player player)
    {
      if (this.ResetOnLeave)
        this.oldTrack = Audio.CurrentMusic;
      Session session = this.SceneAs<Level>().Session;
      session.Audio.Music.Event = Sfxs.EventnameByHandle(this.Track);
      session.Audio.Apply();
    }

    public override void OnLeave(Player player)
    {
      if (!this.ResetOnLeave)
        return;
      Session session = this.SceneAs<Level>().Session;
      session.Audio.Music.Event = this.oldTrack;
      session.Audio.Apply();
    }
  }
}
