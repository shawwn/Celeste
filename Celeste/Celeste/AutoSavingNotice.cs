// Decompiled with JetBrains decompiler
// Type: Celeste.AutoSavingNotice
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;

namespace Celeste
{
  public class AutoSavingNotice : Monocle.Renderer
  {
    public static readonly Color TextColor = Color.White;
    public bool Display = true;
    private float ease = 0.0f;
    private float timer = 0.0f;
    private Sprite icon = GFX.GuiSpriteBank.Create("save");
    private float startTimer = 0.5f;
    private const string title = "autosaving_title_PC";
    private const string desc = "autosaving_desc_PC";
    private const float duration = 3f;
    public bool StillVisible;
    public bool ForceClose;
    private Wiggler wiggler;

    public AutoSavingNotice()
    {
      this.icon.Visible = false;
      this.wiggler = Wiggler.Create(0.4f, 4f, (Action<float>) (f => this.icon.Rotation = f * 0.1f), false, false);
    }

    public override void Update(Scene scene)
    {
      base.Update(scene);
      if ((double) this.startTimer > 0.0)
      {
        this.startTimer -= Engine.DeltaTime;
        if ((double) this.startTimer <= 0.0)
        {
          this.icon.Play("start", false, false);
          this.icon.Visible = true;
        }
      }
      if (scene.OnInterval(1f))
        this.wiggler.Start();
      bool flag = this.ForceClose || !this.Display && (double) this.timer >= 1.0;
      this.ease = Calc.Approach(this.ease, !flag ? 1f : 0.0f, Engine.DeltaTime);
      this.timer += Engine.DeltaTime / 3f;
      this.StillVisible = this.Display || (double) this.ease > 0.0;
      this.wiggler.Update();
      this.icon.Update();
      if (!flag || string.IsNullOrEmpty(this.icon.CurrentAnimationID) || !this.icon.CurrentAnimationID.Equals("idle"))
        return;
      this.icon.Play("end", false, false);
    }

    public override void Render(Scene scene)
    {
      float num = Ease.CubeInOut(this.ease);
      Color color = AutoSavingNotice.TextColor * num;
      Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, (DepthStencilState) null, (RasterizerState) null, (Effect) null, Engine.ScreenMatrix);
      ActiveFont.Draw(Dialog.Clean("autosaving_title_PC", (Language) null), new Vector2(960f, (float) (480.0 - 30.0 * (double) num)), new Vector2(0.5f, 1f), Vector2.One, color);
      if (this.icon.Visible)
      {
        this.icon.RenderPosition = new Vector2(1920f, 1080f) / 2f;
        this.icon.Render();
      }
      ActiveFont.Draw(Dialog.Clean("autosaving_desc_PC", (Language) null), new Vector2(960f, (float) (600.0 + 30.0 * (double) num)), new Vector2(0.5f, 0.0f), Vector2.One, color);
      Draw.SpriteBatch.End();
    }
  }
}

