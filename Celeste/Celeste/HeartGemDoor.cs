// Decompiled with JetBrains decompiler
// Type: Celeste.HeartGemDoor
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Celeste
{
  public class HeartGemDoor : Entity
  {
    private MTexture temp = new MTexture();
    private HeartGemDoor.Particle[] particles = new HeartGemDoor.Particle[50];
    private const string OpenedFlag = "opened_heartgem_door_";
    public static ParticleType P_Shimmer;
    public static ParticleType P_Slice;
    public readonly int Requires;
    public int Size;
    private readonly float openDistance;
    private float openPercent;
    private Solid TopSolid;
    private Solid BotSolid;
    private float offset;
    private Vector2 mist;
    private List<MTexture> icon;

    public int HeartGems
    {
      get
      {
        if (SaveData.Instance.CheatMode)
          return this.Requires;
        return SaveData.Instance.TotalHeartGems;
      }
    }

    public float Counter { get; private set; }

    public bool Opened { get; private set; }

    private float openAmount
    {
      get
      {
        return this.openPercent * this.openDistance;
      }
    }

    public HeartGemDoor(EntityData data, Vector2 offset)
      : base(data.Position + offset)
    {
      this.Requires = data.Int("requires", 0);
      this.Add((Component) new CustomBloom(new Action(this.RenderBloom)));
      this.Size = data.Width;
      this.openDistance = 32f;
      Vector2? nullable = data.FirstNodeNullable(new Vector2?(offset));
      if (nullable.HasValue)
        this.openDistance = Math.Abs(nullable.Value.Y - this.Y);
      this.icon = GFX.Game.GetAtlasSubtextures("objects/heartdoor/icon");
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      Level level1 = scene as Level;
      for (int index = 0; index < this.particles.Length; ++index)
      {
        this.particles[index].Position = new Vector2(Calc.Random.NextFloat((float) this.Size), Calc.Random.NextFloat((float) level1.Bounds.Height));
        this.particles[index].Speed = (float) Calc.Random.Range(4, 12);
        this.particles[index].Color = Color.White * Calc.Random.Range(0.2f, 0.6f);
      }
      Level level2 = level1;
      double x = (double) this.X;
      Rectangle bounds = level1.Bounds;
      double num1 = (double) (bounds.Top - 32);
      Vector2 position1 = new Vector2((float) x, (float) num1);
      double size1 = (double) this.Size;
      double y = (double) this.Y;
      bounds = level1.Bounds;
      double top = (double) bounds.Top;
      double num2 = y - top + 32.0;
      Solid solid1 = this.TopSolid = new Solid(position1, (float) size1, (float) num2, true);
      level2.Add((Entity) solid1);
      this.TopSolid.SurfaceSoundIndex = 32;
      Level level3 = level1;
      Vector2 position2 = new Vector2(this.X, this.Y);
      double size2 = (double) this.Size;
      bounds = level1.Bounds;
      double num3 = (double) bounds.Bottom - (double) this.Y + 32.0;
      Solid solid2 = this.BotSolid = new Solid(position2, (float) size2, (float) num3, true);
      level3.Add((Entity) solid2);
      this.BotSolid.SurfaceSoundIndex = 32;
      if ((this.Scene as Level).Session.GetFlag("opened_heartgem_door_" + (object) this.Requires))
      {
        this.Opened = true;
        this.openPercent = 1f;
        this.Counter = (float) this.Requires;
        this.TopSolid.Y -= this.openDistance;
        this.BotSolid.Y += this.openDistance;
      }
      else
        this.Add((Component) new Coroutine(this.Routine(), true));
    }

    private IEnumerator Routine()
    {
      Level level = this.Scene as Level;
      while (!this.Opened && (double) this.Counter < (double) this.Requires)
      {
        Player player = this.Scene.Tracker.GetEntity<Player>();
        if (player != null && (double) Math.Abs(player.X - this.Center.X) < 80.0)
        {
          if ((double) this.Counter == 0.0 && this.HeartGems > 0)
            Audio.Play("event:/game/09_core/frontdoor_heartfill", this.Position);
          if (this.HeartGems < this.Requires)
            level.Session.SetFlag("granny_door", true);
          int was = (int) this.Counter;
          int target = Math.Min(this.HeartGems, this.Requires);
          this.Counter = Calc.Approach(this.Counter, (float) target, (float) ((double) Engine.DeltaTime * (double) this.Requires * 0.800000011920929));
          if (was != (int) this.Counter)
          {
            yield return (object) 0.1f;
            if ((double) this.Counter < (double) target)
              Audio.Play("event:/game/09_core/frontdoor_heartfill", this.Position);
          }
        }
        else
          this.Counter = Calc.Approach(this.Counter, 0.0f, (float) ((double) Engine.DeltaTime * (double) this.Requires * 4.0));
        yield return (object) null;
        player = (Player) null;
      }
      yield return (object) 0.5f;
      this.Scene.Add((Entity) new HeartGemDoor.WhiteLine(this.Position, this.Size));
      level.Shake(0.3f);
      level.Flash(Color.White * 0.5f, false);
      Audio.Play("event:/game/09_core/frontdoor_unlock", this.Position);
      this.Opened = true;
      level.Session.SetFlag("opened_heartgem_door_" + (object) this.Requires, true);
      this.offset = 0.0f;
      yield return (object) 0.6f;
      float topFrom = this.TopSolid.Y;
      float topTo = this.TopSolid.Y - this.openDistance;
      float botFrom = this.BotSolid.Y;
      float botTo = this.BotSolid.Y + this.openDistance;
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime)
      {
        level.Shake(0.3f);
        this.openPercent = Ease.CubeIn(p);
        this.TopSolid.MoveToY(MathHelper.Lerp(topFrom, topTo, this.openPercent));
        this.BotSolid.MoveToY(MathHelper.Lerp(botFrom, botTo, this.openPercent));
        if ((double) p >= 0.400000005960464 && level.OnInterval(0.1f))
        {
          for (int i = 4; i < this.Size; i += 4)
          {
            level.ParticlesBG.Emit(HeartGemDoor.P_Shimmer, 1, new Vector2((float) ((double) this.TopSolid.Left + (double) i + 1.0), this.TopSolid.Bottom - 2f), new Vector2(2f, 2f), -1.570796f);
            level.ParticlesBG.Emit(HeartGemDoor.P_Shimmer, 1, new Vector2((float) ((double) this.BotSolid.Left + (double) i + 1.0), this.BotSolid.Top + 2f), new Vector2(2f, 2f), 1.570796f);
          }
        }
        yield return (object) null;
      }
      this.TopSolid.MoveToY(topTo);
      this.BotSolid.MoveToY(botTo);
      this.openPercent = 1f;
    }

    public override void Update()
    {
      base.Update();
      if (this.Opened)
        return;
      this.offset += 12f * Engine.DeltaTime;
      this.mist.X -= 4f * Engine.DeltaTime;
      this.mist.Y -= 24f * Engine.DeltaTime;
      for (int index = 0; index < this.particles.Length; ++index)
        this.particles[index].Position.Y += this.particles[index].Speed * Engine.DeltaTime;
    }

    public void RenderBloom()
    {
      if (this.Opened)
        return;
      this.DrawBloom(new Rectangle((int) this.TopSolid.X, (int) this.TopSolid.Y, this.Size, (int) ((double) this.TopSolid.Height + (double) this.BotSolid.Height)));
    }

    private void DrawBloom(Rectangle bounds)
    {
      Draw.Rect((float) (bounds.Left - 4), (float) bounds.Top, 2f, (float) bounds.Height, Color.White * 0.25f);
      Draw.Rect((float) (bounds.Left - 2), (float) bounds.Top, 2f, (float) bounds.Height, Color.White * 0.5f);
      Draw.Rect(bounds, Color.White * 0.75f);
      Draw.Rect((float) bounds.Right, (float) bounds.Top, 2f, (float) bounds.Height, Color.White * 0.5f);
      Draw.Rect((float) (bounds.Right + 2), (float) bounds.Top, 2f, (float) bounds.Height, Color.White * 0.25f);
    }

    private void DrawMist(Rectangle bounds, Vector2 mist)
    {
      Color color = Color.White * 0.6f;
      MTexture mtexture = GFX.Game["objects/heartdoor/mist"];
      int val1_1 = mtexture.Width / 2;
      int val1_2 = mtexture.Height / 2;
      for (int index1 = 0; index1 < bounds.Width; index1 += val1_1)
      {
        for (int index2 = 0; index2 < bounds.Height; index2 += val1_2)
        {
          mtexture.GetSubtexture((int) this.Mod(mist.X, (float) val1_1), (int) this.Mod(mist.Y, (float) val1_2), Math.Min(val1_1, bounds.Width - index1), Math.Min(val1_2, bounds.Height - index2), this.temp);
          this.temp.Draw(new Vector2((float) (bounds.X + index1), (float) (bounds.Y + index2)), Vector2.Zero, color);
        }
      }
    }

    private void DrawInterior(Rectangle bounds)
    {
      Draw.Rect(bounds, Calc.HexToColor("18668f"));
      this.DrawMist(bounds, this.mist);
      this.DrawMist(bounds, new Vector2(this.mist.Y, this.mist.X) * 1.5f);
      Vector2 vector2_1 = (this.Scene as Level).Camera.Position;
      if (this.Opened)
        vector2_1 = Vector2.Zero;
      for (int index = 0; index < this.particles.Length; ++index)
      {
        Vector2 vector2_2 = this.particles[index].Position + vector2_1 * 0.2f;
        vector2_2.X = this.Mod(vector2_2.X, (float) bounds.Width);
        vector2_2.Y = this.Mod(vector2_2.Y, (float) bounds.Height);
        Draw.Pixel.Draw(new Vector2((float) bounds.X, (float) bounds.Y) + vector2_2, Vector2.Zero, this.particles[index].Color);
      }
    }

    private void DrawEdges(Rectangle bounds, Color color)
    {
      MTexture mtexture1 = GFX.Game["objects/heartdoor/edge"];
      MTexture mtexture2 = GFX.Game["objects/heartdoor/top"];
      int height = (int) ((double) this.offset % 8.0);
      if (height > 0)
      {
        mtexture1.GetSubtexture(0, 8 - height, 7, height, this.temp);
        this.temp.DrawJustified(new Vector2((float) (bounds.Left + 4), (float) bounds.Top), new Vector2(0.5f, 0.0f), color, new Vector2(-1f, 1f));
        this.temp.DrawJustified(new Vector2((float) (bounds.Right - 4), (float) bounds.Top), new Vector2(0.5f, 0.0f), color, new Vector2(1f, 1f));
      }
      for (int index = 0; index < bounds.Height; index += 8)
      {
        mtexture1.GetSubtexture(0, 0, 8, Math.Min(8, bounds.Height - index), this.temp);
        this.temp.DrawJustified(new Vector2((float) (bounds.Left + 4), (float) (bounds.Top + index + height)), new Vector2(0.5f, 0.0f), color, new Vector2(-1f, 1f));
        this.temp.DrawJustified(new Vector2((float) (bounds.Right - 4), (float) (bounds.Top + index + height)), new Vector2(0.5f, 0.0f), color, new Vector2(1f, 1f));
      }
      if (!this.Opened)
        return;
      for (int index = 0; index < bounds.Width; index += 8)
      {
        mtexture2.DrawCentered(new Vector2((float) (bounds.Left + 4 + index), (float) (bounds.Top + 4)), color);
        mtexture2.DrawCentered(new Vector2((float) (bounds.Left + 4 + index), (float) (bounds.Bottom - 4)), color, new Vector2(1f, -1f));
      }
    }

    public override void Render()
    {
      Color color = this.Opened ? Color.White * 0.25f : Color.White;
      if (!this.Opened)
      {
        Rectangle bounds = new Rectangle((int) this.TopSolid.X, (int) this.TopSolid.Y, this.Size, (int) ((double) this.TopSolid.Height + (double) this.BotSolid.Height));
        this.DrawInterior(bounds);
        this.DrawEdges(bounds, color);
      }
      else
      {
        Rectangle bounds1 = new Rectangle((int) this.TopSolid.X, (int) this.TopSolid.Y, this.Size, (int) this.TopSolid.Height);
        this.DrawInterior(bounds1);
        this.DrawEdges(bounds1, color);
        Rectangle bounds2 = new Rectangle((int) this.BotSolid.X, (int) this.BotSolid.Y, this.Size, (int) this.BotSolid.Height);
        this.DrawInterior(bounds2);
        this.DrawEdges(bounds2, color);
      }
      float num1 = 12f;
      int num2 = (int) ((double) (this.Size - 8) / (double) num1);
      int num3 = (int) Math.Ceiling((double) this.Requires / (double) num2);
      for (int index1 = 0; index1 < num3; ++index1)
      {
        int num4 = (index1 + 1) * num2 < this.Requires ? num2 : this.Requires - index1 * num2;
        Vector2 vector2 = new Vector2(this.X + (float) this.Size * 0.5f, this.Y) + new Vector2((float) ((double) -num4 / 2.0 + 0.5), (float) ((double) -num3 / 2.0 + (double) index1 + 0.5)) * num1;
        if (this.Opened)
        {
          if (index1 < num3 / 2)
            vector2.Y -= this.openAmount + 8f;
          else
            vector2.Y += this.openAmount + 8f;
        }
        for (int index2 = 0; index2 < num4; ++index2)
        {
          int num5 = index1 * num2 + index2;
          this.icon[(int) ((double) Ease.CubeIn(Calc.ClampedMap(this.Counter, (float) num5, (float) num5 + 1f, 0.0f, 1f)) * (double) (this.icon.Count - 1))].DrawCentered(vector2 + new Vector2((float) index2 * num1, 0.0f), color);
        }
      }
    }

    private float Mod(float x, float m)
    {
      return (x % m + m) % m;
    }

    private struct Particle
    {
      public Vector2 Position;
      public float Speed;
      public Color Color;
    }

    private class WhiteLine : Entity
    {
      private float fade = 1f;
      private int blockSize;

      public WhiteLine(Vector2 origin, int blockSize)
        : base(origin)
      {
        this.Depth = -1000000;
        this.blockSize = blockSize;
      }

      public override void Update()
      {
        base.Update();
        this.fade = Calc.Approach(this.fade, 0.0f, Engine.DeltaTime);
        if ((double) this.fade > 0.0)
          return;
        this.RemoveSelf();
        Level level = this.SceneAs<Level>();
        for (float left = (float) (int) level.Camera.Left; (double) left < (double) level.Camera.Right; ++left)
        {
          if ((double) left < (double) this.X || (double) left >= (double) this.X + (double) this.blockSize)
            level.Particles.Emit(HeartGemDoor.P_Slice, new Vector2(left, this.Y));
        }
      }

      public override void Render()
      {
        Vector2 position = (this.Scene as Level).Camera.Position;
        float height = Math.Max(1f, 4f * this.fade);
        Draw.Rect(position.X - 10f, this.Y - height / 2f, 340f, height, Color.White);
      }
    }
  }
}

