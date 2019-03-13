// Decompiled with JetBrains decompiler
// Type: Celeste.Overworld
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Celeste
{
  public class Overworld : Scene, IOverlayHandler
  {
    public List<Oui> UIs = new List<Oui>();
    public Oui Current = (Oui) null;
    public Oui Last = (Oui) null;
    public Oui Next = (Oui) null;
    public bool ShowInputUI = true;
    public bool ShowConfirmUI = true;
    private int lastArea = -1;
    public bool EnteringPico8;
    private float inputEase;
    public MountainRenderer Mountain;
    public HiresSnow Snow;
    private Snow3D Snow3D;
    public Maddy3D Maddy;
    private Entity routineEntity;
    private bool transitioning;

    public Overlay Overlay { get; set; }

    public Overworld(OverworldLoader loader)
    {
      this.Add((Monocle.Renderer) (this.Mountain = new MountainRenderer()));
      this.Add((Monocle.Renderer) new HudRenderer());
      this.Add(this.routineEntity = new Entity());
      this.Add((Entity) new Overworld.InputEntity(this));
      this.Snow = loader.Snow;
      if (this.Snow == null)
        this.Snow = new HiresSnow(0.45f);
      this.Add((Monocle.Renderer) this.Snow);
      this.RendererList.UpdateLists();
      this.Add((Entity) (this.Snow3D = new Snow3D(this.Mountain.Model)));
      this.Add((Entity) (this.Maddy = new Maddy3D(this.Mountain)));
      this.ReloadMenus(loader.StartMode);
      this.Mountain.OnEaseEnd = (Action) (() =>
      {
        if (this.Mountain.Area >= 0 && (!this.Maddy.Show || this.lastArea != this.Mountain.Area))
        {
          this.Maddy.Running(this.Mountain.Area < 7);
          this.Maddy.Wiggler.Start();
        }
        this.lastArea = this.Mountain.Area;
      });
      this.lastArea = this.Mountain.Area;
      if (this.Mountain.Area < 0)
        this.Maddy.Hide(true);
      else
        this.Maddy.Position = AreaData.Areas[this.Mountain.Area].MountainCursor;
      Settings.Instance.ApplyVolumes();
    }

    public override void Begin()
    {
      base.Begin();
      this.SetNormalMusic();
      FadeWipe fadeWipe = new FadeWipe((Scene) this, true, (Action) null);
      this.RendererList.UpdateLists();
      if (!this.EnteringPico8)
      {
        this.RendererList.MoveToFront((Monocle.Renderer) this.Snow);
        this.RendererList.UpdateLists();
      }
      this.EnteringPico8 = false;
      this.ReloadMountainStuff();
    }

    public override void End()
    {
      if (!this.EnteringPico8)
        this.Mountain.Dispose();
      base.End();
    }

    public void ReloadMenus(Overworld.StartMode startMode = Overworld.StartMode.Titlescreen)
    {
      foreach (Entity ui in this.UIs)
        this.Remove(ui);
      this.UIs.Clear();
      foreach (Type type in Assembly.GetEntryAssembly().GetTypes())
      {
        if (typeof (Oui).IsAssignableFrom(type) && !type.IsAbstract)
        {
          Oui instance = (Oui) Activator.CreateInstance(type);
          instance.Visible = false;
          this.Add((Entity) instance);
          this.UIs.Add(instance);
          if (instance.IsStart(this, startMode))
          {
            instance.Visible = true;
            this.Last = this.Current = instance;
          }
        }
      }
    }

    public void SetNormalMusic()
    {
      Audio.SetMusic("event:/music/menu/level_select", true, true);
      Audio.SetAmbience("event:/env/amb/worldmap", true);
    }

    public void ReloadMountainStuff()
    {
      GFX.MountainTerrain.ReassignVertices();
      GFX.MountainBuildings.ReassignVertices();
      GFX.MountainCoreWall.ReassignVertices();
      this.Mountain.Model.DisposeBillboardBuffers();
      this.Mountain.Model.ResetBillboardBuffers();
    }

    public override void HandleGraphicsReset()
    {
      this.ReloadMountainStuff();
      base.HandleGraphicsReset();
    }

    public override void Update()
    {
      if (this.Mountain.Area >= 0 && !this.Mountain.Animating)
      {
        Vector3 mountainCursor = AreaData.Areas[this.Mountain.Area].MountainCursor;
        if (mountainCursor != Vector3.Zero)
          this.Maddy.Position = mountainCursor + new Vector3(0.0f, (float) Math.Sin((double) this.TimeActive * 2.0) * 0.02f, 0.0f);
      }
      if (this.Overlay != null)
      {
        if (this.Overlay.XboxOverlay)
        {
          this.Mountain.Update((Scene) this);
          this.Snow3D.Update();
        }
        this.Overlay.Update();
        this.Entities.UpdateLists();
        if (this.Snow != null)
          this.Snow.Update((Scene) this);
      }
      else
      {
        if (!this.transitioning || !this.ShowInputUI)
          this.inputEase = Calc.Approach(this.inputEase, !this.ShowInputUI || Input.GuiInputController() ? 0.0f : 1f, Engine.DeltaTime * 4f);
        base.Update();
      }
      float num = 1f;
      bool flag1 = false;
      foreach (Monocle.Renderer renderer in this.RendererList.Renderers)
      {
        if (renderer is ScreenWipe)
        {
          flag1 = true;
          num = (renderer as ScreenWipe).Duration;
        }
      }
      bool flag2 = this.Current is OuiTitleScreen && this.Next == null || this.Next is OuiTitleScreen;
      if (this.Snow == null)
        return;
      this.Snow.ParticleAlpha = Calc.Approach(this.Snow.ParticleAlpha, flag2 | flag1 || this.Overlay != null && !this.Overlay.XboxOverlay ? 1f : 0.0f, Engine.DeltaTime / num);
    }

    public T Goto<T>() where T : Oui
    {
      T ui = this.GetUI<T>();
      if ((object) ui != null)
        this.routineEntity.Add((Component) new Coroutine(this.GotoRoutine((Oui) ui), true));
      return ui;
    }

    public bool IsCurrent<T>() where T : Oui
    {
      if (this.Current != null)
        return this.Current is T;
      return this.Last is T;
    }

    public T GetUI<T>() where T : Oui
    {
      Oui oui = (Oui) null;
      foreach (Oui ui in this.UIs)
      {
        if (ui is T)
          oui = ui;
      }
      return oui as T;
    }

    private IEnumerator GotoRoutine(Oui next)
    {
      while (this.Current == null)
        yield return (object) null;
      this.transitioning = true;
      this.Next = next;
      this.Last = this.Current;
      this.Current = (Oui) null;
      this.Last.Focused = false;
      yield return (object) this.Last.Leave(next);
      if (next.Scene != null)
      {
        yield return (object) next.Enter(this.Last);
        next.Focused = true;
        this.Current = next;
        this.transitioning = false;
      }
      this.Next = (Oui) null;
    }

    public enum StartMode
    {
      Titlescreen,
      ReturnFromOptions,
      AreaComplete,
      AreaQuit,
      ReturnFromPico8,
      MainMenu,
    }

    private class InputEntity : Entity
    {
      public Overworld Overworld;
      private Wiggler confirmWiggle;
      private Wiggler cancelWiggle;
      private float confirmWiggleDelay;
      private float cancelWiggleDelay;

      public InputEntity(Overworld overworld)
      {
        this.Overworld = overworld;
        this.Tag = (int) Tags.HUD;
        this.Depth = -100000;
        this.Add((Component) (this.confirmWiggle = Wiggler.Create(0.4f, 4f, (Action<float>) null, false, false)));
        this.Add((Component) (this.cancelWiggle = Wiggler.Create(0.4f, 4f, (Action<float>) null, false, false)));
      }

      public override void Update()
      {
        if (Input.MenuConfirm.Pressed && (double) this.confirmWiggleDelay <= 0.0)
        {
          this.confirmWiggle.Start();
          this.confirmWiggleDelay = 0.5f;
        }
        if (Input.MenuCancel.Pressed && (double) this.cancelWiggleDelay <= 0.0)
        {
          this.cancelWiggle.Start();
          this.cancelWiggleDelay = 0.5f;
        }
        this.confirmWiggleDelay -= Engine.DeltaTime;
        this.cancelWiggleDelay -= Engine.DeltaTime;
        base.Update();
      }

      public override void Render()
      {
        float inputEase = this.Overworld.inputEase;
        if ((double) inputEase <= 0.0)
          return;
        float scale = 0.5f;
        int num1 = 32;
        string label1 = Dialog.Clean("ui_cancel", (Language) null);
        string label2 = Dialog.Clean("ui_confirm", (Language) null);
        float num2 = ButtonUI.Width(label1, Input.MenuCancel);
        float num3 = ButtonUI.Width(label2, Input.MenuConfirm);
        Vector2 position = new Vector2(1880f, 1024f);
        position.X += (float) ((40.0 + ((double) num3 + (double) num2) * (double) scale + (double) num1) * (1.0 - (double) Ease.CubeOut(inputEase)));
        ButtonUI.Render(position, label1, Input.MenuCancel, scale, 1f, this.cancelWiggle.Value * 0.05f, 1f);
        if (this.Overworld.ShowConfirmUI)
        {
          position.X -= scale * num2 + (float) num1;
          ButtonUI.Render(position, label2, Input.MenuConfirm, scale, 1f, this.confirmWiggle.Value * 0.05f, 1f);
        }
      }
    }
  }
}

