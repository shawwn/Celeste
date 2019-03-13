// Decompiled with JetBrains decompiler
// Type: Celeste.ButtonUI
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  public static class ButtonUI
  {
    public static float Width(string label, VirtualButton button)
    {
      MTexture mtexture = Input.GuiButton(button, "controls/keyboard/oemquestion");
      return (float) (ActiveFont.Measure(label).X + 8.0) + (float) mtexture.Width;
    }

    public static void Render(
      Vector2 position,
      string label,
      VirtualButton button,
      float scale,
      float justifyX = 0.5f,
      float wiggle = 0.0f,
      float alpha = 1f)
    {
      MTexture mtexture = Input.GuiButton(button, "controls/keyboard/oemquestion");
      float num = ButtonUI.Width(label, button);
      ref __Null local = ref position.X;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local = ^(float&) ref local - (float) ((double) scale * (double) num * ((double) justifyX - 0.5));
      mtexture.Draw(position, new Vector2((float) mtexture.Width - num / 2f, (float) mtexture.Height / 2f), Color.op_Multiply(Color.get_White(), alpha), scale + wiggle);
      ButtonUI.DrawText(label, position, num / 2f, scale + wiggle, alpha);
    }

    private static void DrawText(
      string text,
      Vector2 position,
      float justify,
      float scale,
      float alpha)
    {
      float x = (float) ActiveFont.Measure(text).X;
      ActiveFont.DrawOutline(text, position, new Vector2(justify / x, 0.5f), Vector2.op_Multiply(Vector2.get_One(), scale), Color.op_Multiply(Color.get_White(), alpha), 2f, Color.op_Multiply(Color.get_Black(), alpha));
    }
  }
}
