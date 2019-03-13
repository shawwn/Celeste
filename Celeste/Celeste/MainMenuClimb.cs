// Decompiled with JetBrains decompiler
// Type: Celeste.MainMenuClimb
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  public class MainMenuClimb : MenuButton
  {
    private const float MaxLabelWidth = 256f;
    private const float BaseLabelScale = 1.5f;
    private string label;
    private MTexture icon;
    private float labelScale;
    private Wiggler bounceWiggler;
    private Wiggler rotateWiggler;
    private Wiggler bigBounceWiggler;
    private bool confirmed;

    public MainMenuClimb(Oui oui, Vector2 targetPosition, Vector2 tweenFrom, Action onConfirm)
      : base(oui, targetPosition, tweenFrom, onConfirm)
    {
      this.label = Dialog.Clean("menu_begin", (Language) null);
      this.icon = GFX.Gui["menu/start"];
      this.labelScale = 1f;
      float num = (float) (ActiveFont.Measure(this.label).X * 1.5);
      if ((double) num > 256.0)
        this.labelScale = 256f / num;
      this.Add((Component) (this.bounceWiggler = Wiggler.Create(0.25f, 4f, (Action<float>) null, false, false)));
      this.Add((Component) (this.rotateWiggler = Wiggler.Create(0.3f, 6f, (Action<float>) null, false, false)));
      this.Add((Component) (this.bigBounceWiggler = Wiggler.Create(0.4f, 2f, (Action<float>) null, false, false)));
    }

    public override void OnSelect()
    {
      this.confirmed = false;
      this.bounceWiggler.Start();
    }

    public override void Confirm()
    {
      base.Confirm();
      this.confirmed = true;
      this.bounceWiggler.Start();
      this.bigBounceWiggler.Start();
      this.rotateWiggler.Start();
      Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
    }

    public override void Render()
    {
      Vector2 vector2_1;
      ((Vector2) ref vector2_1).\u002Ector(0.0f, this.bounceWiggler.Value * 8f);
      Vector2 vector2_2 = Vector2.op_Addition(Vector2.op_Multiply(Vector2.get_UnitY(), (float) this.icon.Height), new Vector2(0.0f, -Math.Abs(this.bigBounceWiggler.Value * 40f)));
      if (!this.confirmed)
        vector2_2 = Vector2.op_Addition(vector2_2, vector2_1);
      this.icon.DrawOutlineJustified(Vector2.op_Addition(this.Position, vector2_2), new Vector2(0.5f, 1f), Color.get_White(), 1f, (float) ((double) this.rotateWiggler.Value * 10.0 * (Math.PI / 180.0)));
      ActiveFont.DrawOutline(this.label, Vector2.op_Addition(Vector2.op_Addition(this.Position, vector2_1), new Vector2(0.0f, (float) (48 + this.icon.Height))), new Vector2(0.5f, 0.5f), Vector2.op_Multiply(Vector2.op_Multiply(Vector2.get_One(), 1.5f), this.labelScale), this.SelectionColor, 2f, Color.get_Black());
    }

    public override float ButtonHeight
    {
      get
      {
        return (float) ((double) this.icon.Height + (double) ActiveFont.LineHeight + 48.0);
      }
    }
  }
}
