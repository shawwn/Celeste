// Decompiled with JetBrains decompiler
// Type: Celeste.OuiTitleScreen
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Celeste
{
  public class OuiTitleScreen : Oui
  {
    public static readonly MountainCamera MountainTarget = new MountainCamera(new Vector3(0.0f, 12f, 24f), MountainRenderer.RotateLookAt);
    private string version = "v." + (object) Celeste.Instance.Version;
    private const float TextY = 1000f;
    private const float TextOutY = 1200f;
    private const int ReflectionSliceSize = 4;
    private float alpha;
    private float fade;
    private bool hideConfirmButton;
    private Monocle.Image logo;
    private MTexture title;
    private List<MTexture> reflections;
    private float textY;

    public OuiTitleScreen()
    {
      this.logo = new Monocle.Image(GFX.Gui[nameof (logo)]);
      this.logo.CenterOrigin();
      this.logo.Position = new Vector2(1920f, 1080f) / 2f;
      this.title = GFX.Gui[nameof (title)];
      this.reflections = new List<MTexture>();
      for (int y = this.title.Height - 4; y > 0; y -= 4)
        this.reflections.Add(this.title.GetSubtexture(0, y, this.title.Width, 4, (MTexture) null));
      if ((uint) Celeste.PlayMode > 0U)
      {
        if ("".Length > 0)
          this.version += "\n";
        this.version = this.version + "\n" + Celeste.PlayMode.ToString() + " Build";
      }
      if (!Settings.Instance.LaunchWithFMODLiveUpdate)
        return;
      this.version += "\nFMOD Live Update Enabled";
    }

    public override bool IsStart(Overworld overworld, Overworld.StartMode start)
    {
      if (start == Overworld.StartMode.Titlescreen)
      {
        overworld.ShowInputUI = false;
        overworld.Mountain.SnapCamera(-1, OuiTitleScreen.MountainTarget, false);
        this.textY = 1000f;
        this.alpha = 1f;
        this.fade = 1f;
        return true;
      }
      this.textY = 1200f;
      return false;
    }

    public override IEnumerator Enter(Oui from)
    {
      yield return (object) null;
      this.Overworld.ShowInputUI = false;
      MountainCamera camera = this.Overworld.Mountain.Camera;
      Vector3 center = MountainRenderer.RotateLookAt;
      Vector3 normal = (camera.Position - new Vector3(center.X, camera.Position.Y - 2f, center.Z)).SafeNormalize();
      MountainCamera away = new MountainCamera(MountainRenderer.RotateLookAt + normal * 20f, camera.Target);
      this.Add((Component) new Coroutine(this.FadeBgTo(1f), true));
      this.hideConfirmButton = false;
      this.Visible = true;
      double num = (double) this.Overworld.Mountain.EaseCamera(-1, away, new float?(2f), false, false);
      float start = this.textY;
      yield return (object) 0.4f;
      Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeInOut, 0.6f, true);
      tween.OnUpdate = (Action<Tween>) (t =>
      {
        this.alpha = t.Percent;
        this.textY = MathHelper.Lerp(start, 1000f, t.Eased);
      });
      this.Add((Component) tween);
      yield return (object) tween.Wait();
      this.Overworld.Mountain.SnapCamera(-1, OuiTitleScreen.MountainTarget, false);
    }

    public override IEnumerator Leave(Oui next)
    {
      this.Overworld.ShowInputUI = true;
      this.Overworld.Mountain.GotoRotationMode();
      float start = this.textY;
      Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeInOut, 0.6f, true);
      tween.OnUpdate = (Action<Tween>) (t =>
      {
        this.alpha = 1f - t.Percent;
        this.textY = MathHelper.Lerp(start, 1200f, t.Eased);
      });
      this.Add((Component) tween);
      yield return (object) tween.Wait();
      yield return (object) this.FadeBgTo(0.0f);
      this.Visible = false;
    }

    private IEnumerator FadeBgTo(float to)
    {
      for (; (double) this.fade != (double) to; this.fade = Calc.Approach(this.fade, to, Engine.DeltaTime * 2f))
        yield return (object) null;
    }

    public override void Update()
    {
      int gamepadIndex = -1;
      if (this.Selected && Input.AnyGamepadConfirmPressed(out gamepadIndex) && !this.hideConfirmButton)
      {
        Input.Gamepad = gamepadIndex;
        Audio.Play("event:/ui/main/title_firstinput");
        this.Overworld.Goto<OuiMainMenu>();
      }
      base.Update();
    }

    public override void Render()
    {
      Draw.Rect(-10f, -10f, 1940f, 1100f, Color.Black * this.fade);
      if (!this.hideConfirmButton)
        Input.GuiButton(Input.MenuConfirm, "controls/keyboard/oemquestion").DrawJustified(new Vector2(1840f, this.textY), new Vector2(1f, 1f), Color.White * this.alpha, 1f);
      ActiveFont.Draw(this.version, new Vector2(80f, this.textY), new Vector2(0.0f, 1f), Vector2.One * 0.5f, Color.DarkSlateBlue);
      if ((double) this.alpha <= 0.0)
        return;
      float num1 = MathHelper.Lerp(0.5f, 1f, Ease.SineOut(this.alpha));
      this.logo.Color = Color.White * this.alpha;
      this.logo.Scale = Vector2.One * num1;
      this.logo.Render();
      float num2 = this.Scene.TimeActive * 3f;
      float num3 = (float) (1.0 / (double) this.reflections.Count * 6.28318548202515 * 2.0);
      float num4 = (float) this.title.Width / this.logo.Width * num1;
      for (int index = 0; index < this.reflections.Count; ++index)
      {
        float num5 = (float) index / (float) this.reflections.Count;
        Vector2 position = new Vector2(1920f, 1080f) / 2f + new Vector2((float) Math.Sin((double) num2) * 32f * num5, this.logo.Height * 0.5f + (float) (index * 4)) * num4;
        float num6 = (float) ((double) Ease.CubeIn(1f - num5) * (double) this.alpha * 0.899999976158142);
        this.reflections[index].DrawJustified(position, new Vector2(0.5f, 0.5f), Color.White * num6, new Vector2(1f, -1f) * num4);
        num2 += num3 * (float) (Math.Sin((double) this.Scene.TimeActive + (double) index * 6.28318548202515 * 0.0399999991059303) + 1.0);
      }
    }
  }
}

