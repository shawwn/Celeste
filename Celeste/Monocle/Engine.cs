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
    private TimeSpan counterElapsed = TimeSpan.Zero;
    private int fpsCounter = 0;
    public string Title;
    public Version Version;
    public static Action OverloadGameLoop;
    private static bool resizing;
    public static float FreezeTimer;
    public static int FPS;
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
        return Path.Combine(Engine.AssemblyDirectory, Engine.Instance.Content.RootDirectory);
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
      Engine.Instance = this;
      this.Title = this.Window.Title = windowTitle;
      Engine.Width = width;
      Engine.Height = height;
      Engine.ClearColor = Color.Black;
      this.InactiveSleepTime = new TimeSpan(0L);
      Engine.Graphics = new GraphicsDeviceManager((Game) this);
      Engine.Graphics.DeviceReset += new EventHandler<EventArgs>(this.OnGraphicsReset);
      Engine.Graphics.DeviceCreated += new EventHandler<EventArgs>(this.OnGraphicsCreate);
      Engine.Graphics.SynchronizeWithVerticalRetrace = true;
      Engine.Graphics.PreferMultiSampling = false;
      Engine.Graphics.GraphicsProfile = GraphicsProfile.HiDef;
      Engine.Graphics.PreferredBackBufferFormat = SurfaceFormat.Color;
      Engine.Graphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;
      Engine.Graphics.ApplyChanges();
      this.Window.AllowUserResizing = true;
      this.Window.ClientSizeChanged += new EventHandler<EventArgs>(this.OnClientSizeChanged);
      if (fullscreen)
      {
        Engine.Graphics.PreferredBackBufferWidth = this.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
        Engine.Graphics.PreferredBackBufferHeight = this.GraphicsDevice.Adapter.CurrentDisplayMode.Height;
        Engine.Graphics.IsFullScreen = true;
      }
      else
      {
        Engine.Graphics.PreferredBackBufferWidth = windowWidth;
        Engine.Graphics.PreferredBackBufferHeight = windowHeight;
        Engine.Graphics.IsFullScreen = false;
      }
      this.Content.RootDirectory = "Content";
      this.IsMouseVisible = false;
      Engine.ExitOnEscapeKeypress = true;
      GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
    }

    protected virtual void OnClientSizeChanged(object sender, EventArgs e)
    {
      if (this.Window.ClientBounds.Width <= 0 || this.Window.ClientBounds.Height <= 0 || Engine.resizing)
        return;
      Engine.resizing = true;
      Engine.Graphics.PreferredBackBufferWidth = this.Window.ClientBounds.Width;
      Engine.Graphics.PreferredBackBufferHeight = this.Window.ClientBounds.Height;
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

    protected override void OnActivated(object sender, EventArgs args)
    {
      base.OnActivated(sender, args);
      if (this.scene == null)
        return;
      this.scene.GainFocus();
    }

    protected override void OnDeactivated(object sender, EventArgs args)
    {
      base.OnDeactivated(sender, args);
      if (this.scene == null)
        return;
      this.scene.LoseFocus();
    }

    protected override void Initialize()
    {
      base.Initialize();
      MInput.Initialize();
      Tracker.Initialize();
      Engine.Pooler = new Pooler();
      Engine.Commands = new Commands();
    }

    protected override void LoadContent()
    {
      base.LoadContent();
      VirtualContent.Reload();
      Monocle.Draw.Initialize(this.GraphicsDevice);
    }

    protected override void UnloadContent()
    {
      base.UnloadContent();
      VirtualContent.Unload();
    }

    protected override void Update(GameTime gameTime)
    {
      Engine.RawDeltaTime = (float) gameTime.ElapsedGameTime.TotalSeconds;
      Engine.DeltaTime = Engine.RawDeltaTime * Engine.TimeRate * Engine.TimeRateB;
      MInput.Update();
      if (Engine.ExitOnEscapeKeypress && MInput.Keyboard.Pressed(Keys.Escape))
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

    protected override void Draw(GameTime gameTime)
    {
      this.RenderCore();
      base.Draw(gameTime);
      if (Engine.Commands.Open)
        Engine.Commands.Render();
      ++this.fpsCounter;
      this.counterElapsed += gameTime.ElapsedGameTime;
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
      this.GraphicsDevice.SetRenderTarget((RenderTarget2D) null);
      this.GraphicsDevice.Viewport = Engine.Viewport;
      this.GraphicsDevice.Clear(Engine.ClearColor);
      if (this.scene == null)
        return;
      this.scene.Render();
      this.scene.AfterRender();
    }

    protected override void OnExiting(object sender, EventArgs args)
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
      Engine.Graphics.PreferredBackBufferWidth = width;
      Engine.Graphics.PreferredBackBufferHeight = height;
      Engine.Graphics.IsFullScreen = false;
      Engine.Graphics.ApplyChanges();
      Console.WriteLine("WINDOW-" + (object) width + "x" + (object) height);
      Engine.resizing = false;
    }

    public static void SetFullscreen()
    {
      Engine.resizing = true;
      Engine.Graphics.PreferredBackBufferWidth = Engine.Graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
      Engine.Graphics.PreferredBackBufferHeight = Engine.Graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Height;
      Engine.Graphics.IsFullScreen = true;
      Engine.Graphics.ApplyChanges();
      Console.WriteLine("FULLSCREEN");
      Engine.resizing = false;
    }

    private void UpdateView()
    {
      float backBufferWidth = (float) this.GraphicsDevice.PresentationParameters.BackBufferWidth;
      float backBufferHeight = (float) this.GraphicsDevice.PresentationParameters.BackBufferHeight;
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
      Engine.Viewport = new Viewport()
      {
        X = (int) ((double) backBufferWidth / 2.0 - (double) (Engine.ViewWidth / 2)),
        Y = (int) ((double) backBufferHeight / 2.0 - (double) (Engine.ViewHeight / 2)),
        Width = Engine.ViewWidth,
        Height = Engine.ViewHeight,
        MinDepth = 0.0f,
        MaxDepth = 1f
      };
    }
  }
}

