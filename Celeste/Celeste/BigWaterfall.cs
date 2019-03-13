// Decompiled with JetBrains decompiler
// Type: Celeste.BigWaterfall
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste
{
  public class BigWaterfall : Entity
  {
    private List<float> lines = new List<float>();
    private float fade = 0.0f;
    private BigWaterfall.Layers layer;
    private float width;
    private float height;
    private float parallax;
    private Color surfaceColor;
    private Color fillColor;
    private float sine;
    private SoundSource loopingSfx;

    private Vector2 RenderPosition
    {
      get
      {
        return this.RenderPositionAtCamera((this.Scene as Level).Camera.Position + new Vector2(160f, 90f));
      }
    }

    public BigWaterfall(EntityData data, Vector2 offset)
      : base(data.Position + offset)
    {
      this.Tag = (int) Tags.TransitionUpdate;
      this.layer = data.Enum<BigWaterfall.Layers>(nameof (layer), BigWaterfall.Layers.BG);
      this.width = (float) data.Width;
      this.height = (float) data.Height;
      if (this.layer == BigWaterfall.Layers.FG)
      {
        this.Depth = -49900;
        this.parallax = (float) (0.100000001490116 + (double) Calc.Random.NextFloat() * 0.200000002980232);
        this.surfaceColor = Water.SurfaceColor;
        this.fillColor = Water.FillColor;
        this.Add((Component) new DisplacementRenderHook(new Action(this.RenderDisplacement)));
        this.lines.Add(3f);
        this.lines.Add(this.width - 4f);
        this.Add((Component) (this.loopingSfx = new SoundSource()));
        this.loopingSfx.Play("event:/env/local/waterfall_big_main", (string) null, 0.0f);
      }
      else
      {
        this.Depth = 10010;
        this.parallax = (float) -(0.699999988079071 + (double) Calc.Random.NextFloat() * 0.200000002980232);
        this.surfaceColor = Calc.HexToColor("89dbf0") * 0.5f;
        this.fillColor = Calc.HexToColor("29a7ea") * 0.3f;
        this.lines.Add(6f);
        this.lines.Add(this.width - 7f);
      }
      this.fade = 1f;
      this.Add((Component) new TransitionListener()
      {
        OnIn = (Action<float>) (f => this.fade = f),
        OnOut = (Action<float>) (f => this.fade = 1f - f)
      });
      if ((double) this.width <= 16.0)
        return;
      int num = Calc.Random.Next((int) ((double) this.width / 16.0));
      for (int index = 0; index < num; ++index)
        this.lines.Add(8f + Calc.Random.NextFloat(this.width - 16f));
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      if (!(this.Scene as Level).Transitioning)
        return;
      this.fade = 0.0f;
    }

    public Vector2 RenderPositionAtCamera(Vector2 camera)
    {
      Vector2 vector2 = this.Position + new Vector2(this.width, this.height) / 2f - camera;
      Vector2 zero = Vector2.Zero;
      if (this.layer == BigWaterfall.Layers.BG)
        zero -= vector2 * 0.6f;
      else if (this.layer == BigWaterfall.Layers.FG)
        zero += vector2 * 0.2f;
      return this.Position + zero;
    }

    public void RenderDisplacement()
    {
      Draw.Rect(this.RenderPosition.X, this.Y, this.width, this.height, new Color(0.5f, 0.5f, 1f, 1f));
    }

    public override void Update()
    {
      this.sine += Engine.DeltaTime;
      if (this.loopingSfx != null)
        this.loopingSfx.Position = new Vector2(this.RenderPosition.X - this.X, Calc.Clamp((this.Scene as Level).Camera.Position.Y + 90f, this.Y, this.height) - this.Y);
      base.Update();
    }

    public override void Render()
    {
      float x = this.RenderPosition.X;
      Color color1 = this.fillColor * this.fade;
      Color color2 = this.surfaceColor * this.fade;
      Draw.Rect(x, this.Y, this.width, this.height, color1);
      if (this.layer == BigWaterfall.Layers.FG)
      {
        Draw.Rect(x - 1f, this.Y, 3f, this.height, color2);
        Draw.Rect((float) ((double) x + (double) this.width - 2.0), this.Y, 3f, this.height, color2);
        foreach (float line in this.lines)
          Draw.Rect(x + line, this.Y, 1f, this.height, color2);
      }
      else
      {
        Vector2 position = (this.Scene as Level).Camera.Position;
        int num1 = 3;
        float num2 = Math.Max(this.Y, (float) Math.Floor((double) position.Y / (double) num1) * (float) num1);
        float num3 = Math.Min(this.Y + this.height, position.Y + 180f);
        for (float y = num2; (double) y < (double) num3; y += (float) num1)
        {
          int num4 = (int) (Math.Sin((double) y / 6.0 - (double) this.sine * 8.0) * 2.0);
          Draw.Rect(x, y, (float) (4 + num4), (float) num1, color2);
          Draw.Rect((float) ((double) x + (double) this.width - 4.0) + (float) num4, y, (float) (4 - num4), (float) num1, color2);
          foreach (float line in this.lines)
            Draw.Rect(x + (float) num4 + line, y, 1f, (float) num1, color2);
        }
      }
    }

    private enum Layers
    {
      FG,
      BG,
    }
  }
}

