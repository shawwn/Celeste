// Decompiled with JetBrains decompiler
// Type: Celeste.AreaCompleteTitle
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste
{
  public class AreaCompleteTitle : Entity
  {
    public float Alpha = 1f;
    private List<AreaCompleteTitle.Letter> letters = new List<AreaCompleteTitle.Letter>();
    private Vector2 origin;
    private float rectangleEase;
    private float scale;

    public AreaCompleteTitle(Vector2 origin, string text, float scale)
    {
      this.origin = origin;
      this.scale = scale;
      Vector2 vector2_1 = ActiveFont.Measure(text) * scale;
      Vector2 vector2_2 = origin + Vector2.UnitY * vector2_1.Y * 0.5f + Vector2.UnitX * vector2_1.X * -0.5f;
      for (int index1 = 0; index1 < text.Length; ++index1)
      {
        char ch = text[index1];
        Vector2 vector2_3 = ActiveFont.Measure(ch.ToString()) * scale;
        if (text[index1] != ' ')
        {
          int index2 = index1;
          ch = text[index1];
          string str = ch.ToString();
          Vector2 position = vector2_2 + Vector2.UnitX * vector2_3.X * 0.5f;
          this.letters.Add(new AreaCompleteTitle.Letter(index2, str, position));
        }
        vector2_2 += Vector2.UnitX * vector2_3.X;
      }
      Alarm.Set((Entity) this, 2.6f, (Action) (() =>
      {
        Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.SineOut, 0.5f, true);
        tween.OnUpdate = (Action<Tween>) (t => this.rectangleEase = t.Eased);
        this.Add((Component) tween);
      }), Alarm.AlarmMode.Oneshot);
    }

    public override void Update()
    {
      base.Update();
      foreach (AreaCompleteTitle.Letter letter in this.letters)
        letter.Update();
    }

    public void DrawLineUI()
    {
      Draw.Rect(this.X, (float) ((double) this.Y + (double) this.origin.Y - 40.0), 1920f * this.rectangleEase, 80f, Color.Black * 0.65f);
    }

    public override void Render()
    {
      base.Render();
      foreach (AreaCompleteTitle.Letter letter in this.letters)
        letter.Render(this.Position, this.scale, this.Alpha);
    }

    public class Letter
    {
      public string Value;
      public Vector2 Position;
      private float delay;
      private float ease;
      private Vector2 scale;
      private SimpleCurve curve;

      public Letter(int index, string value, Vector2 position)
      {
        this.Value = value;
        this.Position = position;
        this.delay = (float) (0.200000002980232 + (double) index * 0.0199999995529652);
        this.curve = new SimpleCurve(position + Vector2.UnitY * 60f, position, position - Vector2.UnitY * 100f);
        this.scale = new Vector2(0.75f, 1.5f);
      }

      public void Update()
      {
        this.scale.X = Calc.Approach(this.scale.X, 1f, 3f * Engine.DeltaTime);
        this.scale.Y = Calc.Approach(this.scale.Y, 1f, 3f * Engine.DeltaTime);
        if ((double) this.delay > 0.0)
        {
          this.delay -= Engine.DeltaTime;
        }
        else
        {
          if ((double) this.ease >= 1.0)
            return;
          this.ease += 4f * Engine.DeltaTime;
          if ((double) this.ease >= 1.0)
          {
            this.ease = 1f;
            this.scale = new Vector2(1.5f, 0.75f);
          }
        }
      }

      public void Render(Vector2 offset, float scale, float alphaMultiplier)
      {
        if ((double) this.ease <= 0.0)
          return;
        Vector2 position = offset + this.curve.GetPoint(this.ease);
        float num = Calc.LerpClamp(0.0f, 1f, this.ease * 3f) * alphaMultiplier;
        Vector2 scale1 = this.scale * scale;
        if ((double) num < 1.0)
        {
          ActiveFont.Draw(this.Value, position, new Vector2(0.5f, 1f), scale1, Color.White * num);
        }
        else
        {
          ActiveFont.Draw(this.Value, position + Vector2.UnitY * 3.5f * scale, new Vector2(0.5f, 1f), scale1, Color.Black);
          ActiveFont.DrawOutline(this.Value, position, new Vector2(0.5f, 1f), scale1, Color.White, 2f, Color.Black);
        }
      }
    }
  }
}

