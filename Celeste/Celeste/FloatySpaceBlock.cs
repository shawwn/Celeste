// Decompiled with JetBrains decompiler
// Type: Celeste.FloatySpaceBlock
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste
{
  [Tracked(false)]
  public class FloatySpaceBlock : Solid
  {
    private TileGrid tiles;
    private char tileType;
    private float yLerp;
    private float sinkTimer;
    private float sineWave;
    private float dashEase;
    private Vector2 dashDirection;
    private FloatySpaceBlock master;
    private bool awake;
    public List<FloatySpaceBlock> Group;
    public List<JumpThru> Jumpthrus;
    public Dictionary<Platform, Vector2> Moves;
    public Point GroupBoundsMin;
    public Point GroupBoundsMax;

    public bool HasGroup { get; private set; }

    public bool MasterOfGroup { get; private set; }

    public FloatySpaceBlock(
      Vector2 position,
      float width,
      float height,
      char tileType,
      bool disableSpawnOffset)
      : base(position, width, height, true)
    {
      this.tileType = tileType;
      this.Depth = -9000;
      this.Add((Component) new LightOcclude());
      this.SurfaceSoundIndex = SurfaceIndex.TileToIndex[tileType];
      if (!disableSpawnOffset)
        this.sineWave = Calc.Random.NextFloat(6.2831855f);
      else
        this.sineWave = 0.0f;
    }

    public FloatySpaceBlock(EntityData data, Vector2 offset)
      : this(data.Position + offset, (float) data.Width, (float) data.Height, data.Char("tiletype", '3'), data.Bool("disableSpawnOffset"))
    {
    }

    public override void Awake(Scene scene)
    {
      base.Awake(scene);
      this.awake = true;
      if (!this.HasGroup)
      {
        this.MasterOfGroup = true;
        this.Moves = new Dictionary<Platform, Vector2>();
        this.Group = new List<FloatySpaceBlock>();
        this.Jumpthrus = new List<JumpThru>();
        this.GroupBoundsMin = new Point((int) this.X, (int) this.Y);
        this.GroupBoundsMax = new Point((int) this.Right, (int) this.Bottom);
        this.AddToGroupAndFindChildren(this);
        Scene scene1 = this.Scene;
        Rectangle rectangle = new Rectangle(this.GroupBoundsMin.X / 8, this.GroupBoundsMin.Y / 8, (this.GroupBoundsMax.X - this.GroupBoundsMin.X) / 8 + 1, (this.GroupBoundsMax.Y - this.GroupBoundsMin.Y) / 8 + 1);
        VirtualMap<char> mapData = new VirtualMap<char>(rectangle.Width, rectangle.Height, '0');
        foreach (FloatySpaceBlock floatySpaceBlock in this.Group)
        {
          int num1 = (int) ((double) floatySpaceBlock.X / 8.0) - rectangle.X;
          int num2 = (int) ((double) floatySpaceBlock.Y / 8.0) - rectangle.Y;
          int num3 = (int) ((double) floatySpaceBlock.Width / 8.0);
          int num4 = (int) ((double) floatySpaceBlock.Height / 8.0);
          for (int x = num1; x < num1 + num3; ++x)
          {
            for (int y = num2; y < num2 + num4; ++y)
              mapData[x, y] = this.tileType;
          }
        }
        this.tiles = GFX.FGAutotiler.GenerateMap(mapData, new Autotiler.Behaviour()
        {
          EdgesExtend = false,
          EdgesIgnoreOutOfLevel = false,
          PaddingIgnoreOutOfLevel = false
        }).TileGrid;
        this.tiles.Position = new Vector2((float) this.GroupBoundsMin.X - this.X, (float) this.GroupBoundsMin.Y - this.Y);
        this.Add((Component) this.tiles);
      }
      this.TryToInitPosition();
    }

    public override void OnStaticMoverTrigger(StaticMover sm)
    {
      if (!(sm.Entity is Spring))
        return;
      switch ((sm.Entity as Spring).Orientation)
      {
        case Spring.Orientations.Floor:
          this.sinkTimer = 0.5f;
          break;
        case Spring.Orientations.WallLeft:
          this.dashEase = 1f;
          this.dashDirection = -Vector2.UnitX;
          break;
        case Spring.Orientations.WallRight:
          this.dashEase = 1f;
          this.dashDirection = Vector2.UnitX;
          break;
      }
    }

    private void TryToInitPosition()
    {
      if (this.MasterOfGroup)
      {
        foreach (FloatySpaceBlock floatySpaceBlock in this.Group)
        {
          if (!floatySpaceBlock.awake)
            return;
        }
        this.MoveToTarget();
      }
      else
        this.master.TryToInitPosition();
    }

    private void AddToGroupAndFindChildren(FloatySpaceBlock from)
    {
      if ((double) from.X < (double) this.GroupBoundsMin.X)
        this.GroupBoundsMin.X = (int) from.X;
      if ((double) from.Y < (double) this.GroupBoundsMin.Y)
        this.GroupBoundsMin.Y = (int) from.Y;
      if ((double) from.Right > (double) this.GroupBoundsMax.X)
        this.GroupBoundsMax.X = (int) from.Right;
      if ((double) from.Bottom > (double) this.GroupBoundsMax.Y)
        this.GroupBoundsMax.Y = (int) from.Bottom;
      from.HasGroup = true;
      from.OnDashCollide = new DashCollision(this.OnDash);
      this.Group.Add(from);
      this.Moves.Add((Platform) from, from.Position);
      if (from != this)
        from.master = this;
      foreach (JumpThru jp in this.Scene.CollideAll<JumpThru>(new Rectangle((int) from.X - 1, (int) from.Y, (int) from.Width + 2, (int) from.Height)))
      {
        if (!this.Jumpthrus.Contains(jp))
          this.AddJumpThru(jp);
      }
      foreach (JumpThru jp in this.Scene.CollideAll<JumpThru>(new Rectangle((int) from.X, (int) from.Y - 1, (int) from.Width, (int) from.Height + 2)))
      {
        if (!this.Jumpthrus.Contains(jp))
          this.AddJumpThru(jp);
      }
      foreach (FloatySpaceBlock entity in this.Scene.Tracker.GetEntities<FloatySpaceBlock>())
      {
        if (!entity.HasGroup && (int) entity.tileType == (int) this.tileType && (this.Scene.CollideCheck(new Rectangle((int) from.X - 1, (int) from.Y, (int) from.Width + 2, (int) from.Height), (Entity) entity) || this.Scene.CollideCheck(new Rectangle((int) from.X, (int) from.Y - 1, (int) from.Width, (int) from.Height + 2), (Entity) entity)))
          this.AddToGroupAndFindChildren(entity);
      }
    }

    private void AddJumpThru(JumpThru jp)
    {
      jp.OnDashCollide = new DashCollision(this.OnDash);
      this.Jumpthrus.Add(jp);
      this.Moves.Add((Platform) jp, jp.Position);
      foreach (FloatySpaceBlock entity in this.Scene.Tracker.GetEntities<FloatySpaceBlock>())
      {
        if (!entity.HasGroup && (int) entity.tileType == (int) this.tileType && this.Scene.CollideCheck(new Rectangle((int) jp.X - 1, (int) jp.Y, (int) jp.Width + 2, (int) jp.Height), (Entity) entity))
          this.AddToGroupAndFindChildren(entity);
      }
    }

    private DashCollisionResults OnDash(Player player, Vector2 direction)
    {
      if (this.MasterOfGroup && (double) this.dashEase <= 0.20000000298023224)
      {
        this.dashEase = 1f;
        this.dashDirection = direction;
      }
      return DashCollisionResults.NormalOverride;
    }

    public override void Update()
    {
      base.Update();
      if (this.MasterOfGroup)
      {
        bool flag = false;
        foreach (Solid solid in this.Group)
        {
          if (solid.HasPlayerRider())
          {
            flag = true;
            break;
          }
        }
        if (!flag)
        {
          foreach (JumpThru jumpthru in this.Jumpthrus)
          {
            if (jumpthru.HasPlayerRider())
            {
              flag = true;
              break;
            }
          }
        }
        if (flag)
          this.sinkTimer = 0.3f;
        else if ((double) this.sinkTimer > 0.0)
          this.sinkTimer -= Engine.DeltaTime;
        this.yLerp = (double) this.sinkTimer <= 0.0 ? Calc.Approach(this.yLerp, 0.0f, 1f * Engine.DeltaTime) : Calc.Approach(this.yLerp, 1f, 1f * Engine.DeltaTime);
        this.sineWave += Engine.DeltaTime;
        this.dashEase = Calc.Approach(this.dashEase, 0.0f, Engine.DeltaTime * 1.5f);
        this.MoveToTarget();
      }
      this.LiftSpeed = Vector2.Zero;
    }

    private void MoveToTarget()
    {
      float num1 = (float) Math.Sin((double) this.sineWave) * 4f;
      Vector2 vector2_1 = Calc.YoYo(Ease.QuadIn(this.dashEase)) * this.dashDirection * 8f;
      for (int index = 0; index < 2; ++index)
      {
        foreach (KeyValuePair<Platform, Vector2> move in this.Moves)
        {
          Platform key = move.Key;
          bool flag = false;
          JumpThru jumpThru = key as JumpThru;
          Solid solid = key as Solid;
          if (jumpThru != null && jumpThru.HasRider() || solid != null && solid.HasRider())
            flag = true;
          if ((flag || index != 0) && (!flag || index != 1))
          {
            Vector2 vector2_2 = move.Value;
            float num2 = MathHelper.Lerp(vector2_2.Y, vector2_2.Y + 12f, Ease.SineInOut(this.yLerp)) + num1;
            key.MoveToY(num2 + vector2_1.Y);
            key.MoveToX(vector2_2.X + vector2_1.X);
          }
        }
      }
    }

    public override void OnShake(Vector2 amount)
    {
      if (!this.MasterOfGroup)
        return;
      base.OnShake(amount);
      this.tiles.Position += amount;
      foreach (Entity jumpthru in this.Jumpthrus)
      {
        foreach (Component component in jumpthru.Components)
        {
          if (component is Monocle.Image image)
            image.Position = image.Position + amount;
        }
      }
    }
  }
}
