// Decompiled with JetBrains decompiler
// Type: Monocle.Engine
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Reflection;
using System.Runtime;

namespace Monocle
{
  public class Engine : Game
  {
    private static int viewPadding = 0;
    public static float TimeRate = 1f;
    public static float TimeRateB = 1f;
    private static string AssemblyDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
    public string Title;
    public Version Version;
    public static Action OverloadGameLoop;
    private static bool resizing;
    public static float FreezeTimer;
    public static int FPS;
    private TimeSpan counterElapsed;
    private int fpsCounter;
    public static Color ClearColor;
    public static bool ExitOnEscapeKeypress;
    private Scene scene;
    private Scene nextScene;
    public static Matrix ScreenMatrix;

    public static Engine Instance { get; private set; }

    public static GraphicsDeviceManager Graphics { get; private set; }

    public static Commands Commands { get; private set; }

    public static Pooler Pooler { get; private set; }

    public static int Width { get; private set; }

    public static int Height { get; private set; }

    public static int ViewWidth { get; private set; }

    public static int ViewHeight { get; private set; }

    public static int ViewPadding
    {
      get
      {
        return Engine.viewPadding;
      }
      set
      {
        Engine.viewPadding = value;
        Engine.Instance.UpdateView();
      }
    }

    public static float DeltaTime { get; private set; }

    public static float RawDeltaTime { get; private set; }

    public static string ContentDirectory
    {
      get
      {
        return Path.Combine(Engine.AssemblyDirectory, Engine.Instance.get_Content().get_RootDirectory());
      }
    }

    public Engine(
      int width,
      int height,
      int windowWidth,
      int windowHeight,
      string windowTitle,
      bool fullscreen)
    {
      this.\u002Ector();
      Engine.Instance = this;
      string str;
      this.get_Window().set_Title(str = windowTitle);
      this.Title = str;
      Engine.Width = width;
      Engine.Height = height;
      Engine.ClearColor = Color.get_Black();
      this.set_InactiveSleepTime(new TimeSpan(0L));
      Engine.Graphics = new GraphicsDeviceManager((Game) this);
      Engine.Graphics.add_DeviceReset(new EventHandler<EventArgs>(this.OnGraphicsReset));
      Engine.Graphics.add_DeviceCreated(new EventHandler<EventArgs>(this.OnGraphicsCreate));
      Engine.Graphics.set_SynchronizeWithVerticalRetrace(true);
      Engine.Graphics.set_PreferMultiSampling(false);
      Engine.Graphics.set_GraphicsProfile((GraphicsProfile) 1);
      Engine.Graphics.set_PreferredBackBufferFormat((SurfaceFormat) 0);
      Engine.Graphics.set_PreferredDepthStencilFormat((DepthFormat) 3);
      Engine.Graphics.ApplyChanges();
      this.get_Window().set_AllowUserResizing(true);
      this.get_Window().add_ClientSizeChanged(new EventHandler<EventArgs>(this.OnClientSizeChanged));
      if (fullscreen)
      {
        Engine.Graphics.set_PreferredBackBufferWidth(this.get_GraphicsDevice().get_Adapter().get_CurrentDisplayMode().get_Width());
        Engine.Graphics.set_PreferredBackBufferHeight(this.get_GraphicsDevice().get_Adapter().get_CurrentDisplayMode().get_Height());
        Engine.Graphics.set_IsFullScreen(true);
      }
      else
      {
        Engine.Graphics.set_PreferredBackBufferWidth(windowWidth);
        Engine.Graphics.set_PreferredBackBufferHeight(windowHeight);
        Engine.Graphics.set_IsFullScreen(false);
      }
      this.get_Content().set_RootDirectory("Content");
      this.set_IsMouseVisible(false);
      Engine.ExitOnEscapeKeypress = true;
      GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
    }

    protected virtual void OnClientSizeChanged(object sender, EventArgs e)
    {
      if (this.get_Window().get_ClientBounds().Width <= 0 || this.get_Window().get_ClientBounds().Height <= 0 || Engine.resizing)
        return;
      Engine.resizing = true;
      Engine.Graphics.set_PreferredBackBufferWidth((int) this.get_Window().get_ClientBounds().Width);
      Engine.Graphics.set_PreferredBackBufferHeight((int) this.get_Window().get_ClientBounds().Height);
      this.UpdateView();
      Engine.resizing = false;
    }

    protected virtual void OnGraphicsReset(object sender, EventArgs e)
    {
      this.UpdateView();
      if (this.scene != null)
        this.scene.HandleGraphicsReset();
      if (this.nextScene == null || this.nextScene == this.scene)
        return;
      this.nextScene.HandleGraphicsReset();
    }

    protected virtual void OnGraphicsCreate(object sender, EventArgs e)
    {
      this.UpdateView();
      if (this.scene != null)
        this.scene.HandleGraphicsCreate();
      if (this.nextScene == null || this.nextScene == this.scene)
        return;
      this.nextScene.HandleGraphicsCreate();
    }

    protected virtual void OnActivated(object sender, EventArgs args)
    {
      base.OnActivated(sender, args);
      if (this.scene == null)
        return;
      this.scene.GainFocus();
    }

    protected virtual void OnDeactivated(object sender, EventArgs args)
    {
      base.OnDeactivated(sender, args);
      if (this.scene == null)
        return;
      this.scene.LoseFocus();
    }

    protected virtual void Initialize()
    {
      base.Initialize();
      MInput.Initialize();
      Tracker.Initialize();
      Engine.Pooler = new Pooler();
      Engine.Commands = new Commands();
    }

    protected virtual void LoadContent()
    {
      base.LoadContent();
      VirtualContent.Reload();
      Draw.Initialize(this.get_GraphicsDevice());
    }

    protected virtual void UnloadContent()
    {
      base.UnloadContent();
      VirtualContent.Unload();
    }

    protected virtual void Update(GameTime gameTime)
    {
      Engine.RawDeltaTime = (float) gameTime.get_ElapsedGameTime().TotalSeconds;
      Engine.DeltaTime = Engine.RawDeltaTime * Engine.TimeRate * Engine.TimeRateB;
      MInput.Update();
      if (Engine.ExitOnEscapeKeypress && MInput.Keyboard.Pressed((Keys) 27))
        this.Exit();
      else if (Engine.OverloadGameLoop != null)
      {
        Engine.OverloadGameLoop();
        base.Update(gameTime);
      }
      else
      {
        if ((double) Engine.FreezeTimer > 0.0)
          Engine.FreezeTimer = Math.Max(Engine.FreezeTimer - Engine.RawDeltaTime, 0.0f);
        else if (this.scene != null)
        {
          this.scene.BeforeUpdate();
          this.scene.Update();
          this.scene.AfterUpdate();
        }
        if (Engine.Commands.Open)
          Engine.Commands.UpdateOpen();
        else if (Engine.Commands.Enabled)
          Engine.Commands.UpdateClosed();
        if (this.scene != this.nextScene)
        {
          Scene scene = this.scene;
          if (this.scene != null)
            this.scene.End();
          this.scene = this.nextScene;
          this.OnSceneTransition(scene, this.nextScene);
          if (this.scene != null)
            this.scene.Begin();
        }
        base.Update(gameTime);
      }
    }

    protected virtual void Draw(GameTime gameTime)
    {
      this.RenderCore();
      base.Draw(gameTime);
      if (Engine.Commands.Open)
        Engine.Commands.Render();
      ++this.fpsCounter;
      this.counterElapsed += gameTime.get_ElapsedGameTime();
      if (!(this.counterElapsed >= TimeSpan.FromSeconds(1.0)))
        return;
      Engine.FPS = this.fpsCounter;
      this.fpsCounter = 0;
      this.counterElapsed -= TimeSpan.FromSeconds(1.0);
    }

    protected virtual void RenderCore()
    {
      if (this.scene != null)
        this.scene.BeforeRender();
      this.get_GraphicsDevice().SetRenderTarget((RenderTarget2D) null);
      this.get_GraphicsDevice().set_Viewport(Engine.Viewport);
      this.get_GraphicsDevice().Clear(Engine.ClearColor);
      if (this.scene == null)
        return;
      this.scene.Render();
      this.scene.AfterRender();
    }

    protected virtual void OnExiting(object sender, EventArgs args)
    {
      base.OnExiting(sender, args);
      MInput.Shutdown();
    }

    public void RunWithLogging()
    {
      try
      {
        this.Run();
      }
      catch (Exception ex)
      {
        ErrorLog.Write(ex);
        ErrorLog.Open();
      }
    }

    protected virtual void OnSceneTransition(Scene from, Scene to)
    {
      GC.Collect();
      GC.WaitForPendingFinalizers();
      Engine.TimeRate = 1f;
    }

    public static Scene Scene
    {
      get
      {
        return Engine.Instance.scene;
      }
      set
      {
        Engine.Instance.nextScene = value;
      }
    }

    public static Viewport Viewport { get; private set; }

    public static void SetWindowed(int width, int height)
    {
      if (width <= 0 || height <= 0)
        return;
      Engine.resizing = true;
      Engine.Graphics.set_PreferredBackBufferWidth(width);
      Engine.Graphics.set_PreferredBackBufferHeight(height);
      Engine.Graphics.set_IsFullScreen(false);
      Engine.Graphics.ApplyChanges();
      Console.WriteLine("WINDOW-" + (object) width + "x" + (object) height);
      Engine.resizing = false;
    }

    public static void SetFullscreen()
    {
      Engine.resizing = true;
      Engine.Graphics.set_PreferredBackBufferWidth(Engine.Graphics.get_GraphicsDevice().get_Adapter().get_CurrentDisplayMode().get_Width());
      Engine.Graphics.set_PreferredBackBufferHeight(Engine.Graphics.get_GraphicsDevice().get_Adapter().get_CurrentDisplayMode().get_Height());
      Engine.Graphics.set_IsFullScreen(true);
      Engine.Graphics.ApplyChanges();
      Console.WriteLine("FULLSCREEN");
      Engine.resizing = false;
    }

    private void UpdateView()
    {
      float backBufferWidth = (float) this.get_GraphicsDevice().get_PresentationParameters().get_BackBufferWidth();
      float backBufferHeight = (float) this.get_GraphicsDevice().get_PresentationParameters().get_BackBufferHeight();
      if ((double) backBufferWidth / (double) Engine.Width > (double) backBufferHeight / (double) Engine.Height)
      {
        Engine.ViewWidth = (int) ((double) backBufferHeight / (double) Engine.Height * (double) Engine.Width);
        Engine.ViewHeight = (int) backBufferHeight;
      }
      else
      {
        Engine.ViewWidth = (int) backBufferWidth;
        Engine.ViewHeight = (int) ((double) backBufferWidth / (double) Engine.Width * (double) Engine.Height);
      }
      float num = (float) Engine.ViewHeight / (float) Engine.ViewWidth;
      Engine.ViewWidth -= Engine.ViewPadding * 2;
      Engine.ViewHeight -= (int) ((double) num * (double) Engine.ViewPadding * 2.0);
      Engine.ScreenMatrix = Matrix.CreateScale((float) Engine.ViewWidth / (float) Engine.Width);
      Viewport viewport = (Viewport) null;
      ((Viewport) ref viewport).set_X((int) ((double) backBufferWidth / 2.0 - (double) (Engine.ViewWidth / 2)));
      ((Viewport) ref viewport).set_Y((int) ((double) backBufferHeight / 2.0 - (double) (Engine.ViewHeight / 2)));
      ((Viewport) ref viewport).set_Width(Engine.ViewWidth);
      ((Viewport) ref viewport).set_Height(Engine.ViewHeight);
      ((Viewport) ref viewport).set_MinDepth(0.0f);
      ((Viewport) ref viewport).set_MaxDepth(1f);
      Engine.Viewport = viewport;
    }
  }
}
