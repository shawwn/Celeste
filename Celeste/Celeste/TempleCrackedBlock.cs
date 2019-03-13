// Decompiled with JetBrains decompiler
// Type: Celeste.TempleCrackedBlock
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;

namespace Celeste
{
  [Tracked(false)]
  public class TempleCrackedBlock : Solid
  {
    private float frame = 0.0f;
    private EntityID eid;
    private bool persistent;
    private MTexture[,,] tiles;
    private bool broken;
    private int frames;

    public TempleCrackedBlock(
      EntityID eid,
      Vector2 position,
      float width,
      float height,
      bool persistent)
      : base(position, width, height, true)
    {
      this.eid = eid;
      this.persistent = persistent;
      this.Collidable = this.Visible = false;
      int length1 = (int) ((double) width / 8.0);
      int length2 = (int) ((double) height / 8.0);
      List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures("objects/temple/breakBlock");
      this.tiles = new MTexture[length1, length2, atlasSubtextures.Count];
      this.frames = atlasSubtextures.Count;
      for (int index1 = 0; index1 < length1; ++index1)
      {
        for (int index2 = 0; index2 < length2; ++index2)
        {
          int num1 = index1 >= length1 / 2 || index1 >= 2 ? (index1 < length1 / 2 || index1 < length1 - 2 ? 2 + index1 % 2 : 5 - (length1 - index1 - 1)) : index1;
          int num2 = index2 >= length2 / 2 || index2 >= 2 ? (index2 < length2 / 2 || index2 < length2 - 2 ? 2 + index2 % 2 : 5 - (length2 - index2 - 1)) : index2;
          for (int index3 = 0; index3 < atlasSubtextures.Count; ++index3)
            this.tiles[index1, index2, index3] = atlasSubtextures[index3].GetSubtexture(num1 * 8, num2 * 8, 8, 8, (MTexture) null);
        }
      }
      this.Add((Component) new LightOcclude(0.5f));
    }

    public TempleCrackedBlock(EntityID eid, EntityData data, Vector2 offset)
      : this(eid, data.Position + offset, (float) data.Width, (float) data.Height, data.Bool(nameof (persistent), false))
    {
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      if (this.CollideCheck<Player>())
      {
        if (this.persistent)
          this.SceneAs<Level>().Session.DoNotLoad.Add(this.eid);
        this.RemoveSelf();
      }
      else
        this.Collidable = this.Visible = true;
    }

    public override void Update()
    {
      base.Update();
      if (!this.broken)
        return;
      this.frame += Engine.DeltaTime * 15f;
      if ((double) this.frame >= (double) this.frames)
        this.RemoveSelf();
    }

    public override void Render()
    {
      int frame = (int) this.frame;
      if (frame >= this.frames)
        return;
      for (int index1 = 0; (double) index1 < (double) this.Width / 8.0; ++index1)
      {
        for (int index2 = 0; (double) index2 < (double) this.Height / 8.0; ++index2)
          this.tiles[index1, index2, frame].Draw(this.Position + new Vector2((float) index1, (float) index2) * 8f);
      }
    }

    public void Break(Vector2 from)
    {
      if (this.persistent)
        this.SceneAs<Level>().Session.DoNotLoad.Add(this.eid);
      Audio.Play("event:/game/05_mirror_temple/crackedwall_vanish", this.Center);
      this.broken = true;
      this.Collidable = false;
      for (int index1 = 0; (double) index1 < (double) this.Width / 8.0; ++index1)
      {
        for (int index2 = 0; (double) index2 < (double) this.Height / 8.0; ++index2)
          this.Scene.Add((Entity) Engine.Pooler.Create<Debris>().Init(this.Position + new Vector2((float) (index1 * 8 + 4), (float) (index2 * 8 + 4)), '1').BlastFrom(from));
      }
    }
  }
}

