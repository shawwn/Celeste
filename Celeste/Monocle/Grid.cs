// Decompiled with JetBrains decompiler
// Type: Monocle.Grid
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using System;

namespace Monocle
{
  public class Grid : Collider
  {
    public VirtualMap<bool> Data;

    public float CellWidth { get; private set; }

    public float CellHeight { get; private set; }

    public Grid(int cellsX, int cellsY, float cellWidth, float cellHeight)
    {
      this.Data = new VirtualMap<bool>(cellsX, cellsY, false);
      this.CellWidth = cellWidth;
      this.CellHeight = cellHeight;
    }

    public Grid(float cellWidth, float cellHeight, string bitstring)
    {
      this.CellWidth = cellWidth;
      this.CellHeight = cellHeight;
      int num = 0;
      int val1 = 0;
      int rows = 1;
      for (int index = 0; index < bitstring.Length; ++index)
      {
        if (bitstring[index] == '\n')
        {
          ++rows;
          num = Math.Max(val1, num);
          val1 = 0;
        }
        else
          ++val1;
      }
      this.Data = new VirtualMap<bool>(num, rows, false);
      this.LoadBitstring(bitstring);
    }

    public Grid(float cellWidth, float cellHeight, bool[,] data)
    {
      this.CellWidth = cellWidth;
      this.CellHeight = cellHeight;
      this.Data = new VirtualMap<bool>(data, false);
    }

    public Grid(float cellWidth, float cellHeight, VirtualMap<bool> data)
    {
      this.CellWidth = cellWidth;
      this.CellHeight = cellHeight;
      this.Data = data;
    }

    public void Extend(int left, int right, int up, int down)
    {
      this.Position = this.Position - new Vector2((float) left * this.CellWidth, (float) up * this.CellHeight);
      int columns = this.Data.Columns + left + right;
      int rows = this.Data.Rows + up + down;
      if (columns <= 0 || rows <= 0)
      {
        this.Data = new VirtualMap<bool>(0, 0, false);
      }
      else
      {
        VirtualMap<bool> virtualMap = new VirtualMap<bool>(columns, rows, false);
        for (int index1 = 0; index1 < this.Data.Columns; ++index1)
        {
          for (int index2 = 0; index2 < this.Data.Rows; ++index2)
          {
            int index3 = index1 + left;
            int index4 = index2 + up;
            if (index3 >= 0 && index3 < columns && index4 >= 0 && index4 < rows)
              virtualMap[index3, index4] = this.Data[index1, index2];
          }
        }
        for (int index1 = 0; index1 < left; ++index1)
        {
          for (int index2 = 0; index2 < rows; ++index2)
            virtualMap[index1, index2] = this.Data[0, Calc.Clamp(index2 - up, 0, this.Data.Rows - 1)];
        }
        for (int index1 = columns - right; index1 < columns; ++index1)
        {
          for (int index2 = 0; index2 < rows; ++index2)
            virtualMap[index1, index2] = this.Data[this.Data.Columns - 1, Calc.Clamp(index2 - up, 0, this.Data.Rows - 1)];
        }
        for (int index1 = 0; index1 < up; ++index1)
        {
          for (int index2 = 0; index2 < columns; ++index2)
            virtualMap[index2, index1] = this.Data[Calc.Clamp(index2 - left, 0, this.Data.Columns - 1), 0];
        }
        for (int index1 = rows - down; index1 < rows; ++index1)
        {
          for (int index2 = 0; index2 < columns; ++index2)
            virtualMap[index2, index1] = this.Data[Calc.Clamp(index2 - left, 0, this.Data.Columns - 1), this.Data.Rows - 1];
        }
        this.Data = virtualMap;
      }
    }

    public void LoadBitstring(string bitstring)
    {
      int index1 = 0;
      int index2 = 0;
      for (int index3 = 0; index3 < bitstring.Length; ++index3)
      {
        if (bitstring[index3] == '\n')
        {
          for (; index1 < this.CellsX; ++index1)
            this.Data[index1, index2] = false;
          index1 = 0;
          ++index2;
          if (index2 >= this.CellsY)
            break;
        }
        else if (index1 < this.CellsX)
        {
          if (bitstring[index3] == '0')
          {
            this.Data[index1, index2] = false;
            ++index1;
          }
          else
          {
            this.Data[index1, index2] = true;
            ++index1;
          }
        }
      }
    }

    public string GetBitstring()
    {
      string str = "";
      for (int index1 = 0; index1 < this.CellsY; ++index1)
      {
        if ((uint) index1 > 0U)
          str += "\n";
        for (int index2 = 0; index2 < this.CellsX; ++index2)
          str = !this.Data[index2, index1] ? str + "0" : str + "1";
      }
      return str;
    }

    public void Clear(bool to = false)
    {
      for (int index1 = 0; index1 < this.CellsX; ++index1)
      {
        for (int index2 = 0; index2 < this.CellsY; ++index2)
          this.Data[index1, index2] = to;
      }
    }

    public void SetRect(int x, int y, int width, int height, bool to = true)
    {
      if (x < 0)
      {
        width += x;
        x = 0;
      }
      if (y < 0)
      {
        height += y;
        y = 0;
      }
      if (x + width > this.CellsX)
        width = this.CellsX - x;
      if (y + height > this.CellsY)
        height = this.CellsY - y;
      for (int index1 = 0; index1 < width; ++index1)
      {
        for (int index2 = 0; index2 < height; ++index2)
          this.Data[x + index1, y + index2] = to;
      }
    }

    public bool CheckRect(int x, int y, int width, int height)
    {
      if (x < 0)
      {
        width += x;
        x = 0;
      }
      if (y < 0)
      {
        height += y;
        y = 0;
      }
      if (x + width > this.CellsX)
        width = this.CellsX - x;
      if (y + height > this.CellsY)
        height = this.CellsY - y;
      for (int index1 = 0; index1 < width; ++index1)
      {
        for (int index2 = 0; index2 < height; ++index2)
        {
          if (this.Data[x + index1, y + index2])
            return true;
        }
      }
      return false;
    }

    public bool CheckColumn(int x)
    {
      for (int index = 0; index < this.CellsY; ++index)
      {
        if (!this.Data[x, index])
          return false;
      }
      return true;
    }

    public bool CheckRow(int y)
    {
      for (int index = 0; index < this.CellsX; ++index)
      {
        if (!this.Data[index, y])
          return false;
      }
      return true;
    }

    public bool this[int x, int y]
    {
      get
      {
        if (x >= 0 && y >= 0 && x < this.CellsX && y < this.CellsY)
          return this.Data[x, y];
        return false;
      }
      set
      {
        this.Data[x, y] = value;
      }
    }

    public int CellsX
    {
      get
      {
        return this.Data.Columns;
      }
    }

    public int CellsY
    {
      get
      {
        return this.Data.Rows;
      }
    }

    public override float Width
    {
      get
      {
        return this.CellWidth * (float) this.CellsX;
      }
      set
      {
        throw new NotImplementedException();
      }
    }

    public override float Height
    {
      get
      {
        return this.CellHeight * (float) this.CellsY;
      }
      set
      {
        throw new NotImplementedException();
      }
    }

    public bool IsEmpty
    {
      get
      {
        for (int index1 = 0; index1 < this.CellsX; ++index1)
        {
          for (int index2 = 0; index2 < this.CellsY; ++index2)
          {
            if (this.Data[index1, index2])
              return false;
          }
        }
        return true;
      }
    }

    public override float Left
    {
      get
      {
        return this.Position.X;
      }
      set
      {
        this.Position.X = value;
      }
    }

    public override float Top
    {
      get
      {
        return this.Position.Y;
      }
      set
      {
        this.Position.Y = value;
      }
    }

    public override float Right
    {
      get
      {
        return this.Position.X + this.Width;
      }
      set
      {
        this.Position.X = value - this.Width;
      }
    }

    public override float Bottom
    {
      get
      {
        return this.Position.Y + this.Height;
      }
      set
      {
        this.Position.Y = value - this.Height;
      }
    }

    public override Collider Clone()
    {
      return (Collider) new Grid(this.CellWidth, this.CellHeight, this.Data.Clone());
    }

    public override void Render(Camera camera, Color color)
    {
      if (camera == null)
      {
        for (int index1 = 0; index1 < this.CellsX; ++index1)
        {
          for (int index2 = 0; index2 < this.CellsY; ++index2)
          {
            if (this.Data[index1, index2])
              Draw.HollowRect(this.AbsoluteLeft + (float) index1 * this.CellWidth, this.AbsoluteTop + (float) index2 * this.CellHeight, this.CellWidth, this.CellHeight, color);
          }
        }
      }
      else
      {
        int num1 = (int) Math.Max(0.0f, (camera.Left - this.AbsoluteLeft) / this.CellWidth);
        int num2 = (int) Math.Min((double) (this.CellsX - 1), Math.Ceiling(((double) camera.Right - (double) this.AbsoluteLeft) / (double) this.CellWidth));
        int num3 = (int) Math.Max(0.0f, (camera.Top - this.AbsoluteTop) / this.CellHeight);
        int num4 = (int) Math.Min((double) (this.CellsY - 1), Math.Ceiling(((double) camera.Bottom - (double) this.AbsoluteTop) / (double) this.CellHeight));
        for (int index1 = num1; index1 <= num2; ++index1)
        {
          for (int index2 = num3; index2 <= num4; ++index2)
          {
            if (this.Data[index1, index2])
              Draw.HollowRect(this.AbsoluteLeft + (float) index1 * this.CellWidth, this.AbsoluteTop + (float) index2 * this.CellHeight, this.CellWidth, this.CellHeight, color);
          }
        }
      }
    }

    public override bool Collide(Vector2 point)
    {
      if ((double) point.X >= (double) this.AbsoluteLeft && (double) point.Y >= (double) this.AbsoluteTop && (double) point.X < (double) this.AbsoluteRight && (double) point.Y < (double) this.AbsoluteBottom)
        return this.Data[(int) (((double) point.X - (double) this.AbsoluteLeft) / (double) this.CellWidth), (int) (((double) point.Y - (double) this.AbsoluteTop) / (double) this.CellHeight)];
      return false;
    }

    public override bool Collide(Rectangle rect)
    {
      if (!rect.Intersects(this.Bounds))
        return false;
      int x = (int) (((double) rect.Left - (double) this.AbsoluteLeft) / (double) this.CellWidth);
      int y = (int) (((double) rect.Top - (double) this.AbsoluteTop) / (double) this.CellHeight);
      int width = (int) (((double) rect.Right - (double) this.AbsoluteLeft - 1.0) / (double) this.CellWidth) - x + 1;
      int height = (int) (((double) rect.Bottom - (double) this.AbsoluteTop - 1.0) / (double) this.CellHeight) - y + 1;
      return this.CheckRect(x, y, width, height);
    }

    public override bool Collide(Vector2 from, Vector2 to)
    {
      from -= this.AbsolutePosition;
      to -= this.AbsolutePosition;
      from /= new Vector2(this.CellWidth, this.CellHeight);
      to /= new Vector2(this.CellWidth, this.CellHeight);
      bool flag = (double) Math.Abs(to.Y - from.Y) > (double) Math.Abs(to.X - from.X);
      if (flag)
      {
        float x1 = from.X;
        from.X = from.Y;
        from.Y = x1;
        float x2 = to.X;
        to.X = to.Y;
        to.Y = x2;
      }
      if ((double) from.X > (double) to.X)
      {
        Vector2 vector2 = from;
        from = to;
        to = vector2;
      }
      float num1 = 0.0f;
      float num2 = Math.Abs(to.Y - from.Y) / (to.X - from.X);
      int num3 = (double) from.Y < (double) to.Y ? 1 : -1;
      int y = (int) from.Y;
      int x3 = (int) to.X;
      for (int x1 = (int) from.X; x1 <= x3; ++x1)
      {
        if (flag)
        {
          if (this[y, x1])
            return true;
        }
        else if (this[x1, y])
          return true;
        num1 += num2;
        if ((double) num1 >= 0.5)
        {
          y += num3;
          --num1;
        }
      }
      return false;
    }

    public override bool Collide(Hitbox hitbox)
    {
      return this.Collide(hitbox.Bounds);
    }

    public override bool Collide(Grid grid)
    {
      throw new NotImplementedException();
    }

    public override bool Collide(Circle circle)
    {
      return false;
    }

    public override bool Collide(ColliderList list)
    {
      return list.Collide(this);
    }

    public static bool IsBitstringEmpty(string bitstring)
    {
      for (int index = 0; index < bitstring.Length; ++index)
      {
        if (bitstring[index] == '1')
          return false;
      }
      return true;
    }
  }
}

