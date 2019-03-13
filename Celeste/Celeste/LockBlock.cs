// Decompiled with JetBrains decompiler
// Type: Celeste.LockBlock
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class LockBlock : Solid
  {
    public EntityID ID;
    private Sprite sprite;
    private bool opening;
    private bool stepMusicProgress;
    private string unlockSfxName;

    public LockBlock(Vector2 position, EntityID id, bool stepMusicProgress, string spriteName)
      : base(position, 32f, 32f, false)
    {
      this.ID = id;
      this.DisableLightsInside = false;
      this.stepMusicProgress = stepMusicProgress;
      this.Add((Component) new PlayerCollider(new Action<Player>(this.OnPlayer), (Collider) new Monocle.Circle(60f, 16f, 16f), (Collider) null));
      this.Add((Component) (this.sprite = GFX.SpriteBank.Create("lockdoor_" + spriteName)));
      this.sprite.Play("idle", false, false);
      this.sprite.Position = new Vector2(this.Width / 2f, this.Height / 2f);
      this.unlockSfxName = "event:/game/03_resort/key_unlock";
      if (spriteName == "temple_a")
      {
        this.unlockSfxName = "event:/game/05_mirror_temple/key_unlock_light";
      }
      else
      {
        if (!(spriteName == "temple_b"))
          return;
        this.unlockSfxName = "event:/game/05_mirror_temple/key_unlock_dark";
      }
    }

    public LockBlock(EntityData data, Vector2 offset, EntityID id)
      : this(Vector2.op_Addition(data.Position, offset), id, data.Bool(nameof (stepMusicProgress), false), data.Attr(nameof (sprite), "wood"))
    {
    }

    private void OnPlayer(Player player)
    {
      if (this.opening)
        return;
      foreach (Follower follower in player.Leader.Followers)
      {
        if (follower.Entity is Key && !(follower.Entity as Key).StartedUsing)
        {
          this.TryOpen(player, follower);
          break;
        }
      }
    }

    private void TryOpen(Player player, Follower fol)
    {
      this.Collidable = false;
      if (!this.Scene.CollideCheck<Solid>(player.Center, this.Center))
      {
        this.opening = true;
        (fol.Entity as Key).StartedUsing = true;
        this.Add((Component) new Coroutine(this.UnlockRoutine(fol), true));
      }
      this.Collidable = true;
    }

    private IEnumerator UnlockRoutine(Follower fol)
    {
      LockBlock lockBlock = this;
      SoundEmitter emitter = SoundEmitter.Play(lockBlock.unlockSfxName, (Entity) lockBlock, new Vector2?());
      emitter.Source.DisposeOnTransition = true;
      Level level = lockBlock.SceneAs<Level>();
      Key key = fol.Entity as Key;
      lockBlock.Add((Component) new Coroutine(key.UseRoutine(Vector2.op_Addition(lockBlock.Center, new Vector2(0.0f, 2f))), true));
      yield return (object) 1.2f;
      if (lockBlock.stepMusicProgress)
      {
        ++level.Session.Audio.Music.Progress;
        level.Session.Audio.Apply();
      }
      level.Session.DoNotLoad.Add(lockBlock.ID);
      key.RegisterUsed();
      while (key.Turning)
        yield return (object) null;
      lockBlock.Tag |= (int) Tags.TransitionUpdate;
      lockBlock.Collidable = false;
      emitter.Source.DisposeOnTransition = false;
      yield return (object) lockBlock.sprite.PlayRoutine("open", false);
      level.Shake(0.3f);
      Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
      yield return (object) lockBlock.sprite.PlayRoutine("burst", false);
      lockBlock.RemoveSelf();
    }
  }
}
