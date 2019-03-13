// Decompiled with JetBrains decompiler
// Type: Celeste.MainMenuSmallButton
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  public class MainMenuSmallButton : MenuButton
  {
    private const float IconWidth = 64f;
    private const float IconSpacing = 20f;
    private const float MaxLabelWidth = 400f;
    private MTexture icon;
    private string label;
    private float labelScale;
    private Wiggler wiggler;
    private float ease;

    public MainMenuSmallButton(
      string labelName,
      string iconName,
      Oui oui,
      Vector2 targetPosition,
      Vector2 tweenFrom,
      Action onConfirm)
      : base(oui, targetPosition, tweenFrom, onConfirm)
    {
      this.label = Dialog.Clean(labelName, (Language) null);
      this.icon = GFX.Gui[iconName];
      this.labelScale = 1f;
      float x = ActiveFont.Measure(this.label).X;
      if ((double) x > 400.0)
        this.labelScale = 400f / x;
      this.Add((Component) (this.wiggler = Wiggler.Create(0.25f, 4f, (Action<float>) null, false, false)));
    }

    public override void Update()
    {
      base.Update();
      this.ease = Calc.Approach(this.ease, this.Selected ? 1f : 0.0f, 6f * Engine.DeltaTime);
    }

    public override void Render()
    {
      base.Render();
      float scale = 64f / (float) this.icon.Width;
      Vector2 vector2 = new Vector2(Ease.CubeInOut(this.ease) * 32f, (float) ((double) ActiveFont.LineHeight / 2.0 + (double) this.wiggler.Value * 8.0));
      this.icon.DrawOutlineJustified(this.Position + vector2, new Vector2(0.0f, 0.5f), Color.White, scale);
      ActiveFont.DrawOutline(this.label, this.Position + vector2 + new Vector2(84f, 0.0f), new Vector2(0.0f, 0.5f), Vector2.One * this.labelScale, this.SelectionColor, 2f, Color.Black);
    }

    public override void OnSelect()
    {
      this.wiggler.Start();
    }

    public override float ButtonHeight
    {
      get
      {
        return ActiveFont.LineHeight * 1.25f;
      }
    }
  }
}

