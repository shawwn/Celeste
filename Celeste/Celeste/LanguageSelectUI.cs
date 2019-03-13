// Decompiled with JetBrains decompiler
// Type: Celeste.LanguageSelectUI
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  public class LanguageSelectUI : TextMenu
  {
    private bool open = true;
    public Action<string> OnChange;

    public LanguageSelectUI()
    {
      this.Tag = (int) Tags.HUD | (int) Tags.PauseUpdate;
      this.Alpha = 0.0f;
      foreach (Language orderedLanguage in Dialog.OrderedLanguages)
      {
        Language language = orderedLanguage;
        this.Add(new LanguageSelectUI.LanguageOption(language).Pressed((Action) (() =>
        {
          this.open = false;
          this.OnChange(language.Id);
        })));
      }
      this.OnESC = this.OnPause = this.OnCancel = (Action) (() =>
      {
        this.open = false;
        this.Focused = false;
      });
    }

    public override void Update()
    {
      base.Update();
      if (!this.open && (double) this.Alpha <= 0.0)
        this.Close();
      this.Alpha = Calc.Approach(this.Alpha, this.open ? 1f : 0.0f, Engine.DeltaTime * 8f);
    }

    public override void Render()
    {
      Draw.Rect(-10f, -10f, 1940f, 1100f, Color.Black * Ease.CubeOut(this.Alpha));
      base.Render();
    }

    private class LanguageOption : TextMenu.Item
    {
      public Language Language;

      public LanguageOption(Language language)
      {
        this.Selectable = true;
        this.Language = language;
      }

      public override void Added()
      {
        this.Container.InnerContent = TextMenu.InnerContentMode.TwoColumn;
        if (Dialog.Language != this.Language)
          return;
        this.Container.Current = (TextMenu.Item) this;
      }

      public override float LeftWidth()
      {
        return 96f;
      }

      public override float RightWidth()
      {
        return this.Language.FontSize.Measure(this.Language.Label).X;
      }

      public override float Height()
      {
        return (float) this.Language.FontSize.LineHeight;
      }

      public override void Render(Vector2 position, bool highlighted)
      {
        Color color = this.Disabled ? Color.DarkSlateGray : (highlighted ? this.Container.HighlightColor : Color.White) * this.Container.Alpha;
        Color strokeColor = Color.Black * (this.Container.Alpha * this.Container.Alpha * this.Container.Alpha * this.Container.Alpha);
        position += (1f - Ease.CubeOut(this.Container.Alpha)) * Vector2.UnitY * 32f;
        this.Language.FontSize.DrawOutline(this.Language.Label, position + new Vector2(96f, 0.0f), new Vector2(0.0f, 0.5f), Vector2.One, color, 2f, strokeColor);
        if (this.Language.Icon == null)
          return;
        this.Language.Icon.DrawJustified(position, new Vector2(0.0f, 0.5f), Color.White * this.Container.Alpha, 64f / (float) this.Language.Icon.Width);
      }
    }
  }
}

