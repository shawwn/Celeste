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
    private static readonly Color bgTilesColor = Color.op_Multiply(Color.get_DarkSlateGray(), 0.5f);
    private static readonly Color[] fgTilesColor = new Color[7]
    {
      Color.get_White(),
      Calc.HexToColor("f6735e"),
      Calc.HexToColor("85f65e"),
      Calc.HexToColor("37d7e3"),
      Calc.HexToColor("376be3"),
      Calc.HexToColor("c337e3"),
      Calc.HexToColor("e33773")
    };
    private static readonly Color inactiveBorderColor = Color.get_DarkSlateGray();
    private static readonly Color selectedBorderColor = Color.get_Red();
    private static readonly Color hoveredBorderColor = Color.get_Yellow();
    private static readonly Color dummyBgTilesColor = Color.op_Multiply(Color.get_DarkSlateGray(), 0.5f);
    private static readonly Color dummyFgTilesColor = Color.get_LightGray();
    private static readonly Color dummyInactiveBorderColor = Color.get_DarkOrange();
    private static readonly Color firstBorderColor = Color.get_Aqua();
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
      this.ActualWidth = (int) data.Bounds.Width;
      this.ActualHeight = (int) data.Bounds.Height;
      this.Width = (int) Math.Ceiling((double) this.ActualWidth / 8.0);
      this.Height = (int) Math.Ceiling((double) this.ActualHeight / 8.0);
      this.Grid = new Grid(8f, 8f, data.Solids);
      this.Back = new Grid(8f, 8f, data.Bg);
      for (int index1 = 0; index1 < this.Height; ++index1)
      {
        for (int index2 = 0; index2 < this.Width; ++index2)
        {
          int num = 0;
          while (index2 + num < this.Width && this.Back[index2 + num, index1] && !this.Grid[index2 + num, index1])
            ++num;
          if (num > 0)
          {
            this.backs.Add(new Rectangle(index2, index1, num, 1));
            index2 += num - 1;
          }
        }
        for (int index2 = 0; index2 < this.Width; ++index2)
        {
          int num = 0;
          while (index2 + num < this.Width && this.Grid[index2 + num, index1])
            ++num;
          if (num > 0)
          {
            this.solids.Add(new Rectangle(index2, index1, num, 1));
            index2 += num - 1;
          }
        }
      }
      this.Spawns = new List<Vector2>();
      using (List<Vector2>.Enumerator enumerator = data.Spawns.GetEnumerator())
      {
        while (enumerator.MoveNext())
          this.Spawns.Add(Vector2.op_Subtraction(Vector2.op_Division(enumerator.Current, 8f), new Vector2((float) this.X, (float) this.Y)));
      }
      this.Strawberries = new List<Vector2>();
      this.StrawberryMetadata = new List<string>();
      this.Checkpoints = new List<Vector2>();
      this.Jumpthrus = new List<Rectangle>();
      foreach (EntityData entity in data.Entities)
      {
        if (entity.Name.Equals("strawberry") || entity.Name.Equals("snowberry"))
        {
          this.Strawberries.Add(Vector2.op_Division(entity.Position, 8f));
          this.StrawberryMetadata.Add(entity.Int("checkpointID", 0).ToString() + ":" + (object) entity.Int("order", 0));
        }
        else if (entity.Name.Equals("checkpoint"))
          this.Checkpoints.Add(Vector2.op_Division(entity.Position, 8f));
        else if (entity.Name.Equals("jumpThru"))
          this.Jumpthrus.Add(new Rectangle((int) (entity.Position.X / 8.0), (int) (entity.Position.Y / 8.0), entity.Width / 8, 1));
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
          if (allLevel != this)
          {
            Rectangle rect = allLevel.Rect;
            if (((Rectangle) ref rect).Intersects(this.Rect))
            {
              flag = true;
              break;
            }
          }
        }
      }
      if (this.Type == LevelTemplateType.Level)
      {
        Draw.Rect((float) this.X, (float) this.Y, (float) this.Width, (float) this.Height, Color.op_Multiply(flag ? Color.get_Red() : Color.get_Black(), 0.5f));
        if (this.Check(Vector2.get_Zero()))
          Draw.HollowRect((float) (this.X + 1), (float) (this.Y + 1), (float) (this.Width - 2), (float) (this.Height - 2), LevelTemplate.firstBorderColor);
        Draw.HollowRect((float) this.X, (float) this.Y, (float) this.Width, (float) this.Height, this.Dummy ? LevelTemplate.dummyInactiveBorderColor : LevelTemplate.inactiveBorderColor);
        using (List<Rectangle>.Enumerator enumerator = this.backs.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            Rectangle current = enumerator.Current;
            Draw.Rect((float) (this.X + current.X), (float) (this.Y + current.Y), (float) current.Width, (float) current.Height, this.Dummy ? LevelTemplate.dummyBgTilesColor : LevelTemplate.bgTilesColor);
          }
        }
        using (List<Rectangle>.Enumerator enumerator = this.solids.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            Rectangle current = enumerator.Current;
            Draw.Rect((float) (this.X + current.X), (float) (this.Y + current.Y), (float) current.Width, (float) current.Height, this.Dummy ? LevelTemplate.dummyFgTilesColor : LevelTemplate.fgTilesColor[this.EditorColorIndex]);
          }
        }
        using (List<Vector2>.Enumerator enumerator = this.Spawns.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            Vector2 current = enumerator.Current;
            Draw.Rect((float) this.X + (float) current.X, (float) ((double) this.Y + current.Y - 1.0), 1f, 1f, Color.get_Red());
          }
        }
        using (List<Vector2>.Enumerator enumerator = this.Strawberries.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            Vector2 current = enumerator.Current;
            Draw.HollowRect((float) ((double) this.X + current.X - 1.0), (float) ((double) this.Y + current.Y - 2.0), 3f, 3f, Color.get_LightPink());
          }
        }
        using (List<Vector2>.Enumerator enumerator = this.Checkpoints.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            Vector2 current = enumerator.Current;
            Draw.HollowRect((float) ((double) this.X + current.X - 1.0), (float) ((double) this.Y + current.Y - 2.0), 3f, 3f, Color.get_Lime());
          }
        }
        using (List<Rectangle>.Enumerator enumerator = this.Jumpthrus.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            Rectangle current = enumerator.Current;
            Draw.Rect((float) (this.X + current.X), (float) (this.Y + current.Y), (float) current.Width, 1f, Color.get_Yellow());
          }
        }
      }
      else
      {
        Draw.Rect((float) this.X, (float) this.Y, (float) this.Width, (float) this.Height, LevelTemplate.dummyFgTilesColor);
        Draw.Rect((float) (this.X + this.Width) - (float) this.resizeHoldSize.X, (float) (this.Y + this.Height) - (float) this.resizeHoldSize.Y, (float) this.resizeHoldSize.X, (float) this.resizeHoldSize.Y, Color.get_Orange());
      }
      if (!(selected | hovered))
        return;
      Draw.HollowRect((float) this.X, (float) this.Y, (float) this.Width, (float) this.Height, hovered ? LevelTemplate.hoveredBorderColor : LevelTemplate.selectedBorderColor);
    }

    public bool Check(Vector2 point)
    {
      if (point.X >= (double) this.Left && point.Y >= (double) this.Top && point.X < (double) this.Right)
        return point.Y < (double) this.Bottom;
      return false;
    }

    public bool Check(Rectangle rect)
    {
      Rectangle rect1 = this.Rect;
      return ((Rectangle) ref rect1).Intersects(rect);
    }

    public void StartMoving()
    {
      this.moveAnchor = new Vector2((float) this.X, (float) this.Y);
    }

    public void Move(Vector2 relativeMove, List<LevelTemplate> allLevels, bool snap)
    {
      this.X = (int) (this.moveAnchor.X + relativeMove.X);
      this.Y = (int) (this.moveAnchor.Y + relativeMove.Y);
      if (!snap)
        return;
      foreach (LevelTemplate allLevel in allLevels)
      {
        if (allLevel != this)
        {
          if (this.Bottom >= allLevel.Top && this.Top <= allLevel.Bottom)
          {
            int num = Math.Abs(this.Left - allLevel.Right) < 3 ? 1 : 0;
            bool flag = Math.Abs(this.Right - allLevel.Left) < 3;
            if (num != 0)
              this.Left = allLevel.Right;
            else if (flag)
              this.Right = allLevel.Left;
            if ((num | (flag ? 1 : 0)) != 0)
            {
              if (Math.Abs(this.Top - allLevel.Top) < 3)
                this.Top = allLevel.Top;
              else if (Math.Abs(this.Bottom - allLevel.Bottom) < 3)
                this.Bottom = allLevel.Bottom;
            }
          }
          if (this.Right >= allLevel.Left && this.Left <= allLevel.Right)
          {
            int num = Math.Abs(this.Top - allLevel.Bottom) < 5 ? 1 : 0;
            bool flag = Math.Abs(this.Bottom - allLevel.Top) < 5;
            if (num != 0)
              this.Top = allLevel.Bottom;
            else if (flag)
              this.Bottom = allLevel.Top;
            if ((num | (flag ? 1 : 0)) != 0)
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
      this.Width = Math.Max(1, (int) (this.resizeAnchor.X + relativeMove.X));
      this.Height = Math.Max(1, (int) (this.resizeAnchor.Y + relativeMove.Y));
      this.ActualWidth = this.Width * 8;
      this.ActualHeight = this.Height * 8;
    }

    public bool ResizePosition(Vector2 mouse)
    {
      if (mouse.X > (double) (this.X + this.Width) - this.resizeHoldSize.X && mouse.Y > (double) (this.Y + this.Height) - this.resizeHoldSize.Y && mouse.X < (double) (this.X + this.Width))
        return mouse.Y < (double) (this.Y + this.Height);
      return false;
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
