// Decompiled with JetBrains decompiler
// Type: Celeste.StrawberriesCounter
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  public class StrawberriesCounter : Component
  {
    public static readonly Color FlashColor = Calc.HexToColor("FF5E76");
    public bool CanWiggle = true;
    public float Scale = 1f;
    public float Stroke = 2f;
    public Color Color = Color.get_White();
    public Color OutOfColor = Color.get_LightGray();
    private int outOf = -1;
    private const int IconWidth = 60;
    public bool Golden;
    public Vector2 Position;
    public bool CenteredX;
    public float Rotation;
    public bool OverworldSfx;
    private int amount;
    private Wiggler wiggler;
    private float flashTimer;
    private string sAmount;
    private string sOutOf;
    private MTexture x;
    private bool showOutOf;

    public StrawberriesCounter(bool centeredX, int amount, int outOf = 0, bool showOutOf = false)
      : base(true, true)
    {
      this.CenteredX = centeredX;
      this.amount = amount;
      this.outOf = outOf;
      this.showOutOf = showOutOf;
      this.UpdateStrings();
      this.wiggler = Wiggler.Create(0.5f, 3f, (Action<float>) null, false, false);
      this.wiggler.StartZero = true;
      this.wiggler.UseRawDeltaTime = true;
      this.x = GFX.Gui[nameof (x)];
    }

    public int Amount
    {
      get
      {
        return this.amount;
      }
      set
      {
        if (this.amount == value)
          return;
        this.amount = value;
        this.UpdateStrings();
        if (!this.CanWiggle)
          return;
        if (this.OverworldSfx)
          Audio.Play(this.Golden ? "event:/ui/postgame/goldberry_count" : "event:/ui/postgame/strawberry_count");
        else
          Audio.Play("event:/ui/game/increment_strawberry");
        this.wiggler.Start();
        this.flashTimer = 0.5f;
      }
    }

    public int OutOf
    {
      get
      {
        return this.outOf;
      }
      set
      {
        this.outOf = value;
        this.UpdateStrings();
      }
    }

    public bool ShowOutOf
    {
      get
      {
        return this.showOutOf;
      }
      set
      {
        if (this.showOutOf == value)
          return;
        this.showOutOf = value;
        this.UpdateStrings();
      }
    }

    public float FullHeight
    {
      get
      {
        return Math.Max(ActiveFont.LineHeight, (float) GFX.Gui["collectables/strawberry"].Height);
      }
    }

    private void UpdateStrings()
    {
      this.sAmount = this.amount.ToString();
      if (this.outOf > -1)
        this.sOutOf = "/" + this.outOf.ToString();
      else
        this.sOutOf = "";
    }

    public void Wiggle()
    {
      this.wiggler.Start();
      this.flashTimer = 0.5f;
    }

    public override void Update()
    {
      base.Update();
      if (this.wiggler.Active)
        this.wiggler.Update();
      if ((double) this.flashTimer <= 0.0)
        return;
      this.flashTimer -= Engine.RawDeltaTime;
    }

    public override void Render()
    {
      Vector2 vector2_1 = this.RenderPosition;
      Vector2 vector = Calc.AngleToVector(this.Rotation, 1f);
      Vector2 vector2_2;
      ((Vector2) ref vector2_2).\u002Ector((float) -vector.Y, (float) vector.X);
      string text = this.showOutOf ? this.sOutOf : "";
      float x1 = (float) ActiveFont.Measure(this.sAmount).X;
      float x2 = (float) ActiveFont.Measure(text).X;
      float num = (float) (62.0 + (double) this.x.Width + 2.0) + x1 + x2;
      Color color = this.Color;
      if ((double) this.flashTimer > 0.0 && this.Scene != null && this.Scene.BetweenRawInterval(0.05f))
        color = StrawberriesCounter.FlashColor;
      if (this.CenteredX)
        vector2_1 = Vector2.op_Subtraction(vector2_1, Vector2.op_Multiply(Vector2.op_Multiply(vector, num / 2f), this.Scale));
      string index = this.Golden ? "collectables/goldberry" : "collectables/strawberry";
      GFX.Gui[index].DrawCentered(Vector2.op_Addition(vector2_1, Vector2.op_Multiply(Vector2.op_Multiply(Vector2.op_Multiply(vector, 60f), 0.5f), this.Scale)), Color.get_White(), this.Scale);
      this.x.DrawCentered(Vector2.op_Addition(Vector2.op_Addition(vector2_1, Vector2.op_Multiply(Vector2.op_Multiply(vector, (float) (62.0 + (double) this.x.Width * 0.5)), this.Scale)), Vector2.op_Multiply(Vector2.op_Multiply(vector2_2, 2f), this.Scale)), color, this.Scale);
      ActiveFont.DrawOutline(this.sAmount, Vector2.op_Addition(Vector2.op_Addition(vector2_1, Vector2.op_Multiply(Vector2.op_Multiply(vector, (float) ((double) num - (double) x2 - (double) x1 * 0.5)), this.Scale)), Vector2.op_Multiply(Vector2.op_Multiply(vector2_2, this.wiggler.Value * 18f), this.Scale)), new Vector2(0.5f, 0.5f), Vector2.op_Multiply(Vector2.get_One(), this.Scale), color, this.Stroke, Color.get_Black());
      if (!(text != ""))
        return;
      ActiveFont.DrawOutline(text, Vector2.op_Addition(vector2_1, Vector2.op_Multiply(Vector2.op_Multiply(vector, num - x2 / 2f), this.Scale)), new Vector2(0.5f, 0.5f), Vector2.op_Multiply(Vector2.get_One(), this.Scale), this.OutOfColor, this.Stroke, Color.get_Black());
    }

    public Vector2 RenderPosition
    {
      get
      {
        return Vector2.op_Addition(this.Entity != null ? this.Entity.Position : Vector2.get_Zero(), this.Position).Round();
      }
    }
  }
}
