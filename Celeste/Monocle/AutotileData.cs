// Decompiled with JetBrains decompiler
// Type: Monocle.AutotileData
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System.Xml;

namespace Monocle
{
  public class AutotileData
  {
    public int[] Center;
    public int[] Single;
    public int[] SingleHorizontalLeft;
    public int[] SingleHorizontalCenter;
    public int[] SingleHorizontalRight;
    public int[] SingleVerticalTop;
    public int[] SingleVerticalCenter;
    public int[] SingleVerticalBottom;
    public int[] Top;
    public int[] Bottom;
    public int[] Left;
    public int[] Right;
    public int[] TopLeft;
    public int[] TopRight;
    public int[] BottomLeft;
    public int[] BottomRight;
    public int[] InsideTopLeft;
    public int[] InsideTopRight;
    public int[] InsideBottomLeft;
    public int[] InsideBottomRight;

    public AutotileData(XmlElement xml)
    {
      this.Center = Calc.ReadCSVInt(xml.ChildText(nameof (Center), ""));
      this.Single = Calc.ReadCSVInt(xml.ChildText(nameof (Single), ""));
      this.SingleHorizontalLeft = Calc.ReadCSVInt(xml.ChildText(nameof (SingleHorizontalLeft), ""));
      this.SingleHorizontalCenter = Calc.ReadCSVInt(xml.ChildText(nameof (SingleHorizontalCenter), ""));
      this.SingleHorizontalRight = Calc.ReadCSVInt(xml.ChildText(nameof (SingleHorizontalRight), ""));
      this.SingleVerticalTop = Calc.ReadCSVInt(xml.ChildText(nameof (SingleVerticalTop), ""));
      this.SingleVerticalCenter = Calc.ReadCSVInt(xml.ChildText(nameof (SingleVerticalCenter), ""));
      this.SingleVerticalBottom = Calc.ReadCSVInt(xml.ChildText(nameof (SingleVerticalBottom), ""));
      this.Top = Calc.ReadCSVInt(xml.ChildText(nameof (Top), ""));
      this.Bottom = Calc.ReadCSVInt(xml.ChildText(nameof (Bottom), ""));
      this.Left = Calc.ReadCSVInt(xml.ChildText(nameof (Left), ""));
      this.Right = Calc.ReadCSVInt(xml.ChildText(nameof (Right), ""));
      this.TopLeft = Calc.ReadCSVInt(xml.ChildText(nameof (TopLeft), ""));
      this.TopRight = Calc.ReadCSVInt(xml.ChildText(nameof (TopRight), ""));
      this.BottomLeft = Calc.ReadCSVInt(xml.ChildText(nameof (BottomLeft), ""));
      this.BottomRight = Calc.ReadCSVInt(xml.ChildText(nameof (BottomRight), ""));
      this.InsideTopLeft = Calc.ReadCSVInt(xml.ChildText(nameof (InsideTopLeft), ""));
      this.InsideTopRight = Calc.ReadCSVInt(xml.ChildText(nameof (InsideTopRight), ""));
      this.InsideBottomLeft = Calc.ReadCSVInt(xml.ChildText(nameof (InsideBottomLeft), ""));
      this.InsideBottomRight = Calc.ReadCSVInt(xml.ChildText(nameof (InsideBottomRight), ""));
    }

    public int TileHandler()
    {
      if (Tiler.Left && Tiler.Right && (Tiler.Up && Tiler.Down) && (Tiler.UpLeft && Tiler.UpRight && (Tiler.DownLeft && Tiler.DownRight)))
        return this.GetTileID(this.Center);
      if (!Tiler.Up && !Tiler.Down)
      {
        if (Tiler.Left && Tiler.Right)
          return this.GetTileID(this.SingleHorizontalCenter);
        if (!Tiler.Left && !Tiler.Right)
          return this.GetTileID(this.Single);
        if (Tiler.Left)
          return this.GetTileID(this.SingleHorizontalRight);
        return this.GetTileID(this.SingleHorizontalLeft);
      }
      if (!Tiler.Left && !Tiler.Right)
      {
        if (Tiler.Up && Tiler.Down)
          return this.GetTileID(this.SingleVerticalCenter);
        if (Tiler.Down)
          return this.GetTileID(this.SingleVerticalTop);
        return this.GetTileID(this.SingleVerticalBottom);
      }
      if (Tiler.Up && Tiler.Down && (Tiler.Left && !Tiler.Right))
        return this.GetTileID(this.Right);
      if (Tiler.Up && Tiler.Down && (!Tiler.Left && Tiler.Right))
        return this.GetTileID(this.Left);
      if (Tiler.Up && !Tiler.Left && (Tiler.Right && !Tiler.Down))
        return this.GetTileID(this.BottomLeft);
      if (Tiler.Up && Tiler.Left && (!Tiler.Right && !Tiler.Down))
        return this.GetTileID(this.BottomRight);
      if (Tiler.Down && Tiler.Right && (!Tiler.Left && !Tiler.Up))
        return this.GetTileID(this.TopLeft);
      if (Tiler.Down && !Tiler.Right && (Tiler.Left && !Tiler.Up))
        return this.GetTileID(this.TopRight);
      if (Tiler.Up && Tiler.Down && (!Tiler.DownRight && Tiler.DownLeft))
        return this.GetTileID(this.InsideTopLeft);
      if (Tiler.Up && Tiler.Down && (Tiler.DownRight && !Tiler.DownLeft))
        return this.GetTileID(this.InsideTopRight);
      if (Tiler.Up && Tiler.Down && (Tiler.UpLeft && !Tiler.UpRight))
        return this.GetTileID(this.InsideBottomLeft);
      if (Tiler.Up && Tiler.Down && (!Tiler.UpLeft && Tiler.UpRight))
        return this.GetTileID(this.InsideBottomRight);
      if (!Tiler.Down)
        return this.GetTileID(this.Bottom);
      return this.GetTileID(this.Top);
    }

    private int GetTileID(int[] choices)
    {
      if (choices.Length == 0)
        return -1;
      if (choices.Length == 1)
        return choices[0];
      return Calc.Random.Choose<int>(choices);
    }
  }
}
