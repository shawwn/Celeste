// Decompiled with JetBrains decompiler
// Type: Celeste.SolidOnInvinciblePlayer
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  [Tracked(false)]
  public class SolidOnInvinciblePlayer : Component
  {
    private bool wasCollidable;
    private bool wasVisible;
    private SolidOnInvinciblePlayer.Outline outline;

    public SolidOnInvinciblePlayer()
      : base(true, false)
    {
    }

    public override void Added(Entity entity)
    {
      base.Added(entity);
      Audio.Play("event:/game/general/assist_nonsolid_in", entity.Center);
      this.wasCollidable = entity.Collidable;
      this.wasVisible = entity.Visible;
      entity.Collidable = false;
      entity.Visible = false;
      if (entity.Scene == null)
        return;
      entity.Scene.Add((Entity) (this.outline = new SolidOnInvinciblePlayer.Outline(this)));
    }

    public override void Update()
    {
      base.Update();
      this.Entity.Collidable = true;
      if (!this.Entity.CollideCheck<Player>() && !this.Entity.CollideCheck<TheoCrystal>())
        this.RemoveSelf();
      else
        this.Entity.Collidable = false;
    }

    public override void Removed(Entity entity)
    {
      Audio.Play("event:/game/general/assist_nonsolid_out", entity.Center);
      entity.Collidable = this.wasCollidable;
      entity.Visible = this.wasVisible;
      if (this.outline != null)
        this.outline.RemoveSelf();
      base.Removed(entity);
    }

    private class Outline : Entity
    {
      public SolidOnInvinciblePlayer Parent;

      public Outline(SolidOnInvinciblePlayer parent)
      {
        this.Parent = parent;
        this.Depth = -10;
      }

      public override void Render()
      {
        if (this.Parent == null || this.Parent.Entity == null)
          return;
        Entity entity = this.Parent.Entity;
        int left = (int) entity.Left;
        int right = (int) entity.Right;
        int top = (int) entity.Top;
        int bottom = (int) entity.Bottom;
        Draw.Rect((float) (left + 4), (float) (top + 4), entity.Width - 8f, entity.Height - 8f, Color.White * 0.25f);
        for (float x1 = (float) left; (double) x1 < (double) (right - 3); x1 += 3f)
        {
          Draw.Line(x1, (float) top, x1 + 2f, (float) top, Color.White);
          Draw.Line(x1, (float) (bottom - 1), x1 + 2f, (float) (bottom - 1), Color.White);
        }
        for (float y1 = (float) top; (double) y1 < (double) (bottom - 3); y1 += 3f)
        {
          Draw.Line((float) (left + 1), y1, (float) (left + 1), y1 + 2f, Color.White);
          Draw.Line((float) right, y1, (float) right, y1 + 2f, Color.White);
        }
        Draw.Rect((float) (left + 1), (float) top, 1f, 2f, Color.White);
        Draw.Rect((float) (right - 2), (float) top, 2f, 2f, Color.White);
        Draw.Rect((float) left, (float) (bottom - 2), 2f, 2f, Color.White);
        Draw.Rect((float) (right - 2), (float) (bottom - 2), 2f, 2f, Color.White);
      }
    }
  }
}

