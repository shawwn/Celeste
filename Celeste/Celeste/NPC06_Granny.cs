// Decompiled with JetBrains decompiler
// Type: Celeste.NPC06_Granny
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  public class NPC06_Granny : NPC
  {
    private int cutsceneIndex = 0;
    public Hahaha Hahaha;

    public NPC06_Granny(EntityData data, Vector2 position)
      : base(data.Position + position)
    {
      this.Add((Component) (this.Sprite = GFX.SpriteBank.Create("granny")));
      this.Sprite.Scale.X = -1f;
      this.Sprite.Play("idle", false, false);
      this.Add((Component) new GrannyLaughSfx(this.Sprite));
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      scene.Add((Entity) (this.Hahaha = new Hahaha(this.Position + new Vector2(8f, -4f), "", false, new Vector2?())));
      this.Hahaha.Enabled = false;
      while (this.Session.GetFlag("granny_" + (object) this.cutsceneIndex))
        ++this.cutsceneIndex;
      this.Add((Component) (this.Talker = new TalkComponent(new Rectangle(-20, -8, 30, 8), new Vector2(0.0f, -24f), new Action<Player>(this.OnTalk), (TalkComponent.HoverDisplay) null)));
      this.Talker.Enabled = this.cutsceneIndex > 0 && this.cutsceneIndex < 3;
    }

    public override void Update()
    {
      if (this.cutsceneIndex == 0)
      {
        Player entity = this.Level.Tracker.GetEntity<Player>();
        if (entity != null && (double) entity.X > (double) this.X - 60.0)
          this.OnTalk(entity);
      }
      this.Hahaha.Enabled = this.Sprite.CurrentAnimationID == "laugh";
      base.Update();
    }

    private void OnTalk(Player player)
    {
      this.Scene.Add((Entity) new CS06_Granny(this, player, this.cutsceneIndex));
      ++this.cutsceneIndex;
      this.Talker.Enabled = this.cutsceneIndex > 0 && this.cutsceneIndex < 3;
    }
  }
}

