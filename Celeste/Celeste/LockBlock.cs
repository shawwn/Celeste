// Decompiled with JetBrains decompiler
// Type: Celeste.LockBlock
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class LockBlock : Solid
  {
    public static ParticleType P_Appear;
    public EntityID ID;
    public bool UnlockingRegistered;
    private Sprite sprite;
    private bool opening;
    private bool stepMusicProgress;
    private string unlockSfxName;

    public LockBlock(
      Vector2 position,
      EntityID id,
      bool stepMusicProgress,
      string spriteName,
      string unlock_sfx)
      : base(position, 32f, 32f, false)
    {
      this.ID = id;
      this.DisableLightsInside = false;
      this.stepMusicProgress = stepMusicProgress;
      this.Add((Component) new PlayerCollider(new Action<Player>(this.OnPlayer), (Collider) new Monocle.Circle(60f, 16f, 16f)));
      this.Add((Component) (this.sprite = GFX.SpriteBank.Create("lockdoor_" + spriteName)));
      this.sprite.Play("idle");
      this.sprite.Position = new Vector2(this.Width / 2f, this.Height / 2f);
      if (string.IsNullOrWhiteSpace(unlock_sfx))
      {
        this.unlockSfxName = "event:/game/03_resort/key_unlock";
        switch (spriteName)
        {
          case "temple_a":
            this.unlockSfxName = "event:/game/05_mirror_temple/key_unlock_light";
            break;
          case "temple_b":
            this.unlockSfxName = "event:/game/05_mirror_temple/key_unlock_dark";
            break;
        }
      }
      else
        this.unlockSfxName = SFX.EventnameByHandle(unlock_sfx);
    }

    public LockBlock(EntityData data, Vector2 offset, EntityID id)
      : this(data.Position + offset, id, data.Bool(nameof (stepMusicProgress)), data.Attr(nameof (sprite), "wood"), data.Attr("unlock_sfx", (string) null))
    {
    }

    public void Appear()
    {
      this.Visible = true;
      this.sprite.Play("appear");
      this.Add((Component) Alarm.Create(Alarm.AlarmMode.Oneshot, (Action) (() =>
      {
        Level scene = this.Scene as Level;
        if (!this.CollideCheck<Solid>(this.Position - Vector2.UnitX))
        {
          scene.Particles.Emit(LockBlock.P_Appear, 16, this.Position + new Vector2(3f, 16f), new Vector2(2f, 10f), 3.1415927f);
          scene.Particles.Emit(LockBlock.P_Appear, 16, this.Position + new Vector2(29f, 16f), new Vector2(2f, 10f), 0.0f);
        }
        scene.Shake();
      }), 0.25f, true));
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
        this.Add((Component) new Coroutine(this.UnlockRoutine(fol)));
      }
      this.Collidable = true;
    }

    private IEnumerator UnlockRoutine(Follower fol)
    {
      LockBlock follow = this;
      SoundEmitter emitter = SoundEmitter.Play(follow.unlockSfxName, (Entity) follow);
      emitter.Source.DisposeOnTransition = true;
      Level level = follow.SceneAs<Level>();
      Key key = fol.Entity as Key;
      follow.Add((Component) new Coroutine(key.UseRoutine(follow.Center + new Vector2(0.0f, 2f))));
      yield return (object) 1.2f;
      follow.UnlockingRegistered = true;
      if (follow.stepMusicProgress)
      {
        ++level.Session.Audio.Music.Progress;
        level.Session.Audio.Apply();
      }
      level.Session.DoNotLoad.Add(follow.ID);
      key.RegisterUsed();
      while (key.Turning)
        yield return (object) null;
      follow.Tag |= (int) Tags.TransitionUpdate;
      follow.Collidable = false;
      emitter.Source.DisposeOnTransition = false;
      yield return (object) follow.sprite.PlayRoutine("open");
      level.Shake();
      Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
      yield return (object) follow.sprite.PlayRoutine("burst");
      follow.RemoveSelf();
    }
  }
}
