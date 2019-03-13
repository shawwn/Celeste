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
      : base(Vector2.op_Addition(data.Position, offset))
    {
      this.Requires = data.Int("requires", 0);
      this.Add((Component) new CustomBloom(new Action(this.RenderBloom)));
      this.Size = data.Width;
      this.openDistance = 32f;
      Vector2? nullable = data.FirstNodeNullable(new Vector2?(offset));
      if (nullable.HasValue)
        this.openDistance = Math.Abs((float) nullable.Value.Y - this.Y);
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
        this.particles[index].Color = Color.op_Multiply(Color.get_White(), Calc.Random.Range(0.2f, 0.6f));
      }
      Level level2 = level1;
      double x = (double) this.X;
      Rectangle bounds = level1.Bounds;
      double num1 = (double) (((Rectangle) ref bounds).get_Top() - 32);
      Vector2 position1 = new Vector2((float) x, (float) num1);
      double size1 = (double) this.Size;
      double y = (double) this.Y;
      bounds = level1.Bounds;
      double top = (double) ((Rectangle) ref bounds).get_Top();
      double num2 = y - top + 32.0;
      Solid solid1 = this.TopSolid = new Solid(position1, (float) size1, (float) num2, true);
      level2.Add((Entity) solid1);
      this.TopSolid.SurfaceSoundIndex = 32;
      Level level3 = level1;
      Vector2 position2 = new Vector2(this.X, this.Y);
      double size2 = (double) this.Size;
      bounds = level1.Bounds;
      double num3 = (double) ((Rectangle) ref bounds).get_Bottom() - (double) this.Y + 32.0;
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
      HeartGemDoor heartGemDoor = this;
      Level level = heartGemDoor.Scene as Level;
      while (!heartGemDoor.Opened && (double) heartGemDoor.Counter < (double) heartGemDoor.Requires)
      {
        Player entity = heartGemDoor.Scene.Tracker.GetEntity<Player>();
        if (entity != null && (double) Math.Abs(entity.X - (float) heartGemDoor.Center.X) < 80.0)
        {
          if ((double) heartGemDoor.Counter == 0.0 && heartGemDoor.HeartGems > 0)
            Audio.Play("event:/game/09_core/frontdoor_heartfill", heartGemDoor.Position);
          if (heartGemDoor.HeartGems < heartGemDoor.Requires)
            level.Session.SetFlag("granny_door", true);
          int counter1 = (int) heartGemDoor.Counter;
          int target = Math.Min(heartGemDoor.HeartGems, heartGemDoor.Requires);
          heartGemDoor.Counter = Calc.Approach(heartGemDoor.Counter, (float) target, (float) ((double) Engine.DeltaTime * (double) heartGemDoor.Requires * 0.800000011920929));
          int counter2 = (int) heartGemDoor.Counter;
          if (counter1 != counter2)
          {
            yield return (object) 0.1f;
            if ((double) heartGemDoor.Counter < (double) target)
              Audio.Play("event:/game/09_core/frontdoor_heartfill", heartGemDoor.Position);
          }
        }
        else
          heartGemDoor.Counter = Calc.Approach(heartGemDoor.Counter, 0.0f, (float) ((double) Engine.DeltaTime * (double) heartGemDoor.Requires * 4.0));
        yield return (object) null;
      }
      yield return (object) 0.5f;
      heartGemDoor.Scene.Add((Entity) new HeartGemDoor.WhiteLine(heartGemDoor.Position, heartGemDoor.Size));
      level.Shake(0.3f);
      level.Flash(Color.op_Multiply(Color.get_White(), 0.5f), false);
      Audio.Play("event:/game/09_core/frontdoor_unlock", heartGemDoor.Position);
      heartGemDoor.Opened = true;
      level.Session.SetFlag("opened_heartgem_door_" + (object) heartGemDoor.Requires, true);
      heartGemDoor.offset = 0.0f;
      yield return (object) 0.6f;
      float topFrom = heartGemDoor.TopSolid.Y;
      float topTo = heartGemDoor.TopSolid.Y - heartGemDoor.openDistance;
      float botFrom = heartGemDoor.BotSolid.Y;
      float botTo = heartGemDoor.BotSolid.Y + heartGemDoor.openDistance;
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime)
      {
        level.Shake(0.3f);
        heartGemDoor.openPercent = Ease.CubeIn(p);
        heartGemDoor.TopSolid.MoveToY(MathHelper.Lerp(topFrom, topTo, heartGemDoor.openPercent));
        heartGemDoor.BotSolid.MoveToY(MathHelper.Lerp(botFrom, botTo, heartGemDoor.openPercent));
        if ((double) p >= 0.400000005960464 && level.OnInterval(0.1f))
        {
          for (int index = 4; index < heartGemDoor.Size; index += 4)
          {
            level.ParticlesBG.Emit(HeartGemDoor.P_Shimmer, 1, new Vector2((float) ((double) heartGemDoor.TopSolid.Left + (double) index + 1.0), heartGemDoor.TopSolid.Bottom - 2f), new Vector2(2f, 2f), -1.570796f);
            level.ParticlesBG.Emit(HeartGemDoor.P_Shimmer, 1, new Vector2((float) ((double) heartGemDoor.BotSolid.Left + (double) index + 1.0), heartGemDoor.BotSolid.Top + 2f), new Vector2(2f, 2f), 1.570796f);
          }
        }
        yield return (object) null;
      }
      heartGemDoor.TopSolid.MoveToY(topTo);
      heartGemDoor.BotSolid.MoveToY(botTo);
      heartGemDoor.openPercent = 1f;
    }

    public override void Update()
    {
      base.Update();
      if (this.Opened)
        return;
      this.offset += 12f * Engine.DeltaTime;
      ref __Null local1 = ref this.mist.X;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local1 = ^(float&) ref local1 - 4f * Engine.DeltaTime;
      ref __Null local2 = ref this.mist.Y;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local2 = ^(float&) ref local2 - 24f * Engine.DeltaTime;
      for (int index = 0; index < this.particles.Length; ++index)
      {
        ref __Null local3 = ref this.particles[index].Position.Y;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(float&) ref local3 = ^(float&) ref local3 + this.particles[index].Speed * Engine.DeltaTime;
      }
    }

    public void RenderBloom()
    {
      if (this.Opened)
        return;
      this.DrawBloom(new Rectangle((int) this.TopSolid.X, (int) this.TopSolid.Y, this.Size, (int) ((double) this.TopSolid.Height + (double) this.BotSolid.Height)));
    }

    private void DrawBloom(Rectangle bounds)
    {
      Draw.Rect((float) (((Rectangle) ref bounds).get_Left() - 4), (float) ((Rectangle) ref bounds).get_Top(), 2f, (float) bounds.Height, Color.op_Multiply(Color.get_White(), 0.25f));
      Draw.Rect((float) (((Rectangle) ref bounds).get_Left() - 2), (float) ((Rectangle) ref bounds).get_Top(), 2f, (float) bounds.Height, Color.op_Multiply(Color.get_White(), 0.5f));
      Draw.Rect(bounds, Color.op_Multiply(Color.get_White(), 0.75f));
      Draw.Rect((float) ((Rectangle) ref bounds).get_Right(), (float) ((Rectangle) ref bounds).get_Top(), 2f, (float) bounds.Height, Color.op_Multiply(Color.get_White(), 0.5f));
      Draw.Rect((float) (((Rectangle) ref bounds).get_Right() + 2), (float) ((Rectangle) ref bounds).get_Top(), 2f, (float) bounds.Height, Color.op_Multiply(Color.get_White(), 0.25f));
    }

    private void DrawMist(Rectangle bounds, Vector2 mist)
    {
      Color color = Color.op_Multiply(Color.get_White(), 0.6f);
      MTexture mtexture = GFX.Game["objects/heartdoor/mist"];
      int val1_1 = mtexture.Width / 2;
      int val1_2 = mtexture.Height / 2;
      for (int index1 = 0; index1 < bounds.Width; index1 += val1_1)
      {
        for (int index2 = 0; index2 < bounds.Height; index2 += val1_2)
        {
          mtexture.GetSubtexture((int) this.Mod((float) mist.X, (float) val1_1), (int) this.Mod((float) mist.Y, (float) val1_2), Math.Min(val1_1, bounds.Width - index1), Math.Min(val1_2, bounds.Height - index2), this.temp);
          this.temp.Draw(new Vector2((float) (bounds.X + index1), (float) (bounds.Y + index2)), Vector2.get_Zero(), color);
        }
      }
    }

    private void DrawInterior(Rectangle bounds)
    {
      Draw.Rect(bounds, Calc.HexToColor("18668f"));
      this.DrawMist(bounds, this.mist);
      this.DrawMist(bounds, Vector2.op_Multiply(new Vector2((float) this.mist.Y, (float) this.mist.X), 1.5f));
      Vector2 vector2_1 = (this.Scene as Level).Camera.Position;
      if (this.Opened)
        vector2_1 = Vector2.get_Zero();
      for (int index = 0; index < this.particles.Length; ++index)
      {
        Vector2 vector2_2 = Vector2.op_Addition(this.particles[index].Position, Vector2.op_Multiply(vector2_1, 0.2f));
        vector2_2.X = (__Null) (double) this.Mod((float) vector2_2.X, (float) bounds.Width);
        vector2_2.Y = (__Null) (double) this.Mod((float) vector2_2.Y, (float) bounds.Height);
        Draw.Pixel.Draw(Vector2.op_Addition(new Vector2((float) bounds.X, (float) bounds.Y), vector2_2), Vector2.get_Zero(), this.particles[index].Color);
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
        this.temp.DrawJustified(new Vector2((float) (((Rectangle) ref bounds).get_Left() + 4), (float) ((Rectangle) ref bounds).get_Top()), new Vector2(0.5f, 0.0f), color, new Vector2(-1f, 1f));
        this.temp.DrawJustified(new Vector2((float) (((Rectangle) ref bounds).get_Right() - 4), (float) ((Rectangle) ref bounds).get_Top()), new Vector2(0.5f, 0.0f), color, new Vector2(1f, 1f));
      }
      for (int index = 0; index < bounds.Height; index += 8)
      {
        mtexture1.GetSubtexture(0, 0, 8, Math.Min(8, bounds.Height - index), this.temp);
        this.temp.DrawJustified(new Vector2((float) (((Rectangle) ref bounds).get_Left() + 4), (float) (((Rectangle) ref bounds).get_Top() + index + height)), new Vector2(0.5f, 0.0f), color, new Vector2(-1f, 1f));
        this.temp.DrawJustified(new Vector2((float) (((Rectangle) ref bounds).get_Right() - 4), (float) (((Rectangle) ref bounds).get_Top() + index + height)), new Vector2(0.5f, 0.0f), color, new Vector2(1f, 1f));
      }
      if (!this.Opened)
        return;
      for (int index = 0; index < bounds.Width; index += 8)
      {
        mtexture2.DrawCentered(new Vector2((float) (((Rectangle) ref bounds).get_Left() + 4 + index), (float) (((Rectangle) ref bounds).get_Top() + 4)), color);
        mtexture2.DrawCentered(new Vector2((float) (((Rectangle) ref bounds).get_Left() + 4 + index), (float) (((Rectangle) ref bounds).get_Bottom() - 4)), color, new Vector2(1f, -1f));
      }
    }

    public override void Render()
    {
      Color color = this.Opened ? Color.op_Multiply(Color.get_White(), 0.25f) : Color.get_White();
      if (!this.Opened)
      {
        Rectangle bounds;
        ((Rectangle) ref bounds).\u002Ector((int) this.TopSolid.X, (int) this.TopSolid.Y, this.Size, (int) ((double) this.TopSolid.Height + (double) this.BotSolid.Height));
        this.DrawInterior(bounds);
        this.DrawEdges(bounds, color);
      }
      else
      {
        Rectangle bounds1;
        ((Rectangle) ref bounds1).\u002Ector((int) this.TopSolid.X, (int) this.TopSolid.Y, this.Size, (int) this.TopSolid.Height);
        this.DrawInterior(bounds1);
        this.DrawEdges(bounds1, color);
        Rectangle bounds2;
        ((Rectangle) ref bounds2).\u002Ector((int) this.BotSolid.X, (int) this.BotSolid.Y, this.Size, (int) this.BotSolid.Height);
        this.DrawInterior(bounds2);
        this.DrawEdges(bounds2, color);
      }
      float num1 = 12f;
      int num2 = (int) ((double) (this.Size - 8) / (double) num1);
      int num3 = (int) Math.Ceiling((double) this.Requires / (double) num2);
      for (int index1 = 0; index1 < num3; ++index1)
      {
        int num4 = (index1 + 1) * num2 < this.Requires ? num2 : this.Requires - index1 * num2;
        Vector2 vector2 = Vector2.op_Addition(new Vector2(this.X + (float) this.Size * 0.5f, this.Y), Vector2.op_Multiply(new Vector2((float) ((double) -num4 / 2.0 + 0.5), (float) ((double) -num3 / 2.0 + (double) index1 + 0.5)), num1));
        if (this.Opened)
        {
          if (index1 < num3 / 2)
          {
            ref __Null local = ref vector2.Y;
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            ^(float&) ref local = ^(float&) ref local - (this.openAmount + 8f);
          }
          else
          {
            ref __Null local = ref vector2.Y;
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            ^(float&) ref local = ^(float&) ref local + (this.openAmount + 8f);
          }
        }
        for (int index2 = 0; index2 < num4; ++index2)
        {
          int num5 = index1 * num2 + index2;
          this.icon[(int) ((double) Ease.CubeIn(Calc.ClampedMap(this.Counter, (float) num5, (float) num5 + 1f, 0.0f, 1f)) * (double) (this.icon.Count - 1))].DrawCentered(Vector2.op_Addition(vector2, new Vector2((float) index2 * num1, 0.0f)), color);
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
        Draw.Rect((float) (position.X - 10.0), this.Y - height / 2f, 340f, height, Color.get_White());
      }
    }
  }
}
