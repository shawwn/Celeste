// Decompiled with JetBrains decompiler
// Type: Celeste.PlaybackBillboard
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  [Tracked(false)]
  public class PlaybackBillboard : Entity
  {
    public const int BGDepth = 9010;
    public static readonly Color BackgroundColor = Color.Lerp(Color.DarkSlateBlue, Color.Black, 0.6f);
    public uint Seed;
    private MTexture[,] tiles;

    public PlaybackBillboard(EntityData e, Vector2 offset)
    {
      this.Position = e.Position + offset;
      this.Collider = (Collider) new Hitbox((float) e.Width, (float) e.Height);
      this.Depth = 9010;
      this.Add((Component) new CustomBloom(new Action(this.RenderBloom)));
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      scene.Add((Entity) new PlaybackBillboard.FG(this));
    }

    public override void Awake(Scene scene)
    {
      base.Awake(scene);
      MTexture mtexture = GFX.Game["scenery/tvSlices"];
      this.tiles = new MTexture[mtexture.Width / 8, mtexture.Height / 8];
      for (int index1 = 0; index1 < mtexture.Width / 8; ++index1)
      {
        for (int index2 = 0; index2 < mtexture.Height / 8; ++index2)
          this.tiles[index1, index2] = mtexture.GetSubtexture(new Rectangle(index1 * 8, index2 * 8, 8, 8));
      }
      int x1 = (int) ((double) this.Width / 8.0);
      int y1 = (int) ((double) this.Height / 8.0);
      for (int x2 = -1; x2 <= x1; ++x2)
      {
        this.AutoTile(x2, -1);
        this.AutoTile(x2, y1);
      }
      for (int y2 = 0; y2 < y1; ++y2)
      {
        this.AutoTile(-1, y2);
        this.AutoTile(x1, y2);
      }
    }

    private void AutoTile(int x, int y)
    {
      if (!this.Empty(x, y))
        return;
      bool flag1 = !this.Empty(x - 1, y);
      bool flag2 = !this.Empty(x + 1, y);
      bool flag3 = !this.Empty(x, y - 1);
      bool flag4 = !this.Empty(x, y + 1);
      bool flag5 = !this.Empty(x - 1, y - 1);
      bool flag6 = !this.Empty(x + 1, y - 1);
      bool flag7 = !this.Empty(x - 1, y + 1);
      bool flag8 = !this.Empty(x + 1, y + 1);
      if (((flag2 ? 0 : (!flag4 ? 1 : 0)) & (flag8 ? 1 : 0)) != 0)
        this.Tile(x, y, this.tiles[0, 0]);
      else if (((flag1 ? 0 : (!flag4 ? 1 : 0)) & (flag7 ? 1 : 0)) != 0)
        this.Tile(x, y, this.tiles[2, 0]);
      else if (((flag3 ? 0 : (!flag2 ? 1 : 0)) & (flag6 ? 1 : 0)) != 0)
        this.Tile(x, y, this.tiles[0, 2]);
      else if (((flag3 ? 0 : (!flag1 ? 1 : 0)) & (flag5 ? 1 : 0)) != 0)
        this.Tile(x, y, this.tiles[2, 2]);
      else if (flag2 & flag4)
        this.Tile(x, y, this.tiles[3, 0]);
      else if (flag1 & flag4)
        this.Tile(x, y, this.tiles[4, 0]);
      else if (flag2 & flag3)
        this.Tile(x, y, this.tiles[3, 2]);
      else if (flag1 & flag3)
        this.Tile(x, y, this.tiles[4, 2]);
      else if (flag4)
        this.Tile(x, y, this.tiles[1, 0]);
      else if (flag2)
        this.Tile(x, y, this.tiles[0, 1]);
      else if (flag1)
      {
        this.Tile(x, y, this.tiles[2, 1]);
      }
      else
      {
        if (!flag3)
          return;
        this.Tile(x, y, this.tiles[1, 2]);
      }
    }

    private void Tile(int x, int y, MTexture tile)
    {
      Monocle.Image image = new Monocle.Image(tile);
      image.Position = new Vector2((float) x, (float) y) * 8f;
      this.Add((Component) image);
    }

    private bool Empty(int x, int y) => !this.Scene.CollideCheck<PlaybackBillboard>(new Rectangle((int) this.X + x * 8, (int) this.Y + y * 8, 8, 8));

    public override void Update()
    {
      base.Update();
      if (!this.Scene.OnInterval(0.1f))
        return;
      ++this.Seed;
    }

    private void RenderBloom() => Draw.Rect(this.Collider, Color.White * 0.4f);

    public override void Render()
    {
      base.Render();
      uint seed = this.Seed;
      Draw.Rect(this.Collider, PlaybackBillboard.BackgroundColor);
      PlaybackBillboard.DrawNoise(this.Collider.Bounds, ref seed, Color.White * 0.1f);
    }

    public static void DrawNoise(Rectangle bounds, ref uint seed, Color color)
    {
      MTexture mtexture = GFX.Game["util/noise"];
      Vector2 vector2_1 = new Vector2(PlaybackBillboard.PseudoRandRange(ref seed, 0.0f, (float) (mtexture.Width / 2)), PlaybackBillboard.PseudoRandRange(ref seed, 0.0f, (float) (mtexture.Height / 2)));
      Vector2 vector2_2 = new Vector2((float) mtexture.Width, (float) mtexture.Height) / 2f;
      for (float num1 = 0.0f; (double) num1 < (double) bounds.Width; num1 += vector2_2.X)
      {
        float width = Math.Min((float) bounds.Width - num1, vector2_2.X);
        for (float num2 = 0.0f; (double) num2 < (double) bounds.Height; num2 += vector2_2.Y)
        {
          float height = Math.Min((float) bounds.Height - num2, vector2_2.Y);
          Rectangle rectangle = new Rectangle((int) ((double) mtexture.ClipRect.X + (double) vector2_1.X), (int) ((double) mtexture.ClipRect.Y + (double) vector2_1.Y), (int) width, (int) height);
          Draw.SpriteBatch.Draw(mtexture.Texture.Texture, new Vector2((float) bounds.X + num1, (float) bounds.Y + num2), new Rectangle?(rectangle), color);
        }
      }
    }

    private static uint PseudoRand(ref uint seed)
    {
      seed ^= seed << 13;
      seed ^= seed >> 17;
      return seed;
    }

    private static float PseudoRandRange(ref uint seed, float min, float max) => min + (float) ((double) (PlaybackBillboard.PseudoRand(ref seed) % 1000U) / 1000.0 * ((double) max - (double) min));

    private class FG : Entity
    {
      public PlaybackBillboard Parent;

      public FG(PlaybackBillboard parent)
      {
        this.Parent = parent;
        this.Depth = this.Parent.Depth - 5;
      }

      public override void Render()
      {
        uint seed = this.Parent.Seed;
        PlaybackBillboard.DrawNoise(this.Parent.Collider.Bounds, ref seed, Color.White * 0.1f);
        for (int y = (int) this.Parent.Y; (double) y < (double) this.Parent.Bottom; y += 2)
        {
          float num = (float) (0.05000000074505806 + (1.0 + Math.Sin((double) y / 16.0 + (double) this.Scene.TimeActive * 2.0)) / 2.0 * 0.20000000298023224);
          Draw.Line(this.Parent.X, (float) y, this.Parent.X + this.Parent.Width, (float) y, Color.Teal * num);
        }
      }
    }
  }
}
