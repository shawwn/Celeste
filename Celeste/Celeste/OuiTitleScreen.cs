// Decompiled with JetBrains decompiler
// Type: Celeste.OuiTitleScreen
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

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
    private const float TextY = 1000f;
    private const float TextOutY = 1200f;
    private const int ReflectionSliceSize = 4;
    private float alpha;
    private float fade;
    private string version = "v." + (object) Celeste.Instance.Version;
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
        this.reflections.Add(this.title.GetSubtexture(0, y, this.title.Width, 4));
      if (Celeste.PlayMode != Celeste.PlayModes.Normal)
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
        overworld.Mountain.SnapCamera(-1, OuiTitleScreen.MountainTarget);
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
      OuiTitleScreen ouiTitleScreen = this;
      yield return (object) null;
      ouiTitleScreen.Overworld.ShowInputUI = false;
      MountainCamera camera = ouiTitleScreen.Overworld.Mountain.Camera;
      Vector3 rotateLookAt = MountainRenderer.RotateLookAt;
      Vector3 vector3 = (camera.Position - new Vector3(rotateLookAt.X, camera.Position.Y - 2f, rotateLookAt.Z)).SafeNormalize();
      MountainCamera transform = new MountainCamera(MountainRenderer.RotateLookAt + vector3 * 20f, camera.Target);
      ouiTitleScreen.Add((Component) new Coroutine(ouiTitleScreen.FadeBgTo(1f)));
      ouiTitleScreen.hideConfirmButton = false;
      ouiTitleScreen.Visible = true;
      double num = (double) ouiTitleScreen.Overworld.Mountain.EaseCamera(-1, transform, new float?(2f), false);
      float start = ouiTitleScreen.textY;
      yield return (object) 0.4f;
      Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeInOut, 0.6f, true);
      tween.OnUpdate = (Action<Tween>) (t =>
      {
        this.alpha = t.Percent;
        this.textY = MathHelper.Lerp(start, 1000f, t.Eased);
      });
      ouiTitleScreen.Add((Component) tween);
      yield return (object) tween.Wait();
      ouiTitleScreen.Overworld.Mountain.SnapCamera(-1, OuiTitleScreen.MountainTarget);
    }

    public override IEnumerator Leave(Oui next)
    {
      OuiTitleScreen ouiTitleScreen = this;
      ouiTitleScreen.Overworld.ShowInputUI = true;
      ouiTitleScreen.Overworld.Mountain.GotoRotationMode();
      float start = ouiTitleScreen.textY;
      Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeInOut, 0.6f, true);
      tween.OnUpdate = (Action<Tween>) (t =>
      {
        this.alpha = 1f - t.Percent;
        this.textY = MathHelper.Lerp(start, 1200f, t.Eased);
      });
      ouiTitleScreen.Add((Component) tween);
      yield return (object) tween.Wait();
      yield return (object) ouiTitleScreen.FadeBgTo(0.0f);
      ouiTitleScreen.Visible = false;
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
        Input.GuiButton(Input.MenuConfirm).DrawJustified(new Vector2(1840f, this.textY), new Vector2(1f, 1f), Color.White * this.alpha, 1f);
      ActiveFont.Draw(this.version, new Vector2(80f, this.textY), new Vector2(0.0f, 1f), Vector2.One * 0.5f, Color.DarkSlateBlue);
      if ((double) this.alpha <= 0.0)
        return;
      float num1 = MathHelper.Lerp(0.5f, 1f, Ease.SineOut(this.alpha));
      this.logo.Color = Color.White * this.alpha;
      this.logo.Scale = Vector2.One * num1;
      this.logo.Render();
      float a = this.Scene.TimeActive * 3f;
      float num2 = (float) (1.0 / (double) this.reflections.Count * 6.2831854820251465 * 2.0);
      float num3 = (float) this.title.Width / this.logo.Width * num1;
      for (int index = 0; index < this.reflections.Count; ++index)
      {
        float num4 = (float) index / (float) this.reflections.Count;
        Vector2 position = new Vector2(1920f, 1080f) / 2f + new Vector2((float) Math.Sin((double) a) * 32f * num4, this.logo.Height * 0.5f + (float) (index * 4)) * num3;
        float num5 = (float) ((double) Ease.CubeIn(1f - num4) * (double) this.alpha * 0.8999999761581421);
        this.reflections[index].DrawJustified(position, new Vector2(0.5f, 0.5f), Color.White * num5, new Vector2(1f, -1f) * num3);
        a += num2 * (float) (Math.Sin((double) this.Scene.TimeActive + (double) index * 6.2831854820251465 * 0.03999999910593033) + 1.0);
      }
    }
  }
}
