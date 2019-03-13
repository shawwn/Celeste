// Decompiled with JetBrains decompiler
// Type: Monocle.VirtualMap`1
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

namespace Monocle
{
  public class VirtualMap<T>
  {
    public const int SegmentSize = 50;
    public readonly int Columns;
    public readonly int Rows;
    public readonly int SegmentColumns;
    public readonly int SegmentRows;
    public readonly T EmptyValue;
    private T[,][,] segments;

    public VirtualMap(int columns, int rows, T emptyValue)
    {
      this.Columns = columns;
      this.Rows = rows;
      this.SegmentColumns = columns / 50 + 1;
      this.SegmentRows = rows / 50 + 1;
      this.segments = new T[this.SegmentColumns, this.SegmentRows][,];
      this.EmptyValue = emptyValue;
    }

    public VirtualMap(T[,] map, T emptyValue)
      : this(map.GetLength(0), map.GetLength(1), emptyValue)
    {
      for (int index1 = 0; index1 < this.Columns; ++index1)
      {
        for (int index2 = 0; index2 < this.Rows; ++index2)
          this[index1, index2] = map[index1, index2];
      }
    }

    public bool AnyInSegmentAtTile(int x, int y)
    {
      return this.segments[x / 50, y / 50] != null;
    }

    public bool AnyInSegment(int segmentX, int segmentY)
    {
      return this.segments[segmentX, segmentY] != null;
    }

    public T InSegment(int segmentX, int segmentY, int x, int y)
    {
      return this.segments[segmentX, segmentY][x, y];
    }

    public T[,] GetSegment(int segmentX, int segmentY)
    {
      return this.segments[segmentX, segmentY];
    }

    public T SafeCheck(int x, int y)
    {
      if (x >= 0 && y >= 0 && x < this.Columns && y < this.Rows)
        return this[x, y];
      return this.EmptyValue;
    }

    public T this[int x, int y]
    {
      get
      {
        int index1 = x / 50;
        int index2 = y / 50;
        T[,] segment = this.segments[index1, index2];
        if (segment == null)
          return this.EmptyValue;
        return segment[x - index1 * 50, y - index2 * 50];
      }
      set
      {
        int index1 = x / 50;
        int index2 = y / 50;
        if (this.segments[index1, index2] == null)
        {
          this.segments[index1, index2] = new T[50, 50];
          if ((object) this.EmptyValue != null && !this.EmptyValue.Equals((object) default (T)))
          {
            for (int index3 = 0; index3 < 50; ++index3)
            {
              for (int index4 = 0; index4 < 50; ++index4)
                this.segments[index1, index2][index3, index4] = this.EmptyValue;
            }
          }
        }
        this.segments[index1, index2][x - index1 * 50, y - index2 * 50] = value;
      }
    }

    public T[,] ToArray()
    {
      T[,] objArray = new T[this.Columns, this.Rows];
      for (int index1 = 0; index1 < this.Columns; ++index1)
      {
        for (int index2 = 0; index2 < this.Rows; ++index2)
          objArray[index1, index2] = this[index1, index2];
      }
      return objArray;
    }

    public VirtualMap<T> Clone()
    {
      VirtualMap<T> virtualMap = new VirtualMap<T>(this.Columns, this.Rows, this.EmptyValue);
      for (int index1 = 0; index1 < this.Columns; ++index1)
      {
        for (int index2 = 0; index2 < this.Rows; ++index2)
          virtualMap[index1, index2] = this[index1, index2];
      }
      return virtualMap;
    }
  }
}

