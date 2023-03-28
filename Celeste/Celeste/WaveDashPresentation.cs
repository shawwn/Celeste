// Decompiled with JetBrains decompiler
// Type: Celeste.WaveDashPresentation
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using FMOD.Studio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Celeste
{
  public class WaveDashPresentation : Entity
  {
    public Vector2 ScaleInPoint = new Vector2(1920f, 1080f) / 2f;
    public readonly int ScreenWidth = 1920;
    public readonly int ScreenHeight = 1080;
    private float ease;
    private bool loading;
    private float waitingForInputTime;
    private VirtualRenderTarget screenBuffer;
    private VirtualRenderTarget prevPageBuffer;
    private VirtualRenderTarget currPageBuffer;
    private int pageIndex;
    private List<WaveDashPage> pages = new List<WaveDashPage>();
    private float pageEase;
    private bool pageTurning;
    private bool pageUpdating;
    private bool waitingForPageTurn;
    private VertexPositionColorTexture[] verts = new VertexPositionColorTexture[6];
    private EventInstance usingSfx;

    public bool Viewing { get; private set; }

    public Atlas Gfx { get; private set; }

    public bool ShowInput
    {
      get
      {
        if (this.waitingForPageTurn)
          return true;
        return this.CurrPage != null && this.CurrPage.WaitingForInput;
      }
    }

    private WaveDashPage PrevPage => this.pageIndex <= 0 ? (WaveDashPage) null : this.pages[this.pageIndex - 1];

    private WaveDashPage CurrPage => this.pageIndex >= this.pages.Count ? (WaveDashPage) null : this.pages[this.pageIndex];

    public WaveDashPresentation(EventInstance usingSfx = null)
    {
      this.Tag = (int) Tags.HUD;
      this.Viewing = true;
      this.loading = true;
      this.Add((Component) new Coroutine(this.Routine()));
      this.usingSfx = usingSfx;
      RunThread.Start(new Action(this.LoadingThread), "Wave Dash Presentation Loading", true);
    }

    private void LoadingThread()
    {
      this.Gfx = Atlas.FromAtlas(Path.Combine("Graphics", "Atlases", "WaveDashing"), Atlas.AtlasDataFormat.Packer);
      this.loading = false;
    }

    private IEnumerator Routine()
    {
      WaveDashPresentation presentation = this;
      while (presentation.loading)
        yield return (object) null;
      presentation.pages.Add((WaveDashPage) new WaveDashPage00());
      presentation.pages.Add((WaveDashPage) new WaveDashPage01());
      presentation.pages.Add((WaveDashPage) new WaveDashPage02());
      presentation.pages.Add((WaveDashPage) new WaveDashPage03());
      presentation.pages.Add((WaveDashPage) new WaveDashPage04());
      presentation.pages.Add((WaveDashPage) new WaveDashPage05());
      presentation.pages.Add((WaveDashPage) new WaveDashPage06());
      foreach (WaveDashPage page in presentation.pages)
        page.Added(presentation);
      presentation.Add((Component) new BeforeRenderHook(new Action(presentation.BeforeRender)));
      while ((double) presentation.ease < 1.0)
      {
        presentation.ease = Calc.Approach(presentation.ease, 1f, Engine.DeltaTime * 2f);
        yield return (object) null;
      }
      while (presentation.pageIndex < presentation.pages.Count)
      {
        presentation.pageUpdating = true;
        yield return (object) presentation.CurrPage.Routine();
        if (!presentation.CurrPage.AutoProgress)
        {
          presentation.waitingForPageTurn = true;
          while (!Input.MenuConfirm.Pressed)
            yield return (object) null;
          presentation.waitingForPageTurn = false;
          Audio.Play("event:/new_content/game/10_farewell/ppt_mouseclick");
        }
        presentation.pageUpdating = false;
        ++presentation.pageIndex;
        if (presentation.pageIndex < presentation.pages.Count)
        {
          float duration = 0.5f;
          if (presentation.CurrPage.Transition == WaveDashPage.Transitions.Rotate3D)
            duration = 1.5f;
          else if (presentation.CurrPage.Transition == WaveDashPage.Transitions.Blocky)
            duration = 1f;
          presentation.pageTurning = true;
          presentation.pageEase = 0.0f;
          presentation.Add((Component) new Coroutine(presentation.TurnPage(duration)));
          yield return (object) (float) ((double) duration * 0.800000011920929);
        }
      }
      if ((HandleBase) presentation.usingSfx != (HandleBase) null)
      {
        Audio.SetParameter(presentation.usingSfx, "end", 1f);
        int num = (int) presentation.usingSfx.release();
      }
      Audio.Play("event:/new_content/game/10_farewell/cafe_computer_off");
      while ((double) presentation.ease > 0.0)
      {
        presentation.ease = Calc.Approach(presentation.ease, 0.0f, Engine.DeltaTime * 2f);
        yield return (object) null;
      }
      presentation.Viewing = false;
      presentation.RemoveSelf();
    }

    private IEnumerator TurnPage(float duration)
    {
      if (this.CurrPage.Transition != WaveDashPage.Transitions.ScaleIn && this.CurrPage.Transition != WaveDashPage.Transitions.FadeIn)
      {
        if (this.CurrPage.Transition == WaveDashPage.Transitions.Rotate3D)
          Audio.Play("event:/new_content/game/10_farewell/ppt_cube_transition");
        else if (this.CurrPage.Transition == WaveDashPage.Transitions.Blocky)
          Audio.Play("event:/new_content/game/10_farewell/ppt_dissolve_transition");
        else if (this.CurrPage.Transition == WaveDashPage.Transitions.Spiral)
          Audio.Play("event:/new_content/game/10_farewell/ppt_spinning_transition");
      }
      while ((double) this.pageEase < 1.0)
      {
        this.pageEase += Engine.DeltaTime / duration;
        yield return (object) null;
      }
      this.pageTurning = false;
    }

    private void BeforeRender()
    {
      if (this.loading)
        return;
      if (this.screenBuffer == null || this.screenBuffer.IsDisposed)
        this.screenBuffer = VirtualContent.CreateRenderTarget("WaveDash-Buffer", this.ScreenWidth, this.ScreenHeight, true);
      if (this.prevPageBuffer == null || this.prevPageBuffer.IsDisposed)
        this.prevPageBuffer = VirtualContent.CreateRenderTarget("WaveDash-Screen1", this.ScreenWidth, this.ScreenHeight);
      if (this.currPageBuffer == null || this.currPageBuffer.IsDisposed)
        this.currPageBuffer = VirtualContent.CreateRenderTarget("WaveDash-Screen2", this.ScreenWidth, this.ScreenHeight);
      if (this.pageTurning && this.PrevPage != null)
      {
        Engine.Graphics.GraphicsDevice.SetRenderTarget((RenderTarget2D) this.prevPageBuffer);
        Engine.Graphics.GraphicsDevice.Clear(this.PrevPage.ClearColor);
        Draw.SpriteBatch.Begin();
        this.PrevPage.Render();
        Draw.SpriteBatch.End();
      }
      if (this.CurrPage != null)
      {
        Engine.Graphics.GraphicsDevice.SetRenderTarget((RenderTarget2D) this.currPageBuffer);
        Engine.Graphics.GraphicsDevice.Clear(this.CurrPage.ClearColor);
        Draw.SpriteBatch.Begin();
        this.CurrPage.Render();
        Draw.SpriteBatch.End();
      }
      Engine.Graphics.GraphicsDevice.SetRenderTarget((RenderTarget2D) this.screenBuffer);
      Engine.Graphics.GraphicsDevice.Clear(Color.Black);
      if (this.pageTurning)
      {
        if (this.CurrPage.Transition == WaveDashPage.Transitions.ScaleIn)
        {
          Draw.SpriteBatch.Begin();
          Draw.SpriteBatch.Draw((Texture2D) (RenderTarget2D) this.prevPageBuffer, Vector2.Zero, Color.White);
          Draw.SpriteBatch.Draw((Texture2D) (RenderTarget2D) this.currPageBuffer, this.ScaleInPoint, new Rectangle?(this.currPageBuffer.Bounds), Color.White, 0.0f, this.ScaleInPoint, Vector2.One * this.pageEase, SpriteEffects.None, 0.0f);
          Draw.SpriteBatch.End();
        }
        else if (this.CurrPage.Transition == WaveDashPage.Transitions.FadeIn)
        {
          Draw.SpriteBatch.Begin();
          Draw.SpriteBatch.Draw((Texture2D) (RenderTarget2D) this.prevPageBuffer, Vector2.Zero, Color.White);
          Draw.SpriteBatch.Draw((Texture2D) (RenderTarget2D) this.currPageBuffer, Vector2.Zero, Color.White * this.pageEase);
          Draw.SpriteBatch.End();
        }
        else if (this.CurrPage.Transition == WaveDashPage.Transitions.Rotate3D)
        {
          float rotation = -1.5707964f * this.pageEase;
          this.RenderQuad((Texture) (RenderTarget2D) this.prevPageBuffer, this.pageEase, rotation);
          this.RenderQuad((Texture) (RenderTarget2D) this.currPageBuffer, this.pageEase, 1.5707964f + rotation);
        }
        else if (this.CurrPage.Transition == WaveDashPage.Transitions.Blocky)
        {
          Draw.SpriteBatch.Begin();
          Draw.SpriteBatch.Draw((Texture2D) (RenderTarget2D) this.prevPageBuffer, Vector2.Zero, Color.White);
          uint seed = 1;
          int num = this.ScreenWidth / 60;
          for (int x = 0; x < this.ScreenWidth; x += num)
          {
            for (int y = 0; y < this.ScreenHeight; y += num)
            {
              if ((double) WaveDashPresentation.PseudoRandRange(ref seed, 0.0f, 1f) <= (double) this.pageEase)
                Draw.SpriteBatch.Draw((Texture2D) (RenderTarget2D) this.currPageBuffer, new Rectangle(x, y, num, num), new Rectangle?(new Rectangle(x, y, num, num)), Color.White);
            }
          }
          Draw.SpriteBatch.End();
        }
        else
        {
          if (this.CurrPage.Transition != WaveDashPage.Transitions.Spiral)
            return;
          Draw.SpriteBatch.Begin();
          Draw.SpriteBatch.Draw((Texture2D) (RenderTarget2D) this.prevPageBuffer, Vector2.Zero, Color.White);
          Draw.SpriteBatch.Draw((Texture2D) (RenderTarget2D) this.currPageBuffer, Celeste.TargetCenter, new Rectangle?(this.currPageBuffer.Bounds), Color.White, (float) ((1.0 - (double) this.pageEase) * 12.0), Celeste.TargetCenter, Vector2.One * this.pageEase, SpriteEffects.None, 0.0f);
          Draw.SpriteBatch.End();
        }
      }
      else
      {
        Draw.SpriteBatch.Begin();
        Draw.SpriteBatch.Draw((Texture2D) (RenderTarget2D) this.currPageBuffer, Vector2.Zero, Color.White);
        Draw.SpriteBatch.End();
      }
    }

    private void RenderQuad(Texture texture, float ease, float rotation)
    {
      float num1 = (float) this.screenBuffer.Width / (float) this.screenBuffer.Height;
      float x = num1;
      float y = 1f;
      Vector3 vector3_1 = new Vector3(-x, y, 0.0f);
      Vector3 vector3_2 = new Vector3(x, y, 0.0f);
      Vector3 vector3_3 = new Vector3(x, -y, 0.0f);
      Vector3 vector3_4 = new Vector3(-x, -y, 0.0f);
      this.verts[0].Position = vector3_1;
      this.verts[0].TextureCoordinate = new Vector2(0.0f, 0.0f);
      this.verts[0].Color = Color.White;
      this.verts[1].Position = vector3_2;
      this.verts[1].TextureCoordinate = new Vector2(1f, 0.0f);
      this.verts[1].Color = Color.White;
      this.verts[2].Position = vector3_3;
      this.verts[2].TextureCoordinate = new Vector2(1f, 1f);
      this.verts[2].Color = Color.White;
      this.verts[3].Position = vector3_1;
      this.verts[3].TextureCoordinate = new Vector2(0.0f, 0.0f);
      this.verts[3].Color = Color.White;
      this.verts[4].Position = vector3_3;
      this.verts[4].TextureCoordinate = new Vector2(1f, 1f);
      this.verts[4].Color = Color.White;
      this.verts[5].Position = vector3_4;
      this.verts[5].TextureCoordinate = new Vector2(0.0f, 1f);
      this.verts[5].Color = Color.White;
      float num2 = (float) (4.150000095367432 + (double) Calc.YoYo(ease) * 1.7000000476837158);
      Matrix matrix = Matrix.CreateTranslation(0.0f, 0.0f, num1) * Matrix.CreateRotationY(rotation) * Matrix.CreateTranslation(0.0f, 0.0f, -num2) * Matrix.CreatePerspectiveFieldOfView(0.7853982f, num1, 1f, 10f);
      Engine.Instance.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
      Engine.Instance.GraphicsDevice.BlendState = BlendState.AlphaBlend;
      Engine.Instance.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
      Engine.Instance.GraphicsDevice.Textures[0] = texture;
      GFX.FxTexture.Parameters["World"].SetValue(matrix);
      foreach (EffectPass pass in GFX.FxTexture.CurrentTechnique.Passes)
      {
        pass.Apply();
        Engine.Instance.GraphicsDevice.DrawUserPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList, this.verts, 0, this.verts.Length / 3);
      }
    }

    public override void Update()
    {
      base.Update();
      if (this.ShowInput)
        this.waitingForInputTime += Engine.DeltaTime;
      else
        this.waitingForInputTime = 0.0f;
      if (this.loading || this.CurrPage == null || !this.pageUpdating)
        return;
      this.CurrPage.Update();
    }

    public override void Render()
    {
      if (this.loading || this.screenBuffer == null || this.screenBuffer.IsDisposed)
        return;
      float width = (float) this.ScreenWidth * Ease.CubeOut(Calc.ClampedMap(this.ease, 0.0f, 0.5f));
      float height = (float) this.ScreenHeight * Ease.CubeInOut(Calc.ClampedMap(this.ease, 0.5f, 1f, 0.2f));
      Rectangle rectangle = new Rectangle((int) ((1920.0 - (double) width) / 2.0), (int) ((1080.0 - (double) height) / 2.0), (int) width, (int) height);
      Draw.SpriteBatch.Draw((Texture2D) (RenderTarget2D) this.screenBuffer, rectangle, Color.White);
      if (this.ShowInput && (double) this.waitingForInputTime > 0.20000000298023224)
        GFX.Gui["textboxbutton"].DrawCentered(new Vector2(1856f, (float) (1016 + ((double) this.Scene.TimeActive % 1.0 < 0.25 ? 6 : 0))), Color.Black);
      if (!(this.Scene as Level).Paused)
        return;
      Draw.Rect(rectangle, Color.Black * 0.7f);
    }

    public override void Removed(Scene scene)
    {
      base.Removed(scene);
      this.Dispose();
    }

    public override void SceneEnd(Scene scene)
    {
      base.SceneEnd(scene);
      this.Dispose();
    }

    private void Dispose()
    {
      while (this.loading)
        Thread.Sleep(1);
      if (this.screenBuffer != null)
        this.screenBuffer.Dispose();
      this.screenBuffer = (VirtualRenderTarget) null;
      if (this.prevPageBuffer != null)
        this.prevPageBuffer.Dispose();
      this.prevPageBuffer = (VirtualRenderTarget) null;
      if (this.currPageBuffer != null)
        this.currPageBuffer.Dispose();
      this.currPageBuffer = (VirtualRenderTarget) null;
      this.Gfx.Dispose();
      this.Gfx = (Atlas) null;
    }

    private static uint PseudoRand(ref uint seed)
    {
      uint num1 = seed;
      uint num2 = num1 ^ num1 << 13;
      uint num3 = num2 ^ num2 >> 17;
      uint num4 = num3 ^ num3 << 5;
      seed = num4;
      return num4;
    }

    public static float PseudoRandRange(ref uint seed, float min, float max) => min + (float) ((double) (WaveDashPresentation.PseudoRand(ref seed) % 1000U) / 1000.0 * ((double) max - (double) min));
  }
}
