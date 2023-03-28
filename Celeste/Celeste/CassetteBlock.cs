// Decompiled with JetBrains decompiler
// Type: Celeste.CassetteBlock
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;

namespace Celeste
{
  [Tracked(false)]
  public class CassetteBlock : Solid
  {
    public int Index;
    public float Tempo;
    public bool Activated;
    public CassetteBlock.Modes Mode;
    public EntityID ID;
    private int blockHeight = 2;
    private List<CassetteBlock> group;
    private bool groupLeader;
    private Vector2 groupOrigin;
    private Color color;
    private List<Monocle.Image> pressed = new List<Monocle.Image>();
    private List<Monocle.Image> solid = new List<Monocle.Image>();
    private List<Monocle.Image> all = new List<Monocle.Image>();
    private LightOcclude occluder;
    private Wiggler wiggler;
    private Vector2 wigglerScaler;
    private CassetteBlock.BoxSide side;

    public CassetteBlock(
      Vector2 position,
      EntityID id,
      float width,
      float height,
      int index,
      float tempo)
      : base(position, width, height, false)
    {
      this.SurfaceSoundIndex = 35;
      this.Index = index;
      this.Tempo = tempo;
      this.Collidable = false;
      this.ID = id;
      switch (this.Index)
      {
        case 1:
          this.color = Calc.HexToColor("f049be");
          break;
        case 2:
          this.color = Calc.HexToColor("fcdc3a");
          break;
        case 3:
          this.color = Calc.HexToColor("38e04e");
          break;
        default:
          this.color = Calc.HexToColor("49aaf0");
          break;
      }
      this.Add((Component) (this.occluder = new LightOcclude()));
    }

    public CassetteBlock(EntityData data, Vector2 offset, EntityID id)
      : this(data.Position + offset, id, (float) data.Width, (float) data.Height, data.Int("index"), data.Float("tempo", 1f))
    {
    }

    public override void Awake(Scene scene)
    {
      base.Awake(scene);
      Color color1 = Calc.HexToColor("667da5");
      Color color2 = new Color((float) ((double) color1.R / (double) byte.MaxValue * ((double) this.color.R / (double) byte.MaxValue)), (float) ((double) color1.G / (double) byte.MaxValue * ((double) this.color.G / (double) byte.MaxValue)), (float) ((double) color1.B / (double) byte.MaxValue * ((double) this.color.B / (double) byte.MaxValue)), 1f);
      scene.Add((Entity) (this.side = new CassetteBlock.BoxSide(this, color2)));
      foreach (StaticMover staticMover in this.staticMovers)
      {
        if (staticMover.Entity is Spikes entity1)
        {
          entity1.EnabledColor = this.color;
          entity1.DisabledColor = color2;
          entity1.VisibleWhenDisabled = true;
          entity1.SetSpikeColor(this.color);
        }
        if (staticMover.Entity is Spring entity2)
        {
          entity2.DisabledColor = color2;
          entity2.VisibleWhenDisabled = true;
        }
      }
      if (this.group == null)
      {
        this.groupLeader = true;
        this.group = new List<CassetteBlock>();
        this.group.Add(this);
        this.FindInGroup(this);
        float num1 = float.MaxValue;
        float num2 = float.MinValue;
        float num3 = float.MaxValue;
        float y = float.MinValue;
        foreach (CassetteBlock cassetteBlock in this.group)
        {
          if ((double) cassetteBlock.Left < (double) num1)
            num1 = cassetteBlock.Left;
          if ((double) cassetteBlock.Right > (double) num2)
            num2 = cassetteBlock.Right;
          if ((double) cassetteBlock.Bottom > (double) y)
            y = cassetteBlock.Bottom;
          if ((double) cassetteBlock.Top < (double) num3)
            num3 = cassetteBlock.Top;
        }
        this.groupOrigin = new Vector2((float) (int) ((double) num1 + ((double) num2 - (double) num1) / 2.0), (float) (int) y);
        this.wigglerScaler = new Vector2(Calc.ClampedMap(num2 - num1, 32f, 96f, 1f, 0.2f), Calc.ClampedMap(y - num3, 32f, 96f, 1f, 0.2f));
        this.Add((Component) (this.wiggler = Wiggler.Create(0.3f, 3f)));
        foreach (CassetteBlock cassetteBlock in this.group)
        {
          cassetteBlock.wiggler = this.wiggler;
          cassetteBlock.wigglerScaler = this.wigglerScaler;
          cassetteBlock.groupOrigin = this.groupOrigin;
        }
      }
      foreach (Component staticMover in this.staticMovers)
      {
        if (staticMover.Entity is Spikes entity)
          entity.SetOrigins(this.groupOrigin);
      }
      for (float left = this.Left; (double) left < (double) this.Right; left += 8f)
      {
        for (float top = this.Top; (double) top < (double) this.Bottom; top += 8f)
        {
          bool flag1 = this.CheckForSame(left - 8f, top);
          bool flag2 = this.CheckForSame(left + 8f, top);
          bool flag3 = this.CheckForSame(left, top - 8f);
          bool flag4 = this.CheckForSame(left, top + 8f);
          if (flag1 & flag2 & flag3 & flag4)
          {
            if (!this.CheckForSame(left + 8f, top - 8f))
              this.SetImage(left, top, 3, 0);
            else if (!this.CheckForSame(left - 8f, top - 8f))
              this.SetImage(left, top, 3, 1);
            else if (!this.CheckForSame(left + 8f, top + 8f))
              this.SetImage(left, top, 3, 2);
            else if (!this.CheckForSame(left - 8f, top + 8f))
              this.SetImage(left, top, 3, 3);
            else
              this.SetImage(left, top, 1, 1);
          }
          else if (((!(flag1 & flag2) ? 0 : (!flag3 ? 1 : 0)) & (flag4 ? 1 : 0)) != 0)
            this.SetImage(left, top, 1, 0);
          else if (flag1 & flag2 & flag3 && !flag4)
            this.SetImage(left, top, 1, 2);
          else if (((!flag1 ? 0 : (!flag2 ? 1 : 0)) & (flag3 ? 1 : 0) & (flag4 ? 1 : 0)) != 0)
            this.SetImage(left, top, 2, 1);
          else if (!flag1 & flag2 & flag3 & flag4)
            this.SetImage(left, top, 0, 1);
          else if (((!flag1 || flag2 ? 0 : (!flag3 ? 1 : 0)) & (flag4 ? 1 : 0)) != 0)
            this.SetImage(left, top, 2, 0);
          else if (((!(!flag1 & flag2) ? 0 : (!flag3 ? 1 : 0)) & (flag4 ? 1 : 0)) != 0)
            this.SetImage(left, top, 0, 0);
          else if (((!flag1 ? 0 : (!flag2 ? 1 : 0)) & (flag3 ? 1 : 0)) != 0 && !flag4)
            this.SetImage(left, top, 2, 2);
          else if (!flag1 & flag2 & flag3 && !flag4)
            this.SetImage(left, top, 0, 2);
        }
      }
      this.UpdateVisualState();
    }

    private void FindInGroup(CassetteBlock block)
    {
      foreach (CassetteBlock entity in this.Scene.Tracker.GetEntities<CassetteBlock>())
      {
        if (entity != this && entity != block && entity.Index == this.Index && (entity.CollideRect(new Rectangle((int) block.X - 1, (int) block.Y, (int) block.Width + 2, (int) block.Height)) ? 1 : (entity.CollideRect(new Rectangle((int) block.X, (int) block.Y - 1, (int) block.Width, (int) block.Height + 2)) ? 1 : 0)) != 0 && !this.group.Contains(entity))
        {
          this.group.Add(entity);
          this.FindInGroup(entity);
          entity.group = this.group;
        }
      }
    }

    private bool CheckForSame(float x, float y)
    {
      foreach (CassetteBlock entity in this.Scene.Tracker.GetEntities<CassetteBlock>())
      {
        if (entity.Index == this.Index && entity.Collider.Collide(new Rectangle((int) x, (int) y, 8, 8)))
          return true;
      }
      return false;
    }

    private void SetImage(float x, float y, int tx, int ty)
    {
      List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures("objects/cassetteblock/pressed");
      this.pressed.Add(this.CreateImage(x, y, tx, ty, atlasSubtextures[this.Index % atlasSubtextures.Count]));
      this.solid.Add(this.CreateImage(x, y, tx, ty, GFX.Game["objects/cassetteblock/solid"]));
    }

    private Monocle.Image CreateImage(float x, float y, int tx, int ty, MTexture tex)
    {
      Vector2 vector2_1 = new Vector2(x - this.X, y - this.Y);
      Monocle.Image image = new Monocle.Image(tex.GetSubtexture(tx * 8, ty * 8, 8, 8));
      Vector2 vector2_2 = this.groupOrigin - this.Position;
      image.Origin = vector2_2 - vector2_1;
      image.Position = vector2_2;
      image.Color = this.color;
      this.Add((Component) image);
      this.all.Add(image);
      return image;
    }

    public override void Update()
    {
      base.Update();
      if (this.groupLeader && this.Activated && !this.Collidable)
      {
        bool flag = false;
        foreach (CassetteBlock cassetteBlock in this.group)
        {
          if (cassetteBlock.BlockedCheck())
          {
            flag = true;
            break;
          }
        }
        if (!flag)
        {
          foreach (CassetteBlock cassetteBlock in this.group)
          {
            cassetteBlock.Collidable = true;
            cassetteBlock.EnableStaticMovers();
            cassetteBlock.ShiftSize(-1);
          }
          this.wiggler.Start();
        }
      }
      else if (!this.Activated && this.Collidable)
      {
        this.ShiftSize(1);
        this.Collidable = false;
        this.DisableStaticMovers();
      }
      this.UpdateVisualState();
    }

    public bool BlockedCheck()
    {
      TheoCrystal actor1 = this.CollideFirst<TheoCrystal>();
      if (actor1 != null && !this.TryActorWiggleUp((Entity) actor1))
        return true;
      Player actor2 = this.CollideFirst<Player>();
      return actor2 != null && !this.TryActorWiggleUp((Entity) actor2);
    }

    private void UpdateVisualState()
    {
      if (!this.Collidable)
      {
        this.Depth = 8990;
      }
      else
      {
        Player entity = this.Scene.Tracker.GetEntity<Player>();
        if (entity != null && (double) entity.Top >= (double) this.Bottom - 1.0)
          this.Depth = 10;
        else
          this.Depth = -10;
      }
      foreach (Component staticMover in this.staticMovers)
        staticMover.Entity.Depth = this.Depth + 1;
      this.side.Depth = this.Depth + 5;
      this.side.Visible = this.blockHeight > 0;
      this.occluder.Visible = this.Collidable;
      foreach (Component component in this.solid)
        component.Visible = this.Collidable;
      foreach (Component component in this.pressed)
        component.Visible = !this.Collidable;
      if (!this.groupLeader)
        return;
      Vector2 vector2 = new Vector2((float) (1.0 + (double) this.wiggler.Value * 0.05000000074505806 * (double) this.wigglerScaler.X), (float) (1.0 + (double) this.wiggler.Value * 0.15000000596046448 * (double) this.wigglerScaler.Y));
      foreach (CassetteBlock cassetteBlock in this.group)
      {
        foreach (GraphicsComponent graphicsComponent in cassetteBlock.all)
          graphicsComponent.Scale = vector2;
        foreach (Component staticMover in cassetteBlock.staticMovers)
        {
          if (staticMover.Entity is Spikes entity)
          {
            foreach (Component component in entity.Components)
            {
              if (component is Monocle.Image image)
                image.Scale = vector2;
            }
          }
        }
      }
    }

    public void SetActivatedSilently(bool activated)
    {
      this.Activated = this.Collidable = activated;
      this.UpdateVisualState();
      if (activated)
      {
        this.EnableStaticMovers();
      }
      else
      {
        this.ShiftSize(2);
        this.DisableStaticMovers();
      }
    }

    public void Finish() => this.Activated = false;

    public void WillToggle()
    {
      this.ShiftSize(this.Collidable ? 1 : -1);
      this.UpdateVisualState();
    }

    private void ShiftSize(int amount)
    {
      this.MoveV((float) amount);
      this.blockHeight -= amount;
    }

    private bool TryActorWiggleUp(Entity actor)
    {
      foreach (CassetteBlock cassetteBlock in this.group)
      {
        if (cassetteBlock != this && cassetteBlock.CollideCheck(actor, cassetteBlock.Position + Vector2.UnitY * 4f))
          return false;
      }
      bool collidable = this.Collidable;
      this.Collidable = true;
      for (int index = 1; index <= 4; ++index)
      {
        if (!actor.CollideCheck<Solid>(actor.Position - Vector2.UnitY * (float) index))
        {
          actor.Position -= Vector2.UnitY * (float) index;
          this.Collidable = collidable;
          return true;
        }
      }
      this.Collidable = collidable;
      return false;
    }

    public enum Modes
    {
      Solid,
      Leaving,
      Disabled,
      Returning,
    }

    private class BoxSide : Entity
    {
      private CassetteBlock block;
      private Color color;

      public BoxSide(CassetteBlock block, Color color)
      {
        this.block = block;
        this.color = color;
      }

      public override void Render() => Draw.Rect(this.block.X, (float) ((double) this.block.Y + (double) this.block.Height - 8.0), this.block.Width, (float) (8 + this.block.blockHeight), this.color);
    }
  }
}
