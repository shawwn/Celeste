// Decompiled with JetBrains decompiler
// Type: Celeste.CrystalStaticSpinner
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste
{
  [Tracked(false)]
  public class CrystalStaticSpinner : Entity
  {
    private static Dictionary<CrystalColor, string> fgTextureLookup = new Dictionary<CrystalColor, string>()
    {
      {
        CrystalColor.Blue,
        "danger/crystal/fg_blue"
      },
      {
        CrystalColor.Red,
        "danger/crystal/fg_red"
      },
      {
        CrystalColor.Purple,
        "danger/crystal/fg_purple"
      }
    };
    private static Dictionary<CrystalColor, string> bgTextureLookup = new Dictionary<CrystalColor, string>()
    {
      {
        CrystalColor.Blue,
        "danger/crystal/bg_blue"
      },
      {
        CrystalColor.Red,
        "danger/crystal/bg_red"
      },
      {
        CrystalColor.Purple,
        "danger/crystal/bg_purple"
      }
    };
    private float offset = Calc.Random.NextFloat();
    public static ParticleType P_Move;
    public const float ParticleInterval = 0.02f;
    public bool AttachToSolid;
    private Entity filler;
    private CrystalStaticSpinner.Border border;
    private bool expanded;
    private int randomSeed;
    private CrystalColor color;

    public CrystalStaticSpinner(Vector2 position, bool attachToSolid, CrystalColor color)
      : base(position)
    {
      this.color = color;
      this.Tag = (int) Tags.TransitionUpdate;
      this.Collider = (Collider) new ColliderList(new Collider[2]
      {
        (Collider) new Monocle.Circle(6f, 0.0f, 0.0f),
        (Collider) new Hitbox(16f, 4f, -8f, -3f)
      });
      this.Visible = false;
      this.Add((Component) new PlayerCollider(new Action<Player>(this.OnPlayer), (Collider) null, (Collider) null));
      this.Add((Component) new HoldableCollider(new Action<Holdable>(this.OnHoldable), (Collider) null));
      this.Add((Component) new LedgeBlocker((Func<Player, bool>) null));
      this.Depth = -8500;
      this.AttachToSolid = attachToSolid;
      if (attachToSolid)
        this.Add((Component) new StaticMover()
        {
          OnShake = new Action<Vector2>(this.OnShake),
          SolidChecker = new Func<Solid, bool>(this.IsRiding),
          OnDestroy = new Action(((Entity) this).RemoveSelf)
        });
      this.randomSeed = Calc.Random.Next();
    }

    public CrystalStaticSpinner(EntityData data, Vector2 offset, CrystalColor color)
      : this(data.Position + offset, data.Bool("attachToSolid", false), color)
    {
    }

    public override void Awake(Scene scene)
    {
      base.Awake(scene);
      if ((scene as Level).Session.Area.ID == 9)
      {
        this.Add((Component) new CrystalStaticSpinner.CoreModeListener(this));
        this.color = (scene as Level).CoreMode != Session.CoreModes.Cold ? CrystalColor.Red : CrystalColor.Blue;
      }
      if (!this.InView())
        return;
      this.CreateSprites();
    }

    public void ForceInstantiate()
    {
      this.CreateSprites();
      this.Visible = true;
    }

    public override void Update()
    {
      if (!this.Visible)
      {
        this.Collidable = false;
        if (this.InView())
        {
          this.Visible = true;
          if (!this.expanded)
            this.CreateSprites();
        }
      }
      else
      {
        base.Update();
        if (this.Scene.OnInterval(0.25f, this.offset) && !this.InView())
          this.Visible = false;
        if (this.Scene.OnInterval(0.05f, this.offset))
        {
          Player entity = this.Scene.Tracker.GetEntity<Player>();
          if (entity != null)
            this.Collidable = (double) Math.Abs(entity.X - this.X) < 128.0 && (double) Math.Abs(entity.Y - this.Y) < 128.0;
        }
      }
      if (this.filler == null)
        return;
      this.filler.Position = this.Position;
    }

    private bool InView()
    {
      Camera camera = (this.Scene as Level).Camera;
      return (double) this.X > (double) camera.X - 16.0 && (double) this.Y > (double) camera.Y - 16.0 && (double) this.X < (double) camera.X + 320.0 + 16.0 && (double) this.Y < (double) camera.Y + 180.0 + 16.0;
    }

    private void CreateSprites()
    {
      if (this.expanded)
        return;
      Calc.PushRandom(this.randomSeed);
      List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures(CrystalStaticSpinner.fgTextureLookup[this.color]);
      MTexture mtexture = Calc.Random.Choose<MTexture>(atlasSubtextures);
      if (!this.SolidCheck(new Vector2(this.X - 4f, this.Y - 4f)))
        this.Add((Component) new Monocle.Image(mtexture.GetSubtexture(0, 0, 14, 14, (MTexture) null)).SetOrigin(12f, 12f));
      if (!this.SolidCheck(new Vector2(this.X + 4f, this.Y - 4f)))
        this.Add((Component) new Monocle.Image(mtexture.GetSubtexture(10, 0, 14, 14, (MTexture) null)).SetOrigin(2f, 12f));
      if (!this.SolidCheck(new Vector2(this.X + 4f, this.Y + 4f)))
        this.Add((Component) new Monocle.Image(mtexture.GetSubtexture(10, 10, 14, 14, (MTexture) null)).SetOrigin(2f, 2f));
      if (!this.SolidCheck(new Vector2(this.X - 4f, this.Y + 4f)))
        this.Add((Component) new Monocle.Image(mtexture.GetSubtexture(0, 10, 14, 14, (MTexture) null)).SetOrigin(12f, 2f));
      foreach (CrystalStaticSpinner entity in this.Scene.Tracker.GetEntities<CrystalStaticSpinner>())
      {
        if (entity != this && entity.AttachToSolid == this.AttachToSolid && (double) entity.X >= (double) this.X && (double) (entity.Position - this.Position).Length() < 24.0)
          this.AddSprite((this.Position + entity.Position) / 2f - this.Position);
      }
      this.Scene.Add((Entity) (this.border = new CrystalStaticSpinner.Border((Entity) this, this.filler)));
      this.expanded = true;
      Calc.PopRandom();
    }

    private void AddSprite(Vector2 offset)
    {
      if (this.filler == null)
      {
        this.Scene.Add(this.filler = new Entity(this.Position));
        this.filler.Depth = this.Depth + 1;
      }
      List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures(CrystalStaticSpinner.bgTextureLookup[this.color]);
      Monocle.Image image = new Monocle.Image(Calc.Random.Choose<MTexture>(atlasSubtextures));
      image.Position = offset;
      image.Rotation = (float) Calc.Random.Choose<int>(0, 1, 2, 3) * 1.570796f;
      image.CenterOrigin();
      this.filler.Add((Component) image);
    }

    private bool SolidCheck(Vector2 position)
    {
      if (this.AttachToSolid)
        return false;
      foreach (Solid solid in this.Scene.CollideAll<Solid>(position))
      {
        if (solid is SolidTiles)
          return true;
      }
      return false;
    }

    private void ClearSprites()
    {
      if (this.filler != null)
        this.filler.RemoveSelf();
      this.filler = (Entity) null;
      if (this.border != null)
        this.border.RemoveSelf();
      this.border = (CrystalStaticSpinner.Border) null;
      foreach (Component component in this.Components.GetAll<Monocle.Image>())
        component.RemoveSelf();
      this.expanded = false;
    }

    private void OnShake(Vector2 pos)
    {
      foreach (Component component in this.Components)
      {
        if (component is Monocle.Image)
          (component as Monocle.Image).Position = pos;
      }
    }

    private bool IsRiding(Solid solid)
    {
      return this.CollideCheck((Entity) solid);
    }

    private void OnPlayer(Player player)
    {
      player.Die((player.Position - this.Position).SafeNormalize(), false, true);
    }

    private void OnHoldable(Holdable h)
    {
      h.HitSpinner((Entity) this);
    }

    public override void Removed(Scene scene)
    {
      if (this.filler != null && this.filler.Scene == scene)
        this.filler.RemoveSelf();
      if (this.border != null && this.border.Scene == scene)
        this.border.RemoveSelf();
      base.Removed(scene);
    }

    public void Destroy(bool boss = false)
    {
      if (this.InView())
      {
        Audio.Play("event:/game/06_reflection/fall_spike_smash", this.Position);
        Color color = Color.White;
        if (this.color == CrystalColor.Red)
          color = Calc.HexToColor("ff4f4f");
        else if (this.color == CrystalColor.Blue)
          color = Calc.HexToColor("639bff");
        else if (this.color == CrystalColor.Purple)
          color = Calc.HexToColor("ff4fef");
        CrystalDebris.Burst(this.Position, color, boss, 8);
      }
      this.RemoveSelf();
    }

    private class CoreModeListener : Component
    {
      public CrystalStaticSpinner Parent;

      public CoreModeListener(CrystalStaticSpinner parent)
        : base(true, false)
      {
        this.Parent = parent;
      }

      public override void Update()
      {
        Level scene = this.Scene as Level;
        if ((this.Parent.color != CrystalColor.Blue || scene.CoreMode != Session.CoreModes.Hot) && (this.Parent.color != CrystalColor.Red || scene.CoreMode != Session.CoreModes.Cold))
          return;
        this.Parent.color = this.Parent.color != CrystalColor.Blue ? CrystalColor.Blue : CrystalColor.Red;
        this.Parent.ClearSprites();
        this.Parent.CreateSprites();
      }
    }

    private class Border : Entity
    {
      private Entity[] drawing = new Entity[2];

      public Border(Entity parent, Entity filler)
      {
        this.drawing[0] = parent;
        this.drawing[1] = filler;
        this.Depth = parent.Depth + 2;
      }

      public override void Render()
      {
        if (!this.drawing[0].Visible)
          return;
        this.DrawBorder(this.drawing[0]);
        this.DrawBorder(this.drawing[1]);
      }

      private void DrawBorder(Entity entity)
      {
        if (entity == null)
          return;
        foreach (Component component in entity.Components)
        {
          Monocle.Image image = component as Monocle.Image;
          if (image != null)
          {
            Color color = image.Color;
            Vector2 position = image.Position;
            image.Color = Color.Black;
            image.Position = position + new Vector2(0.0f, -1f);
            image.Render();
            image.Position = position + new Vector2(0.0f, 1f);
            image.Render();
            image.Position = position + new Vector2(-1f, 0.0f);
            image.Render();
            image.Position = position + new Vector2(1f, 0.0f);
            image.Render();
            image.Color = color;
            image.Position = position;
          }
        }
      }
    }
  }
}

