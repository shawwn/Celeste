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
      return ActiveFont.Measure(label).X + 8f + (float) mtexture.Width;
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
      position.X -= (float) ((double) scale * (double) num * ((double) justifyX - 0.5));
      mtexture.Draw(position, new Vector2((float) mtexture.Width - num / 2f, (float) mtexture.Height / 2f), Color.White * alpha, scale + wiggle);
      ButtonUI.DrawText(label, position, num / 2f, scale + wiggle, alpha);
    }

    private static void DrawText(
      string text,
      Vector2 position,
      float justify,
      float scale,
      float alpha)
    {
      float x = ActiveFont.Measure(text).X;
      ActiveFont.DrawOutline(text, position, new Vector2(justify / x, 0.5f), Vector2.One * scale, Color.White * alpha, 2f, Color.Black * alpha);
    }
  }
}

