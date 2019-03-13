// Decompiled with JetBrains decompiler
// Type: Celeste.Editor.LevelTemplate
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste.Editor
{
  public class LevelTemplate
  {
    private static readonly Color bgTilesColor = Color.DarkSlateGray * 0.5f;
    private static readonly Color[] fgTilesColor = new Color[7]
    {
      Color.White,
      Calc.HexToColor("f6735e"),
      Calc.HexToColor("85f65e"),
      Calc.HexToColor("37d7e3"),
      Calc.HexToColor("376be3"),
      Calc.HexToColor("c337e3"),
      Calc.HexToColor("e33773")
    };
    private static readonly Color inactiveBorderColor = Color.DarkSlateGray;
    private static readonly Color selectedBorderColor = Color.Red;
    private static readonly Color hoveredBorderColor = Color.Yellow;
    private static readonly Color dummyBgTilesColor = Color.DarkSlateGray * 0.5f;
    private static readonly Color dummyFgTilesColor = Color.LightGray;
    private static readonly Color dummyInactiveBorderColor = Color.DarkOrange;
    private static readonly Color firstBorderColor = Color.Aqua;
    private List<Rectangle> solids = new List<Rectangle>();
    private List<Rectangle> backs = new List<Rectangle>();
    public string Name;
    public LevelTemplateType Type;
    public int X;
    public int Y;
    public int Width;
    public int Height;
    public int ActualWidth;
    public int ActualHeight;
    public Grid Grid;
    public Grid Back;
    public List<Vector2> Spawns;
    public List<Vector2> Strawberries;
    public List<string> StrawberryMetadata;
    public List<Vector2> Checkpoints;
    public List<Rectangle> Jumpthrus;
    public bool Dummy;
    public int EditorColorIndex;
    private Vector2 moveAnchor;
    private Vector2 resizeAnchor;

    private Vector2 resizeHoldSize
    {
      get
      {
        return new Vector2((float) Math.Min(this.Width, 4), (float) Math.Min(this.Height, 4));
      }
    }

    public LevelTemplate(LevelData data)
    {
      this.Name = data.Name;
      this.EditorColorIndex = data.EditorColorIndex;
      this.X = data.Bounds.X / 8;
      this.Y = data.Bounds.Y / 8;
      this.ActualWidth = data.Bounds.Width;
      this.ActualHeight = data.Bounds.Height;
      this.Width = (int) Math.Ceiling((double) this.ActualWidth / 8.0);
      this.Height = (int) Math.Ceiling((double) this.ActualHeight / 8.0);
      this.Grid = new Grid(8f, 8f, data.Solids);
      this.Back = new Grid(8f, 8f, data.Bg);
      for (int y = 0; y < this.Height; ++y)
      {
        for (int x = 0; x < this.Width; ++x)
        {
          int width = 0;
          while (x + width < this.Width && this.Back[x + width, y] && !this.Grid[x + width, y])
            ++width;
          if (width > 0)
          {
            this.backs.Add(new Rectangle(x, y, width, 1));
            x += width - 1;
          }
        }
        for (int x = 0; x < this.Width; ++x)
        {
          int width = 0;
          while (x + width < this.Width && this.Grid[x + width, y])
            ++width;
          if (width > 0)
          {
            this.solids.Add(new Rectangle(x, y, width, 1));
            x += width - 1;
          }
        }
      }
      this.Spawns = new List<Vector2>();
      foreach (Vector2 spawn in data.Spawns)
        this.Spawns.Add(spawn / 8f - new Vector2((float) this.X, (float) this.Y));
      this.Strawberries = new List<Vector2>();
      this.StrawberryMetadata = new List<string>();
      this.Checkpoints = new List<Vector2>();
      this.Jumpthrus = new List<Rectangle>();
      foreach (EntityData entity in data.Entities)
      {
        if (entity.Name.Equals("strawberry") || entity.Name.Equals("snowberry"))
        {
          this.Strawberries.Add(entity.Position / 8f);
          this.StrawberryMetadata.Add(entity.Int("checkpointID", 0).ToString() + ":" + (object) entity.Int("order", 0));
        }
        else if (entity.Name.Equals("checkpoint"))
          this.Checkpoints.Add(entity.Position / 8f);
        else if (entity.Name.Equals("jumpThru"))
          this.Jumpthrus.Add(new Rectangle((int) ((double) entity.Position.X / 8.0), (int) ((double) entity.Position.Y / 8.0), entity.Width / 8, 1));
      }
      this.Dummy = data.Dummy;
      this.Type = LevelTemplateType.Level;
    }

    public LevelTemplate(int x, int y, int w, int h)
    {
      this.Name = "FILLER";
      this.X = x;
      this.Y = y;
      this.Width = w;
      this.Height = h;
      this.ActualWidth = w * 8;
      this.ActualHeight = h * 8;
      this.Type = LevelTemplateType.Filler;
    }

    public void Render(bool selected, bool hovered, List<LevelTemplate> allLevels)
    {
      bool flag = false;
      if (Engine.Scene.BetweenInterval(0.1f))
      {
        foreach (LevelTemplate allLevel in allLevels)
        {
          if (allLevel != this && allLevel.Rect.Intersects(this.Rect))
          {
            flag = true;
            break;
          }
        }
      }
      if (this.Type == LevelTemplateType.Level)
      {
        Draw.Rect((float) this.X, (float) this.Y, (float) this.Width, (float) this.Height, (flag ? Color.Red : Color.Black) * 0.5f);
        if (this.Check(Vector2.Zero))
          Draw.HollowRect((float) (this.X + 1), (float) (this.Y + 1), (float) (this.Width - 2), (float) (this.Height - 2), LevelTemplate.firstBorderColor);
        Draw.HollowRect((float) this.X, (float) this.Y, (float) this.Width, (float) this.Height, this.Dummy ? LevelTemplate.dummyInactiveBorderColor : LevelTemplate.inactiveBorderColor);
        foreach (Rectangle back in this.backs)
          Draw.Rect((float) (this.X + back.X), (float) (this.Y + back.Y), (float) back.Width, (float) back.Height, this.Dummy ? LevelTemplate.dummyBgTilesColor : LevelTemplate.bgTilesColor);
        foreach (Rectangle solid in this.solids)
          Draw.Rect((float) (this.X + solid.X), (float) (this.Y + solid.Y), (float) solid.Width, (float) solid.Height, this.Dummy ? LevelTemplate.dummyFgTilesColor : LevelTemplate.fgTilesColor[this.EditorColorIndex]);
        foreach (Vector2 spawn in this.Spawns)
          Draw.Rect((float) this.X + spawn.X, (float) ((double) this.Y + (double) spawn.Y - 1.0), 1f, 1f, Color.Red);
        foreach (Vector2 strawberry in this.Strawberries)
          Draw.HollowRect((float) ((double) this.X + (double) strawberry.X - 1.0), (float) ((double) this.Y + (double) strawberry.Y - 2.0), 3f, 3f, Color.LightPink);
        foreach (Vector2 checkpoint in this.Checkpoints)
          Draw.HollowRect((float) ((double) this.X + (double) checkpoint.X - 1.0), (float) ((double) this.Y + (double) checkpoint.Y - 2.0), 3f, 3f, Color.Lime);
        foreach (Rectangle jumpthru in this.Jumpthrus)
          Draw.Rect((float) (this.X + jumpthru.X), (float) (this.Y + jumpthru.Y), (float) jumpthru.Width, 1f, Color.Yellow);
      }
      else
      {
        Draw.Rect((float) this.X, (float) this.Y, (float) this.Width, (float) this.Height, LevelTemplate.dummyFgTilesColor);
        Draw.Rect((float) (this.X + this.Width) - this.resizeHoldSize.X, (float) (this.Y + this.Height) - this.resizeHoldSize.Y, this.resizeHoldSize.X, this.resizeHoldSize.Y, Color.Orange);
      }
      if (!(selected | hovered))
        return;
      Draw.HollowRect((float) this.X, (float) this.Y, (float) this.Width, (float) this.Height, hovered ? LevelTemplate.hoveredBorderColor : LevelTemplate.selectedBorderColor);
    }

    public bool Check(Vector2 point)
    {
      return (double) point.X >= (double) this.Left && (double) point.Y >= (double) this.Top && (double) point.X < (double) this.Right && (double) point.Y < (double) this.Bottom;
    }

    public bool Check(Rectangle rect)
    {
      return this.Rect.Intersects(rect);
    }

    public void StartMoving()
    {
      this.moveAnchor = new Vector2((float) this.X, (float) this.Y);
    }

    public void Move(Vector2 relativeMove, List<LevelTemplate> allLevels, bool snap)
    {
      this.X = (int) ((double) this.moveAnchor.X + (double) relativeMove.X);
      this.Y = (int) ((double) this.moveAnchor.Y + (double) relativeMove.Y);
      if (!snap)
        return;
      foreach (LevelTemplate allLevel in allLevels)
      {
        if (allLevel != this)
        {
          if (this.Bottom >= allLevel.Top && this.Top <= allLevel.Bottom)
          {
            bool flag1 = Math.Abs(this.Left - allLevel.Right) < 3;
            bool flag2 = Math.Abs(this.Right - allLevel.Left) < 3;
            if (flag1)
              this.Left = allLevel.Right;
            else if (flag2)
              this.Right = allLevel.Left;
            if (flag1 | flag2)
            {
              if (Math.Abs(this.Top - allLevel.Top) < 3)
                this.Top = allLevel.Top;
              else if (Math.Abs(this.Bottom - allLevel.Bottom) < 3)
                this.Bottom = allLevel.Bottom;
            }
          }
          if (this.Right >= allLevel.Left && this.Left <= allLevel.Right)
          {
            bool flag1 = Math.Abs(this.Top - allLevel.Bottom) < 5;
            bool flag2 = Math.Abs(this.Bottom - allLevel.Top) < 5;
            if (flag1)
              this.Top = allLevel.Bottom;
            else if (flag2)
              this.Bottom = allLevel.Top;
            if (flag1 | flag2)
            {
              if (Math.Abs(this.Left - allLevel.Left) < 3)
                this.Left = allLevel.Left;
              else if (Math.Abs(this.Right - allLevel.Right) < 3)
                this.Right = allLevel.Right;
            }
          }
        }
      }
    }

    public void StartResizing()
    {
      this.resizeAnchor = new Vector2((float) this.Width, (float) this.Height);
    }

    public void Resize(Vector2 relativeMove)
    {
      this.Width = Math.Max(1, (int) ((double) this.resizeAnchor.X + (double) relativeMove.X));
      this.Height = Math.Max(1, (int) ((double) this.resizeAnchor.Y + (double) relativeMove.Y));
      this.ActualWidth = this.Width * 8;
      this.ActualHeight = this.Height * 8;
    }

    public bool ResizePosition(Vector2 mouse)
    {
      return (double) mouse.X > (double) (this.X + this.Width) - (double) this.resizeHoldSize.X && (double) mouse.Y > (double) (this.Y + this.Height) - (double) this.resizeHoldSize.Y && (double) mouse.X < (double) (this.X + this.Width) && (double) mouse.Y < (double) (this.Y + this.Height);
    }

    public Rectangle Rect
    {
      get
      {
        return new Rectangle(this.X, this.Y, this.Width, this.Height);
      }
    }

    public int Left
    {
      get
      {
        return this.X;
      }
      set
      {
        this.X = value;
      }
    }

    public int Top
    {
      get
      {
        return this.Y;
      }
      set
      {
        this.Y = value;
      }
    }

    public int Right
    {
      get
      {
        return this.X + this.Width;
      }
      set
      {
        this.X = value - this.Width;
      }
    }

    public int Bottom
    {
      get
      {
        return this.Y + this.Height;
      }
      set
      {
        this.Y = value - this.Height;
      }
    }
  }
}

