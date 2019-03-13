// Decompiled with JetBrains decompiler
// Type: Celeste.ZipMover
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
  public class ZipMover : Solid
  {
    private static readonly Color ropeColor = Calc.HexToColor("663931");
    private static readonly Color ropeLightColor = Calc.HexToColor("9b6157");
    private MTexture[,] edges = new MTexture[3, 3];
    private List<MTexture> innerCogs = GFX.Game.GetAtlasSubtextures("objects/zipmover/innercog");
    private MTexture temp = new MTexture();
    private SoundSource sfx = new SoundSource();
    public static ParticleType P_Scrape;
    public static ParticleType P_Sparks;
    private Sprite streetlight;
    private BloomPoint bloom;
    private ZipMover.ZipMoverPathRenderer pathRenderer;
    private Vector2 start;
    private Vector2 target;
    private float percent;

    public ZipMover(Vector2 position, int width, int height, Vector2 target)
      : base(position, (float) width, (float) height, false)
    {
      this.Depth = -9999;
      this.start = this.Position;
      this.target = target;
      this.Add((Component) new Coroutine(this.Sequence(), true));
      this.Add((Component) new LightOcclude(1f));
      this.Add((Component) (this.streetlight = new Sprite(GFX.Game, "objects/zipmover/light")));
      this.streetlight.Add("frames", "", 1f);
      this.streetlight.Play("frames", false, false);
      this.streetlight.Active = false;
      this.streetlight.SetAnimationFrame(1);
      this.streetlight.Position = new Vector2((float) ((double) this.Width / 2.0 - (double) this.streetlight.Width / 2.0), 0.0f);
      this.Add((Component) (this.bloom = new BloomPoint(1f, 6f)));
      this.bloom.Position = new Vector2(this.Width / 2f, 4f);
      for (int index1 = 0; index1 < 3; ++index1)
      {
        for (int index2 = 0; index2 < 3; ++index2)
          this.edges[index1, index2] = GFX.Game["objects/zipmover/block"].GetSubtexture(index1 * 8, index2 * 8, 8, 8, (MTexture) null);
      }
      this.SurfaceSoundIndex = 7;
      this.sfx.Position = Vector2.op_Division(new Vector2(this.Width, this.Height), 2f);
      this.Add((Component) this.sfx);
    }

    public ZipMover(EntityData data, Vector2 offset)
      : this(Vector2.op_Addition(data.Position, offset), data.Width, data.Height, Vector2.op_Addition(data.Nodes[0], offset))
    {
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      scene.Add((Entity) (this.pathRenderer = new ZipMover.ZipMoverPathRenderer(this)));
    }

    public override void Removed(Scene scene)
    {
      scene.Remove((Entity) this.pathRenderer);
      this.pathRenderer = (ZipMover.ZipMoverPathRenderer) null;
      base.Removed(scene);
    }

    public override void Update()
    {
      base.Update();
      this.bloom.Y = (float) (this.streetlight.CurrentAnimationFrame * 3);
    }

    public override void Render()
    {
      Vector2 position = this.Position;
      this.Position = Vector2.op_Addition(this.Position, this.Shake);
      Draw.Rect(this.X, this.Y, this.Width, this.Height, Color.get_Black());
      int num1 = 1;
      float num2 = 0.0f;
      int count = this.innerCogs.Count;
      for (int index1 = 4; (double) index1 <= (double) this.Height - 4.0; index1 += 8)
      {
        int num3 = num1;
        for (int index2 = 4; (double) index2 <= (double) this.Width - 4.0; index2 += 8)
        {
          MTexture innerCog = this.innerCogs[(int) ((double) this.mod((float) (((double) num2 + (double) num1 * (double) this.percent * 3.14159274101257 * 4.0) / 1.57079637050629), 1f) * (double) count)];
          Rectangle rectangle;
          ((Rectangle) ref rectangle).\u002Ector(0, 0, innerCog.Width, innerCog.Height);
          Vector2 zero = Vector2.get_Zero();
          if (index2 <= 4)
          {
            zero.X = (__Null) 2.0;
            rectangle.X = (__Null) 2;
            ref __Null local = ref rectangle.Width;
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            ^(int&) ref local = ^(int&) ref local - 2;
          }
          else if ((double) index2 >= (double) this.Width - 4.0)
          {
            zero.X = (__Null) -2.0;
            ref __Null local = ref rectangle.Width;
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            ^(int&) ref local = ^(int&) ref local - 2;
          }
          if (index1 <= 4)
          {
            zero.Y = (__Null) 2.0;
            rectangle.Y = (__Null) 2;
            ref __Null local = ref rectangle.Height;
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            ^(int&) ref local = ^(int&) ref local - 2;
          }
          else if ((double) index1 >= (double) this.Height - 4.0)
          {
            zero.Y = (__Null) -2.0;
            ref __Null local = ref rectangle.Height;
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            ^(int&) ref local = ^(int&) ref local - 2;
          }
          innerCog.GetSubtexture((int) rectangle.X, (int) rectangle.Y, (int) rectangle.Width, (int) rectangle.Height, this.temp).DrawCentered(Vector2.op_Addition(Vector2.op_Addition(this.Position, new Vector2((float) index2, (float) index1)), zero), Color.op_Multiply(Color.get_White(), num1 < 0 ? 0.5f : 1f));
          num1 = -num1;
          num2 += 1.047198f;
        }
        if (num3 == num1)
          num1 = -num1;
      }
      for (int index1 = 0; (double) index1 < (double) this.Width / 8.0; ++index1)
      {
        for (int index2 = 0; (double) index2 < (double) this.Height / 8.0; ++index2)
        {
          int index3 = index1 == 0 ? 0 : ((double) index1 == (double) this.Width / 8.0 - 1.0 ? 2 : 1);
          int index4 = index2 == 0 ? 0 : ((double) index2 == (double) this.Height / 8.0 - 1.0 ? 2 : 1);
          if (index3 != 1 || index4 != 1)
            this.edges[index3, index4].Draw(new Vector2(this.X + (float) (index1 * 8), this.Y + (float) (index2 * 8)));
        }
      }
      base.Render();
      this.Position = position;
    }

    private void ScrapeParticlesCheck(Vector2 to)
    {
      if (!this.Scene.OnInterval(0.03f))
        return;
      bool flag1 = to.Y != this.ExactPosition.Y;
      bool flag2 = to.X != this.ExactPosition.X;
      if (flag1 && !flag2)
      {
        int num1 = Math.Sign((float) (to.Y - this.ExactPosition.Y));
        Vector2 vector2 = num1 != 1 ? this.TopLeft : this.BottomLeft;
        int num2 = 4;
        if (num1 == 1)
          num2 = Math.Min((int) this.Height - 12, 20);
        int num3 = (int) this.Height;
        if (num1 == -1)
          num3 = Math.Max(16, (int) this.Height - 16);
        if (this.Scene.CollideCheck<Solid>(Vector2.op_Addition(vector2, new Vector2(-2f, (float) (num1 * -2)))))
        {
          for (int index = num2; index < num3; index += 8)
            this.SceneAs<Level>().ParticlesFG.Emit(ZipMover.P_Scrape, Vector2.op_Addition(this.TopLeft, new Vector2(0.0f, (float) index + (float) num1 * 2f)), num1 == 1 ? -0.7853982f : 0.7853982f);
        }
        if (!this.Scene.CollideCheck<Solid>(Vector2.op_Addition(vector2, new Vector2(this.Width + 2f, (float) (num1 * -2)))))
          return;
        for (int index = num2; index < num3; index += 8)
          this.SceneAs<Level>().ParticlesFG.Emit(ZipMover.P_Scrape, Vector2.op_Addition(this.TopRight, new Vector2(-1f, (float) index + (float) num1 * 2f)), num1 == 1 ? -2.356194f : 2.356194f);
      }
      else
      {
        if (!flag2 || flag1)
          return;
        int num1 = Math.Sign((float) (to.X - this.ExactPosition.X));
        Vector2 vector2 = num1 != 1 ? this.TopLeft : this.TopRight;
        int num2 = 4;
        if (num1 == 1)
          num2 = Math.Min((int) this.Width - 12, 20);
        int num3 = (int) this.Width;
        if (num1 == -1)
          num3 = Math.Max(16, (int) this.Width - 16);
        if (this.Scene.CollideCheck<Solid>(Vector2.op_Addition(vector2, new Vector2((float) (num1 * -2), -2f))))
        {
          for (int index = num2; index < num3; index += 8)
            this.SceneAs<Level>().ParticlesFG.Emit(ZipMover.P_Scrape, Vector2.op_Addition(this.TopLeft, new Vector2((float) index + (float) num1 * 2f, -1f)), num1 == 1 ? 2.356194f : 0.7853982f);
        }
        if (!this.Scene.CollideCheck<Solid>(Vector2.op_Addition(vector2, new Vector2((float) (num1 * -2), this.Height + 2f))))
          return;
        for (int index = num2; index < num3; index += 8)
          this.SceneAs<Level>().ParticlesFG.Emit(ZipMover.P_Scrape, Vector2.op_Addition(this.BottomLeft, new Vector2((float) index + (float) num1 * 2f, 0.0f)), num1 == 1 ? -2.356194f : -0.7853982f);
      }
    }

    private IEnumerator Sequence()
    {
      ZipMover zipMover = this;
      Vector2 start = zipMover.Position;
      while (true)
      {
        while (!zipMover.HasPlayerRider())
          yield return (object) null;
        zipMover.sfx.Play("event:/game/01_forsaken_city/zip_mover", (string) null, 0.0f);
        Input.Rumble(RumbleStrength.Medium, RumbleLength.Short);
        zipMover.StartShaking(0.1f);
        yield return (object) 0.1f;
        zipMover.streetlight.SetAnimationFrame(3);
        zipMover.StopPlayerRunIntoAnimation = false;
        float at = 0.0f;
        while ((double) at < 1.0)
        {
          yield return (object) null;
          at = Calc.Approach(at, 1f, 2f * Engine.DeltaTime);
          zipMover.percent = Ease.SineIn(at);
          Vector2 vector2 = Vector2.Lerp(start, zipMover.target, zipMover.percent);
          zipMover.ScrapeParticlesCheck(vector2);
          if (zipMover.Scene.OnInterval(0.1f))
            zipMover.pathRenderer.CreateSparks();
          zipMover.MoveTo(vector2);
        }
        zipMover.StartShaking(0.2f);
        Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
        zipMover.SceneAs<Level>().Shake(0.3f);
        zipMover.StopPlayerRunIntoAnimation = true;
        yield return (object) 0.5f;
        zipMover.StopPlayerRunIntoAnimation = false;
        zipMover.streetlight.SetAnimationFrame(2);
        at = 0.0f;
        while ((double) at < 1.0)
        {
          yield return (object) null;
          at = Calc.Approach(at, 1f, 0.5f * Engine.DeltaTime);
          zipMover.percent = 1f - Ease.SineIn(at);
          Vector2 position = Vector2.Lerp(zipMover.target, start, Ease.SineIn(at));
          zipMover.MoveTo(position);
        }
        zipMover.StopPlayerRunIntoAnimation = true;
        zipMover.StartShaking(0.2f);
        zipMover.streetlight.SetAnimationFrame(1);
        yield return (object) 0.5f;
      }
    }

    private float mod(float x, float m)
    {
      return (x % m + m) % m;
    }

    private class ZipMoverPathRenderer : Entity
    {
      private MTexture cog = GFX.Game["objects/zipmover/cog"];
      public ZipMover ZipMover;
      private Vector2 from;
      private Vector2 to;
      private Vector2 sparkAdd;
      private float sparkDirFromA;
      private float sparkDirFromB;
      private float sparkDirToA;
      private float sparkDirToB;

      public ZipMoverPathRenderer(ZipMover zipMover)
      {
        this.Depth = 5000;
        this.ZipMover = zipMover;
        this.from = Vector2.op_Addition(this.ZipMover.start, new Vector2(this.ZipMover.Width / 2f, this.ZipMover.Height / 2f));
        this.to = Vector2.op_Addition(this.ZipMover.target, new Vector2(this.ZipMover.Width / 2f, this.ZipMover.Height / 2f));
        this.sparkAdd = Vector2.op_Subtraction(this.from, this.to).SafeNormalize(5f).Perpendicular();
        float num = Vector2.op_Subtraction(this.from, this.to).Angle();
        this.sparkDirFromA = num + 0.3926991f;
        this.sparkDirFromB = num - 0.3926991f;
        this.sparkDirToA = (float) ((double) num + 3.14159274101257 - 0.392699092626572);
        this.sparkDirToB = (float) ((double) num + 3.14159274101257 + 0.392699092626572);
      }

      public void CreateSparks()
      {
        this.SceneAs<Level>().ParticlesBG.Emit(ZipMover.P_Sparks, Vector2.op_Addition(Vector2.op_Addition(this.from, this.sparkAdd), Calc.Random.Range(Vector2.op_UnaryNegation(Vector2.get_One()), Vector2.get_One())), this.sparkDirFromA);
        this.SceneAs<Level>().ParticlesBG.Emit(ZipMover.P_Sparks, Vector2.op_Addition(Vector2.op_Subtraction(this.from, this.sparkAdd), Calc.Random.Range(Vector2.op_UnaryNegation(Vector2.get_One()), Vector2.get_One())), this.sparkDirFromB);
        this.SceneAs<Level>().ParticlesBG.Emit(ZipMover.P_Sparks, Vector2.op_Addition(Vector2.op_Addition(this.to, this.sparkAdd), Calc.Random.Range(Vector2.op_UnaryNegation(Vector2.get_One()), Vector2.get_One())), this.sparkDirToA);
        this.SceneAs<Level>().ParticlesBG.Emit(ZipMover.P_Sparks, Vector2.op_Addition(Vector2.op_Subtraction(this.to, this.sparkAdd), Calc.Random.Range(Vector2.op_UnaryNegation(Vector2.get_One()), Vector2.get_One())), this.sparkDirToB);
      }

      public override void Render()
      {
        this.DrawCogs(Vector2.get_UnitY(), new Color?(Color.get_Black()));
        this.DrawCogs(Vector2.get_Zero(), new Color?());
        Draw.Rect(new Rectangle((int) ((double) this.ZipMover.X - 1.0), (int) ((double) this.ZipMover.Y - 1.0), (int) this.ZipMover.Width + 2, (int) this.ZipMover.Height + 2), Color.get_Black());
      }

      private void DrawCogs(Vector2 offset, Color? colorOverride = null)
      {
        Vector2 vector = Vector2.op_Subtraction(this.to, this.from).SafeNormalize();
        Vector2 vector2_1 = Vector2.op_Multiply(vector.Perpendicular(), 3f);
        Vector2 vector2_2 = Vector2.op_Multiply(Vector2.op_UnaryNegation(vector.Perpendicular()), 4f);
        float rotation = (float) ((double) this.ZipMover.percent * 3.14159274101257 * 2.0);
        Draw.Line(Vector2.op_Addition(Vector2.op_Addition(this.from, vector2_1), offset), Vector2.op_Addition(Vector2.op_Addition(this.to, vector2_1), offset), colorOverride.HasValue ? colorOverride.Value : ZipMover.ropeColor);
        Draw.Line(Vector2.op_Addition(Vector2.op_Addition(this.from, vector2_2), offset), Vector2.op_Addition(Vector2.op_Addition(this.to, vector2_2), offset), colorOverride.HasValue ? colorOverride.Value : ZipMover.ropeColor);
        float num1 = (float) (4.0 - (double) this.ZipMover.percent * 3.14159274101257 * 8.0 % 4.0);
        while (true)
        {
          double num2 = (double) num1;
          Vector2 vector2_3 = Vector2.op_Subtraction(this.to, this.from);
          double num3 = (double) ((Vector2) ref vector2_3).Length();
          if (num2 < num3)
          {
            Vector2 vector2_4 = Vector2.op_Addition(Vector2.op_Addition(Vector2.op_Addition(this.from, vector2_1), vector.Perpendicular()), Vector2.op_Multiply(vector, num1));
            Vector2 vector2_5 = Vector2.op_Subtraction(Vector2.op_Addition(this.to, vector2_2), Vector2.op_Multiply(vector, num1));
            Draw.Line(Vector2.op_Addition(vector2_4, offset), Vector2.op_Addition(Vector2.op_Addition(vector2_4, Vector2.op_Multiply(vector, 2f)), offset), colorOverride.HasValue ? colorOverride.Value : ZipMover.ropeLightColor);
            Draw.Line(Vector2.op_Addition(vector2_5, offset), Vector2.op_Addition(Vector2.op_Subtraction(vector2_5, Vector2.op_Multiply(vector, 2f)), offset), colorOverride.HasValue ? colorOverride.Value : ZipMover.ropeLightColor);
            num1 += 4f;
          }
          else
            break;
        }
        this.cog.DrawCentered(Vector2.op_Addition(this.from, offset), colorOverride.HasValue ? colorOverride.Value : Color.get_White(), 1f, rotation);
        this.cog.DrawCentered(Vector2.op_Addition(this.to, offset), colorOverride.HasValue ? colorOverride.Value : Color.get_White(), 1f, rotation);
      }
    }
  }
}
