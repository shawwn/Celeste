// Decompiled with JetBrains decompiler
// Type: Celeste.BestTimeDisplay
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  public class BestTimeDisplay : Component
  {
    private static readonly Color IconColor = Color.Lerp(Calc.HexToColor("7CFF70"), Color.Black, 0.25f);
    private static readonly Color FullClearColor = Color.Lerp(Calc.HexToColor("FF3D57"), Color.Black, 0.25f);
    public Vector2 Position;
    private TimeSpan time;
    private string sTime;
    private Wiggler wiggler;
    private MTexture icon;
    private float flashTimer;
    private Color iconColor;

    public BestTimeDisplay(BestTimeDisplay.Modes mode, TimeSpan time)
      : base(true, true)
    {
      this.time = time;
      this.UpdateString();
      this.wiggler = Wiggler.Create(0.5f, 3f, (Action<float>) null, false, false);
      this.wiggler.UseRawDeltaTime = true;
      switch (mode)
      {
        case BestTimeDisplay.Modes.BestFullClear:
          this.icon = GFX.Game["gui/bestFullClearTime"];
          this.iconColor = BestTimeDisplay.FullClearColor;
          break;
        case BestTimeDisplay.Modes.Current:
          this.icon = (MTexture) null;
          break;
        default:
          this.icon = GFX.Game["gui/bestTime"];
          this.iconColor = BestTimeDisplay.IconColor;
          break;
      }
    }

    private void UpdateString()
    {
      this.sTime = this.time.ShortGameplayFormat();
    }

    public void Wiggle()
    {
      this.wiggler.Start();
      this.flashTimer = 0.5f;
    }

    public TimeSpan Time
    {
      get
      {
        return this.time;
      }
      set
      {
        if (!(this.time != value))
          return;
        this.time = value;
        this.UpdateString();
        this.wiggler.Start();
        this.flashTimer = 0.5f;
      }
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
      if (!this.WillRender)
        return;
      Vector2 vector2 = this.RenderPosition - Vector2.UnitY * this.wiggler.Value * 3f;
      Color color = Color.White;
      if ((double) this.flashTimer > 0.0 && this.Scene.BetweenRawInterval(0.05f))
        color = StrawberriesCounter.FlashColor;
      if (this.icon != null)
        this.icon.DrawOutlineCentered(vector2 + new Vector2(-4f, -3f), this.iconColor);
      ActiveFont.DrawOutline(this.sTime, vector2 + new Vector2(0.0f, 4f), new Vector2(0.5f, 0.0f), Vector2.One, color, 2f, Color.Black);
    }

    public Vector2 RenderPosition
    {
      get
      {
        return (this.Entity.Position + this.Position).Round();
      }
    }

    public bool WillRender
    {
      get
      {
        return this.time > TimeSpan.Zero;
      }
    }

    public enum Modes
    {
      Best,
      BestFullClear,
      Current,
    }
  }
}

