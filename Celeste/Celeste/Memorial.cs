// Decompiled with JetBrains decompiler
// Type: Celeste.Memorial
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  public class Memorial : Entity
  {
    private Monocle.Image sprite;
    private MemorialText text;
    private Sprite dreamyText;
    private bool wasShowing;
    private SoundSource loopingSfx;

    public Memorial(Vector2 position)
      : base(position)
    {
      this.Tag = (int) Tags.PauseUpdate;
      this.Add((Component) (this.sprite = new Monocle.Image(GFX.Game["scenery/memorial/memorial"])));
      this.sprite.Origin = new Vector2(this.sprite.Width / 2f, this.sprite.Height);
      this.Depth = 100;
      this.Collider = (Collider) new Hitbox(60f, 80f, -30f, -60f);
      this.Add((Component) (this.loopingSfx = new SoundSource()));
    }

    public Memorial(EntityData data, Vector2 offset)
      : this(data.Position + offset)
    {
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      Level level = scene as Level;
      level.Add((Entity) (this.text = new MemorialText(this, level.Session.Dreaming)));
      if (level.Session.Dreaming)
      {
        this.Add((Component) (this.dreamyText = new Sprite(GFX.Game, "scenery/memorial/floatytext")));
        this.dreamyText.AddLoop("dreamy", "", 0.1f);
        this.dreamyText.Play("dreamy", false, false);
        this.dreamyText.Position = new Vector2((float) (-(double) this.dreamyText.Width / 2.0), -33f);
      }
      if (level.Session.Area.ID != 1 || level.Session.Area.Mode != AreaMode.Normal)
        return;
      Audio.SetMusicParam("end", 1f);
    }

    public override void Update()
    {
      base.Update();
      Level scene = this.Scene as Level;
      if (scene.Paused)
      {
        this.loopingSfx.Pause();
      }
      else
      {
        Player entity = this.Scene.Tracker.GetEntity<Player>();
        bool dreaming = scene.Session.Dreaming;
        this.wasShowing = this.text.Show;
        this.text.Show = entity != null && this.CollideCheck((Entity) entity);
        if (this.text.Show && !this.wasShowing)
        {
          Audio.Play(dreaming ? "event:/ui/game/memorial_dream_text_in" : "event:/ui/game/memorial_text_in");
          if (dreaming)
          {
            this.loopingSfx.Play("event:/ui/game/memorial_dream_loop", (string) null, 0.0f);
            this.loopingSfx.Param("end", 0.0f);
          }
        }
        else if (!this.text.Show && this.wasShowing)
        {
          Audio.Play(dreaming ? "event:/ui/game/memorial_dream_text_out" : "event:/ui/game/memorial_text_out");
          this.loopingSfx.Param("end", 1f);
          this.loopingSfx.Stop(true);
        }
        this.loopingSfx.Resume();
      }
    }
  }
}

