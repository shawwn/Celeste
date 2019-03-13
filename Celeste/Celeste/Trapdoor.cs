// Decompiled with JetBrains decompiler
// Type: Celeste.Trapdoor
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class Trapdoor : Entity
  {
    private Sprite sprite;
    private PlayerCollider playerCollider;
    private LightOcclude occluder;

    public Trapdoor(EntityData data, Vector2 offset)
    {
      this.Position = data.Position + offset;
      this.Depth = 8999;
      this.Add((Component) (this.sprite = GFX.SpriteBank.Create("trapdoor")));
      this.sprite.Play("idle", false, false);
      this.sprite.Y = 6f;
      this.Collider = (Collider) new Hitbox(24f, 4f, 0.0f, 6f);
      this.Add((Component) (this.playerCollider = new PlayerCollider(new Action<Player>(this.Open), (Collider) null, (Collider) null)));
      this.Add((Component) (this.occluder = new LightOcclude(new Rectangle(0, 6, 24, 2), 1f)));
    }

    private void Open(Player player)
    {
      this.Collidable = false;
      this.occluder.Visible = false;
      if ((double) player.Speed.Y >= 0.0)
      {
        Audio.Play("event:/game/03_resort/trapdoor_fromtop", this.Position);
        this.sprite.Play("open", false, false);
      }
      else
      {
        Audio.Play("event:/game/03_resort/trapdoor_frombottom", this.Position);
        this.Add((Component) new Coroutine(this.OpenFromBottom(), true));
      }
    }

    private IEnumerator OpenFromBottom()
    {
      this.sprite.Scale.Y = -1f;
      yield return (object) this.sprite.PlayRoutine("open_partial", false);
      yield return (object) 0.1f;
      this.sprite.Rate = -1f;
      yield return (object) this.sprite.PlayRoutine("open_partial", true);
      this.sprite.Scale.Y = 1f;
      this.sprite.Rate = 1f;
      this.sprite.Play("open", true, false);
    }
  }
}

