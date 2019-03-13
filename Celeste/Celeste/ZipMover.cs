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
    private float percent = 0.0f;
    private SoundSource sfx = new SoundSource();
    public static ParticleType P_Scrape;
    public static ParticleType P_Sparks;
    private Sprite streetlight;
    private BloomPoint bloom;
    private ZipMover.ZipMoverPathRenderer pathRenderer;
    private Vector2 start;
    private Vector2 target;

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
      this.sfx.Position = new Vector2(this.Width, this.Height) / 2f;
      this.Add((Component) this.sfx);
    }

    public ZipMover(EntityData data, Vector2 offset)
      : this(data.Position + offset, data.Width, data.Height, data.Nodes[0] + offset)
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
      this.Position = this.Position + this.Shake;
      Draw.Rect(this.X, this.Y, this.Width, this.Height, Color.Black);
      int num1 = 1;
      float num2 = 0.0f;
      int count = this.innerCogs.Count;
      for (int index1 = 4; (double) index1 <= (double) this.Height - 4.0; index1 += 8)
      {
        int num3 = num1;
        for (int index2 = 4; (double) index2 <= (double) this.Width - 4.0; index2 += 8)
        {
          MTexture innerCog = this.innerCogs[(int) ((double) this.mod((float) (((double) num2 + (double) num1 * (double) this.percent * 3.14159274101257 * 4.0) / 1.57079637050629), 1f) * (double) count)];
          Rectangle rectangle = new Rectangle(0, 0, innerCog.Width, innerCog.Height);
          Vector2 zero = Vector2.Zero;
          if (index2 <= 4)
          {
            zero.X = 2f;
            rectangle.X = 2;
            rectangle.Width -= 2;
          }
          else if ((double) index2 >= (double) this.Width - 4.0)
          {
            zero.X = -2f;
            rectangle.Width -= 2;
          }
          if (index1 <= 4)
          {
            zero.Y = 2f;
            rectangle.Y = 2;
            rectangle.Height -= 2;
          }
          else if ((double) index1 >= (double) this.Height - 4.0)
          {
            zero.Y = -2f;
            rectangle.Height -= 2;
          }
          innerCog.GetSubtexture(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, this.temp).DrawCentered(this.Position + new Vector2((float) index2, (float) index1) + zero, Color.White * (num1 < 0 ? 0.5f : 1f));
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
      bool flag1 = (double) to.Y != (double) this.ExactPosition.Y;
      bool flag2 = (double) to.X != (double) this.ExactPosition.X;
      if (flag1 && !flag2)
      {
        int num1 = Math.Sign(to.Y - this.ExactPosition.Y);
        Vector2 vector2 = num1 != 1 ? this.TopLeft : this.BottomLeft;
        int num2 = 4;
        if (num1 == 1)
          num2 = Math.Min((int) this.Height - 12, 20);
        int num3 = (int) this.Height;
        if (num1 == -1)
          num3 = Math.Max(16, (int) this.Height - 16);
        if (this.Scene.CollideCheck<Solid>(vector2 + new Vector2(-2f, (float) (num1 * -2))))
        {
          for (int index = num2; index < num3; index += 8)
            this.SceneAs<Level>().ParticlesFG.Emit(ZipMover.P_Scrape, this.TopLeft + new Vector2(0.0f, (float) index + (float) num1 * 2f), num1 == 1 ? -0.7853982f : 0.7853982f);
        }
        if (this.Scene.CollideCheck<Solid>(vector2 + new Vector2(this.Width + 2f, (float) (num1 * -2))))
        {
          for (int index = num2; index < num3; index += 8)
            this.SceneAs<Level>().ParticlesFG.Emit(ZipMover.P_Scrape, this.TopRight + new Vector2(-1f, (float) index + (float) num1 * 2f), num1 == 1 ? -2.356194f : 2.356194f);
        }
      }
      else if (flag2 && !flag1)
      {
        int num1 = Math.Sign(to.X - this.ExactPosition.X);
        Vector2 vector2 = num1 != 1 ? this.TopLeft : this.TopRight;
        int num2 = 4;
        if (num1 == 1)
          num2 = Math.Min((int) this.Width - 12, 20);
        int num3 = (int) this.Width;
        if (num1 == -1)
          num3 = Math.Max(16, (int) this.Width - 16);
        if (this.Scene.CollideCheck<Solid>(vector2 + new Vector2((float) (num1 * -2), -2f)))
        {
          for (int index = num2; index < num3; index += 8)
            this.SceneAs<Level>().ParticlesFG.Emit(ZipMover.P_Scrape, this.TopLeft + new Vector2((float) index + (float) num1 * 2f, -1f), num1 == 1 ? 2.356194f : 0.7853982f);
        }
        if (this.Scene.CollideCheck<Solid>(vector2 + new Vector2((float) (num1 * -2), this.Height + 2f)))
        {
          for (int index = num2; index < num3; index += 8)
            this.SceneAs<Level>().ParticlesFG.Emit(ZipMover.P_Scrape, this.BottomLeft + new Vector2((float) index + (float) num1 * 2f, 0.0f), num1 == 1 ? -2.356194f : -0.7853982f);
        }
      }
    }

    private IEnumerator Sequence()
    {
      Vector2 start = this.Position;
      while (true)
      {
        while (!this.HasPlayerRider())
          yield return (object) null;
        this.sfx.Play("event:/game/01_forsaken_city/zip_mover", (string) null, 0.0f);
        Input.Rumble(RumbleStrength.Medium, RumbleLength.Short);
        this.StartShaking(0.1f);
        yield return (object) 0.1f;
        this.streetlight.SetAnimationFrame(3);
        this.StopPlayerRunIntoAnimation = false;
        float at1 = 0.0f;
        while ((double) at1 < 1.0)
        {
          yield return (object) null;
          at1 = Calc.Approach(at1, 1f, 2f * Engine.DeltaTime);
          this.percent = Ease.SineIn(at1);
          Vector2 to = Vector2.Lerp(start, this.target, this.percent);
          this.ScrapeParticlesCheck(to);
          if (this.Scene.OnInterval(0.1f))
            this.pathRenderer.CreateSparks();
          this.MoveTo(to);
          to = new Vector2();
        }
        this.StartShaking(0.2f);
        Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
        this.SceneAs<Level>().Shake(0.3f);
        this.StopPlayerRunIntoAnimation = true;
        yield return (object) 0.5f;
        this.StopPlayerRunIntoAnimation = false;
        this.streetlight.SetAnimationFrame(2);
        float at2 = 0.0f;
        while ((double) at2 < 1.0)
        {
          yield return (object) null;
          at2 = Calc.Approach(at2, 1f, 0.5f * Engine.DeltaTime);
          this.percent = 1f - Ease.SineIn(at2);
          Vector2 to = Vector2.Lerp(this.target, start, Ease.SineIn(at2));
          this.MoveTo(to);
          to = new Vector2();
        }
        this.StopPlayerRunIntoAnimation = true;
        this.StartShaking(0.2f);
        this.streetlight.SetAnimationFrame(1);
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
        this.from = this.ZipMover.start + new Vector2(this.ZipMover.Width / 2f, this.ZipMover.Height / 2f);
        this.to = this.ZipMover.target + new Vector2(this.ZipMover.Width / 2f, this.ZipMover.Height / 2f);
        this.sparkAdd = (this.from - this.to).SafeNormalize(5f).Perpendicular();
        float num = (this.from - this.to).Angle();
        this.sparkDirFromA = num + 0.3926991f;
        this.sparkDirFromB = num - 0.3926991f;
        this.sparkDirToA = (float) ((double) num + 3.14159274101257 - 0.392699092626572);
        this.sparkDirToB = (float) ((double) num + 3.14159274101257 + 0.392699092626572);
      }

      public void CreateSparks()
      {
        this.SceneAs<Level>().ParticlesBG.Emit(ZipMover.P_Sparks, this.from + this.sparkAdd + Calc.Random.Range(-Vector2.One, Vector2.One), this.sparkDirFromA);
        this.SceneAs<Level>().ParticlesBG.Emit(ZipMover.P_Sparks, this.from - this.sparkAdd + Calc.Random.Range(-Vector2.One, Vector2.One), this.sparkDirFromB);
        this.SceneAs<Level>().ParticlesBG.Emit(ZipMover.P_Sparks, this.to + this.sparkAdd + Calc.Random.Range(-Vector2.One, Vector2.One), this.sparkDirToA);
        this.SceneAs<Level>().ParticlesBG.Emit(ZipMover.P_Sparks, this.to - this.sparkAdd + Calc.Random.Range(-Vector2.One, Vector2.One), this.sparkDirToB);
      }

      public override void Render()
      {
        this.DrawCogs(Vector2.UnitY, new Color?(Color.Black));
        this.DrawCogs(Vector2.Zero, new Color?());
        Draw.Rect(new Rectangle((int) ((double) this.ZipMover.X - 1.0), (int) ((double) this.ZipMover.Y - 1.0), (int) this.ZipMover.Width + 2, (int) this.ZipMover.Height + 2), Color.Black);
      }

      private void DrawCogs(Vector2 offset, Color? colorOverride = null)
      {
        Vector2 vector = (this.to - this.from).SafeNormalize();
        Vector2 vector2_1 = vector.Perpendicular() * 3f;
        Vector2 vector2_2 = -vector.Perpendicular() * 4f;
        float rotation = (float) ((double) this.ZipMover.percent * 3.14159274101257 * 2.0);
        Draw.Line(this.from + vector2_1 + offset, this.to + vector2_1 + offset, colorOverride.HasValue ? colorOverride.Value : ZipMover.ropeColor);
        Draw.Line(this.from + vector2_2 + offset, this.to + vector2_2 + offset, colorOverride.HasValue ? colorOverride.Value : ZipMover.ropeColor);
        for (float num = (float) (4.0 - (double) this.ZipMover.percent * 3.14159274101257 * 8.0 % 4.0); (double) num < (double) (this.to - this.from).Length(); num += 4f)
        {
          Vector2 vector2_3 = this.from + vector2_1 + vector.Perpendicular() + vector * num;
          Vector2 vector2_4 = this.to + vector2_2 - vector * num;
          Draw.Line(vector2_3 + offset, vector2_3 + vector * 2f + offset, colorOverride.HasValue ? colorOverride.Value : ZipMover.ropeLightColor);
          Draw.Line(vector2_4 + offset, vector2_4 - vector * 2f + offset, colorOverride.HasValue ? colorOverride.Value : ZipMover.ropeLightColor);
        }
        this.cog.DrawCentered(this.from + offset, colorOverride.HasValue ? colorOverride.Value : Color.White, 1f, rotation);
        this.cog.DrawCentered(this.to + offset, colorOverride.HasValue ? colorOverride.Value : Color.White, 1f, rotation);
      }
    }
  }
}

