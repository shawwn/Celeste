// Decompiled with JetBrains decompiler
// Type: Celeste.AnimatedTiles
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste
{
  public class AnimatedTiles : Component
  {
    public Color Color = Color.get_White();
    public float Alpha = 1f;
    public Camera ClipCamera;
    public Vector2 Position;
    public AnimatedTilesBank Bank;
    private VirtualMap<List<AnimatedTiles.Tile>> tiles;

    public AnimatedTiles(int columns, int rows, AnimatedTilesBank bank)
      : base(true, true)
    {
      this.tiles = new VirtualMap<List<AnimatedTiles.Tile>>(columns, rows, (List<AnimatedTiles.Tile>) null);
      this.Bank = bank;
    }

    public void Set(int x, int y, string name, float scaleX = 1f, float scaleY = 1f)
    {
      if (string.IsNullOrEmpty(name))
        return;
      AnimatedTilesBank.Animation animation = this.Bank.AnimationsByName[name];
      (this.tiles[x, y] ?? (this.tiles[x, y] = new List<AnimatedTiles.Tile>())).Add(new AnimatedTiles.Tile()
      {
        AnimationID = animation.ID,
        Frame = (float) Calc.Random.Next(animation.Frames.Length),
        Scale = new Vector2(scaleX, scaleY)
      });
    }

    public Rectangle GetClippedRenderTiles(int extend)
    {
      Vector2 vector2 = Vector2.op_Addition(this.Entity.Position, this.Position);
      int val1_1;
      int val1_2;
      int val1_3;
      int val1_4;
      if (this.ClipCamera == null)
      {
        val1_1 = -extend;
        val1_2 = -extend;
        val1_3 = this.tiles.Columns + extend;
        val1_4 = this.tiles.Rows + extend;
      }
      else
      {
        Camera clipCamera = this.ClipCamera;
        val1_1 = (int) Math.Max(0.0, Math.Floor(((double) clipCamera.Left - vector2.X) / 8.0) - (double) extend);
        val1_2 = (int) Math.Max(0.0, Math.Floor(((double) clipCamera.Top - vector2.Y) / 8.0) - (double) extend);
        val1_3 = (int) Math.Min((double) this.tiles.Columns, Math.Ceiling(((double) clipCamera.Right - vector2.X) / 8.0) + (double) extend);
        val1_4 = (int) Math.Min((double) this.tiles.Rows, Math.Ceiling(((double) clipCamera.Bottom - vector2.Y) / 8.0) + (double) extend);
      }
      int num1 = Math.Max(val1_1, 0);
      int num2 = Math.Max(val1_2, 0);
      int num3 = Math.Min(val1_3, this.tiles.Columns);
      int num4 = Math.Min(val1_4, this.tiles.Rows);
      return new Rectangle(num1, num2, num3 - num1, num4 - num2);
    }

    public override void Update()
    {
      Rectangle clippedRenderTiles = this.GetClippedRenderTiles(1);
      for (int left = ((Rectangle) ref clippedRenderTiles).get_Left(); left < ((Rectangle) ref clippedRenderTiles).get_Right(); ++left)
      {
        for (int top = ((Rectangle) ref clippedRenderTiles).get_Top(); top < ((Rectangle) ref clippedRenderTiles).get_Bottom(); ++top)
        {
          List<AnimatedTiles.Tile> tile = this.tiles[left, top];
          if (tile != null)
          {
            for (int index = 0; index < tile.Count; ++index)
            {
              AnimatedTilesBank.Animation animation = this.Bank.Animations[tile[index].AnimationID];
              tile[index].Frame += Engine.DeltaTime / animation.Delay;
            }
          }
        }
      }
    }

    public override void Render()
    {
      this.RenderAt(Vector2.op_Addition(this.Entity.Position, this.Position));
    }

    public void RenderAt(Vector2 position)
    {
      Rectangle clippedRenderTiles = this.GetClippedRenderTiles(1);
      Color color = Color.op_Multiply(this.Color, this.Alpha);
      for (int left = ((Rectangle) ref clippedRenderTiles).get_Left(); left < ((Rectangle) ref clippedRenderTiles).get_Right(); ++left)
      {
        for (int top = ((Rectangle) ref clippedRenderTiles).get_Top(); top < ((Rectangle) ref clippedRenderTiles).get_Bottom(); ++top)
        {
          List<AnimatedTiles.Tile> tile1 = this.tiles[left, top];
          if (tile1 != null)
          {
            for (int index = 0; index < tile1.Count; ++index)
            {
              AnimatedTiles.Tile tile2 = tile1[index];
              AnimatedTilesBank.Animation animation = this.Bank.Animations[tile2.AnimationID];
              animation.Frames[(int) tile2.Frame % animation.Frames.Length].Draw(Vector2.op_Addition(Vector2.op_Addition(position, animation.Offset), Vector2.op_Multiply(new Vector2((float) left + 0.5f, (float) top + 0.5f), 8f)), animation.Origin, color, tile2.Scale);
            }
          }
        }
      }
    }

    private class Tile
    {
      public int AnimationID;
      public float Frame;
      public Vector2 Scale;
    }
  }
}
