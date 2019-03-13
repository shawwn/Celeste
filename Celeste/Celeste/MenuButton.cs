// Decompiled with JetBrains decompiler
// Type: Celeste.MenuButton
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  [Tracked(true)]
  public abstract class MenuButton : Entity
  {
    public Vector2 TargetPosition;
    public Vector2 TweenFrom;
    public MenuButton LeftButton;
    public MenuButton RightButton;
    public MenuButton UpButton;
    public MenuButton DownButton;
    public Action OnConfirm;
    private bool canAcceptInput;
    private Oui oui;
    private bool selected;
    private Tween tween;

    public static MenuButton GetSelection(Scene scene)
    {
      foreach (MenuButton entity in scene.Tracker.GetEntities<MenuButton>())
      {
        if (entity.Selected)
          return entity;
      }
      return (MenuButton) null;
    }

    public static void ClearSelection(Scene scene)
    {
      MenuButton selection = MenuButton.GetSelection(scene);
      if (selection == null)
        return;
      selection.Selected = false;
    }

    public MenuButton(Oui oui, Vector2 targetPosition, Vector2 tweenFrom, Action onConfirm)
      : base(tweenFrom)
    {
      this.TargetPosition = targetPosition;
      this.TweenFrom = tweenFrom;
      this.OnConfirm = onConfirm;
      this.oui = oui;
    }

    public override void Update()
    {
      base.Update();
      if (!this.canAcceptInput)
      {
        this.canAcceptInput = true;
      }
      else
      {
        if (!this.oui.Selected || !this.oui.Focused || !this.selected)
          return;
        if (Input.MenuConfirm.Pressed)
          this.Confirm();
        else if (Input.MenuLeft.Pressed && this.LeftButton != null)
        {
          Audio.Play("event:/ui/main/rollover_up");
          this.LeftButton.Selected = true;
        }
        else if (Input.MenuRight.Pressed && this.RightButton != null)
        {
          Audio.Play("event:/ui/main/rollover_down");
          this.RightButton.Selected = true;
        }
        else if (Input.MenuUp.Pressed && this.UpButton != null)
        {
          Audio.Play("event:/ui/main/rollover_up");
          this.UpButton.Selected = true;
        }
        else if (Input.MenuDown.Pressed && this.DownButton != null)
        {
          Audio.Play("event:/ui/main/rollover_down");
          this.DownButton.Selected = true;
        }
      }
    }

    public void TweenIn(float time)
    {
      if (this.tween != null && this.tween.Entity == this)
        this.tween.RemoveSelf();
      Vector2 from = this.Position;
      this.Add((Component) (this.tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeOut, time, true)));
      this.tween.OnUpdate = (Action<Tween>) (t => this.Position = Vector2.Lerp(from, this.TargetPosition, t.Eased));
    }

    public void TweenOut(float time)
    {
      if (this.tween != null && this.tween.Entity == this)
        this.tween.RemoveSelf();
      Vector2 from = this.Position;
      this.Add((Component) (this.tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeIn, time, true)));
      this.tween.OnUpdate = (Action<Tween>) (t => this.Position = Vector2.Lerp(from, this.TweenFrom, t.Eased));
    }

    public Color SelectionColor
    {
      get
      {
        if (this.selected)
          return Settings.Instance.DisableFlashes || this.Scene.BetweenInterval(0.1f) ? TextMenu.HighlightColorA : TextMenu.HighlightColorB;
        return Color.White;
      }
    }

    public bool Selected
    {
      get
      {
        return this.selected;
      }
      set
      {
        if (this.Scene == null)
          throw new Exception("Cannot set Selected while MenuButton is not in a Scene.");
        if (!this.selected & value)
        {
          MenuButton selection = MenuButton.GetSelection(this.Scene);
          if (selection != null)
            selection.Selected = false;
          this.selected = true;
          this.canAcceptInput = false;
          this.OnSelect();
        }
        else
        {
          if (!this.selected || value)
            return;
          this.selected = false;
          this.OnDeselect();
        }
      }
    }

    public virtual void OnSelect()
    {
    }

    public virtual void OnDeselect()
    {
    }

    public virtual void Confirm()
    {
      this.OnConfirm();
    }

    public virtual void StartSelected()
    {
      this.selected = true;
    }

    public abstract float ButtonHeight { get; }
  }
}

