// Decompiled with JetBrains decompiler
// Type: Celeste.DeathsCounter
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  public class DeathsCounter : Component
  {
    public bool CanWiggle = true;
    public float Alpha = 1f;
    public float Scale = 1f;
    public float Stroke = 2f;
    public Color Color = Color.get_White();
    private const int IconWidth = 60;
    public Vector2 Position;
    public bool CenteredX;
    private int amount;
    private int minDigits;
    private Wiggler wiggler;
    private Wiggler iconWiggler;
    private float flashTimer;
    private string sAmount;
    private MTexture icon;
    private MTexture x;

    public DeathsCounter(AreaMode mode, bool centeredX, int amount, int minDigits = 0)
      : base(true, true)
    {
      this.CenteredX = centeredX;
      this.amount = amount;
      this.minDigits = minDigits;
      this.UpdateString();
      this.wiggler = Wiggler.Create(0.5f, 3f, (Action<float>) null, false, false);
      this.wiggler.StartZero = true;
      this.wiggler.UseRawDeltaTime = true;
      this.iconWiggler = Wiggler.Create(0.5f, 3f, (Action<float>) null, false, false);
      this.iconWiggler.UseRawDeltaTime = true;
      this.SetMode(mode);
      this.x = GFX.Gui[nameof (x)];
    }

    private void UpdateString()
    {
      if (this.minDigits > 0)
        this.sAmount = this.amount.ToString("D" + (object) this.minDigits);
      else
        this.sAmount = this.amount.ToString();
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
        this.UpdateString();
        if (!this.CanWiggle)
          return;
        this.wiggler.Start();
        this.flashTimer = 0.5f;
      }
    }

    public int MinDigits
    {
      get
      {
        return this.minDigits;
      }
      set
      {
        if (this.minDigits == value)
          return;
        this.minDigits = value;
        this.UpdateString();
      }
    }

    public void SetMode(AreaMode mode)
    {
      switch (mode)
      {
        case AreaMode.Normal:
          this.icon = GFX.Gui["collectables/skullBlue"];
          break;
        case AreaMode.BSide:
          this.icon = GFX.Gui["collectables/skullRed"];
          break;
        default:
          this.icon = GFX.Gui["collectables/skullGold"];
          break;
      }
      this.iconWiggler.Start();
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
      if (this.iconWiggler.Active)
        this.iconWiggler.Update();
      if ((double) this.flashTimer <= 0.0)
        return;
      this.flashTimer -= Engine.RawDeltaTime;
    }

    public override void Render()
    {
      Vector2 vector2 = this.RenderPosition;
      float x = (float) ActiveFont.Measure(this.sAmount).X;
      float num = (float) (62.0 + (double) this.x.Width + 2.0) + x;
      Color color = this.Color;
      Color strokeColor = Color.get_Black();
      if ((double) this.flashTimer > 0.0 && this.Scene != null && this.Scene.BetweenRawInterval(0.05f))
        color = StrawberriesCounter.FlashColor;
      if ((double) this.Alpha < 1.0)
      {
        color = Color.op_Multiply(color, this.Alpha);
        strokeColor = Color.op_Multiply(strokeColor, this.Alpha);
      }
      if (this.CenteredX)
        vector2 = Vector2.op_Subtraction(vector2, Vector2.op_Multiply(Vector2.op_Multiply(Vector2.get_UnitX(), num / 2f), this.Scale));
      this.icon.DrawCentered(Vector2.op_Addition(vector2, Vector2.op_Multiply(new Vector2(30f, 0.0f), this.Scale)), Color.op_Multiply(Color.get_White(), this.Alpha), this.Scale * (float) (1.0 + (double) this.iconWiggler.Value * 0.200000002980232));
      this.x.DrawCentered(Vector2.op_Addition(vector2, Vector2.op_Multiply(new Vector2(62f + (float) (this.x.Width / 2), 2f), this.Scale)), color, this.Scale);
      ActiveFont.DrawOutline(this.sAmount, Vector2.op_Addition(vector2, Vector2.op_Multiply(new Vector2(num - x / 2f, (float) (-(double) this.wiggler.Value * 18.0)), this.Scale)), new Vector2(0.5f, 0.5f), Vector2.op_Multiply(Vector2.get_One(), this.Scale), color, this.Stroke, strokeColor);
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
