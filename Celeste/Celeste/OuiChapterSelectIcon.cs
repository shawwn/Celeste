// Decompiled with JetBrains decompiler
// Type: Celeste.OuiChapterSelectIcon
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class OuiChapterSelectIcon : Entity
  {
    public const float IdleSize = 100f;
    public const float HoverSize = 144f;
    public const float HoverSpacing = 80f;
    public const float IdleY = 130f;
    public const float HoverY = 140f;
    public const float Spacing = 32f;
    public int Area;
    public bool New;
    public Vector2 Scale = Vector2.One;
    public float Rotation;
    public float sizeEase = 1f;
    public bool AssistModeUnlockable;
    public bool HideIcon;
    private Wiggler newWiggle;
    private bool hidden = true;
    private bool selected;
    private Tween tween;
    private Wiggler wiggler;
    private bool wiggleLeft;
    private int rotateDir = -1;
    private Vector2 shake;
    private float spotlightAlpha;
    private float spotlightRadius;
    private MTexture front;
    private MTexture back;

    public Vector2 IdlePosition
    {
      get
      {
        float x = (float) (960.0 + (double) (this.Area - SaveData.Instance.LastArea.ID) * 132.0);
        if (this.Area < SaveData.Instance.LastArea.ID)
          x -= 80f;
        else if (this.Area > SaveData.Instance.LastArea.ID)
          x += 80f;
        float y = 130f;
        if (this.Area == SaveData.Instance.LastArea.ID)
          y = 140f;
        return new Vector2(x, y);
      }
    }

    public Vector2 HiddenPosition => new Vector2(this.IdlePosition.X, -100f);

    public OuiChapterSelectIcon(int area, MTexture front, MTexture back)
    {
      this.Tag = (int) Tags.HUD | (int) Tags.PauseUpdate;
      this.Position = new Vector2(0.0f, -100f);
      this.Area = area;
      this.front = front;
      this.back = back;
      this.Add((Component) (this.wiggler = Wiggler.Create(0.35f, 2f, (Action<float>) (f =>
      {
        this.Rotation = (float) ((this.wiggleLeft ? -(double) f : (double) f) * 0.4000000059604645);
        this.Scale = Vector2.One * (float) (1.0 + (double) f * 0.5);
      }))));
      this.Add((Component) (this.newWiggle = Wiggler.Create(0.8f, 2f)));
      this.newWiggle.StartZero = true;
    }

    public void Hovered(int dir)
    {
      this.wiggleLeft = dir < 0;
      this.wiggler.Start();
    }

    public void Select()
    {
      Audio.Play("event:/ui/world_map/icon/flip_right");
      this.selected = true;
      this.hidden = false;
      Vector2 from = this.Position;
      this.StartTween(0.6f, (Action<Tween>) (t => this.SetSelectedPercent(from, t.Percent)));
    }

    public void SnapToSelected()
    {
      this.selected = true;
      this.hidden = false;
      this.StopTween();
    }

    public void Unselect()
    {
      Audio.Play("event:/ui/world_map/icon/flip_left");
      this.hidden = false;
      this.selected = false;
      Vector2 to = this.IdlePosition;
      this.StartTween(0.6f, (Action<Tween>) (t => this.SetSelectedPercent(to, 1f - t.Percent)));
    }

    public void Hide()
    {
      this.Scale = Vector2.One;
      this.hidden = true;
      this.selected = false;
      Vector2 from = this.Position;
      this.StartTween(0.25f, (Action<Tween>) (t => this.Position = Vector2.Lerp(from, this.HiddenPosition, this.tween.Eased)));
    }

    public void Show()
    {
      if (SaveData.Instance != null)
        this.New = SaveData.Instance.Areas[this.Area].Modes[0].TimePlayed <= 0L;
      this.Scale = Vector2.One;
      this.hidden = false;
      this.selected = false;
      Vector2 from = this.Position;
      this.StartTween(0.25f, (Action<Tween>) (t => this.Position = Vector2.Lerp(from, this.IdlePosition, this.tween.Eased)));
    }

    public void AssistModeUnlock(Action onComplete) => this.Add((Component) new Coroutine(this.AssistModeUnlockRoutine(onComplete)));

    private IEnumerator AssistModeUnlockRoutine(Action onComplete)
    {
      float p;
      for (p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime * 4f)
      {
        this.spotlightRadius = Ease.CubeOut(p) * 128f;
        this.spotlightAlpha = Ease.CubeOut(p) * 0.8f;
        yield return (object) null;
      }
      this.shake.X = 6f;
      for (int i = 0; i < 10; ++i)
      {
        this.shake.X = -this.shake.X;
        yield return (object) 0.01f;
      }
      this.shake = Vector2.Zero;
      for (p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime * 4f)
      {
        this.shake = new Vector2(0.0f, -160f * Ease.CubeIn(p));
        this.Scale = new Vector2(1f - p, (float) (1.0 + (double) p * 0.25));
        yield return (object) null;
      }
      this.shake = Vector2.Zero;
      this.Scale = Vector2.One;
      this.AssistModeUnlockable = false;
      ++SaveData.Instance.UnlockedAreas;
      this.wiggler.Start();
      yield return (object) 1f;
      for (p = 1f; (double) p > 0.0; p -= Engine.DeltaTime * 4f)
      {
        this.spotlightRadius = (float) (128.0 + (1.0 - (double) Ease.CubeOut(p)) * 128.0);
        this.spotlightAlpha = Ease.CubeOut(p) * 0.8f;
        yield return (object) null;
      }
      this.spotlightAlpha = 0.0f;
      if (onComplete != null)
        onComplete();
    }

    public void HighlightUnlock(Action onComplete)
    {
      this.HideIcon = true;
      this.Add((Component) new Coroutine(this.HighlightUnlockRoutine(onComplete)));
    }

    private IEnumerator HighlightUnlockRoutine(Action onComplete)
    {
      float p;
      for (p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime * 2f)
      {
        this.spotlightRadius = (float) (128.0 + (1.0 - (double) Ease.CubeOut(p)) * 128.0);
        this.spotlightAlpha = Ease.CubeOut(p) * 0.8f;
        yield return (object) null;
      }
      Audio.Play("event:/ui/postgame/unlock_newchapter_icon");
      this.HideIcon = false;
      this.wiggler.Start();
      yield return (object) 2f;
      for (p = 1f; (double) p > 0.0; p -= Engine.DeltaTime * 2f)
      {
        this.spotlightRadius = (float) (128.0 + (1.0 - (double) Ease.CubeOut(p)) * 128.0);
        this.spotlightAlpha = Ease.CubeOut(p) * 0.8f;
        yield return (object) null;
      }
      this.spotlightAlpha = 0.0f;
      if (onComplete != null)
        onComplete();
    }

    private void StartTween(float duration, Action<Tween> callback)
    {
      this.StopTween();
      this.Add((Component) (this.tween = Tween.Create(Tween.TweenMode.Oneshot, duration: duration, start: true)));
      this.tween.OnUpdate = callback;
      this.tween.OnComplete = (Action<Tween>) (t => this.tween = (Tween) null);
    }

    private void StopTween()
    {
      if (this.tween != null)
        this.Remove((Component) this.tween);
      this.tween = (Tween) null;
    }

    private void SetSelectedPercent(Vector2 from, float p)
    {
      OuiChapterPanel ui = (this.Scene as Overworld).GetUI<OuiChapterPanel>();
      Vector2 end = ui.OpenPosition + ui.IconOffset;
      SimpleCurve simpleCurve = new SimpleCurve(from, end, (from + end) / 2f + new Vector2(0.0f, 30f));
      float num = (float) (1.0 + ((double) p < 0.5 ? (double) p * 2.0 : (1.0 - (double) p) * 2.0));
      this.Scale.X = (float) Math.Cos((double) Ease.SineInOut(p) * 6.2831854820251465) * num;
      this.Scale.Y = num;
      this.Position = simpleCurve.GetPoint(Ease.Invert(Ease.CubeInOut)(p));
      this.Rotation = (float) ((double) Ease.UpDown(Ease.SineInOut(p)) * (Math.PI / 180.0) * 15.0) * (float) this.rotateDir;
      if ((double) p <= 0.0)
      {
        this.rotateDir = -1;
      }
      else
      {
        if ((double) p < 1.0)
          return;
        this.rotateDir = 1;
      }
    }

    public override void Update()
    {
      if (SaveData.Instance == null)
        return;
      this.sizeEase = Calc.Approach(this.sizeEase, SaveData.Instance.LastArea.ID == this.Area ? 1f : 0.0f, Engine.DeltaTime * 4f);
      if (SaveData.Instance.LastArea.ID == this.Area)
        this.Depth = -50;
      else
        this.Depth = -45;
      if (this.tween == null)
      {
        if (this.selected)
        {
          OuiChapterPanel ui = (this.Scene as Overworld).GetUI<OuiChapterPanel>();
          this.Position = (!ui.EnteringChapter ? ui.OpenPosition : ui.Position) + ui.IconOffset;
        }
        else if (!this.hidden)
          this.Position = Calc.Approach(this.Position, this.IdlePosition, 2400f * Engine.DeltaTime);
      }
      if (this.New && this.Scene.OnInterval(1.5f))
        this.newWiggle.Start();
      base.Update();
    }

    public override void Render()
    {
      MTexture mtexture = this.front;
      Vector2 scale1 = this.Scale;
      int width = mtexture.Width;
      if ((double) scale1.X < 0.0)
        mtexture = this.back;
      if (this.AssistModeUnlockable)
      {
        mtexture = GFX.Gui["areas/lock"];
        width -= 32;
      }
      if (!this.HideIcon)
      {
        Vector2 scale2 = scale1 * ((float) (100.0 + 44.0 * (double) Ease.CubeInOut(this.sizeEase)) / (float) width);
        if (SaveData.Instance != null && SaveData.Instance.Assists.MirrorMode)
          scale2.X = -scale2.X;
        mtexture.DrawCentered(this.Position + this.shake, Color.White, scale2, this.Rotation);
        if (this.New && SaveData.Instance != null && !SaveData.Instance.CheatMode && this.Area == SaveData.Instance.UnlockedAreas && !this.selected && this.tween == null && !this.AssistModeUnlockable && Celeste.PlayMode != Celeste.PlayModes.Event)
        {
          Vector2 position = this.Position + new Vector2((float) width * 0.25f, (float) -mtexture.Height * 0.25f) + Vector2.UnitY * -Math.Abs(this.newWiggle.Value * 30f);
          GFX.Gui["areas/new"].DrawCentered(position);
        }
      }
      if ((double) this.spotlightAlpha > 0.0)
      {
        HiresRenderer.EndRender();
        SpotlightWipe.DrawSpotlight(new Vector2(this.Position.X, this.IdlePosition.Y), this.spotlightRadius, Color.Black * this.spotlightAlpha);
        HiresRenderer.BeginRender();
      }
      else
      {
        if (!this.AssistModeUnlockable || SaveData.Instance.LastArea.ID != this.Area || this.hidden)
          return;
        ActiveFont.DrawOutline(Dialog.Clean("ASSIST_SKIP"), this.Position + new Vector2(0.0f, 100f), new Vector2(0.5f, 0.0f), Vector2.One * 0.7f, Color.White, 2f, Color.Black);
      }
    }
  }
}
