// Decompiled with JetBrains decompiler
// Type: Celeste.TalkComponent
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  public class TalkComponent : Component
  {
    public static TalkComponent PlayerOver;
    public bool Enabled = true;
    public Rectangle Bounds;
    public Vector2 DrawAt;
    public Action<Player> OnTalk;
    public bool PlayerMustBeFacing = true;
    public TalkComponent.TalkComponentUI UI;
    public TalkComponent.HoverDisplay HoverUI;
    private float cooldown;
    private float hoverTimer;
    private float disableDelay;

    public TalkComponent(
      Rectangle bounds,
      Vector2 drawAt,
      Action<Player> onTalk,
      TalkComponent.HoverDisplay hoverDisplay = null)
      : base(true, true)
    {
      this.Bounds = bounds;
      this.DrawAt = drawAt;
      this.OnTalk = onTalk;
      if (hoverDisplay == null)
        this.HoverUI = new TalkComponent.HoverDisplay()
        {
          Texture = GFX.Gui["hover/highlight"],
          InputPosition = new Vector2(0.0f, -75f)
        };
      else
        this.HoverUI = hoverDisplay;
    }

    public override void Update()
    {
      if (this.UI == null)
        this.Entity.Scene.Add((Entity) (this.UI = new TalkComponent.TalkComponentUI(this)));
      Player entity = this.Scene.Tracker.GetEntity<Player>();
      bool flag = (double) this.disableDelay < 0.05000000074505806 && entity != null && entity.CollideRect(new Rectangle((int) ((double) this.Entity.X + (double) this.Bounds.X), (int) ((double) this.Entity.Y + (double) this.Bounds.Y), this.Bounds.Width, this.Bounds.Height)) && entity.OnGround() && (double) entity.Bottom < (double) this.Entity.Y + (double) this.Bounds.Bottom + 4.0 && entity.StateMachine.State == 0 && (!this.PlayerMustBeFacing || (double) Math.Abs(entity.X - this.Entity.X) <= 16.0 || entity.Facing == (Facings) Math.Sign(this.Entity.X - entity.X)) && (TalkComponent.PlayerOver == null || TalkComponent.PlayerOver == this);
      if (flag)
        this.hoverTimer += Engine.DeltaTime;
      else if (this.UI.Display)
        this.hoverTimer = 0.0f;
      if (TalkComponent.PlayerOver == this && !flag)
        TalkComponent.PlayerOver = (TalkComponent) null;
      else if (flag)
        TalkComponent.PlayerOver = this;
      if (flag && (double) this.cooldown <= 0.0 && entity != null && (int) entity.StateMachine == 0 && Input.Talk.Pressed && this.Enabled && !this.Scene.Paused)
      {
        this.cooldown = 0.1f;
        if (this.OnTalk != null)
          this.OnTalk(entity);
      }
      if (flag && (int) entity.StateMachine == 0)
        this.cooldown -= Engine.DeltaTime;
      if (!this.Enabled)
        this.disableDelay += Engine.DeltaTime;
      else
        this.disableDelay = 0.0f;
      this.UI.Highlighted = flag && (double) this.hoverTimer > 0.10000000149011612;
      base.Update();
    }

    public override void Removed(Entity entity)
    {
      this.Dispose();
      base.Removed(entity);
    }

    public override void EntityRemoved(Scene scene)
    {
      this.Dispose();
      base.EntityRemoved(scene);
    }

    public override void SceneEnd(Scene scene)
    {
      this.Dispose();
      base.SceneEnd(scene);
    }

    private void Dispose()
    {
      if (TalkComponent.PlayerOver == this)
        TalkComponent.PlayerOver = (TalkComponent) null;
      this.Scene.Remove((Entity) this.UI);
      this.UI = (TalkComponent.TalkComponentUI) null;
    }

    public override void DebugRender(Camera camera)
    {
      base.DebugRender(camera);
      Draw.HollowRect(this.Entity.X + (float) this.Bounds.X, this.Entity.Y + (float) this.Bounds.Y, (float) this.Bounds.Width, (float) this.Bounds.Height, Color.Green);
    }

    public class HoverDisplay
    {
      public MTexture Texture;
      public Vector2 InputPosition;
      public string SfxIn = "event:/ui/game/hotspot_main_in";
      public string SfxOut = "event:/ui/game/hotspot_main_out";
    }

    public class TalkComponentUI : Entity
    {
      public TalkComponent Handler;
      private bool highlighted;
      private float slide;
      private float timer;
      private Wiggler wiggler;
      private float alpha = 1f;
      private Color lineColor = new Color(1f, 1f, 1f);

      public bool Highlighted
      {
        get => this.highlighted;
        set
        {
          if (!(this.highlighted != value & this.Display))
            return;
          this.highlighted = value;
          if (this.highlighted)
            Audio.Play(this.Handler.HoverUI.SfxIn);
          else
            Audio.Play(this.Handler.HoverUI.SfxOut);
          this.wiggler.Start();
        }
      }

      public bool Display
      {
        get
        {
          if (!this.Handler.Enabled || this.Scene == null || this.Scene.Tracker.GetEntity<Textbox>() != null)
            return false;
          Player entity = this.Scene.Tracker.GetEntity<Player>();
          if (entity == null || entity.StateMachine.State == 11)
            return false;
          Level scene = this.Scene as Level;
          return !scene.FrozenOrPaused && scene.RetryPlayerCorpse == null;
        }
      }

      public TalkComponentUI(TalkComponent handler)
      {
        this.Handler = handler;
        this.AddTag((int) Tags.HUD | (int) Tags.Persistent);
        this.Add((Component) (this.wiggler = Wiggler.Create(0.25f, 4f)));
      }

      public override void Awake(Scene scene)
      {
        base.Awake(scene);
        if (this.Handler.Entity != null && !this.Scene.CollideCheck<FakeWall>(this.Handler.Entity.Position))
          return;
        this.alpha = 0.0f;
      }

      public override void Update()
      {
        this.timer += Engine.DeltaTime;
        this.slide = Calc.Approach(this.slide, this.Display ? 1f : 0.0f, Engine.DeltaTime * 4f);
        if ((double) this.alpha < 1.0 && this.Handler.Entity != null && !this.Scene.CollideCheck<FakeWall>(this.Handler.Entity.Position))
          this.alpha = Calc.Approach(this.alpha, 1f, 2f * Engine.DeltaTime);
        base.Update();
      }

      public override void Render()
      {
        Level scene = this.Scene as Level;
        if (scene.FrozenOrPaused || (double) this.slide <= 0.0 || this.Handler.Entity == null)
          return;
        Vector2 position1 = this.Handler.Entity.Position + this.Handler.DrawAt - scene.Camera.Position.Floor();
        if (SaveData.Instance != null && SaveData.Instance.Assists.MirrorMode)
          position1.X = 320f - position1.X;
        position1.X *= 6f;
        position1.Y *= 6f;
        position1.Y += (float) (Math.Sin((double) this.timer * 4.0) * 12.0 + 64.0 * (1.0 - (double) Ease.CubeOut(this.slide)));
        float scale = !this.Highlighted ? (float) (1.0 + (double) this.wiggler.Value * 0.5) : (float) (1.0 - (double) this.wiggler.Value * 0.5);
        float num = Ease.CubeInOut(this.slide) * this.alpha;
        Color color = this.lineColor * num;
        if (this.Highlighted)
          this.Handler.HoverUI.Texture.DrawJustified(position1, new Vector2(0.5f, 1f), color * this.alpha, scale);
        else
          GFX.Gui["hover/idle"].DrawJustified(position1, new Vector2(0.5f, 1f), color * this.alpha, scale);
        if (!this.Highlighted)
          return;
        Vector2 position2 = position1 + this.Handler.HoverUI.InputPosition * scale;
        if (Input.GuiInputController())
          Input.GuiButton(Input.Talk).DrawJustified(position2, new Vector2(0.5f), Color.White * num, scale);
        else
          ActiveFont.DrawOutline(Input.FirstKey(Input.Talk).ToString().ToUpper(), position2, new Vector2(0.5f), new Vector2(scale), Color.White * num, 2f, Color.Black);
      }
    }
  }
}
