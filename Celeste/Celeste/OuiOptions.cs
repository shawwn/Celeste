// Decompiled with JetBrains decompiler
// Type: Celeste.OuiOptions
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections;

namespace Celeste
{
  public class OuiOptions : Oui
  {
    private float alpha = 0.0f;
    private TextMenu menu;
    private const float onScreenX = 960f;
    private const float offScreenX = 2880f;
    private string startLanguage;
    private string currentLanguage;

    public override void Added(Scene scene)
    {
      base.Added(scene);
    }

    private void ReloadMenu()
    {
      Vector2 vector2 = Vector2.Zero;
      int num = -1;
      if (this.menu != null)
      {
        vector2 = this.menu.Position;
        num = this.menu.Selection;
        this.Scene.Remove((Entity) this.menu);
      }
      this.menu = MenuOptions.Create(false, (EventInstance) null);
      if (num >= 0)
      {
        this.menu.Selection = num;
        this.menu.Position = vector2;
      }
      this.Scene.Add((Entity) this.menu);
    }

    public override IEnumerator Enter(Oui from)
    {
      this.ReloadMenu();
      this.menu.Visible = this.Visible = true;
      this.menu.Focused = false;
      this.currentLanguage = this.startLanguage = Settings.Instance.Language;
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime * 4f)
      {
        this.menu.X = (float) (2880.0 + -1920.0 * (double) Ease.CubeOut(p));
        this.alpha = Ease.CubeOut(p);
        yield return (object) null;
      }
      this.menu.Focused = true;
    }

    public override IEnumerator Leave(Oui next)
    {
      Audio.Play("event:/ui/main/whoosh_large_out");
      this.menu.Focused = false;
      UserIO.SaveHandler(false, true);
      while (UserIO.Saving)
        yield return (object) null;
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime * 4f)
      {
        this.menu.X = (float) (960.0 + 1920.0 * (double) Ease.CubeIn(p));
        this.alpha = 1f - Ease.CubeIn(p);
        yield return (object) null;
      }
      if (this.startLanguage != Settings.Instance.Language)
      {
        this.Overworld.ReloadMenus(Overworld.StartMode.ReturnFromOptions);
        yield return (object) null;
      }
      this.menu.Visible = this.Visible = false;
      this.menu.RemoveSelf();
      this.menu = (TextMenu) null;
    }

    public override void Update()
    {
      if (this.menu != null && this.menu.Focused && (this.Selected && Input.MenuCancel.Pressed))
      {
        Audio.Play("event:/ui/main/button_back");
        this.Overworld.Goto<OuiMainMenu>();
      }
      if (this.Selected && this.currentLanguage != Settings.Instance.Language)
      {
        this.currentLanguage = Settings.Instance.Language;
        this.ReloadMenu();
      }
      base.Update();
    }

    public override void Render()
    {
      if ((double) this.alpha > 0.0)
        Draw.Rect(-10f, -10f, 1940f, 1100f, Color.Black * this.alpha * 0.4f);
      base.Render();
    }
  }
}

