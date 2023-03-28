// Decompiled with JetBrains decompiler
// Type: Monocle.VirtualMap`1
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

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

    public VirtualMap(int columns, int rows, T emptyValue = default(T))
    {
      this.Columns = columns;
      this.Rows = rows;
      this.SegmentColumns = columns / 50 + 1;
      this.SegmentRows = rows / 50 + 1;
      this.segments = new T[this.SegmentColumns, this.SegmentRows][,];
      this.EmptyValue = emptyValue;
    }

    public VirtualMap(T[,] map, T emptyValue = default(T))
      : this(map.GetLength(0), map.GetLength(1), emptyValue)
    {
      for (int x = 0; x < this.Columns; ++x)
      {
        for (int y = 0; y < this.Rows; ++y)
          this[x, y] = map[x, y];
      }
    }

    public bool AnyInSegmentAtTile(int x, int y) => this.segments[x / 50, y / 50] != null;

    public bool AnyInSegment(int segmentX, int segmentY) => this.segments[segmentX, segmentY] != null;

    public T InSegment(int segmentX, int segmentY, int x, int y) => this.segments[segmentX, segmentY][x, y];

    public T[,] GetSegment(int segmentX, int segmentY) => this.segments[segmentX, segmentY];

    public T SafeCheck(int x, int y) => x >= 0 && y >= 0 && x < this.Columns && y < this.Rows ? this[x, y] : this.EmptyValue;

    public T this[int x, int y]
    {
      get
      {
        int index1 = x / 50;
        int index2 = y / 50;
        T[,] segment = this.segments[index1, index2];
        return segment == null ? this.EmptyValue : segment[x - index1 * 50, y - index2 * 50];
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
      T[,] array = new T[this.Columns, this.Rows];
      for (int x = 0; x < this.Columns; ++x)
      {
        for (int y = 0; y < this.Rows; ++y)
          array[x, y] = this[x, y];
      }
      return array;
    }

    public VirtualMap<T> Clone()
    {
      VirtualMap<T> virtualMap = new VirtualMap<T>(this.Columns, this.Rows, this.EmptyValue);
      for (int x = 0; x < this.Columns; ++x)
      {
        for (int y = 0; y < this.Rows; ++y)
          virtualMap[x, y] = this[x, y];
      }
      return virtualMap;
    }
  }
}
